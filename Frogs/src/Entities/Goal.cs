using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using New_Physics.Entities;
using Microsoft.Xna.Framework;
using New_Physics.Traits;
using Frogs.src;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Frogs.src.Entities
{
    public static class GoalSprites
    {
        public static SpriteFont font;

        public static SoundEffect achive;

        public static Texture2D fly;

        public static void LoadContent(ContentManager Content)
        {
            font = Content.Load<SpriteFont>(@"Score");
            achive = Content.Load<SoundEffect>(@"Achive");
            fly = Content.Load<Texture2D>(@"Fly");
        }
    }
    public class GoalHandler : Entity
    {
        List<Goal> goals;
        public int score = 0;
        public int maxScore = 0;

        public GoalHandler() : base("goalHandler", 0, 0)
        {
            goals = new List<Goal>();
        }

        public override void Update()
        {
            for (int i = 0; i < goals.Count; i++)
            {
                goals[i].Update();
                //Score Detection
                if (!goals[i].Exists)
                {
                    goals.RemoveAt(i);
                    score++;
                    GoalSprites.achive.Play();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            //Draw Goals
            for (int i = 0; i < goals.Count; i++)
            {
                goals[i].Draw(spriteBatch, graphicsDevice);
            }

            //Draw Score
            spriteBatch.Begin();

            spriteBatch.DrawString(GoalSprites.font, "Flies Eaten: " + score + " / " + maxScore, new Vector2(25, 25), Color.Black);

            spriteBatch.End();
        }

        public void createGoal(float x, float y)
        {
            goals.Add(new Goal(x, y));
            maxScore = goals.Count;
        }
    }

    public class Goal : Entity
    {
        //Hitbox Variables
        Hitbox myHitbox = new Hitbox(-20*Camera.gameScale, -20 * Camera.gameScale, 20 * Camera.gameScale, 20 * Camera.gameScale);

        Boolean foundPlayer = false;
        List<int> playerIndexes;

        //Testing Variables
        private Boolean drawHitbox = false;

        public Goal(float x, float y) : base("goal", x, y)
        {
            playerIndexes = new List<int>();

            width = 40 * Camera.gameScale;
            height = 40 * Camera.gameScale;
        }

        public override void Update()
        {
            //Finds and stores player index
            if (!foundPlayer)
            {
                foundPlayer = true;
                for (int i = 0; i < EntityHandler.entities.Count; i++)
                {
                    Entity entity = EntityHandler.entities[i];
                    if (entity.classId == "player") playerIndexes.Add(i);
                }
            }

            //Update Hitbox
            myHitbox.x = x - myHitbox.diffX;
            myHitbox.y = y - myHitbox.diffY;

            //Collision Detection
            for (int i = 0; i < playerIndexes.Count; i++)
            {
                Player player = (Player)EntityHandler.entities[playerIndexes[i]];
                Rigidbody playerRigidbody = (Rigidbody)player.getTrait("rigidbody");
                
                for (int j = 0; j < playerRigidbody.hitboxes.Count; j++)
                {
                    Hitbox playerHitbox = playerRigidbody.hitboxes[j];
                    if (Utils.rectCollision(
                        myHitbox.x,
                        myHitbox.y,
                        myHitbox.width,
                        myHitbox.height,
                        playerHitbox.x,
                        playerHitbox.y,
                        playerHitbox.width,
                        playerHitbox.height))
                    {
                        //Scored
                        Exists = false;
                    }
                }
                
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (drawHitbox)
            {
                Texture2D texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
                texture.SetData<Color>(new Color[] { Color.White });
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                //Draw Hitbox
                spriteBatch.Draw(texture,
                    new Rectangle((int)(myHitbox.x - Camera.X), (int)(myHitbox.y - Camera.Y), (int)(myHitbox.width), (int)(myHitbox.height)),
                    Color.Black);

                spriteBatch.End();
                texture.Dispose();
            }

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(GoalSprites.fly,
                new Rectangle((int)(x + width/4 - Camera.X), (int)(y - Camera.Y), (int)(width), (int)(height)),
                Color.White);

            spriteBatch.End();
        }
    }
}
