using TuApp.ViewModels;

namespace TuApp.Vistas;

public partial class EliminarPinPacientePage : ContentPage
{
    private EliminarPinPacienteViewModel _viewModel;

    public EliminarPinPacientePage()
    {
        InitializeComponent();
        _viewModel = new EliminarPinPacienteViewModel(Navigation, this);
        BindingContext = _viewModel;
    }

    // Manteniendo los manejadores de eventos originales para compatibilidad con el XAML
    private async void RegresarInicio_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InicioPaciente());
    }

    private void EliminarPin_Clicked(object sender, EventArgs e)
    {
        // Actualizar la propiedad del ViewModel con el valor actual del Entry
        _viewModel.Pin = PinEntry.Text;

        // Ejecutar el comando directamente
        if (_viewModel.EliminarPinCommand.CanExecute(null))
            _viewModel.EliminarPinCommand.Execute(null);
    }
}