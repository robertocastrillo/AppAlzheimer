using TuApp.Entidades.Entity;

namespace TuApp.Vistas;

public partial class PerfilPage : ContentPage
{
    public PerfilPage()
    {
        InitializeComponent();

        var usuario = SesionActiva.sesionActiva.usuario;

        lblNombre.Text = $"Nombre: {usuario.Nombre}";
        lblCorreo.Text = $"Correo: {usuario.CorreoElectronico}";
        lblDireccion.Text = $"Dirección: {usuario.Direccion ?? "Sin especificar"}";
        lblFechaNacimiento.Text = $"Nacimiento: {usuario.FechaNacimiento.ToShortDateString()}";
    }
}
