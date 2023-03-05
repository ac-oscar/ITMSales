using ITMSales.Shared.Responses;

namespace ITMSales.API.Services
{
	public interface IApiService
	{
        Task<Response> GetListAsync<T>(string servicePrefix, string controller);
    }
}

