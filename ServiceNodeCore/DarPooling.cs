using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;

using System.Security.Cryptography;

using Communication;
using System.Threading;
using System.ServiceModel;

namespace ServiceNodeCore
{
    /// <summary>
    /// This class implements the interface of the Darpooling service
    /// </summary>
    public class DarPoolingService : IDarPooling
    {
        private static int userCounter;
        private static XDocument usersDB;
        private string samplePassw;
        private static string usersDBPath = @"..\..\..\config\users.xml";


        public DarPoolingService()
        {
            userCounter = -1;
        }

        public void SendCommand(Communication.Command command)
        {

        }

        public Communication.Result GetResult()
        {
            return new Result("");
        }

        public void GetData(User u)
        {
            Result res;
            Console.WriteLine("Satisfying Client Request...");
            if (CheckUser(u.UserName))
            {
                res = new Result("Ok, you can register");
            }
            else
            {
                res = new Result("Sorry, this username is already present!");
            }
            Thread.Sleep(2000);
            Console.WriteLine("Ready to send data back to Client...");
            OperationContext.Current.GetCallbackChannel<IDarPoolingCallback>().GetResult(res);
        }

        private bool CheckUser(string username)
        { 
            usersDB = XDocument.Load(usersDBPath);

            var baseQuery = (from u in usersDB.Descendants("User")
                             where u.Element("UserName").Value.Equals(username)
                             select u);
            if (baseQuery.Count() == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        
        }

        private static string EncodePassword(string password)
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            md5 = new MD5CryptoServiceProvider();

            originalBytes = ASCIIEncoding.Default.GetBytes(password);
            encodedBytes = md5.ComputeHash(originalBytes);
            string encodedPassword = System.Text.RegularExpressions.Regex.Replace(BitConverter.ToString(encodedBytes), "-", "").ToLower();
            return encodedPassword;
        }

        public static void SaveUser(User u)
        {
            usersDB = XDocument.Load(usersDBPath);

            userCounter++;
            u.UserID = userCounter;

            XElement newUser = new XElement("User",
                new XElement("UserID", u.UserID),
                new XElement("UserName", u.UserName),
                //new XElement("Password", EncodePassword("ciccio")),
                new XElement("Name", u.Name),
                new XElement("Sex", u.UserSex),
                new XElement("BirthDate", u.BirthDate),
                new XElement("Email", u.Email),
                new XElement("Smoker", u.Smoker),
                new XElement("SignupDate", u.SignupDate),
                new XElement("Whereabouts", u.Whereabouts)
                );
            usersDB.Element("Users").Add(newUser);
            usersDB.Save(usersDBPath);

            //Console.WriteLine("User Saved!");
        }





    }
}
