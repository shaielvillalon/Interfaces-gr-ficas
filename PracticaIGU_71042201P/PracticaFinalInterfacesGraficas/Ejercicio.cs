using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaFinalInterfaces3
{
    public class Ejercicio : INotifyPropertyChanged
    {


        //public string Nombre { get; set; }
        //public string Descripcion { get; set; }
        //public string GruposMusculares { get; set; }
        private string _nombre, _descripcion, _grupo;

        public event PropertyChangedEventHandler PropertyChanged;


        public Ejercicio(string nombre, string descripcion, string grupo)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            GruposMusculares = grupo;
        }


        public Ejercicio()
        {
            Nombre = "Sin nombre";
            Descripcion = "Sin descripcion";
            GruposMusculares = "Sin grupo";
        }


        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }


        public string Descripcion
        {
            get => _descripcion;
            set
            {
                _descripcion = value;
                OnPropertyChanged(nameof(Descripcion));
            }
        }

        public string GruposMusculares
        {
            get => _grupo;
            set
            {
                _grupo = value;
                OnPropertyChanged(nameof(GruposMusculares));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }

}
