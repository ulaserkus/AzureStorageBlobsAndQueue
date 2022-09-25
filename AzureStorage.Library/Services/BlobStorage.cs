using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace AzureStorage.Library.Services
{
    public class BlobStorage : IBlobStorage
    {
        private readonly BlobServiceClient _blobServiceClient;

        public string BlobUrl => "https://udemyrealstorageaccount.blob.core.windows.net";

        public BlobStorage()
        {
            _blobServiceClient = new BlobServiceClient(ConnectionStrings.AzureStorageConnectionString);
        }

        public async Task DeleteAsync(string fileName, EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());

            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.DeleteAsync();
        }

        public async Task<Stream> DownloadAsync(string fileName, EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());

            var blobClient = containerClient.GetBlobClient(fileName);

            var info = await blobClient.DownloadStreamingAsync();

            return info.Value.Content;
        }

        public async Task<List<string>> GetLogAsync(string fileName)
        {
            
            List<string> logs = new List<string>();

            var containerClient = _blobServiceClient.GetBlobContainerClient(EContainerName.logs.ToString());

            await containerClient.CreateIfNotExistsAsync();

            var appendClient = containerClient.GetAppendBlobClient(fileName);

            await appendClient.CreateIfNotExistsAsync();

            var info = await appendClient.DownloadStreamingAsync();

            using (StreamReader sr = new StreamReader(info.Value.Content))
            {
                string line = string.Empty;

                while ((line = sr.ReadLine()) != null)
                {
                    logs.Add(line);
                }

            }

            return logs;
        }

        public List<string> GetNames(EContainerName eContainerName)
        {
            List<string> blobNames = new List<string>();

            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());

            var blobs = containerClient.GetBlobs();

            blobs.ToList().ForEach(x =>
            {
                blobNames.Add(x.Name);
            });

            return blobNames;
        }

        public async Task SetLogAsync(string text, string fileName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(EContainerName.logs.ToString());

            await containerClient.CreateIfNotExistsAsync();

            var appendClient = containerClient.GetAppendBlobClient(fileName);

            await appendClient.CreateIfNotExistsAsync();

            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write($"{DateTime.Now} : {text} \n ");

                    sw.Flush();

                    ms.Position = 0;

                    await appendClient.AppendBlockAsync(ms);
                }

            }
        }

        public async Task UploadAsync(Stream stream, string fileName, EContainerName eContainerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(eContainerName.ToString());

            await containerClient.CreateIfNotExistsAsync();

            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);

            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(stream,true);

        }
    }
}
