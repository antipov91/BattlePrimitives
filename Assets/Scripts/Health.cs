using UnityEngine;

namespace BattlePrimitives
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int health;

        public int CurrentHealth
        {
            get { return health; }
            set
            {
                health = value;

                if (health <= 0)
                    Destroy(gameObject);
            }
        }
    }
}