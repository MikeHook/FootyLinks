package mhook.FootyLinks;

import java.util.Arrays;
import java.util.List;

import mhook.FootyLinks.Data.DatabaseAdapter;
import mhook.FootyLinks.Data.FootyLinksSQLLiteHelper;
import android.app.Activity;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Color;
import android.os.Bundle;
import android.text.SpannableString;
import android.text.style.UnderlineSpan;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

public class OneDegree  extends Activity {
	
	private Integer ScorePenalty;
	private Integer ClubIndex;	
	//Note, the order of this array is crucial as it is used in the lookup from the PickClub view
	private String[] Clubs = { 
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
	//These are the 'top six' clubs in the Clubs array
	private List<Integer> TopClubIndexes = Arrays.asList(0, 6, 9, 10, 11, 15);		
	private DatabaseAdapter footyLinksDbAdapter;
	
	/** Called when the activity is first created. */
	@Override
    public void onCreate(Bundle savedInstanceState) {
		
        super.onCreate(savedInstanceState);
        setContentView(R.layout.one_degree);
        
        footyLinksDbAdapter = new DatabaseAdapter(this);        
        footyLinksDbAdapter.open();        
        populateFields();
        populateScore();
        
        TextView guess1Button = (TextView) findViewById(R.id.button_guess1);        
        guess1Button.setOnClickListener(new Guess1ButtonClickListener());
        
        Button restartButton = (Button) findViewById(R.id.button_restart);        
        restartButton.setOnClickListener(new RestartButtonClickListener());
    } 
	
	/** Button Listeners */
    public class Guess1ButtonClickListener implements OnClickListener {
		public void onClick(View v) {
			
			Intent pickClubIntent = new Intent(OneDegree.this, PickClub.class);  			
			startActivityForResult(pickClubIntent, 1);
		}    	
    }
    
    public class RestartButtonClickListener implements OnClickListener {
		public void onClick(View v) {		
			
	    	//Decrement the score
			int score = footyLinksDbAdapter.getScore();
			footyLinksDbAdapter.updateScore(score-ScorePenalty);
			populateScore();	
			
			populateFields();
		}    	
    }
	
    /** Called from PickClub to pass back the selected guess ID */
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
    
    /** private helper methods */
    private void populateFields() {
    	
    	ScorePenalty = 1;
    	
		//Generate a random int between 0 and 19, representing the index of the club answer
		ClubIndex = (int)(Math.random() * 20);
    	
    	//Hard code this for now
    	int clubId = footyLinksDbAdapter.getClubId(Clubs[ClubIndex]);
    	
    	//Start player
    	Cursor startPlayerDbCursor = footyLinksDbAdapter.getPlayerByFormerClub(clubId);
    	String startPlayerName = startPlayerDbCursor.getString(
    			startPlayerDbCursor.getColumnIndexOrThrow(FootyLinksSQLLiteHelper.PlayerColumns.Name));
    	
    	TextView startPlayerTextView = (TextView) findViewById(R.id.text_start_player);
    	startPlayerTextView.setText(startPlayerName);
    	
    	//Guess button
		//Update the guess button with the question and enable clicking 
		TextView guess1Button = (TextView) findViewById(R.id.button_guess1);
		SpannableString content = new SpannableString("what club? ");
		content.setSpan(new UnderlineSpan(), 0, content.length(), 0);
		guess1Button.setText(content);
		guess1Button.setClickable(true);
		guess1Button.setTextColor(Color.parseColor("#4876FF"));		
    	
    	//End player
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
    
    private void updateViewForCorrectAnswer()
    {
    	Integer points = 2;
    	if (TopClubIndexes.contains(ClubIndex))
    	{
    		// Guessing these answers should be easier so give less points	
    		points = 1;
    	}
    	
    	//Increment the score
		int score = footyLinksDbAdapter.getScore();
		footyLinksDbAdapter.updateScore(score+points);
		populateScore();	
		
		//Update the guess button with the answer and disable clicking 
		TextView guess1Button = (TextView) findViewById(R.id.button_guess1);
		guess1Button.setText(Clubs[ClubIndex] + " ");
		guess1Button.setClickable(false);
		guess1Button.setTextColor(Color.GREEN);
		
		ScorePenalty = 0;
    }    
}


