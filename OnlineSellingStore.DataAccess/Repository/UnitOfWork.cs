﻿using OnlineSellingStore.DataAccess.Data;
using OnlineSellingStore.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineSellingStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Category = new CategoryRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}