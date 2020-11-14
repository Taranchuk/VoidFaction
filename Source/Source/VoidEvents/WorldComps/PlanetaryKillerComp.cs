using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using RimWorld.Planet;

namespace VoidEvents
{
    public class WorldObjectCompProperties_PlanetaryKiller : WorldObjectCompProperties
    {
        public WorldObjectCompProperties_PlanetaryKiller()
        {
            compClass = typeof(PlanetaryKillerComp);
        }
    }
    [StaticConstructorOnStartup]
    public class PlanetaryKillerComp : WorldObjectComp
    {
        public Map Map => (parent as MapParent)?.Map;


        public int lastReinforcementTick;
        public bool firstTime = true;
        public bool planetKillerIsDestroyed;
        public override void CompTick()
        {
            base.CompTick();
            if (!planetKillerIsDestroyed && this.ParentHasMap)
            {
                if (Find.TickManager.TicksAbs > lastReinforcementTick + 2 * GenDate.TicksPerHour)
                {
                    lastReinforcementTick = Find.TickManager.TicksAbs;
                    if (firstTime)
                    {
                        firstTime = false;
                    }
                    else
                    {
                        IncidentParms parms = new IncidentParms
                        {
                            target = Map,
                            points = StorytellerUtility.DefaultThreatPointsNow(Find.World),
                            faction = this.parent.Faction
                        };
                        IncidentDefOf.RaidEnemy.Worker.TryExecute(parms);
                    }
                }

            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref lastReinforcementTick, "lastReinforcementTick");
            Scribe_Values.Look(ref firstTime, "firstTime");
            Scribe_Values.Look(ref planetKillerIsDestroyed, "planetKillerIsDestroyed");
        }
    }
}
