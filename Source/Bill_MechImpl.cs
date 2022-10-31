using RimWorld;
using System.Text;
using Verse;

namespace ResurrectEnemyMechanoids
{
    class Bill_MechImpl : Bill_Mech
    {
        public Bill_MechImpl(RecipeDef recipe, Precept_ThingStyle precept = null)
            : base(recipe, precept)
        {
        }

        public override float BandwidthCost => throw new NotImplementedException();

        public override Pawn ProducePawn()
        {
            throw new NotImplementedException();
        }

        protected override void AppendFormingInspectionData(StringBuilder sb)
        {
            throw new NotImplementedException();
        }
    }
}
