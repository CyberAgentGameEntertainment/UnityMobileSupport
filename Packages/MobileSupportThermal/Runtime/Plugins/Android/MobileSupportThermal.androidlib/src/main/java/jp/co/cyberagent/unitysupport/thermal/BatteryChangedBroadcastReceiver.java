package jp.co.cyberagent.unitysupport.thermal;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.BatteryManager;

import java.util.HashSet;

public class BatteryChangedBroadcastReceiver extends BroadcastReceiver {

    private static final int UNINITIALIZED_TEMPERATURE = -1;
    private static final IntentFilter intentFilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
    private final HashSet<BatteryTemperatureReceiver> receivers = new HashSet<>();

    private int prevTemperature = UNINITIALIZED_TEMPERATURE;

    @Override
    public void onReceive(Context context, Intent intent) {

        int value = intent.getIntExtra(BatteryManager.EXTRA_TEMPERATURE, 0);

        if (prevTemperature == value) return;
        prevTemperature = value;

        for (BatteryTemperatureReceiver receiver : receivers) {
            receiver.onReceiveBatteryTemperature(value);
        }
    }

    public void registerToContext(Context context) {
        context.registerReceiver(this, intentFilter);
        prevTemperature = UNINITIALIZED_TEMPERATURE;
    }

    public void unregisterFromContext(Context context) {
        context.unregisterReceiver(this);
    }

    public void addReceiver(BatteryTemperatureReceiver receiver) {
        receivers.add(receiver);
        prevTemperature = UNINITIALIZED_TEMPERATURE;
    }

    public void removeReceiver(BatteryTemperatureReceiver receiver) {
        receivers.remove(receiver);
    }
}
