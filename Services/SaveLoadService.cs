using ATimeVisualCalculator.Models.TimeFilters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ATimeVisualCalculator.Services
{
    public static class TimelineSaveManager
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public static string GetDefaultSavePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "AVTiC");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, "saves.json");
        }

        public static void Save(Models.SaveData data, string? path = null)
        {
            path ??= GetDefaultSavePath();
            string dir = Path.GetDirectoryName(path)!;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string json = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(path, json);
        }

        public static Models.SaveData ConvertToSaveData(IEnumerable<Models.TimeManager> managers)
        {
            var saveData = new Models.SaveData();
            foreach (var manager in managers)
            {
                var calcData = new Models.CalculationSaveData
                {
                    Name = manager.Name
                };

                foreach (var entry in manager.TimeLines)
                {
                    var entrySave = new Models.TimelineEntrySaveData
                    {
                        Name = entry.Timeline.Name,
                        FilterType = entry.Filter.Type,
                        Entries = (bool[])entry.Timeline.Entries.Clone()
                    };
                    calcData.Timelines.Add(entrySave);
                }

                saveData.Calculations.Add(calcData);
            }
            return saveData;
        }
    }

    public static class TimelineLoadManager
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public static Models.SaveData Load(string? path = null)
        {
            path ??= TimelineSaveManager.GetDefaultSavePath();

            if (!File.Exists(path))
                return new Models.SaveData();

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Models.SaveData>(json, _options) ?? new Models.SaveData();
        }

        public static List<Models.TimeManager> ConvertFromSaveData(Models.SaveData saveData)
        {
            var managers = new List<Models.TimeManager>();

            foreach (var calcData in saveData.Calculations)
            {
                var manager = new Models.TimeManager(calcData.Name);

                foreach (var entrySave in calcData.Timelines)
                {
                    var timeline = new Models.Timeline(entrySave.Entries, entrySave.Name);
                    var filter = TimeFilterFactory.Create(entrySave.FilterType);
                    manager.AddTimeline(timeline, filter);
                }

                managers.Add(manager);
            }

            return managers;
        }
    }
}
