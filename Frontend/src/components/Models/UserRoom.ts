import type { Room } from "./Room";
import type { User } from "./User";

export interface UserRoom {
    id: number;
    roomid: number;
    userid: number;
    Room: Room;
    User: User;
} 