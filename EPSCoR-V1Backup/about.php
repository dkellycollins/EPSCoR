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

<title>Cybercommons > How To </title>

</head>
	"
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

<h3>The How To Page</h3>

<p>
	<pre><h3>Step 1 : </pre></h3> <center>First <a href='load_steps.php'>Upload Tables</a> in .csv (comma delimited) format 
	<br> using the username "ftpuser" and its password
	<br></center>
	<pre><h3>Step 2 : </h3></pre><center> After uploading your tables, you will be sent to a submission page where you can view the 
	<br> progress of your upload as it is added to the database.
	<br></center>
	<pre><h3>Step 3 : </h3></pre><center> After your file conversions are complete, select your loaded Attribute and Field tables
	<br> (based on filename of uploaded files) as well as the calculations you'd like to do. Submit your calculations.
	<br></center>
	<br><h5><center> *** At this point you should be sent back to a submission page where you can see the  <a href='view_subs.php'>progress</a> of
	your calculation. Once finished you will have the option to preview or download the results in .csv format ***</center></h5>
	<br>
</p>



</div> <!-- end #content -->

<?php include('includes/footer.php'); ?>

		</div> <!-- End #wrapper -->

	</body>

</html>
