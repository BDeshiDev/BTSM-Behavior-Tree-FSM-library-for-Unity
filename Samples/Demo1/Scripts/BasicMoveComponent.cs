using System.Collections;
using UnityEngine;

namespace Bdeshi.BTSM.Samples.Demo1
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicMoveComponent : MonoBehaviour
    {
        Rigidbody rb;
        public float speed = 5;
        public Vector3 MoveInputNextFrame = Vector3.zero;
        [SerializeField] Vector3 MoveVelNextFrame = Vector3.zero;
        // Use this for initialization
        void Awake()
        {
            rb =GetComponent<Rigidbody>();
        }
        public void setLookDirection(Vector3 lookDir)
        {
            rb.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        }

        private void FixedUpdate()
        {
            rb.velocity = MoveVelNextFrame = MoveInputNextFrame * speed;
        }
    }
}