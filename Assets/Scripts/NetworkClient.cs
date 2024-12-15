using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System.Text;

public enum TransportPipeline
{
    NotIdentified,
    ReliableAndInOrder,
    FireAndForget
}

public class NetworkClient : MonoBehaviour
{
    NetworkDriver networkDriver;
    NetworkConnection networkConnection;
    NetworkPipeline reliableAndInOrderPipeline;
    NetworkPipeline nonReliableNotInOrderedPipeline;
    const ushort NetworkPort = 9001;
    const string IPAddress = "10.0.0.82";

    void Start()
    {
        if (NetworkClientProcessing.GetNetworkedClient() == null)
        {
            DontDestroyOnLoad(this.gameObject);
            NetworkClientProcessing.SetNetworkedClient(this);
            Connect();
        }
        else
        {
            UnityEngine.Debug.Log("Singleton-ish architecture violation detected. Check NetworkClient instances.");
            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
        if (networkConnection.IsCreated)
        {
            networkConnection.Disconnect(networkDriver);
        }
        networkConnection = default(NetworkConnection);
        networkDriver.Dispose();
    }

    void Update()
    {
        networkDriver.ScheduleUpdate().Complete();

        if (!networkConnection.IsCreated)
        {
            UnityEngine.Debug.Log("Client is unable to connect to server.");
            return;
        }

        NetworkEvent.Type networkEventType;
        DataStreamReader streamReader;
        NetworkPipeline pipelineUsedToSendEvent;

        while (PopNetworkEventAndCheckForData(out networkEventType, out streamReader, out pipelineUsedToSendEvent))
        {
            TransportPipeline pipelineUsed = TransportPipeline.NotIdentified;
            if (pipelineUsedToSendEvent == reliableAndInOrderPipeline)
                pipelineUsed = TransportPipeline.ReliableAndInOrder;
            else if (pipelineUsedToSendEvent == nonReliableNotInOrderedPipeline)
                pipelineUsed = TransportPipeline.FireAndForget;

            switch (networkEventType)
            {
                case NetworkEvent.Type.Connect:
                    NetworkClientProcessing.ConnectionEvent();
                    break;
                case NetworkEvent.Type.Data:
                    int sizeOfDataBuffer = streamReader.ReadInt();
                    NativeArray<byte> buffer = new NativeArray<byte>(sizeOfDataBuffer, Allocator.Persistent);
                    streamReader.ReadBytes(buffer);
                    byte[] byteBuffer = buffer.ToArray();
                    string msg = Encoding.Unicode.GetString(byteBuffer);
                    NetworkClientProcessing.ReceivedMessageFromServer(msg, pipelineUsed);
                    buffer.Dispose();
                    break;
                case NetworkEvent.Type.Disconnect:
                    NetworkClientProcessing.DisconnectionEvent();
                    networkConnection = default(NetworkConnection);
                    break;
            }
        }
    }

    private bool PopNetworkEventAndCheckForData(out NetworkEvent.Type networkEventType, out DataStreamReader streamReader, out NetworkPipeline pipelineUsedToSendEvent)
    {
        networkEventType = networkConnection.PopEvent(networkDriver, out streamReader, out pipelineUsedToSendEvent);

        if (networkEventType == NetworkEvent.Type.Empty)
            return false;
        return true;
    }

    public void SendMessageToServer(string msg, TransportPipeline pipeline)
    {
        NetworkPipeline networkPipeline = reliableAndInOrderPipeline;
        if (pipeline == TransportPipeline.FireAndForget)
            networkPipeline = nonReliableNotInOrderedPipeline;

        byte[] msgAsByteArray = Encoding.Unicode.GetBytes(msg);
        NativeArray<byte> buffer = new NativeArray<byte>(msgAsByteArray, Allocator.Persistent);

        DataStreamWriter streamWriter;
        networkDriver.BeginSend(networkPipeline, networkConnection, out streamWriter);
        streamWriter.WriteInt(buffer.Length);
        streamWriter.WriteBytes(buffer);
        networkDriver.EndSend(streamWriter);

        buffer.Dispose();
    }

    public void Connect()
    {
        networkDriver = NetworkDriver.Create();
        reliableAndInOrderPipeline = networkDriver.CreatePipeline(typeof(FragmentationPipelineStage), typeof(ReliableSequencedPipelineStage));
        nonReliableNotInOrderedPipeline = networkDriver.CreatePipeline(typeof(FragmentationPipelineStage));
        networkConnection = default(NetworkConnection);
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(IPAddress, NetworkPort, NetworkFamily.Ipv4);
        networkConnection = networkDriver.Connect(endpoint);
    }

    public bool IsConnected()
    {
        return networkConnection.IsCreated;
    }

    public void Disconnect()
    {
        networkConnection.Disconnect(networkDriver);
        networkConnection = default(NetworkConnection);
    }
}
