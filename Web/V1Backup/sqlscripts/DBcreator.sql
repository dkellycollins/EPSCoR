CREATE TABLE IF NOT EXISTS `CC` (`ID_1` float,
`OBJECTID_1` float,
`TARGET_FID` float PRIMARY KEY,
`Fnode` float,
`Tnode` float,
`LCOVV11` float,
`LCOVV21` float,
`LCOVV22` float,
`LCOVV23` float,
`LCOVV24` float,
`LCOVV31` float,
`LCOVV41` float,
`LCOVV42` float,
`LCOVV43` float,
`LCOVV52` float,
`LCOVV71` float,
`LCOVV81` float,
`LCOVV82` float,
`LCOVV90` float,
`LCOVV95` float,
`LCOVV127` float,
`LCOVVSUM` float,
`slopegradd` float,
`slopegradw` float,
`wtdepannmi` float,
`tfact` float,
`weg` float,
`om_l` float,
`om_r` float,
`om_h` float,
`dbthirdbar` float,
`ksat_l` float,
`ksat_r` float,
`ksat_h` float,
`awc_l` float,
`awc_r` float,
`awc_h` float,
`kffact` float,
`brockdepmi` float,
`Area` float,
`Shape_Length` float
) ENGINE = InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `CC_US` (`ID` float,
`OBJECTID` float,
`PolylineID` float PRIMARY KEY,
`US_polyID` float
) ENGINE = InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `RR` ( `OBJECTID_1` float,
`TARGET_FID` float PRIMARY KEY,
`Fnode` float,
`Tnode` float,
`LCOVV11` float,
`LCOVV21` float,
`LCOVV22` float,
`LCOVV23` float,
`LCOVV24` float,
`LCOVV31` float,
`LCOVV41` float,
`LCOVV42` float,
`LCOVV43` float,
`LCOVV52` float,
`LCOVV71` float,
`LCOVV81` float,
`LCOVV82` float,
`LCOVV90` float,
`LCOVV95` float,
`LCOVVSUM` float,
`slopegradd` float,
`slopegradw` float,
`wtdepannmi` float,
`tfact` float,
`weg` float,
`om_l` float,
`om_r` float,
`om_h` float,
`dbthirdbar` float,
`ksat_l` float,
`ksat_r` float,
`ksat_h` float,
`awc_l` float,
`awc_r` float,
`awc_h` float,
`kffact` float,
`brockdepmi` float,
`Area` float,
`Shape_Length` float
) ENGINE = InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `RR_OLD` ( `ID` float,
`OBJECTID_1` float,
`TARGET_FID` float PRIMARY KEY,
`Fnode` float,
`Tnode` float,
`LCOVV11` float,
`LCOVV21` float,
`LCOVV22` float,
`LCOVV23` float,
`LCOVV24` float,
`LCOVV31` float,
`LCOVV41` float,
`LCOVV42` float,
`LCOVV43` float,
`LCOVV52` float,
`LCOVV71` float,
`LCOVV81` float,
`LCOVV82` float,
`LCOVV90` float,
`LCOVV95` float,
`LCOVVSUM` float,
`slopegradd` float,
`slopegradw` float,
`wtdepannmi` float,
`tfact` float,
`weg` float,
`om_l` float,
`om_r` float,
`om_h` float,
`dbthirdbar` float,
`ksat_l` float,
`ksat_r` float,
`ksat_h` float,
`awc_l` float,
`awc_r` float,
`awc_h` float,
`kffact` float,
`brockdepmi` float,
`Area` float,
`Shape_Length` float
) ENGINE = InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `RR_US` (`ID` float,
`OBJECTID` float,
`PolylineID` float PRIMARY KEY,
`US_polyID` float
) ENGINE = InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `UA` (`OBJECTID_12` float,
`TARGET_FID` float PRIMARY KEY,
`Fnode` float,
`Tnode` float,
`LCOVV11` float,
`LCOVV21` float,
`LCOVV22` float,
`LCOVV23` float,
`LCOVV24` float,
`LCOVV31` float,
`LCOVV41` float,
`LCOVV42` float,
`LCOVV43` float,
`LCOVV52` float,
`LCOVV71` float,
`LCOVV81` float,
`LCOVV82` float,
`LCOVV90` float,
`LCOVV95` float,
`LCOVVSUM` float,
`slopegradd` float,
`slopegradw` float,
`wtdepannmi` float,
`tfact` float,
`weg` float,
`om_l` float,
`om_r` float,
`om_h` float,
`dbthirdbar` float,
`ksat_l` float,
`ksat_r` float,
`ksat_h` float,
`awc_l` float,
`awc_r` float,
`awc_h` float,
`kffact` float,
`brockdepmi` float,
`Area` float,
`Shape_Length` float
) ENGINE = InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `UA_OLD` (`ID` float,
`OBJECTID_12` float,
`TARGET_FID` float PRIMARY KEY,
`Fnode` float,
`Tnode` float,
`LCOVV11` float,
`LCOVV21` float,
`LCOVV22` float,
`LCOVV23` float,
`LCOVV24` float,
`LCOVV31` float,
`LCOVV41` float,
`LCOVV42` float,
`LCOVV43` float,
`LCOVV52` float,
`LCOVV71` float,
`LCOVV81` float,
`LCOVV82` float,
`LCOVV90` float,
`LCOVV95` float,
`LCOVVSUM` float,
`slopegradd` float,
`slopegradw` float,
`wtdepannmi` float,
`tfact` float,
`weg` float,
`om_l` float,
`om_r` float,
`om_h` float,
`dbthirdbar` float,
`ksat_l` float,
`ksat_r` float,
`ksat_h` float,
`awc_l` float,
`awc_r` float,
`awc_h` float,
`kffact` float,
`brockdepmi` float,
`Area` float,
`Shape_Length` float
) ENGINE = InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE IF NOT EXISTS `UA_US` (`ID` float,
`OBJECTID` float,
`PolylineID` float PRIMARY KEY,
`US_polyID` float
) ENGINE = InnoDB DEFAULT CHARSET=latin1;
