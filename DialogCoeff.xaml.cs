using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Matrix_Elementary.Scripts;

namespace Matrix_Elementary
{
    /// <summary>
    /// Interaction logic for DialogCoeff.xaml
    /// </summary>
    public partial class DialogCoeff : Window
    {
        int start_coeff;
        public DialogCoeff(bool dark, string caption = "Set coefficient", int start_coeff = 1)
        {
            InitializeComponent();
            if (dark)
            {
                coeffWindow.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                label1.Foreground = coeffWindow.Foreground = new SolidColorBrush(Colors.White);
                coeffWindow.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                coeffWindow.Resources.Clear();
                coeffWindow.Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_black.ico"));
            }
            this.start_coeff = start_coeff;
            coeffWindow.Title = caption;
            CoeffTextBox.Text = start_coeff.ToString();
            label1.Content = caption + ":";
            CoeffTextBox.SelectAll();
            CoeffTextBox.Focus();
        }

        private static readonly Regex _regex = new Regex("[^0-9./-]+");
        private static bool IsTextAllowed(string text) => !_regex.IsMatch(text);
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public Rational Coefficient
        {
            get
            {
                try { return new Rational(CoeffTextBox.Text); }
                catch { return start_coeff; }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult != true)
                DialogResult = false;
        }

        private void coeffWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }
    }
}
