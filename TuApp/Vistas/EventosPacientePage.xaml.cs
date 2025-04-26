using System.Globalization;
using TuApp.VistasModelo;
namespace TuApp.Vistas
{
    public partial class EventosPacientePage : ContentPage
    {
        private EventoPacienteViewModel _viewModel;

        public EventosPacientePage()
        {
            InitializeComponent();

            // Registrar los convertidores
            Resources.Add("PrioridadColorConverter", new PrioridadColorConverter());
            Resources.Add("PrioridadTextConverter", new PrioridadTextConverter());
            Resources.Add("GreaterThanZeroConverter", new GreaterThanZeroConverter());

            // Obtener una referencia al ViewModel
            _viewModel = BindingContext as EventoPacienteViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            // Verificar si el ViewModel existe
            if (_viewModel == null)
            {
                _viewModel = BindingContext as EventoPacienteViewModel;
            }
            // Carga los eventos automáticamente al aparecer la página
            if (_viewModel != null)
            {
                await _viewModel.CargarEventosPacienteAsync();

                // Esta línea ya no es necesaria porque CargarEventosPacienteAsync ya 
                // llama a OnPropertyChanged internamente
                // No intentes llamar a _viewModel.OnPropertyChanged directamente
            }
        }

        private async void RegresarInicio_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new InicioPaciente());
        }
    }

    // Convertidor para mostrar el color según la prioridad
    public class PrioridadColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int prioridad)
            {
                return prioridad switch
                {
                    1 => Color.FromHex("#e74c3c"), // Alta - Rojo
                    2 => Color.FromHex("#f39c12"), // Media - Naranja
                    3 => Color.FromHex("#2ecc71"), // Baja - Verde
                    _ => Color.FromHex("#95a5a6"), // Por defecto - Gris
                };
            }
            return Color.FromHex("#95a5a6");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Convertidor para mostrar el texto de la prioridad
    public class PrioridadTextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int prioridad)
            {
                return prioridad switch
                {
                    1 => "Alta",
                    2 => "Media",
                    3 => "Baja",
                    _ => $"Prioridad {prioridad}"
                };
            }
            return "Prioridad";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Convertidor para verificar si un número es mayor que cero
    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count > 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}