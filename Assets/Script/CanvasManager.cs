using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 2D로 플레이어의 상태를 표현
/// </summary>
public class CanvasManager : MonoBehaviourPun
{
    public PlayerMovement _player = null;
    public bool showCanvas { get; private set; }

    private GameObject focus;
    private GameObject controlGrp;
    private GameObject playerInfo;
    private Text hp;
    private GameObject result;

    void Start()
    {
        showCanvas = true;

        if (controlGrp == null)
        {
            focus = FindChild("Focus");
            controlGrp = FindChild("Controller");
            playerInfo = FindChild("PlayerInfo");

            if (playerInfo != null)
            {
                hp = playerInfo.transform.GetChild(1).GetComponent<Text>();
            }

            result = FindChild("GameResultView");
        }
    }

    /// <summary>
    /// 오브젝트 이름으로 찾기!
    /// </summary>
    /// <returns></returns>
    private GameObject FindChild(string tagName)
    {
        GameObject obj = null;

        foreach (Transform tr in this.transform)
        {
            if (tr.name == tagName)
            {
                obj = tr.gameObject;
            }
        }

        return obj;
    }

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
            return;

        hp.text = "hp : " + _player.hp.ToString();

        if(_player.hp <= 0)
        {
            ShowGameResult(true);
        }

    }

    public void OnChangeToggle(bool bActive)
    {
        showCanvas = bActive;

        controlGrp.SetActive(showCanvas);
        _player.is2dControl = bActive;
    }

    public void ShowGameResult(bool bActive)
    {
        result.SetActive(bActive);
        focus.SetActive(!bActive);
    }

    #region Controller

    public void Movement(int dir)
    {
        switch (dir)
        {
            case 0: //forward
                _player.test(0, 1);
                break;

            case 1: //back
                _player.test(0, -1);
                break;

            case 2: //left
                _player.test(-1, 0);
                break;

            case 3: //right
                _player.test(1, 0);
                break;
        }
    }

    public void Jump()
    {
        _player.Jump();
    }

    public void Attack()
    {
        _player.RunAttack();
    }
    #endregion Controller

    public void btnQuit()
    {
        PhotonNetwork.LoadLevel(0);
    }
}
