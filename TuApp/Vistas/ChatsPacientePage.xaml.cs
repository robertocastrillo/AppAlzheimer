using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades.Entity;

using TuApp.Entidades;
using TuApp.ViewModels;

namespace TuApp.Vistas;
public partial class ChatsPacientePage : ContentPage
{
    private ChatsPacienteViewModel _viewModel;

    public ChatsPacientePage()
    {
        InitializeComponent();
        _viewModel = new ChatsPacienteViewModel(Navigation);
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Reload messages when page appears
        _viewModel.CargarPreguntas();
    }

    // Keeping the original event handler for navigation
    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }
}