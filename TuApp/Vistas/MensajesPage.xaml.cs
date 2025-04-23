using TuApp.Entidades.Entity;
using TuApp.VistasModelo;

namespace TuApp.Vistas;


public partial class MensajesPage : ContentPage
{
    private MensajeViewModel ViewModel => BindingContext as MensajeViewModel;

    public MensajesPage()
    {
        InitializeComponent();

        // Ocultar entrada al iniciar
        MensajePanel.IsVisible = false;

        // Si ya viene con paciente seleccionado por alguna razón
        if (ViewModel?.PacienteSeleccionado != null)
            MensajePanel.IsVisible = true;
    }

    private void PacientesCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Usuario paciente)
        {
            // Ya se seleccionó un paciente
            MensajePanel.IsVisible = true;
        }
        else
        {
            // Ninguno seleccionado
            MensajePanel.IsVisible = false;
        }
    }
    private void Paciente_Tapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Usuario usuario)
        {
            if (BindingContext is MensajeViewModel vm)
            {
                vm.PacienteSeleccionado = usuario;
            }
        }
    }
}
