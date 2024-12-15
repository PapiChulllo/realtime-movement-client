using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) movement.y += 1;
        if (Input.GetKey(KeyCode.S)) movement.y -= 1;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1;
        if (Input.GetKey(KeyCode.D)) movement.x += 1;

        if (movement != Vector2.zero)
        {
            Vector3 newPosition = transform.position + (Vector3)movement.normalized * speed * Time.deltaTime;
            transform.position = newPosition;

            // Notify the server
            NetworkClientProcessing.SendMessageToServer(
                $"PlayerMoved,{newPosition.x},{newPosition.y}",
                TransportPipeline.ReliableAndInOrder
            );
        }
    }
}
