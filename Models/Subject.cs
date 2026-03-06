using System.ComponentModel.DataAnnotations;


namespace ExcelUploadApi.Models
{
      public class Subject
        {
            [Key]
            public int SubjectId {get;set;}

            public required string Code {get;set;} = null!;

            public required string Name{get;set;} = null!;

            public int Credit {get;set;} 

            public string? Grade {get;set;}

            public decimal? GradePoint {get;set;}

            public string? Status {get;set;}

            public int? Semester {get;set;}

            public int? SemesterYear{get;set;}

            public decimal? CGPA{get;set;}

        }


}
