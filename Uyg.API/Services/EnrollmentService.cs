using Microsoft.EntityFrameworkCore;
using Uyg.API.DTOs;
using Uyg.API.Models;

namespace Uyg.API.Services
{
    public class EnrollmentService
    {
        private readonly AppDbContext _context;

        public EnrollmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EnrollmentDto>> GetAllEnrollmentsAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Lessons)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentDbId = e.StudentDbId,
                    StudentName = e.Student == null ? string.Empty : e.Student.Name,
                    StudentSurname = e.Student == null ? string.Empty : e.Student.Surname,
                    StudentId = e.Student == null ? string.Empty : e.Student.StudentId,
                    LessonsId = e.LessonsId,
                    LessonsName = e.Lessons == null ? string.Empty : e.Lessons.Name,
                    EnrollmentDate = e.EnrollmentDate,
                    IsActive = e.IsActive
                })
                .ToListAsync();
        }

        public async Task<EnrollmentDto> GetEnrollmentByIdAsync(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Lessons)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
                return null;

            return new EnrollmentDto
            {
                Id = enrollment.Id,
                StudentDbId = enrollment.StudentDbId,
                StudentName = enrollment.Student == null ? string.Empty : enrollment.Student.Name,
                StudentSurname = enrollment.Student == null ? string.Empty : enrollment.Student.Surname,
                StudentId = enrollment.Student == null ? string.Empty : enrollment.Student.StudentId,
                LessonsId = enrollment.LessonsId,
                LessonsName = enrollment.Lessons == null ? string.Empty : enrollment.Lessons.Name,
                EnrollmentDate = enrollment.EnrollmentDate,
                IsActive = enrollment.IsActive
            };
        }

        public async Task<EnrollmentDto> CreateEnrollmentAsync(EnrollmentCreateDto createDto)
        {
            var student = await _context.Students.FindAsync(createDto.StudentId);
            if (student == null)
                throw new ArgumentException($"ID'si '{createDto.StudentId}' olan öğrenci bulunamadı.");

            var lessons = await _context.Lessons.FindAsync(createDto.LessonsId);
            if (lessons == null)
                throw new ArgumentException($"ID'si '{createDto.LessonsId}' olan ders bulunamadı.");

            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentDbId == createDto.StudentId && 
                                        e.LessonsId == createDto.LessonsId && 
                                        e.IsActive);

            if (existingEnrollment != null)
                throw new InvalidOperationException($"'{student.Name} {student.Surname}' adlı öğrenci bu derse zaten kayıtlı.");

            var enrollment = new Enrollment
            {
                StudentDbId = createDto.StudentId,
                LessonsId = createDto.LessonsId,
                EnrollmentDate = DateTime.UtcNow,
                IsActive = true
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return await GetEnrollmentByIdAsync(enrollment.Id);
        }

        public async Task<bool> DeactivateEnrollmentAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return false;

            enrollment.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 