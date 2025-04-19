using TuApp.ViewModels;

namespace TuApp.Vistas;

public partial class GenerarPinPacientePage : ContentPage
{
    private GenerarPinPacienteViewModel _viewModel;

    public GenerarPinPacientePage()
    {
        InitializeComponent();
        _viewModel = new GenerarPinPacienteViewModel(Navigation, this);
        BindingContext = _viewModel;
    }

    // Manteniendo los manejadores de eventos originales para compatibilidad con el XAML
    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }

    private void GenerarPin_Clicked(object sender, EventArgs e)
    {
        // Actualizar la propiedad del ViewModel con el valor actual del Entry
        _viewModel.Pin = PinEntry.Text;

        // Ejecutar el comando directamente
        if (_viewModel.GenerarPinCommand.CanExecute(null))
            _viewModel.GenerarPinCommand.Execute(null);
    }
}