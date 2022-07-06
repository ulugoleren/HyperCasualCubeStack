using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private Vector3 _initialTouchPosition;
    private Vector3 _currentTouchPosition;
    private bool _canMove = false;
    private Rigidbody _rigidbody;

    [SerializeField] private Animator animator;
    [SerializeField] private float moveThreshold;
    [SerializeField] private float speedMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        var currentDistance = 0f;
        if (Input.GetMouseButtonDown(0))
        {
            _initialTouchPosition = Input.mousePosition;
            _canMove = true;
        }
        else if (Input.GetMouseButton(0))
        {
            _currentTouchPosition = Input.mousePosition;
            currentDistance = Vector3.Distance(_currentTouchPosition, _initialTouchPosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _canMove = false;
        }

        if (_canMove && currentDistance > moveThreshold)
        {
            var direction = _currentTouchPosition - _initialTouchPosition;
            direction = direction.normalized;
            _rigidbody.velocity = new Vector3(direction.x, 0f, direction.y) * speedMultiplier;
            
            //transform.LookAt(transform.position + new Vector3(direction.x, 0f, direction.y));
            var target = transform.position + new Vector3(direction.x, 0f, direction.y);
            Quaternion lookRotation = Quaternion.LookRotation(target - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, .03f);
            animator.SetBool("isWalking" , true);
        }
        else
        {
            _rigidbody.velocity = Vector3.zero;
            animator.SetBool("isWalking" , false);
        }
    }
    
    
}
