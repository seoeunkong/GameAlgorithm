using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minifantasy
{
    public class SetAnimatorParameter : MonoBehaviour
    {
        private Animator animator;

        public string parameterName = "Idle";
        public float waitTime = 0f;

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            Invoke("ToggleAnimatorParameter", waitTime);
            //ToggleDirection();
        }

        public void ToggleAnimatorParameter()
        {
            animator.SetBool(parameterName, true);
        }

        public void ToggleDirection(Vector3 inputPos)
        {
            Vector3 dir = (new Vector3(inputPos.x - transform.position.x, 0 ,0 )).normalized;

            transform.GetComponentInChildren<SpriteRenderer>().flipX = (dir.x > 0) ? false : true;

        }
    }
}