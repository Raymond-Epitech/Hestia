import { HubConnectionBuilder, HubConnection, LogLevel, HttpTransportType } from '@microsoft/signalr'
import { useUserStore } from '~/store/user'

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

    let resolveReady: () => void
    const ready = new Promise<void>((resolve) => {
        resolveReady = resolve
    })

    connection.serverTimeoutInMilliseconds = 60000;
    connection.keepAliveIntervalInMilliseconds = 20000;
    connection.onclose(err => console.error("SignalR connection closed", err));

    connection.onreconnected(async () => {
        console.log("SignalR reconnected")

        const userStore = useUserStore()
        if (userStore.user?.colocationId) {
            try {
                await connection.invoke(
                    "JoinColocationGroup",
                    userStore.user.colocationId
                )
                console.log("Rejoined colocation group")
            } catch (err) {
                console.error("Failed to rejoin group", err)
            }
        }
    })


    const startConnection = async () => {
        try {
            await connection.start()
            console.log('SignalR connected')
            resolveReady()
        } catch (err) {
            console.error('SignalR Connection Error:', err)
            setTimeout(startConnection, 5000)
        }
    }

    startConnection()

    return {
        provide: {
            signalr: connection,
            signalrReady: ready
        }
    }
})
