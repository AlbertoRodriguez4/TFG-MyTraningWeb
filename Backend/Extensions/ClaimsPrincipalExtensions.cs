using System.Security.Claims;
using AA2_CS.Model.Common;

namespace AA2_CS.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            // Intentar obtener el claim del identificador de usuario y convertirlo a un entero. Si no se puede convertir, se devuelve null.
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(claim, out int userId))
                return userId;

            return null;
        }

       
        public static string? GetUserRole(this ClaimsPrincipal user)
        {
            // Obtener el claim del rol del usuario. Si no existe, se devuelve null.
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

       
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            // Verificar si el rol del usuario es "userMaster", lo que indica que es un administrador. Se utiliza la constante definida en la clase Roles para evitar errores de tipeo.
            return user.GetUserRole() == Roles.userMaster;
        }

      
        public static bool IsSelfOrAdmin(this ClaimsPrincipal user, int userId)
        {
            // Verificar si el usuario es el mismo que el userId proporcionado o si es un administrador. Se obtiene el ID del usuario actual y se compara con el userId proporcionado, o se verifica si el usuario es un administrador.
            var currentUserId = user.GetUserId();
            return currentUserId.HasValue && (currentUserId.Value == userId || user.IsAdmin());
        }
    }
}
