using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            return Ok();
        }

        [HttpGet]
        public IActionResult List()
        {
            var dir = new DirectoryInfo($"{_hostingEnvironment.WebRootPath}/uploads/");
            var result = new Dictionary<FileInfo, string>();
            var files = dir.GetFiles().OrderByDescending(entry => entry.CreationTime).ToList();
            using (var engine = new TesseractEngine($"{_hostingEnvironment.ContentRootPath}/tessdata", "eng"))
            {
                foreach (var file in files)
                {
                    using (var pix = Pix.LoadFromFile(file.FullName))
                    {
                        using (var page = engine.Process(pix))
                        {
                            result.Add(file, page.GetText().Replace("\n","<br>"));
                        }
                    }
                }
            }
            return View(result);
        }

        [HttpGet]
        public IActionResult Test()
        {
            return View();
        }
    }
}
