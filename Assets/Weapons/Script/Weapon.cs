using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    const string ATTACK_ANIM = "Attack";
    [SerializeField] Animator animator;
    public PlayerMovement _player = null;

    protected bool IsAttacking => animator.GetCurrentAnimatorStateInfo(0).IsName(ATTACK_ANIM);

    private void Awake()
    {
        if (_player == null)
            _player = this.transform.parent.parent.parent.GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        if (_player != null)
            _player.OnAttack += Attack;
    }

    private void OnDisable()
    {
        if (_player != null)
            _player.OnAttack -= Attack;
    }

    protected virtual void Attack()
    {
        if (!IsAttacking)
            animator.SetTrigger(ATTACK_ANIM);
    }
}
