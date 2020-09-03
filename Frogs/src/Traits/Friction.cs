using New_Physics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Physics.Traits
{
    public class Friction : Trait
    {
        Entity parent;
        public float coefficient;
        public float airCoefficient;
        public float frictionForce = 0;
        public Friction(Entity parent, float coefficient) : base("friction", parent)
        {
            this.parent = parent;
            this.coefficient = coefficient;
            airCoefficient = 0;
        }

        public Friction(Entity parent, float coefficient, float airCoefficient) : base("friction", parent)
        {
            this.parent = parent;
            this.coefficient = coefficient;
            this.airCoefficient = airCoefficient;
        }


        public override void Update()
        {
            if (parent.hasTrait("gravity") && ((Gravity)parent.getTrait("gravity")).grounded && coefficient != 0) 
                frictionForce = parent.dx - (parent.dx) / coefficient;

            else if (airCoefficient != 0) 
                frictionForce = parent.dx - (parent.dx) / airCoefficient;

            else 
                frictionForce = 0;

            parent.dx -= (frictionForce * parent.tm);
        }
    }
}
