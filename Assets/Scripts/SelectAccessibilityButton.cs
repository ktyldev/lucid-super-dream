using UnityEngine;

public class SelectAccessibilityButton : MonoBehaviour
{
    [SerializeField] private AccessibilityMode mode;
    [SerializeField] private AccessibilityOptions options;
    [SerializeField] private TunnelController tunnel;
    
    public void Select()
    {

        options.Mode = mode;
        tunnel.Reset();
    }
}