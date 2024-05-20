

    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/chat")
        .build();


    let CurrentUserId = "";
    let userName = "";

    async function startChat() {
        let groupId = document.getElementById("groupId").value;
        userName = document.getElementById("userName").value;
        CurrentUserId = document.getElementById("userId").value;


        const currentTime = new Date(); // Получаем текущее время       

        // Получаем строку времени в формате "hh:mm:ss"
        const timeString = currentTime.toLocaleTimeString('ru-RU', { hour12: false });

        // Получаем строку даты в формате "dd-MM-yyyy"
        const dateString = currentTime.toLocaleDateString('ru-RU');

        // Объединяем время и дату в нужном формате
        const formattedTime = `${timeString} ${dateString}`;
       



        //const user = document.getElementById("userInput").value;
        if (hubConnection.state === signalR.HubConnectionState.Disconnected) {
            await hubConnection.start();
            //await hubConnection.invoke("leaveChat", groupId);
            await hubConnection.invoke("AddToGroup", groupId);
            await hubConnection.invoke("SendToGroup", {
                groupId: groupId,
                message: "inviteGroup",
                UserSenderId: CurrentUserId,
                UserSenderName: userName,
                SendTime: formattedTime
            });
        }
        else {
            //await hubConnection.invoke("leaveChat", groupId);
            await hubConnection.invoke("AddToGroup", groupId);
            await hubConnection.invoke("SendToGroup", {
                groupId: groupId,
                message: "inviteGroup",
                UserSenderId: CurrentUserId,
                UserSenderName: userName,
                SendTime: formattedTime
            });
        }
    }
    


    async function sendMessage() {
        let groupId = document.getElementById("groupId").value;
        const message = document.getElementById("message").value;

        const currentTime = new Date(); // Получаем текущее время

        // Получаем строку времени в формате "hh:mm:ss"
        const timeString = currentTime.toLocaleTimeString('ru-RU', { hour12: false });

        // Получаем строку даты в формате "dd-MM-yyyy"
        const dateString = currentTime.toLocaleDateString('ru-RU');

        // Объединяем время и дату в нужном формате
        const formattedTime = `${timeString} ${dateString}`;
        


        await hubConnection.invoke("SendToGroup", {
            groupId: groupId,
            message: message,
            UserSenderId: CurrentUserId,
            UserSenderName: userName,
            SendTime: formattedTime
        });
    }



    hubConnection.on("Receive", function (message) {

        console.log(message);
      

        const messageContainer = document.createElement("div");
        messageContainer.classList.add("message-container");

        const messageHeader = document.createElement("div");
        messageHeader.classList.add("message-header");

        const messageUserName = document.createElement("span");
        messageUserName.classList.add("sender-name");

        const messageTime = document.createElement("span");
        messageTime.classList.add("message-time");

        const messageContent = document.createElement("div");
        messageContent.classList.add("message-content");

        const messageText = document.createElement("p");



        const messageContainerInvGroup = document.createElement("div");
        const messageTextInvGroup = document.createElement("p");

        CurrentUserId = document.getElementById("userId").value;

        console.log(CurrentUserId);

        if (message.userSenderId === CurrentUserId ) {                     

            if (message.message === "inviteGroup") {
                messageContainer.innerHTML = "<p class=\"notifMsg\"> Вы присоединились к чату </p>"
            } else {
                messageUserName.textContent = "";


                messageHeader.appendChild(messageUserName);


                messageContainer.appendChild(messageHeader);

                messageText.textContent = message.message;

                messageContent.appendChild(messageText);

                messageTime.textContent = message.sendTime;
                messageContent.appendChild(messageTime);

                messageContainer.appendChild(messageContent);
                messageContainer.classList.add("user-message");
            }

        } else {
            if (message.message === "inviteGroup") {
                messageContainer.innerHTML = "<p class=\"notifMsg\">" + message.userSenderName + " присоединился в чат </p>"
            } else {
                messageUserName.textContent = message.userSenderName;
                

                messageHeader.appendChild(messageUserName);
               

                messageContainer.appendChild(messageHeader);

                messageText.textContent = message.message;

                messageContent.appendChild(messageText);

                messageTime.textContent = message.sendTime;
                messageContent.appendChild(messageTime);

                messageContainer.appendChild(messageContent);
                messageContainer.classList.add("other-message");
            }
        }

       
        var messagesList = document.getElementById("messagesList");

        messagesList.appendChild(messageContainer);

        messagesList.scrollTop = messagesList.scrollHeight;
        
    });



    window.addEventListener('beforeunload', function (e) {
        // Здесь можно вызвать функцию для выполнения необходимых действий,
        // например, уведомить сервер о выходе пользователя из чата или группы.

        // Вызов функции
        leaveChat();

        // Если вы хотите отобразить пользователю сообщение перед закрытием страницы,
        // установите свойство returnValue в объекте события.
        // e.returnValue = 'Вы уверены, что хотите покинуть страницу?';
    });

    function leaveChat() {
        let groupId = document.getElementById("groupId").value;

        // Вызов метода на сервере для уведомления о выходе пользователя из группы
        hubConnection.invoke("leaveChat", groupId);
    }


    hubConnection.on("LoadMessageHistory", function (messageHistory) {
        messageHistory.forEach(function (message) {
            displayMessage(message);
        });
    });



    function displayMessage(message) {
        const messageContainer = document.createElement("div");
        messageContainer.classList.add("message-container");

        const messageHeader = document.createElement("div");
        messageHeader.classList.add("message-header");

        const messageUserName = document.createElement("span");
        messageUserName.classList.add("sender-name");

        const messageTime = document.createElement("span");
        messageTime.classList.add("message-time");

        const messageContent = document.createElement("div");
        messageContent.classList.add("message-content");

        const messageText = document.createElement("p");



        const messageContainerInvGroup = document.createElement("div");
        const messageTextInvGroup = document.createElement("p");


        console.log(message.sendTime);

        if (message.userSenderId === CurrentUserId && message.message != "inviteGroup") {

            messageUserName.textContent = "";
          

            messageHeader.appendChild(messageUserName);
           

            messageContainer.appendChild(messageHeader);

            messageText.textContent = message.message;

            messageContent.appendChild(messageText);

            messageTime.textContent = message.sendTime;
            messageContent.appendChild(messageTime);

            messageContainer.appendChild(messageContent);
            messageContainer.classList.add("user-message");

        } else {
            if (message.message === "inviteGroup") {
                messageContainer.innerHTML = "<p class=\"notifMsg\">" + message.userSenderName + " присоединился в чат </p>"
            } else {
                messageUserName.textContent = message.userSenderName;
               

                messageHeader.appendChild(messageUserName);
                

                messageContainer.appendChild(messageHeader);

                messageText.textContent = message.message;

                messageContent.appendChild(messageText);

                messageTime.textContent = message.sendTime;
                messageContent.appendChild(messageTime);

                messageContainer.appendChild(messageContent);
                messageContainer.classList.add("other-message");
            }
        }

        document.getElementById("messagesList").appendChild(messageContainer);


    }
