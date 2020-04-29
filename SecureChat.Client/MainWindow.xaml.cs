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
        private List<char> _list;

        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder().WithUrl("http://localhost:54847/chathub")
                .Build();

            StartConnection();

            connection.On("ReceiveMessage", (string str, User user, DateTime dateTime) => ReceiveMessage(str, user, dateTime));
            connection.On("OnUserNameChanged", (User user) => OnUserNameChanged(user));

            SaveToList(@"Key.txt");

            connection.Closed += async (error) =>
            {
                MessageBox.Show(error.Message);
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
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

        private string GetCifer(int lengthOfKey)
        {
            string cifer = new string(_list.TakeLast(lengthOfKey).ToArray());
            _list.RemoveRange(_list.Count-lengthOfKey, lengthOfKey);
            UpdateCounterTextBox();
            return cifer;
        }
        
        private void ReceiveMessage(string str, User user, DateTime dateTime)
        {

            var cifer = GetCifer(str.Length);
            var decrypted = new string(CryptoAlgorithms.OneTimePad.encrypt(cifer, str).ToArray());
            ReceivedMessage.Text = "[ " + dateTime + "] " + user.Name + " (" + user.UserId + "): " +  decrypted;
        }
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text != "")
            {
                try
                {
                    var cifer = GetCifer(ClientMessage.Text.Length);
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

        private void SaveToList(string path) => _list = File.ReadAllText(path).ToArray().ToList();

        private void UpdateCounterTextBox()
        {
            CounterTextBox.Text = _list.Count.ToString();
        }

        private void OnClearing(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(sender, ClearReceived))
                ReceivedMessage.Clear();
            else if (ReferenceEquals(sender, ClearClient))
                ClientMessage.Clear();                
        }
    }
}
