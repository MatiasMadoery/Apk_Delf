using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Delf_Apk.Models;

namespace Delf_Apk.ViewModels
{    public class MainPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Viajante> _viajantes;
        private Viajante _viajanteSeleccionado;
        private ObservableCollection<Cliente> _clientes;
        private Cliente _clienteSeleccionado;
        private ObservableCollection<Categoria> _categorias;
        private Categoria _categoriaSeleccionada;
        private ObservableCollection<Articulo> _articulosFiltrados;
        private ObservableCollection<ArticuloCantidad> _articulosSeleccionados;
        private ObservableCollection<Articulo> _articulosDisponibles;
        private Articulo _articuloSeleccionado;
        private int _cantidad;

        public ObservableCollection<Viajante> Viajantes
        {
            get => _viajantes;
            set { _viajantes = value; OnPropertyChanged(nameof(Viajantes)); }
        }

        public Viajante ViajanteSeleccionado
        {
            get => _viajanteSeleccionado;
            set
            {
                _viajanteSeleccionado = value;
                OnPropertyChanged(nameof(ViajanteSeleccionado));
                FiltrarClientes();
            }
        }

        public ObservableCollection<Cliente> Clientes
        {
            get => _clientes;
            set { _clientes = value; OnPropertyChanged(nameof(Clientes)); }
        }

        public ObservableCollection<Cliente> ClientesFiltrados { get; set; } = new ObservableCollection<Cliente>();

        public Cliente ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set { _clienteSeleccionado = value; OnPropertyChanged(nameof(ClienteSeleccionado)); }
        }

        public ObservableCollection<Categoria> Categorias
        {
            get => _categorias;
            set { _categorias = value; OnPropertyChanged(nameof(Categorias)); }
        }

        public Categoria CategoriaSeleccionada
        {
            get => _categoriaSeleccionada;
            set
            {
                _categoriaSeleccionada = value;
                OnPropertyChanged(nameof(CategoriaSeleccionada));
                FiltrarArticulosPorCategoria();
            }
        }

        public ObservableCollection<Articulo> ArticulosFiltrados
        {
            get => _articulosFiltrados;
            set { _articulosFiltrados = value; OnPropertyChanged(nameof(ArticulosFiltrados)); }
        }

        public ObservableCollection<Articulo> ArticulosDisponibles
        {
            get => _articulosDisponibles;
            set { _articulosDisponibles = value; OnPropertyChanged(nameof(ArticulosDisponibles)); }
        }

        public Articulo ArticuloSeleccionado
        {
            get => _articuloSeleccionado;
            set { _articuloSeleccionado = value; OnPropertyChanged(nameof(ArticuloSeleccionado)); }
        }

        public int Cantidad
        {
            get => _cantidad;
            set { _cantidad = value; OnPropertyChanged(nameof(Cantidad)); }
        }

        public ObservableCollection<ArticuloCantidad> ArticulosSeleccionados
        {
            get => _articulosSeleccionados;
            set { _articulosSeleccionados = value; OnPropertyChanged(nameof(ArticulosSeleccionados)); }
        }

        private readonly ApiService _apiService;

        public Command AgregarArticuloCommand { get; }
        public Command CrearPedidoCommand { get; }
        public Command EliminarArticuloCommand { get; }

        public MainPageViewModel(ApiService apiService)
        {
            _apiService = apiService;

            // Inicializar colecciones
            Viajantes = new ObservableCollection<Viajante>();
            Clientes = new ObservableCollection<Cliente>();
            Categorias = new ObservableCollection<Categoria>();
            ArticulosFiltrados = new ObservableCollection<Articulo>();
            ArticulosSeleccionados = new ObservableCollection<ArticuloCantidad>();
            ArticulosDisponibles = new ObservableCollection<Articulo>();

            // Cargar datos
            CargarViajantes();
            CargarClientes();
            CargarCategorias();
            CargarArticulos();

            // Inicializar comandos
            AgregarArticuloCommand = new Command(AgregarArticulo);
            CrearPedidoCommand = new Command(async () => await CrearPedidoAsync());
            EliminarArticuloCommand = new Command<ArticuloCantidad>(EliminarArticulo);
        }

        private async void CargarViajantes()
        {
            try
            {
                var viajantes = await _apiService.GetViajantes();
                if (viajantes != null)
                {
                    foreach (var viajante in viajantes)
                    {
                        Viajantes.Add(viajante);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar viajantes: {ex.Message}");
            }
        }

        private async void CargarClientes()
        {
            try
            {
                var clientes = await _apiService.GetClientes();
                if (clientes != null)
                {
                    foreach (var cliente in clientes)
                    {
                        cliente.Viajante = await _apiService.GetViajante(cliente.ViajanteId);
                        Clientes.Add(cliente);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar clientes: {ex.Message}");
            }
        }

        private async void CargarCategorias()
        {
            try
            {
                var categorias = await _apiService.GetCategorias();
                if (categorias != null)
                {
                    foreach (var categoria in categorias)
                    {
                        Categorias.Add(categoria);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar categorías: {ex.Message}");
            }
        }

        private void FiltrarArticulosPorCategoria()
        {
            ArticulosFiltrados.Clear();
            if (CategoriaSeleccionada != null)
            {
                var articulosFiltrados = ArticulosDisponibles.Where(a => a.CategoriaId == CategoriaSeleccionada.Id);
                foreach (var articulo in articulosFiltrados)
                {
                    ArticulosFiltrados.Add(articulo);
                }
            }
            else
            {
                foreach (var articulo in ArticulosDisponibles)
                {
                    ArticulosFiltrados.Add(articulo);
                }
            }
        }
        private async void CargarArticulos()
        {
            try
            {
                var articulos = await _apiService.GetArticulos();
                if (articulos != null)
                {
                    foreach (var articulo in articulos)
                    {
                        ArticulosDisponibles.Add(articulo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar artículos: {ex.Message}");
            }
        }

        private void AgregarArticulo()
        {
            if (ArticuloSeleccionado != null && Cantidad > 0)
            {
                ArticulosSeleccionados.Add(new ArticuloCantidad
                {
                    Articulo = ArticuloSeleccionado,
                    Cantidad = Cantidad
                });
                Cantidad = 0;
                ArticuloSeleccionado = null!;
            }
        }
        private void EliminarArticulo(ArticuloCantidad articulo)
        {
            if (articulo != null)
            {
                ArticulosSeleccionados.Remove(articulo);
            }
        }

        private async Task CrearPedidoAsync()
        {
            if (ClienteSeleccionado != null && ClienteSeleccionado.Viajante != null && ArticulosSeleccionados.Count > 0)
            {
                try
                {
                    string numeroPedido = await GenerarNumeroPedido();
                    var cliente = ClienteSeleccionado;
                    var pedido = new Pedido
                    {
                        Numero = numeroPedido,
                        Fecha = DateTime.Now,
                        ClienteId = cliente.Id!,
                        ArticuloCantidades = ArticulosSeleccionados!.Select(a => new ArticuloCantidad
                        {
                            ArticuloId = a.Articulo!.Id,
                            Cantidad = a.Cantidad!
                        }).ToList()
                    };
                    Console.WriteLine($"Pedido generado: {JsonSerializer.Serialize(pedido)}");
                    bool resultado = await _apiService.CrearPedido(pedido);
                    if (resultado)
                    {
                        await Application.Current!.MainPage!.DisplayAlert("Éxito", "Pedido creado correctamente", "OK");
                        ArticulosSeleccionados.Clear();
                        ClienteSeleccionado = null!;
                    }
                    else
                    {
                        await Application.Current!.MainPage!.DisplayAlert("Error", "No se pudo crear el pedido", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current!.MainPage!.DisplayAlert("Error", $"Excepción: {ex.Message}", "OK");
                }
            }
            else
            {
                string clienteInfo = ClienteSeleccionado == null ? "ClienteSeleccionado es null" :
                                     ClienteSeleccionado.Viajante == null ? "Viajante en ClienteSeleccionado es null" :
                                     "ArticulosSeleccionados.Count = " + ArticulosSeleccionados.Count;

                await Application.Current!.MainPage!.DisplayAlert("Error", $"Seleccione un cliente con viajante y al menos un artículo. {clienteInfo}", "OK");
            }
        }


        private void FiltrarClientes()
        {
            ClientesFiltrados.Clear();
            if (ViajanteSeleccionado != null)
            {
                var clientesFiltrados = Clientes.Where(c => c.ViajanteId == ViajanteSeleccionado.Id);
                foreach (var cliente in clientesFiltrados)
                {
                    ClientesFiltrados.Add(cliente);
                }
            }
        }

        private async Task<string> GenerarNumeroPedido()
        {
            try
            {
                var pedidos = await _apiService.GetPedidos();
                int ultimoNumero = 0;

                if (pedidos != null && pedidos.Any())
                {
                    ultimoNumero = pedidos
                        .Where(p => int.TryParse(p.Numero, out _))
                        .Select(p => int.Parse(p.Numero!))
                        .DefaultIfEmpty(0)
                        .Max();
                }

                return (ultimoNumero + 1).ToString("D6");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar número de pedido: {ex.Message}");
                return "000001";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}



//    public class MainPageViewModel : INotifyPropertyChanged
//    {
//        // Colecciones
//        public ObservableCollection<Viajante> Viajantes { get; set; } = new ObservableCollection<Viajante>();
//        public ObservableCollection<Cliente> Clientes { get; set; } = new ObservableCollection<Cliente>();
//        public ObservableCollection<Categoria> Categorias { get; set; } = new ObservableCollection<Categoria>();
//        public ObservableCollection<Articulo> ArticulosFiltrados { get; set; } = new ObservableCollection<Articulo>();
//        public ObservableCollection<ArticuloCantidad> ArticulosSeleccionados { get; set; } = new ObservableCollection<ArticuloCantidad>();
//        public ObservableCollection<Articulo> ArticulosDisponibles { get; set; } = new ObservableCollection<Articulo>();
//        public ObservableCollection<Cliente> ClientesFiltrados { get; set; } = new ObservableCollection<Cliente>();

//        // Propiedades
//        public Viajante ViajanteSeleccionado { get; set; }
//        public Cliente ClienteSeleccionado { get; set; }
//        public Categoria CategoriaSeleccionada { get; set; }
//        public Articulo ArticuloSeleccionado { get; set; }
//        public int Cantidad { get; set; }

//        // Comandos
//        public Command AgregarArticuloCommand { get; }
//        public Command CrearPedidoCommand { get; }
//        public Command EliminarArticuloCommand { get; }

//        private readonly ApiService _apiService;

//        public MainPageViewModel(ApiService apiService)
//        {
//            _apiService = apiService;

//            AgregarArticuloCommand = new Command(AgregarArticulo);
//            CrearPedidoCommand = new Command(async () => await CrearPedidoAsync());
//            EliminarArticuloCommand = new Command<ArticuloCantidad>(EliminarArticulo);

//            CargarDatos();
//        }

//        // Cargar datos de manera optimizada (paralela)
//        private async void CargarDatos()
//        {
//            try
//            {
//                var viajantesTask = _apiService.GetViajantes();
//                var clientesTask = _apiService.GetClientes();
//                var categoriasTask = _apiService.GetCategorias();
//                var articulosTask = _apiService.GetArticulos();

//                await Task.WhenAll(viajantesTask, clientesTask, categoriasTask, articulosTask);

//                // Viajantes
//                foreach (var viajante in viajantesTask.Result)
//                {
//                    Viajantes.Add(viajante);
//                }

//                // Clientes
//                foreach (var cliente in clientesTask.Result)
//                {
//                    cliente.Viajante = await _apiService.GetViajante(cliente.ViajanteId);
//                    Clientes.Add(cliente);
//                }

//                // Categorías
//                foreach (var categoria in categoriasTask.Result)
//                {
//                    Categorias.Add(categoria);
//                }

//                // Artículos
//                foreach (var articulo in articulosTask.Result)
//                {
//                    ArticulosDisponibles.Add(articulo);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error al cargar datos: {ex.Message}");
//            }
//        }

//        // Método optimizado para agregar artículos
//        private void AgregarArticulo()
//        {
//            if (ArticuloSeleccionado != null && Cantidad > 0)
//            {
//                var articuloExistente = ArticulosSeleccionados.FirstOrDefault(a => a.Articulo.Id == ArticuloSeleccionado.Id);
//                if (articuloExistente != null)
//                {
//                    articuloExistente.Cantidad += Cantidad;
//                }
//                else
//                {
//                    ArticulosSeleccionados.Add(new ArticuloCantidad { Articulo = ArticuloSeleccionado, Cantidad = Cantidad });
//                }
//                Cantidad = 0;
//                ArticuloSeleccionado = null!;
//            }
//        }

//        // Método optimizado para eliminar artículos
//        private void EliminarArticulo(ArticuloCantidad articulo)
//        {
//            ArticulosSeleccionados.Remove(articulo);
//        }

//        // Método optimizado para crear un pedido
//        private async Task CrearPedidoAsync()
//        {
//            if (ClienteSeleccionado != null && ArticulosSeleccionados.Count > 0)
//            {
//                var pedido = new Pedido
//                {
//                    ClienteId = ClienteSeleccionado.Id,
//                    ArticuloCantidades = ArticulosSeleccionados.Select(a => new ArticuloCantidad
//                    {
//                        ArticuloId = a.Articulo.Id,
//                        Cantidad = a.Cantidad
//                    }).ToList()
//                };

//                var resultado = await _apiService.CrearPedido(pedido);
//                if (resultado)
//                {
//                    await Application.Current.MainPage.DisplayAlert("Éxito", "Pedido creado correctamente", "OK");
//                    ArticulosSeleccionados.Clear();
//                    ClienteSeleccionado = null!;
//                }
//                else
//                {
//                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo crear el pedido", "OK");
//                }
//            }
//            else
//            {
//                await Application.Current.MainPage.DisplayAlert("Error", "Seleccione un cliente y artículos", "OK");
//            }
//        }

//        public event PropertyChangedEventHandler? PropertyChanged;
//        protected virtual void OnPropertyChanged(string propertyName)
//        {
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }
//    }
//}
