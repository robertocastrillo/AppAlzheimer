using Newtonsoft.Json;
using System.Text;
using TuApp.Entidades.Entity;
using TuApp.Entidades.Req.ReqUsuario;

namespace TuApp.Vistas
{
    public partial class PerfilPage : ContentPage
    {
        private bool modoEdicion = false;
        private string nombreOriginal;
        private DateTime fechaNacimientoOriginal;
        private string direccionOriginal;

        public PerfilPage()
        {
            InitializeComponent();
            CargarDatosUsuario();
        }

        private void CargarDatosUsuario()
        {
            var usuario = SesionActiva.sesionActiva.usuario;

            lblBienvenida.Text = $"Hola {usuario.Nombre}, esta es tu información.";
            lblNombre.Text = $"Tu nombre: {usuario.Nombre}";
            lblFecha.Text = $"Fecha de Nacimiento: {usuario.FechaNacimiento:yyyy-MM-dd}";
            lblDireccion.Text = $"Dirección de domicilio: {usuario.Direccion}";
            lblUbicacion.Text = $"Código: {usuario.Codigo}";

            nombreOriginal = usuario.Nombre;
            fechaNacimientoOriginal = usuario.FechaNacimiento;
            direccionOriginal = usuario.Direccion;

            if (usuario.FotoPerfil != null && usuario.FotoPerfil.Length > 0)
            {
                imgPerfil.Source = ImageSource.FromStream(() => new MemoryStream(usuario.FotoPerfil));
            }
            else
            {
                imgPerfil.Source = "usuario_default.png";
            }

            txtNombre.IsVisible = false;
            dpFechaNacimiento.IsVisible = false;
            txtDireccion.IsVisible = false;
            txtPasswordActual.IsVisible = false;
            txtPasswordNueva.IsVisible = false;
        }

        private async void EditarDatos_Clicked(object sender, EventArgs e)
        {
            var usuario = SesionActiva.sesionActiva.usuario;

            if (!modoEdicion && usuario.IdTipoUsuario == 1 && !string.IsNullOrEmpty(usuario.Pin))
            {
                string pinIngresado = await DisplayPromptAsync("PIN Requerido", "Ingresa tu PIN para editar:", "Aceptar", "Cancelar", keyboard: Keyboard.Numeric);
                if (pinIngresado != usuario.Pin)
                {
                    await DisplayAlert("Error", "PIN incorrecto.", "OK");
                    return;
                }
            }

            modoEdicion = !modoEdicion;

            if (modoEdicion)
            {
                lblNombre.IsVisible = false;
                lblFecha.IsVisible = false;
                lblDireccion.IsVisible = false;

                txtNombre.IsVisible = true;
                dpFechaNacimiento.IsVisible = true;
                txtDireccion.IsVisible = true;
                txtPasswordActual.IsVisible = true;
                txtPasswordNueva.IsVisible = true;

                btnEditar.Text = "Guardar";

                txtNombre.Text = usuario.Nombre;
                dpFechaNacimiento.Date = usuario.FechaNacimiento;
                txtDireccion.Text = usuario.Direccion;
            }
            else
            {
                await GuardarCambios();
            }
        }

        private async Task GuardarCambios()
        {
            var usuario = SesionActiva.sesionActiva.usuario;

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                await DisplayAlert("Error", "El nombre es obligatorio", "OK");
                return;
            }

            bool huboCambios = txtNombre.Text != nombreOriginal ||
                                dpFechaNacimiento.Date != fechaNacimientoOriginal ||
                                txtDireccion.Text != direccionOriginal ||
                                (!string.IsNullOrWhiteSpace(txtPasswordActual.Text) && !string.IsNullOrWhiteSpace(txtPasswordNueva.Text));

            if (!huboCambios)
            {
                await DisplayAlert("Sin cambios", "No se ha hecho ningún cambio.", "Aceptar");

                // Restaurar la vista a modo lectura
                txtNombre.IsVisible = false;
                dpFechaNacimiento.IsVisible = false;
                txtDireccion.IsVisible = false;
                txtPasswordActual.IsVisible = false;
                txtPasswordNueva.IsVisible = false;

                lblNombre.IsVisible = true;
                lblFecha.IsVisible = true;
                lblDireccion.IsVisible = true;

                btnEditar.Text = "Editar Datos";
                modoEdicion = false;
                return;
            }

            string ping = usuario.Pin;
            ReqActualizarUsuario reqUsuario = new ReqActualizarUsuario
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = txtNombre.Text,
                FechaNacimiento = dpFechaNacimiento.Date,
                Direccion = txtDireccion.Text,
                Pin = ping
            };

            var jsonContent = new StringContent(JsonConvert.SerializeObject(reqUsuario), Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                var respuesta = await client.PostAsync("https://localhost:44347/api/usuario/actualizarusuario", jsonContent);
                if (!respuesta.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error", "No se pudo actualizar el usuario", "Aceptar");
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtPasswordActual.Text) && !string.IsNullOrWhiteSpace(txtPasswordNueva.Text))
            {
                ReqActualizarContrasena reqPass = new ReqActualizarContrasena
                {
                    IdUsuario = usuario.IdUsuario,
                    ContrasenaActual = txtPasswordActual.Text,
                    NuevaContrasena = txtPasswordNueva.Text,
                    Pin = ping
                };

                var passContent = new StringContent(JsonConvert.SerializeObject(reqPass), Encoding.UTF8, "application/json");
                using (HttpClient client = new HttpClient())
                {
                    var respuesta = await client.PostAsync("https://localhost:44347/api/usuario/actualizarcontrasena", passContent);
                    if (!respuesta.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Error", "No se pudo actualizar la contraseña", "Aceptar");
                        return;
                    }
                }
            }

            await DisplayAlert("Éxito", "Datos actualizados correctamente", "Aceptar");

            lblNombre.Text = $"Tu nombre: {txtNombre.Text}";
            lblFecha.Text = $"Fecha de Nacimiento: {dpFechaNacimiento.Date:yyyy-MM-dd}";
            lblDireccion.Text = $"Dirección de domicilio: {txtDireccion.Text}";

            txtNombre.IsVisible = false;
            dpFechaNacimiento.IsVisible = false;
            txtDireccion.IsVisible = false;
            txtPasswordActual.IsVisible = false;
            txtPasswordNueva.IsVisible = false;

            lblNombre.IsVisible = true;
            lblFecha.IsVisible = true;
            lblDireccion.IsVisible = true;

            btnEditar.Text = "Editar Datos";
            modoEdicion = false;
        }
    }
}
