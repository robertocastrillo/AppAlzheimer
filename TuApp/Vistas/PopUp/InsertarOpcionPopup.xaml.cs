using CommunityToolkit.Maui.Views;
using System;
using TuApp.Entidades;

namespace TuApp
{
    public partial class InsertarOpcionPopup : Popup
    {
        public InsertarOpcionPopup()
        {
            InitializeComponent();
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            string descripcion = descripcionEntry.Text?.Trim();
            bool condicion = condicionCheck.IsChecked;

            if (string.IsNullOrEmpty(descripcion))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar una descripción.", "OK");
                return;
            }

            Opcion nueva = new Opcion
            {
                Descripcion = descripcion,
                Condicion = condicion
            };

            Close(nueva);
        }
    }
}