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

    private const int Port = 8765;
    private const float RetryDelay = 5f; // seconds between discovery attempts
    private const int ProbeTimeout = 200; // ms for a single IP probe

    private string latestJson;

    private string GetConfiguredHost()
    {
        string host = System.Environment.GetEnvironmentVariable("WEBSOCKET_HOST");
        if (string.IsNullOrEmpty(host) && PlayerPrefs.HasKey("WebSocketHost"))
            host = PlayerPrefs.GetString("WebSocketHost");

        return host;
    }

    private void ConnectTo(string url)
    {
        ws = new WebSocket(url);
        ws.OnMessage += (sender, e) => latestJson = e.Data;
        try
        {
            ws.Connect();
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Failed to connect to {url}: {ex.Message}");
            ws = null;
        }
    }

    private void Start()
    {
        string host = GetConfiguredHost();
        if (!string.IsNullOrEmpty(host))
        {
            ConnectTo($"ws://{host}:{Port}");
        }
        else
        {
            StartCoroutine(DiscoverAndConnect());
        }
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

    private System.Collections.IEnumerator DiscoverAndConnect()
    {
        while (ws == null || ws.ReadyState != WebSocketState.Open)
        {
            string prefix = GetSubnetPrefix();
            if (!string.IsNullOrEmpty(prefix))
            {
                for (int i = 1; i < 255 && (ws == null || ws.ReadyState != WebSocketState.Open); i++)
                {
                    string ip = prefix + i;
                    if (IsPortOpen(ip, Port, ProbeTimeout))
                    {
                        ConnectTo($"ws://{ip}:{Port}");
                        if (ws != null && ws.ReadyState == WebSocketState.Open)
                            break;
                    }

                    if (i % 10 == 0)
                        yield return null;
                }
            }

            if (ws == null || ws.ReadyState != WebSocketState.Open)
            {
                if (ws != null)
                {
                    ws.Close();
                    ws = null;
                }
                yield return new WaitForSeconds(RetryDelay);
            }
        }
    }

    private string GetSubnetPrefix()
    {
        foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != System.Net.NetworkInformation.OperationalStatus.Up)
                continue;

            var props = ni.GetIPProperties();
            foreach (var addr in props.UnicastAddresses)
            {
                if (addr.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork &&
                    !System.Net.IPAddress.IsLoopback(addr.Address))
                {
                    var bytes = addr.Address.GetAddressBytes();
                    return $"{bytes[0]}.{bytes[1]}.{bytes[2]}.";
                }
            }
        }

        return null;
    }

    private bool IsPortOpen(string host, int port, int timeoutMs)
    {
        try
        {
            using (var client = new System.Net.Sockets.TcpClient())
            {
                var result = client.BeginConnect(host, port, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(timeoutMs);
                client.Close();
                return success;
            }
        }
        catch
        {
            return false;
        }
    }
}
