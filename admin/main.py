# Python program Locic App and Azure Container Instance administration.
# Chris Joakim, Microsoft, 2020/02/26

import csv
import json
import sys
import time
import os


def gen_aci_delete_script():
    print('gen_aci_delete_script')

def read_text_file(infile):
    lines = list()
    with open(infile, 'rt') as f:
        for idx, line in enumerate(f):
            lines.append(line.strip())
    return lines

def write_lines(outfile, lines):
    with open(outfile, "w", newline="\n") as out:
        for line in lines:
            out.write(line + "\n")
        print('file written: {}'.format(outfile))

def load_json_file(infile):
    with open(infile) as json_file:
        return json.load(json_file)

def write_obj_as_json_file(outfile, obj):
    txt = json.dumps(obj, sort_keys=True, indent=2)
    with open(outfile, 'wt') as f:
        f.write(txt)
    print("file written: " + outfile)


if __name__ == "__main__":
    print(sys.argv)

    if len(sys.argv) > 1:
        func  = sys.argv[1].lower()
        print("func: {}".format(func))

        if func == 'gen_aci_delete_script':
            gen_aci_delete_script()
        elif func == 'xxx':
            pass
        else:
            print('invalid command-line function: {}'.format(func))
    else:
        print('no command-line function given')
