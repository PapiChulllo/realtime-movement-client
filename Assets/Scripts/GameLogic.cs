using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private Dictionary<int, GameObject> balloons = new Dictionary<int, GameObject>();
    private Sprite circleTexture;

    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);
    }

    public void SpawnNewBalloon(int balloonID, Vector2 screenPosition)
    {
        if (circleTexture == null)
            circleTexture = Resources.Load<Sprite>("Circle");

        GameObject balloon = new GameObject("Balloon");
        balloon.AddComponent<SpriteRenderer>();
        balloon.GetComponent<SpriteRenderer>().sprite = circleTexture;
        balloon.AddComponent<CircleClick>().balloonID = balloonID;
        balloon.AddComponent<CircleCollider2D>();

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
        pos.z = 0;
        balloon.transform.position = pos;

        balloons[balloonID] = balloon;
    }

    public void RemoveBalloon(int balloonID)
    {
        if (!balloons.ContainsKey(balloonID)) return;

        Destroy(balloons[balloonID]);
        balloons.Remove(balloonID);
    }
}
