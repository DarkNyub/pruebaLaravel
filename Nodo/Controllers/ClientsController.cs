using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ExcelDataReader;
using System.Data;
using Newtonsoft.Json;
using System.Data.OleDb;
using ClosedXML.Excel;

namespace Nodo.Controllers
{
    public class ClientsController : Controller
    {
        // GET: Clients
        public ActionResult Index()
        {
            Session["url"] = "cli";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            ViewBag.row = data.ObtenerData("SP_getClientsCampaignAll").Rows;
            return View("/Views/Clients/table.cshtml");
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            Session["url"] = "cli";
            if (Session["idClient"] == null)
            {
                Session["message"] = "Tiene que ingresar a la aplicación primero.";
                Session["type"] = "error";
                Session["title"] = "Error";
                return RedirectToAction("Index", "Home");

            }

            return View("/Views/Clients/create.cshtml");
        }
        public JsonResult getCampaign()
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable dd = data.ObtenerData("SP_getCampaigns");
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCatalogbyCampaign(int id)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            DataTable dd = data.getCatalogsByCampaign(id, 0);
            ViewBag.result = datatabletojson(dd);
            return Json(ViewBag.result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ImportClientExcel(HttpPostedFileBase importFile)
        {
            ConnectionDataBase.StoreProcediur data = new ConnectionDataBase.StoreProcediur();
            string ClientNotInserted = "";
            int entre = 1;
            string vurl = "/Content/options/files/";
            int vidCampaign = Convert.ToInt32(Request["idCampaign"]);
            string vNameOptional = "";
            if (Request["nameOptionalFile"].ToString() != null)
                vNameOptional = Request["nameOptionalFile"].ToString();

            DataTable st = data.getCatalogsByCampaign(vidCampaign, 0);
            int vcontCells = 4 + st.AsEnumerable().Where(m => m.Field<int>("type") == 1).Count();
            int ahoraEntre = 0;
            if (importFile == null) return Json(new { Status = 0, Message = "No File Selected" });
            try
            {
                var nomaas = Request.Files;
                string vfileName = Path.GetFileNameWithoutExtension(nomaas[0].FileName);
                string vfileExtension = Path.GetExtension(nomaas[0].FileName);
                vfileName = DateTime.Now.ToString("yyyyMMddHHmm") + "_" + vfileName.Trim() + vfileExtension;
                nomaas[0].SaveAs(System.Web.HttpContext.Current.Server.MapPath(vurl) + vfileName);

                string Import_FileName = Server.MapPath(vurl) + vfileName;
                using (XLWorkbook workBook = new XLWorkbook(Import_FileName))
                {
                    //Read the first Sheet from Excel file.
                    IXLWorksheet workSheet = workBook.Worksheet(1);
                    //Create a new DataTable.
                    DataTable dt = new DataTable();
                    //Loop through the Worksheet rows.
                    int totalRows = workSheet.LastRowUsed().RowNumber();
                    int countRows = 0;
                    bool firstRow = true;
                    DataTable dtfinal = new DataTable();
                    dtfinal.Columns.Add(new DataColumn("DOCUMENTO", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("NOMBRE", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("APELLIDO", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("CELULAR", typeof(string)));
                    dtfinal.Columns.Add(new DataColumn("TEXTO", typeof(string)));
                    foreach (IXLRow row in workSheet.Rows())
                    {
                        countRows++;
                        int i = 0;
                        //Use the first row to add columns to DataTable.
                        if (firstRow)
                        {
                            foreach (IXLCell cell in row.Cells())
                            {
                                if (i < 4)
                                    dt.Columns.Add(cell.Value.ToString().ToUpper());
                                if (i >= 4)
                                {
                                    dt.Columns.Add(cell.Value.ToString().ToUpper());
                                    dt.Columns.Add(cell.Value.ToString() + "_1".ToUpper());
                                }
                                i++;
                            }
                            firstRow = false;
                        }
                        else
                        {
                            //Add rows to DataTable.
                            dtfinal.Rows.Add();
                            dt.Rows.Add();
                            i = 0;
                            string holas = "";
                            foreach (IXLCell cell in row.Cells(row.FirstCellUsed().Address.ColumnNumber, row.LastCellUsed().Address.ColumnNumber))
                            {
                                if (i < 4)
                                    dtfinal.Rows[dtfinal.Rows.Count - 1][i] = cell.Value.ToString();
                                dt.Rows[dt.Rows.Count - 1][i] = cell.Value.ToString();
                                if (i >= 4)
                                {

                                    holas += dt.Columns[i].ToString() + '|';
                                    i++;
                                    holas += cell.Value.ToString() + '|';
                                }
                                i++;
                            }
                            holas = holas.Trim('|');
                            dtfinal.Rows[dtfinal.Rows.Count - 1][4] = holas;
                            dynamic asd = dt.Rows[dt.Rows.Count - 1];
                            dynamic dd = "";
                            //int vidLocale = data.storeLocaleClient(asd["localidad"].ToString().ToUpper(), asd["codigo"].ToString()).Rows[0]["idLocation"];
                            //int vidGender = data.storeGenderClient(asd["genero"].ToString()).Rows[0]["idGender"];
                            if (asd["DOCUMENTO"].ToString() != "" && asd["NOMBRE"].ToString() != "" && asd["APELLIDO"].ToString() != "" && asd["CELULAR"].ToString() != "")
                            {
                                /*dd = data.storeClientExcel(asd["NOMBRE"].ToString().ToUpper()
                                                    , asd["APELLIDO"].ToString().ToUpper()
                                                    , asd["DOCUMENTO"].ToString()
                                                    , asd["CELULAR"].ToString()
                                                    , vidCampaign).Rows[0];
                                for(int kk = 4; kk < asd.ItemArray.Length; kk++)
                                {
                                    string vCatalogFather = dt.Columns[kk].ToString();
                                    string vCatalogSon = asd.ItemArray[kk].ToString();
                                    data.storeClientCatalogExcel(Convert.ToInt32(dd["idClient"]), vidCampaign, vCatalogFather, vCatalogSon);
                                }*/
                            }
                            else
                            {
                                entre = 0;
                                ClientNotInserted += "Documento: "+ asd["DOCUMENTO"].ToString() + ", Nombre: " + asd["NOMBRE"].ToString() + ", Apellido: " + asd["APELLIDO"].ToString() + ", Celular: " + asd["CELULAR"].ToString() + "<hr>";
                            }
                        }
                        if(countRows == totalRows)
                            break;
                    }
                    if (ahoraEntre == 0)
                    {
                        DataTable ff = data.StoreAllRowClientCatalog(vidCampaign, dtfinal, vNameOptional);
                        if (ff.Rows.Count > 0)
                        {
                            DataTable dtfinal_2 = new DataTable();//[0] = idclient: 1 [1] = vtext = distribuidor|hola|mensajeria|nfasdnfl|ciudad|huila|rango|otros
                            dtfinal_2.Columns.Add(new DataColumn("idClient", typeof(string)));
                            dtfinal_2.Columns.Add(new DataColumn("fatherCatalog", typeof(string)));
                            dtfinal_2.Columns.Add(new DataColumn("SonCatalog", typeof(string)));
                            foreach (dynamic row in ff.Rows)
                            {
                                if (ff.Rows[0]["vChatText"].ToString() != "")
                                {
                                    string[] hola = row["vChatText"].ToString().Split('|');
                                    for (int i = 0; i < hola.Length; i++)
                                    {
                                        dtfinal_2.Rows.Add();
                                        dtfinal_2.Rows[dtfinal_2.Rows.Count - 1][0] = row["vidClient"];
                                        dtfinal_2.Rows[dtfinal_2.Rows.Count - 1][1] = hola[i];
                                        dtfinal_2.Rows[dtfinal_2.Rows.Count - 1][2] = hola[i + 1];
                                        i++;
                                    }
                                }
                            }//[0] idlcient =1[]catalfather = deistris[2]catalgosson = oirl emresa
                            if (dtfinal_2.Rows.Count > 0)
                            {
                                dynamic sdf = data.storeClientCatalogExcelCursor(vidCampaign, dtfinal_2);
                                //[0]idlcient =1[]catalfather = 11515 [2]catalgosson = 9846468
                                sdf = data.storeClientCatalogExcelCursor(vidCampaign, sdf, 1);
                            }
                        }
                    }
                    else
                    {
                        return Json(new { Result = 0, message = "Las columnas del archivo no concuerdan con las columnas establecidas", clients = "" }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { Result = entre, clients = ClientNotInserted }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
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
