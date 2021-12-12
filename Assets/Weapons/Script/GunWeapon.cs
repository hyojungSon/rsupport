using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunWeapon : Weapon
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform laser;

    protected override void Attack()
    {
        base.Attack();

        if (!IsAttacking)
            SpawnBullet();
    }

    void SpawnBullet()
    {
        // �����ϰ��� �ϴ� ���ӿ�����Ʈ��, ������ ��ġ, ������Ʈ�� ȸ����
        var clone = Instantiate(bulletPrefab, laser.position, laser.rotation);
        clone.GetComponent<Bullet>().owner = PhotonNetwork.NickName;
        clone.GetComponent<Bullet>().player = _player;
    }
}
