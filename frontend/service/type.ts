export type Reminder = {
    id: string;
    color: string;
    content: string;
    createdBy: string;
    createdAt: string;
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