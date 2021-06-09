using EBookStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBookStore.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        void Update(ShoppingCart obj);

    }
}
