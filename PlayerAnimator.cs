using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameInput gameInput;


    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

    }

    //Use start not Awake for events cause it happens after
    void Start() {
        gameInput.OnEnforceAction += GameInput_OnEnforceAction;
    }

    private void GameInput_OnEnforceAction(object sender, System.EventArgs e) {
        animator.Play("Idle", -1, 0f);
    }

    private void Update()
    {
        animator.SetBool("isWalkingForward", player.IsWalkingForward());

        if (player.IsSideStepping()) {
            float inputX = Input.GetAxisRaw("Horizontal");

            // Mirror the animation based on input direction
            if (inputX < 0) {
                animator.SetBool("isSideSteppingLeft", true);
            } else if (inputX > 0) {
                animator.SetBool("isSideSteppingRight", true);
            }
        } else {
            animator.SetBool("isSideSteppingRight", false);
            animator.SetBool("isSideSteppingLeft", false);
        }
    }
}
