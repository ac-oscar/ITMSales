using System;
using ITMSales.Shared.Responses;

namespace ITMSales.API.Helpers
{
	public interface IOrdersHelper
	{
        Task<Response> ProcessOrderAsync(string email, string remarks);
    }
}

