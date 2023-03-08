using BookShop.Areas.Admin.Data;
using BookShop.Classes;
using BookShop.Models;
using BookShop.Models.Repository;
using BookShop.Models.UnitOfWork;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static BookShop.Models.ViewModels.BooksCreateEditViewModel;

namespace BookShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    [DisplayName("مدیریت کتاب ها")]
    public class BooksController : Controller
    {
        private readonly IUnitOfWork _UW;
        private readonly IHostingEnvironment _env;
        //private readonly BookRepository _booksRepository;
        public BooksController(IUnitOfWork UW, IHostingEnvironment env)
        {
            _UW = UW;
            _env = env;
        }
        [Authorize(Policy =ConstantPolicies.DynamicPermission)]
        [DisplayName("مشاهده کتاب ها")]
        public IActionResult Index(string Msg, int page = 1, int row = 5, string sortExpression = "Title",string title="")
        {
            title = String.IsNullOrEmpty(title) ? "" : title;
            if (Msg != null)
            {
                ViewBag.Msg = "در ثبت اطلاعات خطایی رخ داده است لطفا مجددا تلاش کنید !!!";
            }
            List<int> Rows = new List<int>
            {
                5,10,15,20,50,100
            };
            ViewBag.RowID = new SelectList(Rows,row );
            ViewBag.NumOfRow = (page - 1) * row + 1;
            ViewBag.Search = title;

            var PagingModel = PagingList.Create(_UW.BookRepository.GetAllBooks(title,"","","","","",""), row, page,sortExpression,"Title");
            PagingModel.RouteValue = new RouteValueDictionary
            {
                {"row",row },
                {"title",title }
            };

            ViewBag.Categories = _UW.BookRepository.GetAllCategories();
            ViewBag.LanguageID = new SelectList(_UW.BaseRepository<Language>().FindAll(), "LanguageName", "LanguageName");
            ViewBag.PublisherID = new SelectList(_UW.BaseRepository<Publisher>().FindAll(), "PublisherName", "PublisherName");
            ViewBag.AuthorID = new SelectList(_UW.BaseRepository<Author>().FindAll().Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + " " + t.LastName }), "NameFamily", "NameFamily");
            ViewBag.TranslatorID = new SelectList(_UW.BaseRepository<Translator>().FindAll().Select(z => new TranslatorList { TranslatorID = z.TranslatorID, NameFamily = z.Name + " " + z.Family }), "NameFamily", "NameFamily");

            return View(PagingModel);
        }


        public IActionResult AdvancedSearch(BooksAdvancedSearch ViewModel)
        {
            ViewModel.Title = String.IsNullOrEmpty(ViewModel.Title) ? "" : ViewModel.Title;
            ViewModel.ISBN = String.IsNullOrEmpty(ViewModel.ISBN) ? "" : ViewModel.ISBN;
            ViewModel.Language = String.IsNullOrEmpty(ViewModel.Language) ? "" : ViewModel.Language;
            ViewModel.Publisher = String.IsNullOrEmpty(ViewModel.Publisher) ? "" : ViewModel.Publisher;
            ViewModel.Author = String.IsNullOrEmpty(ViewModel.Author) ? "" : ViewModel.Author;
            ViewModel.Translator = String.IsNullOrEmpty(ViewModel.Translator) ? "" : ViewModel.Translator;
            ViewModel.Category = String.IsNullOrEmpty(ViewModel.Category) ? "" : ViewModel.Category;
            ViewModel.Language = String.IsNullOrEmpty(ViewModel.Language) ? "" : ViewModel.Language;
            var Books = _UW.BookRepository.GetAllBooks(ViewModel.Title,ViewModel.ISBN,ViewModel.Language,ViewModel.Publisher,ViewModel.Author,ViewModel.Translator,ViewModel.Category);
            return View(Books);
        }

        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        [DisplayName("افزودن کتاب جدید")]
        public IActionResult Create()
        {
            ViewBag.LanguageID = new SelectList(_UW.BaseRepository<Language>().FindAll(), "LanguageID", "LanguageName");
            ViewBag.PublisherID = new SelectList(_UW.BaseRepository<Publisher>().FindAll(), "PublisherID", "PublisherName");
            ViewBag.AuthorID = new SelectList(_UW.BaseRepository<Author>().FindAll().Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + " " + t.LastName }),"AuthorID","NameFamily");
            ViewBag.TranslatorID = new SelectList(_UW.BaseRepository<Translator>().FindAll().Select(z => new TranslatorList { TranslatorID = z.TranslatorID ,NameFamily = z.Name + " " + z.Family}),"TranslatorID","NameFamily");
            BooksSubCategoriesViewModel booksSubCategoryVM = new BooksSubCategoriesViewModel(_UW.BookRepository.GetAllCategories(), null);
            BooksCreateEditViewModel BooksViewModel = new BooksCreateEditViewModel(booksSubCategoryVM);
            
            return View(BooksViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BooksCreateEditViewModel ViewModel)
        {
            if (ModelState.IsValid)
            {
                if(ViewModel.File != null)
                {
                    string FileName = ViewModel.File.FileName;
                    string FileExtension = Path.GetExtension(FileName);
                    string NewFileName = String.Concat(Guid.NewGuid().ToString(), FileExtension);
                    var path = $"{_env.WebRootPath}/BookFiles/{NewFileName}";
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await ViewModel.File.CopyToAsync(stream);
                    }
                    ViewModel.FileName = NewFileName;
                }
                if (await _UW.BookRepository.CreateBookAsync(ViewModel))
                    return RedirectToAction("Index");
                else
                    ViewBag.Error = "در انجام عملیات خطایی رخ داده است";
            }
            ViewBag.LanguageID = new SelectList(_UW.BaseRepository<Language>().FindAll(), "LanguageID", "LanguageName");
            ViewBag.PublisherID = new SelectList(_UW.BaseRepository<Publisher>().FindAll(), "PublisherID", "PublisherName");
            ViewBag.AuthorID = new SelectList(_UW.BaseRepository<Author>().FindAll().Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + " " + t.LastName }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(_UW.BaseRepository<Translator>().FindAll().Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.Name + " " + t.Family }), "TranslatorID", "NameFamily");
            ViewModel.SubCategoriesVM = new BooksSubCategoriesViewModel(_UW.BookRepository.GetAllCategories(), ViewModel.CategoryID);
            return View(ViewModel);

        }

        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        [DisplayName("مشاهده جزئیات کتاب")]
        public IActionResult Details(int id)
        {
            var BookInfo = _UW._Context.Query<ReadAllBook>().Where(b => b.BookID == id).First();
            return View(BookInfo);
        }

        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        [DisplayName("حذف کتاب")]
        public async Task<IActionResult> Delete(int id)
        {
            //var Book = _context.Books.Find(id);
            var Book = await _UW.BaseRepository<Book>().FindByIdAsync(id);
            if(Book != null)
            {
                Book.Delete = true;
                await _UW.Commit();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize(Policy = ConstantPolicies.DynamicPermission)]
        [DisplayName("ویرایش اطلاعات کتاب")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            else
            {
                var Book = await _UW._Context.Books.FindAsync(id);
                if (Book == null)
                {
                    return NotFound();
                }

                else
                {
                    var ViewModel = await(from b in _UW._Context.Books.Include(l => l.Language)
                                     .Include(p => p.Publisher)
                                     where (b.BookID == id)
                                     select new BooksCreateEditViewModel
                                     {
                                         BookID = b.BookID,
                                         Title = b.Title,
                                         ISBN = b.ISBN,
                                         NumOfPages = b.NumOfPages,
                                         Price = b.Price,
                                         Stock = b.Stock,
                                         IsPublish = (bool)b.IsPublish,
                                         LanguageID = b.LanguageID,
                                         PublisherID = b.Publisher.PublisherID,
                                         PublishYear = b.PublishYear,
                                         Summary = b.Summary,
                                         Weight = b.Weight,
                                         RecentIsPublish = (bool)b.IsPublish,
                                         PublishDate = b.PublishDate,


                                     }).FirstAsync();

                    int[] AuthorsArray =  await(from a in _UW._Context.Author_Books
                                                where (a.BookID == id)
                                                select a.AuthorID).ToArrayAsync();

                    int[] TranslatorsArray = await(from t in _UW._Context.Book_Translators
                                                    where (t.BookID == id)
                                                    select t.TranslatorID).ToArrayAsync();

                    int[] CategoriesArray = await(from c in _UW._Context.Book_Categories
                                                   where (c.BookID == id)
                                                   select c.CategoryID).ToArrayAsync();

                    ViewModel.AuthorID = AuthorsArray;
                    ViewModel.TranslatorID = TranslatorsArray;
                    ViewModel.CategoryID = CategoriesArray;

                    ViewBag.LanguageID = new SelectList(_UW.BaseRepository<Language>().FindAll(), "LanguageID", "LanguageName");
                    ViewBag.PublisherID = new SelectList(_UW.BaseRepository<Publisher>().FindAll(), "PublisherID", "PublisherName");
                    ViewBag.AuthorID = new SelectList(_UW.BaseRepository<Author>().FindAll().Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + " " + t.LastName }), "AuthorID", "NameFamily");
                    ViewBag.TranslatorID = new SelectList(_UW.BaseRepository<Translator>().FindAll().Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.Name + " " + t.Family }), "TranslatorID", "NameFamily");
                    ViewModel.SubCategoriesVM = new BooksSubCategoriesViewModel(_UW.BookRepository.GetAllCategories(), CategoriesArray);

                    return View(ViewModel);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BooksCreateEditViewModel ViewModel)
        {
            ViewBag.LanguageID = new SelectList(_UW.BaseRepository<Language>().FindAll(), "LanguageID", "LanguageName");
            ViewBag.PublisherID = new SelectList(_UW.BaseRepository<Publisher>().FindAll(), "PublisherID", "PublisherName");
            ViewBag.AuthorID = new SelectList(_UW.BaseRepository<Author>().FindAll().Select(t => new AuthorList { AuthorID = t.AuthorID, NameFamily = t.FirstName + " " + t.LastName }), "AuthorID", "NameFamily");
            ViewBag.TranslatorID = new SelectList(_UW.BaseRepository<Translator>().FindAll().Select(t => new TranslatorList { TranslatorID = t.TranslatorID, NameFamily = t.Name + " " + t.Family }), "TranslatorID", "NameFamily");
            ViewModel.SubCategoriesVM = new BooksSubCategoriesViewModel(_UW.BookRepository.GetAllCategories(), ViewModel.CategoryID);
            if (ModelState.IsValid)
            {
                if(await _UW.BookRepository.EditBookAsync(ViewModel))
                {
                    ViewBag.MsgSuccess = "ذخیره تغییرات با موفقیت انجام شد";
                    return View(ViewModel);
                }
                else
                {
                    ViewBag.MsgSuccess = "در ذخیره تغییرات خطایی رخ داد";
                    return View(ViewModel);
                }
            }
            else
            {
                ViewBag.MsgFailed = "اطلاعات فرم نامعتبر است.";
                return View(ViewModel);
            }
        }
        
        public async Task<IActionResult> SearchByISBN(string ISBN)
        {
            if (ISBN != null)
            {
                var Book = (from b in _UW._Context.Books
                            where (b.ISBN == ISBN)
                            select new BooksIndexViewModel
                            {
                                Title = b.Title,
                                Author = BookShopContext.GetAllAuthors(b.BookID),
                                Translator = BookShopContext.GetAllTranslators(b.BookID),
                                Category = BookShopContext.GetAllCategories(b.BookID),
                            }).FirstOrDefaultAsync();
                if (Book.Result == null)
                {
                    ViewBag.Msg = "کتابی با این شابک پیدا نشد";
                }
                return View(await Book);
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Download(int id)
        {
            var Book = await _UW.BaseRepository<Book>().FindByIdAsync(id);
            if(Book == null)
                return NotFound();
            var Path = $"{_env.WebRootPath}/BookFiles/{Book.File}";
            var memory = new MemoryStream();
            using(var stream = new FileStream(Path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, FileExtensions.GetContentType(Path), Book.File);
        }
    }
}
