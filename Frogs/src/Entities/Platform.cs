using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using New_Physics.Traits;
using Hammer_Knight.src;
using Frogs.src;
using Microsoft.Xna.Framework.Content;

namespace New_Physics.Entities
{
    public static class PlatformSprites
    {
        public static Texture2D ss;
        public static Rectangle[] sprites;

        public static void LoadContent(ContentManager Content)
        {
            ss = Content.Load<Texture2D>("Platform");

            sprites = Utils.spriteSheetLoader(5, 5, 9, 1);
        }
    }
    public class Platform : Entity
    {
        public Boolean sRight = false;
        public Boolean sLeft = false;
        public Boolean sTop = false;
        public Boolean sBottom = false;

        public Platform(float x, float y, float width, float height,
            Boolean sRight = true, Boolean sLeft = true, Boolean sTop = true, Boolean sBottom = true) : base("platform", x, y)
        {
            Init(x, y, width, height, sRight, sLeft, sTop, sBottom);
        }

        private void Init(float x, float y, float width, float height,
            Boolean sRight, Boolean sLeft, Boolean sTop, Boolean sBottom)
        {
            base.width = width;
            base.height = height;

            addTrait(new Rigidbody(this, true));

            this.sRight = sRight;
            this.sLeft = sLeft;
            this.sTop = sTop;
            this.sBottom = sBottom;
        }


        public override void Update()
        {

            traitUpdate();
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            //Draw Hitbox
            Texture2D texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.White });
            spriteBatch.Begin();
            spriteBatch.Draw(texture, new Rectangle((int)(x - Camera.X), (int)(y - Camera.Y), (int)width, (int)height), Color.Black);
            spriteBatch.End();
            texture.Dispose();

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            int w = (int)(50 * Camera.gameScale);
            int h = (int)(50 * Camera.gameScale);

            for (int y = 0; y < height; y += h)
            {
                for (int x = 0; x < width; x += w)
                {
                    //If left of platform
                    if (x == 0)
                    {
                        //If top left of platform
                        if (y == 0)
                        {
                            spriteBatch.Draw(PlatformSprites.ss,
                                new Rectangle((int)(this.x - Camera.X), (int)(this.y + y - Camera.Y), w, h),
                                sourceRectangle: PlatformSprites.sprites[0],
                                color: Color.White);
                        }
                        //If bottom left of platform
                        else if (y >= height - h)
                        {
                            spriteBatch.Draw(PlatformSprites.ss,
                                new Rectangle((int)(this.x - Camera.X), (int)(this.y + height - h - Camera.Y), w, h),
                                sourceRectangle: PlatformSprites.sprites[3],
                                color: Color.White);
                        }
                        //If just left of platform
                        else
                        {
                            spriteBatch.Draw(PlatformSprites.ss,
                                new Rectangle((int)(this.x - Camera.X), (int)(this.y + y - Camera.Y), w, h),
                                sourceRectangle: PlatformSprites.sprites[6],
                                color: Color.White);
                        }
                    }
                    //If right of platform
                    else if (x >= width - w)
                    {
                        //If top right of platofrm
                        if (y == 0)
                        {
                            spriteBatch.Draw(PlatformSprites.ss,
                                new Rectangle((int)(this.x + width - w - Camera.X), (int)(this.y + y - Camera.Y), w, h),
                                sourceRectangle: PlatformSprites.sprites[2],
                                color: Color.White);
                        }
                        //If bottom right of platform
                        else if (y == height - h)
                        {
                            spriteBatch.Draw(PlatformSprites.ss,
                                new Rectangle((int)(this.x + width - w - Camera.X), (int)(this.y + height - h - Camera.Y), w, h),
                                sourceRectangle: PlatformSprites.sprites[3],
                                color: Color.White);
                        }
                        //If just right of platform
                        else
                        {
                            spriteBatch.Draw(PlatformSprites.ss,
                                new Rectangle((int)(this.x + width - w - Camera.X), (int)(this.y + y - Camera.Y), w, h),
                                sourceRectangle: PlatformSprites.sprites[8],
                                color: Color.White);
                        }
                    }
                    //If middle of platform
                    else
                    {
                        //if top middle of platform
                        if (y == 0)
                        {
                            spriteBatch.Draw(PlatformSprites.ss,
                                new Rectangle((int)(this.x + x - Camera.X), (int)(this.y + y - Camera.Y), w, h),
                                sourceRectangle: PlatformSprites.sprites[1],
                                color: Color.White);
                        }
                        //If bottom middle of platform
                        else if (y >= height - 50)
                        {
                            spriteBatch.Draw(PlatformSprites.ss,
                                new Rectangle((int)(this.x + x - Camera.X), (int)(this.y + height - h - Camera.Y), w, h),
                                sourceRectangle: PlatformSprites.sprites[4],
                                color: Color.White);
                        }
                        //If just middle of platform
                        else
                        {
                            spriteBatch.Draw(PlatformSprites.ss,
                                new Rectangle((int)(this.x + x - Camera.X), (int)(this.y + y - Camera.Y), w, h),
                                sourceRectangle: PlatformSprites.sprites[7],
                                color: Color.White);
                        }
                    }
                }
            }

            spriteBatch.End();
        }
    }
}
