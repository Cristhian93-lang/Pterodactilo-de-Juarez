using UnityEngine;

/// <summary>
/// Makes the camera follow the player vehicle in third person.
/// The camera keeps a fixed distance (offset) behind and above the vehicle.
/// Standard coding documentation can be found in
/// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    /// <summary>
    /// The vehicle the camera follows. Assigned from the Inspector.
    /// </summary>
    public GameObject player;

    /// <summary>
    /// Distance from the vehicle to the camera: 6 units up and 7 units behind.
    /// Same values as the camera's initial position, so it does not jump at start.
    /// </summary>
    public Vector3 offset = new Vector3(0, 9, -9);

    /// <summary>
    /// This method is called before the first frame update.
    /// </summary>
    void Start()
    {

    }

    /// <summary>
    /// This method is called once per frame, after every Update has run.
    /// The vehicle moves first in Update, then the camera follows it here,
    /// which avoids the jitter seen when both run in Update.
    /// </summary>
    void LateUpdate()
    {
        // Place the camera at the vehicle position plus the offset
        transform.position = player.transform.position + offset;
    }
}