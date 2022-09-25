using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AzureStorage.Library;
using AzureStorage.Library.Models;
using AzureStorage.Library.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace WatermarkProcessFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public async static Task Run([QueueTrigger("watermarkqueue", Connection = "")] PictureWatermarkQueue myQueueItem, ILogger log)
        {
            AzureStorage.Library.ConnectionStrings.AzureStorageConnectionString = "Azure storage connection string;";
            IBlobStorage blobStorage = new BlobStorage();
            INoSqlStorage<UserPicture> noSqlStorage = new TableStorage<UserPicture>();
            foreach (var item in myQueueItem.Pictures)
            {
                using var stream = await blobStorage.DownloadAsync(item,EContainerName.pictures);

                using var memoryStream = AddWaterMark(myQueueItem.WatermarkText, stream);

                await blobStorage.UploadAsync(memoryStream, item, EContainerName.watermarkpictures);

                log.LogInformation($"{item} resmine watermark eklenmiþtir");
            }

            var userPicture = await noSqlStorage.GetAsync(myQueueItem.UserId, myQueueItem.City);
            if (userPicture.WatermarkRawPaths != null)
            {
                myQueueItem.Pictures.AddRange(userPicture.WaterMarkPaths);
            }

            userPicture.WaterMarkPaths = myQueueItem.Pictures;

            await noSqlStorage.UpdateAsync(userPicture);

            HttpClient httpClient = new HttpClient();

            var response = await httpClient.GetAsync("https://localhost:44335/api/Notifications/CompleteWatermarkProcess/" + myQueueItem.ConnectionId);

            log.LogInformation($"{myQueueItem.ConnectionId} Client Bilgilendirilmiþtir.");
        }


        public static MemoryStream AddWaterMark(string watermarkText, Stream pictureStream)
        {
            MemoryStream ms = new MemoryStream();

            using (Image image = Bitmap.FromStream(pictureStream))
            {
                using (Bitmap tempBitmap = new Bitmap(image.Width, image.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(tempBitmap))
                    {
                        graphics.DrawImage(image, 0, 0);

                        var font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold);

                        var color = Color.FromArgb(255, 0, 0);

                        var brush = new SolidBrush(color);

                        var point = new Point(20, image.Height - 50);

                        graphics.DrawString(watermarkText, font, brush, point);

                        tempBitmap.Save(ms, ImageFormat.Jpeg);
                    }
                }
            }

            ms.Position = 0;

            return ms;
        }


    }
}
