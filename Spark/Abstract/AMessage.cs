using AddressSelecter.Interface;
using AddressSelecter;
using FileSystemModule.FileOperation;
using FileSystemModule.Interface;
using FTPHelper;
using Spark.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Documents;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Spark.Abstract
{
    public class AMessage
    {
        protected FTPHostModel _fTPHost;
        protected string _fileName;
        protected string receiveString;
        protected int _CurrentChannelNum;
        protected int _StartPoint;
        protected int _EndPoint;
        protected TextMessageModel _messageModel;
        protected ObservableCollection<TextMessageDetailInfo> _messages=new ObservableCollection<TextMessageDetailInfo>();
        protected ContactModel _contactModel;
        
        public AMessage()
        {
            _StartPoint = 1;
            _EndPoint = 5;
        }

        protected TextMessageModel getNewTextMessageModel(string speaker, string textContent) 
        {
            ObservableCollection<TextMessageDetailInfo> messages = new ObservableCollection<TextMessageDetailInfo>();
            TextMessageDetailInfo textMessageDetailInfo = new TextMessageDetailInfo();
            textMessageDetailInfo.TextContent= textContent;
            textMessageDetailInfo.Speaker = speaker;
            textMessageDetailInfo.Datetime = DateTime.Now.ToString();

            messages.Add(textMessageDetailInfo);

            TextMessageModel messageModel=new TextMessageModel();
            messageModel.HashCode = getHashCode(textContent + speaker + DateTime.Now.ToString());
            messageModel.MessageDetail= messages;

            return messageModel;
        }

        protected string getHashCode(string tempString) 
        {
            string tempMessageByte;
            using (SHA256 _mySHA256 = SHA256.Create()) 
            {
                tempMessageByte = Encoding.ASCII.GetString(_mySHA256.ComputeHash(Encoding.ASCII.GetBytes(tempString)));
            }
                
            return tempMessageByte;
        }

        protected void connectFTPserver(FTPHostModel fTPHostModel, string fileName, int CurrentChannelNum)
        {
            _fTPHost = fTPHostModel;
            _fileName = fileName;
            _CurrentChannelNum = CurrentChannelNum;

        }

        protected ContactModel readContactInfo(string fileName) 
        {
            IFileReadControl fileReadControl = new ReadTXTFile(fileName);
            List<string> tempList = fileReadControl.fileOperator();

            return JsonSerializer.Deserialize<ContactModel>(tempList[0]);
        }

        protected ObservableCollection<TextMessageDetailInfo> readChatHistory(ContactModel sendMessageContactModel) 
        {
            _contactModel=sendMessageContactModel;

            IFileReadControl fileReadControl = new ReadTXTFile(AppDomain.CurrentDomain.BaseDirectory + @"History\" + _contactModel.ContactFileName);
            List<string> tempString=fileReadControl.fileOperator();
            if (tempString.Count>0)
            {
                return JsonSerializer.Deserialize<ObservableCollection<TextMessageDetailInfo>>(tempString[0]);
            }
            else
            {

                return _messages;
            }
            
        }

        protected void writeChatHistory(ObservableCollection<TextMessageDetailInfo> ChatHistoryCollection, TextMessageDetailInfo textMessage) 
        {
            ChatHistoryCollection.Add(textMessage);
            List<string> chatHistoryTemp = new List<string>();
            chatHistoryTemp.Add(JsonSerializer.Serialize(ChatHistoryCollection));
            IFileWriteControl fileWriteControl = new WriteTXTFile(AppDomain.CurrentDomain.BaseDirectory + @"History\"+ _contactModel.ContactFileName, chatHistoryTemp);
            fileWriteControl.writeFileContent();
        }

        protected string receiveMessage()
        {
            try
            {
                FTPHelperClass fTPHelper = new FTPHelperClass(_fTPHost.ftpHost, _fTPHost.ftpUsername, _fTPHost.ftpPassword, _fTPHost.ftpDirectory);
                return fTPHelper.fload(_fileName);
            }
            catch (System.Exception)
            {

                return "serverDown";
            }

        }

        protected string detectFTPServer() 
        {
            try
            {
                FTPHelperClass fTPHelper = new FTPHelperClass(_fTPHost.ftpHost, _fTPHost.ftpUsername, _fTPHost.ftpPassword, _fTPHost.ftpDirectory);
                return fTPHelper.detectFTPServer();
            }
            catch (System.Exception)
            {
                return "serverDown";
            }
        }

        protected string buildContactRelationFile(string[] tempString, string contact, int swtich)
        {
            string tempFileShortPath = "";
            if (swtich==0)
            {
                tempFileShortPath = tempString[0] + "+" + tempString[1] + "+" + contact + ".txt";
            }
            else
            {
                tempFileShortPath = contact + "+" + tempString[1] + "+" +MyProfileHelper.username + ".txt";
            }

            string tempFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Temp\" + tempFileShortPath;

            //for test content list
            List<string> tempList = new List<string>();
            //tempList.Add("Hello!");
            IFileWriteControl fileWriteControl = new WriteTXTFile(tempFilePath, tempList);
            fileWriteControl.writeFileContent();

            return tempFileShortPath;
        }

        protected ContactModel buildContactNameCard(ContactModel tempContactModel, string tempFileName, string contactName)
        {
            //string saveFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Contact\" + contactName + "+" + tempString[1] + ".txt";
            ContactModel contactNameCard = createContactObject(tempFileName, contactName, tempContactModel.ContactNameCardFileName);
            string contactModelString = JsonSerializer.Serialize(contactNameCard);

            List<string> ObjectContent = new List<string>();
            ObjectContent.Add(contactModelString);

            IFileWriteControl fileWriteControl = new WriteTXTFile(tempContactModel.ContactNameCardFileName, ObjectContent);
            fileWriteControl.writeFileContent();

            return contactNameCard;
        }


        protected void buildChatHistory(string[] tempString, string contact, int swtich)
        {
            string tempFilePath = "";
            if (swtich == 0)
            {
                tempFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\History\" + tempString[0] + "+" + tempString[1] + "+" + contact + ".txt";
            }
            else
            {
                tempFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\History\" + contact + "+" + tempString[1] + "+" + MyProfileHelper.username + ".txt";
            }


            //for test content list
            List<string> tempList = new List<string>();
            IFileWriteControl fileWriteControl = new WriteTXTFile(tempFilePath, tempList);
            fileWriteControl.writeFileContent();
        }

        protected ContactModel createContactObject(string tempFileName, string contactName, string saveFilePath)
        {
            IAddressSelecter addressSelecter = new IterationClass(_StartPoint, _EndPoint, _CurrentChannelNum);
            ContactModel contactModel = new ContactModel();
            contactModel.NextChannelNum = addressSelecter.getAddress();
            contactModel.ContactName = contactName;
            contactModel.ContactFileName = tempFileName;
            contactModel.ContactNameCardFileName = saveFilePath;

            return contactModel;
        }

        protected bool sendMessage(string fileName)
        {
            try
            {
                FTPHelperClass fTPHelper = new FTPHelperClass(_fTPHost.ftpHost, _fTPHost.ftpUsername, _fTPHost.ftpPassword, _fTPHost.ftpDirectory);
                 return fTPHelper.onload(fileName);
            }
            catch (System.Exception)
            {

                throw;
            }

        }

        protected void deleteContactTempFile(string fileNameDel)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\Temp\" + fileNameDel))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\Temp\" + fileNameDel);
            }
        }
        protected void deleteFTPServerTempFile(string fileNameDel)
        {
            FTPHelperClass fTPHelperClass = new FTPHelperClass(_fTPHost.ftpHost, _fTPHost.ftpUsername, _fTPHost.ftpPassword, _fTPHost.ftpDirectory);
            fTPHelperClass.DeleteFile(fileNameDel);
        }

    }
}
