using AA2_CS.Database;
using AA2_CS.Model.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AA2_CS.Repository
{
    public class EmailVerificationRepository
    {
        private readonly AppDbContext _context;

        public EmailVerificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task<EmailVerification> AddAsync(EmailVerification verification)
        {
            await _context.EmailVerifications.AddAsync(verification);
            await _context.SaveChangesAsync();
            return verification;
        }

        public async System.Threading.Tasks.Task<EmailVerification?> GetValidVerificationAsync(int userId, string code)
        {
            // Obtener una verificación de correo electrónico válida para un usuario específico, verificando que el código coincida, que no se haya usado y que no haya expirado
            var verification = await _context.EmailVerifications
                .FirstOrDefaultAsync(v =>
                    v.userid == userId &&
                    v.code == code &&
                    v.isused == false &&
                    v.expiresat > DateTime.UtcNow);

            return verification;
        }

        public async System.Threading.Tasks.Task MarkAsUsedAsync(EmailVerification verification)
        {
            // Marcar una verificación de correo electrónico como usada, para evitar su reutilización
            verification.isused = true;
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteExpiredAsync(int userId)
        {
            // Eliminar todas las verificaciones de correo electrónico expiradas para un usuario específico, para mantener la base de datos limpia y evitar que se acumulen verificaciones antiguas
            var expired = await _context.EmailVerifications
                .Where(v => v.userid == userId && v.expiresat <= DateTime.UtcNow)
                .ToListAsync();

            if (expired.Any())
            {
                _context.EmailVerifications.RemoveRange(expired);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task InvalidatePreviousAsync(int userId)
        {
            // Invalidar todas las verificaciones de correo electrónico anteriores no usadas para un usuario específico, para asegurarse de que solo la última verificación enviada sea válida
            var previousVerifications = await _context.EmailVerifications
                .Where(v => v.userid == userId && v.isused == false)
                .ToListAsync();

            foreach (var verification in previousVerifications)
            {
                verification.isused = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
