using System.Collections;
using UnityEngine;

public class Spring : MonoBehaviour
{
    //public GameObject _player;
    [SerializeField] private float bounce = 30f;
    [SerializeField] private AudioClip springClip;

    private Animator _anim;
    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (SFXManager.instance != null && springClip != null) {
                SFXManager.instance.PlaySFXClip(springClip, transform, 1f);
            }
            other.gameObject.GetComponent<Rigidbody2D>().linearVelocityY = bounce;
            if (_anim != null)
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
