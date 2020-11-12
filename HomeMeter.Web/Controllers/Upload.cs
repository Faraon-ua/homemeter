using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HomeMeter.Domain.Models;
using HomeMeter.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tesseract;

namespace HomeMeter.Web.Controllers
{
    public class Upload : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Upload(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var filePath = $"{_hostingEnvironment.WebRootPath}/uploads/[{DateTime.UtcNow:yyyy-dd-M--HH-mm-ss}]{imageFile.FileName}";
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
            }
            var dir = new DirectoryInfo($"{_hostingEnvironment.WebRootPath}/uploads/");
            var filesToRemove = dir.GetFiles().OrderByDescending(entry => entry.CreationTime).Skip(3);
            foreach (var file in filesToRemove)
            {
                file.Delete();
            }
            var requestLog = new RequestLog
            {
                TimeStamp = DateTime.UtcNow,
                Headers = Request.Headers.Select(entry => new HeaderLog { Name = entry.Key, Value = entry.Value }).ToList(),
            };
            using (StreamReader reader
                  = new StreamReader(Request.Body, Encoding.UTF8, true, 1024, true))
            {
                requestLog.Body = await reader.ReadToEndAsync();
            }
            var xmlPath = $"{_hostingEnvironment.WebRootPath}/xml/requests.xml";
            var logs = XmlService.Deserialize<RequestLogs>(xmlPath) ?? new RequestLogs { Logs = new List<RequestLog>() };
            logs.Logs = logs.Logs.OrderByDescending(entry => entry.TimeStamp).Take(5).ToList();
            logs.Logs.Add(requestLog);
            XmlService.Serialize(xmlPath, logs);
            return Ok();
        }

        [HttpGet]
        public IActionResult Requests()
        {
            var xmlPath = $"{_hostingEnvironment.WebRootPath}/xml/requests.xml";
            var logs = XmlService.Deserialize<RequestLogs>(xmlPath) ?? new RequestLogs { Logs = new List<RequestLog>() };
            return View(logs.Logs);
        }


        [HttpGet]
        public IActionResult List()
        {
            var dir = new DirectoryInfo($"{_hostingEnvironment.WebRootPath}/uploads/");
            var result = new Dictionary<FileInfo, string>();
            var files = dir.GetFiles().OrderByDescending(entry => entry.CreationTime).ToList();
            //using (var engine = new TesseractEngine($"{_hostingEnvironment.ContentRootPath}/tessdata", "eng"))
            //{
            foreach (var file in files)
            {
                result.Add(file, string.Empty);

                //using (var pix = Pix.LoadFromFile(file.FullName))
                //{
                //    using (var page = engine.Process(pix))
                //    {
                //        //result.Add(file, page.GetText().Replace("\n", "<br>"));
                //    }
                //}
            }
            //}
            return View(result);
        }

        [HttpGet]
        public IActionResult Test()
        {
            return View();
        }
    }
}
