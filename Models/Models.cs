using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;

namespace Cocodrinks.Models
{
    public class CocodrinksContext : DbContext
    {
       
        public CocodrinksContext (DbContextOptions<CocodrinksContext> options)
            : base(options)
        {
            //TODO initializer options
        }

        public DbSet<Cocodrinks.Models.User> Users { get; set; }
        public DbSet<Cocodrinks.Models.Article> Articles { get; set; }
        public DbSet<Cocodrinks.Models.Order> Orders { get; set; }
        public DbSet<Cocodrinks.Models.Onderdelen> Onderdelen { get; set; }
        public DbSet<Cocodrinks.Models.OrderLine> OrderLines { get; set; }
        public DbSet<Cocodrinks.Models.Image> Images { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderLine>()
                .HasOne(p => p.Order)
                .WithMany(b => b.OrderLines)
                .HasForeignKey(p => p.OrderId);
        }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User")]
        public string Name { get; set; }

        
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }
    }
    public class User
    {
        public User(){
            this.CreateDate = DateTime.Now;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }
        
        public int Logincount { get; set; }

        public int AccessLevel { get; set; }
    }

    public class Article
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }       
        public string Imagelocation { get; set; } 

        //public string color {get; set;}
    }

    public class Onderdelen
    {
        public int id { get; set; }
        public string type { get; set; }
        public int hoeveelheid { get; set; }      

    }

    public class Order
    {
        public Order() 
        {
            this.OrderLines = new HashSet<OrderLine>();
            this.CreateDate = DateTime.Now;
            this.DeliveryDate = DateTime.Now.AddDays(7);
        }
        public int Id { get; set; }
        public string BankAccount { get; set; }
        public string Comment { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; } 

        [DataType(DataType.Date)]
        public DateTime DeliveryDate { get; set; }     
        public User User { get; set; }
        public int UserId { get; set; }
        public ICollection<OrderLine> OrderLines { get; set; }
    }

    //the view is not for the database
    public class OrderView : Order{
        [Display(Name = "Amount")]
        public int newLineQuantity{get; set;}

        [Display(Name = "Article")]
        public int newLineArticleId{get; set;}

        public String addArticle{get;set;}
        public ICollection<Article> Articles { get; set; }
    }

    public class OrderLine{
        public int Id { get; set; }
        public int Quantity { get; set; }

        public int ArticleId { get; set; }
        public Article Article{ get; set; }

        public int OrderId { get; set; }
        public Order Order{get; set; }


    }

    public class AdminViewModel
    {
        private CocodrinksContext _context;

        public AdminViewModel()
        {

        }

        public AdminViewModel(CocodrinksContext context)
        {
            _context = context;
            Orders = _context.Orders.Include(o => o.OrderLines).ToList();
            Articles = _context.Articles.ToList();
            Users = _context.Users.ToList();
        }

        public ICollection<Order> Orders { get; set; }
        public ICollection<Article> Articles { get; set; }
        public ICollection<User> Users { get; set; }
    }

    public class FileUpload
    {
        [Required]
        [Display(Name="Title")]
        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }

        [Required]
        [Display(Name="Image file")]
        public IFormFile UploadImage { get; set; }

    }

    public class Image
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public string ImageName { get; set; }

        [Display(Name = "Image Size (bytes)")]
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public long ImageSize { get; set; }

        [Display(Name = "Uploaded (UTC)")]
        [DisplayFormat(DataFormatString = "{0:F}")]
        public DateTime UploadDT { get; set; }
    }

    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new CocodrinksContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<CocodrinksContext>>()))
            {
                if (context.Articles.Any())
                {
                    return;   // DB has been seeded
                }

                context.Articles.AddRange(
                    new Article
                    {
                        Name = "red drink",
                        Description = "Romantic Drink",
                        Imagelocation = "/media/pinksoda.png"
                    },
                    new Article
                    {
                        Name = "black drink",
                        Description = "Dark drink",
                        Imagelocation = "/media/bluesoda.png"
                    }

                   
                );
                 context.Users.AddRange(
                    new User
                    {
                        Name = "admin",
                        Password = "admin",
                        Email = "admin@admin.admin",
                        AccessLevel = 0
                    },
                    new User
                    {
                        Name = "test",
                        Password = "admin",
                        Email = "test@test.test",
                        AccessLevel = 10
                    }
                );

                 context.Onderdelen.AddRange(
                    new Onderdelen
                    {
                        type = "Moertjes",
                        hoeveelheid = 5000
                    },
                    new Onderdelen
                    {
                        type = "Wastrommels",
                        hoeveelheid = 44
                    },
                    new Onderdelen
                    {
                        type = "Stekkers",
                        hoeveelheid = 98
                    },
                    new Onderdelen
                    {
                        type = "Water filters",
                        hoeveelheid = 280
                    },
                    new Onderdelen
                    {
                        type = "Stof filters",
                        hoeveelheid = 143
                    },
                    new Onderdelen
                    {
                        type = "Knopjes",
                        hoeveelheid = 2500
                    },
                    new Onderdelen
                    {
                        type = "Zekeringen",
                        hoeveelheid = 3149
                    }
                );
                context.SaveChanges();
            }
        }
    }
}