using Amazon;
using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmiChecker
{
    class Program
    {

        static void Main(string[] args)
        {
            new AwsSdkVending().Init();

            if (string.IsNullOrEmpty(AwsSdkVending.AccessKeyId)
                || string.IsNullOrEmpty(AwsSdkVending.SecretKey))
            {
                Console.WriteLine("AWS credentials not provided.");
                return;
            }


            List<Ec2InstanceImage> lst = new List<Ec2InstanceImage>();
            Parallel.ForEach(GetRegions(), (region) =>
           {
               var instances = new EC2Process().GetDeprecatedInstanceIds(region).Result;
               if (instances.Count() > 0)
                   lst.AddRange(instances);

           });

            if (lst.Count > 0)
            {
                var strFilePath = new CsvWrite().WriteToCsv(lst);
                Console.WriteLine($"EC2 instance list written to {strFilePath}");
            }
            else
            {
                Console.WriteLine("There were no EC2 instances found that were marked for deprecation. All clear!");
            }

            Console.WriteLine("Process completed.");
        }

        static Amazon.RegionEndpoint[] GetRegions()
        {
            return RegionEndpoint.EnumerableAllRegions.Where(r => r != RegionEndpoint.CNNorth1 &&
            r != RegionEndpoint.CNNorthWest1 && r != RegionEndpoint.USGovCloudEast1 &&
            r != RegionEndpoint.USGovCloudWest1).ToArray();

        }
    }
}
