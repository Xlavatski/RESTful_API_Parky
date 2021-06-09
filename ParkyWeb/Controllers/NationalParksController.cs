using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository _repoPark;

        public NationalParksController(INationalParkRepository repoPark)
        {
            _repoPark = repoPark;
        }
        public IActionResult Index()
        {
            return View(new NationalPark() { });
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            NationalPark obj = new NationalPark();

            if (id == null)
            {
                return View(obj);
            }

            obj = await _repoPark.GetAsync(SD.NationalParkAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWToken"));

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark objNP) 
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    objNP.Picture = p1;
                }
                else
                {
                    var objFromDb = await _repoPark.GetAsync(SD.NationalParkAPIPath, objNP.Id, HttpContext.Session.GetString("JWToken"));
                    if (objFromDb == null)
                    {
                        var fileBytes = System.IO.File.ReadAllBytes(@"C:\Users\dglavas\Desktop\Moje\NP\NoImage.jpg");
                        objNP.Picture = fileBytes;

                    }
                    else 
                    {
                        objNP.Picture = objFromDb.Picture;
                    }
                }

                if (objNP.Id == 0)
                {
                    await _repoPark.CreateAsync(SD.NationalParkAPIPath, objNP, HttpContext.Session.GetString("JWToken"));
                }
                else
                {
                    await _repoPark.UpdateAsync(SD.NationalParkAPIPath+objNP.Id, objNP, HttpContext.Session.GetString("JWToken"));
                }
                return RedirectToAction(nameof(Index));
            }
            else 
            {
                return View(objNP);
            }
        }

        public async Task<IActionResult> GetAllNationalPark() 
        {
            return Json(new { data = await _repoPark.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWToken")) });
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id) 
        {
            var status = await _repoPark.DeleteAsync(SD.NationalParkAPIPath, id, HttpContext.Session.GetString("JWToken"));

            if (status) 
            {
                return Json(new { success = true, message = "Delete Successful" });
            }
            return Json(new { success = false, message = "Delete Not Successful" });
        }


    }
}
