using UnityEngine;

namespace DuckAdventure.Agent
{
    [DefaultExecutionOrder(-40)]
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class MarshPatrol : MonoBehaviour
    {
        [SerializeField] private Vector2 pointA;
        [SerializeField] private Vector2 pointB;
        [SerializeField] private float speed = 0.85f;
        [SerializeField] private float phase;
        [SerializeField] private SectionData section;
        [SerializeField] private string encounterId;
        [SerializeField] private EnemyHitPoints health;
        private EnemyData definition;
        public EnemyData Definition => definition;
        private Rigidbody2D body;
        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            if(section!=null)
            {
                if(health==null)throw new System.InvalidOperationException("Configured patrol requires its health reference.");
                var encounter=section.GetEncounter(encounterId);definition=encounter.data;definition.ValidateData();
                pointA=encounter.pointA;pointB=encounter.pointB;body.position=encounter.position;transform.position=encounter.position;health.ConfigureForEncounter(definition.health);
            }
        }
        private void FixedUpdate()
        {
            float length = Vector2.Distance(pointA, pointB);
            float t = Mathf.PingPong(Time.time * (definition==null?speed:definition.moveSpeed) + phase, length) / length;
            body.MovePosition(Vector2.Lerp(pointA, pointB, t));
        }
    }
}

