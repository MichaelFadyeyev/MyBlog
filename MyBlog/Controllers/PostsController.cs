using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MyBlog.Data;
using MyBlog.Models;
using MyBlog.ViewModels;

namespace MyBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private readonly string[] allowedExt = new string[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

        public PostsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Posts
        public async Task<IActionResult> Index(int? categoryId, int pageNumber = 1)
        {
            // 1 Формування колекції публікацій для виведення
            var posts = _context.Posts.Include(p => p.Category).ToList();
            if(categoryId !=null && categoryId !=0 )
            {
                posts = posts.Where(p => p.CategoryId == categoryId).ToList(); // пости певної категорії
            }

            // 2 розділення колекції на сторінки пагінації
            int pageSize = 3; // кількість постів на сторінці
            int count = posts.Count;

            // 3 Формування колекції категорії для створення фільтру:
            List<Category> categories = await _context.Categories.ToListAsync();
            categories.Insert(0, new Category()
            {
                Id = 0,
                Name = "Всі категорії"
            });
            var items = posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // 4 Створення менеджера  пагінації
            PageViewModel paginator = new PageViewModel(count, pageNumber, pageSize);

            // 5 Створення агрегуючої моделі
            PostsViewModel viewModel = new PostsViewModel()
            {
                Posts = items,
                Paginator = paginator,
                Categories = new SelectList(categories, "Id", "Name")
            };


            // ->
            var applicationDbContext = _context.Posts.Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Content,PublishDate,PublishTime,ImagePath,CategoryId")] Post post, IFormFile uploadFile)
        {
            if (ModelState.IsValid)
            {
                //*
                if (uploadFile != null)
                {
                    string name = uploadFile.FileName;
                    string ext = Path.GetExtension(uploadFile.FileName);
                    if (allowedExt.Contains(ext))
                    {
                        string path = $"/files/{name}";
                        string serverPath = _env.WebRootPath + path;
                        using (FileStream fs = new FileStream(serverPath, FileMode.Create, FileAccess.Write))
                            await uploadFile.CopyToAsync(fs);

                        post.ImagePath = path;
                        _context.Posts.Add(post);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    else return RedirectToAction(nameof(ExtensionError), "Errors");
                }
                else return RedirectToAction(nameof(UploadError), "Errors");
            }
            // ->
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            if(post.ImagePath != null) ViewBag.ImagePath = post.ImagePath;

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Content,PublishDate,PublishTime,ImagePath,CategoryId")] Post post, IFormFile uploadFile)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (uploadFile != null)
                    {
                        string name = uploadFile.FileName;
                        string ext = Path.GetExtension(uploadFile.FileName);
                        if (allowedExt.Contains(ext))
                        {
                            string path = $"/files/{name}";
                            string serverPath = _env.WebRootPath + path;
                            using (FileStream fs = new FileStream(serverPath, FileMode.Create, FileAccess.Write))
                                await uploadFile.CopyToAsync(fs);

                            post.ImagePath = path;

                        }
                    }
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        #region NewErrorHandlers
        public IActionResult ExtensionError()
        {
            ViewData["message"] = "Формат файлу некорректний";
            return View();
        }

        public IActionResult UploadError()
        {
            ViewData["message"] = "Помилка завантаження файлу";
            return View();
        }

        public IActionResult NotFoundError()
        {
            ViewData["message"] = "Помилка редагування публікації";
            return View();
        } 
        #endregion
    }
}
