using ETICARET.DataAccess.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class EfCoreGenericRepository<T, TContext> : IRepository<T> where T : class where TContext : DbContext, new()
    {
        //yeni bir nesne ekler
        public void Create(T entity)
        {
            using (var context = new TContext())
            {
                context.Set<T>().Add(entity);
                context.SaveChanges();
            }
        }

        //var olan nesneyi siler
        public virtual void Delete(T entity)
        {
            using (var context = new TContext())
            {
                context.Set<T>().Remove(entity);
                context.SaveChanges();
            }
        }

        //tüm nesneleri getirir
        public virtual List<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            using(var context = new TContext())
            {
                return filter == null ? context.Set<T>().ToList(): context.Set<T>().Where(filter).ToList();
            }
        }

        //Id ile nesne getirir
        public T GetById(int id)
        {
            using (var context = new TContext())
            {
                return context.Set<T>().Find(id);
            }
        }
        //belirtilen filtreye göre tek bir nesne getirir
        public T GetOne(Expression<Func<T, bool>> filter = null)
        {
            using (var context = new TContext())
            {
                return context.Set<T>().Where(filter).FirstOrDefault();
            }
        }

        //var olan nesneyi günceller
        public virtual void Update(T entity)
        {
            using (var context = new TContext())
            {
                context.Entry(entity).State = EntityState.Modified; //varlık durumunu modified olarak işaretler
                context.SaveChanges();
            }
        }
    }
}
