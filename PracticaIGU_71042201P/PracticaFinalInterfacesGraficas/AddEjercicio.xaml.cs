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
    /// Lógica de interacción para AddEjercicio.xaml
    /// </summary>
    public partial class AddEjercicio : Window
    {

        Ejercicio nuevoejercicio;
        public Ejercicio AddEj { get { return nuevoejercicio; } }


        public AddEjercicio()
        {
            InitializeComponent();

            this.Topmost = true;

            nombreEjercicio.Focus();
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
        private bool Validar_TextBox()
        {
            bool check = true;

            errorNombre.Visibility = Visibility.Hidden;
            errorDescripcion.Visibility = Visibility.Hidden;
            errorGrupo.Visibility = Visibility.Hidden;
            nombreEjercicio.BorderBrush = Brushes.Gray;
            descripcionEjercicio.BorderBrush = Brushes.Gray;
            grupoEjercicio.BorderBrush = Brushes.Gray;


            if (!String.IsNullOrEmpty(nombreEjercicio.Text) &&
                (System.Text.RegularExpressions.Regex.IsMatch(nombreEjercicio.Text, @"^[a-zA-Z\s]+$"))) { }
            else
            {
                nombreEjercicio.BorderBrush = Brushes.Red;
                errorNombre.Visibility = Visibility.Visible;
                check = false;
            }
            if (!String.IsNullOrEmpty(descripcionEjercicio.Text) &&
                (System.Text.RegularExpressions.Regex.IsMatch(descripcionEjercicio.Text, @"^[a-zA-Z\s]+$"))) { }
            else
            {
                descripcionEjercicio.BorderBrush = Brushes.Red;
                errorDescripcion.Visibility = Visibility.Visible;
                check = false;
            }
            if (!String.IsNullOrEmpty(grupoEjercicio.Text) && (grupoEjercicio.Text is string))
            {

                string[] gruposMuscularesCorrectos = { "Brazos", "Pecho", "Espalda", "Piernas", "Core" };

                string entradaUsuario = grupoEjercicio.Text;

                string[] gruposIngresados = entradaUsuario.Split(',').Select(g => g.Trim()).ToArray(); //CHATGPT

                bool todosValidos = gruposIngresados.All(grupo => gruposMuscularesCorrectos.Contains(grupo)); //CHATGPT

                if (!todosValidos)
                {
                    grupoEjercicio.BorderBrush = Brushes.Red;
                    errorGrupo.Visibility = Visibility.Visible;
                    check = false;
                    MessageBox.Show("El grupo muscular ingresado no es válido. Por favor, ingrese uno de los siguientes: Brazos, Pecho, Espalda, Piernas, Core. Recuerde la mayúscula");
                }
                else
                { }

            }
            else
            {
                grupoEjercicio.BorderBrush = Brushes.Red;
                errorGrupo.Visibility = Visibility.Visible;
                check = false;
            }


            return check;
        }

        private void añadirEjercicio_Click(object sender, RoutedEventArgs e)
        {
            if (Validar_TextBox() == true)
            {
                nuevoejercicio = new Ejercicio(nombreEjercicio.Text, descripcionEjercicio.Text, grupoEjercicio.Text);
                DialogResult = true;
            }
        }


    }
}
