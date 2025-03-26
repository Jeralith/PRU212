using UnityEngine;

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile Instance;

    [SerializeField] public string playerName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // N?u ?ã t?n t?i, xoá b?n m?i
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Ch? g?i 1 l?n duy nh?t ? object ??u tiên
    }
}
