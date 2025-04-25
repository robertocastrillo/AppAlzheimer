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

            // Configurar la disponibilidad de los botones de PIN según el estado del usuario
            ConfigurarBotonesPIN();
        }

        private void ConfigurarBotonesPIN()
        {
            var usuario = SesionActiva.sesionActiva?.usuario;
            bool tienePIN = usuario != null && usuario.pin != null && !string.IsNullOrEmpty(usuario.pin.Codigo);

            // Si el usuario tiene PIN
            if (tienePIN)
            {
                // Deshabilitar el botón Crear PIN
                BtnCrearPin.BackgroundColor = Color.FromHex("#CCCCCC");
                BtnCrearPin.TextColor = Color.FromHex("#888888");
                BtnCrearPin.IsEnabled = false;

                // Habilitar los botones Editar y Eliminar
                BtnEditarPin.BackgroundColor = Color.FromHex("#e0cdff");
                BtnEditarPin.TextColor = Color.FromHex("#3b0764");
                BtnEditarPin.IsEnabled = true;

                BtnEliminarPin.BackgroundColor = Color.FromHex("#e0cdff");
                BtnEliminarPin.TextColor = Color.FromHex("#3b0764");
                BtnEliminarPin.IsEnabled = true;
            }
            // Si el usuario no tiene PIN
            else
            {
                // Habilitar el botón Crear PIN
                BtnCrearPin.BackgroundColor = Color.FromHex("#e0cdff");
                BtnCrearPin.TextColor = Color.FromHex("#3b0764");
                BtnCrearPin.IsEnabled = true;

                // Deshabilitar los botones Editar y Eliminar
                BtnEditarPin.BackgroundColor = Color.FromHex("#CCCCCC");
                BtnEditarPin.TextColor = Color.FromHex("#888888");
                BtnEditarPin.IsEnabled = false;

                BtnEliminarPin.BackgroundColor = Color.FromHex("#CCCCCC");
                BtnEliminarPin.TextColor = Color.FromHex("#888888");
                BtnEliminarPin.IsEnabled = false;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Si quieres personalizar el comportamiento del botón Back
            Application.Current.MainPage = new NavigationPage(new InicioPaciente());
            return true; // Indicar que hemos manejado el evento
        }

        private async void GenerarPin_Clicked(object sender, EventArgs e)
        {
            var usuario = SesionActiva.sesionActiva?.usuario;
            bool tienePIN = usuario != null && usuario.pin != null && !string.IsNullOrEmpty(usuario.pin.Codigo);

            // Si ya tiene PIN, no debería poder crear uno nuevo
            if (tienePIN)
            {
                await customAlertDialog.ShowAsync("Acción no permitida", "Ya tienes un PIN configurado. Si deseas cambiarlo, usa la opción 'Editar PIN'.", "Aceptar");
                return;
            }

            // Si no tiene PIN, puede crear uno directamente sin verificación
            Application.Current.MainPage = new NavigationPage(new GenerarPinPacientePage());
        }

        private async void EditarPin_Clicked(object sender, EventArgs e)
        {
            var usuario = SesionActiva.sesionActiva?.usuario;
            bool tienePIN = usuario != null && usuario.pin != null && !string.IsNullOrEmpty(usuario.pin.Codigo);

            // Si no tiene PIN, no debería poder editarlo
            if (!tienePIN)
            {
                await customAlertDialog.ShowAsync("Acción no permitida", "No tienes un PIN configurado. Primero debes crear un PIN.", "Aceptar");
                return;
            }

            // Si tiene PIN, debe verificarlo antes de editar
            if (await VerificarPin())
            {
                Application.Current.MainPage = new NavigationPage(new EditarPinPacientePage());
            }
        }

        private async void EliminarPin_Clicked(object sender, EventArgs e)
        {
            var usuario = SesionActiva.sesionActiva?.usuario;
            bool tienePIN = usuario != null && usuario.pin != null && !string.IsNullOrEmpty(usuario.pin.Codigo);

            // Si no tiene PIN, no debería poder eliminarlo
            if (!tienePIN)
            {
                await customAlertDialog.ShowAsync("Acción no permitida", "No tienes un PIN configurado para eliminar.", "Aceptar");
                return;
            }

            // Si tiene PIN, debe verificarlo antes de eliminar
            if (await VerificarPin())
            {
                Application.Current.MainPage = new NavigationPage(new EliminarPinPacientePage());
            }
        }

        private async void RelacionCuidador_Clicked(object sender, EventArgs e)
        {
            
                Application.Current.MainPage = new NavigationPage(new RelacionCuidadorPage());
            
        }

        private async void CerrarSesion_Clicked(object sender, EventArgs e)
        {
            // Verificar PIN antes de cerrar sesión
            if (await VerificarPin())
            {
                var resultado = await customAlertDialog.ShowWithConfirmationAsync(
                "Confirmar",
                "¿Está seguro que desea cerrar sesión?",
                "Sí", "No");

                if (resultado == "Sí")
                {
                    SesionActiva.sesionActiva = null;
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                }
            }
        }

        private async Task<bool> VerificarPin()
        {
            var usuario = SesionActiva.sesionActiva?.usuario;

            // Si el usuario es nulo, no hay sesión activa
            if (usuario == null)
            {
                return false;
            }

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