using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRecoil : MonoBehaviour
{
    //Put on parent of whatever you want to recoil (camera in this case)
    //Set the settings and call Recoil() to shake;

    [SerializeField] CameraRecoilSettings settings;

    private Vector3 currentRotation;
    private Vector3 rot;

    public void ChangeSettings(CameraRecoilSettings newSet)
    {
        settings = newSet;
    }


    private void FixedUpdate()
    {
        //if game paused return

        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, settings.returnSpeed * Time.deltaTime);
        rot = Vector3.Slerp(rot, currentRotation, settings.rotationSpeed * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(rot);
    }

    public void Recoil()
    {
        //if game paused return

        currentRotation += new Vector3(-settings.RecoilRotation.x, Random.Range(-settings.RecoilRotation.y, settings.RecoilRotation.y), Random.Range(-settings.RecoilRotation.z, settings.RecoilRotation.z));
    }





}

[System.Serializable]
public class CameraRecoilSettings
{
    [Header("Recoil Settings")]
    public float rotationSpeed = 6f;
    public float returnSpeed = 25;
    [Space()]
    [Header("Hipfire")]
    public Vector3 RecoilRotation = new Vector3(10f, 20f, 10f);
}
