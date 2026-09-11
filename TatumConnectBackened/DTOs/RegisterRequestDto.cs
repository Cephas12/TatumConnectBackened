using System.ComponentModel.DataAnnotations;

namespace TatumConnectBackened.DTOs
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        [MaxLength(254, ErrorMessage = "Email address is too long")]

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,100}$",
            ErrorMessage = "Password must contain at least one uppercase letter, " + "one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s'-]+$", ErrorMessage = "First name contains invalid characters")]
        public string? FirstName { get; set; }

        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s'-]+$", ErrorMessage = "Last name contains invalid characters")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(?:\+234|0)[789][01]\d{8}$", ErrorMessage = "Please provide a valid Nigerian phone number.")]
       
        public string? Phone { get; set; } = string.Empty;

    }

}
