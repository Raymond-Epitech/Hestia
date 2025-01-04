import type { Reminder, User, Collocation } from "./type";

export class bridge {
    url: string = "http://localhost:8080";

    seturl(new_url: string) {
        this.url = new_url;
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
        const response: Response = await fetch(this.url + "/api/Reminder/GetByCollocation/" + id_colloc);
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

    async addReminder(data: Reminder) {
        return await fetch(this.url + "/api/Reminder", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        }).then(response => response.json());
    }

    async updateReminder(data: Reminder) {
        return await fetch(this.url + "/api/Reminder/" + data.id, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        }).then(response => response.json());
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
            method: 'DELETE'
        }).then(response => response.json());
    }

    // User section:

    async login(client_id: string, google_token: string) {
        return await fetch(this.url + "/Login?googleToken=" + google_token + "&clientId=" + client_id, {
            method: 'POST'
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async addUser(user: User) {
        return await fetch(this.url + "/api/User", {
            method: 'POST',
            headers: {
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

    async updateUser(user: User) {
        return await fetch(this.url + "/api/User", {
            method: 'PUT',
            headers: {
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

    async getUserbyCollocId(collocid: string) {
        return await fetch(this.url + "/api/User/GetByCollocationId/" + collocid, {
            method: 'GET'
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return []
        });
    }

    // collocation section: 

    async addCollocation(collocation: Collocation) {
        return await fetch(this.url + "/api/Collocation", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(collocation)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async updateCollocation(collocation: Collocation) {
        return await fetch(this.url + "/api/Collocation", {
            method: 'PUT',
            headers: {
                'Content-type': 'application/json'
            },
            body: JSON.stringify(collocation)
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async deleteCollocation(id: string) {
        return await fetch(this.url + "/api/Collocation/" + id, {
            method: 'DELETE',
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async getAllCollocation() {
        return await fetch(this.url + "/api/Collocation", {
            method: 'GET',
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }

    async getCollocationById(id: string) {
        return await fetch(this.url + "/api/Collocation/" + id, {
            method: 'GET',
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return {};
        })
    }
}