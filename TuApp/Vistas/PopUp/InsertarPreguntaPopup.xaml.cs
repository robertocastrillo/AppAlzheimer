
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui;
using SkiaSharp;
using System;
using System.IO;
using TuApp.Entidades;
using TuApp.VistasModelo;

namespace TuApp
{
    public partial class InsertarPreguntaPopup : Popup
    {

        private PreguntaViewModel viewModel;

        public InsertarPreguntaPopup()
        {
            InitializeComponent();

        }



        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            string descripcion = descripcionEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar una descripción.", "OK");
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
