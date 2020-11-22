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
    /// Interaction logic for LaTeX_Export.xaml
    /// </summary>
    public partial class LaTeX_ExportWindow : Window
    {
        bool dark_mode = false;
        public LaTeX_ExportWindow(bool dark, string text)
        {
            InitializeComponent();
            if (dark) ChangeTheme();
            Clipboard.SetText(text);
            textBox1.Text = text;
            textBox1.Focus();
        }

        public void ChangeTheme()
        {
            if (!dark_mode)
            {
                latexWindow.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                latexWindow.Foreground = Brushes.White;
                latexWindow.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                latexWindow.Resources.Clear();
                latexWindow.Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_black.ico"));
                popup1.Darken();
                dark_mode = true;
            } else
            {
                latexWindow.Background = Brushes.White;
                latexWindow.Foreground = Brushes.Black;
                latexWindow.BorderBrush = null;
                latexWindow.Resources.Add(typeof(TextBox), new Style() { TargetType = typeof(TextBox) });
                latexWindow.Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_white.ico"));
                popup1.Lighten();
                dark_mode = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            popup1.ShowDialogBox(this, "Text copied!");
            textBox1.SelectAll();
        }

        private void latexWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
}
