using System;
using System.Collections.Generic;
using System.Linq;
using FishFood;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace FishFoodUI
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        IStateSimulation _fishBowl;

        public Game(IStateSimulation fishBowl)
        {
            _fishBowl = fishBowl;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = FishBowl.DEFAULT_WIDTH;
            _graphics.PreferredBackBufferHeight = FishBowl.DEFAULT_HEIGHT;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _fishBowl.Init();
            base.Initialize();
        }

        private Texture2D _fishSprite;
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _fishSprite = Content.Load<Texture2D>("Fish");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Is the SPACE key down?
            //if (Keyboard.GetState().IsKeyDown(Keys.Space))
                  _fishBowl.UpdateState();
            
            if (!_fishBowl.KeepGoing()) // run out of fish, make more 
                _fishBowl.Init();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Vector2 origin = new Vector2(0, 0);
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            bool firstFish = true;
            foreach (var fish in _fishBowl.GetState())
            {
                Vector2 fishPos = new Vector2(fish.X, fish.Y);
                //Vector2 fishPos = new Vector2(10, 10);
               
                _spriteBatch.Draw(_fishSprite, fishPos, null, Color.White, 0, origin, fish.Scale, SpriteEffects.None, 0);

            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
