using ITMSales.API.Services;
using ITMSales.Shared.Entities;
using ITMSales.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace ITMSales.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IApiService _apiService;

        public SeedDb(DataContext context, IApiService apiService)
        {
            _context = context;
            _apiService = apiService;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                Response responseCountries = await _apiService.GetListAsync<CountryResponse>("/v1", "/countries");

                if (responseCountries.IsSuccess)
                {
                    List<CountryResponse> countries = (List<CountryResponse>)responseCountries.Result!;

                    foreach (CountryResponse countryResponse in countries)
                    {
                        Country country = await _context.Countries!.FirstOrDefaultAsync(c => c.Name == countryResponse.Name!)!;

                        if (country == null)
                        {
                            country = new() { Name = countryResponse.Name!, States = new List<State>() };

                            Response responseStates = await _apiService.GetListAsync<StateResponse>("/v1", $"/countries/{countryResponse.Iso2}/states");

                            if (responseStates.IsSuccess)
                            {
                                List<StateResponse> states = (List<StateResponse>)responseStates.Result!;

                                foreach (StateResponse stateResponse in states!)
                                {
                                    State state = country.States!.FirstOrDefault(s => s.Name == stateResponse.Name!)!;

                                    if (state == null)
                                    {
                                        state = new() { Name = stateResponse.Name!, Cities = new List<City>() };

                                        Response responseCities = await _apiService.GetListAsync<CityResponse>("/v1", $"/countries/{countryResponse.Iso2}/states/{stateResponse.Iso2}/cities");

                                        if (responseCities.IsSuccess)
                                        {
                                            List<CityResponse> cities = (List<CityResponse>)responseCities.Result!;

                                            foreach (CityResponse cityResponse in cities)
                                            {
                                                if (cityResponse.Name == "Mosfellsbær" || cityResponse.Name == "Șăulița")
                                                {
                                                    continue;
                                                }

                                                City city = state.Cities!.FirstOrDefault(c => c.Name == cityResponse.Name!)!;

                                                if (city == null)
                                                {
                                                    state.Cities.Add(new City() { Name = cityResponse.Name! });
                                                }
                                            }
                                        }

                                        if (state.CitiesNumber > 0)
                                        {
                                            country.States.Add(state);
                                        }
                                    }
                                }
                            }

                            if (country.StatesNumber > 0)
                            {
                                _context.Countries.Add(country);
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Calzado" });
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Accesorios" });
                _context.Categories.Add(new Category { Name = "Electrodomesticos" });
                _context.Categories.Add(new Category { Name = "Celulares" });
                _context.Categories.Add(new Category { Name = "Computadores" });
                _context.Categories.Add(new Category { Name = "Cocina" });
                _context.Categories.Add(new Category { Name = "Jugueteria" });
                _context.Categories.Add(new Category { Name = "Colchones" });
                _context.Categories.Add(new Category { Name = "Televisores" });

                _context.Categories.Add(new Category { Name = "Calzado2" });
                _context.Categories.Add(new Category { Name = "Ropa2" });
                _context.Categories.Add(new Category { Name = "Accesorios2" });
                _context.Categories.Add(new Category { Name = "Electrodomesticos2" });
                _context.Categories.Add(new Category { Name = "Celulares2" });
                _context.Categories.Add(new Category { Name = "Computadores2" });
                _context.Categories.Add(new Category { Name = "Cocina2" });
                _context.Categories.Add(new Category { Name = "Jugueteria2" });
                _context.Categories.Add(new Category { Name = "Colchones2" });
                _context.Categories.Add(new Category { Name = "Televisores2" });

                _context.Categories.Add(new Category { Name = "Calzado3" });
                _context.Categories.Add(new Category { Name = "Ropa3" });
                _context.Categories.Add(new Category { Name = "Accesorios3" });
                _context.Categories.Add(new Category { Name = "Electrodomesticos3" });
                _context.Categories.Add(new Category { Name = "Celulares3" });
                _context.Categories.Add(new Category { Name = "Computadores3" });
                _context.Categories.Add(new Category { Name = "Cocina3" });
                _context.Categories.Add(new Category { Name = "Jugueteria3" });
                _context.Categories.Add(new Category { Name = "Colchones3" });
                _context.Categories.Add(new Category { Name = "Televisores3" });

                _context.Categories.Add(new Category { Name = "Calzado4" });
                _context.Categories.Add(new Category { Name = "Ropa4" });
                _context.Categories.Add(new Category { Name = "Accesorios4" });
                _context.Categories.Add(new Category { Name = "Electrodomesticos4" });
                _context.Categories.Add(new Category { Name = "Celulares4" });
                _context.Categories.Add(new Category { Name = "Computadores4" });
                _context.Categories.Add(new Category { Name = "Cocina4" });
                _context.Categories.Add(new Category { Name = "Jugueteria4" });
                _context.Categories.Add(new Category { Name = "Colchones4" });
                _context.Categories.Add(new Category { Name = "Televisores4" });

                _context.Categories.Add(new Category { Name = "Calzado5" });
                _context.Categories.Add(new Category { Name = "Ropa5" });
                _context.Categories.Add(new Category { Name = "Accesorios5" });
                _context.Categories.Add(new Category { Name = "Electrodomesticos5" });
                _context.Categories.Add(new Category { Name = "Celulares5" });
                _context.Categories.Add(new Category { Name = "Computadores5" });
                _context.Categories.Add(new Category { Name = "Cocina5" });
                _context.Categories.Add(new Category { Name = "Jugueteria5" });
                _context.Categories.Add(new Category { Name = "Colchones5" });
                _context.Categories.Add(new Category { Name = "Televisores5" });

                await _context.SaveChangesAsync();
            }
        }
    }
}
