using EBookStore.DataAccess.Data;
using EBookStore.DataAccess.Repository.IRepository;
using EBookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBookStore.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

        public void Update(Company company)
        {
            var objFromDb = _db.Companies.FirstOrDefault(s=>s.Id == company.Id);
            if(objFromDb != null)
            {
                objFromDb.Name = company.Name;
            }

        }
    }
}
