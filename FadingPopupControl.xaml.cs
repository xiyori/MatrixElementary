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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace Matrix_Elementary
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FadingPopupControl : UserControl
    {
        Window ParentWindow { get; set; }
        bool duration_changed = false;
        new public Brush Foreground
        {
            get => popupLabel.Foreground;
            set => popupLabel.Foreground = value;
        }
        new public Brush Background
        {
            get => popupLabel.Background;
            set => popupLabel.Background = value;
        }
        public FadingPopupControl()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        public void ShowDialogBox(Window parentWindow, string message)
        {
            ParentWindow = parentWindow;
            popupLabel.Content = message;
            Storyboard StatusFader = (Storyboard)Resources["StatusFader"];
            //ParentWindow.IsEnabled = false;
            FrameworkElement root = (FrameworkElement)ParentWindow.Content;
            this.Height = root.ActualHeight;
            this.Width = root.ActualWidth;
            //TODO: Determine why there is 1 pixel extra whitespace.
            //Tried playing with Margins and Alignment to no avail.
            popup.Height = root.ActualHeight + 1;
            popup.Width = root.ActualWidth + 1;
            popup.IsOpen = true;
            StatusFader.Begin(popupLabel);
        }

        public void ShowDialogBox(Window parentWindow, string message, double duration)
        {
            ((Storyboard)Resources["StatusFader"]).Children[0].Duration = new Duration(new TimeSpan(0, 0, 0, (int)duration, (int)(duration * 1000) % 1000));
            duration_changed = true;
            ShowDialogBox(parentWindow, message);
        }

        public void Darken()
        {
            Foreground = Brushes.White;
            Background = new SolidColorBrush(Color.FromArgb(238, 45, 45, 45));
        }

        public void Lighten()
        {
            Foreground = Brushes.Black;
            Background = new SolidColorBrush(Color.FromArgb(238, 245, 245, 245));
        }

        void StatusFader_Completed(object sender, EventArgs e)
        { 
            popup.IsOpen = false;
            if (duration_changed)
            {
                ((Storyboard)Resources["StatusFader"]).Children[0].Duration = new Duration(new TimeSpan(0, 0, 0, 1, 300));
                duration_changed = false;
            }
            //ParentWindow.IsEnabled = true;
            //ParentWindow.Focus();
        }
    }
}
