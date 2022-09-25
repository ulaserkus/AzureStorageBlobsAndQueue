using AzureStorage.Library;
using AzureStorage.Library.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcWebApp.Controllers
{
    public class TableStoragesController : Controller
    {
        private readonly INoSqlStorage<Product> _noSqlStorage;

        public TableStoragesController(INoSqlStorage<Product> noSqlStorage)
        {
            _noSqlStorage = noSqlStorage;
        }

        public IActionResult Index()
        {

            ViewBag.isUpdate = false;

            ViewBag.products = _noSqlStorage.All().ToList();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            product.PartitionKey = "Kalemler";
            await _noSqlStorage.AddAsync(product);

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Update(string rowKey, string partitionKey)
        {
            var product = await _noSqlStorage.GetAsync(rowKey, partitionKey);

            ViewBag.products = _noSqlStorage.All().ToList();

            ViewBag.isUpdate = true;

            return View("Index", product);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product)
        {
            product.ETag = Azure.ETag.All;

            ViewBag.isUpdate = true;

            await _noSqlStorage.UpdateAsync(product);

            return RedirectToAction("Index");

        }


        public async Task<IActionResult> Delete(string rowKey,string partitionKey)
        {
            await _noSqlStorage.DeleteAsync(rowKey,partitionKey);

            return RedirectToAction("Index");

        }

        [HttpGet]
        public IActionResult Query(int price)
        {
            ViewBag.isUpdate = false;

            ViewBag.products = _noSqlStorage.Query(x => x.Price > price).ToList();

            return View("Index");
        }
    }
}
