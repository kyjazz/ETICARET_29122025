using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface ICommentService
    {
        Comment GetById(int id); //belirli bir yorumu getirir
        List<Comment> GetAll(); //tüm yorumları getirir
        List<Comment> GetByProductId(int productId); //ürüne göre yorumları getirir
        void Create(Comment entity); //yeni yorum oluşturur
        void Update(Comment entity); //var olan yorumu günceller
        void Delete(Comment entity); //var olan yorumu siler
    }
}
