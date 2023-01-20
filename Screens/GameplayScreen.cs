using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.Core;
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
        static readonly HookManager HookManager = new HookManager();

		static readonly MethodInfo AliveManageDamage = TimeSpinnerType
            .Get("Timespinner.GameObjects.BaseClasses.Alive")
            .GetMethod("ManageDamage");
        static readonly MethodInfo OnDamageReceivedMethod = typeof(GameplayScreen)
            .GetMethod("OnDamageReceived", BindingFlags.Static | BindingFlags.NonPublic);
		static readonly MethodInfo LunaisOrbDetermineDamage = TimeSpinnerType
            .Get("Timespinner.GameObjects.Heroes.Orbs.LunaisBaseOrbDamageArea")
            .GetMethod("DetermineDamage");
        static readonly MethodInfo OnDealDamageMethod = typeof(GameplayScreen)
            .GetMethod("OnDealDamage", BindingFlags.Static | BindingFlags.NonPublic);

		RoomSpecification currentRoom;

		public GCM GameContentManager { get; private set; }

		dynamic LevelReflected => ((object)Dynamic._level).AsDynamic();


		public GameplayScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Initialize(GCM gameContentManager)
		{
			GameContentManager = gameContentManager;

            HookManager.Hook(AliveManageDamage, OnDamageReceivedMethod);
            HookManager.Hook(LunaisOrbDetermineDamage, OnDealDamageMethod);
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

        static bool OnDamageReceived(
			Alive alive,
            int damage,
            Vector2 velocity,
            Point where,
            Rectangle sourceRectangle,
            EDamageType type,
            EDamageElement element,
            bool doesKnockBack)
        {
			//do something

			//call base
			HookManager.Unhook(AliveManageDamage);
            var ret = (bool)AliveManageDamage.Invoke(alive, 
                new object[] { damage, velocity, where, sourceRectangle, type, element, doesKnockBack });
			HookManager.Hook(AliveManageDamage, OnDamageReceivedMethod);

            return ret;
        }

        static bool OnDealDamage(dynamic damageArea, Alive target, Rectangle collisionRectangle)
        {
			//do something (damageArea is provided as dynamic since class LunaisBaseOrbDamageArea is internal to Timespinner)

			//call base
			HookManager.Unhook(LunaisOrbDetermineDamage);
            var ret = (bool)LunaisOrbDetermineDamage.Invoke(damageArea, 
                new object[] { target, collisionRectangle });
            HookManager.Hook(LunaisOrbDetermineDamage, OnDealDamageMethod);

            return ret;

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
