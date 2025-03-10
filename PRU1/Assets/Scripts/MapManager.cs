using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [SerializeField] private GameObject _miniMap;
    [SerializeField] private GameObject _largeMap;

    public bool IsLargeMapOpen { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        CloseLargeMap();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!IsLargeMapOpen)
            {
                OpenLargeMap();
            }
            else
            {
                CloseLargeMap();
            }

        }
    }

    public void OpenLargeMap()
    {
        _miniMap.SetActive(false);
        _largeMap.SetActive(true);
        IsLargeMapOpen = true;
    }

    public void CloseLargeMap()
    {
        _miniMap.SetActive(true);
        _largeMap.SetActive(false);
        IsLargeMapOpen = false;
    }
}
