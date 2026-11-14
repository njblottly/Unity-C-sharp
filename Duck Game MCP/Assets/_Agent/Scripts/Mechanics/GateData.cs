using System;
using System.Linq;
using UnityEngine;
namespace DuckAdventure.Agent
{
    [CreateAssetMenu(menuName="Duck Adventure/Coin Gate")]
    public sealed class GateData:ScriptableObject
    {
        public string id;
        public int cost=3;
        public string[] requiredMechanics=new string[0];
        public void ValidateData()
        {
            if(string.IsNullOrWhiteSpace(id)||cost<1||cost>3||requiredMechanics==null||requiredMechanics.Any(string.IsNullOrWhiteSpace)||requiredMechanics.Distinct().Count()!=requiredMechanics.Length)throw new InvalidOperationException("Invalid gate definition: "+name);
        }
    }
}
