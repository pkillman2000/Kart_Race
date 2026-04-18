using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
    [SerializeField]
    private Image _tachometer;
    [SerializeField]
    private GameObject _speedometerNeedle;
    [SerializeField]
    private TMP_Text _gear;
    [SerializeField]
    private TMP_Text _speed;

    void Start()
    {

    }


    void Update()
    {

    }

    public void SetTachometer(float value)
    {
        // Only half of the tachometer fill is used, so we divide the value by 2
        // Add 0.03f to show engine idle at 0 value
        _tachometer.fillAmount = (value / 2) + 0.03f;
    }

    public void SetSpeedometerNeedle(float value)
    {
        // Convert value between 90 and -90 degrees
        // 0 = 90 degrees, 1 = -90 degrees

        float angle = Mathf.Lerp(90, -90, value);
        _speedometerNeedle.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetGear(string value)
    {
        _gear.text = value;
    }

    public void SetSpeed(string value)
    {
        _speed.text = value;
    }
}