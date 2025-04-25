using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui;
using SkiaSharp;
using System;
using System.IO;
using TuApp.Entidades;
using TuApp.VistasModelo;
using TuApp.Styles; // Añadido para importar los custom dialogs

namespace TuApp
{
    public partial class InsertarPreguntaPopup : Popup
    {
        private PreguntaViewModel viewModel;
        // Declaración del diálogo personalizado estático para los popups
        private static readonly CustomAlertDialog customAlertDialog = new CustomAlertDialog();

        public InsertarPreguntaPopup()
        {
            InitializeComponent();

            // Para popups, necesitamos asegurar que el diálogo esté en la página principal
            EnsureDialogInMainPage();
        }

        // Método para asegurar que el diálogo esté en la página principal
        private void EnsureDialogInMainPage()
        {
            var mainPage = Application.Current.MainPage;
            if (mainPage != null)
            {
                // Si mainPage es una ContentPage directamente
                if (mainPage is ContentPage contentPage)
                {
                    AddDialogToPage(contentPage);
                }
                // Si mainPage es NavigationPage
                else if (mainPage is NavigationPage navPage && navPage.CurrentPage is ContentPage currentPage)
                {
                    AddDialogToPage(currentPage);
                }
            }
        }

        // Método para añadir el diálogo a una página
        private void AddDialogToPage(ContentPage page)
        {
            // Verificar si el diálogo ya ha sido añadido
            if (page.Content is Grid grid)
            {
                bool dialogExists = false;
                foreach (var child in grid.Children)
                {
                    if (child is CustomAlertDialog)
                    {
                        dialogExists = true;
                        break;
                    }
                }

                if (!dialogExists)
                {
                    grid.Children.Add(customAlertDialog);
                }
            }
            else
            {
                // Guardar el contenido original
                var originalContent = page.Content;

                // Crear una nueva grid para contener todo
                var mainGrid = new Grid();

                // Añadir el contenido original
                mainGrid.Children.Add(originalContent);

                // Añadir el diálogo (inicialmente invisible)
                mainGrid.Children.Add(customAlertDialog);

                // Establecer la grid como el nuevo contenido
                page.Content = mainGrid;
            }
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            string descripcion = descripcionEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                // Reemplazar DisplayAlert con customAlertDialog
                await customAlertDialog.ShowAsync("Error", "Debe ingresar una descripción.", "OK");
                return;
            }
            var pregunta = new Pregunta
            {
                Descripcion = descripcion,
            };
            Close(pregunta);
        }
    }
}