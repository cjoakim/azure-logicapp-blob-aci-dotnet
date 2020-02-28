#!/bin/bash

# Execute 'docker-compose xxx'; the value of xxx is provided by the first
# command-line arg, or $1.
# Usage:
# $ ./compose.sh up
# $ ./compose.sh ps
# $ ./compose.sh down
#
# Chris Joakim, Microsoft, 2020/02/28

docker-compose $1 $2 $3 
