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
            public int totalRounds;
            public int activeRounds;
            public int roundCapacity;
            public readonly string gunClass;
            public readonly int gunId;


            public GunData() { }
            public GunData(string gunClass, int gunId)
            {
                this.gunClass = gunClass;
                this.gunId = gunId;
            }
        }

        //** --ATTRIBUTES--
        //** --SERIALIZED ATTRIBUTES--

        [Header("Gun core backend settings")]
        [SerializeField] private string gunSettingsResourceFetchString;
        [SerializeField] private string bulletPrefabResourceFetchString;
        [SerializeField] private KB_GunBackendData _gunSettings = null;
        [SerializeField] private GameObject bulletPrefab = null;
        [SerializeField] private Transform projectileTransform = null;
        [SerializeField] private int poolSize = 0;
        [SerializeField] private string poolName = "BulletPool_";

        //** --PRIVATE ATTRIBUTES--

        private float projectileVelocity = 0;
        private float impactDamage = 0;
        private float firingCooldown = 0;
        private float reloadTime = 0;
        private float gunRecoil = 0;
        private float cameraShakeIntensity = 0;
        private bool canFire = true;
        private bool isEmpty = false;
        private bool isReloadComplete = true;
        private bool isReloading = false;
        private const int _MAXIMUM_BULLETS = 269;
        private GameObject user = null;
        private GunData _gunData = null;
        private KB_CameraController cameraController = null;

        //** --PRIVATE REFERENCES--

        private Vector2 firingDirection => projectileTransform.rotation * Vector2.right;

        //** --PUBLIC REFERENCES--

        public KB_GunBackendData gunSettings { get { return _gunSettings; } }
        public bool canUse { get; set; }
        public GunData gunData { get { return _gunData; } }


        //** --METHODS--
        //** --PUBLIC METHODS--

        /// <summary>
        /// Method to use this weapon
        /// </summary>
        /// <param name="user">Owner off this item</param>
        public void UseItem(GameObject user)
        {
            if (!canUse)
                return;
            if (this.user == null || this.user != user)
                this.user = user;

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
            int additionalRounds = gun.gunData.totalRounds + gun.gunData.activeRounds;
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
            _gunData = new GunData(_gunSettings.gunClass, _gunSettings.gunId);
            _gunData.roundCapacity = _gunSettings.roundCapacity;
            _gunData.totalRounds = _gunSettings.totalRounds;
            _gunData.activeRounds = _gunSettings.roundCapacity;

            projectileVelocity = _gunSettings.projectileVelocity;
            impactDamage = _gunSettings.impactDamage;
            firingCooldown = 60 / _gunSettings.bulletRoundsPerMinute;
            reloadTime = _gunSettings.reloadTime;
            gunRecoil = _gunSettings.gunRecoil;
            cameraShakeIntensity = _gunSettings.cameraShakeIntensity;

            CreateBulletPool();

            KB_ReferenceHandler.GetReference("MainCameraController", out cameraController);
        }

        /// <summary>
        /// Method to fire the gun
        /// </summary>
        private void FireGun()
        {
            if (!canFire)
                return;

            if (ShouldReload())
                ReloadGun();
            else
            {
                ShootingCooldown();
                --_gunData.activeRounds;
                SpawnBulletFromPool(projectileTransform, firingDirection, projectileVelocity, impactDamage);

                //ApplyPlayerKnockback();
                //ShakeCamera();
            }
        }

        /// <summary>
        /// Returns true if the gun should reload
        /// </summary>
        /// <returns></returns>
        private bool ShouldReload() => (!isReloadComplete) && (_gunData.activeRounds <= 0);

        /// <summary>
        /// Method to reload the gun
        /// </summary>
        private void ReloadGun()
        {
            if (isEmpty)
            {
                if (_gunData.activeRounds == 0)
                    DestroyPool();
                return;
            }
            if (!(_gunData.totalRounds > 0))
            {
                isEmpty = true;
                return;
            }
            if (!isReloading)
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
            GameObject bulletInstance = KB_PoolHandler.instance.GetFromPool(poolName);
            bulletInstance.transform.CopyPositionAndRotation(projectileTransform);

            bulletInstance.GetComponent<KB_BulletCore>().SetBulletParameters(impactDamage, direction, velocity);
            bulletInstance.GetComponent<IUsableEntity>().UseItem(user);
        }

        /// <summary>
        /// Method to apply player knockback
        /// </summary>
        private void ApplyPlayerKnockback() => user.GetComponent<KB_PlayerController>().knockbackHandler.CauseKnockback(gunRecoil, -firingDirection);

        /// <summary>
        /// Method to shake the camera
        /// </summary>
        private void ShakeCamera()
        {
            if (cameraController == null)
            {
                if (KB_ReferenceHandler.GetReference("MainCameraController", out cameraController))
                {
                    cameraController.ShakeCameraWithMagnitude(cameraShakeIntensity);
                    return;
                }
            }
            cameraController?.ShakeCameraWithMagnitude(cameraShakeIntensity);
        }

        /// <summary>
        /// Method to create the bullet pool
        /// </summary>
        private void CreateBulletPool()
        {
            if (bulletPrefab == null)
            {
                Debug.LogError("BULLET PREFAB IS NULL");
                return;
            }
            if (KB_PoolHandler.instance.DoesPoolExist(poolName + bulletPrefab.name))
                return;

            KB_PoolHandler.instance.CreatePool((poolName + bulletPrefab.name), bulletPrefab, poolSize);
        }

        /// <summary>
        /// Method to destroy the bullet pool
        /// </summary>
        private void DestroyPool() => KB_PoolHandler.instance.DestroyPool(poolName + bulletPrefab.name);

        /// <summary>
        /// Cool down coroutine
        /// </summary>
        private IEnumerator Cooldown()
        {
            canFire = false;
            yield return new WaitForSecondsRealtime(firingCooldown);
            canFire = true;
        }

        /// <summary>
        /// Reloading coroutine
        /// </summary>
        private IEnumerator DoReload()
        {
            isReloading = true;
            int usedClipCount = gunData.roundCapacity - gunData.activeRounds;

            if (usedClipCount == 0)
            {
                StopReload();
                isReloading = false;
            }
            else if (gunData.roundCapacity > gunData.totalRounds)
            {
                canFire = isReloadComplete = false;
                yield return new WaitForSecondsRealtime(reloadTime);
                gunData.activeRounds = gunData.totalRounds;
                gunData.totalRounds = 0;
                canFire = isReloadComplete = true;
                isReloading = false;
            }
            else
            {
                canFire = isReloadComplete = false;
                yield return new WaitForSeconds(reloadTime);
                gunData.activeRounds = gunData.roundCapacity;
                gunData.totalRounds = gunData.totalRounds - usedClipCount;
                canFire = isReloadComplete = true;
                isReloading = false;
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
            if (gun.gunSettings.gunClass == _gunSettings.gunClass)
                if (gun.gunSettings.gunId == _gunSettings.gunId)
                    return true;
            return false;
        }

        /// <summary>
        /// Returns true ammoClass and gunClass matches
        /// </summary>
        /// <param name="ammoClass">Target gunClass to be matched</param>
        /// <returns></returns>
        private bool CanAddAmmo(string ammoClass) => _gunSettings.gunClass == ammoClass;

        /// <summary>
        /// Call this method to update the rounds externally
        /// </summary>
        /// <param name="additionalRounds">Total additional rounds</param>
        private void AddAmmo(int additionalRounds)
        {
            if (_gunData.totalRounds >= _MAXIMUM_BULLETS)
                return;
            _gunSettings.totalRounds = Mathf.Clamp(_gunSettings.totalRounds + additionalRounds, 0, _MAXIMUM_BULLETS);
            return;
        }
    }
}
