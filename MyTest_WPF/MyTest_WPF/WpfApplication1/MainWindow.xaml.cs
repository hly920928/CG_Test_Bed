using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GraphicsBook;
using System.Windows.Media.Animation;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Polygon myPolygon = new Polygon();
        Polygon mySubdivPolygon = new Polygon();
        bool isSubdivided = false;
        GraphPaper gp = null;
        Polygon myTriangle = null;
        bool ready = false;
        int subTime = 0;
        public MainWindow()
        {
            Debug.Print("In WpfApplication1::MainWindow()\n");
            InitializeComponent();
            InitializeCommands();
            InitializeInteraction();
            gp = this.FindName("Paper") as GraphPaper;
            gp.Background = Brushes.LightSlateGray;
            initPoly(myPolygon, Brushes.Black);
            initPoly(mySubdivPolygon, Brushes.Firebrick);
            gp.Children.Add(myPolygon);
            gp.Children.Add(mySubdivPolygon);
            
             //Gobal ready Flag,Avoid Sync Bug
             ready = true;
        }
        private void initPoly(Polygon p, SolidColorBrush b)
        {
            p.Stroke = b;
            p.StrokeThickness =3; 
            p.StrokeMiterLimit = 1;
            p.Fill = null;
        }
        #region XAML_Element Interaction handling
        public void b1Click(object sender, RoutedEventArgs e) {
            isSubdivided = true;
            if (subTime > 10) return;
            if (subTime != 0) myPolygon.Points = mySubdivPolygon.Points.Clone();
            mySubdivPolygon.Points.Clear();
            int len = myPolygon.Points.Count;
            //CreateSubDivPloy
            for(int i = 0; i < len; i++)
            {
                double x1 = myPolygon.Points[i].X;
                double x2 = myPolygon.Points[(i + 1)%len].X;
                double y1 = myPolygon.Points[i].Y;
                double y2 = myPolygon.Points[(i + 1) % len].Y;
                double delta_x = (x2 - x1) / 3;
                double delta_y = (y2 - y1) / 3;
                mySubdivPolygon.Points.Add(new Point(x1 + delta_x, y1 + delta_y));
                mySubdivPolygon.Points.Add(new Point(x1 + delta_x*2, y1 + delta_y*2));
            }
            subTime++;
        }
        public void b2Click(object sender, RoutedEventArgs e)
        {
            Debug.Print("Clear button clicked!\n");
            myPolygon.Points.Clear();
            mySubdivPolygon.Points.Clear();
            isSubdivided = false;
            e.Handled = true;
            subTime = 0;
        }
        public void slider1change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Debug.Print("Slider changed, ready = " + ready +
            ", and val = " + e.NewValue + ". \n");
            e.Handled = true;
            if (ready)
            {
                myPolygon.StrokeThickness = e.NewValue / 5;
            }
        }
        #endregion
        #region Menu, command, and keypress handling
        protected static RoutedCommand ExitCommand;
        protected void InitializeCommands()
        {
            InputGestureCollection inp = new InputGestureCollection();
            inp.Add(new KeyGesture(Key.X, ModifierKeys.Control));
            ExitCommand = new RoutedCommand("Exit", typeof(MainWindow), inp);
            CommandBindings.Add(new CommandBinding(ExitCommand, CloseApp));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseApp));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.New, NewCommandHandler));
        }
        protected void InitializeInteraction()
        {
            MouseLeftButtonDown += MouseButtonDownA;
            MouseLeftButtonUp += MouseButtonUpA;
            MouseMove += RESPOND_MouseMoveA;
        }
        void NewCommandHandler(Object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("You selected the New command",
                                Title,
                                MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
        }
        void KeyDownHandler(object sender, KeyEventArgs e)
        {
        }
        void CloseApp(Object sender, ExecutedRoutedEventArgs args)
        {
            if (MessageBoxResult.Yes ==
                MessageBox.Show("Really Exit?",
                                Title,
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Question)
               ) Close();
        }

        #endregion
        #region Mouse Event Handling
        public void MouseButtonUpA(object sender, RoutedEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseButtonEventArgs ee =
              (System.Windows.Input.MouseButtonEventArgs)e;
            Debug.Print("MouseUp at " + ee.GetPosition(this));
            e.Handled = true;
        }

        public void MouseButtonDownA(object sender, RoutedEventArgs e)
        {
            Debug.Print("Mouse down");
            if (ready)
            {
                if (sender != this) return;
                System.Windows.Input.MouseButtonEventArgs ee =
                   (System.Windows.Input.MouseButtonEventArgs)e;
                Debug.Print("MouseDown at " + ee.GetPosition(this));
                if (!isSubdivided)
                {
                    myPolygon.Points.Add(ee.GetPosition(gp));
                }
            }
            e.Handled = true;
        }


        public void RESPOND_MouseMoveA(object sender, MouseEventArgs e)
        {
            if (sender != this) return;
            System.Windows.Input.MouseEventArgs ee =
              (System.Windows.Input.MouseEventArgs)e;
            // Uncommment following line to get a flood of mouse-moved messages. 
            // Debug.Print("MouseMove at " + ee.GetPosition(this));

            e.Handled = true;
        }
        #endregion
    }
}
