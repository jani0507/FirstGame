using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum Firemode { Auto,Burst,Single}
    public Firemode fireMode;

    public Transform[] projectileSpawn;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public int burstCount;

    public Transform shell;
    public Transform shellEjection;
    Muzzleflash muzzleflash;
    float nextShotTime;

    bool triggerReleasedSinceLastShot;
    int shootsRemaininginBurst;

    private void Start()
    {
        muzzleflash = GetComponent<Muzzleflash>();
        shootsRemaininginBurst = burstCount;
    }
    

     void Shoot()
    {
        if (Time.time > nextShotTime)
        {
            if(fireMode == Firemode.Burst)
            {
                if(shootsRemaininginBurst == 0)
                {
                    return;
                }
                shootsRemaininginBurst--;
            }
            else if(fireMode == Firemode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < projectileSpawn.Length; i++)
            {
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellEjection.position, shellEjection.rotation);
            muzzleflash.Activate();
        }
    }

    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shootsRemaininginBurst = burstCount;

    }
}
