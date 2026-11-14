using System;
using UnityEngine;
namespace DuckAdventure.Agent
{
    public enum MarshMechanicKind { Perch, InspectionLamp, MailRouting }
    [CreateAssetMenu(menuName="Duck Adventure/Mechanic")]
    public sealed class MechanicData:ScriptableObject
    {
        public string id,title;
        public MarshMechanicKind kind;
        public int cost;
        [TextArea] public string hint;
        [TextArea] public string journalText;
        public string prerequisite="";
        public void ValidateData()
        {
            if(string.IsNullOrWhiteSpace(id)||string.IsNullOrWhiteSpace(title)||string.IsNullOrWhiteSpace(hint)||string.IsNullOrWhiteSpace(journalText)||cost<0||cost>3||kind==MarshMechanicKind.Perch&&cost!=0||kind==MarshMechanicKind.InspectionLamp&&cost!=1||kind==MarshMechanicKind.MailRouting&&cost!=2)throw new InvalidOperationException("Invalid mechanic definition: "+name);
        }
    }
}
