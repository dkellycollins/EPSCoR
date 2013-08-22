<?php
	session_start(); //Start the session
	define(ADMIN,$_SESSION['name']); //Get the user name from the previously registered super global variable
	if(!session_is_registered("admin")){ //If session not registered
		header("location:login.php"); // Redirect to login.php page
	}
	else //Continue to current page
		header( 'Content-Type: text/html; charset=utf-8' );
	;

	//$tableView = "";
    $hd = mysql_connect("localhost", "cywrker","")
          or die ("Unable to connect");
    // Select database
    mysql_select_db ("cybercomm", $hd)
          or die ("Unable to select database");

    $res = mysql_query("SHOW TABLES",$hd)
          or die ("Unable to run query");

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
              echo "<center><select name=\"$field\" style=\"width:200px\">\n";
	      //echo "<option value=\"NULL\">Select Table</option>\n";

	      while($row=mysql_fetch_assoc($result)) {
		foreach($row as $key=>$value) { ?><option value="<?=$value?>"><?=$value?></option><? }
	      }
              echo "</select></center>";
	  }
	  ?></table><?
	  }

	  echo "<h3>Please Select the Following</h3>";
	  echo "<form name=\"calcs\" method=\"post\" action=\"calcs.php\"><p>";
	  echo_result($res, "Attribute");
	  echo "</p>";

	  $res = mysql_query("SHOW TABLES",$hd)
          or die ("Unable to run query");

	  echo "<left><p>";
	  echo_result($res, "Upstream");
	  echo "<br>Calculation : </left><center>";
	  echo "<select name='calc' style=\"width:200px\">\n";
	  echo "<option value=''></option>\n";
	  echo "<option value='SUM'>Sum</option>\n";
	  echo "<option value='AVG'>Avg</option>\n";
	  echo "</select></center><left>";
	  echo "</p><br>\n</br>";
	  echo "<br><input type='submit' value='Submit'></form></left>";

    // Close connection
    mysql_close($hd);
	
?>
