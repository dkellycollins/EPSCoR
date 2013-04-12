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
	
    	<h2>How to upload your database to our server</h2>
	<div align="right">
	>> <a href="load_steps.php">Back to Export How To</a>	
	<div align="right">
	>> <a href="load_t.php">Skip to Upload</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
	</div>
	</div>
	<h3> Step 1 </h3>
	<p>
	In your broswer, click on the menu bar button for this website entitled "Load Table" and run the java applet.
	</p>		
	<img src="img/uprun.jpg" alt="Step 1" height="429" width="600" />
	<h3> Step 2 </h3>
	<p>
	Note in the applet the left and right file viewers which denote local and remote filesystems respectively.	
	</p>		
	<img src="img/upstep1.jpg" alt="Step 1" height="429" width="391" />
	<h3> Step 3 </h3>
	<p>
	Start a new connection to the cybercommons server via SFTP.
	</p>
	<img src="img/upstep2.jpg" alt="Step 2" height="429" width="570" />
	<h3> Step 4 </h3>
	<p>
	Connect using the supplied username and password.
	</p>
	<img src="img/upstep3.jpg" alt="Step 3" height="429" width="576" />
	<h3> Step 5 </h3>
	<p>
	Change the remote working directy to "/var/www/upload"
	</p>
	<img src="img/upstep4.jpg" alt="Step 4" height="429" width="570" />
	<h3> Step 6 </h3>
	<p>
	Select the file you want to upload locally, submit it, and wait for the download manager to finish.
	</p>
	<img src="img/upstep6.jpg" alt="Step 5" height="400" width="670" />
	<p>
	Once uploading is finished, in your browser proceed to the data viewer / calculator.
	</p>

	<div align="right">
	>> <a href="load_steps.php">Back to Upload How To</a>	
	<div align="right">
	>> <a href="load_t.php">Skip to Upload</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
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
