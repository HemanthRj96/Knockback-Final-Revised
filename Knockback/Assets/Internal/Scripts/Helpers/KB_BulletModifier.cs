using Knockback.Core;
using Knockback.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Knockback.Helpers
{
    /// <summary>
    /// This class implements bullet dynamics such as passthrough and ricochet and more if desired
    /// </summary>
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
        public int m_ricochetCounter = 0;
        public int m_totalRicochetCounter = 0;
        private Rigidbody2D m_rb = null;
        private KB_BulletCore bulletCore = null;
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

            if (m_shouldRicochet)
            {
                CreatePredictionPath();
                m_updateThroughPathpoints = true;
            }
            else
                MoveWithRigidbody(m_bulletDirection);
        }

        /// <summary>
        /// Call this method when the bullet hits something
        /// </summary>
        /// <param name="hit">Target hit parameter</param>
        public bool OnHit(RaycastHit2D hit)
        {
            bool hasDamage = TryApplyingDamage(hit);

            if (hasDamage)
            {
                if (!m_shouldPassthrough)
                    StopBullet(true);
            }
            else if (m_shouldPassthrough)
            {
                return true;
            }
            else if(!CanRicochet())
            {
                StopBullet(true);
            }
            else if (m_shouldRicochet)
            {
                TryBulletRicochet(hit);
            }
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
        private void Awake()
        {
            m_rb = GetComponent<Rigidbody2D>();
            bulletCore = GetComponent<KB_BulletCore>();
        }

        /// <summary>
        /// FixedUpdate where the bullet will move according to the pathpoints if updateThroughPathPoints is set as true
        /// </summary>
        private void FixedUpdate()
        {
            if (m_updateThroughPathpoints)
                TranslateThroughPathPoints();
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
                    ApplyNewRotation();
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
        /// Method to apply passthrough and ricochet ability to the bullet
        /// </summary>
        /// <param name="hit">Hit parameter</param>
        /// <param name="damageHandler">Damage handler</param>
        private void TryBulletRicochet(RaycastHit2D hit)
        {
            if (CheckIfTheHitExists(hit))
                return;
            UpdatePredictionPath(hit);
        }

        /// <summary>
        /// Create a new path for the bullet to go
        /// </summary>
        private void CreatePredictionPath()
        {
            Vector2 startPosition = transform.position;
            Vector2 direction = transform.rotation * Vector2.right;
            RaycastHit2D hit;
            int index = 0;

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
                        if (index == 0)
                            m_upcomingHit = hit;

                        ++index;
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
        /// Method to update the current path to new one
        /// </summary>
        /// <param name="hit"></param>
        private void UpdatePredictionPath(RaycastHit2D hit)
        {
            m_updateThroughPathpoints = false;
            transform.position = hit.point - (m_bulletDirection * 0.25f);
            TruncateRicochetList(m_ricochetCounter - 1);
            m_totalRicochetCounter = m_ricochetCounter;
            CreatePredictionPath();
        }

        /// <summary>
        /// Returns true if the ricochetCounter has reached the maximum
        /// </summary>
        private bool CanRicochet() => m_ricochetCounter < m_maximumRicochetLimit;

        /// <summary>
        /// Truncate the elements after the index
        /// </summary>
        /// <param name="index"></param>
        private void TruncateRicochetList(int index)
        {
            if (index < 0)
            {
                m_ricochetHitPoints.Clear();
                m_ricochetPointRotations.Clear();
                m_hitResults.Clear();
            }
            else
            {
                m_ricochetHitPoints.RemoveRange(index, (m_ricochetHitPoints.Count - 1) - index);
                m_ricochetPointRotations.RemoveRange(index, (m_ricochetPointRotations.Count - 1) - index);
                m_hitResults.RemoveRange(index, (m_hitResults.Count - 1) - index);
            }
        }

        /// <summary>
        /// Applies new rotation to the bullet
        /// </summary>
        private void ApplyNewRotation()
        {
            if ((m_ricochetCounter + 1) < m_hitResults.Count)
                m_upcomingHit = m_hitResults[m_ricochetCounter + 1];
            transform.rotation = m_ricochetPointRotations[m_ricochetCounter];
        }

        /// <summary>
        /// Method to move bullet using rigidBody
        /// </summary>
        private void MoveWithRigidbody(Vector2 direction)
        {
            if (m_rb.velocity.magnitude < m_speed)
                m_rb.velocity = m_speed * direction;
        }

        /// <summary>
        /// Move the bullet to the target endPoint
        /// </summary>
        /// <param name="endPoint">Target endpoint</param>
        private void MoveTowards(Vector2 endPoint) => transform.position = Vector2.MoveTowards(transform.position, endPoint, m_speed * Time.deltaTime);

        /// <summary>
        /// Method to apply damage to the damage handler
        /// </summary>
        /// <param name="damageHandler"></param>
        private bool TryApplyingDamage(RaycastHit2D hit)
        {
            IDamage damageHandler;
            if (hit.collider.TryGetComponent(out damageHandler))
            {
                damageHandler?.ApplyDamage(m_impactDamage, bulletCore.m_source);
                return true;
            }
            else return false;
        }

        /// <summary>
        /// Returns true if the target reaches the end point
        /// </summary>
        private bool CheckForClosingDistance(Vector2 endPoint) => Vector2.Distance(transform.position, endPoint) < 0.001f;

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
        /// Method to stop the bullet at any point
        /// </summary>
        /// <param name="shouldDeactivate">Assign true if you want to deactivate the bullet</param>
        private void StopBullet(bool shouldDeactivate = false)
        {
            if (shouldDeactivate)
                DeactivateBullet();
            else
            {
                m_updateThroughPathpoints = false;
                m_rb.velocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Method to deactivate the bullet
        /// </summary>
        private void DeactivateBullet()
        {
            m_updateThroughPathpoints = false;
            m_canDetect = false;
            m_rb.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }
    }
}