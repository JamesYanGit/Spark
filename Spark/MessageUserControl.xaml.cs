using Spark.Controller.MessageController;
using Spark.Interface;
using Spark.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Spark
{
    /// <summary>
    /// Interaction logic for MessageUserControl.xaml
    /// </summary>
    public partial class MessageUserControl : UserControl
    {
        ObservableCollection<TextMessageModel> textChatHistoryMessages = new ObservableCollection<TextMessageModel>();
        public ContactModel _sendMessageContactModel { get; set; }
        public MessageUserControl()
        {
            InitializeComponent();
        }

        public void setContactModel(ContactModel sendMessageContactModel) 
        {
            _sendMessageContactModel = sendMessageContactModel;

            if (_sendMessageContactModel.ContactName == null)
            {
                sandPlan.Visibility = Visibility.Hidden;
            }
            else
            {
                sandPlan.Visibility = Visibility.Visible;
                //read chat history
                bindingChatHistory();
            }
        }

        public void bindingChatHistory() 
        {
            IReceiveMessage message = new MessageHistoryController(_sendMessageContactModel);
            ObservableCollection<TextMessageDetailInfo> tempModel = message.opreateInfo();
            ChatRecord.ItemsSource = tempModel;
        }


        private void sendMessagebutton_Click(object sender, RoutedEventArgs e)
        {
            if (SandText.Text != "" && SandText.Text != null)
            {
                ISendMessage sendMessage = new SendMessageController(_sendMessageContactModel, SandText.Text.Trim());
                sendMessage.opreateInfo();
                //Refresh history window
                bindingChatHistory();
            }

        }
    }
}
