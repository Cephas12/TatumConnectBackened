namespace TatumConnectBackened.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? StaffId { get; set; }
        public string Role {get; set; } = null!;
        public string? RegistrationOtp { get; set; }
        public bool IsActive { get; set; } 
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        //Customer accounts

        public List<AccountDto> Accounts { get; set; } = new();

    }
}
