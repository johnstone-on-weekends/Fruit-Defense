using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollicleAttack : MonoBehaviour
{

    [SerializeField] private float directionDegrees;
    private Vector3 _directionVector;
    private float _moveSpeed;
    private Collider2D _rangeCollider;
    private Collider2D _collider;
    
    // Start is called before the first frame update
    void Start()
    {
        float radians = directionDegrees * Mathf.Deg2Rad;
        _collider = GetComponent<Collider2D>();
        _directionVector = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0);
        _moveSpeed = GetComponentInParent<HairShooting>().GetFollicleAttackSpeed();
        _rangeCollider = GetComponentInParent<HairShooting>().GetRangeCollider();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.position += _directionVector * (_moveSpeed * Time.fixedDeltaTime);
        if (!_rangeCollider.bounds.Contains(transform.position))
            Destroy(gameObject);
    }
    
    private void OnTriggerEnter2D (Collider2D other) 
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Enemy" && !other.GetComponent<Enemy>().IsDead())
        {
            other.GetComponent<Enemy>().LoseLife(1);
            Destroy(gameObject);
        }
    }
}