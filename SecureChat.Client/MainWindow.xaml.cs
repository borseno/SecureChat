using Microsoft.AspNetCore.SignalR.Client;
using SecureChat.Shared;
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

            connection = new HubConnectionBuilder().WithUrl("http://localhost:54847/chathub")
                .Build();

            StartConnection();

            connection.On("ReceiveMessage", (string str, User user, DateTime dateTime) => ReceiveMessage(str, user, dateTime));
            connection.On("OnUserNameChanged", (User user) => OnUserNameChanged(user));

            connection.Closed += async (error) =>
            {
                MessageBox.Show(error.Message);
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

         private string GetCiferFromTxtAsync(int lengthOfKey)
        {
            string path = @"Key.txt";
            try
            {
                using (var fs = new FileStream(path, FileMode.Open))
                using (var reader = new StreamReader(fs, Encoding.UTF8))
                {
                    fs.Seek(-lengthOfKey, SeekOrigin.End);
                    string cifer = reader.ReadToEnd();

                    fs.Seek(-lengthOfKey, SeekOrigin.End);
                    fs.SetLength(fs.Position);

                    return cifer;
                }
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
        
        private void ReceiveMessage(string str, User user, DateTime dateTime)
        {

            var cifer = GetCiferFromTxtAsync(str.Length);
            var decrypted = new string(CryptoAlgorithms.OneTimePad.encrypt(cifer, str).ToArray());
            ReceivedMessage.Text = "[ " + dateTime + "] " + user.Name + " (" + user.UserId + "): " +  decrypted;
        }
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text != "")
            {
                try
                {
                    var cifer = GetCiferFromTxtAsync(ClientMessage.Text.Length);
                    var encrypted = new string(CryptoAlgorithms.OneTimePad.encrypt(cifer, ClientMessage.Text).ToArray());
                    await connection.InvokeAsync("SendMessage", encrypted, NameTextBox.Text, DateTime.Now);
                }
                catch (Exception e1)
                {
                    MessageBox.Show(e1.Message);
                }
            }
            else
                MessageBox.Show("Please enter your name.");
            
        }

        private void OnUserNameChanged(User user)
        {
            //MessageBox.Show(String.Format("User {0} has changed his username to {1}", user.UserId, Name));
        }
       
        private void ClearButton_Click(object sender, RoutedEventArgs e)

        private void OnClearing(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(sender, ClearReceived))
                ReceivedMessage.Clear();
            else if (ReferenceEquals(sender, ClearClient))
                ClientMessage.Clear();
        }
    }
}
