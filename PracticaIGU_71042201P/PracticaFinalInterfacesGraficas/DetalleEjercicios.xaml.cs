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
    /// Lógica de interacción para DetalleEjercicios.xaml
    /// </summary>
    /// 

    public class CambioSeleccionEjecucionEventArgs : EventArgs
    {
        public Ejecucion LaEjecucion { get; set; }
    }


    public partial class DetalleEjercicios : Window
    {


        public event EventHandler<CambioSeleccionEjecucionEventArgs> SeleccionarEjecucionListaEvent;
        
        ObservableCollection<Ejecucion> Ejecuciones;

        private Ejecucion _ejecucion;

        string rutaArchivo;

        private int lastSelectedIndex = -1;


        public DetalleEjercicios(Ejercicio ejercicio)
        {
            InitializeComponent();


            rutaArchivo = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ejecuciones", $"{ejercicio.Nombre}.xml");

            panel.SizeChanged += (s, e) => dibuja(panel);

        }


        private ObservableCollection<Ejecucion> CargarEjerciciosArchivo()
        {
            if (File.Exists(rutaArchivo))
            {
                using (FileStream fs = new FileStream(rutaArchivo, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Ejecucion>));
                    return (ObservableCollection<Ejecucion>)serializer.Deserialize(fs);
                }
            }
            else
            {
                // Retornar ejercicios predeterminados si el archivo no existe
                return new ObservableCollection<Ejecucion>
                { };

            }
        }

     
        private void GuardarEjerciciosArchivo()
        {

            string carpeta = System.IO.Path.GetDirectoryName(rutaArchivo);
            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            using (FileStream fs = new FileStream(rutaArchivo, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Ejecucion>));
                serializer.Serialize(fs, Ejecuciones);
            }
        }


        public void MostrarEjercicio(Ejercicio ejercicio)
        {

            this.Title = $"Detalles del ejercicio: {ejercicio.Nombre}";

            rutaArchivo = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Ejecuciones", $"{ejercicio.Nombre}.xml");

            Ejecuciones = CargarEjerciciosArchivo();
            ListaEjecuciones.ItemsSource = Ejecuciones;
            
            dibuja(panel);

        }



        private void BotonEliminarEjecucion_Click(object sender, RoutedEventArgs e)
        {

            Ejercicio ejercicio = e.Source as Ejercicio;

            if (ListaEjecuciones.SelectedItem is Ejecucion ejecucionSeleccionada)
            {
                var result = MessageBox.Show("¿Estás seguro de que deseas eliminar esta ejecución?", "Confirmación", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    Ejecuciones.Remove(ejecucionSeleccionada);
                    GuardarEjerciciosArchivo();

                    CambioSeleccionEjecucionEventArgs args = new CambioSeleccionEjecucionEventArgs { LaEjecucion = ejecucionSeleccionada };
                    OnSeleccionarEjercicioLista(args);

                }

                if (result == MessageBoxResult.No)
                {
                    ListaEjecuciones.SelectedItem = null;
                    BotonEliminarEjecucion.IsEnabled = false;
                }

            }
            else
            {
                MessageBox.Show("Por favor, selecciona una ejecución antes de eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


        private void ListaEjecuciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
                if (ListaEjecuciones.SelectedIndex >= 0)
                {
                    BotonEliminarEjecucion.IsEnabled = true;
                }
                else
                {
                    BotonEliminarEjecucion.IsEnabled = false;
                }

            if (ListaEjecuciones.SelectedItem is Ejecucion ejecucionSeleccionada)
            {
                CambioSeleccionEjecucionEventArgs args = new CambioSeleccionEjecucionEventArgs { LaEjecucion = ejecucionSeleccionada };
                OnSeleccionarEjercicioLista(args);
            }

        }


        protected virtual void OnSeleccionarEjercicioLista(CambioSeleccionEjecucionEventArgs e)
        {
            SeleccionarEjecucionListaEvent?.Invoke(this, e);
        }


        private void BotonAñadirEjecucion_Click(object sender, RoutedEventArgs e)
        {
            AddEjecucion newejecucion = new AddEjecucion();
            newejecucion.ShowDialog();

            if (newejecucion.DialogResult == true)
            {
                Ejecucion nuevaEjecucion = new Ejecucion(newejecucion.AddEjec.FechaYHora);

                Ejecuciones.Add(newejecucion.AddEjec);

                GuardarEjerciciosArchivo();


                CambioSeleccionEjecucionEventArgs args = new CambioSeleccionEjecucionEventArgs { LaEjecucion = nuevaEjecucion };
                OnSeleccionarEjercicioLista(args);

            }
        }



        private void panel_Loaded(object sender, RoutedEventArgs e)
        {
            Ejercicio ejercicio = e.Source as Ejercicio;

            if (tabControl.SelectedIndex == 1)
            {
                dibuja(panel);
            }
        }


        void dibuja(Canvas panel)
        {
            panel.Children.Clear();


            // Crear el TextBlock
            TextBlock etiquetaReps = new TextBlock
            {
                Text = "Reps",
                FontSize = 10,
                Foreground = Brushes.Red,
                Margin = new Thickness(3)
            };

            Canvas.SetLeft(etiquetaReps, 0);
            Canvas.SetTop(etiquetaReps, 0);

            panel.Children.Add(etiquetaReps);


            var existingPeso = panel.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Text == "Peso");
            if (existingPeso != null)
                return;

            // Crear el TextBlock
            TextBlock pesoTextBlock = new TextBlock
            {
                Text = "Peso",
                FontSize = 10,
                Foreground = Brushes.Blue,
                Margin = new Thickness(3)
            };

            double widthcanvas = panel.ActualWidth;

            Canvas.SetLeft(pesoTextBlock, widthcanvas - pesoTextBlock.ActualWidth - 35);
            Canvas.SetTop(pesoTextBlock, 0);

            panel.Children.Add(pesoTextBlock);




            double maxReps;
            double maxPeso;

            if (Ejecuciones.Count == 0)
            {
                maxReps = 100;
                maxPeso = 100;

                double canvasWidth = panel.ActualWidth;
                double canvasHeight = panel.ActualHeight;
                DibujarEjes(canvasWidth, canvasHeight, maxReps, maxPeso);

            }
            else
            {
                maxReps = Ejecuciones.Max(e => e.Repeticiones);
                maxPeso = Ejecuciones.Max(e => e.Peso);

                List<Point> puntos = new List<Point>();

                double ypantmin = 0;
                double ypantmax = panel.ActualHeight;
                double yrealmin = -maxReps / 11;
                double yrealmax = maxReps + maxReps / 11;

                double ypantminPESO = 0;
                double ypantmaxPESO = panel.ActualHeight;
                double yrealminPESO = -maxPeso / 11;
                double yrealmaxPESO = maxPeso + maxPeso / 11;

                double canvasWidth = panel.ActualWidth;
                double canvasHeight = panel.ActualHeight;

                var ejecucionesAgrupadas = Ejecuciones.GroupBy(e => e.FechaYHora.Date).OrderBy(g => g.Key);

                int totalDias = ejecucionesAgrupadas.Count();
                int totalBarras = Ejecuciones.Count();

                double espacioPorDia = 100;
                double anchoRequerido = totalDias * espacioPorDia;

                double anchoDisponibleCanvas = canvasWidth * 0.9;

                double escalaX = anchoDisponibleCanvas / anchoRequerido;

                escalaX = Math.Min(escalaX, 1);

                DibujarEjes(canvasWidth, canvasHeight, maxReps, maxPeso);

                double offsetX = 30;
                double barraSeparacion = 5 * escalaX;
                double barraAnchoMax = 20 * escalaX;

                foreach (var grupo in ejecucionesAgrupadas)
                {
                    int totalEjecuciones = grupo.Count();

                    double espacioGrupo = (espacioPorDia * escalaX);
                    double anchoDisponible = espacioGrupo - (barraSeparacion * (totalEjecuciones - 1));

                    double barraAncho = anchoDisponible / totalEjecuciones;
                    barraAncho = Math.Min(barraAncho, barraAnchoMax);

                    double inicioGrupo = offsetX + (espacioGrupo - (barraAncho * totalEjecuciones + barraSeparacion * (totalEjecuciones - 1))) / 2;

                    int numeroEjecucion = 0;
                    foreach (var ejecucion in grupo)
                    {
                        double yBase = (ypantmin - ypantmax) * ((0 - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                        double yTop = (ypantmin - ypantmax) * ((ejecucion.Repeticiones - yrealmin) / (yrealmax - yrealmin)) + ypantmax;
                        double barraAltura = yBase - yTop;

                        Rectangle bar = new Rectangle
                        {
                            Width = barraAncho,
                            Height = barraAltura,
                            Fill = Brushes.Red
                        };

                        double posicionX = inicioGrupo + numeroEjecucion * (barraAncho + barraSeparacion);
                        Canvas.SetLeft(bar, posicionX);
                        Canvas.SetTop(bar, yTop);

                        panel.Children.Add(bar);


                        double posicionY = (ypantminPESO - ypantmaxPESO) * ((ejecucion.Peso - yrealminPESO) / (yrealmaxPESO - yrealminPESO)) + ypantmaxPESO;

                        puntos.Add(new Point(((posicionX - 2.5) + barraAncho / 2) + 2.5, posicionY + 2.5));

                        Ellipse punto = new Ellipse
                        {
                            Width = 5,
                            Height = 5,
                            Fill = Brushes.Blue,
                            Tag = new { Fecha = ejecucion.FechaYHora, Peso = ejecucion.Peso, Repeticiones = ejecucion.Repeticiones }
                        };

                        Canvas.SetLeft(punto, (posicionX - 2.5) + barraAncho / 2);
                        Canvas.SetTop(punto, posicionY);

                        punto.MouseLeftButtonDown += Punto_MouseLeftButtonDown; ;

                        panel.Children.Add(punto);

                        numeroEjecucion++;
                    }

                    string fechaTexto = grupo.Key.ToString("dd/MM/yyyy");
                    TextBlock textBlock = new TextBlock
                    {
                        Text = fechaTexto,
                        FontSize = 10,
                        Foreground = Brushes.Black
                    };

                    double centroFecha = offsetX + (espacioGrupo / 2) - (fechaTexto.Length * 3);
                    Canvas.SetLeft(textBlock, centroFecha);
                    Canvas.SetTop(textBlock, canvasHeight - 20);

                    panel.Children.Add(textBlock);

                    offsetX += espacioGrupo;
                }



                if (puntos.Count > 1)
                {
                    Polyline linea = new Polyline
                    {
                        Stroke = Brushes.Blue,
                        StrokeThickness = 2,
                        StrokeDashArray = new DoubleCollection { 2, 2 }
                    };

                    // Agregar los puntos al Polyline
                    foreach (var punto in puntos)
                    {
                        linea.Points.Add(punto);
                    }

                    // Agregar la línea al panel
                    panel.Children.Add(linea);
                }





                if (anchoRequerido > anchoDisponibleCanvas)
                {
                }
                else
                {
                    panel.Width = canvasWidth;
                }

            }
        }


        private void DibujarEjes(double canvasWidth, double canvasHeight, double maxReps, double maxPeso)
        {

            double ypantmin = 0;
            double ypantmax = canvasHeight;

            double yrealmin = -maxReps / 11 * 3 / 2;
            double yrealmax = maxReps + maxReps / 11;

            for (int i = 0; i <= 10; i++)
            {
                double yrealValue = maxReps * i / 10;
                double ypant = (ypantmin - ypantmax) * ((yrealValue - yrealmin) / (yrealmax - yrealmin)) + ypantmax;

                TextBlock etiquetaReps = new TextBlock
                {
                    Text = yrealValue.ToString("0"),
                    FontSize = 10,
                    Foreground = Brushes.Red
                };
                Canvas.SetLeft(etiquetaReps, 5);
                Canvas.SetTop(etiquetaReps, ypant);
                panel.Children.Add(etiquetaReps);

            }

            double ypantmin2 = 0;
            double ypantmax2 = canvasHeight;

            double yrealmin2 = -maxPeso / 11 * 3 / 2;
            double yrealmax2 = maxPeso + maxPeso / 11;

            for (int i = 0; i <= 10; i++)
            {
                double yrealValue = maxPeso * i / 10;
                double ypant = (ypantmin2 - ypantmax2) * ((yrealValue - yrealmin2) / (yrealmax2 - yrealmin2)) + ypantmax2;

                TextBlock etiquetaPeso = new TextBlock
                {
                    Text = yrealValue.ToString("0"),
                    FontSize = 10,
                    Foreground = Brushes.Blue
                };
                Canvas.SetLeft(etiquetaPeso, canvasWidth - 30);
                Canvas.SetTop(etiquetaPeso, ypant);
                panel.Children.Add(etiquetaPeso);
            }

        }


        private void Punto_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Ellipse dot && dot.Tag != null)
            {
                var info = (dynamic)dot.Tag;
                DateTime fecha = info.Fecha;
                int peso = info.Peso;
                int repeticiones = info.Repeticiones;

                MessageBox.Show($"Fecha: {fecha}\nPeso: {peso} kg\nRepeticiones: {repeticiones}",
                                "Información del Punto",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RedibujarGrafica();
        }



        private void RedibujarGrafica()
        {
            if (panel == null) return;

            panel.Children.Clear(); // Limpia el canvas antes de volver a dibujar

            // Aquí debes volver a dibujar la gráfica con los datos existentes
            dibuja(panel);
        }


    }
}
