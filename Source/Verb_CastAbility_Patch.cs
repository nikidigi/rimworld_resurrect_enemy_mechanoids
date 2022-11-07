using HarmonyLib;
using RimWorld;
using Verse.AI;
using Verse;

namespace ResurrectEnemyMechanoids
{
    [HarmonyPatch(typeof(Verb_CastAbility), nameof(Verb_CastAbility.OrderForceTarget))]
    class Verb_CastAbility_OrderForceTarget_Patch
    {
        static bool Prefix(Verb_CastAbility __instance, ref LocalTargetInfo target)
        {
            if (__instance.ability.def != AbilityDefOf.ResurrectionMech)
            {
                return true;
            }

            IntVec3 dest;

            if (CastPositionFinder.TryFindCastPosition(new CastPositionRequest()
            {
                caster = __instance.CasterPawn,
                target = target.Thing,
                verb = __instance.ability.verb,
                maxRangeFromTarget = __instance.ability.verb.verbProps.range,
                wantCoverFromTarget = false,
                maxRegions = 50,
            }, out dest))
            {
                __instance.ability.QueueCastingJob(target, dest);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Verb_CastAbility), nameof(Verb_CastAbility.IsApplicableTo))]
    class Verb_CastAbility_IsApplicableTo_Patch
    {
        static bool Prefix(Verb_CastAbility __instance, ref bool __result, ref LocalTargetInfo target)
        {
            if (__instance.ability.def != AbilityDefOf.ResurrectionMech)
            {
                return true;
            }

            __result = __instance.ability.CanApplyOn(target);
            return false;
        }
    }
}
