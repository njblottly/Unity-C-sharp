using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    [CreateAssetMenu(menuName = "Duck Adventure/Campaign Chapter")]
    public sealed class ChapterData : ScriptableObject
    {
        public string id;
        public string displayName;
        public string theme;
        [Range(1, 4)] public int act = 1;
        public float targetMinMinutes = 4;
        public float targetMaxMinutes = 5;
        public void ValidateData()
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(displayName) || targetMinMinutes <= 0 || targetMaxMinutes < targetMinMinutes)
                throw new InvalidOperationException("Invalid campaign chapter: " + name);
        }
    }
}
