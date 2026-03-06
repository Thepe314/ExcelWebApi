using ExcelUploadApi.Data;
using ExcelUploadApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ExcelUploadApi.Repository
{
    public class StudentRepository :IStudentRepository
    {

        //readonly
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

         //Login by email
        public async Task<Student?> GetByEmail(string email)
        {
            return await _context.Students
            .FirstOrDefaultAsync(u=> u.Email == email);
        }

        //Register
        public async Task RegisterUser(Student student)
        {
             _context.Students.Add(student);
             await _context.SaveChangesAsync();

        }


    }

    





}