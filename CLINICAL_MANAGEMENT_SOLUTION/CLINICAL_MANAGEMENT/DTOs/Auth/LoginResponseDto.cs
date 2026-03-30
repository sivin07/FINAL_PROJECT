namespace CLINICAL_MANAGEMENT.DTOs.Auth
{
    public class LoginResponseDto
    {
        public int StaffId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
