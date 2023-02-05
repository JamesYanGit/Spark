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
            ftp.ftpHost = "ftpupload.net";
            ftp.ftpPort = 21;
            ftp.ftpUsername = "epiz_33000147";
            ftp.ftpPassword = "uw1MEHA2Vo";
            ftp.ftpDirectory = @"/htdocs/";
            ftpCollection.Add(1, ftp);

            FTPHostModel ftp1 = new FTPHostModel();
            ftp1.ftpHost = "sandplan2.6te.net";
            ftp1.ftpPort = 21;
            ftp1.ftpUsername = "sandplan2.6te.net";
            ftp1.ftpPassword = "wuxianjingzheng8";
            ftp1.ftpDirectory = @"/";
            ftpCollection.Add(2, ftp1);

            FTPHostModel ftp2 = new FTPHostModel();
            ftp2.ftpHost = "sandplan4.eu5.org";
            ftp2.ftpPort = 21;
            ftp2.ftpUsername = "sandplan4.eu5.org";
            ftp2.ftpPassword = "wuxianjingzheng8";
            ftp2.ftpDirectory = @"/";
            ftpCollection.Add(3, ftp2);

            FTPHostModel ftp3 = new FTPHostModel();
            ftp3.ftpHost = "files.000webhost.com";
            ftp3.ftpPort = 21;
            ftp3.ftpUsername = "sandplan";
            ftp3.ftpPassword = "VfRjC2ZBtO@!oFBzNQwl";
            ftp3.ftpDirectory = @"/";
            ftpCollection.Add(4, ftp3);

            FTPHostModel ftp4 = new FTPHostModel();
            ftp4.ftpHost = "f30-preview.awardspace.net";
            ftp4.ftpPort = 21;
            ftp4.ftpUsername = "4220030";
            ftp4.ftpPassword = "isusNF4v9:I[9[DC";
            ftp4.ftpDirectory = @"/sandplan.com/";
            ftpCollection.Add(5, ftp4);
        } 

    }

}
