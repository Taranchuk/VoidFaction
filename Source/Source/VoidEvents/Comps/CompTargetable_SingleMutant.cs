﻿using System;
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
	public class MutantsToTarget : DefModExtension
	{
		public List<PawnKindDef> mutants;
	}
	public class CompTargetable_SingleMutant : CompTargetable
	{
		protected override bool PlayerChoosesTarget => true;
		protected override TargetingParameters GetTargetingParameters()
		{
			return new TargetingParameters
			{
				canTargetPawns = true,
				canTargetBuildings = false,
				validator = ((TargetInfo x) => BaseTargetValidator(x.Thing) && CanTargetPawn(x.Thing as Pawn))
			};
		}

		public bool CanTargetPawn(Pawn pawn)
        {
			if (pawn != null)
            {
				var options = this.parent.def.GetModExtension<MutantsToTarget>();
				if (options != null && options.mutants.Contains(pawn.kindDef))
				{
					return true;
				}
			}
			return false;
        }

		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
		}
	}

	public class CompTargetEffect_TameMutant : CompTargetEffect
	{
		public override void DoEffectOn(Pawn user, Thing target)
		{
			Pawn pawn = (Pawn)target;
			pawn.SetFaction(Faction.OfPlayer);
			if (!pawn.health.hediffSet.HasHediff(VoidDefOf.Void_SecronomControlChip))
            {
				var hediff = HediffMaker.MakeHediff(VoidDefOf.Void_SecronomControlChip, pawn);
				pawn.health.AddHediff(hediff);
            }
		}
	}
}