using UnityEngine;
using System;
using MQTTnet;
using MQTTnet.Client;
using System.Threading.Tasks;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class UnityMQTT : MonoBehaviour
{
    // MQTT Fields
    private IMqttClient mqttClient;
    private string brokerAddress = "178.128.213.67";  // Your broker address
    private int brokerPort = 1883;                    // MQTT port
    private string mqttTopic = "update_everyone";     // MQTT topic
    private string mqttUsername = "admin";            // MQTT username
    private string mqttPassword = "Team9Team";

    // RabbitMQ Fields
    private IConnection rabbitConnection;
    private IModel rabbitChannel;
    private string rabbitMQHost = "178.128.213.67";
    private int rabbitMQPort = 5672;
    private string rabbitMQUsername = "admin";
    private string rabbitMQPassword = "Team9Team";
    private string updateGEQueue = "update_ge_queue";

    private GameState gameState;

    [Serializable]
    public class PlayerState
    {
        public int hp;
        public int bullets;
        public int bombs;
        public int shield_hp;
        public int deaths;
        public int shields;
        public bool opponent_hit;
        public bool opponent_visible;
        public int opponent_in_rain_bomb;
        public bool disconnected;
        public bool login;
    }

    [Serializable]
    public class GameStateUpdate
    {
        public GameStateData game_state;
        public string action;
        public int player_id;
    }

    [Serializable]
    public class GameStateData
    {
        public PlayerState p1;
        public PlayerState p2;
    }

    async void Start()
    {
        gameState = GameState.Instance;

        gameState.EnemyActiveChanged += SendGameUpdateMessage;
        gameState.EnemyInWaterBombCountChanged += SendGameUpdateMessage;

        await ConnectToMQTT();
        await SubscribeToTopic();

        await ConnectToRabbitMQ();
    }

    #region MQTT Methods

    private async Task ConnectToMQTT()
    {
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithClientId(Guid.NewGuid().ToString())
            .WithTcpServer(brokerAddress, brokerPort)
            .WithCredentials(mqttUsername, mqttPassword)
            .WithCleanSession()
            .Build();

        mqttClient.ApplicationMessageReceivedAsync += HandleReceivedApplicationMessageAsync;

        try
        {
            await mqttClient.ConnectAsync(options);
            Debug.Log("Connected to MQTT broker.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error connecting to MQTT broker: {ex.Message}");
        }
    }

    private async Task SubscribeToTopic()
    {
        if (mqttClient.IsConnected)
        {
            await mqttClient.SubscribeAsync(mqttTopic);
            Debug.Log($"Subscribed to topic: {mqttTopic}");
        }
        else
        {
            Debug.LogError("MQTT client is not connected.");
        }
    }

    private async Task HandleReceivedApplicationMessageAsync(MqttApplicationMessageReceivedEventArgs e)
    {
        string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        Debug.Log($"Received message: {message}");

        // Parse the JSON data using Json.NET
        GameStateUpdate gameStateUpdate = JsonConvert.DeserializeObject<GameStateUpdate>(message);

        if (gameStateUpdate != null && gameStateUpdate.game_state != null)
        {
            // Update game state accordingly
            UpdateGameState(gameStateUpdate);
        }

        await Task.CompletedTask; // Required for async method
    }

    private void UpdateGameState(GameStateUpdate gameStateUpdate)
    {
        if (gameStateUpdate.game_state.p1 != null)
        {
            Debug.Log("Game State Update P1: " + gameStateUpdate.game_state.p1.hp + " " + gameStateUpdate.game_state.p1.bullets + " " + gameStateUpdate.game_state.p1.bombs + " " + gameStateUpdate.game_state.p1.shield_hp + " " + gameStateUpdate.game_state.p1.deaths + " " + gameStateUpdate.game_state.p1.shields);

            // Update player 1's game state
            gameState.HealthValue = gameStateUpdate.game_state.p1.hp;
            gameState.AmmoCount = gameStateUpdate.game_state.p1.bullets;
            gameState.WaterbombCount = gameStateUpdate.game_state.p1.bombs;
            gameState.ShieldValue = gameStateUpdate.game_state.p1.shield_hp;
            gameState.ShieldCount = gameStateUpdate.game_state.p1.shields;
            gameState.EnemyHit = gameStateUpdate.game_state.p1.opponent_hit;
        }

        if (gameStateUpdate.game_state.p2 != null)
        {
            Debug.Log("Game State Update P2: " + gameStateUpdate.game_state.p2.hp + " " + gameStateUpdate.game_state.p2.bullets + " " + gameStateUpdate.game_state.p2.bombs + " " + gameStateUpdate.game_state.p2.shield_hp + " " + gameStateUpdate.game_state.p2.deaths + " " + gameStateUpdate.game_state.p2.shields);

            // Update player 2's game state
            gameState.EnemyHealthValue = gameStateUpdate.game_state.p2.hp;
            gameState.EnemyShieldValue = gameStateUpdate.game_state.p2.shield_hp;
        }
    }

    #endregion

    #region RabbitMQ Methods

    private async Task ConnectToRabbitMQ()
    {
        var factory = new ConnectionFactory()
        {
            HostName = rabbitMQHost,
            Port = rabbitMQPort,
            UserName = rabbitMQUsername,
            Password = rabbitMQPassword
        };

        try
        {
            await Task.Run(() =>
            {
                rabbitConnection = factory.CreateConnection();
                rabbitChannel = rabbitConnection.CreateModel();

                // Declare the queue in case it doesn't exist
                rabbitChannel.QueueDeclare(queue: updateGEQueue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            });

            Debug.Log($"Connected to RabbitMQ and declared queue: {updateGEQueue}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unexpected error connecting to RabbitMQ: {ex.Message}");
        }
    }

    public async Task SendRabbitMessage(string message)
    {
        if (rabbitChannel != null && rabbitChannel.IsOpen)
        {
            var body = Encoding.UTF8.GetBytes(message);

            var properties = rabbitChannel.CreateBasicProperties();
            properties.Persistent = true;

            try
            {
                await Task.Run(() =>
                {
                    rabbitChannel.BasicPublish(exchange: "",
                                         routingKey: updateGEQueue,
                                         basicProperties: properties,
                                         body: body);
                });

                Debug.Log($"Message sent to RabbitMQ: {message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error sending message to RabbitMQ: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError("RabbitMQ channel is not open.");
        }
    }

    // Existing function to send a game update message when opponent visibility and enemyInWaterBombCount changes
    public async void SendGameUpdateMessage()
    {
        var message = new JObject
        {
            ["game_state"] = new JObject()
        };
        message["game_state"]["p1"] = new JObject();
        message["game_state"]["p1"]["opponent_visible"] = gameState.EnemyActive;
        message["game_state"]["p1"]["opponent_in_rain_bomb"] = gameState.EnemyInWaterBombCount;

        string jsonMessage = message.ToString(Formatting.None);
        await SendRabbitMessage(jsonMessage);
    }

    // Ssend the full game state information
    public async void SendFullGameState()
    {
        var message = new JObject
        {
            ["update"] = true,
            ["game_state"] = new JObject()
        };

        // Player 1's game state
        var p1State = new JObject
        {
            ["hp"] = gameState.HealthValue,
            ["bullets"] = gameState.AmmoCount,
            ["bombs"] = gameState.WaterbombCount,
            ["shield_hp"] = gameState.ShieldValue,
            ["shields"] = gameState.ShieldCount,
        };

        // Player 2's game state with default values
        var p2State = new JObject
        {
            ["hp"] = gameState.EnemyHealthValue,
            ["bullets"] = 6,
            ["bombs"] = 2,
            ["shield_hp"] = gameState.EnemyShieldValue,
            ["shields"] = 0,
        };

        message["game_state"]["p1"] = p1State;
        message["game_state"]["p2"] = p2State;

        string jsonMessage = message.ToString(Formatting.None);
        await SendRabbitMessage(jsonMessage);

        Debug.Log("Full game state message sent to RabbitMQ: " + jsonMessage);
    }

    // Send game action messages
    public async void SendGameActionMessage(string action_type, bool hit)
    {
        var message = new JObject
        {
            ["action"] = true,
            ["action_type"] = action_type,
            ["player_id"] = 1,
            ["hit"] = hit,
            ["game_state"] = new JObject()
        };
        message["game_state"]["p1"] = new JObject();
        message["game_state"]["p1"]["opponent_visible"] = gameState.EnemyActive;

        string jsonMessage = message.ToString(Formatting.None);
        await SendRabbitMessage(jsonMessage);
    }

    // Methods to be called from Unity button selector
    public void SendGameActionMessageHit(string action_type)
    {
        SendGameActionMessage(action_type, true);
    }

    public void SendGameActionMessageNotHit(string action_type)
    {
        SendGameActionMessage(action_type, false);
    }
    #endregion

    private async void OnApplicationQuit()
    {
        // Disconnect MQTT
        if (mqttClient != null && mqttClient.IsConnected)
        {
            try
            {
                await mqttClient.DisconnectAsync();
                Debug.Log("Disconnected from MQTT Broker");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error disconnecting from MQTT broker: {ex.Message}");
            }
        }

        // Disconnect RabbitMQ
        if (rabbitChannel != null && rabbitChannel.IsOpen)
        {
            try
            {
                await Task.Run(() =>
                {
                    rabbitChannel.Close();
                    rabbitConnection.Close();
                });

                Debug.Log("Disconnected from RabbitMQ");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error disconnecting from RabbitMQ: {ex.Message}");
            }
        }

        if (gameState != null)
        {
            gameState.EnemyActiveChanged -= SendGameUpdateMessage;
            gameState.EnemyInWaterBombCountChanged -= SendGameUpdateMessage;
        }
    }
}