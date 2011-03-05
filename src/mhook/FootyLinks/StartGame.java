package mhook.FootyLinks;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.Spinner;

public class StartGame extends Activity {
    	
	private Spinner spinner;
	
	/** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.start_game);
        
        spinner = (Spinner) findViewById(R.id.spinner_difficulty);
        ArrayAdapter<CharSequence> adapter = ArrayAdapter.createFromResource(
                this, R.array.difficulty_array, android.R.layout.simple_spinner_item);
        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
        spinner.setAdapter(adapter);
        
        Button startButton = (Button) findViewById(R.id.button_start);
        startButton.setOnClickListener(new StartButtonClickListener());
    }   

    public class StartButtonClickListener implements OnClickListener {

		@Override
		public void onClick(View v) {
			
			Intent intent = new Intent(StartGame.this, OneDegree.class);     
            startActivity(intent);	
		}    	
    } 
}

