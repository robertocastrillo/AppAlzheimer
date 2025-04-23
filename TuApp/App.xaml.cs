namespace TuApp
{
    public partial class App : Application
    {
        public const string API_URL = "https://api20250422175315-g0gab5gedbf4c4e0.canadacentral-01.azurewebsites.net/api/";

        //public const string API_URL = "https://localhost:44302/api/";
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }
    }
}
