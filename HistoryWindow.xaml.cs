using System.Windows;
using System.IO;
namespace Calculator
{
    /// <summary>
    /// Логика взаимодействия для HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
        }

        //Функция выводящая данные из history.txt при открытии окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if the history file exists
                string historyFilePath = "history.txt";
                if (File.Exists(historyFilePath))
                {
                    // Read the contents of the history.txt file
                    string historyContent = File.ReadAllText(historyFilePath);

                    // Display the content in the TextBox
                    HistoryTextBox.Text = historyContent;
                }
                else
                {
                    // If the file doesn't exist, display a message
                    HistoryTextBox.Text = "История пуста.";
                }
            }
            catch (Exception ex)
            {
                // Handle any errors while reading the file
                MessageBox.Show($"Ошибка при загрузке истории: {ex.Message}");
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CalculatorWindow calculatorWindow = new();
            calculatorWindow.Show();
        }

        
    }
}
