using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frogs.src
{
    class Utils
    {
        public static Boolean pointRectCollision(float pointX, float pointY, float x, float y, float w, float h)
        {
            if (x < pointX && pointX < x + w && y < pointY && pointY < y + h)
            {
                return true;
            }

            return false;
        }

        public static Boolean rectCollision(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2)
        {
            return (
                y1 + h1 > y2 &&
                y1 < y2 + h2 &&
                x1 < x2 + w2 &&
                x1 + w1 > x2
                );
        }
        public static Boolean circlePointCollision(float px, float py, float cx, float cy, float r)
        {
            //Get distance between the point and the circle's center
            float distX = px - cx;
            float distY = py - cy;
            float distance = (float)Math.Sqrt((distX * distX) + (distY * distY));

            if (distance <= r) return true;
            return false;
        }

        public static Boolean rectCollision2(float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2)
        {
            if (w1 < w2)
            {
                if ((x1 > x2 && x1 < x2 + w2) || (x1 + w1 > x2 && x1 + w1 < x2 + w2))
                {
                    if (h1 < h2)
                    {
                        if ((y1 >= y2 && y1 <= y2 + h2) || (y1 + h1 >= y2 && y1 + h1 <= y2 + h2))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if ((y2 >= y1 && y2 <= y1 + h1) || (y2 + h2 >= y1 && y2 + h2 <= y1 + h1))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if ((x2 > x1 && x2 < x1 + w1) || (x2 + w2 > x1 && x2 + w2 < x1 + w1))
                {
                    if (h1 < h2)
                    {
                        if ((y1 >= y2 && y1 <= y2 + h2) || (y1 + h1 >= y2 && y1 + h1 <= y2 + h2))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if ((y2 >= y1 && y2 <= y1 + h1) || (y2 + h2 >= y1 && y2 + h2 <= y1 + h1))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static float getDistance(float x1, float y1, float x2, float y2)
        {
            return (float)Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        /// <summary>
        /// Loads Sprite Sheet Into List Of Rectangles To Be Used 
        /// As Parameters In spriteBatch.Draw() as the parameter sourceRectangle.  
        /// 
        /// Example:
        /// init(){
        ///     Rectangle[] sprites = new Rectangle[200];
        ///     sprites = spriteSheetLoader(32, 32, 320, 320);
        /// }
        /// draw(){
        ///     spriteBatch.Begin();
        ///     spriteBatch.Draw(texture, 
        ///         destinationRectangle: new Rectangle(this.x, this.y, this.width, this.height), 
        ///         sourceRectangle: sprites[0],   <<<======
        ///         color: Color.White);
        ///     spriteBatch.End();
        /// }
        /// </summary>
        /// <param name="spriteWidth"></param>
        /// <param name="spriteHeight"></param>
        /// <param name="spriteSheetWidth"></param>
        /// <param name="spriteSheetHeight"></param>
        /// <returns> Rectangle[] </returns>
        public static Rectangle[] spriteSheetLoader(int spriteWidth, int spriteHeight, int columns, int rows)
        {
            int spritesInSpriteSheet = columns * rows;

            Rectangle[] placeholder = new Rectangle[0];
            return spriteSheetLoader(placeholder, spriteWidth, spriteHeight, columns, rows, 0, spritesInSpriteSheet, false);
        }
        public static Rectangle[] spriteSheetLoader(int spriteWidth, int spriteHeight, int columns, int rows, int endingSprite)
        {
            Rectangle[] placeholder = new Rectangle[0];
            return spriteSheetLoader(placeholder, spriteWidth, spriteHeight, columns, rows, 0, endingSprite, false);
        }
        public static Rectangle[] spriteSheetLoader(int spriteWidth, int spriteHeight, int columns, int rows, int startingSprite, int endingSprite)
        {
            Rectangle[] placeholder = new Rectangle[0];
            return spriteSheetLoader(placeholder, spriteWidth, spriteHeight, columns, rows, startingSprite, endingSprite, false);
        }
        public static Rectangle[] spriteSheetLoader(int spriteWidth, int spriteHeight, int columns, int rows, int startingSprite, int endingSprite, Boolean inReverse)
        {
            Rectangle[] placeholder = new Rectangle[0];
            return spriteSheetLoader(placeholder, spriteWidth, spriteHeight, columns, rows, startingSprite, endingSprite, inReverse);
        }
        public static Rectangle[] spriteSheetLoader(Rectangle[] spriteSheet, int spriteWidth, int spriteHeight, int columns, int rows, int startingSprite, int endingSprite, Boolean inReverse)
        {
            //TODO: Implement Starting Sprite Functionality!!! <<<========  High Priority
            Rectangle[] toReturn = new Rectangle[spriteSheet.Count() + Math.Abs(endingSprite - startingSprite)];

            Boolean wantToBreak = false;
            int spriteCounter = 0;

            //Writes loaded spriteSheet to toReturn
            for (int i = 0; i < spriteSheet.Count(); i++)
            {
                toReturn[i] = spriteSheet[i];
            }

            //FOR GOING NORMAL DIRECTION
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (spriteCounter >= startingSprite)
                    {
                        toReturn[(spriteCounter - startingSprite) + spriteSheet.Count()] = new Rectangle(x * spriteWidth, y * spriteHeight, spriteWidth, spriteHeight);
                    }
                    if (spriteCounter + 2 > endingSprite)
                    {
                        wantToBreak = true;
                        break;
                    }

                    spriteCounter++;
                }
                if (wantToBreak)
                {
                    break;
                }
            }

            if (inReverse)
            {
                Rectangle[] reverseReturn = new Rectangle[toReturn.Count()];

                //Loads previous sprite sheet
                for (int i = 0; i < spriteSheet.Count(); i++)
                {
                    reverseReturn[i] = spriteSheet[i];
                }

                //Reverses new sprites
                for (int i = spriteSheet.Count(); i < toReturn.Count(); i++)
                {
                    reverseReturn[reverseReturn.Count() + spriteSheet.Count() - i - 1] = toReturn[i];
                }
                return reverseReturn;
            }

            return toReturn;
        }

        //Draws Lines
        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            return new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f)
        {
            var distance = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            DrawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f)
        {
            var origin = new Vector2(0f, 0.5f);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, origin, scale, SpriteEffects.None, 0);
        }
    }
}
