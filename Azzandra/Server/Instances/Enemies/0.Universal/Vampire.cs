using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Vampire : Enemy
    {
        public override int GetSightRange() => 10;
        public override int AggressiveRange => 5;
        public override int GetMovementSpeed() => Form == FormType.Humanoid ? 1 : 2;
        public override EntityType EntityType => EntityType.Vampire;


        protected virtual int GetMaxShapeShiftTime() => 20;
        protected int ShapeShiftTimer = 0;
        protected bool CanShapeShift = true;


        public enum FormType { Humanoid, Bat, Mist };
        public FormType Form { get; private set; } = FormType.Humanoid;


        public Vampire(int x, int y) : base(x, y)
        {

        }


        public override void Turn()
        {
            base.Turn();

            if (ShapeShiftTimer <= 0)
            {
                //Revert to humanoid
                if (Form != FormType.Humanoid)
                {
                    Form = FormType.Humanoid;
                    SetMoveType(MoveType.Walk);
                    ShapeShiftTimer = -1;
                }

                //Change to bat
                if (CanShapeShift && Hp <= 15)
                {
                    Form = FormType.Bat;
                    SetMoveType(MoveType.Fly);
                    ShapeShiftTimer = GetMaxShapeShiftTime();
                    CanShapeShift = false;

                    //Lose target
                    Target = null;
                }
            }
            else
            {
                ShapeShiftTimer--;
            }
        }


        //Targeting:
        protected override bool FightBack() => Form == FormType.Humanoid ? true : false;
        protected override bool IsATarget(Entity inst)
        {
            if (Form == FormType.Humanoid && Hp > 10)
                return base.IsATarget(inst);
            else
                return false;
        }


        public override Symbol GetSymbol()
        {
            var col = Target != null ? Color.Red : Color.Maroon;
            char c = Form == FormType.Humanoid ? 'v' : Form == FormType.Bat ? 'b' : 'm';
            return new Symbol(c, col);
        }

        public override string Name => Form == FormType.Humanoid ? "vampire" : Form == FormType.Bat ? "vampire bat" : "vampire mist";
    }
}
