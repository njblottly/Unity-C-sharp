using System;
using System.Linq;
using UnityEngine;
namespace DuckAdventure.Agent
{
    [CreateAssetMenu(menuName = "Duck Adventure/Campaign")]
    public sealed class CampaignData : ScriptableObject
    {
        public ChapterData[] chapters;
        public SectionData[] sections;
        public MarshStory story;
        public int archiveGateIndex = 2;
        public string endingStory = "ending";
        public void ValidateData()
        {
            if (chapters == null || sections == null || chapters.Length != 5 || sections.Length != chapters.Length || story == null || archiveGateIndex < 0 || archiveGateIndex >= sections.Length)
                throw new InvalidOperationException("Campaign needs five chapters/sections, a story and a valid archive gate.");
            if (chapters.Any(c => c == null) || sections.Any(s => s == null)) throw new InvalidOperationException("Missing chapter or section asset.");
            if (chapters.Select(c => c.id).Distinct().Count() != chapters.Length || sections.Select(s => s.id).Distinct().Count() != sections.Length)
                throw new InvalidOperationException("Duplicate campaign chapter/section ID.");
            for (int i = 0; i < sections.Length; i++)
            {
                sections[i].ValidateData();
                if (sections[i].chapter != chapters[i]) throw new InvalidOperationException("Section/chapter order mismatch.");
                foreach (string id in sections[i].storyIds) story.Get(id);
            }
            story.ValidateData(); story.Get(endingStory);
        }
    }
}
