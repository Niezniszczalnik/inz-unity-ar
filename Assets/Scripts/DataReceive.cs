using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

[System.Serializable]
public class Vector3Data
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class SensorData
{
    public string timestamp;
    public float hr;
    public float spo2;
    // Używamy zwykłych typów zmiennoprzecinkowych, ponieważ Unity
    // JsonUtility nie obsługuje pól typu Nullable.
    // Brakujące wartości w danych JSON będą po prostu przyjmowały
    // domyślną wartość 0.
    public float object_temp;
    public float ambient_temp;
    public Vector3Data accel;
    public Vector3Data gyro;
    public float lux;
    public float temperature;
    public float pressure;
    public float humidity;
    public float gas_resistance;
}

public class DataReceiver : MonoBehaviour
{
    private WebSocket ws;
    public Text hudText;

    private string latestJson;

    private void Start()
    {
        ws = new WebSocket("ws://192.168.100.234:8765");
        ws.OnMessage += (sender, e) => latestJson = e.Data;
        ws.Connect();
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(latestJson))
            return;

        // deserializacja JSON-a na obiekt SensorData
        SensorData data = JsonUtility.FromJson<SensorData>(latestJson);

        // przygotowanie czytelnego tekstu
        hudText.text =
            $"Time: {data.timestamp}\n" +
            $"HR: {data.hr} bpm\n" +
            $"SpO2: {data.spo2}%\n" +
            $"ObjT: {data.object_temp:F1} °C\n" +
            $"AmbT: {data.ambient_temp:F1} °C\n" +
            $"Accel: {data.accel.x:F2}, {data.accel.y:F2}, {data.accel.z:F2}\n" +
            $"Gyro: {data.gyro.x:F2}, {data.gyro.y:F2}, {data.gyro.z:F2}\n" +
            $"Lux: {data.lux}\n" +
            $"Env.T: {data.temperature:F1} °C\n" +
            $"Press: {data.pressure:F1} hPa\n" +
            $"Hum: {data.humidity:F1}%\n" +
            $"Gas: {data.gas_resistance} Ω";

        latestJson = null;
    }

    private void OnDestroy()
    {
        if (ws != null)
            ws.Close();
    }
}
