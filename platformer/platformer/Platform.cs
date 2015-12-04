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
    class Platform
    {
        private Game mainGame;
        private Texture2D texture;

        public int xPos;
        public int yPos;
        private int lenght;

        public Rectangle platBounds;

        public Platform(Game game, int xPos, int yPos, int lenght)
        {
            this.mainGame = game;
            this.xPos = xPos;
            this.yPos = yPos;
            this.lenght = lenght;
        }
        
        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            texture = theContentManager.Load<Texture2D>("platform");
            this.platBounds = new Rectangle(xPos, yPos , lenght, texture.Height);
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(texture, new Rectangle(xPos, yPos, lenght, texture.Height), Color.White);
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
