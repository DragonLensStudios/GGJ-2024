using UnityEngine;

namespace UI
{
    [System.Serializable]
    public struct CrosshairData
    {
        [field:Tooltip("The image that will be used for this weapon's crosshair")]
        [field:SerializeField] public Sprite CrosshairSprite { get; set; }

        [field:Tooltip("The size of the crosshair image")]
        [field:SerializeField] public int CrosshairSize { get; set; }

        [field:Tooltip("The color of the crosshair image")]
        [field:SerializeField] public Color CrosshairColor { get; set; }
    }
}