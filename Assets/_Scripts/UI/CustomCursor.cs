using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D t;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(t, new Vector2(4, 4), CursorMode.Auto);
    }
}
