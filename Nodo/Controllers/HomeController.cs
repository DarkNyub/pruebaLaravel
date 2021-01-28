using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nodo.Hubs;
using Nodo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace Nodo.Controllers
{
    
    public class HomeController : Controller
    {
        public JsonResult explusa()
        {
            return Json(new { result = Session["hola"] }, JsonRequestBehavior.AllowGet);
        }
        // GET: Home
        public ActionResult Index()
        {
            return View("/Views/Home/login.cshtml");
        }
        public ActionResult SingIn(Models.Login model)
        {
            try
            {
                Session["idClient"] = null;
                Session["idProfile"] = null;
                Session["name"] = null;
                Session["image"] = null;
                Session["email"] = null;
                Session["imageBack"] = null;
                Session["config"] = null;
                Session["activity"] = null;
                Session["idActivity"] = null;

                if (model.usuario != null)
                {
                    ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
                    CredencialesDeAcceso acceso = new CredencialesDeAcceso();
                    DataTable dt = data.ValidarIngresoUsuario(model.usuario, GetMACAddress().ToString());
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        byte[] contrasena = (byte[])row["password"];
                        byte[] key = (byte[])row["pKEY"];
                        byte[] iv = (byte[])row["PIV"];
                        if (contrasena.Length > 2)
                        {
                            string contrasenaFinal = acceso.DecryptStringFromBytes(contrasena, key, iv);
                            if (contrasenaFinal == model.contrasena)
                            {
                                if (Convert.ToInt32(row["idEstado"]) > 1)
                                    Session["message"] = "Este usuario se encuentra inactivo, comuníquese con el administrador.";
                                else
                                {
                                    //data.ChangeActivityUser(Convert.ToInt32(row["idEmployee"]), 1);
                                    Session["idClient"] = row["idEmployee"].ToString();
                                    Session["idProfile"] = row["idProfile"].ToString();
                                    Session["nombre"] = row["nombre"].ToString();
                                    Session["image"] = row["srcImage"].ToString();
                                    Session["email"] = row["email"].ToString();
                                    Session["imageBack"] = row["srcImageBack"].ToString();
                                    Session["NameProfile"] = row["perfil"].ToString();
                                    Session["idActivity"] = data.ChangeActivityUser(Convert.ToInt32(row["idEmployee"]), 1).Rows[0]["idActivity"].ToString();
                                    dynamic ow = data.getDataCampaingByUSer(Convert.ToInt32(row["idEmployee"])).Rows;
                                    Session["idCampaign"] = null;
                                    Session["nameCampaign"] = "Campaña no activa";
                                    dynamic holas = "";
                                    if (ow.Count > 0)
                                    {
                                        Session["idCampaign"] = ow[0]["idCampaigns"].ToString();
                                        Session["nameCampaign"] = ow[0]["nameCampaign"].ToString();
                                        holas = data.getConfigurationByCampaign(Convert.ToInt32(ow[0]["idCampaigns"])).Rows;
                                        List<Models.Configuration> LConfig = new List<Models.Configuration>();
                                        foreach (dynamic rows in holas)
                                        {
                                            LConfig.Add(new Models.Configuration { idConfig = rows["idConfig"], name = rows["name"], value = rows["value"] });
                                        }
                                        Session["config"] = LConfig;
                                    }
                                    holas = data.ObtenerData("SP_getActivity").Rows;
                                    List<Models.Activity> LActivity = new List<Models.Activity>();
                                    foreach (dynamic rows in holas)
                                    {
                                        LActivity.Add(new Models.Activity { idActivity = rows["idActivity"], name = rows["name"], icon = rows["icon"] });
                                    }
                                    Session["activity"] = LActivity;

                                    return RedirectToAction("Dashboard", "Home");

                                }
                            }
                            else
                                Session["message"] = "El usuario y contraseña no coinciden, por favor validar.";
                        }
                    }
                    else
                        Session["message"] = "No se encontró un nombre de usuario con esas credenciales, por favor cree uno.";
                }
                else
                    Session["message"] = "Los campos estan vacios, por favor verifique.";

                Session["title"] = "Error";
                Session["type"] = "error";
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                string sas = e.Message;
                throw;
            }
        }
        // public JsonResult NewMessage()
        //{


        public JsonResult NewMessage(Models.NotifyMessage pbody)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<SignalHub>();

            if (pbody.messages != null)
            {
                if (!pbody.messages[0].chatId.Contains('-'))
                {
                    var numberPhone = pbody.messages[0].chatId.Split('@')[0].ToString().ToCharArray();
                    string numPhone = "", numPhone1 = "", code = "";
                    int oks = 1;
                    for (int gg = numberPhone.Length - 1; gg >= 0; gg--)
                    {
                         numPhone1 = numberPhone[gg].ToString() + numPhone;

                        if (oks < 11)
                            numPhone = numberPhone[gg].ToString() + numPhone;
                        else
                            code = numberPhone[gg].ToString() + code;
                        oks++;
                    }
                   // if (Convert.ToInt32(code) == 57)
                        context.Clients.All.reciveNewMessage(pbody, numPhone1);
                }
            }
            return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
        }

        /*
          public JsonResult NewMessage()
        {
           
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            var context = GlobalHost.ConnectionManager.GetHubContext<SignalHub>();
            string pbody = "";
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var body = reader.ReadToEndAsync();
                    pbody = body.Result;
                }
                NotifyMessage vChats = JsonConvert.DeserializeObject<NotifyMessage>(pbody);
                if (vChats.messages != null)
                {
                    if (!vChats.messages[0].chatId.Contains('-'))
                    {
                        var numberPhone = vChats.messages[0].chatId.Split('@')[0].ToString().ToCharArray();
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
                        if (Convert.ToInt32(code) == 57)
                        {
                            dynamic dt = data.getDataCampaignByInstance(vChats.instanceId).Rows;
                            if (dt.Count > 0)
                            {
                                //_ = SendMessage(pbody.messages[0].chatId, pbody.messages[0].body + " estoy devolviendo el mensaje porque se que llego bien a la plataforma", dt[0]["idCampaigns"]);
                                //bool entreBot = false;
                                //if (Convert.ToInt32(dt[0]["isBot"]) == 1)
                               // {
                                //    dynamic rowBot = data.getStatusBotClient(numPhone).Rows;
                                 //   if (rowBot.Count > 0)
                                 //   {
                                  //      if (Convert.ToInt32(rowBot[0]["idBotIntent"]) != Convert.ToInt32(dt[0]["agentCode"]))
                                   //         entreBot = true;
                                   // }
                               // }
                                //if (!entreBot)
                                context.Clients.All.reciveNewMessage(vChats);
                            }
                        }
                    }
                }
                return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                //_ = SendMessage("573127888573@c.us"," ocurrio un error y este es el mensaje "+ex.Message, 1);
                //_ = SendMessage("573103067897@c.us", " ocurrio un error y este es el mensaje " + ex.Message, 1);
                //data.storeLog("ocurrio un error al enviar a este numero: " + " y este es el error: " + ex.Message);
                return Json(new { result = 1 }, JsonRequestBehavior.AllowGet);
            }
        }
           */
        

        public async Task<string> SendMessage(string chatID, string text, int pidCampaign)
        {
            var data = new Dictionary<string, string>() { { "chatId", chatID }, { "body", text } };
            return await SendRequest("sendMessage", JsonConvert.SerializeObject(data), pidCampaign);
        }

        public async Task<string> SendRequest(string method, string data, int pidCampaign)
        {
            ConnectionDataBase.StoreProcediur dtos = new ConnectionDataBase.StoreProcediur();
            List<Configuration> LConfig = new List<Configuration>();
            dynamic vConfig = dtos.getConfigurationByCampaign(pidCampaign).Rows;
            foreach (dynamic rows in vConfig)
            {
                LConfig.Add(new Configuration { idConfig = rows["idConfig"], idCampaign = rows["idCampaign"], name = rows["name"], value = rows["value"] });
            }
            string VTOKEN = LConfig.Where(ee => ee.name == "token").First().value;
            string VINSTANCE = LConfig.Where(ee => ee.name == "instance").First().value;
            string VURL = LConfig.Where(ee => ee.name == "urlApi").First().value;
            var VURLMESSAGE = method;
            string UrlApi = "instance" + VINSTANCE + "/" + VURLMESSAGE;

            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(VURL);
                    var content = new StringContent(data, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync(UrlApi + "?token=" + VTOKEN, content);
                    string valor = await result.Content.ReadAsStringAsync();
                    //dtos.storeLog("por fin se envio el mensajes y dice esto: " + valor);
                    return valor;
                }
                catch (Exception ex)
                {
                    dtos.storeLog("ocurrio un error al enviar a este numero: " + data + " y este es el error: " + ex.Message);
                    return ex.Message;
                }
            }
        }

        public ActionResult ForgotPassword()
        {
            return View("/Views/Home/passwordRecovery.cshtml");
        }
        public ActionResult SendPasswordRecovery(Models.Login model)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            CredencialesDeAcceso acceso = new CredencialesDeAcceso();
            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.GenerateKey();
            myRijndael.GenerateIV();
            string contrasena = acceso.creacionContrasena();
            Byte[] contrasenaEncriptada = acceso.EncryptStringToBytes(contrasena, myRijndael.Key, myRijndael.IV);
            DataTable dt = data.actualizarContrasena(model.usuario.Trim(), contrasenaEncriptada, myRijndael.Key, myRijndael.IV);
            EnviarCorreos correoCreacion = new EnviarCorreos();
            //correoCreacion.EnviarCorreo(model.email, "Universidad de Voluntarios", "transborder1@hotmail.com", bodyCorreo, "transborder1@hotmail.com", "transborder1@hotmail.com", "trans#123", "");
            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                string bodyCorreo = correoCreacion.ArmarCorreoRecuperacionContrasena(row["nombre"].ToString(), contrasena, 0, row["phoneMobile"].ToString(), Session["urlHttp"].ToString());
                correoCreacion.EnviarCorreo(row["email"].ToString(), "NODO WS - Recuperacion de contraseña", "", bodyCorreo, correoCreacion.vusuario, correoCreacion.vcorreo, correoCreacion.vcontraseña, "");

                Session["title"] = "Muy bien";
                Session["type"] = "success";
                Session["message"] = "Se ha enviado un correo con las instrucciones, por favor validar.";
            }
            else
            {
                Session["error"] = "error";
                Session["title"] = "Correo no encontrado";
                Session["message"] = "Por favor validar el correo que se encuentra digitando";
                return RedirectToAction("ForgotPassword");
            }
            return RedirectToAction("Index");
        }
        public ActionResult RenewPassword(string id)
        {
            if (Session["type"] == null)
                Session.RemoveAll();

            Session["customerId"] = id;
            return View("/Views/Home/updatePassword.cshtml");
        }
        public ActionResult UpdatePassword(Models.Login model)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            CredencialesDeAcceso acceso = new CredencialesDeAcceso();
            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.GenerateKey();
            myRijndael.GenerateIV();
            if (model.newcontrasena == model.confirmcontrasena)
            {
                string sub = Session["customerId"].ToString();
                Byte[] contrasenaEncriptada = acceso.EncryptStringToBytes(model.newcontrasena, myRijndael.Key, myRijndael.IV);
                DataTable dtd = data.validacionContrasenaActual(sub);
                DataRow rowd = dtd.Rows[0];
                byte[] contrasena = (byte[])rowd["password"];
                byte[] key = (byte[])rowd["pKEY"];
                byte[] iv = (byte[])rowd["PIV"];
                string contrasenaFinal = acceso.DecryptStringFromBytes(contrasena, key, iv);
                if (contrasenaFinal != model.newcontrasena)
                {
                    Byte[] contrasenaEncriptadaAntigua = acceso.EncryptStringToBytes(model.contrasenatemp, myRijndael.Key, myRijndael.IV);
                    DataTable dt = data.actualizarContrasenaConfirmacion(sub, contrasenaEncriptada, myRijndael.Key, myRijndael.IV, contrasenaEncriptadaAntigua, myRijndael.Key, myRijndael.IV);
                    DataRow row = dt.Rows[0];
                    if (dt.Rows.Count > 0)
                    {
                        Session["message"] = "Contraseña cambiada con éxito, por favor ingrese con su usuario y nueva contraseña.";
                        Session["type"] = "success";
                        Session["title"] = "Muy bien";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    Session["message"] = "Contraseña nueva es igual es la contraseña guardada.";
                }
            }
            else
            {
                Session["message"] = "Las contraseñas no coinciden, por favor validar.";
            }
            Session["type"] = "error";
            Session["title"] = "Error";
            return RedirectToAction("RenewPassword", new { @id = Session["customerId"] });
        }
        
        public ActionResult Dashboard()
        {
            if(Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index");
            }
            Session["url"] = "dash";
            return View("/Views/Home/dashboard.cshtml");
        }



        public ActionResult LogOut()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            data.ChangeActivityUser(Convert.ToInt32(Session["idClient"]), 4);
            Session.RemoveAll();
            return RedirectToAction("Index");
        }


        /********************/
        public string GetMACAddress()
        {
            System.Net.NetworkInformation.NetworkInterface[] nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (System.Net.NetworkInformation.NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    System.Net.NetworkInformation.IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAddress;
        }
        /***********************/

    }
}