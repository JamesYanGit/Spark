using AddressSelecter;
using AddressSelecter.Interface;
using FileSystemModule.FileOperation;
using FileSystemModule.Interface;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace Spark
{
    /// <summary>
    /// Interaction logic for myProfile.xaml
    /// </summary>
    public partial class myProfile : Window
    {
        int selectedAddress = 0;
        List<string> files = new List<string>();
        public myProfile()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
            nickname.Text = MyProfileHelper.username;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)this.Owner;
            mainWindow.ChannelNum = selectedAddress;
        }

        private void getMyContactID_Click(object sender, RoutedEventArgs e)
        {
            bool chooseSever = true;
            //while (true)
            //{
                //test random address
                IAddressSelecter addressSelecter = new RandomClass(MyProfileHelper.startSeverPoint, MyProfileHelper.endSeverPoint);  //range of the addresses
                selectedAddress = addressSelecter.getAddress();
                ChannelNumber.Text = selectedAddress.ToString();
                getMyContactID.IsEnabled = false;
            //}
           
            myContactID.Text = nickname.Text + "+" + DateTimeOffset.Now.ToUnixTimeSeconds() + "+" + selectedAddress.ToString();
            writeContactRequestFile(myContactID.Text.Trim());
        }

        private void writeContactRequestFile(string fileName) 
        {
            IFileWriteControl fileWriteControl = new WriteTXTFile(AppDomain.CurrentDomain.BaseDirectory + @"ContactRequestFolder\" + fileName+@".txt", files);
            fileWriteControl.writeFileContent();
        }
    }
}
