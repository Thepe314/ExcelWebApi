using ExcelUploadApi.Data;
using ExcelUploadApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ExcelUploadApi.Repository
{
    
    public class SubjectRepository :ISubjectRepository
    {
        //readonly 
        private readonly ApplicationDbContext _context;

        public SubjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all subjects
       public async Task<IEnumerable<Subject>> GetSubjectsAsync()
        {
            return await  _context.Subjects
            .FromSqlRaw("SELECT * FROM Subjects")
            .ToListAsync();

        }

        public async Task<Subject?> GetSubjectByID(int SubjectId)
        {
            return await _context.Subjects
            .FromSqlRaw("SELECT * FROM Subjects WHERE SubjectId = {0}",SubjectId)
            .FirstOrDefaultAsync();
           
        }

         public async Task<Subject?> GetSubjectByCode(string code)
        {
           return await _context.Subjects
            .FromSqlRaw("SELECT * FROM Subjects WHERE Code = {0}",code)
            .FirstOrDefaultAsync();
        }

        public async Task CreateSubject(Subject subject)
        {
            await _context.Database
            .ExecuteSqlRawAsync("INSERT INTO Subjects(Code, Name, Credit, Grade, GradePoint, Status, Semester, SemesterYear, CGPA) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
            subject.Code,
            subject.Name,
            subject.Credit,
            subject.Grade ?? "",
            subject.GradePoint ?? 0,
            subject.Status ?? "",
            subject.Semester ?? 0,
            subject.SemesterYear ?? 0,
            subject.CGPA ?? 0


            
            );
        }

        public async Task UpdateSubject(Subject subject)
        {
            
            await _context.Database
            .ExecuteSqlRawAsync("UPDATE Subjects SET Code = {0}, Name = {1}, Credit = {2}, Grade = {3}, GradePoint = {4}, Status = {5}, Semester = {6}, SemesterYear ={7}, CGPA ={8} WHERE SubjectId = {9}",
            subject.Code,
            subject.Name,
            subject.Credit,
            subject.Grade ?? "",
            subject.GradePoint ?? 0,
            subject.Status ?? "",
            subject.Semester ?? 0,
            subject.SemesterYear ?? 0,
            subject.CGPA ?? 0,
            subject.SubjectId
            
            
            );
        }
              

        public async Task DeleteSubject(int SubjectId)
        {
             await _context.Database
            .ExecuteSqlRawAsync("DELETE FROM Subjects WHERE SubjectId = {0}",SubjectId);
        }

           public async Task<IEnumerable<Subject>> SearchBySemAndYear(int Semester, int SemesterYear)
        {
            return await _context.Subjects
            .FromSqlRaw("SELECT * FROM Subjects WHERE Semester = {0} AND SemesterYear = {1}" ,Semester, SemesterYear)
            .ToListAsync();
            
        }

        public async Task<decimal>GetCGPA(int semester,int semesterYear )
        {

            //Using SQL Query
            // var results = await _context.Set<CgpaResult>()
            // .FromSqlRaw(@"
            //     SELECT 
            //         CASE 
            //             WHEN SUM(Credit) = 0 THEN 0
            //             ELSE CAST(SUM(GradePoint * Credit) AS DECIMAL(5,2)) / SUM(Credit)
            //         END AS Cgpa
            //     FROM Subjects
            //     WHERE GradePoint IS NOT NULL 
            //     AND Credit > 0 
            //     AND Semester = {0} 
            //     AND SemesterYear = {1}", Semester, SemesterYear)
            // .AsNoTracking()
            // .FirstOrDefaultAsync();

            // return results?.Cgpa ?? 0;

        //Using EF.

        var subject = await _context.Subjects
        .Where( s=> s.GradePoint.HasValue 
        && s.Credit > 0
        && s.Semester == semester
        && s.SemesterYear == semesterYear)
        .ToListAsync();

        if(!subject.Any())
        return 0;

        var totalPoints= subject.Sum(s=> s.GradePoint!.Value *  s.Credit);

        var totalCredits = subject.Sum(s => s.Credit);

        return Math.Round(totalPoints/totalCredits,2);

        }   


        public async Task<IEnumerable<Subject>> SearchGradeBySemAndYear(int semester, int semesterYear, string grade)
        {
            return await _context.Subjects
            .FromSqlRaw("Select * FROM Subjects WHERE Semester = {0} AND SemesterYear = {1} AND Grade = {2}",semester,semesterYear,grade)
            .ToListAsync();
        }
    }
}