using FileSystemModule.FileOperation;
using FileSystemModule.Interface;
using FTPHelper;
using Spark.Abstract;
using Spark.Model;
using System.Collections.Generic;
using System;
using System.Text.Json;

namespace Spark.Controller.ContactController
{
    public class ContactInfoController:AMessage
    {
        private string _tempContactID;
        public ContactInfoController(string contactDetail) 
        {
            _tempContactID = contactDetail;
        }
        public void operateInfo() 
        {
            string[] contactDetail = _tempContactID.Split('+');
            int channelNumber = Convert.ToInt32(contactDetail[2]);
            string contactRequestFileName = contactDetail[0] + "+" + contactDetail[1] + "+ContactRequest.txt";
            

            List<string> myName = new List<string>();
            myName.Add(JsonSerializer.Serialize(getContactObject(MyProfileHelper.username)));

            IFileWriteControl fileWriteControl = new WriteTXTFile(AppDomain.CurrentDomain.BaseDirectory + @"Temp\" + contactRequestFileName, myName);
            fileWriteControl.writeFileContent();

            connectFTPserver(FTPinfo.ftpCollection[channelNumber], contactRequestFileName, channelNumber);

            sendMessage(AppDomain.CurrentDomain.BaseDirectory + @"Temp\" + contactRequestFileName);

            //string saveFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Contact\" + contactName + "+" + tempString[1] + ".txt";

            string tempFileName = buildContactRelationFile(contactDetail, contactDetail[0],1);

            ContactModel contactModel = getContactObject(contactDetail[0]);
            contactModel.ContactNameCardFileName = AppDomain.CurrentDomain.BaseDirectory + @"\Contact\" + contactDetail[0] + "+" + contactDetail[1] + ".txt";
            contactModel.ContactFileName = tempFileName;
            contactModel.NextChannelNum = channelNumber;
            ContactModel completeContact = buildContactNameCard(contactModel, tempFileName, contactDetail[0]);
            //Build chat history file 
            buildChatHistory(contactDetail, contactDetail[0],1);
        }

        private ContactModel getContactObject(string contactName)
        {
            ContactModel contactModel = new ContactModel();
            contactModel.ContactName = contactName;
            return contactModel;
        }
    }
}
