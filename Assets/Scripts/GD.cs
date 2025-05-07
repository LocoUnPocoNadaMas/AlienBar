using UnityEngine;

public class GD : MonoBehaviour
{
    public static void Assert(bool condition, string error)
    {
        if (!condition)
        {
            Debug.LogError(error);
            Debug.Break(); // Detiene la ejecución en el editor
        }
    }
}