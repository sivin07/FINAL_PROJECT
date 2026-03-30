using CLINICAL_MANAGEMENT.DTOs.Auth;
using CLINICAL_MANAGEMENT.Repositories;

namespace CLINICAL_MANAGEMENT.Services
{


    public class AuthServiceImpl : IAuthService
    {
        private readonly IAuthRepository _repo;

        public AuthServiceImpl(IAuthRepository repo)
        {
            _repo = repo;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
            => await _repo.LoginAsync(dto);
    }



}
