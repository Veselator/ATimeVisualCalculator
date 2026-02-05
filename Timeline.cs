using System.Text;

namespace ATimeVisualCalculator
{
    public class Timeline
    {
        // Модель данных для конкретной временной шкалы

        public string Name { get; set; }

        // 48 - каждый по пол часа, всего 24 часа

        private bool[] _entries = new bool[48];
        public bool[] Entries => _entries;

        public bool this[int index] => GetEntry(index);
        public int Length => _entries.Length;

        // string - шкала в тексте (для WPF)
        // int - индекс изменившегося поля

        public Action<string, int> OnTimelineChanged;

        public Timeline()
        {
            for (int i = 0; i < _entries.Length; i++)
            {
                _entries[i] = true;
            }
        }

        public Timeline(bool[] entries, string name = "New timeline")
        {
            Name = name;

            for (int i = 0; i < Math.Min(_entries.Length, entries.Length); i++)
            {
                _entries[i] = true;
            }
        }

        public void SetEntry(int index, bool value)
        {
            if (index < 0 || index >= _entries.Length)
            {
                return;
            }

            _entries[index] = value;
            OnTimelineChanged?.Invoke(ToString(), index);
        }

        public void Reset()
        {
            for (int i = 0; i < _entries.Length; i++)
            {
                _entries[i] = true;
            }
        }

        public bool GetEntry(int index)
        {
            if (index < 0 || index >= _entries.Length)
            {
                return false;
            }

            return _entries[index];
        }

        public void CopyFrom(Timeline other)
        {
            for (int i = 0; i < _entries.Length; i++)
            {
                _entries[i] = other[i];
            }
        }

        public void CopyFrom(bool[] arr)
        {
            for (int i = 0; i < Math.Min(arr.Length, _entries.Length); i++)
            {
                _entries[i] = arr[i];
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(48);
            for (int i = 0; i < _entries.Length; i++)
            {
                sb.Append(_entries[i] ? '1' : '0');
            }

            return sb.ToString();
        }
    }
}
