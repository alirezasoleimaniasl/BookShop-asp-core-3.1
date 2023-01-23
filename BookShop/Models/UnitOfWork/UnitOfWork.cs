using BookShop.Classes;
using BookShop.Models.Repository;
using System.Threading.Tasks;

namespace BookShop.Models.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public BookShopContext _Context { get; }
        private IBookRepository _IBookRepository;
        private readonly IConvertDate _convertDate;
        public UnitOfWork(BookShopContext Context, IConvertDate convertDate)
        {
            _Context = Context;
            _convertDate = convertDate;
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
                    _IBookRepository = new BookRepository(this,_convertDate);
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
