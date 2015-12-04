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
    class NewClient
    {
        private Texture2D texture;
        public int xPos;
        public int yPos;
        public int ClientID;

        public NewClient(int ClientID, int xPos, int yPos)
        {
            this.ClientID = ClientID;
            this.xPos = xPos;
            this.yPos = yPos;
        }

        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            texture = theContentManager.Load<Texture2D>("char");
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            theSpriteBatch.Draw(texture, new Rectangle(xPos, yPos, texture.Width, texture.Height), Color.White);
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
