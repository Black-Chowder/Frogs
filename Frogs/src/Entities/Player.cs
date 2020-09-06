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
            cursor = Content.Load<Texture2D>("Cursor");

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

        // <Attack Variables>
        private Boolean isAttacking = false;
        // </Attack Variables>

        // <Swing Variables>
        private Boolean isSwinging = false;

        //Tracks center of swing
        private float sox = 0;
        private float soy = 0;
        private float tongueLength = 0;
        private float maxTongueLength = 0;

        private Vector2 prePos = new Vector2(0, 0);
        private Vector2 preDel = new Vector2(0, 0);

        private Boolean swingInit = true;

        // </Swing Variables>


        //Testing Variables
        private Boolean showHitbox = true;

        public Player(float x, float y) : base("player", x, y)
        {
            width = 50;
            height = 50;
            addTrait(new Gravity(this, 1f));
            addTrait(new Friction(this, (float)1.5 ,(float)1.02));
            addTrait(new Timer(this, "timer", 300));

            List<Hitbox> hitboxes = new List<Hitbox>();
            hitboxes.Add(new Hitbox(width/2, height/2, width, height));
            //hitboxes.Add(new Hitbox(-25, 56, this.width, 5));

            addTrait(new Rigidbody(this, hitboxes, false));
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
                releaseSling();
            }
            slingHandler();


            //Swing Handling
            if (mouse.RightButton == ButtonState.Pressed && swingInit)
            {
                swingInit = false;
                if (isSwinging) releaseSwing();
                else sendSwing();
            }
            else if (mouse.RightButton != ButtonState.Pressed) swingInit = true;


            //Save current position before moving
            //Used for swing variables to calculate velocity
            prePos = new Vector2(x, y);
            preDel = new Vector2(dx, dy);

            //  UPDATE TRAITS  //
            base.traitUpdate();  //  <<<======  UPDATE TRAITS
            //  UPDATE TRAITS  //

            //Handle Swinging From Tongue
            swingHandler();



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
            spriteBatch.Draw(PlayerSprites.cursor,
                destinationRectangle: new Rectangle((int)(mouse.X), (int)(mouse.Y), 30, 30),
                color: Color.White);



            spriteBatch.End();
            

            spriteBatch.Begin();

            if (isSwinging)
            {
                spriteBatch.Draw(texture,
                    new Rectangle((int)(sox - Camera.X), (int)(soy - Camera.Y), 50, 5),
                    Color.White);

                //Draw Tongue
                Utils.DrawLine(spriteBatch,
                    new Vector2((int)(sox - Camera.X), (int)(soy - Camera.Y)),
                    new Vector2((int)(x - Camera.X), (int)(y - Camera.Y)),
                    Color.Red);

                spriteBatch.Draw(texture,
                    new Rectangle((int)(sox - 5 - Camera.X), (int)(soy - 5 - Camera.Y), 10, 10),
                    Color.Red);
            }


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

        private void sendSwing()
        {
            sox = mouse.X + Camera.X;
            soy = mouse.Y + Camera.Y;

            List<Vector2> tongueEnds = new List<Vector2>();

            //Calculate endpoint of tongue
            for (int i = 0; i < EntityHandler.entities.Count; i++)
            {
                //Skip if not a platform
                if (EntityHandler.entities[i].classId != "platform") continue;
                
                Entity entity = EntityHandler.entities[i];
                Rigidbody entityR = ((Rigidbody)entity.getTrait("rigidbody"));

                for (int j = 0; j < entityR.hitboxes.Count; j++)
                {
                    Hitbox eHitbox = entityR.hitboxes[j];
                    //Line Calculations
                    //m = ((y - soy)/(x - sox))
                    //b = (y - ((y - soy)/(x - sox))*X)
                    //f(actX) = ((y - soy)/(x - sox))*actX + (y - ((y - soy)/(x - sox))*X)

                    float m = ((y - soy) / (x - sox));
                    float b = (y - ((y - soy) / (x - sox)) * x);

                    //Collision With Right Of Hitboxes
                    sox = eHitbox.x + eHitbox.width;
                    soy = m * (eHitbox.x + eHitbox.width) + b;
                    if (soy > eHitbox.y && soy < eHitbox.y + eHitbox.height && 
                        Utils.getDistance(x, y, sox, soy) > Utils.getDistance(mouse.X + Camera.X, mouse.Y + Camera.Y, sox, soy))
                    {
                        isSwinging = true;
                        tongueEnds.Add(new Vector2(sox, soy));
                    }

                    //Collision With Left Of Hitboxes
                    sox = (eHitbox.x);
                    soy = m * eHitbox.x + b;
                    if (soy > eHitbox.y && soy < eHitbox.y + eHitbox.height && 
                        Utils.getDistance(x, y, sox, soy) > Utils.getDistance(mouse.X + Camera.X, mouse.Y + Camera.Y, sox, soy))
                    {
                        isSwinging = true;
                        tongueEnds.Add(new Vector2(sox, soy));
                    }

                    //Collision With Top Of Hitboxes
                    sox = (eHitbox.y - b) / m;
                    soy = eHitbox.y;
                    if (sox > eHitbox.x && sox < eHitbox.x + eHitbox.width &&
                        Utils.getDistance(x, y, sox, soy) > Utils.getDistance(mouse.X + Camera.X, mouse.Y + Camera.Y, sox, soy))
                    {
                        isSwinging = true;
                        tongueEnds.Add(new Vector2(sox, soy));
                    }

                    //Collision With Bottom Of Hitboxes
                    sox = ((eHitbox.y + eHitbox.height) - b)/m;
                    soy = eHitbox.y + eHitbox.height;
                    //Make sure is within bounds of entity hitbox
                    if (sox > eHitbox.x && sox < eHitbox.x + eHitbox.width
                        && Utils.getDistance(x, y, sox, soy) > Utils.getDistance(mouse.X + Camera.X, mouse.Y + Camera.Y, sox, soy))
                    {
                        isSwinging = true;
                        tongueEnds.Add(new Vector2(sox, soy));
                    }
                }
            }

            //Find closest tongue end to player
            Vector2 closest = new Vector2(0, 0);
            Boolean foundOne = false;
            for (int i = 0; i < tongueEnds.Count; i++)
            {
                if (!foundOne)
                {
                    closest = tongueEnds[i];
                    foundOne = true;
                    continue;
                }

                if (Utils.getDistance(x, y, tongueEnds[i].X, tongueEnds[i].Y) < Utils.getDistance(x, y, closest.X, closest.Y)) closest = tongueEnds[i];

            }

            sox = closest.X;
            soy = closest.Y;
            tongueLength = Utils.getDistance(sox, soy, x, y);
        }

        private void releaseSwing()
        {
            isSwinging = false;
        }

        private void swingHandler()
        {
            if (!isSwinging) return;
            Gravity gravity = (Gravity)getTrait("gravity");

            //Handle Swinging

            //Set difference between tongue end and player position to unit circle(normalize)
            Vector2 vector = new Vector2(sox - x, soy - y);
            vector.Normalize();
            //Move player to point along circle scaled to tongue length
            vector.X *= tongueLength;
            vector.Y *= tongueLength;

            //Move player
            x = sox - vector.X;
            y = soy - vector.Y;

            //Shift Velocity Along Circle
            //Find difference between where player was and where it is
            //Rotates Velocity
            float newAng = (float)Math.Atan2(y - prePos.Y, x - prePos.X);

            //Set velocities to new re-positioned velocities
            dx = (float)(Math.Cos(newAng) * preDel.Length());
            dy = (float)(Math.Sin(newAng) * preDel.Length()) + gravity.weight;
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