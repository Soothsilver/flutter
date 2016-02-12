<?php
	@mysql_connect("sql2.webzdarma.cz", "fluttershy.w8014", "Kindness*8");
	@mysql_select_db("fluttershy.w8014");
	$command = mysql_real_escape_string($_GET["command"]);
	$ip = $_SERVER["REMOTE_ADDR"];
	$datetime = date("Y-m-d H:i:s");
	@mysql_query("INSERT INTO feedback (IP, Command, Datetime) VALUES ('" . $ip . "', '". $command . "', '" . $datetime . "')");
	echo "OK";
?>