using System;
using System.Collections.ObjectModel;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Threading;
using System.Runtime.Serialization;
using System.Windows;

namespace Aphrodite
{
    [Serializable]
    public class ObservableCollectionThreadSafe<T> : ObservableCollection<T>, ISerializable
    {
        private delegate void ClearHandler();
        private delegate void InsertHandler(int index, T item);
        private delegate void MoveHandler(int oldIndex, int newIndex);
        private delegate void RemoveHandler(int index);

        private Dispatcher _oDispatcher = null; // Non serializable

        public Dispatcher Dispatcher
        {
            get { return _oDispatcher; }
            set { _oDispatcher = value; }
        }

        // Construtor
        public ObservableCollectionThreadSafe()
            : base()
        {
            _oDispatcher = Application.Current.Dispatcher;
        }

        protected ObservableCollectionThreadSafe(SerializationInfo info, StreamingContext context)
            : base()
        {
            int intCount = info.GetInt32("COUNT");

            for (int i = 0; i < intCount; i++)
                this.Add((T)info.GetValue("ITEM_" + i.ToString(), typeof(T)));
        }

        // Serialization
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("COUNT", this.Count);

            for (int i = 0; i < this.Count; i++)
                info.AddValue("ITEM_" + i.ToString(), this.Items[i]);
        }

        // Overrides

        // -------------------------------------------------------------------
        protected override void ClearItems()
        {
            ClearHandler h = BaseClearItems;

            if (_oDispatcher != null && _oDispatcher.Thread != Thread.CurrentThread)
                _oDispatcher.Invoke(h, null);
            else
                BaseClearItems();
        }

        private void BaseClearItems()
        {
            base.ClearItems();
        }

        // -------------------------------------------------------------------
        protected override void InsertItem(int index, T item)
        {
            InsertHandler h = BaseInsertItem;

            if (_oDispatcher != null && _oDispatcher.Thread != Thread.CurrentThread)
                _oDispatcher.Invoke(h, new object[] { index, item });
            else
                BaseInsertItem(index, item);
        }

        private void BaseInsertItem(int index, T item)
        {
            base.InsertItem(index, item);
        }

        // -------------------------------------------------------------------
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            MoveHandler h = BaseMoveItem;

            if (_oDispatcher != null && _oDispatcher.Thread != Thread.CurrentThread)
                _oDispatcher.Invoke(h, new object[] { oldIndex, newIndex });
            else
                BaseMoveItem(oldIndex, newIndex);
        }

        private void BaseMoveItem(int oldIndex, int newIndex)
        {
            base.MoveItem(oldIndex, newIndex);
        }

        // -------------------------------------------------------------------
        protected override void RemoveItem(int index)
        {
            RemoveHandler h = BaseRemoveItem;

            if (_oDispatcher != null && _oDispatcher.Thread != Thread.CurrentThread)
                _oDispatcher.Invoke(h, new object[] { index });
            else
                BaseRemoveItem(index);
        }

        private void BaseRemoveItem(int index)
        {
            base.RemoveItem(index);
        }

        // -------------------------------------------------------------------
        protected override void SetItem(int index, T item)
        {
            InsertHandler h = BaseSetItem;

            if (_oDispatcher != null && _oDispatcher.Thread != Thread.CurrentThread)
                _oDispatcher.Invoke(h, new object[] { index, item });
            else
                BaseSetItem(index, item);
        }

        private void BaseSetItem(int index, T item)
        {
            base.SetItem(index, item);
        }
    }
}