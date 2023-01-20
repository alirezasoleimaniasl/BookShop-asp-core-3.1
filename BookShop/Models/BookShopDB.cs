using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Models
{
    [Table("BookInfo")]
    public class Book
    {
        private Language _language;
        private Publisher _publisher;
        private ILazyLoader LazyLoader { get; set; }
        public Book()
        {

        }
        private Book(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        [Key]
        public int BookID { get; set; }

        [Required]
        public string Title { get; set; }
        public string Summary { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
        public string File { get; set; }
        public int NumOfPages { get; set; }
        public short Weight { get; set; }
        public string ISBN { get; set; }
        public bool? IsPublish { get; set; }
        public DateTime? PublishDate { get; set; }
        public int PublishYear { get; set; }
        public bool? Delete { get; set; }
        public int PublisherID { get; set; }

        [Column(TypeName ="image")]
        public byte[] Image { get; set; }
        public int LanguageID { get; set; }
        public Language Language 
        {
            get => LazyLoader.Load(this, ref _language);
            set => _language = value;
        }
        public Discount Discount { get; set; }
        public List<Author_Book> Author_Books { get; set; }
        public List<Order_Book> Order_Books { get; set; }
        public List<Book_Translator> book_Tranlators { get; set; }
        public List<Book_Category> book_Categories { get; set; }
        public Publisher Publisher 
        {
            get => LazyLoader.Load(this, ref _publisher);
            set => _publisher = value;
        }
    }

    public class Book_Category
    {
        public int BookID { get; set; }
        public int CategoryID { get; set; }

        public Book Book { get; set; }
        public Category Category { get; set; }
    }


    public class Publisher
    {
        [Key]
        public int PublisherID { get; set; }
        [Display(Name ="نام ناشر")]
        public string PublisherName { get; set; }

        public List<Book> Books { get; set; }
    }

    public class Book_Translator
    {
        public int TranslatorID { get; set; }
        public int BookID { get; set; }

        public Book Book { get; set; }
        public Translator Translator { get; set; }
    }

    public class Translator
    {
        [Key]
        public int TranslatorID { get; set; }
        [Display(Name = "نام")]
        public string Name { get; set; }
        [Display(Name = "نام خانوادگی")]
        public string Family { get; set; }

        public List<Book_Translator> book_Tranlators { get; set; }
    }

    public class Author_Book
    {
        private ILazyLoader LazyLoader { get; set; }
        private Book _Book;
        public Author_Book()
        {

        }
        private Author_Book(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        public int BookID { get; set; }
        public int AuthorID { get; set; }

        public Book Book 
        {
            get => LazyLoader.Load(this, ref _Book);
            set => _Book = value;
        }
        public Author Author { get; set; }
    }

    public class Author
    {
        private ILazyLoader LazyLoader { get; set; }
        private List<Author_Book> _Author_Books;
        public Author() 
        { 
        }
        private Author(ILazyLoader lazyLoader)
        {
            LazyLoader = lazyLoader;
        }
        [Key]
        public int AuthorID { get; set; }
        [Display(Name ="نام")]
        public string FirstName { get; set; }
        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        public List<Author_Book> Author_Books 
        {
            //get;set;
            get => LazyLoader.Load(this, ref _Author_Books);
            set => _Author_Books = value;
        }
    }

    public class Discount
    {
        [Key,ForeignKey("Book")]
        public int BookID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public byte Percent { get; set; }

        public Book Book { get; set; }
    }

    public class Language
    {
        public int LanguageID { get; set; }
        [Display(Name = "زبان")]
        public string LanguageName { get; set; }

        public List<Book> Books { get; set; }
    }

    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }

        [ForeignKey("category")]
        public int? ParentCategoryID { get; set; }

        public Category category { get; set; }
        public List<Category> categories { get; set; }
        public List<Book_Category> book_Categories { get; set; }
    }


    public class Order
    {
        public string OrderID { get; set; }
        public long AmountPaid { get; set; }
        public string DispatchNumber { get; set; }
        public DateTime BuyDate { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public Customer Customer { get; set; }
        public List<Order_Book> Order_Books { get; set; }
    }

    public class Order_Book
    {
        public string OrderID { get; set; }
        public int BookID { get; set; }

        public Order Order { get; set; }
        public Book Book { get; set; }
    }


    public class OrderStatus
    {
        public int OrderStatusID { get; set; }
        public string OrderStatusName { get; set; }

        public List<Order> Orders { get; set; }
    }

    public class Customer
    {
        public string CustomerID { get; set; }
        //These fields are similar to ASP.NET Identity Section
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public DateTime BirthDate { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        //public string Mobile { get; set; }
        public string Tell { get; set; }
        public string Image { get; set; }

        //public int Age { get; set; }
        public string PostalCode1 { get; set; }
        public string PostalCode2 { get; set; }

        public int CityID1 { get; set; }
        public int CityID2 { get; set; }

        public City city1 { get; set; }
        public City city2 { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class Province
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int ProvinceID { get; set; }
        [Display(Name ="نام استان")]
        public string ProvinceName { get; set; }

        public List<City> City { get; set; }
    }

    public class City
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public int CityID { get; set; }
        public string CityName { get; set; }
        [ForeignKey("Province")]
        public int? ProvinceID { get; set; }

        public Province Province { get; set; }
        public List<Customer> Customers1 { get; set; }
        public List<Customer> Customers2 { get; set; }
    }
}
