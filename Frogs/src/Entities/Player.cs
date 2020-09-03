using Microsoft.Xna.Framework.Graphics;
using New_Physics.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Hammer_Knight.src;
using System.ComponentModel;
using System.Linq.Expressions;
using Hammer_Knight.src.Traits;
using Frogs.src;

namespace New_Physics.Entities
{
    public static class PlayerSprites
    {
        public static Texture2D cursor;

        public static Texture2D player;
        public static Rectangle[] idle;
        public static Rectangle[] prepSling;
        public static Rectangle[] slingLaunch;
        //Attack Animation Load
        public static Rectangle[] anticipa1;
        public static Rectangle[] cont1;
        public static Rectangle[] recov1;

        public static void LoadContent(ContentManager Content)
        {
            //cursor = Content.Load<Texture2D>("Cursor");

            //JumpDustSprites.LoadContent(Content);
            //PlayerSmashSprites.LoadContent(Content);

        }
    }


    public class Player : Entity
    {
        private float speed = 2.2f;
        MouseState mouse;

        Boolean isFacingRight = true;

        String animation = "neutral";
        int animator = 0;

        // <Player Slingshot>
        private float startingX;
        private float startingY;
        private Boolean isSlinging = false;

        private float maxSlingPowerX = 15;
        private float maxSlingPowerY = 25;

        private float dragPower = .25f;

        private int projectionRange = 1000;

        private int toProjectModifier = 5;
        private Projection[] projections;

        private float minSlingX = 3;
        private float minSlingY = 3;

        private int shouldSlamTimer = 0;
        private const int shouldSlamTimerMax = 20;
        private Boolean shouldSlam = false;
        // </Player Slingshot>

        // <Hanging Variables>
        private Boolean isHanging = false;
        private Boolean hangingRight = false;
        private int hangingEntity;
        private int hangingHitbox;
        private int selfHangingHitbox;

        private float hangingX;
        private float hangingY;
        // </Hanging Variables>

        // <Attack Variables>
        private Boolean isAttacking = false;
        private int attPhase = 1;//1 = anticipation, 2 = contact, 3 = recovery

        private Hitbox attackHitboxR;
        private Hitbox attackHitboxL;

        private Hitbox attackHitboxSlam;
        // </Attack Variables>


        //Testing Variables
        private Boolean showHitbox = true;

        public Player(float x, float y) : base("player", x, y)
        {
            width = 50;
            height = 50;
            addTrait(new Gravity(this, 1f));
            addTrait(new Friction(this, (float)1.5 ,(float)1));
            addTrait(new Timer(this, "timer", 300));

            List<Hitbox> hitboxes = new List<Hitbox>();
            hitboxes.Add(new Hitbox(width/2, height/2, width, height));
            //hitboxes.Add(new Hitbox(-25, 56, this.width, 5));

            addTrait(new Rigidbody(this, hitboxes, false));

            //Setup Attack Hitboxes
            attackHitboxR = new Hitbox(0, 0, width * 2, height * 2);
            attackHitboxL = new Hitbox(width * 2, height * 2, width * 2, height * 2);

            attackHitboxSlam = new Hitbox(width * 2, height * 2, width * 4, height * 4);
        }

        public override void Update()
        {
            animation = "neutral";
            KeyboardState keys = Keyboard.GetState();
            mouse = Mouse.GetState();

            if (keys.IsKeyDown(Keys.W))
            {
                dy -= speed * tm;
            }
            if (keys.IsKeyDown(Keys.A))
            {
                isFacingRight = false;
                dx -= speed * tm;
            }
            if (keys.IsKeyDown(Keys.S))
            {
                dy += speed * tm;
            }
            if (keys.IsKeyDown(Keys.D))
            {
                isFacingRight = true;
                dx += speed * tm;
            }

            //Modifying time
            if (keys.IsKeyDown(Keys.Down)) EntityHandler.modTm(.01f);
            else if (keys.IsKeyDown(Keys.Up)) EntityHandler.modTm(-.01f);

            if (Math.Abs(dx) > 10)
            {
                if (dx < 0) dx = -10;
                else dx = 10;
            }

            //Sling Handling
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                beginSling();
            }
            else if (mouse.LeftButton != ButtonState.Pressed)
            {
                if (isSlinging) isHanging = false;
                releaseSling();
            }
            slingHandler();


            //  UPDATE TRAITS  //
            base.traitUpdate();  //  <<<======  UPDATE TRAITS
            //  UPDATE TRAITS  //



            //Pretty self explanatory
            prepAnimation();

            Camera.GoTo(this.x - Camera.Width/2, this.y - Camera.Height/2);
        }

        private void prepAnimation()
        {
            animator++;
            //TODO: Change if statements to switch statemets
            if (animator < 0)
            {
                animator = 0;
            }
            if (animation == "neutral" && animator != 0)
            {
                animator = 0;
            }
        }

        [Obsolete]
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            //Draw Hitbox
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (showHitbox) for (int i = 0; i < ((Rigidbody)getTrait("rigidbody")).hitboxes.Count; i++)
            {
                Hitbox hitbox = ((Rigidbody)getTrait("rigidbody")).hitboxes[i];
                spriteBatch.Draw(texture, new Rectangle((int)(hitbox.x - Camera.X), (int)(hitbox.y - Camera.Y), (int)(hitbox.width), (int)(hitbox.height)), Color.White);
            }

            


            //Draws Cursor

            spriteBatch.End();
            

            spriteBatch.Begin();


            if (isSlinging)
            {
                //Shows projection dots of estemated trajectory
                for (int i = 0; i < projections.Count(); i++)
                {
                    Projection projection = projections[i];
                    if (projection == null) continue;

                    //Draws Projection Dots
                    spriteBatch.Draw(texture,
                        new Rectangle((int)(projection.x - Camera.X), (int)(projection.y - Camera.Y), 5, 5),
                        Color.Red);
                }

                //Shows line from starting sling to current mouse position
                Utils.DrawLine(spriteBatch, 
                    new Vector2((int)(mouse.X), (int)(mouse.Y)), 
                    new Vector2((int)(startingX), (int)(startingY)), 
                    Color.Black);
            }

            spriteBatch.End();
            texture.Dispose();
        }
        // </Draw>

        private void setAnimation(String animation)
        {
            this.animation = animation;
            animator = 0;
        }


        private void beginSling()
        {
            if (isAttacking) return;
            animation = "prepSling";
            if (isSlinging) return;
            isSlinging = true;
            startingX = mouse.X;
            startingY = mouse.Y;
        }

        private Boolean slingInit = false;

        private void releaseSling()
        {
            if (!isSlinging || isAttacking) return;


            float xPower = (startingX - mouse.X) * dragPower;
            float yPower = (startingY - mouse.Y) * dragPower;

            
            if ((xPower < minSlingX && xPower > -minSlingX) && (yPower < minSlingY && yPower > -minSlingY))
            {
                isSlinging = false;
                return;
            }

            //Handle Sling X Axis
            if (xPower > maxSlingPowerX || xPower < -maxSlingPowerX) if (xPower > 0) dx += maxSlingPowerX; else dx -= maxSlingPowerX;
            else dx += xPower;

            //Handle Sling Y Axis
            if (yPower > maxSlingPowerY || yPower < -maxSlingPowerY) if (yPower > 0) dy += maxSlingPowerY; else dy -= maxSlingPowerY;
            else dy += yPower;

            isSlinging = false;

            if (dx > 0) isFacingRight = true;
            else isFacingRight = false;

            //Create Particle
            //if (((Gravity)getTrait("gravity")).grounded) ParticleHandler.particles.Add(new JumpDust(x, y, true));

            //Sets up animation for sling handler
            animation = "slingLaunch";
            slingInit = true;
            ((Gravity)getTrait("gravity")).grounded = false;
        }

        private void slingHandler()
        {
            //Animation Handler
            if (slingInit)
            {
                animation = "slingLaunch";

                shouldSlamTimer++;

                if (shouldSlamTimer >= shouldSlamTimerMax)
                {
                    shouldSlam = true;
                }

                if (((Gravity)getTrait("gravity")).grounded)
                {
                    if (shouldSlam)
                    {
                        Camera.Shake(100, 15);
                        //ParticleHandler.particles.Add(new PlayerSmash(x, y, isFacingRight));

                        //Attack Handler
                        for (int i = 0; i < EntityHandler.entities.Count; i++)
                        {
                            Entity entity = EntityHandler.entities[i];
                            if (entity == this || entity.classId != "hostile") continue;

                            for (int j = 0; j < ((Rigidbody)entity.getTrait("rigidbody")).hitboxes.Count(); j++)
                            {
                                Hitbox entityHitbox = ((Rigidbody)entity.getTrait("rigidbody")).hitboxes[j];

                                //Update Hitboxes
                                attackHitboxSlam.x = x - attackHitboxSlam.diffX;
                                attackHitboxSlam.y = y - attackHitboxSlam.diffY;

                                //Actual Collision Calculations
                                if (Utils.rectCollision(attackHitboxSlam.x, attackHitboxSlam.y, attackHitboxSlam.width, attackHitboxSlam.height, entityHitbox.x, entityHitbox.y, entityHitbox.width, entityHitbox.height))
                                {
                                    if (x > entity.x)
                                    {
                                        ((Health)entity.getTrait("health")).Damage(1, false);
                                    }
                                    else
                                    {
                                        ((Health)entity.getTrait("health")).Damage(1, true);
                                    }
                                }
                            }
                        }
                    }
                    slingInit = false;
                }
            }
            else
            {
                shouldSlamTimer = 0;
                shouldSlam = false;
            }
            

            //Handles Trajectory Calculations
            if (!isSlinging || isAttacking) return;

            projections = new Projection[projectionRange / toProjectModifier];
            int projected = 0;

            float ghostX = x;
            float ghostY = y;

            float xPower = (startingX - mouse.X) * dragPower;
            float yPower = (startingY - mouse.Y) * dragPower;

            //Sets ghost delta position to power or to max power
            float ghostDx = (xPower > maxSlingPowerX || xPower < -maxSlingPowerX) ? xPower > 0 ? maxSlingPowerX : -maxSlingPowerX : xPower;
            float ghostDy = (yPower > maxSlingPowerY || yPower < -maxSlingPowerY) ? yPower > 0 ? maxSlingPowerY : -maxSlingPowerY : yPower;


            float ghostWeight = ((Gravity)getTrait("gravity")).weight;

            float ghostFriction = ((Friction)getTrait("friction")).airCoefficient;

            List<Hitbox> ghostHitboxes;
            ghostHitboxes = ((Rigidbody)getTrait("rigidbody")).hitboxes;

            //Temporary variable to detect when projection has hit an object
            Boolean detected = false;

            //Applies ground friction to first itteration for more accurate projection
            ghostDx -= ghostDx - (ghostDx) / ((Friction)getTrait("friction")).coefficient;

            //  Simulate Trajectory  //
            for (int i = 0; i < projectionRange; i++)
            {
                //Apply Gravity
                ghostDy += ghostWeight; // * parent.tm

                //Apply Friction
                ghostDx -= ghostDx - (ghostDx) / ghostFriction;

                //Detect If Colliding
                for (int j = 0; j < EntityHandler.entities.Count; j++)
                {
                    Entity entity = EntityHandler.entities[j];
                    if (entity.classId == "player" || !entity.hasTrait("rigidbody")) continue;

                    for (int k = 0; k < ghostHitboxes.Count; k++)
                    {
                        Hitbox ghostHitbox = ghostHitboxes[k];
                        Rigidbody entityRigidbody = ((Rigidbody)entity.getTrait("rigidbody"));
                        for (int l = 0; l < entityRigidbody.hitboxes.Count; l++)
                        {
                            Hitbox entityHitbox = entityRigidbody.hitboxes[l];
                            //Rectangle collision on entities.  If true, end loop and tell where is
                            if (Utils.rectCollision(ghostX - ghostHitbox.diffX, ghostY - ghostHitbox.diffY, ghostHitbox.width, ghostHitbox.height, entityHitbox.x, entityHitbox.y, entityHitbox.width, entityHitbox.height))
                            {
                                detected = true;
                                break;
                            }
                        } if (detected) break;
                    }if (detected) break;
                }

                //Settle expected landing position if detected, then break
                if (detected)
                {
                    projections[projections.Length - 1] = new Projection(ghostX, ghostY);
                    break;
                }

                //Draws Projections In Series
                if ((projectionRange % toProjectModifier) == 0 && projected != projections.Length - 1)
                {
                    projections[projected] = new Projection(ghostX, ghostY);
                    projected++;
                }

                //Move Ghost
                ghostX += ghostDx;
                ghostY += ghostDy;
            }
        }
    }

    //Used by the player class to display where the player will be after a leap
    public class Projection
    {
        public float x;
        public float y;
        public Projection(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}