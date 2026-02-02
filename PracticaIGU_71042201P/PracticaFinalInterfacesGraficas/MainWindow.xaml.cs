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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace PracticaFinalInterfaces3
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public string rutaEjercicios = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ejercicios", "ejercicios.xml");

        ObservableCollection<Ejercicio> Ejercicios;

        DetalleEjercicios detalleEjercicios;

        Ejecucion _ejecucion;

        private DateTime dateSelection;


        public MainWindow()
        {
            InitializeComponent();

            this.Topmost = true;

            Ejercicios = CargarEjerciciosDesdeArchivo();
            ListaEjercicios.ItemsSource = Ejercicios;

        }



        private ObservableCollection<Ejercicio> CargarEjerciciosDesdeArchivo()
        {
            if (File.Exists(rutaEjercicios))
            {
                using (FileStream fs = new FileStream(rutaEjercicios, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Ejercicio>));
                    return (ObservableCollection<Ejercicio>)serializer.Deserialize(fs);
                }
            }
            else
            {
                return new ObservableCollection<Ejercicio> {
                    new Ejercicio ("Plancha", "Un ejercicio isométrico para trabajar el core, especialmente los abdominales", "Core"),
                    new Ejercicio ("Curl de Bíceps", "Un ejercicio simple pero efectivo para desarrollar los brazos, especialmente los bíceps", "Brazos" ),
                    new Ejercicio ("Press de banca", "Este ejercicio se realiza en una máquina guiada y permite trabajar los músculos del pecho con mayor control.",  "Pecho" ),
                    new Ejercicio ("Jalón al pecho", "Un ejercicio en máquina para trabajar la espalda, especialmente el dorsal ancho",  "Espalda" ),
                    new Ejercicio ("Prensa de pierna", "Una máquina guiada para trabajar los músculos de las piernas, especialmente los cuádriceps.",  "Piernas" ),
                    new Ejercicio ("Extensión de pierna", "Este ejercicio se enfoca en el desarrollo de los cuádriceps mediante una máquina guiada",  "Piernas" ),
                    new Ejercicio ("Press de hombros", "Un ejercicio para trabajar los hombros utilizando una máquina guiada.",  "Brazos" )
                };
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
                serializer.Serialize(fs, Ejercicios);
            }
        }



        private void ListaEjercicios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (ListaEjercicios.SelectedItem is Ejercicio ejercicioSeleccionado)
            {
                _ejecucion = null;

                if (detalleEjercicios != null)
                {
                    detalleEjercicios.MostrarEjercicio(ejercicioSeleccionado);
                }
                else
                {
                    detalleEjercicios = new DetalleEjercicios(ejercicioSeleccionado);
                    detalleEjercicios.MostrarEjercicio(ejercicioSeleccionado);
                    detalleEjercicios.SeleccionarEjecucionListaEvent += DetalleEjercicios_SeleccionarEjecucionListaEvent1;
                    detalleEjercicios.Closed += (s, args) => detalleEjercicios = null;
                    detalleEjercicios.Owner = this;
                    detalleEjercicios.Show();
                }
            }

        }

        private void DetalleEjercicios_SeleccionarEjecucionListaEvent1(object sender, CambioSeleccionEjecucionEventArgs e)
        {
            if (e.LaEjecucion != null)
            {
                _ejecucion = e.LaEjecucion;
                ActualizarTextoEjercicio();
            }
            else {}
        }

        private void ActualizarTextoEjercicio()
        {
            if (_ejecucion == null)
            {                
                double centerX = GraphCanvas.ActualWidth / 2;
                double centerY = GraphCanvas.ActualHeight / 2;
                double radius = Math.Min(centerX, centerY) - 20;
                DrawAxes(centerX, centerY, radius);
                SetEjecucionSeleccionada(dateSelection);
            }
            else
            {
                SetEjecucionSeleccionada(_ejecucion.FechaYHora);
            }
        }

        public void SetEjecucionSeleccionada(DateTime date)
        {
            DateTime dateSelected = date.Date;
            dateSelection = dateSelected;

            if (date == DateTime.MinValue)
            {
                TextoEjercicio.Text =  $"Fecha seleccionada:  ";
            }
            else
            {
                if (dateSelection == DateTime.Now.Date)
                {
                    NextDayButton.IsEnabled = false;
                }
                else
                {
                    NextDayButton.IsEnabled = true;
                }

                PreviousDayButton.IsEnabled = true;
                TextoEjercicio.Text = $"Fecha: {date:dd/MM/yyyy}";
                //SelectedDateText.Text = $"Fecha seleccionada: {dateSelection.ToString("dd/MM/yyyy")}";

                string rutaEjecuciones;

                int ejecucionesCore = 0;
                int ejecucionesBrazos = 0;
                int ejecucionesPecho = 0;
                int ejecucionesEspalda = 0;
                int ejecucionesPiernas = 0;


                if (File.Exists(rutaEjercicios))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Ejercicio>));

                    using (FileStream fs = new FileStream(rutaEjercicios, FileMode.Open))
                    {
                        List<Ejercicio> ejercicios = (List<Ejercicio>)serializer.Deserialize(fs);

                        foreach (var ejercicio in ejercicios)
                        {
                            rutaEjecuciones = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ejecuciones", $"{ejercicio.Nombre}.xml");

                            if (File.Exists(rutaEjecuciones))
                            {
                                XmlSerializer serializer2 = new XmlSerializer(typeof(List<Ejecucion>));

                                using (FileStream fs2 = new FileStream(rutaEjecuciones, FileMode.Open))
                                {
                                    List<Ejecucion> ejecuciones = (List<Ejecucion>)serializer2.Deserialize(fs2);

                                    foreach (var ejecucion2 in ejecuciones)
                                    {

                                        if (ejecucion2.FechaYHora.Date == dateSelection)
                                        {
                                            if (ejercicio.GruposMusculares.Contains("Core"))
                                            {
                                                ejecucionesCore = ejecucionesCore + ejecucion2.Repeticiones;
                                            }

                                            if (ejercicio.GruposMusculares.Contains("Brazos"))
                                            {
                                                ejecucionesBrazos = ejecucionesBrazos + ejecucion2.Repeticiones;
                                            }

                                            if (ejercicio.GruposMusculares.Contains("Pecho"))
                                            {
                                                ejecucionesPecho = ejecucionesPecho + ejecucion2.Repeticiones;
                                            }

                                            if (ejercicio.GruposMusculares.Contains("Espalda"))
                                            {
                                                ejecucionesEspalda = ejecucionesEspalda + ejecucion2.Repeticiones;
                                            }

                                            if (ejercicio.GruposMusculares.Contains("Piernas"))
                                            {
                                                ejecucionesPiernas = ejecucionesPiernas + ejecucion2.Repeticiones;
                                            }

                                        }

                                    }
                                }
                            }

                        }
                    }
                }

                dibujarDailyInsight(ejecucionesBrazos, ejecucionesCore, ejecucionesEspalda, ejecucionesPecho, ejecucionesPiernas);

            }

        }


        private void dibujarDailyInsight(int brazos, int core, int espalda, int pecho, int piernas)
        {
            GraphCanvas.Children.Clear();

            double centerX = GraphCanvas.ActualWidth / 2;
            double centerY = GraphCanvas.ActualHeight / 2;
            double radius = Math.Min(centerX, centerY) - 20;

            DrawAxes(centerX, centerY, radius);

            List<Point> points = new List<Point>();

            int[] valores = { pecho, core, espalda, piernas, brazos };

            for (int i = 0; i < 5; i++)
            {
                double angle = (Math.PI / 2) + (2 * Math.PI / 5) * i;

                double porcentaje = Math.Min(valores[i], 100) / 100.0;
                double x = centerX + (radius * porcentaje) * Math.Cos(angle);
                double y = centerY - (radius * porcentaje) * Math.Sin(angle);

                points.Add(new Point(x, y));
            }

            foreach (var point in points)
            {
                Ellipse dot = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = Brushes.Red,
                };
                Canvas.SetLeft(dot, point.X - 3);
                Canvas.SetTop(dot, point.Y - 3);
                GraphCanvas.Children.Add(dot);
            }

            var distinctPoints = points.Distinct().ToList();
            if (distinctPoints.Count == 2)
            {
                Point differentPoint = distinctPoints.First(p => points.Count(p2 => p2.Equals(p)) == 1);
                Point duplicatePoint = distinctPoints.First(p => points.Count(p2 => p2.Equals(p)) > 1);

                Line line = new Line
                {
                    X1 = differentPoint.X,
                    Y1 = differentPoint.Y,
                    X2 = duplicatePoint.X,
                    Y2 = duplicatePoint.Y,
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1
                };
                GraphCanvas.Children.Add(line);
            }
            else
            {
                Polygon polygon = new Polygon
                {
                    Stroke = Brushes.Blue,
                    StrokeThickness = 1,
                    Fill = Brushes.LightBlue,
                    Opacity = 0.6
                };
                polygon.Points = new PointCollection(points);

                GraphCanvas.Children.Add(polygon);
            }
        }


        private void DrawAxes(double centerX, double centerY, double radius)
        {

            string[] gruposMusculares = { "Pecho", "Core", "Espalda", "Piernas", "Brazos" };

            for (int i = 0; i < 5; i++)
            {
                double angle = (Math.PI / 2) + (2 * Math.PI / 5) * i;

                double x = centerX + radius * Math.Cos(angle);
                double y = centerY - radius * Math.Sin(angle);

                Line axis = new Line
                {
                    X1 = centerX,
                    Y1 = centerY,
                    X2 = x,
                    Y2 = y,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1
                };

                GraphCanvas.Children.Add(axis);


                if (i == 0)
                {
                    TextBlock textoPecho = new TextBlock
                    {
                        Text = gruposMusculares[i],
                        FontSize = 10,
                        Foreground = Brushes.Gray
                    };

                    Canvas.SetLeft(textoPecho, x - 13);
                    Canvas.SetTop(textoPecho, y - 15);

                    GraphCanvas.Children.Add(textoPecho);
                }

                if (i == 1)
                {
                    TextBlock textoCore = new TextBlock
                    {
                        Text = gruposMusculares[i],
                        FontSize = 10,
                        Foreground = Brushes.Gray
                    };

                    Canvas.SetLeft(textoCore, x - 30);
                    Canvas.SetTop(textoCore, y - 10);

                    GraphCanvas.Children.Add(textoCore);
                }

                if (i == 2)
                {
                    TextBlock textoEspaldas = new TextBlock
                    {
                        Text = gruposMusculares[i],
                        FontSize = 10,
                        Foreground = Brushes.Gray
                    };

                    Canvas.SetLeft(textoEspaldas, x - 25);
                    Canvas.SetTop(textoEspaldas, y + 5);

                    GraphCanvas.Children.Add(textoEspaldas);
                }

                if (i == 3)
                {
                    TextBlock textoPiernas = new TextBlock
                    {
                        Text = gruposMusculares[i],
                        FontSize = 10,
                        Foreground = Brushes.Gray
                    };

                    Canvas.SetLeft(textoPiernas, x - 10);
                    Canvas.SetTop(textoPiernas, y + 5);

                    GraphCanvas.Children.Add(textoPiernas);
                }

                if (i == 4)
                {
                    TextBlock textoBrazos = new TextBlock
                    {
                        Text = gruposMusculares[i],
                        FontSize = 10,
                        Foreground = Brushes.Gray
                    };

                    Canvas.SetLeft(textoBrazos, x + 10);
                    Canvas.SetTop(textoBrazos, y - 10);

                    GraphCanvas.Children.Add(textoBrazos);
                }



            }
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e) //CHAT GPT
        {
            GraphCanvas.Children.Clear();

            double centerX = GraphCanvas.ActualWidth / 2;
            double centerY = GraphCanvas.ActualHeight / 2;
            double radius = Math.Min(centerX, centerY) - 20;

            DrawAxes(centerX, centerY, radius);

            //SetEjecucionSeleccionada(dateSelection);
        }



        private void BotonAñadir_Click(object sender, RoutedEventArgs e)
        {
            AddEjercicio newejercicio = new AddEjercicio();
            newejercicio.ShowDialog();
            if (newejercicio.DialogResult == true)
            {
                Ejercicios.Add(newejercicio.AddEj);
                GuardarEjerciciosEnArchivo();
            }
        }

        private void BotonEliminarEjercicio_Click(object sender, RoutedEventArgs e)
        {
            EliminarEjercicio ejercicio = new EliminarEjercicio();
            ejercicio.ShowDialog();
            if (ejercicio.DialogResult == false)
            {
                Ejercicios = CargarEjerciciosDesdeArchivo();
                ListaEjercicios.ItemsSource = Ejercicios;
                if (detalleEjercicios != null)
                {
                    detalleEjercicios.Close();
                }                
            }            
        }

        private void PreviousDayButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime previousDay = dateSelection.AddDays(-1);
            SetEjecucionSeleccionada(previousDay);
        }

        private void NextDayButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime siguiente = dateSelection.AddDays(1);
            SetEjecucionSeleccionada(siguiente);
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime today = DateTime.Now.Date;
            SetEjecucionSeleccionada(today);

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedibujarGrafica();
        }

        
        private void RedibujarGrafica()
        {
            if (GraphCanvas == null) return;

            GraphCanvas.Children.Clear(); // Limpia el canvas antes de volver a dibujar

            // Aquí debes volver a dibujar la gráfica con los datos existentes
            ActualizarTextoEjercicio();
        }

        private void GraphCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            RedibujarGrafica();            
        }
    }
}
