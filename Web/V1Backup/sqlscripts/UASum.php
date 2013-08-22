<?php
session_start(); //Start the session
	define(ADMIN,$_SESSION['name']); //Get the user name from the previously registered super global variable
	if(!session_is_registered("admin")){ //If session not registered
		header("location:login.php"); // Redirect to login.php page
	}
	else //Continue to current page
		header( 'Content-Type: text/html; charset=utf-8' );
	
?>
<style type="text/css">
body
{
font-family:arial,helvetica,tahoma,verdana,sans-serif;
font-size:11px;
line-height:14px;
}
</style> 

<?php
      session_start(); //Start the session
	define(ADMIN,$_SESSION['name']); //Get the user name from the previously registered super global variable
	if(!session_is_registered("admin")){ //If session not registered
		header("location:login.php"); // Redirect to login.php page
	}
	else //Continue to current page
		header( 'Content-Type: text/html; charset=utf-8' );

    // Connect to database server
    $hd = mysql_connect("localhost", "cywrker","")
          or die ("Unable to connect");

    // Select database
    mysql_select_db ("cybercomm", $hd)
          or die ("Unable to select database");
 
    // Execute sample query (delete all data in customer table)

    $res = mysql_query("SELECT PolylineID,
	SUM(Fnode),
	SUM(Tnode),
	SUM(LCOVV11),
	SUM(LCOVV21),
	SUM(LCOVV22),
	SUM(LCOVV23),
	SUM(LCOVV24),
	SUM(LCOVV31),
	SUM(LCOVV41),
	SUM(LCOVV42),
	SUM(LCOVV43),
	SUM(LCOVV52),
	SUM(LCOVV71),
	SUM(LCOVV81),
	SUM(LCOVV82),
	SUM(LCOVV90),
	SUM(LCOVV95),
	SUM(LCOVVSUM),
	SUM(slopegradd),
	SUM(slopegradw),
	SUM(wtdepannmi),
	SUM(tfact),
	SUM(weg),
	SUM(om_l),
	SUM(om_r),
	SUM(om_h),
	SUM(dbthirdbar),
	SUM(ksat_l),
	SUM(ksat_r),
	SUM(ksat_h),
	SUM(awc_l),
	SUM(awc_r),
	SUM(awc_h),
	SUM(kffact),
	SUM(brockdepmi),
	SUM(Area),
	SUM(Shape_Length)
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
       FROM UA, UA_US 
       WHERE PolylineID = TARGET_FID ) Prod
WHERE PolylineID = Prod.PolylineID
GROUP BY PolylineID",$hd)
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
