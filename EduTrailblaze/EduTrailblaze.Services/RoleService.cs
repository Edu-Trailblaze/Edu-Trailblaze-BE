using EduTrailblaze.Entities;
using EduTrailblaze.Services.Interfaces;
using EduTrailblaze.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace EduTrailblaze.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AsyncPolicyWrap _dbPolicyWrap;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        public RoleService(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbRetryPolicy = Policy.Handle<SqlException>()
                                      .WaitAndRetryAsync(1, retryAttempt => TimeSpan.FromSeconds(Math.Pow(1, retryAttempt)),
                                      (exception, timeSpan, retryCount, context) =>
                                      {
                                          Console.WriteLine($"[Db Retry] Attempt {retryCount} after {timeSpan} due to: {exception.Message}");
                                      });
            _dbTimeoutPolicy = Policy.TimeoutAsync(10, TimeoutStrategy.Optimistic, (context, timeSpan, task) =>
            {
                Console.WriteLine($"[Db Timeout] Operation timed out after {timeSpan}");
                return Task.CompletedTask;
            });
            _dbPolicyWrap = Policy.WrapAsync(_dbRetryPolicy, _dbTimeoutPolicy);
        }

        public async Task<ApiResponse> ChangeRoleToInstructor(ChangeInstructorRoleModel model)
        {
            var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByIdAsync(model.UserId));
            if (user == null)
            {
                return new ApiResponse { StatusCode = StatusCodes.Status404NotFound, Message = "User not found." };
            }

            var isUserInRole = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.IsInRoleAsync(user, "Instructor"));
            if (isUserInRole)
            {
                return new ApiResponse { StatusCode = StatusCodes.Status400BadRequest, Message = "User already in role." };
            }
            await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.AddToRoleAsync(user, "Instructor"));
            return new ApiResponse { StatusCode = StatusCodes.Status200OK, Message = "Role assigned successfully." };
        }

        public async Task<ApiResponse> AssignRole(AssignRoleModel model)
        {
            var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByIdAsync(model.UserId));
            if (user == null)
            {
                return new ApiResponse { StatusCode = StatusCodes.Status404NotFound, Message = "User not found." };
            }
            var role = await _dbPolicyWrap.ExecuteAsync(async () => await _roleManager.FindByNameAsync(model.RoleName));
            if (role == null)
            {
                return new ApiResponse { StatusCode = StatusCodes.Status404NotFound, Message = "Role not found." };
            }
            var isUserInRole = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.IsInRoleAsync(user, role.Name));
            if (isUserInRole)
            {
                return new ApiResponse { StatusCode = StatusCodes.Status400BadRequest, Message = "User already in role." };
            }
            await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.AddToRoleAsync(user, role.Name));
            return new ApiResponse { StatusCode = StatusCodes.Status200OK, Message = "Role assigned successfully." };
        }
    }
}
