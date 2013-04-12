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

<title>CIS Cybercommons - KSU 2012 - Calculation</title>

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

<?php
    // Connect to database server
    $hd = mysql_connect("localhost", "cywrker","")
          or die ("Unable to connect");

    // Select database
    mysql_select_db ("cybercomm", $hd)
          or die ("Unable to select database");
 
    // Execute sample query (delete all data in customer table)
    $tname = $f2 + "-" + $f3;
    $res = mysql_query("CREATE TABLE $tname SELECT PolylineID,
	$f1(Fnode),
	$f1(Tnode),
	$f1(LCOVV11),
	$f1(LCOVV21),
	$f1(LCOVV22),
	$f1(LCOVV23),
	$f1(LCOVV24),
	$f1(LCOVV31),
	$f1(LCOVV41),
	$f1(LCOVV42),
	$f1(LCOVV43),
	$f1(LCOVV52),
	$f1(LCOVV71),
	$f1(LCOVV81),
	$f1(LCOVV82),
	$f1(LCOVV90),
	$f1(LCOVV95),
	$f1(LCOVVSUM),
	$f1(slopegradd),
	$f1(slopegradw),
	$f1(wtdepannmi),
	$f1(tfact),
	$f1(weg),
	$f1(om_l),
	$f1(om_r),
	$f1(om_h),
	$f1(dbthirdbar),
	$f1(ksat_l),
	$f1(ksat_r),
	$f1(ksat_h),
	$f1(awc_l),
	$f1(awc_r),
	$f1(awc_h),
	$f1(kffact),
	$f1(brockdepmi),
	$f1(Area),
	$f1(Shape_Length)
FROM ( SELECT TARGET_FID,
	PolylineID,
	Fnode,
	Tnode,
	LCOVV11,
	LCOVV21,
	LCOVV22,
	LCOVV23,
	LCOVV24,
	LCOVV31,
	LCOVV41,
	LCOVV42,
	LCOVV43,
	LCOVV52,
	LCOVV71,
	LCOVV81,
	LCOVV82,
	LCOVV90,
	LCOVV95,
	LCOVVSUM,
	slopegradd,
	slopegradw,
	wtdepannmi,
	tfact,
	weg,
	om_l,
	om_r,
	om_h,
	dbthirdbar,
	ksat_l,
	ksat_r,
	ksat_h,
	awc_l,
	awc_r,
	awc_h,
	kffact,
	brockdepmi,
	Area,
	Shape_Length
       FROM $f2, $f3 
       WHERE PolylineID = TARGET_FID ) Prod
WHERE $f4
GROUP BY $f5",$hd)
          or die ("Unable to run query");

	function echo_result($result) {
	  ?><table><tr><?
	  if(! $result) { ?><th>result not valid</th><? }
	  else {
	    $i = 0;
	    while ($i < mysql_num_fields($result)) {
	      $meta = mysql_fetch_field($result, $i);
	      ?><th style="white-space:nowrap"><?=$meta->name?></th><?
	      $i++;
	    }
	    ?></tr><?
	   
	    if(mysql_num_rows($result) == 0) {
	      ?><tr><td colspan="<?=mysql_num_fields($result)?>">
	      <strong><center>no result</center></strong>
	      </td></tr><?
	    } else
	      while($row=mysql_fetch_assoc($result)) {
		?><tr style="white-space:nowrap"><?
		foreach($row as $key=>$value) { ?><td><?=$value?></td><? }
		?></tr><?
	      }
	  }
	  ?></table><?
	}

    echo_result($res);

    while(1)

    {
        // Fetch one row
        $row = mysql_fetch_row($res);

        // If no data row found, exit loop
        if (!$row) break;

        // Perform loop on each value in array
        for($nf=0; $nf<count($row); $nf++)
        {
            // Assign value
            $val = $row[$nf];

            // Output single field sequential number and data
            echo "Field # $nf = $val  \n";
        }

        // Separate rows from each other
        echo "----- \n";
    }


    // Close connection
    mysql_close($hd);

?>

d</div> <!-- end #content -->



<?php include('includes/footer.php'); ?>

		</div> <!-- End #wrapper -->

	</body>

</html>
