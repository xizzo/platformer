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

namespace platformer
{
    class Player : Game
    {
        private Texture2D texture;
        public int xPos = 0;
        public int yPos = 0;

        private bool movingLeft = false;
        private bool movingRight = false;
        private bool movingUp = false;
        private bool movingDown = false;

        private bool jumpStart = false;
        private int jumpStartY = 0;
        private bool jumpHeightReached = false;
        private int jumpHeight = 300;
        private int jumpSpeed = 1;
        private bool isFalling = false;
        private bool spaceKeyReleased = true;
        private int oldXPos = 0;
        private int oldYPos = 0;
        public bool posMoved = false;

        public Game mainGame;

        private int normSpeed = 6;

        private KeyboardState kState;

        private Rectangle playerBounds;

        private List<Platform> platforms;
        private bool isRunningOnPlat = false;   

        public Player(Game game, List<Platform> platforms)
        {
            mainGame = game;
            this.platforms = platforms;
        }

        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            texture = theContentManager.Load<Texture2D>("char");
            yPos = mainGame.GraphicsDevice.Viewport.Height - texture.Height;
            playerBounds = new Rectangle(xPos - texture.Width/2, yPos - texture.Height/2, texture.Width, texture.Height + 5);
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(texture, new Rectangle(xPos, yPos, texture.Width, texture.Height), Color.White);
        }

        public void Update(GameTime gameTime)
        {
            playerBounds = new Rectangle(xPos , yPos  , texture.Width, texture.Height +5);
            ResetDirections();
            kState = Keyboard.GetState();

            if (kState.IsKeyDown(Keys.Up) || kState.IsKeyDown(Keys.Z))
                movingUp = true;   
            if (kState.IsKeyDown(Keys.Left) || kState.IsKeyDown(Keys.Q) || kState.IsKeyDown(Keys.A))
                movingLeft = true;
            if (kState.IsKeyDown(Keys.Down) || kState.IsKeyDown(Keys.S))
                movingDown = true;
            if (kState.IsKeyDown(Keys.Right) || kState.IsKeyDown(Keys.D))
                movingRight = true;
            if (kState.IsKeyDown(Keys.Space))
                if (!jumpStart && !isFalling && spaceKeyReleased)
                {
                    spaceKeyReleased = false;
                    jumpStart = true;
                    jumpStartY = yPos;
                    jumpHeightReached = false;
                    jumpSpeed = 23;
                }
            if (kState.IsKeyUp(Keys.Space))
            {
                if (!spaceKeyReleased)
                    spaceKeyReleased = true;
            }

            //if (movingDown)
                //yPos += normSpeed;
            if(movingLeft & xPos >= 0)
                xPos -= normSpeed;
            if (movingRight & xPos <= mainGame.GraphicsDevice.Viewport.Width - texture.Width)
                xPos += normSpeed;
            //if (movingUp)
                //yPos -= normSpeed;

            if (isRunningOnPlat && !jumpStart)
            {
                bool fellOf = true;
                foreach (Platform plat in platforms)
                {
                    if (playerBounds.Intersects(plat.platBounds))
                    {
                        fellOf = false;
                    }
                }
                if (fellOf)
                {
                    jumpStart = true;
                    jumpSpeed = 14;
                    jumpHeightReached = true;
                    isRunningOnPlat = false;
                }
            }


            if (jumpStart)
            {
                if (!jumpHeightReached)
                {
                    foreach(Platform plat in platforms)
                    {
                        if (((playerBounds.X + playerBounds.Width / 2) > plat.xPos && (playerBounds.X + playerBounds.Width / 2) < plat.xPos + plat.platBounds.Width) && (playerBounds.Y <= plat.platBounds.Y + plat.platBounds.Height + 12 && playerBounds.Y >= plat.platBounds.Y - plat.platBounds.Height - 12))
                        {
                            jumpHeightReached = true;
                        }
                    }
                    yPos -= jumpSpeed;
                    jumpSpeed -= 1;
                    if (jumpSpeed == 3)
                    {
                        jumpSpeed = 3;
                        jumpHeightReached = true;
                    }
                    if (yPos <= (jumpStartY - jumpHeight))
                    {
                        jumpHeightReached = true;
                    }
                }
                else if (jumpHeightReached)
                {
                    foreach (Platform plat in platforms)
                    {
                        if (((playerBounds.X + playerBounds.Width / 2) > plat.xPos && (playerBounds.X + playerBounds.Width / 2) < plat.xPos + plat.platBounds.Width) && (playerBounds.Y < plat.platBounds.Y - plat.platBounds.Height*2) && playerBounds.Y > plat.platBounds.Y - playerBounds.Height)
                        {
                            jumpHeightReached = true;
                            jumpStart = false;
                            yPos = plat.platBounds.Y-playerBounds.Height - 13;
                            isRunningOnPlat = true;
                        }
                    }
                    yPos += jumpSpeed;
                    jumpSpeed += 1;
                    if (yPos >= mainGame.GraphicsDevice.Viewport.Height - texture.Height)
                    {
                        yPos = mainGame.GraphicsDevice.Viewport.Height - texture.Height;
                        jumpStart = false;
                        isRunningOnPlat = false;
                    }
                }
            }
            if (xPos != oldXPos || yPos != oldYPos)
            {
                posMoved = true;
            }
            else
            {
                posMoved = false;
            }
            oldXPos = xPos;
            oldYPos = yPos;

        }


        private void ResetDirections()
        {
            movingUp = false;
            movingRight = false;
            movingLeft = false;
            movingDown = false;
        }

    }
}
