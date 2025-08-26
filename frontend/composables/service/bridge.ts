import { json } from "stream/consumers";
import type { Reminder, User, Colocation, Chore, Coloc, Expenseget, Expense, UserBalance, ExpenseList, Expense_Modif, refund, shoppinglist, shoppinglist_item, expenses_category, expenses_category_get, message } from "./type";
import { Capacitor } from '@capacitor/core'
import { Filesystem, Directory } from '@capacitor/filesystem';

function isNative() {
    console.log("Capacitor.isNativePlatform():", Capacitor.isNativePlatform());
    return Capacitor.isNativePlatform()
}

export class bridge {
    constructor() {
        console.log('Bridge instance created')
    }
    url: string = "https://hestiaapp.org";
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

    async getAllReminders(id_colloc: string): Promise<Reminder[]> {
        const response: Response = await fetch(this.url + "/api/Reminder?colocationId=" + id_colloc,
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
        const formData = new FormData();
        formData.append('ColocationId', data.colocationId || '');
        formData.append('CreatedBy', data.createdBy);
        formData.append('CoordX', String(data.coordX));
        formData.append('CoordY', String(data.coordY));
        formData.append('CoordZ', String(data.coordZ));
        formData.append('ReminderType', String(data.reminderType));
        formData.append('Content', data.content);
        formData.append('Color', data.color);
        if (data.image) {
            formData.append('File', data.image, data.image.name);
        } else {
            formData.append('File', '');
        }
        formData.append('ShoppingListName', data.shoppingListName);
        formData.append("PollInput.Title", data.pollInput.title);
        formData.append("PollInput.Description", data.pollInput.description);
        formData.append("PollInput.ExpirationDate", data.pollInput.expirationdate);
        formData.append("PollInput.IsAnonymous", String(data.pollInput.isanonymous));
        formData.append("PollInput.AllowMultipleChoices", String(data.pollInput.allowmultiplechoice));
        return await fetch(this.url + "/api/Reminder", {
            method: 'POST',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            },
            body: formData
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

    // Shopping list for reminder section:

    async getReminderShoppingList(id: string) {
        return await fetch(this.url + "/api/Reminder/ShoppingList?reminderId=" + id, {
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

    async addReminderShoppingListItem(item: any) {
        return await fetch(this.url + "/api/Reminder/ShoppingList", {
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

    async updateReminderShoppingListItem(item: any) {
        return await fetch(this.url + "/api/Reminder/ShoppingList", {
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

    async deleteReminderShoppingListItem(id: string) {
        return await fetch(this.url + "/api/Reminder/ShoppingList/" + id, {
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

    // Poll for reminder section:

    async getReminderPoll(id: string) {
        return await fetch(this.url + "/api/Reminder/PollVote?reminderId=" + id, {
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

    async addReminderPollVote(vote: any) {
        return await fetch(this.url + "/api/Reminder/PollVote", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify(vote)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async deleteReminderPollVote(Id: string, userId: string) {
        return await fetch(this.url + "/api/Reminder/PollVote/id" + Id, {
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


    // Reaction for reminder section:

    async addReactionReminder(reminderId: string, userId: string, reaction: string) {
        return await fetch(this.url + "/api/Reminder/Reminder/Reactions", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify({ reminderId: reminderId, userId: userId, type: reaction })
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async deleteReactionReminder(reminderId: string, userId: string) {
        return await fetch(this.url + "/api/Reminder/Reminder/Reactions", {
            method: 'DELETE',
             headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + this.jwt
            },
            body: JSON.stringify({ reminderId: reminderId, userId: userId })
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        });
    }

    async getReactionsReminder(reminderId: string) {
        return await fetch(this.url + "/api/Reminder/Reminder/Reactions?reminderId=" + reminderId, {
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

    // User section:

    async login(google_token: string, fcm_token: string) {
        const options: RequestInit = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        };
        if (fcm_token && fcm_token !== "") {
            options.body = JSON.stringify({ fcmToken: fcm_token });
        }
        return await fetch(this.url + "/api/User/Login?googleToken=" + google_token, options)
            .then(async response => {
                if (response.status == 200) {
                    return await response.json();
                } else if (response.status == 404) {
                    const jsonresponse = await response.json();
                    if (jsonresponse.message == "User not found") {
                        return { error: "User not found" };
                    }
                    return { error: "Internal server error" };
                } else if (response.status == 422) {
                    const jsonresponse = await response.json();
                    if (jsonresponse.message == "Invalid json body") {
                        return { error: "Invalid json body" };
                    }
                }
                return {};
            });
    }

    async logout(userId: string, fcm_token: string) {
        if (!fcm_token || fcm_token === "") {
            return { error: "FCM token is required (could be absent on browser)" };
        }
        return await fetch(this.url + "/api/User/Logout", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ userId: userId, fcmToken: fcm_token })
        }).then(async response => {
            if (response.status == 200) {
                return await response.json();
            } else if (response.status == 500) {
                const jsonresponse = await response.json();
                if (jsonresponse.message == "Internal server error") {
                    return { error: "Internal server error" };
                }
            }
            return {};
        });
    }

    async addUser(user: User, google_token: string, fcm_token: string) {
        return await fetch(this.url + "/api/User/Register?googleToken=" + google_token, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username: user.username, colocationId: user.colocationId, fcmToken: fcm_token })
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
                'Authorization': 'Bearer ' + 
                this.jwt,
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

    async getImagetocache(name: string): Promise<string> {
        const url = `${this.url}/api/Reminder/images/${name}`;
        if (await this.getImagefromcache(name) != null) {
            return 'OK';
        }
        return await fetch(url, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(async response => {
            if (response.status == 200) {
                const blob = await response.blob();
                if (isNative()) {
                    await new Promise<void>((resolve, reject) => {
                        const reader = new FileReader();
                        reader.onloadend = async () => {
                            try {
                                const base64Data = (reader.result as string).split(',')[1];
                                await Filesystem.writeFile({
                                    path: name,
                                    data: base64Data,
                                    directory: Directory.Data
                                });
                                resolve();
                            } catch (err) {
                                reject(err);
                            }
                        };
                        reader.onerror = reject;
                        reader.readAsDataURL(blob);
                    });
                } else {
                    const cache = await caches.open('images-cache');
                    await cache.put(url, new Response(blob));
                }
                return 'OK';
            }
            return 'KO';
        });
    }

    async getImagefromcache(name: string): Promise<string | null> {
        const url = `${this.url}/api/Reminder/images/${name}`;
        if (isNative()) {
            // Mobile : lecture depuis le disque
            try {
                const result = await Filesystem.readFile({
                    path: name,
                    directory: Directory.Data
                })
                return `data:image/jpeg;base64,${result.data}`
            } catch {
                return null
            }
        } else {
            // Web : lecture depuis Cache Storage API
            const cache = await caches.open('images-cache')
            const response = await cache.match(url)
            if (!response) return null
            const blob = await response.blob()
            return URL.createObjectURL(blob)
        }
    }

    // Message section:
    async getMessageByColocationId(colocationId: string): Promise<message[]> {
        return await fetch(`${this.url}/api/Message?colocationId=${colocationId}`, {
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

    async addMessage(data: message): Promise<boolean> {
        return await fetch(`${this.url}/api/Message`, {
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

    async deleteMessage(id: string): Promise<boolean> {
        return await fetch(`${this.url}/api/Message/${id}`, {
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

    async getMessageById(id: string): Promise<message> {
        return await fetch(`${this.url}/api/Message/${id}`, {
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + this.jwt
            }
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return {} as message;
        });
    }

    async updateMessage(data: message): Promise<boolean> {
        return await fetch(`${this.url}/api/Message`, {
            method: 'PUT',
            headers: {
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
}
