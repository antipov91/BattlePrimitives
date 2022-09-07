using BattlePrimitives.StateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BattlePrimitives
{
    public partial class Mob : MonoBehaviour
    {
        public float Velocity { get { return velocity; } }

        [SerializeField] private int health = 10;
        [SerializeField] private int damage = 1;
        [SerializeField] private float attackRate = 1f;
        [SerializeField] private float velocity = 1f;
        [SerializeField] private string wallsTag = "Walls";
        [SerializeField] private float searchDistance = 5f;

        public int Health
        {
            get { return health; }
            set
            {
                health = value;

                if (health <= 0)
                    Destroy(gameObject);
            }
        }

        private Action<Mob> OnMobEntered;
        private Action<Mob> OnMobExited;

        private Mob targetAttackMob;

        private Mob leaderMob;
        private bool isLeader;
        private bool isSlave;

        private List<Mob> neighboringMobs;

        private StateMachine<Mob> stateMachine;

        private SearchState searchState;
        private AttackState attackState;
        private FollowState followState;


        private void Awake()
        {
            neighboringMobs = new List<Mob>();

            stateMachine = new StateMachine<Mob>();

            searchState = new SearchState(this, stateMachine);
            attackState = new AttackState(this, stateMachine);
            followState = new FollowState(this, stateMachine);

            stateMachine.CurrentState = searchState;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<Mob>(out var mob))
            {
                if (neighboringMobs.Contains(mob) == false)
                {
                    neighboringMobs.Add(mob);
                    OnMobEntered?.Invoke(mob);
                }
            } 
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent<Mob>(out var mob))
            {
                neighboringMobs.Remove(mob);
                OnMobExited?.Invoke(mob);
            }
        }

        private void Update()
        {
            stateMachine.CurrentState.Update();
        }

        private void LateUpdate()
        {
            stateMachine.CurrentState.LateUpdate();
        }
    }
}