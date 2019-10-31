using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class LocationDataHolder : ShortLifeSingleton<LocationDataHolder>
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LocationServiceCheckRoutine());
    }

    void RequestPermission()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
#endif
    }

    #region Location Service
    [System.Serializable]
    public class LocationData
    {
        public bool valid = false;
        public float longitude = 0f;
        public float latitude = 0f;
        public float altitude = 0f;
        public double timestamp = 0f;

        public LocationData Clone()
        {
            LocationData data = new LocationData();
            data.valid = valid;
            data.longitude = longitude;
            data.latitude = latitude;
            data.altitude = altitude;
            data.timestamp = timestamp;
            return data;
        }
    }

    public LocationData GetLocationData()
    {
        return locationData.Clone();
    }

    LocationData locationData = new LocationData();

    IEnumerator LocationServiceCheckRoutine()
    {
        RequestPermission();
        while (!Input.location.isEnabledByUser)
        {
            if (!Application.isEditor)
            {
                Debug.LogWarning("Location Service Not Enabled!");
            }
            yield return null;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1f);
            maxWait--;
        }

        if (maxWait < 1)
        {
            Debug.LogError("Location Service TimeOut");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            locationData.valid = true;
            locationData.latitude = Input.location.lastData.latitude;
            locationData.longitude = Input.location.lastData.longitude;
            locationData.altitude = Input.location.lastData.altitude;
            locationData.timestamp = Input.location.lastData.timestamp;

            Debug.Log("LocationData:" + "latitude-" + locationData.latitude + "  longtitude-" + locationData.longitude);
        }

        Input.location.Stop();
    }
    #endregion
}
