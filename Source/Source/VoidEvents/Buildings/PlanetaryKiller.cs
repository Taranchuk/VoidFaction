using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace VoidEvents
{
	public class PlanetaryKiller : Building
	{
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            var comp = this.Map.Parent.GetComponent<PlanetaryKillerComp>();
            if (comp != null)
            {
                var list = new List<GameCondition>();
                Find.World.GameConditionManager.GetAllGameConditionsAffectingMap(this.Map, list);
                var gameCond = list.Where(x => x.def == VoidDefOf2.Void_PlanetKiller).FirstOrDefault();
                if (gameCond is GameCondition_VoidPlanetkiller planetKiller)
                {
                    planetKiller.EndNoImpact();
                }
                comp.planetKillerIsDestroyed = true;

                var parms = StorytellerUtility.DefaultParmsNow(VoidDefOf.Void_GiveQuest_EndGame_ShipEscape.category, Find.World);
                parms.faction = this.Faction;
                Find.Storyteller.incidentQueue.Add(VoidDefOf.Void_GiveQuest_EndGame_ShipEscape, Find.TickManager.TicksGame + new IntRange(40, 60).RandomInRange, parms);
            }
            base.Destroy(mode);
        }
    }
}
