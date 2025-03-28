namespace TuApp.Vistas;

public partial class ConfiguracionPage : ContentPage
{
    public ConfiguracionPage()
    {
        InitializeComponent();
    }

    private async void ActualizarContrasena_Clicked(object sender, EventArgs e)
    {
        // Navegar a la página para actualizar contraseña
        await DisplayAlert("Acción", "Actualizar contraseña", "OK");
    }

    private async void EditarPin_Clicked(object sender, EventArgs e)
    {
        // Navegar a la página para definir/editar PIN
        await DisplayAlert("Acción", "Editar PIN", "OK");
    }

    private async void EliminarPin_Clicked(object sender, EventArgs e)
    {
        // Navegar a la página para eliminar PIN
        await DisplayAlert("Acción", "Eliminar PIN", "OK");
    }

    private async void RelacionPaciente_Clicked(object sender, EventArgs e)
    {
        // Navegar a la página para relación paciente-cuidador
        await DisplayAlert("Acción", "Relación Paciente", "OK");
    }
}
