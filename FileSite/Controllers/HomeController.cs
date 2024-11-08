using FileSite.Data.Interfaces;
using FileSite.Data.ViewModels;
using FileSite.Models;
using FileSite.Repositories;
using FileSite.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace FileSite.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;//DEPRECATED
        private readonly IFileDataRepository _fileDataRepository;
        private readonly GlobalDataRepository _globalDataRepository;
        private readonly FileTypeCounter _fileTypeCounter;
        public HomeController(//ILogger<HomeController> logger,//DEPRECATED
                              IFileDataRepository fileDataRepository,
                              GlobalDataRepository globalDataRepository,
                              FileTypeCounter fileTypeCounter)
        {
            //_logger = logger;//DEPRECATED
            _fileDataRepository = fileDataRepository;
            _globalDataRepository = globalDataRepository;
            _fileTypeCounter = fileTypeCounter;

        }
        
        public IActionResult Index()
        {   
            _globalDataRepository.UpdateGlobalData();
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

            string hash = await _fileDataRepository.Add(fileView, "/home/ardad/Desktop/FilesUploaded");
            if (hash == null) {
                string existingHash = BitConverter.ToString(MD5.Create().ComputeHash(fileView.File.OpenReadStream())).Replace("-", "").ToLower();
                ViewData["Message"] = $"Duplicate file! <SITEURL>/Home/Files/{existingHash} \n!";
                return View();
            }

            
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

        [HttpPost]
        public async Task<ActionResult> GetFileTypes()
        {
            List<object> fileTypes = new List<object>();
            List<string> labels=_fileTypeCounter.Dict.Keys.ToList();
            List<int> count = _fileTypeCounter.Dict.Values.ToList();
            fileTypes.Add(labels);
            fileTypes.Add(count);
            return this.Json(fileTypes); 
        }
    }
}
