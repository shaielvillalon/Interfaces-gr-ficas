using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace PracticaFinalInterfaces3
{
    /// <summary>
    /// Lógica de interacción para EliminarEjercicio.xaml
    /// </summary>
    public partial class EliminarEjercicio : Window
    {
        ObservableCollection<Ejercicio> EjerciciosPrincipales;
        ObservableCollection<Ejercicio> Ejercicios;
        ObservableCollection<Ejercicio> EjerciciosFinal = new ObservableCollection<Ejercicio>();
        ObservableCollection<Ejercicio> Ejecuciones;


        public string rutaEjercicios = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ejercicios", "ejercicios.xml");



        public EliminarEjercicio()
        {

            InitializeComponent();

            this.Topmost = true;

            Ejercicios = CargarEjerciciosDesdeArchivo();
            ListaEjerciciosEliminar.ItemsSource = Ejercicios;
            EjerciciosPrincipales = CargarEjerciciosPrincipalesDesdeArchivo();
        }


        private void ListaEjerciciosEliminar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BotonEliminarEjercicio.IsEnabled = true;
        }


        private ObservableCollection<Ejercicio> CargarEjerciciosPrincipalesDesdeArchivo()
        {
            return new ObservableCollection<Ejercicio>
        {
            new Ejercicio ("Plancha", "Un ejercicio isométrico para trabajar el core, especialmente los abdominales", "Core"),
            new Ejercicio ("Curl de Bíceps", "Un ejercicio simple pero efectivo para desarrollar los brazos, especialmente los bíceps", "Brazos" ),
            new Ejercicio ("Press de banca", "Este ejercicio se realiza en una máquina guiada y permite trabajar los músculos del pecho con mayor control.",  "Pecho" ),
            new Ejercicio ("Jalón al pecho", "Un ejercicio en máquina para trabajar la espalda, especialmente el dorsal ancho",  "Espalda" ),
            new Ejercicio ("Prensa de pierna", "Una máquina guiada para trabajar los músculos de las piernas, especialmente los cuádriceps.",  "Piernas" ),
            new Ejercicio ("Extensión de pierna", "Este ejercicio se enfoca en el desarrollo de los cuádriceps mediante una máquina guiada",  "Piernas" ),
            new Ejercicio ("Press de hombros", "Un ejercicio para trabajar los hombros utilizando una máquina guiada.",  "Brazos" )
        };

        }


        private ObservableCollection<Ejercicio> CargarEjerciciosDesdeArchivo()
        {
            if (File.Exists(rutaEjercicios))
            {
                using (FileStream fs = new FileStream(rutaEjercicios, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Ejercicio>));
                    var ejercicios = (ObservableCollection<Ejercicio>)serializer.Deserialize(fs);

                    return new ObservableCollection<Ejercicio>(ejercicios.Skip(7));
                }
            }
            else
            {
                return null;
            }

        }

        private void BotonEliminarEjercicio_Click(object sender, RoutedEventArgs e)
        {
            if (ListaEjerciciosEliminar.SelectedItem is Ejercicio ejercicioSeleccionado)
            {
                var result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta ejecución?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {

                    string rutaEjecuciones = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ejecuciones", $"{ejercicioSeleccionado.Nombre}.xml");

                    if (File.Exists(rutaEjecuciones))
                    {
                        File.Delete(rutaEjecuciones);
                    }

                    Ejercicios.Remove(ejercicioSeleccionado);

                    if (EjerciciosPrincipales != null)
                    {
                        foreach (var ejercicio in EjerciciosPrincipales)
                        {
                            EjerciciosFinal.Add(ejercicio);
                        }
                    }

                    if (Ejercicios != null)
                    {
                        foreach (var ejercicio in Ejercicios)
                        {
                            EjerciciosFinal.Add(ejercicio);
                        }
                    }

                    GuardarEjerciciosEnArchivo();

                    this.Close();

                }

                if (result == MessageBoxResult.No)
                {
                    ListaEjerciciosEliminar.SelectedItem = null;
                    BotonEliminarEjercicio.IsEnabled = false;
                }

            }
            else
            {
                MessageBox.Show("Por favor, selecciona una ejecución antes de eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void GuardarEjerciciosEnArchivo()
        {
            string carpeta = System.IO.Path.GetDirectoryName(rutaEjercicios);
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            using (FileStream fs = new FileStream(rutaEjercicios, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Ejercicio>));
                serializer.Serialize(fs, EjerciciosFinal);
            }
        }
    }
}
