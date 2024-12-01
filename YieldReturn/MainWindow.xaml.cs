using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YieldReturn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void OnGenerateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MainProgressBar.Value = 0;
                StartGeneration().ContinueWith(t => //То что происходит после завершения асинхронной задачи(в нашем случае обработка ошибок).
                {
                    if (t.IsFaulted)
                    {
                        Dispatcher.Invoke(() => MessageBox.Show($"Ошибка генерации: {t.Exception?.InnerException?.Message ?? "Неизвестная ошибка"}"));
                    }
                },
                TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        async Task StartGeneration()
        {

            int start = Convert.ToInt32(StartValueTextBox.Text);
            int end = Convert.ToInt32(FinalValueTextBox.Text);
            if(start < end && start == 0) 
            {
                await foreach (int number in GenerateNumbersAsync(start, end))
                {

                    MainListBox.Items.Add(number);

                }
            }
            else if(start < 0)
            {
                MessageBox.Show("Начальное значение меньше нуля.");
                return;
            }
            else if(start > end) 
            {
                MessageBox.Show("Начальное значение больше конеченого.");
                return ;
            }
            else if(start == end) 
            {
                MessageBox.Show("Начальное значение равно конечному.");
                return;
            }
            
           

            MessageBox.Show("Генерация завершена, сгенерировано " + end + " чисел.");
        }


        public async IAsyncEnumerable<int> GenerateNumbersAsync(int start, int end)
        {
            int totalNumbers = end - start + 1;
            double progressBarIncrement = 100.0 / totalNumbers;
            for (int i = start; i <= end; i++)
            {
                await Task.Delay(300);
                Dispatcher.Invoke(() => MainProgressBar.Value += progressBarIncrement);
                yield return i;

            }

        }
    }
}