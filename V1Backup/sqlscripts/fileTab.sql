
use ccommfiles;
DROP TABLE file;
CREATE TABLE IF NOT EXISTS `file` (
  `tables` varchar(100) NOT NULL default '',
  `type` varchar(10) NOT NULL default '',
  `time` DATETIME NOT NULL,
  `issuer` varchar(65) NOT NULL default '',
  PRIMARY KEY(`tables`)
);
