using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Orchard.Settings.Utilities;

namespace Orchard.Settings {
    public class ContentPart : DynamicObject, IContent {

        public virtual ContentItem ContentItem { get; set; }

        /// <summary>
        /// The ContentItem's identifier.
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public int Id {
            get { return ContentItem.Id; }
        }

        public T Retrieve<T>(string fieldName) {
            return InfosetHelper.Retrieve<T>(this, fieldName);
        }

        public T RetrieveVersioned<T>(string fieldName) {
            return this.Retrieve<T>(fieldName, true);
        }

        public virtual void Store<T>(string fieldName, T value) {
            InfosetHelper.Store(this, fieldName, value);
        }

        public virtual void StoreVersioned<T>(string fieldName, T value) {
            this.Store(fieldName, value, true);
        }

    }

    public class ContentPart<TRecord> : ContentPart {

        protected TProperty Retrieve<TProperty>(Expression<Func<TRecord, TProperty>> targetExpression) {
            return InfosetHelper.Retrieve(this, targetExpression);
        }

        protected TProperty Retrieve<TProperty>(
            Expression<Func<TRecord, TProperty>> targetExpression,
            Func<TRecord, TProperty> defaultExpression) {

            return InfosetHelper.Retrieve(this, targetExpression, defaultExpression);
        }
        protected TProperty Retrieve<TProperty>(
                    Expression<Func<TRecord, TProperty>> targetExpression,
                    TProperty defaultValue) {

            return InfosetHelper.Retrieve(this, targetExpression, (Func<TRecord, TProperty>)(x => defaultValue));
        }

        protected ContentPart<TRecord> Store<TProperty>(
            Expression<Func<TRecord, TProperty>> targetExpression,
            TProperty value) {

            InfosetHelper.Store(this, targetExpression, value);
            return this;
        }

        public readonly LazyField<TRecord> _record = new LazyField<TRecord>();
        public TRecord Record { get { return _record.Value; } set { _record.Value = value; } }
    }

}
