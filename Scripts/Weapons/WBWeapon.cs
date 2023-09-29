using Unity.VisualScripting;
using UnityEngine;

namespace WeirdBrothers.ThirdPersonController
{
    public enum WeaponSlot
    {
        None,
        First,
        Second,
        Meele,
        Pistol
    }

    public class WBWeapon : MonoBehaviour, IItemImage, IItemName
    {
        [Header("Weawpon Data")]
        [Space]
        public WBWeaponData Data;

        public WeaponSlot WeaponSlot;

        [Space]
        public Transform LeftHandRef;

        [SerializeField]
        private Transform _shellSpawnPoint;

        [SerializeField]
        private ParticleSystem _muzzelFlash;

        private int _currentAmmo;
        public int CurrentAmmo => _currentAmmo;

        private AudioSource _audioSource;
        private float _nextFire;
        private int _index;
        private Vector3 _directionToTarget;
        private RaycastHit _hit;
        private int _damage;

        [SerializeField] GameObject _miniMapIcon;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _currentAmmo = Data.MagSize;
            _damage = (int)Data.Damage;
            _directionToTarget = Vector3.zero;
            _hit = new RaycastHit();
        }

        private void Update()
        {
            if (_miniMapIcon != null)
            {
                if (WeaponSlot != 0)
                {
                    _miniMapIcon.SetActive(false);
                }
                else
                {
                    _miniMapIcon.SetActive(true);
                }
            }

        }

        public void RemoveRigidBody()
        {
            Destroy(GetComponent<Rigidbody>());
        }

        public Sprite GetItemImage()
        {
            return Data.WeaponImage;
        }

        public string GetItemName()
        {
            return Data.WeaponName;
        }

        public void MeeleAttack()
        {
            _audioSource.PlayOneShotAudioClip(Data.FireSound);
        }

        public void Fire(Vector3 hitPoint, LayerMask DamageLayer)
        {
            if (Time.time > _nextFire)
            {
                if (_currentAmmo > 0)
                {
                    _nextFire = Time.time + Data.FireRate;
                    _currentAmmo--;
                    _audioSource.PlayOneShotAudioClip(Data.FireSound);
                    OnCaseOut();
                    _muzzelFlash.Play();

                    if (hitPoint != Vector3.zero)
                    {
                        _directionToTarget = hitPoint - _muzzelFlash.transform.position;
                        if (Physics.Raycast(_muzzelFlash.transform.position,
                            CalculatelateSpread(Data.WeaponSpread, _directionToTarget), out _hit,
                            Data.Range, DamageLayer))
                        {
                            OnTargetHit(_hit);
                        }
                    }
                }
            }
        }

        private void OnTargetHit(RaycastHit hit)
        {
            if (hit.transform.gameObject.tag == "Enemy")
            {
                print("Hit");
                //
                GameObject bulletHole = Instantiate(FindObjectOfType<BloodControl>().bloodEffect, hit.point + (hit.normal * 0.01f), Quaternion.FromToRotation(Vector3.forward, hit.normal));
                bulletHole.transform.SetParent(hit.transform);
                Destroy(bulletHole, 10);
                hit.transform.gameObject.GetComponent<ZombieDamageController>().Hit(_damage);
                //
            }
            else
            {
                print("Hit Ground");
                GameObject bulletHole = Instantiate(Data.BulletHole, hit.point + (hit.normal * 0.01f), Quaternion.FromToRotation(-Vector3.forward, hit.normal));
                bulletHole.transform.SetParent(hit.transform);
                Destroy(bulletHole, 10);
            }
        }

        private Vector3 CalculatelateSpread(float inaccuracy, Vector3 direction)
        {
            if (inaccuracy == 0)
                return direction;
            direction.x += Random.Range(-inaccuracy, inaccuracy);
            direction.y += Random.Range(-inaccuracy, inaccuracy);
            direction.z += Random.Range(-inaccuracy, inaccuracy);
            return direction;
        }

        public void AddAmmo(int ammount)
        {
            _currentAmmo += ammount;
        }

        public void MagIn()
        {
            _audioSource.PlayOneShotAudioClip(Data.MagInSound);
        }

        public void MagOut()
        {
            _audioSource.PlayOneShotAudioClip(Data.MagOutSound);
        }

        public void Bolt()
        {
            _audioSource.PlayOneShotAudioClip(Data.BoltSound);
        }

        private void OnCaseOut()
        {
            GameObject ejectedCase = Instantiate(Data.BulletShell, _shellSpawnPoint.position, _shellSpawnPoint.rotation);
            Rigidbody caseRigidbody = ejectedCase.GetComponent<Rigidbody>();
            caseRigidbody.velocity = _shellSpawnPoint.TransformDirection(-Vector3.left * Data.BulletEjectingSpeed);
            caseRigidbody.AddTorque(Random.Range(-0.5f, 0.5f), Random.Range(0.2f, 0.3f), Random.Range(-0.5f, 0.5f));
            caseRigidbody.AddForce(0, Random.Range(2f, 4f), 0, ForceMode.Impulse);
            Destroy(ejectedCase, 5f);
        }
    }
}