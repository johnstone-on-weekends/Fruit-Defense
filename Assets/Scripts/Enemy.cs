using SingletonScripts;
using UnityEngine;
public class Enemy : MonoBehaviour
{
    
    
    [Header("Attributes")] 
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int lifePoints = 5;
    
    private Animator _animator;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    private bool _isDead;
    
    // This boolean determines if the enemy has been queued up in the queue that the towers pick their targets from. We only queue when the enemy becomes visible; we don't want shooting off the screen
    private bool _isQueued;
    
    private string _currentAnimation;
    private Transform _target;
    private int _nextWaypointIndex;
    
    

    public void LoseLife(int lifeLost)
    {
        lifePoints -= lifeLost;
        if (lifePoints <= 0)
        {
            LevelManager.Main.DequeueEnemy(false);
        }
    }

    private void Start()
    {
        _target = LevelManager.Main.path[_nextWaypointIndex];
        _spriteRenderer = gameObject.GetComponent <SpriteRenderer>();
        _animator = gameObject.GetComponent<Animator>();
        _rb =  gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!_isQueued && _spriteRenderer.isVisible)
        {
            LevelManager.Main.EnqueueEnemy(gameObject);
            _isQueued = true;
        }
        UpdateNextWaypoint();
    }

    private void FixedUpdate()
    {
        if (_isDead)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !_animator.IsInTransition(0))
            {
                Destroy(gameObject);
            }

            return;
        }
        
        Vector2 direction = (_target.position - transform.position).normalized;
        AnimateEnemy(direction);

        _rb.velocity = direction * moveSpeed;
    }

    private void AnimateEnemy(Vector2 direction)
    {
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;

        var newAnimation = angle switch
        {
            >= 45 and < 135 => "MoveUp",
            >= 135 and < 225 => "MoveLeft",
            >= 225 and < 315 => "MoveDown",
            _ => "MoveRight"
        };

        if (newAnimation != _currentAnimation)
        {
            _currentAnimation = newAnimation;
            _animator.SetTrigger(_currentAnimation);
        }
    }

    
    private void UpdateNextWaypoint()
    {
        if (!(Vector2.Distance(_target.position, transform.position) < 0.1f)) return;
        _nextWaypointIndex++;
            
        if (_nextWaypointIndex == LevelManager.Main.path.Length)
        {
            LevelManager.Main.DequeueEnemy(true);
            Destroy(gameObject);
        }
        else
        {
            _target = LevelManager.Main.path[_nextWaypointIndex];
        }
    }

    public void KillEnemy()
    {
        _isDead = true;
        _spriteRenderer.sortingOrder -= 1;
        _rb.velocity = Vector2.zero;
        _animator.SetTrigger("Bleed");
    }

    public bool IsDead()
    {
        return _isDead;
    }
}