session_start(); //Start the session
	define(ADMIN,$_SESSION['name']); //Get the user name from the previously registered super global variable
	if(!session_is_registered("admin")){ //If session not registered
		header("location:login.php"); // Redirect to login.php page
	}
	else //Continue to current page
		header( 'Content-Type: text/html; charset=utf-8' );
	
      $db_host = 'localhost';
	$db_user = 'cywrker';
	$db_pwd = '';
	$datbase = 'cybercomm';

	if (isset($_GET["page"])) { $page  = $_GET["page"]; } else { $page=1; }; 
	$start_from = ($page-1) * 20; 
	$sql = "SELECT * FROM $tableView LIMIT $start_from, 20"; 

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

	$sql = "SELECT COUNT FROM "; 
	$rs_result = mysql_query($sql,$connection); 
	$row = mysql_fetch_row($rs_result); 
	$total_records = $row[0]; 
	$total_pages = ceil($total_records / 20); 
 
	for ($i=1; $i<=$total_pages; $i++) { 
            echo "<a href='pagination.php?page=".$i."'>".$i."</a> "; 
	}; 
