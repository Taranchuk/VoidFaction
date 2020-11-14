using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;

namespace VoidEvents
{
	[DefOf]
	public static class VoidDefOf
	{
		public static FactionDef RH_VOID;

		public static ThingDef RH_Nerotonin8B;

		public static PawnKindDef RH_VOID_Member;

		public static PawnKindDef RH_VOID_Elite;

		public static LetterDef Void_ThreatBig;

		public static ThingDef Void_DefoliatorShipPart;

		public static HediffDef Void_SecronomControlChip;
		public static HediffDef Void_RapidHealing;
		public static JobDef UseSecronomControlChip;

		public static IncidentDef Void_GiveQuest_EndGame_ShipEscape;
		public static IncidentDef Void_PlanetKiller;
		public static ThingDef Void_PlanetaryKiller;
		public static WorldObjectDef Void_PlanetaryKillerSite;

	}

	[DefOf]
	public static class VoidDefOf2
	{
		public static SitePartDef Void_PlanetaryKillerSite;
		public static GameConditionDef Void_PlanetKiller;
	}
}
