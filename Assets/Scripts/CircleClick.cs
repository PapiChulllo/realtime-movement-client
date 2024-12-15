using UnityEngine;

public class CircleClick : MonoBehaviour
{
    public int balloonID;

    void OnMouseDown()
    {
        NetworkClientProcessing.SendMessageToServer($"{(int)ClientToServerSignifiers.BalloonPopped},{balloonID}", TransportPipeline.ReliableAndInOrder);
        Destroy(gameObject);
    }
}
