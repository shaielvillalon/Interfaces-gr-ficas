using System;
using System.Collections.Generic;
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

namespace PracticaFinalInterfaces3
{
    /// <summary>
    /// Lógica de interacción para AddEjecucion.xaml
    /// </summary>
    public partial class AddEjecucion : Window
    {

        Ejecucion nuevaejecucion;
        public Ejecucion AddEjec { get { return nuevaejecucion; } }

        public AddEjecucion()
        {
            InitializeComponent();

            this.Topmost = true;

            repeticiones.Focus();
        }


        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e) //CHATGPT
        {
            if (e.Key == Key.Down)
            {
                var request = new TraversalRequest(FocusNavigationDirection.Next);
                var focusedElement = Keyboard.FocusedElement as UIElement;

                if (focusedElement != null)
                {
                    focusedElement.MoveFocus(request);
                    e.Handled = true;
                }
            }

        }


        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e) //CHATGPT
        {
            if (e.Key == Key.Up)
            {
                var request = new TraversalRequest(FocusNavigationDirection.Previous);
                var focusedElement = Keyboard.FocusedElement as UIElement;

                if (focusedElement != null)
                {
                    focusedElement.MoveFocus(request);
                    e.Handled = true;
                }
            }

        }



        private void añadirEjecucion_Click(object sender, RoutedEventArgs e)
        {


            if (Validar_TextBox() == true)
            {
                int repeticionesInt;
                int.TryParse(repeticiones.Text, out repeticionesInt);

                int pesoInt;
                int.TryParse(peso.Text, out pesoInt);


                DateTime fechaSeleccionada = fecha.SelectedDate.Value;
                string horaTexto = hora.Text.Trim();

                if (TimeSpan.TryParse(horaTexto, out TimeSpan horaSeleccionada))
                {
                    DateTime fechaHoraFinal = fechaSeleccionada.Date + horaSeleccionada;


                    nuevaejecucion = new Ejecucion(repeticionesInt, pesoInt, fechaHoraFinal);
                    DialogResult = true;
                }


            }
        }

        private bool Validar_TextBox()
        {
            bool check = true;

            errorRepeticiones.Visibility = Visibility.Hidden;
            errorPeso.Visibility = Visibility.Hidden;
            errorFecha.Visibility = Visibility.Hidden;
            errorHora.Visibility = Visibility.Hidden;

            repeticiones.BorderBrush = Brushes.Gray;
            peso.BorderBrush = Brushes.Gray;
            fecha.BorderBrush = Brushes.Gray;
            hora.BorderBrush = Brushes.Gray;

            if (!String.IsNullOrEmpty(repeticiones.Text) && int.TryParse(repeticiones.Text, out int repeticionesValor) && repeticionesValor >= 1) { }
            else
            {
                repeticiones.BorderBrush = Brushes.Red;
                errorRepeticiones.Visibility = Visibility.Visible;
                check = false;
            }

            if (!String.IsNullOrEmpty(peso.Text) && int.TryParse(peso.Text, out int pesoValor) && pesoValor >= 0) { }
            else
            {
                peso.BorderBrush = Brushes.Red;
                errorPeso.Visibility = Visibility.Visible;
                check = false;
            }
            if (fecha.SelectedDate != null && fecha.SelectedDate <= DateTime.Now.Date) { }
            else
            {
                fecha.BorderBrush = Brushes.Red;
                errorFecha.Visibility = Visibility.Visible;
                check = false;
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(hora.Text, @"^(?:[01]\d|2[0-3]):[0-5]\d:[0-5]\d$")) { }
            else
            {
                // La hora no es válida, muestra un mensaje o toma otra acción
                hora.BorderBrush = Brushes.Red;
                errorHora.Visibility = Visibility.Visible;
                check = false;
                MessageBox.Show("Por favor, ingrese la hora en el formato hh:mm:ss (ej., 13:33:33).",
                    "Formato incorrecto", MessageBoxButton.OK, MessageBoxImage.Warning);
            }



            return check;
        }
    }
}
