namespace Delf_Apk
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            var apiService = new ApiService();
        }
    }
}
