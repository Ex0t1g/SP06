using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SP06
{

    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task[] _tasks;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            StatusText.Text = "Статус: Запуск...";

            _tasks = new Task[]
            {
                Task.Run(() => FillProgressBar(ProgressBar1, 1, _cancellationTokenSource.Token)),
                Task.Run(() => FillProgressBar(ProgressBar2, 2, _cancellationTokenSource.Token)),
                Task.Run(() => FillProgressBar(ProgressBar3, 3, _cancellationTokenSource.Token)),
                Task.Run(() => FillProgressBar(ProgressBar4, 4, _cancellationTokenSource.Token))
            };

            Task.WhenAll(_tasks).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Ошибка: {t.Exception.InnerExceptions[0].Message}");
                        StatusText.Text = "Статус: Готово";
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        StatusText.Text = "Статус: Выполненно!";
                    });
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            StatusText.Text = "Статус: Отмена...";
        }
        private void FillProgressBar(ProgressBar progressBar, int barNumber, CancellationToken cancellationToken)
        {
            try
            {
                for (int i = 0; i <= 100; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    Dispatcher.Invoke(() => progressBar.Value = i);
                    Thread.Sleep(100); 

                    if (i == 50)
                    {
                        throw new Exception($"в загрузки -> {barNumber}!");
                    }
                }
                Dispatcher.Invoke(() => progressBar.Value = 100); 
            }
            catch (OperationCanceledException)
            {
                Dispatcher.Invoke(() => StatusText.Text = $"Загрузка -> {barNumber} отменнена.");
            }
        }
    }
}