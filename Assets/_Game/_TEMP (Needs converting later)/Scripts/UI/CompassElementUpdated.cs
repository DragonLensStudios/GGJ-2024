using System;
using System.Collections;
using System.Collections.Generic;
using FPS.Scripts.UI;
using UnityEngine;

public class CompassElementUpdated : MonoBehaviour
{
    [field: SerializeField] public CompassMarkerUpdated CompassMarkerPrefab { get; set; }
    [field: SerializeField] public string TextDirection { get; set; }

    protected CompassUpdated Compass;
    
    private void Awake()
    {
        Compass = FindObjectOfType<CompassUpdated>();
        //
        // var markerInstance = Instantiate(CompassMarkerPrefab);
        //
        // markerInstance.Initialize(this, TextDirection);
        // Compass.RegisterCompassElement(transform, markerInstance, markerInstance);
        
        Compass.RegisterCompassElement(this, transform, CompassMarkerPrefab, CompassMarkerPrefab);
    }

    private void OnDestroy()
    {
        Compass.UnregisterCompassElement(transform);
    }
}
