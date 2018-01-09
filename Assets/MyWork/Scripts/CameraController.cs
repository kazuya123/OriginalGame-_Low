using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private GameObject _player;
    private Vector3 offset = Vector3.zero;

    void Start()
    {
        _player = GameObject.Find("Player_00");
        offset = transform.position - _player.transform.position;
    }

    void LateUpdate()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = _player.transform.position.x + offset.x;
        newPosition.y = _player.transform.position.y + offset.y;
        newPosition.z = _player.transform.position.z + offset.z;
        transform.position = Vector3.Lerp(transform.position, newPosition, 5.0f * Time.deltaTime);
    }
}
