
var chatHub = $.connection.signalHub;
var srp = "../";
var IMAGEURL = '';
var UserID = parseInt($("#hdnCurrentUserID").val());
var UserIdProfile = parseInt($("#vidProfile").val());

//Math.round((new Date()).getTime() / 1000);

$(document).ready(function () {
    // Declare a proxy to reference the hub. 
    //var chatHub = $.connection.signalHub;
    // Start Hub
    newConection();
});

function newConection() {
    $.connection.hub.start().done(function () {
        chatHub.server.connect(UserID, urlHttp);
    });

    console.log("Estado de la conexión: " + $.connection.hub.state )
}

function Reconnect() {
    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            newConection()
        }, 1000); // Restart connection after 5 seconds.
    });
}
// On New User Connected
chatHub.client.onConnected = function (param, pidconex, pidCampaing,  ConextedUsers) {
    $("#hdId").val(pidconex)
    $("#vIdCampaign").val(pidCampaing);
    //console.log(param);
    console.log(ConextedUsers);
    if (vTypeUrl == "mes") {
        //console.log("Empleado conex: " + $("#hdId").val())
        chatHub.server.setClientsAsync($("#hdId").val(), parseInt(UserID), $("#hdChatClient").val());
    }
};

// On User Disconnected
chatHub.client.onUserDisconnected = function (id, userName) {
};

chatHub.client.reciveNewMessage = function (param, numphone) {
    Reconnect()
    //Esta funcion viene del controlador de que ha llegado un nuevo mensaje y se va al hub
    chatHub.server.reciveNewMessage($("#hdId").val(), param, $("#hdChatClient").val());
};
chatHub.client.notifyNewMessage = function (pChatNumber, pPhoneNumber, pNameClient) {
    Reconnect()
    //esto es noficando que ha llegado un nuevo mensaje desde el hub
    var stringName = "";
    if (pNameClient != undefined) {
        if (pNameClient != '') {
            if (!pNameClient.toLowerCase().includes('+'))
                stringName = " - " + pNameClient;
        }
    }
    if (pChatNumber == undefined) {
        toastr.success("Ha llegado un mensaje del número " + pPhoneNumber + stringName, "Hola!...", { timeOut: 1000 });
        //console.log("Empleado conex: " + $("#hdId").val())
        console.log("Ha llegado un mensaje del número " + pPhoneNumber + stringName)

        if (vTypeUrl == "mes") {
            //chatHub.server.setClientsAsync($("#hdId").val(), parseInt(UserID), $("#hdChatClient").val());
        }
    }
    else {
        if ($("#hdChatClient").val() == pChatNumber)
            sendMessageFromEmployee(pChatNumber);
    }
};
function reloadChatList() {
    newConection()
    //chatHub.server.setClientsAsync($("#hdId").val(), parseInt(UserID), $("#hdChatClient").val());
}
function notifySelectedClient(pNameAsesor, pNumberPhone) {
    var stringCol = 'El asesor ' + pNameAsesor + ' atendera al ' + pNumberPhone + '';
    //console.log("entre aqui, notifySelectedClient");
    chatHub.server.notifySelectedClient(parseInt(UserID), pNumberPhone, parseInt($("#vIdCampaign").val()), stringCol);
    //toastr.warning('El asesor ' + pNameAsesor + ' atendera al ' + pNumberPhone+'', "Notificación", { timeOut: 1000, newestOnTop: true });
}
chatHub.client.notifySelectedClientServer = function (pNotification) {
    //setTimeout(function () { chatHub.server.setClientsAsync($("#hdId").val(), parseInt(UserID), $("#hdChatClient").val()); }, 1000);
    toastr.warning(pNotification, "Notificación", { timeOut: 3000, newestOnTop: true, positionClass: "toast-top-left" });
    console.log("Estado de la conexión notifySelectedClientServer: " + $.connection.hub.state)
}

chatHub.client.GetClients = function (pLClient, bolda) {
    console.log("Estado de la conexión getclients: " + $.connection.hub.state)
    $("#spinerIconListClient").addClass("d-none");
    if (bolda) {
        if (pLClient == null)
            toastr.warning("- Este usuario no tiene una campaña asignada.<br>- La campaña no esta activa.<br>- Ambos Casos.", "Advertencia", { timeOut: 5000 });
        else {
            //var lastUsedUnix = $("#hdLastTimeMessage").val();
            $(".tabss").DataTable().clear().destroy();
            $("#tableClients").html("")
            var nowDate = new Date().toLocaleString("es", { year: "numeric", month: "2-digit", day: "numeric", day: "2-digit" })
            $.each(pLClient, function (index, row) {
                ShowIndividualClient(row, "#tableClients")
            })
           //$("#hdLastTimeMessage").val(pLClient[0].lastUsedUnix);
            $(".tabss").DataTable({
                "info": false,
                paging: false,
                destroy: true,
                ordering: false,
                "scrollX": false,
                "scrollY": 500,
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.10.19/i18n/Spanish.json'
                }
            });
        }
    }
    else {
        if (vTypeUrl == "mes") {
            //console.log("Empleado conex: " + $("#hdId").val())
            //location.reload();
        }
        console.log("Hay un problema interno obteniendo todos los clientes, mensaje: <br>" + pLClient);
        //toastr.warning("Hay un problema interno, mensaje: <br>" + pLClient, "Información", { timeOut: 5000 });
    }
};

function getMessagesMaster(pid, pname, pIMAGEURL ) {
    var spin = $("#spinerIcon");
    IMAGEURL = pIMAGEURL;
    spin.removeClass("d-none");
    //console.log("Empleado conex: " + $("#hdId").val())
    $('#panelChatList_' + UserID).html("");
    console.log("Estado de la conexión getmessagesmaster: " + $.connection.hub.state)
    chatHub.server.getMessages($("#hdId").val(), pid, $("#hdnCurrentUserID").val());
};
chatHub.client.DisplayAllMessages = function (pLMEssages, bolda, lastMessageNumber) {
   // $.connection.hub.logging = true;
    console.log("Estado de la conexión displayallmessages: " + $.connection.hub.state)
    var spin = $("#spinerIcon");
    spin.addClass("d-none");
    if (bolda) {
        $("#hdLastMessageNumber").val(lastMessageNumber)
        if (pLMEssages.length == 0) {
            toastr.success('No hay datos en la conversacion', { timeOut: 500 });
        }
        else {
            //setTimeout(function () { chatHub.server.setClientsAsync($("#hdId").val(), parseInt(UserID), $("#hdChatClient").val()); }, 500);
            $('#panelChatList_' + UserID).html("");
            toastr.success('Información de conversación obtenida', { timeOut: 500 });
            $.each(pLMEssages, function (index, item) {
                //console.log(item)
                //var dTime = convertDateTime(item.time);
                $('#panelChatList_' + UserID).append(showIndividualMessage(item))//*/
            });
            console.log("Voy a verificar en BD los mensajes");
            chatHub.server.storeMessages($("#hdId").val(), pLMEssages);
        }
        $('#panelChatList_' + UserID).animate({ scrollTop: 80000 }, 1000);
    }
    else {
        console.log("Hay un problema interno mostrando todos los mensajes, mensaje: <br>" + pLMEssages);
        if (vTypeUrl == "mes") {
            //console.log("Empleado conex: " + $("#hdId").val())
          // location.reload();
        }
        //toastr.warning("Hay un problema interno, mensaje: <br>" + pLMEssages, "Información", { timeOut: 5000 });
    }
    
};

chatHub.client.storeMessages = function (param) {
    console.log(param);
}

function sendMessageFromEmployee(paramPhone) {
    //console.log("entre aqui cuando se envio un mensaje al cliente, num: " + paramPhone);
    //console.log("Empleado conex: " + $("#hdId").val())
    chatHub.server.getLastMessage($("#hdId").val(), paramPhone, $("#hdLastMessageNumber").val(), $("#hdnCurrentUserID").val());
}
chatHub.client.DisplayLastMessages = function (pLMEssages, bolda, lastMessageNumber) {
    console.log("entre aqui para mostrar el ultimo mensaje")
    //console.log(pLMEssages)
    //console.log(bolda)
    ///console.log(lastMessageNumber)
    console.log("Estado de la conexión displaylastmessages: " + $.connection.hub.state)
    if (bolda) {
        $("#hdLastMessageNumber").val(lastMessageNumber)
        if (pLMEssages.length == 0) {
        }
        else {
            //setTimeout(function () { chatHub.server.setClientsAsync($("#hdId").val(), parseInt(UserID), $("#hdChatClient").val()); }, 500);
            $.each(pLMEssages, function (index, item) {
                //console.log(item)
                //var dTime = convertDateTime(item.time);
                $('#panelChatList_' + UserID).append(showIndividualMessage(item))//*/
            });
            console.log("Voy a verificar en BD los mensajes");
            chatHub.server.storeMessages(pLMEssages);
        }
        $('#panelChatList_' + UserID).animate({ scrollTop: 80000 }, 1000);
    }
    else {
        console.log("Hay un problema interno mostrando el ultimo mensaje, mensaje: <br>" + pLMEssages);
        if (vTypeUrl == "mes") {
            //console.log("Empleado conex: " + $("#hdId").val())
            //location.reload();
        }
        //toastr.warning("Hay un problema interno, mensaje: <br>" + pLMEssages, "Información", { timeOut: 5000 });
    }
}

function showIndividualMessage(item) {
    var sks = "";
    var dt = new Date('1900-01-01 ' + item.time.split(' ')[1]);
    var h = dt.getHours(), m = (dt.getMinutes() < 10) ? '0' + dt.getMinutes() : dt.getMinutes();
    var thistime = (h >= 12) ? (((h == 12) ? (h) : (h - 12)) + ':' + m.toString() + ' p.m.') : (h + ':' + m.toString() + ' a.m.');
    if (item.fromMe) {
        sks = '<li class="media media-chat-item-reverse pt-1 pb-1">';
        if (item.caption != null)
            sks += '<div class="media-body chat_hola">';
        else
            sks += '<div class="media-body ' + item.type + '_hola">';
        var mediainfo = item.body;
        if (item.type == "vcard")
            mediainfo = '<a href="' + item.body + '" target="_blank" class="text-black">DOCUMENTO (presione para descargar)"</a>';
        if (item.type == "document")
            mediainfo = '<a href="' + item.body + '" target="_blank" class="text-black">DOCUMENTO (presione para descargar)"</a>';
        if (item.type == "ptt" || item.type == "audio")
            mediainfo = '<audio controls="controls" class=""><source src="' + item.body + '" /></audio>';
        if (item.type == "image")
            mediainfo = '<img class="showMedia cursor-pointer " data-href="' + item.body + '" data-type="' + item.type + '" src="' + item.body + '" width="200" heigth="200" />';
        if (item.type == "video")
            mediainfo = '<video class="showMedia cursor-pointer " data-href="' + item.body + '" data-type="' + item.type + '" width="320" height="200"><source src="' + item.body + '" /></video>';
        var vcaption = "";
        if (item.caption != null)
            vcaption = '<br>' + item.caption;

        sks += '<div class="media-chat-item">' + mediainfo + vcaption + '</div>';
        sks += '<div class="font-size-sm text-muted">' + item.time.split(' ')[0] + ' - ' + thistime + ' </div>';
        sks += '</div>';
        sks += '<div class="ml-1">';
        //sks += '<a href="' + IMAGEURL + '" target="_blank">';
        //sks += '<img src="' + IMAGEURL + '" class="rounded-circle" width="40" height="40" alt="">';
        //sks += '</a>';
        sks += '</div>';
        sks += '</li>';
    } else {
        sks = '<li class="media pt-1 pb-1">';
        sks += '<div class="mr-1">';
        //sks += '<a href="/Content/global_assets/images/placeholders/placeholder.jpg" target="_blank">';
        //sks += '<img src="/Content/global_assets/images/placeholders/placeholder.jpg" class="rounded-circle" width="40" height="40" alt="">';
        //sks += '</a>';
        sks += '</div>';
        if (item.caption != null)
            sks += '<div class="media-body chat_hola">';
        else
            sks += '<div class="media-body ' + item.type + '_hola">';
        var mediainfo = item.body;
        if (item.type == "vcard")
            mediainfo = '<a href="' + item.body + '" target="_blank" class="text-black">DOCUMENTO (presione para descargar)"</a>';
        if (item.type == "document")
            mediainfo = '<a href="' + item.body + '" target="_blank" class="text-black">DOCUMENTO (presione para descargar)"</a>';
        if (item.type == "ptt" || item.type == "audio")
            mediainfo = '<audio controls="controls" class=""><source src="' + item.body + '" /></audio>';
        if (item.type == "image")
            mediainfo = '<img class="showMedia cursor-pointer " data-href="' + item.body + '" data-type="' + item.type + '" src="' + item.body + '" width="200" heigth="200" />';
        if (item.type == "video")
            mediainfo = '<video class="showMedia cursor-pointer " data-href="' + item.body + '" data-type="' + item.type + '" width="320" height="200"><source src="' + item.body + '" /></video>';
        var vcaption = "";
        if (item.caption != null)
            vcaption = '<br>' + item.caption;

        sks += '<div class="media-chat-item">' + mediainfo + vcaption + '</div>';
        sks += '<div class="font-size-sm text-muted">' + item.time.split(' ')[0] + ' - ' + thistime + ' </div>';
        sks += '</div>';
        sks += '</li>';
    }
    return sks;
}
function ShowIndividualClient(prowClient, pContainer) {
    var row = prowClient;
    var lastIdEmployee = '<b class="text-blue"> en el CHATBOT</b>';
    /*var timeDateLast = new Date(row.lastUsed.replace('T', ' ')).toLocaleTimeString("es", { year: "numeric", month: "2-digit", day: "numeric", day: "2-digit" });
    var dt = new Date('1900-01-01 ' + timeDateLast.split(' ')[1]);
    var h = dt.getHours(), m = (dt.getMinutes() < 10) ? '0' + dt.getMinutes() : dt.getMinutes();
    var timeLast = (h >= 12) ? (((h == 12) ? (h) : (h - 12)) + ':' + m.toString() + ' p.m.') : (h + ':' + m.toString() + ' a.m.');
    if (timeDateLast.split(' ')[0] < nowDate) {
        timeLast = nowDate;
    }*/
    vhtml = "";
    vhtml += '<tr>';
    vhtml += '<td class="pl-0 pr-0">';
    vhtml += '<div class="d-flex align-items-center">';
    if (parseInt(row.lastIdEmployee) == parseInt(UserID)) {
        vhtml += '<div class="list-icons mr-2">';
        vhtml += '<div class="dropdown">';
        vhtml += '<a href="#" class="list-icons-item dropdown-toggle caret-0" data-toggle="dropdown"><i class="icon-menu7"></i></a>';
        vhtml += '<div class="dropdown-menu dropdown-menu-right">';
        vhtml += '<a class="dropdown-item cursor-pointer freeClient"';
        vhtml += 'data-id="' + row.idClient + '"';
        vhtml += 'data-phone="' + row.phoneNumber + '"><i class="icon-unlocked2"></i> Liberar</a>';
        vhtml += '<a class="dropdown-item cursor-pointer freeClientBot"';
        vhtml += 'data-id="' + row.idClient + '"';
        vhtml += 'data-phone="' + row.phoneNumber + '"><i class="icon-airplane2"></i> Liberar y devolver al bot</a>';
        vhtml += '</div>';
        vhtml += '</div>';
        vhtml += '</div>';
    }
    vhtml += '<div class="mr-2">';
    vhtml += '<a href="' + row.image + '" target="_blank">';
    vhtml += '<img src="' + row.image + '" class="rounded-circle" width="32" height="32" alt="">';
    vhtml += '</a>';
    vhtml += '</div>';
    var kos = 'cursor-default ';
    if (row.lastIdEmployee == null || row.lastIdEmployee == 0 || parseInt(row.lastIdEmployee) == parseInt(UserID) || UserIdProfile != 4)
        kos = 'cursor-pointer btnselected';
    vhtml += '<div class="' + kos + '"';
    vhtml += 'data-id="' + row.idClient + '"';
    vhtml += 'data-phone="' + row.phoneNumber + '"';
    var lastName = row.LastName;
    if (row.LastName == null)
        lastName = '';
    if (row.lastIdEmployee == 0)
        lastIdEmployee = '<b class="text-pink-800"> esperando atención</b>';
    else if (row.lastIdEmployee > 0)
        lastIdEmployee = '<br><b class="text-warning" id="free_' + row.idClient + '"> Atendido por ' + row.nameEmployee + '</b>';
    vhtml += 'data-name="' + row.firstName + " " + lastName + '">';
    vhtml += '<a class="text-default font-weight-semibold">' + row.firstName + ' ' + lastName + ' - ' + lastIdEmployee;
    //if (row.lastUsedUnix > lastUsedUnix)
    //    vhtml += ' <span class="badge badge-mark border-green"></span>';
    vhtml += '<div class="text-muted font-size-sm-1">' + row.phoneNumber + '</div>';
    vhtml += '</a>';
    vhtml += '</div>';
    vhtml += '</div>';
    vhtml += '</td>';
    //vhtml += '<td class="pl-0 pr-1">';
    //vhtml += '<a class="text-default font-weight-semibold">' + timeLast + '</a>';
    //vhtml += '</td>';
    vhtml += '</tr>';
    $(pContainer).append(vhtml);
}
