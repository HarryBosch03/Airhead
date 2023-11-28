using System;
using Airhead.Runtime.Entities;
using UnityEngine;

namespace Airhead.Runtime.Player
{
    
    public class PlayerAnimator : MonoBehaviour
    {
        public Animator animator;
        
        private BipedController biped;

        private void Awake()
        {
            biped = GetComponent<BipedController>();
        }

        private void FixedUpdate()
        {
            animator.SetBool("grounded", biped.IsOnGround);
            animator.SetFloat("movement.x", biped.IsOnGround ? biped.body.velocity.x : 0.0f);
            animator.SetFloat("movement.y", biped.IsOnGround ? biped.body.velocity.z : 0.0f);
        }
    }
}
