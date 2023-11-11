using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace PhoneStore.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public string Display { get; set; }

        public string OS { get; set; }

        public string Proccessor { get; set; }

        public string Memory { get; set; }

        public string Camera { get; set; }

        public double Price { get; set; }

        public double PriceForSale { get; set; }

        public bool IsInStock { get; set; }

        public bool IsSale { get; set; }

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }


        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }



    }
}
