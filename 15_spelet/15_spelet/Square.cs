using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _15_spelet_av_Yakup_Yildiz
{
    class Square
    {
        // Lägg till variabler som ska användas som en egenskap för en bricka
        //Texture2D texture;
        Video video;
        VideoPlayer videoPlayer;
        SpriteFont spriteFont;

        // Gör om positionen till en rektangel så att videofilen går att sätta som bakgrund.
        public Rectangle position;

        Color color;

        public int targetX, targetY;

        Rectangle rect;

        public int index;
        // Sätt en hastighet på bytet mellan brickorna
        int speed = 5;
        string tileNr;


        // Ange vad en "tile" ska ha för egenskaper
        public Square(int posX, int posY, Rectangle rect, Color color, int index, Video video, VideoPlayer videoPlayer, SpriteFont spriteFont)
        {
            // Ladda in allt nödvändigt för varje bricka
            //this.texture = texture;
            this.rect = rect;
            this.color = color;
            this.index = index;
            this.targetX = posX;
            this.targetY = posY;
            this.video = video;
            this.videoPlayer = videoPlayer;
            this.spriteFont = spriteFont;

            this.position = new Rectangle(150, 150, 150, 150);

        }

        public void Update()
        {
            // Räkna ut absoluta värden om speed är större än position(X/Y) - target(X/Y)
            if (Math.Abs(position.X - targetX) < speed)
                position.X = targetX;
            if (Math.Abs(position.Y - targetY) < speed)
                position.Y = targetY;
            // Kontrollera och rör en bricka åt vardera håll
            // om villkoren stämmer.

            tileNr = index.ToString();

            if (position.X < targetX)
            {
                position.X = position.X + speed;
            }
            if (position.X > targetX)
            {
                position.X = position.X - speed;
            }
            if (position.Y < targetY)
            {
                position.Y = position.Y + speed;
            }
            if (position.Y > targetY)
            {
                position.Y = position.Y - speed;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Sätt programmets textur till videospelarens videoklipp.
            Texture2D texture = videoPlayer.GetTexture();
            spriteBatch.Draw(texture, position, rect, color);
            spriteBatch.DrawString(spriteFont, tileNr, new Vector2(position.X+70,position.Y+70), Color.Orange);
        }

        // Skapa en metod som kontrollerar att det är möjligt att swappa plats.
        public bool CanSwap()
        {
            // returnera rätta värden
            return position.X == targetX && position.Y == targetY;
        }
    }
}
