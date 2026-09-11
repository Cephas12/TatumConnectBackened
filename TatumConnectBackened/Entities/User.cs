using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using TatumConnectBackened.Common.Constants;

namespace TatumConnectBackened.Entities
{
    public class User
    {
            public Guid Id { get; set; }

            [Required]
            [MaxLength(255)]
            public string Email { get; set; } = null!;

            public string? PasswordHash { get; set; }

            [MaxLength(100)]
            public string? FirstName { get; set; }

            [MaxLength(100)]
            public string? LastName { get; set; }

            [MaxLength(30)]
            public string? PhoneNumber { get; set; }

            [MaxLength(150)]
            public string? Department { get; set; }

            [MaxLength(500)]
            public string? ProfileImageUrl { get; set; }

            [MaxLength(30)]
            public string? StaffId { get; set; }

            [Required]
            [MaxLength(50)]
            public string Role { get; set; } = UserRoles.Customer;

            ///<summary>
            /// Indicates whether the user has completed password setup.
            ///</summary>


            public bool IsActive { get; set; } = false;

            // PASSWORD SETUP / INVITATION

            /// <summary>
            /// one-time token used by invited admins to set
            /// their password.
            /// </summary>

            public string? PasswordSetupToken { get; set; }

            public DateTime? PasswordSetupTokenExpiresAt { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

            public DateTime? UpdatedAt { get; set; }

            public DateTime? LastLoginAt { get; set; }

            //OTP / REGISTRATION VERIFICATION

            ///<summary>
            /// Expiration time for the registration OTIp.
            /// </summary>
            public DateTime? RegistrationOtpExpiresAt { get; set; }

            ///<summary>
            /// Number of OTP verifcation attempts
            /// </summary>
            public int OtpAttempts { get; set; }
            ///<summary>
            /// Indicates whether the customer's registration has been verified.
            /// </summary>
            public bool IsRegistrationVerified
            {
                get; set;
            } = false;
            public string? PasswordResetToken { get; set; }
            public DateTime? PasswordResetTokenExpiresAt { get; set; }

            public string? PasswordResetOtp { get; set; }
            public DateTime? PasswordResetOtpExpiresAt { get; set; }

            public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

            public ICollection<Account> Accounts { get; set; } = new List<Account>();
        public string? RegistrationOtp { get;  set; }
        public string? Phone { get;  set; }
    }


    }

