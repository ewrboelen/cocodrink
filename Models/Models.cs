using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Cocodrinks.Models
{
    public class CocodrinksContext : DbContext
    {
        public CocodrinksContext (DbContextOptions<CocodrinksContext> options)
            : base(options)
        {
        }

        public DbSet<Cocodrinks.Models.User> User { get; set; }
        public DbSet<Cocodrinks.Models.Article> Articles { get; set; }
        public DbSet<Cocodrinks.Models.Order> Orders { get; set; }
        public DbSet<Cocodrinks.Models.Image> Images { get; set; }
    }
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }
        
        public int Logincount { get; set; }
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
        public int Id { get; set; }
        public string Comment { get; set; }       
        public User User { get; set; }
        public ICollection<Article> Articles { get; set; }
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
                        Imagelocation = "images/reddrink.jpg"
                    },
                    new Article
                    {
                        Name = "black drink",
                        Description = "Romantic Comedy",
                        Imagelocation = "images/reddrink.jpg"
                    }

                   
                );
                context.SaveChanges();
            }
        }
    }
}