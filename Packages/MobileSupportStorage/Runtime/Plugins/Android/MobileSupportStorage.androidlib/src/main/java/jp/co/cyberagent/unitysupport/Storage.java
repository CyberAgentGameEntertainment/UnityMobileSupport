package jp.co.cyberagent.unitysupport;

import android.content.Context;
import android.os.Build;
import android.os.storage.StorageManager;

import java.io.File;
import java.io.IOException;
import java.util.UUID;

public class Storage {
    private Storage() {}

    public static long getInternalUsableSpace(Context context, boolean isAccurate, long wantSpace) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O && isAccurate) {
            // Android 8.0以上は、削除可能な領域含めて計算する
            return getInternalUsableSpaceAboveO(context, wantSpace);
        }

        return getInternalUsableSpaceBelowO(context);
    }

    public static long getInternalUsableSpaceBelowO(Context context) {
        File file = context.getFilesDir();
        long usableSpace = file.getUsableSpace();
        return usableSpace;
    }

    public static long getInternalUsableSpaceAboveO(Context context, long wantSpace) {
        try {
            StorageManager storageManager = context.getSystemService(StorageManager.class);
            UUID appSpecificInternalDirUuid = storageManager.getUuidForPath(context.getFilesDir());
            long availableBytes = storageManager.getAllocatableBytes(appSpecificInternalDirUuid);
            if (wantSpace > 0 && availableBytes >= wantSpace) {
                storageManager.allocateBytes(appSpecificInternalDirUuid, wantSpace);
            }
            return availableBytes;
        } catch (IOException e) {
            return -1;
        }
    }
}
