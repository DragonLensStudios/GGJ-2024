using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonorController : MonoBehaviour
{
    [field: SerializeField] public int DonationMin { get; set; } = 1;
    [field: SerializeField] public int DonationMax { get; set; } = 5000;
    
    
}
