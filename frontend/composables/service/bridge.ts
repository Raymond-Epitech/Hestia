import { json } from "stream/consumers";
import type { Reminder, User, UserInfo, Colocation, Chore, Coloc, Expenseget, Expense, UserBalance, ExpenseList, Expense_Modif, refund, shoppinglist, shoppinglist_item, expenses_category, expenses_category_get, message, UpdateChore, ReminderItem, Locale } from "./type";
import { Capacitor } from '@capacitor/core'
import { Filesystem, Directory } from '@capacitor/filesystem';

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
        try {
            const response = await fetch(this.url + "/api/Reminder?colocationId=" + id_colloc, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getAllReminders', err);
            throw err;
        }
    }

    async getReminderbyID(id: number): Promise<Reminder> {
        try {
            const response = await fetch(this.url + "/api/Reminder/" + id, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getReminderbyID', err);
            throw err;
        }
    }

    async addReminder(data: any): Promise<string> {
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
        formData.append('ShoppingListName', data.shoppinglistName);
        formData.append("PollInput.Title", data.pollInput.title);
        formData.append("PollInput.Description", data.pollInput.description);
        formData.append("PollInput.ExpirationDate", data.pollInput.expirationdate);
        formData.append("PollInput.IsAnonymous", String(data.pollInput.isanonymous));
        formData.append("PollInput.AllowMultipleChoices", String(data.pollInput.allowmultiplechoice));
        try {
            const response = await fetch(this.url + "/api/Reminder", {
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: formData
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addReminder', err);
            throw err;
        }
    }

    async updateReminder(data: any, id: string): Promise<boolean> {
        data.id = id;
        try {
            const response = await fetch(this.url + "/api/Reminder/", {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error updateReminder', err);
            throw err;
        }
    }

    async updateReminderRange(data: Reminder[]): Promise<string> {
        try {
            const response = await fetch(this.url + "/api/Reminder/Range", {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error updateReminderRange', err);
            throw err;
        }
    }

    async deleteReminder(id: string): Promise<boolean> {
        try {
            const response = await fetch(this.url + "/api/Reminder/" + id, {
                method: 'DELETE',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteReminder', err);
            throw err;
        }
    }

    // Shopping list for reminder section:

    async getReminderShoppingList(id: string) {
        try {
            const response = await fetch(this.url + "/api/Reminder/ShoppingList?reminderId=" + id, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getReminderShoppingList', err);
            throw err;
        }
    }

    async addReminderShoppingListItem(item: any) {
        try {
            const response = await fetch(this.url + "/api/Reminder/ShoppingList", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(item)
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addReminderShoppingListItem', err);
            throw err;
        }
    }

    async updateReminderShoppingListItem(item: ReminderItem) {
        try {
            const response = await fetch(this.url + "/api/Reminder/ShoppingList", {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify({
                    createdBy: item.createdBy,
                    id: item.id,
                    isChecked: item.isChecked,
                    name: item.name
                })
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error updateReminderShoppingListItem', err);
            throw err;
        }
    }

    async deleteReminderShoppingListItem(id: string) {
        try {
            const response = await fetch(this.url + "/api/Reminder/ShoppingList/" + id, {
                method: 'DELETE',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteReminderShoppingListItem', err);
            throw err;
        }
    }

    // Poll for reminder section:

    async getReminderPoll(id: string) {
        try {
            const response = await fetch(this.url + "/api/Reminder/PollVote?reminderId=" + id, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getReminderPoll', err);
            throw err;
        }
    }

    async addReminderPollVote(vote: any): Promise<boolean> {
        try {
            const response = await fetch(this.url + "/api/Reminder/PollVote", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(vote)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addReminderPollVote', err);
            throw err;
        }
    }

    async deleteReminderPollVote(Id: string, userId: string): Promise<boolean> {
        try {
            const response = await fetch(this.url + "/api/Reminder/PollVote/id" + Id, {
                method: 'DELETE',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteReminderPollVote', err);
            throw err;
        }
    }


    // Reaction for reminder section:

    async addReactionReminder(reminderId: string, userId: string, reaction: string): Promise<boolean> {
        try {
            const response = await fetch(this.url + "/api/Reminder/Reactions", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify({ reminderId: reminderId, userId: userId, type: reaction })
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addReactionReminder', err);
            throw err;
        }
    }

    async deleteReactionReminder(reminderId: string, userId: string): Promise<boolean> {
        try {
            const response = await fetch(this.url + "/api/Reminder/Reactions", {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify({ reminderId: reminderId, userId: userId })
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteReactionReminder', err);
            throw err;
        }
    }

    async getReactionsReminder(reminderId: string) {
        try {
            const response = await fetch(this.url + "/api/Reminder/Reactions?reminderId=" + reminderId, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getReactionsReminder', err);
            throw err;
        }
    }

    // User section:

    async login(google_token: string, fcm_token: string) {
        try {
            const options: RequestInit = {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            };
            if (fcm_token && fcm_token !== "") {
                options.body = JSON.stringify({ fcmToken: fcm_token });
            }
            const response = await fetch(this.url + "/api/User/Login?googleToken=" + google_token, options);
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error login', err);
            throw err;
        }
    }

    async logout(userId: string, fcm_token: string) {
        if (!fcm_token || fcm_token === "") {
            return { error: "FCM token is required (could be absent on browser)" };
        }
        try {
            const response = await fetch(this.url + "/api/User/Logout", {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ userId: userId, fcmToken: fcm_token })
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error logout', err);
            throw err;
        }
    }

    async addUser(user: User, google_token: string, fcm_token: string) {
        try {
            const response = await fetch(this.url + "/api/User/Register?googleToken=" + google_token, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username: user.username, colocationId: user.colocationId, fcmToken: fcm_token })
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addUser', err);
            throw err;
        }
    }

    async updateUser(user: UserInfo) {
        try {
            const response = await fetch(this.url + "/api/User", {
                method: 'PUT',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(user)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error updateUser', err);
            throw err;
        }
    }

    async deleteUser(user: User) {
        try {
            const response = await fetch(this.url + "/api/User/" + user.id, {
                method: 'DELETE',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteUser', err);
            throw err;
        }
    }

    async getUserbyId(id: string) {
        try {
            const response = await fetch(this.url + "/api/User/GetById/" + id, {
                method: 'GET',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getUserbyId', err);
            throw err;
        }
    }

    async getUserbyCollocId(collocid: string): Promise<Coloc[]> {
        try {
            const response = await fetch(this.url + "/api/User/GetByColocationId/" + collocid, {
                method: 'GET',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getUserbyCollocId', err);
            throw err;
        }
    }

    async getLanguage(id: string): Promise<Locale> {
        try {
            const response = await fetch(`${this.url}/api/User/Language/${id}`, {
                method: 'GET',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.text() as Locale;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getLanguage', err);
            throw err;
        }
    }

    async updateLanguage(lang: string, id: string): Promise<boolean> {
        try {
            const response = await fetch(`${this.url}/api/User/Language`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json-patch+json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify({
                    userId: id,
                    language: lang
                })
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error updateLanguage', err);
            throw err;
        }
    }

    // Colocation section: 

    async addColocation(colocation: Colocation) {
        try {
            const response = await fetch(`${this.url}/api/Colocation`, {
                method: 'POST',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt,
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(colocation)
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addColocation', err);
            throw err;
        }
    }

    async updateColocation(colocation: Colocation) {
        try {
            const response = await fetch(`${this.url}/api/Colocation`, {
                method: 'PUT',
                headers: {
                    'Content-type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(colocation)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error updateColocation', err);
            throw err;
        }
    }

    async deleteColocation(id: string) {
        try {
            const response = await fetch(`${this.url}/api/Colocation/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteColocation', err);
            throw err;
        }
    }

    async getAllColocation() {
        try {
            const response = await fetch(`${this.url}/api/Colocation`, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getAllColocation', err);
            throw err;
        }
    }

    async getColocationById(id: string) {
        try {
            const response = await fetch(`${this.url}/api/Colocation/${id}`, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt,
                }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getColocationById', err);
            throw err;
        }
    }

    // Chore section:

    async addChore(chore: any) {
        try {
            const response = await fetch(this.url + "/api/Chore", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(chore)
            });
            if (response.ok) return true;
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addChore', err);
            throw err;
        }
    }

    async updateChore(chore: UpdateChore) {
        try {
            const response = await fetch(this.url + "/api/Chore", {
                method: 'PUT',
                headers: {
                    'Content-type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(chore)
            });
            if (response.ok) return true;
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error updateChore', err);
            throw err;
        }
    }

    async deleteChore(id: string) {
        try {
            const response = await fetch(this.url + "/api/Chore/" + id, {
                method: 'DELETE',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) return true;
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteChore', err);
            throw err;
        }
    }

    async getAllChore(colocationId: string) {
        try {
            const response = await fetch(this.url + "/api/Chore/GetByColocationId/" + colocationId, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) return await response.json();
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getAllChore', err);
            throw err;
        }
    }

    async getChoreById(id: string) {
        try {
            const response = await fetch(this.url + "/api/Chore/GetById/" + id, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) return await response.json();
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getChoreById', err);
            throw err;
        }
    }

    async addChoreMessage(choreId: string, userId: string, message: string) {
        try {
            const response = await fetch(this.url + "/api/Chore/Message", {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify({
                    choreId: choreId,
                    createdBy: userId,
                    content: message
                })
            });
            if (response.ok) return true;
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addChoreMessage', err);
            throw err;
        }
    }

    async deleteChoreMessage(id: string) {
        try {
            const response = await fetch(this.url + "/api/Chore/Message/" + id, {
                method: 'DELETE',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) return true;
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteChoreMessage', err);
            throw err;
        }
    }

    async getChoreMessage(choreId: string) {
        try {
            const response = await fetch(this.url + "/api/Chore/Message/" + choreId, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) return await response.json();
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getChoreMessage', err);
            throw err;
        }
    }

    async addChoreUser(choreId: string, userId: string) {
        try {
            const response = await fetch(`${this.url}/api/Chore/Enroll?ChoreId=${choreId}&UserId=${userId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) return true;
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addChoreUser', err);
            throw err;
        }
    }

    async deleteChoreUser(choreId: string, userId: string) {
        try {
            const response = await fetch(`${this.url}/api/Chore/Enroll?ChoreId=${choreId}&UserId=${userId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) return true;
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteChoreUser', err);
            throw err;
        }
    }

    async getChoreByUser(userId: string) {
        try {
            const response = await fetch(`${this.url}/api/Chore/Enroll/ByUser?UserId=${userId}`, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) return await response.json();
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getChoreByUser', err);
            throw err;
        }
    }

    async getUserEnrollChore(choreId: string): Promise<User[]> {
        try {
            const response = await fetch(`${this.url}/api/Chore/Enroll/ByChore?ChoreId=${choreId}`, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) return await response.json();
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getUserEnrollChore', err);
            throw err;
        }
    }

    // Expense section:

    async getExpenseByColocationId(colocationId: string): Promise<expenses_category_get[]> {
        try {
            const response = await fetch(`${this.url}/api/Expense/GetByColocationId/${colocationId}`, {
                method: 'GET',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async getExpensebycategoryId(categoryId: string): Promise<Expenseget[]> {
        try {
            const response = await fetch(`${this.url}/api/Expense/GetByExpenseCategoryId/${categoryId}`, {
                method: 'GET',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async getExpenseById(id: string): Promise<Expenseget> {
        try {
            const response = await fetch(`${this.url}/api/Expense/GetById/${id}`, {
                method: 'GET',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async addExpense(data: Expense, isRecurring: boolean, dayOfTheRecursion: string): Promise<boolean> {
        const newdata = { ...data, isRecurring, dayOfTheRecursion };
        try {
            const response = await fetch(`${this.url}/api/Expense`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(newdata)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async updateExpense(data: Expense_Modif): Promise<boolean> {
        try {
            const response = await fetch(`${this.url}/api/Expense`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async deleteExpense(id: string): Promise<boolean> {
        try {
            const response = await fetch(`${this.url}/api/Expense/${id}`, {
                method: 'DELETE',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async getBalance(colocationId: string): Promise<UserBalance> {
        try {
            const response = await fetch(`${this.url}/api/Expense/GetBalance/${colocationId}`, {
                method: 'GET',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async updateBalance(colocationId: string): Promise<UserBalance[]> {
        try {
            const response = await fetch(`${this.url}/api/Expense/CalculBalance/${colocationId}`, {
                method: 'PUT',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async getRefund(colocationId: string): Promise<refund[]> {
        try {
            const response = await fetch(`${this.url}/api/Expense/GetRefundMethods/${colocationId}`, {
                method: 'GET',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async addexpensecategory(category: expenses_category): Promise<boolean> {
        try {
            const response = await fetch(`${this.url}/api/Expense/Category`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(category)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async updateexpensecategory(category: expenses_category): Promise<boolean> {
        try {
            const response = await fetch(`${this.url}/api/Expense/Category`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(category)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    async deleteexpensecategory(id: string): Promise<boolean> {
        try {
            const response = await fetch(`${this.url}/api/Expense/Category/${id}`, {
                method: 'DELETE',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error', err);
            throw err;
        }
    }

    // shoppinglist section:
    async getShoppingListByColocationId(colocationId: string): Promise<shoppinglist[]> {
        try {
            const response = await fetch(`${this.url}/api/ShoppingList/GetByColocationId/${colocationId}`, {
                method: 'GET',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getShoppingListByColocationId', err);
            throw err;
        }
    }

    async getShoppingListById(id: string): Promise<shoppinglist> {
        try {
            const response = await fetch(`${this.url}/api/ShoppingList/GetById/${id}`, {
                method: 'GET',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getShoppingListById', err);
            throw err;
        }
    }

    async addShoppingList(data: shoppinglist) {
        try {
            const response = await fetch(`${this.url}/api/ShoppingList`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addShoppingList', err);
            throw err;
        }
    }

    async updateShoppingList(data: shoppinglist) {
        try {
            const response = await fetch(`${this.url}/api/ShoppingList`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error updateShoppingList', err);
            throw err;
        }
    }

    async deleteShoppingList(id: string) {
        try {
            const response = await fetch(`${this.url}/api/ShoppingList/${id}`, {
                method: 'DELETE',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteShoppingList', err);
            throw err;
        }
    }

    async addShoppingListItem(item: shoppinglist_item) {
        try {
            const response = await fetch(`${this.url}/api/ShoppingList/Item`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(item)
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addShoppingListItem', err);
            throw err;
        }
    }

    async updateShoppingListItem(item: shoppinglist_item) {
        try {
            const response = await fetch(`${this.url}/api/ShoppingList/Item`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(item)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error updateShoppingListItem', err);
            throw err;
        }
    }

    async deleteShoppingListItem(id: string) {
        try {
            const response = await fetch(`${this.url}/api/ShoppingList/Item?shoppingItemId=${id}`, {
                method: 'DELETE',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteShoppingListItem', err);
            throw err;
        }
    }

    // Image section:

    async deleteImage(name: string): Promise<boolean> {
        try {
            const response = await fetch(`${this.url}/Images/${name}`, {
                method: 'DELETE',
                headers: { 'Authorization': 'Bearer ' + this.jwt }
            });
            if (response.ok) {
                if (Capacitor.getPlatform() !== 'web') {
                    await Filesystem.deleteFile({
                        path: name,
                        directory: Directory.Data
                    });
                } else {
                    const cache = await caches.open('images-cache');
                    await cache.delete(`${this.url}/api/Image/${name}`);
                }
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteImage', err);
            throw err;
        }
    }

    async getImagetocache(name: string): Promise<string> {
        const url = `${this.url}/api/Image/${name}`;
        if (await this.getImagefromcache(name) != null) {
            return 'OK';
        }
        try {
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.status == 200) {
                const blob = await response.blob();
                if (Capacitor.getPlatform() !== 'web') {
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
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getImagetocache', err);
            throw err;
        }
    }

    async getImagefromcache(name: string): Promise<string | null> {
        const url = `${this.url}/api/Image/${name}`;
        if (Capacitor.getPlatform() !== 'web') {
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
        try {
            const response = await fetch(`${this.url}/api/Message?colocationId=${colocationId}`, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getMessageByColocationId', err);
            throw err;
        }
    }

    async addMessage(data: message): Promise<boolean> {
        try {
            const response = await fetch(`${this.url}/api/Message`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error addMessage', err);
            throw err;
        }
    }

    async deleteMessage(id: string): Promise<boolean> {
        try {
            const response = await fetch(`${this.url}/api/Message/${id}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error deleteMessage', err);
            throw err;
        }
    }

    async getMessageById(id: string): Promise<message> {
        try {
            const response = await fetch(`${this.url}/api/Message/${id}`, {
                method: 'GET',
                headers: {
                    'Authorization': 'Bearer ' + this.jwt
                }
            });
            if (response.ok) {
                return await response.json();
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error getMessageById', err);
            throw err;
        }
    }

    async updateMessage(data: message): Promise<boolean> {
        try {
            const response = await fetch(`${this.url}/api/Message`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': 'Bearer ' + this.jwt
                },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                return true;
            }
            const errBody = await response.json();
            throw { status: response.status, body: errBody };
        } catch (err) {
            console.error('Network / fetch error updateMessage', err);
            throw err;
        }
    }
}
