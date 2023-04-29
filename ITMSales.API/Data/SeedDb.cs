using System.Runtime.InteropServices;
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
        private readonly IFileStorage _fileStorage;

        public SeedDb(
            DataContext context,
            IApiService apiService,
            IUserHelper userHelper,
            IFileStorage fileStorage
            )
        {
            _context        = context;
            _apiService     = apiService;
            _userHelper     = userHelper;
            _fileStorage    = fileStorage;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
            await CheckRolesAsync();
            await CheckProductsAsync();
            await CheckUserAsync("1010", "Oscar", "Agudelo", "magoos@yopmail.com", "3178378851", "A donde no suben los taxis", "test.jpg", UserType.Admin);
            await CheckUserAsync("5050", "Bob", "Marley", "bob@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "bob.jpg", UserType.User);
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, string image, UserType userType)
        {
            var user = await _userHelper.GetUserAsync(email);

            if (user == null)
            {
                var city = await _context.Cities.FirstOrDefaultAsync(x => x.Name == "Medellín");

                if (city == null)
                {
                    city = await _context.Cities.FirstOrDefaultAsync();
                }

                string filePath;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    filePath = $"{Environment.CurrentDirectory}\\Images\\users\\{image}";
                }
                else
                {
                    filePath = $"{Environment.CurrentDirectory}/Images/users/{image}";
                }

                var fileBytes = File.ReadAllBytes(filePath);
                var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "users");

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
                    Photo = imagePath
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
                        Country? country = await _context.Countries!.FirstOrDefaultAsync(c => c.Name == countryResponse.Name!)!;

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
                _context.Categories.Add(new Category { Name = "Apple" });
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
                _context.Categories.Add(new Category { Name = "Deportes" });
                _context.Categories.Add(new Category { Name = "Electrodomesticos" });
                _context.Categories.Add(new Category { Name = "Erotismo" });
                _context.Categories.Add(new Category { Name = "Escritorios" });
                _context.Categories.Add(new Category { Name = "Espejos" });
                _context.Categories.Add(new Category { Name = "Ferretería" });
                _context.Categories.Add(new Category { Name = "Gamer" });
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
                _context.Categories.Add(new Category { Name = "Nutrición" });
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

        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                await AddProductAsync("Adidas Barracuda", 270000M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "adidas_barracuda.png" });
                await AddProductAsync("Adidas Superstar", 250000M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "Adidas_superstar.png" });
                await AddProductAsync("AirPods", 1300000M, 12F, new List<string>() { "Tecnología", "Apple" }, new List<string>() { "airpos.png", "airpos2.png" });
                await AddProductAsync("Audifonos Bose", 870000M, 12F, new List<string>() { "Tecnología" }, new List<string>() { "audifonos_bose.png" });
                await AddProductAsync("Bicicleta Ribble", 12000000M, 6F, new List<string>() { "Deportes" }, new List<string>() { "bicicleta_ribble.png" });
                await AddProductAsync("Camisa Cuadros", 56000M, 24F, new List<string>() { "Ropa" }, new List<string>() { "camisa_cuadros.png" });
                await AddProductAsync("Casco Bicicleta", 820000M, 12F, new List<string>() { "Deportes" }, new List<string>() { "casco_bicicleta.png", "casco.png" });
                await AddProductAsync("iPad", 2300000M, 6F, new List<string>() { "Tecnología", "Apple" }, new List<string>() { "ipad.png" });
                await AddProductAsync("iPhone 13", 5200000M, 6F, new List<string>() { "Tecnología", "Apple" }, new List<string>() { "iphone13.png", "iphone13b.png", "iphone13c.png", "iphone13d.png" });
                await AddProductAsync("Mac Book Pro", 12100000M, 6F, new List<string>() { "Tecnología", "Apple" }, new List<string>() { "mac_book_pro.png" });
                await AddProductAsync("Mancuernas", 370000M, 12F, new List<string>() { "Deportes" }, new List<string>() { "mancuernas.png" });
                await AddProductAsync("Mascarilla Cara", 26000M, 100F, new List<string>() { "Belleza" }, new List<string>() { "mascarilla_cara.png" });
                await AddProductAsync("New Balance 530", 180000M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "newbalance530.png" });
                await AddProductAsync("New Balance 565", 179000M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "newbalance565.png" });
                await AddProductAsync("Nike Air", 233000M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "nike_air.png" });
                await AddProductAsync("Nike Zoom", 249900M, 12F, new List<string>() { "Calzado", "Deportes" }, new List<string>() { "nike_zoom.png" });
                await AddProductAsync("Buso Adidas Mujer", 134000M, 12F, new List<string>() { "Ropa", "Deportes" }, new List<string>() { "buso_adidas.png" });
                await AddProductAsync("Suplemento Boots Original", 15600M, 12F, new List<string>() { "Nutrición" }, new List<string>() { "Boost_Original.png" });
                await AddProductAsync("Whey Protein", 252000M, 12F, new List<string>() { "Nutrición" }, new List<string>() { "whey_protein.png" });
                await AddProductAsync("Arnes Mascota", 25000M, 12F, new List<string>() { "Mascotas" }, new List<string>() { "arnes_mascota.png" });
                await AddProductAsync("Cama Mascota", 99000M, 12F, new List<string>() { "Mascotas" }, new List<string>() { "cama_mascota.png" });
                await AddProductAsync("Teclado Gamer", 67000M, 12F, new List<string>() { "Gamer", "Tecnología" }, new List<string>() { "teclado_gamer.png" });
                await AddProductAsync("Silla Gamer", 980000M, 12F, new List<string>() { "Gamer", "Tecnología" }, new List<string>() { "silla_gamer.png" });
                await AddProductAsync("Mouse Gamer", 132000M, 12F, new List<string>() { "Gamer", "Tecnología" }, new List<string>() { "mouse_gamer.png" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddProductAsync(string name, decimal price, float stock, List<string> categories, List<string> images)
        {
            Product prodcut = new()
            {
                Description = name,
                Name = name,
                Price = price,
                Stock = stock,
                ProductCategories = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>()
            };

            foreach (var categoryName in categories)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);

                if (category != null)
                {
                    prodcut.ProductCategories.Add(new ProductCategory { Category = category });
                }
            }

            foreach (string? image in images)
            {
                string filePath;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    filePath = $"{Environment.CurrentDirectory}\\Images\\products\\{image}";
                }
                else
                {
                    filePath = $"{Environment.CurrentDirectory}/Images/products/{image}";
                }

                var fileBytes = File.ReadAllBytes(filePath);

                var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "products");

                prodcut.ProductImages.Add(new ProductImage { Image = imagePath });
            }

            _context.Products.Add(prodcut);
        }
    }
}
