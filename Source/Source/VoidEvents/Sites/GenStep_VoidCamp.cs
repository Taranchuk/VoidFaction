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
			Faction faction = (map.ParentFaction != null && map.ParentFaction != Faction.OfPlayer) ? map.ParentFaction : Find.FactionManager.RandomEnemyFaction();
			if (!MapGenerator.TryGetVar("UsedRects", out List<CellRect> var))
			{
				var = new List<CellRect>();
				MapGenerator.SetVar("UsedRects", var);
			}
			CellRect cellRect = CellRect.CenteredOn(map.Center, 10, 10).ClipInsideMap(map);

			ResolveParams resolveParams = default(ResolveParams);
			resolveParams.rect = cellRect;
			resolveParams.faction = faction;
			var planetaryKiller = ThingMaker.MakeThing(VoidDefOf.Void_PlanetaryKiller);
			resolveParams.conditionCauser = planetaryKiller;
			RimWorld.BaseGen.BaseGen.globalSettings.map = map;
			RimWorld.BaseGen.BaseGen.symbolStack.Push("conditionCauserRoom", resolveParams);
			RimWorld.BaseGen.BaseGen.Generate();
			MapGenerator.SetVar("RectOfInterest", cellRect);
			var.Add(cellRect);

			//if (!MapGenerator.TryGetVar("RectOfInterest", out CellRect var))
			//{
			//	var = CellRect.SingleCell(map.Center);
			//}
			//if (!MapGenerator.TryGetVar("UsedRects", out List<CellRect> var2))
			//{
			//	var2 = new List<CellRect>();
			//	MapGenerator.SetVar("UsedRects", var2);
			//}
			//Faction faction = (map.ParentFaction != null && map.ParentFaction != Faction.OfPlayer) ? map.ParentFaction : Find.FactionManager.RandomEnemyFaction();
			//ResolveParams resolveParams = default(ResolveParams);
			//var complexityPoints = StorytellerUtility.DefaultThreatPointsNow(Find.World) * 2f;
			//var complexityLevel = complexityCurve.OrderBy(item => Math.Abs(complexityPoints - item.Value)).First().Key;
			//resolveParams.rect = GetOutpostRect(var, var2, map, complexityLevel);
			//resolveParams.faction = faction;
			//resolveParams.edgeDefenseWidth = 3;
			//resolveParams.edgeDefenseTurretsCount = (int)(Rand.RangeInclusive(0, 1) * complexityLevel * 2f);
			//resolveParams.edgeDefenseMortarsCount = 0;
			//resolveParams.settlementPawnGroupPoints = complexityPoints;
			//RimWorld.BaseGen.BaseGen.globalSettings.map = map;
			//RimWorld.BaseGen.BaseGen.globalSettings.minBuildings = 1;
			//RimWorld.BaseGen.BaseGen.globalSettings.minBarracks = 1;
			//RimWorld.BaseGen.BaseGen.symbolStack.Push("settlement", resolveParams);
			//if (faction != null && faction == Faction.Empire)
			//{
			//	RimWorld.BaseGen.BaseGen.globalSettings.minThroneRooms = 1;
			//	RimWorld.BaseGen.BaseGen.globalSettings.minLandingPads = 1;
			//}
			//var planetaryKiller = ThingMaker.MakeThing(VoidDefOf.Void_PlanetaryKiller);
			//resolveParams.conditionCauser = planetaryKiller;
			//RimWorld.BaseGen.BaseGen.symbolStack.Push("conditionCauserRoom", resolveParams);
			//RimWorld.BaseGen.BaseGen.Generate();
			//if (faction != null && faction == Faction.Empire && RimWorld.BaseGen.BaseGen.globalSettings.landingPadsGenerated == 0)
			//{
			//	GenStep_Settlement.GenerateLandingPadNearby(resolveParams.rect, map, faction, out CellRect usedRect);
			//	var2.Add(usedRect);
			//}
			//var2.Add(resolveParams.rect);
		}

		private CellRect GetOutpostRect(CellRect rectToDefend, List<CellRect> usedRects, Map map, float complexityLevel)
		{
			var size = (int)(16f * complexityLevel);
			if (size > 60)
			{
				size = 60;
			}

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

	}
}
