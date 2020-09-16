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
        public static string gamestate = "startScreen";

        private static StartScreen startScreen;

        public static void Init(GraphicsDeviceManager graphics)
        {
            Camera.SetDimensions(graphics, 1024, 576);
            //Camera.SetDimensions(graphics, GraphicsDevice);
            EntityHandler.Init();

            startScreen = new StartScreen();
        }

        public static void Update()
        {
            switch (gamestate)
            {
                case "startScreen":
                    startScreen.Update();
                    if (startScreen.Begin) gamestate = "initLevel";
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
            startScreen.LoadContent(Content);
            PlayerSprites.LoadContent(Content);
            GoalSprites.LoadContent(Content);
        }

        public static void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.CornflowerBlue);
            EntityHandler.Draw(spriteBatch, graphicsDevice);
            if (gamestate == "startScreen") startScreen.Draw(spriteBatch, graphicsDevice);
        }
    }
}
