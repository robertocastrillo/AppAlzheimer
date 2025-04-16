
using CommunityToolkit.Maui.Views;
using SkiaSharp;
using TuApp.Entidades;
using TuApp.VistasModelo;

namespace TuApp.Vistas;

public partial class DetallePregunta : ContentPage
{
    private byte[] imagenComprimida;
    private readonly Pregunta pregunta;
    private readonly int idJuego;
    private readonly int idUsuario;
    private PreguntaViewModel viewModel;
    public bool MostrarBotonEliminar { get; set; } = true;
    public DetallePregunta(Pregunta pregunta, PreguntaViewModel viewModel, int idJuego, int idUsuario)
    {
        InitializeComponent();

        this.pregunta = pregunta;
        this.viewModel = viewModel;
        this.idJuego = idJuego;
        this.idUsuario = idUsuario;

        BindingContext = pregunta;
    }

    public DetallePregunta(Pregunta pregunta)
    {
        
        InitializeComponent();

        this.pregunta = pregunta;
        BindingContext = pregunta;
        btnGuardar.IsVisible = false;
        btnAgregar.IsVisible = false;
        btnSubirImagen.IsVisible = false;
        MostrarBotonEliminar = false;


    }

    private async void AgregarOpcion_Clicked(object sender, EventArgs e)
    {
        var popup = new InsertarOpcionPopup();
        var opcion = await this.ShowPopupAsync(popup) as Opcion;

        if (BindingContext is Pregunta pregunta)
        {
            pregunta.opciones.Add(opcion);
        }
    }

    private async void OnSubirImagenClicked(object sender, EventArgs e)
    {
        try
        {
            // Invocar el file picker
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Seleccioná una imagen",
                FileTypes = FilePickerFileType.Images
            });

            if (result == null)
            {
                // Usuario canceló el diálogo de selección
                await Application.Current.MainPage.DisplayAlert("Cancelado", "No se seleccionó ninguna imagen.", "OK");
                return;
            }

            // Validación extra: extensión permitida (opcional)
            var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
            if (!extensionesPermitidas.Contains(Path.GetExtension(result.FileName).ToLower()))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Formato no soportado. Seleccione una imagen JPG o PNG.", "OK");
                return;
            }

            using var stream = await result.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            byte[] imagenOriginal = ms.ToArray();

            // Validación del tamaño (opcional)
            if (imagenOriginal.Length > 2 * 1024 * 1024) // 2MB
            {
                await Application.Current.MainPage.DisplayAlert("Advertencia", "La imagen es muy grande. Seleccione una menor a 2MB.", "OK");
                return;
            }

            // Comprimir imagen y actualizar en la interfaz
            imagenComprimida = ComprimirImagen(imagenOriginal);
            if (BindingContext is Pregunta pregunta)
            {
                pregunta.Imagen = imagenComprimida;
            }
            imagenPregunta.Source = ImageSource.FromStream(() => new MemoryStream(imagenComprimida));
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error al cargar imagen", ex.Message, "OK");
        }
    }


    private byte[] ComprimirImagen(byte[] originalBytes, int calidad = 75, int ancho = 400, int alto = 300)
    {
        using var inputStream = new MemoryStream(originalBytes);
        using var skStream = new SKManagedStream(inputStream);
        using var original = SKBitmap.Decode(skStream);

        var resized = original.Resize(new SKImageInfo(ancho, alto), SKFilterQuality.Medium);
        using var image = SKImage.FromBitmap(resized);

        using var output = new MemoryStream();
        image.Encode(SKEncodedImageFormat.Jpeg, calidad).SaveTo(output);

        return output.ToArray();
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        if (pregunta == null || pregunta.opciones.Count == 0)
        {
            await DisplayAlert("Validación", "La pregunta debe tener al menos una opción.", "OK");
            return;
        }

        await viewModel.InsertarPregunta(pregunta, idJuego, idUsuario);
        await Navigation.PopAsync(); // Regresa a DetalleJuego
    }
    private void EliminarOpcion(Opcion opcion)
    {
        if (opcion != null && pregunta.opciones.Contains(opcion))
        {
            pregunta.opciones.Remove(opcion);
        }
    }

    private void EliminarOpcion_Clicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is Opcion opcion)
        {
            EliminarOpcion(opcion);
        }
    }
}