<?php
	session_start(); //Start the session
	define(ADMIN,$_SESSION['name']); //Get the user name from the previously registered super global variable
	if(!session_is_registered("admin")){ //If session not registered
		header("location:login.php"); // Redirect to login.php page
	}
	else //Continue to current page
		header( 'Content-Type: text/html; charset=utf-8' );

	$deltab = $_GET['dtable'];

	$deltab = stripslashes($deltab);
       $deltab = trim($deltab);
       $deltab = mysqli_real_escape_string($dbC, $deltab);
	echo $deltab;
	$hd = mysql_connect("localhost", "cywrker","")
          or die ("Unable to connect");
       // Select database
       mysql_select_db("cybercomm", $hd)
          or die ("Unable to select database");
	$sql = "DROP TABLE $deltab";
       $res = mysql_query($sql)
          or die ("Unable to run query");
	
	echo $res;
	mysql_close($hd);

	$hd1 = mysql_connect("localhost", "cywrker","")
          or die ("Unable to connect");
       // Select database
       mysql_select_db ("ccommfiles", $hd1)
          or die ("Unable to select database");
	$sql = "DELETE FROM file WHERE tables = '$deltab'";
	echo $sql;
       $res1 = mysql_query($sql1)
          or die ("Unable to run query");
	
	echo $res1;
	mysql_close($hd1);


	header("location:select_tables.php");
?>