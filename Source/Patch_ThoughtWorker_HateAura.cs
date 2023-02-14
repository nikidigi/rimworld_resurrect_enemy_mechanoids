using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System;

namespace ResurrectEnemyMechanoids
{
    [HarmonyPatch(typeof(RimWorld.ThoughtWorker_HateAura), "CurrentStateInternal")]
    class Patch_ThoughtWorker_HateAura_CurrentStateInternal
    {
        static readonly CodeInstruction EP = new(
            OpCodes.Callvirt,
            AccessTools.Method(typeof(Verse.ListerThings), nameof(Verse.ListerThings.ThingsOfDef), new Type[] { typeof(Verse.ThingDef) })
        );

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            List<CodeInstruction> code = new(instructions);

            int injectionOffset = -1;

            for (int i = 0; i < code.Count; i++)
            {
                if (EP.opcode == code[i].opcode && EP.operand == code[i].operand)
                {
                    injectionOffset = i;
                    break;
                }
            }

            if (injectionOffset == -1)
            {
                return code;
            }

            LocalBuilder iter = il.DeclareLocal(typeof(int));
            List<CodeInstruction> labels = new();
            List<CodeInstruction> patch = new();

            labels.Add(new CodeInstruction(OpCodes.Ldloc, iter));
            labels.Last().labels.Add(il.DefineLabel());

            labels.Add(new CodeInstruction(OpCodes.Ldloc_1));
            labels.Last().labels.Add(il.DefineLabel());

            labels.Add(new CodeInstruction(OpCodes.Ldloc, iter));
            labels.Last().labels.Add(il.DefineLabel());

            patch.Add(new CodeInstruction(OpCodes.Ldloc_1));
            patch.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<Verse.Thing>), "get_Count")));
            patch.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
            patch.Add(new CodeInstruction(OpCodes.Sub));
            patch.Add(new CodeInstruction(OpCodes.Stloc, iter));

            patch.Add(new CodeInstruction(OpCodes.Br_S, labels[0].labels[0]));

            patch.Add(labels[1]);
            patch.Add(new CodeInstruction(OpCodes.Ldloc, iter));
            patch.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<Verse.Thing>), "get_Item", new Type[] { typeof(int) })));

            patch.Add(new CodeInstruction(OpCodes.Ldarg_1));
            patch.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Verse.Thing), "get_Faction")));
            patch.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RimWorld.GenHostility), nameof(RimWorld.GenHostility.HostileTo), new Type[] { typeof(Verse.Thing), typeof(RimWorld.Faction) })));

            patch.Add(new CodeInstruction(OpCodes.Brtrue_S, labels[2].labels[0]));

            patch.Add(new CodeInstruction(OpCodes.Ldloc_1));
            patch.Add(new CodeInstruction(OpCodes.Ldloc, iter));
            patch.Add(new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<Verse.Thing>), "RemoveAt", new Type[] { typeof(int) })));

            patch.Add(labels[2]);
            patch.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
            patch.Add(new CodeInstruction(OpCodes.Sub));
            patch.Add(new CodeInstruction(OpCodes.Stloc, iter));

            patch.Add(labels[0]);
            patch.Add(new CodeInstruction(OpCodes.Ldc_I4_0));
            patch.Add(new CodeInstruction(OpCodes.Bge_S, labels[1].labels[0]));

            // C# code of the patch
            // List<Thing> list = new List<Thing>(p.Map.listerThings.ThingsOfDef(ThingDefOf.Mech_Apocriton));
            // for (int i = list.Count - 1; i >= 0; i--)
            // {
            //     if (!list[i].HostileTo(p.Faction))
            //     {
            //         list.RemoveAt(i);
            //     }
            // }

            code.Insert(injectionOffset + 1, new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(List<Verse.Thing>), new Type[] { typeof(List<Verse.Thing>) })));
            code.InsertRange(injectionOffset + 3, patch);

            return code;
        }
    }
}
