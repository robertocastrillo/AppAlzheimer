using TuApp.VistasModelo;

namespace TuApp;

public partial class JuegosPaciente : ContentPage
{
    private readonly JuegoViewModel viewModel;
    public JuegosPaciente()
	{
		InitializeComponent();
        viewModel = new JuegoViewModel();
        BindingContext = viewModel;
    }
}