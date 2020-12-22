using Knockback.Handlers;
using Knockback.Scriptable;
using Knockback.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace Knockback.Core
{
    public class KB_GunCore : MonoBehaviour, IUsableEntity
    {
        [Header("Gun core backend settings")]
        [Space]

        //*** Backend variables ***//
        [SerializeField]
        private KB_GunData _gunSettings = null;

        //*** Reference variables ***//

        public bool canUse { get; set; }
        public KB_GunData gunSettings { get; private set; }

        //*** Cached varaiable ***//

        private Shooter shooter;


        //*** Default function ***//

        private void Awake()
        {
            shooter = new Shooter(this, _gunSettings);
            gunSettings = _gunSettings;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        //*** Public functions ***//

        /// <summary>
        /// Method to use this weapon
        /// </summary>
        /// <param name="source">Owner off this item</param>
        public void UseItem(GameObject source)
        {
            if (!canUse)
                return;
            shooter.Shoot(source);
        }

        /// <summary>
        /// Returns the total and live rounds of this gun
        /// </summary>
        public int[] GetLiveRoundsData()
        {
            return shooter.RoundData();
        }

        /// <summary>
        /// Call this method to merge two guns if they are same
        /// </summary>
        /// <param name="targetItem">Target gun to be merged</param>
        public void TryGunMerge(GameObject targetItem)
        {
            if (!IsCompatible(targetItem))
                return;
            KB_GunCore gun = targetItem.GetComponent<KB_GunCore>();
            shooter.UpdateRoundsExternally(gun.GetLiveRoundsData()[0] + gun.GetLiveRoundsData()[1]);
            Destroy(targetItem);
        }

        /// <summary>
        /// Call this method to add ammo to this gun
        /// </summary>
        /// <param name="ammoCount">The amount of ammo you want to add</param>
        /// <param name="gunClass">The class of gun</param>
        public void TryAddAmmo(int ammoCount, string gunClass)
        {
            if (IsCompatible(gunClass))
                shooter.UpdateRoundsExternally(ammoCount);
        }

        //*** Private functions ***//

        private bool IsCompatible(GameObject targetItem)
        {
            KB_GunCore gun;
            if (!targetItem.TryGetComponent(out gun))
                return false;
            if (gun.gunSettings.gunClass == _gunSettings.gunClass)
            {
                if (gun.gunSettings.gunId == _gunSettings.gunId)
                    return true;
                return false;
            }
            return false;
        }

        private bool IsCompatible(string gunClass)
        {
            if (_gunSettings.gunClass == gunClass)
                return true;
            return false;
        }


        //*** Internal class ***//

        internal class Shooter
        {
            //*** Backend ***//

            private KB_GunCore gunCore = null;
            private Transform projectileTransform = null;
            private Transform gunTransform = null;
            private float projectileVelocity = 0;
            private float impactDamage = 0;
            private float firingCooldown = 0;
            private float reloadTime = 0;
            private int roundCapacity = 0;
            private int totalRounds = 0;
            private int activeRounds = 0;
            private float gunRecoil = 0;
            private float cameraShakeIntensity = 0;
            private int poolSize = 0;

            //*** Other variables ***//

            private bool canFire = true;
            private bool isEmpty = false;
            private bool isReloadComplete = true;
            private bool isReloading = false;
            private GameObject source = null;
            private string poolName = "BulletPool_";


            public Shooter
                (
                    KB_GunCore gunCore,
                    KB_GunData gunData
                )
            {
                this.gunCore = gunCore;
                projectileTransform = gunCore.transform.GetChild(0);
                gunTransform = gunCore.transform;
                projectileVelocity = gunData.projectileVelocity;
                impactDamage = gunData.impactDamage;
                firingCooldown = 60 / gunData.bulletRoundsPerMinute;
                reloadTime = gunData.reloadTime;
                roundCapacity = gunData.roundCapacity;
                totalRounds = gunData.totalRounds;
                activeRounds = roundCapacity;
                gunRecoil = gunData.gunRecoil;
                cameraShakeIntensity = gunData.cameraShakeIntensity;
                poolSize = gunData.bulletPoolSize;
                //CreateBulletPool(gunData.bulletPrefab);
            }

            ~Shooter()
            {
                DestroyPool();
            }

            //*** Public methods ***//

            /// <summary>
            /// Call this function to shoot the gun
            /// </summary>
            /// <param name="source"></param>
            public void Shoot(GameObject source)
            {
                if (!canFire)
                    return;
                if (this.source != source || this.source == null)
                    this.source = source;
                ShootOnce();
            }

            /// <summary>
            /// Method to add additional ammo into the gun
            /// </summary>
            /// <param name="additionalRounds"></param>
            /// <returns></returns>
            public bool UpdateRoundsExternally(int additionalRounds)
            {
                if (!(totalRounds < 256))
                    return false;
                totalRounds = Mathf.Clamp(totalRounds + additionalRounds, 0, 256);
                return true;
            }

            /// <summary>
            /// Returns the total and live rounds
            /// </summary>
            /// <returns></returns>
            public int[] RoundData()
            {
                return new int[] { activeRounds, totalRounds };
            }

            //*** Private methods ***//

            private void ShootOnce()
            {
                if (ShouldReload())
                    ReloadGun();
                else
                {
                    ShootingCooldown();
                    --activeRounds;
                    SpawnBulletFromPool(projectileTransform, projectileVelocity, impactDamage);
                    AddPlayerKnockback();
                    ShakeCamera();
                }
            }

            private bool ShouldReload() { return !(isReloadComplete && activeRounds > 0); }

            private void ReloadGun()
            {
                if (isEmpty)
                {
                    if (activeRounds == 0)
                        DestroyPool();
                    return;
                }
                if (!(totalRounds > 0))
                {
                    isEmpty = true;
                    return;
                }
                if (!isReloading)
                    gunCore.StartCoroutine(DoReload());
            }

            private void ShootingCooldown() { gunCore.StartCoroutine(Cooldown()); }

            private void StopReload() { gunCore.StopCoroutine(DoReload()); }

            private void AddPlayerKnockback()
            {
                if (source == null)
                    return;
                source.GetComponent<KB_PlayerKnockbackCore>().CauseKnockback(gunRecoil, -(projectileTransform.rotation * Vector2.right));
            }

            private void ShakeCamera()
            {
                try
                {
                    KB_PlayerHandler.instance.localCameraController.ShakeCameraWithMagnitude(cameraShakeIntensity);
                }
                catch (Exception)
                {
                    new KBLog("Player handler not found!!", 1);
                    return;
                }
            }            

            private void CreateBulletPool(GameObject bulletPrefab)
            {
                KB_PoolHandler.instance.CreatePool((poolName + gunCore.GetInstanceID()), bulletPrefab, poolSize);
            }

            private void DestroyPool()
            {
                KB_PoolHandler.instance.DestroyPool(poolName + gunCore.GetInstanceID());
            }

            private void SpawnBulletFromPool(Transform projectileTransform, float velocity, float impactDamage)
            {
                GameObject bulletInstance = KB_PoolHandler.instance.GetFromPool(poolName);
                bulletInstance.transform.position = projectileTransform.position;
                bulletInstance.transform.rotation = projectileTransform.rotation;
                bulletInstance.GetComponent<KB_BulletCore>().SetBulletParameters(impactDamage, projectileTransform.rotation * Vector2.right, velocity);
                bulletInstance.GetComponent<IUsableEntity>().UseItem(gunCore.gameObject);
            }

            private IEnumerator Cooldown()
            {
                canFire = false;
                yield return new WaitForSecondsRealtime(firingCooldown);
                canFire = true;
            }

            private IEnumerator DoReload()
            {
                isReloading = true;
                int usedClipCount = roundCapacity - activeRounds;

                if (usedClipCount == 0)
                {
                    StopReload();
                    isReloading = false;
                }
                else if (roundCapacity > totalRounds)
                {
                    canFire = isReloadComplete = false;
                    yield return new WaitForSecondsRealtime(reloadTime);
                    activeRounds = totalRounds;
                    totalRounds = 0;
                    canFire = isReloadComplete = true;
                    isReloading = false;
                }
                else
                {
                    canFire = isReloadComplete = false;
                    yield return new WaitForSeconds(reloadTime);
                    activeRounds = roundCapacity;
                    totalRounds = totalRounds - usedClipCount;
                    canFire = isReloadComplete = true;
                    isReloading = false;
                }
            }            
        }
    }
}
