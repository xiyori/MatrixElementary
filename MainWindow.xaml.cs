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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Matrix_Elementary.Scripts;

namespace Matrix_Elementary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MyMatrix matrix, start_copy;
        List<List<TextBox>> controls;
        List<List<Rectangle>> guides;
        List<List<bool>> controls_sync;
        int select_ind = 0;
        bool row_mode = true, selected = false, dark_mode = false, edit_mode = true, easter_found, owner_mode = false, awaiting_confirmation = false, show_dim_dialog = true;
        bool focus = false, text_change_handled = false, drag_mode = false, to_deselect = false;
        List<KeyValuePair<int[], Rational>> history;
        int story_ind, over_ind;
        List<LaTeX_ExportWindow> export_windows;
        string name;
        static Dictionary<string, MainWindow> instance_links;
        MainWindow owner, input_child;
        Action action_in_progress;
        double drag_x0, drag_y0;

        public MainWindow()
        {
            InitializeComponent();
            Commands.Init();
            export_windows = new List<LaTeX_ExportWindow>();
            name = "A";
            Title = "Matrix Elementary: " + name;
            instance_links = new Dictionary<string, MainWindow>();
            instance_links[name] = this;
            Cache.CheckFirstRun();
            bool[] cache = Cache.ReadCache();
            if (cache[0]) ChangeTheme();
            easter_found = cache[1];
            AnimeCheck();
        }

        public MainWindow(string name, bool dark, bool easter_found)
        {
            InitializeComponent();
            Commands.Init();
            export_windows = new List<LaTeX_ExportWindow>();
            this.name = name;
            Title = "Matrix Elementary: " + name;
            instance_links[name] = this;
            if (dark) ChangeTheme();
            this.easter_found = easter_found;
            AnimeCheck();
        }

        public MainWindow(string name, MainWindow owner, bool show_dim_dialog = true)
        {
            InitializeComponent();
            Commands.Init();
            export_windows = new List<LaTeX_ExportWindow>();
            this.name = name;
            Title = "Matrix Elementary: " + name;
            instance_links[name] = this;
            this.owner = owner;
            owner_mode = true;
            actionButton.Content = "Confirm";
            if (owner.dark_mode) ChangeTheme();
            easter_found = owner.easter_found;
            this.show_dim_dialog = show_dim_dialog;
            AnimeCheck();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (show_dim_dialog) dimButton_Click(new object(), new RoutedEventArgs());
            else Init(matrix.Dimensions);
        }

        public void Init(int[] dims)
        {
            //bool easter_mode = false;
            if (!easter_found && dims[0] == 92 && dims[1] == 92)
            {
                /*easter_mode = true;
                dims[0] = 15;
                dims[1] = 14;
                matrix = new MyMatrix(15, 14);
                Rational val = new Rational(92, 37);
                matrix = FromWolfram("{{0,92/3,92/3,92/3,0,92/3,92/3,92/3,0,92/3,0,0,92/3,0},{0,92/3,0,0,0,92/3,0,92/3,0,92/3,92/3,0,92/3,0},{0,92/3,0,0,0,92/3,0,92/3,0,92/3,0,92/3,92/3,0},{0,92/3,92/3,92/3,0,92/3,92/3,92/3,0,92/3,0,0,92/3,0},{0,0,0,0,0,0,0,0,0,0,0,0,0},{0,92/3,92/3,92/3,0,92/3,92/3,92,0,92,92/3,92,0,0},{0,92/3,0,0,0,92/3,0,92/3,0,92/3,0,92/3,0,0},{0,92/3,0,92/3,0,92/3,92/3,0,0,92/3,92/3,92/3,0,0},{0,92/3,92/3,92/3,0,92/3,0,92/3,0,92/3,0,92/3,0,0},{0,0,0,0,0,0,0,0,0,0,0,0,0},{0,92/3,92/3,92/3,0,92/3,92/3,92/3,0,0,92/3,0,0,0},{0,0,92/3,0,0,92/3,0,0,0,0,92/3,0,0,0},{0,0,92/3,0,0,0,92/3,0,0,0,92,0,0,0},{0,0,92/3,0,0,0,0,92/3,0,0,0,0,0},{0,0,92/3,0,0,92/3,92/3,92/3,0,0,92/3,0,0,0}}");*/
                if (matrix == null)
                    dims[0] = dims[1] = 3;
                else
                {
                    dims[0] = matrix.N;
                    dims[1] = matrix.M;
                }
                UnlockAnime();
            }
            if (dims[0] > 14 || dims[1] > 14)
            {
                if (dims[0] > 14) dims[0] = 14;
                if (dims[1] > 14) dims[1] = 14;
                popup1.ShowDialogBox(this, "Too large dimensions!");
            }
            if (matrix != null)
            {
                matrix.N = dims[0];
                matrix.M = dims[1];
            }
            else
                matrix = new MyMatrix(dims[0], dims[1]);
            Grid table = new Grid
            {
                Width = matrix.M * 105,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                ShowGridLines = false,
            };
            table.Uid = "matrix";
            for (int i = 0; i < matrix.M; i++)
                table.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < matrix.N; i++)
                table.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(50)
                });
            controls = new List<List<TextBox>>(matrix.N);
            guides = new List<List<Rectangle>>(matrix.N);
            controls_sync = new List<List<bool>>(matrix.N);
            for (int i = 0; i < matrix.N; i++)
            {
                controls.Add(new List<TextBox>(matrix.M));
                guides.Add(new List<Rectangle>(matrix.M));
                controls_sync.Add(new List<bool>(matrix.M));
                for (int j = 0; j < matrix.M; j++)
                {
                    TextBox box = new TextBox
                    {
                        Text = matrix[i, j].ToString(),
                        TextAlignment = TextAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        FontSize = 22,
                        Uid = $"{i},{j}_matrix_cell",
                        AcceptsReturn = true
                    };
                    /*if (easter_mode && matrix[i, j] == 0)
                    {
                        box.Text = "";
                        controls_sync[i].Add(false);
                    }
                    else*/
                    controls_sync[i].Add(true);
                    box.PreviewTextInput += Box_PreviewTextInput;
                    box.TextChanged += Box_TextChanged;
                    box.GotFocus += Box_GotFocus;
                    box.LostFocus += Box_LostFocus;
                    box.PreviewMouseUp += Box_MouseUp;
                    box.PreviewKeyDown += Box_KeyDown;
                    box.PreviewKeyUp += Box_KeyUp;
                    Grid.SetRow(box, i);
                    Grid.SetColumn(box, j);
                    Grid.SetZIndex(box, 1);
                    controls[i].Add(box);
                    table.Children.Add(box);

                    Rectangle rect = new Rectangle
                    {
                        Fill = Brushes.Transparent,
                        StrokeThickness = 0,
                        Visibility = Visibility.Hidden,
                        Uid = $"{i},{j}_matrix_guide"
                    };
                    rect.MouseDown += Rect_MouseDown;
                    rect.MouseUp += Rect_MouseUp;
                    Grid.SetRow(rect, i);
                    Grid.SetColumn(rect, j);
                    Grid.SetZIndex(rect, 2);
                    guides[i].Add(rect);
                    table.Children.Add(rect);
                }
            }
            if (105 * matrix.M > Width - 30 || 105 * matrix.M < Width - 30)
                Width = easter_found ? Math.Max(818, 105 * matrix.M + 30) :
                    105 * matrix.M + 30;
            if (50 * matrix.N > Height - 100 || 50 * matrix.N < Height - 100)
                Height = easter_found ? Math.Max(470, 50 * matrix.N + 100) : 50 * matrix.N + 100;
            if (MainGrid.Children[MainGrid.Children.Count - 1].Uid == "matrix")
                MainGrid.Children.RemoveAt(MainGrid.Children.Count - 1);
            MainGrid.Children.Add(table);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (selected && !drag_mode && (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed))
            {
                drag_mode = true;
                if (row_mode)
                {
                    for (int j = 0; j < matrix.M; j++)
                    {
                        guides[select_ind][j].Fill = new SolidColorBrush(Color.FromArgb(70, 255, 255, 0));
                        Grid.SetZIndex(controls[select_ind][j], 3);
                        Grid.SetZIndex(guides[select_ind][j], 4);
                    }
                } else
                {
                    for (int i = 0; i < matrix.N; i++)
                    {
                        guides[i][select_ind].Fill = new SolidColorBrush(Color.FromArgb(70, 255, 255, 0));
                        Grid.SetZIndex(controls[i][select_ind], 3);
                        Grid.SetZIndex(guides[i][select_ind], 4);
                    }
                }
            }
            if (drag_mode && row_mode)
            {
                double mouse_x_dif = e.GetPosition(this).X - drag_x0, mouse_y_dif = e.GetPosition(this).Y - drag_y0;
                over_ind = (int)((mouse_y_dif + Math.Sign(mouse_y_dif) * 25) / 50) + select_ind;
                if (over_ind < 0)
                {
                    mouse_y_dif = -select_ind * 50 - 25;
                    over_ind = 0;
                }
                else if (over_ind >= matrix.N)
                {
                    mouse_y_dif = (matrix.N - select_ind) * 50 - 25;
                    over_ind = matrix.N - 1;
                }
                if (e.LeftButton == MouseButtonState.Released)
                {
                    EndDrag();
                    return;
                }
                for (int j = 0; j < matrix.M; j++)
                {
                    guides[select_ind][j].Margin = new Thickness(mouse_x_dif, mouse_y_dif, -mouse_x_dif, -mouse_y_dif);
                    controls[select_ind][j].Margin = guides[select_ind][j].Margin;
                    for (int i = 0; i < matrix.N; i++)
                        if (i != select_ind)
                        {
                            if (i == over_ind)
                                controls[i][j].Margin = new Thickness(0, -(over_ind - select_ind) * 50, 0, (over_ind - select_ind) * 50);
                            else controls[i][j].Margin = new Thickness();
                        }
                }
            } else if (drag_mode && !row_mode)
            {
                double mouse_x_dif = e.GetPosition(this).X - drag_x0, mouse_y_dif = e.GetPosition(this).Y - drag_y0;
                over_ind = (int)((mouse_x_dif + Math.Sign(mouse_x_dif) * 52.5) / 105) + select_ind;
                if (over_ind < 0)
                {
                    mouse_x_dif = -select_ind * 105 - 52.5;
                    over_ind = 0;
                }
                else if (over_ind >= matrix.M)
                {
                    mouse_x_dif = (matrix.M - select_ind) * 105 - 52.5;
                    over_ind = matrix.M - 1;
                }
                if (e.RightButton == MouseButtonState.Released)
                {
                    EndDrag();
                    return;
                }
                for (int i = 0; i < matrix.N; i++)
                {
                    guides[i][select_ind].Margin = new Thickness(mouse_x_dif, mouse_y_dif, -mouse_x_dif, -mouse_y_dif);
                    controls[i][select_ind].Margin = guides[i][select_ind].Margin;
                    for (int j = 0; j < matrix.M; j++)
                        if (j != select_ind)
                        {
                            if (j == over_ind)
                                controls[i][j].Margin = new Thickness(-(over_ind - select_ind) * 105, 0, (over_ind - select_ind) * 105, 0);
                            else controls[i][j].Margin = new Thickness();
                        }
                }
            }
        }

        private void Rect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int[] coords = GetUidCoords(sender);
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                if (selected) Deselect();
                for (int j = 0; j < matrix.M; j++)
                    guides[coords[0]][j].Fill = new SolidColorBrush(Color.FromArgb(70, 255, 255, 0));
                var w = new DialogCoeff(dark_mode);
                w.Owner = this;
                w.ShowDialog();
                if (w.DialogResult == true)
                {
                    matrix.ElementaryProd(coords[0], w.Coefficient);
                    ApplyChanges();
                    AddEntry(coords[0], w.Coefficient);
                }
                for (int j = 0; j < matrix.M; j++)
                    guides[coords[0]][j].Fill = Brushes.Transparent;
            }
            else if (e.ChangedButton == MouseButton.Right && e.ClickCount == 2)
            {
                if (selected) Deselect();
                for (int i = 0; i < matrix.N; i++)
                    guides[i][coords[1]].Fill = new SolidColorBrush(Color.FromArgb(70, 255, 255, 0));
                var w = new DialogCoeff(dark_mode);
                w.Owner = this;
                w.ShowDialog();
                if (w.DialogResult == true)
                {
                    matrix.ElementaryProd(coords[1], w.Coefficient, false);
                    ApplyChanges();
                    AddEntry(coords[1], w.Coefficient, false);
                }
                for (int i = 0; i < matrix.N; i++)
                    guides[i][coords[1]].Fill = Brushes.Transparent;
            }
            else if (e.ChangedButton == MouseButton.Left && !selected)
            {
                for (int j = 0; j < matrix.M; j++)
                    guides[coords[0]][j].Fill = new SolidColorBrush(Color.FromArgb(70, 0, 230, 255));
                selected = true;
                select_ind = coords[0];
                row_mode = true;
                drag_x0 = e.GetPosition(this).X;
                drag_y0 = e.GetPosition(this).Y;
            }
            else if (e.ChangedButton == MouseButton.Right && !selected)
            {
                for (int i = 0; i < matrix.N; i++)
                    guides[i][coords[1]].Fill = new SolidColorBrush(Color.FromArgb(70, 0, 230, 255));
                selected = true;
                select_ind = coords[1];
                row_mode = false;
                drag_x0 = e.GetPosition(this).X;
                drag_y0 = e.GetPosition(this).Y;
            }
            else if (row_mode)
            {
                if (select_ind == coords[0])
                {
                    to_deselect = true;
                    drag_x0 = e.GetPosition(this).X;
                    drag_y0 = e.GetPosition(this).Y;
                    return;
                }
                for (int j = 0; j < matrix.M; j++)
                    guides[coords[0]][j].Fill = new SolidColorBrush(Color.FromArgb(70, 255, 255, 0));
                var w = new DialogCoeff(dark_mode);
                w.Owner = this;
                w.ShowDialog();
                if (w.DialogResult == true)
                {
                    matrix.ElementarySum(coords[0], select_ind, w.Coefficient);
                    ApplyChanges();
                    AddEntry(coords[0], select_ind, w.Coefficient);
                }
                for (int j = 0; j < matrix.M; j++)
                    guides[coords[0]][j].Fill = Brushes.Transparent;
                Deselect();
            }
            else
            {
                if (select_ind == coords[1])
                {
                    to_deselect = true;
                    drag_x0 = e.GetPosition(this).X;
                    drag_y0 = e.GetPosition(this).Y;
                    return;
                }
                for (int i = 0; i < matrix.N; i++)
                    guides[i][coords[1]].Fill = new SolidColorBrush(Color.FromArgb(70, 255, 255, 0));
                var w = new DialogCoeff(dark_mode);
                w.Owner = this;
                w.ShowDialog();
                if (w.DialogResult == true)
                {
                    matrix.ElementarySum(coords[1], select_ind, w.Coefficient, false);
                    ApplyChanges();
                    AddEntry(coords[1], select_ind, w.Coefficient, false);
                }
                for (int i = 0; i < matrix.N; i++)
                    guides[i][coords[1]].Fill = Brushes.Transparent;
                Deselect();
            }
            e.Handled = true;
        }

        private void Rect_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (drag_mode) EndDrag();
            else if (to_deselect)
            {
                to_deselect = false;
                Deselect();
            }
        }

        private void mainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!to_deselect && !drag_mode && selected) Deselect();
        }

        private void EndDrag()
        {
            Deselect();
            drag_mode = false;
            to_deselect = false;
            if (row_mode)
            {
                for (int j = 0; j < matrix.M; j++)
                {
                    guides[select_ind][j].Fill = Brushes.Transparent;
                    guides[select_ind][j].Margin = new Thickness();
                    controls[select_ind][j].Margin = new Thickness();
                    guides[over_ind][j].Margin = new Thickness();
                    controls[over_ind][j].Margin = new Thickness();
                    Grid.SetZIndex(controls[select_ind][j], 1);
                    Grid.SetZIndex(guides[select_ind][j], 2);
                }
                if (select_ind == over_ind) return;
                matrix.ElementarySwap(over_ind, select_ind);
                AddEntry(over_ind, select_ind);
            } else
            {
                for (int i = 0; i < matrix.N; i++)
                {
                    guides[i][select_ind].Fill = Brushes.Transparent;
                    guides[i][select_ind].Margin = new Thickness();
                    controls[i][select_ind].Margin = new Thickness();
                    guides[i][over_ind].Margin = new Thickness();
                    controls[i][over_ind].Margin = new Thickness();
                    Grid.SetZIndex(controls[i][select_ind], 1);
                    Grid.SetZIndex(guides[i][select_ind], 2);
                }
                if (select_ind == over_ind) return;
                matrix.ElementarySwap(over_ind, select_ind, false);
                AddEntry(over_ind, select_ind, false);
            }
            ApplyChanges();
        }

        private void Deselect()
        {
            selected = false;
            if (row_mode)
            {
                for (int j = 0; j < matrix.M; j++)
                    guides[select_ind][j].Fill = Brushes.Transparent;
            } else
            {
                for (int i = 0; i < matrix.N; i++)
                    guides[i][select_ind].Fill = Brushes.Transparent;
            }
        }
        
        private void Box_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (text_change_handled)
            {
                text_change_handled = false;
                return;
            }
            try
            {
                if (!easter_found && ((TextBox)sender).Text.ToLower().Contains("anime"))
                {
                    int[] coords = GetUidCoords(sender);
                    text_change_handled = true;
                    ((TextBox)sender).Text = matrix[coords].ToString();
                    ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length;
                    UnlockAnime();
                }
                else if (((TextBox)sender).Text.Contains("&") || ((TextBox)sender).Text.Contains("\\\\"))
                {
                    matrix = LaTeXTools.FromLaTeX(((TextBox)sender).Text);
                    Init(matrix.Dimensions);
                }
                else if (!((TextBox)sender).Text.Contains("frac") && ((TextBox)sender).Text.Contains("{"))
                {
                    matrix = LaTeXTools.FromWolfram(((TextBox)sender).Text);
                    Init(matrix.Dimensions);
                }
                else
                {
                    if (((TextBox)sender).Text.Contains("\n") || ((TextBox)sender).Text.Contains("\r"))
                    {
                        text_change_handled = true;
                        ((TextBox)sender).Text = ((TextBox)sender).Text.Replace("\n", "").Replace("\r", "");
                        ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length;
                    }
                    int[] coords = GetUidCoords(sender);
                    if (((TextBox)sender).Text.Contains("frac"))
                    {
                        matrix[coords] = LaTeXTools.ParseFrac(((TextBox)sender).Text);
                        text_change_handled = true;
                        ((TextBox)sender).Text = matrix[coords].ToString();
                        ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length;
                    }
                    else
                        matrix[coords] = new Rational(((TextBox)sender).Text);
                    controls_sync[coords[0]][coords[1]] = true;
                }
            }
            catch {
                int[] coords = GetUidCoords(sender);
                if (((TextBox)sender).Text.Contains("&") || ((TextBox)sender).Text.Contains("{"))
                {
                    text_change_handled = true;
                    ((TextBox)sender).Text = matrix[coords].ToString();
                    ((TextBox)sender).CaretIndex = ((TextBox)sender).Text.Length;
                } else
                {
                    controls_sync[coords[0]][coords[1]] = false;
                }
            }
        }

        private static readonly Regex _regex = new Regex("[^0-9./-]+");
        private static bool IsTextAllowed(string text) => !_regex.IsMatch(text);
        private void Box_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        void ApplyChanges()
        {
            for (int i = 0; i < matrix.N; i++)
                for (int j = 0; j < matrix.M; j++)
                {
                    text_change_handled = true;
                    controls[i][j].Text = matrix[i, j].ToString();
                    controls_sync[i][j] = true;
                }
            text_change_handled = false;
        }

        void AddEntry(int i, Rational c, bool row = true)
        {
            SyncStory();
            history.Add(new KeyValuePair<int[], Rational>(
                new int[] { 0, Convert.ToInt32(row), i }, c));
        }

        void AddEntry(int i1, int i2, bool row = true)
        {
            SyncStory();
            history.Add(new KeyValuePair<int[], Rational>(
                new int[] { 1, Convert.ToInt32(row), i1, i2 }, 0));
        }

        void AddEntry(int i1, int i2, Rational c, bool row = true)
        {
            SyncStory();
            history.Add(new KeyValuePair<int[], Rational>(
                new int[] { 2, Convert.ToInt32(row), i1, i2 }, c));
        }

        void SyncStory()
        {
            while (history.Count > story_ind)
                history.RemoveAt(history.Count - 1);
            story_ind++;
        }

        void SyncInput()
        {
            for (int i = 0; i < matrix.N; i++)
                for (int j = 0; j < matrix.M; j++)
                    if (!controls_sync[i][j])
                        matrix[i, j] = 0;
            ApplyChanges();
        }

        private void modeButton_Click(object sender, RoutedEventArgs e)
        {
            if (matrix == null) return;
            if (edit_mode)
            {
                history = new List<KeyValuePair<int[], Rational>>();
                story_ind = 0;
                edit_mode = false;
                modeButton.Focus();
                start_copy = matrix.Clone();
                SyncInput();
                modeButton.Content = "Edit";
                actionButton.IsEnabled = false;
                foreach(var row in guides)
                    foreach (var rect in row)
                        rect.Visibility = Visibility.Visible;
            } else
            {
                if (selected) Deselect();
                edit_mode = true;
                modeButton.Content = "Transform";
                actionButton.IsEnabled = true;
                foreach (var row in guides)
                    foreach (var rect in row)
                        rect.Visibility = Visibility.Hidden;
            }
        }

        private void dimButton_Click(object sender, RoutedEventArgs e)
        {
            if (!edit_mode)
                modeButton_Click(modeButton, new RoutedEventArgs());
            Dialogue w;
            if (matrix != null)
                w = new Dialogue(dark_mode, matrix.N, matrix.M);
            else
                w = new Dialogue(dark_mode);
            w.Owner = this;
            w.ShowDialog();
            if (w.DialogResult == true)
                Init(w.Dimensions);
        }

        private void Box_LostFocus(object sender, RoutedEventArgs e)
        {
            int[] coords = GetUidCoords(sender);
            if (coords[0] >= matrix.N || coords[1] >= matrix.M)
                return;
            if (!controls_sync[coords[0]][coords[1]])
            {
                matrix[coords] = 0;
                text_change_handled = true;
                ((TextBox)sender).Text = "0";
            }
        }

        private void Box_GotFocus(object sender, RoutedEventArgs e)
        {
            focus = true;
        }

        private void Box_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (focus)
            {
                focus = false;
                ((TextBox)sender).SelectAll();
            }
        }

        private void Box_KeyUp(object sender, KeyEventArgs e)
        {
            if (focus)
            {
                focus = false;
                ((TextBox)sender).SelectAll();
            }
        }

        private void Box_KeyDown(object sender, KeyEventArgs e)
        {
            int[] coords = GetUidCoords(sender);
            if (e.Key == Key.Right && (((TextBox)sender).CaretIndex == ((TextBox)sender).Text.Length || ((TextBox)sender).SelectionLength == ((TextBox)sender).Text.Length))
            {
                if (++coords[1] >= matrix.M)
                {
                    if (++coords[0] < matrix.N)
                        controls[coords[0]][0].Focus();
                } else
                    controls[coords[0]][coords[1]].Focus();
            } else if (e.Key == Key.Left && (((TextBox)sender).CaretIndex == 0 || ((TextBox)sender).SelectionLength == ((TextBox)sender).Text.Length))
            {
                if (--coords[1] < 0)
                {
                    if (--coords[0] >= 0)
                        controls[coords[0]][matrix.M - 1].Focus();
                }
                else
                    controls[coords[0]][coords[1]].Focus();
            } else if (e.Key == Key.Up)
            {
                if (--coords[0] >= 0)
                        controls[coords[0]][coords[1]].Focus();
            }
            else if (e.Key == Key.Down)
            {
                if (++coords[0] < matrix.N)
                    controls[coords[0]][coords[1]].Focus();
            }
        }

        private void Undo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!edit_mode) {
               
                if (story_ind > 0)
                {
                    story_ind--;
                    int[] args = history[story_ind].Key;
                    Rational c = history[story_ind].Value;
                    if (args[0] == 0)
                        matrix.ElementaryProd(args[2], 1 / c, Convert.ToBoolean(args[1]));
                    else if (args[0] == 1)
                        matrix.ElementarySwap(args[2], args[3], Convert.ToBoolean(args[1]));
                    else
                        matrix.ElementarySum(args[2], args[3], -c, Convert.ToBoolean(args[1]));
                    ApplyChanges();
                }
            }
        }

        private void Redo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!edit_mode)
            {
                if (story_ind < history.Count)
                {
                    ApplyElementary(matrix, history[story_ind].Key, history[story_ind].Value);
                    story_ind++;
                    ApplyChanges();
                }
            }
        }

        void ApplyElementary(MyMatrix matrix, int[] args, Rational c)
        {
            if (args[0] == 0)
                matrix.ElementaryProd(args[2], c, Convert.ToBoolean(args[1]));
            else if (args[0] == 1)
                matrix.ElementarySwap(args[2], args[3], Convert.ToBoolean(args[1]));
            else
                matrix.ElementarySum(args[2], args[3], c, Convert.ToBoolean(args[1]));
        }

        private void latex_Click(object sender, RoutedEventArgs e)
        {
            if (matrix == null) return;
            SyncInput();
            LaTeXExport export = new LaTeXExport(matrix.M);
            if (!edit_mode)
            {
                MyMatrix tmp = start_copy.Clone();
                for (int i = 0; i < story_ind + 1; i++)
                {
                    if (i != 0)
                        ApplyElementary(tmp, history[i - 1].Key, history[i - 1].Value);
                    export.Add(tmp.GetLaTeX());
                }
            }
            else
            {
                export.Add(matrix.GetLaTeX());
            }
            Export(export.Result);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (instance_links.Count == 1)
            {
                Cache.WriteCache(dark_mode, easter_found);
                foreach (var w in export_windows)
                    w.Close();
            } else
            {
                instance_links.Remove(name);
                if (awaiting_confirmation)
                {
                    CancelAwait();
                    input_child.popup1.ShowDialogBox(this, $"\"{action_in_progress}\" cancelled!");
                }
                else if (owner_mode)
                {
                    owner.CancelAwait();
                    owner.popup1.ShowDialogBox(this, $"\"{action_in_progress}\" cancelled!");
                }
            }
        }

        private void mainWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (!edit_mode) modeButton_Click(new object(), new RoutedEventArgs());
                else if (awaiting_confirmation || owner_mode)
                {
                    if (awaiting_confirmation) CancelAwait();
                    else owner.CancelAwait();
                    popup1.ShowDialogBox(this, $"\"{action_in_progress}\" cancelled!");
                }
            }
        }

        private void actionButton_Click(object sender, RoutedEventArgs e)
        {
            if (matrix == null) return;
            if (!edit_mode)
            {
                popup1.ShowDialogBox(this, "Not available in transform mode!");
                return;
            }
            if (owner_mode)
            {
                SyncInput();
                try { owner.ConfirmAction(); }
                catch (Exception ex)
                {
                    popup1.ShowDialogBox(this, ex.Message);
                    return;
                }
                owner_mode = false;
                return;
            }
            ActionPanel panel = new ActionPanel(dark_mode, matrix.Determinant, matrix.Rank, name);
            panel.Owner = this;
            panel.ShowDialog();
            if (panel.MatrixName.Length > 0) TryRename(panel.MatrixName);
            if (panel.DialogResult == true) DoAction(panel.SelectedOption);
        }

        void DoAction(Action action, string arg = "")
        {
            if (!edit_mode)
            {
                popup1.ShowDialogBox(this, "Not available in transform mode!");
                return;
            }
            //Focus();
            if (action == Action.New)
            {
                StringDialog w = new StringDialog(dark_mode);
                w.Owner = this;
                w.ShowDialog();
                if (w.DialogResult == true)
                {
                    MainWindow another_input =
                        new MainWindow(w.Output, dark_mode, easter_found);
                    another_input.Show();
                }
            }
            else if (action == Action.Transpose)
            {
                SyncInput();
                matrix.Transpose();
                if (matrix.IsSquare)
                    ApplyChanges();
                else
                    Init(matrix.Dimensions);
            }
            else if (action == Action.Inverse)
            {
                SyncInput();
                try
                {
                    if (arg.Length == 0) Export(matrix.InverseLaTeX());
                    else matrix.Inverse();
                    ApplyChanges();
                }
                catch (InvalidOperationException)
                {
                    popup1.ShowDialogBox(this, "Matrix is not square!");
                }
                catch (Exception ex)
                {
                    popup1.ShowDialogBox(this, ex.Message);
                }
            }
            else if (action == Action.Gauss)
            {
                SyncInput();
                DialogCoeff w = new DialogCoeff(dark_mode, "Set free columns", 0);
                w.Owner = this;
                w.ShowDialog();
                if (w.DialogResult == true)
                {
                    Export(matrix.GaussLaTeX(matrix.M - (int)w.Coefficient.Numerator));
                    ApplyChanges();
                }
            }
            else if (action == Action.Im)
            {
                SyncInput();
                Export(matrix.ImBasis());
            }
            else if (action == Action.Ker)
            {
                SyncInput();
                Export(matrix.KerBasis());
            }
            else if (action == Action.NewBasis)
            {
                if (!matrix.IsSquare)
                {
                    popup1.ShowDialogBox(this, "Matrix is not square!");
                    return;
                }
                if (arg == "")
                {
                    StringDialog w = new StringDialog(dark_mode);
                    w.Owner = this;
                    w.ShowDialog();
                    if (w.DialogResult == true) arg = w.Output;
                    else return;
                }
                if (instance_links.ContainsKey(arg))
                {
                    MyMatrix C = instance_links[arg].matrix;
                    ChooseDialog w = new ChooseDialog(dark_mode, "Choose method", new List<string> { "Operator", "Bilinear" });
                    w.Owner = this;
                    w.ShowDialog();
                    if (w.DialogResult != true)
                    {
                        CancelAwait();
                        input_child.popup1.ShowDialogBox(this, $"\"{action_in_progress}\" cancelled!");
                        return;
                    }
                    if (w.Variant == 0)
                    {
                        try
                        {
                            matrix = C.Inverted() * matrix * C;
                            Export($"Find ${arg}^{{-1}}$:\n" + C.Clone().InverseLaTeX() + $"\n$${name}\'={arg}^{{-1}}{name}{arg}=" + matrix.GetLaTeX() + "$$");
                        } catch (Exception ex)
                        {
                            popup1.ShowDialogBox(this, ex.Message);
                            return;
                        }
                    }
                    else
                    {
                        matrix = C.Transposed() * matrix * C;
                        Export($"$${arg}^T={C.Transposed().GetLaTeX()}$$\n$${name}\'={arg}^T{name}{arg}=" + matrix.GetLaTeX() + "$$");
                    }
                    ApplyChanges();
                }
                else
                {
                    SyncInput();
                    MainWindow another_input = new MainWindow(arg, this, false);
                    another_input.matrix = matrix.Id;
                    another_input.Show();
                    Await(another_input, action);
                }
            }
            else if (action == Action.Multiply)
            {
                if (arg == "")
                {
                    StringDialog w = new StringDialog(dark_mode);
                    w.Owner = this;
                    w.ShowDialog();
                    if (w.DialogResult == true) arg = w.Output;
                    else return;
                }
                if (instance_links.ContainsKey(arg))
                {
                    bool refresh_mode = matrix.M == instance_links[arg].matrix.M;
                    matrix *= instance_links[arg].matrix;
                    if (refresh_mode)
                        ApplyChanges();
                    else
                        Init(matrix.Dimensions);
                    Focus();
                }
                else
                {
                    SyncInput();
                    MainWindow another_input = new MainWindow(arg, this);
                    another_input.matrix = new MyMatrix(matrix.M, matrix.N);
                    another_input.Show();
                    Await(another_input, action);
                }
            }
            else if (action == Action.Sum)
            {
                if (arg == "")
                {
                    StringDialog w = new StringDialog(dark_mode);
                    w.Owner = this;
                    w.ShowDialog();
                    if (w.DialogResult == true) arg = w.Output;
                    else return;
                }
                if (instance_links.ContainsKey(arg))
                {
                    matrix += instance_links[arg].matrix;
                    ApplyChanges();
                    Focus();
                }
                else
                {
                    SyncInput();
                    MainWindow another_input = new MainWindow(arg, this, false);
                    another_input.matrix = new MyMatrix(matrix.N, matrix.M);
                    another_input.Show();
                    Await(another_input, action);
                }
            }
            else if (action == Action.Fill)
            {
                if (arg.Length > 0)
                {
                    Randomize();
                    ApplyChanges();
                    return;
                }
                ChooseDialog w = new ChooseDialog(dark_mode, "Choose fill", new List<string> { "Reset", "Number", "Random", "Identity" });
                w.Owner = this;
                w.ShowDialog();
                if (w.DialogResult != true) return;
                if (w.Variant == 0) matrix.Reset();
                else if (w.Variant == 1)
                {
                    DialogCoeff c_dialg = new DialogCoeff(dark_mode, "Set number");
                    c_dialg.Owner = this;
                    c_dialg.ShowDialog();
                    if (c_dialg.DialogResult == true)
                        matrix.Reset(c_dialg.Coefficient);
                }
                else if (w.Variant == 2) Randomize();
                else matrix = matrix.Id;
                ApplyChanges();
            }
            else if (action == Action.Projector)
            {
                SyncInput();
                DialogCoeff w = new DialogCoeff(dark_mode, "Set free columns", 0);
                w.Owner = this;
                w.ShowDialog();
                if (w.DialogResult == true)
                {
                    try
                    {
                        if (w.Coefficient == 0)
                        {
                            if (matrix.IsSquare)
                            {
                                Export($"${name}$ is square:\n$$P={name}\\left({name}^T{name}\\right)^{{-1}}{name}^T={name}{name}^{{-1}}\\left({name}^T\\right)^{{-1}}{name}^T=" + matrix.Id.GetLaTeX() + "$$");
                            }
                            else
                            {
                                MyMatrix At = matrix.Transposed();
                                MyMatrix tmp = matrix * (At * matrix).Inverted() * At;
                                Export($"Find $\\left({name}^T{name}\\right)^{{-1}}$:\n" + (At * matrix).InverseLaTeX() + $"\n$$P={name}\\left({name}^T{name}\\right)^{{-1}}{name}^T=" + tmp.GetLaTeX() + "$$");
                            }
                        }
                        else
                        {
                            MyMatrix A = matrix.Clone();
                            A.M -= (int)w.Coefficient.Numerator;
                            if ((A.Transposed() * A).Determinant == 0)
                            {
                                string output = $"$${name}={A.GetLaTeX()},\\ \\det {name}^T{name}=0$$\n";
                                A.Transpose();
                                output += $"Ortogonalize ${name}$:\n" + A.GramSchmidt() + "\n";
                                A.Transpose();
                                MyMatrix new_system = matrix.Clone();
                                for (int i = A.M; i < matrix.M; i++)
                                {
                                    LaTeXExport export = new LaTeXExport(A.M, "+", 1, begin: $"$$\\text{{pr}}_{{{name}}}b_{{{i - A.M}}}=", begin_width: 2);
                                    MyMatrix free_column = matrix.GetColumn(i);
                                    MyMatrix pr = new MyMatrix(matrix.N, 1);
                                    for (int j = 0; j < A.M; j++) {
                                        MyMatrix vectorIm = A.GetColumn(j);
                                        if (vectorIm.IsZero) continue;
                                        Rational scalar = MyMatrix.ScalarProduct(vectorIm, free_column), square = MyMatrix.ScalarProduct(vectorIm, vectorIm);
                                        Rational.RemoveDivisor(ref scalar, ref square);
                                        export.Add($"\\frac{{{scalar}}}{{{square}}}" + vectorIm.Transposed().GetLaTeXLine());
                                        pr += scalar / square * vectorIm;
                                    }
                                    export.Add(pr.Transposed().GetLaTeXLine(), "=", 1);
                                    output += export.Result + "\n";
                                    new_system.SetColumn(i, pr);
                                }
                                Export(output + "Solve system for projections:\n" + new_system.GaussLaTeX(A.M));
                            }
                            else
                            {
                                MyMatrix tmp;
                                string output;
                                if (A.IsSquare)
                                {
                                    tmp = A.Clone();
                                    output = $"$${name}={A.GetLaTeX()}$$\n${name}$ is square:\n$$P_x=\\left({name}^T{name}\\right)^{{-1}}{name}^T={name}^{{-1}}\\left({name}^T\\right)^{{-1}}{name}^T={name}^{{-1}},\\ x={name}^{{-1}}\\cdot b$$\nFind ${name}^{{-1}}$:\n" + tmp.InverseLaTeX() + "\n";
                                }
                                else
                                {
                                    MyMatrix At = A.Transposed();
                                    tmp = (At * A).Inverted() * At;
                                    output = $"$${name}={A.GetLaTeX()}$$\nFind $\\left({name}^T{name}\\right)^{{-1}}$:\n" + (At * A).InverseLaTeX() + $"\n$$P_x=\\left({name}^T{name}\\right)^{{-1}}{name}^T=" + tmp.GetLaTeX() + ",\\ x=P_x\\cdot b$$\n";
                                }
                                LaTeXExport export = new LaTeXExport(5, ",\\ ", 1, false);
                                for (int i = A.M; i < matrix.M; i++)
                                    export.Add($"x_{{{i - A.M}}}=P_x" + matrix.GetColumn(i).GetLaTeX() + "=" + (tmp * matrix.GetColumn(i)).GetLaTeX());
                                Export(output + export.Result);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        popup1.ShowDialogBox(this, ex.Message);
                    }
                }
            }
            else if (action == Action.Diagonalize)
            {
                int method = 2;
                if (matrix.IsSquare)
                {
                    ChooseDialog w = new ChooseDialog(dark_mode, "Choose method", new List<string> { "Sym Gauss", "Jacobi", "Gram-Schmidt" });
                    w.Owner = this;
                    w.ShowDialog();
                    if (w.DialogResult != true) return;
                    method = w.Variant;
                }
                SyncInput();
                try
                {
                    if (method == 0)
                        Export(matrix.Clone().SymmetricGauss());
                    else if (method == 1)
                        Export(matrix.JacobiMethod());
                    else
                    {
                        Export(matrix.GramSchmidt());
                        ApplyChanges();
                    }
                }
                catch (Exception ex)
                {
                    popup1.ShowDialogBox(this, ex.Message);
                }
            }
        }

        void Randomize()
        {
            RandomSettings w = new RandomSettings(dark_mode, matrix.IsSquare);
            w.Owner = this;
            w.ShowDialog();
            try
            {
                if (w.DialogResult == true)
                {
                    matrix.Randomize(w.Min, w.Max, w.Step);
                    if (w.Method == RandomMethod.Symmetric)
                    {
                        for (int i = 0; i < matrix.N; i++)
                            for (int j = 0; j < i; j++)
                                matrix[i, j] = matrix[j, i];
                    }
                    else if (w.Method == RandomMethod.Skew)
                    {
                        for (int i = 0; i < matrix.N; i++)
                            for (int j = 0; j < i; j++)
                                matrix[i, j] = -matrix[j, i];
                        for (int i = 0; i < matrix.N; i++)
                            matrix[i, i] = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                popup1.ShowDialogBox(this, ex.Message);
            }
        }

        private void Transpose(object sender, ExecutedRoutedEventArgs e) =>
            DoAction(Action.Transpose);
        private void Inverse(object sender, ExecutedRoutedEventArgs e) =>
            DoAction(Action.Inverse, "no_export");
        private void Gauss(object sender, ExecutedRoutedEventArgs e) =>
            DoAction(Action.Gauss);
        private void ImBasis(object sender, ExecutedRoutedEventArgs e) =>
            DoAction(Action.Im);
        private void KerBasis(object sender, ExecutedRoutedEventArgs e) =>
            DoAction(Action.Ker);
        private void NewBasis(object sender, ExecutedRoutedEventArgs e) =>
            DoAction(Action.NewBasis);
        private void Multiply(object sender, ExecutedRoutedEventArgs e) =>
            DoAction(Action.Multiply);
        private void Sum(object sender, ExecutedRoutedEventArgs e) =>
            DoAction(Action.Sum);
        private void Fill(object sender, ExecutedRoutedEventArgs e) =>
            DoAction(Action.Fill);
        private void Random(object sender, ExecutedRoutedEventArgs e) =>
            DoAction(Action.Fill, "Random");

        private void Action_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            actionButton_Click(new object(), new RoutedEventArgs());
        }

        private void ConfirmAction()
        {
            DoAction(action_in_progress, input_child.name);
            awaiting_confirmation = false;
            CancelAwait();
        }

        private void Await(MainWindow input, Action action)
        {
            awaiting_confirmation = true;
            action_in_progress = action;
            actionButton.IsEnabled = modeButton.IsEnabled = dimButton.IsEnabled = false;
            input.action_in_progress = action;
            input_child = input;
        }

        private void CancelAwait()
        {
            awaiting_confirmation = false;
            actionButton.IsEnabled = modeButton.IsEnabled = dimButton.IsEnabled = true;
            input_child.actionButton.Content = "Action";
            input_child.owner_mode = false;
        }

        private void Export(string output)
        {
            LaTeX_ExportWindow le = new LaTeX_ExportWindow(dark_mode, output);
            export_windows.Add(le);
            le.Show();
        }

        private void TryRename(string new_name)
        {
            if (instance_links.Keys.Contains(new_name)) return;
            instance_links.Remove(name);
            name = new_name;
            instance_links[name] = this;
            Title = "Matrix Elementary: " + name;
        }

        private static int[] GetUidCoords(object field)
        {
            string uid = ((UIElement)field).Uid;
            int comInd = uid.IndexOf(',');
            return new int[] { Convert.ToInt32(uid.Substring(0, comInd)),
                Convert.ToInt32(uid.Substring(comInd + 1, uid.IndexOf('_') - comInd - 1)) };
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            Settings w = new Settings(dark_mode, easter_found);
            w.Owner = this;
            w.ShowDialog();
        }

        public static void ChangeAppTheme()
        {
            foreach (var w in instance_links.Values) w.ChangeTheme();
        }

        public void ChangeTheme()
        {
            foreach (var e in export_windows)
                e.ChangeTheme();
            if (!dark_mode)
            {
                dark_mode = true;
                mainWindow.Background = new SolidColorBrush(Color.FromArgb(255, 34, 34, 34));
                mainWindow.Foreground = Brushes.White;
                mainWindow.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 80, 80, 80));
                mainWindow.Resources.Clear();
                mainWindow.Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_black.ico"));
                popup1.Darken();
            }
            else
            {
                dark_mode = false;
                mainWindow.Background = Brushes.White;
                mainWindow.Foreground = Brushes.Black;
                mainWindow.BorderBrush = null;
                mainWindow.Resources.Add(typeof(Button), new Style() { TargetType = typeof(Button) });
                mainWindow.Resources.Add(typeof(TextBox), new Style() { TargetType = typeof(TextBox) });
                mainWindow.Icon = new BitmapImage(new Uri(@"pack://application:,,,/images/ico_white.ico"));
                popup1.Lighten();
            }
            if (easter_found) Anime();
        }

        void UnlockAnime()
        {
            WindowState = WindowState.Maximized;
            easter_found = true;
            imageBack.Visibility = Visibility.Visible;
            Anime();
            popup1.ShowDialogBox(this, "Anime mode unlocked!", 3);
        }

        void AnimeCheck()
        {
            if (easter_found)
            {
                imageBack.Visibility = Visibility.Visible;
                Width = 818;
                Height = 470;
                Anime();
            }
        }

        private void Anime()
        {
            if (dark_mode)
                imageBack.Source = new BitmapImage(new Uri(@"pack://application:,,,/images/Akemi_Homura.jpg"));
            else
                imageBack.Source = new BitmapImage(new Uri(@"pack://application:,,,/images/Madoka_Kaname.jpg"));
        }

        public static void Disney()
        {
            foreach (var w in instance_links.Values)
            {
                w.imageBack.Visibility = Visibility.Hidden;
                w.easter_found = false;
            }
        }
    }

    public static class Commands
    {
        public static RoutedCommand CtrlT = new RoutedCommand();
        public static RoutedCommand CtrlD = new RoutedCommand();
        public static RoutedCommand CtrlE = new RoutedCommand();
        public static RoutedCommand CtrlA = new RoutedCommand();
        public static RoutedCommand CtrlI = new RoutedCommand();
        public static RoutedCommand CtrlG = new RoutedCommand();
        public static RoutedCommand CtrlB = new RoutedCommand();
        public static RoutedCommand CtrlK = new RoutedCommand();
        public static RoutedCommand CtrlM = new RoutedCommand();
        public static RoutedCommand CtrlP = new RoutedCommand();
        public static RoutedCommand CtrlC = new RoutedCommand();
        public static RoutedCommand CtrlN = new RoutedCommand();
        public static RoutedCommand CtrlS = new RoutedCommand();
        public static RoutedCommand CtrlF = new RoutedCommand();
        public static RoutedCommand CtrlL = new RoutedCommand();
        public static RoutedCommand CtrlR = new RoutedCommand();

        public static void Init()
        {
            CtrlT.InputGestures.Add(new KeyGesture(Key.T, ModifierKeys.Control));
            CtrlD.InputGestures.Add(new KeyGesture(Key.D, ModifierKeys.Control));
            CtrlE.InputGestures.Add(new KeyGesture(Key.E, ModifierKeys.Control));
            CtrlA.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            CtrlI.InputGestures.Add(new KeyGesture(Key.I, ModifierKeys.Control));
            CtrlG.InputGestures.Add(new KeyGesture(Key.G, ModifierKeys.Control));
            CtrlB.InputGestures.Add(new KeyGesture(Key.B, ModifierKeys.Control));
            CtrlK.InputGestures.Add(new KeyGesture(Key.K, ModifierKeys.Control));
            CtrlM.InputGestures.Add(new KeyGesture(Key.M, ModifierKeys.Control));
            CtrlP.InputGestures.Add(new KeyGesture(Key.P, ModifierKeys.Control));
            CtrlC.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control));
            CtrlN.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
            CtrlS.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
            CtrlF.InputGestures.Add(new KeyGesture(Key.F, ModifierKeys.Control));
            CtrlL.InputGestures.Add(new KeyGesture(Key.L, ModifierKeys.Control));
            CtrlR.InputGestures.Add(new KeyGesture(Key.R, ModifierKeys.Control));
        }
    }
}
