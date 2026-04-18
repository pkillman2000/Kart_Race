using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Gauge _gauge;

    void Start()
    {

    }


    void Update()
    {

    }

    public void SetCurrentGearName(string currentGearName)
    {
        _gauge.SetGear(currentGearName);
    }

    public void SetSpeed(float speed, string speedText)
    {
        _gauge.SetSpeedometerNeedle(speed);
        _gauge.SetSpeed(speedText);
    }

    public void SetTachometer(float value)
    {
        _gauge.SetTachometer(value);
    }
}
