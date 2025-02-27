import type { Reminder, User, Collocation, Chore } from "./type";

export class bridge {
    url: string = "http://91.134.48.124:8080";
    jwt: string = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMTE1OTE1ODU5OTA5MDMxOTM3MzAiLCJlbWFpbCI6ImJlbmphbWluYm91cmV6QGdtYWlsLmNvbSIsIm5hbWUiOiJCZW5qYW1pbiIsInBpY3R1cmUiOiJodHRwczovL2xoMy5nb29nbGV1c2VyY29udGVudC5jb20vYS9BQ2c4b2NMbnpUTlJNOF9ERTBYWWVhcGdGWlNjNlUxTC11NXQtRzJxZzA1MVF3bllYLWZnV2EwPXM5Ni1jIiwiaXNzIjoiaGVzdGlhLWFwcCIsImF1ZCI6Imhlc3RpYS1tb2JpbGUifQ.E9bPI5VXWu5tH1Y0zq8_YQhZfkf962B2Ohnd_2aqMeg";

    seturl(new_url: string) {
        this.url = new_url;
    }

    setjwt(new_jwt: string) {
        this.jwt = new_jwt;
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
        const response: Response = await fetch(this.url + "/api/Reminder/GetByCollocation/" + id_colloc,
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
        return await fetch(this.url + "/Login?googleToken=" + google_token, {
            method: 'POST'
        }).then(response => {
            if (response.status == 200) {
                
                return true;
            }
            return false;
        })
    }

    async addUser(user: User, google_token: string) {
        return await fetch(this.url + "/Register?googleToken=" + google_token, {
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

    // Chore section:

    async addChore(chore: Chore) {
        return await fetch(this.url + "/api/Chore", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
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
                'Content-type': 'application/json'
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

    async getAllChore(collocationId: string) {
        return await fetch(this.url + "/api/Chore/GetByCollocationId/" + collocationId, {
            method: 'GET',
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
                'Content-Type': 'application/json'
            }
        }).then(response => {
            if (response.status == 200) {
                return true;
            }
            return false;
        })
    }

    async deleteChoreUser(choreId: string, userId: string) {
        return await fetch(`${this.url}/api/Chore/Unenroll?ChoreId=${choreId}&UserId=${userId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
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

    async getUserEnrollChore(choreId: string) {
        return await fetch(`${this.url}/api/Chore/Enroll/ByChore?ChoreId=${choreId}`, {
            method: 'GET',
        }).then(response => {
            if (response.status == 200) {
                return response.json();
            }
            return [];
        })
    }
}
