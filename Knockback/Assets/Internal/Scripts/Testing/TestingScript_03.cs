using UnityEngine;

namespace Knockback.Testing
{
    public class TestingScript_03 : MonoBehaviour
    {
        public LayerMask layer;
        public int maximumRays = 4;

        private void FixedUpdate()
        {
           // if (Input.GetKeyDown(KeyCode.Mouse0))
                DoRayCast(transform.position, GetCalculatedRotation());
        }

        private void DoRayCast(Vector2 startPos, Vector2 direction)
        {
            for (int index = 0; index < maximumRays; index++)
            {
                RaycastHit2D hit = Physics2D.Raycast(startPos, direction);

                if (hit.collider != null)
                {
                    Debug.DrawLine(startPos, hit.point, Color.red, 0.01f);

                    direction = Vector2.Reflect(direction, hit.normal);
                    startPos = hit.point + (direction.normalized * 0.01f);
                }
            }

        }

        public Vector2 GetCalculatedRotation()
        {
            Vector2 difference = Vector2.zero;
            difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            difference.Normalize();
            return difference;
        }
    }
}