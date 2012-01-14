package mhook.FootyLinks.Data;

import java.io.IOException;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.DatabaseUtils;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;
import android.util.Log;

/**
 * Database access helper class.
 * Work in progress
 */
public class DatabaseAdapter {

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
    
    public boolean updateScore(int newScore) {
        ContentValues args = new ContentValues();
        args.put(FootyLinksSQLLiteHelper.ScoreColumns.HighScore, newScore);

        return sqlLiteDatabase.update(FootyLinksSQLLiteHelper.Tables.Score, 
        		args, FootyLinksSQLLiteHelper.ScoreColumns._id + "=" + 0, null) > 0;
    }
    
    public int getScore() {
    	
    	long rows = DatabaseUtils.queryNumEntries(sqlLiteDatabase, FootyLinksSQLLiteHelper.Tables.Score);
    	
    	long rowId;
    	if (rows == 0)
    	{
        	//If no score exists then insert one
        	ContentValues initialValues = new ContentValues();
        	initialValues.put(FootyLinksSQLLiteHelper.ScoreColumns._id, 0);
        	initialValues.put(FootyLinksSQLLiteHelper.ScoreColumns.HighScore, 0);           
            rowId = sqlLiteDatabase.insert(FootyLinksSQLLiteHelper.Tables.Score, null, initialValues);     	
    	}
    	
    	Cursor mCursor =
            sqlLiteDatabase.query(true, FootyLinksSQLLiteHelper.Tables.Score, 
            		new String[] {	FootyLinksSQLLiteHelper.ScoreColumns._id , 
            						FootyLinksSQLLiteHelper.ScoreColumns.HighScore}, 
            		FootyLinksSQLLiteHelper.ScoreColumns._id + "=0", 
            		null, null, null, null, null);        
        if (mCursor != null) {
            mCursor.moveToFirst();
            return mCursor.getInt(1);
        }     
        
        return 0;
    }
    
    //Fetch the clubId for the passed in compactName    
    public int getClubId(String compactName) throws SQLException {

        Cursor mCursor =
            sqlLiteDatabase.query(true, FootyLinksSQLLiteHelper.Tables.Club, 
            		new String[] {FootyLinksSQLLiteHelper.ClubColumns._id}, 
            		FootyLinksSQLLiteHelper.ClubColumns.CompactName + "= '" + compactName + "'", 
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
            sqlLiteDatabase.query(true, FootyLinksSQLLiteHelper.Tables.Player, 
            		new String[] {FootyLinksSQLLiteHelper.PlayerColumns._id, 
            		FootyLinksSQLLiteHelper.PlayerColumns.Name, 
            		FootyLinksSQLLiteHelper.PlayerColumns.CurrentClub_id}, 
            		FootyLinksSQLLiteHelper.PlayerColumns.CurrentClub_id + "=" + clubId + 
            		" AND " + FootyLinksSQLLiteHelper.PlayerColumns.SquadNumber + "< 26" +
            		" AND " + FootyLinksSQLLiteHelper.PlayerColumns.Age + "< 36" 
            		, 
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
            sqlLiteDatabase.query(true, FootyLinksSQLLiteHelper.Tables.Player, 
            		new String[] {FootyLinksSQLLiteHelper.PlayerColumns._id, FootyLinksSQLLiteHelper.PlayerColumns.Name, FootyLinksSQLLiteHelper.PlayerColumns.CurrentClub_id}, 
            		FootyLinksSQLLiteHelper.PlayerColumns._id + "=" + playerId, 
            		null, null, null, null, null);
        if (mCursor != null) {
            mCursor.moveToFirst();
        }
        return mCursor;    	
    }
    
    private int getPlayerIdByFormerClubId(int formerClubId) 
    {
    	Cursor mCursor =
            sqlLiteDatabase.query(true, FootyLinksSQLLiteHelper.Tables.PlayerClub, new String[] {FootyLinksSQLLiteHelper.PlayerClubColumns.Player_id}, 
            		FootyLinksSQLLiteHelper.PlayerClubColumns.Club_id + "=" + formerClubId, 
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
		int randomRowPosition = (int)(Math.random() * (rowCount));    		
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