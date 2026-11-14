using UnityEngine;
using Unity.Cinemachine;

public class RPGCameraManager : MonoBehaviour
{
    public static RPGCameraManager sharedInstance = null;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private string virtualCameraTag = "VirtualCamera";

    [Header("Startup")]
    [SerializeField] private bool findCameraOnStart = true;
    [SerializeField] private bool warnIfCameraMissing = true;

    public CinemachineCamera VirtualCamera => virtualCamera;

    private void Awake()
    {
        if (sharedInstance != null && sharedInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        sharedInstance = this;

        if (findCameraOnStart)
        {
            EnsureVirtualCamera();
        }
    }

    private void Start()
    {
        if (findCameraOnStart)
        {
            EnsureVirtualCamera();
        }
    }

    public void SetFollowTarget(Transform target)
    {
        if (target == null)
        {
            Debug.LogWarning("RPGCameraManager cannot set Follow target because target is null.");
            return;
        }

        if (!EnsureVirtualCamera())
        {
            return;
        }

        virtualCamera.Follow = target;

        Debug.Log("Camera Follow target set to: " + target.name);
    }

    public void ClearFollowTarget()
    {
        if (!EnsureVirtualCamera())
        {
            return;
        }

        virtualCamera.Follow = null;

        Debug.Log("Camera Follow target cleared.");
    }

    private bool EnsureVirtualCamera()
    {
        if (virtualCamera != null)
        {
            return true;
        }

        virtualCamera = FindVirtualCamera();

        if (virtualCamera == null && warnIfCameraMissing)
        {
            Debug.LogWarning("RPGCameraManager could not find a CinemachineCamera. Assign CM vcam1 manually in the Inspector.");
        }

        return virtualCamera != null;
    }

    private CinemachineCamera FindVirtualCamera()
    {
        if (!string.IsNullOrEmpty(virtualCameraTag))
        {
            try
            {
                GameObject taggedCameraObject = GameObject.FindWithTag(virtualCameraTag);

                if (taggedCameraObject != null)
                {
                    CinemachineCamera taggedCamera = taggedCameraObject.GetComponent<CinemachineCamera>();

                    if (taggedCamera != null)
                    {
                        return taggedCamera;
                    }

                    if (warnIfCameraMissing)
                    {
                        Debug.LogWarning(
                            taggedCameraObject.name +
                            " is tagged as " +
                            virtualCameraTag +
                            " but does not have a CinemachineCamera component."
                        );
                    }
                }
            }
            catch (UnityException)
            {
                if (warnIfCameraMissing)
                {
                    Debug.LogWarning(
                        "The tag '" +
                        virtualCameraTag +
                        "' does not exist. Create it in Unity or assign the camera manually."
                    );
                }
            }
        }

        CinemachineCamera[] cameras = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None);

        if (cameras.Length == 1)
        {
            return cameras[0];
        }

        if (cameras.Length > 1 && warnIfCameraMissing)
        {
            Debug.LogWarning(
                "More than one CinemachineCamera was found. Assign the correct one manually in the RPGCameraManager Inspector."
            );
        }
        return null;
    }
}