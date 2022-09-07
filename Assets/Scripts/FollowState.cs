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
                if (context.isLeader || context.isSlave)
                {
                    if (context.leaderMob == otherMob || otherMob.leaderMob == context)
                        return;

                    context.targetAttackMob = otherMob;
                    stateMachine.CurrentState = context.attackState;
                }
            }

            public override void Update()
            {
                if (context.neighboringMobs.Count > 1)
                {
                    context.targetAttackMob = context.neighboringMobs.Find(x => x.leaderMob != context && x != context.leaderMob);
                    if (context.targetAttackMob != null)
                        stateMachine.CurrentState = context.attackState;
                }

                if (Vector3.Distance(context.transform.position, context.leaderMob.transform.position) < followThreshold)
                    return;

                context.transform.position = Vector3.MoveTowards(context.transform.position, context.leaderMob.transform.position, Time.deltaTime * context.Velocity);
                context.transform.forward = context.leaderMob.transform.position - context.transform.position;
            }

            public override void Deactivate()
            {
                context.OnMobEntered -= MobEnterHandle;
            }
        }
    }
}