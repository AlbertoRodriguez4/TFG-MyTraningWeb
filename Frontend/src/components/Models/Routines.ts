export interface Routines {
    id: number;
    name: string;
    description: string;
    difficulty: number;
    reward: number;
    iscompleted: boolean;
    createdat: Date;
    userId: number;
    trainingfocus: string;
}

export interface DifficultyLevel {
    value: number;
    label: string;
    icon: string;
    color: string;
    xpBonus: number;
}

export interface TrainingType {
    value: string;
    label: string;
    icon: string;
    color: string;
    description: string;
}