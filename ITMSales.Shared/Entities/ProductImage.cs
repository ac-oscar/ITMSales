using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace ITMSales.Shared.Entities
{
	public class ProductImage
	{
        public int Id { get; set; }

        public Product Product { get; set; } = null!;

        public int ProductId { get; set; }

        [Display(Name = "Imagen")]
        public string Image { get; set; } = null!;
    }
}

