using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uyg.API.DTOs;
using Uyg.API.Services;

namespace Uyg.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : ControllerBase
    {
        private readonly EnrollmentService _enrollmentService;

        public EnrollmentController(EnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<List<EnrollmentDto>>> GetAllEnrollments()
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            return Ok(enrollments);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<EnrollmentDto>> GetEnrollmentById(int id)
        {
            var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
            if (enrollment == null)
                return NotFound();

            return Ok(enrollment);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EnrollmentDto>> CreateEnrollment(EnrollmentCreateDto createDto)
        {
            try
            {
                var enrollment = await _enrollmentService.CreateEnrollmentAsync(createDto);
                return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.Id }, enrollment);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateEnrollment(int id)
        {
            var result = await _enrollmentService.DeactivateEnrollmentAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
} 