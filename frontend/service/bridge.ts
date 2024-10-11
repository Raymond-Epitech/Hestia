class bridge {
    url: string = "http://localhost:8080";

    seturl(new_url: string) {
        this.url = new_url;
    }

    async getTest() {
        return await fetch(this.url + "/Test").then(response => response.json());
    }

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

    async addReminder(data: any) {
        return await fetch(this.url + "/api/Reminder", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        }).then(response => response.json());
    }
}