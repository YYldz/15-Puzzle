using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace _15_spelet_av_Yakup_Yildiz
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        // Variabel för textur på brickorna (icke-animrad)
        //Texture2D boardTex;
        Video video;
        VideoPlayer videoPlayer;

        MouseState mouseState, oldmouseState;

        /// *********************

        // Dessa värden kan ändras på om så önskas..

        // Ändra detta värde för att ange hur många gånger shuffleTiles metoden ska köras.
        int amountofshuffles = 100;

        // Ange två värden som anger storleken på brickorna
        // Konstanta värden, går ej att ändra på senare!
        const int tileWidth = 150;
        const int tileHeight = 150;

        // Dessa tal avgör hur många bitar det ska vara i spelet
        int nrofcols = 4;
        int nrofrows = 4;

        // Sätt storleken på skärmen m.h.a dessa variabler
        const int height = 750;
        const int width = 750;

        /// *********************

        // Ändra ej på värdena nedan!

        // Anger platsen för den blanka brickan
        int blankX, blankY;
        int posX, posY;
        // Variabel som håller antalet drag gjorda.
        int nrofMoves = 0;
        // Variabel som används för att kolla hur många loops canWin metoden gör
        int nrofloops = 0;
        // Variabel för att börja rita ut brickorna från en viss punkt.
        int offset = 75;

        Random rand;

        Square[,] tiles;

        bool hasWon;
        int nrofcorrecttiles;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            base.Initialize();
        }


        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Sätt bestämd storlek på skärmen
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;
            graphics.ApplyChanges();

            IsMouseVisible = true;

            rand = new Random();

            // Detta är en icke-animerad bild
            //boardTex = Content.Load<Texture2D>("cactaur");

            video = Content.Load<Video>("Wildlife");
            videoPlayer = new VideoPlayer();
            spriteFont = Content.Load<SpriteFont>("SpriteFont1");

            // Skapa en array för brickorna
            tiles = new Square[nrofrows, nrofcols];
            // sätt en index på en tile
            int index = 0;

            // Ange vad den blanka brickan har för index (X/Y)
            blankX = 3;
            blankY = 3;

            // Column = x
            // Row = y
            for (int row = 0; row < nrofcols; row++)
            {
                for (int column = 0; column < nrofrows; column++)
                {
                    // Definera posX/posY som avgör hur stora en bricka ska vara.
                    posX = column * 150;
                    posY = row * 150;
                    // Skapa varje bricka som en ny "Square".
                    tiles[row, column] = new Square(posX + offset, posY + offset, new Rectangle
                        (column * (600 / nrofcols), row * (600 / nrofrows),
                        tileWidth, tileHeight), Color.White, index, video, videoPlayer, spriteFont);
                    index++;
                }
            }
            // Innan loadcontent är klar, kör en shuffle!
            shuffleTiles();
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            // Stäng av spelet om Esc trycks ned. Bortkommenterad då den inte ska finnas i slutprodukten!
            //if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    this.Exit();

            mouseState = Mouse.GetState();

            // Om hasWon är falsk, spela upp spelets (textur)video.
            if (hasWon == false)
            {
                videoPlayer.IsLooped = true;
                videoPlayer.Play(video);
                // Stäng av ljudet på videon
                videoPlayer.IsMuted = true;
            }

                for (int row = 0; row < nrofcols; row++)
                {
                    for (int column = 0; column < nrofrows; column++)
                    {
                        // Skapa en boundingbox för varje bricka
                        Rectangle tileBbox = new Rectangle((int)tiles[column, row].position.X, (int)tiles[column, row].position.Y, tileWidth, tileHeight);
                        // Lägg till kod som ändrar plats på två "tiles" vid klick
                        // Ha något som kontrollerar att en ruta först är tom.
                        // Sedan så flytta en bricka till den tomma platsen
                        if (tileBbox.Contains(mouseState.X, mouseState.Y) && (mouseState.LeftButton == ButtonState.Pressed &&
                            oldmouseState.LeftButton == ButtonState.Released) &&
                            (Math.Abs(blankX - column) == 0 && Math.Abs(blankY - row) == 1 || Math.Abs(blankX - column) == 1 && Math.Abs(blankY - row) == 0) && hasWon == false)
                        {
                            if (swapPos(blankX, blankY, column, row))
                            {
                                nrofMoves++;
                                checkIfWon();
                            }
                        }

                    }
                }

            // Om spelet är vunnet och spelaren tryckt på Enter,
            // starta om spelet, sätt hasWon till falskt och starta om drag- räknaren.
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && hasWon == true)
            {
                hasWon = false;
                shuffleTiles();
                nrofMoves = 0;
            }
            // Om spelet är vunnet och Esc trycks ned, avsluta spelet.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) && hasWon == true)
            {
                this.Exit();
            }

            oldmouseState = mouseState;

            foreach (Square t in tiles)
            {
                t.Update();
            }
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            // Rita ut brickorna med hjälp av drawTiles metoden
            drawTiles();
            // Skapa två texter där den ena visar antalet drag samt den andra skriver ut ett vinstmeddelande
            string moves = "Moves Done: " + nrofMoves;
            string winString = "You have done it! Press Enter to restart or Esc to exit";

            spriteBatch.DrawString(spriteFont, moves, new Vector2(20, 20), Color.White);
            // Kör om spelet är vunnet. 
            if (hasWon == true)
            {
                // Skriv ut vinsttexten på koordinaterna 20 & 40.
                spriteBatch.DrawString(spriteFont, winString, new Vector2(20, 40), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }


        // Skapa en metod för att rita ut brickorna.
        private void drawTiles()
        {
            for (int row = 0; row < nrofcols; row++)
            {
                for (int column = 0; column < nrofrows; column++)
                {
                    // Rita inte ut bricka med index 15!
                    if (tiles[row, column].index == 15)
                        continue;

                    tiles[row, column].Draw(spriteBatch);
                }
            }
        }


        // Skapa en metod som används för att byta plats på tom plats och en bricka.
        private bool swapPos(int startX, int startY, int startX_2, int startY_2)
        {
            // Returnera falskt om inga brickor rör sig åt något håll.
            if (!tiles[startX, startY].CanSwap() || !tiles[blankX, blankY].CanSwap())
                return false;

            // Skapa två temporära positioner, X/Y led
            int tempX, tempY;

            tempX = (int)tiles[startX, startY].position.X;
            tempY = (int)tiles[startX, startY].position.Y;

            tiles[startX, startY].targetX = (int)tiles[startX_2, startY_2].targetX;
            tiles[startX, startY].targetY = (int)tiles[startX_2, startY_2].targetY;

            tiles[startX_2, startY_2].targetX = tempX;
            tiles[startX_2, startY_2].targetY = tempY;

            Square tempTiles = tiles[startX, startY];
            tiles[startX, startY] = tiles[startX_2, startY_2];
            tiles[startX_2, startY_2] = tempTiles;

            blankX = startX_2;
            blankY = startY_2;
            return true;
        }

        // Skapa en metod som lägger de blandade brickorna på plats.
        private void swapPosShuffle(int startX, int startY, int startX_2, int startY_2)
        {
            // Skapa två temporära positioner, X/Y led
            int tempX, tempY;

            // start(x/y) anger position för första brickan. start(x/y)_2 anger position för andra brickan

            tempX = (int)tiles[startX, startY].targetX;
            tempY = (int)tiles[startX, startY].targetY;

            tiles[startX, startY].targetX = (int)tiles[startX_2, startY_2].targetX;
            tiles[startX, startY].targetY = (int)tiles[startX_2, startY_2].targetY;

            tiles[startX_2, startY_2].targetX = tempX;
            tiles[startX_2, startY_2].targetY = tempY;

            tempX = (int)tiles[startX, startY].position.X;
            tempY = (int)tiles[startX, startY].position.Y;

            tiles[startX, startY].position.X = tiles[startX_2, startY_2].position.X;
            tiles[startX, startY].position.Y = tiles[startX_2, startY_2].position.Y;

            tiles[startX_2, startY_2].position.X = tempX;
            tiles[startX_2, startY_2].position.Y = tempY;

            Square tempTiles = tiles[startX, startY];
            tiles[startX, startY] = tiles[startX_2, startY_2];
            tiles[startX_2, startY_2] = tempTiles;

            blankX = startX_2;
            blankY = startY_2;
        }

        // Ha en metod med en for-loop som blandar alla tiles
        // Garantera lösbarheten!
        // Antalet loops anges som variabel tidigare.
        private void shuffleTiles()
        {
            for (int i = 0; i < amountofshuffles; )
            {
                // Slumpa värden som anger en brickas nya position.
                int newCol = rand.Next(0, nrofcols);
                int newRow = rand.Next(0, nrofrows);
                // Garantera lösbarheten med hjälp av absoluta värden.
                if ((Math.Abs(blankX - newCol) == 0 && Math.Abs(blankY - newRow) == 1 || Math.Abs(blankX - newCol) == 1 && Math.Abs(blankY - newRow) == 0))
                {
                    swapPosShuffle(blankX, blankY, newCol, newRow);
                    i++;
                }
            }
        }

        /* Skapa en metod som kontrollerar alla brickor.
         * Om en bricka har rätt plats (kontrollera via index), så lägg till
         * ett "poäng" i nrofcorrecttiles. Om alla 15 brickor ligger på rätt plats,
         * sätt hasWon till sant. Om spelet är vunnet, resetta alla variabler och
         * börja om från början!
         */
        private void checkIfWon()
        {
            for (int row = 0; row < nrofcols; row++)
            {
                for (int column = 0; column < nrofrows; column++)
                {
                    if (tiles[row, column].index == nrofloops)
                    {
                        nrofcorrecttiles++;
                        if (nrofcorrecttiles == 15)
                        {
                            // Sätt hasWon till sann då spelet är vunnet
                            hasWon = true;
                            // Stanna den animerade bilden när spelet är vunnet!
                            videoPlayer.Stop();
                        }
                    }
                    nrofloops++;
                }
            }
            nrofcorrecttiles = 0;
            nrofloops = 0;
        }
    }
}
