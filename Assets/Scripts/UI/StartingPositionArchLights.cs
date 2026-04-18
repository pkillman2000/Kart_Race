using UnityEngine;

public class StartingPositionArchLights : MonoBehaviour
{
    /*
     * When adding the lights to the array,
     * add them in 0 - 9 order.  This script
     * will assume that light 0 and 5 are the rightmost
     * lights in the groups.
    */
    [SerializeField]
    private BoxLights[] _lights;

    void Start()
    {
        SetLightsOff();
    }

    public void SetLightsOff()
    {
        for(int i = 0; i < _lights.Length; i++)
        {
            _lights[i].SetAllLightsOff();
        }
    }

    public void SetLightsOn(int countdownIndex)
    {
        switch (countdownIndex)
        {
            case 5: // Code for the first light
                _lights[0].SetRedLightsOn();
                _lights[5].SetRedLightsOn();
                break;
            case 4: // Code for the second light
                _lights[1].SetRedLightsOn();
                _lights[6].SetRedLightsOn();
                break;
            case 3: // Code for the third light
                _lights[2].SetRedLightsOn();
                _lights[7].SetRedLightsOn();
                break;
            case 2: // Code for the fourth light
                _lights[3].SetRedLightsOn();
                _lights[8].SetRedLightsOn();
                break;
            case 1:
                _lights[4].SetRedLightsOn();
                _lights[9].SetRedLightsOn();
                break;
            case 0: // Code for Go!  Red lights off, green lights on
                for (int i = 0; i < _lights.Length; i++)
                {
                    _lights[i].SetAllLightsOff();
                    _lights[i].SetGreenLightOn();
                }
                break;
        }
    }
}
