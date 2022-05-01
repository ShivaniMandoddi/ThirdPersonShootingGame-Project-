using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float playerSpeed;
    public float rotationSpeed;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement =new Vector3(Input.GetAxis("Horizontal") * playerSpeed,0f, Input.GetAxis("Vertical") * playerSpeed);
       
        transform.Translate(movement.x*Time.deltaTime, 0f, movement.z*Time.deltaTime);
        if (movement.x!=0 || movement.z!=0)
            animator.SetFloat("Blend",movement.magnitude);
        float rotateX = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(0f, rotateX, 0f);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("IsAttack");
        }

    }

}
