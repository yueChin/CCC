using UnityEngine;
using UnityEngine.InputSystem;
public class MovePlayer : MonoBehaviour
{
    public Vector2 moveVal;
    public float moveSpeed;
    void OnMove(InputValue value)
    {
        moveVal = value.Get<Vector2>();
    }
    void Update()
    {
        transform.Translate(new Vector3(moveVal.x, moveVal.y, 0) * moveSpeed * Time.deltaTime);
    }
}