using TuApp; 
namespace TuApp;

public partial class InicioPage : ContentPage
{
    private Usuario usuario;

    public InicioPage(Usuario usuario)
    {
        InitializeComponent();
        this.usuario = usuario;

        lblBienvenida.Text = $"¡Bienvenido, {usuario.Nombre}!";
        lblCorreo.Text = usuario.CorreoElectronico;
        lblDireccion.Text = string.IsNullOrEmpty(usuario.Direccion) ? "No definida" : usuario.Direccion;
        lblFechaNacimiento.Text = usuario.FechaNacimiento.ToString("dd/MM/yyyy");
        lblTipoUsuario.Text = usuario.IdTipoUsuario == 1 ? "Paciente" : "Cuidador";
    }
}