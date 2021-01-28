using System.Net.Mail;
using System.Web.Mvc;
using System.Linq;
using System;

public class EnviarCorreos : Controller
{
    private static Random random = new Random();
    public string vusuario = "nodolabs@outlook.com";
    public string vcorreo = "nodolabs@outlook.com";
    public string vcontraseña = "12345#nodo";
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJLMNOPQRSTUVWXYZabcdefghijlmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public void EnviarCorreo(string correoAlQueEnvio, string asuntoDelCorreo, string copiaCorreoEnvio, string textoDelCorreo, string correoDesdeElQueEnvio, string usuarioCorreEnvio, string contrasenaCorreoEnvio, string archivo)
    {
        /*-------------------------MENSAJE DE CORREO----------------------*/
        System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();
        //Direccion de correo electronico a la que queremos enviar el mensaje
        mmsg.To.Add(correoAlQueEnvio);
        //Asunto
        mmsg.Subject = asuntoDelCorreo;
        mmsg.SubjectEncoding = System.Text.Encoding.UTF32;
        //Direccion de correo electronico que queremos que reciba una copia del mensaje
        if(copiaCorreoEnvio.Length > 0)
            mmsg.Bcc.Add(copiaCorreoEnvio);
        //Cuerpo del Mensaje
        mmsg.Body = textoDelCorreo;

        mmsg.BodyEncoding = System.Text.Encoding.UTF32;
        mmsg.IsBodyHtml = false;
        if (archivo.Length > 2) mmsg.Attachments.Add(new Attachment(archivo));
        //Correo electronico desde la que enviamos el mensaje
        mmsg.From = new System.Net.Mail.MailAddress(correoDesdeElQueEnvio);

        /*-------------------------CLIENTE DE CORREO----------------------*/
        System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();
        //Hay que crear las credenciales del correo emisor
        cliente.Credentials = new System.Net.NetworkCredential(usuarioCorreEnvio, contrasenaCorreoEnvio);
        //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
        cliente.Port = 587;
        cliente.EnableSsl = true;
        //cliente.Host = "smtp.office365.com";
        cliente.Host = "smtp.live.com";
        cliente.DeliveryMethod = SmtpDeliveryMethod.Network;
        //cliente.Timeout = 5000;
        //cliente.Host = "asael.colombiahosting.com.co";
        //Para Otros Correos cliente.Host = "smtp.live.com";
        mmsg.IsBodyHtml = true;
        try
        {
            cliente.Send(mmsg);
        }
        catch (System.Net.Mail.SmtpException e)
        {
            string saa = e.Message;
            throw;
        }
    }

    public string ArmarCorreoRecuperacionContrasena(string nombreUsuario, string contrasena, int genero, string celular, string urlpath)
    {
        string strBody = "<HTML>";
        strBody += "<head><style type=\"text/css\">.curpointer {cursor: pointer;}</style></head> ";
        strBody += "<Body style='font-family: Arial, icomoon, sans-serif; font-size: 12px; color: #1F1F1F'> ";
        strBody += "<p>&iexcl;Hola "+nombreUsuario.Split(' ')[0]+"!</p>";
        strBody += "<p>Si has solicitado una actualizaci&oacute;n de contrase&ntilde;a, realiza clic en el siguiente bot&oacute;n.</p>";
        strBody += "<p>Si no has realizado esta&nbsp;solicitud, ignora este email.</p>";
        strBody += "<p>Tu contrase&ntilde;a temporal es: " + contrasena + "</p>";
        strBody += "<a href='" + urlpath + "/Home/RenewPassword?id={1}'><img src=\"http://livend.azurewebsites.net/Content/options/button_restablecer-la-contrasena.png\" width= \"300\" height=\"50\" alt=\"logo\"></a> ";
        strBody += "<p>&iexcl;Gracias!</p>";
        strBody += "<p>Equipo de NODO WS.</p>";
        strBody += "<img src=\"http://livend.azurewebsites.net/Content/options/img/logo_nodo.png \" width= \"200\" height=\"50\" alt=\"logo\">";
        strBody += "<br><br>";
        strBody += "</Body>";
        strBody += "</HTML>";
        strBody = strBody.Replace("{1}", celular);
        return strBody;
    }
    public string ArmarCorreoElectronicoPrimerContacto(string name, string LastName, string usuario, string contrasena)
    {
        string strBody = "<HTML>";
        strBody += "<head><style type=\"text/css\">.curpointer {cursor: pointer;}</style></head> ";
        strBody += "<Body style='font-family: Arial, icomoon, sans-serif; font-size: 12px; color: #1F1F1F'> ";
        strBody += "<p> ¡Hola " + name  + "!</p>";
        strBody += "<p> Te damos la bienvenida a NODO WS.</p>";
        strBody += "<p> Te invitamos a ingresar a nuestro sistema con los siguientes datos:</p>";
        strBody += "<p> <b>USUARIO</b>: " + usuario+ "</p>"; ;
        strBody += "<p> <b>CONTRASEÑA</b>: " + contrasena + "</p>";
        strBody += "<p>Haz clic en el botón para ingresar</p>";
        strBody += "<a href='http://livend.azurewebsites.net/'><img src=\"http://livend.azurewebsites.net/Content/options/button_ir-al-sitio-web.png\" width= \"300\" height=\"50\" alt=\"logo\"></a> ";
        strBody += "<p>&iexcl;Gracias!</p>";
        strBody += "<p>Equipo de NODO WS.</p>";
        strBody += "<img src=\"http://livend.azurewebsites.net/Content/options/img/logo_nodo.png \" width= \"200\" height=\"50\" alt=\"logo\">";
        strBody += "<br><br>";
        strBody += "</Body>";
        strBody += "</HTML>";
        //strBody = strBody.Replace("{1}", celular);
        return strBody;
    }
    
}
