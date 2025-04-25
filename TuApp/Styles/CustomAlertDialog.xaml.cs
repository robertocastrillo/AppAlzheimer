using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TuApp.Styles
{
    public partial class CustomAlertDialog : ContentView
    {
        private TaskCompletionSource<bool> tcs;
        private TaskCompletionSource<string> confirmationTcs;

        public CustomAlertDialog()
        {
            InitializeComponent();
        }

        public Task ShowAsync(string title, string message, string buttonText)
        {
            tcs = new TaskCompletionSource<bool>();

            // Configurar el contenido
            TitleLabel.Text = title;
            MessageLabel.Text = message;
            ActionButton.Text = buttonText;

            // Ocultar el botón de cancelar si estuviera visible
            CancelButton.IsVisible = false;

            // Mostrar el diálogo
            IsVisible = true;

            return tcs.Task;
        }

        private void OnButtonClicked(object sender, System.EventArgs e)
        {
            IsVisible = false;

            // Si estamos en modo confirmación
            if (confirmationTcs != null && !confirmationTcs.Task.IsCompleted)
            {
                confirmationTcs.TrySetResult(ActionButton.Text);
                confirmationTcs = null;
            }
            else
            {
                // Modo normal
                tcs?.TrySetResult(true);
            }
        }

        private void OnCancelButtonClicked(object sender, System.EventArgs e)
        {
            IsVisible = false;

            // Solo actúa en modo confirmación
            if (confirmationTcs != null && !confirmationTcs.Task.IsCompleted)
            {
                confirmationTcs.TrySetResult(CancelButton.Text);
                confirmationTcs = null;
            }
        }

        public Task<string> ShowWithConfirmationAsync(string title, string message, string acceptButtonText, string cancelButtonText)
        {
            confirmationTcs = new TaskCompletionSource<string>();

            // Configurar el diálogo
            TitleLabel.Text = title;
            MessageLabel.Text = message;
            ActionButton.Text = acceptButtonText;
            CancelButton.Text = cancelButtonText;

            // Mostrar botón de cancelar para este tipo de diálogo
            CancelButton.IsVisible = true;

            // Mostrar el diálogo
            IsVisible = true;

            // Devolver la tarea que se completará cuando se pulse un botón
            return confirmationTcs.Task;
        }
    }
}