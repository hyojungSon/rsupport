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
        // 생성하고자 하는 게임오브젝트명, 생성될 위치, 오브젝트의 회전값
        var clone = Instantiate(bulletPrefab, laser.position, laser.rotation);
        clone.GetComponent<Bullet>().owner = PhotonNetwork.NickName;
        clone.GetComponent<Bullet>().player = _player;
    }
}
