using EduTrailblaze.Services.Models;

namespace EduTrailblaze.Services.Interfaces
{
    public interface IRoleService
    {
        Task<ApiResponse> ChangeRoleToInstructor(ChangeInstructorRoleModel model);
        Task<ApiResponse> AssignRole(AssignRoleModel model);
    }
}
