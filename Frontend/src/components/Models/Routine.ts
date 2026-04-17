import type { User } from "./User";

export interface Routine {
    id: number;
    userId: number;
    user: User; // Asumiendo que ya tienes una interfaz IUser para el modelo User
    date: string; // Usamos string para representar la fecha en formato ISO
    isCompleted: boolean;
}
