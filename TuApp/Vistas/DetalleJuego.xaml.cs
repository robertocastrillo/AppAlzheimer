using TuApp.Entidades.Entity;


namespace TuApp.Vistas;

public partial class DetalleJuego : ContentPage
{
    public DetalleJuego(Juego juego)
    {
        InitializeComponent();
        BindingContext = juego;
    }
}
