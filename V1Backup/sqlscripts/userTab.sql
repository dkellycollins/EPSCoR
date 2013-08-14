CREATE DATABASE members;

use members;

CREATE TABLE IF NOT EXISTS `member` (
  `id` int(4) NOT NULL auto_increment,
  `username` varchar(100) NOT NULL default '',
  `password` varchar(65) NOT NULL default '',
  PRIMARY KEY  (`id`)
);
