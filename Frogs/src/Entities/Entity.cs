using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using New_Physics.Traits;

namespace New_Physics.Entities
{
    public static class EntityHandler
    {
        //List of all entities
        public static List<Entity> entities;

        //Animation Variables
        public static float aniMod = 1;

        public static void Init()
        {
            entities = new List<Entity>();

            //Particle Handler Handler
            //ParticleHandler.Init();

            
            entities.Add(new Player(100, 350));
            entities.Add(new Platform(0, 400, 1000, 500));
            //entities.Add(new Block(200, 100, 50, 50, (float)5, (float)1.5));
            //entities.Add(new Platform(500, 200, 1000, 20));
            //entities.Add(new Block(600, 150, 50, 50, (float)1.8, (float)1));
            entities.Add(new Platform(400, 0, 100, 100));
            //entities.Add(new TestEnemy(700, 300));
            //entities.Add(new Platform(600, 200, 50, 50));
        }

        public static void Update()
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Update();
            }
            //Console.WriteLine("time mod = " + entities[0].tm);

            //ParticleHandler.Update();
        }

        public static void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].Draw(spriteBatch, graphicsDevice);
            }

            //ParticleHandler.Draw(spriteBatch, graphicsDevice);
        }

        //Set time modifier
        public static void setTm(float set)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                entities[i].tm = set;
            }
        }

        //Modify/change time modifier by mod
        public static void modTm(float mod)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].tm + mod >= 0) entities[i].tm += mod;
                else entities[i].tm = 0;
            }
        }
    }
    public abstract class Entity
    {
        public List<Trait> traits;

        public string classId;

        public float x;
        public float y;

        public float dx = 0;
        public float dy = 0;

        public float repDx = 0;
        public float repDy = 0;


        public float width;
        public float height;

        public float tm = 1f;

        public Entity(string classId, float x, float y)
        {
            traits = new List<Trait>();
            this.classId = classId;
            this.x = x;
            this.y = y;
        }

        public void addTrait(Trait t)
        {
            traits.Add(t);
        }

        public Trait getTrait(string name)
        {
            for (int i = 0; i < traits.Count; i++)
            {
                if (traits[i].name == name)
                {
                    return traits[i];
                }
            }
            return null;
        }

        public Boolean hasTrait(string name)
        {
            for (int i = 0; i < traits.Count; i++)
            {
                if (traits[i].name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public abstract void Update();

        protected void traitUpdate()
        {
            for (int i = 0; i < traits.Count; i++)
            {
                if (traits[i].name != "rigidbody") traits[i].Update();
                
            }
            repDx = dx * tm;
            repDy = dy * tm;
            for (int i = 0; i < traits.Count; i++)
            {
                if (traits[i].name == "rigidbody") traits[i].Update();
            }
            repDx = dx * tm;
            repDy = dy * tm;
            x += repDx;
            y += repDy;
        }

        public abstract void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice);
    }
}
