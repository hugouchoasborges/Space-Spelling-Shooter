﻿using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class BulletController : MonoBehaviour
{
    #region Variables

    // Control the bullet Destroy
    private bool _toBeDestroyed;
    
    // Components
    private Rigidbody2D _rigidbody2D;
    public bool lastBullet;
    
    [Header("VFX Prefabs")] 
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    
    #endregion

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
        Assert.IsNotNull(_rigidbody2D, "The Bullet must have a RigidBody2D Component");
    }

    private void Start()
    {
        if (muzzlePrefab)
        {
            GameObject muzzleVfx = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVfx.transform.forward = transform.forward;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Enemy"))
            return;
            
        if (hitPrefab)
        {
            ContactPoint2D contactPoint2D = other.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contactPoint2D.normal);
            Vector2 pos = contactPoint2D.point;
            
            GameObject hitVfx = Instantiate(hitPrefab, pos, rot);
            hitVfx.transform.forward = transform.forward;
        }

        EnemyDisplay enemy = other.gameObject.GetComponent<EnemyDisplay>();
        _toBeDestroyed = true;
        StopAllCoroutines();

        if (lastBullet)
        {
            enemy.enemyManager.DestroyEnemy(other.gameObject);
        }
        Destroy(gameObject);
    }

    public IEnumerator ShootAtTarget(Transform target, float bulletSpeed)
    {
        if (!_rigidbody2D)
            yield break;

        float enemyRadius = target.GetComponent<CircleCollider2D>().radius;

        Vector2 bulletPosition = transform.position;
        Vector2 targetVector = (Vector2)target.position - bulletPosition;
        
        while (!_toBeDestroyed && targetVector.magnitude > enemyRadius)
        {
            bulletPosition = transform.position;
            targetVector = (Vector2)target.position - bulletPosition;
            _rigidbody2D.MovePosition(
                bulletPosition + bulletSpeed * Time.deltaTime * targetVector.normalized);
            yield return null;
        }
    }
}
