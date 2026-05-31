using System.Security.Claims;
using AA2_CS.Model.Entities;
using AA2_CS.Repository;

namespace AA2_CS.Services
{
    public class AuthService
    {
        private readonly AuthRepository _repository;

        public AuthService(AuthRepository repository)
        {
            _repository = repository;
        }

        public string Login(string email, string password)
        {
            return _repository.Login(email, password);
        }

        public async Task<(User user, bool emailSent)> RegisterAsync(User user)
        {
            return await _repository.RegisterAsync(user);
        }

        public async Task<bool> VerifyEmailAsync(string email, string code)
        {
            return await _repository.VerifyEmailAsync(email, code);
        }

        public async Task<bool> ResendVerificationEmailAsync(string email)
        {
            return await _repository.ResendVerificationEmailAsync(email);
        }

        public bool HasAccessToResource(int requestedUserID, ClaimsPrincipal user)
        {
            return _repository.HasAccessToResource(requestedUserID, user);
        }
    }
}
