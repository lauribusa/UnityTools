using UnityEngine;

namespace GBehavior
{
    public class TickTest: GBehavior
    {
        internal override void OnUpdate(float deltaTime, float unscaledDeltaTime, int ticks)
        {
            //Debug.Log($"<color=cyan>TickTest::OnUpdate</color> {deltaTime} {unscaledDeltaTime} {ticks}");
            base.OnUpdate(deltaTime, unscaledDeltaTime, ticks);
        }

        internal override void OnFixedUpdate(float deltaTime, float unscaledDeltaTime, int ticks)
        {
            //Debug.Log($"<color=red>TickTest::OnFixedUpdate</color> {deltaTime} {unscaledDeltaTime} {ticks}");
            base.OnFixedUpdate(deltaTime, unscaledDeltaTime, ticks);
        }

        internal override void OnLateUpdate(float deltaTime, float unscaledDeltaTime, int ticks)
        {
            //Debug.Log($"<color=orange>TickTest::OnLateUpdate</color> {deltaTime} {unscaledDeltaTime} {ticks}");
            base.OnLateUpdate(deltaTime, unscaledDeltaTime, ticks);
        }
    }
}