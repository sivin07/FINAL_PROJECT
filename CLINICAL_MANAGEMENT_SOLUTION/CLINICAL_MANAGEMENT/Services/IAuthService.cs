using CLINICAL_MANAGEMENT.DTOs.Auth;

namespace CLINICAL_MANAGEMENT.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}
