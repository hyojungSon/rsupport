using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    // 게임버전, 포톤과 버전 맟출 때 사용
    [SerializeField] private string gameVersion = "0.0.1";
    // 한 방에 최대 인원수
    [SerializeField] private byte maxPlayerPerRoom = 3;
    // 닉네임
    [SerializeField] private string nickName = string.Empty;

    // 상호작용 버튼
    [SerializeField] private Button connectButton = null;

    private void Awake()
    {
        // 마스터가 PhotonNetwork.LoadLevel()을 호출하면,
        // 모든 플레이어가 동일한 레벨을 자동으로 로드
        // Sync: 동기화, Async: 비동기
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        // 접속용 버튼, 버튼 입력시 상호작용 기능 사용여부
        connectButton.interactable = true;
    }

    // ConnectButton이 눌러지면 호출
    public void Connect()
    {
        // 닉네임이 없을 경우 함수 종료
        if (string.IsNullOrEmpty(nickName.Trim()))
        {
            // 닉네임 비어있으면
            Debug.Log("NickName is empty");
            // 함수종료
            return;
        }
        // 연결시 랜덤 방에 입장
        // Photon 사이트에서 만든 애플리케이션에 접속이 되었는가
        else if(PhotonNetwork.IsConnected)
        {
            // 무작위 방에 참가하기
            PhotonNetwork.JoinRandomRoom();
        }
        // 아직 접속이 안되었다면
        else
        {
            Debug.LogFormat("Connect : {0}", gameVersion);
            // 포톤상 버전과 게임버전 매칭
            PhotonNetwork.GameVersion = gameVersion;
            // 포톤 클라우드에 접속을 시작하는 지점
            // 접속에 성공하면 OnConnectedToMaster 메서드 호출 -> 이런거 처리를 위해 콜백 상속받음
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnValueChangedNickName(string _nickName)
    {
        nickName = _nickName;
        // 유저이름 지정
        PhotonNetwork.NickName = nickName;
    }

    // 마스터에 접속(마스터 서버에 접속), 방에 들어간것과 다름
    public override void OnConnectedToMaster()
    {
        Debug.LogFormat("Connected to Master: {0}", nickName);
        // 접속 이후 버튼 상호작용 불필요
        connectButton.interactable = false;
        // 무작위방에 참가
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// 접속 끊겼을 때, 인자 값은 끊겨진 이유 
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 접속 끊긴 이유 워닝포맷으로 로그 출력
        Debug.LogWarningFormat("Disconnected: {0}", cause);
        // 다시 접속할 수 있게 버튼 활성화
        connectButton.interactable = true;

        // 방을 생성
        Debug.Log("Create Room");
        // 방이름으로 찾아갈 경우 null에 방이름
        // RoomOptions 동적할당해서 MaxPlayers의 값만 저장
        // 방을 생성하면 OnJoinedRoom 호출
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    /// <summary>
    /// 방만들어지면 자동으로 호출
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");

        // 마스터가 동시에 게임을 시작하게하는 구조가 아니기 때문에 각자 씬을 부르면 됨, 원하는 방식에 따라 코드추가
        //PhotonNetwork.LoadLevel("Room"); 레디받아서 처리할 경우 단, 조건확인해서 사용( 자신이 마스터인가 검사,
        SceneManager.LoadScene("Room");
    }

    /// <summary>
    /// 무작위 방 참가에 실패한 경우, 1) 방이없다. 2) 인원 수 가득참
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message">원인</param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("JoinRandomFailed({0}): {1}", returnCode, message);
        // 사실상 불필요한 코드
        connectButton.interactable = true;
        Debug.Log("Create Room");
        // 방 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }
}
