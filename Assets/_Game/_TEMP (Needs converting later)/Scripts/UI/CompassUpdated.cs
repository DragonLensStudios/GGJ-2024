using System.Collections.Generic;
using UnityEngine;
using FPS.Scripts.Gameplay;
using FPS.Scripts.UI;

public class CompassUpdated : MonoBehaviour
{
    [field: SerializeField] public RectTransform InGameCompassRect { get; set; }
    [field: SerializeField] public RectTransform InStreamCompassRect { get; set; }
    [field: SerializeField] public float VisibilityAngle { get; set; } = 180f;
    [field: SerializeField] public float HeightDifferenceMultiplier { get; set; } = 2f;
    [field: SerializeField] public float MinScale { get; set; } = 0.5f;
    [field: SerializeField] public float DistanceMinScale { get; set; } = 50f;
    [field: SerializeField] public float CompasMarginRatio { get; set; } = 0.8f;
    [field: SerializeField] public GameObject MarkerDirectionPrefab { get; set; }
    
    protected Transform PlayerTransform;
    protected Dictionary<Transform, (CompassMarkerUpdated inGameMarker, CompassMarkerUpdated inStreamMarker)> ElementsDictionary = new Dictionary<Transform, (CompassMarkerUpdated, CompassMarkerUpdated)>();
    
    float InGameWidthMultiplier;
    float InGameHeightOffset;
    float InStreamWidthMultiplier;
    float InStreamHeightOffset;

    private void Awake()
    {
        PlayerCharacterController playerCharacterController = FindObjectOfType<PlayerCharacterController>();
        
        PlayerTransform = playerCharacterController.transform;
        InGameWidthMultiplier = InGameCompassRect.rect.width / VisibilityAngle;
        InGameHeightOffset = -InGameCompassRect.rect.height / 2;
        InStreamWidthMultiplier = InStreamCompassRect.rect.width / VisibilityAngle;
        InStreamHeightOffset = -InStreamCompassRect.rect.height / 2;
    }

    private void Update()
    {
        foreach (var element in ElementsDictionary)
        {
            UpdateCompassMarker(element.Key, element.Value.inGameMarker, InGameCompassRect, InGameWidthMultiplier, InGameHeightOffset);
            UpdateCompassMarker(element.Key, element.Value.inStreamMarker, InStreamCompassRect, InStreamWidthMultiplier, InStreamHeightOffset);
        }
    }

    // Adjusted method to apply correct multipliers and offsets for each compass
    private void UpdateCompassMarker(Transform elementTransform, CompassMarkerUpdated marker, RectTransform compassRect, float widthMultiplier, float heightOffset)
    {
        float distanceRatio = 1;
        float heightDifference = 0;
        float angle;

        if (marker.IsDirection)
        {
            angle = Vector3.SignedAngle(PlayerTransform.forward, elementTransform.localPosition.normalized, Vector3.up);
        }
        else
        {
            Vector3 targetDir = (elementTransform.position - PlayerTransform.position).normalized;
            targetDir = Vector3.ProjectOnPlane(targetDir, Vector3.up);
            Vector3 playerForward = Vector3.ProjectOnPlane(PlayerTransform.forward, Vector3.up);
            angle = Vector3.SignedAngle(playerForward, targetDir, Vector3.up);

            Vector3 directionVector = elementTransform.position - PlayerTransform.position;

            heightDifference = (directionVector.y) * HeightDifferenceMultiplier;
            heightDifference = Mathf.Clamp(heightDifference, -compassRect.rect.height / 2 * CompasMarginRatio, compassRect.rect.height / 2 * CompasMarginRatio);

            distanceRatio = directionVector.magnitude / DistanceMinScale;
            distanceRatio = Mathf.Clamp01(distanceRatio);
        }

        if (angle > -VisibilityAngle / 2 && angle < VisibilityAngle / 2)
        {
            marker.CanvasGroup.alpha = 1;
            marker.CanvasGroup.transform.localPosition = new Vector2(widthMultiplier * angle, heightDifference + heightOffset);
            marker.CanvasGroup.transform.localScale = Vector3.one * Mathf.Lerp(1, MinScale, distanceRatio);
        }
        else
        {
            marker.CanvasGroup.alpha = 0;
        }
    }

    public void RegisterCompassElement(CompassElementUpdated compassElement, Transform element, CompassMarkerUpdated inGameMarkerPrefab, CompassMarkerUpdated inStreamMarkerPrefab)
    {
        // Instantiate markers for both in-game and in-stream UIs from the prefabs
        var inGameMarkerInstance = Instantiate(inGameMarkerPrefab, InGameCompassRect.transform);
        var inStreamMarkerInstance = Instantiate(inStreamMarkerPrefab, InStreamCompassRect.transform);
        
        // Initialize the markers with the direction text
        inGameMarkerInstance.Initialize(compassElement, compassElement.TextDirection);
        inStreamMarkerInstance.Initialize(compassElement, compassElement.TextDirection);

        ElementsDictionary.Add(element, (inGameMarkerInstance, inStreamMarkerInstance));
    }

    public void UnregisterCompassElement(Transform element)
    {
        if (ElementsDictionary.TryGetValue(element, out var markers))
        {
            if (markers.inGameMarker.CanvasGroup != null)
                Destroy(markers.inGameMarker.CanvasGroup.gameObject);
            if (markers.inStreamMarker.CanvasGroup != null)
                Destroy(markers.inStreamMarker.CanvasGroup.gameObject);

            ElementsDictionary.Remove(element);
        }
    }
}
