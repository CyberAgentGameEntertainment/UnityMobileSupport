package jp.co.cyberagent.unitysupport.app;

import java.util.function.Consumer;

public class BatteryStatusReceiver implements jp.co.cyberagent.unitysupport.thermal.BatteryStatusReceiver {
    private Consumer<Integer> onReceiveTemperature;
    private Consumer<Integer> onReceiveVoltage;
    private Consumer<Float> onReceiveLevel;
    private Consumer<Integer> onReceiveStatus;

    @Override
    public void onReceiveBatteryTemperature(int temperature) {
        onReceiveTemperature.accept(new Integer(temperature));
    }

    @Override
    public void onReceiveVoltage(int voltage) {
        onReceiveVoltage.accept(new Integer(voltage));
    }

    @Override
    public void onReceiveLevel(float level) {
        onReceiveLevel.accept(new Float(level));
    }

    @Override
    public void onReceiveStatus(int status) {
        onReceiveStatus.accept(new Integer(status));
    }

    public BatteryStatusReceiver(
            Consumer<Integer> onReceiveTemperature,
            Consumer<Integer> onReceiveVoltage,
            Consumer<Float> onReceiveLevel,
            Consumer<Integer> onReceiveStatus) {
        this.onReceiveTemperature = onReceiveTemperature;
        this.onReceiveVoltage = onReceiveVoltage;
        this.onReceiveLevel = onReceiveLevel;
        this.onReceiveStatus = onReceiveStatus;
    }
}
