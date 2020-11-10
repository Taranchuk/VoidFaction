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
    public class Hediff_RapidHealing : HediffWithComps
    {
        public int lastHarmTick;
        public override bool ShouldRemove => false;
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            lastHarmTick = Find.TickManager.TicksGame;
        }
        public override void Tick()
        {
            base.Tick();
            if (Find.TickManager.TicksGame < lastHarmTick + (4 * GenDate.TicksPerHour))
            {
                this.Severity = 0;
            }
            else
            {
                this.Severity = 1f;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref lastHarmTick, "lastHarmTick");
        }
    }
}