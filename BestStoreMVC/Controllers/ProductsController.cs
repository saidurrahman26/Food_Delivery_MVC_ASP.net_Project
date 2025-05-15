using BestStoreMVC.Models;
using BestStoreMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BestStoreMVC.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var products = context.Products.OrderByDescending( p =>p.Id).ToList(); //new product will be at top
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
           
            if(productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The image file is required");
            }
           
            if(!ModelState.IsValid)
            {
                return View(productDto);
            }

            //This is for Saving the Image File

            string newFileName = DateTime.Now.ToString("yyyymmddhhmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = Path.Combine(environment.WebRootPath, "products", newFileName);

            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            //This is For New Product In the Database
            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Category = productDto.Category,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageFileName = newFileName,
                CreateAt = DateTime.Now,
            };

            context.Products.Add(product);
            context.SaveChanges();
           
            return RedirectToAction("Index", "Products");
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);
            if(product== null)
            {
                return RedirectToAction("Index", "Products");
            }

            // create productdto from product
            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category= product.Category,
                Price = product.Price,
                Description= product.Description,
            };

            // return productdto to view
            ViewData["productId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreateAt"] = product.CreateAt.ToString("MM/dd/yyyy");

            return View(productDto);
        }
        
        
        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index", "products");
            }

            if (!ModelState.IsValid)
            {
                ViewData["productId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreateAt"] = product.CreateAt.ToString("MM/dd/yyyy");


                return View(productDto);
            }
            string newFileName = product.ImageFileName;
            if(productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = Path.Combine(environment.WebRootPath, "products", newFileName);

                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }
                // dellethe the old pic
                string oldImageFullPath = Path.Combine(environment.WebRootPath, "products", product.ImageFileName);

                System.IO.File.Delete(oldImageFullPath);

            }

            //update the product in databse
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Category = productDto.Category;
            product.Price = productDto.Price;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            context.SaveChanges();
            return RedirectToAction("Index", "Products");

        }

        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }
            string imageFullPath = environment.WebRootPath + "/products" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Products.Remove(product);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Products");
        }
    
    }
}
