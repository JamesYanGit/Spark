using Spark.Model;
using System.Collections.Generic;

namespace Spark
{
    public static class FTPinfo
    {
        public static Dictionary<int, FTPHostModel> ftpCollection = new Dictionary<int, FTPHostModel>();

        public static void addFTPInfo() 
        {
            FTPHostModel ftp = new FTPHostModel();
            ftp.ftpHost = "";
            ftp.ftpPort = 00;
            ftp.ftpUsername = "";
            ftp.ftpPassword = "";
            ftp.ftpDirectory = @"";
            ftpCollection.Add(1, ftp);

            
        } 

    }

}
