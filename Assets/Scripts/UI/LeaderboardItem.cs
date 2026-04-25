using TMPro;
using UnityEngine;

public class LeaderboardItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _positionText;
    [SerializeField]
    private TMP_Text _nameText;

    public void UpdateItem(int position, string name)
    {
        _positionText.text = position.ToString();
        _nameText.text = name;
    }
}
