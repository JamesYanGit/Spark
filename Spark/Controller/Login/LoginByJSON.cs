using FileSystemModule.FileOperation;
using FileSystemModule.Interface;
using Spark.Interface;
using Spark.Model;
using System;
using System.IO;
using System.Text.Json;

namespace Spark.Controller.Login
{
    public class LoginByJSON : ILogin
    {
        public bool isLogin()
        {
            if (MyProfileHelper.username!=null)
            {
                return true;
            }
            else
            {
                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"MyProfile");
                if (files.Length==0)
                {
                    return false;
                }
                else
                {
                    IFileReadControl fileReadControl = new ReadTXTFile(files[0]);
                    ;
                    MyProfileHelper.username = JsonSerializer.Deserialize<UserProfileModel>(fileReadControl.fileOperator()[0]).username;
                    return true;
                }
            }
        }

        public void Login(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
