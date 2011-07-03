package mhook.FootyLinks;

import android.app.Activity;
import android.content.Intent;
import android.database.Cursor;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

public class OneDegree  extends Activity {
	
	private FootyLinksDbAdapter footyLinksDbAdapter;
    private TextView startPlayerTextView;
	
	/** Called when the activity is first created. */
	@Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.one_degree);
        
        footyLinksDbAdapter = new FootyLinksDbAdapter(this);
        
        footyLinksDbAdapter.open();
        
        startPlayerTextView = (TextView) findViewById(R.id.text_start_player);   
        
        populateFields();
        
        Button guess1Button = (Button) findViewById(R.id.button_guess1);        
        guess1Button.setOnClickListener(new Guess1ButtonClickListener());        
    } 
	
    private void populateFields() {   	
    	
    	Cursor footyLinksDbCursor = footyLinksDbAdapter.fetchPlayer(49842);
    	String startPlayerName = footyLinksDbCursor.getString(
    			footyLinksDbCursor.getColumnIndexOrThrow(FootyLinksDbAdapter.PlayerColumns.Name));
    	
    	startPlayerTextView.setText(startPlayerName);
    }
	
    public class Guess1ButtonClickListener implements OnClickListener {

		public void onClick(View v) {
			
			Intent pickClubIntent = new Intent(OneDegree.this, PickClub.class);  			
			startActivityForResult(pickClubIntent, 1);
		}    	
    }
	
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent intent) {
        super.onActivityResult(requestCode, resultCode, intent);
        Bundle extras = intent.getExtras();
		switch(requestCode) {
		case 1:
			int clubId = extras.getInt("ClubId");
			//As this is just a prototype, hard code the clubId here, 
			//The answer is Chelsea, whose id is 6 
			if (clubId == 6)
			{
				Toast.makeText(OneDegree.this, "Congrats, you selected the correct club!", 
						Toast.LENGTH_SHORT).show(); 
			}
			else
			{
				Toast.makeText(OneDegree.this, "Sorry that's not right, please try again.", 
						Toast.LENGTH_SHORT).show();
			}			
			break;
		}
    }
}


