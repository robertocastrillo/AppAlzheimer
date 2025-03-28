using TuApp;
using TuApp.Entidades.Entity;
namespace TuApp;

public partial class InicioPage : ContentPage
{


    public InicioPage()
    {
        InitializeComponent();

        lblBienvenida.Text = $"¡Bienvenido, {SesionActiva.sesionActiva.usuario.Nombre}!";
        lblCorreo.Text = SesionActiva.sesionActiva.usuario.CorreoElectronico;
        lblDireccion.Text = string.IsNullOrEmpty(SesionActiva.sesionActiva.usuario.Direccion) ? "No definida" : SesionActiva.sesionActiva.usuario.Direccion;
        lblFechaNacimiento.Text = SesionActiva.sesionActiva.usuario.FechaNacimiento.ToString("dd/MM/yyyy");
        lblTipoUsuario.Text = SesionActiva.sesionActiva.usuario.IdTipoUsuario == 1 ? "Paciente" : "Cuidador";
    }
}