package jp.co.cyberagent.unitysupport.app;

import android.os.Build;

import java.util.function.Consumer;

public class BatteryTemperatureReceiver implements jp.co.cyberagent.unitysupport.thermal.BatteryTemperatureReceiver {
    private Consumer<Integer> onReceive;

    @Override
    public void onReceiveBatteryTemperature(int level) {
        onReceive.accept(new Integer(level));
    }
    
    public BatteryTemperatureReceiver(Consumer<Integer> onReceive)
    {

        this.onReceive = onReceive;
    }
}
