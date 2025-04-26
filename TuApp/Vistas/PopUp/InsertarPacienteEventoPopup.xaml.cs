using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using TuApp.Entidades;
using TuApp.ViewModels;

namespace TuApp;

public partial class InsertarPacienteEventoPopup : Popup
{
    private readonly RelacionViewModel viewModel;
    public InsertarPacienteEventoPopup()
	{
		InitializeComponent();
        viewModel = new RelacionViewModel();
        BindingContext = viewModel;
    }
    private void OnPacienteTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Usuario paciente)
        {
            Close(paciente); // Devuelve el paciente seleccionado
        }
    }


    private void OnCerrarClicked(object sender, EventArgs e)
    {
        Close(null);
    }
}