export interface Item {
    id: number;         
    name: string;       
    type: "Strength" | "Endurance"; 
    bonus: number;      
    price: number;   
    imageUrl?: string;   
}
