using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace ExcelUploadApi.Models
{
      public class SubjectCreateDto
        {
        
            public required string Code {get;set;} = null!;

            public required string Name{get;set;} = null!;

            public int Credit {get;set;} 

            public string? Grade {get;set;}

            public decimal? GradePoint {get;set;}

            public string? Status {get;set;}

            public int? Semester {get;set;}

            public int? SemesterYear{get;set;}

            public int? CGPA{get;set;}


        }


}
