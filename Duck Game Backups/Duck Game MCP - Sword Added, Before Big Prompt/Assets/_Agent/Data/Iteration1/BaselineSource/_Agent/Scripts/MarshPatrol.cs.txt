using UnityEngine;

namespace DuckAdventure.Agent
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class MarshPatrol : MonoBehaviour
    {
        [SerializeField] private Vector2 pointA;
        [SerializeField] private Vector2 pointB;
        [SerializeField] private float speed = 0.85f;
        [SerializeField] private float phase;
        private Rigidbody2D body;
        private void Awake() { body = GetComponent<Rigidbody2D>(); }
        private void FixedUpdate()
        {
            float length = Vector2.Distance(pointA, pointB);
            float t = Mathf.PingPong(Time.time * speed + phase, length) / length;
            body.MovePosition(Vector2.Lerp(pointA, pointB, t));
        }
    }
}
