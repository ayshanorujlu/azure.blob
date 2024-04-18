using AzureStorageLibrary;
using Microsoft.AspNetCore.Mvc;
using MVCWebApp.Models;

namespace MVCWebApp.Controllers
{
    public class BlobsController : Controller
    {
        private readonly IBlobStorage _blobStorage;

        public BlobsController(IBlobStorage blobStorage)
        {
            _blobStorage = blobStorage;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var names = _blobStorage.GetNames(EContainerName.pictures);
           string blobUrl = $"{_blobStorage.BlobUrl}/{EContainerName.pictures.ToString()}";

            ViewBag.logs = await _blobStorage.GetLogsAsync("controller.txt");

            ViewBag.blobs = names.Select(x => new FileBlob { Name = x, Url = $"{blobUrl}/{x}" }).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile picture)
        {
            try
            {
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);

                await _blobStorage.UploadAsync(picture.OpenReadStream(), newFileName, EContainerName.pictures);
                await _blobStorage.SetLogAsync("File uploaded successfully", "controller.txt");
            }
            catch (Exception)
            {
                await _blobStorage.SetLogAsync("Error in File Upload", "controller.txt");
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Download(string filename)
        {
            try
            {

                var stream = await _blobStorage.DownloadAsync(filename, EContainerName.pictures);
                await _blobStorage.SetLogAsync("File downloaded successfully", "controller.txt");
                return File(stream, "application/octet-stream", filename);
            }
            catch (Exception)
            {
                await _blobStorage.SetLogAsync("Error in File Download", "controller.txt");
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string filename)
        {
            await _blobStorage.DeleteAsync(filename, EContainerName.pictures);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadPdf(IFormFile pdf)
        {
            try
            {

                var newFilename = Guid.NewGuid().ToString() + Path.GetExtension(pdf.FileName);
                await _blobStorage.UploadAsync(pdf.OpenReadStream(), newFilename, EContainerName.pdf, "application/pdf");
                await _blobStorage.SetLogAsync("Pdf File Uploaded succesfully", "controller.txt");
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("Index");
        }
    }
}
