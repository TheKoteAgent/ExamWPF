using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExamWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public const string DialogFilePath = "last_dialog.txt";
    public MainWindow()
    {
        InitializeComponent();
    }
    private void SendButton_Click(object sender, RoutedEventArgs e)
    {
        // Отримати запит від користувача
        string userInput = UserInputTextBox.Text;

        if (string.IsNullOrWhiteSpace(userInput))
        {
            MessageBox.Show("Будь ласка, введіть текст для запиту.", "Помилка");
            return;
        }

        // Отримати відповідь від ChatGPT
        string response = GetChatGptResponse(userInput);

        // Відобразити відповідь
        ResponseTextBox.Text = response;

        // Зберегти діалог у файл
        SaveDialogToFile(userInput, response);
    }

    private void SaveDialogToFile(string question, string answer)
    {
        string filePath = "last_dialog.txt";

        try
        {
            File.WriteAllText(filePath, $"Question: {question}\nAnswer: {answer}");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при збереженні діалогу: " + ex.Message, "Помилка");
        }
    }

    private void LoadButton_Click(object sender, RoutedEventArgs e)
    {
        string filePath = "last_dialog.txt";

        try
        {
            if (File.Exists(filePath))
            {
                string dialog = File.ReadAllText(filePath);
                ResponseTextBox.Text = dialog;
            }
            else
            {
                MessageBox.Show("Файл з останнім діалогом не знайдено.", "Інформація");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Помилка при завантаженні діалогу: " + ex.Message, "Помилка");
        }
    }

    private string GetChatGptResponse(string prompt)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("x-apihub-key", "brP3JlyXvyDGE8GKPh0Q39ngKjsly5ol7sRj7Q18LU1VHBwJ7f");
            client.DefaultRequestHeaders.Add("x-apihub-host", "Cheapest-GPT-AI-Summarization.allthingsdev.co");
            client.DefaultRequestHeaders.Add("x-apihub-endpoint", "cba23f99-e8fb-4d0c-ab0b-2e0b38cc47b4");
            var requestBody = new
            {
                text = prompt,
                length = prompt.Length,
                style = "text"
            };

            string json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = client.PostAsync("https://Cheapest-GPT-AI-Summarization.proxy-production.allthingsdev.co/api/summarize", content).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;

                using (JsonDocument doc = JsonDocument.Parse(responseContent))
                {
                    if (doc.RootElement.TryGetProperty("result", out JsonElement result))
                    {
                        return result.GetString();
                    }
                    else if (doc.RootElement.TryGetProperty("summary", out JsonElement summary))
                    {
                        return summary.GetString();
                    }
                    else if (doc.RootElement.TryGetProperty("error", out JsonElement error))
                    {
                        return "Server Error: " + error.GetString();
                    }
                    else
                    {
                        return "Unknown response structure: " + responseContent;
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }


    private void Button_Click(object sender, RoutedEventArgs e)
    {
        string youtubeUrl = "https://youtu.be/dQw4w9WgXcQ?si=koN2Rf7Gz-0lHBy2";

        Process.Start(new ProcessStartInfo
        {
            FileName = youtubeUrl,
            UseShellExecute = true
        });
    }

    private void UserInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }
}