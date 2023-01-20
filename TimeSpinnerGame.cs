﻿using Timespinner;
using TsMod.Extensions;

namespace TsMod
{
	class TimeSpinnerGame : TimespinnerGame
	{
		readonly PlatformHelper platformHelper;

		public static dynamic Constants => new Constants();

		public TimeSpinnerGame(PlatformHelper platformHelper) : base(platformHelper)
		{
			this.platformHelper = platformHelper;
			HookScreenManager();
		}

		void HookScreenManager()
		{
			var screenManager = Components.FirstOfType<Timespinner.GameStateManagement.ScreenManager.ScreenManager>();
			var newScreenManager = new ScreenManager(this, platformHelper);

			this.AsDynamic()._screenManager = newScreenManager;

			newScreenManager.CopyScreensFrom(screenManager);

			Components.ReplaceComponent(screenManager, newScreenManager);
		}
	}
}
