using FileSystemModule.FileOperation;
using FileSystemModule.Interface;
using Spark.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;

namespace Spark
{
    public delegate void DelegateChangeTextVal();
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public event DelegateChangeTextVal ChangeTextVal;
        private string _Username;
        private string _Password;
        public Login()
        {
            InitializeComponent();
            this.Closing += Window_Closing;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ChangeTextVal();
        }


        private void checkMyProfileExist()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"MyProfile\" + _Username + ".txt"))
            {
                using (FileStream fs = File.Create(AppDomain.CurrentDomain.BaseDirectory + @"MyProfile\" + _Username + ".txt"));
            }
        }

        private string serializarProfile()
        {
            UserProfileModel userProfileModel = new UserProfileModel();
            userProfileModel.username = _Username;
            userProfileModel.password = _Password;
            return JsonSerializer.Serialize(userProfileModel);
        }

        private void saveMyProfile(List<string> list)
        {
            IFileWriteControl fileWriteControl = new WriteTXTFile(AppDomain.CurrentDomain.BaseDirectory + @"MyProfile\" + _Username + ".txt", list);
            fileWriteControl.writeFileContent();
        }
        private void saveMyProfileInVariable()
        {
            MyProfileHelper.username = _Username;
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            _Username = username.Text;
            _Password = password.Password;

            checkMyProfileExist();

            List<string> list = new List<string>();
            list.Add(serializarProfile());

            saveMyProfile(list);

            saveMyProfileInVariable();

            this.Close();
        }
    }
}
