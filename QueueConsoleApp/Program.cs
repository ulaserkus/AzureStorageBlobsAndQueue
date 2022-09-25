using AzureStorage.Library.Services;
using System;
using System.Text;
using System.Threading.Tasks;

namespace QueueConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AzureStorage.Library.ConnectionStrings.AzureStorageConnectionString = "Azure storage connection string";

            AzQueue azQueue = new AzQueue("testkuyruk");
            //string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Ulaş Erkuş"));
            //azQueue.SendMessageAsync(base64).Wait();
            var queue = azQueue.RetrieveNextMessageAsync().Result;
            //string message = Encoding.UTF8.GetString(Convert.FromBase64String(queue.MessageText));

            await azQueue.DeleteMessageAsync(queue.MessageId, queue.PopReceipt);

            Console.WriteLine("mesaj silindi");
        }
    }
}
