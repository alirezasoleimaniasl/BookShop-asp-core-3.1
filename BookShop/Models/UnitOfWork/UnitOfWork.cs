using BookShop.Models.Repository;
using System.Threading.Tasks;

namespace BookShop.Models.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public BookShopContext _Context { get; }
        private IBookRepository _IBookRepository;
        public UnitOfWork(BookShopContext Context)
        {
            _Context = Context;
        }

        public IRepositoryBase<TEntity> BaseRepository<TEntity>() where TEntity : class
        {
            IRepositoryBase<TEntity> repository = new RepositoryBase<TEntity, BookShopContext>(_Context);
            return repository;
        }

        public IBookRepository BookRepository
        {
            get
            {
                if (_IBookRepository == null)
                {
                    _IBookRepository = new BookRepository(_Context);
                }
                return _IBookRepository;
            }
        }

        public async Task Commit()
        {
            await _Context.SaveChangesAsync();
        }
    }
}
