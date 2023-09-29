using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieDamageController : MonoBehaviour
{    
    [SerializeField] private int _health;

    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;
    private Animator _animator;
    private bool _isAlive = true;

    private AudioSource _audioSource;
    private ZVoice _voice;

    public bool _playerShoot;

    void Start()
    {
        _voice = GetComponent<ZVoice>();
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();   
        _rigidbody= GetComponent<Rigidbody>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public void Hit(int damage)
    {
        _playerShoot = true;
        GetComponent<ZMove>().CallGroupOfZombies();
        GetComponent<ZMove>().SlowDown();
        _health -= damage;
        if (_health <= 0 && _isAlive)
        {
            Die();
        }
    }

    private void Die()
    {
        _rigidbody.useGravity = false;
        GetComponent<NavMeshAgent>().enabled = false;

        _animator.SetBool("Dead", true);
        _animator.SetLayerWeight(1, 0);
        GetComponent<ZMove>().enabled = false;        
        
        int randomNumber = Random.Range(0, 4);
        _animator.SetTrigger("Die" + randomNumber);

        _isAlive = false;
        _capsuleCollider.enabled = false;

        _audioSource.enabled = false;
        _voice.enabled = false;

        Destroy(gameObject, 5);
    }

    public void PlayerDetected()
    {
        StartCoroutine(PlayerDetect());
    }

    private IEnumerator PlayerDetect()
    {
        float randomTime = Random.Range(0.1f, 1f);

        yield return new WaitForSeconds(randomTime);

        // Обратный отсчет завершен, можно выполнить какое-либо действие.
        _playerShoot = true;
    }
}
