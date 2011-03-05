package mhook.FootyLinks;

import mhook.FootyLinks.StartGame.StartButtonClickListener;
import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;
import android.widget.Toast;

public class OneDegree  extends Activity {
	
	/** Called when the activity is first created. */
	@Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.one_degree);       
        
        /**
        Button startButton = (Button) findViewById(R.id.button_start);        
        startButton.setOnClickListener(new StartButtonClickListener());
        */
    }   

}
