using FileSite.Data;
using FileSite.Data.Interfaces;
using FileSite.Data.ViewModels;
using FileSite.Models;
using FileSite.Repositories;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;

namespace FileSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IFileDataRepository _fileDataRepository;
        private readonly GlobalDataRepository _globalDataRepository;

        public HomeController(ILogger<HomeController> logger,
                              IFileDataRepository fileDataRepository,
                              GlobalDataRepository globalDataRepository)
        {
            _logger = logger;
            _fileDataRepository = fileDataRepository;
            _globalDataRepository = globalDataRepository;

        }
        
        public IActionResult Index()
        { 
            return View();
        }

        public IActionResult Upload()
        {
            return View();
        }

        [RequestSizeLimit(30_000_000L)]
        [HttpPost]
        public async Task<IActionResult> Upload(FileDataVM fileView)
        {
            if (fileView.File.Length > 24_000_000L)
            {
                ViewData["Message"] = "File size too big! (>24mb)";
                return View();
            }
            if (fileView.File == null)
            {
                ViewData["Message"] = "No File detected?";
                return View();
            }

            string hash = await _fileDataRepository.Add(fileView, "C:\\zFileSite");
            if (hash == null) {
                string existingHash = BitConverter.ToString(MD5.Create().ComputeHash(fileView.File.OpenReadStream())).Replace("-", "").ToLower();
                ViewData["Message"] = $"Duplicate file! <SITEURL>/Home/Files/{existingHash} \n!";
                return View();
            }

            _globalDataRepository.UpdateGlobalData();
            ViewData["message"] = $"Your download link: <SITEURL>/Home/Files/{hash} \n Do not Lose this link!";
            return View();
        }

        [Route("{controller}/{action}/{hash?}")]
        public async Task<IActionResult> Files(string? hash)
        {
            if (hash == null) {
                ViewData["message"] = "Error!";
                return View();
            }
            FileData? file = await _fileDataRepository.RequestFileData(hash);
            if (file == null)
            {
                ViewData["message"] = "Error!";
                return View();
            }

            return View(file);

        }

        [HttpPost("{controller}/{action}/{hash?}")]
        public IActionResult Files(string hash, string path)
        {

            return  File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));

        }

    }
}
