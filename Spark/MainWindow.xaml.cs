using AddressSelecter;
using AddressSelecter.Interface;
using FileSystemModule.FileOperation;
using FileSystemModule.Interface;
using Spark.Controller.Login;
using Spark.Controller.MessageController;
using Spark.Interface;
using Spark.Model;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Windows;
using TimeAdjuster.Adjuster;
using TimeAdjuster.Interface;

namespace Spark
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> files = new List<string>();
        Dictionary<int, FTPHostModel> _ftpCollection = new Dictionary<int, FTPHostModel>();

        ObservableCollection<ContactModel> contactModels = new ObservableCollection<ContactModel>();

        ContactModel sendMessageContactModel = new ContactModel();
        MessageUserControl messageUser = new MessageUserControl();

        int sleepTimespan = 20000;

        

        public int ChannelNum { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            checkFolders();

            //login to system
            checkLogin();

            //initialzing FTPinfo
            FTPinfo.addFTPInfo();
            _ftpCollection = FTPinfo.ftpCollection;

            //run a new thread for change channel
            Thread channelThread = new Thread(changeChannel);
            channelThread.Start();

            //run a new thread for listening message
            Thread t = new Thread(listeningMessage);
            t.Start();

            //run a new thread for listening message
            Thread contactThread = new Thread(listeningContact);
            contactThread.Start();

            readingContact();

           

            //config send and chat history pannel
            configuration();

            this.Closing += Window_Closing;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void checkLogin()
        {
            ILogin login = new LoginByJSON();
            if (!login.isLogin())
            {
                this.Hide();
                Login login1 = new Login();
                login1.ChangeTextVal += new DelegateChangeTextVal(callBack);
                login1.ShowDialog();
            }
        }

        private void callBack()
        {
            this.Visibility = Visibility.Visible;
        }

        private void changeChannel()
        {
            int sleepTime;
            const int SLEEPFREQUENCE= 20;
            
            while (true)
            {
                IFileReadControl fileReadControl = new ReadFileName(AppDomain.CurrentDomain.BaseDirectory + @"Contact");
                List<string> files = fileReadControl.fileOperator();

                //int tsec=0;

                IAdjuster ta = new WorldTimeAdjuester();
                int tm = Convert.ToInt32(ta.getMins());

                int times = tm / SLEEPFREQUENCE;
                if (times==0)
                {
                    times = 1;
                }

                if (tm - SLEEPFREQUENCE > 0)
                {
                    sleepTime = SLEEPFREQUENCE - Math.Abs(tm - SLEEPFREQUENCE * times);
                }
                else if (tm - SLEEPFREQUENCE < 0)
                {
                    sleepTime = SLEEPFREQUENCE - tm;
                }
                else
                {
                    sleepTime = SLEEPFREQUENCE;
                }
                int tp = Convert.ToInt32(tm.ToString().Substring(0, 1));

                foreach (var item in files)
                {
                    try
                    {
                        fileReadControl = new ReadTXTFile(AppDomain.CurrentDomain.BaseDirectory + @"Contact\" + item);
                        List<string> fileContent = fileReadControl.fileOperator();
                        string tempSpeaker = item.Split('+')[0];
                        ContactModel contactObject = JsonSerializer.Deserialize<ContactModel>(fileContent[0]);

                        //DateTime startTime = System.DateTime.Now;
                        //adjust channel according to the clock
                        IAddressSelecter addressSelecter = new IterationClass(1, 5, tp);//next channel
                        contactObject.NextChannelNum=addressSelecter.getAddress();
                        //DateTime endTime = System.DateTime.Now;
                        //TimeSpan ts = endTime - startTime;
                        //tsec=Convert.ToInt16(ts.TotalSeconds);

                        //write into contact namecard
                        buildContactNameCard(contactObject);

                        //automatic change channel
                        tp = contactObject.NextChannelNum;
                    } 
                    catch (Exception)
                    {

                    }
                }
                Dispatcher.Invoke(() =>
                {
                    getMessageTime.Text = tm.ToString();
                    SleepTime.Text = sleepTime.ToString();
                });
                Thread.Sleep(sleepTime * 1000); //according to how much time left
            } 
        }

        //Update Channel number
        protected void buildContactNameCard(ContactModel tempContactModel)
        {
            string contactModelString = JsonSerializer.Serialize(tempContactModel);

            List<string> ObjectContent = new List<string>();
            ObjectContent.Add(contactModelString);

            IFileWriteControl fileWriteControl = new WriteTXTFile(tempContactModel.ContactNameCardFileName, ObjectContent);
            fileWriteControl.writeFileContent();

        }

        private void readingContact() 
        {
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"Contact");
            if (files.Length == 0)
            {
                ContactList.ItemsSource = contactModels;
            }
            else
            {
                contactModels.Clear();
                foreach (var item in files)
                {
                    IFileReadControl fileReadControl = new ReadTXTFile(item);
                    contactModels.Add(JsonSerializer.Deserialize<ContactModel>(fileReadControl.fileOperator()[0]));
                }
                ContactList.ItemsSource = contactModels;
            }
            //Thread.Sleep(sleepTimespan);
        }
        private void listeningMessage()
        {
            while (true)
            {
                IFileReadControl fileReadControl = new ReadFileName(AppDomain.CurrentDomain.BaseDirectory+@"Contact");
                List<string> files =fileReadControl.fileOperator();
                foreach (var item in files)
                {
                    try
                    {
                        fileReadControl = new ReadTXTFile(AppDomain.CurrentDomain.BaseDirectory + @"Contact\" + item);
                        List<string> fileContent = fileReadControl.fileOperator();
                        string tempSpeaker = item.Split('+')[0];
                        ContactModel contactObject = JsonSerializer.Deserialize<ContactModel>(fileContent[0]);

                        IReceiveMessage receiveMessage = new ReceiveMessageController(contactObject);
                        ObservableCollection<TextMessageDetailInfo> TextMessage = receiveMessage.opreateInfo();

                        if (TextMessage != null && TextMessage.Count != 0)
                        {
                            if (TextMessage[TextMessage.Count-1].Speaker == sendMessageContactModel.ContactName)
                            {
                                //messageUser.bindingChatHistory();
                                Dispatcher.Invoke(() =>
                                {
                                    //MessageBox.Show("Update UI!");
                                    configuration();
                                    ChannelNumberText.Text = contactObject.NextChannelNum.ToString();
                                });
                                
                            }

                        }
                        
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                       // throw;
                    }
                    Thread.Sleep(sleepTimespan);
                }
            }
            ////List<string> files = new List<string>();
            //while (true)
            //{
            //    FTPHelperClass fTPHelper = new FTPHelperClass("ftpupload.net", "epiz_33000147", "uw1MEHA2Vo", @"/htdocs/");
            //    string receiveString = fTPHelper.fload("1.txt");
            //    try
            //    {
            //        TextMessageModel tempString = JsonSerializer.Deserialize<TextMessageModel>(receiveString);
            //        IFileReadControl fileReadControl = new ReadTXTFile(AppDomain.CurrentDomain.BaseDirectory + "Temp\\1.txt");
            //        List<string> strings = fileReadControl.fileOperator();

            //        if (strings.Count != 0)
            //        {
            //            TextMessageModel fileString = JsonSerializer.Deserialize<TextMessageModel>(strings[0]);
            //            if (tempString.Datetime != fileString.Datetime)
            //            {
            //                writeMessage(receiveString);
            //            }

            //        }
            //        else
            //        {
            //            writeMessage(receiveString);
            //        }

            //        Thread.Sleep(sleepTimespan);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.ToString());
            //    }
            //}


        }

        private void listeningContact()
        {
            while (true)
            {
                IReceiveMessage receive = new ReceiveContactController();
                receive.opreateInfo();
                Dispatcher.Invoke(() =>
                    {
                        readingContact();
                    });
                
                Thread.Sleep(sleepTimespan);
            }
        }

        private void chooseContact_DoubleClick(object sender, RoutedEventArgs e)
        {
            ContactModel rowSelected = ContactList.SelectedItem as ContactModel;
            if (rowSelected != null)
            {
                sendMessageContactModel = rowSelected;
                configuration(); //refresh send pannel 
            }
        }

        private void configuration()
        {
            messagePanel.Children.Clear();
            messageUser.setContactModel(sendMessageContactModel);
            messageUser.bindingChatHistory();
            messagePanel.Children.Add(messageUser);
        }

        //private void writeMessage(string receiveString)
        //{
        //    files.Clear();
        //    files.Add(receiveString);
        //    IFileWriteControl fileWriteControl = new WriteTXTFile(AppDomain.CurrentDomain.BaseDirectory + "Temp\\1.txt", files);
        //    fileWriteControl.writeFileContent();
        //    sleepTimespan = 10000;

        //    IFileReadControl fileReadControl1 = new ReadTXTFile(AppDomain.CurrentDomain.BaseDirectory + "Temp\\1.txt");
        //    Dispatcher.Invoke(() =>
        //    {
        //        ReceiveText.Text = JsonSerializer.Deserialize<TextMessageModel>(fileReadControl1.fileOperator()[0]).TextContent;
        //    });
        //}

        private void myProfile_Click(object sender, RoutedEventArgs e)
        {
            myProfile profile = new myProfile();
            profile.Owner = this;
            profile.ShowDialog();
            ChannelNumberText.Text = ChannelNum.ToString();
        }

        private void checkFolders()
        {
            string directoryName = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + @"ContactRequestFolder\");
            // If path is a file name only, directory name will be an empty string
            if (directoryName.Length > 0)
            {
                // Create all directories on the path that don't already exist
                Directory.CreateDirectory(directoryName);
            }

            string directoryName1 = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + @"Contact\");
            // If path is a file name only, directory name will be an empty string
            if (directoryName1.Length > 0)
            {
                // Create all directories on the path that don't already exist
                Directory.CreateDirectory(directoryName1);
            }

            string directoryName2 = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + @"MyProfile\");
            // If path is a file name only, directory name will be an empty string
            if (directoryName2.Length > 0)
            {
                // Create all directories on the path that don't already exist
                Directory.CreateDirectory(directoryName2);
            }

            string directoryName3 = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory + @"History\");
            // If path is a file name only, directory name will be an empty string
            if (directoryName3.Length > 0)
            {
                // Create all directories on the path that don't already exist
                Directory.CreateDirectory(directoryName3);
            }
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            AddContact newContact = new AddContact();
            newContact.ShowDialog();
        }

        //private void button_Click(object sender, RoutedEventArgs e)
        //{
        //    files.Clear();

        //    TextMessageModel textMessageModel = new TextMessageModel();
        //    textMessageModel.TextContent = SendText.Text.Trim();
        //    textMessageModel.Datetime = DateTime.Now.ToFileTimeUtc().ToString();
        //    string tempJson = JsonSerializer.Serialize(textMessageModel);
        //    files.Add(tempJson);
        //    IFileWriteControl fileWriteControl = new WriteTXTFile(AppDomain.CurrentDomain.BaseDirectory + "Temp\\1.txt", files);
        //    fileWriteControl.writeFileContent();

        //    FTPHelperClass fTPHelper = new FTPHelperClass("ftpupload.net", "epiz_33000147", "uw1MEHA2Vo", @"/htdocs/");
        //    fTPHelper.onload(AppDomain.CurrentDomain.BaseDirectory + "Temp\\1.txt");

        //}

        
    }
}
