
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class CatchCollider_Vehicle : UdonSharpBehaviour
{
    [HideInInspector] [SerializeField] public VehicleInSideSeatMNG local_SeatMNG;

    public GameObject vehicleObject;
    public GameObject inVehicleCollider;

    public bool autoCatch = true;

    [HideInInspector][SerializeField] public int local_Id_OnSeatMNG;
    void Start()
    {
        if (vehicleObject == null) vehicleObject = this.gameObject;
        DisableInteractive = true;
    }


    public override void Interact() {
        //Debug.Log("Player " + Networking.LocalPlayer.displayName + " Enter Vehicle " + local_Id_OnSeatMNG);
        local_SeatMNG.ForcedRidingOnVehicle(local_Id_OnSeatMNG);
        DisableInteractive = true;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerChaser>() != null)
        {
            DisableInteractive = false;
            if (autoCatch)
            {
                //Debug.Log("Player " + Networking.LocalPlayer.displayName + " Enter Vehicle " + local_Id_OnSeatMNG);
                DisableInteractive = local_SeatMNG.EnterOnVehicle(local_Id_OnSeatMNG);//auto catch success: disable interact, if not, enableInteract
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerChaser>() != null)
        {
            //Debug.Log("Player " + Networking.LocalPlayer.displayName + " Exit Vehicle " + local_Id_OnSeatMNG);
            local_SeatMNG.Exit(local_Id_OnSeatMNG);
            DisableInteractive = true;
        }
    }
}
