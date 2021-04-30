using Microsoft.Xna.Framework.Graphics;
using New_Physics.Traits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Hammer_Knight.src;
using Frogs.src;

namespace New_Physics.Entities
{
    public class Block : Entity
    {
        public Block(float x, float y, float width, float height, float friction, float weight) : base("block", x, y)
        {
            base.width = width;
            base.height = height;

            List<Hitbox> hitboxes = new List<Hitbox>();
            hitboxes.Add(new Hitbox(this, 0, 0, this.width, this.height));
            hitboxes.Add(new Hitbox(this, -25, 50, this.width, 50));

            addTrait(new Gravity(this, weight));
            addTrait(new Friction(this, (float)friction));
            addTrait(new Rigidbody(this, hitboxes, false));
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

            for (int i = 0; i < ((Rigidbody)getTrait("rigidbody")).hitboxes.Count; i++)
            {
                Hitbox hitbox = ((Rigidbody)getTrait("rigidbody")).hitboxes[i];
                spriteBatch.Draw(texture, new Rectangle((int)(hitbox.x - Camera.X), (int)(hitbox.y - Camera.Y), (int)(hitbox.width), (int)(hitbox.height)), Color.Black);
            }

            //spriteBatch.Draw(texture, new Rectangle((int)(x), (int)(y), (int)width, (int)height), Color.Black);
            spriteBatch.End();
            texture.Dispose();
        }
    }
}
