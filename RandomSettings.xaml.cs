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
using System.Text.RegularExpressions;
using Matrix_Elementary.Scripts;

namespace Matrix_Elementary
{
    public enum RandomMethod
    {
        Normal,
        Symmetric,
        Skew
    }


    /// <summary>
    /// Interaction logic for RandomSettings.xaml
    /// </summary>
    public partial class RandomSettings : Window
    {
        RandomMethod method;
        bool focus = false;
        public RandomSettings(bool dark, bool show_options)
        {
            InitializeComponent();
            if (dark)
            {
                Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                normalChecker.Foreground = symmetricChecker.Foreground = skewChecker.Foreground = labelP.Foreground = label1.Foreground = label1.Foreground = label2.Foreground = label3.Foreground = Foreground = new SolidColorBrush(Colors.White);
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                Resources.Clear();
                Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_black.ico"));
            }
            if (!show_options)
            {
                stackOptions.Visibility = Visibility.Hidden;
                Height = 155;
            }
            minTextBox.Focus();
            minTextBox.SelectAll();
        }

        private static readonly Regex _regex = new Regex("[^0-9./-]+");
        private static bool IsTextAllowed(string text) => !_regex.IsMatch(text);
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !IsTextAllowed(e.Text);
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (minTextBox.Text.Length > 0 && maxTextBox.Text.Length > 0 &&stepTextBox.Text.Length > 0) DialogResult = true;
        }

        public Rational Min
        {
            get
            {
                try
                {
                    return new Rational(minTextBox.Text);
                } catch
                {
                    return -10;
                }
            }
        }

        public Rational Max
        {
            get
            {
                try
                {
                    return new Rational(maxTextBox.Text);
                }
                catch
                {
                    return 10;
                }
            }
        }

        public Rational Step
        {
            get
            {
                try
                {
                    return new Rational(stepTextBox.Text);
                }
                catch
                {
                    return 1;
                }
            }
        }
        public RandomMethod Method => method;

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
            if (focus)
            {
                focus = false;
                ((TextBox)sender).SelectAll();
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e) => focus = true;

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
            else if (e.Key == Key.Up && stackOptions.Visibility == Visibility.Visible)
            {
                if (method == RandomMethod.Skew)
                    symmetricChecker.IsChecked = true;
                else if (method == RandomMethod.Symmetric)
                    normalChecker.IsChecked = true;
            }
            else if (e.Key == Key.Down && stackOptions.Visibility == Visibility.Visible)
            {
                if (method == RandomMethod.Normal)
                    symmetricChecker.IsChecked = true;
                else if (method == RandomMethod.Symmetric)
                    skewChecker.IsChecked = true;
            }
        }

        private void minTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && (minTextBox.CaretIndex == minTextBox.Text.Length || minTextBox.SelectionLength == minTextBox.Text.Length))
                maxTextBox.Focus();
        }

        private void maxTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left && (maxTextBox.CaretIndex == 0 || maxTextBox.SelectionLength == maxTextBox.Text.Length))
                minTextBox.Focus();
            else if (e.Key == Key.Right && (maxTextBox.CaretIndex == maxTextBox.Text.Length || maxTextBox.SelectionLength == maxTextBox.Text.Length))
                stepTextBox.Focus();
        }

        private void stepTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left && (stepTextBox.CaretIndex == 0 || stepTextBox.SelectionLength == stepTextBox.Text.Length))
                maxTextBox.Focus();
        }

        private void normalChecker_Checked(object sender, RoutedEventArgs e) => method = RandomMethod.Normal;

        private void symmetricChecker_Checked(object sender, RoutedEventArgs e) => method = RandomMethod.Symmetric;

        private void skewChecker_Checked(object sender, RoutedEventArgs e) => method = RandomMethod.Skew;
    }
}
