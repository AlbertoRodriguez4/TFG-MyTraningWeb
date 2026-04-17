export function decodeToken(token: string): any {
  if (!token || typeof token !== 'string') {
    throw new Error('Token inválido o vacío');
  }

  const parts = token.split('.');
  if (parts.length !== 3) {
    throw new Error('Token JWT malformado');
  }

  try {
    const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
    
    // Añadir padding si falta
    const paddedBase64 = base64.padEnd(base64.length + (4 - base64.length % 4) % 4, '=');

    const decoded = atob(paddedBase64);
    return JSON.parse(decoded);
  } catch (err) {
    console.error('Fallo al decodificar el token:', err);
    throw new Error('No se pudo decodificar el token JWT');
  }
}
