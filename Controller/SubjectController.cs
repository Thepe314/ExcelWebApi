using System.Formats.Asn1;
using System.Globalization;
using System.Security.Permissions;
using CsvHelper;
using CsvHelper.Configuration;
using ExcelUploadApi.Data;
using ExcelUploadApi.Models;
using ExcelUploadApi.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExcelUploadApi.Controller
{
    
    [ApiController]
    [Route("/api/[Controller]")]
    public class SubjectController : ControllerBase
    {
        //readonly
        private readonly ISubjectRepository _sRepo;

        private readonly ApplicationDbContext _context;
        public SubjectController(ISubjectRepository sRepo, ApplicationDbContext context)
        {
            _sRepo = sRepo;

            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult>GetAllSubjects()
        {
            var subjects = await _sRepo.GetSubjectsAsync();
            return Ok(subjects);
        }   

        [HttpGet("{SubjectId:int}")]
        public async Task<IActionResult>GetById(int SubjectId)
        {
            var subjects = await _sRepo.GetSubjectByID(SubjectId);
            return Ok(subjects);
        }   

        [HttpPost]
        public async Task<IActionResult>CreateSubject(SubjectCreateDto dto)
        {
            var subject = new Subject
            {
                Code = dto.Code,
                Name = dto.Name,
                Credit = dto.Credit,
                Grade = dto.Grade,
                GradePoint = dto. GradePoint,
                Status = dto.Status,
                Semester = dto.Semester,
                SemesterYear = dto.SemesterYear

            };

            await _sRepo.CreateSubject(subject);
          
            return Ok(subject);
        }   

        [HttpPut]
        public async Task<IActionResult>UpdateSubject(int SubjectId, SubjectCreateDto dto)
        {
            var existing = await _sRepo.GetSubjectByID(SubjectId);
            if(existing == null)
            {
                return NotFound("Suject doesnt exist");
            }

                existing.Code = dto.Code;
                existing.Name = dto.Name;
                existing.Credit = dto.Credit; 
                existing.Grade = dto.Grade;
                existing.GradePoint = dto. GradePoint;
                existing.Status = dto.Status;
                existing.Semester = dto.Semester;
                existing.SemesterYear = dto.SemesterYear;

            await _sRepo.UpdateSubject(existing);
            return Ok(existing);
        }
    

    [HttpDelete("{SubjectId:int}")]
        public async Task<IActionResult>DeleteSubject(int SubjectId)
        {
             await _sRepo.DeleteSubject(SubjectId);
            return Ok(new {Message = "Subject Deleted Successfully"});
        }   
        
        [HttpGet("SemWithYear")]
        public async Task<IActionResult>SearchBySemesterAndYear(int Semester, int SemesterYear)
        {
            var subject = await _sRepo.SearchBySemAndYear(Semester, SemesterYear);

            if(subject == null)
            {
                return NotFound("No subject found");
            }

           
            return Ok(subject);
        }   

        //Cgpa
        [HttpGet("CGPA")]
        public async Task<IActionResult> GetCgpa([FromQuery] int semester, [FromQuery] int semesterYear)
        {
           var cgpa = await _sRepo.GetCGPA(semester, semesterYear);
            return Ok(
                    new
                    {
                        Semester = semester, 
                        Year = semesterYear,
                        CGPA = cgpa
                    }
            );
        }

        [HttpGet("FilterBy-Grades")]
        public async Task<IActionResult>SearchGradeBySemAndYear(int semester, int semesterYear, string grade)
        {
            var subject = await _sRepo.SearchGradeBySemAndYear(semester, semesterYear,grade);

            if(!subject.Any())
            {
                return NotFound("No Grades found");
            }

           
            return Ok(subject);
        }   

    
    // ExportSubjectsCsv:
    // 1. Fetch all subjects from the repository.
    // 2. Add CSV header row with column names.
    // 3. Loop through subjects and append each as a CSV row.
    // 4. Return the CSV as a downloadable file named "Subjects.csv".
    [HttpGet("export-csv")]
    public async Task<IActionResult> ExportSubjectsCsv()
    {
        var subjects = await _sRepo.GetSubjectsAsync();

         // 1️⃣ Add header row
        var csv = "SubjectId,Code,Name,Credit,Grade,GradePoint,Status,Semester,Semester Year\n";

        // 2️⃣ Add data rows
        foreach (var s in subjects)
        {
            csv += $"{s.SubjectId},{s.Code},{s.Name},{s.Credit},{s.Grade},{s.GradePoint},{s.Status},{s.Semester},{s.SemesterYear}\n";
        }

        // 3️⃣ Return as a downloadable CSV file
        return File(
            System.Text.Encoding.UTF8.GetBytes(csv),
            "text/csv",
            "Subjects.csv"
        );
    }

    // ImportSubjectsCsv:
    /// Imports subjects from a CSV file into the database.
    /// - Validates the uploaded file and size.
    /// - Reads CSV rows and maps them to Subject objects.
    /// - Checks if each subject already exists by Code:
    ///     • If it exists → updates the existing record.
    ///     • If it doesn’t exist → inserts a new record (SubjectId auto-generated).
    /// Saves all changes to the database in a single batch.
    /// Returns a summary of total, inserted, and updated subjects.
    /// Import CSV efficiently: use a dictionary for fast lookup of existing subjects 
    /// to update or insert, and use CsvHelper mapping to ignore SubjectId so the DB auto-generates it.
  
   [HttpPost("import-csv")]
        public async Task<IActionResult> ImportSubjectsCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if(file.Length > 5*1024*1024) // 5 MB limit
                return BadRequest("File is too large. Max 5 MB allowed.");

            // Open a stream to read the uploaded CSV file so we can process its contents.
            using var reader = new StreamReader(file.OpenReadStream());

            // Configure CsvHelper to read the CSV file:
            // - HeaderValidated = null → don’t throw error if a header is missing
            // - MissingFieldFound = null → don’t throw error if a field is missing in a row
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,  
                MissingFieldFound = null 
            };

            //Csv Reader to read the uploaded file
            using var csv = new CsvReader(reader, csvConfig);

            // Register mapping to ignore SubjectId
            csv.Context.RegisterClassMap<SubjectMap>();

            //Throws a BadRequest if the Uploaded CSV file is empty
            var records = csv.GetRecords<Subject>().ToList();
            if (!records.Any())
                return BadRequest("CSV file is empty.");

            // Load existing subjects from DB once
            // This allows us to check quickly whether a subject already exists
            var existingSubjects = await _context.Subjects.ToListAsync();

            // Convert list to dictionary for fast lookup by 'Code'
            var existingDict = existingSubjects.ToDictionary(s => s.Code);

            int inserted = 0; // Counter for new subjects added
            int updated = 0; // Counter for existing subjects updated

            // Loop through each subject from the uploaded CSV
            foreach (var s in records)
            {
                 // Check if this subject already exists in DB (using Code as key)
                if (existingDict.TryGetValue(s.Code, out var existing))
                {
                    // Update the existing subject with new values from CSV
                    existing.Name = s.Name;
                    existing.Credit = s.Credit;
                    existing.Grade = s.Grade;
                    existing.GradePoint = s.GradePoint;
                    existing.Status = s.Status;
                    existing.Semester = s.Semester;
                    existing.SemesterYear = s.SemesterYear;
                    existing.CGPA = s.CGPA;
                    updated++;
                }
                else
                {
                    // If subject does not exist, add as a new record
                    // SubjectId will be auto-generated by the database
                    _context.Subjects.Add(s);
                    inserted++;
                }
            }

            // Save all changes (inserts + updates) to the database at once
            await _context.SaveChangesAsync();

            // Return summary of import results
            return Ok(new
            {
                Message = "Import completed successfully.",
                Total = records.Count, // Total rows read from CSV
                Inserted = inserted,   // Number of new subjects added
                Updated = updated      // Number of existing subjects updated
            });
        }

        // CsvHelper mapping to specify which CSV columns map to Subject properties
        // Ignores SubjectId so database can auto-generate it
        public class SubjectMap : ClassMap<Subject>
        {
            public SubjectMap()
            {
                Map(m => m.Code);
                Map(m => m.Name);
                Map(m => m.Credit);
                Map(m => m.Grade);
                Map(m => m.GradePoint);
                Map(m => m.Status);
                Map(m => m.Semester);
                Map(m => m.SemesterYear);
                Map(m => m.CGPA);
                
            }
        }
    }
    }