using System;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float Speed = 10f;
    public bool _toMove;
    
    private GameObject _projectilePrefab;
    private GameObject _projectile;

    private Transform _startPosition;
    private Transform _destination;
    private Vector3 _direction;
    
    private void Start()
    {
        _toMove = true;
    }

    private void Update()
    {
        _projectile.transform.position += _direction * (Speed * Time.deltaTime);

        var curPos = _projectile.transform.position;
        if (Math.Abs(curPos.x - _destination.position.x) < 0.1f && 
            Math.Abs(curPos.y - _destination.position.y) < 0.1f)
        {
            _destination.gameObject.GetComponent<TapCharacter>().OnProjectiveHit();
            Destroy(_projectile);
            Destroy(this);
        }
        
    }

    public void SetPositions(Transform startPos, Transform endPos)
    {
        _startPosition = startPos;
        _destination = endPos;
        CreateProjectile();
    }

    private void CreateProjectile()
    {
        _projectilePrefab = (GameObject)Resources.Load("Prefabs/Projectile", typeof(GameObject));
        _projectile = Instantiate(_projectilePrefab);
        
        var position = _startPosition.position;
        _projectile.transform.position = new Vector3(position.x, position.y, -0.5f);

        _direction = _destination.position - position;
        var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        _projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

        
    }
}
