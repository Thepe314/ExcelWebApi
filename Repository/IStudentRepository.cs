using ExcelUploadApi.Models;

namespace ExcelUploadApi.Repository
{
    public interface IStudentRepository
    {

        //Login
        Task<Student?> GetByEmail(string email);

        //Register
        Task RegisterUser(Student student);


    }
}