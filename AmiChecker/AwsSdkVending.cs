using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AmiChecker
{
    public class AwsSdkVending
    {
        public void Init()
        {

            Console.WriteLine("Enter in your access key ID:");
            AccessKeyId = Console.ReadLine();
            Console.WriteLine("Enter in your secret key:");
            SecretKey = Console.ReadLine();


            Console.Clear();
        }
        public static string AccessKeyId { get; set; }

        public static string SecretKey { get; set; }
    }
}
