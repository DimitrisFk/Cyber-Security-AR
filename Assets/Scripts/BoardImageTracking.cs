using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class BoardImageTracking : MonoBehaviour
{
    private ARTrackedImageManager m_trackedImageManager;

    [SerializeField]
    private TrackedPrefab[] prefabToInstantiate;

    [SerializeField]
    private ARSession _arSession;

    private Dictionary<string, GameObject> instanciatePrefab;

    void Start()
    {
        _arSession.Reset();
    }

    private void Awake()
    {
        m_trackedImageManager = GetComponent<ARTrackedImageManager>();
        instanciatePrefab = new Dictionary<string, GameObject>();
    }

    private void OnEnable()
    {
        m_trackedImageManager.trackedImagesChanged += OnTrackedImageChanged;
    }

    private void OnDisable()
    {
        m_trackedImageManager.trackedImagesChanged -= OnTrackedImageChanged;
    }

    private void OnTrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage addedImage in eventArgs.added)
        {
            InstantiateGameObject(addedImage);
        }

        foreach (ARTrackedImage updatedImage in eventArgs.updated)
        {
            if (updatedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
            {
                UpdateTrackingGameObject(updatedImage);
            }
            else if (updatedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Limited)
            {
                UpdateLimitedGameObject(updatedImage);
            }
            else
            {
                UpdateNoneGameObject(updatedImage);
            }
        }

        foreach (ARTrackedImage removedImage in eventArgs.removed)
        {
            DestroyGameObject(removedImage);
        }
    }

    private void InstantiateGameObject(ARTrackedImage addedImage)
    {
        for (int i = 0; i < prefabToInstantiate.Length; i++)
        {
            if (addedImage.referenceImage.name == prefabToInstantiate[i].name)
            {
                GameObject prefab = Instantiate<GameObject>(prefabToInstantiate[i].prefab, transform.parent);
                prefab.transform.position = addedImage.transform.position;
                prefab.transform.rotation = addedImage.transform.rotation;

                instanciatePrefab.Add(addedImage.referenceImage.name, prefab);
            }
        }
    }

    private void UpdateTrackingGameObject(ARTrackedImage updatedImage)
    {

        for (int i = 0; i < instanciatePrefab.Count; i++)
        {
            if (instanciatePrefab.TryGetValue(updatedImage.referenceImage.name, out GameObject prefab))
            {
                prefab.transform.position = updatedImage.transform.position;
                prefab.transform.rotation = updatedImage.transform.rotation;
                prefab.SetActive(true);
            }
        }
    }

    private void UpdateLimitedGameObject(ARTrackedImage updatedImage)
    {
        for (int i = 0; i < instanciatePrefab.Count; i++)
        {
            if (instanciatePrefab.TryGetValue(updatedImage.referenceImage.name, out GameObject prefab))
            {
                if (!prefab.GetComponent<ARTrackedImage>().destroyOnRemoval)
                {
                    prefab.transform.position = updatedImage.transform.position;
                    prefab.transform.rotation = updatedImage.transform.rotation;
                    prefab.SetActive(true);
                }
                else
                {
                    prefab.SetActive(false);
                }

            }
        }
    }

    private void UpdateNoneGameObject(ARTrackedImage updateImage)
    {
        for (int i = 0; i < instanciatePrefab.Count; i++)
        {
            if (instanciatePrefab.TryGetValue(updateImage.referenceImage.name, out GameObject prefab))
            {
                prefab.SetActive(false);
            }
        }
    }

    private void DestroyGameObject(ARTrackedImage removedImage)
    {
        for (int i = 0; i < instanciatePrefab.Count; i++)
        {
            if (instanciatePrefab.TryGetValue(removedImage.referenceImage.name, out GameObject prefab))
            {
                instanciatePrefab.Remove(removedImage.referenceImage.name);
                Destroy(prefab);
            }
        }
    }
}

[System.Serializable]
public struct TrackedPrefab
{
    public string name;
    public GameObject prefab;
}