using BattlePrimitives.StateMachine;
using System.Linq;
using UnityEngine;

namespace BattlePrimitives
{
    public partial class Mob
    {
        public class AttackState : State<Mob>
        {
            private float reloadDuration;
            private float meleeThreshold = 1.25f;

            public AttackState(Mob context, StateMachine<Mob> stateMachine) : base(context, stateMachine) { }

            public override void Activate()
            {
                reloadDuration = 0f;
            }

            public override void Update()
            {
                if (Vector3.Distance(context.transform.position, context.targetAttackMob.transform.position) > meleeThreshold)
                {
                    context.transform.position = Vector3.MoveTowards(context.transform.position, context.targetAttackMob.transform.position, Time.deltaTime * context.Velocity);
                    context.transform.forward = context.targetAttackMob.transform.position - context.transform.position;
                    return;
                }

                if (reloadDuration > 0f)
                {
                    reloadDuration -= Time.deltaTime;
                    return;
                }

                reloadDuration = 1f / context.attackRate;
                context.targetAttackMob.Health -= context.damage;
            }

            public override void LateUpdate()
            {
                if (context.targetAttackMob.Health <= 0)
                {
                    var enemies = context.neighboringMobs.Where(x => x.leaderMob != context && x != context.leaderMob && x.Health > 0);
                    
                    if (enemies.Count() == 1)
                        context.targetAttackMob = enemies.First();
                    else if (enemies.Count() == 0)
                    {
                        if (context.leaderMob != null)
                            stateMachine.CurrentState = context.followState;
                        else
                            stateMachine.CurrentState = context.searchState;
                    }
                    else
                    {
                        var bestEnemy = enemies.First();
                        var bestDistance = Vector3.Distance(context.transform.position, bestEnemy.transform.position);
                        foreach (var enemy in enemies)
                        {
                            var distance = Vector3.Distance(context.transform.position, enemy.transform.position);
                            if (distance < bestDistance)
                            {
                                bestDistance = distance;
                                bestEnemy = enemy;
                            }
                        }
                        context.targetAttackMob = bestEnemy;
                    }
                }

                if (context.leaderMob != null && context.leaderMob.Health <= 0)
                    context.leaderMob = null;
            }
        }
    }
}