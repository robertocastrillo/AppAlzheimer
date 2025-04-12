
using TuApp.Entidades.Entity;
using TuApp.VistasModelo;

namespace TuApp;

public partial class InsertarJuegoPopup : CommunityToolkit.Maui.Views.Popup
{
    private JuegoViewModel viewModel;

    public InsertarJuegoPopup(JuegoViewModel vm)
    {
        InitializeComponent();
        viewModel = vm;
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        string nombre = nombreEntry.Text?.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar un nombre", "OK");
            return;
        }

        // ?? Llamar directamente al método del ViewModel
        await viewModel.InsertarJuego(nombre);

        // Cerrar popup
        Close();
    }

}
