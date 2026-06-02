import { defineStore } from "pinia";
import { ref } from "vue";
import { useI18n } from 'vue-i18n'
import { decodeToken } from "../JWT/JWTDecode";
import type { User } from "@/components/Models/User";
import type { PurchasedItem } from "@/components/Models/PurchasedItem";
import { API_BASE_URL, getAuthHeaders, hasValidToken, isTokenExpired } from '@/config/api';
import { logger } from '@/utils/logger';

export const purchasedItems = ref<PurchasedItem[]>([]);
const BASE_URL = API_BASE_URL;

export const useUserStore = defineStore('user', () => {
  const { t } = useI18n()
  const user = ref<User[]>([]);
  const loggedUser = ref<User | null>(null);

  async function fetchUser() {
    try {
      const response = await fetch(`${BASE_URL}/api/User`, {
        method: "GET",
        mode: "cors",
        headers: getAuthHeaders()
      });

      if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

      const data = await response.json();
      user.value = data.map((d: any) => ({
        id: d.id,
        name: d.name,
        email: d.email,
        passwordhash: d.passwordhash,
        level: d.level,
        strength: d.strength,
        endurance: d.endurance,
        gold: d.gold,
        experience: d.experience,
        avatarUrl: d.avatarUrl,
        role: d.role
      }));
    } catch (error) {
      logger.error("Error fetching users:", error);
    }
  }

  async function fetchCommunityUsers() {
    try {
      const response = await fetch(`${BASE_URL}/api/User/community`, {
        method: "GET",
        mode: "cors",
        headers: getAuthHeaders()
      });

      if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

      const data = await response.json();
      user.value = data.map((d: any) => ({
        id: d.id,
        name: d.name,
        email: '',
        passwordhash: '',
        level: d.level,
        strength: d.strength,
        endurance: d.endurance,
        gold: 0,
        experience: 0,
        avatarUrl: d.avatarUrl,
        role: 'user'
      }));
    } catch (error) {
      logger.error("Error fetching community users:", error);
    }
  }
  // Función para registrar un nuevo usuario, devuelve un objeto con el resultado de la operación, el id del nuevo usuario (si se creó correctamente), 
  // si se envió el email de verificación y el email del usuario registrado (si se creó correctamente)
  async function registerUser(newUser: User): Promise<{ success: boolean; userId?: number; emailSent?: boolean; email?: string }> {
    try {
      const response = await fetch(`${BASE_URL}/api/auth/register`, {
        method: "POST",
        mode: "cors",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(newUser)
      });

      if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

      const data = await response.json();

      return {
        success: true,
        userId: data.userId,
        emailSent: data.emailSent !== false,
        email: data.email
      };
    } catch (error) {
      logger.error("Error registering user:", error);
      return { success: false };
    }
  }
  // Función para verificar el email del usuario, devuelve true si la verificación fue exitosa y false si hubo un error o la verificación falló
  async function verifyEmail(email: string, code: string): Promise<boolean> {
    try {
      const response = await fetch(`${BASE_URL}/api/auth/verify-email`, {
        method: "POST",
        mode: "cors",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ email, code })
      });

      if (!response.ok) return false;

      return true;
    } catch (error) {
      logger.error("Error verifying email:", error);
      return false;
    }
  }

  async function resendVerificationCode(email: string): Promise<boolean> {
    try {
      const response = await fetch(`${BASE_URL}/api/auth/resend-verification`, {
        method: "POST",
        mode: "cors",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({ email })
      });

      if (!response.ok) return false;

      return true;
    } catch (error) {
      logger.error("Error resending verification code:", error);
      return false;
    }
  }

  function initializeSession() {
    const token = localStorage.getItem('token');

    if (!token) {
      logger.info('No hay token en localStorage, sesión no inicializada');
      return;
    }

    // Validar formato del token antes de decodificar
    const tokenParts = token.split('.');
    if (tokenParts.length !== 3) {
      logger.warn('Token malformado detectado, limpiando sesión');
      localStorage.removeItem('token');
      return;
    }

    // Validar si el token ha expirado
    if (isTokenExpired(token)) {
      logger.warn('Token expirado detectado, limpiando sesión');
      localStorage.removeItem('token');
      return;
    }

    try {
      const decoded = decodeToken(token);
      loggedUser.value = {
        id: Number(decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]),
        name: decoded.name,
        email: decoded.email,
        passwordhash: '',
        role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
        level: Number(decoded.level),
        strength: Number(decoded.strength),
        endurance: Number(decoded.endurance),
        consistencyStreak: Number(decoded.consistencystreak),
        gold: Number(decoded.gold),
        experience: Number(decoded.experience),
        xpRequired: Number(decoded.xpRequired),
        xpRemaining: Number(decoded.xpRemaining),
        equippedStrengthItemId: decoded.equippedStrengthItemId !== "" ? Number(decoded.equippedStrengthItemId) : undefined,
        equippedEnduranceItemId: decoded.equippedEnduranceItemId !== "" ? Number(decoded.equippedEnduranceItemId) : undefined,
        avatarUrl: decoded.avatarUrl && decoded.avatarUrl !== "" ? decoded.avatarUrl : undefined
      };
      logger.info('Sesión inicializada correctamente para el usuario:', loggedUser.value?.email);
    } catch (error) {
      logger.error('Error al decodificar el token JWT:', error);
      localStorage.removeItem('token');
    }
  }

  // Inicializar sesión al cargar el store
  initializeSession();

  // Refrescar datos del usuario si hay sesión activa
  if (loggedUser.value) {
    refreshLoggedUser();
  }
  // Función para refrescar el token automáticamente
  async function refreshToken(): Promise<boolean> {
    const token = localStorage.getItem('token');
    if (!token || !loggedUser.value) {
      return false;
    }

    try {

      // verificamos que el token sigue siendo válido
      const response = await fetch(`${BASE_URL}/api/User`, {
        method: 'GET',
        headers: getAuthHeaders()
      });

      if (response.ok) {
        // El token sigue siendo válido, no necesitamos hacer nada
        logger.info('Token válido, sesión mantenida');
        return true;
      }

      if (response.status === 401) {
        // Token inválido, cerrar sesión
        logger.warn('Token inválido, cerrando sesión');
        logoutUser();
        return false;
      }

      return false;
    } catch (error) {
      logger.error('Error al refrescar token:', error);
      return false;
    }
  }

  // Verificar y refrescar token periódicamente (cada 5 minutos)
  setInterval(() => {
    if (loggedUser.value) {
      refreshToken();
    }
  }, 5 * 60 * 1000); // 5 minutos (5 * 60 * 1000 ms)

  async function loginUser(email: string, password: string): Promise<{ success: boolean; requiresEmailVerification?: boolean; message?: string }> {
    try {
      const response = await fetch(`${BASE_URL}/api/auth/login`, {
        method: "POST",
        mode: "cors",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
      });

      if (response.status === 403) {
        const data = await response.json();
        return {
          success: false,
          requiresEmailVerification: true,
          message: data?.message || t('must_verify_email')
        };
      }

      if (!response.ok) {
        return { success: false, message: `HTTP error! Status: ${response.status}` };
      }
      // Settear el token en localStorage y actualizar el estado de loggedUser con los datos decodificados del token para mantener la sesión del usuario
      // Solo ocurre si el login es exitoso, obviamente
      const data = await response.json();
      const token = data.token;
      const decoded = decodeToken(token);

      localStorage.setItem('token', token);

      loggedUser.value = {
        id: Number(decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]),
        name: decoded.name,
        email: decoded.email,
        passwordhash: '',
        role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
        level: Number(decoded.level),
        strength: Number(decoded.strength),
        endurance: Number(decoded.endurance),
        consistencyStreak: Number(decoded.consistencystreak),
        gold: Number(decoded.gold),
        experience: Number(decoded.experience),
        xpRequired: Number(decoded.xpRequired),
        xpRemaining: Number(decoded.xpRemaining),
        equippedStrengthItemId: decoded.equippedStrengthItemId !== "" ? Number(decoded.equippedStrengthItemId) : undefined,
        equippedEnduranceItemId: decoded.equippedEnduranceItemId !== "" ? Number(decoded.equippedEnduranceItemId) : undefined,
        avatarUrl: decoded.avatarUrl && decoded.avatarUrl !== "" ? decoded.avatarUrl : undefined
      };

      return { success: true };
    } catch (error) {
      logger.error("Login failed:", error);
      return { success: false, message: t('unexpected_error') };
    }
  }

// Función para recargar los datos del usuario actualmente logueado, se llama después de editar el usuario para actualizar los datos en la sesión 
// y después de iniciar sesión para cargar los datos del usuario en la sesión
  async function refreshLoggedUser() {
    if (!loggedUser.value) return;

    try {
      const response = await fetch(`${BASE_URL}/api/User/${loggedUser.value.id}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error("No se pudo refrescar el usuario");

      const updatedUser = await response.json();

      loggedUser.value = {
        ...loggedUser.value,
        ...updatedUser,
        role: updatedUser.role?.trim(),
        consistencyStreak: updatedUser.consistencystreak ?? updatedUser.consistencyStreak,
        equippedStrengthItemId: updatedUser.equippedStrengthId ?? updatedUser.equippedStrengthItemId,
        equippedEnduranceItemId: updatedUser.equippedEnduranceId ?? updatedUser.equippedEnduranceItemId,
        avatarUrl: updatedUser.avatarUrl
      };

    } catch (error) {
      logger.error("Error actualizando el usuario:", error);
    }
  }

  async function getTopThreeUsers() {
    try {
      const response = await fetch(`${BASE_URL}/api/User/getTopThreeUsers`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Error buscando usuarios');

      const data = await response.json();
      return data.map((d: any) => ({
        name: d.name,
        level: d.level,
        strength: d.strength,
        endurance: d.endurance,
        equippedStrengthItem: d.equippedStrengthItem,
        equippedEnduranceItem: d.equippedEnduranceItem,
        avatarUrl: d.avatarUrl
      }));
    } catch (error) {
      logger.error('Error en getTopThreeUsers:', error);
      return [];
    }
  }
// Función para obtener estadísticas de la comunidad, devuelve un objeto con el total de usuarios, usuarios activos hoy y total de check-ins
  async function getCommunityStats() {
    try {
      const response = await fetch(`${BASE_URL}/api/User/community-stats`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error('Error obteniendo estadísticas de comunidad');

      const data = await response.json();
      return {
        totalUsers: data.totalUsers || 0,
        activeToday: data.activeToday || 0,
        totalCheckIns: data.totalCheckIns || 0
      };
    } catch (error) {
      logger.error('Error en getCommunityStats:', error);
      return { totalUsers: 0, activeToday: 0, totalCheckIns: 0 };
    }
  }

  async function getItems() {
    try {
      const response = await fetch(`${BASE_URL}/api/Purchase/my-purchases`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

      const data: PurchasedItem[] = await response.json();
      purchasedItems.value = data;
    } catch (error) {
      logger.error("Error al obtener los items comprados:", error);
    }
  }

  async function editUser(id: number, updatedUserData: User): Promise<void> {
    try {
      const response = await fetch(`${BASE_URL}/api/User/${id}`, {
        method: 'PUT',
        mode: 'cors',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify(updatedUserData)
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`HTTP error! Status: ${response.status} - ${errorText}`);
      }

      const updatedUserResponse = await response.json();

      if (id === loggedUser.value?.id) {
        loggedUser.value = {
          ...loggedUser.value,
          ...updatedUserResponse
        };
        await refreshLoggedUser();
      }
      const userIndex = user.value.findIndex(u => u.id === id);
      if (userIndex !== -1) {
        user.value[userIndex] = {
          ...user.value[userIndex],
          ...updatedUserResponse
        };
      }

      if (loggedUser.value?.role === 'userMaster') {
        await fetchUser();
      }

    } catch (error) {
      logger.error('Error editing user:', error);
      throw error;
    }
  }

  function logoutUser() {
    localStorage.removeItem('token');
    loggedUser.value = null;
  }

  async function DeleteUser(userId: number): Promise<void> {
    try {
      const response = await fetch(`${BASE_URL}/api/User/${userId}`, {
        method: 'DELETE',
        mode: 'cors',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error(`HTTP error! Status: ${response.status}`);

      await fetchUser();
    } catch (error) {
      logger.error('Error deleting user:', error);
    }
  }
// Función para equipar un item, se llama desde el inventario del usuario, recibe el id del item a equipar, actualiza el estado de loggedUser para reflejar el 
// cambio en la interfaz y hace una petición al backend para actualizar el item equipado del usuario en la base de datos
// El backend se encarga de validar que el item pertenece al usuario y que es del tipo correcto (fuerza o resistencia) antes de actualizar la base de datos
  async function equipItem(itemId: number) {
    if (!loggedUser.value) return;

    try {
      const itemToEquip = purchasedItems.value.find(i => i.itemId === itemId);

      if (!itemToEquip) {
        logger.error("Item no encontrado en inventario");
        return;
      }

      const response = await fetch(`${BASE_URL}/api/User/equip/${loggedUser.value.id}/${itemId}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || "Error al equipar objeto");
      }
 // Typelower se usa para comparar el tipo del item (fuerza o resistencia) sin importar mayúsculas/minúsculas ni el idioma (inglés o español) y actualizar el estado de loggedUser con el id del item equipado en la ranura correspondiente (ranura de fuerza o ranura de resistencia) para reflejar el cambio en la interfaz después de equipar el item
      const typeLower = itemToEquip.itemType.toLowerCase();

      if (typeLower === 'strength' || typeLower === 'fuerza') {
        loggedUser.value.equippedStrengthItemId = itemId;
      } else if (typeLower === 'endurance' || typeLower === 'resistencia') {
        loggedUser.value.equippedEnduranceItemId = itemId;
      }

      await refreshLoggedUser();

    } catch (error) {
      logger.error("Error equipando objeto:", error);
    }
  }
// Misma lógica que equipItem pero para desequipar un item, se llama desde el inventario del usuario, 
// recibe el tipo del item a desequipar (fuerza o resistencia), actualiza el estado de loggedUser para reflejar el
  async function unequipItem(type: string) {
    if (!loggedUser.value) return;

    try {
      const response = await fetch(`${BASE_URL}/api/User/unequip/${loggedUser.value.id}/${type}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) throw new Error("Error al desequipar");

      const typeLower = type.toLowerCase();

      if (typeLower === 'strength' || typeLower === 'fuerza') {
        loggedUser.value.equippedStrengthItemId = undefined;
      } else if (typeLower === 'endurance' || typeLower === 'resistencia') {
        loggedUser.value.equippedEnduranceItemId = undefined;
      }

      await refreshLoggedUser();
    } catch (error) {
      logger.error("Error desequipando:", error);
    }
  }
// Función para cambiar la contraseña del usuario, se llama desde la configuración de la cuenta, recibe la contraseña actual y 
// la nueva contraseña, hace una petición al backend para cambiar la contraseña del usuario en la base de datos, el backend se encarga de validar que la 
// contraseña actual es correcta antes de actualizarla por la nueva contraseña
  async function changePassword(currentPassword: string, newPassword: string): Promise<boolean> {
    if (!loggedUser.value) return false;

    try {
      const response = await fetch(`${BASE_URL}/api/User/change-password`, {
        method: 'POST', 
        mode: 'cors',
        headers: {
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        // Enviamos la actual (para verificar) y la nueva
        body: JSON.stringify({
          currentPassword: currentPassword,
          newPassword: newPassword
        })
      });

      if (!response.ok) {
        return false; // Retornamos false si la contraseña actual era incorrecta
      }

      return true;
    } catch (error) {
      logger.error("Error al cambiar la contraseña:", error);
      return false;
    }
  }
  return {
    user,
    fetchUser,
    fetchCommunityUsers,
    loginUser,
    loggedUser,
    getTopThreeUsers,
    getCommunityStats,
    getItems,
    purchasedItems,
    refreshLoggedUser,
    editUser,
    logoutUser,
    DeleteUser,
    registerUser,
    verifyEmail,
    resendVerificationCode,
    equipItem,
    unequipItem,
    changePassword
  };
}); 
