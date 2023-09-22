using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    private CharacterController controller;
    private Transform cam;
    private Vector3 moveDirection;
    public float gravity;
    public float colliderRadius;
    public float smoothRotTime;
    private float turnSmoothVelocity;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        getMouseInput();
    }

    private void Move()
    {
        if (controller.isGrounded)
        {
            //pega a entrada na horizontal (tecla direita/esquerda)
            float horizontal = Input.GetAxisRaw("Horizontal");
            //pega entrada na vertical (tecla cima/baixo)
            float vertical = Input.GetAxisRaw("Vertical");
            //vertical local que armazena o valor do eixo horizontal e vertical
            Vector3 direction = new Vector3(horizontal, 0f, vertical);

            if (direction.magnitude > 0)
            {
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity,
                    smoothRotTime);
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
                moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * speed;
                
                anim.SetInteger("transition", 1);
            }
            else
            {
                moveDirection = Vector3.zero;
                anim.SetInteger("transition", 0);
            }

        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    void getMouseInput()
    {
        if (controller.isGrounded)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        anim.SetInteger("transition", 2);
        yield return new WaitForSeconds(1f);
    }

    void GetEnemiesList()
    {
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position+transform.forward, colliderRadius);
    }
}