export type Reminder = {
    colocationId?: string,
    id: string,
    createdBy: string,
    content: string,
    isImage: boolean,
    image?: File | null,
    linkToPP: string,
    color: string,
    coordX: number,
    coordY: number,
    coordZ: number
};

export type Coloc = {
    id: string,
    username: string,
    email: string,
    colocationId: string
}

export type Expense = {
    colocationId: string,
    expenseCategoryId: string,
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

export type Expense_Modif = {
    id: string,
    colocationId: string,
    expenseCategoryId: string,
    name: string,
    description: string,
    amount: number,
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

export type Expenseget = {
    id: string,
    createdAt: string,
    createdBy: string,
    colocationId: string,
    name: string,
    description: string,
    amount: number,
    paidBy: string
    splitBetween: {
        [key: string]: number
    },
    splitType: number,
    dateOfPayment: string,
    expenseCategoryName: string,
    expenseCategoryId: string,
}

export type ExpenseList = {
    category: string,
    totalAmount: number,
    expenses: Expenseget[]
}

export type UserBalance = {
    [key: string]: number
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
    createdBy: string;
    createdAt: string;
    colocationId: string;
    dueDate: string;
    title: string;
    description: string;
    isDone: boolean;
    enrolledUsers: string[];
}

export type UpdateChore = {
    id: string;
    colocationId: string;
    dueDate: string;
    title: string;
    description: string;
    isDone: boolean;
    enrolled: string[];
}

export type Locale = 'fr' | 'en' | 'es' | 'de' | 'zh' | 'ja';

export type refund = {
    from: string,
    fromUsername: string,
    to: string,
    toUsername: string,
    amount: number,
}

export type shoppinglist = {
    id?: string,
    createdBy?: string,
    colocationId?: string,
    name: string
    shoppingItems?: shoppinglist_item[],
}

export type shoppinglist_item = {
    id?: string,
    createdBy: string,
    shoppinglistId: string,
    name: string,
    isChecked: boolean
}

export type expenses_category = {
    id?: string,
    colocationId?: string,
    name: string,
}

export type expenses_category_get = {
    id: string,
    name: string,
    totalAmount: number,
}

export type message = {
    id?: string,
    colocationId: string,
    content: string,
    sendBy: string,
    sendAt?: string,
}

export type SignalRClient = {
    on(event: string, callback: (data: any) => void): void;
}