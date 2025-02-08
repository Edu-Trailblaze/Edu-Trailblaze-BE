using EduTrailblaze.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduTrailblaze.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseClassController : ControllerBase
    {
        private readonly ICourseClassService _courseClassService;
    }
}
