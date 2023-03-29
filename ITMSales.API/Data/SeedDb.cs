using ITMSales.API.Helpers;
using ITMSales.API.Services;
using ITMSales.Shared.Entities;
using ITMSales.Shared.Enums;
using ITMSales.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace ITMSales.API.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IApiService _apiService;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext context,
            IApiService apiService,
            IUserHelper userHelper
            )
        {
            _context    = context;
            _apiService = apiService;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Oscar", "Agudelo", "magoos@yopmail.com", "3178378851", "A donde no suben los taxis", UserType.Admin);
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);

            if (user == null)
            {
                var city = await _context.Cities.FirstOrDefaultAsync(x => x.Name == "Medellín");

                if (city == null)
                {
                    city = await _context.Cities.FirstOrDefaultAsync();
                }

                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = city,
                    UserType = userType,
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                await _userHelper.ConfirmEmailAsync(user, token);
            }

            return user;
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
                _context.Categories.Add(new Category { Name = "Abrigos" });
                _context.Categories.Add(new Category { Name = "Accesorios" });
                _context.Categories.Add(new Category { Name = "Alimentos" });
                _context.Categories.Add(new Category { Name = "Anteojos" });
                _context.Categories.Add(new Category { Name = "Armas" });
                _context.Categories.Add(new Category { Name = "Articulos de Baño" });
                _context.Categories.Add(new Category { Name = "Belleza" });
                _context.Categories.Add(new Category { Name = "Bicicletas" });
                _context.Categories.Add(new Category { Name = "Bisuteria" });
                _context.Categories.Add(new Category { Name = "Bolsos" });
                _context.Categories.Add(new Category { Name = "Calzado" });
                _context.Categories.Add(new Category { Name = "Cámaras" });
                _context.Categories.Add(new Category { Name = "Camas" });
                _context.Categories.Add(new Category { Name = "Celulares" });
                _context.Categories.Add(new Category { Name = "Cerámica" });
                _context.Categories.Add(new Category { Name = "Cocina" });
                _context.Categories.Add(new Category { Name = "Colchones" });
                _context.Categories.Add(new Category { Name = "Comedores" });
                _context.Categories.Add(new Category { Name = "Computadores" });
                _context.Categories.Add(new Category { Name = "Confitería" });
                _context.Categories.Add(new Category { Name = "Cortinas" });
                _context.Categories.Add(new Category { Name = "Cosmeticos" });
                _context.Categories.Add(new Category { Name = "Cuadros" });
                _context.Categories.Add(new Category { Name = "Cubiertos" });
                _context.Categories.Add(new Category { Name = "Electrodomesticos" });
                _context.Categories.Add(new Category { Name = "Erotismo" });
                _context.Categories.Add(new Category { Name = "Escritorios" });
                _context.Categories.Add(new Category { Name = "Espejos" });
                _context.Categories.Add(new Category { Name = "Ferretería" });
                _context.Categories.Add(new Category { Name = "Jugueteria" });
                _context.Categories.Add(new Category { Name = "Lavadoras" });
                _context.Categories.Add(new Category { Name = "Libros" });
                _context.Categories.Add(new Category { Name = "Luces Led" });
                _context.Categories.Add(new Category { Name = "Manteles" });
                _context.Categories.Add(new Category { Name = "Mascotas" });
                _context.Categories.Add(new Category { Name = "Materas" });
                _context.Categories.Add(new Category { Name = "Medicamentos" });
                _context.Categories.Add(new Category { Name = "Muebles" });
                _context.Categories.Add(new Category { Name = "Neveras" });
                _context.Categories.Add(new Category { Name = "Ollas" });
                _context.Categories.Add(new Category { Name = "Patines" });
                _context.Categories.Add(new Category { Name = "Peluches" });
                _context.Categories.Add(new Category { Name = "Perfumes" });
                _context.Categories.Add(new Category { Name = "Pintura" });
                _context.Categories.Add(new Category { Name = "Planchas y Secadores" });
                _context.Categories.Add(new Category { Name = "Plantas" });
                _context.Categories.Add(new Category { Name = "Puertas" });
                _context.Categories.Add(new Category { Name = "Relojes" });
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Ropa Deportiva" });
                _context.Categories.Add(new Category { Name = "Ropa Infantil" });
                _context.Categories.Add(new Category { Name = "Ropa Interior" });
                _context.Categories.Add(new Category { Name = "Sillas" });
                _context.Categories.Add(new Category { Name = "Tecnología" });
                _context.Categories.Add(new Category { Name = "Televisores" });
                _context.Categories.Add(new Category { Name = "Tendidos" });
                _context.Categories.Add(new Category { Name = "Toallas" });
                _context.Categories.Add(new Category { Name = "Vajillas" });
                _context.Categories.Add(new Category { Name = "Vestidos de Baño" });
                _context.Categories.Add(new Category { Name = "Videojuegos" });

                await _context.SaveChangesAsync();
            }
        }
    }
}
