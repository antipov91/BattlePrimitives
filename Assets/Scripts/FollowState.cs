using BattlePrimitives.StateMachine;
using UnityEngine;

namespace BattlePrimitives
{
    public partial class Mob
    {
        public class FollowState : State<Mob>
        {
            private float followThreshold = 0.8f;

            public FollowState(Mob context, StateMachine<Mob> stateMachine) : base(context, stateMachine) { }

            public override void Activate()
            {
                context.OnMobEntered += MobEnterHandle;
            }

            private void MobEnterHandle(Mob otherMob)
            {
                if (context.leaderMob == otherMob || otherMob.leaderMob == context)
                    return;

                context.targetAttackMob = otherMob;
                stateMachine.CurrentState = context.attackState;
            }

            public override void Update()
            {
                if (Vector3.Distance(context.transform.position, context.leaderMob.transform.position) < followThreshold)
                    return;

                context.transform.position = Vector3.MoveTowards(context.transform.position, context.leaderMob.transform.position, Time.deltaTime * context.Velocity);
                context.transform.forward = context.leaderMob.transform.position - context.transform.position;
            }

            public override void LateUpdate()
            {
                if (context.leaderMob != null && context.leaderMob.Health <= 0)
                {
                    context.leaderMob = null;
                    stateMachine.CurrentState = context.searchState;
                }
            }

            public override void Deactivate()
            {
                context.OnMobEntered -= MobEnterHandle;
            }
        }
    }
}