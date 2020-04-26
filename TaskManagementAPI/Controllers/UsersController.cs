using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TaskManagementAPI.Controllers
{
    [Authorize(Roles = "1")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {      
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public UsersController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _config = configuration;
        }
        
        // POST api/<controller>
        [HttpPost("create")]
        public async Task<IActionResult> Create(UserCreateDto userCreateDto)
        {
            userCreateDto.UserName = userCreateDto.UserName.ToLower();
            if (await _unitOfWork.Users.IsUserExist(userCreateDto.UserName))           
                return BadRequest("username already exist");

            var userToCreate = new User
            {
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                UserName = userCreateDto.UserName.ToLower(),
                Role = userCreateDto.Role
            };

            var createdUser = await _unitOfWork.Users.Create(userToCreate, userCreateDto.Password);
            await _unitOfWork.Complete();

            return StatusCode(201);            
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var userFromRepo = await _unitOfWork.Users.Login(userLoginDto.UserName.ToLower(), userLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.UserName),
                new Claim(ClaimTypes.Role, userFromRepo.Role.ToString())                
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }
                
        [HttpGet("list")]
        public async Task<IEnumerable<User>> GetUsersList(int pageIndex, int pageSize)
        {
            return await _unitOfWork.Users.GetAll(pageIndex, pageSize);
        }
        
        [HttpGet("{id}")]
        public async Task<User> GetUser(int id)
        {
            return await _unitOfWork.Users.GetById(id);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> UpdateUser(User user)
        {
            var userToUpdate = await _unitOfWork.Users.GetById(user.Id);
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Role = user.Role;            
            _unitOfWork.Users.Update(userToUpdate);
            await _unitOfWork.Complete();
            return Ok("User Updated Successfuly");
        }

        [HttpPost("delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            var userToDelete = await _unitOfWork.Users.GetById(Id);
            if (userToDelete != null)
                _unitOfWork.Users.Remove(userToDelete);
            await _unitOfWork.Complete();

            return Ok("User Deleted Successfuly");
        }
    }
}
