using ExcelUploadApi.Models;

namespace ExcelUploadApi.Repository
{
    
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetSubjectsAsync();

        Task<Subject?> GetSubjectByID(int SubjectId);

        Task CreateSubject(Subject subject);

        Task UpdateSubject(Subject subject);

        Task DeleteSubject(int SubjectId);

        Task<Subject?> GetSubjectByCode(string code);

        Task<IEnumerable<Subject>> SearchBySemAndYear(int Semester, int SemesterYear);
        

        Task<decimal> GetCGPA(int Semester,int SemesterYear);

        Task<IEnumerable<Subject>> SearchGradeBySemAndYear(int Semester, int SemesterYear, string Grade);



    }
}