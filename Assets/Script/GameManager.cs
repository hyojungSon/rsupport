using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// �������� ������ �÷��̾��� ������
    /// </summary>
    [SerializeField] private GameObject playerPrefab = null;

    /// <summary>
    /// �÷��̾� ��ġ �ʱⰪ
    /// </summary>
    [SerializeField] private Vector3[] playerPosition = null;

    //[SerializeField] private PhotonView pv = null;

    GameObject go;

    // Start is called before the first frame update
    void Awake()
    {
        if (playerPrefab == null)
            playerPrefab = Resources.Load("Player") as GameObject;
    }

    void Start()
    {
        if (PhotonNetwork.CountOfPlayers == 0)
        {
            PhotonNetwork.LeaveRoom();
            return;
        }

        if (playerPrefab != null)
        {
            // �����Ʈ��ũ�� �ִ� �ν��Ͻÿ���Ʈ, �濡 �ִ� ��� �������� ������ �������
            go = PhotonNetwork.Instantiate(playerPrefab.name,
                                            playerPosition[PhotonNetwork.CountOfPlayers - 1],
                                            Quaternion.identity, 0);

            //pv = go.GetPhotonView();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerPrefab != null)
        {
            PhotonView pv = go.GetPhotonView();

            pv.RPC("SetMaterial", RpcTarget.All, PhotonNetwork.CountOfPlayers);
            pv.RPC("NicknameDisplay", RpcTarget.All, PhotonNetwork.NickName);
        }
    }

    // PhotonNetwork.LeaveRooom �Լ��� ȣ��Ǹ� ȣ��
    // �� ������ �� ȣ��
    public override void OnLeftRoom()
    {
        // �泪���ٴ� �α�
        Debug.Log("Left Room");
        // ��ó ȣ��
        SceneManager.LoadScene("PhotonLauncher");
    }

    // �÷��̾ ������ �� ȣ��Ǵ� �Լ�, ������ �÷��̾� ���ڰ����� ����
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        // ���� �����ߴ��� ���
        Debug.LogFormat("Player Entered Room: {0}", otherPlayer.NickName);
    }

    // �÷��̾ ���� �� ȣ��Ǵ� �Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player Left Room: {0}", otherPlayer.NickName);
    }

    // �泪����� �� ȣ��
    public void LeaveRoom()
    {
        // ��ư���� ȣ���ϸ� �泪���� ��
        PhotonNetwork.LeaveRoom();
    }
}
