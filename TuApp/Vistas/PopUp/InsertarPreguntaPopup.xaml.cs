using CommunityToolkit.Maui.Views;
using SkiaSharp;
using System;
using System.IO;
using TuApp.Entidades;

namespace TuApp
{
    public partial class InsertarPreguntaPopup : Popup
    {
        private byte[] imagenComprimida;

        public InsertarPreguntaPopup()
        {
            InitializeComponent();
        }

        private async void OnSubirImagenClicked(object sender, EventArgs e)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Seleccioná una imagen",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                try
                {
                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);

                    byte[] imageBytesOriginal = ms.ToArray();
                    imagenComprimida = ComprimirImagen(imageBytesOriginal, calidad: 75, ancho: 400, alto: 300);

                    lblResultado.Text = $"Imagen cargada. Tamaño: {imagenComprimida.Length} bytes";
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Cancelado", "No se seleccionó ninguna imagen.", "OK");
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
            string descripcion = descripcionEntry.Text?.Trim();

            if (string.IsNullOrEmpty(descripcion))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar una descripción.", "OK");
                return;
            }

            if (imagenComprimida == null || imagenComprimida.Length == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe seleccionar una imagen.", "OK");
                return;
            }

            Pregunta nueva = new Pregunta
            {
                Descripcion = descripcion,
                Imagen = imagenComprimida
            };

            Close(nueva);
        }
    }
}
