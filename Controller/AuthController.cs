using ExcelUploadApi.Models;
using Microsoft.AspNetCore.Mvc;
using ExcelUploadApi.Repository;


namespace ExcelUploadApi.Controller
{
    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController : ControllerBase
    {
        //Readonly

        private readonly IStudentRepository _sRepo;

        public AuthController (IStudentRepository sRepo)
        {
            _sRepo = sRepo;
        }

        //Endpoints

        //Register a new user
        [HttpPost("signup")]
        public async Task<IActionResult> RegisterUser( SignupDto dto)
        {
            //Check if password is < 6 
            if(dto.Password.Length <6)
            {
                return BadRequest(new{Message = "Password must be at least 6 characters long"});
            }

           //Hash the password
           string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

           //Map DTO to Entity
            var student = new Student
            {
                Email = dto.Email,
                Password = hashedPassword,
            };

            await _sRepo.RegisterUser(student);


            return Ok(new{Message="User sucessfully registered"});
        }

        //Login user
        [HttpPost("login")]
        public async Task<IActionResult> LoginUser( LoginDto dto)
        {
           
            
           //Fetch user by email
           var existing = await _sRepo.GetByEmail(dto.Email);
           if(existing == null)
            {
                return Unauthorized(new{Message = "Invalid Email or Password"});
            }

            

            //Verify password using bycrypt
            if(!BCrypt.Net.BCrypt.Verify(dto.Password,existing.Password))
            {
                return Unauthorized(new{Message = "Invalid Email or Password"});
            }

            //Return Ok
            return Ok(new{Message="User sucessfully Login",Email= dto.Email});
        }
    }
}