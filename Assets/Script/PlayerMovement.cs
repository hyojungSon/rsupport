using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    /// <summary>
    /// �÷��̾� ����
    /// </summary>
    [SerializeField] private Color[] colors = null;

    //public static Action<int> OnCurrentControllerChange;
    [SerializeField] public Action OnAttack;

    const float GRAVITY = -9.81f;

    Collider col;

    Vector3 velocity = Vector3.zero;

    int currentController = 0;

    bool isGrounded = false;
    bool wasGrounded = false;

    [Header("Properties"), SerializeField] CharacterController controller;
    [SerializeField] Animator animator;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float gravityScale = 1f;
    [SerializeField] float jumpHeight = 1f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] AnimatorOverrideController[] controllers;

    [Header("Controlls"), SerializeField] KeyCode jumpKeyCode = KeyCode.Space;
    [SerializeField] KeyCode runKeyCode = KeyCode.LeftShift;
    [SerializeField] KeyCode attackKeyCode = KeyCode.Mouse0;

    public float Speed => IsRunning ? runSpeed : walkSpeed;
    public bool IsRunning => Input.GetKey(runKeyCode);
    public bool IsMoving { get; set; }
    public bool isJumping { get; set; }

    /// <summary>
    /// ü��
    /// </summary>
    [SerializeField] public int hp { get; private set; }

    /// <summary>
    /// ĳ���� �������� (Default: false)
    /// </summary>
    private bool isDead = false;
    /// <summary>
    /// ĵ������ ��ư���� ��Ʈ�� ��� ����
    /// </summary>
    public bool is2dControl { get; set; }

    
    public float rotateSpeed = 2.0f;

    [SerializeField] private GameObject _cam;
    [SerializeField] private Transform _eyes;
    [SerializeField] private CanvasManager _canvas;

    /// <summary>
    /// player NickName ǥ��
    /// </summary>
    [SerializeField] private TextMesh playerName = null;


    public float zoomSpeed = 10.0f;

    /// <summary>
    /// Camera Zoom : �þ� �ִ�
    /// </summary>
    private float MaxFieldOfView = 45.0f;
    /// <summary>
    /// Camera Zoom : �þ� �ּ�
    /// </summary>
    private float MinFieldOfView = 35.0f;


    private void Start()
    {
        if (photonView.IsMine)
        {
            isDead = false;
            is2dControl = false;
            hp = 100;

            //Cursor.lockState = CursorLockMode.Locked;
            col = controller.GetComponent<Collider>();

            _cam = GameObject.Find("Main Camera");
            _eyes = this.FindChild("Eyes");

            
         
            if(GameObject.Find("Canvas"))
            {
                GameObject.Find("Canvas").GetComponent<CanvasManager>()._player = this;
            }
        }

        playerName = this.transform.GetChild(1).GetChild(0).GetComponent<TextMesh>();

        UpdateCurrentController();
    }

    /// <summary>
    /// �±׸�� ������ ������Ʈ ã��
    /// </summary>
    /// <returns></returns>
    private Transform FindChild(string tagName)
    {
        Transform obj = null;

        foreach (Transform tr in this.transform.GetChild(0))
        {
            if (tr.tag == tagName)
            {
                obj = tr;
            }
        }

        return obj;
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (isDead)
            return;

        IsGrounded();
        Gravity();

        if(!this.is2dControl)
        {
            Inputs();
            Movement();

            //ī�޶� ȸ��
            Rotate();
            Zoom();
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void IsGrounded()
    {
        // Is grounded
        isGrounded = IsGroundedSphere(col, controller.radius, groundMask, true);

        if (isGrounded && velocity.y < 0)
            velocity.y = -5f;

        // Play sound fx on land
        if (isGrounded && !wasGrounded)
            AudioManager.Instance.Play("Land");

        wasGrounded = isGrounded;
        animator.SetBool("Float", !isGrounded);
    }

    bool IsGroundedSphere(Collider collider, float radius, LayerMask groundMask, bool debug = false)
    {
        var groundCheckPos = Vector3X.IgnoreY(transform.position, collider.bounds.min.y);
        return Physics.CheckSphere(groundCheckPos, radius, groundMask);
    }

    /// <summary>
    /// Jump : �÷��̾ ���� ���� �� ����
    /// attackKeyCode ���ý� Attack�� ���� (�Ѿ��� ���� �÷��̾��� hp ����!)
    /// </summary>
    private void Inputs()
    {
        if (Input.GetKeyDown(jumpKeyCode))
            Jump();

        if (Input.GetKeyDown(attackKeyCode))
            RunAttack();
    }

    /// <summary>
    /// �߷�ȿ��..
    /// </summary>
    private void Gravity()
    {
        velocity.y = velocity.y + GRAVITY * gravityScale * Time.deltaTime;
    }

    /// <summary>
    /// Player �̵�
    /// </summary>
    private void Movement()
    {
        if (!isGrounded)
            return;

        var moveX = Input.GetAxis("Horizontal");
        var moveY = Input.GetAxis("Vertical");

        test(moveX, moveY);
    }

    public void test(float moveX, float moveY)
    {
        const float IS_MOVING_MIN_MAG = .02f;

        var move = (transform.right * moveX + transform.forward * moveY).normalized;

        animator.SetFloat("MoveX", GetAnimMoveVal(moveX, animator.GetFloat("MoveX")));
        animator.SetFloat("MoveY", GetAnimMoveVal(moveY, animator.GetFloat("MoveY")));

        controller.Move(move * Speed * Time.deltaTime);
        IsMoving = move.magnitude >= IS_MOVING_MIN_MAG;


        _cam.transform.position = _eyes.position;
    }

    float GetAnimMoveVal(float move, float animCurVal)
    {
        const float SMOOTH_TIME = 10f;
        const float WALK_VAL = 1f;
        const float RUN_VAL = 2f;
        var newVal = move * (IsRunning ? RUN_VAL : WALK_VAL);
        var res = Mathf.Lerp(animCurVal, newVal, SMOOTH_TIME * Time.deltaTime);
        return newVal;
    }

    /// <summary>
    /// ĳ���͸� ȸ��
    /// (ĳ������ �þ� ���� ī�޶� ȸ��)
    /// </summary>
    private void Rotate()
    {
        Vector3 rot = transform.rotation.eulerAngles; // ���� ī�޶��� ������ Vector3�� ��ȯ

        rot.y += Input.GetAxis("Mouse X") * rotateSpeed; // ���콺 X ��ġ * ȸ�� ���ǵ�
                                                         //rot.x += Input.GetAxis("Mouse Y") * rotateSpeed; // ���콺 Y ��ġ * ȸ�� ���ǵ�
        rot.x = transform.rotation.x;

        rot.z = 0;

        Quaternion q = Quaternion.Euler(rot); // Quaternion���� ��ȯ
        q.z = 0;

        //Quaternion.Slerp : ���� ȸ�� �� ���� ��ǥ�� ȸ���� �����ð��� �ξ� ����
        transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f);

        _cam.transform.rotation = Quaternion.Slerp(transform.rotation, q, 2f);

    }

    /// <summary>
    /// Camera zoom(���콺 ��)
    /// </summary>
    private void Zoom()
    {
        float distance = Input.GetAxis("Mouse ScrollWheel") * -1 * zoomSpeed;

        float _fieldOfView = _cam.GetComponent<Camera>().fieldOfView;
        _fieldOfView += distance;

        if (distance > 0 && _fieldOfView >= MaxFieldOfView)
            _fieldOfView = MaxFieldOfView;
        else if (distance < 0 && _fieldOfView <= MinFieldOfView)
            _fieldOfView = MinFieldOfView;
    }

    void UpdateCurrentController()
    {
        animator.runtimeAnimatorController = controllers[currentController];
    }

    public void Jump()
    {
        if (isGrounded == false)
            return;

        isJumping = true;
        AudioManager.Instance.Play("Jump");
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * GRAVITY * gravityScale);

    }
    public void RunAttack()
    {
        this.GetComponent<PhotonView>().RPC("Attack", RpcTarget.All);
    }

    [PunRPC]
    void Attack()
    {
        animator.SetTrigger("Attack");
        OnAttack?.Invoke();
    }

    [PunRPC]
    public void NicknameDisplay(string name)
    {
        if (playerName != null)
            playerName.text = name;
    }

    [PunRPC]
    // ���� ���� �Լ�
    public void SetMaterial(int _playerNum)
    {
        if (_playerNum > colors.Length) return;

        animator.gameObject.GetComponent<Renderer>().material.color = colors[_playerNum - 1];
    }

    public void OnDamage(int _dmg)
    {
        this.GetComponent<PhotonView>().RPC("ApplyHp", RpcTarget.Others, _dmg);
    }

    [PunRPC]
    void ApplyHp(int _dmg)
    {
        this.hp -= _dmg;

        if (this.hp <= 0)
        {
            this.hp = 0;
            isDead = true;
        }
    }
}
