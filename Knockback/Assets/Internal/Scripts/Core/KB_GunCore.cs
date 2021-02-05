using Knockback.Controllers;
using Knockback.Handlers;
using Knockback.Helpers;
using Knockback.Scriptables;
using Knockback.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace Knockback.Core
{
    public class KB_GunCore : MonoBehaviour, IUsableEntity
    {

        //** --INTERNAL CLASS--

        public class GunData
        {
            public int im_totalRounds;
            public int im_activeRounds;
            public int im_roundCapacity;
            public readonly string im_gunClass;
            public readonly int im_gunId;


            public GunData() { }
            public GunData(string gunClass, int gunId)
            {
                im_gunClass = gunClass;
                im_gunId = gunId;
            }
        }

        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Gun core backend settings")]
        [SerializeField] private string m_gunSettingsResourceFetchString;
        [SerializeField] private string m_bulletPrefabResourceFetchString;
        [SerializeField] private KB_GunBackendData m_gunSettings = null;
        [SerializeField] private GameObject m_bulletPrefab = null;
        [SerializeField] private Transform m_projectileTransform = null;
        [SerializeField] private int m_poolSize = 0;
        [SerializeField] private string m_poolName = "BulletPool_";

        //** --PRIVATE ATTRIBUTES--

        private float m_projectileVelocity = 0;
        private float m_impactDamage = 0;
        private float m_firingCooldown = 0;
        private float m_reloadTime = 0;
        private float m_gunRecoil = 0;
        private float m_cameraShakeIntensity = 0;
        private bool m_canFire = true;
        private bool m_isEmpty = false;
        private bool m_isReloadComplete = true;
        private bool m_isReloading = false;
        private const int _MAXIMUM_BULLETS = 269;
        private GameObject m_user = null;
        private GunData m_gunData = null;
        private KB_CameraController m_cameraController = null;

        //** --PRIVATE REFERENCES--

        private Vector2 m_firingDirection => m_projectileTransform.rotation * Vector2.right;

        //** --PUBLIC REFERENCES--

        public KB_GunBackendData gunSettings { get { return m_gunSettings; } }
        public bool i_canUse { get; set; }
        public GunData gunData { get { return m_gunData; } }


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Method to use this weapon
        /// </summary>
        /// <param name="user">Owner off this item</param>
        public void UseItem(GameObject user)
        {
            if (!i_canUse)
                return;
            if (this.m_user == null || this.m_user != user)
                this.m_user = user;

            FireGun();
        }

        /// <summary>
        /// Call this method to merge two guns if they are same
        /// </summary>
        /// <param name="targetItem">Target gun to be merged</param>
        public void TryItemMerge(GameObject targetItem)
        {
            if (!CanMerge(targetItem))
                return;
            KB_GunCore gun = targetItem.GetComponent<KB_GunCore>();
            int additionalRounds = gun.gunData.im_totalRounds + gun.gunData.im_activeRounds;
            AddAmmo(additionalRounds);
            Destroy(targetItem);
        }

        /// <summary>
        /// Call this method to add ammo to this gun
        /// </summary>
        /// <param name="ammoCount">The amount of ammo you want to add</param>
        /// <param name="gunClass">The class of gun</param>
        public void TryAddAmmo(int ammoCount, string gunClass)
        {
            if (CanAddAmmo(gunClass))
                AddAmmo(ammoCount);
        }

        //** --PRIVATE METHODS--

        /// <summary>
        /// Initiate bootstrap
        /// </summary>
        private void Awake() => Bootstrap();

        /// <summary>
        /// Do this on disable
        /// </summary>
        private void OnDisable() => StopAllCoroutines();

        /// <summary>
        /// Do this on destroy
        /// </summary>
        private void OnDestroy() => DestroyPool();

        /// <summary>
        /// Bootstrapping and initialization
        /// </summary>
        private void Bootstrap()
        {
            m_gunData = new GunData(m_gunSettings.gunClass, m_gunSettings.gunId);
            m_gunData.im_roundCapacity = m_gunSettings.roundCapacity;
            m_gunData.im_totalRounds = m_gunSettings.totalRounds;
            m_gunData.im_activeRounds = m_gunSettings.roundCapacity;

            m_projectileVelocity = m_gunSettings.projectileVelocity;
            m_impactDamage = m_gunSettings.impactDamage;
            m_firingCooldown = 60 / m_gunSettings.bulletRoundsPerMinute;
            m_reloadTime = m_gunSettings.reloadTime;
            m_gunRecoil = m_gunSettings.gunRecoil;
            m_cameraShakeIntensity = m_gunSettings.cameraShakeIntensity;

            CreateBulletPool();

            KB_ReferenceHandler.GetReference("MainCameraController", out m_cameraController);
        }

        /// <summary>
        /// Method to fire the gun
        /// </summary>
        private void FireGun()
        {
            if (!m_canFire)
                return;

            if (ShouldReload())
                ReloadGun();
            else
            {
                ShootingCooldown();
                --m_gunData.im_activeRounds;
                SpawnBulletFromPool(m_projectileTransform, m_firingDirection, m_projectileVelocity, m_impactDamage);

                //ApplyPlayerKnockback();
                //ShakeCamera();
            }
        }

        /// <summary>
        /// Returns true if the gun should reload
        /// </summary>
        /// <returns></returns>
        private bool ShouldReload() => (!m_isReloadComplete) && (m_gunData.im_activeRounds <= 0);

        /// <summary>
        /// Method to reload the gun
        /// </summary>
        private void ReloadGun()
        {
            if (m_isEmpty)
            {
                if (m_gunData.im_activeRounds == 0)
                    DestroyPool();
                return;
            }
            if (!(m_gunData.im_totalRounds > 0))
            {
                m_isEmpty = true;
                return;
            }
            if (!m_isReloading)
                StartCoroutine(DoReload());
        }

        /// <summary>
        /// Shooting cooldown buffer
        /// </summary>
        private void ShootingCooldown() => StartCoroutine(Cooldown());

        /// <summary>
        /// Reload delay buffer
        /// </summary>
        private void StopReload() => StopCoroutine(DoReload());

        /// <summary>
        /// Method to spawn the bullet from the pool
        /// </summary>
        /// <param name="projectileTransform">Transform for the projectile</param>
        /// <param name="direction">Direction of the bullet</param>
        /// <param name="velocity">Velocity of the bullet</param>
        /// <param name="impactDamage">Impact damage of the bullet</param>
        private void SpawnBulletFromPool(Transform projectileTransform, Vector2 direction, float velocity, float impactDamage)
        {
            GameObject bulletInstance = KB_PoolHandler.instance.GetFromPool(m_poolName);
            bulletInstance.transform.CopyPositionAndRotation(projectileTransform);

            bulletInstance.GetComponent<KB_BulletCore>().SetBulletParameters(impactDamage, direction, velocity);
            bulletInstance.GetComponent<IUsableEntity>().UseItem(m_user);
        }

        /// <summary>
        /// Method to apply player knockback
        /// </summary>
        private void ApplyPlayerKnockback() => m_user.GetComponent<KB_PlayerController>()?.knockbackHandler.CauseKnockback(m_gunRecoil, -m_firingDirection);

        /// <summary>
        /// Method to shake the camera
        /// </summary>
        private void ShakeCamera()
        {
            if (m_cameraController == null)
            {
                if (KB_ReferenceHandler.GetReference("MainCameraController", out m_cameraController))
                {
                    m_cameraController.ShakeCameraWithMagnitude(m_cameraShakeIntensity);
                    return;
                }
            }
            m_cameraController?.ShakeCameraWithMagnitude(m_cameraShakeIntensity);
        }

        /// <summary>
        /// Method to create the bullet pool
        /// </summary>
        private void CreateBulletPool()
        {
            if (m_bulletPrefab == null)
            {
                Debug.LogError("BULLET PREFAB IS NULL");
                return;
            }
            if (KB_PoolHandler.instance.DoesPoolExist(m_poolName + m_bulletPrefab.name))
                return;

            KB_PoolHandler.instance.CreatePool((m_poolName + m_bulletPrefab.name), m_bulletPrefab, m_poolSize);
        }

        /// <summary>
        /// Method to destroy the bullet pool
        /// </summary>
        private void DestroyPool() => KB_PoolHandler.instance.DestroyPool(m_poolName + m_bulletPrefab.name);

        /// <summary>
        /// Cool down coroutine
        /// </summary>
        private IEnumerator Cooldown()
        {
            m_canFire = false;
            yield return new WaitForSecondsRealtime(m_firingCooldown);
            m_canFire = true;
        }

        /// <summary>
        /// Reloading coroutine
        /// </summary>
        private IEnumerator DoReload()
        {
            m_isReloading = true;
            int usedClipCount = gunData.im_roundCapacity - gunData.im_activeRounds;

            if (usedClipCount == 0)
            {
                StopReload();
                m_isReloading = false;
            }
            else if (gunData.im_roundCapacity > gunData.im_totalRounds)
            {
                m_canFire = m_isReloadComplete = false;
                yield return new WaitForSecondsRealtime(m_reloadTime);
                gunData.im_activeRounds = gunData.im_totalRounds;
                gunData.im_totalRounds = 0;
                m_canFire = m_isReloadComplete = true;
                m_isReloading = false;
            }
            else
            {
                m_canFire = m_isReloadComplete = false;
                yield return new WaitForSeconds(m_reloadTime);
                gunData.im_activeRounds = gunData.im_roundCapacity;
                gunData.im_totalRounds = gunData.im_totalRounds - usedClipCount;
                m_canFire = m_isReloadComplete = true;
                m_isReloading = false;
            }
        }

        /// <summary>
        /// Returns true if the guns can be merged together
        /// </summary>
        /// <param name="targetItem">The target gun to be merged</param>
        /// <returns></returns>
        private bool CanMerge(GameObject targetItem)
        {
            KB_GunCore gun;
            if (!targetItem.TryGetComponent(out gun))
                return false;
            if (gun.gunSettings.gunClass == m_gunSettings.gunClass)
                if (gun.gunSettings.gunId == m_gunSettings.gunId)
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true ammoClass and gunClass matches
        /// </summary>
        /// <param name="ammoClass">Target gunClass to be matched</param>
        /// <returns></returns>
        private bool CanAddAmmo(string ammoClass) => m_gunSettings.gunClass == ammoClass;

        /// <summary>
        /// Call this method to update the rounds externally
        /// </summary>
        /// <param name="additionalRounds">Total additional rounds</param>
        private void AddAmmo(int additionalRounds)
        {
            if (m_gunData.im_totalRounds >= _MAXIMUM_BULLETS)
                return;
            m_gunSettings.totalRounds = Mathf.Clamp(m_gunSettings.totalRounds + additionalRounds, 0, _MAXIMUM_BULLETS);
            return;
        }
    }
}
