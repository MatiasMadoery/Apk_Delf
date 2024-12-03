using Delf_Apk.ViewModels;

namespace Delf_Apk
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _viewModel;

        public MainPage(ApiService apiService)
        {
            InitializeComponent();

            // Inicializamos el ViewModel pasándole el servicio de API
            _viewModel = new MainPageViewModel(apiService);

            // Establecemos el BindingContext de la vista
            BindingContext = _viewModel;
        }
    }
}

