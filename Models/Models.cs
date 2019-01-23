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
            //Database.SetInitializer<CocodrinksContext>(new CreateDatabaseIfNotExists<CocodrinksContext>());
            
        }

        public DbSet<Cocodrinks.Models.User> Users { get; set; }
        public DbSet<Cocodrinks.Models.Article> Articles { get; set; }
        public DbSet<Cocodrinks.Models.Order> Orders { get; set; }

        public DbSet<Cocodrinks.Models.OrderLine> OrderLines { get; set; }
        public DbSet<Cocodrinks.Models.Image> Images { get; set; }
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
    }
    public class User
    {
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
    }

    public class Order
    {
        public Order() 
        {
            this.OrderLines = new HashSet<OrderLine>();
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

    public class OrderLine{
        public int Id { get; set; }
        public int Quantity { get; set; }

        public int OrderId { get; set; }

        public Order Order{get; set; }

        public int ArticleId { get; set; }
        public Article Article{ get; set; }
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
                context.SaveChanges();
            }
        }
    }
}