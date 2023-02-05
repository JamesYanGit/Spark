using FileSystemModule.FileOperation;
using FileSystemModule.Interface;
using FTPHelper;
using Spark.Abstract;
using Spark.Interface;
using Spark.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;

namespace Spark.Controller.MessageController
{
    public class ReceiveContactController : AMessage, IReceiveMessage
    {
        List<string> _contactRequestList = new List<string>();
        private string _contactName;
        public ReceiveContactController()
        {
            _contactRequestList = getContactRequestList();  //get Contact request list from ContactRequstFolder
        }

        private List<string> getContactRequestList()
        {
            IFileReadControl fileReadControl = new ReadFileName(AppDomain.CurrentDomain.BaseDirectory + @"\ContactRequestFolder\");
            return fileReadControl.fileOperator();
        }
        public ObservableCollection<TextMessageDetailInfo> opreateInfo()
        {
            ObservableCollection<TextMessageDetailInfo> textMessages = new ObservableCollection<TextMessageDetailInfo>();
            try
            {
                foreach (string item in _contactRequestList)
                {
                    try
                    {
                        string[] tempString = item.Replace(".txt", "").Split('+');
                        FTPHostModel ftpInfo = getChannel(tempString);
                        string fileNameTemp = getReceiveFileName(tempString);
                        int currentChannelNum = Convert.ToInt32(tempString[2]);
                        connectFTPserver(ftpInfo, fileNameTemp, currentChannelNum); //get ftp file content receiveString                                                          
                        receiveString = receiveMessage();//Get info from server

                        ContactModel tempContact = JsonSerializer.Deserialize<ContactModel>(receiveString);

                        MessageBox.Show(tempContact.ContactName + " wants to add you to contact.");

                        string contactName = getContactName();
                        string tempFileName = buildContactRelationFile(tempString, contactName, 0);

                        tempContact.ContactNameCardFileName = AppDomain.CurrentDomain.BaseDirectory + @"\Contact\" + tempContact.ContactName + "+" + tempString[1] + ".txt";
                        tempContact.ContactFileName = tempFileName;
                        tempContact.NextChannelNum = currentChannelNum;
                        //add textMessage to collection
                        textMessages.Add(exchangeContactModelToTextMessage(tempContact).MessageDetail[0]);


                        ContactModel completeContact=buildContactNameCard(tempContact, tempFileName, contactName);

                        //Build chat history file 
                        buildChatHistory(tempString, contactName,0);

                        //Delete local temp contact file

                        deleteContactTempFile(fileNameTemp);
                        //Delete FTP server temp contact file
                        deleteFTPServerTempFile(fileNameTemp);

                        //send confirm message
                        string tempConfirmMessage="<"+MyProfileHelper.username+"> has added you as contact.";
                        SendMessageController sendMessage = new SendMessageController(completeContact, tempConfirmMessage);
                        sendMessage.opreateInfo();

                        Thread.Sleep(100000);
                    }
                    catch (Exception)
                    {
                        continue;
                    }


                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
             return textMessages;
        }

        private TextMessageModel exchangeContactModelToTextMessage(ContactModel receiveMessage) 
        {
            
            return getNewTextMessageModel(receiveMessage.ContactName, "");
        }
        private FTPHostModel getChannel(string[] tempString)
        {

            return FTPinfo.ftpCollection[Convert.ToInt32(tempString[2])];
        }

        private string getReceiveFileName(string[] tempString)
        {
            return tempString[0] + "+" + tempString[1] + "+" + "ContactRequest.txt";
        }

        
        private string getContactName()
        {
            ContactModel contactString = JsonSerializer.Deserialize<ContactModel>(receiveString);
            return contactString.ContactName;
        }
        
    }
}
