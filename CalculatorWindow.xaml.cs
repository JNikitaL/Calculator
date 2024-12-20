using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Calculator
{
    public partial class CalculatorWindow : Window
    {
        public CalculatorWindow()
        {
            InitializeComponent();
        }

        private void LoadHistory()
        {
            try
            {
                string historyFilePath = "history.txt";

                if (File.Exists(historyFilePath))
                {
                    string historyContent = File.ReadAllText(historyFilePath);

                    HistoryTextBox.Text = historyContent;
                }
                else
                {
                    HistoryTextBox.Text = "История пуста.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории: {ex.Message}");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadHistory();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inputNumber = InputNumberBox.Text;
                int inputBase, outputBase;

                if (!int.TryParse(InputBaseBox.Text, out inputBase) || inputBase < 2)
                {
                    MessageBox.Show("Основание входной системы счисления должно быть числом больше или равно 2.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(OutputBaseBox.Text, out outputBase) || outputBase < 2)
                {
                    MessageBox.Show("Основание выходной системы счисления должно быть числом больше или равно 2.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!IsValidNumberInBase(inputNumber, inputBase))
                {
                    MessageBox.Show($"Число \"{inputNumber}\" не является допустимым в системе счисления с основанием {inputBase}.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                long decimalValue = ConvertToDecimal(inputNumber, inputBase);

                string result = ConvertFromDecimal(decimalValue, outputBase);

                ResultBox.Text = result;

                CreateHistoryFile(inputNumber, inputBase, outputBase, result);
                LoadHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool IsValidNumberInBase(string number, int baseValue)
        {
            const string validChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string allowedChars = validChars.Substring(0, baseValue);

            foreach (char c in number.ToUpper())
            {
                if (!allowedChars.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        private long ConvertToDecimal(string number, int fromBase)
        {
            number = number.ToUpper();
            long decimalValue = 0;
            int power = 0;

            for (int i = number.Length - 1; i >= 0; i--)
            {
                char digitChar = number[i];
                int digitValue = (digitChar >= '0' && digitChar <= '9') ? digitChar - '0' : digitChar - 'A' + 10;

                if (digitValue >= fromBase)
                {
                    throw new FormatException($"Символ '{digitChar}' не допустим в системе счисления с основанием {fromBase}.");
                }

                decimalValue += digitValue * (long)Math.Pow(fromBase, power);
                power++;
            }

            return decimalValue;
        }

        private string ConvertFromDecimal(long decimalValue, int toBase)
        {
            if (decimalValue == 0) return "0";

            StringBuilder result = new StringBuilder();

            while (decimalValue > 0)
            {
                int remainder = (int)(decimalValue % toBase);
                result.Insert(0, (remainder < 10) ? (char)(remainder + '0') : (char)(remainder - 10 + 'A'));
                decimalValue /= toBase;
            }

            return result.ToString();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            InputNumberBox.Clear();
            InputBaseBox.Clear();
            OutputBaseBox.Clear();
            ResultBox.Clear();
            Console.WriteLine("Сброс");
        }

        private void CreateHistoryFile(string inputNumber, int inputBase, int outputBase, string result)
        {
            string filePath = "history.txt";

            if (!File.Exists(filePath))
            {

                using (StreamWriter writer = new StreamWriter(filePath, append: true))
                {
                    writer.WriteLine("---- История вычислений ----");
                    writer.WriteLine($"     {DateTime.Now}");
                }
            }

            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                writer.WriteLine($"Число: {inputNumber}, Основание ввода: {inputBase}, Основание вывода: {outputBase}, Результат: {result}");
            }
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (IsRussianLetter(e.Text))
            {
                e.Handled = true;
            }
        }

        private bool IsRussianLetter(string input)
        {
            return input.Any(c => (c >= 'А' && c <= 'Я') || (c >= 'а' && c <= 'я'));
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z)
            {
                OpenConsole();
            }
        }

        private void OpenConsole()
        {
            AllocConsole();
            Console.WriteLine("Консоль открыта!");
        }

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        private void InputNumberBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
    }
}
