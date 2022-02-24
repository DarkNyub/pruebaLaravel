using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using Newtonsoft.Json;
using Nodo.Models;
using System.Text;

namespace Nodo.Controllers
{
    public class MessagesController : Controller
    {
        static List<Configuration> LConfig = new List<Configuration>();
        // GET: Messages
        public ActionResult Index()
        {
            Session["url"] = "mes";
            Session["idClient"] = "1";
            Session["idProfile"] = "1";
            //if (Session["idClient"] == null)
            //{
            //    Session["message"] = "Tiene que ingresar a la aplicación primero.";
            //    Session["type"] = "error";
            //    Session["title"] = "Error";
            //    return RedirectToAction("Index", "Home");
            //}

            return View("/Views/Messages/tableMaster.cshtml");
        }
        /******************* SmartData ********************/
        public ActionResult smartDataPage()
        {

            Session["url"] = "mes";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");
            }
            return View("/Views/Messages/Partials/smartDataPage.cshtml");
        }

        public async Task<JsonResult> VerifyClientSmartDataAsync()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable vConfig = data.getConfigurationByCampaign(Convert.ToInt32(Session["idCampaign"]));
            LConfig = vConfig.AsEnumerable().Select(m => new Configuration()
            {
                idConfig = m.Field<dynamic>("idConfig"),
                idCampaign = m.Field<dynamic>("idCampaign"),
                name = m.Field<dynamic>("name"),
                value = m.Field<dynamic>("value")
            }).ToList();
            int vTypeDocu = Convert.ToInt32(Request["typedocu"]);
            string vDocument = Request["document"].ToString();
            dynamic result;
            ClientSmartData vClientSmart = new ClientSmartData();
            string VURL = LConfig.Where(ee => ee.name == "smartDataApiUrl").First().value;
            string resultContent = "";
            using (var client = new HttpClient())
            {
                string vToken = await getTokenSmartData();
                client.BaseAddress = new Uri(VURL + "api/Cliente/" + vTypeDocu.ToString() + "/" + vDocument);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", vToken);
                result = await client.GetAsync("");
                if (result.IsSuccessStatusCode)
                {
                    resultContent = await result.Content.ReadAsStringAsync();
                    vClientSmart = JsonConvert.DeserializeObject<ClientSmartData>(resultContent);
                    return Json(vClientSmart, JsonRequestBehavior.AllowGet);
                    //dynamic olas = resultContent;//.Replace("\"", "'");
                }
                else
                    return Json(new { result = result.StatusCode.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<string> getTokenSmartData(string pUser = "api@dipaola.com", string pPassword = "2o?z1krC")
        {
            try
            {
                pUser = LConfig.Where(ee => ee.name == "smartDataUser").First().value;
                pPassword = LConfig.Where(ee => ee.name == "smartDataPass").First().value;
                string VURL = LConfig.Where(ee => ee.name == "smartDataApiUrl").First().value;
                TokenSmartData vToken = new TokenSmartData();
                var data = new Dictionary<string, string>() { { "username", pUser }, { "password", pPassword } };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                dynamic result;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(VURL + "api/authorization/Authenticate");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    result = await client.PostAsync("", content);
                    if (result.IsSuccessStatusCode)
                    {
                        var resultContent = await result.Content.ReadAsStringAsync();
                        dynamic olas = resultContent;//.Replace("\"", "'");
                        vToken = JsonConvert.DeserializeObject<TokenSmartData>(olas);
                    }
                }
                return vToken.token;
            }
            catch (Exception ex)
            {
                string asda = ex.Message;
                throw;
            }
        }
        public async Task<JsonResult> FindCitySmartData()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable vConfig = data.getConfigurationByCampaign(Convert.ToInt32(Session["idCampaign"]));
            LConfig = vConfig.AsEnumerable().Select(m => new Configuration()
            {
                idConfig = m.Field<dynamic>("idConfig"),
                idCampaign = m.Field<dynamic>("idCampaign"),
                name = m.Field<dynamic>("name"),
                value = m.Field<dynamic>("value")
            }).ToList();
            List<CityOWL> LCities = new List<CityOWL>();
            try
            {
                dynamic osk = "";
                dynamic result;
                string VURL = LConfig.Where(ee => ee.name == "smartDataApiUrl").First().value;
                using (var client = new HttpClient())
                {
                    string vToken = await getTokenSmartData();
                    client.BaseAddress = new Uri(VURL + "api/Ciudad");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", vToken);
                    result = await client.GetAsync("");
                    if (result.IsSuccessStatusCode)
                    {
                        string resultContent = await result.Content.ReadAsStringAsync();
                        dynamic olas = resultContent.Replace("nombre", "text");//.Replace("\"", "'");
                        LCities = JsonConvert.DeserializeObject<List<CityOWL>>(olas);
                    }
                }
                return osk;
            }
            catch (Exception ex)
            {
                string asas = ex.Message;
            }
            return Json(LCities, JsonRequestBehavior.AllowGet);
        }
        public JsonResult VerifyClientDataBase()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            string vPhoneNumber = Request["phonenumber"].ToString();
            DataTable dd = data.getDataClientByNumber(vPhoneNumber);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SaveClientToSmartData()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            string vDataString = await storeClientSmartData(Request["email"].ToString(), Request["lastnameClient"].ToString(), Request["nameClient"].ToString()
                                                       , Request["numDocument"].ToString(), Request["idTypeDocument"].ToString(), Request["idClientCity"].ToString()
                                                       , Request["address"].ToString(), Request["birthdate"].ToString());
            dynamic entre = 0;
            if (vDataString.Split('%')[0].ToString() == "OK")
            {
                data.storeSmartDataClientIdToDatabase(Convert.ToInt32(Request["idClient"]), Convert.ToInt32(Request["idCampaigns"]), vDataString.Split('%')[1].ToString());
                _ = storePhoneClientSmartData(vDataString.Split('%')[1].ToString(), Request["numDocument"].ToString());
                return Json(new { resultado = 0 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                entre = (vDataString.Split('%')[1].ToString());
                return Json(entre, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<string> storeClientSmartData(string pemail, string papellidos, string pnombre, string pnumeroIdentificacion, string ptipoDocumentoId, string pciudadId, string pdireccion, string pNacimiento)
        {
            try
            {
                ConnectionDataBase.StoreProcediur dts = new ConnectionDataBase.StoreProcediur();
                string osk = "";
                var data = new Dictionary<string, dynamic>() {
                    { "email", pemail },{"apellidos", papellidos}, { "nombre", pnombre},
                    { "numeroIdentificacion", pnumeroIdentificacion}, { "tipoDocumentoId", ptipoDocumentoId},{ "ciudadId", pciudadId },
                    {"direccion", pdireccion},{"aceptaHabeas",1}, { "fechaTerminos",DateTime.Now.ToLocalTime().AddHours(-5) },
                    { "nacimiento", pNacimiento}
                };
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                /*dynamic result;
                string VURL = LConfig.Where(ee => ee.name == "smartDataApiUrl").First().value;
                using (var client = new HttpClient())
                {
                    string vToken = await getTokenSmartData();
                    client.BaseAddress = new Uri(VURL + "api/Cliente");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", vToken);
                    result = await client.PostAsync("", content);
                    var resultContent = await result.Content.ReadAsStringAsync();
                    if (result.IsSuccessStatusCode)//va dentro de los rango 200 -210
                    {
                        vToken = JsonConvert.DeserializeObject<String>(resultContent);
                        osk = result.StatusCode + "%" + vToken;
                    }
                    else
                        osk = result.StatusCode + "%"+ resultContent.ToString();
                }*/
                osk = "OK%dasafnaisbnakhsfbviasdbj";
                return osk;
            }
            catch (Exception ex)
            {
                string asd = ex.Message;
                throw;
            }

        }
        public async Task<string> storePhoneClientSmartData(string pidClientSmartData, string pNumberPhone)
        {
            try
            {
                ConnectionDataBase.StoreProcediur dts = new ConnectionDataBase.StoreProcediur();
                string osk = "";
                var data = new Dictionary<string, dynamic>() {
                    { "clienteId", pidClientSmartData },{"celular", pNumberPhone}
                };
                string VURL = LConfig.Where(ee => ee.name == "smartDataApiUrl").First().value;
                var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                dynamic result;
                using (var client = new HttpClient())
                {
                    string vToken = await getTokenSmartData();
                    client.BaseAddress = new Uri(VURL + "api/CelularCliente");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", vToken);
                    result = await client.PostAsync("", content);
                    /*if (result.IsSuccessStatusCode)
                    {
                        var resultContent = await result.Content.ReadAsStringAsync();
                        dynamic olas = resultContent;//.Replace("\"", "'");
                    }*/
                    var resultContent = await result.Content.ReadAsStringAsync();
                    if (result.IsSuccessStatusCode)
                    {
                        vToken = JsonConvert.DeserializeObject<String>(resultContent);
                        osk = result.StatusCode + "," + vToken;
                    }
                    else
                        osk = result.StatusCode + ",hay un error";
                }
                return osk;
            }
            catch (Exception ex)
            {
                string asd = ex.Message;
                throw;
            }

        }

        /********************** Fin SmartData ******************/
            public async Task<JsonResult> SendMessageText()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            string BASEURL = "https://api.api4bot.com/instance126372/";
            string URLWORK = Session["urlHttp"].ToString();
            string TOKEN = "4fj4wp5g54koce16";
            string UrlApi = "sendMessage";
            var nomaas = Request.Files;
            string UrlPath = "/Content/messages/others/";
            string vphone = Request["numberphone"].ToString();
            string vbody = Request["messagebody"].ToString();
            string vfilename = "";
            string vcaption = "";
            //dynamic myJson = new { chatId = "", phone = Request["numberphone"].ToString(), body = Request["messagebody"].ToString() };
            dynamic rowType = data.getTypeFiles().Rows;
            List<Models.TypeFiles> LtypeF = new List<Models.TypeFiles>();
            foreach(dynamic row in rowType)
            {
                LtypeF.Add(new Models.TypeFiles { idTypeFile =row["idTypeFile"], nameType = row["nameType"], idGroupFile = row["idGroupFile"], nameGroup = row["nameGroup"], srcFile = row["srcFile"] });
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BASEURL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                for (var i = 0; i < nomaas.Count; i++)
                {
                    if (nomaas[i].ContentLength > 0)
                    {
                        UrlApi = "sendFile";
                        vfilename = Path.GetFileNameWithoutExtension(nomaas[i].FileName);
                        string FileExtension = Path.GetExtension(nomaas[i].FileName);

                        Models.TypeFiles vtypeF = LtypeF.Where(u => u.nameType.ToString().ToUpper().Contains(FileExtension.Replace(".", "").ToUpper())).FirstOrDefault();
                        if (vtypeF != null)
                        {
                            UrlPath = vtypeF.srcFile.ToString();
                            if (Convert.ToInt32(vtypeF.idGroupFile) == 3)
                                UrlApi = "sendPTT";
                        }

                        vfilename = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + vfilename.Trim() + FileExtension;
                        string UploadPath = System.Web.HttpContext.Current.Server.MapPath(UrlPath);
                        nomaas[i].SaveAs(UploadPath + vfilename);
                        vcaption = vbody;
                        vbody = URLWORK + UrlPath + vfilename;
                    }
                }
                dynamic result = "";
                if(UrlApi.Contains("Message"))
                    result = await client.PostAsJsonAsync(UrlApi + "?token="+ TOKEN, new { chatId = "", phone = vphone, body = vbody });
                if (UrlApi.Contains("File"))
                    result = await client.PostAsJsonAsync(UrlApi + "?token=" + TOKEN, new { chatId = "", phone = vphone, body = vbody, filename = vfilename, caption = vcaption, audio = vbody });
                if (UrlApi.Contains("PTT"))
                    result = await client.PostAsJsonAsync(UrlApi + "?token=" + TOKEN, new { chatId = "", phone = vphone, audio = vbody });
                string resultContent = await result.Content.ReadAsStringAsync();
                Session["return"] = resultContent.Replace("\"", "'");
            }

            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult StoreFileClientAsync()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            List<Nodo.Models.Configuration> Lconfig = (List<Nodo.Models.Configuration>)Session["config"];
            string URLWORK = Session["urlHttp"].ToString();
            string UrlApi = "sendFile";
            string UrlPath = "/Content/messages/others/";
            string vfilename = "";
            string vbody = "";
            List<Models.TypeFiles> LtypeF = new List<Models.TypeFiles>();
            LtypeF.Add(new Models.TypeFiles { nameType = "doc", idGroupFile = "1"});
            LtypeF.Add(new Models.TypeFiles { nameType = "docx", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "txt", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "xls", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "xlsx", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "pdf", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "zip", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "png", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "jpeg", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "jpg", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "webp", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "gif", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "mp4", idGroupFile = "1" });
            LtypeF.Add(new Models.TypeFiles { nameType = "mp3", idGroupFile = "3" });
            LtypeF.Add(new Models.TypeFiles { nameType = "ogg", idGroupFile = "3" });
            //using (var client = new HttpClient())
            //{
            //client.BaseAddress = new Uri(BASEURL);
            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var nomaas = Request.Files;
            string FileExtension = "";
            for (var i = 0; i < nomaas.Count; i++)
            {
                if (nomaas[i].ContentLength > 0)
                {
                    vfilename = Path.GetFileNameWithoutExtension(nomaas[i].FileName);
                    FileExtension = Path.GetExtension(nomaas[i].FileName);

                    Models.TypeFiles vtypeF = LtypeF.Where(u => u.nameType.ToString().ToUpper().Contains(FileExtension.Replace(".", "").ToUpper())).FirstOrDefault();
                    if (vtypeF != null)
                    {
                        //UrlPath = vtypeF.srcFile.ToString();
                        if (Convert.ToInt32(vtypeF.idGroupFile) == 3)
                            UrlApi = "sendPTT";
                    }

                    vfilename = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + vfilename.Trim() + FileExtension;
                    string UploadPath = System.Web.HttpContext.Current.Server.MapPath(UrlPath);
                    nomaas[i].SaveAs(UploadPath + vfilename);
                    vbody = URLWORK + UrlPath + vfilename;
                }
            }
            //dynamic result = "";
            //if (UrlApi.Contains("File"))
            //    result = await client.PostAsJsonAsync(UrlApi+TOKEN, new { chatId = "", phone = vphone, body = vbody, filename = vfilename, caption = vcaption, audio = vbody});
            //if (UrlApi.Contains("PTT"))
            //    result = await client.PostAsJsonAsync(UrlApi + TOKEN, new { chatId = "", phone = vphone, audio = vbody});
            //resultContent = await result.Content.ReadAsStringAsync();
            //resultContent = resultContent.Replace("\"", "'");
           // }

            return Json(new { respuesta = 1, href = vbody, filetype = FileExtension, filename = vfilename, sendUrl = UrlApi }, JsonRequestBehavior.AllowGet);
        }

        /******** Comentarios hacia los clientes *****/
        public JsonResult getCommentByClient(int id)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable dd = data.getCommentByClient(id);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult storeComment()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidClient = Convert.ToInt32(Request["idClient"]);
            int vidEmployee = Convert.ToInt32(Session["idClient"]);
            string vcomment = Request["newCommnet"].ToString();
            int vidCampaign = Convert.ToInt32(data.getAllDataCampaingByUSer(vidEmployee).Rows[0]["idCampaigns"]);
            DataTable dd = data.storeNewComment(vidEmployee, vidClient, vcomment, vidCampaign);
            ViewBag.result = datatabletojson(dd);

            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        /********* fin Comentarios *******/
        /****** Campañas y catálogos ******/
        public JsonResult getCampaignCatalogByClient(int id)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidCampaing = Convert.ToInt32(Request["vidCampaign"]);
            DataTable dd = data.getCampaignCatalogByClient(id, vidCampaing);
            
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult storeClientCatalog()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidClient = Convert.ToInt32(Request["idClient"]);
            int vidCatalogFather = Convert.ToInt32(Request["idCatalogFather"]);
            int vidout = 0;
            string vidCatalogSon = Request["idCatalogSon"].ToString();
            if (Int32.TryParse(Request["idCatalogSon"].ToString(), out vidout))
                vidCatalogSon = "N-" + vidCatalogSon;
            else
            {
               // vidCatalogSon = Request["idCatalogSon"].ToString().Split('|')[0].ToString();
            }
            DataTable dd = data.storeClientCatalog(vidClient, vidCatalogFather, vidCatalogSon);
            ViewBag.result = datatabletojson(dd);
            DataTable dm = data.storeClientCatalogFormulario(vidClient, vidCatalogFather, vidCatalogSon);
            ViewBag.resultform = datatabletojson(dm);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        /********* End de cmapañas y catálogos *******/
        /********** Data Cliente ************/
        public JsonResult getDataClient(int id)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable dd = data.getDataClient(id);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult findClientByPhoneNumber()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            string vPhoneNumber = Request["pPhoneNumber"].ToString();
            int vidCampaign = Convert.ToInt32(Request["idCampaigns"]);
            DataTable dd = data.findClientByPhoneNumber(vidCampaign, vPhoneNumber);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FreeAllByAgent()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidEmployee = Convert.ToInt32(Session["idClient"]);
            DataTable dd = data.FreeAllByAgent(vidEmployee);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }

        
            public JsonResult FreeClientByAgent()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidEmployee = Convert.ToInt32(Session["idClient"]);//este es el agente
            int vidClient = Convert.ToInt32(Request["idClient"]);//este es el cliente
            DataTable dd = data.FreeClientByAgent(vidEmployee, vidClient);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FreeClientBotByAgent()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidEmployee = Convert.ToInt32(Session["idClient"]);//este es el agente
            int vidClient = Convert.ToInt32(Request["idClient"]);//este es el cliente
            DataTable dd = data.FreeClientBotByAgent(vidEmployee, vidClient);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        

        public JsonResult updateDataClient()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int pidClient = Convert.ToInt32(Request["idClient"]);
            int ptype = Convert.ToInt32(Request["type"]);
            string pdata = Request["data"].ToString();
            DataTable dd = new DataTable();
            if (ptype == 0)//nombre
            {
                dd = data.storeUpdateDataClient(pidClient, pdata, ptype);
            }
            if (ptype == 2)//apellido
            {
                dd = data.storeUpdateDataClient(pidClient, pdata, ptype);
            }
            if (ptype == 6)//genero
                dd = data.storeUpdateDataClient(pidClient, pdata.Split('-')[0].ToString(), ptype);

            if(dd.Rows.Count == 0)//todo lo demas
                dd = data.storeUpdateDataClient(pidClient, pdata, ptype);

            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult verifyBotCampaign()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            dynamic ow = data.getDataCampaingByUSer(Convert.ToInt32(Session["idClient"])).Rows;
            int vidCampaign = Convert.ToInt32(ow[0]["idCampaigns"]);
            DataTable dd = data.verifyBotCampaign(vidCampaign);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult changeClientToBot(int id)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable dd = data.changeClientToBot(id);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult changeClientToAgent(int id)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable dd = data.changeClientToAgent(id);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FinalizeChatClient()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidCampaing = Convert.ToInt32(Request["idCampaign"]);
            string phonenumber = Request["phoneNumber"].ToString();

            string vNumberPhoneChat = phonenumber.Substring(phonenumber.Length - 10);

            DataTable dd = data.updatePhoneAwaitAgentChatList(vNumberPhoneChat,-3, vidCampaing);

            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }


        /********** end data cliente ********/
        public string datatabletojson(DataTable table)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(table);
            return JSONString;
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}