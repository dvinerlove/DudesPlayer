using ClassLibrary.Models;
using CrashReporterDotNET;
using DudesPlayer.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DudesPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static bool IsLoged { get; set; }
        //public static bool IsHost { get; set; } = false;
        //public static User CurrentUser { get; set; }
        //public static List<User> UserList { get; internal set; } = new List<User>();


        public App()
        {

            ClientData.BaseUrl = "http://dudesplayer.somee.com";
            if (File.Exists("api.txt"))
            {
                ClientData.BaseUrl = File.ReadAllText("api.txt");
            }
            ClientData.EventsUrl = "ws://144.217.180.130:41235/";
            DudesPlayer.Properties.Settings.Default.Upgrade();
        }


        public static event EventHandler UpdateFieldsEvent; 
        internal static void Update()
        {
            UpdateFieldsEvent?.Invoke(null, EventArgs.Empty);
        }
        internal static void UpdatePlayer()
        {
            UpdateFieldsEvent?.Invoke(null, EventArgs.Empty);
        }
        private static ReportCrash _reportCrash;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.Current.DispatcherUnhandledException += DispatcherOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            _reportCrash = new ReportCrash("Email where you want to receive crash reports")
            {
                Silent = true
            };
            _reportCrash.RetryFailedReports();
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            SendReport(unobservedTaskExceptionEventArgs.Exception);
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            SendReport(dispatcherUnhandledExceptionEventArgs.Exception);
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            SendReport((Exception)unhandledExceptionEventArgs.ExceptionObject);
        }

        public static void SendReport(Exception exception, string developerMessage = "")
        {
            _reportCrash.Silent = false;
            MessageBox.Show($"Exception!!!\n\n{exception.Message}\n\n\n{exception.ToMessageString}\n\n\n{exception.StackTrace}");
            //_reportCrash.Send(exception);
        }

        public static void SendReportSilently(Exception exception, string developerMessage = "")
        {
            _reportCrash.Silent = true;
            //_reportCrash.Send(exception);
        }
        #region test
        private double deltaX;
        private double deltaY;
        private Point? buttonPosition;
        private TranslateTransform currentTranslateTransform;
        private Point mouseDownPosition;
        private Point mouseUpPosition;
        private bool toggleOpen;

        // Threshold of 3 pixels, but you can make this whatever your want.
        private const double DISTANCE_THRESHOLD = 3.0d;

        public void PreviewMouseDownEventHandler(object sender, MouseButtonEventArgs e)
        {
            var element = sender as UIElement;
            var container = VisualTreeHelper.GetParent(element) as UIElement;

            if (this.buttonPosition == null)
            {
                this.buttonPosition = element.TransformToAncestor(container).Transform(new Point(0, 0));
            }

            this.mouseDownPosition = Mouse.GetPosition(container);
            this.deltaX = this.mouseDownPosition.X - this.buttonPosition.Value.X;
            this.deltaY = this.mouseDownPosition.Y - this.buttonPosition.Value.Y;
        }

        public void PreviewMouseUpEventHandler(object sender, MouseButtonEventArgs e)
        {
            var element = sender as UIElement;
            var container = VisualTreeHelper.GetParent(element) as UIElement;
            this.currentTranslateTransform = element.RenderTransform as TranslateTransform;
            this.mouseUpPosition = Mouse.GetPosition(container);
            var distance = Point.Subtract(this.mouseUpPosition, this.mouseDownPosition).Length;

            if (distance < DISTANCE_THRESHOLD)
            {
                // Allow the popup to occur.
                var controlTemplate = ((Button)element).Template;
                var popup = (Popup)controlTemplate.FindName("myPopup", sender as FrameworkElement);
                toggleOpen = !toggleOpen;
                popup.IsOpen = toggleOpen;
            }
        }

        public void PreviewMouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var element = sender as UIElement;
                var container = VisualTreeHelper.GetParent(element) as UIElement;
                var mousePoint = Mouse.GetPosition(container);

                var offsetX = (this.currentTranslateTransform == null ?
                    this.buttonPosition.Value.X : this.buttonPosition.Value.X - this.currentTranslateTransform.X) + this.deltaX - mousePoint.X;
                var offsetY = (this.currentTranslateTransform == null ?
                    this.buttonPosition.Value.Y : this.buttonPosition.Value.Y - this.currentTranslateTransform.Y) + this.deltaY - mousePoint.Y;

                element.RenderTransform = new TranslateTransform(-offsetX, -offsetY);

                // To Stop Bubbling of MouseMove event
                e.Handled = true;
            }
        }
        #endregion
    }
}
