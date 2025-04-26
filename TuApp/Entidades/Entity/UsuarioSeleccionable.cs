using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TuApp.Entidades.Entity
{
    public class UsuarioSeleccionable : INotifyPropertyChanged
    {
        public Usuario Usuario { get; set; }

        private bool _seleccionado;
        public bool Seleccionado
        {
            get => _seleccionado;
            set
            {
                _seleccionado = value;
                OnPropertyChanged();
            }
        }

        private bool _puedeAsignar = true;
        public bool PuedeAsignar
        {
            get => _puedeAsignar;
            set
            {
                _puedeAsignar = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
