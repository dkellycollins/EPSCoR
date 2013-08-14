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
<p>
<?php
	
?>

</p>
</center>

</div> <!-- end #content -->


<?php include('includes/footer.php'); ?>

		</div> <!-- End #wrapper -->

	</body>

</html>
