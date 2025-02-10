using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour
{
    //public GameObject _player;
    [SerializeField] private float bounce = 30f;
    private Animator _anim;
    void Start()
    {
        _anim = GetComponent<Animator>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //other.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            other.gameObject.GetComponent<Rigidbody2D>().linearVelocityY = bounce;
            StartCoroutine(SpringAnimation());
        }
    }
    private IEnumerator SpringAnimation()
    {
        _anim.SetBool("PlayAnimation", true);
        yield return new WaitForSeconds(0.5833f);
        _anim.SetBool("PlayAnimation", false);
    }
}
