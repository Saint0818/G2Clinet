using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class MailMgr : KnightSingleton<MailMgr>
{
	public void BugReport(string message)
	{
		MailMessage mail = new MailMessage();
		
		mail.From = new MailAddress("knight49.bugreport@gmail.com");
		mail.To.Add("knight49.bugreport@gmail.com");
		mail.Subject = "BugReport";
		mail.Body = message;
		
		SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
		smtpServer.Port = 587;
		smtpServer.Credentials = new System.Net.NetworkCredential("knight49.bugreport@gmail.com", "csharp2014") as ICredentialsByHost;
		smtpServer.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback = 
			delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
		{ return true; };
		smtpServer.Send(mail);
	}
}

