using UnityEngine;

public enum ClientToServerSignifiers
{
    BalloonPopped = 1
}

public enum ServerToClientSignifiers
{
    SpawnBalloon = 1,
    BalloonPopped = 2
}

static public class NetworkClientProcessing
{
    static NetworkClient networkClient;
    static GameLogic gameLogic;

    static public void ReceivedMessageFromServer(string msg, TransportPipeline pipeline)
    {
        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == (int)ServerToClientSignifiers.SpawnBalloon)
        {
            int balloonID = int.Parse(csv[1]);
            float x = float.Parse(csv[2]);
            float y = float.Parse(csv[3]);
            gameLogic.SpawnNewBalloon(balloonID, new Vector2(x, y));
        }
        else if (signifier == (int)ServerToClientSignifiers.BalloonPopped)
        {
            int balloonID = int.Parse(csv[1]);
            gameLogic.RemoveBalloon(balloonID);
        }
    }

    static public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        networkClient.SendMessageToServer(msg, pipeline);
    }

    static public void ConnectionEvent()
    {
        UnityEngine.Debug.Log("Network Connection Event!");
    }

    static public void DisconnectionEvent()
    {
        UnityEngine.Debug.Log("Network Disconnection Event!");
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
