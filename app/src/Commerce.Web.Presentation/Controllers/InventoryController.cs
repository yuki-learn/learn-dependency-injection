using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Ploeh.Samples.Commerce.Domain;
using Ploeh.Samples.Commerce.Domain.Commands;
using Ploeh.Samples.Commerce.Web.Presentation.Models;

namespace Ploeh.Samples.Commerce.Web.Presentation.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IProductRepository repository;
        private readonly IInventoryRepository inventoryRepository;
        private readonly ICommandService<AdjustInventory> inventoryAdjuster;

        public InventoryController(
            IProductRepository repository, IInventoryRepository inventoryRepository, ICommandService<AdjustInventory> inventoryAdjuster)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (inventoryRepository == null) throw new ArgumentNullException(nameof(inventoryRepository));
            if (inventoryAdjuster == null) throw new ArgumentNullException(nameof(inventoryAdjuster));

            this.repository = repository;
            this.inventoryRepository = inventoryRepository;
            this.inventoryAdjuster = inventoryAdjuster;
        }

        [Route("inventory/")]
        public ActionResult Index()
        {
            return this.View(this.Populate(new AdjustInventoryViewModel()));
        }

        [Route("inventory/adjustinventory")]
        public ActionResult AdjustInventory(AdjustInventoryViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(nameof(Index), this.Populate(viewModel));
            }

            AdjustInventory command = viewModel.Command;

            this.inventoryAdjuster.Execute(command);

            this.TempData["SuccessMessage"] = "Inventory successfully adjusted.";

            return this.RedirectToAction(nameof(HomeController.Index), "Home");
        }

        private AdjustInventoryViewModel Populate(AdjustInventoryViewModel vm)
        {
            var products = this.repository.GetAll();
            vm.Products = products.Join(this.inventoryRepository.GetAll(), (a) => a.Id, (b) => b.Id, (a, b) => new ProductInventoryInfo { ProductId = a.Id, ProductName = a.Name, Quantity = b.Quantity }).ToArray();
            vm.ProductsItem = products.Select(x => new SelectListItem(x.Name, x.Id.ToString()));

            vm.DecreaseOptions = new[]
            {
                new SelectListItem("Yes", bool.TrueString),
                new SelectListItem("No", bool.FalseString)
            };
            return vm;
        }
    }
}