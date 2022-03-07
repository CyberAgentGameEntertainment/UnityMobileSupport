package jp.co.cyberagent.unitysupport.app;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.widget.TextView;

import jp.co.cyberagent.unitysupport.Storage;

public class MainActivity extends AppCompatActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        TextView textViewUsableSpace = findViewById(R.id.textViewUsableSpaceValue);
        long usableSpace = Storage.getInternalUsableSpaceBelowO(getApplicationContext());
        textViewUsableSpace.setText(String.valueOf(usableSpace));
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
            TextView textViewUsableSpaceAboveO = findViewById(R.id.textViewUsableSpaceAboveOValue);
            long usableSpaceAbove0 = Storage.getInternalUsableSpaceAboveO(getApplicationContext(), -1);
            textViewUsableSpaceAboveO.setText(String.valueOf(usableSpaceAbove0));
        }
    }
}
