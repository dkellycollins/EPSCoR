import MySQLdb as mdb
import sys
import os
import shutil

def file_mod(afile):
	shutil.move( afile, afile+"~" )
	print afile
	destination = open( afile, "w" )
	source= open( afile+"~", "r" )
	count = 0
	for line in source:
    		if count == 0:
    			destination.write( line[:-2] + ",\"uni\"\n")
			count = count + 1
    		else :
			destination.write( line[:-2] + "," + `count` + "\n")
			count = count + 1
		print count

    		#if <some condition>:
    		#    destination.write( >some additional line> + "\n" )
	source.close()
	destination.close()

for root, dirs, files in os.walk('/var/www/upload'):
	for name in files:
		file_mod('/var/www/upload/' + name)