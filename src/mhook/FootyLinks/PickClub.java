package mhook.FootyLinks;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.GridView;
import android.widget.Toast;

public class PickClub  extends Activity {
	
	/** Called when the activity is first created. */
	@Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.pick_club);       
        
        GridView pickClubGridView = (GridView) findViewById(R.id.gridview_pick_club);
        pickClubGridView.setAdapter(new ImageAdapter(this));
        
        pickClubGridView.setOnItemClickListener(new OnItemClickListener() {
            public void onItemClick(AdapterView<?> parent, View v, int position, long id) {
                Toast.makeText(PickClub.this, "" + position, Toast.LENGTH_SHORT).show();
            }
        });
    }
}