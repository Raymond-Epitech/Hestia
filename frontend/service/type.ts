export type Reminder = {
    id: number,
    createdBy: string,
    content: string,
    color: string,
    coordX: number,
    coordY: number,
    coordZ: number
};

export type Task = {
    id: string,
    createdBy: string,
    createdAt: string,
    dueDate: string,
    title: string,
    description: string,
    isDone: Boolean
}