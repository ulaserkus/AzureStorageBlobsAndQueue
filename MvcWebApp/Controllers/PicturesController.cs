using AzureStorage.Library;
using AzureStorage.Library.Models;
using AzureStorage.Library.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcWebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcWebApp.Controllers
{
    public class PicturesController : Controller
    {
        INoSqlStorage<UserPicture> _noSql;
        IBlobStorage _blobStorage;
        public string UserId { get; set; } = "12345";
        public string City { get; set; } = "istanbul";

        public PicturesController(INoSqlStorage<UserPicture> noSql, IBlobStorage blobStorage)
        {
            _noSql = noSql;
            _blobStorage = blobStorage;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.UserId = UserId;
            ViewBag.City = City;

            List<FileBlob> fileBlobs = new List<FileBlob>();

            var user = await _noSql.GetAsync(UserId, City);

            if (user != null)
            {
                user.Paths.ForEach(x =>
                {
                    fileBlobs.Add(new FileBlob() { Name = x, Url = $"{_blobStorage.BlobUrl}/{EContainerName.pictures}/{x}" });
                });

            }

            ViewBag.fileBlobs = fileBlobs;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IEnumerable<IFormFile> pictures)
        {
            List<string> pictureList = new List<string>();

            foreach (var item in pictures)
            {
                var newPictureName = $"{Guid.NewGuid()}{Path.GetExtension(item.FileName)}";

                await _blobStorage.UploadAsync(item.OpenReadStream(), newPictureName, EContainerName.pictures);

                pictureList.Add(newPictureName);
            }

            var isUser = await _noSql.GetAsync(UserId, City);

            if (isUser != null)
            {
                pictureList.AddRange(isUser.Paths);
                isUser.Paths = pictureList;

                await _noSql.UpdateAsync(isUser);
            }
            else
            {
                isUser = new UserPicture();
                isUser.RowKey = UserId;
                isUser.PartitionKey = City;
                isUser.Paths = pictureList;
                isUser.ETag = Azure.ETag.All;
                isUser.Timestamp = DateTime.Now;

                await _noSql.AddAsync(isUser);
            }



            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ShowWatermark()
        {
            var userPictures = await _noSql.GetAsync(UserId, City);

            List<FileBlob> fileBlobs = new List<FileBlob>();

            var user = await _noSql.GetAsync(UserId, City);

            if (user != null)
            {
                user.Paths.ForEach(x =>
                {
                    fileBlobs.Add(new FileBlob() { Name = x, Url = $"{_blobStorage.BlobUrl}/{EContainerName.watermarkpictures}/{x}" });
                });

            }

            ViewBag.fileBlobs = fileBlobs;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWatermark(PictureWatermarkQueue pictureWatermarkQueue )
        {
            var jsonString = JsonConvert.SerializeObject(pictureWatermarkQueue);
            
            string jsonStringBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

            AzQueue azQueue = new AzQueue("watermarkqueue");

            await azQueue.SendMessageAsync(jsonStringBase64);

            return Ok();
        }
    }
}
