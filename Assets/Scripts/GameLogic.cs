using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    private Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    void Start()
    {
        NetworkClientProcessing.SetGameLogic(this);
    }

    public void UpdatePlayerPosition(int playerId, Vector2 newPosition)
    {
        if (!players.ContainsKey(playerId))
        {
            // Create a circle using Unity's built-in 2D object
            GameObject player = new GameObject($"Player_{playerId}");
            player.AddComponent<SpriteRenderer>();
            player.AddComponent<CircleCollider2D>();

            // Use Unity's default sprite
            Sprite circleSprite = CreateCircleSprite();
            player.GetComponent<SpriteRenderer>().sprite = circleSprite;
            players[playerId] = player;
        }

        players[playerId].transform.position = new Vector3(newPosition.x, newPosition.y, 0);
    }

    // Method to create a simple circle sprite programmatically
    private Sprite CreateCircleSprite()
    {
        Texture2D texture = new Texture2D(128, 128);
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), new Vector2(texture.width / 2, texture.height / 2));
                texture.SetPixel(x, y, distance < texture.width / 2 ? Color.white : Color.clear);
            }
        }
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
