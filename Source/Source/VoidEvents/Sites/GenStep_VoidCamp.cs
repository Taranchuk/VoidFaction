using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VoidEvents
{
	public class GenStep_VoidCamp : GenStep
	{

		public FloatRange defaultPawnGroupPointsRange = SymbolResolver_Settlement.DefaultPawnsPoints;

		private static List<CellRect> possibleRects = new List<CellRect>();

		public override int SeedPart => 398138181;


		public Dictionary<float, float> complexityCurve = new Dictionary<float, float>
		{
			{1f, 200},
			{1.5f, 400},
			{2f, 800 },
			{2.5f, 1200},
			{3f, 1600},
			{3.5f, 2000}
		};
		public override void Generate(Map map, GenStepParams parms)
		{
			if (!MapGenerator.TryGetVar("RectOfInterest", out CellRect var))
			{
				var = CellRect.SingleCell(map.Center);
			}
			if (!MapGenerator.TryGetVar("UsedRects", out List<CellRect> var2))
			{
				var2 = new List<CellRect>();
				MapGenerator.SetVar("UsedRects", var2);
			}
			Faction faction = (map.ParentFaction != null && map.ParentFaction != Faction.OfPlayer) ? map.ParentFaction : Find.FactionManager.RandomEnemyFaction();
			ResolveParams resolveParams = default(ResolveParams);
			var complexityPoints = StorytellerUtility.DefaultThreatPointsNow(Find.World);
			var complexityLevel = complexityCurve.OrderBy(item => Math.Abs(complexityPoints - item.Value)).First().Key;
			resolveParams.rect = GetOutpostRect(var, var2, map, complexityLevel);
			var2.Add(resolveParams.rect);
			resolveParams.faction = faction;
			resolveParams.wallStuff = ThingDefOf.Plasteel;
			resolveParams.edgeDefenseWidth = 3;
			resolveParams.edgeDefenseTurretsCount = (int)(Rand.RangeInclusive(3, 5) * complexityLevel * 2f);
			resolveParams.edgeDefenseMortarsCount = 0;
			resolveParams.settlementPawnGroupPoints = Mathf.Max(5000, complexityPoints * 20);
			Log.Message("resolveParams.settlementPawnGroupPoints: " + resolveParams.settlementPawnGroupPoints);
			RimWorld.BaseGen.BaseGen.globalSettings.map = map;
			RimWorld.BaseGen.BaseGen.globalSettings.minBuildings = 1;
			RimWorld.BaseGen.BaseGen.globalSettings.minBarracks = 1;
			RimWorld.BaseGen.BaseGen.symbolStack.Push("voidSettlement", resolveParams);
			if (faction != null && faction == Faction.Empire)
			{
				RimWorld.BaseGen.BaseGen.globalSettings.minThroneRooms = 1;
				RimWorld.BaseGen.BaseGen.globalSettings.minLandingPads = 1;
			}
			RimWorld.BaseGen.BaseGen.Generate();
			GenerateBuildingNearby(resolveParams.rect, map, faction, out CellRect usedRect);
			var2.Add(usedRect);
		}

		private CellRect GetOutpostRect(CellRect rectToDefend, List<CellRect> usedRects, Map map, float complexityLevel)
		{
			var size = Rand.RangeInclusive(40, 50);
			possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.CenterCell.z - size / 2, size, size));
			possibleRects.Add(new CellRect(rectToDefend.minX - 1 - size, rectToDefend.CenterCell.z - size / 2, size, size));
			possibleRects.Add(new CellRect(rectToDefend.maxX + 1, rectToDefend.CenterCell.z - size / 2, size, size));
			possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.minZ - 1 - size, size, size));
			possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.maxZ + 1, size, size));
			CellRect mapRect = new CellRect(0, 0, map.Size.x, map.Size.z);
			possibleRects.RemoveAll((CellRect x) => !x.FullyContainedWithin(mapRect));
			if (possibleRects.Any())
			{
				IEnumerable<CellRect> source = possibleRects.Where((CellRect x) => !usedRects.Any((CellRect y) => x.Overlaps(y)));
				if (!source.Any())
				{
					possibleRects.Add(new CellRect(rectToDefend.minX - 1 - size * 2, rectToDefend.CenterCell.z - size / 2, size, size));
					possibleRects.Add(new CellRect(rectToDefend.maxX + 1 + size, rectToDefend.CenterCell.z - size / 2, size, size));
					possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.minZ - 1 - size * 2, size, size));
					possibleRects.Add(new CellRect(rectToDefend.CenterCell.x - size / 2, rectToDefend.maxZ + 1 + size, size, size));
				}
				if (source.Any())
				{
					return source.First();
				}
				return possibleRects.RandomElement();
			}
			return rectToDefend;
		}


		private static List<IntVec3> tmpCandidates = new List<IntVec3>();

		public static void GenerateBuildingNearby(CellRect rect, Map map, Faction faction, out CellRect usedRect)
		{
			ResolveParams resolveParams = default(ResolveParams);
			MapGenerator.TryGetVar("UsedRects", out List<CellRect> usedRects);
			tmpCandidates.Clear();
			int size = 9;
			tmpCandidates.Add(new IntVec3(rect.maxX + 1, 0, rect.CenterCell.z));
			tmpCandidates.Add(new IntVec3(rect.minX - size, 0, rect.CenterCell.z));
			tmpCandidates.Add(new IntVec3(rect.CenterCell.x, 0, rect.maxZ + 1));
			tmpCandidates.Add(new IntVec3(rect.CenterCell.x, 0, rect.minZ - size));
			if (!tmpCandidates.Where(delegate (IntVec3 x)
			{
				CellRect r = new CellRect(x.x, x.z, size, size);
				return r.InBounds(map) && (usedRects == null || !usedRects.Any((CellRect y) => y.Overlaps(r)));
			}).TryRandomElement(out IntVec3 result))
			{
				usedRect = CellRect.Empty;
				return;
			}
			resolveParams.rect = new CellRect(result.x, result.z, size, size);
			resolveParams.faction = faction;
			resolveParams.wallStuff = ThingDefOf.Plasteel;
			RimWorld.BaseGen.BaseGen.globalSettings.map = map;
			var planetaryKiller = ThingMaker.MakeThing(VoidDefOf.Void_PlanetaryKiller);
			resolveParams.conditionCauser = planetaryKiller;
			RimWorld.BaseGen.BaseGen.globalSettings.map = map;
			RimWorld.BaseGen.BaseGen.symbolStack.Push("conditionCauserRoom", resolveParams);
			usedRect = resolveParams.rect;
			RimWorld.BaseGen.BaseGen.Generate();
		}

	}
}
