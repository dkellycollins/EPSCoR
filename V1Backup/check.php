<style type="text/css">
body
{
font-family:arial,helvetica,tahoma,verdana,sans-serif;
font-size:11px;
line-height:14px;
}
</style> 

<!--
mysql> describe members;
+----------+--------------+------+-----+---------+----------------+
| Field    | Type         | Null | Key | Default | Extra          |
+----------+--------------+------+-----+---------+----------------+
| id       | int(4)       | NO   | PRI | NULL    | auto_increment |
| username | varchar(100) | NO   |     |         |                |
| password | varchar(65)  | NO   |     |         |                |
+----------+--------------+------+-----+---------+----------------+
-->

<?php
//INSERT INTO `members` VALUES(1, 'admin', 'd033e22ae348aeb5660fc2140aec35850c4da997');

//blackwolf, l105
?>

<!-- remove user
DELETE FROM members WHERE username='admin';
-->


<?php
define(DOC_ROOT,dirname(__FILE__)); // To properly get the config.php file
$username = $_POST['username']; //Set UserName
$password = $_POST['password']; //Set Password
$msg ='';

function sanitize($data){
	//remove spaces from the input
	$data=trim($data);
	//convert special characters to html entities
	//most hacking inputs in XSS are HTML in nature, so converting them to special characters so that they are not harmful
	$data=htmlspecialchars($data);
	//sanitize before using any MySQL database queries
	//this will escape quotes in the input.
	$data = mysql_real_escape_string($data);
	return $data;
}

if(isset($username, $password)) {
    ob_start();
    include(DOC_ROOT.'/config.php'); //Initiate the MySQL connection
    // To protect MySQL injection (more detail about MySQL injection)
    $myusername = stripslashes($username);
    $mypassword = stripslashes($password);
    $myusername = trim($myusername);
    $mypassword = trim($mypassword);
    $myusername = mysqli_real_escape_string($dbC, $myusername);
    $mypassword = mysqli_real_escape_string($dbC, $mypassword);
    
    $sql="SELECT * FROM member WHERE username='$myusername' and password=SHA('$mypassword')";
    $result=mysqli_query($dbC, $sql);
    // Mysql_num_row is counting table row
    $count=mysqli_num_rows($result);
    // If result matched $myusername and $mypassword, table row must be 1 row
    if($count==1){
        // Register $myusername, $mypassword and redirect to file "admin.php"
	 $_SESSION["admin"] = $myusername;
	 //echo $myusername;
	 echo $_SESSION['admin'];
	 session_register("admin");
        session_register("password");
        header("location:index.php");
    }
    else {
        $msg = "Wrong Username or Password";
        header("location:login.php");
    }
    ob_end_flush();
}
else {
    header("location:login.php");
}
?>

