import { json } from "stream/consumers";
import type { Reminder, User, Colocation, Chore, Coloc, Expenseget, Expense, UserBalance, ExpenseList, Expense_Modif, refund, shoppinglist, shoppinglist_item, expenses_category, expenses_category_get } from "./type";


export class bridge {
    constructor() {
        console.log('Bridge instance created')
    }
    url: string = "http://91.134.48.124:8081";
    jwt: string = "";

    seturl(new_url: string) {
        this.url = new_url;
    }

    setjwt(new_jwt: string) {
        this.jwt = new_jwt;
    }

    getjwt() {
        console.log("jwt:", this.jwt);
        return this.jwt;
    }

    // Version section: get version

    async getVersion() {
        const response: Response = await fetch(this.url + "/api/Version");
        if (response.status == 200) {
            return await response.json();
        }
        return {};
    }

    // Reminder section: get all reminders, get reminder by ID, add reminder, update reminder, delete reminder

    async getAllReminders(id_colloc: string) {
        const response: Response = await fetch(this.url + "/api/Reminder/GetByColocation/" + id_colloc,
            {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            }
        );
        if (response.status == 200) {
            return await response.json();
        }
        return [];
    }

    async getReminderbyID(id: number) {
        const response: Response = await fetch(this.url + "/api/Reminder/" + id);
        if (response.status == 200) {
            return await response.json();
        }
        return {};
    }

    async addReminder(data: any) {
        return await fetch(this.url + "/api/Reminder", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(data)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async updateReminder(data: Reminder) {
        return await fetch(this.url + "/api/Reminder/" + data.id, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async updateReminderRange(data: [Reminder]) {
        return await fetch(this.url + "/api/Reminder/Range", {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        }).then(response => response.json());
    }

    async deleteReminder(id: string) {
        return await fetch(this.url + "/api/Reminder/" + id, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    // User section:

    async login(google_token: string) {
        return await fetch(this.url + "/api/User/Login?googleToken=" + google_token, {
            method: 'POST'
        }).then(async response => {
            if (response.status == 200) {
                return await response.json();
            } else if (response.status == 404) {
                const jsonresponse = await response.json();
                if (jsonresponse.message == "User not found") {
                    return { error: "User not found" };
                }
                return { error: "Internal server error" };
            }
            return {};
        })
    }

    async addUser(user: User, google_token: string) {
        return await fetch(this.url + "/api/User/Register?googleToken=" + google_token, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        }).then(async response => {
            if (response.status == 200) {
                return await response.json();
            }
            return {};
        })
    }

    async updateUser(user: User) {
        return await fetch(this.url + "/api/User", {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + this.jwt,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async deleteUser(user: User) {
        return await fetch(this.url + "/api/User/" + user.id, {
            method: 'DELETE',
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async getUserbyId(id: string) {
        return await fetch(this.url + "/api/User/GetById/" + id, {
            method: 'GET'
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return {}
        });
    }

    async getUserbyCollocId(collocid: string): Promise<Coloc[]> {
        return await fetch(this.url + "/api/User/GetByColocationId/" + collocid, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return []
        });
    }

    // Colocation section: 

    async addColocation(colocation: Colocation) {
        return await fetch(this.url + "/api/Colocation", {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + this.jwt,
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(colocation)
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return false;
        })
    }

    async updateColocation(colocation: Colocation) {
        return await fetch(this.url + "/api/Colocation", {
            method: 'PUT',
            headers: {
                'Content-type': 'application/json'
            },
            body: JSON.stringify(colocation)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async deleteColocation(id: string) {
        return await fetch(this.url + "/api/Colocation/" + id, {
            method: 'DELETE',
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async getAllColocation() {
        return await fetch(this.url + "/api/Colocation", {
            method: 'GET',
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }

    async getColocationById(id: string) {
        return await fetch(this.url + "/api/Colocation/" + id, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt,
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return {};
        })
    }

    // Chore section:

    async addChore(chore: any) {
        return await fetch(this.url + "/api/Chore", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(chore)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async updateChore(chore: Chore) {
        return await fetch(this.url + "/api/Chore", {
            method: 'PUT',
            headers: {
                'Content-type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(chore)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async deleteChore(id: string) {
        return await fetch(this.url + "/api/Chore/" + id, {
            method: 'DELETE',
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async getAllChore(colocationId: string) {
        return await fetch(this.url + "/api/Chore/GetByColocationId/" + colocationId, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }

    async getChoreById(id: string) {
        return await fetch(this.url + "/api/Chore/GetById/" + id, {
            method: 'GET',
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return {};
        })
    }

    async addChoreMessage(choreId: string, userId: string, message: string) {
        return await fetch(this.url + "/api/Chore/Message", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                choreId: choreId,
                createdBy: userId,
                content: message
            })
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async deleteChoreMessage(id: string) {
        return await fetch(this.url + "/api/Chore/Message/" + id, {
            method: 'DELETE',
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async getChoreMessage(choreId: string) {
        return await fetch(this.url + "/api/Chore/Message/" + choreId, {
            method: 'GET',
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }

    async addChoreUser(choreId: string, userId: string) {
        return await fetch(`${this.url}/api/Chore/Enroll?ChoreId=${choreId}&UserId=${userId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async deleteChoreUser(choreId: string, userId: string) {
        return await fetch(`${this.url}/api/Chore/Enroll?ChoreId=${choreId}&UserId=${userId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async getChoreByUser(userId: string) {
        return await fetch(`${this.url}/api/Chore/Enroll/ByUser?UserId=${userId}`, {
            method: 'GET',
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }

    async getUserEnrollChore(choreId: string): Promise<User[]> {
        return await fetch(`${this.url}/api/Chore/Enroll/ByChore?ChoreId=${choreId}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }

    // Expense section:

    async getExpenseByColocationId(colocationId: string): Promise<expenses_category_get[]> {
        return await fetch(`${this.url}/api/Expense/GetByColocationId/${colocationId}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }


    async getExpensebycategoryId(categoryId: string): Promise<Expenseget[]> {
        return await fetch(`${this.url}/api/Expense/GetByExpenseCategoryId/${categoryId}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        });
    }

    async getExpenseById(id: string): Promise<Expenseget> {
        return await fetch(`${this.url}/api/Expense/GetById/${id}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return {} as Expenseget;
        })
    }

    async addExpense(data: Expense): Promise<boolean | { error: any }> {
        return await fetch(`${this.url}/api/Expense`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(data)
        }).then(async response => {
            if (response.status == 200) {
                return true;
            }
            const errorJson = await response.json();
            console.error("Error adding expense:", errorJson);
            return { error: errorJson };
        }).catch((error) => {
            console.error("Network error:", error);
            return { error };
        });
    }

    async updateExpense(data: Expense_Modif) {
        return await fetch(`${this.url}/api/Expense`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(data)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async deleteExpense(id: string) {
        return await fetch(`${this.url}/api/Expense/${id}`, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async getBalance(colocationId: string): Promise<UserBalance> {
        return await fetch(`${this.url}/api/Expense/GetBalance/${colocationId}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }

    async updateBalance(colocationId: string): Promise<UserBalance[]> {
        return await fetch(`${this.url}/api/Expense/CalculBalance/${colocationId}`, {
            method: 'PUT',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }

    async getRefund(colocationId: string): Promise<refund[]> {
        return await fetch(`${this.url}/api/Expense/GetRefundMethods/${colocationId}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }

    async addexpensecategory(category: expenses_category) {
        return await fetch(`${this.url}/api/Expense/Category`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(category)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async updateexpensecategory(category: expenses_category) {
        return await fetch(`${this.url}/api/Expense/Category`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(category)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async deleteexpensecategory(id: string) {
        return await fetch(`${this.url}/api/Expense/Category/${id}`, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    // shoppinglist section:
    async getShoppingListByColocationId(colocationId: string): Promise<shoppinglist[]> {
        return await fetch(`${this.url}/api/ShoppingList/GetByColocationId/${colocationId}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        });
    }

    async getShoppingListById(id: string): Promise<shoppinglist> {
        return await fetch(`${this.url}/api/ShoppingList/GetById/${id}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return {} as shoppinglist;
        });
    }

    async addShoppingList(data: shoppinglist) {
        return await fetch(`${this.url}/api/ShoppingList`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(data)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async updateShoppingList(data: shoppinglist) {
        return await fetch(`${this.url}/api/ShoppingList`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(data)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async deleteShoppingList(id: string) {
        return await fetch(`${this.url}/api/ShoppingList/${id}`, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async addShoppingListItem(item: shoppinglist_item) {
        return await fetch(`${this.url}/api/ShoppingList/Item`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(item)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async updateShoppingListItem(item: shoppinglist_item) {
        return await fetch(`${this.url}/api/ShoppingList/Item`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(item)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async deleteShoppingListItem(id: string) {
        return await fetch(`${this.url}/api/ShoppingList/Item?shoppingItemId=${id}`, {
            method: 'DELETE',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    // Image section:

    async uploadImage(file: File,): Promise<string> {
        const formData = new FormData();
        formData.append('file', file, file.name);
        return await fetch(`${this.url}/images`, {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            },
            body: formData
        }).then(response => {
            if (response.status == 200) {
                return response.text();
            }
            return '';
        });
    }

    async getImage(name: string): Promise<string> {
        return await fetch(`${this.url}/images/${name}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            console.log("Image response status:", response.status);
            console.log("Image response:", response);
            if (response.status == 200) {
                return response.blob().then(blob => {
                    return URL.createObjectURL(blob);
                });
            }
            return '';
        });
    }

}
