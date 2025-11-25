// utils/jwt-utils.ts
export function decodeJwt(token: string): any {
  if (!token) return null;
  const payload = token.split('.')[1];
  const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
  return JSON.parse(decoded);
}
