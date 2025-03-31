using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
using TuApp.Entidades;
using TuApp.Entidades.Entity;

namespace TuApp.Vistas;

public partial class JuegoCuidador : ContentPage
{

    public JuegoCuidador()
    {
        InitializeComponent();
    }

    private async void JuegoSeleccionado(object sender, SelectionChangedEventArgs e)
    {
        var juego = e.CurrentSelection.FirstOrDefault() as ResObtenerJuegosCuidador;
        if (juego != null)
        {
            await Navigation.PushAsync(new DetalleJuego(juego));
        }
        ((CollectionView)sender).SelectedItem = null;
    }

    private async void AgregarJuego_Clicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new CrearJuegoPage());
    }
}