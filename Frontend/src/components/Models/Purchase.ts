import type { Item } from "./Item";
import type { User } from "./User";

export interface Purchase {
    id: number;
    userId: number;
    user: User; // Asumiendo que ya tienes una interfaz IUser para el modelo User
    itemId: number;
    item: Item; // Asumiendo que ya tienes una interfaz IItem para el modelo Item
    purchaseDate: string; // Usamos string para representar la fecha en formato ISO
}
