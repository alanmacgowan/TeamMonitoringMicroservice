let connection = null;

setupConnection = () => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:44316/hubs/monitoring")
        .build();

    connection.on("ProximityDetectedNotification", (event) => {
        const statusDiv = document.getElementById("status");
        statusDiv.innerHTML = event.sourceMemberName;
    }
    );

    connection.on("finished", function () {
        connection.stop();
    }
    );

    connection.start()
        .catch(err => console.error(err.toString()));
};

setupConnection();