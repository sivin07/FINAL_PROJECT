using CLINICAL_MANAGEMENT.DTOs.Auth;
using CLINICAL_MANAGEMENT.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CLINICAL_MANAGEMENT.Repositories
{
    public class AuthRepoImpl : IAuthRepository
    {
        private readonly CmsContext _context;
        private readonly IConfiguration _config;

        // Constructor Injection ✅
        public AuthRepoImpl(CmsContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var staff = await _context.Staff
                .Include(s => s.Role)
                .FirstOrDefaultAsync(s =>
                    s.Username == dto.Username &&
                    s.Password == dto.Password);

            if (staff == null) return null;

            return new LoginResponseDto
            {
                StaffId = staff.StaffId,
                Name = staff.Name,
                Username = staff.Username,
                Role = staff.Role.RoleName,
                Token = GenerateToken(staff)
            };
        }

        private string GenerateToken(Staff staff)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, staff.StaffId.ToString()),
            new Claim(ClaimTypes.Name, staff.Username),
            new Claim(ClaimTypes.Role, staff.Role.RoleName),
            new Claim("StaffId", staff.StaffId.ToString()),
            new Claim("Name", staff.Name)
        };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    double.Parse(_config["Jwt:DurationInMinutes"] ?? "60")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
