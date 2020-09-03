using New_Physics.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Physics.Traits
{
    public class Gravity : Trait
    {
        Entity parent;
        public float weight;
        public Boolean grounded = false;
        public Gravity(Entity parent, float weight) : base("gravity", parent)
        {
            this.parent = parent;
            this.weight = weight;
        }

        public override void Update()
        {
            parent.dy += weight * parent.tm;
        }
    }
}
