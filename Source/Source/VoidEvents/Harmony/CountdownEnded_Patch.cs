using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse.AI.Group;

namespace VoidEvents
{
    [HarmonyPatch(typeof(ShipCountdown), "CountdownEnded")]
    class CountdownEnded_Patch
    {
        public static bool Prefix(Building ___shipRoot, string ___customLaunchString)
        {
            var map = ___shipRoot?.Map;
            if (map != null)
            {
                var comp = map.Parent.GetComponent<VoidEscapeShipComp>();
                if (comp != null)
                {
                    CountdownEnded(___shipRoot, ___customLaunchString);
                    return false;
                }
            }
            return true;
		}

        private static void CountdownEnded(Building shipRoot, string customLaunchString)
        {
            if (shipRoot != null)
            {
                List<Building> list = ShipUtility.ShipBuildingsAttachedTo(shipRoot).ToList();
                StringBuilder stringBuilder = new StringBuilder();
                foreach (Building item in list)
                {
                    Building_CryptosleepCasket building_CryptosleepCasket = item as Building_CryptosleepCasket;
                    if (building_CryptosleepCasket != null && building_CryptosleepCasket.HasAnyContents)
                    {
                        stringBuilder.AppendLine("   " + building_CryptosleepCasket.ContainedThing.LabelCap);
                        Find.StoryWatcher.statsRecord.colonistsLaunched++;
                        TaleRecorder.RecordTale(TaleDefOf.LaunchedShip, building_CryptosleepCasket.ContainedThing);
                    }
                }
                GameVictoryUtility.ShowCredits(GameVictoryUtility.MakeEndCredits("GameOverShipLaunchedIntro".Translate(), "VoidGameOverShipLaunchedEnding".Translate(), stringBuilder.ToString()));
                foreach (Building item2 in list)
                {
                    item2.Destroy();
                }
            }
        }
	}
}