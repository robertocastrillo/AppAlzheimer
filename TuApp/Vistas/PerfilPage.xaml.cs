using System;
using TuApp.Entidades.Entity;

namespace TuApp.Vistas;

public partial class PerfilPage : ContentPage
{
    public PerfilPage()
    {
        InitializeComponent();
        CargarDatos();
    }

    private void CargarDatos()
    {
        var usuario = SesionActiva.sesionActiva.usuario;

        lblBienvenida.Text = $"Hola {usuario.Nombre}, esta es tu información.";
        lblNombre.Text = $"Tu nombre: {usuario.Nombre}";
        lblFecha.Text = $"Fecha de Nacimiento: {usuario.FechaNacimiento.ToShortDateString()}";
        lblDireccion.Text = $"Dirección de domicilio: {usuario.Direccion}";
        lblUbicacion.Text = $"Vives en: {usuario.Codigo}";

        if (usuario.FotoPerfil != null && usuario.FotoPerfil.Length > 0)
        {
            imgPerfil.Source = ImageSource.FromStream(() => new MemoryStream(usuario.FotoPerfil));
        }
        else
        {
            imgPerfil.Source = "usuario_default.png"; // Ícono por defecto si no hay imagen
        }
    }

    private async void EditarDatos_Clicked(object sender, EventArgs e)
    {
/*        await Navigation.PushAsync(new EditarPerfilPage());*/ // Se crea luego
    }

    private async void CambiarFoto_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Foto", "Aquí podrás cambiar la foto de perfil (pendiente)", "OK");
    }
}
