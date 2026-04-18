using UnityEngine;

public class BoxLights : MonoBehaviour
{
    [SerializeField]
    private Material _redUnlit;
    [SerializeField] 
    private Material _redLit;
    [SerializeField]
    private Material _yellowUnlit;
    [SerializeField]
    private Material _yellowLit;
    [SerializeField]
    private Material _greenUnlit;
    [SerializeField]
    private Material _greenLit;

    [SerializeField]
    private Renderer _redLight1;
    [SerializeField]
    private Renderer _redLight2;
    [SerializeField]
    private Renderer _yellowLight1;
    [SerializeField]
    private Renderer _yellowLight2;
    [SerializeField]
    private Renderer _greenLight;

    void Start()
    {
        SetAllLightsOff();
    }

    public void SetAllLightsOff()
    {
        _redLight1.material = _redUnlit;
        _redLight2.material = _redUnlit;
        _yellowLight1.material = _yellowUnlit;
        _yellowLight2.material = _yellowUnlit;
        _greenLight.material = _greenUnlit;
    }

    public void SetRedLightsOn()
    {
        SetAllLightsOff();
        _redLight1.material = _redLit;
        _redLight2.material = _redLit;
    }

    public void SetYellowLightsOn()
    {
        SetAllLightsOff();
        _yellowLight1.material = _yellowLit;
        _yellowLight2.material = _yellowLit;
    }

    public void SetGreenLightOn()
    {
        SetAllLightsOff();
        _greenLight.material = _greenLit;
    }
}
