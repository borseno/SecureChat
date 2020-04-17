using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
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

            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:54847/")
                .Build();

            StartConnection();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

        private void GetNewVernamKey()
        {




            //PublicKey.Text = newKey;
        }

        private async void StartConnection()
        {
            await connection.StartAsync();
        }
        

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", ClientMessage.Text, PublicKey.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong... Please, try again");
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            //PublicKey.Text = "";
            ClientMessage.Text = "";
        }
    }
}
