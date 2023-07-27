using BookShop.Classes;
using BookShop.Models.UnitOfWork;
using BookShop.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading.Tasks;

namespace BookShop.Models.Repository
{
    public class BookRepository:IBookRepository
    {
        private readonly BookShopContext _context;
        private readonly IConvertDate _convertDate;
        private readonly IUnitOfWork _UW;
        public BookRepository(IUnitOfWork UW, IConvertDate convertDate)
        {
            _context = UW._Context;
            _convertDate = convertDate;
            _UW = UW;
        }
        public List<TreeViewCategory> GetAllCategories()
        {
            var Categories = (from c in _context.Categories
                              where (c.ParentCategoryID == null)
                              select new TreeViewCategory { id = c.CategoryID, title = c.CategoryName }).ToList();
            foreach (var item in Categories)
            {
                BindSubCategories(item);
            }
            return Categories;
        }

        public void BindSubCategories(TreeViewCategory category)
        {
            var SubCategories = (from c in _context.Categories 
                                 where (c.ParentCategoryID == category.id)
                                 select new TreeViewCategory { id=c.CategoryID,title = c.CategoryName}).ToList();
            foreach(var item in SubCategories)
            {
                BindSubCategories(item);
                category.subs.Add(item);
            }
        }

        public List<BooksIndexViewModel> GetAllBooks(string title, string ISBN, string Language, string Publisher, string Author, string Translator, string Category )
        {

            string AuthorsName = "";
            string TranslatorName = "";
            string CategoryName = "";
            List<BooksIndexViewModel> ViewModel = new List<BooksIndexViewModel>();

            //Eager Loading
            var Books = (from u in _context.Author_Books.Include(b => b.Book).ThenInclude(p => p.Publisher)
                         .Include(a => a.Author)
                         join l in _context.Languages on u.Book.LanguageID equals l.LanguageID
                         join s in _context.Book_Translators on u.Book.BookID equals s.BookID into bt
                         from bts in bt.DefaultIfEmpty()
                         join t in _context.Translator on bts.TranslatorID equals t.TranslatorID into tr
                         from trl in tr.DefaultIfEmpty()
                         join r in _context.Book_Categories on u.Book.BookID equals r.BookID into bc
                         from bct in bc.DefaultIfEmpty()
                         join c in _context.Categories on bct.CategoryID equals c.CategoryID into cg
                         from cog in cg.DefaultIfEmpty()
                         where (/*u.Book.Delete == false &&*/ //Replaced by quer filter in BookShopContext
                         u.Book.Title.Contains(title.TrimStart().TrimEnd())
                         && u.Book.ISBN.Contains(ISBN.TrimStart().TrimEnd())
                         && l.LanguageName.Contains(Language.TrimStart().TrimEnd()))
                         && EF.Functions.Like(u.Book.Publisher.PublisherName, "%" + Publisher + "%")
                         select new
                         {

                             Author = u.Author.FirstName + " " + u.Author.LastName,
                             Translator = bts != null ? trl.Name + " " + trl.Family : "",
                             Category = bct != null ? cog.CategoryName : "",
                             l.LanguageName,
                             u.Book.BookID,
                             u.Book.ISBN,
                             u.Book.IsPublish,
                             u.Book.Price,
                             u.Book.PublishDate,
                             u.Book.Publisher.PublisherName,
                             u.Book.Stock,
                             u.Book.Title,
                         }).Where(a => a.Author.Contains(Author)
                         && a.Translator.Contains(Translator)
                         && a.Category.Contains(Category))
                         //.IgnoreQueryFilters() -- Ignorance of query filter has typed in BookShopContext
                         .AsEnumerable() // -- AsEnumerable() needs to be added before grouping, because you need items before grouping.
                         .GroupBy(b => b.BookID)
                         .Select(g => new { BookID = g.Key, BookGroups = g});

            foreach (var item in Books)
            {
                AuthorsName = "";
                TranslatorName = "";
                CategoryName = "";
                foreach (var group in item.BookGroups.Select(a => a.Author).Distinct())
                {
                    if (AuthorsName == "")
                        AuthorsName = group;
                    else
                        AuthorsName = AuthorsName + " - " + group;
                }

                foreach (var group in item.BookGroups.Select(a => a.Translator).Distinct())
                {
                    if (TranslatorName == "")
                        TranslatorName = group;
                    else
                        TranslatorName = TranslatorName + " - " + group;
                }

                foreach (var group in item.BookGroups.Select(a => a.Category).Distinct())
                {
                    if (CategoryName == "")
                        CategoryName = group;
                    else
                        CategoryName = CategoryName + " - " + group;
                }

                BooksIndexViewModel VM = new BooksIndexViewModel()
                {
                    Author = AuthorsName,
                    BookID = item.BookID,
                    ISBN = item.BookGroups.First().ISBN,
                    Title = item.BookGroups.First().Title,
                    Price = item.BookGroups.First().Price,
                    IsPublish = item.BookGroups.First().IsPublish == true ? "منتشر شده":"پیش نویس",
                    PublishDate = item.BookGroups.First().PublishDate != null ? _convertDate.ConvertMiladiToShamsi((DateTime)item.BookGroups.First().PublishDate,"dddd d MMMM yyyy ساعت HH:mm:ss") : "",
                    PublisherName = item.BookGroups.First().PublisherName,
                    Stock = item.BookGroups.First().Stock,
                    Translator = TranslatorName,
                    Category = CategoryName,
                    Language = item.BookGroups.First().LanguageName,
                };
                ViewModel.Add(VM);
            }
            return ViewModel;
        }
    
        public async Task<bool> CreateBookAsync(BooksCreateEditViewModel ViewModel)
        {
            try
            {
                byte[] Image = null;
                if (!string.IsNullOrWhiteSpace(ViewModel.ImageBase64))
                {
                    Image = Convert.FromBase64String(ViewModel.ImageBase64);
                }
                List<Book_Translator> translators = new List<Book_Translator>();
                List<Book_Category> categories = new List<Book_Category>();
                if (ViewModel.TranslatorID != null)
                    translators = ViewModel.TranslatorID.Select(a => new Book_Translator { TranslatorID = a }).ToList();
                if (ViewModel.CategoryID != null)
                    categories = ViewModel.CategoryID.Select(a => new Book_Category { CategoryID = a }).ToList();

                DateTime? PublishDate = null;

                if (ViewModel.IsPublish == true)
                {
                    PublishDate = DateTime.Now;
                }
                Book book = new Book()
                {
                    //Delete = false,
                    ISBN = ViewModel.ISBN,
                    IsPublish = ViewModel.IsPublish,
                    NumOfPages = ViewModel.NumOfPages,
                    Stock = ViewModel.Stock,
                    Price = ViewModel.Price,
                    LanguageID = ViewModel.LanguageID,
                    Summary = ViewModel.Summary,
                    Title = ViewModel.Title,
                    Image = Image,
                    PublishYear = ViewModel.PublishYear,
                    PublishDate = PublishDate,
                    Weight = ViewModel.Weight,
                    PublisherID = ViewModel.PublisherID,
                    Author_Books = ViewModel.AuthorID.Select(a => new Author_Book { AuthorID = a }).ToList(),
                    book_Tranlators = translators,
                    book_Categories = categories,
                    File = ViewModel.FileName,
                };

                //Start - Save Image to Database
                if (ViewModel.Image != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        string FileExtension = Path.GetExtension(ViewModel.Image.FileName);
                        await ViewModel.Image.CopyToAsync(memoryStream);
                        var types = FileExtensions.FileType.Image;
                        bool result = FileExtensions.IsValidFile(memoryStream.ToArray(), types, FileExtension.Replace('.', ' '));
                        book.Image = memoryStream.ToArray();
                        if (result)
                            book.Image = memoryStream.ToArray();
                    }
                }
                //End - Save Image to Database

                await _UW.BaseRepository<Book>().CreateAsync(book);
                await _UW.Commit();

                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<EntityOperationResult> EditBookAsync(BooksCreateEditViewModel ViewModel)
        {
            try
            {
                var Book = await _UW.BaseRepository<Book>().FindByIdAsync(ViewModel.BookID);
                if(Book != null)
                {
                    DateTime? PublishDate;
                    if (ViewModel.IsPublish == true && Book.IsPublish == false)
                    {
                        PublishDate = DateTime.Now;
                    }
                    else if (Book.IsPublish == true && ViewModel.IsPublish == false)
                    {
                        PublishDate = null;
                    }
                    else
                    {
                        PublishDate = ViewModel.PublishDate;
                    }
                    Book.BookID = ViewModel.BookID;
                    Book.BookID = ViewModel.BookID;
                    Book.Title = ViewModel.Title;
                    Book.ISBN = ViewModel.ISBN;
                    Book.NumOfPages = ViewModel.NumOfPages;
                    Book.Price = ViewModel.Price;
                    Book.Stock = ViewModel.Stock;
                    Book.IsPublish = ViewModel.IsPublish;
                    Book.LanguageID = ViewModel.LanguageID;
                    Book.PublisherID = ViewModel.PublisherID;
                    Book.PublishYear = ViewModel.PublishYear;
                    Book.Summary = ViewModel.Summary;
                    Book.Weight = ViewModel.Weight;
                    if (ViewModel.PublishDate == null && ViewModel.IsPublish == true)
                        Book.PublishDate = DateTime.Now;
                    Book.File = ViewModel.FileName;
                    Book.Delete = false;

                    var RecentAuthors = (from a in _UW._Context.Author_Books
                                         where (a.BookID == ViewModel.BookID)
                                         select a.AuthorID).ToArray();

                    var RecentTranslators = (from a in _UW._Context.Book_Translators
                                             where (a.BookID == ViewModel.BookID)
                                             select a.TranslatorID).ToArray();

                    var RecentCategories = (from c in _UW._Context.Book_Categories
                                            where (c.BookID == ViewModel.BookID)
                                            select c.CategoryID).ToArray();

                    if (ViewModel.TranslatorID == null)
                        ViewModel.TranslatorID = new int[] { };
                    if (ViewModel.CategoryID == null)
                        ViewModel.CategoryID = new int[] { };
                    var DeletedAuthors = RecentAuthors.Except(ViewModel.AuthorID);
                    var DeletedTranslators = RecentTranslators.Except(ViewModel.TranslatorID);
                    var DeletedCategories = RecentCategories.Except(ViewModel.CategoryID);

                    var AddedAuthors = ViewModel.AuthorID.Except(RecentAuthors);
                    var AddedTranslators = ViewModel.TranslatorID.Except(RecentTranslators);
                    var AddedCategories = ViewModel.CategoryID.Except(RecentCategories);

                    if (DeletedAuthors.Count() != 0)
                        _UW.BaseRepository<Author_Book>().DeleteRange(DeletedAuthors.Select(a => new Author_Book { AuthorID = a, BookID = ViewModel.BookID }).ToList());

                    if (DeletedTranslators.Count() != 0)
                        _UW.BaseRepository<Book_Translator>().DeleteRange(DeletedTranslators.Select(a => new Book_Translator { TranslatorID = a, BookID = ViewModel.BookID }).ToList());

                    if (DeletedCategories.Count() != 0)
                        _UW.BaseRepository<Book_Category>().DeleteRange(DeletedCategories.Select(a => new Book_Category { CategoryID = a, BookID = ViewModel.BookID }).ToList());

                    if (AddedAuthors.Count() != 0)
                        await _UW.BaseRepository<Author_Book>().CreateRange(AddedAuthors.Select(a => new Author_Book { AuthorID = a, BookID = ViewModel.BookID }).ToList());

                    if (AddedTranslators.Count() != 0)
                        await _UW.BaseRepository<Book_Translator>().CreateRange(AddedTranslators.Select(a => new Book_Translator { TranslatorID = a, BookID = ViewModel.BookID }).ToList());

                    if (AddedCategories.Count() != 0)
                        await _UW.BaseRepository<Book_Category>().CreateRange(AddedCategories.Select(a => new Book_Category { CategoryID = a, BookID = ViewModel.BookID }).ToList());

                    await _UW.Commit();
                    return new EntityOperationResult(true,null);
                }
                else
                    return new EntityOperationResult(false, new List<string>() { "کتابی یافت نشد" });
            }
            catch(Exception ex)
            {
                //return new EntityOperationResult(false,new List<string>() { ex.Message});
                return new EntityOperationResult(false, new List<string>() { "در انجام عملیات خطایی رخ داده است" });
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var Book = await _UW.BaseRepository<Book>().FindByIdAsync(id);
            if (Book != null)
            {
                Book.Delete = true;
                await _UW.Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<UploadFileResult> UploadFileAsync(IFormFile file,string path)
        {
            string FileExtension = Path.GetExtension(file.FileName);
            var types = FileExtensions.FileType.PDF;
            bool result = true;
            using (var memory = new MemoryStream())
            {
                await file.CopyToAsync(memory);
                result = FileExtensions.IsValidFile(memory.ToArray(), types, FileExtension.Replace('.', ' '));
                if (result)
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return new UploadFileResult(true, null);
                }
                else
                    return new UploadFileResult(false,new List<string>() {"فایل انتخاب شده معتبر نمی باشد" });
                    
            }
        }

        public string CheckFileName(string fileName)
        {
            string FileExtension = Path.GetExtension(fileName);
            int FileNameCount = _UW.BaseRepository<Book>().FindByConditionAsync(f => f.File == fileName).Result.Count();
            int j = 1;
            while (FileNameCount != 0)
            {
                fileName = fileName.Replace(FileExtension, "") + j + FileExtension;
                FileNameCount = _UW.BaseRepository<Book>().FindByConditionAsync(f => f.File == fileName).Result.Count();
                j++;
            }
            return fileName;
        }
    }

}
