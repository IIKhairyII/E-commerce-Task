using e_commerce.Data;
using e_commerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace e_commerce.Controllers
{
    public class ProductsController : Controller
    {
        public ProductsController(ApplicationDbContext context,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            FilesManagerController fileManager)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _fileManager = fileManager;
        }
        private readonly ApplicationDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly FilesManagerController _fileManager;

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> Create(Products product)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        string rootFolder = _hostingEnvironment.WebRootPath;
                        rootFolder = Path.Combine(rootFolder, _configuration.GetRequiredSection("ImagesFolder").Value);
                        //string imageName = Path.Combine(rootFolder, $"{product.ProuctName}_{Guid.NewGuid()}.{Path.GetExtension(product.Image.FileName)}");
                        string imageName = $"{product.ProuctName}_{Guid.NewGuid()}.{Path.GetExtension(product.Image.FileName)}";

                        product.ImageUrl = $"{Request.Scheme}://{Request.Host}/api/fileManager/download?imageName={imageName}";
                        _context.Products.Add(product);
                        _context.SaveChanges();

                        //Uploading Image must be performed after saving in database
                        if (product.ImageUrl != null)
                        {
                            bool isImageSaved = await _fileManager.FileManager(product.Image, rootFolder, Path.Combine(rootFolder, imageName));
                            if (!isImageSaved)
                                throw new Exception("");
                        }

                        transaction.Commit();
                        product.ResponseMessage = "Product created successfully";
                        product.isCreated = true;
                        return View(product);
                    }
                    else
                    {
                        throw new ArgumentException("invalid_state");
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    if (ex is ArgumentException)
                        product.ResponseMessage = "Product not created as some values are nulls";
                    else if (ex is NullReferenceException)
                    {
                        product = new();
                        product.ResponseMessage = "Server ERROR!!! Product Not Created..";
                    }
                    else
                        product.ResponseMessage = "Server Error";
                    product.isCreated = false;

                    if (!string.IsNullOrEmpty(product.ImageUrl) && System.IO.File.Exists(product.ImageUrl))
                        System.IO.File.Delete(product.ImageUrl);

                    return View(product);
                }
            }
        }

    }
}
