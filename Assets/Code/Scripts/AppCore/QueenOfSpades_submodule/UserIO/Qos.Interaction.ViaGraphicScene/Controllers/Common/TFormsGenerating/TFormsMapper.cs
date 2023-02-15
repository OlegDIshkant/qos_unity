using CommonTools.Math;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    /// <summary>
    /// Вспомогательный класс учавствующий в процессе генерации <see cref="Transform"/>-ов для определенных объектов <typeparamref name="Key"/>.
    /// Он запоминает для каких именно объектов генерировать <see cref="Transform"/>, и знает, к какому объекту какой из сгенерированных <see cref="Transform"/> будет относится. 
    /// </summary>
    public class TFormsMapper<Key>
    {
        private readonly ICalcIndexStrategy<Key> _calcIndexStrategy;

        private Dictionary<Key, int> _objectIndicies;

        public int ObjectsAmmount => _objectIndicies.Count;
        public ReadOnlyDictionary<Key, int> ObjectIndicies;


        public TFormsMapper(ICalcIndexStrategy<Key> calcIndexStrategy)
        {
            _objectIndicies = new Dictionary<Key, int>();
            ObjectIndicies = new ReadOnlyDictionary<Key, int>(_objectIndicies);
            _calcIndexStrategy = calcIndexStrategy;
        }


        public TFormsMapper() : this(new SimpleCalcIndexStrategy<Key>())
        {
        }


        public void RememberObjects(IEnumerable<Key> keys)
        {
            CheckForNonExistance(keys);
            _calcIndexStrategy.AddToDictionary(keys, _objectIndicies);
        }


        public void ForgetObjects(IEnumerable<Key> keys)
        {
            CheckForExistance(keys);
            _calcIndexStrategy.RemoveFromDictionary(keys, _objectIndicies);
        }


        public void ForgetAllKnownObjects() => ForgetObjects(_objectIndicies.Keys.ToList());


        /// <summary>
        /// Сопоставляет сгенерированным <paramref name="transforms"/> объекты <typeparamref name="Key"/>
        /// (добавленные ранее через метод <see cref="RememberObjects(IEnumerable{Key})"/>).
        /// </summary>
        public Dictionary<Key, Transform> Map(Transform[] transforms)
        {
            return _objectIndicies.ToDictionary(item => item.Key, item => transforms[item.Value]);
        }


        private void CheckForNonExistance(IEnumerable<Key> keys)
        {
            foreach (var key in keys)
            {
                if (_objectIndicies.Keys.Contains(key))
                {
                    throw new System.Exception($"Key '{key}' is already known.");
                }
            }
        }


        private void CheckForExistance(IEnumerable<Key> keys)
        {
            foreach (var key in keys)
            {
                if (!_objectIndicies.Keys.Contains(key))
                {
                    throw new System.Exception($"Key '{key}' is not known.");
                }
            }
        }


        /// <summary>
        /// Узнать порядковый номер для определенного объекта <paramref name="key"/>.
        /// </summary>
        public int MapIndex(Key key) => _objectIndicies[key];


        /// <summary>
        /// Узнать, какому объекту <typeparamref name="Key"/> соответствует тот или иной порядковый номер.
        /// </summary>
        public Key MapId(int index) => _objectIndicies.Where(item => item.Value == index).Single().Key;

    }





    /// <summary>
    /// Стратегия, пересчитывающая индексы для ключей.
    /// </summary>
    public interface ICalcIndexStrategy<Key>
    {
        void AddToDictionary(IEnumerable<Key> keys, Dictionary<Key, int> dictionary);
        void RemoveFromDictionary(IEnumerable<Key> keys, Dictionary<Key, int> dictionary);
    }





    /// <summary>
    /// Стратегия, дающая ключам простые индексы (без какой-либо хитрой логики), как правило в порядке их добавления.
    /// </summary>
    public class SimpleCalcIndexStrategy<Key> : ICalcIndexStrategy<Key>
    {
        public void AddToDictionary(IEnumerable<Key> keys, Dictionary<Key, int> dictionary)
        {
            foreach (var key in keys)
            {
                AssertValidArgs(key);
                dictionary.Add(key, dictionary.Count);
            }

            void AssertValidArgs(Key key)
            {
                Trace.Assert(!dictionary.ContainsKey(key));
            }
        }


        public void RemoveFromDictionary(IEnumerable<Key> keys, Dictionary<Key, int> dictionary)
        {
            foreach (var key in keys)
            {
                AssertValidArgs(key);
                dictionary.Remove(key);
            }

            int i = 0;
            var newIndicies = dictionary
                .OrderBy(item => item.Value)
                .ToDictionary(item => item.Key, _ => i++);

            foreach (var item in newIndicies)
            {
                dictionary[item.Key] = item.Value;
            }

            void AssertValidArgs(Key key)
            {
                Trace.Assert(dictionary.ContainsKey(key));
            }
        }
    }

}
