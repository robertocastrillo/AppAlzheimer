using Newtonsoft.Json;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqUsuario;
using TuApp.Entidades.Res.ResUsuario;
using TuApp.ViewModels;

using CommunityToolkit.Mvvm.Input;



namespace TuApp.Vistas;

public partial class EditarPinPacientePage : ContentPage
{
    private EditarPinPacienteViewModel _viewModel;

    public EditarPinPacientePage()
    {
        InitializeComponent();
        _viewModel = new EditarPinPacienteViewModel(Navigation, this);
        BindingContext = _viewModel;
    }

    // Keeping the original event handlers to maintain compatibility with the XAML
    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }

    private void ActualizarPin_Clicked(object sender, EventArgs e)
    {
        // Update ViewModel properties with current Entry values
        _viewModel.PinActual = PinActualEntry.Text;
        _viewModel.NuevoPin = NuevoPinEntry.Text;

        // Execute the command directly
        if (_viewModel.ActualizarPinCommand.CanExecute(null))
            _viewModel.ActualizarPinCommand.Execute(null);
    }
}