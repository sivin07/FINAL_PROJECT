using CLINICAL_MANAGEMENT.DTOs.Auth;
using CLINICAL_MANAGEMENT.Models;

namespace CLINICAL_MANAGEMENT.Repositories
{
    public interface IAuthRepository
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}
