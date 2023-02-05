using Spark.Abstract;
using Spark.Interface;
using Spark.Model;
using System;
using System.Collections.ObjectModel;

namespace Spark.Controller.MessageController
{
    public class MessageHistoryController : AMessage, IReceiveMessage
    {
        private string _textContent;
        ContactModel _sendMessageContactModel;
        public MessageHistoryController(ContactModel sendMessageContactModel)
        {
            _sendMessageContactModel=sendMessageContactModel;
        }
        public ObservableCollection<TextMessageDetailInfo> opreateInfo()
        {
            return readChatHistory(_sendMessageContactModel);
        }

    }
}
