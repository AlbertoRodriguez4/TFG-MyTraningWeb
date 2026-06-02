import type { User } from "./User";

export interface Room {
    id: number;
    name: string;
    minlevel: number;
    minstats: number;
    minconsistency: number;
    description: string;
    date: string;
    localization?: string;
    creatorRole?: string;
    creatorId?: number;
}
