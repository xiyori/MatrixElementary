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

namespace Matrix_Elementary
{
    /// <summary>
    /// Interaction logic for Dialogue.xaml
    /// </summary>
    public partial class Dialogue : Window
    {
        bool focus = false;

        public Dialogue(bool dark, int N = 3, int M = 3)
        {
            InitializeComponent();
            if (dark)
            {
                dimWindow.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                label1.Foreground = label2.Foreground = dimWindow.Foreground = new SolidColorBrush(Colors.White);
                dimWindow.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                dimWindow.Resources.Clear();
                dimWindow.Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_black.ico"));
            }
            YDimTextBox.Text = N.ToString();
            XDimTextBox.Text = M.ToString();
            YDimTextBox.Focus();
            YDimTextBox.SelectAll();
            focus = false;
        }

        private static readonly Regex _regex = new Regex("[^0-9]+");
        private static bool IsTextAllowed(string text) => !_regex.IsMatch(text);
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public int[] Dimensions
        {
            get
            {
                int[] dim = new int[2];
                try
                {
                    dim[0] = Convert.ToInt32(YDimTextBox.Text);
                }
                catch
                {
                    dim[0] = 1;
                }
                try
                {
                    dim[1] = Convert.ToInt32(XDimTextBox.Text);
                }
                catch
                {
                    dim[1] = 1;
                }
                return dim;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult != true)
                DialogResult = false;
        }

        private void TextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (focus)
            {
                focus = false;
                ((TextBox)sender).SelectAll();
            }
        }

        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (focus && (e.Key == Key.Right || e.Key == Key.Left))
            {
                focus = false;
                ((TextBox)sender).SelectAll();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            focus = true;
        }

        private void dimWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        private void YDimTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && (YDimTextBox.CaretIndex == YDimTextBox.Text.Length || YDimTextBox.SelectionLength == YDimTextBox.Text.Length))
                XDimTextBox.Focus();
        }

        private void XDimTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left && (XDimTextBox.CaretIndex == 0 || XDimTextBox.SelectionLength == XDimTextBox.Text.Length))
                YDimTextBox.Focus();
        }
    }
}
