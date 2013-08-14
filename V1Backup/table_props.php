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
<?php
	$sView = $_GET['sTable'];
?>
<head>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <meta name="author" content="" />
    <link rel="stylesheet" type="text/css" href="style.css" media="screen" />

    <title>table_viewer</title>
   
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8">
    <title>Highcharts Example</title>

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js"></script>
    <script type="text/javascript">
	var chart;
	$(document).ready(function() {
	chart = new Highcharts.Chart({
		chart: {
			renderTo: 'container',
			defaultSeriesType: 'bar'
		},
		title: {
			text: 'Table <? echo $sView;?> Averages'
		},
		subtitle: {
			text: 'Source: Cybercommons'
		},
		yAxis: {
			categories: [
    <?php
		$db_host = 'localhost';
		$db_user = 'cywrker';
		$db_pwd = '';

		$database = 'cybercomm';
		$table = $sView;

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
		echo "' ', ";
		for($i=0; $i<$fields_num; $i++)
		{
	    		$field = mysql_fetch_field($result);
			echo " '{$field->name}'";
			if($i != $fields_num - 1) echo ", ";
			
		}
		mysql_free_result($result);

    ?> ],
			title: {
				text: null
			}
		},
		xAxis: {
			<? //min: 0, ?>
			title: {
				text: ' ',
				align: 'high'
			}
		},
		tooltip: {
			formatter: function() {
				return ''+
					this.series.name +': '+ this.y +'';
			}
		},
		plotOptions: {
			bar: {
				dataLabels: {
					enabled: false
				}
			}
		},
		legend: {
			layout: 'horizontal',
			align: 'left',
			verticalAlign: 'bottom',
			x: 10,
			y: 0,
			floating: true,
			borderWidth: 1,
			backgroundColor: '#FFFFFF',
			shadow: true
		},
		credits: {
			enabled: false
		},
		series: [

	<?php
		// build table to be fed into chart creator
		if (!mysql_connect($db_host, $db_user, $db_pwd))
	    		die("Can't connect to database");

		if (!mysql_select_db($database))
	    		die("Can't select database");

		// sending query
		$result2 = mysql_query("SELECT * FROM {$table}");
		if (!$result2) {
	    		die("Query to show fields from table failed");
		}

		$fields_num1 = mysql_num_fields($result2);
		// printing table headers
		for($i=0; $i<$fields_num1; $i++)
		{
			echo "{";
			$field1 = mysql_fetch_field($result2);
			$sql1 = "SELECT AVG({$field1->name}) FROM {$table}";
			$reresult = mysql_query($sql1);
			if (!$result) {
	    			die("Query to show fields from table failed");
			}
			
	    			echo "name: '{$field1->name}', ";
				echo "data: [";
				$j = 0;
				while($row = mysql_fetch_row($reresult))
				{
	    				// $row is array... foreach( .. ) puts every element
	    				// of $row to $cell variable
	    				foreach($row as $cell){
						echo $cell;
						$j++;
					}
					//<form name=$row[0] 
				}
				echo "]}";
				if($i != $fields_num1 - 1) echo ", ";
			
			mysql_free_result($reresult);
		}
		mysql_free_result($result2);
		
	?>
		
		]
	});
});

		</script>
	</head>
	

<div id="container" style="width: 370px; height: 800px; margin: auto"></div>
<script type="text/javascript" src="../../js/highcharts.js"></script>
<script type="text/javascript" src="../../js/modules/exporting.js"></script>
	<body>
    <style type="text/css">
	body
	{
	font-family:arial,helvetica,tahoma,verdana,sans-serif;
	font-size:11px;
	line-height:14px;
	}
     </style> 

	<?php
       function echo_result($result, $field) {
	 ?><table><tr><?
	  if(! $result) { ?><th>result not valid</th><? }
	  else {
	    $i = 0;
	    ?></tr><?
	   
	    if(mysql_num_rows($result) == 0) {
	      ?><tr><td colspan="<?=mysql_num_fields($result)?>">
	      <strong><center>no result</center></strong>
	      </td></tr><?
	    } else
	      if(i==0) $tableView = $field;
	      echo $field, " Table : " ;
              echo "<select name=\"$field\" ";
		echo "OnChange=\"location.href=loadL.$field.options[selectedIndex].value\">\n";
	      echo "<option value=\"NULL\">Select Table</option>\n";

	      while($row=mysql_fetch_assoc($result)) {
		foreach($row as $key=>$value) { ?><option value="table_props.php?sTable=<?=$value?>"><?=$value?></option><? }
	      }
              echo "</select>";
	  }
	  ?></table><?
	  }


    echo "<form name='loadL' action='?'><p>";
    echo "</p>";

    $hd = mysql_connect("localhost", "cywrker","")
          or die ("Unable to connect");
    // Select database
    mysql_select_db ("cybercomm", $hd)
          or die ("Unable to select database");

    $res = mysql_query("SHOW TABLES",$hd)
          or die ("Unable to run query");

    echo_result($res, "View");  
	// Close connection
    mysql_close($hd);
    ?>



    </body>

</html>
