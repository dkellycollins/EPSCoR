#!/usr/local/bin/python
# -*- coding: utf-8 -*-

import MySQLdb as mdb
import sys
import os
import shutil
import time
import datetime
from datetime import datetime

#import re
#import xml.sax.handler

"""def xml2obj(src):
    
    #A simple function to converts XML data into native Python object.
    
    non_id_char = re.compile('[^_0-9a-zA-Z]')
    def _name_mangle(name):
        return non_id_char.sub('_', name)

    class DataNode(object):
        def __init__(self):
            self._attrs = {}    # XML attributes and child elements
            self.data = None    # child text data
        def __len__(self):
            # treat single element as a list of 1
            return 1
        def __getitem__(self, key):
            if isinstance(key, basestring):
                return self._attrs.get(key,None)
            else:
                return [self][key]
        def __contains__(self, name):
            return self._attrs.has_key(name)
        def __nonzero__(self):
            return bool(self._attrs or self.data)
        def __getattr__(self, name):
            if name.startswith('__'):
                # need to do this for Python special methods???
                raise AttributeError(name)
            return self._attrs.get(name,None)
        def _add_xml_attr(self, name, value):
            if name in self._attrs:
                # multiple attribute of the same name are represented by a list
                children = self._attrs[name]
                if not isinstance(children, list):
                    children = [children]
                    self._attrs[name] = children
                children.append(value)
            else:
                self._attrs[name] = value
        def __str__(self):
            return self.data or ''
        def __repr__(self):
            items = sorted(self._attrs.items())
            if self.data:
                items.append(('data', self.data))
            return u'{%s}' % ', '.join([u'%s:%s' % (k,repr(v)) for k,v in items])

    class TreeBuilder(xml.sax.handler.ContentHandler):
        def __init__(self):
            self.stack = []
            self.root = DataNode()
            self.current = self.root
            self.text_parts = []
        def startElement(self, name, attrs):
            self.stack.append((self.current, self.text_parts))
            self.current = DataNode()
            self.text_parts = []
            # xml attributes --> python attributes
            for k, v in attrs.items():
                self.current._add_xml_attr(_name_mangle(k), v)
        def endElement(self, name):
            text = ''.join(self.text_parts).strip()
            if text:
                self.current.data = text
            if self.current._attrs:
                obj = self.current
            else:
                # a text only node is simply represented by the string
                obj = text or ''
            self.current, self.text_parts = self.stack.pop()
            self.current._add_xml_attr(_name_mangle(name), obj)
        def characters(self, content):
            self.text_parts.append(content)

    builder = TreeBuilder()
    if isinstance(src,basestring):
        xml.sax.parseString(src, builder)
    else:
        xml.sax.parse(src, builder)
    return builder.root._attrs.values()[0]"""

con = None

def filetype(filename):
	return filename.split(".")[-1]

def filename_notype(filename):
	return filename.split(".")[0]

def is_csv(filename):
	typ = filetype(filename)
	if typ != "csv":
		print "Not csv"
		return False
	else:
		print "Is csv"
		return True

def is_us(filename):
	no_t = filename_notype(filename)
	if len(no_t) == len(no_t.split("_US")[0]):
		print "Not US"		
		return False
	else:
		print "IS US"
		return True

def files():
	for root, dirs, files in os.walk('/var/www/upload'):
		for name in files:       
			print name
			print filetype(name)
			print filename_notype(name)
			is_csv(name)
			is_us(name)
			print "\n"

files()

def db_add_file(filename, typ):
	try:
		con = mdb.connect('localhost', 'cywrker', '', 'ccommfiles')
		with con: 
			t = datetime.now()
			cur = con.cursor()
			#value = datetime.strptime(t, "%d %m %l %h:%m:%s %Y")
			comm = "INSERT INTO file(tables, type, time, issuer) VALUES('" + filename + "','" + typ + "', NOW(), 'ftp')"
			cur.execute(comm)
			rows = cur.fetchall()
			for row in rows:
				print row
	except mdb.Error, e:
		print "Error %d: %s" % (e.args[0], e.args[1])
		sys.exit(1)
	finally:
		if con:
			con.close()

def update_stat(filename):
	try:
		con = mdb.connect('localhost', 'cywrker', '', 'ccommfiles')
		with con: 
			t = datetime.now()
			#value = datetime.strptime(t, "%d %m %l %h:%m:%s %Y")
			cur = con.cursor()
			cur.execute("UPDATE file SET status = 'FINISHED' AND end_time = NOW() WHERE filename = %s", filename)
			rows = cur.fetchall()
			print "Rows updated : %d" % cur.rowcount
	except mdb.Error, e:
		print "Error %d: %s" % (e.args[0], e.args[1])
		sys.exit(1)
	finally:
		if con:
			con.close()

def create_att_table(filename):
	file = open("/var/www/upload/" + filename + ".csv",'r')#specify file to open
	file.seek(0)
	head = file.readline()
	file.close()
	_head = head.replace('\"', '')
	s_head = _head.split(',')
	call = ""
	for field in s_head:
		print "field : " + field + "\n"
		if field.rfind("weg") != -1 or field.rfind("cokey") != -1 or field.rfind("kffact") != -1:
			call = call + field + " varchar(25), "
		else:	
			call = call + field + " double, "
	#call = call[:-2]
	call = call + "PRIMARY KEY(ARCID)"
	print call + "\n"
	try:
		con = mdb.connect('localhost', 'cywrker', '', 'cybercomm')
		with con: 
			cur = con.cursor()
			comm = "CREATE TABLE IF NOT EXISTS " + filename + " ( " + call + " ) ENGINE = InnoDB DEFAULT CHARSET=latin1"
			print "COMM : " + comm
			cur.execute("CREATE TABLE IF NOT EXISTS " + filename + " ( " + call + " ) ENGINE = InnoDB DEFAULT CHARSET=latin1")
			rows = cur.fetchall()
			for row in rows:
				print row
	except mdb.Error, e:
		print "Error %d: %s" % (e.args[0], e.args[1])
		sys.exit(1)
	finally:
		if con:
			con.close()

def create_us_table(filename):
	file = open("/var/www/upload/" + filename + ".csv",'r')#specify file to open
	file.seek(0)
	head = file.readline()
	file.close()
	_head = head.replace('\"', '')
	s_head = _head.split(',')
	call = ""
	tpe = False
	for field in s_head:
		print "field : " + field + "\n"	
		call = call + field + " double, "
	#call = call[:-2]
	call = call + "PRIMARY KEY(OBJECTID)"	
	print call + "\n"
	try:
		con = mdb.connect('localhost', 'cywrker', '', 'cybercomm')
		with con: 
			cur = con.cursor()
			field = "CREATE TABLE IF NOT EXISTS " + filename + " ( " + call + " ) ENGINE = InnoDB DEFAULT CHARSET=latin1"
			print "FIELD : " + field			
			cur.execute("CREATE TABLE IF NOT EXISTS " + filename + " ( " + call + " ) ENGINE = InnoDB DEFAULT CHARSET=latin1")
			rows = cur.fetchall()
			for row in rows:
				print row
	except mdb.Error, e:
		print "Error %d: %s" % (e.args[0], e.args[1])
		sys.exit(1)
	finally:
		if con:
			con.close()
	
def conv_file(filename):
	con = None	
	try:
		con = mdb.connect('localhost', 'cywrker', '', 'cybercomm')
		with con: 
			cur = con.cursor()
			#comm = "LOAD DATA LOCAL INFILE '/var/www/conversions/" + filename + ".txt' INTO TABLE " + filename + " FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '\"' LINES TERMINATED BY '\r\n' IGNORE 1 LINES"
			#comm = "LOAD XML LOCAL INFILE '/var/www/conversions/" + filename + ".xml' INTO TABLE " + filename + " ROWS IDENTIFIED BY '<" + filename + ">'"			
			cur.execute("LOAD DATA LOCAL INFILE '/var/www/conversions/" + filename + ".csv' INTO TABLE " + filename + " FIELDS TERMINATED BY ',' OPTIONALLY ENCLOSED BY '\"' LINES TERMINATED BY '\n' IGNORE 1 LINES")
			rows = cur.fetchall()
			for row in rows:
				print row
	except mdb.Error, e:
		print "Error %d: %s" % (e.args[0], e.args[1])
		sys.exit(1)
	finally:
		if con:
			con.close()

def del_file(filename):
	os.remove(filename)

def mov_file_to_conv(filename):
	src = "/var/www/upload/" + filename + ".csv"
	dest = "/var/www/conversions/" + filename + ".csv"
	os.system ("mv"+ " " + src + " " + dest)

def arch_file(filename):
	src = "/var/www/conversions/" + filename + ".csv"
	dest = "/mnt/fileshare/catchnet/arch/" + filename + ".csv"
	os.system ("mv"+ " " + src + " " + dest)

def exists(f_name):
	res = False
	for root, dirs, files in os.walk('/var/www/upload'):
		for name in files:       
			if filename_notype(name) == filename_notype(f_name):
				res = True
	return res

def add_file(name):		
	typ = filetype(name)
	n_notyp = filename_notype(name)
	csv = is_csv(name)
	us = is_us(name)
	if(csv):
		if(us):
			print "US table added\n"
			db_add_file(n_notyp, "us")
			create_us_table(n_notyp)	
		else:
			print "ATT table added\n"
			db_add_file(n_notyp, "att")
			create_att_table(n_notyp)
	else:
		print "Not a CSV file"

def file_check():
	try:
		con = mdb.connect('localhost', 'cywrker', '', 'ccommfiles')
		with con: 
			cur = con.cursor()
			cur.execute("SELECT * FROM file")
			rows = cur.fetchall()
			if len(rows) == 0:
				for root, dirs, files in os.walk('/var/www/upload'):
					for name in files:
						add_file(name)
						print "File Not yet in database"
						n_notyp = filename_notype(name)
						mov_file_to_conv(n_notyp)
						conv_file(n_notyp)
						print "File " + n_notyp + " added"
			else:
				for row in rows:
					print row[0]
					fn = row[0]
					print "Res : " + fn + "\n"
					if exists(fn) == False:
						add_file(fn)
						n_notyp = filename_notype(fn)
						print "File Not yet in database"
						mov_file_to_conv(n_notyp)
						conv_file(fn)
						print "File " + fn + " added"						
					else:
						print "File already exists"	

	except mdb.Error, e:
		print "Error %d: %s" % (e.args[0], e.args[1])
		sys.exit(1)
	finally:
		if con:
			con.close()

file_check()
