using HarmonyLib;
using RimWorld;
using Verse;

namespace ResurrectEnemyMechanoids
{
    [HarmonyPatch(typeof(Bill_ResurrectMech), nameof(Bill_ResurrectMech.IsFixedOrAllowedIngredient))]
    class Patch_Bill_ResurrectMech_IsFixedOrAllowedIngredient
    {
        static void Postfix(ref bool __result, ref Thing thing)
        {
            if (!__result && thing is Corpse corpse && corpse.InnerPawn.RaceProps.IsMechanoid) {
                __result = true;
            }
        }
    }

    [HarmonyPatch(typeof(Bill_ResurrectMech), nameof(Bill_ResurrectMech.CreateProducts))]
    class Patch_Bill_ResurrectMech_CreateProducts
    {
        static bool Prefix(Bill_ResurrectMech __instance)
        {
            Pawn innerPawn = __instance.Gestator.ResurrectingMechCorpse.InnerPawn;

            if (innerPawn.Faction != Faction.OfPlayer)
            {
                innerPawn.SetFaction(Faction.OfPlayer);

                if (Prefs.DevMode)
                {
                    Log.Message("ResurrectEnemyMechanoids | " + innerPawn + " has been resurrected");
                }
            }

            return true;
        }
    }
}
