PdfTools
==============

PdfTools is an utility to merge and split pdf files and images

## Usage:
  pdft.exe command files [options]

Available commands:

## merge
  m, merge: merges one or more jpg, png or pdf files into a single pdf file

Parameters:
- files: a list of jpg, png, pdf files

Options:
-  --outFile, --of:  output file name (the default is merged_yyyyMMdd_HHmmss.pdf)

Examples:
- pdft.exe m mydoc.pdf logo1.png logo2.jpg --outFile=merged.pdf

## split
  s, split: splits one or more pdf files into n jpg images, one per page
  (note: this command requires ghostscript: https://ghostscript.com/releases/gsdnld.html)

Parameters:
-  files: a list of pdf files

Options:
-  --outFile, --of:  output file name (the default is splitted_yyyyMMdd_HHmmss_n.jpg)

Examples:
-  pdft.exe s mydoc.pdf --outFile=hello
-  pdft.exe s mydoc.pdf --outFile=hello.jpg

both commands create: hello_0001.jpg, hello_0002.jpg, ...

# Todo
- ~~split command~~
- handle ghostscript not installed error
- landscape option
- page size and image quality options (?)

# Dependencies

## Libraries
- Magick.NET.Core
- PdfSharpCore

## Software
- [Ghostscript](https://ghostscript.com/releases/gsdnld.html) needs to be installed on your machine:  (split command only)

## License
GNU GENERAL PUBLIC LICENSE V 3

---

Copyright (C) [Massimo Barbieri](http://www.massimobarbieri.it) 
