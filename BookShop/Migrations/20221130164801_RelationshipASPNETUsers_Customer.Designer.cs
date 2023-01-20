﻿// <auto-generated />
using System;
using BookShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookShop.Migrations
{
    [DbContext(typeof(BookShopContext))]
    [Migration("20221130164801_RelationshipASPNETUsers_Customer")]
    partial class RelationshipASPNETUsers_Customer
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.30")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BookShop.Models.Author", b =>
                {
                    b.Property<int>("AuthorID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuthorID");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("BookShop.Models.Author_Book", b =>
                {
                    b.Property<int>("BookID")
                        .HasColumnType("int");

                    b.Property<int>("AuthorID")
                        .HasColumnType("int");

                    b.HasKey("BookID", "AuthorID");

                    b.HasIndex("AuthorID");

                    b.ToTable("Author_Books");
                });

            modelBuilder.Entity("BookShop.Models.Book", b =>
                {
                    b.Property<int>("BookID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool?>("Delete")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValueSql("0");

                    b.Property<string>("File")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Image")
                        .HasColumnType("image");

                    b.Property<bool?>("IsPublish")
                        .HasColumnType("bit");

                    b.Property<int>("LanguageID")
                        .HasColumnType("int");

                    b.Property<int>("NumOfPages")
                        .HasColumnType("int");

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PublishDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PublishYear")
                        .HasColumnType("int");

                    b.Property<int>("PublisherID")
                        .HasColumnType("int");

                    b.Property<int>("Stock")
                        .HasColumnType("int");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<short>("Weight")
                        .HasColumnType("smallint");

                    b.HasKey("BookID");

                    b.HasIndex("LanguageID");

                    b.HasIndex("PublisherID");

                    b.ToTable("BookInfo");
                });

            modelBuilder.Entity("BookShop.Models.Book_Category", b =>
                {
                    b.Property<int>("BookID")
                        .HasColumnType("int");

                    b.Property<int>("CategoryID")
                        .HasColumnType("int");

                    b.HasKey("BookID", "CategoryID");

                    b.HasIndex("CategoryID");

                    b.ToTable("Book_Categories");
                });

            modelBuilder.Entity("BookShop.Models.Book_Translator", b =>
                {
                    b.Property<int>("BookID")
                        .HasColumnType("int");

                    b.Property<int>("TranslatorID")
                        .HasColumnType("int");

                    b.HasKey("BookID", "TranslatorID");

                    b.HasIndex("TranslatorID");

                    b.ToTable("Book_Translators");
                });

            modelBuilder.Entity("BookShop.Models.Category", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CategoryName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ParentCategoryID")
                        .HasColumnType("int");

                    b.HasKey("CategoryID");

                    b.HasIndex("ParentCategoryID");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("BookShop.Models.City", b =>
                {
                    b.Property<int>("CityID")
                        .HasColumnType("int");

                    b.Property<string>("CityName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProvinceID")
                        .HasColumnType("int");

                    b.HasKey("CityID");

                    b.HasIndex("ProvinceID");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("BookShop.Models.Customer", b =>
                {
                    b.Property<string>("CustomerID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CityID1")
                        .HasColumnType("int");

                    b.Property<int>("CityID2")
                        .HasColumnType("int");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Tell")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CustomerID");

                    b.HasIndex("CityID1");

                    b.HasIndex("CityID2");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("BookShop.Models.Discount", b =>
                {
                    b.Property<int>("BookID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<byte>("Percent")
                        .HasColumnType("tinyint");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("BookID");

                    b.ToTable("Discounts");
                });

            modelBuilder.Entity("BookShop.Models.Language", b =>
                {
                    b.Property<int>("LanguageID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("LanguageName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LanguageID");

                    b.ToTable("Languages");
                });

            modelBuilder.Entity("BookShop.Models.Order", b =>
                {
                    b.Property<string>("OrderID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("AmountPaid")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("BuyDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DispatchNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OrderStatusID")
                        .HasColumnType("int");

                    b.HasKey("OrderID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("OrderStatusID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("BookShop.Models.OrderStatus", b =>
                {
                    b.Property<int>("OrderStatusID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("OrderStatusName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrderStatusID");

                    b.ToTable("OrderStatuses");
                });

            modelBuilder.Entity("BookShop.Models.Order_Book", b =>
                {
                    b.Property<string>("OrderID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("BookID")
                        .HasColumnType("int");

                    b.HasKey("OrderID", "BookID");

                    b.HasIndex("BookID");

                    b.ToTable("Order_Books");
                });

            modelBuilder.Entity("BookShop.Models.Province", b =>
                {
                    b.Property<int>("ProvinceID")
                        .HasColumnType("int");

                    b.Property<string>("ProvinceName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProvinceID");

                    b.ToTable("Provinces");
                });

            modelBuilder.Entity("BookShop.Models.Publisher", b =>
                {
                    b.Property<int>("PublisherID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("PublisherName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PublisherID");

                    b.ToTable("Publishers");
                });

            modelBuilder.Entity("BookShop.Models.Translator", b =>
                {
                    b.Property<int>("TranslatorID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Family")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TranslatorID");

                    b.ToTable("Translator");
                });

            modelBuilder.Entity("BookShop.Models.Author_Book", b =>
                {
                    b.HasOne("BookShop.Models.Author", "Author")
                        .WithMany("Author_Books")
                        .HasForeignKey("AuthorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookShop.Models.Book", "Book")
                        .WithMany("Author_Books")
                        .HasForeignKey("BookID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookShop.Models.Book", b =>
                {
                    b.HasOne("BookShop.Models.Language", "Language")
                        .WithMany("Books")
                        .HasForeignKey("LanguageID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookShop.Models.Publisher", "Publisher")
                        .WithMany("Books")
                        .HasForeignKey("PublisherID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookShop.Models.Book_Category", b =>
                {
                    b.HasOne("BookShop.Models.Book", "Book")
                        .WithMany("book_Categories")
                        .HasForeignKey("BookID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookShop.Models.Category", "Category")
                        .WithMany("book_Categories")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookShop.Models.Book_Translator", b =>
                {
                    b.HasOne("BookShop.Models.Book", "Book")
                        .WithMany("book_Tranlators")
                        .HasForeignKey("BookID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookShop.Models.Translator", "Translator")
                        .WithMany("book_Tranlators")
                        .HasForeignKey("TranslatorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookShop.Models.Category", b =>
                {
                    b.HasOne("BookShop.Models.Category", "category")
                        .WithMany("categories")
                        .HasForeignKey("ParentCategoryID");
                });

            modelBuilder.Entity("BookShop.Models.City", b =>
                {
                    b.HasOne("BookShop.Models.Province", "Province")
                        .WithMany("City")
                        .HasForeignKey("ProvinceID");
                });

            modelBuilder.Entity("BookShop.Models.Customer", b =>
                {
                    b.HasOne("BookShop.Models.City", "city1")
                        .WithMany("Customers1")
                        .HasForeignKey("CityID1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookShop.Models.City", "city2")
                        .WithMany("Customers2")
                        .HasForeignKey("CityID2")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("BookShop.Models.Discount", b =>
                {
                    b.HasOne("BookShop.Models.Book", "Book")
                        .WithOne("Discount")
                        .HasForeignKey("BookShop.Models.Discount", "BookID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BookShop.Models.Order", b =>
                {
                    b.HasOne("BookShop.Models.Customer", "Customer")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerID");

                    b.HasOne("BookShop.Models.OrderStatus", "OrderStatus")
                        .WithMany("Orders")
                        .HasForeignKey("OrderStatusID");
                });

            modelBuilder.Entity("BookShop.Models.Order_Book", b =>
                {
                    b.HasOne("BookShop.Models.Book", "Book")
                        .WithMany("Order_Books")
                        .HasForeignKey("BookID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookShop.Models.Order", "Order")
                        .WithMany("Order_Books")
                        .HasForeignKey("OrderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
