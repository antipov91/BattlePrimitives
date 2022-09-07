using BattlePrimitives.StateMachine;
using System;
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
                context.OnMobExited += MobExitHandle;
                context.targetAttackMob.OnDestroyed += DestroyedHandle;
            }

            private void DestroyedHandle(Mob other)
            {
                ChangeState();
            }

            private void MobExitHandle(Mob otherMob)
            {
                if (otherMob == context.targetAttackMob)
                {
                    ChangeState();
                }
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
                var enemyHealth = context.targetAttackMob.GetComponent<Health>();
                enemyHealth.CurrentHealth -= context.damage;

                if (enemyHealth.CurrentHealth <= 0)
                    ChangeState();
            }

            private void ChangeState()
            {
                if (context.leaderMob != null)
                    stateMachine.CurrentState = context.followState;
                else
                    stateMachine.CurrentState = context.searchState;
            }

            public override void Deactivate()
            {
                context.OnMobExited -= MobExitHandle;
                context.targetAttackMob.OnDestroyed -= DestroyedHandle;
            }
        }
    }
}