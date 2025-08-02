import { HubConnectionBuilder, HubConnection, LogLevel, HttpTransportType } from '@microsoft/signalr'

export default defineNuxtPlugin(() => {
    const config = useRuntimeConfig()

    const connection = new HubConnectionBuilder()
        .withUrl("https://hestiaapp.org/hestiaHub", {
            skipNegotiation: true,
            transport: HttpTransportType.WebSockets,
            withCredentials: true
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build()

    const startConnection = async () => {
        try {
            await connection.start()
            console.log('SignalR connected')
        } catch (err) {
            console.error('SignalR Connection Error:', err)
            setTimeout(startConnection, 5000)
        }
    }

    startConnection()

    return {
        provide: {
            signalr: connection
        }
    }
})
