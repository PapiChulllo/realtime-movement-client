using UnityEngine;

static public class NetworkClientProcessing
{
    static NetworkClient networkClient;
    static GameLogic gameLogic;

    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        string[] csv = msg.Split(',');

        if (csv[0] == "PlayerMoved")
        {
            int playerId = int.Parse(csv[1]);
            float x = float.Parse(csv[2]);
            float y = float.Parse(csv[3]);
            gameLogic.UpdatePlayerPosition(playerId, new Vector2(x, y));
        }
    }

    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        networkClient.SendMessageToServer(msg, pipeline);
    }

    static public void ConnectionEvent()
    {
        UnityEngine.Debug.Log("Client successfully connected to the server!");
    }

    static public void DisconnectionEvent()
    {
        UnityEngine.Debug.Log("Client disconnected from the server!");
    }

    static public NetworkClient GetNetworkedClient()
    {
        return networkClient;
    }

    static public void SetNetworkedClient(NetworkClient NetworkClient)
    {
        networkClient = NetworkClient;
    }

    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }
}
