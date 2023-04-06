﻿using System;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    private const float _speed = 10f;

    private GameObject _projectilePrefab;
    private GameObject _projectile;

    private Transform _startPosition;
    private Transform _destination;
    private Vector3 _direction;

    private void Update()
    {
        // moving the projectile every tick
        _projectile.transform.position += _direction * (_speed * Time.deltaTime);

        // check if projectile hit the enemy
        var curPos = _projectile.transform.position;
        if (Math.Abs(curPos.x - _destination.position.x) < 0.3f && 
            Math.Abs(curPos.y - _destination.position.y) < 0.3f)
        {
            Destroy(_projectile);   // destroy the projectile
            Destroy(this);          // remove this component from Figure
            
            // call method "OnProjectiveHit"
            _destination.gameObject.GetComponent<TapCharacter>().OnProjectiveHit(_startPosition.gameObject);
        }
    }

	// setting a start and end positions of projectile
    public void SetPositions(Transform startPos, Transform endPos)
    {
        _startPosition = startPos;
        _destination = endPos;
        CreateProjectile();
    }

	// create a projectile when positions are setted
    private void CreateProjectile()
    {
        _projectilePrefab = (GameObject)Resources.Load("Prefabs/Projectile", typeof(GameObject)); // found prefab of projectile
        _projectile = Instantiate(_projectilePrefab); // Instatiate projectile
        
		// moving the projectile to start position
        var position = _startPosition.position;
        _projectile.transform.position = new Vector3(position.x, position.y, -0.5f);

		// rotating the projectile to end position
        _direction = _destination.position - position;
        var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        _projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
