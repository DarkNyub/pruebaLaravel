using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

public class ConnectionDataBase : Controller
{
    public class StoreProcediur
    {
        public DataTable validacionContrasenaActual(string numeroCelular)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SPValidacionContrasenaAntigua", con);
                da.SelectCommand.Parameters.Add("@numeroTelefonico", SqlDbType.VarChar).Value = numeroCelular;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable actualizarContrasena(string email, byte[] contrasena, byte[] key, byte[] iv)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SPActualizarContrasena", con);
                da.SelectCommand.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                da.SelectCommand.Parameters.Add("@password", SqlDbType.VarBinary).Value = contrasena;
                da.SelectCommand.Parameters.Add("@Key", SqlDbType.VarBinary).Value = key;
                da.SelectCommand.Parameters.Add("@IV", SqlDbType.VarBinary).Value = iv;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable actualizarContrasenaConfirmacion(string numeroCelular, byte[] contrasena, byte[] key, byte[] iv, byte[] contrasenaAntigua, byte[] keyAntigua, byte[] ivAntigua)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SPActualizarContrasenaConfirmacion", con);
                da.SelectCommand.Parameters.Add("@numeroCelular", SqlDbType.VarChar).Value = numeroCelular;
                da.SelectCommand.Parameters.Add("@contrasena", SqlDbType.VarBinary).Value = contrasena;
                da.SelectCommand.Parameters.Add("@Key", SqlDbType.VarBinary).Value = key;
                da.SelectCommand.Parameters.Add("@IV", SqlDbType.VarBinary).Value = iv;
                da.SelectCommand.Parameters.Add("@contrasenaAntigua", SqlDbType.VarBinary).Value = contrasenaAntigua;
                da.SelectCommand.Parameters.Add("@KeyAntigua", SqlDbType.VarBinary).Value = keyAntigua;
                da.SelectCommand.Parameters.Add("@IVAntigua", SqlDbType.VarBinary).Value = ivAntigua;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable ValidarIngresoUsuario(string usuario, string macAddress)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SPValidacionIngresoCliente", con);
                da.SelectCommand.Parameters.Add("@Usuario", SqlDbType.VarChar).Value = usuario;
                da.SelectCommand.Parameters.Add("@macAddress", SqlDbType.VarChar).Value = macAddress;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable GetClientsCampaign(int idCampaign)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getClientsCampaignAll", con);
                if (idCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.VarChar).Value = idCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTable ObtenerData(string SP)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter(SP, con);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable saveEmployee(Nodo.Models.employee model, byte[] contrasena, Byte[] pKEY, Byte[] pIV)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeEmployee", con);
                da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = model.idEmployee;
                da.SelectCommand.Parameters.Add("@pfirstName", SqlDbType.VarChar).Value = model.firstName;
                da.SelectCommand.Parameters.Add("@plastName", SqlDbType.VarChar).Value = model.lastName;
                da.SelectCommand.Parameters.Add("@pemail", SqlDbType.VarChar).Value = model.email;
                da.SelectCommand.Parameters.Add("@pphoneMobile", SqlDbType.VarChar).Value = model.phoneMobile;
                da.SelectCommand.Parameters.Add("@pphoneLandLine", SqlDbType.VarChar).Value = model.landlinePhone;
                da.SelectCommand.Parameters.Add("@pidprofile", SqlDbType.Int).Value = model.idprofile;
                da.SelectCommand.Parameters.Add("@planguage", SqlDbType.Int).Value = model.language;
                da.SelectCommand.Parameters.Add("@pidCompany", SqlDbType.Int).Value = model.idCompany;
                da.SelectCommand.Parameters.Add("@puser", SqlDbType.VarChar).Value = model.user_cur;
                da.SelectCommand.Parameters.Add("@ppassword", SqlDbType.VarBinary).Value = contrasena;
                da.SelectCommand.Parameters.Add("@pkey", SqlDbType.VarBinary).Value = pKEY;
                da.SelectCommand.Parameters.Add("@piv", SqlDbType.VarBinary).Value = pIV;
                da.SelectCommand.Parameters.Add("@psrcImage", SqlDbType.VarChar).Value = model.srcImage;
                da.SelectCommand.Parameters.Add("@psrcImageBack", SqlDbType.VarChar).Value = model.srcImageBack;
                da.SelectCommand.Parameters.Add("@pstatuse", SqlDbType.VarChar).Value = model.statuse;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception e)
            {

                string sas = e.Message; throw;
            }
        }
        public DataTable StoreFiles(string pNameFile = "", string pSrcFile = "", string pTypeFile = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeFiles", con);
                da.SelectCommand.Parameters.Add("@pNameFile", SqlDbType.VarChar).Value = pNameFile;
                da.SelectCommand.Parameters.Add("@pSrcFile", SqlDbType.VarChar).Value = pSrcFile;
                da.SelectCommand.Parameters.Add("@pTypeFile", SqlDbType.VarChar).Value = pTypeFile;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable getTypeFiles(int pidGroupFile = 0)
        {

            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getTypeFiles", con);
                if(pidGroupFile != 0) da.SelectCommand.Parameters.Add("@pidGroupFile", SqlDbType.Int).Value = pidGroupFile;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable getCommentByClient(int pidClient = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getCommentByClient", con);
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeNewComment(int pidEmployee = 0, int pidClient = 0, string pcomment = "", int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeNewComment", con);
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                if (pcomment != "") da.SelectCommand.Parameters.Add("@pcomment", SqlDbType.VarChar).Value = pcomment;
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getCampaignCatalogByClient(int pidClient = 0, int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getCampaignCatalogByClient", con);
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeClientCatalog(int pidClient = 0, int pidCatalogFather = 0, string pidCatalogSon = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeClientCatalog", con);
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                if (pidCatalogFather != 0) da.SelectCommand.Parameters.Add("@pidCatalogFather", SqlDbType.Int).Value = pidCatalogFather;
                if (pidCatalogSon != "") da.SelectCommand.Parameters.Add("@pidCatalogSon", SqlDbType.VarChar).Value = pidCatalogSon;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public DataTable storeClientCatalogFormulario(int pidClient = 0, int pidCatalogFather = 0, string pidCatalogSon = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeClientCatalogformulario", con);
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                if (pidCatalogFather != 0) da.SelectCommand.Parameters.Add("@pidCatalogFather", SqlDbType.Int).Value = pidCatalogFather;
                if (pidCatalogSon != "") da.SelectCommand.Parameters.Add("@pidCatalogSon", SqlDbType.VarChar).Value = pidCatalogSon;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public DataTable storeClientCatalogExcel(int pidClient = 0, int pidCampaign = 0, string pidCatalogFather = "", string pidCatalogSon = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeClientCatalogExcel", con);
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                if (pidCatalogFather != "") da.SelectCommand.Parameters.Add("@pidCatalogFather", SqlDbType.VarChar).Value = pidCatalogFather;
                if (pidCatalogSon != "") da.SelectCommand.Parameters.Add("@pidCatalogSon", SqlDbType.VarChar).Value = pidCatalogSon;
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getEmployee(int pidEmployee = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getEmployee", con);
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public DataTable getDataClient(int pidClient = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getClients", con);
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeUpdateDataClient(int pidClient = 0, string pdataUpdate = "", int ptypeData = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeUpdateDataClient", con);
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                if (pdataUpdate != "") da.SelectCommand.Parameters.Add("@pdataUpdate", SqlDbType.VarChar).Value = pdataUpdate;
                da.SelectCommand.Parameters.Add("@ptypeData", SqlDbType.Int).Value = ptypeData;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        



        public DataTable storeCompany(string pnameCompany = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeCompany", con);
                if (pnameCompany != "") da.SelectCommand.Parameters.Add("@pnameCompany", SqlDbType.VarChar).Value = pnameCompany;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeCampaign(Nodo.Models.campaigns model)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeCampaign", con);
                da.SelectCommand.Parameters.Add("@pnameCampaign", SqlDbType.VarChar).Value = model.name;
                da.SelectCommand.Parameters.Add("@pidCompany", SqlDbType.Int).Value = model.idCompany;
                if(model.dateEnd != null) da.SelectCommand.Parameters.Add("@pdateEnd", SqlDbType.VarChar).Value = DateTime.Parse(model.dateEnd).ToString("yyyy-MM-dd 00:00:00");
                da.SelectCommand.Parameters.Add("@pphoneNumber", SqlDbType.VarChar).Value = model.phoneNumber;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTable UpdateCampaign(Nodo.Models.campaigns model)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_updateCampaign", con);
                da.SelectCommand.Parameters.Add("@pidCampaigns", SqlDbType.Int).Value = model.idCampaigns;
                da.SelectCommand.Parameters.Add("@pnameCampaign", SqlDbType.VarChar).Value = model.name;
                da.SelectCommand.Parameters.Add("@pidCompany", SqlDbType.Int).Value = model.idCompany;
                if (model.dateEnd != null) da.SelectCommand.Parameters.Add("@pdateEnd", SqlDbType.VarChar).Value = DateTime.Parse(model.dateEnd).ToString("yyyy-MM-dd 00:00:00");
                da.SelectCommand.Parameters.Add("@pphoneNumber", SqlDbType.VarChar).Value = model.phoneNumber;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
        }
        
        public DataTable storeCatalog(string pnameCatalog = "", int ptype = 0, int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeCatalog", con);
                da.SelectCommand.Parameters.Add("@pnameCatalog", SqlDbType.VarChar).Value = pnameCatalog;
                da.SelectCommand.Parameters.Add("@ptype", SqlDbType.Int).Value = ptype;
                if(pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeCatalogCampaign(int pidCatalog = 0, int pidCampaign = 0, int pidtype = 0, int pCatalogAlone = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeCatalogCampaign", con);
                da.SelectCommand.Parameters.Add("@pidCatalog", SqlDbType.Int).Value = pidCatalog;
                da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.Parameters.Add("@pidtype", SqlDbType.Int).Value = pidtype;
                da.SelectCommand.Parameters.Add("@pCatalogAlone", SqlDbType.Int).Value = pCatalogAlone;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getEmployeeCampaign(int pidCampaign = 0, int ptype = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getEmployeeCampaign", con);
                if(pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (ptype != 0) da.SelectCommand.Parameters.Add("@ptype", SqlDbType.Int).Value = ptype;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getClientsCampaign(int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getClientsCampaign", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeCampaignEmployee(int pidCampaign = 0, int pidEmployee = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeCampaignEmployee", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeCampaignClient(int pidCampaign = 0, int pidClient = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeCampaignClient", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeClientExcel(string pfirstName = "", string plastName = "", string pdocument = "",string pnumberPhone = "", int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeClientExcel", con);
                if (pfirstName != "") da.SelectCommand.Parameters.Add("@pfirstName", SqlDbType.VarChar).Value = pfirstName;
                if (plastName != "") da.SelectCommand.Parameters.Add("@plastName", SqlDbType.VarChar).Value = plastName;
                if (pdocument != "") da.SelectCommand.Parameters.Add("@pdocument", SqlDbType.VarChar).Value = pdocument;
                if (pnumberPhone != "") da.SelectCommand.Parameters.Add("@pnumberPhone", SqlDbType.VarChar).Value = pnumberPhone;
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeLocaleClient(string pnameLocale = "", string pcode = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeLocaleClient", con);
                if (pnameLocale != "") da.SelectCommand.Parameters.Add("@pnameLocale", SqlDbType.VarChar).Value = pnameLocale;
                if (pcode != "") da.SelectCommand.Parameters.Add("@pcode", SqlDbType.VarChar).Value = pcode;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeGenderClient(string pnameGender = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeGenderClient", con);
                if (pnameGender != "") da.SelectCommand.Parameters.Add("@pnameGender", SqlDbType.VarChar).Value = pnameGender;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable ChangeActivityCampaign(int pidCampaign = 0, int pidActivity = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_ChangeActivityCampaign", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (pidActivity != 0) da.SelectCommand.Parameters.Add("@pidActivity", SqlDbType.Int).Value = pidActivity;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable ChangeActivityUser(int pidEmployee = 0, int pidActivity = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_ChangeActivityUser", con);
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                if (pidActivity != 0) da.SelectCommand.Parameters.Add("@pidActivity", SqlDbType.Int).Value = pidActivity;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public DataTable getCatalogsByCampaign(int pidCampaign = 0, int pidFather = -1)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getCatalogsByCampaign", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (pidFather >= 0) da.SelectCommand.Parameters.Add("@pidFather", SqlDbType.Int).Value = pidFather;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getAllCatalogByCampaign(int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getAllCatalogByCampaign", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getCampaigns(int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getCampaigns", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getAllDataClientsCampaign(int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getAllDataClientsCampaign", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getClientsCatalogByCampaign(int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getClientsCatalogByCampaign", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public DataTable getDataCampaingByUSer(int pidEmployee = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getDataCampaingByUSer", con);
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getAllDataCampaingByUSer(int pidEmployee = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getAllDataCampaingByUSer", con);
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getChatListByCampaing(int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getChatListByCampaing", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getChatListByCampaingByClient(int pidCampaign = 0, int pidClient = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getChatListByCampaingByClient", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable StoreUpdateDataChatList(int pidCampaign = 0, int pidClient = 0, string pOid = "", string pimage = "",string pLastUsed = "", string pName ="", string pPhoneNumber = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_StoreUpdateDataChatList", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                if (pOid != "") da.SelectCommand.Parameters.Add("@pOid", SqlDbType.VarChar).Value = pOid;
                if (pimage !="") da.SelectCommand.Parameters.Add("@pimage", SqlDbType.VarChar).Value = pimage;
                if (pLastUsed !="") da.SelectCommand.Parameters.Add("@pLastUsed", SqlDbType.VarChar).Value = pLastUsed;
                if (pName != "") da.SelectCommand.Parameters.Add("@pName", SqlDbType.VarChar).Value = pName;
                if (pPhoneNumber != "") da.SelectCommand.Parameters.Add("@pPhoneNumber", SqlDbType.VarChar).Value = pPhoneNumber;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable StoreDataChatList(DataTable chatList, int pidCampaign = 0, string pNotPhoneNumbers = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_StoreDataChatList", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.Parameters.Add("@TablaCargar", SqlDbType.Structured).Value = chatList;
                da.SelectCommand.Parameters.Add("@pNotPhoneNumbers", SqlDbType.VarChar).Value = pNotPhoneNumbers;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable findClientByPhoneNumber(int pidCampaign = 0, string pPhoneNumber = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_findClientByPhoneNumber", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.Parameters.Add("@pPhoneNumber", SqlDbType.VarChar).Value = pPhoneNumber;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable FreeAllByAgent( int pidEmployee = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_FreeAllByAgent", con);
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable FreeClientByAgent(int pidEmployee = 0, int pidClient = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_FreeClientByAgent", con);
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable FreeClientBotByAgent(int pidEmployee = 0, int pidClient = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_FreeClientBotByAgent", con);
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        


        public DataTable StoreMessagesByClient(Nodo.Models.Message model = null, int pidCampaign = 0, string pNumberClient = "", int pidEmployee = 0  )
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_StoreMessagesByClient", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (pNumberClient != "") da.SelectCommand.Parameters.Add("@pNumberClient", SqlDbType.VarChar).Value = pNumberClient;
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                if (model.id != "") da.SelectCommand.Parameters.Add("@pid", SqlDbType.VarChar).Value = model.id;
                if (model.body != "") da.SelectCommand.Parameters.Add("@pbody", SqlDbType.VarChar).Value = model.body;
                if (model.type != "") da.SelectCommand.Parameters.Add("@ptype", SqlDbType.VarChar).Value = model.type;
                if (model.senderName != "") da.SelectCommand.Parameters.Add("@psenderName", SqlDbType.VarChar).Value = model.senderName;
                if (model.fromMe.ToString() != "") da.SelectCommand.Parameters.Add("@pfromMe", SqlDbType.VarChar).Value = model.fromMe.ToString();
                if (model.author != "") da.SelectCommand.Parameters.Add("@pauthor", SqlDbType.VarChar).Value = model.author;
                da.SelectCommand.Parameters.Add("@ptime", SqlDbType.VarChar).Value = model.time;
                if (model.chatId != "") da.SelectCommand.Parameters.Add("@pchatId", SqlDbType.VarChar).Value = model.chatId;
                if (model.messageNumber.ToString() != "") da.SelectCommand.Parameters.Add("@pmessageNumber", SqlDbType.VarChar).Value = model.messageNumber.ToString();
                if (model.self != "") da.SelectCommand.Parameters.Add("@pself", SqlDbType.VarChar).Value = model.self;
                if (model.isForwarded != "") da.SelectCommand.Parameters.Add("@pisForwarded", SqlDbType.VarChar).Value = model.isForwarded;
                if (model.quotedMsgBody != "") da.SelectCommand.Parameters.Add("@pquotedMsgBody", SqlDbType.VarChar).Value = model.quotedMsgBody;
                if (model.quotedMsgId != "") da.SelectCommand.Parameters.Add("@pquotedMsgId", SqlDbType.VarChar).Value = model.quotedMsgId;
                if (model.chatName != "") da.SelectCommand.Parameters.Add("@pchatName", SqlDbType.VarChar).Value = model.chatName;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable StoreDifusionMessagesByClient(Nodo.Models.NotifyMessage model = null, int pidCampaign = 0, int pidEmployee = 0, string pNumberClient = "", string time = "")

        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_StoreDifusionMessagesByClient", con);
                if (model.messages[0].id != "") da.SelectCommand.Parameters.Add("@pid", SqlDbType.VarChar).Value = model.messages[0].id;
                if (model.messages[0].body.ToString() != "") da.SelectCommand.Parameters.Add("@pbody", SqlDbType.VarChar).Value = model.messages[0].body.ToString();
                if (model.messages[0].type.ToString() != "") da.SelectCommand.Parameters.Add("@ptype", SqlDbType.VarChar).Value = model.messages[0].type.ToString();
                if (model.messages[0].senderName != "") da.SelectCommand.Parameters.Add("@psenderName", SqlDbType.VarChar).Value = model.messages[0].senderName;
                if (model.messages[0].fromMe.ToString() != "") da.SelectCommand.Parameters.Add("@pfromMe", SqlDbType.VarChar).Value = model.messages[0].fromMe.ToString();
                if (model.messages[0].author!= "") da.SelectCommand.Parameters.Add("@pauthor", SqlDbType.VarChar).Value = model.messages[0].author;
                if (model.messages[0].chatId.ToString() != "") da.SelectCommand.Parameters.Add("@pchatId", SqlDbType.VarChar).Value = model.messages[0].chatId.ToString();
                if (model.messages[0].messageNumber != 0) da.SelectCommand.Parameters.Add("@pmessageNumber", SqlDbType.Int).Value = model.messages[0].messageNumber;
                if (model.messages[0].self != "") da.SelectCommand.Parameters.Add("@pself", SqlDbType.VarChar).Value = model.messages[0].self;
                if (model.messages[0].isForwarded.ToString() != "") da.SelectCommand.Parameters.Add("@pisForwarded", SqlDbType.VarChar).Value = model.messages[0].isForwarded.ToString();
                if (model.messages[0].quotedMsgBody != "") da.SelectCommand.Parameters.Add("@pquotedMsgBody", SqlDbType.VarChar).Value = model.messages[0].quotedMsgBody;
                if (model.messages[0].quotedMsgId != "") da.SelectCommand.Parameters.Add("@pquotedMsgId", SqlDbType.VarChar).Value = model.messages[0].quotedMsgId;
                if (model.messages[0].chatName.ToString() != "") da.SelectCommand.Parameters.Add("@pchatName", SqlDbType.VarChar).Value = model.messages[0].chatName.ToString();
                da.SelectCommand.Parameters.Add("@ptime", SqlDbType.VarChar).Value = time;
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (pNumberClient != "") da.SelectCommand.Parameters.Add("@pNumberClient", SqlDbType.VarChar).Value = pNumberClient;
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getAllMessageChatsByClientByCampagin(int pidCampaign = 0, int pidClient = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getAllMessageChatsByClientByCampagin", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getCommentsByCampaign(int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getCommentsByCampaign", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable getDataClientByNumber(string pNumberPhone = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getDataClientByNumber", con);
                if (pNumberPhone != "") da.SelectCommand.Parameters.Add("@pNumberPhone", SqlDbType.VarChar).Value = pNumberPhone;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable StoreAllRowClientCatalog(int pidCampaign = 0, DataTable chatList = null, string pNameOptional = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_StoreAllRowClientCatalog", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.Parameters.Add("@TablaCargar", SqlDbType.Structured).Value = chatList;
                if (pNameOptional != "") da.SelectCommand.Parameters.Add("@pNameOptional", SqlDbType.VarChar).Value = pNameOptional;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeClientCatalogExcelCursor(int pidCampaign = 0, DataTable chatList = null, int pTruns = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeClientCatalogExcelCursor", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.Parameters.Add("@TablaCargar", SqlDbType.Structured).Value = chatList;
                if (pTruns != 0) da.SelectCommand.Parameters.Add("@pTruns", SqlDbType.Int).Value = pTruns;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeSmartDataClientIdToDatabase(int pidClient = 0, int pidCampaign = 0, string pSmartDataClientId = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeSmartDataClientIdToDatabase", con);
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                if (pSmartDataClientId != "") da.SelectCommand.Parameters.Add("@pSmartDataClientId", SqlDbType.VarChar).Value = pSmartDataClientId;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public DataTable getDataCampaignByInstance(string pInstanceId = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getDataCampaignByInstance", con);
                if (pInstanceId != "") da.SelectCommand.Parameters.Add("@pInstanceId", SqlDbType.VarChar).Value = pInstanceId;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable getStatusBotClient(string pNumberPhone = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getStatusBotClient", con);
                if (pNumberPhone != "") da.SelectCommand.Parameters.Add("@pNumberPhone", SqlDbType.VarChar).Value = pNumberPhone;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataTable getConfigurationByCampaign(int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_getConfigurationByCampaign", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable storeLog(string pDescription = "")
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_storeLog", con);
                if (pDescription != "") da.SelectCommand.Parameters.Add("@pDescription", SqlDbType.VarChar).Value = pDescription;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable verifyBotCampaign(int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_verifyBotCampaign", con);
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable changeClientToBot(int pidClient = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_changeClientToBot", con);
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable changeClientToAgent(int pidClient = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_changeClientToAgent", con);
                if (pidClient != 0) da.SelectCommand.Parameters.Add("@pidClient", SqlDbType.Int).Value = pidClient;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public DataTable updatePhoneAwaitAgentChatList(string pPhoneNumber = "", int pStatus = 0, int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_updatePhoneAwaitAgentChatList", con);
                if (pPhoneNumber != "") da.SelectCommand.Parameters.Add("@pPhoneNumber", SqlDbType.VarChar).Value = pPhoneNumber;
                if (pStatus != 0) da.SelectCommand.Parameters.Add("@pStatus", SqlDbType.Int).Value = pStatus;
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable StoreSelectedClientByAgent(int pidEmployee = 0 ,string pPhoneNumber = "", int pidCampaign = 0)
        {
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerDataConnectionString"].ConnectionString);
                SqlDataAdapter da = new SqlDataAdapter("SP_StoreSelectedClientByAgent", con);
                if (pidEmployee != 0) da.SelectCommand.Parameters.Add("@pidEmployee", SqlDbType.Int).Value = pidEmployee;
                if (pPhoneNumber != "") da.SelectCommand.Parameters.Add("@pPhoneNumber", SqlDbType.VarChar).Value = pPhoneNumber;
                if (pidCampaign != 0) da.SelectCommand.Parameters.Add("@pidCampaign", SqlDbType.Int).Value = pidCampaign;
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                throw;
            }
        }
        



    }
}
