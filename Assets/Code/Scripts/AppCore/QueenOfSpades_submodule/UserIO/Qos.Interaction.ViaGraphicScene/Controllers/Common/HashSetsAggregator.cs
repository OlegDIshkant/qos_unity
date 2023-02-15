using Qos.Interaction.ViaGraphicScene.SceneObjects;
using System.Collections.Generic;
using System.Linq;


namespace Qos.Interaction.ViaGraphicScene.Controllers.Common
{
    /// <summary>
    /// ����������, ������� ���������� �������� ������ ���������� <see cref="IHashSetDataProvider{Key}"/>.
    /// </summary>
    public class HashSetsAggregator<Key> : AbstractController, IHashSetDataProvider<Key>
        where Key : struct
    {
        /// <summary>
        /// ���� � ��� �� ���� ����� ���������� � ������ ����������. 
        /// � ���� ���� ��������� �� ������ ���������, �� ����, ��� �� �������� �� �������.
        /// ����� ��������, ����� ���� ������ �� ���� ����������, ����� ����� ���� ����� ��������� ����� � ������ ���������.
        /// </summary>
        private Dictionary<Key, int> _keysCounters = new Dictionary<Key, int>();
        private List<IHashSetDataProvider<Key>> _sources;


        private ListData<Key>.Editable _output;
        public ListData<Key> HashSetOutput { get; private set; }


        public HashSetsAggregator(Contexts contexts, IEnumerable<IHashSetDataProvider<Key>> sources) : 
            base(contexts)
        {
            _sources = sources.Distinct().ToList();
            HashSetOutput = new ListData<Key>(out _output);
        }


        public override void Update()
        {
            foreach (var source in ChangedDataSources())
            {
                foreach (var addedKey in source.Added)
                {
                    RememberKeyIfNeeded(addedKey);
                }

                foreach (var removed in source.Removed)
                {
                    ForgetKeyIfNeeded(removed);
                }
            }
        }


        private IEnumerable<ListData<Key>> ChangedDataSources()
        {
            foreach (var source in _sources)
            {
                if (source.HashSetOutput.HasChanged)
                {
                    yield return source.HashSetOutput;
                }
            }
        }


        private void RememberKeyIfNeeded(Key key)
        {
            if (!_keysCounters.ContainsKey(key))
            {
                _keysCounters[key] = 0;
                _output.AddItem(key);
            }
            _keysCounters[key]++;
        }


        private void ForgetKeyIfNeeded(Key key)
        {
            _keysCounters[key]--;

            if (_keysCounters[key] <= 0)
            {
                _keysCounters.Remove(key);
                _output.RemoveItem(key);
            }
        }

    }

}