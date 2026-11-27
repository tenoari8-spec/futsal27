using UnityEngine;

namespace Futsal27.Match
{
    /// <summary>Detecta a bola dentro do gol. side = +1 gol do lado +X (marca o time da casa), -1 lado -X.</summary>
    public class GoalSensor : MonoBehaviour
    {
        public int side;
        public System.Action<int> onGoal;

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<BallController>() != null && onGoal != null) onGoal(side);
        }
    }
}
