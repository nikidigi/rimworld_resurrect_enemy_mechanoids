using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace ResurrectEnemyMechanoids
{
    [HarmonyPatch(typeof(CompAbilityEffect_ResurrectMech), nameof(CompAbilityEffect_ResurrectMech.Apply))]
    class Patch_CompAbilityEffect_ResurrectMech_Apply
    {
        static void Prefix(ref LocalTargetInfo target, out Pawn __state)
        {
            Corpse corpse = (Corpse)target.Thing;

            __state = corpse.InnerPawn.IsColonyMech ? corpse.InnerPawn : null;
        }

        static void Postfix(CompAbilityEffect_ResurrectMech __instance, Pawn __state)
        {
            if (__state is not null)
            {
                Pawn overseer = __instance.parent.pawn.GetOverseer();

                if (overseer is not null && MechanitorUtility.CanControlMech(overseer, __state))
                {
                    overseer.relations.AddDirectRelation(PawnRelationDefOf.Overseer, __state);
                }

                 __instance.parent.GetType()
                    .GetField("charges", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(__instance.parent, __instance.ChargesRemaining);
            }
        }
    }
}
