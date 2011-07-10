package mhook.FootyLinks.Data;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

import android.content.Context;
import android.database.SQLException;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteException;
import android.database.sqlite.SQLiteOpenHelper;

class FootyLinksSQLLiteHelper extends SQLiteOpenHelper {    
	
	private Context myContext;
    private static final String DATABASE_FOLDER = "/data/data/mhook.FootyLinks/databases/";
    private static final String DATABASE_NAME = "footylinks.db";
    private static final int DATABASE_VERSION = 3;
	
	FootyLinksSQLLiteHelper(Context context) {
        super(context, DATABASE_NAME, null, DATABASE_VERSION);    
        
        this.myContext = context;
        
        if(databaseExists())
    	{
    		//do nothing - database already exist
    		return;    		
    	}
    	
    	//Creates an empty db at the default system path, which we can overwrite with our own
    	this.getReadableDatabase();
       	try 
       	{     
       		copyDataBase();     
    	} 
       	catch (IOException e) 
    	{     
       		throw new Error("Error copying database");     
        }    
    }
	
    public SQLiteDatabase openDatabase() throws SQLException {
    	//Open the database
        String myPath = DATABASE_FOLDER + DATABASE_NAME;            
    	return SQLiteDatabase.openDatabase(myPath, null, SQLiteDatabase.OPEN_READONLY);     
    }
	
    //Check if the database already exists to avoid re-copying the file each time you open the application.
    private boolean databaseExists(){

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

    /** Copy the database in the local assets-folder to the just created empty database in the
    	system folder, from where it can be accessed and handled. */
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

	@Override
	public void onCreate(SQLiteDatabase db) {	}

	@Override
	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {	}
}