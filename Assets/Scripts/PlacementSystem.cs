using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectsDatabaseSO database;
    [SerializeField] private GameObject gridVisualization;
    private int _selectedObjectIndex = -1;
    

    private void Awake()
    {
        GD.Assert(mouseIndicator != null, "mouseIndicator null");
        GD.Assert(cellIndicator != null, "cellIndicator null");
        GD.Assert(inputManager != null, "inputManager null");
        GD.Assert(grid != null, "grid null");
        GD.Assert(database != null, "database null");
        GD.Assert(gridVisualization != null, "gridVisualization null");
    }

    // Start is called before the first frame update
    void Start()
    {
        StopPlacement();
    }


    // Update is called once per frame
    void Update()
    {
        if(_selectedObjectIndex < 0)
            return;
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        mouseIndicator.transform.position = mousePosition;
        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
    }
    
    
    private void StopPlacement()
    {
        _selectedObjectIndex = -1;
        gridVisualization.SetActive(false);
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }
    
    
    public void StartPlacement(int id)
    {
        StopPlacement();
        _selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == id);
        if (_selectedObjectIndex < 0)
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
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        // mouseIndicator.transform.position = mousePosition;
        GameObject newObject = Instantiate(database.objectsData[_selectedObjectIndex].Prefab);
        newObject.transform.position = grid.CellToWorld(gridPosition);
    }

}
