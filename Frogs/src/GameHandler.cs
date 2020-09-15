using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using New_Physics.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frogs.src.Entities;
using Microsoft.Xna.Framework.Content;

namespace Frogs.src
{
    public static class GameHandler
    {
        public static string gamestate = "initLevel";

        public static void Init(GraphicsDeviceManager graphics)
        {
            Camera.SetDimensions(graphics, 1024, 576);
            //Camera.SetDimensions(graphics, GraphicsDevice);
            EntityHandler.Init();
        }

        public static void Update()
        {
            switch (gamestate)
            {
                case "startScreen":

                    break;
                case "initLevel":
                    Level_Loader.LoadLevel();

                    gamestate = "level";
                    break;
                case "level":
                    EntityHandler.Update();
                    Camera.Update();
                    break;
            }
            //Level_Loader.LoadLevel();
        }

        public static void LoadContent(ContentManager Content)
        {
            PlayerSprites.LoadContent(Content);
            GoalSprites.LoadContent(Content);
        }

        public static void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.CornflowerBlue);
            EntityHandler.Draw(spriteBatch, graphicsDevice);
        }
    }

    public static class startScreenSprites
    {


    }
}
