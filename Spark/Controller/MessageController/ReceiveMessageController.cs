using AddressSelecter;
using AddressSelecter.Interface;
using FileSystemModule.FileOperation;
using FileSystemModule.Interface;
using Spark.Abstract;
using Spark.Interface;
using Spark.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Spark.Controller.MessageController
{
    public class ReceiveMessageController : AMessage, IReceiveMessage
    {
        private ContactModel _lastestContactModel;

        private string _hashcode;
        public ReceiveMessageController(ContactModel lastestContactModel) 
        {
            _lastestContactModel=lastestContactModel;
        }
        public ObservableCollection<TextMessageDetailInfo> opreateInfo()
        {
            ////1. read channel from contact file
            //ContactModel lastestContactModel = readContactInfo(AppDomain.CurrentDomain.BaseDirectory + @"Contact\" + _contactModel.ContactNameCardFileName);
            //2. connect to the FTP server based on the channel number
            connectFTPserver(FTPinfo.ftpCollection[_lastestContactModel.NextChannelNum], _lastestContactModel.ContactFileName, _lastestContactModel.NextChannelNum);
            
            //4. read chat history
            ObservableCollection<TextMessageDetailInfo> tempHistory = readChatHistory(_lastestContactModel);

            //if (detectFTPServer()!= "serverDown")
            //{
                //3. receive message
                string receiveTextString = receiveMessage();
                //5. Add receive message to history
                if (receiveTextString != "" && receiveTextString != null)
                {
                    //change recevie message to model
                    TextMessageModel textMessageModel = JsonSerializer.Deserialize<TextMessageModel>(receiveTextString);
                    //verify data integrity
                    if (verifyDataIntegrity(textMessageModel))
                    {
                        if (verifyNewMessage(textMessageModel))
                        {
                            if (textMessageModel.MessageDetail[0].Speaker != MyProfileHelper.username)
                            {
                                if (textMessageModel.ReceiveStatus == "" || textMessageModel.ReceiveStatus == null)
                                {

                                    foreach (TextMessageDetailInfo item in textMessageModel.MessageDetail)
                                    {
                                        if (item.Speaker != MyProfileHelper.username&&item.TextContent!="")
                                        {
                                            //write chat into history file
                                            writeChatHistory(tempHistory, item);
                                        }

                                    }

                                    sendSuccessReceiveMsg();
                                }
                                else if (textMessageModel.ReceiveStatus == "success")
                                {
                                    //delete file on server
                                    deleteFTPServerTempFile(_lastestContactModel.ContactFileName);

                                    //clear temp file
                                    List<string> tempList = new List<string>();
                                    IFileWriteControl fileWriteControl = new WriteTXTFile(AppDomain.CurrentDomain.BaseDirectory + @"Temp\" + _lastestContactModel.ContactFileName, tempList);
                                    fileWriteControl.writeFileContent();

                                    //change channel
                                    IAddressSelecter addressSelecter = new IterationClass(1, 5, _lastestContactModel.NextChannelNum);
                                    _lastestContactModel.NextChannelNum = addressSelecter.getAddress();

                                    //write into contact namecard
                                    buildContactNameCard(_lastestContactModel, _lastestContactModel.ContactFileName, _lastestContactModel.ContactName);
                                }
                                else if (textMessageModel.ReceiveStatus == "failure")
                                {
                                    reSendMsg();
                                }

                            }
                        }
                        

                    }
                    else
                    {
                        sendFailureReceiveMsg();
                    }

                }
            //}
            //else
            //{
            //    //change channel
            //    IAddressSelecter addressSelecter = new IterationClass(1, 5, _lastestContactModel.NextChannelNum);
            //    _lastestContactModel.NextChannelNum = addressSelecter.getAddress();

            //    //write into contact namecard
            //    buildContactNameCard(_lastestContactModel, _lastestContactModel.ContactFileName, _lastestContactModel.ContactName);
            //}

            return tempHistory;
        }

        private bool verifyDataIntegrity(TextMessageModel tempString) 
        {
            string tempContent="";
            
            foreach (TextMessageDetailInfo item in tempString.MessageDetail)
            {
                tempContent += item.TextContent + item.Speaker + item.Datetime;
            }

            if (getHashCode(tempContent)== tempString.HashCode)
            {
                return true;
            }
            return false;
        }

        private bool verifyNewMessage(TextMessageModel tempString) 
        {
            if (tempString.HashCode!= _hashcode)
            {
                //It is new code
                _hashcode = tempString.HashCode;
                return true;

            }
            return false;
        }

        private void sendSuccessReceiveMsg() 
        {
            TextMessageModel newText = getNewTextMessageModel(MyProfileHelper.username, "");
            newText.ReceiveStatus = "success";

            ISendMessage sendMessage = new SendMessageController(_lastestContactModel, "");
            sendMessage.sendBackMsg(newText);
        }

        private void sendFailureReceiveMsg()
        {
            TextMessageModel newText = getNewTextMessageModel(MyProfileHelper.username, "");
            newText.ReceiveStatus = "failure";

            ISendMessage sendMessage = new SendMessageController(_lastestContactModel, "");
            sendMessage.sendBackMsg(newText);
        }

        private void reSendMsg() 
        {
            ISendMessage sendMessage = new SendMessageController(_lastestContactModel, "");
            sendMessage.reSendMsg();
        }

    }
}
