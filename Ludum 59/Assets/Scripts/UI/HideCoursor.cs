using UnityEngine;

public class HideCoursor : MonoBehaviour
{
    public void HideAndLockCoursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
