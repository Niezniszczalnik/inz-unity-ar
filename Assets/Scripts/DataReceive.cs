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
        ws = new WebSocket("ws://192.168.40.136:8765");
        ws.OnMessage += (sender, e) => latestJson = e.Data;
        ws.Connect();
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(latestJson))
            return;

        // deserializacja JSON-a na obiekt SensorData
        SensorData data = JsonUtility.FromJson<SensorData>(latestJson);

        // sformatuj czas jeśli to możliwe
        string formattedTimestamp = data.timestamp;
        if (System.DateTime.TryParse(data.timestamp, out var parsedTime))
        {
            formattedTimestamp = parsedTime.ToString("HH:mm:ss dd.MMM.yyyy");
        }

        // przygotowanie czytelnego tekstu
        hudText.text =
            $"Godzina: {formattedTimestamp}\n" +
            $"Tętno: {data.hr} bpm\n" +
            $"Saturacja: {data.spo2} SpO₂%\n" +
            $"Temp. ciała: {data.object_temp:F1} °C\n" +
            $"Temp. otoczenia: {data.ambient_temp:F1} °C\n" +
            $"Akcelerometr:\n" +
            $" x: {data.accel.x:F2} m/s², y: {data.accel.y:F2} m/s², z: {data.accel.z:F2} m/s²\n" +
            $"Żyroskop: x: {data.gyro.x:F2} °/s, y: {data.gyro.y:F2} °/s, z: {data.gyro.z:F2} °/s\n" +
            $"Natężenie światła: {data.lux:F2} lux\n" +
            $"Temp. powietrza: {data.temperature:F1} °C\n" +
            $"Ciśnienie powietrza: {data.pressure:F1} hPa\n" +
            $"Wilgotność powietrza: {data.humidity:F1}%\n";

        latestJson = null;
    }

    private void OnDestroy()
    {
        if (ws != null)
            ws.Close();
    }
}
