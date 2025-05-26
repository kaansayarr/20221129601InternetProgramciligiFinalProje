using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Uyg.API.DTOs;
using Uyg.API.Models;
using Uyg.API.Repositories;

namespace Uyg.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly LessonsRepository _LessonsRepository;
        private readonly IMapper _mapper;
        ResultDto _result = new ResultDto();
        public CategoryController(IMapper mapper, CategoryRepository categoryRepository, LessonsRepository LessonsRepository)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _LessonsRepository = LessonsRepository;
        }

        [HttpGet]
        public async Task<List<CategoryDto>> List()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var CategoryDtos = _mapper.Map<List<CategoryDto>>(categories);
            return CategoryDtos;
        }

        [HttpGet("{id}")]
        public async Task<CategoryDto> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            var CategoryDto = _mapper.Map<CategoryDto>(category);
            return CategoryDto;
        }



        [HttpGet("{id}/Egitimler")]
        public async Task<List<LessonsDto>> LessonsList(int id)
        {
            var Lessons = await _LessonsRepository.Where(s=>s.CategoryId==id).Include(i=>i.Category).ToListAsync();
            var LessonsDtos = _mapper.Map<List<LessonsDto>>(Lessons);
            return LessonsDtos;
        }
        [HttpPost]
        public async Task<ResultDto> Add(CategoryDto model)
        {
            var list = _categoryRepository.Where(s => s.Name == model.Name).ToList();
            if (list.Count() > 0)
            {
                _result.Status = false;
                _result.Message = "Girilen Kategori Adı Kayıtlıdır!";
                return _result;
            }
            var category = _mapper.Map<Category>(model);
            category.Created = DateTime.Now;
            category.Updated = DateTime.Now;
            await _categoryRepository.AddAsync(category);
            _result.Status = true;
            _result.Message = "Kayıt Eklendi";
            return _result;
        }
        [HttpPut]
        public async Task<ResultDto> Update(Category model)
        {
            var category =await _categoryRepository.GetByIdAsync(model.Id);
            category.Name = model.Name;
            category.IsActive = model.IsActive;
            category.Updated = DateTime.Now;
            await _categoryRepository.UpdateAsync(category);
            _result.Status = true;
            _result.Message = "Kayıt GüncelLendi";
            return _result;
        }

        [HttpDelete("{id}")]
        public async Task<ResultDto> Delete(int id)
        {
            var list =await _LessonsRepository.Where(s => s.CategoryId == id).ToListAsync();
            if (list.Count() > 0)
            {
                _result.Status = false;
                _result.Message = "Üzerine Ürün Kaydı Olan Kategori Silinemez!";
                return _result;
            }

            await _categoryRepository.DeleteAsync(id);
            _result.Status = true;
            _result.Message = "Kayıt Silindi";
            return _result;

        }
    }
}
