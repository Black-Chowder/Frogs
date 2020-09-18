using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Frogs.src
{

    public class HelpScreen
    {
        public Boolean Begin = false;
        private Rectangle buttonPos;
        private Boolean initClick = false;

        private Texture2D background;
        private MouseState mouse;
        private int buttonState = 0;
        //0 = neutral
        //1 = hover
        //2 = clicked

        private Rectangle[] button;
        private Texture2D buttonSpriteSheet;

        public HelpScreen()
        {
            buttonPos = new Rectangle(Convert.ToInt32(Camera.Width - (30+10) * 10 * Camera.gameScale),
                Convert.ToInt32(Camera.Height / 2 + 350 * Camera.gameScale),
                Convert.ToInt32(30 * 10 * Camera.gameScale),
                Convert.ToInt32(15 * 10 * Camera.gameScale));
        }

        public void Update()
        {
            mouse = Mouse.GetState();

            //Button Handling
            buttonState = 0;
            if (Utils.pointRectCollision(mouse.X, mouse.Y, buttonPos.X, buttonPos.Y, buttonPos.Width, buttonPos.Height))
            {
                buttonState = 1;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    initClick = true;
                    buttonState = 2;
                }
                else if (initClick)
                {
                    Begin = true;
                }
            }
            else
            {
                initClick = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            //Draw Background
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(background,
                new Rectangle(0, 0, Camera.Width, Camera.Height),
                Color.White);

            spriteBatch.End();


            //Draw Button
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(buttonSpriteSheet,
                buttonPos, sourceRectangle: button[buttonState], color: Color.White);

            spriteBatch.End();
        }

        public void LoadContent(ContentManager Content)
        {
            background = Content.Load<Texture2D>(@"HowToPlay");
            buttonSpriteSheet = Content.Load<Texture2D>(@"SmallButton");
            button = Utils.spriteSheetLoader(30, 15, 3, 1);
        }
    }


    public class StartScreen
    {
        private Texture2D background;
        private Texture2D buttonSpriteSheet;
        private Rectangle[] button;

        private int buttonState = 0;
        //0 = neutral
        //1 = hover
        //2 = clicked

        private Rectangle buttonPos;

        private MouseState mouse;
        private Boolean initClick = false;

        public Boolean Begin = false;

        public StartScreen()
        {
            buttonPos = new Rectangle(Convert.ToInt32(Camera.Width / 2 - 70*10 / 2 * Camera.gameScale), 
                Convert.ToInt32(Camera.Height / 2 + 175 * Camera.gameScale), 
                Convert.ToInt32(70 * 10* Camera.gameScale), 
                Convert.ToInt32(31 * 10 * Camera.gameScale));
        }

        public void Update()
        {
            mouse = Mouse.GetState();

            //Button Handling
            buttonState = 0;
            if (Utils.pointRectCollision(mouse.X, mouse.Y, buttonPos.X, buttonPos.Y, buttonPos.Width, buttonPos.Height))
            {
                buttonState = 1;
                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    initClick = true;
                    buttonState = 2;
                }
                else if (initClick)
                {
                    Begin = true;
                }
            }
            else
            {
                initClick = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            //Draw Background
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(background,
                new Rectangle(0, 0, Camera.Width, Camera.Height),
                Color.White);

            spriteBatch.End();


            //Draw Button
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(buttonSpriteSheet,
                buttonPos, sourceRectangle: button[buttonState], color: Color.White);

            spriteBatch.End();
        }

        public void LoadContent(ContentManager Content)
        {
            background = Content.Load<Texture2D>("TitleScreen");
            buttonSpriteSheet = Content.Load<Texture2D>("StartButton");
            button = Utils.spriteSheetLoader(70, 31, 3, 1);
        }
    }
}
