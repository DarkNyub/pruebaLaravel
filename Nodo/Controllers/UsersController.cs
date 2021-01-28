using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.IO;
using System.Security.Cryptography;

namespace Nodo.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            Session["url"] = "user";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");
            }

            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            if(Convert.ToInt32(Session["idProfile"]) == 1)
                ViewBag.row= data.ObtenerData("SP_getEmployee").Rows;
            else
            {
                dynamic rowCampaing = data.getDataCampaingByUSer(Convert.ToInt32(Session["idClient"])).Rows[0];
                ViewBag.row = data.getEmployeeCampaign(Convert.ToInt32(rowCampaing["idCampaigns"]), 1).Rows;
            }
            ViewBag.rowActivity = data.ObtenerData("SP_getActivity").Rows;

            return View("/Views/Users/table.cshtml");
        }
        public ActionResult Create()
        {
            Session["url"] = "user";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");
            }
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            ViewBag.rowProfiles = data.ObtenerData("SP_getProfiles").Rows;
            
            return View("/Views/Users/create.cshtml");
        }
        public ActionResult StoreUser(Models.employee model)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            var nomaas = Request.Files;
            for (var i = 0; i < nomaas.Count; i++)
            {
                if (nomaas[i].ContentLength > 0)
                {
                    string FileName = Path.GetFileNameWithoutExtension(nomaas[i].FileName);
                    string FileExtension = Path.GetExtension(nomaas[i].FileName);
                    FileName = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + FileName.Trim() + FileExtension;
                    string UploadPath = System.Web.HttpContext.Current.Server.MapPath("/Content/options/uploads/");
                    string stringimg = "/Content/options/uploads/" + FileName;
                    nomaas[i].SaveAs(UploadPath + FileName);
                    if(i == 0)
                        model.srcImage = "/Content/options/uploads/" + FileName;
                    if(i == 1)
                        model.srcImageBack = "/Content/options/uploads/" + FileName;
                }
            }

            CredencialesDeAcceso acceso = new CredencialesDeAcceso();
            string contrasena = acceso.creacionContrasena();
            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.GenerateKey();
            myRijndael.GenerateIV();
            string usuario = acceso.creacionUsuario(model.firstName, model.lastName).ToLower();
            model.user_cur = usuario;
            Byte[] contrasenaEncriptada = acceso.EncryptStringToBytes(contrasena, myRijndael.Key, myRijndael.IV);
            //DataTable dt = data.actualizarContrasena(model.email.Trim(), contrasenaEncriptada, myRijndael.Key, myRijndael.IV);


            DataTable dt = data.saveEmployee(model, contrasenaEncriptada, myRijndael.Key, myRijndael.IV);
            DataRow row = dt.Rows[0];
            if (dt.Rows.Count == 1)
            {
                EnviarCorreos correoCreacion = new EnviarCorreos();
                string bodyCorreo = correoCreacion.ArmarCorreoElectronicoPrimerContacto(model.firstName, model.lastName, model.user_cur, contrasena);
                correoCreacion.EnviarCorreo(model.email, "Creación de usuario", "", bodyCorreo, correoCreacion.vcorreo, correoCreacion.vusuario,correoCreacion.vcontraseña, "");
                Session["message"] = "Usted va a recibir un correo con sus credenciales de acceso. Si no ve el correo verifique otros lugares.";
                Session["title"] = "Usuario creado con éxito";
                Session["type"] = "success";
            }
            else
            {
                Session["message"] = "Problemas creando el usuario, intentelo nuevamente.";
                Session["title"] = "Error";
                Session["type"] = "success";
            }
            return RedirectToAction("Index");
        }
        public JsonResult ChangeActivityUsers(int id)//id del empleado
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidActivity = Convert.ToInt32(Request["value"]);
            DataTable dd = data.ChangeActivityUser(id, vidActivity);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
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
