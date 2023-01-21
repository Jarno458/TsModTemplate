using System.Reflection;
using Microsoft.Xna.Framework;
using Timespinner.Core;
using Timespinner.GameObjects.BaseClasses;

namespace TsMod
{
    static class DamageHooks
    {
        static readonly HookManager HookManager = new HookManager();

        static readonly MethodInfo AliveManageDamage = TimeSpinnerType
            .Get("Timespinner.GameObjects.BaseClasses.Alive")
            .GetMethod("ManageDamage");
        static readonly MethodInfo OnDamageReceivedMethod = typeof(DamageHooks)
            .GetMethod("OnDamageReceived", BindingFlags.Static | BindingFlags.NonPublic);
        static readonly MethodInfo LunaisOrbDetermineDamage = TimeSpinnerType
            .Get("Timespinner.GameObjects.Heroes.Orbs.LunaisBaseOrbDamageArea")
            .GetMethod("DetermineDamage");
        static readonly MethodInfo OnDealDamageMethod = typeof(DamageHooks)
            .GetMethod("OnDealDamage", BindingFlags.Static | BindingFlags.NonPublic);

        public static void InitializeHooks()
        {
            HookManager.Hook(AliveManageDamage, OnDamageReceivedMethod);
            HookManager.Hook(LunaisOrbDetermineDamage, OnDealDamageMethod);
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
    }
}
