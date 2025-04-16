using CommunityToolkit.Maui.Views;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
using TuApp.Entidades;
using TuApp.Entidades.Entity;
using TuApp;
using TuApp.VistasModelo;
using TuApp.Vistas.VistasJuegoCuidador;

namespace TuApp.Vistas;

public partial class JuegosCuidador : ContentPage
{

    private readonly JuegoViewModel viewModel;
    public JuegosCuidador()
    {
        InitializeComponent();
        viewModel = new JuegoViewModel();
        BindingContext = viewModel;
    }

    private async void JuegoSeleccionado(object sender, SelectionChangedEventArgs e)
    {
        var juego = e.CurrentSelection.FirstOrDefault() as JuegoCuidador;
        if (juego != null)
        {
            await Navigation.PushAsync(new DetalleJuego(juego));
        }
        ((CollectionView)sender).SelectedItem = null;
    }
    private async void AgregarJuego_Clicked(object sender, EventArgs e)
    {
        var vm = BindingContext as JuegoViewModel;

        var popup = new InsertarJuegoPopup(vm);
        var nuevoJuego = await this.ShowPopupAsync(popup) as JuegoCuidador;

        if (nuevoJuego != null && vm != null)
        {
            vm.ListaJuegos.Add(nuevoJuego);
        }
    }
    private async void EliminarJuego_Clicked(object sender, EventArgs e)
    {
        var vm = BindingContext as JuegoViewModel;

        var button = sender as Button;
        if (button?.CommandParameter is int idJuego && vm != null)
        {
            await vm.EliminarJuego(idJuego); // ? aquí te faltaba el await
        }
    }

    private async void AbrirPacientesAsignados_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var juego = button?.CommandParameter as JuegoCuidador;

        if (juego != null)
        {
            await Navigation.PushAsync(new PacientesJuego(juego, viewModel));
        }
    }

}