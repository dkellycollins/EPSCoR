<div id="sidebar">

<!--
<h3>Navigation</h3>
	<li><a href="index.php">Home</a></li>
	<li><a href="about.php">How To</a></li>
-->

<?php
	
	session_start(); //Start the session
	define(ADMIN,$_SESSION['name']); //Get the user name from the previously registered super global variable
	if(!session_is_registered("admin")){ //If session not registered
		//header("location:login.php"); // Redirect to login.php page
	}
	else{ //Continue to current page
		echo "<h3>Actions</h3>";
		echo "Step 1: <li><a href='load_steps.php'>Upload New Tables</a></li><br>";
		echo "Step 2: <li><a href='select_tables.php'>Select Tables / Calcs</a></li><br>";
		echo "Step 3: <li><a href='view_subs.php'>View Submissions</a></li><br>";

		//echo "View:<br>";
		//echo "<a href='view_subs.php'>Uploads</a><br>";
		//echo "<a href='view_subs.php'>Active Calcs</a>";
		
	}
?>
</div> <!-- end #sidebar -->

