using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timespinner.GameAbstractions;
using Timespinner.GameAbstractions.Gameplay;
using Timespinner.GameObjects.BaseClasses;
using Timespinner.GameStateManagement.ScreenManager;
using TsMod.Extensions;

namespace TsMod.Screens
{
	//The framework automaticly binds this to the corresponding timespinner screen when its open
	[TimeSpinnerType("Timespinner.GameStateManagement.Screens.InGame.GameplayScreen")]
	class GameplayScreen : Screen
    {
        bool DrawAggroBbox = false;
		bool DrawBaseSprite = true;
		bool DrawApendages = true;
        bool DrawBoundingBox = false;
        bool DrawParticle = true;
		bool DrawProjectileDamageArea = false;
		
        public GameplayScreen(ScreenManager screenManager, GameScreen screen) : base(screenManager, screen)
		{
		}

		public override void Initialize(GCM gameContentManager)
		{
		}

		//called each update frame
		public override void Update(GameTime gameTime, InputState input)
		{
			Level level = Dynamic._level;
            var levelDynamic = level.AsDynamic();

            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.F1, null, out _))
            {
                DrawAggroBbox = !DrawAggroBbox;
            }
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.F2, null, out _))
            {
                DrawBaseSprite = !DrawBaseSprite;
            }
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.F3, null, out _))
            {
                DrawApendages = !DrawApendages;
            }
			if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.F4, null, out _))
            {
                DrawBoundingBox = !DrawBoundingBox;
            }
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.F5, null, out _))
            {
                DrawParticle = !DrawParticle;
            }
            if (input.IsNewKeyPress(Microsoft.Xna.Framework.Input.Keys.F6, null, out _))
            {
                DrawProjectileDamageArea = !DrawProjectileDamageArea;
            }

            foreach (Monster monster in levelDynamic._enemies.Values)
            {
                var enemy = monster.AsDynamic();

                enemy._doesDrawAggroBbox = DrawAggroBbox;
				enemy._doesDrawBaseSprite = DrawBaseSprite;
                enemy._doesDrawAppendages = DrawApendages;
				monster.DoesDrawBoundingBox = DrawBoundingBox;
				enemy._doesDrawParticleSystems = DrawParticle;
            }

            if (level.MainHero != null)
            {
                var lunais = level.MainHero.AsDynamic();

                lunais._doesDrawBaseSprite = DrawBaseSprite;
                lunais._doesDrawAppendages = DrawApendages;
                lunais._doesDrawParticleSystems = DrawParticle;
            }

            foreach (Projectile projectiletje in levelDynamic._enemyProjectiles.Values)
            {
                var projectile = projectiletje.AsDynamic();

                projectile._doesDrawDamageBbox = DrawProjectileDamageArea;
            }

            foreach (Projectile projectiletje in levelDynamic._heroProjectiles.Values)
            {
                var projectile = projectiletje.AsDynamic();

                projectile._doesDrawDamageBbox = DrawProjectileDamageArea;
            }
        }

        //called each render frame
		public override void Draw(SpriteBatch spriteBatch, SpriteFont menuFont)
		{
		}
	}
}
