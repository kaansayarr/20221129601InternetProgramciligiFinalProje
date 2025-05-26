using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Uyg.API.DTOs;
using Uyg.API.Models;
using Uyg.API.Repositories;
using System.Security.Claims;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Uyg.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonsController : ControllerBase
    {
        private readonly LessonsRepository _LessonsRepository;
        private readonly CourseRepository _courseRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        ResultDto _result = new ResultDto();
        public LessonsController(
            LessonsRepository LessonsRepository,
            CourseRepository courseRepository,
            CategoryRepository categoryRepository,
            IMapper mapper)

        {
            _LessonsRepository = LessonsRepository;
            _courseRepository = courseRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<LessonsDto>> GetById(int id)
        {
            try
            {
                var lesson = await _LessonsRepository.Where(l => l.Id == id)
                    .Include(l => l.Category)
                    .Include(l => l.Course)
                    .FirstOrDefaultAsync();

                if (lesson == null)
                    return NotFound(new { status = false, message = "Ders bulunamadı" });

                var lessonDto = _mapper.Map<LessonsDto>(lesson);
                return Ok(lessonDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = false, message = "Ders alınırken hata oluştu: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<LessonsDto>>> List()
        {
            try
            {
                var lessons = await _LessonsRepository.Where(l => true)
                    .Include(l => l.Category)
                    .Include(l => l.Course)
                    .OrderBy(l => l.CategoryId)
                    .ToListAsync();

                var lessonDtos = _mapper.Map<List<LessonsDto>>(lessons);
                return Ok(lessonDtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = false, message = "Dersler alınırken hata oluştu: " + ex.Message });
            }
        }




        [HttpPost]
        public async Task<ResultDto> Add([FromBody] LessonsDto model)
        {
            // Validate Course exists
            var course = await _courseRepository.GetByIdAsync(model.CourseId);
            if (course == null)
            {
                _result.Status = false;
                _result.Message = "Course not found";
                return _result;
            }

            // Validate Category exists
            var category = await _categoryRepository.GetByIdAsync(model.CategoryId);
            if (category == null)
            {
                _result.Status = false;
                _result.Message = "Category not found";
                return _result;
            }

            var Lessons = _mapper.Map<Lessons>(model);
            Lessons.Created = DateTime.Now;
            Lessons.Updated = DateTime.Now;
            Lessons.Course = course;
            Lessons.Category = category;
            await _LessonsRepository.AddAsync(Lessons);
            _result.Status = true;
            _result.Message = "Kayıt Eklendi";
            return _result;
        }

        [HttpPut("{id}")]
        public async Task<ResultDto> Update(int id, [FromBody] LessonsDto model)
        {
            try
            {
                // ID kontrolü
                if (id != model.Id)
                {
                    _result.Status = false;
                    _result.Message = "ID uyuşmazlığı";
                    return _result;
                }

                // Dersin var olup olmadığını kontrol et
                var existingLesson = await _LessonsRepository.Where(l => l.Id == id)
                    .Include(l => l.Course)
                    .Include(l => l.Category)
                    .FirstOrDefaultAsync();

                if (existingLesson == null)
                {
                    _result.Status = false;
                    _result.Message = "Ders bulunamadı";
                    return _result;
                }

                // Kurs kontrolü
                var course = await _courseRepository.GetByIdAsync(model.CourseId);
                if (course == null)
                {
                    _result.Status = false;
                    _result.Message = "Kurs bulunamadı";
                    return _result;
                }

                // Kategori kontrolü
                var category = await _categoryRepository.GetByIdAsync(model.CategoryId);
                if (category == null)
                {
                    _result.Status = false;
                    _result.Message = "Kategori bulunamadı";
                    return _result;
                }

                // Mevcut dersi güncelle
                existingLesson.Name = model.Name;
                existingLesson.Description = model.Description;
                existingLesson.VideoUrl = model.VideoUrl;
                existingLesson.PhotoUrl = model.PhotoUrl;
                existingLesson.IsActive = model.IsActive;
                existingLesson.Updated = DateTime.Now;
                existingLesson.CourseId = model.CourseId;
                existingLesson.CategoryId = model.CategoryId;

                await _LessonsRepository.UpdateAsync(existingLesson);

                _result.Status = true;
                _result.Message = "Ders başarıyla güncellendi";
                return _result;
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "Güncelleme sırasında bir hata oluştu: " + ex.Message;
                return _result;
            }
        }

        [HttpDelete("{id}")]
        public async Task<ResultDto> Delete(int id)
        {
            try
            {
                // Dersin var olup olmadığını kontrol et
                var lesson = await _LessonsRepository.Where(l => l.Id == id)
                    .Include(l => l.Course)
                    .FirstOrDefaultAsync();

                if (lesson == null)
                {
                    _result.Status = false;
                    _result.Message = "Ders bulunamadı";
                    return _result;
                }

                // Dersi sil
                await _LessonsRepository.DeleteAsync(id);

                _result.Status = true;
                _result.Message = "Ders başarıyla silindi";
                return _result;
            }
            catch (Exception ex)
            {
                _result.Status = false;
                _result.Message = "Silme sırasında bir hata oluştu: " + ex.Message;
                return _result;
            }
        }

        // List ve GetById metodlarını da güncelleyelim
  

     
    }
    }
