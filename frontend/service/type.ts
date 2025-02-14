export type Reminder = {

    id: string,
    createdBy: string,
    content: string,
    color: string,
    coordX: number,
    coordY: number,
    coordZ: number
};

export type User = {
    username: string;
    email: string;
    collocationId: string;
    id: string;
}

export type Collocation = {
    name: string;
    addresse: string;
    createdBy: string;
    id: string;
}

export type Chore = {
    id: string;
    title: string;
    description: string;
    createdBy: string;
    createdAt: string;
    dueDate: string;
    isDone: boolean;    
}
export type Locale = 'fr' | 'en' | 'es' | 'de' | 'zh' | 'ja';
