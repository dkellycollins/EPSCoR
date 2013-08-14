<?php
session_start(); //Start the session
	define(ADMIN,$_SESSION['name']); //Get the user name from the previously registered super global variable
	if(!session_is_registered("admin")){ //If session not registered
		header("location:login.php"); // Redirect to login.php page
	}
	else //Continue to current page
		header( 'Content-Type: text/html; charset=utf-8' );
	
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">

<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <meta name="author" content="" />
    <link rel="stylesheet" type="text/css" href="style.css" media="screen" />

    <title>CIS Cybercommons - KSU 2012</title>
   
</head>
<body>
    <style type="text/css">
body
{
font-family:arial,helvetica,tahoma,verdana,sans-serif;
font-size:11px;
line-height:14px;
}
</style> 
    <div id="wrapper">

    <?php include('includes/header.php'); ?>

    <?php include('includes/nav.php'); ?>

    <div id="content">

    <center>
	
    <p> <center>
	<div align="right">
	>> <a href="upload_steps.php">Skip to Upload How To</a>	
	<div align="right">
	>> <a href="load_t.php">Skip to Upload</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
	</div>
	</div>
    	<h2>How to export your database from Microsoft Access</h2>
	
	<h3> Step 1 </h3>
	<p>
	<pre>First, open up your associated .accdb database file in Microsoft 
	Access 2007 or newer, and look for the tables section on the 
	left side (default)</pre>
	</p>		
	<img src="img/step1.jpg" alt="Step 1" height="429" width="391" />
	<h3> Step 2 </h3>
	<p><pre>
	Right click on the database table you'd like to upload, select "Export", and then select "Text File"
	</p></pre>
	<img src="img/step2.jpg" alt="Step 2" height="429" width="570" />>
	<h3> Step 3 </h3>
	<p><pre>
	At the output filename prompt, type in the name of your output file (the table name is a good name) and change the
	.txt filetype to .csv
	<b>
	<h2>*** NOTE ***</h2> </b>When naming files, it is necessary for you to ensure proper operation by ending all upstream filenames
	with "_US" before the .csv filetype. Otherwise your uploaded table will be assumed as an attribute table i.e. 
	if your attribute filename is CC.csv then your upstream filename would be CC_US.csv
	</p></pre>
	<img src="img/step3.jpg" alt="Step 3" height="429" width="576" />
	<h3> Step 4 </h3>
	<p><pre>
	After clicking OK, make sure in the next pane that the "Delimited" radio button is selected, and then press Next.
	</p></pre>
	<img src="img/step4.jpg" alt="Step 4" height="429" width="570" />
	<h3> Step 5 </h3>
	<p><pre>
	In the next window make sure that the "Comma" radio button is selected, and that the check box next to 
	"Include Field Names on First Row" is ckecked. Click Next.
	</p></pre>
	<img src="img/step5.jpg" alt="Step 5" height="429" width="570" />
	<h3> Step 6 </h3>
	<p><pre>
	At the last step, check to see that your export file is named correctly and that it has the .csv extension. Click
	finish and let Access do its job. Once you have exported all the table files you'd like to upload you can proceed
	to the upload stage.
	</p></pre>
	<img src="img/step6.jpg" alt="Step 6" height="429" width="570" />
	<!--	
	<h3> Step 7 </h3>
	<p><pre>

	</p></pre>
	<img src="img/step7.jpg" alt="Step 7" height="429" width="570" />
	-->	
	</center>
	<div align="right">
	>> <a href="upload_steps.php">Go to Upload How To</a>	
	<div align="right">
	>> <a href="load_t.php">Skip to Upload</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
	</div>
	</div>
    </p>
    <p>
     
    </p>
	
    
    </center>
    </div> <!-- End #wrapper -->

    <?php include('includes/footer.php'); ?>
   
		

    </body>

</html>
