using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Res.Chat;
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
    protected override bool OnBackButtonPressed()
    {
        // Si quieres personalizar el comportamiento del botón Back
        Application.Current.MainPage = new NavigationPage(new InicioPaciente());
        return true; // Indicar que hemos manejado el evento
    }
}