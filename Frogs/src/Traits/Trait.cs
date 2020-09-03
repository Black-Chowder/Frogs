using New_Physics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace New_Physics.Traits
{
    public abstract class Trait
    {
        public string name;
        Entity parent;

        public Trait(String name, Entity parent)
        {
            this.name = name;
            this.parent = parent;
        }

        public abstract void Update();
    }
}
