using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
[Serializable]
public class ImagePrefabEntry
{
    public string imageName;
    public GameObject prefab;
}
public class ARImageTracker : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager imageManager;
    [SerializeField] private List<ImagePrefabEntry> imagePrefabs;
    private Dictionary<string, GameObject> _prefabLookup = new Dictionary<string, GameObject>();
    private void Awake() {
        foreach (var entry in imagePrefabs)
        {
            if (!_prefabLookup.ContainsKey(entry.imageName))
            {
                _prefabLookup[entry.imageName] = entry.prefab;
            }
        }
    }
    private void OnEnable()
    {
        imageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }
    private void OnDisable()
    {
        imageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }
    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            string imageName = trackedImage.referenceImage.name;
            // Only spawn if this image name is registered in the list
            if (_prefabLookup.TryGetValue(imageName, out GameObject prefab))
            {
                GameObject spawnedContent = Instantiate(
                prefab,
                trackedImage.transform.position,
                trackedImage.transform.rotation
                );
                spawnedContent.transform.SetParent(trackedImage.transform);
            }
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            if (trackedImage.transform.childCount > 0)
            {
                GameObject content = trackedImage.transform.GetChild(0).gameObject;
                bool isTracking = trackedImage.trackingState == TrackingState.Tracking;
                content.SetActive(isTracking);
            }
        }
        foreach (var pair in eventArgs.removed)
        {
            Debug.Log("Image removed: " + pair.Value.referenceImage.name);
        }
    }
}