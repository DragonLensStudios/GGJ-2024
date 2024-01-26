using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class FPSController : FirstPersonBasicController
    {
        [field:Header("FPS Settings")]
        [field: SerializeField] public GameObject CurrentWeapon { get; set; }

        protected override void Update()
        {
            base.Update();

            if (_input.Reload)
            {
                Debug.Log("Reloading");
            }
            if (_input.SwitchWeapon > 0)
            {
                Debug.Log("Switching to next weapon");
            }
            else if (_input.SwitchWeapon < 0)
            {
                Debug.Log("Switching to previous weapon");
            }
            else
            {
                Debug.Log("No input detected");
            }
        }
    }
}