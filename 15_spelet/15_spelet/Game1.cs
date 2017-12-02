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
        // Variabel f�r textur p� brickorna (icke-animrad)
        //Texture2D boardTex;
        Video video;
        VideoPlayer videoPlayer;

        MouseState mouseState, oldmouseState;

        /// *********************

        // Dessa v�rden kan �ndras p� om s� �nskas..

        // �ndra detta v�rde f�r att ange hur m�nga g�nger shuffleTiles metoden ska k�ras.
        int amountofshuffles = 100;

        // Ange tv� v�rden som anger storleken p� brickorna
        // Konstanta v�rden, g�r ej att �ndra p� senare!
        const int tileWidth = 150;
        const int tileHeight = 150;

        // Dessa tal avg�r hur m�nga bitar det ska vara i spelet
        int nrofcols = 4;
        int nrofrows = 4;

        // S�tt storleken p� sk�rmen m.h.a dessa variabler
        const int height = 750;
        const int width = 750;

        /// *********************

        // �ndra ej p� v�rdena nedan!

        // Anger platsen f�r den blanka brickan
        int blankX, blankY;
        int posX, posY;
        // Variabel som h�ller antalet drag gjorda.
        int nrofMoves = 0;
        // Variabel som anv�nds f�r att kolla hur m�nga loops canWin metoden g�r
        int nrofloops = 0;
        // Variabel f�r att b�rja rita ut brickorna fr�n en viss punkt.
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
            // S�tt best�md storlek p� sk�rmen
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;
            graphics.ApplyChanges();

            IsMouseVisible = true;

            rand = new Random();

            // Detta �r en icke-animerad bild
            //boardTex = Content.Load<Texture2D>("cactaur");

            video = Content.Load<Video>("Wildlife");
            videoPlayer = new VideoPlayer();
            spriteFont = Content.Load<SpriteFont>("SpriteFont1");

            // Skapa en array f�r brickorna
            tiles = new Square[nrofrows, nrofcols];
            // s�tt en index p� en tile
            int index = 0;

            // Ange vad den blanka brickan har f�r index (X/Y)
            blankX = 3;
            blankY = 3;

            // Column = x
            // Row = y
            for (int row = 0; row < nrofcols; row++)
            {
                for (int column = 0; column < nrofrows; column++)
                {
                    // Definera posX/posY som avg�r hur stora en bricka ska vara.
                    posX = column * 150;
                    posY = row * 150;
                    // Skapa varje bricka som en ny "Square".
                    tiles[row, column] = new Square(posX + offset, posY + offset, new Rectangle
                        (column * (600 / nrofcols), row * (600 / nrofrows),
                        tileWidth, tileHeight), Color.White, index, video, videoPlayer, spriteFont);
                    index++;
                }
            }
            // Innan loadcontent �r klar, k�r en shuffle!
            shuffleTiles();
        }


        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }


        protected override void Update(GameTime gameTime)
        {
            // St�ng av spelet om Esc trycks ned. Bortkommenterad d� den inte ska finnas i slutprodukten!
            //if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    this.Exit();

            mouseState = Mouse.GetState();

            // Om hasWon �r falsk, spela upp spelets (textur)video.
            if (hasWon == false)
            {
                videoPlayer.IsLooped = true;
                videoPlayer.Play(video);
                // St�ng av ljudet p� videon
                videoPlayer.IsMuted = true;
            }

                for (int row = 0; row < nrofcols; row++)
                {
                    for (int column = 0; column < nrofrows; column++)
                    {
                        // Skapa en boundingbox f�r varje bricka
                        Rectangle tileBbox = new Rectangle((int)tiles[column, row].position.X, (int)tiles[column, row].position.Y, tileWidth, tileHeight);
                        // L�gg till kod som �ndrar plats p� tv� "tiles" vid klick
                        // Ha n�got som kontrollerar att en ruta f�rst �r tom.
                        // Sedan s� flytta en bricka till den tomma platsen
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

            // Om spelet �r vunnet och spelaren tryckt p� Enter,
            // starta om spelet, s�tt hasWon till falskt och starta om drag- r�knaren.
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && hasWon == true)
            {
                hasWon = false;
                shuffleTiles();
                nrofMoves = 0;
            }
            // Om spelet �r vunnet och Esc trycks ned, avsluta spelet.
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
            // Rita ut brickorna med hj�lp av drawTiles metoden
            drawTiles();
            // Skapa tv� texter d�r den ena visar antalet drag samt den andra skriver ut ett vinstmeddelande
            string moves = "Moves Done: " + nrofMoves;
            string winString = "You have done it! Press Enter to restart or Esc to exit";

            spriteBatch.DrawString(spriteFont, moves, new Vector2(20, 20), Color.White);
            // K�r om spelet �r vunnet. 
            if (hasWon == true)
            {
                // Skriv ut vinsttexten p� koordinaterna 20 & 40.
                spriteBatch.DrawString(spriteFont, winString, new Vector2(20, 40), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }


        // Skapa en metod f�r att rita ut brickorna.
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


        // Skapa en metod som anv�nds f�r att byta plats p� tom plats och en bricka.
        private bool swapPos(int startX, int startY, int startX_2, int startY_2)
        {
            // Returnera falskt om inga brickor r�r sig �t n�got h�ll.
            if (!tiles[startX, startY].CanSwap() || !tiles[blankX, blankY].CanSwap())
                return false;

            // Skapa tv� tempor�ra positioner, X/Y led
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

        // Skapa en metod som l�gger de blandade brickorna p� plats.
        private void swapPosShuffle(int startX, int startY, int startX_2, int startY_2)
        {
            // Skapa tv� tempor�ra positioner, X/Y led
            int tempX, tempY;

            // start(x/y) anger position f�r f�rsta brickan. start(x/y)_2 anger position f�r andra brickan

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
        // Garantera l�sbarheten!
        // Antalet loops anges som variabel tidigare.
        private void shuffleTiles()
        {
            for (int i = 0; i < amountofshuffles; )
            {
                // Slumpa v�rden som anger en brickas nya position.
                int newCol = rand.Next(0, nrofcols);
                int newRow = rand.Next(0, nrofrows);
                // Garantera l�sbarheten med hj�lp av absoluta v�rden.
                if ((Math.Abs(blankX - newCol) == 0 && Math.Abs(blankY - newRow) == 1 || Math.Abs(blankX - newCol) == 1 && Math.Abs(blankY - newRow) == 0))
                {
                    swapPosShuffle(blankX, blankY, newCol, newRow);
                    i++;
                }
            }
        }

        /* Skapa en metod som kontrollerar alla brickor.
         * Om en bricka har r�tt plats (kontrollera via index), s� l�gg till
         * ett "po�ng" i nrofcorrecttiles. Om alla 15 brickor ligger p� r�tt plats,
         * s�tt hasWon till sant. Om spelet �r vunnet, resetta alla variabler och
         * b�rja om fr�n b�rjan!
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
                            // S�tt hasWon till sann d� spelet �r vunnet
                            hasWon = true;
                            // Stanna den animerade bilden n�r spelet �r vunnet!
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
