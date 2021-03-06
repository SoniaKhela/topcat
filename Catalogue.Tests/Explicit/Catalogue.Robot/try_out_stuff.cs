﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Catalogue.Data.Write;
using Catalogue.Robot.Publishing.OpenData;
using FluentAssertions;
using NUnit.Framework;

namespace Catalogue.Tests.Explicit.Catalogue.Robot
{
    class try_out_stuff
    {
        [Explicit, Test]
        public void try_ftp_client()
        {
            string password = "password";
            var c = new FtpClient("topcat", password);

            string s = c.DownloadString("ftp://data.jncc.gov.uk/waf/index.html");
            Console.WriteLine(s);

            string filePath = @"C:\Work\test-data.csv";
            File.Exists(filePath).Should().BeTrue();
            c.UploadFile("ftp://data.jncc.gov.uk/" + new FileInfo(filePath).Name, filePath);
        }
    }
}
