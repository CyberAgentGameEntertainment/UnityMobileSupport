package jp.co.cyberagent.unitysupport.thermal;

public interface BatteryStatusReceiver {
    void onReceiveBatteryTemperature(int temperature);

    void onReceiveVoltage(int voltage);

    void onReceiveLevel(float level);

    void onReceiveStatus(int status);
}
