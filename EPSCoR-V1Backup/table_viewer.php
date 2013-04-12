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
<style type="text/css">
	body
	{
	font-family:arial,helvetica,tahoma,verdana,sans-serif;
	font-size:11px;
	line-height:14px;
	}
     </style> 
<style>
			td {
				
				height: 1em;
				padding: 1em;
				text-align: left;	
			}

			tr:hover {
				cursor: pointer;
				background-color: yellow;
			}
			.select {
				background-color: #0ea026;	
			}
</style>

	<script type=/"text//javascript/"><!-- 
<?php
		// build table formatting
		$tableView = $_GET['vTable'];	
		if(!$tableView){
			//echo "no table loaded";
		}
		else{
		$hd1 = mysql_connect("localhost", "cywrker","")
          		or die ("Unable to connect");
    		// Select database
    		mysql_select_db ("cybercomm", $hd1)
          		or die ("Unable to select database");
		$sql = "SELECT * FROM $tableView";
    		$res = mysql_query($sql,$hd1)
          		or die ("Unable to run query");
		
		//$num_rows = mysql_num_rows($res);
		$num_rows = 20;
		for($i=1; $i<=$num_rows; $i++){
			echo "var td$i = null;\n";
		}

		?>function highlight(obj){ if ( td1 <?
		for($i=2; $i<=$num_rows; $i++){
			echo " || td$i\n";
		}
		?>){<?

		for($i=1; $i<=$num_rows; $i++){
			echo "td$i.className = null;\n" .PHP_EOL;
		}

		?>}<?

		for($i=0; $i<$num_rows; $i++){
			$nu = $i + 1;
			//echo "obj.cells[$i].className = \"select\";\n";
		}


		for($i=0; $i<$num_rows; $i++){
			$nu = $i + 1;
			echo "td$nu = obj.cells[$i];\n";
		}
		}
		?>}//--> </script><?

		// Close connection
    		mysql_close($hd1);

?>
    <meta http-equiv="content-type" content="text/html; charset=utf-8" />
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <meta name="author" content="" />
    <link rel="stylesheet" type="text/css" href="style.css" media="screen" />

    <title>table_viewer</title>
   
</head>
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
		echo "OnChange=\"location.href=loadT.$field.options[selectedIndex].value\">\n";
	      echo "<option value=\"NULL\">Select Table</option>\n";

	      while($row=mysql_fetch_assoc($result)) {
		foreach($row as $key=>$value) { ?><option value="table_viewer.php?vTable=<?=$value?>"><?=$value?></option><? }
	      }
              echo "</select>";
	  }
	  ?></table><?
	  }


    echo "<form name='loadT' action='?'><p>";
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
</form>
<?php
	// split table up into sections / rows of 20. Add next/previous/goto page at bottom of frame
	$pagenum = $_GET['pagenum'];	
	if(!$pagenum || $pagenum < 1){
		$pagenum = 1;
	}
	if(!$_POST['page']){ }
		else{
			$pg = $_POST['page'];
			stripslashes($pg);
		       $pg = stripslashes($pg);
    	 		$pagenum = trim($pg);
    			//$pg = mysqli_real_escape_string($dbC, $pg);
		}
	if ($tableView){
		echo "<h3><center>Table : $tableView</center></h3>";
		$connect = mysql_connect("localhost", "cywrker","")
          	   or die ("Unable to connect");

		$db_host = 'localhost';
		$db_user = 'cywrker';
		$db_pwd = '';
		$datbase = 'cybercomm';
	
		$conn = mysql_connect("localhost", "cywrker","")
     	     or die ("Unable to connect");
		// Select database
    		mysql_select_db("cybercomm", $conn)
       	   or die ("Unable to select database");


		$sq = "SELECT * FROM $tableView";
		//echo "sq $sq";
		$data = mysql_query($sq, $conn) or die(mysql_error()); 

		$rows = mysql_num_rows($data); 
 		//This is the number of results displayed per page 
 		$page_rows = 10; 
	
		 //This tells us the page number of our last page 
 		$last = ceil($rows/$page_rows); 

 		//this makes sure the page number isn't below one, or more than our maximum pages 
		if ($pagenum < 1) 
		{ 
 			$pagenum = 1; 
		} 
 		elseif ($pagenum > $last) 
 		{ 
 			$pagenum = $last; 
 		} 
		//This sets the range to display in our query 
 		$max = 'LIMIT ' .($pagenum - 1) * $page_rows .',' .$page_rows; 

		mysql_close($connection);

		$sql = "SELECT * FROM $tableView $max"; 
		//echo "sql $sql";
		$connection = mysql_connect("localhost", "cywrker","")
          		or die ("Unable to connect");

    		// Select database
    		mysql_select_db ("cybercomm", $connection)
      		    or die ("Unable to select database");

		$rs_result = mysql_query ($sql,$connection); 

		?><table><tr><?
		  if(! $rs_result) { ?><th>result not valid</th><? }
		  else {
		    $i = 0;
		    while ($i < mysql_num_fields($rs_result)) {
		      $meta = mysql_fetch_field($rs_result, $i);
	 	     ?><th onclick="highlight(this);" ><?=$meta->name?></th><?
	 	     $i++;
	 	   }
	 	   ?></tr><?
	   
	 	   if(mysql_num_rows($rs_result) == 0) {
	 	     ?><tr onclick="highlight(this);"><td colspan="<?=mysql_num_fields($rs_result)?>">
	 	     <strong><center>no result</center></strong>
	 	     </td></tr><?
	 	   } else
	 	     while($row=mysql_fetch_assoc($rs_result)) {
			?><tr onclick="highlight(this);"><?
			foreach($row as $key=>$value) { ?><td><?=$value?></td><? }
			?></tr><?
	  	    }
	 	 }
	 	 ?></table><?
		mysql_close($connection);
	
		echo " --Page $pagenum of $last-- <p>";
		echo "<table><td>";
		 if ($pagenum == 1) { } 
 		 else {
 			echo " <center><a href='{$_SERVER['PHP_SELF']}?pagenum=1&vTable=$tableView'> <<-First</a> ";
 			echo " ";
			$previous = $pagenum-1;
			echo " <a href='{$_SERVER['PHP_SELF']}?pagenum=$previous&vTable=$tableView'> <-Previous</a></center> ";
		 } 
 		//just a spacer
 		echo "<td></td>";
 		//This does the same as above, only checking if we are on the last page, and then generating the Next and Last links
 		if ($pagenum == $last) 
 		{ } 
 		else {
			echo "</td><td><left>";
 			$next = $pagenum+1;
			echo " <a href='{$_SERVER['PHP_SELF']}?pagenum=$next&vTable=$tableView'>Next -></a> ";
			echo " ";
			echo " <a href='{$_SERVER['PHP_SELF']}?pagenum=$last&vTable=$tableView'>Last ->></a></left> ";
 		} 
		echo "</td><td>";
		echo "<left><form method=\"post\" action=\"{$_SERVER['PHP_SELF']}?vTable=$tableView&\" >
			pg:<input type=\"text\" name=\"page\" size=\"8\" maxlength=\"12\" />
			<input type=\"submit\" value=\">\" />
			</form></left>";
		echo "</td></table>";
		
	}
       //********************************************************************
	// SCRIPT LINKS
	// http://www.designplace.org/scripts.php?page=1&c_id=25
	// http://www.php-mysql-tutorial.com/wikis/php-tutorial/paging-using-php.aspx
	// http://php.about.com/od/phpwithmysql/ss/php_pagination_4.htm
	//********************************************************************


?>


    </body>

</html>
