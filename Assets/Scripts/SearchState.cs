using BattlePrimitives.StateMachine;
using UnityEngine;

namespace BattlePrimitives
{
    public partial class Mob
    {
        public class SearchState : State<Mob>
        {
            private Vector3 target;
            private bool isGoal;

            private float targetThreshold = 0.1f;

            public SearchState(Mob context, StateMachine<Mob> stateMachine) : base(context, stateMachine) { }

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
                else
                {
                    context.isLeader = true;
                    otherMob.isSlave = true;
                    otherMob.leaderMob = context;
                    otherMob.stateMachine.CurrentState = otherMob.followState;
                    context.OnDestroyed += otherMob.OnLeaderDestroyed;
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

                if (isGoal == false)
                {
                    target = context.transform.position + new Vector3(Random.Range(-context.searchDistance, context.searchDistance), 0f, Random.Range(-context.searchDistance, context.searchDistance));
                    isGoal = true;
                }

                context.transform.position = Vector3.MoveTowards(context.transform.position, target, Time.deltaTime * context.Velocity);
                context.transform.forward = target - context.transform.position;

                if (Physics.Raycast(context.transform.position, context.transform.forward, out var hit, 1f))
                {
                    if (hit.collider.gameObject.CompareTag(context.wallsTag))
                    {
                        isGoal = false;
                        return;
                    }
                        

                    if (hit.collider.gameObject.TryGetComponent<Mob>(out var mob))
                    {
                        if (mob.leaderMob == context)
                        {
                            isGoal = false;
                            return;
                        }
                    }
                }
                
                if (Vector3.Distance(context.transform.position, target) < targetThreshold)
                    isGoal = false;
            }

            public override void Deactivate()
            {
                context.OnMobEntered -= MobEnterHandle;
            }
        }
    }
}