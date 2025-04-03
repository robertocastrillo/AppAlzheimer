using TuApp.Entidades.Entity;
using TuApp.VistasModelo;

namespace TuApp.Vistas;

public partial class MensajesPage : ContentPage
{
    public MensajesPage()
    {
        InitializeComponent();
        BindingContext = new TuApp.VistasModelo.MensajeViewModel();
    }
}
