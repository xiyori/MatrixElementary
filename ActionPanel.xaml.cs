using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Matrix_Elementary.Scripts;

namespace Matrix_Elementary
{
    public enum Action
    {
        New,
        Transpose,
        Inverse,
        Gauss,
        Im,
        Ker,
        NewBasis,
        Multiply,
        Sum,
        Fill,
        Projector,
        Diagonalize,
        Canonical
    }

    /// <summary>
    /// Interaction logic for ActionPanel.xaml
    /// </summary>
    public partial class ActionPanel : Window
    {
        Action action;
        bool focus = false;

        public ActionPanel(bool dark, Rational det, int rank, string name)
        {
            InitializeComponent();
            if (dark)
            {
                actionWindow.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                nameLabel.Foreground = detLabel.Foreground = rkLabel.Foreground = actionWindow.Foreground = new SolidColorBrush(Colors.White);
                actionWindow.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                actionWindow.Resources.Clear();
                actionWindow.Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_black.ico"));
            }
            try { detLabel.Content += det.ToString(); }
            catch {
                detLabel.Content += "none";
                inverseButton.IsEnabled = false;
                basisButton.IsEnabled = false;
                diagButton.Content = "Ortogonal";
            }
            nameTextBox.Text = name;
            rkLabel.Content += rank.ToString();
        }

        public Action SelectedOption => action;
        public string MatrixName => nameTextBox.Text;

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult != true)
                DialogResult = false;
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.New;
            DialogResult = true;
        }

        private void transposeButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Transpose;
            DialogResult = true;
        }

        private void inverseButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Inverse;
            DialogResult = true;
        }

        private void gaussButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Gauss;
            DialogResult = true;
        }

        private void imButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Im;
            DialogResult = true;
        }

        private void kerButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Ker;
            DialogResult = true;
        }

        private void basisButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.NewBasis;
            DialogResult = true;
        }

        private void multButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Multiply;
            DialogResult = true;
        }

        private void sumButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Sum;
            DialogResult = true;
        }

        private void fillButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Fill;
            DialogResult = true;
        }

        private void projectButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Projector;
            DialogResult = true;
        }

        private void diagButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Diagonalize;
            DialogResult = true;
        }

        private void canonButton_Click(object sender, RoutedEventArgs e)
        {
            action = Action.Canonical;
            DialogResult = true;
        }

        private void actionWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void nameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            focus = true;
        }

        private void nameTextBox_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (focus)
            {
                focus = false;
                nameTextBox.SelectAll();
            }
        }
    }
}
