using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using GoogletranslateApi.DAL;
using GoogletranslateApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoogletranslateApi.Controllers
{
    public class TranslateController : Controller
    {
        private AppDbContext _context;
        public TranslateController(AppDbContext context)
        {
            _context = context;
                
        }
        public IActionResult Index()
        {
            return View(_context.Products);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {if (!ModelState.IsValid) return View();
            string fromlanguage = "en";
            string tolanguage = "az";


           var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromlanguage}&tl={tolanguage}&dt=t&q={HttpUtility.UrlEncode(product.Description)}";

            var webClient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            var result = webClient.DownloadString(url);
            try
            {
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
               
            }
            catch
            {
                return null;
            }


            product.DescriptionENG = result;

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int? id)
        {if (id == null) return View();
            Product product = await _context.Products.FindAsync(id);
            if (product == null) return View();

            return View(product);
        }

        public async Task< IActionResult >Delete(int? id)
        {if (id == null) return View();
            Product product = await _context.Products.FindAsync(id);
            if (product == null) return View();

            return View(product);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteProduct(int? id)
        {if (id == null) return NotFound();
            Product product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
             _context.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task< IActionResult> Update(int?id)
        {if (id == null) return NotFound();

            Product product =await _context.Products.FindAsync(id);
            if (product == null) return NotFound(); 
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int? id,Product product)
        {
            if (id == null) return RedirectToAction(nameof(Index));
            Product dbproduct = await _context.Products.FindAsync(id);
            if (!ModelState.IsValid) return View(dbproduct);
            dbproduct.Name = product.Name;
            dbproduct.Description = product.Description;
            dbproduct.DescriptionENG = product.DescriptionENG;
            dbproduct.Price = product.Price;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}