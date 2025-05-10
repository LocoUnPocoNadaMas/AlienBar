using System.Collections.Generic;
using Model;
using UnityEngine;
using Utils;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabaseSo database;
    [SerializeField] private GameObject gridVisualization;
    [SerializeField] private GameObject grass;
    [SerializeField] private AudioSource audioSource;
    private int selectedObjectIndex = -1;
    private GridData floorData, furnitureData, itemData;
    private Renderer previewRenderer;
    private List<GameObject> placedGameObjects = new();

    private Vector3Int lastPos;
    private Vector2Int gridSize;

    private void Awake()
    {
        Debug.Assert(audioSource != null, "audioSource null");
        Debug.Assert(database.objectsData.Find(data => data.ObjectType == ObjectsType.None) == null, "objectData none");
        // Debug.Assert(audioSource.clip != null, "audioClip null");
        Debug.Assert(!database.GetGridSize.Equals(new Vector2Int(0, 0)), "gridSize 0");
        
    }

    // Start is called before the first frame update
    private void Start()
    {
        SetGridSize();
        StopPlacement();
        floorData = new();
        furnitureData = new();
        itemData = new();
        previewRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;
        var mousePosition = inputManager.GetSelectedMapPosition();
        var gridPosition = grid.WorldToCell(mousePosition);
        
        // This save a lot of cpu cycles
        if (gridPosition != lastPos)
        {
            lastPos = gridPosition;
            Debug.Log(lastPos);
            
            var visualizationPos = gridVisualization.transform.position;
            visualizationPos.y = lastPos.y + 0.01f;
            gridVisualization.transform.position = visualizationPos;
        }
        var type = database.objectsData[selectedObjectIndex].ObjectType;
        var placementVal = CheckPlacementValidity(gridPosition, selectedObjectIndex, type);
        previewRenderer.material.color = placementVal != -1 ? Color.white : Color.red;
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }

    private void SetGridSize()
    {
        gridSize = database.GetGridSize;
        var gridVisualScale = gridVisualization.transform.localScale;
        var grasScale = grass.transform.localScale;
        gridVisualScale.x = (float)gridSize.x / 10;
        gridVisualScale.z = (float)gridSize.y / 10;
        grasScale.x = (float)gridSize.x / 10 + 0.1f;
        grasScale.z = (float)gridSize.y / 10 + 0.1f;
        gridVisualization.transform.localScale = gridVisualScale;
        grass.transform.localScale =  grasScale;
        Debug.Log("pasto: "+ grasScale + " grilla: "+ gridVisualScale);
        
        if (gridSize.x % 2 == 0 || gridSize.y % 2 == 0) return;
        var gridPos = grid.transform.position;
        if (gridSize.x % 2 != 0)
        {
            gridPos.x -= -0.5f;
        }
        if (gridSize.y % 2 != 0)
        {
            gridPos.z -= -0.5f;
        }
        grid.transform.position = gridPos;
    }


    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }


    public void StartPlacement(int id)
    {
        StopPlacement();
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == id);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {id}");
            return;
        }

        gridVisualization.SetActive(true);
        cellIndicator.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUi())
            return;
        var mousePosition = inputManager.GetSelectedMapPosition();
        var gridPosition = grid.WorldToCell(mousePosition);

        var objectType = database.objectsData[selectedObjectIndex].ObjectType;

        var placementVal = CheckPlacementValidity(gridPosition, selectedObjectIndex, objectType);
        if (placementVal == -1)
            return;
        audioSource.Play();
        // mouseIndicator.transform.position = mousePosition;
        var newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
        placedGameObjects.Add(newObject);
        var selectedData = GetGridData(selectedObjectIndex);
        var newObjectData = database.objectsData[selectedObjectIndex];

        selectedData.AddObjectAt(gridPosition,
            newObjectData.Size,
            newObjectData.ID,
            placedGameObjects.Count - 1,
            gridSize,
            objectType);
    }

    private GridData GetGridData(int index)
    {
        GridData gridData = new();
        var objectType = database.objectsData[index].ObjectType;
        switch (objectType)
        {
            case ObjectsType.Floor:
            {
                gridData = floorData;

                break;
            }
            case ObjectsType.Furniture:
            {
                gridData = furnitureData;
                break;
            }
            case ObjectsType.Item:
                gridData = itemData;
                break;
            default:
                // throw new ArgumentException($"Check index: {index} in Database with type: {objectType}");
                Debug.LogError($"Check index: {index} in Database with type: {objectType}");
                Debug.Break();
                break;
        }

        return gridData;
    }

    private int CheckPlacementValidity(Vector3Int gridPosition, int index, ObjectsType objectType)
    {
        var selectedData = GetGridData(index);
        //GridData selectedData = database.objectsData[index].ObjectsType == ObjectsType.Floor ? floorData : furnitureData;
        return selectedData.CanPlaceObjectAt(
            gridPosition, database.objectsData[index].Size, gridSize, objectType);
    }
}