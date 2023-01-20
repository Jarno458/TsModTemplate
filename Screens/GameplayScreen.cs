using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core.Specifications;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameAbstractions.Saving;
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
            GameSave Save = (GameSave)Dynamic.SaveFile;
			Level Level = (Level)Dynamic._level;

			if (IsRoomChanged())
            {
				//Timespinner loads 1 room at a time so after a room chance its a good moment to update the state of the level
            }
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
