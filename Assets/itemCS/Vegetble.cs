using UnityEngine;

[CreateAssetMenu(fileName = "Vegetble", menuName = "Game/VegetbleDate")]
public class ItemDate : ScriptableObject 
{
    [SerializeField] private string _ItemId;
    [SerializeField] private string _itemName;
    [SerializeField] private string _date;
    [SerializeField] private Sprite _sprite;
}

public class Vegetble : MonoBehaviour
{




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
