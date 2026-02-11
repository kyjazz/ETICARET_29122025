using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    /// <summary>
    /// Veri erişim katmanı için genel CRUD işlemlerini tanımlayan generic repository arayüzü.
    /// Bu arayüz, farklı veri modelleri için standart veri erişim işlemlerini sağlar.
    /// </summary>
    /// <typeparam name="T">Repository'nin çalışacağı entity tipi</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Belirtilen ID'ye sahip tek bir nesneyi getirir.
        /// </summary>
        /// <param name="id">Getirilecek nesnenin benzersiz kimlik numarası</param>
        /// <returns>Belirtilen ID'ye sahip entity nesnesi</returns>
        T GetById(int id);

        /// <summary>
        /// Belirtilen koşula uyan ilk nesneyi getirir.
        /// </summary>
        /// <param name="filter">Filtreleme koşulunu belirten lambda ifadesi. Null ise ilk nesne döner.</param>
        /// <returns>Koşula uyan ilk entity nesnesi veya null</returns>
        T GetOne(Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Belirtilen koşula uyan tüm nesneleri getirir.
        /// </summary>
        /// <param name="filter">Filtreleme koşulunu belirten lambda ifadesi. Null ise tüm nesneler döner.</param>
        /// <returns>Koşula uyan entity nesnelerinin listesi</returns>
        List<T> GetAll(Expression<Func<T, bool>> filter = null);

        /// <summary>
        /// Veritabanına yeni bir nesne ekler.
        /// </summary>
        /// <param name="entity">Eklenecek entity nesnesi</param>
        void Create(T entity);

        /// <summary>
        /// Veritabanında var olan bir nesneyi günceller.
        /// </summary>
        /// <param name="entity">Güncellenecek entity nesnesi</param>
        void Update(T entity);

        /// <summary>
        /// Veritabanından var olan bir nesneyi siler.
        /// </summary>
        /// <param name="entity">Silinecek entity nesnesi</param>
        void Delete(T entity);
    }
}
/* Bu Katman Neden Var?
Bu yapı, veri erişim katmanında ortak olan CRUD (Create, Read, Update, Delete) işlemlerini standartlaştırmak için kullanılır.
Generic yapısı sayesinde, farklı veri modelleri için aynı arayüzü kullanarak kod tekrarını azaltır ve bakımını kolaylaştırır.
 */
