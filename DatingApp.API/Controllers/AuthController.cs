using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _cofig;
        public AuthController(IAuthRepository repo, IConfiguration cofig)
        {
            _cofig = cofig;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserToRegisterDTO userToRegisterDTO)
        {

            //if(!ModelState.IsValid)
            //return BadRequest(ModelState);
            userToRegisterDTO.Username = userToRegisterDTO.Username.ToLower();

            if (await _repo.UserExists(userToRegisterDTO.Username))
                return BadRequest("username already exists");

            var userToCreate = new User
            {

                UserName = userToRegisterDTO.Username
            };

            var createdUser = await _repo.Register(userToCreate, userToRegisterDTO.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(UserToLoginDTO userToLoginDTO)
        {
            var userFromRepo = await _repo.Login(userToLoginDTO.Username.ToLower(), userToLoginDTO.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[] {

                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cofig.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {

                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(
                new {
                token = tokenHandler.WriteToken(token)
                }
               );


        }
    }
}