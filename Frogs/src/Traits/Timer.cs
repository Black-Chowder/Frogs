using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using New_Physics.Traits;
using New_Physics.Entities;

namespace New_Physics.Traits
{
    public class Timer : Trait
    {
        Entity parent;
        String name = "timer";

        float timer = 0;
        float set = 0;

        public Timer(Entity parent, String name, float set) : base(name, parent)
        {
            this.parent = parent;
            this.name = name;
            this.set = set;
            this.timer = set;
        }

        public override void Update()
        {
            if (timer <= 0)
            {
                timer = 0;
                return;
            }
            timer = timer - 1 * parent.tm;
        }

        //Turn this into an event
        public Boolean didEnd()
        {
            if (timer == 0) return true;
            return false;
        }

        public void Set(int num) { timer = num; }
        public float Get() { return timer; }

        public void Reset() { timer = set; }

        public void Reset(float set)
        {
            timer = set;
            this.set = set;
        }
    }
}
