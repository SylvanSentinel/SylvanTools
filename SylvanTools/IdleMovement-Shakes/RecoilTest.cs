using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilTest : MonoBehaviour
{
    [SerializeField] WeaponRecoil weaponRecoil;
    [SerializeField] CamRecoil camRecoil;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            weaponRecoil.Recoil();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            camRecoil.Recoil();
        }
    }
}
