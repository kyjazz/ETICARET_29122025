using ETICARET.Business.Abstract;
using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Concrete
{
    public class CommentManager : ICommentService
    {
        private ICommentDal _commentDal;
        public CommentManager(ICommentDal commentDal)
        {
            _commentDal = commentDal;
        }
        public void Create(Comment entity)
        {
            _commentDal.Create(entity);
        }

        public void Delete(Comment entity)
        {
            _commentDal.Delete(entity);
        }

        public Comment GetById(int id)
        {
            return _commentDal.GetById(id);
        }

        public List<Comment> GetAll()
        {
            return _commentDal.GetAll();
        }

        public List<Comment> GetByProductId(int productId)
        {
            return _commentDal.GetAll(x => x.ProductId == productId);
        }

        public void Update(Comment entity)
        {
            _commentDal.Update(entity);
        }
    }
}
