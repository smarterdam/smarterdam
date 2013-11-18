using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoRepository;

namespace Smarterdam.DataAccess
{
    public class MongoRepositoryAdapter<T> : IRepository<T> where T:IEntity<string>
    {
        private readonly MongoRepository<T> _repository;

        public MongoRepositoryAdapter(string collectionName)
        {
            _repository = new MongoRepository<T>("mongodb://localhost/smarterdam", collectionName);
        }

        public T GetById(string id)
        {
            return _repository.GetById(id);
        }

        public T Add(T entity)
        {
            return _repository.Add(entity);
        }

        public void Add(IEnumerable<T> entities)
        {
            _repository.Add(entities);
        }

        public T Update(T entity)
        {
            return _repository.Update(entity);
        }

        public void Update(IEnumerable<T> entities)
        {
            _repository.Update(entities);
        }

        public void Delete(string id)
        {
            _repository.Delete(id);
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
        }

        public void Delete(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            _repository.Delete(predicate);
        }

        public void DeleteAll()
        {
            _repository.DeleteAll();
        }

        public long Count()
        {
            return _repository.Count();
        }

        public bool Exists(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return _repository.Exists(predicate);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _repository.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _repository.GetEnumerator();
        }

        public Type ElementType
        {
            get { return _repository.ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return _repository.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _repository.Provider; }
        }
    }
}
