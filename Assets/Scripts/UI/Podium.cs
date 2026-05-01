using UnityEngine;

public class Podium : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _podiumPositions;


    void Start()
    {
        PopulatePodiumArray();
    }

    private void PopulatePodiumArray()
    {
        _podiumPositions = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            _podiumPositions[i] = transform.GetChild(i).gameObject;
        }
    }

    public Transform GetPodiumPosition(int index)
    {
        return _podiumPositions[index].transform;
    }
}
