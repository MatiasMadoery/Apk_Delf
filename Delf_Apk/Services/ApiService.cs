using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Delf_Apk.Models;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string _apiUrl = "http://localhost:5081/api/";

    public ApiService()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    // Realiza el login con usuario y contraseña
    public async Task<bool> LoginAsync(string username, string password)
    {
        var loginRequest = new
        {
            Usuario = username,
            Contrasena = password
        };

        var json = JsonSerializer.Serialize(loginRequest);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_apiUrl + "Usuarios/login", content);

        return response.IsSuccessStatusCode;
    }

    // Método genérico para obtener listas
    private async Task<List<T>?> GetListAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(_apiUrl + endpoint);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Ignora diferencias de mayúsculas/minúsculas
            });
        }
        return new List<T>();
    }

    // Trae Lista de clientes
    public Task<List<Cliente>?> GetClientes() => GetListAsync<Cliente>("Clientes");

    // Trae Lista de categorias
    public Task<List<Categoria>?> GetCategorias() => GetListAsync<Categoria>("Categorias");

    // Trae Lista de artículos
    public Task<List<Articulo>?> GetArticulos() => GetListAsync<Articulo>("Articulos");

    // Trae Lista de viajantes
    public Task<List<Viajante>?> GetViajantes() => GetListAsync<Viajante>("Viajantes");

    // Trae Lista de pedidos
    public async Task<List<Pedido>?> GetPedidos()
    {
        try
        {
            return await GetListAsync<Pedido>("Pedidos");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener pedidos: {ex.Message}");
            return new List<Pedido>();
        }
    }

    // Crear pedido
    public async Task<bool> CrearPedido(Pedido pedido)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("http://localhost:5081/api/Pedidos", pedido);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error al crear el pedido: {errorContent}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción en CrearPedido: {ex.Message}");
            return false;
        }
    }

    // Obtiene un cliente específico por ID e incluye su viajante
    public async Task<Cliente?> GetCliente(int id)
    {
        var cliente = await _httpClient.GetFromJsonAsync<Cliente>($"{_apiUrl}Clientes/{id}");
        if (cliente != null)
        {
            cliente.Viajante = await GetViajante(cliente.ViajanteId);
        }
        return cliente;
    }


    // Obtiene un viajante específico por ID
    public async Task<Viajante?> GetViajante(int id)
    {
        return await _httpClient.GetFromJsonAsync<Viajante>($"{_apiUrl}Viajantes/{id}");
    }
}





