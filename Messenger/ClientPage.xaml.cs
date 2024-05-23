using System.Windows;
using System.Windows.Controls;

namespace Messenger
{
    /// <summary>
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        private Client client;
        private Frame mainFrame;
        public ClientPage(Frame mainFrame)
        {
            InitializeComponent();
            this.mainFrame = mainFrame;
            client = new Client(AuthorizationWindow.IpServer);

            client.NewMessageHasBeenReceived += ClientPage_NewMessageHasBeenReceived;
            client.UserHasBeenConnected += ClientPage_UserHasBeenConnected;
            client.UserHasBeenDisconnected += ClientPage_UserHasBeenDisconnected;
        }

        private void ClientPage_UserHasBeenDisconnected(object? sender, string e)
        {
            var mainWindow = WindowManager.Instance.GetWindowByFrame(mainFrame);
            if (mainWindow != null && mainWindow is ChatWindow chatWindow)
            {
                chatWindow.RemoveUser(e);
            }
        }

        private void ClientPage_UserHasBeenConnected(object? sender, string e)
        {
            var mainWindow = WindowManager.Instance.GetWindowByFrame(mainFrame);
            if (mainWindow != null && mainWindow is ChatWindow chatWindow)
            {
                chatWindow.AddUser(e);
            }
        }

        private void ClientPage_NewMessageHasBeenReceived(object? sender, string e)
            => messagesListBox.Items.Add(e);

        private async void sendButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            await client.SendMessage(messageTextBox.Text);
            messageTextBox.Text = string.Empty;
        }

        private void exitButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            client.Disconnect();
            WindowManager.Instance.CloseWindowByFrame(mainFrame);
        }
    }
}