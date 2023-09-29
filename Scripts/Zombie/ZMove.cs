using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZMove : MonoBehaviour
{
    GameObject _playerObject;
    Animator _animator;
    NavMeshAgent _agent;

    [SerializeField] float _rotationSpeed;
    [SerializeField] float _playerDetectionArea;
    [SerializeField] float _playerAttackZone;
    [SerializeField] LayerMask _layerMaskGet;

    [SerializeField] private int damage = 10;         // ����, ��������� ������.
    [SerializeField] private float attackRange = 2f;  // ������ ����� �����.
    [SerializeField] private LayerMask playerLayer;   // ����, �� ������� ��������� �����.
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private Transform _ray;
    private bool canAttack = true;

    private bool _moveToPlayerToAttack = true;
    private Quaternion targetRotation;
    private float _startChaseTimer = 1f;
    private float _actualSpeed;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _actualSpeed = _agent.speed;
        _animator = GetComponent<Animator>();
        _playerObject = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, _playerObject.transform.position);

        MoveToPlayer(distanceToPlayer);
        AttackPlayer(distanceToPlayer);
        RotateToPlayer(distanceToPlayer);
    }
    private void OnDrawGizmosSelected()
    {
        // ������ ������� ������ ����������� � ��������� Unity.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerDetectionArea);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _playerAttackZone);
    }

    private void MoveToPlayer(float distanceToPlayer)
    {
        // ���� ����� ��������� � �������� ������� (��������, 10 ������), �������� ��������.
        if (distanceToPlayer <= _playerDetectionArea || GetComponent<ZombieDamageController>()._playerShoot)
        {
            // ���� ������ �� ��������, �������� ��������.
            if (_moveToPlayerToAttack)
            {
                _animator.SetTrigger("Run");

                // ���������� ������ ������ � ����������� ������ � ������ ��������.
                _agent.destination = _playerObject.gameObject.transform.position;
            }

        }
        else
        {
            // ���� ����� �� ��������� � ������� �����������, ������������� �������� � ���������� ����.
            _animator.SetTrigger("Idle");
        }
    }

    private void RotateToPlayer(float distanceToPlayer)
    {
        if (distanceToPlayer <= _playerDetectionArea || GetComponent<ZombieDamageController>()._playerShoot)
        {
            // ��������� ����������� � ������.
            Vector3 direction = (_playerObject.transform.position - transform.position).normalized;

            // ��������� ����������, ������������ � ������.
            targetRotation = Quaternion.LookRotation(direction);

            // ������ ������������ ������ � ������.
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
    }

    private void AttackPlayer(float distanceToPlayer)
    {
        if (distanceToPlayer <= _playerAttackZone && canAttack)
        {
            _moveToPlayerToAttack = false;
            _animator.SetTrigger("Idle");
            _animator.SetBool("Attack", true);

            _animator.SetLayerWeight(1, 1f);
            Attack();
        }
        else if (!_moveToPlayerToAttack && distanceToPlayer > _playerAttackZone)
        {
            _animator.SetLayerWeight(1, 0);
            StartCoroutine(StartChasing());
        }
        else if (distanceToPlayer <= _playerAttackZone && !canAttack)
        {
            _animator.SetBool("Attack", false);
            _moveToPlayerToAttack = false;
            _animator.SetLayerWeight(1, 1f);
        }
    }

    private IEnumerator StartChasing()
    {
        float currentTime = _startChaseTimer;

        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f); // ���� 1 �������.

            currentTime -= 1f;
        }

        // �������� ������ ��������, ����� ��������� �����-���� ��������.
        _moveToPlayerToAttack = true;
    }

    public void CallGroupOfZombies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, _playerDetectionArea, _layerMaskGet);

        foreach (Collider enemy in enemies)
        {
            print(enemy);
            enemy.GetComponent<ZombieDamageController>().PlayerDetected();
        }
    }

    public void SlowDown()
    {
        StartCoroutine(Slow());
    }

    private IEnumerator Slow()
    {
        _agent.speed = 1f;
        yield return new WaitForSeconds(0.5f);
        _agent.speed = _actualSpeed;
    }

    public void Attack()
    {
        // ������� ���, ������� �� ������� ����� � ������������ ������.
        Ray ray = new Ray(_ray.position, transform.forward);
        RaycastHit hit;

        // ���������, ������������ �� ��� � �������� ������.
        if (Physics.Raycast(ray, out hit, _playerAttackZone, playerLayer))
        {
            // ���� ��� ������������ � �������, �������� ����� ApplyDamage() � ������.
            HealthControl playerHealth = hit.collider.GetComponent<HealthControl>();
            if (playerHealth != null)
            {
                GetComponent<AudioSource>().Stop();
                canAttack = false;
                playerHealth.DamagePlayer(damage);
                GetComponent<AudioSource>().PlayOneShot(_audioClip);
                StartCoroutine(CanHitPlayer());
                
            }
        }
    }

    private IEnumerator CanHitPlayer()
    {
/*        _animator.SetBool("Attack", false);
*/        yield return new WaitForSeconds(2f);
        canAttack = true;
    }
}
