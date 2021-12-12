using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// 동적으로 생성할 플레이어의 프리팹
    /// </summary>
    [SerializeField] private GameObject playerPrefab = null;

    /// <summary>
    /// 플레이어 위치 초기값
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
            // 포톤네트워크에 있는 인스턴시에이트, 방에 있는 모든 유저에게 프리팹 만들어줌
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

    // PhotonNetwork.LeaveRooom 함수가 호출되면 호출
    // 방 떠났을 때 호출
    public override void OnLeftRoom()
    {
        // 방나갔다는 로그
        Debug.Log("Left Room");
        // 런처 호출
        SceneManager.LoadScene("PhotonLauncher");
    }

    // 플레이어가 입장할 때 호출되는 함수, 접속한 플레이어 인자값으로 받음
    public override void OnPlayerEnteredRoom(Player otherPlayer)
    {
        // 누가 입장했는지 출력
        Debug.LogFormat("Player Entered Room: {0}", otherPlayer.NickName);
    }

    // 플레이어가 나갈 때 호출되는 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("Player Left Room: {0}", otherPlayer.NickName);
    }

    // 방나고싶을 때 호출
    public void LeaveRoom()
    {
        // 버튼으로 호출하면 방나가기 됨
        PhotonNetwork.LeaveRoom();
    }
}
