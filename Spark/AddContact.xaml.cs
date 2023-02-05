using FileSystemModule.FileOperation;
using FileSystemModule.Interface;
using FTPHelper;
using Spark.Controller.ContactController;
using Spark.Model;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;

namespace Spark
{
    /// <summary>
    /// Interaction logic for AddContact.xaml
    /// </summary>
    public partial class AddContact : Window
    {
        public AddContact()
        {
            InitializeComponent();
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            string tempContactID = ContactID.Text.Trim();

            
            if (tempContactID != "")
            {
                ContactInfoController contactInfoController = new ContactInfoController(tempContactID);
                contactInfoController.operateInfo();
            }
        }


        
    }
}
