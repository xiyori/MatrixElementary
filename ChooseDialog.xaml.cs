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
    /// Interaction logic for ChooseDialog.xaml
    /// </summary>
    public partial class ChooseDialog : Window
    {
        int variant_ind;
        public ChooseDialog(bool dark, string caption, List<string> variants)
        {
            InitializeComponent();
            if (dark)
            {
                Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                label1.Foreground = Foreground = new SolidColorBrush(Colors.White);
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                Resources.Clear();
                Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_black.ico"));
            }
            Title = caption;
            label1.Content = caption + ":";
            for (int i = 0; i < variants.Count; i++)
            {
                Button variant = new Button
                {
                    Width = 105,
                    Height = 25,
                    FontSize = 15,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Uid = i.ToString() + "_button",
                    Content = variants[i]
                };
                variant.Click += Variant_Click;
                Grid.SetRow(variant, i / 2);
                Grid.SetColumn(variant, i % 2);
                variantGrid.Children.Add(variant);
                if (i == 0) variant.Focus();
            }
        }

        public int Variant => variant_ind;

        private void Variant_Click(object sender, RoutedEventArgs e)
        {
            variant_ind = Convert.ToInt32(((Button)sender).Uid.Substring(0, ((Button)sender).Uid.IndexOf('_')));
            DialogResult = true;
        }

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
