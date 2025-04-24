using TuApp.Styles;
using TuApp.Entidades.Entity;

namespace TuApp.Vistas
{
    public partial class ConfiguracionPacientePage : ContentPage
    {
        private PinDialog pinDialog;
        private CustomAlertDialog customAlertDialog;

        public ConfiguracionPacientePage()
        {
            InitializeComponent();

            // Crear los diálogos
            pinDialog = new PinDialog();
            customAlertDialog = new CustomAlertDialog();

            // Guardar el contenido original
            var originalContent = Content;

            // Crear una nueva grid para contener todo
            var mainGrid = new Grid();

            // Añadir el contenido original
            mainGrid.Children.Add(originalContent);

            // Añadir los diálogos (inicialmente invisibles)
            mainGrid.Children.Add(pinDialog);
            mainGrid.Children.Add(customAlertDialog);

            // Establecer la grid como el nuevo contenido
            Content = mainGrid;
        }

        private async void RegresarInicio_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new InicioPaciente());
        }

        private async void GenerarPin_Clicked(object sender, EventArgs e)
        {
            if (await VerificarPin())
            {
                Application.Current.MainPage = new NavigationPage(new GenerarPinPacientePage());
            }
        }

        private async void EditarPin_Clicked(object sender, EventArgs e)
        {
            if (await VerificarPin())
            {
                Application.Current.MainPage = new NavigationPage(new EditarPinPacientePage());
            }
        }

        private async void EliminarPin_Clicked(object sender, EventArgs e)
        {
            if (await VerificarPin())
            {
                Application.Current.MainPage = new NavigationPage(new EliminarPinPacientePage());
            }
        }

        private async void RelacionCuidador_Clicked(object sender, EventArgs e)
        {
            if (await VerificarPin())
            {
                Application.Current.MainPage = new NavigationPage(new RelacionCuidadorPage());
            }
        }

        private async void CerrarSesion_Clicked(object sender, EventArgs e)
        {
            // Verificar PIN antes de cerrar sesión
            if (await VerificarPin())
            {
                var confirmar = await DisplayAlert("Confirmar", "¿Está seguro que desea cerrar sesión?", "Sí", "No");
                if (confirmar)
                {
                    // Limpiar la sesión activa
                    SesionActiva.sesionActiva = null;
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                }
            }
        }

        private async Task<bool> VerificarPin()
        {
            var usuario = SesionActiva.sesionActiva.usuario;

            // Verificar si el usuario es un paciente y tiene un PIN configurado
            if (usuario.IdTipoUsuario == 1 && usuario.pin != null && !string.IsNullOrEmpty(usuario.pin.Codigo))
            {
                // Mostrar el diálogo PIN
                string pinIngresado = await pinDialog.ShowAsync();

                // Si el usuario canceló (pinIngresado es null) o el PIN es incorrecto
                if (pinIngresado == null)
                {
                    return false; // El usuario canceló la entrada
                }
                else if (pinIngresado != usuario.pin.Codigo)
                {
                    // PIN incorrecto - mostrar error
                    await customAlertDialog.ShowAsync("Verificación fallida", "El PIN ingresado es incorrecto.", "Aceptar");
                    return false;
                }
                else
                {
                    // PIN correcto - mostrar confirmación y permitir continuar
                    await customAlertDialog.ShowAsync("Verificación exitosa", "PIN correcto. Puede continuar.", "Aceptar");
                    return true;
                }
            }

            // Si el usuario no es un paciente o no tiene PIN configurado, permitir el acceso
            return true;
        }
    }
}