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
using Verse.AI;

namespace VoidEvents
{
    public class JobDriver_UseItem : JobDriver
    {
        private int useDuration = 480;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref useDuration, "useDuration", 0);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil toil = Toils_General.Wait(useDuration);
            toil.WithProgressBarToilDelay(TargetIndex.A);
            toil.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return toil;
            Toil use = new Toil();
            use.initAction = delegate
            {
                Pawn actor = use.actor;
                Pawn pawn = (Pawn)job.targetA.Thing;
                pawn.SetFaction(Faction.OfPlayer);
                if (!pawn.health.hediffSet.HasHediff(VoidDefOf.Void_SecronomControlChip))
                {
                	var hediff = HediffMaker.MakeHediff(VoidDefOf.Void_SecronomControlChip, pawn);
                	pawn.health.AddHediff(hediff);
                }
            };
            use.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return use;
        }
    }
}
