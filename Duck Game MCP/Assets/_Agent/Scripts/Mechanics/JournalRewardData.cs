using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    [CreateAssetMenu(menuName="Duck Adventure/Journal Rewards")]
    public sealed class JournalRewardData:ScriptableObject
    {
        public int badgeThreshold=4,codaThreshold=8;
        [TextArea] public string coda;
        public void ValidateData(int marks){if(badgeThreshold<=0||codaThreshold<badgeThreshold||codaThreshold>marks||string.IsNullOrWhiteSpace(coda))throw new InvalidOperationException("Invalid cosmetic journal rewards.");}
    }
}
