using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    private WeaponManager weapon_Manager;

    public float fireRate = 15f;
    private float nextTimeToFire;
    public float damage = 20f;

    private Animator zoomCameraAnim;
    private bool zoomed;

    private Camera mainCam;
    private GameObject crosshair;

    private bool is_Aiming;

    [SerializeField]
    private GameObject arrow_Prefab, spear_Prefab;

    [SerializeField]
    private Transform arrow_Bow_StartPosition;

    private void Awake() {

        weapon_Manager = GetComponent<WeaponManager>();

        zoomCameraAnim = transform.Find(Tags.LOOK_ROOT).transform.Find(Tags.ZOOM_CAMERA).GetComponent<Animator>();

        crosshair = GameObject.FindWithTag(Tags.CROSSHAIR);

        mainCam = Camera.main;

    }

    private void Update() {
        WeaponShoot();
        ZoomInAndOut();
    }

    void WeaponShoot() {

        // if we have assault rifle
        if (weapon_Manager.GetCurrentSelectedWeapon().fireType == WeaponFireType.MULTIPLE) {

            // if we press and hold
            if (Input.GetMouseButton(0) && Time.time > nextTimeToFire) {

                nextTimeToFire = Time.time + 1f / fireRate;

                weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();

                BulletFired();
            }

        } else { // if we have a regular weapon that shoots once

            if (Input.GetMouseButtonDown(0)) {

                // handle axe
                if (weapon_Manager.GetCurrentSelectedWeapon().tag == Tags.AXE_TAG) {
                    weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();
                }

                // handle shoot
                if (weapon_Manager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.BULLET) {

                    weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();

                    BulletFired();
                } else { // we have a bow (arrow) or spear

                    if(is_Aiming) {
                        weapon_Manager.GetCurrentSelectedWeapon().ShootAnimation();

                        if(weapon_Manager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.ARROW) {
                            // throw arrow

                            ThrowArrowOrSpear(true);

                        } else if(weapon_Manager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.SPEAR) {
                            // throw spear

                            ThrowArrowOrSpear(false);

                        }
                    }

                } // else

            } // if input get mouse button 0

        } // else 
    }

    void ZoomInAndOut() {

        // we are doing to aim with our camera on the weapon
        if (weapon_Manager.GetCurrentSelectedWeapon().weapon_Aim == WeaponAim.AIM) {


            if (Input.GetMouseButtonDown(1)) {

                zoomCameraAnim.Play(AnimationTags.ZOOM_IN_ANIM);

                crosshair.SetActive(false);
            }

            // when we release the right mouse button click
            if (Input.GetMouseButtonUp(1)) {

                zoomCameraAnim.Play(AnimationTags.ZOOM_OUT_ANIM);

                crosshair.SetActive(true);
            }
        } // if we need to zoom the weapon

        if (weapon_Manager.GetCurrentSelectedWeapon().weapon_Aim == WeaponAim.SELF_AIM) {

            if (Input.GetMouseButtonDown(1)) {

                weapon_Manager.GetCurrentSelectedWeapon().Aim(true);

                is_Aiming = true;
            }

            if (Input.GetMouseButtonUp(1)) {

                weapon_Manager.GetCurrentSelectedWeapon().Aim(false);

                is_Aiming = false;
            }

        } // weapoin self aim

    } // zoom in and out

    void ThrowArrowOrSpear(bool throwArrow) {

        if(throwArrow) {
            
            GameObject arrow = Instantiate(arrow_Prefab);
            arrow.transform.position = arrow_Bow_StartPosition.position;

            print("throw arrow at pos: " + arrow.transform.position);

            arrow.GetComponent<ArrowAndBowScript>().Launch(mainCam);
        } else {
            GameObject spear = Instantiate(spear_Prefab);
            spear.transform.position = arrow_Bow_StartPosition.position;

            spear.GetComponent<ArrowAndBowScript>().Launch(mainCam);
        }

    } // throw arrow or spear

    void BulletFired() {

        RaycastHit hit;

        if(Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit)) {

            if(hit.transform.tag == Tags.ENEMY_TAG) {
                hit.transform.GetComponent<HealthScript>().ApplyDamage(damage);
            }

        }

    } // bullet fired

} // class