using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    [CreateAssetMenu(menuName="Duck Adventure/Reed Mark")]
    public sealed class CollectibleData:ScriptableObject
    {
        public string id,title;
        [TextArea] public string memory;
        public Sprite sprite;
        public void ValidateData(){if(string.IsNullOrWhiteSpace(id)||string.IsNullOrWhiteSpace(title)||string.IsNullOrWhiteSpace(memory)||sprite==null)throw new InvalidOperationException("Invalid Reed Mark: "+name);}
    }
}
