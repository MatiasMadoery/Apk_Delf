using System;
using Microsoft.Maui.Controls;

namespace Delf_Apk
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiService apiService;

        public LoginPage()
        {
            InitializeComponent();
            apiService = new ApiService();
        }

        // M�todo llamado cuando el usuario hace clic en "Iniciar Sesi�n"
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var username = UsernameEntry.Text;
            var password = PasswordEntry.Text;

            // Validamos que el usuario y la contrase�a no est�n vac�os
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Por favor ingrese usuario y contrase�a", "OK");
                return;
            }

            // Intentamos realizar el login
            var isValid = await apiService.LoginAsync(username, password);

            if (isValid)
            {
                // Si el login es correcto, redirigimos a la p�gina de pedidos
                await Navigation.PushAsync(new MainPage(apiService));
            }
            else
            {
                // Si el login falla, mostramos un mensaje de error
                await DisplayAlert("Error", "Usuario o contrase�a incorrectos", "OK");
            }
        }
    }
}

