using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;
using RimWorld.Planet;

namespace VoidEvents
{
    public class IncidentWorker_ActivateVoidPlanetaryKiller : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            GameConditionManager gameConditionManager = parms.target.GameConditionManager;
            if (gameConditionManager == null)
            {
                Log.ErrorOnce($"Couldn't find condition manager for incident target {parms.target}", 70849667);
                return false;
            }
            if (gameConditionManager.ConditionIsActive(def.gameCondition))
            {
                return false;
            }
            List<GameCondition> activeConditions = gameConditionManager.ActiveConditions;
            for (int i = 0; i < activeConditions.Count; i++)
            {
                if (!def.gameCondition.CanCoexistWith(activeConditions[i].def))
                {
                    return false;
                }
            }
            if (Find.World.worldObjects.Settlements.Where(x => x.Faction.def == VoidDefOf.RH_VOID).Any())
            {
                return false;
            }
            return true;
        }

        public int FindTileAtIceSheet()
        {
            var list = new List<int>();
            for (int i = 0; i < Find.WorldGrid.TilesCount; i++)
            {
                if (Find.WorldGrid[i].biome == BiomeDefOf.IceSheet)
                {
                    list.Add(i);
                }
            }
            return list.RandomElement();
        }
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            var tile = FindTileAtIceSheet();
            var worldObject = SiteMaker.MakeSite(VoidDefOf.Void_BanditCamp, tile, parms.faction);
            Find.WorldObjects.Add(worldObject);
            GameConditionManager gameConditionManager = parms.target.GameConditionManager;
            GameCondition gameCondition = GameConditionMaker.MakeCondition(duration: Mathf.RoundToInt(def.durationDays.RandomInRange * 60000f), def: def.gameCondition);
            gameConditionManager.RegisterCondition(gameCondition);
            parms.letterHyperlinkThingDefs = gameCondition.def.letterHyperlinks;
            SendStandardLetter(def.letterLabel, gameCondition.LetterText, def.letterDef, parms, worldObject);
            return true;
        }
    }
}