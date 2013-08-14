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
 
// Optional: add the name of XSLT file.
// $xslt_file = "mysql-result.xsl"; 
 
// -------------------------- no changes needed below
 
$DB_link = mysql_connect($host, $user, $pass) or die("Could not connect to host.");
mysql_select_db($database, $DB_link) or die ("Could not find or access the database.");
$result = mysql_query ($SQL_query, $DB_link) or die ("Data not found. Your SQL query didn't work... ");
 
// we produce XML

$XML = "<?xml version=\"1.0\"?>\n";
if ($xslt_file) $XML .= "<?xml-stylesheet href=\"$xslt_file\" type=\"text/xsl\" ?>";
 
// root node
$XML .= "<result>\n";
// rows
header('Content-Disposition: attachment; filename="'.$ptable.'.xml"');

while ($row = mysql_fetch_array($result, MYSQL_ASSOC)) {    
  $XML .= "\t<row>\n"; 
  $i = 0;
  // cells
  foreach ($row as $cell) {
    // Escaping illegal characters - not tested actually ;)
    $cell = str_replace("&", "&amp;", $cell);
    $cell = str_replace("<", "&lt;", $cell);
    $cell = str_replace(">", "&gt;", $cell);
    $cell = str_replace("\"", "&quot;", $cell);
    $col_name = mysql_field_name($result,$i);
    // creates the "<tag>contents</tag>" representing the column
    $XML .= "\t\t<" . $col_name . ">" . $cell . "</" . $col_name . ">\n";
    $i++;
  }
  $XML .= "\t</row>\n"; 
 }
$XML .= "</result>\n";
mysql_close($DB_link);
// output the whole XML string
echo $XML;
