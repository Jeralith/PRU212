using UnityEngine;

public class Parallax : MonoBehaviour
{
    
    private Material material;
    public float distance;
    [Range(0f, 0.5f)]
    public float _speed = 0.2f;
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        distance += Time.deltaTime * _speed;
        material.SetTextureOffset("_MainTex", Vector2.right * distance);
    }
}
