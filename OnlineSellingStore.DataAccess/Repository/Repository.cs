using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OnlineSellingStore.DataAccess.Data;
using OnlineSellingStore.DataAccess.Repository.IRepository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OnlineSellingStore.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
            //_db.Categories == dbSet;
            _db.Products.Include(u => u.Category);
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T>? query;
            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }

            query = dbSet.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var property in includeProperties.
                    Split(new char[] { ',' }, StringSplitOptions.
                    RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault();
        }


        public IEnumerable<T> GetAll(Expression<Func<T, bool>> ? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null){
                query = dbSet.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach(var property in includeProperties.
                    Split(new char[] {',' }, StringSplitOptions.
                    RemoveEmptyEntries))
                { 
                    query = query.Include(property);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
