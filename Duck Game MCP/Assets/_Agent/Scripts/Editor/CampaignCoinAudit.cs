#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
namespace DuckAdventure.Agent.Editor
{
    // Finite resource model, independent of physical movement. Runtime/route tests cover geometry.
    public static class CampaignCoinAudit
    {
        [Serializable] public sealed class Result
        {
            public int states,transitions,endingStates,recoverableStates;
            public bool conserved,allCanFinish;
            public int[] stages,relayPhaseCoverage,routePhaseCoverage;
            public int lampCombinationCoverage;
            public string scope="All reachable loan/claim/reroute/lamp/sword/perch/death resource states. Marks are independent cosmetic bits; physical geometry checked separately.";
        }
        private struct State
        {
            public int stage,relay,lamps,clues,route0,route1,perches,weapon;
            public int Key=>stage|(relay<<4)|(lamps<<9)|(clues<<11)|(route0<<13)|(route1<<15)|(perches<<17)|(weapon<<20);
        }
        private static int Relay(State s,int index)=>s.relay/(index==0?1:index==1?3:9)%3;
        private static State SetRelay(State s,int index,int value){int power=index==0?1:index==1?3:9;s.relay+=(value-Relay(s,index))*power;return s;}
        public static Result Run(MarshCampaign campaign)
        {
            campaign.ValidateReferences();
            if(campaign.MainGates.Length!=5||campaign.Mechanics.Length!=7||campaign.Level.CoinRelay.LoanCount!=3)throw new InvalidOperationException("Audit topology must be updated for this campaign.");
            if(campaign.Mechanic("lamp-haven").Data.prerequisite!="perch-haven"||campaign.Mechanic("route-haven").Data.prerequisite!="lamp-haven"||campaign.Mechanic("route-storm").Data.prerequisite!=""||campaign.Mechanic("lamp-archive").Data.prerequisite!="")throw new InvalidOperationException("Update audit prerequisites to match authored definitions.");
            var main=campaign.Data.sections.Select(s=>s.mainGate.cost).ToArray();var relay=Enumerable.Range(0,3).Select(campaign.Level.CoinRelay.Cost).ToArray();
            int lampCost=campaign.Mechanic("lamp-archive").Data.cost,mailCost=campaign.Mechanic("route-storm").Data.cost;
            if(campaign.Mechanic("lamp-haven").Data.cost!=lampCost||campaign.Mechanic("route-haven").Data.cost!=mailCost)throw new InvalidOperationException("Audit requires matching per-type costs.");
            var relaySo=new SerializedObject(campaign.Level.CoinRelay);var loans=relaySo.FindProperty("loans");int[] dependency=Enumerable.Range(0,3).Select(i=>loans.GetArrayElementAtIndex(i).FindPropertyRelative("requiresReturnedLoan").intValue).ToArray();
            Func<State,int> coins=s=>3-((s.stage%2==1)?main[s.stage/2]:0)-Enumerable.Range(0,3).Where(i=>Relay(s,i)==1).Sum(i=>relay[i])-((s.lamps&1)>0?lampCost:0)-((s.lamps&2)>0?lampCost:0)-(s.route0==1||s.route0==2?mailCost:0)-(s.route1==1||s.route1==2?mailCost:0)-s.weapon;
            Func<State,string,bool> token=(s,id)=>
            {
                switch(id)
                {
                    case "perch-reeds":return (s.perches&1)>0;
                    case "perch-storm":return (s.perches&2)>0;
                    case "perch-haven":return (s.perches&4)>0;
                    case "lamp-archive":return (s.clues&1)>0;
                    case "lamp-haven":return (s.clues&2)>0;
                    case "route-storm":return s.route0==3;
                    case "route-haven":return s.route1==3;
                    default:throw new InvalidOperationException("Unhandled audit token "+id);
                }
            };
            var stages=new int[11];var relayCoverage=new int[3];var routeCoverage=new int[2];int lampCoverage=0;
            var states=new Dictionary<int,State>();var reverse=new Dictionary<int,List<int>>();var queue=new Queue<State>();var endings=new List<int>();int edges=0;
            Action<State,State> add=(from,to)=>
            {
                int balance=coins(to);if(balance<0||balance>3)throw new InvalidOperationException("Currency creation/debt at resource state "+to.Key);
                edges++;if(!reverse.ContainsKey(to.Key))reverse[to.Key]=new List<int>();reverse[to.Key].Add(from.Key);
                if(!states.ContainsKey(to.Key)){states.Add(to.Key,to);queue.Enqueue(to);}
            };
            var start=new State();states.Add(start.Key,start);queue.Enqueue(start);
            while(queue.Count>0)
            {
                if(states.Count>500000)throw new InvalidOperationException("Resource audit exceeded bounded state budget.");
                var s=queue.Dequeue();stages[s.stage]++;for(int i=0;i<3;i++)relayCoverage[i]|=1<<Relay(s,i);routeCoverage[0]|=1<<s.route0;routeCoverage[1]|=1<<s.route1;lampCoverage|=1<<s.lamps;int balance=coins(s),chapter=Math.Min((s.stage+1)/2,4);
                if(s.stage==10&&balance==3&&s.lamps==0&&s.weapon==0)endings.Add(s.Key);
                if(s.stage<10)
                {
                    if(s.stage%2==1){var n=s;n.stage++;add(s,n);}
                    else
                    {
                        int gate=s.stage/2;
                        bool ready=campaign.Gate(gate).requiredMechanics.All(id=>token(s,id));
                        if(gate==campaign.Data.archiveGateIndex)ready&=Enumerable.Range(0,3).All(i=>Relay(s,i)==2);
                        if(ready&&balance>=main[gate]){var n=s;n.stage++;add(s,n);}
                    }
                }
                // Free assistance/inspection and recall do not own currency; completed inspections latch.
                if(chapter>=1&&(s.perches&1)==0){var n=s;n.perches|=1;add(s,n);}
                if(chapter>=3&&(s.perches&2)==0){var n=s;n.perches|=2;add(s,n);}
                if(chapter>=4&&(s.perches&4)==0){var n=s;n.perches|=4;add(s,n);}
                if(chapter>=2)
                {
                    for(int i=0;i<3;i++)
                    {
                        int phase=Relay(s,i);
                        if(phase==0&&balance>=relay[i]&&(dependency[i]<0||Relay(s,dependency[i])==2))add(s,SetRelay(s,i,1));
                        if(phase==1)add(s,SetRelay(s,i,2));
                    }
                }
                for(int i=0;i<2;i++)
                {
                    int flag=1<<i;bool reachable=i==0?chapter>=2:chapter>=4&&(s.perches&4)>0;
                    if(!reachable)continue;
                    var n=s;
                    if((s.lamps&flag)==0&&balance>=lampCost){n.lamps|=flag;n.clues|=flag;add(s,n);}
                    else if((s.lamps&flag)>0){n.lamps&=~flag;add(s,n);}
                }
                for(int i=0;i<2;i++)
                {
                    bool reachable=i==0?chapter>=3:chapter>=4&&(s.clues&2)>0;if(!reachable)continue;
                    int phase=i==0?s.route0:s.route1;
                    if(phase==0&&balance>=mailCost){var n=s;if(i==0)n.route0=1;else n.route1=1;add(s,n);}
                    if(phase==1||phase==2)
                    {
                        var n=s;if(i==0)n.route0=3-phase;else n.route1=3-phase;add(s,n);
                        bool canClaim=phase==1||i==1||(s.perches&2)>0;
                        if(canClaim){n=s;if(i==0)n.route0=3;else n.route1=3;add(s,n);}
                    }
                }
                if(s.weapon==0&&balance>=1){var n=s;n.weapon=1;add(s,n);}
                if(s.weapon==1){var n=s;n.weapon=0;add(s,n);}
                // Death/pause refund temporary lamps and sword, keeping latches/addressed loans/journal.
                if(s.lamps!=0||s.weapon!=0){var n=s;n.lamps=0;n.weapon=0;add(s,n);}
            }
            var safe=new HashSet<int>(endings);var back=new Queue<int>(endings);
            while(back.Count>0){int key=back.Dequeue();List<int> parents;if(!reverse.TryGetValue(key,out parents))continue;foreach(int parent in parents)if(safe.Add(parent))back.Enqueue(parent);}
            var result=new Result{stages=stages,relayPhaseCoverage=relayCoverage,routePhaseCoverage=routeCoverage,lampCombinationCoverage=lampCoverage,states=states.Count,transitions=edges,endingStates=endings.Count,recoverableStates=safe.Count,conserved=true,allCanFinish=safe.Count==states.Count};
            if(!result.allCanFinish)throw new InvalidOperationException("Soft lock in resource graph: "+(states.Count-safe.Count)+" states cannot finish.");
            return result;
        }
    }
}
#endif
