<style type="text/css">
body
{
font-family:arial,helvetica,tahoma,verdana,sans-serif;
font-size:11px;
line-height:14px;
}
</style> 

<?php
	$dbHost = 'localhost';
	$dbUser = 'cywrker';
	$dbPass = '';
	$dbName = 'members';
	$dbC = mysqli_connect($dbHost, $dbUser, $dbPass, $dbName)
		or die('Error Connecting to MySQL DataBase');
?>
