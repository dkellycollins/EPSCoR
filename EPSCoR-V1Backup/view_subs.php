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
<style>
			td {
				
				height: 1em;
				padding: 1em;
				text-align: left;	
			}

			tr:hover {
				cursor: pointer;
				background-color: whitesmoke;
			}
			.select {
				background-color: #0ea026;	
			}
		</style>
		<script type="text/javascript"><!--
var td1 = null;
var td2 = null;
var td3 = null;
var td4 = null;
var td5 = null;

function highlight(obj) {
	if (td1 || td2 || td3 || td4 || td5) {
		td1.className = null;
		td2.className = null;
		td3.className = null;
		td4.className = null;
		td5.className = null;
			}

	obj.cells[0].className = "select";
	obj.cells[1].className = "select";
	obj.cells[2].className = "select";
	obj.cells[3].className = "select";
	obj.cells[4].className = "select";

	td1 = obj.cells[0];
	td2 = obj.cells[1];
	td3 = obj.cells[2];
	td4 = obj.cells[3];
	td5 = obj.cells[4];
}
//-->
</script>
<meta http-equiv="content-type" content="text/html; charset=utf-8" />

<meta name="description" content="" />

<meta name="keywords" content="" />

<meta name="author" content="" />

<link rel="stylesheet" type="text/css" href="style.css" media="screen" />

<title>CIS Cybercommons - KSU 2012</title>

</head>
<style type="text/css">
body
{
font-family:arial,helvetica,tahoma,verdana,sans-serif;
font-size:11px;
line-height:12px;
}
</style> 
<script languaje="javascript">
	function reloadIt()
	{
		frm=document.getElementsByName("subsFrame")[0];//we get the iframe object
		frm.src=frm.src;//or you can set the src to a new src.
		setTimeout("reloadIt()",60000);//the function will run every 60000 miliseconds, or 60 seconds
	}
</script>
	<body onload="reloadIt()">

		
<div id="content">
<left>
<?php
	define(DOC_ROOT,dirname(__FILE__)); 
	//passthru('python ' . $_SERVER['DOCUMENT_ROOT'] . 'f_man.py');
	//$att_table = $_POST['Attribute'];

	$db_host = 'localhost';
	$db_user = 'cywrker';
	$db_pwd = '';

	$database = 'ccommfiles';
	$table = 'file';

	if (!mysql_connect($db_host, $db_user, $db_pwd))
	    die("Can't connect to database");

	if (!mysql_select_db($database))
	    die("Can't select database");

	// sending query
	$result = mysql_query("SELECT * FROM {$table}");
	if (!$result) {
	    die("Query to show fields from table failed");
	}

	$fields_num = mysql_num_fields($result);

	//echo "<h1>Recent Uploads : </h1>";
	echo "<table border='0'><tr>";
	// printing table headers
	for($i=0; $i<$fields_num; $i++)
	{
	    $field = mysql_fetch_field($result);
	    echo "<td>{$field->name}</td>";
	}
	echo "<td></td>";
	echo "</tr>\n";
	// printing table rows
	while($row = mysql_fetch_row($result))
	{
	    echo "<tr onclick='highlight(this);'>";

	    // $row is array... foreach( .. ) puts every element
	    // of $row to $cell variable
	    foreach($row as $cell)
		echo "<td>$cell</td>";
	       echo "<td>
		<form name=xml method='post' action='table2xml.php?ptable=$row[0]'>
		<input type='submit' name='$row[0]' value='XML'></form></td>";
		echo "<td>
		<form name=csv method='post' action='table2csv.php?ptable=$row[0]'>
		<input type='submit' name='$row[0]' value='CSV'></form></td>";
		echo "<td>
		<form name='del$row[0]' method='post' action='delTab.php?dtable=$row[0]'>
		<input type='submit' name='del$row[0]' value='X' style='background-color:#c00; color:#fff;'></form></td>";
	    
	    echo "</tr>\n";
	}
	echo "</table>";
	mysql_free_result($result);

	// build the attribute table
	function create_att_tab($filename){
		$hd_f2 = mysql_connect("localhost", "cywrker","")
          		or die ("Unable to connect");

         	mysql_select_db ("cybercomm", $hd_f2)
          		or die ("Unable to select database");

         	mysql_query("CREATE TABLE IF NOT EXISTS $filename (ID float,
			OBJECTID_1 float,
			TARGET_FID float,
			PRIMARY KEY(TARGET_FID),
			Fnode float,
			Tnode float,
			LCOVV11 float,
			LCOVV21 float,
			LCOVV22 float,
			LCOVV23 float,
			LCOVV24 float,
			LCOVV31 float,
			LCOVV41 float,
			LCOVV42 float,
			LCOVV43 float,
			LCOVV52 float,
			LCOVV71 float,
			LCOVV81 float,
			LCOVV82 float,
			LCOVV90 float,
			LCOVV95 float,
			LCOVVSUM float,
			slopegradd float,
			slopegradw float,
			wtdepannmi float,
			tfact float,
			weg float,
			om_l float,
			om_r float,
			om_h float,
			dbthirdbar float,
			ksat_l float,
			ksat_r float,
			ksat_h float,
			awc_l float,
			awc_r float,
			awc_h float,
			kffact float,
			brockdepmi float,
			Area float,
			Shape_Length float
			) ENGINE = InnoDB DEFAULT CHARSET=latin1",$hd_f2)
          		   or die ("Unable to run query");	

    		mysql_close($hd_f2);
	}
         
?>


</left>
</div> <!-- end #content -->

	</body>

</html>
