using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Nodo.Controllers
{
    public class BroadcastMController : Controller
    {
        // GET: BroadcastM
        public ActionResult Index()
        {
            Session["url"] = "broad";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();

            dynamic rowCampaing = data.getDataCampaingByUSer(Convert.ToInt32(Session["idClient"])).Rows;
            if(rowCampaing.Count > 0)
                ViewBag.rowClients = data.getClientsCampaign(Convert.ToInt32(rowCampaing[0]["idCampaigns"])).Rows;
            else
            {
                Session["message"] = "- Este usuario no tiene una campaña asignada.<br>- La campaña no esta activa.<br>- Ambos Casos.";
                Session["type"] = "warning";
                Session["title"] = "Advertencia";
            }
            return View("/Views/BroadcastM/tableMaster.cshtml");
        }

        [HttpPost]
        public JsonResult StoreFileClientAsync()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            List<Nodo.Models.Configuration> Lconfig = (List<Nodo.Models.Configuration>)Session["config"];
            string URLWORK = Session["urlHttp"].ToString();
            string UrlApi = "sendFile";
            dynamic rowType = data.getTypeFiles().Rows;
            string UrlPath = "/Content/messages/others/";
            string vfilename = "";
            string vbody = "";
            List<Models.TypeFiles> LtypeF = new List<Models.TypeFiles>();
            foreach (dynamic row in rowType)
            {
                LtypeF.Add(new Models.TypeFiles { idTypeFile = row["idTypeFile"], nameType = row["nameType"], idGroupFile = row["idGroupFile"], nameGroup = row["nameGroup"], srcFile = row["srcFile"] });
            }
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
    }
}
