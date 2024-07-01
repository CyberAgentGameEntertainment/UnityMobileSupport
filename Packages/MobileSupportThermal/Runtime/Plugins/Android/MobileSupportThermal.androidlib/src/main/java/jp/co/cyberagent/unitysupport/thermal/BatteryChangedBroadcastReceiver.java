package jp.co.cyberagent.unitysupport.thermal;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.BatteryManager;

import java.util.HashSet;

public class BatteryChangedBroadcastReceiver extends BroadcastReceiver {
    private static final int VALUE_UNINITIALIZED = -1;
    private static final IntentFilter intentFilter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
    private final HashSet<BatteryStatusReceiver> receivers = new HashSet<>();

    private int prevTemperature = VALUE_UNINITIALIZED;
    private int prevVoltage = VALUE_UNINITIALIZED;
    private int prevStatus = VALUE_UNINITIALIZED;
    private float prevLevel = Float.NaN;

    @Override
    public void onReceive(Context context, Intent intent) {
        onReceive(intent);
    }

    private void onReceive(Intent intent) {
        // battery temperature
        int temperature = intent.getIntExtra(BatteryManager.EXTRA_TEMPERATURE, 0);

        int voltage = intent.getIntExtra(BatteryManager.EXTRA_VOLTAGE, -1);

        int status = intent.getIntExtra(BatteryManager.EXTRA_STATUS, 1); // unknown is 1

        int scale = intent.getIntExtra(BatteryManager.EXTRA_SCALE, -1);
        int levelScaled = intent.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
        float level = levelScaled / (float) scale;

        if (prevTemperature != temperature) {
            prevTemperature = temperature;
            for (BatteryStatusReceiver receiver : receivers) {
                receiver.onReceiveBatteryTemperature(temperature);
            }
        }

        if (prevVoltage != voltage) {
            prevVoltage = voltage;
            for (BatteryStatusReceiver receiver : receivers) {
                receiver.onReceiveVoltage(voltage);
            }
        }

        if (prevStatus != status) {
            prevStatus = status;
            for (BatteryStatusReceiver receiver : receivers) {
                receiver.onReceiveStatus(status);
            }
        }

        if (prevLevel != level) {
            prevLevel = level;
            for (BatteryStatusReceiver receiver : receivers) {
                receiver.onReceiveLevel(level);
            }
        }
    }

    public void registerToContext(Context context) {
        Intent stickyIntent = context.registerReceiver(this, intentFilter);
        if (stickyIntent != null) {
            onReceive(stickyIntent);
        }
    }

    public void unregisterFromContext(Context context) {
        context.unregisterReceiver(this);
    }

    public void addReceiver(BatteryStatusReceiver receiver) {
        receivers.add(receiver);

        if (prevTemperature != VALUE_UNINITIALIZED) {
            receiver.onReceiveBatteryTemperature(prevTemperature);
        }

        if (prevVoltage != VALUE_UNINITIALIZED) {
            receiver.onReceiveVoltage(prevVoltage);
        }

        if (prevStatus != VALUE_UNINITIALIZED) {
            receiver.onReceiveStatus(prevStatus);
        }

        if (!Float.isNaN(prevLevel)) {
            receiver.onReceiveLevel(prevLevel);
        }
    }

    public void removeReceiver(BatteryStatusReceiver receiver) {
        receivers.remove(receiver);
    }
}
