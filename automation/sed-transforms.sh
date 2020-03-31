#!/bin/bash

# Bash shell script to execute the sed program to create modified files.
# Chris Joakim, Microsoft, 2020/03/31

sed_commands_file='sed-transforms.txt'
infile='original.json'
outfile='modified.json'

sed -f $sed_commands_file $infile > $outfile

echo 'diffs:'
diff $infile $outfile

echo 'done'
