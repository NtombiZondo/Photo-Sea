using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Photo_Sea.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Photo_Sea.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHostingEnvironment _oIHostingEvironment;

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment oIHostingEnvironment)
        {
            _logger = logger;
            _oIHostingEvironment = oIHostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public FileResult DownloadZip()
        {
            var webRoot = _oIHostingEvironment.WebRootPath;
            var fileName = "MyZip.zip";
            var temOutPut = webRoot + "/Photo/" + fileName;

            using (ZipOutputStream oZipOutputStream = new ZipOutputStream(System.IO.File.Create(temOutPut)))
            {
                oZipOutputStream.SetLevel(9);
                byte[] buffer = new byte[4096];
                var ImageList = new List<String>();
                ImageList.Add(webRoot + "/Photo/img1.jpg");
                ImageList.Add(webRoot + "/Photo/img.jpg");
                ImageList.Add(webRoot + "/Photo/img2.jpg");

                for (int i = 0; i < ImageList.Count; i++)
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName(ImageList[i]));
                    entry.DateTime = DateTime.Now;
                    entry.IsUnicodeText = true;
                    oZipOutputStream.PutNextEntry(entry);

                    using (FileStream oFileStream = System.IO.File.OpenRead(ImageList[i]))
                    {
                        int sourceBytes;
                        do
                        {
                            sourceBytes = oFileStream.Read(buffer, 0, buffer.Length);
                            oZipOutputStream.Write(buffer, 0, sourceBytes);
                        } while (sourceBytes > 0);
                    }
                }
                oZipOutputStream.Finish();
                oZipOutputStream.Flush();
                oZipOutputStream.Close();

            }
            byte[] finalResult = System.IO.File.ReadAllBytes(temOutPut);
            if (System.IO.File.Exists(temOutPut))
            {
                System.IO.File.Delete(temOutPut);
            }
            if (finalResult == null)
            {
                throw new Exception(String.Format("Nothing Found"));
            }
            return File(finalResult, "application/zip", fileName);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
