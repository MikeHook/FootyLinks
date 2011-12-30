package mhook.FootyLinks;

import mhook.FootyLinks.Data.DatabaseAdapter;
import mhook.FootyLinks.Data.FootyLinksSQLLiteHelper;
import android.app.Activity;
import android.content.Intent;
import android.content.res.ColorStateList;
import android.database.Cursor;
import android.graphics.Color;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

public class OneDegree  extends Activity {
	
	public int ClubIndex;
	
	//Note, the order of this array is crucial as it is used in the lookup from the PickClub view
	public String[] Clubs = { 
			"Arsenal",
			"Aston Villa",
			"Birmingham",
			"Blackburn",
			"Blackpool",
			"Bolton",
			"Chelsea",
			"Everton",
			"Fulham",
			"Liverpool",
			"Man City",
			"Man Utd",
			"Newcastle",
			"Stoke",
			"Sunderland",
			"Tottenham",
			"West Brom",
			"West Ham",
			"Wigan",
			"Wolves"			
	};
	
	private DatabaseAdapter footyLinksDbAdapter;
	
	/** Called when the activity is first created. */
	@Override
    public void onCreate(Bundle savedInstanceState) {
		//Generate a random int between 0 and 20
		ClubIndex = (int)(Math.random() * (21));   ;
		
        super.onCreate(savedInstanceState);
        setContentView(R.layout.one_degree);
        
        footyLinksDbAdapter = new DatabaseAdapter(this);        
        footyLinksDbAdapter.open();        
        populateFields();
        populateScore();
        
        TextView guess1Button = (TextView) findViewById(R.id.button_guess1);        
        guess1Button.setOnClickListener(new Guess1ButtonClickListener());
    } 
	
    private void populateFields() {   	
    	
    	//Hard code this for now
    	int clubId = footyLinksDbAdapter.getClubId(Clubs[ClubIndex]);
    	
    	Cursor startPlayerDbCursor = footyLinksDbAdapter.getPlayerByFormerClub(clubId);
    	String startPlayerName = startPlayerDbCursor.getString(
    			startPlayerDbCursor.getColumnIndexOrThrow(FootyLinksSQLLiteHelper.PlayerColumns.Name));
    	
    	TextView startPlayerTextView = (TextView) findViewById(R.id.text_start_player);
    	startPlayerTextView.setText(startPlayerName);
    	
    	Cursor endPlayerDbCursor = footyLinksDbAdapter.getPlayerByCurrentClub(clubId);
    	String endPlayerName = endPlayerDbCursor.getString(
    			endPlayerDbCursor.getColumnIndexOrThrow(FootyLinksSQLLiteHelper.PlayerColumns.Name));
    	
    	TextView endPlayerTextView = (TextView) findViewById(R.id.text_end_player);
    	endPlayerTextView.setText(endPlayerName);
    }
    
	private void populateScore()
	{
		String scoreText = "Your score: " + footyLinksDbAdapter.getScore();
        TextView scoreTextView = (TextView) findViewById(R.id.text_score);
        scoreTextView.setText(scoreText);		
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
        if(intent == null)
        {
        	//User must of clicked the back button instead of making a guess
        	return;
        }
        
        Bundle extras = intent.getExtras();
		switch(requestCode) {
		case 1:
			int guessedClubIndex = extras.getInt(PickClub.ClubIndex);
			//If the chosen clubId matches that of the ClubIndex then the guess is correct, 
			if (guessedClubIndex == ClubIndex)
			{
				Toast.makeText(OneDegree.this, "Congrats, you selected the correct club!", 
						Toast.LENGTH_SHORT).show(); 
				updateViewForCorrectAnswer();
			}
			else
			{
				Toast.makeText(OneDegree.this, "Sorry that's not right, please try again.", 
						Toast.LENGTH_SHORT).show();
			}			
			break;
		}
    }
    
    protected void updateViewForCorrectAnswer()
    {
    	//Increment the score
		int score = footyLinksDbAdapter.getScore();
		footyLinksDbAdapter.updateScore(score+1);
		populateScore();	
		
		//Update the guess button with the answer and disable clicking 
		TextView guess1Button = (TextView) findViewById(R.id.button_guess1);
		guess1Button.setText(Clubs[ClubIndex]);
		guess1Button.setClickable(false);
		guess1Button.setTextColor(Color.GREEN);		
    }    
}


