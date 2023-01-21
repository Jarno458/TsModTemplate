using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameStateManagement.ScreenManager;
using TsMod.Extensions;

namespace TsMod.Screens
{
	//The framework automaticly binds this to the corresponding timespinner screen when its open
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen")]
	class GameplayScreen : Screen
    {
		RoomSpecification currentRoom;

		public GCM GameContentManager { get; private set; }

		dynamic LevelReflected => ((object)Dynamic._level).AsDynamic();

		public GameplayScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Initialize(GCM gameContentManager)
		{
			GameContentManager = gameContentManager;
		}

		//called each update frame
		public override void Update(GameTime gameTime, InputState input)
		{
            GameSave save = Dynamic.SaveFile;
			Level level = Dynamic._level;

			if (IsRoomChanged())
            {
				//Timespinner loads 1 room at a time so after a room chance its a good moment to update the state of the level

                if (level.ID == 1 && level.RoomID == 13)
                {
                    SpawnBird(level, new Point(21, 9));
                }
                else if (level.ID == 1 && level.RoomID == 1)
                {
                    SpawnPlatform(level, new Point(11, 6));
                    SpawnPlatform(level, new Point(16, 3));
				}
			}
		}

        static void SpawnBird(Level level, Point point)
        {
			var babyBird = new ObjectTileSpecification
            {
                Category = EObjectTileCategory.Enemy,
                Layer = ETileLayerType.Objects,
                ObjectID = (int)EEnemyTileType.ForestBabyCheveux,
                IsFlippedHorizontally = level.MainHero.Position.X > level.RoomSize.X / 2,
                X = point.X,
                Y = point.Y
            };

            level.PlaceEvent(babyBird, false);
		}

        static void SpawnPlatform(Level level, Point point)
        {
            var tileSpecifications = new[] {
				//ID = the sprite to use, try different id's for different sprites
                new TileSpecification { Layer = ETileLayerType.Middle, ID = 109, X = point.X, Y = point.Y },
                new TileSpecification { Layer = ETileLayerType.Middle, ID = 110, X = point.X + 1, Y = point.Y },
                new TileSpecification { Layer = ETileLayerType.Middle, ID = 110, X = point.X + 2, Y = point.Y },
                new TileSpecification { Layer = ETileLayerType.Middle, ID = 111, X = point.X + 3, Y = point.Y },
            };

            foreach (var tile in tileSpecifications)
                level.SolidTiles.Add(new Point(tile.X, tile.Y), Tile.FromSpecification(tile, level));
		}

		//called each render frame
		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
			if (currentRoom == null)
				return;

            //the AsDynamic framework allow us to directly interact with private or protected members/methods
			var levelId = LevelReflected._id;
			var text = $"TsMod Level: {levelId}, Room ID: {currentRoom.ID}";

			var inGameZoom = (int)TimeSpinnerGame.Constants.InGameZoom;

			using (spriteBatch.BeginUsing())
				spriteBatch.DrawString(menuFont, text, new Vector2(30, 130), Color.Yellow, inGameZoom);
		}
		
		bool IsRoomChanged()
		{
			if (currentRoom != null && LevelReflected.CurrentRoom == currentRoom) return false;

			currentRoom = LevelReflected.CurrentRoom;

			return true;
		}
	}
}
