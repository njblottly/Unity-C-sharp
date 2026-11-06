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
            [TextArea(3, 8)] public string[] pages;
        }

        [SerializeField] private Entry[] entries;
        public Entry[] Entries => entries;

        public Entry Get(string id)
        {
            foreach (Entry entry in entries) if (entry.id == id) return entry;
            throw new InvalidOperationException("Missing story entry: " + id);
        }
    }
}
