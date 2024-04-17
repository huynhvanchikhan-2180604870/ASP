﻿using System.ComponentModel.DataAnnotations;

namespace SchoolSuppliesStore.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
