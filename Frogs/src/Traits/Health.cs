using New_Physics.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using New_Physics.Traits;

namespace Hammer_Knight.src.Traits
{
    public class Health : Trait
    {
        Entity parent;
        public float health;
        public Boolean isVulnerable = true;
        public Boolean isAlive = true;

        public Boolean wasHit = false;
        public Boolean trackDirection = false;
        public Boolean hitFromLeft = false;

        public Health(Entity parent, float health) : base("health", parent)
        {
            this.parent = parent;
            this.health = health;
        }
        public Health(Entity parent, float health, Boolean isVulnerable) : base("health", parent)
        {
            this.parent = parent;
            this.health = health;
            this.isVulnerable = isVulnerable;
        }

        public override void Update()
        {
            wasHit = false;
            trackDirection = false;
        }

        public void Damage(float damage)
        {
            if (!isVulnerable) return;
            wasHit = true;
            health -= damage;
            UpdateAlive();
        }
        public void Damage(float damage, Boolean hitFromLeft)
        {
            if (!isVulnerable) return;
            wasHit = true;
            health -= damage;
            trackDirection = true;
            this.hitFromLeft = hitFromLeft;
            UpdateAlive();
        }

        public void SudoDamage(float damage)
        {
            wasHit = true;
            health -= damage;
            UpdateAlive();
        }

        public void SetHealth(float health)
        {
            this.health = health;
        }

        public void UpdateAlive()
        {
            if (health <= 0) isAlive = false;
        }
    }
}
