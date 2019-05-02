using Amazon.EC2.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AmiChecker
{
    public class EC2Process
    {
        static string[] DeprecatedDescriptions = new string[] {
            @"^.+Windows Server\s\w*\s*2008.+$",
            @"^.+SQL\s\w*\s*Server\s\w*\s*2008.+$",
            @"^.+SQL\s\w*\s*Server\s\w*\s*2005.+$",
            @"^.+Windows Server\s\w*\s*2003.+$",
        };
        public async Task<IEnumerable<Ec2InstanceImage>> GetDeprecatedInstanceIds(Amazon.RegionEndpoint currentRegion)
        {

            Amazon.EC2.AmazonEC2Client ec2Client = new Amazon.EC2.AmazonEC2Client(AwsSdkVending.AccessKeyId, AwsSdkVending.SecretKey, currentRegion);


            string NextToken = null;

            List<Ec2InstanceImage> lstInstances = new List<Ec2InstanceImage>();

            List<string> lstImageIds = new List<string>();

            List<Image> lstAffectedImages = new List<Image>();

            do
            {
                var describeRequest = new DescribeInstancesRequest();
                describeRequest.Filters = new List<Filter>();
                describeRequest.Filters.Add(new Filter("platform", new List<string> { "windows" }));
                describeRequest.Filters.Add(new Filter("instance-state-name", new List<string> { "pending", "running", "shutting-down", "stopping", "stopped" }));
                describeRequest.NextToken = NextToken;

                var describeInstancesResult = await ec2Client.DescribeInstancesAsync(describeRequest);

                NextToken = describeRequest.NextToken;


                foreach (var instance in describeInstancesResult.Reservations.SelectMany(r => r.Instances))
                {
                    if (!lstImageIds.Contains(instance.ImageId))
                        lstImageIds.Add(instance.ImageId);

                }


            } while (NextToken != null);

            if (lstImageIds.Count > 0)
            {

                var describeImagesRequest = new DescribeImagesRequest { ImageIds = lstImageIds };
                describeImagesRequest.Filters = new List<Filter>();
                describeImagesRequest.Filters.Add(new Filter { Name = "is-public", Values = new List<string> { "true" } });
                var describeImagesResult = await ec2Client.DescribeImagesAsync(describeImagesRequest);

                foreach (var currentImage in describeImagesResult.Images)
                {
                    if (string.IsNullOrEmpty(currentImage.Description))
                        continue;

                    if (DeprecatedDescriptions.Any(d => Regex.IsMatch(currentImage.Description, d, RegexOptions.IgnoreCase)))
                    {
                        lstAffectedImages.Add(currentImage);
                        Console.WriteLine($"{currentImage.ImageId} - {currentImage.Description} is facing deprecation");

                    }
                }


                Console.WriteLine($"{lstAffectedImages.Count} images are marked for deprecation in {currentRegion.DisplayName}");
            }

            if (lstAffectedImages.Count > 0)
            {
                foreach (var image in lstAffectedImages)
                {
                    do
                    {
                        var describeInstancesRequest = new DescribeInstancesRequest();

                        describeInstancesRequest.Filters = new List<Filter>();
                        describeInstancesRequest.Filters.Add(new Filter("image-id", new List<string> { image.ImageId }));
                        describeInstancesRequest.Filters.Add(new Filter("instance-state-name", new List<string> { "pending", "running", "shutting-down", "stopping", "stopped" }));
                        describeInstancesRequest.NextToken = NextToken;

                        var describeInstancesResult = await ec2Client.DescribeInstancesAsync(describeInstancesRequest);

                        NextToken = describeInstancesRequest.NextToken;


                        foreach (var instance in describeInstancesResult.Reservations.SelectMany(r => r.Instances))
                        {
                            lstInstances.Add(new Ec2InstanceImage
                            {
                                Region = currentRegion.DisplayName,
                                ImageDescription = image.Description,
                                ImageId = image.ImageId,
                                ImageName = image.Name,
                                InstanceId = instance.InstanceId
                            });

                            Console.WriteLine($"Instance {instance.InstanceId} in {currentRegion.DisplayName} ({image.Name}) is marked for deprecation and needs to be snapshotted.");

                        }



                    } while (NextToken != null);
                }
            }

            return lstInstances;
        }
    }
}
