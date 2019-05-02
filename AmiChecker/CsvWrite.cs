using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace AmiChecker
{
    public class CsvWrite
    {
        public string WriteToCsv(IEnumerable<Ec2InstanceImage> lst)
        {
            var currentDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            var strFilePath = Path.Combine(currentDirectory, DateTime.Now.ToString("ddMMyyyy-HHmmss") + "-instances.csv");
            using (var writer = new StreamWriter(strFilePath))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(lst);

            }
            return strFilePath;

        }
    }
}
