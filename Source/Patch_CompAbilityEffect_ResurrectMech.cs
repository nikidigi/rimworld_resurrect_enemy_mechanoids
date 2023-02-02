using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace ResurrectEnemyMechanoids
{
    [HarmonyPatch(typeof(CompAbilityEffect_ResurrectMech), nameof(CompAbilityEffect_ResurrectMech.Apply))]
    class Patch_CompAbilityEffect_ResurrectMech_Apply
    {
        static Pawn mech = null;

        static void Prefix(ref LocalTargetInfo target)
        {
            Corpse corpse = (Corpse)target.Thing;

            if (corpse.InnerPawn.IsColonyMech)
            {
                mech = corpse.InnerPawn;
            }
        }

        static void Postfix(CompAbilityEffect_ResurrectMech __instance)
        {
            if (mech is not null)
            {
                Pawn overseer = __instance.parent.pawn.GetOverseer();

                if (overseer is not null && MechanitorUtility.CanControlMech(overseer, mech))
                {
                    overseer.relations.AddDirectRelation(PawnRelationDefOf.Overseer, mech);
                }

                mech = null;

                 __instance.parent.GetType()
                    .GetField("charges", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(__instance.parent, __instance.ChargesRemaining);
            }
        }
    }
}
