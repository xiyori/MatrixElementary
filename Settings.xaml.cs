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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        bool dark_mode = false;
        public Settings(bool dark, bool easter)
        {
            InitializeComponent();
            if (dark) ChangeTheme();
            if (easter) easterButton.Visibility = Visibility.Visible;
        }

        public void ChangeTheme()
        {
            if (!dark_mode)
            {
                darkButton.Content = "Dark On";
                Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                Foreground = aboutLabel.Foreground = label1.Foreground = Brushes.White;
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                Resources.Clear();
                Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_black.ico"));
                dark_mode = true;
            }
            else
            {
                darkButton.Content = "Dark Off";
                Background = Brushes.White;
                Foreground = aboutLabel.Foreground = label1.Foreground = Brushes.Black;
                BorderBrush = null;
                Resources.Add(typeof(Button), new Style() { TargetType = typeof(Button) });
                Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_white.ico"));
                dark_mode = false;
            }
        }
    
        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void darkButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ChangeAppTheme();
            ChangeTheme();
        }

        private void easterButton_Click(object sender, RoutedEventArgs e)
        {
            easterButton.Visibility = Visibility.Hidden;
            MainWindow.Disney();
        }
    }
}
