using CommunityToolkit.Maui.Views;
using TuApp.Entidades;
using TuApp.VistasModelo;

namespace TuApp.Vistas;

public partial class DetallePregunta : ContentPage
{
	public DetallePregunta(Pregunta pregunta)
	{
		InitializeComponent();
		BindingContext = pregunta;
	}

    private async void AgregarOpcion_Clicked(object sender, EventArgs e)
    {
        var popup = new InsertarOpcionPopup();
        var pregunta = await this.ShowPopupAsync(popup) as Pregunta;

        if (pregunta != null && BindingContext is PreguntaViewModel vm)
        {
            vm.ListaPregunta.Add(pregunta);
        }
    }
    
}