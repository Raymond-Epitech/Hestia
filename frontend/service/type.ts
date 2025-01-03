export type Reminder = {
    id: string;
    color: string;
    content: string;
    createdBy: string;
    createdAt: string;
};

export type User = {
    username: string,
    email: string,
    collocationId: string,
    id: string
}