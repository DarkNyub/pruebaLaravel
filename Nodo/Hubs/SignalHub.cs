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
        static List<employee> ConnectedUsers = new List<employee>();
        static List<Configuration> LConfig = new List<Configuration>();
        static string sessionUrlHttp = "", VTOKEN = "", VINSTANCE = "", VURL = "";

        public void Connect( int UserID, string urlHttp)
        {
            var id = Context.ConnectionId;
            sessionUrlHttp = urlHttp;
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();

            dynamic RowUser = data.getEmployee(UserID).Rows[0];
            dynamic ow = data.getDataCampaingByUSer(UserID).Rows;
            int icaps = 0;
            if (ow.Count > 0)
                icaps = Convert.ToInt32(ow[0]["idCampaigns"]);
            if (ConnectedUsers.Count(x => x.idEmployee == UserID) == 0)
            {
                ConnectedUsers.Add(new employee {
                    ConnectionId = id,
                    idEmployee = UserID,
                    idprofile = RowUser["idProfile"].ToString(),
                    user_cur = RowUser["user_cur"].ToString(),
                    firstName = RowUser["firstName"].ToString(),
                    lastName = RowUser["lastName"].ToString(),
                    srcImage = RowUser["srcImage"].ToString(),
                    idActivity = RowUser["idActivity"].ToString(),
                    nameActivity = RowUser["nameActivity"].ToString(),
                    iconActivity = RowUser["iconActivity"].ToString(),
                    isMessage = 0,
                    idCampaign = icaps
                });
            }
            employee current = ConnectedUsers.FirstOrDefault(e => e.idEmployee == UserID);
            if (vcount == 1)
            {
                dynamic holas = data.getConfigurationByCampaign(icaps).Rows;
                try
                {
                    foreach (dynamic rows in holas)
                    {
                        LConfig.Add(new Configuration { idConfig = rows["idConfig"], idCampaign = rows["idCampaign"], name = rows["name"], value = rows["value"] });
                    }
                    VTOKEN = LConfig.Where(ee => ee.name == "token").First().value;
                    VINSTANCE = LConfig.Where(ee => ee.name == "instance").First().value;
                    VURL = LConfig.Where(ee => ee.name == "urlApi").First().value;
                }
                catch (Exception ex)
                {
                    string sa = ex.Message;
                }
            }
            vcount++;
            // send to caller
            Clients.Caller.onConnected("Empleado agregado sin problemas, con este id de conexion "+ current.ConnectionId, current.ConnectionId, icaps, ConnectedUsers);
            // send to all except caller client
            //Clients.AllExcept(CurrentUser.ConnectionId).onNewUserConnected(CurrentUser.UserID.ToString(), CurrentUser.UserName, CurrentUser.UserID, CurrentUser.profileImg);
        }
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
                ConnectedUsers.Remove(item);
            return base.OnDisconnected(stopCalled);
        }
        public void notifySelectedClient(int pidEmployee, string pNumberPhone,int pidCampaign, String pNotification)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            data.StoreSelectedClientByAgent(pidEmployee, pNumberPhone, pidCampaign);
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
            
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();        
            var id = pContextConnet;            
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
             for (int i = 0; i < ConnectedUsers.Count(); i++){
                 employee rowE = ConnectedUsers[i];
     
                 {
                     if (rowE.isMessage == 1 && rowE.isOccupied == 1 && rowE.lastNumberChat == pChatNumber)
                         entre = 1;
                 }
                 rowClient = data.getDataClientByNumber(numPhone).Rows[0];
             }

            data.updatePhoneAwaitAgentChatList(numPhone);//esto es para que salga esperando atencion.
             var vClientName = "";
             if (rowClient != null)
                 vClientName = rowClient["firstName"].ToString() + " " + rowClient["LastName"].ToString();
             if (entre == 1)
             {
                 for (int i = 0; i < ConnectedUsers.Count(); i++)
                 {
                     employee rowE = ConnectedUsers[i];
                     if (rowE.isMessage == 1 && rowE.isOccupied == 1 && plastMessageNumber == pChatNumber)
                     {
                         Clients.Client(id).notifyNewMessage(pChatNumber);
                         data.storeLog("Llegó un mensaje de este número: "+ pChatNumber +" y este agente es el que lo tiene: "+ id);
                    }
                    
                 }
                string notification = notifyMessage.messages[0].fromMe.ToString();
                Clients.Caller.notifyNewMessage(pChatNumber, notification);
            }
             else        
            Clients.Caller.notifyNewMessage(null, numPhone, vClientName);
        }


        //inserta los Chats en la tabla TB_DifusionMessage
        public void reciveNewMessageDifussion(string pContextConnet, Models.NotifyMessage notifyMessage, string plastMessageNumber)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();

            var user = ConnectedUsers.Where(e => e.ConnectionId == pContextConnet).FirstOrDefault();
            string pNumberClient = (notifyMessage.messages[0].author.Split('@')[0].ToString()).Substring(2, 10);

            var time1 = UnixTimeToDateTime(Convert.ToInt64(notifyMessage.messages[0].time.ToString()));
            var time = time1.ToString("yyyy-MM-dd HH:mm");

            data.StoreDifusionMessagesByClient(notifyMessage, Convert.ToInt32(user.idCampaign), user.idEmployee, pNumberClient, time);
        }

        public async Task setClientsAsync(string pContextConnet, int pidEmployee, string pNumberPhone = "")
        {
            try
            {
                int COUNT_CHAT_LIST = 50;
                string notPhones = "";
                //int countAgent = 0;
                var id = Context.ConnectionId;
                for (int i = 0; i < ConnectedUsers.Count; i++)
                {
                    if (ConnectedUsers[i].ConnectionId == pContextConnet)
                    {
                        ConnectedUsers[i].isMessage = 1;
                        //i = ConnectedUsers.Count;
                    }
                    if(ConnectedUsers[i].isOccupied == 1)
                    {
                        notPhones += ConnectedUsers[i].lastNumberChat;
                        if (i < ConnectedUsers.Count)
                            notPhones += ",";
                    }
                }
                var user = ConnectedUsers.Where(e => e.ConnectionId == pContextConnet).FirstOrDefault();
                ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
                dynamic rowCampaing = user.idCampaign;
                string UrlApi = "instance" + VINSTANCE + "/dialogs";
                listDialogs vChats = new listDialogs();
                HttpResponseMessage result;
                //List<employee> LEmployees = new List<employee>();
                if (rowCampaing > 0)
                {
                    int vIdCampaign = rowCampaing;
                    /*for (int i = 0; i < ConnectedUsers.Count; i++)
                    {
                        if (ConnectedUsers[i].isMessage == 1 && ConnectedUsers[i].idCampaign == vIdCampaign)
                        {
                            LEmployees.Add(ConnectedUsers[i]);
                            countAgent++;

                        }
                    }*/
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
                    vChats.dialogs = vChats.dialogs.Where(e => !e.id.Contains('-')).OrderByDescending(e=>e.last_time).Take((COUNT_CHAT_LIST)).ToList();
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
                                dtfinal.Rows[dtfinal.Rows.Count - 1][0] = vIdCampaign;
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
                    notPhones = notPhones.Trim(',').ToString();
                    DataTable dt = data.StoreDataChatList(dtfinal, vIdCampaign, notPhones);
                    LChats = dt.AsEnumerable().Select(m => new ChatList()
                    {
                        firstName = m.Field<dynamic>("firstName"),
                        LastName = m.Field<dynamic>("LastName"),
                        oid = m.Field<dynamic>("oid"),
                        idCampaign = m.Field<dynamic>("idCampaign"),
                        image = m.Field<dynamic>("image"),
                        lastUsedUnix = m.Field<dynamic>("respuesta"),
                        lastUsed = m.Field<dynamic>("lastUsed"),
                        phoneNumber = m.Field<dynamic>("phoneNumber"),
                        idClient = m.Field<dynamic>("idClient"),
                        lastIdEmployee = m.Field<dynamic>("lastIdEmployee"),
                        nameEmployee = m.Field<dynamic>("nameEmployee")
                    }).ToList();
                    Clients.Caller.GetClients(LChats, true);
                }
                else
                    Clients.Caller.GetClients(null, true);
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
                ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();

                string menssages = LConfig.Where(ee => ee.name == "urlMessages").First().value;
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
                        for(int i = 0; i < ConnectedUsers.Count; i++)
                        {
                            if(ConnectedUsers[i].idEmployee == pidEmployee)
                            {
                                ConnectedUsers[i].isOccupied = 1;
                                ConnectedUsers[i].lastNumberChat = pidNumber;
                            }
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

                string menssages = LConfig.Where(ee => ee.name == "urlMessages").First().value;
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
                            for (int i = 0; i < ConnectedUsers.Count; i++)
                            {
                                if (ConnectedUsers[i].idEmployee == pidEmployee)
                                {
                                    ConnectedUsers[i].isOccupied = 1;
                                    ConnectedUsers[i].lastNumberChat = pidNumber;
                                }
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
            try
            {
                var user = ConnectedUsers.Where(e => e.ConnectionId == pContextConnet).FirstOrDefault();
                if(user != null)
                {
                
                    ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();                    
                    for (int i = 0; i < param.Count; i++)
                    {
                        Message row = param[i];                      
                        var numberPhone = row.chatId.Split('@')[0].ToString().ToCharArray();
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
                        dynamic aar = data.StoreMessagesByClient(row, Convert.ToInt32(user.idCampaign), numPhone.ToString(), user.idEmployee);
                    }
                    Clients.Caller.storeMessages("Se ha registrado todo cambio en la BD sin problemas");
                }
            }
            catch (Exception ex)
            {
                Clients.Caller.storeMessages(ex.Message);
            }
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
