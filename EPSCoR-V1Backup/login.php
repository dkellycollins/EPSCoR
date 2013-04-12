<?php
	session_start(); //Start the session
	define(ADMIN,$_SESSION['name']); //Get the user name from the previously registered super global variable
	if(session_is_registered("admin")){ //If session not registered
		header("location:index.php"); // Redirect to login.php page
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

<p>
<?php
    $login_form = <<<EOD
<table width="300" border="0" align="center" cellpadding="0" cellspacing="1" bgcolor="#CCCCCC">
<tr>
<form name="login" method="post" action="check.php">
<td>
<table width="100%" border="0" cellpadding="3" cellspacing="1" bgcolor="#FFFFFF">
<tr>
<td colspan="3"><strong>Member Login </strong></td>
</tr>
<tr>
<td width="78">Username</td>
<td width="6">:</td>
<td width="294"><input name="username" type="text" id="username"></td>
</tr>
<tr>
<td>Password</td>
<td>:</td>
<td><input name="password" type="password" id="password"></td>
</tr>
<tr>
<td>&nbsp;</td>
<td>&nbsp;</td>
<td><input type="submit" name="Submit" value="Login"><input type="reset" name="reset" id="reset" value="Reset"/></td>
</tr>
</table>
</td>
</form>
</tr>
</table>
EOD;
function sanitize($data){
	//remove spaces from the input
	$data=trim($data);
	//convert special characters to html entities
	//most hacking inputs in XSS are HTML in nature, so converting them to special characters so that they are not harmful
	$data=htmlspecialchars($data);
	//sanitize before using any MySQL database queries
	//this will escape quotes in the input.
	$data = mysql_real_escape_string($data);
	return $data;
}
//$msg = mysqli_real_escape_string(trim(stripslashes($_GET['msg'])));  //GET the message
//if($msg!='') echo '<center><p>'.$msg.'</p></center>'; //If message is set echo it
echo "<center><h2>Please enter your Login Information</h2></center>";
echo $login_form;
?>
</p>

</div> <!-- end #content -->



<?php include('includes/footer.php'); ?>
<?php

?>
		</div> <!-- End #wrapper -->

	</body>

</html>
