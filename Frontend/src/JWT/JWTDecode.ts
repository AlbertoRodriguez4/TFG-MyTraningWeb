import i18n from '@/i18n/i18nConfigurer'

export function decodeToken(token: string): any {
  if (!token || typeof token !== 'string') {
    throw new Error(i18n.global.t('jwt_invalid_token'));
  }

  const parts = token.split('.');
  if (parts.length !== 3) {
    throw new Error(i18n.global.t('jwt_malformed'));
  }

  try {
    const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    
    // Añadir padding si falta
    const paddedBase64 = base64.padEnd(base64.length + (4 - base64.length % 4) % 4, '=');

    const decoded = atob(paddedBase64);
    return JSON.parse(decoded);
  } catch (err) {
    console.error('Fallo al decodificar el token:', err);
    throw new Error(i18n.global.t('jwt_decode_error'));
  }
}
