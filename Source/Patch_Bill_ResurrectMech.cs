using HarmonyLib;
using RimWorld;
using Verse;

namespace ResurrectEnemyMechanoids
{
    [HarmonyPatch(typeof(Bill_ResurrectMech), nameof(Bill_ResurrectMech.IsFixedOrAllowedIngredient))]
    class Patch_Bill_ResurrectMech_IsFixedOrAllowedIngredient
    {
        static bool Prefix(Bill_ResurrectMech __instance, ref bool __result, ref Thing thing)
        {
            __result = true;

            Bill_MechImpl super = new(__instance.recipe, __instance.precept);

            if (!super.IsFixedOrAllowedIngredient(thing))
            {
                __result = false;
            }
            else if (thing is Corpse)
            {
                __result = __instance.ingredientFilter.Allows(thing);
            }

            return false;
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
