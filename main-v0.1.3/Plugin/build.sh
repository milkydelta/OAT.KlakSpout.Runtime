#!/bin/bash
make -f Makefile.64 clean
make -f Makefile.32 clean

make -f Makefile.64
make -f Makefile.64 copy
make -f Makefile.64 clean


make -f Makefile.32
make -f Makefile.32 copy
make -f Makefile.32 clean
