using System;
using System.Collections.Generic;

namespace Ploeh.Samples.Commerce.Domain
{
    public interface IInventoryRepository
    {
        IEnumerable<ProductInventory> GetAll();
        ProductInventory GetByIdOrNull(Guid id);
        void Save(ProductInventory productInventory);
    }
}