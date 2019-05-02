using System;
using System.Collections.Generic;
using System.Text;

namespace AmiChecker
{
    public class Ec2InstanceImage
    {

        public string Region { get; set; }
        public string ImageId { get; set; }
        public string InstanceId { get; set; }
        public string ImageDescription { get; set; }
        public string ImageName { get; set; }

    }
}
