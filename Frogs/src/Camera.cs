using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using New_Physics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Frogs.src
{
    public static class Camera
    {
        private static Random rand = new Random();

        public static float X = 0;
        public static float Y = 0;

        public static int Width = 100;
        public static int Height = 100;

        private static Vector2 nScreenSize = new Vector2(1920, 1080);

        public static float gameScale = 1f;

        //Requested position.  The destination the camera wants to go to
        public static float reqX = 0;
        public static float reqY = 0;

        private static float eX = 0;
        private static float eY = 0;

        public static float speed = 15f;

        //Camera Shake Variables
        private static float intensity = 0;
        private static float durration = 0;
        private static float bufferZone = 2;

        public static void Update()
        {
            camShakeHandler();

            //Move camera to req location based on speed
            if (speed != 0 && durration > 0)
            {
                X += (reqX - X) / speed;
                Y += (reqY - Y) / speed;
            }
            else if (durration > 0)
            {
                X = reqX;
                Y = reqY;
            }
            else
            {
                X += (reqX - X) / speed;
                Y += (reqY - Y) / speed;
            }

            if (Utils.getDistance(X, Y, reqX, reqY) <= bufferZone)
            {
                SudoGoTo(reqX, reqY);
            }


            //Console.WriteLine("Camera Position = (" + X + "," + Y + ")");
        }

        private static void updateGameScale()
        {
            gameScale = Width / 1920f;
         
        }

        public static void SetDimensions(GraphicsDeviceManager graphics, GraphicsDevice GraphicsDevice)
        {
            SetDimensions(graphics, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height, true);
        }

        public static void SetDimensions(GraphicsDeviceManager graphics, int Width, int Height)
        {
            SetDimensions(graphics, Width, Height, false);
        }

        public static void SetDimensions(GraphicsDeviceManager graphics, int Width, int Height, Boolean isFullScreen)
        {
            graphics.PreferredBackBufferWidth = Width;
            graphics.PreferredBackBufferHeight = Height;
            graphics.IsFullScreen = isFullScreen;
            graphics.ApplyChanges();

            Camera.Width = Width;
            Camera.Height = Height;

            updateGameScale();
        }

        public static void SudoGoTo(float x, float y)
        {
            X = x;
            Y = y;
            reqX = X;
            reqY = y;
        }

        public static void GoTo(float x, float y)
        {
            reqX = x;
            reqY = y;
        }

        public static void Shake(float intensity, float durration)
        {
            Camera.intensity = intensity;
            Camera.durration = durration;
        }

        private static void camShakeHandler()
        {
            //Move camera to random location between zero and intensity if in buffer zone
            if (durration <= 0) return;
            durration--;

            X += ((float)rand.NextDouble() * intensity) / speed;
            Y += ((float)rand.NextDouble() * intensity) / speed;

            //TODO: Make camera shake speed faster than normal camera speed
        }
    }
}
