using System;
using ITMSales.Shared.Responses;

namespace ITMSales.API.Helpers
{
	public interface IMailHelper
	{
        Response SendMail(string toName, string toEmail, string subject, string body);
    }
}

