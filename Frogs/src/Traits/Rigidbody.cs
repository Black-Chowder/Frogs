using Microsoft.Xna.Framework;
using New_Physics.Entities;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Frogs.src;

namespace New_Physics.Traits
{
    public class Hitbox
    {
        public float x;
        public float y;

        public float diffX;
        public float diffY;

        public float width;
        public float height;

        public Hitbox(float x, float y, float width, float height)
        {
            this.diffX = x;
            this.diffY = y;
            this.width = width;
            this.height = height;
        }
    }

    public class CollisionData
    {
        public Boolean top = false;
        public Boolean bottom = false;
        public Boolean left = false;
        public Boolean right = false;

        public int cTop;
        public int cBottom;
        public int cLeft;
        public int cRight;

        public int hTop;
        public int hBottom;
        public int hLeft;
        public int hRight;

        public int shTop;
        public int shBottom;
        public int shLeft;
        public int shRight;

        public Boolean oTop = false;
        public Boolean oBottom = false;
        public Boolean oLeft = false;
        public Boolean oRight = false;

        public List<int> cIndexes = new List<int>();

        public CollisionData() { }

        public void Reset()
        {
            top = false;
            bottom = false;
            left = false;
            right = false;

            oTop = false;
            oBottom = false;
            oLeft = false;
            oRight = false;

            cIndexes = new List<int>();
        }
        //, entityIndex, entityHitboxIndex, selfHitboxIndex

        public void SaveIndex(Entity entity)
        {
            for (int i = 0; i < EntityHandler.entities.Count; i++)
            {
                if (entity == EntityHandler.entities[i])
                {
                    cIndexes.Add(i);
                    return;
                }
            }
        }

        public void SaveIndex(int entityIndex)
        {
            cIndexes.Add(entityIndex);
        }

        public void SetTop(Entity entity, int entityIndex, int entityHitboxIndex, int selfHitboxIndex)
        {
            top = true;
            cTop = entityIndex;
            hTop = entityHitboxIndex;
            oTop = entity.hasTrait("rigidbody") && ((Rigidbody)entity.getTrait("rigidbody")).isOverride ? true : false;
            shTop = selfHitboxIndex;
        }

        public void SetBottom(Entity entity, int entityIndex, int entityHitboxIndex, int selfHitboxIndex)
        {
            bottom = true;
            cBottom = entityIndex;
            hBottom = entityHitboxIndex;
            oBottom = entity.hasTrait("rigidbody") && ((Rigidbody)entity.getTrait("rigidbody")).isOverride ? true : false;
            shBottom = selfHitboxIndex;
        }

        public void SetLeft(Entity entity, int entityIndex, int entityHitboxIndex, int selfHitboxIndex)
        {
            left = true;
            cLeft = entityIndex;
            hLeft = entityHitboxIndex;
            oLeft = entity.hasTrait("rigidbody") && ((Rigidbody)entity.getTrait("rigidbody")).isOverride ? true : false;
            shLeft = selfHitboxIndex;
        }

        public void SetRight(Entity entity, int entityIndex, int entityHitboxIndex, int selfHitboxIndex)
        {
            right = true;
            cRight = entityIndex;
            hRight = entityHitboxIndex;
            oRight = entity.hasTrait("rigidbody") && ((Rigidbody)entity.getTrait("rigidbody")).isOverride ? true : false;
            shRight = selfHitboxIndex;
        }

        private int FindEntity(Entity entity)
        {
            for (int i = 0; i < EntityHandler.entities.Count; i++)
            {
                if (EntityHandler.entities[i] == entity)
                {
                    return i;
                }
            }
            return -1;
        }
    }


    public class Rigidbody : Trait
    {
        Entity parent;

        //Stores collision data
        public CollisionData collisionData = new CollisionData();

        //Stores hitboxes
        public List<Hitbox> hitboxes = new List<Hitbox>();

        //isOverride means the entity cannot be pushed by other entities
        public Boolean isOverride = false;

        //isTotal means all rigidbody entities will collide with this entity
        //If set to false, it means that only override rigidbodies will collide with object
        public Boolean isTotal = true;  //<<== May Remove.  Not implemented Yet

        private int entityIndex;
        private int entityHitboxIndex;
        private int selfHitboxIndex;

        public Rigidbody(Entity parent) : base("rigidbody", parent)
        {
            List<Hitbox> temp = new List<Hitbox>();
            temp.Add(new Hitbox(0, 0, parent.width, parent.height));

            Init(parent, temp, false, true);
        }
        public Rigidbody(Entity parent, Boolean isOverride) : base("rigidbody", parent)
        {
            List<Hitbox> temp = new List<Hitbox>();
            temp.Add(new Hitbox(0, 0, parent.width, parent.height));

            Init(parent, temp, isOverride, true);
        }

        public Rigidbody(Entity parent, List<Hitbox> hitboxes, Boolean isOverride) : base("rigidbody", parent)
        {
            Init(parent, hitboxes, isOverride, true);
        }

        public Rigidbody(Entity parent, List<Hitbox> hitboxes, Boolean isOverride, Boolean isTotal) : base("rigidbody", parent)
        {
            Init(parent, hitboxes, isOverride, isTotal);
        }

        private void Init(Entity parent, List<Hitbox> hitboxes, Boolean isOverride, Boolean isTotal)
        {
            this.parent = parent;
            this.isOverride = isOverride;
            this.hitboxes = hitboxes;
            this.isTotal = isTotal;
        }

        public override void Update()
        {

            if (isOverride) return;

            collisionData.Reset();

            //Sets grounded to false if applicable
            Boolean hasGravity = parent.hasTrait("gravity");
            if (hasGravity) ((Gravity)parent.getTrait("gravity")).grounded = false;

            for (int i = 0; i < EntityHandler.entities.Count; i++)
            {
                Entity entity = EntityHandler.entities[i];

                entityIndex = i;

                //Skips entity if entity is self
                if (parent == entity) continue;

                //Skips entity if entity doens't have rigidbody
                if (!entity.hasTrait("rigidbody")) continue;

                //Skips entity if entity.isTotal == false
                if (!((Rigidbody)entity.getTrait("rigidbody")).isTotal) continue;

                if (!((Rigidbody)parent.getTrait("rigidbody")).isTotal && !((Rigidbody)entity.getTrait("rigidbody")).isOverride)
                {
                    isTotalHandler(entity);
                    continue;
                }

                //Checks if entity has override rigidbody
                Boolean isOverride = false;
                if (entity.hasTrait("rigidbody") && ((Rigidbody)entity.getTrait("rigidbody")).isOverride) isOverride = true;

                for (int j = 0; j < hitboxes.Count; j++)
                {
                    selfHitboxIndex = j;
                    for (int k = 0; k < ((Rigidbody)entity.getTrait("rigidbody")).hitboxes.Count; k++)
                    {
                        updateHitboxes();
                        ((Rigidbody)entity.getTrait("rigidbody")).updateHitboxes();

                        entityHitboxIndex = k;

                        calculateCollision(parent, hitboxes[j], entity, ((Rigidbody)entity.getTrait("rigidbody")).hitboxes[k], isOverride);
                    }
                }
            }
        }

        public void updateHitboxes()
        {
            for (int i = 0; i < hitboxes.Count; i++)
            {
                hitboxes[i].x = parent.x - hitboxes[i].diffX;
                hitboxes[i].y = parent.y - hitboxes[i].diffY;
            }
        }

        private void isTotalHandler(Entity entity)
        {
            //Checks if entity has override rigidbody
            Boolean isOverride = false;
            if (entity.hasTrait("rigidbody") && ((Rigidbody)entity.getTrait("rigidbody")).isOverride) isOverride = true;

            for (int j = 0; j < hitboxes.Count; j++)
            {
                selfHitboxIndex = j;
                for (int k = 0; k < ((Rigidbody)entity.getTrait("rigidbody")).hitboxes.Count; k++)
                {
                    updateHitboxes();
                    ((Rigidbody)entity.getTrait("rigidbody")).updateHitboxes();

                    entityHitboxIndex = k;

                    Hitbox parentHitbox = hitboxes[j];
                    Hitbox entityHitbox = ((Rigidbody)entity.getTrait("rigidbody")).hitboxes[k];

                    if (Utils.rectCollision(parentHitbox.x + parent.dx, parentHitbox.y + parent.dy, parentHitbox.width, parentHitbox.height, entityHitbox.x, entityHitbox.y, entityHitbox.width, entityHitbox.height))
                    {
                        collisionData.SaveIndex(entity);
                    }
                }
            }
        }

        private void calculateCollision(Entity parent, Hitbox parentHitbox, Entity entity, Hitbox entityHitbox, Boolean isOverride)
        {
            //Checks if entity is colliding with parent
            //Checking along y axis
            if (Utils.rectCollision(parentHitbox.x, parentHitbox.y + parent.repDy, parentHitbox.width, parentHitbox.height, entityHitbox.x, entityHitbox.y, entityHitbox.width, entityHitbox.height))
            {
                //Colliding with bottom of parent
                if (parentHitbox.y + parent.repDy < entityHitbox.y && !this.isOverride)
                {
                    if (!isOverride) entity.dy += parent.dy;
                    parent.dy = 0;

                    parent.y = entityHitbox.y - parentHitbox.height + parentHitbox.diffY;


                    //Grounds parent if applicable
                    if (parent.hasTrait("gravity")) ((Gravity)parent.getTrait("gravity")).grounded = true;

                    //Update Collision Data
                    collisionData.SetBottom(entity, entityIndex, entityHitboxIndex, selfHitboxIndex);
                }

                //Colliding with top of parent and entity has override rigidbody
                else if (isOverride)
                {
                    parent.dy = 0;
                    parent.y = entityHitbox.y + entityHitbox.height + parentHitbox.diffY;
                    
                    //Update Collision Data
                    collisionData.SetTop(entity, entityIndex, entityHitboxIndex, selfHitboxIndex);
                }

                else
                {
                    if (!isOverride) entity.dy += parent.dy;
                    parent.dy = 0;

                    parent.y = entityHitbox.y + entityHitbox.height - parentHitbox.diffY;

                    //Update Collision Data
                    collisionData.SetTop(entity, entityIndex, entityHitboxIndex, selfHitboxIndex);
                }


            }
            //Checking along x axis
            else if (Utils.rectCollision(parentHitbox.x + parent.repDx, parentHitbox.y + parent.repDy, parentHitbox.width, parentHitbox.height, entityHitbox.x, entityHitbox.y, entityHitbox.width, entityHitbox.height))
            {
                //Checks what side is colliding

                //Colliding with left side of parent
                if (parentHitbox.x < entityHitbox.x)
                {
                    if (!isOverride) entity.dx += parent.dx;
                    parent.dx = 0;
                    if (isOverride) parent.dx = 0;

                    //Moves parent to corresponding side of entity
                    parent.x = entityHitbox.x - parentHitbox.width - parent.dx + parentHitbox.diffX;

                    //Update Collision Data
                    collisionData.SetRight(entity, entityIndex, entityHitboxIndex, selfHitboxIndex);
                }
                //Handles of entity is override (I haven't tested taking this part away yet)
                else if (isOverride)
                {
                    parent.dx = 0;

                    //Moves parent to corresponding side of entity
                    parent.x = entityHitbox.x + entityHitbox.width + parentHitbox.diffX;

                    //Update Collision Data
                    collisionData.SetLeft(entity, entityIndex, entityHitboxIndex, selfHitboxIndex);
                }
                //Colliding with right side of parent
                else
                {
                    if (!isOverride) entity.dx += parent.dx;
                    parent.dx = 0;

                    //Moves parent to corresponding side of entity
                    parent.x = entityHitbox.x + entityHitbox.width - parent.dx + parentHitbox.diffX;

                    //Update Collision Data
                    collisionData.SetLeft(entity, entityIndex, entityHitboxIndex, selfHitboxIndex);
                }

            }
        }
    }
}
