using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonLauncher : MonoBehaviourPunCallbacks
{
    // ���ӹ���, ����� ���� ���� �� ���
    [SerializeField] private string gameVersion = "0.0.1";
    // �� �濡 �ִ� �ο���
    [SerializeField] private byte maxPlayerPerRoom = 3;
    // �г���
    [SerializeField] private string nickName = string.Empty;

    // ��ȣ�ۿ� ��ư
    [SerializeField] private Button connectButton = null;

    private void Awake()
    {
        // �����Ͱ� PhotonNetwork.LoadLevel()�� ȣ���ϸ�,
        // ��� �÷��̾ ������ ������ �ڵ����� �ε�
        // Sync: ����ȭ, Async: �񵿱�
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        // ���ӿ� ��ư, ��ư �Է½� ��ȣ�ۿ� ��� ��뿩��
        connectButton.interactable = true;
    }

    // ConnectButton�� �������� ȣ��
    public void Connect()
    {
        // �г����� ���� ��� �Լ� ����
        if (string.IsNullOrEmpty(nickName.Trim()))
        {
            // �г��� ���������
            Debug.Log("NickName is empty");
            // �Լ�����
            return;
        }
        // ����� ���� �濡 ����
        // Photon ����Ʈ���� ���� ���ø����̼ǿ� ������ �Ǿ��°�
        else if(PhotonNetwork.IsConnected)
        {
            // ������ �濡 �����ϱ�
            PhotonNetwork.JoinRandomRoom();
        }
        // ���� ������ �ȵǾ��ٸ�
        else
        {
            Debug.LogFormat("Connect : {0}", gameVersion);
            // ����� ������ ���ӹ��� ��Ī
            PhotonNetwork.GameVersion = gameVersion;
            // ���� Ŭ���忡 ������ �����ϴ� ����
            // ���ӿ� �����ϸ� OnConnectedToMaster �޼��� ȣ�� -> �̷��� ó���� ���� �ݹ� ��ӹ���
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void OnValueChangedNickName(string _nickName)
    {
        nickName = _nickName;
        // �����̸� ����
        PhotonNetwork.NickName = nickName;
    }

    // �����Ϳ� ����(������ ������ ����), �濡 ���Ͱ� �ٸ�
    public override void OnConnectedToMaster()
    {
        Debug.LogFormat("Connected to Master: {0}", nickName);
        // ���� ���� ��ư ��ȣ�ۿ� ���ʿ�
        connectButton.interactable = false;
        // �������濡 ����
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// ���� ������ ��, ���� ���� ������ ���� 
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        // ���� ���� ���� ������������ �α� ���
        Debug.LogWarningFormat("Disconnected: {0}", cause);
        // �ٽ� ������ �� �ְ� ��ư Ȱ��ȭ
        connectButton.interactable = true;

        // ���� ����
        Debug.Log("Create Room");
        // ���̸����� ã�ư� ��� null�� ���̸�
        // RoomOptions �����Ҵ��ؼ� MaxPlayers�� ���� ����
        // ���� �����ϸ� OnJoinedRoom ȣ��
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }

    /// <summary>
    /// �游������� �ڵ����� ȣ��
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");

        // �����Ͱ� ���ÿ� ������ �����ϰ��ϴ� ������ �ƴϱ� ������ ���� ���� �θ��� ��, ���ϴ� ��Ŀ� ���� �ڵ��߰�
        //PhotonNetwork.LoadLevel("Room"); ����޾Ƽ� ó���� ��� ��, ����Ȯ���ؼ� ���( �ڽ��� �������ΰ� �˻�,
        SceneManager.LoadScene("Room");
    }

    /// <summary>
    /// ������ �� ������ ������ ���, 1) ���̾���. 2) �ο� �� ������
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message">����</param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("JoinRandomFailed({0}): {1}", returnCode, message);
        // ��ǻ� ���ʿ��� �ڵ�
        connectButton.interactable = true;
        Debug.Log("Create Room");
        // �� ����
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayerPerRoom });
    }
}
