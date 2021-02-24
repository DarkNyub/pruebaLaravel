using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.OleDb;
using ClosedXML.Excel;
using System.IO;
using System.IO.Compression;

namespace Nodo.Controllers
{
    public class CampaignsController : Controller
    {
        // GET: Campaigns
        public ActionResult Index()
        {
            Session["url"] = "campa";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            if(Convert.ToInt32(Session["idProfile"]) == 1)
                ViewBag.row = data.ObtenerData("SP_getCampaigns").Rows;
            else
                ViewBag.row = data.getAllDataCampaingByUSer(Convert.ToInt32(Session["idClient"])).Rows;

            ViewBag.rowActivityCampa = data.ObtenerData("SP_getActivityCampaign").Rows;

            return View("/Views/Campaigns/table.cshtml");
        }
        public ActionResult Backup(int id)
        {
            Session["url"] = "campa";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            ViewBag.rowCampaign = data.getCampaigns(id).Rows[0];
            ViewBag.rowClients = data.ObtenerData("SP_getClients").Rows;
            return View("/Views/Campaigns/Report/Backup.cshtml");
        }
        public ActionResult ShowConversation(int id)//id de la campaña
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            Session["url"] = "campa";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }
            int pidClient = 0;
            if (Request["idClient"] != null)
                pidClient = Convert.ToInt32(Request["idClient"]);
            ViewBag.rowCampaign = data.getCampaigns(id).Rows[0];
            ViewBag.rowChatList = data.getChatListByCampaingByClient(id, pidClient).Rows;
            //ViewBag.rowClientMessage = data.getAllMessageChatsByClientByCampagin(id, pidClient).Rows;

            return View("/Views/Campaigns/Report/ShowConversation.cshtml");
        }    
        public JsonResult getMessagesByCampaingByClient(int id)//id de la campaña
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidClient = Convert.ToInt32(Request["idClient"]);
            DataTable dd = data.getAllMessageChatsByClientByCampagin(id, vidClient);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }

        // GET: Campaigns/Create
        public ActionResult Create()
        {
            Session["url"] = "campa";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            ViewBag.rowCompany = data.ObtenerData("SP_getCompany").Rows;
            return View("/Views/Campaigns/create.cshtml");
        }

        // GET: Campaigns/Details/5
        public ActionResult StoreCampaign(Models.campaigns model)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            model.idCompany = Request["idCompany"];
            int prue = 0;
            if(int.TryParse(model.idCompany.ToString(), out prue)){}
            else
                model.idCompany = data.storeCompany(model.idCompany).Rows[0]["idCompany"];
            if (model.dateEnd != null)
                model.dateEnd = DateTime.Parse(model.dateEnd.Split('-')[2]+'-'+ model.dateEnd.Split('-')[1]+'-'+ model.dateEnd.Split('-')[0]).ToString("yyyy-MM-dd 00:00:00");
            dynamic vrowCampaig = data.storeCampaign(model).Rows[0];
            int vidcatalog = 0;
            if (Request["catalogtypedata[]"] != null)
            {//esto es para datos del cliente
                string[] rowCatalogs = Request["catalogtypedata[]"].ToString().Split('#');
                for (var i = 1; i < rowCatalogs.Length; i++)
                {
                    vidcatalog = 0;
                    string[] rowCata = rowCatalogs[i].ToString().Trim(',').Split(',');
                    int vCatalogAlone = 1;
                    if (rowCata.Length > 1)
                        vCatalogAlone = 0;
                    for (var j = 0; j < rowCata.Length; j++)
                    {
                        dynamic vtalog = data.storeCatalog(rowCata[j].ToString(), vidcatalog).Rows[0];
                        if (j == 0)
                            vidcatalog = Convert.ToInt32(vtalog["idCatalog"]);
                    }
                    data.storeCatalogCampaign(vidcatalog, vrowCampaig["idCampaigns"], 1, vCatalogAlone);
                }
            }
            //para los datos del cliente por archivos
            var nomaas = Request.Files;
            var snd = nomaas.GetMultiple("catalogExceltypedata[]");
            for (var i = 0; i < snd.Count; i++)
            {
                if (snd[i].ContentLength > 0)
                {
                    string vurl = "/Content/options/files/";
                    string vfileName = Path.GetFileNameWithoutExtension(snd[i].FileName);
                    string vfileExtension = Path.GetExtension(snd[i].FileName);
                    vfileName = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + vfileName.Trim() + vfileExtension;
                    snd[i].SaveAs(System.Web.HttpContext.Current.Server.MapPath(vurl) + vfileName);
                    string Import_FileName = Server.MapPath(vurl) + vfileName;
                    using (XLWorkbook workBook = new XLWorkbook(Import_FileName))
                    {
                        //Read the first Sheet from Excel file.
                        IXLWorksheet workSheet = workBook.Worksheet(1);
                        //Create a new DataTable.
                        DataTable dt = new DataTable();
                        //Loop through the Worksheet rows.
                        bool firstRow = true;
                        int totalRows = workSheet.LastRowUsed().RowNumber();
                        int countRows = 0;
                        vidcatalog = 0;
                        int vCatalogAlone = 1;
                        foreach (IXLRow row in workSheet.Rows())
                        {
                            countRows++;
                            //Use the first row to add columns to DataTable.
                            if (firstRow)
                            {
                                foreach (IXLCell cell in row.Cells())
                                {
                                    dynamic catalogFat = data.storeCatalog(cell.Value.ToString(), vidcatalog).Rows[0];
                                    vidcatalog = Convert.ToInt32(catalogFat["idCatalog"]);
                                }
                                firstRow = false;
                            }
                            else
                            {
                                vCatalogAlone = 0;
                                //Add rows to DataTable.
                                dt.Rows.Add();
                                int j = 0;
                                foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                                {
                                    dynamic catalogFat = data.storeCatalog(cell.Value.ToString(), vidcatalog).Rows[0];
                                    j++;
                                }
                            }
                            if (countRows == totalRows)
                                break;
                        }
                        data.storeCatalogCampaign(vidcatalog, vrowCampaig["idCampaigns"], 1, vCatalogAlone);
                    }
                }
            }
            vidcatalog = 0;
            if (Request["catalogtypedata2[]"] != null)
            {//esto es para los catalogos
                string[] rowCatalogs = Request["catalogtypedata2[]"].ToString().Split('#');
                for (var i = 1; i < rowCatalogs.Length; i++)
                {
                    vidcatalog = 0;
                    string[] rowCata = rowCatalogs[i].ToString().Trim(',').Split(',');
                    int vCatalogAlone = 1;
                    if (rowCata.Length > 1)
                        vCatalogAlone = 0;
                    for (var j = 0; j < rowCata.Length; j++)
                    {
                        dynamic vtalog = data.storeCatalog(rowCata[j].ToString(), vidcatalog).Rows[0];
                        if (j == 0)
                            vidcatalog = Convert.ToInt32(vtalog["idCatalog"]);
                    }
                    data.storeCatalogCampaign(vidcatalog, vrowCampaig["idCampaigns"], 2, vCatalogAlone);
                }
            }
            snd = nomaas.GetMultiple("catalogExceltypedata2[]");
            for (var i = 0; i < snd.Count; i++)
            {
                if (snd[i].ContentLength > 0)
                {
                    string vurl = "/Content/options/files/";
                    string vfileName = Path.GetFileNameWithoutExtension(snd[i].FileName);
                    string vfileExtension = Path.GetExtension(snd[i].FileName);
                    vfileName = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + vfileName.Trim() + vfileExtension;
                    snd[i].SaveAs(System.Web.HttpContext.Current.Server.MapPath(vurl) + vfileName);
                    string Import_FileName = Server.MapPath(vurl) + vfileName;
                    using (XLWorkbook workBook = new XLWorkbook(Import_FileName))
                    {
                        //Read the first Sheet from Excel file.
                        IXLWorksheet workSheet = workBook.Worksheet(1);
                        //Create a new DataTable.
                        DataTable dt = new DataTable();
                        //Loop through the Worksheet rows.
                        bool firstRow = true;
                        int totalRows = workSheet.LastRowUsed().RowNumber();
                        int countRows = 0;
                        vidcatalog = 0;
                        int vCatalogAlone = 1;
                        foreach (IXLRow row in workSheet.Rows())
                        {
                            countRows++;
                            //Use the first row to add columns to DataTable.
                            if (firstRow)
                            {
                                foreach (IXLCell cell in row.Cells())
                                {
                                    dynamic catalogFat = data.storeCatalog(cell.Value.ToString(), vidcatalog).Rows[0];
                                    vidcatalog = Convert.ToInt32(catalogFat["idCatalog"]);
                                }
                                firstRow = false;
                            }
                            else
                            {
                                vCatalogAlone = 0;
                                //Add rows to DataTable.
                                dt.Rows.Add();
                                int j = 0;
                                foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                                {
                                    dynamic catalogFat = data.storeCatalog(cell.Value.ToString(), vidcatalog).Rows[0];
                                    j++;
                                }
                            }
                            if (countRows == totalRows)
                                break;
                        }
                        data.storeCatalogCampaign(vidcatalog, vrowCampaig["idCampaigns"],2, vCatalogAlone);
                    }
                }
            }


            Session["message"] = "Campaña creada correctamente";
            Session["type"] = "success";
            Session["title"] = "Muy bien";
            return RedirectToAction("Index");
        }

        public JsonResult getEmployeeCampaign(int id)//id de la campaña
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable dd = data.getEmployeeCampaign(id);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getClientsCampaign(int id)//id de la campaña
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable dd = data.getClientsCampaign(id);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult storeCampaignEmployee()//id de la campaña
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidCampaign = Convert.ToInt32(Request["idcampaign"]);
            int vidEmployee = Convert.ToInt32(Request["idrow"]);
            DataTable dd = data.storeCampaignEmployee(vidCampaign, vidEmployee);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult storeCampaignClient()//id de la campaña
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidCampaign = Convert.ToInt32(Request["idcampaign"]);
            int vidClient = Convert.ToInt32(Request["idrow"]);
            DataTable dd = data.storeCampaignClient(vidCampaign, vidClient);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ChangeActivityCampaign(int id)//id de la campaña
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            int vidActivity = Convert.ToInt32(Request["value"]);
            DataTable dd = data.ChangeActivityCampaign(id, vidActivity);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReportCampaign(int id) //id de la campaña
        {
            Session["url"] = "campa";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            ViewBag.rowCampaign = data.getCampaigns(id).Rows[0];

            ViewBag.clientsCampaign = data.getClientsCampaign(id).Rows; //data.getAllDataClientsCampaign(id).Rows;
            ViewBag.rowClientCatalog = data.getClientsCatalogByCampaign(id).Rows;
            ViewBag.rowCatalog = data.getCatalogsByCampaign(id).Rows;
            ViewBag.rowCommentsByCampaign = data.getCommentsByCampaign(id).Rows;
            ViewBag.idCampaign = id;
            return View("/Views/Campaigns/Report/Clients.cshtml");
        }

        // GET: Campaigns/Edit/5
        public ActionResult Edit(int id)
        {
            Session["url"] = "campa";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            ViewBag.rowCompany = data.ObtenerData("SP_getCompany").Rows;
            ViewBag.rowCampaign = data.getCampaigns(id).Rows[0];
            ViewBag.rowcatalogs = data.getAllCatalogByCampaign(id).Rows;
            return View("/Views/Campaigns/edit.cshtml");
        }
        public ActionResult UpdateCampaign(Models.campaigns model)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            model.idCompany = Request["idCompany"];
            //string[] rowCatalogs = Request["catalog[]"].ToString().Split('#');
            int prue = 0;
            if (int.TryParse(model.idCompany.ToString(), out prue)) { }
            else
                model.idCompany = data.storeCompany(model.idCompany).Rows[0]["idCompany"];
            if(model.dateEnd != null)
                model.dateEnd = DateTime.Parse(model.dateEnd.Split('-')[2] + '-' + model.dateEnd.Split('-')[1] + '-' + model.dateEnd.Split('-')[0]).ToString("yyyy-MM-dd 00:00:00");

            dynamic vrowCampaig = data.UpdateCampaign(model).Rows[0];
            int vidcatalog = 0;
            if (Request["catalogtypedata[]"] != null)
            {//esto es para datos del cliente
                string[] rowCatalogs = Request["catalogtypedata[]"].ToString().Split('#');
                for (var i = 1; i < rowCatalogs.Length; i++)
                {
                    vidcatalog = 0;
                    string[] rowCata = rowCatalogs[i].ToString().Trim(',').Split(',');
                    int vCatalogAlone = 1;
                    if (rowCata.Length > 1)
                        vCatalogAlone = 0;
                    for (var j = 0; j < rowCata.Length; j++)
                    {
                        dynamic vnamcata = rowCata[j].ToString();
                        dynamic vtalog = "";
                        if (int.TryParse(rowCata[j].ToString(), out prue))
                        {
                            if (j == 0)
                                vidcatalog = Convert.ToInt32(rowCata[j]);
                        }
                        else
                        {
                            vtalog = data.storeCatalog(vnamcata, vidcatalog, vrowCampaig["idCampaigns"]).Rows[0];
                            if (j == 0)
                                vidcatalog = Convert.ToInt32(vtalog["idCatalog"]);
                        }
                    }
                    data.storeCatalogCampaign(vidcatalog, vrowCampaig["idCampaigns"], 1, vCatalogAlone);
                }
            }

            //para los datos del cliente por archivos
            var nomaas = Request.Files;
            var snd = nomaas.GetMultiple("catalogExceltypedata[]");
            for (var i = 0; i < snd.Count; i++)
            {
                if (snd[i].ContentLength > 0)
                {
                    string vurl = "/Content/options/files/";
                    string vfileName = Path.GetFileNameWithoutExtension(snd[i].FileName);
                    string vfileExtension = Path.GetExtension(snd[i].FileName);
                    vfileName = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + vfileName.Trim() + vfileExtension;
                    snd[i].SaveAs(System.Web.HttpContext.Current.Server.MapPath(vurl) + vfileName);
                    string Import_FileName = Server.MapPath(vurl) + vfileName;
                    using (XLWorkbook workBook = new XLWorkbook(Import_FileName))
                    {
                        //Read the first Sheet from Excel file.
                        IXLWorksheet workSheet = workBook.Worksheet(1);
                        //Create a new DataTable.
                        DataTable dt = new DataTable();
                        //Loop through the Worksheet rows.
                        bool firstRow = true;
                        vidcatalog = 0;
                        int vCatalogAlone = 1;
                        foreach (IXLRow row in workSheet.Rows())
                        {
                            //Use the first row to add columns to DataTable.
                            if (firstRow)
                            {
                                foreach (IXLCell cell in row.Cells())
                                {
                                    dynamic catalogFat = data.storeCatalog(cell.Value.ToString(), vidcatalog).Rows[0];
                                    vidcatalog = Convert.ToInt32(catalogFat["idCatalog"]);
                                }
                                firstRow = false;
                            }
                            else
                            {
                                vCatalogAlone = 0;
                                //Add rows to DataTable.
                                dt.Rows.Add();
                                int j = 0;
                                foreach (IXLCell cell in row.Cells())
                                {
                                    dynamic catalogFat = data.storeCatalog(cell.Value.ToString(), vidcatalog).Rows[0];
                                    j++;
                                }
                            }
                        }
                        data.storeCatalogCampaign(vidcatalog, vrowCampaig["idCampaigns"], 1, vCatalogAlone);
                    }
                }
            }
            vidcatalog = 0;
            if (Request["catalogtypedata2[]"] != null)
            {//esto es para los catalogos
                string[] rowCatalogs = Request["catalogtypedata2[]"].ToString().Split('#');
                for (var i = 1; i < rowCatalogs.Length; i++)
                {
                    vidcatalog = 0;
                    string[] rowCata = rowCatalogs[i].ToString().Trim(',').Split(',');
                    int vCatalogAlone = 1;
                    if (rowCata.Length > 1)
                        vCatalogAlone = 0;
                    for (var j = 0; j < rowCata.Length; j++)
                    {
                        dynamic vnamcata = rowCata[j].ToString();
                        dynamic vtalog = "";
                        if (int.TryParse(rowCata[j].ToString(), out prue))
                        {
                            if (j == 0)
                                vidcatalog = Convert.ToInt32(rowCata[j]);
                        }
                        else
                        {
                            vtalog = data.storeCatalog(vnamcata, vidcatalog, vrowCampaig["idCampaigns"]).Rows[0];
                            if (j == 0)
                                vidcatalog = Convert.ToInt32(vtalog["idCatalog"]);
                        }
                    }
                    data.storeCatalogCampaign(vidcatalog, vrowCampaig["idCampaigns"], 2, vCatalogAlone);
                }
            }
            snd = nomaas.GetMultiple("catalogExceltypedata2[]");
            for (var i = 0; i < snd.Count; i++)
            {
                if (snd[i].ContentLength > 0)
                {
                    string vurl = "/Content/options/files/";
                    string vfileName = Path.GetFileNameWithoutExtension(snd[i].FileName);
                    string vfileExtension = Path.GetExtension(snd[i].FileName);
                    vfileName = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + vfileName.Trim() + vfileExtension;
                    snd[i].SaveAs(System.Web.HttpContext.Current.Server.MapPath(vurl) + vfileName);
                    string Import_FileName = Server.MapPath(vurl) + vfileName;
                    using (XLWorkbook workBook = new XLWorkbook(Import_FileName))
                    {
                        //Read the first Sheet from Excel file.
                        IXLWorksheet workSheet = workBook.Worksheet(1);
                        //Create a new DataTable.
                        DataTable dt = new DataTable();
                        //Loop through the Worksheet rows.
                        bool firstRow = true;
                        vidcatalog = 0;
                        int vCatalogAlone = 1;
                        foreach (IXLRow row in workSheet.Rows())
                        {
                            //Use the first row to add columns to DataTable.
                            if (firstRow)
                            {
                                foreach (IXLCell cell in row.Cells())
                                {
                                    dynamic catalogFat = data.storeCatalog(cell.Value.ToString(), vidcatalog).Rows[0];
                                    vidcatalog = Convert.ToInt32(catalogFat["idCatalog"]);
                                }
                                firstRow = false;
                            }
                            else
                            {
                                vCatalogAlone = 0;
                                //Add rows to DataTable.
                                dt.Rows.Add();
                                int j = 0;
                                foreach (IXLCell cell in row.Cells())
                                {
                                    dynamic catalogFat = data.storeCatalog(cell.Value.ToString(), vidcatalog).Rows[0];
                                    j++;
                                }
                            }
                        }
                        data.storeCatalogCampaign(vidcatalog, vrowCampaig["idCampaigns"], 2, vCatalogAlone);
                    }
                }
            }

            Session["message"] = "Campaña actualizada correctamente";
            Session["type"] = "success";
            Session["title"] = "Muy bien";
            return RedirectToAction("Index");
        }

        public ActionResult CreateEditConfigByCampaign(int id)//este es el identificador de la campaña
        {
            Session["url"] = "campa";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable row = data.getConfigurationByCampaign(id);
            List<Models.Configuration> rowConfiguration = new List<Models.Configuration>();
            if (row.Rows.Count > 0)
            {
                rowConfiguration = row.AsEnumerable().Select(m => new Models.Configuration()
                {
                    idCampaign = m.Field<int>("idCampaign"),
                    idConfig = m.Field<int>("idConfig"),
                    value = m.Field<string>("value"),
                    name = m.Field<string>("name")
                }).ToList();
            }
            else
            {
                rowConfiguration.Add(new Models.Configuration { idCampaign = id, idConfig = -1, name = "instance", value = "" });
                rowConfiguration.Add(new Models.Configuration { idCampaign = id, idConfig = -1, name = "token", value = "" });
                rowConfiguration.Add(new Models.Configuration { idCampaign = id, idConfig = -1, name = "urlApi", value = "" });
            }
            ViewBag.idCampaign = id;
            ViewBag.rowConfiguration = rowConfiguration;

            return View("/Views/Campaigns/CreateEditConfigByCampaign.cshtml");

        }
        public ActionResult storeOrUpdateConfigByCampaign(int id)
        {
            Session["url"] = "campa";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }
            try
            {
                ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
                //DataTable dt = null;
                string vValue;


                //dinamicas
                vValue = Request["instance"].ToString();
                data.storeOrUpdateConfigByCampaign(id, "instance", vValue);
                vValue = Request["token"].ToString();
                data.storeOrUpdateConfigByCampaign(id, "token", vValue);
                vValue = Request["urlApi"].ToString();
                data.storeOrUpdateConfigByCampaign(id, "urlApi", vValue);
                //fin dinamicos
                //estaticas
                vValue = Request["urlWork"].ToString();
                data.storeOrUpdateConfigByCampaign(id, "urlWork", vValue);
                vValue = Request["urlSendMessage"].ToString();
                data.storeOrUpdateConfigByCampaign(id, "urlSendMessage", vValue);
                vValue = Request["urlSendFile"].ToString();
                data.storeOrUpdateConfigByCampaign(id, "urlSendFile", vValue);
                vValue = Request["urlSendPTT"].ToString();
                data.storeOrUpdateConfigByCampaign(id, "urlSendPTT", vValue);
                vValue = Request["urlMessages"].ToString();
                data.storeOrUpdateConfigByCampaign(id, "urlMessages", vValue);
                //fin estaticos


                Session["message"] = "Todos los valores se han ingresado correctamente.";
                Session["type"] = "success";
                Session["title"] = "Muy Bien";
            }
            catch (Exception ex)
            {
                Session["message"] = "Hubo un problema, contacte al administrador.";
                Session["type"] = "error";
                Session["title"] = "Error";
            }

            return RedirectToAction("Index");
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
