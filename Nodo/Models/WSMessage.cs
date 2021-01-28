using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nodo.Models
{
    public partial class CitySmartData
    {
        public int id { set; get; }
        public string nombre { set; get; }
    }
    public partial class CityOWL
    {
        public int id { set; get; }
        public string text { set; get; }
    }
    public partial class ClientSmartData
    {
        public string id { set; get; }
        public string nombre { set; get; }
        public string apellidos { set; get; }
        public string numeroIdentificacion { set; get; }
        public string tipoDocumentoId { set; get; }
        public string ciudadId { set; get; }
        public string direccion { set; get; }
        public int aceptaHabeas { set; get; }
        public string fechaTerminos { set; get; }
        public CuentaSmartData cuenta { set; get; }
        public CelularSmartData[] listCelular { set; get; }
    }
    public partial class CuentaSmartData
    {
        public string acumulacion { set; get; }
        public string oportunidades { set; get; }
        public string codigosAcumulados { set; get; }
        public string faltante { set; get; }

    }
    public partial class CelularSmartData
    {
        public string celular { get; set; }
    }

    public partial class TokenSmartData
    {
        public string token { get; set; }
        public bool mustChangePassword { set; get; }
    }
    //para la lista de chats de la base de datos
    public class ChatList
    {
        public dynamic idChatList { get; set; }
        public dynamic idCampaign { get; set; }
        public dynamic oid { get; set; }
        public dynamic idClient { get; set; }
        public dynamic firstName { get; set; }
        public dynamic LastName { get; set; }
        public dynamic phoneNumber { get; set; }
        public dynamic image { get; set; }
        public dynamic lastUsedUnix { get; set; }
        public dynamic lastUsed { get; set; }
        public dynamic lastIdEmployee { get; set; }
        public dynamic nameEmployee { get; set; }
        public dynamic created_at { get; set; }
    }

    //para traer la info de la lista de chats
    public class listDialogs
    {
        public List<conversation> dialogs { get; set; }
    }
    public class conversation
    {
        public string id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public metadataConversation metadata { get; set; }
        public string last_time { get; set; }
    }
    public class metadataConversation
    {
        public string isGroup { get; set; }
        public string[] participants { get; set; }
        public string groupInviteLink { get; set; }
    }
    //para las notoficaciones de un mensaje nuevo
    public class NotifyMessage
    {
        [JsonProperty("instanceId")]
        public string instanceId { get; set; }
        [JsonProperty("messages")]
        public Message[] messages { get; set; }
        [JsonProperty("smartdata")]
        public Message[] smartdata { get; set; }

    }
    public class ListMessages
    {
        public List<Message> messages { get; set; }
        public string lastMessageNumber { get; set; }
    }
    public class Message
    {
        internal string[] push;

        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("body")]
        public string body { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("senderName")]
        public string senderName { get; set; }
        [JsonProperty("fromMe")]
        public bool fromMe { get; set; }
        [JsonProperty("author")]
        public string author { get; set; }
        [JsonProperty("time")]
        public dynamic time { get; set; }
        public dynamic timeUnix { get; set; }
        [JsonProperty("chatId")]
        public string chatId { get; set; }
        [JsonProperty("messageNumber")]
        public long messageNumber { get; set; }
        [JsonProperty("self")]
        public string self { get; set; }
        [JsonProperty("isForwarded")]
        public string isForwarded { get; set; }
        [JsonProperty("quotedMsgBody")]
        public string quotedMsgBody { get; set; }
        [JsonProperty("quotedMsgId")]
        public string quotedMsgId { get; set; }
        [JsonProperty("chatName")]
        public string chatName { get; set; }
        [JsonProperty("caption")]
        public string caption { get; set; }
        [JsonProperty("filename")]
        public string filename { get; set; }
        [JsonProperty("phone")]
        public string phone { get; set; }
    }

    public class Activity
    {
        public dynamic idActivity { get; set; }
        public dynamic name { get; set; }
        public dynamic icon { get; set; }
    }
    public class Configuration
    {
        public dynamic idConfig { get; set; }
        public dynamic idCampaign { get; set; }
        public dynamic name { get; set; }
        public dynamic value { get; set; }
    }
    public class TypeFiles
    {
        public dynamic idTypeFile { get; set; }
        public dynamic nameType { get; set; }
        public dynamic idGroupFile { get; set; }
        public dynamic nameGroup { get; set; }
        public dynamic srcFile { get; set; }
    }
    public class WSMessage
    {
    }
    public class Login
    {
        public string usuario { get; set; }
        public string contrasena { get; set; }
        public string contrasenatemp { get; set; }
        public string newcontrasena { get; set; }
        public string confirmcontrasena { get; set; }
    }

    public class employee
    {
        public string ConnectionId { get; set; }
        public int idEmployee { get; set; }
        public int statuse { get; set; }
        public string user_cur { get; set; }
        public string password { get; set; }
        public byte[] passwordEncriptado { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string jobPosition { get; set; }
        public string phoneMobile { get; set; }
        public string landlinePhone { get; set; }
        public string idprofile { get; set; }
        public string language { get; set; }
        public string idCompany { get; set; }
        public string srcImage { get; set; }
        public string srcImageBack { get; set; }
        public string permiso { get; set; }
        public string idActivity { get; set; }
        public string nameActivity { get; set; }
        public string iconActivity { get; set; }
        public int isMessage { get; set; }
        public dynamic idCampaign { get; set; }
        public List<ChatList> ChatListOwn { get; set; }
        public int isOccupied { get; set; }
        public String lastNumberChat { get; set; }
        public ChatList lastConversation { get; set; }

    }
    public class campaigns
    {
        public int idCampaigns { get; set; }
        public dynamic idCompany { get; set; }
        public string name { get; set; }
        public string dateStart { get; set; }
        public string dateEnd { get; set; }
        public int idStatus { get; set; }
        public string phoneNumber { get; set; }
    }
    public class Client
    {                
        public dynamic idClient { get; set; }
        public dynamic firstName { get; set; }
        public dynamic SecondName { get; set; }
        public dynamic LastName { get; set; }
        public dynamic SecondSurName { get; set; }
        public dynamic numDocument { get; set; }
        public dynamic idGender { get; set; }
        public dynamic email { get; set; }
        public dynamic email2 { get; set; }
        public dynamic phoneNumber { get; set; }
        public dynamic phoneNumber2 { get; set; }
        public dynamic birthdate { get; set; }
        public dynamic idLocation { get; set; }
        public dynamic idStatus { get; set; }
        public dynamic srcImage { get; set; }
        public dynamic statusChat { get; set; }
    }
}