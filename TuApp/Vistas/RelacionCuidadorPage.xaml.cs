using TuApp.Entidades.Entity;

namespace TuApp.Vistas;

public partial class RelacionCuidadorPage : ContentPage
{
	public RelacionCuidadorPage()
	{
		InitializeComponent();
        if (SesionActiva.sesionActiva != null && SesionActiva.sesionActiva.usuario != null)
        {
            lblCodigo.Text = $"Codigo:{SesionActiva.sesionActiva.usuario.Codigo}";
        }
        else
        {
            lblCodigo.Text = "Codigo:";
        }


    }
    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new NavigationPage(new InicioPaciente());
    }
}