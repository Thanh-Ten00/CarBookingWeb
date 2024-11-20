﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CarBooking.DataAccess.data;
using CarBooking.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CarBooking.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDBcontext _db;

        internal DbSet<T> dbSet;

        public Repository(ApplicationDBcontext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
            //for connect foreignkey auto
            _db.Seats.Include(u => u.Car);


        }

        public void Add(T entity)
        {
           dbSet.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null,bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query = dbSet;
                
            }
            else
            {
                query = dbSet.AsNoTracking();
                
            }
            query = query.Where(filter);
                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includeprop in includeProperties
                        .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeprop);
                    }
                }
                return query.FirstOrDefault();
        }
            
        

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if(filter!= null)
            {
                query = query.Where(filter);
            }
            
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach(var includeprop in includeProperties
                    .Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)) 
                { 
                    query = query.Include(includeprop);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);
        }
    }
}

