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

namespace Matrix_Elementary
{
    /// <summary>
    /// Interaction logic for StringDialog.xaml
    /// </summary>
    public partial class StringDialog : Window
    {
        public StringDialog(bool dark, string caption = "Input name", string start = "")
        {
            InitializeComponent();
            if (dark)
            {
                stringDialogWindow.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                label1.Foreground = stringDialogWindow.Foreground = new SolidColorBrush(Colors.White);
                stringDialogWindow.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                stringDialogWindow.Resources.Clear();
                stringDialogWindow.Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_black.ico"));
            }
            stringDialogWindow.Title = caption;
            textBox1.Text = start;
            label1.Content = caption + ":";
            textBox1.SelectAll();
            textBox1.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text.Length > 0) DialogResult = true;
        }

        public string Output => textBox1.Text;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult != true)
                DialogResult = false;
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }
    }
}
