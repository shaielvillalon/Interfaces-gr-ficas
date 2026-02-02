using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticaFinalInterfaces3
{
    public class Ejecucion : INotifyPropertyChanged
    {

        private int _repeticiones, _peso;
        DateTime _fechaYHora;

        public event PropertyChangedEventHandler PropertyChanged;

        //public int Repeticiones { get; set; }
        //public int Peso { get; set; }
        //public DateTime FechaYHora { get; set; }


        public Ejecucion(int reps, int peso, DateTime fecha_hora)
        {
            Repeticiones = reps;
            Peso = peso;
            FechaYHora = fecha_hora;
        }

        public Ejecucion()
        {
            Repeticiones = 0;
            Peso = 0;
            FechaYHora = DateTime.Now;

        }

        public Ejecucion(DateTime fecha_hora)
        {
            FechaYHora = fecha_hora;
        }
        

        public int Repeticiones
        {
            get => _repeticiones;
            set
            {
                _repeticiones = value;
                OnPropertyChanged(nameof(Repeticiones));
            }
        }


        public int Peso
        {
            get => _peso;
            set
            {
                _peso = value;
                OnPropertyChanged(nameof(Peso));
            }
        }

        public DateTime FechaYHora
        {
            get => _fechaYHora;
            set
            {
                _fechaYHora = value;
                OnPropertyChanged(nameof(FechaYHora));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }




    }
}
