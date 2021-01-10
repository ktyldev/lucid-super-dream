using UnityEngine;

public class WrapAroundOnEnable : MonoBehaviour
{
    [SerializeField] private float xMin, xMax;
    
    private void OnEnable()
    {
        WaitUtils.Wait(Time.deltaTime, true, () =>
        {
            
        var w = Mathf.Abs(xMax - xMin);
        var x = transform.localPosition.x;
        if (x > xMax)
        {
            x -= w;
        }
        else if (x < xMin)
        {
            x += w;
        }
        transform.localPosition = new Vector3(
            x,
            transform.localPosition.y,
            transform.localPosition.z);
        });
    }
}