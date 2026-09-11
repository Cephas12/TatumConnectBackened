using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using TatumConnectBackened.Auth;
using TatumConnectBackened.Common.Constants;
using TatumConnectBackened.DTOs;
using TatumConnectBackened.Entities;
using TatumConnectBackened.Repositories;
using TatumConnectBackened.Responses;

namespace TatumConnectBackened.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly JwtService _jwtService;
        private readonly JwtSettings _settings;
        private readonly INotificationService _notificationService;
        private readonly EmailSettings _emailSettings;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository, IConfiguration configuration,
            JwtService jwtservice, IOptions<EmailSettings> emailOptions, IAccountRepository accountRepository,
            IOptions<JwtSettings> options, INotificationService notificationService
            )
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _jwtService = jwtservice;
            _settings = options.Value;
            _notificationService = notificationService;
            _emailSettings = emailOptions.Value;
            _configuration = configuration;
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var phone = request.Phone.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                return ApiResponse<UserDto>.Fail(
                    "Email is required",
                    new List<ApiError>
                    {
            new(
                "InvalidEmail",
                "Email is required")
                     });
            }
            if (string.IsNullOrWhiteSpace(phone))
            {
                return ApiResponse<UserDto>.Fail(
                    "Phone number is required.",
                    new List<ApiError>
                    {
                        new (
                            "InvalidPhone",
                            "Phone number is required"
                            )
                    }
                    );
            }
            //check existing user
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                if (!existingUser.IsRegistrationVerified)
                {
                    return ApiResponse<UserDto>.Fail(
                        "Registration is already pendimg verification",
                        new List<ApiError>
                        {
                    new (
                        "Registration pending",
                        "A registration already exist for this email. Please verify the OTP"
                        )}
                        );
                }
                return ApiResponse<UserDto>.Fail(
                    "An account with this email already exists",
                    new List<ApiError>
                    {
                new(
                    "EmailExists",
                    "An account with this email already exists."
                    )}
                    );
            }
            var otp = GenerateOtp();
            var user = new Entities.User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Phone = phone,
                FirstName = request.FirstName?.Trim(),
                LastName = request.LastName?.Trim(),
                PasswordHash = HashPassword(request.Password),
                Role = UserRoles.Customer,
                IsActive = false,
                IsRegistrationVerified = false,
                RegistrationOtp = otp,
                RegistrationOtpExpiresAt = DateTime.UtcNow.AddMinutes(10),
                OtpAttempts = 8,
                CreatedAt = DateTime.UtcNow,
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SavedChangesAsync(ct);
            //send OTP
            await SendRegistrationOtpAsync(
                user,
                otp,
                ct);
            //response
            return ApiResponse<UserDto>.Ok(
                MapToDto(user),
                "Registration initiated successfully. " +
                "A verification code has been sent to your email and phone"
                );
        }



        private static string GenerateOtp()
        {
            return RandomNumberGenerator
                .GetInt32(100000, 1000000)
                .ToString();
        }

        private async Task SendRegistrationOtpAsync(
        Entities.User user,
        string otp,
        CancellationToken ct)
        {
            var subject = "TatumConnect Registration Verification";
            var message = $"""
            Hello {user.FirstName},

            Welcome to TatumConnect.

            Your registration verification code is:

            {otp}

            This code expires in 10 minutes.

            If you did not initiate this registration, please ignore this message.

            Regards.
             TatumConnect
            """;

            //email
            await _notificationService.SendEmailAsync(
                user.Email,
                subject,
                message,
                ct);
        }


        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            return HashPassword(password) == passwordHash;

        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Department = user.Department,
                ProfileImageUrl = user.ProfileImageUrl,
                StaffId = user.StaffId,
                Role = user.Role,
                RegistrationOtp = user.RegistrationOtp,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
            };
        }

        public async Task<ApiResponse<UserDto>> VerifyRegistrationOtpAsync(VerifyRegistrationOtpRequestDto request, CancellationToken ct = default)
        {
            var email = request.Email?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email))
            {
                return ApiResponse<UserDto>.Fail("Email is required",
                    new List<ApiError> { new("InvalidEmail", "Email is required") });
            }

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return ApiResponse<UserDto>.Fail("User not found",
                    new List<ApiError> { new("NotFound", "No account found for the provided email") });
            }

            if (user.IsRegistrationVerified)
            {
                return ApiResponse<UserDto>.Fail("Registration already verified",
                    new List<ApiError> { new("AlreadyVerified", "Registration already verified") });
            }

            if (user.RegistrationOtp != request.Otp)
            {
                return ApiResponse<UserDto>.Fail("Invalid verification code",
                    new List<ApiError> { new("InvalidOtp", "The provided verification code is invalid") });
            }

            // Optionally check expiry
            if (user.RegistrationOtpExpiresAt.HasValue && user.RegistrationOtpExpiresAt.Value < DateTime.UtcNow)
            {
                return ApiResponse<UserDto>.Fail("Verification code expired",
                    new List<ApiError> { new("OtpExpired", "The verification code has expired") });
            }

            user.IsRegistrationVerified = true;
            user.IsActive = true;
            user.RegistrationOtp = null;
            user.RegistrationOtpExpiresAt = null;

            await _userRepository.SavedChangesAsync(ct);

            return ApiResponse<UserDto>.Ok(MapToDto(user), "Registration verified successfully");
        }

        public async Task<ApiResponse<UserDto>> ResendRegistrationOtpAsync(ResendRegistrationOtpRequestDto request, CancellationToken ct = default)
        {
            var email = request.Email?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email))
            {
                return ApiResponse<UserDto>.Fail("Email is required",
                    new List<ApiError> { new("InvalidEmail", "Email is required") });
            }

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return ApiResponse<UserDto>.Fail("User not found",
                    new List<ApiError> { new("NotFound", "No account found for the provided email") });
            }

            if (user.IsRegistrationVerified)
            {
                return ApiResponse<UserDto>.Fail("Registration already verified",
                    new List<ApiError> { new("AlreadyVerified", "Registration already verified") });
            }

            var otp = GenerateOtp();
            user.RegistrationOtp = otp;
            user.RegistrationOtpExpiresAt = DateTime.UtcNow.AddMinutes(10);
            user.OtpAttempts = 8;

            await _userRepository.SavedChangesAsync(ct);

            await SendRegistrationOtpAsync(user, otp, ct);

            return ApiResponse<UserDto>.Ok(MapToDto(user), "Verification code resent");
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
        {
            var email = request.Email?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return ApiResponse<LoginResponseDto>.Fail("Email and password are required",
                    new List<ApiError> { new("InvalidRequest", "Email and password are required") });
            }

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                return ApiResponse<LoginResponseDto>.Fail("Invalid credentials",
                    new List<ApiError> { new("InvalidCredentials", "Invalid email or password") });
            }

            if (!user.IsActive || !user.IsRegistrationVerified)
            {
                return ApiResponse<LoginResponseDto>.Fail("Account is not active or verified",
                    new List<ApiError> { new("AccountInactive", "Account is not active or verified") });
            }

            var token = _jwtService.GenerateAccessToken(user);
            var response = new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes),
                User = MapToDto(user)
            };

            return ApiResponse<LoginResponseDto>.Ok(response, "Login successful");
        }
    }
}
