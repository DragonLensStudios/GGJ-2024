using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class FPSController : FirstPersonBasicController
    {
        [field:Header("FPS Settings")]
        [field: SerializeField] public GameObject Weapon { get; set; }

        protected override void Update()
        {
            base.Update();
            if(_input.Fire)
            {
                Fire();
            }
        }
        
        public virtual void Fire()
        {
            Debug.Log("Fire");
            // if(Weapon != null)
            // {
            //     Weapon.GetComponent<Weapon>().Fire();
            // }
        }
    }
}