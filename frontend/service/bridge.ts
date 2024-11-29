import type { Reminder } from "./type";

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

    async getAllReminders() {
        const response: Response = await fetch(this.url + "/api/Reminder");
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
    //here data is an any type because the back is not expecting an ID, might need to be changed
    async addReminder(data: any) {
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

    async deleteReminder(id: string) {
        return await fetch(this.url + "/api/Reminder/" + id, {
            method: 'DELETE'
        }).then(response => response.json());
    }
}