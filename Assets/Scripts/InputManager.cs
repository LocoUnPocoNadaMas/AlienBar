using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    private Vector3 _lastPosition;
    [SerializeField] private LayerMask layerMask;

    private void Awake()
    {
        GD.Assert(sceneCamera != null, "sceneCamera null");
        GD.Assert(layerMask != 0, "layerMask null");
    }

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            _lastPosition = hit.point;
        }

        return _lastPosition;
    }
}
