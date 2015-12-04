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
    class Level1
    {
        private Game mainGame;

        public List<Platform> platforms = new List<Platform>();

        public Platform plat1;
        public Platform plat2;

        public Level1(Game game)
        {
            this.mainGame = game;
            plat1 = new Platform(game, 0, mainGame.GraphicsDevice.Viewport.Height - mainGame.GraphicsDevice.Viewport.Height/6, mainGame.GraphicsDevice.Viewport.Width/4);
            plat2 = new Platform(game, 400, mainGame.GraphicsDevice.Viewport.Height - mainGame.GraphicsDevice.Viewport.Height / 6-150, 300);
            platforms.Add(plat1);
            platforms.Add(plat2);
        }

        public void LoadContent(ContentManager theContentManager, string theAssetName)
        {
            plat1.LoadContent(theContentManager, "Plat1");
            plat2.LoadContent(theContentManager, "Plat2");
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            foreach (Platform plat in platforms)
            {
                plat.Draw(theSpriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {

        }

    }
}
