package mhook.FootyLinks;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteException;
import android.database.sqlite.SQLiteOpenHelper;
import android.util.Log;

/**
 * Database access helper class.
 * Work in progress
 */
public class FootyLinksDbAdapter {

	public static class PlayerColumns {
	    public static final String Name = "Name";
	    public static final String CurrentClub_id = "CurrentClub_id";
	    public static final String _id = "_id";	
	}

    private static final String TAG = "FootyLinksDbAdapter";
    private SQLiteDatabase sqlLiteDatabase;

    private static final String DATABASE_FOLDER = "/data/data/mhook.FootyLinks/databases/";
    private static final String DATABASE_NAME = "footylinks.db";
    private static final String PLAYER_TABLE = "Player";
    private static final int DATABASE_VERSION = 3;

    private final Context context;

    private static class FootyLinksHelper extends SQLiteOpenHelper {    
    	
    	private Context myContext;
    	
    	FootyLinksHelper(Context context) {
            super(context, DATABASE_NAME, null, DATABASE_VERSION);    
            
            this.myContext = context;
        }

    	  /**
         * Creates a empty database on the system and rewrites it with your own database.
         * */
        public void createDataBase() throws IOException{
     
        	boolean dbExist = checkDataBase();
     
        	if(dbExist){
        		//do nothing - database already exist
        	}else{     
        		//By calling this method and empty database will be created into the default system path
                //of your application so we are gonna be able to overwrite that database with our database.
            	this.getReadableDatabase();
     
            	try {     
        			copyDataBase();     
        		} catch (IOException e) {     
            		throw new Error("Error copying database");     
            	}
        	}    
        }
     
        /**
         * Check if the database already exist to avoid re-copying the file each time you open the application.
         * @return true if it exists, false if it doesn't
         */
        private boolean checkDataBase(){
     
        	SQLiteDatabase sqlLiteDatabase = null;     
        	try
        	{  
        		sqlLiteDatabase = openDatabase();    
        	}
        	catch(SQLiteException e){     
        		//database does't exist yet.     
        	}
     
        	if(sqlLiteDatabase != null)
        	{     
        		sqlLiteDatabase.close();     
        	}
     
        	return sqlLiteDatabase != null ? true : false;
        }
     
        /**
         * Copies your database from your local assets-folder to the just created empty database in the
         * system folder, from where it can be accessed and handled.
         * This is done by transfering bytestream.
         * */
        private void copyDataBase() throws IOException{
     
        	//Open your local db as the input stream
        	InputStream myInput = myContext.getAssets().open(DATABASE_NAME);
     
        	// Path to the just created empty db
        	String outFileName = DATABASE_FOLDER + DATABASE_NAME;
     
        	//Open the empty db as the output stream
        	OutputStream myOutput = new FileOutputStream(outFileName);
     
        	//transfer bytes from the inputfile to the outputfile
        	byte[] buffer = new byte[1024];
        	int length;
        	while ((length = myInput.read(buffer))>0){
        		myOutput.write(buffer, 0, length);
        	}
     
        	//Close the streams
        	myOutput.flush();
        	myOutput.close();
        	myInput.close();     
        }
     
        public SQLiteDatabase openDatabase() throws SQLException{     
   	
        	//Open the database
            String myPath = DATABASE_FOLDER + DATABASE_NAME;            
        	return SQLiteDatabase.openDatabase(myPath, null, SQLiteDatabase.OPEN_READONLY);     
        }
     
    	@Override
    	public void onCreate(SQLiteDatabase db) {
     
    	}
     
    	@Override
    	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
     
    	}
     
            // Add your public helper methods to access and get content from the database.
           // You could return cursors by doing "return myDataBase.query(....)" so it'd be easy
           // to you to create adapters for your views.
     
    }
    
 
    /**
     * Constructor - takes the context to allow the database to be
     * opened/created
     * 
     * @param ctx the Context within which to work
     */
    public FootyLinksDbAdapter(Context ctx) {
        this.context = ctx;
    }

    /**
     * Open the notes database. If it cannot be opened, try to create a new
     * instance of the database. If it cannot be created, throw an exception to
     * signal the failure
     * 
     * @return this (self reference, allowing this to be chained in an
     *         Initialisation call)
     * @throws SQLException if the database could be neither opened or created
     */
    public FootyLinksDbAdapter open() throws SQLException {
    	FootyLinksHelper myDbHelper = new FootyLinksHelper(this.context); 
        try { 
        	myDbHelper.createDataBase();
 
        } catch (IOException ioe) { 
        	throw new Error("Unable to create database"); 		
        }
        
        sqlLiteDatabase = myDbHelper.openDatabase();
        	
        return this;
    }

    public void close() {
    	sqlLiteDatabase.close();    
    }
    
    /**
     * Return a Cursor positioned at the note that matches the given rowId
     * 
     * @param Id of player to retrieve
     * @return Cursor positioned to matching note, if found
     * @throws SQLException if note could not be found/retrieved
     */    
    public Cursor fetchPlayer(long Id) throws SQLException {

        Cursor mCursor =
            sqlLiteDatabase.query(true, PLAYER_TABLE, new String[] {PlayerColumns._id,
            		PlayerColumns.Name, PlayerColumns.CurrentClub_id}, PlayerColumns._id + "=" + Id, 
            		null, null, null, null, null);
        if (mCursor != null) {
            mCursor.moveToFirst();
        }
        return mCursor;

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