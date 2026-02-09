using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace ATimeVisualCalculator.Models
{
    public class Timeline : INotifyPropertyChanged
    {
        private string _name = "New timeline";
        private bool[] _entries = new bool[48];

        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public bool[] Entries => _entries;

        public bool[] EntriesForSerialization
        {
            get => _entries;
            set
            {
                if (value != null)
                {
                    for (int i = 0; i < Math.Min(_entries.Length, value.Length); i++)
                        _entries[i] = value[i];
                }
            }
        }

        [JsonIgnore]
        public bool this[int index] => GetEntry(index);

        [JsonIgnore]
        public int Length => _entries.Length;

        [JsonIgnore]
        public Action<string, int>? OnTimelineChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Timeline()
        {
            for (int i = 0; i < _entries.Length; i++)
                _entries[i] = true;
        }

        public Timeline(bool[] entries, string name = "New timeline")
        {
            _name = name;
            for (int i = 0; i < _entries.Length; i++)
                _entries[i] = true;

            for (int i = 0; i < Math.Min(_entries.Length, entries.Length); i++)
                _entries[i] = entries[i];
        }

        public void SetEntry(int index, bool value)
        {
            if (index < 0 || index >= _entries.Length) return;
            _entries[index] = value;
            OnTimelineChanged?.Invoke(ToString(), index);
            OnPropertyChanged(nameof(Entries));
        }

        public void ToggleEntry(int index)
        {
            if (index < 0 || index >= _entries.Length) return;
            _entries[index] = !_entries[index];
            OnTimelineChanged?.Invoke(ToString(), index);
            OnPropertyChanged(nameof(Entries));
        }

        public void Reset()
        {
            for (int i = 0; i < _entries.Length; i++)
                _entries[i] = true;
            OnPropertyChanged(nameof(Entries));
        }

        public bool GetEntry(int index)
        {
            if (index < 0 || index >= _entries.Length) return false;
            return _entries[index];
        }

        public void CopyFrom(Timeline other)
        {
            for (int i = 0; i < _entries.Length; i++)
                _entries[i] = other[i];
            OnPropertyChanged(nameof(Entries));
        }

        public void CopyFrom(bool[] arr)
        {
            for (int i = 0; i < Math.Min(arr.Length, _entries.Length); i++)
                _entries[i] = arr[i];
            OnPropertyChanged(nameof(Entries));
        }

        public override string ToString()
        {
            StringBuilder sb = new(48);
            for (int i = 0; i < _entries.Length; i++)
                sb.Append(_entries[i] ? '1' : '0');
            return sb.ToString();
        }

        public void ShiftTimeline(int offset)
        {
            bool[] temp = new bool[48];
            for (int i = 0; i < _entries.Length; i++)
            {
                int newIndex = (i - offset) % 48;
                while (newIndex < 0) newIndex += 48;
                temp[i] = _entries[newIndex];
            }
            _entries = temp;
            OnPropertyChanged(nameof(Entries));
        }

        public int GreenCount
        {
            get
            {
                int c = 0;
                foreach (var e in _entries) if (e) c++;
                return c;
            }
        }

        public int RedCount => 48 - GreenCount;

        public double GreenHours => GreenCount * 0.5;
        public double RedHours => RedCount * 0.5;

        public void NotifyAllChanged()
        {
            OnPropertyChanged(nameof(Entries));
            OnPropertyChanged(nameof(GreenCount));
            OnPropertyChanged(nameof(RedCount));
            OnPropertyChanged(nameof(GreenHours));
            OnPropertyChanged(nameof(RedHours));
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
