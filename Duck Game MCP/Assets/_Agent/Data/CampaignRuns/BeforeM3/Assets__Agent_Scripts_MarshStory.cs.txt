using System;
using UnityEngine;

namespace DuckAdventure.Agent
{
    [CreateAssetMenu(menuName = "Duck Adventure/Marsh Story")]
    public sealed class MarshStory : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            public string id;
            public string speaker;
            public bool oncePerRun;
            public string requiredCapability = "";
            public string[] prerequisites = new string[0];
            [TextArea(3, 8)] public string[] pages;
        }

        [SerializeField] private Entry[] entries;
        public Entry[] Entries => entries;

        public void ValidateData()
        {
            if (entries == null || entries.Length == 0) throw new InvalidOperationException("Story has no entries.");
            var ids = new System.Collections.Generic.HashSet<string>();
            foreach (Entry entry in entries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.id) || !ids.Add(entry.id) || string.IsNullOrWhiteSpace(entry.speaker) || entry.pages == null || entry.pages.Length == 0 || entry.prerequisites == null)
                    throw new InvalidOperationException("Invalid/duplicate story entry in " + name);
                foreach (string page in entry.pages) if (string.IsNullOrWhiteSpace(page)) throw new InvalidOperationException("Empty story page: " + entry.id);
            }
            foreach (Entry entry in entries) foreach (string id in entry.prerequisites) Get(id);
        }

        public Entry Get(string id)
        {
            foreach (Entry entry in entries) if (entry.id == id) return entry;
            throw new InvalidOperationException("Missing story entry: " + id);
        }
    }
}
