using AddressSelecter.Interface;
using AddressSelecter;
using FileSystemModule.FileOperation;
using FileSystemModule.Interface;
using FTPHelper;
using Spark.Abstract;
using Spark.Interface;
using Spark.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Spark.Controller.MessageController
{
    public class SendMessageController : AMessage, ISendMessage
    {


        private string _textContent;
        private string _messageTempFilePath;
        private bool _sendSuccess=true;

        public SendMessageController(ContactModel contactModel, string messageContent)
        {
            _contactModel = contactModel;
            _textContent = messageContent;
            _messageTempFilePath = AppDomain.CurrentDomain.BaseDirectory + @"Temp\" + base._contactModel.ContactFileName;
        }
        public void opreateInfo()
        {
            _sendSuccess = true;

            //1. read channel from contact file
            ContactModel lastestContactModel = readContactInfo(_contactModel.ContactNameCardFileName);

            //2. get text content model
            TextMessageModel newText = getNewTextMessageModel(MyProfileHelper.username, _textContent);
            //3. append message into temp file and ready to send 
            writeTextToTempFile(newText);
            while (_sendSuccess)
            {
                //4. connect to the FTP server based on the channel number
                connectFTPserver(FTPinfo.ftpCollection[lastestContactModel.NextChannelNum], lastestContactModel.ContactFileName, lastestContactModel.NextChannelNum);
                //5. upload the temp file

            
                if (sendMessage(AppDomain.CurrentDomain.BaseDirectory + @"Temp\" + lastestContactModel.ContactFileName))
                {
                    //6. update name card
                    //buildContactNameCard(_contactModel, _contactModel.ContactFileName, _contactModel.ContactName);
                    //6. Read chat history
                    ObservableCollection<TextMessageDetailInfo> tempHistory = readChatHistory(lastestContactModel);
                    //7. write text into history 
                    writeChatHistory(tempHistory, newText.MessageDetail[0]);

                    _sendSuccess=false;
                }
                else
                {
                    //change channel
                    IAddressSelecter addressSelecter = new IterationClass(MyProfileHelper.startSeverPoint, MyProfileHelper.endSeverPoint, lastestContactModel.NextChannelNum);
                    lastestContactModel.NextChannelNum = addressSelecter.getAddress();

                    //write into contact namecard
                    buildContactNameCard(lastestContactModel, lastestContactModel.ContactFileName, lastestContactModel.ContactName);
                }
            }
           

        }

        public void sendBackMsg(TextMessageModel newText)
        {
            //1. read channel from contact file
            ContactModel lastestContactModel = readContactInfo(_contactModel.ContactNameCardFileName);
            //3. append message into temp file and ready to send 
            writeTextToTempFile(newText);
            //4. connect to the FTP server based on the channel number
            connectFTPserver(FTPinfo.ftpCollection[lastestContactModel.NextChannelNum], lastestContactModel.ContactFileName, lastestContactModel.NextChannelNum);
            //5. upload the temp file
            if (sendMessage(AppDomain.CurrentDomain.BaseDirectory + @"Temp\" + lastestContactModel.ContactFileName))
            {
                //6. update name card
                buildContactNameCard(_contactModel, _contactModel.ContactFileName, _contactModel.ContactName);
                //6. Read chat history
                //ObservableCollection<TextMessageDetailInfo> tempHistory = readChatHistory(lastestContactModel);
                //7. write text into history 
                //writeChatHistory(tempHistory, newText.MessageDetail[0]);
            }

        }

        public void reSendMsg()
        {
            //1. read channel from contact file
            ContactModel lastestContactModel = readContactInfo(_contactModel.ContactNameCardFileName);
            //4. connect to the FTP server based on the channel number
            connectFTPserver(FTPinfo.ftpCollection[lastestContactModel.NextChannelNum], lastestContactModel.ContactFileName, lastestContactModel.NextChannelNum);
            //5. upload the temp file
            sendMessage(AppDomain.CurrentDomain.BaseDirectory + @"Temp\" + lastestContactModel.ContactFileName);

        }

        private void writeTextToTempFile(TextMessageModel newText)
        {
            List<string> tempMessageList = readTextFromTempFile();

            TextMessageModel tempHistory = new TextMessageModel();
            ObservableCollection<TextMessageDetailInfo> tempMessageContent = new ObservableCollection<TextMessageDetailInfo>();

            if (tempMessageList.Count > 0) //there are some message in temp file
            {
                tempHistory = JsonSerializer.Deserialize<TextMessageModel>(tempMessageList[0]);
                tempHistory.ReceiveStatus = newText.ReceiveStatus;
                tempMessageContent = tempHistory.MessageDetail;
                tempMessageContent.Add(newText.MessageDetail[0]);
                getTotalHashCode(tempHistory);
                tempMessageList.Clear();
                tempMessageList.Add(JsonSerializer.Serialize(tempHistory));
            }
            else
            {
                tempMessageList.Add(JsonSerializer.Serialize(newText));
            }

            IFileWriteControl fileWriteControl = new WriteTXTFile(_messageTempFilePath, tempMessageList);
            fileWriteControl.writeFileContent();
        }

        private List<string> readTextFromTempFile() 
        {
            IFileReadControl fileReadControl = new ReadTXTFile(_messageTempFilePath);
            List<string> tempString = fileReadControl.fileOperator();
            return tempString;
            
        }

        private void getTotalHashCode(TextMessageModel tempHistory) 
        {
            string tempString="";
            foreach (TextMessageDetailInfo item in tempHistory.MessageDetail)
            {
                tempString += item.TextContent + item.Speaker + item.Datetime;
            }
            tempHistory.HashCode = getHashCode(tempString);
        }


    }
}
