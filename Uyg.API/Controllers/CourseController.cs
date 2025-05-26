using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uyg.API.DTOs;
using Uyg.API.Models;
using Uyg.API.Repositories;
using System.Security.Claims;

namespace Uyg.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly CourseRepository _courseRepository;
        private readonly LessonsRepository _lessonsRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        ResultDto _result = new ResultDto();

        public CourseController(
            CourseRepository courseRepository, 
            LessonsRepository lessonsRepository,
            CategoryRepository categoryRepository,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
            _lessonsRepository = lessonsRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Moderator,Editor")]
        public async Task<List<CourseDto>> List()
        {
            var courses = await _courseRepository.GetAllAsync();
            var courseDtos = _mapper.Map<List<CourseDto>>(courses);
            return courseDtos;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Moderator,Editor")]
        public async Task<CourseDto> GetById(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            var courseDto = _mapper.Map<CourseDto>(course);
            return courseDto;
        }

        [HttpGet("{id}/Lessons")]
        public async Task<List<CourseLessonDto>> GetLessons(int id)
        {
            var course = await _courseRepository.Where(c => c.Id == id)
                .Include(c => c.Lessons)
                .ThenInclude(l => l.Category)
                .FirstOrDefaultAsync();
            
            if (course == null)
                return new List<CourseLessonDto>();

            var lessonsDtos = _mapper.Map<List<CourseLessonDto>>(course.Lessons);
            return lessonsDtos;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ResultDto> Add([FromBody] CourseDto model)
        {
            if (model.Lessons == null || !model.Lessons.Any())
            {
                _result.Status = false;
                _result.Message = "At least one lesson is required to create a course";
                return _result;
            }

            var course = _mapper.Map<Course>(model);
            course.Created = DateTime.Now;
            course.Updated = DateTime.Now;

            // Add the course first
            await _courseRepository.AddAsync(course);

            // Add lessons and associate them with the course
            foreach (var lessonDto in model.Lessons)
            {
                // Verify category exists
                var category = await _categoryRepository.GetByIdAsync(lessonDto.CategoryId);
                if (category == null)
                {
                    _result.Status = false;
                    _result.Message = $"Category with ID {lessonDto.CategoryId} not found";
                    return _result;
                }

                var lesson = new Lessons
                {
                    Name = lessonDto.Name,
                    Description = lessonDto.Description,
                    VideoUrl = lessonDto.VideoUrl,
                    PhotoUrl = lessonDto.PhotoUrl,
                    Price = lessonDto.Price,
                    CategoryId = lessonDto.CategoryId,
                    CourseId = course.Id,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Category = category
                };
                await _lessonsRepository.AddAsync(lesson);
            }

            _result.Status = true;
            _result.Message = "Course and lessons added successfully";
            return _result;
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ResultDto> Update([FromBody] CourseDto model)
        {
            if (model.Lessons == null || !model.Lessons.Any())
            {
                _result.Status = false;
                _result.Message = "At least one lesson is required to update a course";
                return _result;
            }

            var course = await _courseRepository.GetByIdAsync(model.Id);
            if (course == null)
            {
                _result.Status = false;
                _result.Message = "Course not found";
                return _result;
            }

            course.Name = model.Name;
            course.Description = model.Description;
            course.PhotoUrl = model.PhotoUrl;
            course.Price = model.Price;
            course.IsActive = model.IsActive;
            course.Updated = DateTime.Now;

            // Update the course
            await _courseRepository.UpdateAsync(course);

            // Update or add lessons
            var existingLessons = await _lessonsRepository.Where(l => l.CourseId == course.Id)
                .Include(l => l.Category)
                .ToListAsync();

            // Update existing lessons or add new ones
            foreach (var lessonDto in model.Lessons)
            {
                // Verify category exists
                var category = await _categoryRepository.GetByIdAsync(lessonDto.CategoryId);
                if (category == null)
                {
                    _result.Status = false;
                    _result.Message = $"Category with ID {lessonDto.CategoryId} not found";
                    return _result;
                }

                var existingLesson = existingLessons.FirstOrDefault(l => l.Id == lessonDto.Id);
                if (existingLesson != null)
                {
                    existingLesson.Name = lessonDto.Name;
                    existingLesson.Description = lessonDto.Description;
                    existingLesson.VideoUrl = lessonDto.VideoUrl;
                    existingLesson.PhotoUrl = lessonDto.PhotoUrl;
                    existingLesson.Price = lessonDto.Price;
                    existingLesson.CategoryId = lessonDto.CategoryId;
                    existingLesson.Updated = DateTime.Now;
                    existingLesson.Category = category;
                    await _lessonsRepository.UpdateAsync(existingLesson);
                }
                else
                {
                    var newLesson = new Lessons
                    {
                        Name = lessonDto.Name,
                        Description = lessonDto.Description,
                        VideoUrl = lessonDto.VideoUrl,
                        PhotoUrl = lessonDto.PhotoUrl,
                        Price = lessonDto.Price,
                        CategoryId = lessonDto.CategoryId,
                        CourseId = course.Id,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        Category = category
                    };
                    await _lessonsRepository.AddAsync(newLesson);
                }
            }

            // Remove lessons that are no longer in the course
            var lessonIdsToKeep = model.Lessons.Select(l => l.Id).ToList();
            var lessonsToRemove = existingLessons.Where(l => !lessonIdsToKeep.Contains(l.Id));
            foreach (var lesson in lessonsToRemove)
            {
                await _lessonsRepository.DeleteAsync(lesson.Id);
            }

            _result.Status = true;
            _result.Message = "Course and lessons updated successfully";
            return _result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ResultDto> Delete(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
            {
                _result.Status = false;
                _result.Message = "Course not found";
                return _result;
            }

            // Delete all lessons associated with the course
            var lessons = await _lessonsRepository.Where(l => l.CourseId == id).ToListAsync();
            foreach (var lesson in lessons)
            {
                await _lessonsRepository.DeleteAsync(lesson.Id);
            }

            // Delete the course
            await _courseRepository.DeleteAsync(id);
            _result.Status = true;
            _result.Message = "Course and associated lessons deleted successfully";
            return _result;
        }
    }
} 