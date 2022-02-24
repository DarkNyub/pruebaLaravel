using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Web;
using Microsoft.AspNet.SignalR;
using Nodo.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Spreadsheet;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Threading;

namespace Nodo.Hubs
{
    public class SignalHub : Microsoft.AspNet.SignalR.Hub
    {
        static int vcount = 1;
        static List<String> ConnectedUsers = new List<String>();
        static string sessionUrlHttp = "", VTOKEN = "", VINSTANCE = "", VURL = "";

        public void Connect( int UserID, string urlHttp)
        {
            var id = Context.ConnectionId;
            sessionUrlHttp = urlHttp;
            
            
            VTOKEN = "mrdqgdmavubawwwh";
            VINSTANCE = "408905";
            VURL = "https://api.chat-api.com/";
                
            Clients.Caller.onConnected("Empleado agregado sin problemas, con este id de conexion ");
            // send to all except caller client
            //Clients.AllExcept(CurrentUser.ConnectionId).onNewUserConnected(CurrentUser.UserID.ToString(), CurrentUser.UserName, CurrentUser.UserID, CurrentUser.profileImg);
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
        public void notifySelectedClient(int pidEmployee, string pNumberPhone,int pidCampaign, String pNotification)
        {
            Clients.All.notifySelectedClientServer(pNotification);
        }

        /*
        public void reciveNewMessage(string pContextConnet, Models.NotifyMessage notifyMessage)
        {
            var id = Context.ConnectionId;
            var user = ConnectedUsers.Where(e => e.ConnectionId == pContextConnet).FirstOrDefault();
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            string pChatNumber = notifyMessage.messages[0].chatId.Split('@')[0];
            dynamic rowClient = null;
            int entre = 0;
            string numPhone = "";
            int oks = 1;
            for (int gg = pChatNumber.Length - 1; gg >= 0; gg--)
            {
                if (oks < 11)
                    numPhone = pChatNumber[gg].ToString() + numPhone;
                oks++;
            }
            for (int i = 0; i < ConnectedUsers.Count(); i++)
            {
                employee rowE = ConnectedUsers[i];
                if (rowE.ConnectionId == id)
                {
                    if (rowE.isMessage == 1 && rowE.isOccupied == 1 && rowE.lastNumberChat == pChatNumber)
                        entre = 1;
                }
                rowClient = data.getDataClientByNumber(numPhone).Rows[0];
            }
            var vClientName = "";
            if (rowClient != null)
                vClientName = rowClient["firstName"].ToString() + " " + rowClient["LastName"].ToString();
            if (entre == 1)
                Clients.Caller.notifyNewMessage(pChatNumber);
            else
                Clients.Caller.notifyNewMessage(null, numPhone, vClientName);
        }
        */
        public void reciveNewMessage(string pContextConnet, Models.NotifyMessage notifyMessage, string plastMessageNumber)
        {
            var id = pContextConnet;
            string pInstanceId = notifyMessage.instanceId;
            string pChatNumber = notifyMessage.messages[0].chatId.Split('@')[0];
            dynamic rowClient = null;
            int entre = 0;
            string numPhone = "";
            int oks = 1;

            for (int gg = pChatNumber.Length - 1; gg >= 0; gg--)
            {
                if (oks < 11)
                    numPhone = pChatNumber[gg].ToString() + numPhone;
                oks++;
            }
            List<String> vlistemplid = new List<string>();

            string notification = notifyMessage.messages[0].fromMe.ToString();
            // Clients.Users(vlistemplid).notifyNewMessage(pChatNumber, notification);
            Clients.Caller.notifyNewMessage(pChatNumber, notification);
        }


        //inserta los Chats en la tabla TB_DifusionMessage
        public void reciveNewMessageDifussion(string pContextConnet, Models.NotifyMessage notifyMessage, string plastMessageNumber)
        {
            string pInstanceId = notifyMessage.instanceId;
            
            string pNumberClient = (notifyMessage.messages[0].author.Split('@')[0].ToString()).Substring(2, 10);

            var time1 = UnixTimeToDateTime(Convert.ToInt64(notifyMessage.messages[0].time.ToString()));
            var time = time1.ToString("yyyy-MM-dd HH:mm");

           // data.StoreDifusionMessagesByClient(notifyMessage,1, 1, pNumberClient, time);
        }

        public void setClientsAsyncCampaing(string pPhoneNumber,int dt)
        {
            try
            {
                ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
                data.UpdateCampingLine(pPhoneNumber, dt);
            }
            catch (Exception ex)
            {
                Clients.Caller.GetClients(ex.Message);
            }

        }

        public async Task setClientsAsync(string pContextConnet, int pidEmployee, string pNumberPhone = "")
        {
            try
            {
                string UrlApi = "instance" + VINSTANCE + "/dialogs";
                listDialogs vChats = new listDialogs();
                HttpResponseMessage result;
                    List<Client> LClient = new List<Client>();
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(VURL);
                        result = await client.GetAsync(UrlApi + "?token=" + VTOKEN);
                        if (result.IsSuccessStatusCode)
                        {
                            var resultContent = await result.Content.ReadAsStringAsync();
                            dynamic olas = resultContent;//.Replace("\"", "'");
                            vChats = JsonConvert.DeserializeObject<listDialogs>(olas);
                        }
                    }
                    List<ChatList> LChats = new List<ChatList>();
                    vChats.dialogs = vChats.dialogs.Where(e => !e.id.Contains('-')).OrderByDescending(e=>e.last_time).Take((5)).ToList();
                    DataTable dtfinal = new DataTable();
                    dtfinal.Columns.Add(new DataColumn("pidCampaign", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("pidClient", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("pOid", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("pimage", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("pLastUsed", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("pName", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("pPhoneNumber", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("pLastUsedUnix", typeof(string)));
                    for (int i = 0; i < vChats.dialogs.Count; i++)
                    {
                        conversation koko = vChats.dialogs[i];
                        string last_time = UnixTimeToDateTime(Convert.ToInt64(koko.last_time)).ToString("yyyy-MM-dd HH:mm");
                        var numberPhone = koko.id.Split('@')[0].ToString().ToCharArray();
                        string numPhone = "", code = "";
                        int oks = 1;
                        for (int gg = numberPhone.Length - 1; gg >= 0; gg--)
                        {
                            if (oks < 11)
                                numPhone = numberPhone[gg].ToString() + numPhone;
                            else
                                code = numberPhone[gg].ToString() + code;
                            oks++;
                        }
                        if(code != "")
                        {
                            if (Convert.ToInt32(code) == 57)
                            {
                                if (koko.image == "undefined" || koko.image == "" || koko.image == "null")
                                    koko.image = "/Content/global_assets/images/placeholders/user_bg2.jpg";

                                dtfinal.Rows.Add();
                                dtfinal.Rows[dtfinal.Rows.Count - 1][0] = 1;
                                dtfinal.Rows[dtfinal.Rows.Count - 1][1] = 0;
                                dtfinal.Rows[dtfinal.Rows.Count - 1][2] = koko.id;
                                dtfinal.Rows[dtfinal.Rows.Count - 1][3] = koko.image;
                                dtfinal.Rows[dtfinal.Rows.Count - 1][4] = last_time;
                                dtfinal.Rows[dtfinal.Rows.Count - 1][5] = koko.name;
                                dtfinal.Rows[dtfinal.Rows.Count - 1][6] = numPhone.ToString();
                                dtfinal.Rows[dtfinal.Rows.Count - 1][7] = koko.last_time;
                            }
                        }
                        else{
                            string sada = "";
                        }
                    }
                    int VAAS = 0;
                    LChats = dtfinal.AsEnumerable().Select(m => new ChatList()
                    {
                        firstName = m.Field<dynamic>("pName"),
                        LastName = "",
                        oid = m.Field<dynamic>("pOid"),
                        idCampaign = 1,
                        image = m.Field<dynamic>("pimage"),
                        lastUsedUnix = m.Field<dynamic>("pLastUsedUnix"),
                        lastUsed = m.Field<dynamic>("pLastUsed"),
                        phoneNumber = m.Field<dynamic>("pPhoneNumber"),
                        idClient = m.Field<dynamic>("pidClient"),
                        lastIdEmployee = 1,
                        nameEmployee = "nambe"
                    }).ToList();
                    Clients.Caller.GetClients(LChats, true);
            }
            catch (Exception ex)
            {
                Clients.Caller.GetClients(ex.Message);
            }
        }
        public async Task getMessages(string pContextConnet, string pidNumber, int pidEmployee)
        {
            try
            {

                string menssages = "messages";
                string UrlApi = "instance" + VINSTANCE + "/" + menssages;
                ListMessages LMessages = new ListMessages();
                HttpResponseMessage result;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(VURL);
                    result = await client.GetAsync(UrlApi + "?token=" + VTOKEN + "&last=true&chatId=" + pidNumber + "@c.us");
                    if (result.IsSuccessStatusCode)
                    {
                        var resultContent = await result.Content.ReadAsStringAsync();
                        dynamic olas = resultContent;//.Replace("\"", "'");
                        LMessages = JsonConvert.DeserializeObject<ListMessages>(olas);
                    }
                    if (LMessages.messages.Count > 0)
                    {
                        for (int i = 0; i < LMessages.messages.Count; i++)
                        {
                            Message row = LMessages.messages[i];
                            LMessages.messages[i].timeUnix = row.time;
                            row.time = UnixTimeToDateTime(Convert.ToInt64(row.time));
                            LMessages.messages[i].time = row.time.ToString("yyyy-MM-dd HH:mm");
                        }
                    }
                }
                Clients.Caller.DisplayAllMessages(LMessages.messages.OrderBy(e => e.timeUnix).ToList(), true, LMessages.lastMessageNumber);
            }
            catch (Exception ex)
            {
                Clients.Caller.DisplayAllMessages(ex.Message);
            }
        }

        public async Task getLastMessage(string pContextConnet, string pidNumber, string plastMessageNumber, int pidEmployee)
        {
            try
            {
                //var user = ConnectedUsers.Where(e => e.ConnectionId == pContextConnet).FirstOrDefault();
                ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
                //dynamic rowCampaing = user.idCampaign;

                string menssages = "messages";
                string UrlApi = "instance" + VINSTANCE + "/" + menssages;
                ListMessages LMessages = new ListMessages();
                HttpResponseMessage result;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(VURL);
                    do
                    {
                        result = await client.GetAsync(UrlApi + "?token=" + VTOKEN + "&chatId=" + pidNumber + "@c.us&lastMessageNumber=" + plastMessageNumber);
                        if (result.IsSuccessStatusCode)
                        {
                            var resultContent = await result.Content.ReadAsStringAsync();
                            dynamic olas = resultContent;//.Replace("\"", "'");
                            LMessages = JsonConvert.DeserializeObject<ListMessages>(olas);
                        }
                        if (LMessages.messages.Count > 0)
                        {
                            for (int i = 0; i < LMessages.messages.Count; i++)
                            {
                                Message row = LMessages.messages[i];
                                LMessages.messages[i].timeUnix = row.time;
                                row.time = UnixTimeToDateTime(Convert.ToInt64(row.time));
                                LMessages.messages[i].time = row.time.ToString("yyyy-MM-dd HH:mm");
                            }
                        }
                        Thread.Sleep(1000);
                    } while (LMessages.messages.Count == 0);
                }
                if (LMessages.lastMessageNumber == null)
                    LMessages.lastMessageNumber = (Convert.ToInt32(plastMessageNumber)+1).ToString();
                Clients.Caller.DisplayLastMessages(LMessages.messages.OrderBy(e => e.timeUnix).ToList(), true, LMessages.lastMessageNumber);
            }
            catch(Exception ex)
            {
                Clients.Caller.DisplayLastMessages(ex.Message);
            }
        }
        public void storeMessages(string pContextConnet, List<Message> param)
        {
            //try
            //{
            //    var user = ConnectedUsers.Where(e => e.ConnectionId == pContextConnet).FirstOrDefault();
            //    if(user != null)
            //    {
                
            //        ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();                    
            //        for (int i = 0; i < param.Count; i++)
            //        {
            //            Message row = param[i];                      
            //            var numberPhone = row.chatId.Split('@')[0].ToString().ToCharArray();
            //            string numPhone = "", code = "";
            //            int oks = 1;
            //            for (int gg = numberPhone.Length - 1; gg >= 0; gg--)
            //            {
            //                if (oks < 11)
            //                    numPhone = numberPhone[gg].ToString() + numPhone;
            //                else
            //                    code = numberPhone[gg].ToString() + code;
            //                oks++;
            //            }
            //            dynamic aar = data.StoreMessagesByClient(row, Convert.ToInt32(user.idCampaign), numPhone.ToString(), user.idEmployee);
            //        }
            //        Clients.Caller.storeMessages("Se ha registrado todo cambio en la BD sin problemas");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Clients.Caller.storeMessages(ex.Message);
            //}
        }

        public DateTime UnixTimeToDateTime(long unixtime)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (sessionUrlHttp.Contains("localhost"))
                dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
            else
                dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime().AddHours(-5);
            return dtDateTime;
        }
    }
}
