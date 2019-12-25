using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DatingApp.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DatingApp.Api.Models;
using DatingApp.Api.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        private readonly IConfiguration _config ;

        public AuthController(IAuthRepository repo,IConfiguration config)
        {
             _repo = repo;
             _config  = config;
             
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegistorsDto userForRegistorsDto){
              
              userForRegistorsDto.UserName = userForRegistorsDto.UserName.ToLower();
              if(await _repo.UserExist(userForRegistorsDto.UserName))
                 return BadRequest("User already exist");
            
             var userToCreate = new User{
                  Username =userForRegistorsDto.UserName
             };

             var createdUser=await _repo.Register(userToCreate,userForRegistorsDto.Password);
             return StatusCode(201);
 
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.UserName.ToLower(),userForLoginDto.Password);
            if(userFromRepo == null)  return Unauthorized();

            var claims = new []
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name ,userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cred
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
               token = tokenHandler.WriteToken(token)
            });
            
        }
        
    }
}