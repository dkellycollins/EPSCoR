<?php
	session_start(); //Start the session
	define(ADMIN,$_SESSION['name']); //Get the user name from the previously registered super global variable
	if(!session_is_registered("admin")){ //If session not registered
		header("location:login.php"); // Redirect to login.php page
	}
	else //Continue to current page
		header( 'Content-Type: text/html; charset=utf-8' );
	
// database constants
// make sure the information is correct
$ptable = $_GET['ptable']; //Set table

	$db_host = 'localhost';
	$db_user = 'cywrker';
	$db_pwd = '';
	$db_name = 'cybercomm';
	error_reporting(E_ALL);
 
// ---------------------------  Configuration section
 
// Fill in according to your db settings (example is for a local and locked "play" WAMP server)
// Make sure to keep the ; and the ""
$host       = "localhost";
$user       = "cywrker";
$pass       = "";
$database   = "cybercomm";
 
// Replace by a query that matches your database
$SQL_query = "SELECT * FROM $ptable";
 
$DB_link = mysql_connect($host, $user, $pass) or die("Could not connect to host.");
mysql_select_db($database, $DB_link) or die ("Could not find or access the database.");
$result = mysql_query ($SQL_query, $DB_link) or die ("Data not found. Your SQL query didn't work... ");
 
// we produce XML

$XML = "";

// rows
header('Content-Disposition: attachment; filename="'.$ptable.'.csv"');

$columns = "";
$h = 0;
while ($row = mysql_fetch_array($result, MYSQL_ASSOC)) {    
  $i = 0;
  // cells
  foreach ($row as $cell) {
    
    $cell .= ","; 
    $cell = str_replace("&", "&amp;", $cell);
    $cell = str_replace("<", "&lt;", $cell);
    $cell = str_replace(">", "&gt;", $cell);
    $col_name = mysql_field_name($result,$i); 
	if ($h == 0 ){
    	if ($i != 0){	
    		$columns .= ",";
    	}
    	$columns .= "\"" . $col_name . "\"";
    }
    $XML .= $cell;
    $i++;
  }
  $h = 1;
  $XML = substr($XML,0,-2);
  $XML .= "\n"; 
}
mysql_close($DB_link);
// output the whole XML string
//$XML .= $columns . "\n" . $XML;
echo $columns;
echo "\n";
echo $XML;
