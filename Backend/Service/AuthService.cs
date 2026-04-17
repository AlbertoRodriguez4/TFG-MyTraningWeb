using System.Security.Claims;
using System.Security.Cryptography;
using AA2_CS.JWT;
using AA2_CS.Model;
using AA2_CS.Service;
using AA2_CS.Repository;

namespace AA2_CS.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly JWTConfigurer _jwtConfigurer;
        private readonly EmailService _emailService;
        private readonly EmailVerificationRepository _verificationRepository;

        public AuthService(UserService userService, JWTConfigurer jwtConfigurer, EmailService emailService, EmailVerificationRepository verificationRepository)
        {
            _userService = userService;
            _jwtConfigurer = jwtConfigurer;
            _emailService = emailService;
            _verificationRepository = verificationRepository;
        }

        public string Login(string email, string password)
        {
            var user = _userService.Login(email, password);
            if (user == null)
                return null;
            if (!user.isEmailVerified)
                throw new EmailNotVerifiedException("Debes verificar tu correo antes de iniciar sesión.");

            return _jwtConfigurer.GenerateToken(user);
        }

        public async Task<(User user, bool emailSent)> RegisterAsync(User user)
        {
            var registeredUser = _userService.Register(user);
            var verificationCode = GenerateVerificationCode();

            await _verificationRepository.InvalidatePreviousAsync(registeredUser.id);
            var verification = new EmailVerification(registeredUser.id, verificationCode, expirationMinutes: 15);
            await _verificationRepository.AddAsync(verification);
            bool emailSent = await _emailService.SendVerificationEmail(registeredUser.email, registeredUser.name, verificationCode);
            return (registeredUser, emailSent);
        }

        public async Task<bool> VerifyEmailAsync(string email, string code)
        {
            var user = _userService.FindByEmail(email);
            if (user == null)
            {
                return false;
            }

            var verification = await _verificationRepository.GetValidVerificationAsync(user.id, code);

            if (verification == null)
            {
                return false;
            }

            await _verificationRepository.MarkAsUsedAsync(verification);

            user.isEmailVerified = true;
            _userService.Update(user);

            return true;
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            var user = _userService.FindByEmail(email);
            if (user == null || user.isEmailVerified)
            {
                return false;
            }

            var verificationCode = GenerateVerificationCode();
            await _verificationRepository.InvalidatePreviousAsync(user.id);
            var verification = new EmailVerification(user.id, verificationCode, expirationMinutes: 15);
            await _verificationRepository.AddAsync(verification);
            return await _emailService.SendVerificationEmail(user.email, user.name, verificationCode);
        }

        private string GenerateVerificationCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000).ToString("D6");
        }

        public bool HasAccessToResource(int requestedUserID, ClaimsPrincipal user) 
        {
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim is null || !int.TryParse(userIdClaim.Value, out int userId)) 
            { 
                return false; 
            }
            var isOwnResource = userId == requestedUserID;

            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var isAdmin = roleClaim?.Value == Roles.userMaster;
            
            var hasAccess = isOwnResource || isAdmin;
            return hasAccess;
        }
    }

    public sealed class EmailNotVerifiedException : Exception
    {
        public EmailNotVerifiedException(string message) : base(message)
        {
        }
    }
}
