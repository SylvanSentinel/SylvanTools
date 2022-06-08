using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{

    //Put on parent of whatever you want to recoil (weapon in this case)
    //Set the settings and call Recoil() to shake

    private Transform recoilPosition;
    private Transform rotationPoint;

    [SerializeField] WeaponRecoilSettings settings;

    Vector3 rotationalRecoil;
    Vector3 positionalRecoil;
    Vector3 rot;

    private void Awake()
    {
        recoilPosition = this.transform;
        rotationPoint = this.transform;
    }

    public void ChangeSettings(WeaponRecoilSettings newSet)
    {
        settings = newSet;
    }

    private void FixedUpdate()
    {
        //if game paused return

        rotationalRecoil = Vector3.Lerp(rotationalRecoil, Vector3.zero, settings.rotaionalReturnSpeed * Time.deltaTime);
        positionalRecoil = Vector3.Lerp(positionalRecoil, Vector3.zero, settings.positionalReturnSpeed * Time.deltaTime);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionalRecoil, settings.positionalRecoilSpeed * Time.fixedDeltaTime);
        rot = Vector3.Slerp(rot, rotationalRecoil, settings.rotationalRecoilSpeed * Time.fixedDeltaTime);
        rotationPoint.localRotation = Quaternion.Euler(rot);
    }

    public void Recoil()
    {
        //if game paused return

        rotationalRecoil += new Vector3(-settings.RecoilRotation.x, Random.Range(-settings.RecoilRotation.y, settings.RecoilRotation.y), Random.Range(settings.RecoilRotation.z, settings.RecoilRotation.z));
        positionalRecoil += new Vector3(Random.Range(-settings.RecoilKickBack.x, settings.RecoilKickBack.x), Random.Range(-settings.RecoilKickBack.y, settings.RecoilKickBack.y), settings.RecoilKickBack.z);
    }
}

[System.Serializable]
public class WeaponRecoilSettings
{
    [Header("Speed Settings")]
    public float positionalRecoilSpeed = 8f;
    public float rotationalRecoilSpeed = 8f;
    [Space(10)]
    public float positionalReturnSpeed = 18f;
    public float rotaionalReturnSpeed = 38f;
    [Space(10)]
    [Header("Amount Settings")]
    public Vector3 RecoilRotation = new Vector3(10, 5, 7);
    public Vector3 RecoilKickBack = new Vector3(.015f, 0f, -.2f);
}
