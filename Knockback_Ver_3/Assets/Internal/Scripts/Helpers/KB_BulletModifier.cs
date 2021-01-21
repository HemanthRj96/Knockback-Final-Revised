using Knockback.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Helpers
{
    public class KB_BulletModifier : MonoBehaviour
    {
        //** --ATTRIBUTES-- 
        //** --SERIALIZED ATTRIBUTES--

        [SerializeField] private float m_minimumAngleFromNormal = 40;
        [SerializeField] private int m_maximumRicochetLimit = 10;
        [SerializeField] private float m_bulletLifeTime = 10;
        [SerializeField] private LayerMask m_detectionLayer = 1 << 8;

        //** --PRIVATE ATTRIBUTES--        

        private bool m_shouldRicochet = true;
        private float m_impactDamage;
        private float m_speed;
        private bool m_updateThroughPathpoints = false;
        private bool m_shouldPassthrough = false;
        private int m_ricochetCounter = 0;
        private int m_totalRicochetCounter = 0;
        private Rigidbody2D m_rb = null;
        private RaycastHit2D m_upcomingHit;
        private List<Vector2> m_ricochetHitPoints = new List<Vector2>();
        private List<Quaternion> m_ricochetPointRotations = new List<Quaternion>();
        private List<RaycastHit2D> m_hitResults = new List<RaycastHit2D>();

        //** --PUBLIC ATTRIBUTES--

        public bool m_canDetect = true;

        //** --PRIVATE REFERENCES--

        private Vector2 m_bulletDirection => transform.rotation * Vector2.right;


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Methods to enable and disable ricochet and passthrough
        /// </summary>
        public void EnableRicochet() => m_shouldRicochet = true;
        public void DisableRicochet() => m_shouldRicochet = false;
        public void EnablePassthrough() => m_shouldPassthrough = true;
        public void DisablePassthrough() => m_shouldPassthrough = false;

        /// <summary>
        /// Method to start this bullet to move
        /// </summary>
        public void StartBulletTranslation()
        {
            Invoke("DeactivateBullet", m_bulletLifeTime);
            CreateNewPath();
            if (m_totalRicochetCounter == 0)
                MoveWithRigidbody(m_bulletDirection);
            else
                m_updateThroughPathpoints = true;
        }

        /// <summary>
        /// Call this method when the bullet hits something
        /// </summary>
        /// <param name="hit">Target hit parameter</param>
        public bool OnHit(RaycastHit2D hit)
        {
            IDamage damageHandler = null;

            if (hit.collider.gameObject.TryGetComponent(out damageHandler))
                ApplyDamage(damageHandler);

            TryBulletDynamics(hit, damageHandler);
            return true;
        }

        /// <summary>
        /// Returns a hit result
        /// </summary>
        public RaycastHit2D DoRayCast(Vector2 startPoint, Vector2 direction, float distance = -1)
        {
            if (distance == -1)
                return Physics2D.Raycast(startPoint, direction, 500, m_detectionLayer);
            else
                return Physics2D.Raycast(startPoint, direction, distance, m_detectionLayer);
        }

        /// <summary>
        /// Returns true if the hit matches with the item
        /// </summary>
        /// <param name="hit">The target hit we're checking for</param>
        /// <returns></returns>
        public bool CheckIfTheHitExists(RaycastHit2D hit) => m_upcomingHit == hit;

        /// <summary>
        /// Add a new layer to the detection layerMask
        /// </summary>
        /// <param name="layerIndex"></param>
        public void AddNewDetectMask(int layerIndex) => m_detectionLayer = m_detectionLayer | 1 << layerIndex;

        /// <summary>
        /// Method to set the bullet damage and speed
        /// </summary>
        /// <param name="impactDamage">Damage value</param>
        /// <param name="speed">Speed value</param>
        public void SetDamageAndSpeed(float impactDamage, float speed)
        {
            m_impactDamage = impactDamage;
            m_speed = speed;
        }

        /// <summary>
        /// Returns true if ricochet is enabled
        /// </summary>
        public bool ShouldRicochet() => m_shouldRicochet;


        //** --PRIVATE METHODS--

        /// <summary>
        /// Assign the rigidbody 
        /// </summary>
        private void Awake() => m_rb = GetComponent<Rigidbody2D>();

        /// <summary>
        /// FixedUpdate where the bullet will move according to the pathpoints if updateThroughPathPoints is set as true
        /// </summary>
        private void FixedUpdate()
        {
            if (m_updateThroughPathpoints)
                TranslateThroughPathPoints();
        }

        /// <summary>
        /// Method to apply passthrough and ricochet ability to the bullet
        /// </summary>
        /// <param name="hit">Hit parameter</param>
        /// <param name="damageHandler">Damage handler</param>
        private void TryBulletDynamics(RaycastHit2D hit, IDamage damageHandler)
        {
            if (m_shouldPassthrough)
                return;
            else if (damageHandler != null || !m_shouldRicochet)
            {
                m_canDetect = false;
                StopBullet();
                return;
            }
            else if (m_ricochetCounter == m_maximumRicochetLimit)
            {
                m_updateThroughPathpoints = false;
                StopBullet(true);
                return;
            }
            else
            {
                if (CheckIfTheHitExists(hit))
                    return;
                m_totalRicochetCounter = m_ricochetCounter;
                m_ricochetCounter = 0;
                ClearRicochetList();
                CreateNewPath();
                return;
            }
        }

        /// <summary>
        /// Method to move the bullet
        /// </summary>
        private void TranslateThroughPathPoints()
        {
            if (m_ricochetCounter < m_ricochetHitPoints.Count)
            {
                if (CheckForClosingDistance(m_ricochetHitPoints[m_ricochetCounter]))
                {
                    UpdateBulletRotation();
                    ++m_ricochetCounter;
                }
                else
                    MoveTowards(m_ricochetHitPoints[m_ricochetCounter]);
            }
            else
            {
                m_updateThroughPathpoints = false;
                MoveWithRigidbody(m_bulletDirection);
            }
        }

        /// <summary>
        /// Just make the bullet go indefinitely
        /// </summary>
        private void MoveWithRigidbody(Vector2 direction)
        {
            if (m_rb.velocity.magnitude < m_speed)
                m_rb.velocity = m_speed * direction;
        }

        /// <summary>
        /// Create a new path for the bullet to go
        /// </summary>
        private void CreateNewPath()
        {
            if (!m_shouldRicochet)
                return;

            Vector2 startPosition = transform.position;
            Vector2 direction = transform.rotation * Vector2.right;
            RaycastHit2D hit;

            while (true)
            {
                hit = DoRayCast(startPosition, direction);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.GetComponent<IDamage>() != null)
                        break;
                    if (HasMinimumRicochetAngle(hit.normal, direction))
                    {
                        if (m_totalRicochetCounter == m_maximumRicochetLimit)
                            break;
                        if (m_ricochetCounter == 0)
                            m_upcomingHit = hit;
                        ++m_totalRicochetCounter;

                        direction = Vector2.Reflect(direction, hit.normal);
                        startPosition = hit.point + (direction.normalized * 0.01f);

                        m_ricochetHitPoints.Add(hit.point);
                        m_ricochetPointRotations.Add(GetNewRicochetRotation(direction));
                        m_hitResults.Add(hit);

                        continue;
                    }
                    else
                        break;
                }
                break;
            }
        }

        /// <summary>
        /// Method to clear the ricochet lists
        /// </summary>
        private void ClearRicochetList()
        {
            m_ricochetHitPoints.Clear();
            m_ricochetPointRotations.Clear();
            m_hitResults.Clear();
        }

        /// <summary>
        /// Method to apply damage to the damage handler
        /// </summary>
        /// <param name="damageHandler"></param>
        private void ApplyDamage(IDamage damageHandler) => damageHandler?.ApplyDamage(m_impactDamage);

        /// <summary>
        /// Returns true if the target reaches the end point
        /// </summary>
        private bool CheckForClosingDistance(Vector2 endPoint) => Vector2.Distance(transform.position, endPoint) < 0.001f;

        /// <summary>
        /// Move the bullet to the target endPoint
        /// </summary>
        /// <param name="endPoint">Target endpoint</param>
        private void MoveTowards(Vector2 endPoint) => transform.position = Vector2.MoveTowards(transform.position, endPoint, m_speed * Time.deltaTime);

        /// <summary>
        /// Returns true if the bullet has the minimum ricochet angle
        /// </summary>
        /// <param name="normal">The normal of the suface it collides</param>
        /// <param name="direction">The direction of the bullet</param>
        private bool HasMinimumRicochetAngle(Vector2 normal, Vector2 direction) => (180 - Vector2.Angle(normal, direction)) > m_minimumAngleFromNormal;

        /// <summary>
        /// Returns a new rotation after ricochet
        /// </summary>
        /// <param name="direction">The target direction</param>
        /// <returns></returns>
        private Quaternion GetNewRicochetRotation(Vector2 direction) => Quaternion.Euler(0, 0, (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));

        /// <summary>
        /// Update to the next point if it exists
        /// </summary>
        private void UpdateBulletRotation()
        {
            if ((m_ricochetCounter + 1) < m_hitResults.Count)
                m_upcomingHit = m_hitResults[m_ricochetCounter + 1];
            transform.rotation = m_ricochetPointRotations[m_ricochetCounter];
        }

        /// <summary>
        /// Method to stop the bullet at any point
        /// </summary>
        /// <param name="shouldDeactivate">Assign true if you want to deactivate the bullet</param>
        private void StopBullet(bool shouldDeactivate = false)
        {
            if (shouldDeactivate)
                DeactivateBullet();
            else
                m_rb.velocity = Vector3.zero;
        }

        /// <summary>
        /// Method to deactivate the bullet
        /// </summary>
        private void DeactivateBullet()
        {
            m_rb.velocity = Vector3.zero;
            m_canDetect = false;
            gameObject.SetActive(false);
        }
    }
}