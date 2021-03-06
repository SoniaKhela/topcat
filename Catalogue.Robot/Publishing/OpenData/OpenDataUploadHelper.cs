﻿using System;
using System.IO;
using System.Linq;
using Catalogue.Data.Model;
using Catalogue.Data.Write;
using Catalogue.Utilities.DriveMapping;
using Catalogue.Utilities.Logging;
using Catalogue.Utilities.Text;
using log4net;

namespace Catalogue.Robot.Publishing.OpenData
{
    public class OpenDataUploadHelper : IOpenDataUploadHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OpenDataUploadHelper));

        private readonly OpenDataUploadConfig config;
        private readonly IFtpClient ftpClient;
        private readonly IOpenDataXmlHelper xmlHelper;

        public OpenDataUploadHelper(OpenDataUploadConfig config)
        {
            this.config = config;
            ftpClient = new FtpClient(config.FtpUsername, config.FtpPassword);
            xmlHelper = new OpenDataXmlHelper();
        }

        public void UploadDataFile(Guid recordId, string filePath)
        {
            filePath = JnccDriveMappings.GetUncPath(filePath);

            string unrootedDataPath = WebificationUtility.GetUnrootedDataPath(recordId, filePath);
        
            string dataFtpPath = config.FtpRootUrl + "/" + unrootedDataPath;
            Logger.Info("Data file path: "+filePath);
            Logger.Info("Data FTP path: "+dataFtpPath);
        
            ftpClient.UploadFile(dataFtpPath, filePath);
            Logger.Info("Uploaded data file successfully");
        }

        public void UploadAlternativeResources(Record record)
        {
            // check no duplicate filenames after webifying
            var fileNames = from r in record.Publication.OpenData.Resources
                let fileName = WebificationUtility.ToUrlFriendlyString(Path.GetFileName(r.Path))
                group r by fileName;
            if (fileNames.Count() != record.Publication.OpenData.Resources.Count)
            {
                var e = new Exception("There are duplicate resource file names (after webifying) for this record.");
                e.LogAndThrow(Logger);
            }

            // upload the resources
            foreach (var r in record.Publication.OpenData.Resources)
            {
                UploadDataFile(record.Id, r.Path);
            }
        }

        public void UploadMetadataDocument(Record record)
        {
            string resourceUrl = config.HttpRootUrl + "/" + WebificationUtility.GetUnrootedDataPath(record.Id, record.Path);
            var metaXmlDoc = xmlHelper.GetMetadataDocument(record, resourceUrl);
            string metaPath = String.Format("waf/{0}.xml", record.Id);
            string metaFtpPath = config.FtpRootUrl + "/" + metaPath;

            Logger.Info("Metadata file path: " + metaPath);
            Logger.Info("Metadata FTP path: " + metaFtpPath);

            ftpClient.UploadBytes(metaFtpPath, metaXmlDoc);
            Logger.Info("Uploaded metadata document successfully");
        }

        public void UploadWafIndexDocument(Record record)
        {
            string indexDocFtpPath = String.Format("{0}/waf/index.html", config.FtpRootUrl);
            string indexDocHtml = ftpClient.DownloadString(indexDocFtpPath);
            string updatedIndexDoc = xmlHelper.UpdateWafIndexDocument(record, indexDocHtml);
            ftpClient.UploadString(indexDocFtpPath, updatedIndexDoc);
        }

        public string GetHttpRootUrl()
        {
            return config.HttpRootUrl;
        }
    }
}
