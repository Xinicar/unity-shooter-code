using System;
using UnityEngine;
using Cinemachine;
using WeirdBrothers.IKHepler;
using WeirdBrothers.CharacterController;

namespace WeirdBrothers.ThirdPersonController
{
    [Serializable]
    public class WBPlayerContext
    {
        //private properties
        [SerializeField] private WBPlayerData _data;
        public WBPlayerData Data => _data;

        [SerializeField] private LookAtIK _weaponIK;
        public LookAtIK WeaponIK => _weaponIK;

        [Space]
        [SerializeField] private CrossHairSettings _crossHairSetting;
        public CrossHairSettings CrossHair => _crossHairSetting;

        [Space]
        [SerializeField] private CinemachineVirtualCamera _camera;

        private WBThirdPersonAnimator _animator;
        public WBThirdPersonAnimator Animator => _animator;

        private WBCharacterController _controller;
        public WBCharacterController Controller => _controller;

        private WBInputHandler _input;
        public WBInputHandler Input => _input;

        private WBItemPickUpManager _pickUpManager;
        public WBItemPickUpManager PickUpManager => _pickUpManager;

        private Camera _playerCamera;
        public Camera PlayerCamera => _playerCamera;

        private CinemachinePOV _pov;
        public CinemachinePOV Pov => _pov;

        private WBPlayerInventory _inventory;
        public WBPlayerInventory Inventory => _inventory;

        private Transform _transform;
        public Transform Transform => _transform;

        private WBWeaponHandler _weaponHandler;
        public WBWeaponHandler WeaponHandler => _weaponHandler;

        private WBWeaponSlots _weaponSlots;
        public WBWeaponSlots WeaponSlots => _weaponSlots;

        //public properties        
        [HideInInspector] public Transform CurrentPickUpItem;
        [HideInInspector] public WBWeapon CurrentWeapon;
        [HideInInspector] public float RecoilTime;
        [HideInInspector] public float Speed;
        
        public void SetData(Transform transform)
        {
            _animator = new WBThirdPersonAnimator(transform);
            _controller = transform.GetComponent<WBCharacterController>();
            _input = transform.GetComponent<WBInputHandler>();
            _pov = _camera.GetCinemachineComponent<CinemachinePOV>();
            _pickUpManager = new WBItemPickUpManager();
            _playerCamera = Camera.main;
            _inventory = new WBPlayerInventory();
            _transform = transform;
            _weaponHandler = new WBWeaponHandler();
            _weaponSlots = GetWeaponSlots(transform);
        }

        private WBWeaponSlots GetWeaponSlots(Transform transform)
        {
            Animator animator = transform.GetComponent<Animator>();
            var rightHandRef = animator.GetBoneTransform(HumanBodyBones.RightHand).Find("RightHandRef");
            var primarySlot1 = animator.GetBoneTransform(HumanBodyBones.Spine).Find("PrimarySlot1");
            var primarySlot2 = animator.GetBoneTransform(HumanBodyBones.Spine).Find("PrimarySlot2");
            var secondarySlot = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg).Find("SecondarySlot");
            var meleeSlot = animator.GetBoneTransform(HumanBodyBones.Spine).Find("MeleeSlot");

            WBWeaponSlots weaponSlots = new WBWeaponSlots
            {
                RightHandReference = rightHandRef,
                PrimarySlot1 = primarySlot1,
                PrimarySlot2 = primarySlot2,
                SecondarySlot = secondarySlot,
                MeleeSlot = meleeSlot
            };
            return weaponSlots;
        }

        public void SetAnimator()
        {
            if (CurrentWeapon == null)
            {
                Animator.SetWeaponIdex(0);
            }
            else
            {
                int index = CurrentWeapon.Data.WeaponIndex;
                Animator.SetWeaponIdex(index);
            }
        }

        public void SetAnimator(int index)
        {
            Animator.SetWeaponIdex(index);
        }

        public void UpdateAmmo(WBWeapon weapon)
        {
            var index = 0;
            if (weapon.Data.WeaponType == WBWeaponType.Primary)
            {
                if (weapon.WeaponSlot == WeaponSlot.First)
                {
                    index = 1;
                }
                else if (weapon.WeaponSlot == WeaponSlot.Second)
                {
                    index = 2;
                }
            }
            else if (weapon.Data.WeaponType == WBWeaponType.Secondary)
            {
                index = 3;
            }
            else if (weapon.Data.WeaponType == WBWeaponType.Melee)
            {
                index = 4;
            }

            var weaponImage = weapon.gameObject.GetItemImage();
            var currentAmmo = weapon.CurrentAmmo;
            var totalAmmo = Inventory.GetAmmo(weapon.Data.AmmoType);

            WBUIActions.SetPrimaryWeaponUI?.Invoke(index, weaponImage, currentAmmo, totalAmmo);
        }

        public void UpdateAmmo()
        {
            WBWeapon[] weapons = Transform.GetComponentsInChildren<WBWeapon>();
            Array.ForEach(weapons, weapon =>
            {
                var index = 0;
                if (weapon.Data.WeaponType == WBWeaponType.Primary)
                {
                    if (weapon.WeaponSlot == WeaponSlot.First)
                    {
                        index = 1;
                    }
                    else if (weapon.WeaponSlot == WeaponSlot.Second)
                    {
                        index = 2;
                    }
                }
                else if (weapon.Data.WeaponType == WBWeaponType.Secondary)
                {
                    index = 3;
                }
                else if (weapon.Data.WeaponType == WBWeaponType.Melee)
                {
                    index = 4;
                }

                var weaponImage = weapon.gameObject.GetItemImage();
                var currentAmmo = weapon.CurrentAmmo;
                var totalAmmo = Inventory.GetAmmo(weapon.Data.AmmoType);

                WBUIActions.SetPrimaryWeaponUI?.Invoke(index, weaponImage, currentAmmo, totalAmmo);
            });
        }

        public void GenerateRecoil(float time)
        {
            RecoilTime = time;
        }
    }
}