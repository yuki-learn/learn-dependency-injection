using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ploeh.Samples.Commerce.Domain;
using Ploeh.Samples.Commerce.Domain.Commands;

namespace Ploeh.Samples.Commerce.Web.Presentation.Models
{
    public class AdjustInventoryViewModel
    {
        public IEnumerable<SelectListItem> ProductsItem { get; set; }
        public IEnumerable<SelectListItem> DecreaseOptions { get; set; }
        public IEnumerable<ProductInventoryInfo> Products { get; set; }

        [Required]
        public AdjustInventory Command { get; set; }
    }

    public class ProductInventoryInfo
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}