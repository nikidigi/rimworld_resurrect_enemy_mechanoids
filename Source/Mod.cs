using HarmonyLib;
using Verse;

namespace ResurrectEnemyMechanoids
{
    [StaticConstructorOnStartup]
    public class Mod
    {
        static Mod()
        {
            Harmony harmony = new("nikidigi.ResurrectEnemyMechanoids");

            harmony.PatchAll();

            Log.Message("ResurrectEnemyMechanoids initialized");
        }
    }
}
