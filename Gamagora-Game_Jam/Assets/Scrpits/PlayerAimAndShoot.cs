using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimAndShoot : MonoBehaviour
{
    public GameObject flare;
    [SerializeField] private Transform flareSpawnPoint;
    [SerializeField] private GameObject rotationMouseGO; //GUN
    private GameObject flareInst;
    private Vector2 worldPosition;
    private Vector2 directionVector;
    private float angle;

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        HandleFlareShooting();
    }

    private void HandleRotation()
    {
        worldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        directionVector = (worldPosition - (Vector2)rotationMouseGO.transform.position).normalized;
        rotationMouseGO.transform.right = directionVector;

        //flip when reach 90 degree threshold
        angle = Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg;
        Vector3 localScale = new Vector3(1f, 1f, 1f);
        if(angle > 90 || angle < -90)
        {
            localScale.y = -1f;
        }
        else
        {
            localScale.y = 1f;

        }
        rotationMouseGO.transform.localScale = localScale;
    }

    private void HandleFlareShooting()
    {
        if(Input.GetMouseButtonDown(0))
        {
            flareInst = Instantiate(flare, flareSpawnPoint.position, rotationMouseGO.transform.rotation);
        }
    }
}
