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

namespace New_Physics.Entities
{
    public class Platform : Entity
    {
        public Platform(float x, float y, float width, float height) : base("platform", x, y)
        {
            base.width = width;
            base.height = height;

            //addTrait(new Friction(this, 1000));
            addTrait(new Rigidbody(this, true));
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
        }
    }
}
