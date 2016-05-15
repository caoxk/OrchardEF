using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Orchard.Logging;
using Orchard.Utility.Extensions;

namespace Orchard.Data {
    public class Repository<T> : IRepository<T> where T : class {
        private readonly ITransactionManager _transactionManager;

        public Repository(ITransactionManager transactionManager) {
            _transactionManager = transactionManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        protected virtual DbContext Session {
            get { return _transactionManager.GetSession(); }
        }

        public virtual DbSet<T> Table
        {
            get { return Session.Set<T>(); }
        }

        #region IRepository<T> Members

        void IRepository<T>.Create(T entity) {
            Create(entity);
        }

        void IRepository<T>.Update(T entity) {
            Update(entity);
        }

        void IRepository<T>.Delete(T entity) {
            Delete(entity);
        }

        void IRepository<T>.Copy(T source, T target) {
            Copy(source, target);
        }

        void IRepository<T>.Flush() {
            Flush();
        }

        T IRepository<T>.Get(int id) {
            return Get(id);
        }

        T IRepository<T>.Get(Expression<Func<T, bool>> predicate) {
            return Get(predicate);
        }

        IQueryable<T> IRepository<T>.Table {
            get { return Table; }
        }

        int IRepository<T>.Count(Expression<Func<T, bool>> predicate) {
            return Count(predicate);
        }

        IEnumerable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate) {
            return Fetch(predicate).ToReadOnlyCollection();
        }

        IEnumerable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order) {
            return Fetch(predicate, order).ToReadOnlyCollection();
        }

        IEnumerable<T> IRepository<T>.Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip,
                                            int count) {
            return Fetch(predicate, order, skip, count).ToReadOnlyCollection();
        }

        #endregion

        public virtual T Get(int id) {
            return Table.Find(id);
        }

        public virtual T Get(Expression<Func<T, bool>> predicate) {
            return Fetch(predicate).SingleOrDefault();
        }

        public virtual void Create(T entity) {
            Logger.Debug("Create {0}", entity);
            Table.Add(entity);
            Flush();
        }

        public virtual void Update(T entity) {
            Logger.Debug("Update {0}", entity);
            Table.Attach(entity);
            Session.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(T entity) {
            Logger.Debug("Delete {0}", entity);
            Table.Remove(entity);
            Flush();          
        }

        public virtual void Copy(T source, T target) {
            Logger.Debug("Copy {0} {1}", source, target);
            //var entityType = Session.Model.FindEntityType(typeof(T));

            //var metadata = Session.SessionFactory.GetClassMetadata(typeof (T));
            //var values = metadata.GetPropertyValues(source, EntityMode.Poco);

            ////This method is currently only used by StorageVersionFilter<>.Versioning()
            ////In order to prevent shared references to the same collection instance
            ////Instances of IList<> need to be copied to a new collection instance
            //for (var index = 0; index < values.Length; index++) {
            //    var value = values[index];
            //    if (value == null)
            //        continue;
                
            //    var type = value.GetType();
            //    var isGenericList = type.GetInterfaces()
            //        .Where(i => i.IsGenericType)
            //        .Any(i => i.GetGenericTypeDefinition() == typeof(IList<>));

            //    if(!isGenericList)
            //        continue;

            //    var genericArgument = type.GetGenericArguments().First();
            //    var genericType = typeof(List<>).MakeGenericType(new[] { genericArgument });

            //    var listValues = ((IList)value);
            //    values[index] = Activator.CreateInstance(genericType, new[] { listValues });
            //}

            //metadata.SetPropertyValues(target, values, EntityMode.Poco);
        }

        public virtual void Flush() {
            Session.SaveChanges();
        }

        public virtual int Count(Expression<Func<T, bool>> predicate) {
            return Fetch(predicate).Count();
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate) {
            return Table.Where(predicate);
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order) {
            var orderable = new Orderable<T>(Fetch(predicate));
            order(orderable);
            return orderable.Queryable;
        }

        public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int skip,
                                           int count) {
            return Fetch(predicate, order).Skip(skip).Take(count);
        }
    }
}