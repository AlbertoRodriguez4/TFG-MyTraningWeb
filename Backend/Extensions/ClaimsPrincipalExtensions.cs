using System.Security.Claims;
using AA2_CS.Model;

namespace AA2_CS.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Obtiene el ID del usuario desde el claim NameIdentifier.
        /// </summary>
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claim, out int userId))
                return userId;

            return null;
        }

        /// <summary>
        /// Obtiene el rol del usuario desde el claim Role.
        /// </summary>
        public static string? GetUserRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

        /// <summary>
        /// Verifica si el usuario tiene el rol de administrador (userMaster).
        /// </summary>
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.GetUserRole() == Roles.userMaster;
        }

        /// <summary>
        /// Verifica si el usuario autenticado es el mismo que el userId proporcionado,
        /// o si es administrador.
        /// </summary>
        public static bool IsSelfOrAdmin(this ClaimsPrincipal user, int userId)
        {
            var currentUserId = user.GetUserId();
            return currentUserId.HasValue && (currentUserId.Value == userId || user.IsAdmin());
        }
    }
}
