using BankingSystem.Constants;
using BankingSystem.Data.models;
using BankingSystem.DTOS.Auth_IdentityDTOs.Request_DTOs;
using BankingSystem.DTOS.Auth_IdentityDTOs.RequestDTOs;
using BankingSystem.DTOS.Auth_IdentityDTOs.ResponseDto;
using BankingSystem.DTOS.Shared;
using BankingSystem.Exceptions;
using BankingSystem.Repository.GenericRepository.UnitOfWork;
using BankingSystem.Service.AuthService;
using BankingSystem.Service.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BankingSystem.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration configuration,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterCustomerAsync(RegisterCustomerDto request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Attempt to register with already existing email {Email}.", request.Email);
                throw new ConflictException("That Email Already Has An Account.");
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.Email.Split('@')[0]
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Customer creation failed for {Email}. Errors: {Errors}", request.Email, errors);
                throw new BadRequestException($"Failed to create user: {errors}");
            }

            await EnsureRoleExistsAsync(AppRoles.Customer, "Standard Bank Customer Role");
            await _userManager.AddToRoleAsync(user, AppRoles.Customer);

            var customer = new Customer
            {
                Name = request.Name,
                Street = request.Street,
                City = request.City,
                UserId = user.Id,
                user = user,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            await SentEmailConfirmation(user, request.Email);

            _logger.LogInformation("Customer User {UserId} registered successfully.", user.Id);

            var refreshToken = await GenerateAndSaveRefreshToken(user.Id);

            return new ApiResponseDto<AuthResponseDto>
            {
                Data = new AuthResponseDto
                {
                    Token = GenerateJwtToken(AppRoles.Customer, user, customer.Id),
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Email = user.Email,
                    Role = AppRoles.Customer,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.ExpiresOn
                },
                Message = "Customer registered successfully. Please check your email to confirm your account."
            };
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RegisterBankerAsync(RegisterBankerDto request)
        {
            if (request.SecretCode != _configuration["BankerSecretKey"])
            {
                _logger.LogWarning("SECURITY ALERT: Failed attempt to register Banker for email {Email} using an invalid secret code.", request.Email);
                throw new UnauthorizedException("The Banker Secret Key Is Wrong.");
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Banker Registration failed: Attempt to register with already existing email {Email}.", request.Email);
                throw new ConflictException("That Email Already Has An Account.");
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.Name.Replace(" ", ""),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Banker creation failed for {Email}. Errors: {Errors}", request.Email, errors);
                throw new BadRequestException($"Failed to create user: {errors}");
            }

            await EnsureRoleExistsAsync(AppRoles.Banker, "Bank Employee Role");
            await _userManager.AddToRoleAsync(user, AppRoles.Banker);

            var banker = new Banker
            {
                Name = request.Name,
                BranchId = request.BranchId,
                UserId = user.Id,
                user = user,
                IsActive = true
            };

            await _unitOfWork.Bankers.AddAsync(banker);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Banker User {UserId} registered successfully for Branch {BranchId}.", user.Id, banker.BranchId);

            var refreshToken = await GenerateAndSaveRefreshToken(user.Id);

            return new ApiResponseDto<AuthResponseDto>
            {
                Data = new AuthResponseDto
                {
                    Token = GenerateJwtToken(AppRoles.Banker, user, banker.Id),
                    Expiration = DateTime.UtcNow.AddHours(8),
                    Email = user.Email,
                    Role = AppRoles.Banker,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.ExpiresOn
                },
                Message = "Banker registered successfully."
            };
        }

        public async Task<ApiResponseDto<AuthResponseDto>> LoginAsync(LoginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogWarning("Failed login attempt for email: {Email}.", request.Email);
                throw new UnauthorizedException("Invalid email or password.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                _logger.LogWarning("User {UserId} attempted to log in without confirming their email.", user.Id);
                throw new UnauthorizedException("رجاءً تأكيد بريدك الإلكتروني أولاً عبر الرابط المرسل إليك.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var primaryRole = userRoles.FirstOrDefault() ?? AppRoles.Customer;
            int profileId = 0;

            if (primaryRole == AppRoles.Customer)
            {
                var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(user.Id,false);
                if (customer == null || !customer.IsActive)
                {
                    _logger.LogWarning("Data inconsistency or inactive account: Customer profile missing for User {UserId}.", user.Id);
                    throw new NotFoundException("Profile not found or corrupted.");
                }
                profileId = customer.Id;
            }
            else if (primaryRole == AppRoles.Banker)
            {
                var banker = await _unitOfWork.Bankers.GetBankerByUserIdAsync(user.Id, false);
                if (banker == null || !banker.IsActive)
                {
                    _logger.LogWarning("Data inconsistency or inactive account: Banker profile missing for User {UserId}.", user.Id);
                    throw new NotFoundException("Profile not found or corrupted.");
                }
                profileId = banker.Id;
            }

            _logger.LogInformation("User {UserId} logged in successfully as {Role}.", user.Id, primaryRole);

            var refreshToken = await GenerateAndSaveRefreshToken(user.Id);

            return new ApiResponseDto<AuthResponseDto>
            {
                Data = new AuthResponseDto
                {
                    Token = GenerateJwtToken(primaryRole, user, profileId),
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Email = user.Email,
                    Role = primaryRole,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiration = refreshToken.ExpiresOn
                },
                Message = "Login successful."
            };
        }

        public async Task<ApiResponseDto<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var existingToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.Token);

            if (existingToken == null || !existingToken.IsActive)
            {
                _logger.LogWarning("Security Warning: Attempted to use an invalid, expired, or revoked refresh token.");
                throw new UnauthorizedException("Invalid or Expired Refresh Token. Please login again.");
            }

            existingToken.RevokedOn = DateTime.UtcNow;
            await _unitOfWork.RefreshTokens.Update(existingToken);

            var user = await _userManager.FindByIdAsync(existingToken.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("User associated with this token no longer exists.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var primaryRole = userRoles.FirstOrDefault() ?? AppRoles.Customer;
            int profileId = 0;

            if (primaryRole == AppRoles.Customer)
            {
                var customer = await _unitOfWork.Customers.GetCustomerByUserIdAsync(user.Id, false);
                if (customer != null) profileId = customer.Id;
            }
            else if (primaryRole == AppRoles.Banker)
            {
                var banker = await _unitOfWork.Bankers.GetBankerByUserIdAsync(user.Id, false);
                if (banker != null) profileId = banker.Id;
            }

            var newJwtToken = GenerateJwtToken(primaryRole, user, profileId);
            var newRefreshToken = await GenerateAndSaveRefreshToken(user.Id);

            _logger.LogInformation("Refresh token successfully exchanged for User {UserId}.", user.Id);

            return new ApiResponseDto<AuthResponseDto>
            {
                Data = new AuthResponseDto
                {
                    Token = newJwtToken,
                    Expiration = DateTime.UtcNow.AddHours(1),
                    Email = user.Email,
                    Role = primaryRole,
                    RefreshToken = newRefreshToken.Token,
                    RefreshTokenExpiration = newRefreshToken.ExpiresOn
                },
                Message = "Token refreshed successfully."
            };
        }

        public async Task<ApiResponseDto<string>> ConfirmEmailAsync(int userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                _logger.LogWarning("Email confirmation failed: User ID {UserId} not found.", userId);
                throw new NotFoundException("User not found.");
            }

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(code);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Email confirmation failed for User {UserId}. Errors: {Errors}", userId, errors);
                throw new BadRequestException($"Invalid email confirmation token: {errors}");
            }

            _logger.LogInformation("User {UserId} successfully confirmed their email.", userId);

            return new ApiResponseDto<string>
            {
                Data = null,
                Message = "Your email has been confirmed successfully! You can now log in."
            };
        }

        public async Task<ApiResponseDto<string>> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("try Forgot Password failed: Attempt to rest Password for a non existing email {Email}.", request.Email);
                throw new BadRequestException("That Email does not Have An Account");
            }

            await SentForgotPassword(user, request.Email);

            return new ApiResponseDto<string>
            {
                Data = null,
                Message = "Email Has Been Sent"
            };
        }

        public async Task<ApiResponseDto<string>> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new BadRequestException("Invalid Request.");
            }

            var decodedTokenBytes = WebEncoders.Base64UrlDecode(request.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Password reset failed for {Email}. Errors: {Errors}", request.Email, errors);
                throw new BadRequestException($"Failed to reset password: {errors}");
            }

            _logger.LogInformation("Password for {Email} has been reset successfully.", request.Email);

            return new ApiResponseDto<string>
            {
                Data = null,
                Message = "Your password has been reset successfully. You can now log in."
            };
        }

        #region Private Helper Methods

        private string GenerateJwtToken(string role, User user, int profileId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role),
                new Claim("ProfileId", profileId.ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                signingCredentials: credentials,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:DurationInMinutes"] ?? "60"))
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RefreshToken> GenerateAndSaveRefreshToken(string userId)
        {
            var randomNumber = new byte[32];
            RandomNumberGenerator.Fill(randomNumber);
            var tokenString = Convert.ToBase64String(randomNumber);

            var refreshToken = new RefreshToken
            {
                CreatedOn = DateTime.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddDays(7),
                RevokedOn = null,
                Token = tokenString,
                UserId = userId
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            return refreshToken;
        }

        private async Task EnsureRoleExistsAsync(string roleName, string description)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new Role
                {
                    Name = roleName,
                    Description = description,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }
        }

        private async Task SentEmailConfirmation(User user, string email)
        {
            var emailConfirmation = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmation));
            string link = $"https://localhost:7132/api/Auth/Confirm-Email?userId={user.Id}&code={confirmationToken}";

            string emailBody = $@"
            <!DOCTYPE html>
            <html lang='ar' dir='rtl'>
            <head><meta charset='UTF-8'></head>
            <body style='font-family: Arial, sans-serif; text-align: center; padding: 30px; background-color: #f9f9f9;'>
                <div style='background-color: #ffffff; padding: 20px; border-radius: 8px; max-width: 500px; margin: auto; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                    <h2 style='color: #333;'>أهلاً بك في نظامنا البنكي! 🏦</h2>
                    <p style='color: #555; font-size: 16px; line-height: 1.5;'>
                        سعداء بانضمامك إلينا. لتفعيل حسابك البنكي والبدء في إجراء معاملاتك بأمان، يرجى الضغط على الزر أدناه:
                    </p>
                    <br>
                    <a href='{link}' style='background-color: #512BD4; color: #ffffff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                        تأكيد الحساب البنكي
                    </a>
                    <br><br>
                    <p style='color: #999; font-size: 12px;'>لو لم تقم بالتسجيل لدينا، يرجى تجاهل هذه الرسالة.</p>
                </div>
            </body>
            </html>";

            await _emailService.SendEmailAsync(email, "تأكيد حسابك البنكي - Confirm your email", emailBody);
        }

        private async Task SentForgotPassword(User user, string email)
        {
            var passwordReset = await _userManager.GeneratePasswordResetTokenAsync(user);
            var confirmationToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(passwordReset));
            string link = $"https://localhost:7132/reset-password?email={user.Email}&token={confirmationToken}";

            string emailBody = $@"
            <!DOCTYPE html>
            <html lang='ar' dir='rtl'>
            <head><meta charset='UTF-8'></head>
            <body style='font-family: Arial, sans-serif; text-align: center; padding: 30px; background-color: #f9f9f9;'>
                <div style='background-color: #ffffff; padding: 20px; border-radius: 8px; max-width: 500px; margin: auto; box-shadow: 0 0 10px rgba(0,0,0,0.1);'>
                    <h2 style='color: #333;'>إعادة تعيين كلمة المرور البنكية 🔒</h2>
                    <p style='color: #555; font-size: 16px; line-height: 1.5;'>
                        لقد تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك البنكي. لتعيين كلمة مرور جديدة، يرجى الضغط على الزر أدناه:
                    </p>
                    <br>
                    <a href='{link}' style='background-color: #512BD4; color: #ffffff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>
                        تغيير كلمة المرور
                    </a>
                    <br><br>
                    <p style='color: #999; font-size: 12px;'>إذا لم تكن أنت من طلب هذا التغيير، تجاهل هذه الرسالة ولن يتم تغيير شيء.</p>
                </div>
            </body>
            </html>";

            await _emailService.SendEmailAsync(email, "إعادة تعيين كلمة المرور - Confirm your Password", emailBody);
        }

        #endregion
    }
}