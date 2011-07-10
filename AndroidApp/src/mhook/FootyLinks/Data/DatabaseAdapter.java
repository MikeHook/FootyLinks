package mhook.FootyLinks.Data;

import java.io.IOException;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;
import android.util.Log;

/**
 * Database access helper class.
 * Work in progress
 */
public class DatabaseAdapter {

	public static class Tables {
		public static final String Club = "Club";
		public static final String Player = "Player";
		public static final String PlayerClub = "PlayerClub";
		
	}
	
	public static class ClubColumns {
	    public static final String _id = "_id";		
	    public static final String Name = "Name";
	    public static final String CompactName = "CompactName";
	}
	
	public static class PlayerClubColumns {
	    public static final String Club_id = "Club_id";
	    public static final String Player_id = "Player_id";
	}
	
	public static class PlayerColumns {
	    public static final String Name = "Name";
	    public static final String CurrentClub_id = "CurrentClub_id";
	    public static final String _id = "_id";	
	}

    private static final String TAG = "DatabaseAdapter";
    private SQLiteDatabase sqlLiteDatabase;    
    private final Context context;

    public DatabaseAdapter(Context context) {
        this.context = context;
    }

    /**
     * Open the database, it will be created in the constructor if it doesn't already exist     * 
     * @return this (self reference, allowing this to be chained in an Initialisation call)
     * @throws SQLException if the database could be neither opened or created
     */    
    public DatabaseAdapter open() throws SQLException {
    	FootyLinksSQLLiteHelper footyLinksSQLLiteHelper = new FootyLinksSQLLiteHelper(this.context);       
        sqlLiteDatabase = footyLinksSQLLiteHelper.openDatabase();        	
        return this;
    }

    public void close() {
    	sqlLiteDatabase.close();    
    }
    
    //Fetch the clubId for the passed in compactName    
    public int getClubId(String compactName) throws SQLException {

        Cursor mCursor =
            sqlLiteDatabase.query(true, Tables.Club, 
            		new String[] {ClubColumns._id}, 
            		ClubColumns.CompactName + "= '" + compactName + "'", 
            		null, null, null, null, null);
        if (mCursor != null) {
            mCursor.moveToFirst();
            return mCursor.getInt(0);
        }
        
        return -1;
    }
    
    /**
     * Fetch a random current player for the passed in clubIdd
     * 
     * @param Id of player to retrieve
     * @return Cursor positioned to matching player, if found
     * @throws SQLException if player could not be found/retrieved
     */    
    public Cursor getPlayerByCurrentClub(int clubId) throws SQLException {

        Cursor mCursor =
            sqlLiteDatabase.query(true, Tables.Player, 
            		new String[] {PlayerColumns._id, PlayerColumns.Name, PlayerColumns.CurrentClub_id}, 
            		PlayerColumns.CurrentClub_id + "=" + clubId, 
            		null, null, null, null, null);
        if (mCursor != null) {
    		moveCursorToRandomPosition(mCursor); 
    	}
        
        return mCursor;
    }
    
    /**
     * Fetch a random former player for the passed in clubId      * 
     * @param clubId
     * @return Cursor positioned to randomly selected former player
     * @throws SQLException
     */
    public Cursor getPlayerByFormerClub(int formerClubId)  throws SQLException 
    {
    	int playerId = getPlayerIdByFormerClubId(formerClubId);    	
    	if (playerId == -1)
    	{
    		return null;
    	}
    	
    	Cursor mCursor =
            sqlLiteDatabase.query(true, Tables.Player, 
            		new String[] {PlayerColumns._id, PlayerColumns.Name, PlayerColumns.CurrentClub_id}, 
            		PlayerColumns._id + "=" + playerId, 
            		null, null, null, null, null);
        if (mCursor != null) {
            mCursor.moveToFirst();
        }
        return mCursor;    	
    }
    
    private int getPlayerIdByFormerClubId(int formerClubId) 
    {
    	Cursor mCursor =
            sqlLiteDatabase.query(true, Tables.PlayerClub, new String[] {PlayerClubColumns.Player_id}, 
            		PlayerClubColumns.Club_id + "=" + formerClubId, 
            		null, null, null, null, null);
        
    	if (mCursor != null) {
    		moveCursorToRandomPosition(mCursor);    		
    		//Returns the integer in column 0, this is the PlayerId
            return mCursor.getInt(0);
        }
        return -1;    
    }    
    
    private void moveCursorToRandomPosition(Cursor cursor){
    	int rowCount = cursor.getCount();
		int randomRowPosition = (int)(Math.random() * (rowCount + 1));    		
        cursor.moveToPosition(randomRowPosition);    	
    }
    
    /**
     * Return a Cursor positioned at the note that matches the given rowId     * 
     * @param rowId id of note to retrieve
     * @return Cursor positioned to matching note, if found
     * @throws SQLException if note could not be found/retrieved
     */
    /*
    public Cursor fetchNote(long rowId) throws SQLException {

        Cursor mCursor =

            mDb.query(true, DATABASE_TABLE, new String[] {KEY_ROWID,
                    KEY_TITLE, KEY_BODY}, KEY_ROWID + "=" + rowId, null,
                    null, null, null, null);
        if (mCursor != null) {
            mCursor.moveToFirst();
        }
        return mCursor;

    }*/
}