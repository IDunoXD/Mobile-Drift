using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class WaypointManager : MonoBehaviour
{
    [SerializeField] TouchManager touchManager;
    public LayerMask layerMask;
    public GameObject prefab;
    public List<GameObject> Waypoints = new List<GameObject>();
    private void OnEnable()
    {
        TouchManager.OnClicked += SetWaypoint;
    }
    private void OnDisable()
    {
        TouchManager.OnClicked -= SetWaypoint;
    }
    private void SetWaypoint(){
        Ray ray = Camera.main.ScreenPointToRay(touchManager.ScreenPoint);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            Waypoints.Add(Instantiate(prefab, hit.point, hit.transform.rotation, transform));
        }
    }
    public GameObject FindClosestTo(Transform Pos){
        float distance = 0;
        float minDistance = Mathf.Infinity;
        int index = -1;
        for(int i = 0; i < Waypoints.Count ;i++){
            if(Waypoints[i].GetComponent<Waypoint>().triggered == true) continue;
            distance = Vector3.Distance(Waypoints[i].transform.position,Pos.position);
            if(minDistance > distance) {
                minDistance = distance;
                index = i;
            }
        }
        return index==-1 ? null : Waypoints[index];
    }
    public GameObject FindLastPaced(){
        return Waypoints.Count == 0 ? null : Waypoints[Waypoints.Count-1];
    }
    public void RemoveObj(GameObject obj){
        Waypoints.Remove(obj);
    }
}
