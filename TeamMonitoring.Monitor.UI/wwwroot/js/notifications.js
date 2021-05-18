let connection = null;
let hubUrl = $("#HubUrl").val();

setupConnection = () => {
    connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl + "/hubs/monitoring")
        .build();

    connection.on("ProximityDetectedNotification", (event) => {
        toastr.success('New Proximity detected: ' + event.sourceMemberName + ' - ' + event.targetMemberName);
        const statusDiv = document.getElementById("status");
        statusDiv.innerHTML += '<div> Team: ' + event.teamName + ' - Source: '  + event.sourceMemberName + ' - Target: ' + event.targetMemberName + '</div>';       
    }
    );

    connection.start()
        .catch(err => console.error(err.toString()));
};

setupConnection();