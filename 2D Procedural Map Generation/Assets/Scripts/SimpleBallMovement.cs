using System.Collections.Generic;
using UnityEngine;

public class SimpleBallMovement : MonoBehaviour
{
    [SerializeField] private float _groundMovementSpeed, _airMovementSpeed;

    private Rigidbody2D _rb;
    private Vector2 _movementVector;
    private bool _isGrounded;

    private void Start ()
	{
	    _rb = GetComponent<Rigidbody2D>();
        Freeze(true);
	}
	
	private void FixedUpdate ()
	{

	    _movementVector.x = Input.GetAxis("Horizontal");

        if (_isGrounded)
	    {
	        _rb.AddForce(_movementVector * _groundMovementSpeed);
        }
        else
        {
            _rb.AddForce(_movementVector * _airMovementSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Floor"))
        {
            _isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag("Floor"))
        {
            _isGrounded = false;
        }
    }

    // Called by UI Button
    public void Freeze(bool setFreeze)
    {
        _rb.velocity = Vector2.zero;
        _rb.simulated = !setFreeze;
    }

    // Called by UI Button
    public void TeleportPlayerToMiddleOfMap()
    {
        var generator = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<ProceduralMapGenerator>();
        if (generator.mapLength <= 0)
        {
            return;
        }

        int middleTileX = generator.GetTilemapMiddleTile().x;
        List<Vector3Int> middleRowTile = new List<Vector3Int>();

        foreach (var tilePos in generator.SpawnedTilePositions)
        {
            if (tilePos.x == middleTileX)
            {
                middleRowTile.Add(tilePos);
            }
        }

        Vector3Int middleTilePos = middleRowTile[middleRowTile.Count / 2];

        if (generator.levelHeight <= 0)
        {
            transform.position = new Vector3(middleTilePos.x, middleTilePos.y + 2, 0);
            return;
        }

        transform.position = new Vector3(middleTilePos.x, middleTilePos.y + 0.5f, 0);
    }
}
