using EBookStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBookStore.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company category);

    }
}
