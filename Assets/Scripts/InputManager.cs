using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask layerMask;
    private Vector3 lastPosition;

    public event Action OnClicked, OnExit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }
    }

    public bool IsPointerOverUi() => EventSystem.current.IsPointerOverGameObject();

    private void Awake()
    {
        Debug.Assert(layerMask != 0, "layerMask == 0?");
    }

    public Vector3 GetSelectedMapPosition()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = sceneCamera.nearClipPlane;
        var ray = sceneCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out var hit, 100, layerMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }
}