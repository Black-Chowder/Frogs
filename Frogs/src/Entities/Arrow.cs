using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using New_Physics.Entities;

namespace Frogs.src.Entities
{
    public static class ArrowSprites
    {
        public static Texture2D ss;
        public static Rectangle[] sprites;

        public static void LoadContent(ContentManager Content)
        {
            ss = Content.Load<Texture2D>("Arrow");

            sprites = Utils.spriteSheetLoader(8, 8, 2, 1);
        }
    }
    public class Arrow : Entity
    {
        int type;

        public Arrow(float x, float y, int type) : base("arrow", x, y)
        {
            this.type = type;
        }

        public override void Update()
        {
            
        }

        [Obsolete]
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            int w = (int)(50 * Camera.gameScale);
            int h = (int)(50 * Camera.gameScale);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            switch (type)
            {
                case 1://Right
                    spriteBatch.Draw(ArrowSprites.ss,
                        new Rectangle((int)(x - Camera.X), (int)(y - Camera.Y), w, h),
                        sourceRectangle: ArrowSprites.sprites[0],
                        color: Color.White);
                    break;
                case 2://Bottom Left
                    spriteBatch.Draw(ArrowSprites.ss,
                        destinationRectangle: new Rectangle((int)(x - Camera.X), (int)(y - Camera.Y), w, h),
                        sourceRectangle: ArrowSprites.sprites[1],
                        effects: SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically,
                        color: Color.White);
                    break;

                case 3://Up
                    spriteBatch.Draw(ArrowSprites.ss,
                        destinationRectangle: new Rectangle((int)(x - Camera.X), (int)(y - h - Camera.Y), w, h),
                        sourceRectangle: ArrowSprites.sprites[0],
                        rotation: -(float)(Math.PI/2),
                        color: Color.White);
                    break;
            }

            spriteBatch.End();
        }
    }
}
