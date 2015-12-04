using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Text;

namespace platformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        Texture2D crossHair;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player;
        Level1 level;
        int xPos = 0;
        int yPos = 0;
        KeyboardState oldKeys;

        //networking
        public Boolean isConnected = false;
        Thread receiveDataThread;
        TcpClient client;
        IPEndPoint serverAdress = new IPEndPoint(IPAddress.Parse("84.198.108.23"), 7777);
        List<NewClient> connectedClients = new List<NewClient>();
        public int NetworkUpdateInterval = 3;
        public int currNetworkInterval = 0;

        //ui
        SpriteFont gameFont;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
           // this.graphics.PreferredBackBufferWidth = 1920;
            //this.graphics.PreferredBackBufferHeight = 1080;
            //this.graphics.IsFullScreen = true;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            level = new Level1(this);
            player = new Player(this, level.platforms);
            player.mainGame = this;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player.LoadContent(this.Content, "Player");
            crossHair = Content.Load<Texture2D>("crosshair");
            level.LoadContent(this.Content, "Level");

            gameFont = Content.Load<SpriteFont>("gameFont");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            player.Update(gameTime);
            // TODO: Add your update logic here

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.Escape))
                Exit();
            if (ks.IsKeyDown(Keys.C) && oldKeys.IsKeyUp(Keys.C))
                Connect();
            if (ks.IsKeyDown(Keys.M) && oldKeys.IsKeyUp(Keys.M))
                SendData("000%Drwolf");

            if (player.posMoved)
            {
                currNetworkInterval++;
                if (currNetworkInterval == 3)
                {
                    SendPosition(player.xPos, player.yPos);
                    currNetworkInterval = 0;
                }
                
            }

            oldKeys = Keyboard.GetState();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            player.Draw(this.spriteBatch);
            level.Draw(this.spriteBatch);
            spriteBatch.Draw(crossHair, new Rectangle(ms.X, ms.Y, crossHair.Width, crossHair.Height), Color.White);

            foreach (NewClient client in connectedClients)
            {
                client.Draw(this.spriteBatch);
            }

            spriteBatch.DrawString(gameFont, "Connected: " + isConnected.ToString(), new Vector2(20, 20), Color.White);
            spriteBatch.DrawString(gameFont, "Moved : " + player.posMoved.ToString(), new Vector2(20, 50), Color.White);
            base.Draw(gameTime);
            spriteBatch.End();
        }

        private void Connect()
        {
            if (!isConnected)
            {
                try
                {
                    client = new TcpClient();
                    client.Connect(serverAdress);
                    isConnected = true;
                    receiveDataThread = new Thread(receiveData);
                    receiveDataThread.Start();
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                client.Close();
                isConnected = false;
            }
        }

        private void receiveData()
        {
            byte[] bytesFrom = new byte[100025];
            string dataFromClient = null;

            while (true)
            {
                try
                {
                    NetworkStream networkStream = client.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)client.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                    ReadData(dataFromClient);
                }
                catch (Exception ex)
                {
                    client.Close();
                    if (isConnected)
                    {
                        receiveDataThread.Abort();
                    }
                }
            }
        }

        private void ReadData(string s)
        {
            int code = int.Parse(s.Substring(0, 3));
            string val = GetDataFromCode(s);
            int clientID;
            int x;
            int y;
            try
            {
                switch (code)
                {
                    case 001:
                        //create new player
                        clientID = int.Parse(val.Substring(0, val.IndexOf(":")));
                        x = int.Parse(val.Substring(val.IndexOf(":") + 1, val.LastIndexOf(":") - val.IndexOf(":") - 1));
                        y = int.Parse(val.Substring(val.LastIndexOf(":") + 1, val.Length - val.LastIndexOf(":") - 1));
                        NewClient client = new NewClient(clientID, x, y);
                        client.LoadContent(this.Content, "newPlayer" + clientID.ToString());
                        connectedClients.Add(client);
                        break;
                    case 002:
                        clientID = int.Parse(val.Substring(0, val.IndexOf(":")));
                        x = int.Parse(val.Substring(val.IndexOf(":") + 1, val.LastIndexOf(":") - val.IndexOf(":") - 1));
                        y = int.Parse(val.Substring(val.LastIndexOf(":") + 1, val.Length - val.LastIndexOf(":") - 1));
                        foreach (NewClient cl in connectedClients)
                        {
                            if (clientID == cl.ClientID)
                            {
                                cl.xPos = x;
                                cl.yPos = y;
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private string GetDataFromCode(string s)
        {
            s = s.Substring(s.IndexOf("%") + 1, s.Length - s.IndexOf("%") - 1);
            return s;
        }

        int uniqueNr = 0;
        private void SendData(string s)
        {
            try
            {
                Byte[] sendBytes = null;
                NetworkStream networkStream = client.GetStream();
                sendBytes = Encoding.ASCII.GetBytes(s + "$" + uniqueNr + "^");
                networkStream.Write(sendBytes, 0, sendBytes.Length);
                networkStream.Flush();
                uniqueNr += 1;
                if (uniqueNr == 10)
                    uniqueNr = 0;
            }
            catch (Exception)
            {
                isConnected = false;
                client = new TcpClient();
            }
        }

        public virtual void SendPosition(int xPos, int yPos)
        {
            SendData("001%" + xPos + ":" + yPos);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            try
            {
                client.Close();
                receiveDataThread.Abort();
                isConnected = false;
            }
            catch
            {

            }
        }
    }
}
