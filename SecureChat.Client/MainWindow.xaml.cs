using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace SecureChat.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HubConnection connection;
        public MainWindow()
        {
            InitializeComponent();

            StartConnection();
            connection = new HubConnectionBuilder().WithUrl("http://localhost:54847/chathub")
                .Build();

            connection.On("ReceiveMessage", (string str) => ReceiveMessage(str));
            
            connection.Closed += async (error) =>
            {
                MessageBox.Show(error.Message);
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

         private string GetCiferFromTxt(int lengthOfKey)
        {
            string path = @"Key.txt";
            try
            {
                string content = File.ReadAllText(path);

                var cifer = content.Substring(0, lengthOfKey);

                File.WriteAllText(path, content.Substring(lengthOfKey-2), Encoding.UTF8);

                return cifer;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        private async void StartConnection()
        {
            try
            {
                await connection.StartAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        private void ReceiveMessage(string str)
        {

            var cifer = GetCiferFromTxt(str.Length);
            var decrypted = new string(CryptoAlgorithms.OneTimePad.encrypt(cifer, str).ToArray());
            ReceivedMessage.Text = decrypted;
        }
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cifer = GetCiferFromTxt(ClientMessage.Text.Length);
                var encrypted = new string(CryptoAlgorithms.OneTimePad.encrypt(cifer, ClientMessage.Text).ToArray());
                await connection.InvokeAsync("SendMessage", encrypted);
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
            
        }
        private void ClearButton2_Click(object sender, RoutedEventArgs e)
        {
            ReceivedMessage.Text = "";
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClientMessage.Text = "";
        }
    }
}
