export interface PurchasedItem {
    purchaseId: number;
    userId: number;
    userName: string;
    userEmail: string;
    itemId: number;
    itemName: string;
    itemType: "Strength" | "Endurance" | string; 
    itemBonus: number;
    itemPrice: number;
    purchaseDate: string; 
    imageUrl?: string;
  }
  