namespace Delf_Apk
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var apiService = new ApiService();
            MainPage = new NavigationPage(new LoginPage());
        }
        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    return new Window(new AppShell());
        //}

    }
}