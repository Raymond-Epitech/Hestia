export type Reminder = {
    id: string,
    createdBy: string,
    content: string,
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

export type Expenseget = {
    id:string,
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
    title: string;
    description: string;
    createdBy: string;
    createdAt: string;
    dueDate: string;
    isDone: boolean;
}

export type Locale = 'fr' | 'en' | 'es' | 'de' | 'zh' | 'ja';

export type refund = {
    from: string,
    to: string,
    amount: number,
}

export type shoppinglist = {
    id?: string,
    createdBy?: string,
    colocationId?: string,
    name: string
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
