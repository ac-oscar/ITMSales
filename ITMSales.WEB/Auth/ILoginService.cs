using System;
namespace ITMSales.WEB.Auth
{
	public interface ILoginService
	{
        Task LoginAsync(string token);

        Task LogoutAsync();
    }
}

