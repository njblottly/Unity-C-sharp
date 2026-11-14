using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace DuckAdventure.Agent
{
    // Optional scene configuration on the existing level object, never a singleton.
    [DisallowMultipleComponent]
    public sealed class MarshCampaign : MonoBehaviour
    {
        [SerializeField] private MarshLevel level;
        [SerializeField] private CampaignData data;
        [SerializeField] private MarshNode[] mainGates;
        [SerializeField] private string[] capabilities = new string[0];
        [SerializeField] private MarshMechanic[] mechanics=new MarshMechanic[0];
        [SerializeField] private ReedMark[] marks=new ReedMark[0];
        [SerializeField] private JournalRewardData journalRewards;
        private readonly HashSet<string> collected=new HashSet<string>();
        public MarshMechanic[] Mechanics=>mechanics;
        public ReedMark[] Marks=>marks;
        public int ExtraEscrow=>mechanics.Sum(m=>m.CoinsInEscrow);
        public int MarkCount=>collected.Count;
        public bool HasBadge=>journalRewards!=null&&MarkCount>=journalRewards.badgeThreshold;
        public bool HasCoda=>journalRewards!=null&&MarkCount>=journalRewards.codaThreshold;
        public bool HasMark(string id)=>collected.Contains(id);
        public MarshMechanic Mechanic(string id)=>mechanics.Single(m=>m.Id==id);
        public bool HasToken(string id)=>Mechanic(id).Completed;
        public bool CollectMark(CollectibleData definition)
        {
            if(!marks.Any(m=>m.Data==definition))throw new InvalidOperationException("Reed Mark is not authored in this campaign.");
            return collected.Add(definition.id);
        }
        public GateData Gate(int index)=>data.sections[index].mainGate;
        public bool GateReady(int index)=>Gate(index)==null||Gate(index).requiredMechanics.All(HasToken);
        public string GateHint(int index)
        {
            var gate=Gate(index);if(gate==null)return "";
            foreach(string id in gate.requiredMechanics)if(!HasToken(id))return Mechanic(id).Data.hint;
            return "";
        }
        public void FreezeLoans(){foreach(var mechanic in mechanics)mechanic.FreezeTemporaryLoan();}
        public string EndingCoda=>HasCoda?journalRewards.coda:"";
        public string[] JournalPages
        {
            get
            {
                var pages=new List<string>();
                pages.Add("Reed Marks "+MarkCount+" / "+marks.Length+"\n\n"+(HasBadge?"A small brass duck badge sits beside the ledger.":"Four marks earn a cosmetic brass duck badge.")+"\n\nEight marks add a closing memory. No coin, health or damage advantage.\n\nE keeps a mark. Memories survive a checkpoint restart; Play Again starts a fresh journal.\n\nUse Space / left or right arrows to turn pages. Tab or Esc closes.");
                foreach(var m in mechanics.Where(m=>m.Completed))pages.Add(m.Data.title+"\n\n"+m.Data.journalText);
                foreach(var mark in marks.Where(m=>HasMark(m.Data.id)))pages.Add(mark.Data.title+"\n\n"+mark.Data.memory);
                return pages.ToArray();
            }
        }
        public string JournalText
        {
            get
            {
                var text=new StringBuilder("REED MARKS  "+MarkCount+" / "+marks.Length+(HasBadge?"    BRASS DUCK BADGE":"    4 marks: brass duck badge")+"\n");
                foreach(var m in mechanics.Where(m=>m.Completed))text.Append("\n"+m.Data.title+": "+m.Data.journalText+"\n");
                foreach(var mark in marks.Where(m=>HasMark(m.Data.id)))text.Append("\n"+mark.Data.title+": "+mark.Data.memory+"\n");
                if(collected.Count==0)text.Append("\nKeep the optional feather tokens with E. Memories never occupy inventory slots.\n");
                text.Append("\n8 marks add a closing memory. Coins and health are unchanged.");return text.ToString();
            }
        }
        public MarshLevel Level => level;
        public CampaignData Data => data;
        public MarshNode[] MainGates => mainGates;
        public int ChapterIndex => Mathf.Min(level.CheckpointIndex, data.chapters.Length - 1);
        public bool HasCapability(string id) => capabilities.Contains(id);
        public string ChapterName => data.chapters[ChapterIndex].displayName;
        public string Objective
        {
            get
            {
                int count = level.BellsOpened;
                if (count == 0 && level.CoinsInEscrow == 0 && level.Player.PlayerInventory.Count(Item.ItemType.COIN) < 3)
                    return "Gather the three brass coins along the landing trail, then find Landing Bell.";
                if (count == mainGates.Length) return "Recover every loan, then bring Duckduckgoose into the Haven circle.";
                if (count<mainGates.Length&&!GateReady(count))return GateHint(count);
                if (ExtraEscrow>0)return "Reclaim lamp or addressed mail coins; the sorting box can always reroute to the open low road.";
                if (count == data.archiveGateIndex && !level.CoinRelay.Completed) return level.CoinRelay.Objective;
                if (level.Player.PlayerInventory.Count(Item.ItemType.COIN) + level.Weapon.BorrowedCoins < 3)
                    return "Your coins are safe in their return boxes. Reclaim all three before the next bell.";
                return data.sections[Mathf.Min(count, data.sections.Length - 1)].objective;
            }
        }
        public void ValidateReferences()
        {
            if (level == null || data == null || mainGates == null || capabilities == null) throw new InvalidOperationException("Campaign has missing Inspector references.");
            data.ValidateData();
            if(mechanics==null||marks==null||mechanics.Any(m=>m==null)||marks.Any(m=>m==null)||mechanics.Select(m=>m.Id).Distinct().Count()!=mechanics.Length||marks.Select(m=>m.Data.id).Distinct().Count()!=marks.Length)throw new InvalidOperationException("Missing/duplicate campaign mechanic or mark.");
            if(mechanics.Length>0)
            {
                if(journalRewards==null)throw new InvalidOperationException("Journal reward data is missing.");
                journalRewards.ValidateData(marks.Length);
                foreach(var m in mechanics){m.Data.ValidateData();if(!string.IsNullOrEmpty(m.Data.prerequisite))Mechanic(m.Data.prerequisite);}
                foreach(var section in data.sections)
                {
                    if(section.mainGate==null)throw new InvalidOperationException("Campaign gate data missing.");
                    section.mainGate.ValidateData();foreach(string id in section.mainGate.requiredMechanics)Mechanic(id);
                }
            }
            if (mainGates.Length != data.sections.Length || mainGates.Any(n => n == null) || mainGates.Distinct().Count() != mainGates.Length)
                throw new InvalidOperationException("Campaign needs one unique main bell per chapter.");
            for (int i = 0; i < mainGates.Length; i++) if (mainGates[i].Sequence != i || mainGates[i].Kind != MarshNodeKind.Bell)
                throw new InvalidOperationException("Campaign main bells are out of order.");
        }
    }
}
