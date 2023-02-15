using CommonTools;
using CommonTools.Math;
using Qos.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.SceneObjects
{
    public interface IChangableData
    {
        bool HasChanged { get; }
    }

    /// <summary>
    /// Любой, кто предоставляет ТОЛЬКО ОДИН набор расположений игроков.
    /// </summary>
    /// <remarks>
    /// ВНИМАНИЕ! Если потенциальный имплементатор предоставляет несколько разных наборов , то
    /// НЕ ИСПОЛЬЗОВАТЬ данный интерфейс, а взамен написать другой, чтобы избежать предоставления не тех карт.
    /// </remarks>
    public interface IPlayerTFormsProvider
    {
        DictionaryData<PlayerId, Transform> PlayerTForms { get; }
    }



    /// <summary>
    /// Предоставляет доступ к словарю типа <see cref="DictionaryData{K, V}"/>.
    /// </summary>
    /// <remarks>
    /// ВНИМАНИЕ! Использовать этот интерфейс, только если его потенциальный имплементатор должен предоставлять доступ 
    /// ИСКЛЮЧИТЕЛЬНО к одному единственому <see cref="DictionaryData{K, V}"/> (в ином случае ради избежания неоднозначностей следует писать другой интерфейс).
    /// </remarks>
    public interface IDictionaryDataProvider<K, V>
        where V : struct
    {
        DictionaryData<K, V> DictionaryOutput { get; }
    }


    public static class IDictionaryDataProviderExtentions
    {
        public static V GetFor<K, V>(this IDictionaryDataProvider<K, V> dataProvider, K key)
            where V : struct
        {
            return dataProvider.DictionaryOutput.Items[key];
        }
    }


    /// <summary>
    /// Предоставляет доступ к данным типа <see cref="SingleData{V}"/>.
    /// </summary>
    /// <remarks>
    /// ВНИМАНИЕ! Использовать этот интерфейс, только если его потенциальный имплементатор должен предоставлять доступ 
    /// ИСКЛЮЧИТЕЛЬНО к одному единственому <see cref="SingleData{V}"/> (в ином случае ради избежания неоднозначностей следует писать другой интерфейс).
    /// </remarks>
    public interface ISingleDataProvider<V>
        where V : struct
    {
        SingleData<V> SingleOutput { get; }
    }


    /// <summary>
    /// Предоставляет доступ к данным типа <see cref="ListData{V}"/>.
    /// </summary>
    /// <remarks>
    /// ВНИМАНИЕ! Использовать этот интерфейс, только если его потенциальный имплементатор должен предоставлять доступ
    /// ИСКЛЮЧИТЕЛЬНО к одному единственому <see cref="ListData{V}"/> (в ином случае ради избежания неоднозначностей следует писать другой интерфейс).
    /// </remarks>
    public interface IHashSetDataProvider<T>
        where T : struct
    {
        ListData<T> HashSetOutput { get; }
    }


    /// <summary>
    /// Данные, изменения которых можно отвлеживать.
    /// </summary>
    public abstract class ChangableData : IChangableData
    {
        private static List<ChangableData> AllObjects = new List<ChangableData>();

        public event Action<ChangableData> OnChanged;

        public bool HasChanged { get; private set; }


        public ChangableData()
        {
            AllObjects.Add(this);
        }


        public static void MarkAllAsUnchanged()
        {
            foreach (var obj in AllObjects)
            {
                obj.HasChanged = false;
                obj.OnMarkingAsUnchanged();
            }

        }

        protected void MarkAsChanged()
        {
            HasChanged = true;
            OnChanged?.Invoke(this);
        }

        protected virtual void OnMarkingAsUnchanged() { }
    }



    /// <summary>
    /// Словарь "ключ-значение", изменения в котором можно отслеживать.
    /// </summary>
    public class DictionaryData<K, V> : ChangableData
        where V : struct
    {
        private Dictionary<K, V> _addedOrChanged = new Dictionary<K, V>();
        private List<K> _removed = new List<K>();
        private Dictionary<K, V> _items = new Dictionary<K, V>();

        public ReadOnlyDictionary<K, V> AddedOrChanged { get; private set; }
        public ReadOnlyCollection<K> Removed { get; private set; }
        public ReadOnlyDictionary<K, V> Items { get; private set; }



        public DictionaryData(out Editable editable)
        {
            editable = new Editable(SetItem, RemoveItem, Clear);

            AddedOrChanged = new ReadOnlyDictionary<K, V>(_addedOrChanged);
            Removed = new ReadOnlyCollection<K>(_removed);
            Items = new ReadOnlyDictionary<K, V>(_items);
        }


        private void SetItem(K key, V value)
        {
            Logger.Verbose($"Значение для '{key}' изменено на '{value}'.");
            _items[key] = value;
            _addedOrChanged[key] = value;
            _removed.Remove(key); // если элемент добавлен или изменен, то он не может быть удаленным
            MarkAsChanged();
        }


        private void RemoveItem(K key)
        {
            Logger.Verbose($"Удаление элемента '{key}'.");
            _items.Remove(key);
            _addedOrChanged.Remove(key); // если элемент удален, то он не может быть добавленным или измененным
            _removed.Add(key);
            MarkAsChanged();
        }


        private void Clear()
        {
            Logger.Verbose($"Полная чистка: '{_items.MembersToString()}'.");
            _removed.AddRange(_items.Keys);
            _addedOrChanged.Clear(); // если все элементы удалены, то никакой из них не может быть добавленным или измененным
            _items.Clear();
            MarkAsChanged();
        }


        protected override void OnMarkingAsUnchanged()
        {
            base.OnMarkingAsUnchanged();
            _addedOrChanged.Clear();
            _removed.Clear();
        }


        public class Editable
        {
            private Action<K, V> _SetItem;
            private Action<K> _RemoveItem;
            private Action _Clear;


            public Editable(Action<K, V> SetItem, Action<K> RemoveItem, Action Clear)
            {
                _SetItem = SetItem;
                _RemoveItem = RemoveItem;
                _Clear = Clear;
            }

            public void SetItem(K key, V value) => _SetItem(key, value);
            public void RemoveItem(K item) => _RemoveItem(item);
            public void Clear() => _Clear();
        }
    }



    /// <summary>
    /// Данные, изменения в которых можно отслеживать.
    /// </summary>
    public class SingleData<V> : ChangableData
        where V : struct
    {
        public V? Value { get; private set; }

        public SingleData(out Editable editable)
        {
            editable = new Editable(SetItem, ClearItem);
        }

        private void SetItem(V value)
        {
            Logger.Verbose($"Значение изменено на '{value}'.");
            Value = value;
            MarkAsChanged();
        }

        private void ClearItem()
        {
            Logger.Verbose($"Значение '{Value}' удалено.");
            Value = null;
            MarkAsChanged();
        }


        public class Editable
        {
            private Action<V> _SetItem;
            private Action _ClearItem;


            public Editable(Action<V> SetItem, Action ClearItem)
            {
                _SetItem = SetItem;
                _ClearItem = ClearItem;
            }

            public void Set(V value) => _SetItem(value);

            public void Clear() => _ClearItem();
        }
    }





    /// <summary>
    /// Набор элементов, изменения в котором можно отслеживать.
    /// </summary>
    public class ListData<V> : ChangableData
            where V : struct
    {
        private List<V> _added = new List<V>();
        private List<V> _removed = new List<V>();
        private List<V> _items = new List<V>();


        public ReadOnlyCollection<V> Added { get; private set; }
        public ReadOnlyCollection<V> Removed { get; private set; }
        public ReadOnlyCollection<V> Items { get; private set; }


        public ListData(out Editable editable)
        {
            editable = new Editable(AddItem, RemoveItem);

            Added = new ReadOnlyCollection<V>(_added);
            Removed = new ReadOnlyCollection<V>(_removed);
            Items = new ReadOnlyCollection<V>(_items);
        }


        private void AddItem(V value)
        {
            _items.Add(value);
            _added.Add(value); 
            _removed.Remove(value); // если элемент добавлен, то он не может быть удаленным
            MarkAsChanged();
        }


        private void RemoveItem(V value)
        {
            _items.Remove(value);
            _removed.Add(value);
            _added.Remove(value); // если элемент удалн, то он не может быть добавленным
            MarkAsChanged();
        }


        protected override void OnMarkingAsUnchanged()
        {
            base.OnMarkingAsUnchanged();
            _added.Clear();
            _removed.Clear();
        }


        public class Editable
        {
            private Action<V> _AddItem;
            private Action<V> _RemoveItem;


            public Editable(Action<V> AddItem, Action<V> RemoveItem)
            {
                _AddItem = AddItem;
                _RemoveItem = RemoveItem;
            }

            public void AddItem(V item) => _AddItem(item);
            public void RemoveItem(V item) => _RemoveItem(item);
        }
    }



    public class LookupData<Key, Val> : ChangableData
        where Val : struct
    {
        private Dictionary<Key, ListData<Val>> _sets = new Dictionary<Key, ListData<Val>>();
        private Dictionary<Key, ListData<Val>.Editable> _setEdits = new Dictionary<Key, ListData<Val>.Editable>();

        public ImmutableHashSet<Key> Added { get; private set; } = ImmutableHashSet.Create<Key>();
        public ImmutableHashSet<Key> Changed { get; private set; } = ImmutableHashSet.Create<Key>();
        public ImmutableHashSet<Key> Removed { get; private set; } = ImmutableHashSet.Create<Key>();


        public LookupData(out Editable editable)
        {
            editable = new Editable(AddSet, GetSet, RemoveSet, AddToSet, RemoveFromSet);
        }


        private ListData<Val> AddSet(Key key)
        {
            var newSet = new ListData<Val>(out var newSetEdit);
            _sets.Add(key, newSet);
            _setEdits.Add(key, newSetEdit);

            newSet.OnChanged += OnSomeSetChanged;

            Added = Added.Add(key);
            NotMarkAsChanged(key);
            NotMarkAsRemoved(key);
            
            MarkAsChanged();

            return newSet;
        }

        public ListData<Val> GetSet(Key key) => _sets[key];


        private void RemoveSet(Key key)
        {
            _sets[key].OnChanged += OnSomeSetChanged;

            _sets.Remove(key);
            _setEdits.Remove(key);

            Removed = Removed.Add(key);
            NotMarkAsAdded(key);
            NotMarkAsRemoved(key);

            MarkAsChanged();
        }


        private void OnSomeSetChanged(ChangableData obj)
        {
            var key = _sets.Single(i => i.Value == obj).Key;
            if (!Changed.Contains(key))
            {
                Changed = Changed.Add(key);
            }

            MarkAsChanged();
        }

        private void NotMarkAsAdded(Key key)
        {
            if (Added.Contains(key))
            {
                Added = Added.Remove(key);
            }
        }

        private void NotMarkAsChanged(Key key)
        {
            if (Changed.Contains(key))
            {
                Changed = Changed.Remove(key);
            }
        }

        private void NotMarkAsRemoved(Key key)
        {
            if (Removed.Contains(key))
            {
                Removed = Removed.Remove(key);
            }
        }


        private void AddToSet(Key key, Val value)
        {
            _setEdits[key].AddItem(value);
        }


        private void RemoveFromSet(Key key, Val value)
        {
            _setEdits[key].RemoveItem(value);
        }


        protected override void OnMarkingAsUnchanged()
        {
            base.OnMarkingAsUnchanged();
            Added = Added.Clear();
            Removed = Removed.Clear();
            Changed = Changed.Clear();
        }


        public class Editable
        {
            private readonly Func<Key, ListData<Val>> _AddSet;
            private readonly Func<Key, ListData<Val>> _GetSet;
            private readonly Action<Key> _RemoveSet;
            private readonly Action<Key,Val> _AddToSet;
            private readonly Action<Key,Val> _RemoveFromSet;


            public Editable(
                Func<Key, ListData<Val>> AddSet,
                Func<Key, ListData<Val>> GetSet,
                Action<Key> RemoveSet,
                Action<Key, Val> AddToSet,
                Action<Key, Val> RemoveFromSet)
            {
                _AddSet = AddSet;
                _GetSet = GetSet;
                _RemoveSet = RemoveSet;
                _AddToSet = AddToSet;
                _RemoveFromSet = RemoveFromSet;
            }


            public ListData<Val> AddSet(Key key) => _AddSet(key);
            public void RemoveSet(Key key) => _RemoveSet(key);
            public void AddToSet(Key key, Val value) => _AddToSet(key, value);  
            public void RemoveFromSet(Key key, Val value) => _RemoveFromSet(key, value);  
        }
    }
}
