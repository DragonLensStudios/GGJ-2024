using System;
using System.Collections.Generic;
using Enums;
using Health;
using Messaging;
using Messaging.Messages;
using UnityEngine;
using Weapons;

namespace Player
{
    public class FPSController : FirstPersonBasicController, IDamagable
    {
        [field: Header("Player Settings")]
        [field: Tooltip("Player Damage Multiplier")]
        [field: SerializeField] public float DamageMultiplier { get; set; } = 1;
        [field:Tooltip("Player Sensibility to Self Damage")]
        [field:Range(0,1)]
        [field:SerializeField] public float SensibilityToSelfDamage { get; set; } = 0.5f;
        
        [field:Header("FPS Settings")]
        [field:Tooltip("Field of view when not aiming")]
        [field: SerializeField] public virtual float DefaultFOV { get; set; } = 60f;
        [field:Tooltip("Portion of the regular FOV to apply to the weapon camera")]
        [field: SerializeField] public virtual float WeaponFOVMultiplier { get; set; } = 1f;
        
        [field:Header("Weapons Manager")]
        [field:Tooltip("Carried Weapons")]
        [field:SerializeField] public virtual WeaponController[] WeaponSlots { get; set; } = new WeaponController[9];
        [field:Tooltip("Starting Weapons")]
        [field:SerializeField] public virtual List<WeaponController> StartingWeapons { get; set; } = new();
        [field:Tooltip("Current Weapon")]
        [field:SerializeField] public virtual WeaponController CurrentWeapon { get; set; }
        [field: Tooltip("Current Weapon Index")]
        [field: SerializeField] public virtual int ActiveWeaponIndex { get; set; } = -1;
        [field:Tooltip("Weapon Switch Delay")]
        [field:SerializeField] public virtual float WeaponSwitchDelay { get; set; } = 0.5f;
        [field:Tooltip("Weapon Switch State")]
        [field:SerializeField] public virtual WeaponSwitchState WeaponSwitchState { get; set; } = WeaponSwitchState.Down;
        
        [field:Header("Weapon Settings")]
        [field:Tooltip("Layer to set FPS weapon gameObjects to")]
        [field:SerializeField] public virtual LayerMask FpsWeaponLayer { get; set; }
        [field:Tooltip("Weapon Bob Frequency at which the weapon will move around in the screen when the player is in movement")]
        [field:SerializeField] public virtual float BobFrequency { get; set; } = 10f;
        [field:Tooltip("Weapon Bob How fast the weapon bob is applied, the bigger value the fastest")]
        [field:SerializeField] public virtual float BobSharpness { get; set; } = 10f;
        [field:Tooltip("Weapon Bob Distance the weapon bobs when not aiming")]
        [field:SerializeField] public float DefaultBobAmount { get; set; } = 0.05f;
        [field:Tooltip("Weapon Bob Distance the weapon bobs when aiming")]
        [field:SerializeField] public float AimingBobAmount { get; set; }= 0.02f;
        [field:Tooltip("Weapon Recoil This will affect how fast the recoil moves the weapon, the bigger the value, the fastest")]
        [field:SerializeField] public float RecoilSharpness { get; set; } = 50f;
        [field:Tooltip("Weapon Recoil Maximum distance the recoil can affect the weapon")]
        [field:SerializeField] public float MaxRecoilDistance { get; set; } = 0.5f;
        [field:Tooltip("Weapon Recoil How fast the weapon goes back to it's original position after the recoil is finished")]
        [field:SerializeField] public float RecoilRestitutionSharpness { get; set; } = 10f;
        [field:Tooltip("Weapon Animation Speed at which the aiming animation is played")]
        [field:SerializeField] public float AimingAnimationSpeed { get; set; } = 10f;
        
        [field:Header("References")]
        [field:Tooltip("Weapon Camera")]
        [field:SerializeField] public virtual Camera WeaponCamera { get; set; }
        [field:Tooltip("Parent transform where all weapon will be added in the hierarchy")]
        [field:SerializeField] public virtual Transform WeaponParentSocket { get; set; }
        [field:Tooltip("Position for weapons when active but not actively aiming")]
        [field:SerializeField] public virtual Transform DefaultWeaponPosition { get; set; }
        [field:Tooltip("Position for weapons when aiming")]
        [field:SerializeField] public virtual Transform AimingWeaponPosition { get; set; }
        [field:Tooltip("Position for innactive weapons")]
        [field:SerializeField] public virtual Transform DownWeaponPosition { get; set; }
        
        [field:Header("Runtime Values")]
        [field:Tooltip("Is Aiming?")]
        [field:SerializeField] public virtual bool IsAiming { get; set; }
        [field:Tooltip("Is Pointing at the target?")]
        [field:SerializeField] public virtual bool IsPointingAtTarget { get; set; }

        protected float weaponBobFactor;
        protected Vector3 lastCharacterPosition;
        protected Vector3 weaponMainLocalPosition;
        protected Vector3 weaponBobLocalPosition;
        protected Vector3 weaponRecoilLocalPosition;
        protected Vector3 accumulatedRecoil;
        protected float timeStartedWeaponSwitch;
        protected int weaponSwitchNewWeaponIndex;
        
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            SetFov(DefaultFOV);

            foreach (var weapon in StartingWeapons)
            {
                AddWeapon(weapon);
            }
            
            SwitchWeapon(true);
        }
        
        protected virtual void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<WeaponMessage>(MessageChannels.Weapons, WeaponMessageHandler);
        }

        protected virtual void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<WeaponMessage>(MessageChannels.Weapons, WeaponMessageHandler);
        }

        
        protected override void Update()
        {
            base.Update();
            // shoot handling
            WeaponController activeWeapon = GetActiveWeapon();

            if (activeWeapon != null && activeWeapon.IsReloading)
                return;

            if (activeWeapon != null && WeaponSwitchState == WeaponSwitchState.Up)
            {
                if (!activeWeapon.AutomaticReload && _input.Reload && activeWeapon.CurrentAmmoRatio < 1.0f)
                {
                    IsAiming = false;
                    activeWeapon.StartReloadAnimation();
                    return;
                }
                // handle aiming down sights
                IsAiming = _input.Aim;

                // handle shooting
                bool hasFired = activeWeapon.HandleShootInputs(
                    _input.FirePressed,
                    _input.FireHeld,
                    _input.FireReleased);

                // Handle accumulating recoil
                if (hasFired)
                {
                    accumulatedRecoil += Vector3.back * activeWeapon.RecoilForce;
                    accumulatedRecoil = Vector3.ClampMagnitude(accumulatedRecoil, MaxRecoilDistance);
                }
            }

            // weapon switch handling
            if (!IsAiming &&
                (activeWeapon == null || !activeWeapon.IsCharging) &&
                (WeaponSwitchState == WeaponSwitchState.Up || WeaponSwitchState == WeaponSwitchState.Down))
            {
                switch (_input.SwitchWeapon)
                {
                    case < 0:
                        SwitchWeapon(false);
                        break;
                    case > 0:
                        SwitchWeapon(true);
                        break;
                }

                if (_input.SelectWeapon > 0)
                {
                    if (GetWeaponAtSlotIndex(_input.SelectWeapon - 1) != null)
                        SwitchToWeaponIndex(_input.SelectWeapon - 1);
                }
            }

            // Pointing at enemy handling
            IsPointingAtTarget = false;
            if (activeWeapon)
            {
                if (Physics.Raycast(WeaponCamera.transform.position, WeaponCamera.transform.forward, out RaycastHit hit,
                    1000, -1, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.GetComponentInParent<HealthController>() != null)
                    {
                        IsPointingAtTarget = true;
                    }
                }
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            UpdateWeaponAiming();
            UpdateWeaponBob();
            UpdateWeaponRecoil();
            UpdateWeaponSwitching();

            // Set final weapon socket position based on all the combined animation influences
            WeaponParentSocket.localPosition = weaponMainLocalPosition + weaponBobLocalPosition + weaponRecoilLocalPosition;
        }
        
        // Iterate on all weapon slots to find the next valid weapon to switch to
        public void SwitchWeapon(bool ascendingOrder)
        {
            int newWeaponIndex = -1;
            int closestSlotDistance = WeaponSlots.Length;
            for (int i = 0; i < WeaponSlots.Length; i++)
            {
                // If the weapon at this slot is valid, calculate its "distance" from the active slot index (either in ascending or descending order)
                // and select it if it's the closest distance yet
                if (i != ActiveWeaponIndex && GetWeaponAtSlotIndex(i) != null)
                {
                    int distanceToActiveIndex = GetDistanceBetweenWeaponSlots(ActiveWeaponIndex, i, ascendingOrder);

                    if (distanceToActiveIndex < closestSlotDistance)
                    {
                        closestSlotDistance = distanceToActiveIndex;
                        newWeaponIndex = i;
                    }
                }
            }

            // Handle switching to the new weapon index
            SwitchToWeaponIndex(newWeaponIndex);
        }
        
        
        // Switches to the given weapon index in weapon slots if the new index is a valid weapon that is different from our current one
        public void SwitchToWeaponIndex(int newWeaponIndex, bool force = false)
        {
            if (force || (newWeaponIndex != ActiveWeaponIndex && newWeaponIndex >= 0))
            {
                // Store data related to weapon switching animation
                weaponSwitchNewWeaponIndex = newWeaponIndex;
                timeStartedWeaponSwitch = UnityEngine.Time.time;

                // Handle case of switching to a valid weapon for the first time (simply put it up without putting anything down first)
                if (GetActiveWeapon() == null)
                {
                    weaponMainLocalPosition = DownWeaponPosition.localPosition;
                    WeaponSwitchState = WeaponSwitchState.PutUpNew;
                    ActiveWeaponIndex = weaponSwitchNewWeaponIndex;

                    WeaponController newWeapon = GetWeaponAtSlotIndex(weaponSwitchNewWeaponIndex);
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Weapons, new WeaponMessage(WeaponChangedOperation.Switched, newWeapon));
                }
                // otherwise, remember we are putting down our current weapon for switching to the next one
                else
                {
                    WeaponSwitchState = WeaponSwitchState.PutDownPrevious;
                }
            }
        }
        
        public WeaponController HasWeapon(WeaponController weaponPrefab)
        {
            // Checks if we already have a weapon coming from the specified prefab
            for (var index = 0; index < WeaponSlots.Length; index++)
            {
                var w = WeaponSlots[index];
                if (w != null && w.SourcePrefab == weaponPrefab.gameObject)
                {
                    return w;
                }
            }

            return null;
        }
        
        // Updates weapon position and camera FoV for the aiming transition
        void UpdateWeaponAiming()
        {
            if (WeaponSwitchState == WeaponSwitchState.Up)
            {
                WeaponController activeWeapon = GetActiveWeapon();
                if (IsAiming && activeWeapon)
                {
                    weaponMainLocalPosition = Vector3.Lerp(weaponMainLocalPosition,
                        AimingWeaponPosition.localPosition + activeWeapon.AimOffset,
                        AimingAnimationSpeed * UnityEngine.Time.deltaTime);
                    SetFov(Mathf.Lerp(_camera.fieldOfView,
                        activeWeapon.AimZoomRatio * DefaultFOV, AimingAnimationSpeed * UnityEngine.Time.deltaTime));
                }
                else
                {
                    weaponMainLocalPosition = Vector3.Lerp(weaponMainLocalPosition,
                        DefaultWeaponPosition.localPosition, AimingAnimationSpeed * UnityEngine.Time.deltaTime);
                    SetFov(Mathf.Lerp(_camera.fieldOfView, DefaultFOV,
                        AimingAnimationSpeed * UnityEngine.Time.deltaTime));
                }
            }
        }

        // Updates the weapon bob animation based on character speed
        void UpdateWeaponBob()
        {
            if (UnityEngine.Time.deltaTime > 0f)
            {
                Vector3 playerCharacterVelocity =
                    (transform.position - lastCharacterPosition) / UnityEngine.Time.deltaTime;

                // calculate a smoothed weapon bob amount based on how close to our max grounded movement velocity we are
                float characterMovementFactor = 0f;
                if (Grounded)
                {
                    characterMovementFactor = !_input.Sprint ? Mathf.Clamp01(playerCharacterVelocity.magnitude / MoveSpeed) : Mathf.Clamp01(playerCharacterVelocity.magnitude / SprintSpeed);
                }

                weaponBobFactor = Mathf.Lerp(weaponBobFactor, characterMovementFactor, BobSharpness * UnityEngine.Time.deltaTime);

                // Calculate vertical and horizontal weapon bob values based on a sine function
                float bobAmount = IsAiming ? AimingBobAmount : DefaultBobAmount;
                float frequency = BobFrequency;
                float hBobValue = Mathf.Sin(UnityEngine.Time.time * frequency) * bobAmount * weaponBobFactor;
                float vBobValue = ((Mathf.Sin(UnityEngine.Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount * weaponBobFactor;

                // Apply weapon bob
                weaponBobLocalPosition.x = hBobValue;
                weaponBobLocalPosition.y = Mathf.Abs(vBobValue);

                lastCharacterPosition = transform.position;
            }
        }

        // Updates the weapon recoil animation
        void UpdateWeaponRecoil()
        {
            // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
            if (weaponRecoilLocalPosition.z >= accumulatedRecoil.z * 0.99f)
            {
                weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, accumulatedRecoil,
                    RecoilSharpness * UnityEngine.Time.deltaTime);
            }
            // otherwise, move recoil position to make it recover towards its resting pose
            else
            {
                weaponRecoilLocalPosition = Vector3.Lerp(weaponRecoilLocalPosition, Vector3.zero,
                    RecoilRestitutionSharpness * UnityEngine.Time.deltaTime);
                accumulatedRecoil = weaponRecoilLocalPosition;
            }
        }

        // Updates the animated transition of switching weapons
        void UpdateWeaponSwitching()
        {
            // Calculate the time ratio (0 to 1) since weapon switch was triggered
            float switchingTimeFactor = 0f;
            if (WeaponSwitchDelay == 0f)
            {
                switchingTimeFactor = 1f;
            }
            else
            {
                switchingTimeFactor = Mathf.Clamp01((UnityEngine.Time.time - timeStartedWeaponSwitch) / WeaponSwitchDelay);
            }

            // Handle transiting to new switch state
            if (switchingTimeFactor >= 1f)
            {
                if (WeaponSwitchState == WeaponSwitchState.PutDownPrevious)
                {
                    // Deactivate old weapon
                    WeaponController oldWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                    if (oldWeapon != null)
                    {
                        oldWeapon.ShowWeapon(false);
                    }

                    ActiveWeaponIndex = weaponSwitchNewWeaponIndex;
                    switchingTimeFactor = 0f;

                    // Activate new weapon
                    WeaponController newWeapon = GetWeaponAtSlotIndex(ActiveWeaponIndex);
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Weapons, new WeaponMessage(WeaponChangedOperation.Switched, newWeapon));


                    if (newWeapon)
                    {
                        timeStartedWeaponSwitch = UnityEngine.Time.time;
                        WeaponSwitchState = WeaponSwitchState.PutUpNew;
                    }
                    else
                    {
                        // if new weapon is null, don't follow through with putting weapon back up
                        WeaponSwitchState = WeaponSwitchState.Down;
                    }
                }
                else if (WeaponSwitchState == WeaponSwitchState.PutUpNew)
                {
                    WeaponSwitchState = WeaponSwitchState.Up;
                }
            }

            // Handle moving the weapon socket position for the animated weapon switching
            if (WeaponSwitchState == WeaponSwitchState.PutDownPrevious)
            {
                weaponMainLocalPosition = Vector3.Lerp(DefaultWeaponPosition.localPosition,
                    DownWeaponPosition.localPosition, switchingTimeFactor);
            }
            else if (WeaponSwitchState == WeaponSwitchState.PutUpNew)
            {
                weaponMainLocalPosition = Vector3.Lerp(DownWeaponPosition.localPosition,
                    DefaultWeaponPosition.localPosition, switchingTimeFactor);
            }
        }

        // Adds a weapon to our inventory
        public bool AddWeapon(WeaponController weaponPrefab)
        {
            // if we already hold this weapon type (a weapon coming from the same source prefab), don't add the weapon
            if (HasWeapon(weaponPrefab) != null)
            {
                return false;
            }

            // search our weapon slots for the first free one, assign the weapon to it, and return true if we found one. Return false otherwise
            for (int i = 0; i < WeaponSlots.Length; i++)
            {
                // only add the weapon if the slot is free
                if (WeaponSlots[i] == null)
                {
                    // spawn the weapon prefab as child of the weapon socket
                    WeaponController weaponInstance = Instantiate(weaponPrefab, WeaponParentSocket);
                    weaponInstance.transform.localPosition = Vector3.zero;
                    weaponInstance.transform.localRotation = Quaternion.identity;

                    // Set owner to this gameObject so the weapon can alter projectile/damage logic accordingly
                    weaponInstance.Owner = gameObject;
                    weaponInstance.SourcePrefab = weaponPrefab.gameObject;
                    weaponInstance.ShowWeapon(false);

                    // Assign the first person layer to the weapon
                    int layerIndex =
                        Mathf.RoundToInt(Mathf.Log(FpsWeaponLayer.value,
                            2)); // This function converts a layermask to a layer index
                    foreach (Transform t in weaponInstance.gameObject.GetComponentsInChildren<Transform>(true))
                    {
                        t.gameObject.layer = layerIndex;
                    }

                    WeaponSlots[i] = weaponInstance;
                    
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Weapons, new WeaponMessage(WeaponChangedOperation.Added, weaponInstance, i));

                    return true;
                }
            }

            // Handle auto-switching to weapon if no weapons currently
            if (GetActiveWeapon() == null)
            {
                SwitchWeapon(true);
            }

            return false;
        }

        public bool RemoveWeapon(WeaponController weaponInstance)
        {
            // Look through our slots for that weapon
            for (int i = 0; i < WeaponSlots.Length; i++)
            {
                // when weapon found, remove it
                if (WeaponSlots[i] == weaponInstance)
                {
                    WeaponSlots[i] = null;
                    
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Weapons, new WeaponMessage(WeaponChangedOperation.Removed, weaponInstance, i));

                    Destroy(weaponInstance.gameObject);

                    // Handle case of removing active weapon (switch to next weapon)
                    if (i == ActiveWeaponIndex)
                    {
                        SwitchWeapon(true);
                    }

                    return true;
                }
            }

            return false;
        }

        public WeaponController GetActiveWeapon()
        {
            return GetWeaponAtSlotIndex(ActiveWeaponIndex);
        }

        public WeaponController GetWeaponAtSlotIndex(int index)
        {
            // find the active weapon in our weapon slots based on our active weapon index
            if (index >= 0 &&
                index < WeaponSlots.Length)
            {
                return WeaponSlots[index];
            }

            // if we didn't find a valid active weapon in our weapon slots, return null
            return null;
        }

        // Calculates the "distance" between two weapon slot indexes
        // For example: if we had 5 weapon slots, the distance between slots #2 and #4 would be 2 in ascending order, and 3 in descending order
        int GetDistanceBetweenWeaponSlots(int fromSlotIndex, int toSlotIndex, bool ascendingOrder)
        {
            int distanceBetweenSlots = 0;

            if (ascendingOrder)
            {
                distanceBetweenSlots = toSlotIndex - fromSlotIndex;
            }
            else
            {
                distanceBetweenSlots = -1 * (toSlotIndex - fromSlotIndex);
            }

            if (distanceBetweenSlots < 0)
            {
                distanceBetweenSlots = WeaponSlots.Length + distanceBetweenSlots;
            }

            return distanceBetweenSlots;
        }

        // Sets the FOV of the main camera and the weapon camera simultaneously
        public virtual void SetFov(float fov)
        {
            _camera.fieldOfView = fov;
            WeaponCamera.fieldOfView = fov * WeaponFOVMultiplier;
        }

        public virtual void TakeDamage(float damage, bool isExplosionDamage, GameObject source, GameObject target = null)
        {
            if(target != gameObject) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Health, new HealthChangedMessage(HealthChangedOperation.Damaged, damage, source, target));
        }

        public virtual void Kill(GameObject source, GameObject target)
        {
            if(target != gameObject) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Health, new HealthChangedMessage(HealthChangedOperation.Died, 0, source, gameObject));
        }
        
        public virtual void WeaponMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<WeaponMessage>().HasValue) return;
            var data = message.Message<WeaponMessage>().GetValueOrDefault();
            switch (data.Operation)
            {
                case WeaponChangedOperation.None:
                    break;
                case WeaponChangedOperation.Switched:
                    if (data.Weapon != null)
                    {
                        data.Weapon.ShowWeapon(true);
                    }
                    break;
                case WeaponChangedOperation.PickedUp:
                    break;
                case WeaponChangedOperation.Dropped:
                    break;
                case WeaponChangedOperation.Added:
                    break;
                case WeaponChangedOperation.Removed:
                    break;
                case WeaponChangedOperation.Equipped:
                    break;
                case WeaponChangedOperation.Unequipped:
                    break;
                case WeaponChangedOperation.Reloaded:
                    break;
            }

        }


    }
}