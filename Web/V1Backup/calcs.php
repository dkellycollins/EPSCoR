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

<title>CIS Cybercommons - KSU 2012 - Calculated Table</title>

</head>

	<body onLoad="initTime()">
<!--<div id="loading" style="position:absolute; width:100%; text-align:center; top:300px;">
<img src="loading.gif" border=0></div>-->

<script type="text/javascript">
        var time0;
        var stopt = 0;
		function initTime() {
            time0 = new Date();
            window.setInterval("updateTime()", 100);
        }
        function updateTime() {
			if(stopt != 1){
				var timeNow = new Date();
				var deltas = (Number(timeNow) - Number(time0))/1000;
				var deltah = ("0"+String(Math.round(deltas / 3600))).substr(-2);
				deltah = deltah.substr(-2);
				deltas %= 3600;
				var deltam = ("0"+String(Math.round(deltas / 60))).substr(-2);
				deltas = ("0"+String(Math.round(deltas % 60))).substr(-2);
				document.getElementById("timedisplay").firstChild.data=deltah+":"+deltam+":"+deltas+":"+timeNow.getMilliseconds();
			}
			else{
				time0 = new Date();
			}
        }
		function killTime(){
			stopt = 1;
		}
</script>


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
<style type="text/css">
body
{
font-family:arial,helvetica,tahoma,verdana,sans-serif;
font-size:11px;
line-height:14px;
}
</style> 

<div id="content">

<center>
<p>

<?php
	define(DOC_ROOT,dirname(__FILE__)); 
	$att_table = $_POST['Attribute'];
	$field_table = $_POST['Upstream'];
        $f1 = $_POST['calc'];
	$f2 = "US_POLYID = Prod.US_POLYID";
	$f3 = "US_POLYID";
	$msg ='';
	

   function q($cmm){
    // Connect to database server
    $hd = mysql_connect("localhost", "cywrker","")
          or die ("Unable to connect");

    // Select database
    mysql_select_db ("cybercomm", $hd)
          or die ("Unable to select database");
 
    // Execute sample query 
	?><p><? 
	echo "\n Giving the server time to process ... \n";
    ?><p><?
	$res = mysql_query($cmm,$hd)
          or die ("Unable to run query ...");

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
   }
   
	session_start();
	define(ADMIN,$_SESSION['name']);
	$db_host = 'localhost';
	$db_user = 'cywrker';
	$db_pwd = '';
	$datbase = 'cybercomm';
	$tble = $att_table;	
	?><p><div> Elapsed Time <span id="timedisplay">00:00:00:00</span></div></p><?
	echo "\n Connecting ... \n";
	$hd1 = mysql_connect("localhost", "cywrker","")
          or die ("Unable to connect");

    	// Select database
    	mysql_select_db ("cybercomm", $hd1)
          or die ("Unable to select database");

	// sending query
	$rsult = mysql_query("SHOW COLUMNS FROM {$tble}",$hd1);
	if (!$rsult) {
	    die("Query to show fields from table failed");
	}

	$f_num = mysql_num_fields($rsult);

	$comm1 = "SELECT POLYLINEID, ARCID, US_POLYID";
	$comm_f1 = "";
	$head_f1 = "";

	$comm2 = "SELECT POLYLINEID, ARCID, US_POLYID";
	$comm_f2 = "";
	$head_f2 = "";
	
	if (mysql_num_rows($rsult) > 0) {
	    while ($row = mysql_fetch_row($rsult)) {
		$r = $row[0];
		if ($r != "ID" and $r != "ARCID" and $r != "OBJECTID" and $r != "uni"){
			$comm_f1 = "$comm_f1, $f1($r)";
			$head_f1 = "$head_f1, $r";

			$comm_f2 = "$comm_f2, $r";
			$head_f2 = "$head_f2, $r";
		}
	    }
	}
	mysql_free_result($rsult);
	mysql_close($hd1);

	//$r1 = substr($comm_f, 0, -1);
	//$r2 = substr($head_f, 0, -1);
	$su1 = "_";
	$su2 = "_CALC";

	$comm00 = "CREATE TABLE $att_table$su1$field_table$su2 $comm1$comm_f1 
		    FROM $att_table, $field_table 
		    WHERE ARCID = POLYLINEID 
		    GROUP BY POLYLINEID
		    ORDER by ARCID";
		
       // join without sum
	$comm00 = "$comm2$comm_f2
		    FROM $att_table, $field_table
		    WHERE ARCID = US_POLYID
		    ORDER BY POLYLINEID
		    LIMIT 40";

	// join with sum
	$comm0 = "$comm1$comm_f1
		    FROM ($comm2$comm_f2
		    	FROM $att_table, $field_table
		    	WHERE ARCID = US_POLYID) Prod
		    GROUP BY Prod.POLYLINEID
		    LIMIT 40";
	
	// debug commands
	$comm11 = "$comm$comm_f  
		    FROM $att_table, $field_table 
		    WHERE ARCID = POLYLINEID 
		    GROUP BY POLYLINEID
		    ORDER by ARCID";
				
	$dull = "SELECT COUNT(*) FROM $field_table";

	// Other junk queries I tried to get this calc system to work ...
	//$comm1 = "CREATE TABLE $att_table$su1$field_table$su2 $comm$comm_f 
	//	   FROM ( SELECT ARCID, US_POLYID $head_f 
	//			   FROM $att_table, $field_table 
	//      	   WHERE US_POLYID = ARCID ) Prod 
	//	   WHERE $f2 GROUP BY $f3";
	//$comm2 = "$comm$comm_f 
	//	   FROM ( SELECT ARCID, US_POLYID $head_f 
	//		   FROM $att_table, $field_table 
	//       	   WHERE US_POLYID = ARCID ) Prod 
	//	   WHERE $f2 GROUP BY $f3";
	//$comm3 = "SELECT US_POLYID, SUM(LCOVV21) 
	//	   FROM (SELECT LCOVV21, ARCID 
	//		  FROM CC_ATT, CC_US ) Prod 
	//	   WHERE US_POLYID = Prod.ARCID 
	//	   GROUP BY Prod.ARCID";
	//$comm4 = "SELECT pd.US_POLYID, sum(vd.LCOVV21) 
	//	   FROM CC_ATT AS vd 
	//		INNER JOIN 
	//	   CC_US AS pd ON pd.US_POLYID = vd.ARCID 
	//	   WHERE pd.uni != 0 
	//	   GROUP BY pd.US_POLYID";
	//$comm5 = "SELECT * FROM CC_ATT AS vd INNER 
	//		JOIN 
	//	   CC_US AS pd ON pd.US_POLYID = vd.ARCID 
	//	   WHERE pd.uni != 0 
	//	   GROUP BY pd.US_POLYID";
	//$comm6 = "SELECT * FROM CC_US 
	//	   WHERE uni!=0 
	//	   ORDER BY uni";
	//$comm7 = "CREATE PROCEDURE sums( IN s CHAR(20), OUT sumd double ); 
	//	   DECLARE cursor CURSOR FOR 
	//		SELECT * FROM CC_US 
	//		WHERE US_POLYID = S; 
	//	   DECLARE newSum double;
	//	   DECLARE count double; 
	//	   BEGIN 
	//		SET sumd = 0.0; 
	//		SET count = 0.0; 
	//		OPEN cursor; 
	//		sumLoop: LOOP 
	//			FETCH FROM cursor INTO newSum; 
	//			IF Not_Found THEN LEAVE sumLoop 
	//			END IF; 
	//			SET count = count + 1; 
	//			SET sumd = sumd + newSum";
	//$comm8 = "CREATE PROCEDURE summing()
	//	   BEGIN
	//	   	DECLARE done INT DEFAULT 0;
	//	   	DECLARE cur1 CURSOR FOR SELECT ARCID FROM CC_ATT;
	//	   	DECLARE CONTINUE HANDLER FOR NOT FOUND SET done = TRUE;
	//	   	OPEN cur1;
	//	   	read_loop : LOOP
	//			FETCH cur1 INTO ARCID;
	//			IF NOT done THEN
	//				SELECT ARCID, SUM(LCOVV21) FROM CC_US, CC_ATT WHERE POLYLINEID = ARCID;
	//			END IF;
	//	   	END LOOP;
	//	   	CLOSE cur1;
	//	   END;";
	//$comm9 = "SELECT ARCID, LCOVV21 
	//	   FROM CC_ATT, CC_US 
	//	   WHERE ARCID = POLYLINEID 
	//	   ORDER by ARCID";
	
	$commf = $comm0;
	//$commf = $dull;?><p><?
	echo "\n Running query : $commf ";
	//q($comm0);
	q($commf);
	?><script><killTime();</script><p><?
	echo "updating file table ...";
	?></p><?
	$name = $_SESSION['admin'];
	$sql = "INSERT INTO file(tables, type, time, issuer) 
		VALUES('$att_table$su1$field_table$su2', 'calc', NOW(), '$name')";
	echo $sql;
	echo $name;
	echo $_SESSION['admin'];
	$hd2 = mysql_connect("localhost", "cywrker","")
        or die ("Unable to connect");

    	//Select database
    	mysql_select_db ("ccommfiles", $hd2)
         or die ("Unable to select database");
 
    	//Execute sample query 
    	//$res = mysql_query($sql,$hd2)
       //  or die ("Unable to run query : $sql");
	//mysql_close($hd2);
   	//mysql_free_result($res);
	echo "Done!";
	//sleep(10);
	//header("location:select_tables.php");
?>
</p>
</p>
</center>

</div> <!-- end #content -->




	</body>

</html>

