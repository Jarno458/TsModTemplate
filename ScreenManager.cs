using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Timespinner;
using Timespinner.GameAbstractions;
using Timespinner.GameStateManagement.ScreenManager;
using TsMod.Extensions;
using TsMod.Screens;

namespace TsMod
{
	class ScreenManager : Timespinner.GameStateManagement.ScreenManager.ScreenManager
	{
		readonly LookupDictionary<GameScreen, Screen> hookedScreens
			= new LookupDictionary<GameScreen, Screen>(s => s.GameScreen);
		readonly List<GameScreen> foundScreens = new List<GameScreen>(20);


		public readonly dynamic Dynamic;
		public GCM GameContentManager => Dynamic.GCM;

		public ScreenManager(TimespinnerGame game, PlatformHelper platformHelper) : base(game, platformHelper)
		{
			Dynamic = this.AsDynamic();
		}

		public override void Update(GameTime gameTime)
		{
			var input = (InputState)Dynamic._input;

			DetectNewScreens();
			UpdateScreens(gameTime, input);

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			DrawGameplayScreens();
		}

		void DetectNewScreens()
		{
			foundScreens.Clear();

			foreach (var screen in GetScreens())
			{
				if (hookedScreens.Contains(screen))
				{
					foundScreens.Add(screen);

					continue;
				}

				if(!Screen.RegisteredTypes.TryGetValue(screen.GetType(), out var handlerType))
					continue;

				var screenHandler = (Screen)Activator.CreateInstance(handlerType, this, screen);
				hookedScreens.Add(screenHandler);
				foundScreens.Add(screen);

				screenHandler.Initialize(GameContentManager);
			}

			if (foundScreens.Count != hookedScreens.Count)
				hookedScreens.Filter(foundScreens, s => s.Unload());
		}

		void UpdateScreens(GameTime gameTime, InputState input)
		{
			foreach (var screen in hookedScreens)
				screen.Update(gameTime, input);
		}

		void DrawGameplayScreens()
		{
			foreach (var screen in hookedScreens)
				screen.Draw(SpriteBatch, MenuFont);
		}

		public void CopyScreensFrom(Timespinner.GameStateManagement.ScreenManager.ScreenManager screenManager)
		{
			foreach (var screen in screenManager.GetScreens())
				AddScreen(screen, null);
		}

		public T FirstOrDefault<T>() where T : Screen => (T)hookedScreens.FirstOrDefault(s => s.GetType() == typeof(T));
	}
}
