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

<title>CIS Cybercommons - KSU 2012 - Calculations</title>


	<body onLoad="init()">
<div id="loading" style="position:absolute; width:100%; text-align:center; top:300px;">
<img src="loading.gif" border=0></div>

<script>
 var ld=(document.all);
  var ns4=document.layers;
 var ns6=document.getElementById&&!document.all;
 var ie4=document.all;
  if (ns4)
 	ld=document.loading;
 else if (ns6)
 	ld=document.getElementById("loading").style;
 else if (ie4)
 	ld=document.all.loading.style;
  function init()
 {
 if(ns4){ld.visibility="hidden";}
 else if (ns6||ie4) ld.display="none";
 }
 </script>


</head>
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

<?php include('includes/nav.php'); 
	exec('python /var/www/f_man.py >/dev/null &
?>
<script language="javascript">
function reloadIt()
{
frm=document.getElementsByName("pagers")[0];//we get the iframe object
frm.src=frm.src;//or you can set the src to a new src.
setTimeout("reloadIt()",60000);//the function will run every 60000 miliseconds, or 60 seconds
}
</script>');?>

<div id="content">



<table border='0'><tr><td width="15%">
<left><h2>Calculations</h2><br></left>
<?php
	include('/var/www/calc_frame.php');	
?>



</td>

<td width="50%">
<left><h2>Converted Tables</h2><br>
<iframe src="view_subs.php" width="875" height="220" style="overflow-x:hidden" frameborder="0" name="subsFrame" id="subsFrame" scrolling="auto"></iframe> 
</left>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<input type="button" onclick="document.frames('subsFrame').location.reload();" value="Refresh">
</td>

</tr>
</table>

<table border='0'><tr>
<td width=100%>
<center>

<iframe src="table_viewer.php" width="1086" height="565"  style="overflow-y:hidden" frameborder="0" name="tviewFrame" id="tviewFrame" scrolling="yes">

</iframe> 

<tr>
</table>
<table border='0'><tr>
<td width="100%">
<center>
<h2>Table Properties</h2><br></center><left>
<iframe src="table_props.php?sTable=CC_" width="1100" height="600" style="overflow-x:hidden" frameborder="0" name="propsFrame" id="propsFrame" scrolling="auto"></iframe> 
</p><br>	 
</left>

</td>

</tr>

</center>
</td>
</tr>
</table>
</div> <!-- end #content -->

<?php include('includes/footer.php'); ?>


		</div> <!-- End #wrapper -->

	</body>

</html>
