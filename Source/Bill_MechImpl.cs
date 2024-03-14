using RimWorld;
using System.Text;
using System;
using Verse;

namespace ResurrectEnemyMechanoids
{
    class Bill_MechImpl(RecipeDef recipe, Precept_ThingStyle precept = null) : Bill_Mech(recipe, precept)
    {
        public override float BandwidthCost => throw new NotImplementedException();
    }
}
