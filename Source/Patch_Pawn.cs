using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace ResurrectEnemyMechanoids
{
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.TickRare))]
    class Patch_Pawn_TickRare
    {
        static Dictionary<string, int> apocritons = new();

        const int MaxCharges = 30;

        const int RechargeCooldownTicks = GenDate.TicksPerDay / 5;

        static void Postfix(Pawn __instance)
        {
            if (__instance.kindDef != PawnKindDefOf.Mech_Apocriton || !__instance.IsColonyMech || __instance.Dead || !__instance.Spawned)
            {
                return;
            }

            int ticks;

            if (!apocritons.TryGetValue(__instance.ThingID, out ticks))
            {
                ticks = Find.TickManager.TicksGame;

                apocritons.SetOrAdd(__instance.ThingID, ticks);
            }

            ticks = Find.TickManager.TicksGame - ticks;

            // Probably another save has been loaded
            if (ticks < 0)
            {
                apocritons.Clear();
                return;
            }

            if (ticks < RechargeCooldownTicks)
            {
                return;
            }

            Ability resurrection = __instance.abilities.GetAbility(AbilityDefOf.ResurrectionMech);

            for (int i = 0; i < resurrection.EffectComps.Count; i++)
            {
                if (resurrection.EffectComps[i] is CompAbilityEffect_ResurrectMech comp)
                {
                    if (comp.ChargesRemaining < MaxCharges)
                    {
                        int charges = comp.ChargesRemaining + 1;

                        comp.GetType()
                            .GetField("charges", BindingFlags.NonPublic | BindingFlags.Instance)
                            .SetValue(comp, charges);

                        resurrection.GetType()
                            .GetField("charges", BindingFlags.NonPublic | BindingFlags.Instance)
                            .SetValue(resurrection, charges);
                    }
                    break;
                }
            }

            apocritons.SetOrAdd(__instance.ThingID, Find.TickManager.TicksGame);
        }
    }
}
