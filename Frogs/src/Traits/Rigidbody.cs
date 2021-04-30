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
using Black_Magic.src;

namespace New_Physics.Traits
{
    //WARNING: DO NOT USE THIS SPECIFIC FILE OF CODE AS A TEMPLATE FOR ANY FUTURE PROJECTS
    public class Hitbox
    {
        Entity parent;
        public float x;
        public float y;
        public float diffX;
        public float diffY;
        public float width; //Width is repurposed to be radius when isCircle = true
        public float height;
        public Boolean isCircle = false;

        //Collision Data
        public Entity left = null;
        public Entity right = null;
        public Entity top = null;
        public Entity bottom = null;

        //Constructor(s)
        //Auto-Generate (square) Hitbox
        public Hitbox(Entity parent)
        {
            this.parent = parent;
            diffX = 0;
            diffY = 0;
            width = parent.width;
            height = parent.height;

            Update();
        }

        //Square Hitbox
        public Hitbox(Entity parent, float diffX, float diffY, float width, float height)
        {
            this.parent = parent;
            this.diffX = diffX;
            this.diffY = diffY;
            this.width = width;
            this.height = height;

            Update();
        }

        //Circle Hitbox
        public Hitbox(Entity parent, float diffX, float diffY, float radius)
        {
            this.parent = parent;
            this.diffX = diffX;
            this.diffY = diffY;
            this.width = radius;
            this.height = radius;
            isCircle = true;

            Update();
        }

        //Updates position of hitboxes relative to parent
        public void Update()
        {
            x = parent.x + diffX;
            y = parent.y + diffY;
        }

        public void resetCollisionData()
        {
            left = null;
            right = null;
            top = null;
            bottom = null;
        }

        //Gets distance from edge of hitbox to a given point
        public float getDistance(Vector2 point)
        {
            //Calculations for circle
            if (isCircle)
            {
                float cx = x;
                float cy = y;
                float cr = width / 2;

                return Utils.getDistance(point.X, point.Y, cx, cy) - cr;
            }

            //Calculations for rectangle
            // Ripped from  https://www.youtube.com/watch?v=Cp5WWtMoeKg&t=45s
            // signed -> if point is inside the rect, then dist is negative
            float px = point.X;
            float py = point.Y;
            //These are myself I assume
            float rx = x;
            float ry = y;
            float rw = width;
            float rh = height;

            float ox = rx + rw / 2;
            float oy = ry + rh / 2;
            float offsetX = Math.Abs(px - ox) - rw / 2;
            float offsetY = Math.Abs(py - oy) - rh / 2;

            float unsignedDist = Utils.getDistance(0, 0, Math.Max(offsetX, 0), Math.Max(offsetY, 0));
            float distInsideBox = Math.Max(Math.Min(offsetX, 0), Math.Min(offsetY, 0));
            return unsignedDist + distInsideBox;
        }

        // Getters / Setters
        //TODO: Create getters and setters
    }
    public class Rigidbody : Trait
    {
        //Stores hitboxes
        public List<Hitbox> hitboxes = new List<Hitbox>();

        //isOverride means the entity cannot be pushed by other entities
        public Boolean isOverride = false;

        //isTotal means all rigidbody entities will collide with this entity
        public Boolean isTotal = false;  //<<== May Remove.  Not implemented Yet

        public Boolean isCircle = false; //Maybe change (probably change)

        //The number of rays the rigibody samples from 
        public int raysPerSide = 4;

        //Thickness from how far inside the hitbox the collision detection rays will be cast
        public float skinWidth = 5;

        // <Temporary Testing Variables>
        public List<Rectangle> testRects = new List<Rectangle>();
        public Boolean testBool = false;
        public List<Entity> doNotCollide = new List<Entity>();
        // </Temporary Testing Variables>

        //Constructor(s)
        const String traitName = "rigidbody";
        public Rigidbody(Entity parent, Boolean isOverride = false, Boolean isCircle = false) : base(traitName, parent)
        {
            this.isOverride = isOverride;

            //Create Hitbox
            if (isCircle) hitboxes.Add(new Hitbox(parent, 0, 0, parent.width));
            else hitboxes.Add(new Hitbox(parent, 0, 0, parent.width, parent.height));
        }

        //Pass in collision method
        //TODO

        //Allows entity to create its own hitboxes
        public Rigidbody(Entity parent, List<Hitbox> hitboxes, Boolean isOverride = false) : base(traitName, parent)
        {
            this.isOverride = isOverride;
            this.hitboxes = hitboxes;
        }



        //Gets the distance from the edge of the shape (only accepts circle and rectangle at the moment)
        //and a point given to it.
        public float getDistance(Vector2 pos)
        {
            float shortestDist = float.PositiveInfinity;
            foreach (Hitbox hitbox in hitboxes)
            {
                hitbox.Update();
                float dist = hitbox.getDistance(pos);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                }
            }
            return shortestDist;
        }

        public void updateHitboxes()
        {
            foreach (Hitbox hitbox in hitboxes)
            {
                hitbox.Update();
            }
        }

        //Actual Updating Part//
        public override void Update()
        {
            //Clear Testing Variables
            testRects = new List<Rectangle>();
            testBool = false;

            updateHitboxes();

            if (isOverride) return;

            Black_Magic.src.Ray ray;
            Vector2? rayData;

            Boolean hasGravity = parent.hasTrait("gravity");
            Gravity gravity = (Gravity)parent.getTrait("gravity");
            gravity.grounded = false;

            //TODO: if !isOverride, then change self variables and others to properly apply forces

            foreach (Hitbox hitbox in hitboxes)
            {
                hitbox.resetCollisionData();
                for (int i = 0; i < raysPerSide; i++)
                {
                    updateHitboxes();
                    //Calculate Ray Casting Points
                    float raycastY = hitbox.y + skinWidth + (hitbox.height - skinWidth * 2) * i / (raysPerSide - 1);
                    float raycastX = hitbox.x + skinWidth + (hitbox.width - skinWidth * 2) * i / (raysPerSide - 1);

                    //Local Variable Used For Storing Entity Currently Colliding With:
                    Entity entity;
                    Rigidbody entityRigidbody;

                    //Top
                    ray = new Black_Magic.src.Ray(raycastX, hitbox.y + skinWidth, (float)(Math.PI * 3 / 2));
                    rayData = ray.cast(EntityHandler.entities, parent);
                    if (rayData.HasValue && rayData.Value.Y > hitbox.y + parent.dy && parent.dy < 0)
                    {
                        entity = ray.getEntity();
                        entityRigidbody = (Rigidbody)entity.getTrait(traitName);
                        if (!entityRigidbody.isOverride) entity.dy = parent.dy;

                        hitbox.top = entity;

                        parent.dy = 0;
                        parent.y = rayData.Value.Y - hitbox.diffY;
                    }

                    //Bottom
                    ray = new Black_Magic.src.Ray(raycastX, hitbox.y + hitbox.height - skinWidth, (float)(Math.PI / 2));
                    rayData = ray.cast(EntityHandler.entities, parent);
                    if (rayData.HasValue && rayData.Value.Y < hitbox.y + hitbox.height + parent.dy && parent.dy > 0)
                    {
                        entity = ray.getEntity();
                        entityRigidbody = (Rigidbody)entity.getTrait(traitName);
                        if (!entityRigidbody.isOverride) entity.dy = parent.dy;

                        hitbox.bottom = entity;
                        if (hasGravity) gravity.grounded = true;

                        parent.dy = 0;
                        parent.y = rayData.Value.Y - hitbox.height - hitbox.diffY;
                    }

                    //Right
                    ray = new Black_Magic.src.Ray(hitbox.x + hitbox.width - skinWidth, raycastY, 0);
                    rayData = ray.cast(EntityHandler.entities, parent);
                    if (rayData.HasValue && rayData.Value.X < hitbox.x + hitbox.width + parent.dx && parent.dx > 0)
                    {
                        entity = ray.getEntity();
                        entityRigidbody = (Rigidbody)entity.getTrait(traitName);
                        if (!entityRigidbody.isOverride) entity.dx = parent.dx;

                        hitbox.left = entity;

                        parent.dx = 0;
                        parent.x = rayData.Value.X - hitbox.width - hitbox.diffX;
                    }

                    //Left
                    ray = new Black_Magic.src.Ray(hitbox.x + skinWidth, raycastY, (float)(Math.PI));
                    rayData = ray.cast(EntityHandler.entities, parent);
                    if (rayData.HasValue && rayData.Value.X > hitbox.x + parent.dx && parent.dx < 0)
                    {
                        entity = ray.getEntity();
                        entityRigidbody = (Rigidbody)entity.getTrait(traitName);
                        if (!entityRigidbody.isOverride) entity.dx = parent.dx;

                        hitbox.right = entity;

                        parent.dx = 0;
                        parent.x = rayData.Value.X - hitbox.diffX;
                    }
                }
            }
        }
    }
}
