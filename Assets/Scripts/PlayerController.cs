using UnityEngine;

namespace MoIVQuest
{
    /// <summary>
    /// Dead-simple top-down movement for MO. Arrow keys / WASD.
    /// Uses the legacy Input class, which works with Unity's default
    /// "Input Manager (Old)" setting — no extra setup needed.
    ///
    /// Attach to MO's sprite GameObject. A Rigidbody2D is optional;
    /// if present we move via physics, otherwise we move the transform.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed = 4f;

        private Rigidbody2D _rb;
        private Vector2 _input;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            // GetAxisRaw = snappy, no smoothing. Good for top-down grid feel.
            _input = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;

            if (_rb == null)
                transform.Translate(_input * moveSpeed * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (_rb != null)
                _rb.MovePosition(_rb.position + _input * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
