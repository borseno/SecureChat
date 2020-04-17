using CryptoAlgorithms.Helpers;
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
        string cifer;
        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder().WithUrl("http://localhost:54847/chathub")
                .Build();

            StartConnection();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

         private void GetCiferFromTxt()
        {
            string path = @"C:\GitHub\SecureChat\SecureChat.Client\bin\Debug\netcoreapp3.1";
            int lengthOfKey = ClientMessage.Text.Length;

            try
            {
                string content = File.ReadAllText(path);

                cifer = content.Substring(0, lengthOfKey);
                
                File.WriteAllText(path, content.Substring(lengthOfKey - 1), Encoding.UTF8);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
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
        

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetCiferFromTxt();
                await connection.InvokeAsync("SendMessage", ClientMessage.Text, cifer);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClientMessage.Text = "";
        }
    }
}
