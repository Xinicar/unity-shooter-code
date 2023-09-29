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

    [SerializeField] private int damage = 10;         // Урон, наносимый игроку.
    [SerializeField] private float attackRange = 2f;  // Радиус атаки врага.
    [SerializeField] private LayerMask playerLayer;   // Слой, на котором находится игрок.
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
        // Рисуем красный радиус обнаружения в редакторе Unity.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _playerDetectionArea);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _playerAttackZone);
    }

    private void MoveToPlayer(float distanceToPlayer)
    {
        // Если игрок находится в заданном радиусе (например, 10 единиц), начинаем движение.
        if (distanceToPlayer <= _playerDetectionArea || GetComponent<ZombieDamageController>()._playerShoot)
        {
            // Если объект не движется, начинаем движение.
            if (_moveToPlayerToAttack)
            {
                _animator.SetTrigger("Run");

                // Перемещаем объект вперед в направлении игрока с учетом скорости.
                _agent.destination = _playerObject.gameObject.transform.position;
            }

        }
        else
        {
            // Если игрок не находится в радиусе обнаружения, останавливаем движение и сбрасываем флаг.
            _animator.SetTrigger("Idle");
        }
    }

    private void RotateToPlayer(float distanceToPlayer)
    {
        if (distanceToPlayer <= _playerDetectionArea || GetComponent<ZombieDamageController>()._playerShoot)
        {
            // Вычисляем направление к игроку.
            Vector3 direction = (_playerObject.transform.position - transform.position).normalized;

            // Вычисляем ориентацию, направленную к игроку.
            targetRotation = Quaternion.LookRotation(direction);

            // Плавно поворачиваем объект к игроку.
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
            yield return new WaitForSeconds(1f); // Ждем 1 секунду.

            currentTime -= 1f;
        }

        // Обратный отсчет завершен, можно выполнить какое-либо действие.
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
        // Создаем луч, начиная от позиции врага и направленный вперед.
        Ray ray = new Ray(_ray.position, transform.forward);
        RaycastHit hit;

        // Проверяем, сталкивается ли луч с объектом игрока.
        if (Physics.Raycast(ray, out hit, _playerAttackZone, playerLayer))
        {
            // Если луч сталкивается с игроком, вызываем метод ApplyDamage() у игрока.
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
