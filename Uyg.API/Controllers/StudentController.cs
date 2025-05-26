using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Uyg.API.DTOs;
using Uyg.API.Models;
using Uyg.API.Repositories;

namespace Uyg.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly StudentRepository _studentRepository;
        private readonly IMapper _mapper;
        ResultDto _result = new ResultDto();

        public StudentController(StudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<List<StudentDto>> List()
        {
            var students = await _studentRepository.GetAllAsync();
            var studentDtos = _mapper.Map<List<StudentDto>>(students);
            return studentDtos;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<StudentDto> GetById(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            var studentDto = _mapper.Map<StudentDto>(student);
            return studentDto;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ResultDto> Add([FromBody] StudentCreateDto model)
        {
            // Check if student with same StudentId already exists
            var existingStudent = await _studentRepository.Where(s => s.StudentId == model.StudentId).FirstOrDefaultAsync();
            if (existingStudent != null)
            {
                _result.Status = false;
                _result.Message = "Bu öğrenci numarası ile kayıtlı bir öğrenci zaten var!";
                return _result;
            }

            var student = _mapper.Map<Student>(model);
            student.Created = DateTime.Now;
            student.Updated = DateTime.Now;
            student.IsActive = true;

            await _studentRepository.AddAsync(student);
            _result.Status = true;
            _result.Message = "Öğrenci başarıyla kaydedildi.";
            return _result;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ResultDto> Update([FromBody] StudentDto model)
        {
            var student = await _studentRepository.GetByIdAsync(model.Id);
            if (student == null)
            {
                _result.Status = false;
                _result.Message = "Öğrenci bulunamadı!";
                return _result;
            }

            // Check if another student has the same StudentId
            var existingStudent = await _studentRepository.Where(s => s.StudentId == model.StudentId && s.Id != model.Id).FirstOrDefaultAsync();
            if (existingStudent != null)
            {
                _result.Status = false;
                _result.Message = "Bu öğrenci numarası başka bir öğrenci tarafından kullanılıyor!";
                return _result;
            }

            student.Name = model.Name;
            student.Surname = model.Surname;
            student.StudentId = model.StudentId;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;
            student.IsActive = model.IsActive;
            student.Updated = DateTime.Now;

            await _studentRepository.UpdateAsync(student);
            _result.Status = true;
            _result.Message = "Öğrenci bilgileri güncellendi.";
            return _result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ResultDto> Delete(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
            {
                _result.Status = false;
                _result.Message = "Öğrenci bulunamadı!";
                return _result;
            }

            await _studentRepository.DeleteAsync(id);
            _result.Status = true;
            _result.Message = "Öğrenci silindi.";
            return _result;
        }
    }
} 