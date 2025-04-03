export type Reminder = {
    id: string,
    createdBy: string,
    content: string,
    color: string,
    coordX: number,
    coordY: number,
    coordZ: number
};

export type Expense = {
    colocationId: string,
    createdBy: string,
    name: string,
    description: string,
    amount: number,
    category: string,
    paidBy: string
    splitType: number,
    splitBetween: string[],
    splitValues: {
        [key: string]: number
    }
    splitPercentages: {
        [key: string]: number
    }
    dateOfPayment: string,
}

export type User = {
    username: string;
    email: string;
    colocationId: string;
    id: string;
}

export type Colocation = {
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
