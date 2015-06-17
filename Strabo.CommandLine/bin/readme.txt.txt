Installation:
1. Unzip the Strabo release file to a folder where you have execute/write permissions.
2. If you don't have Visual C++ Redistributable for Visual Studio 2010 (x86) and 2012, download and install them from:
https://www.microsoft.com/en-us/download/details.aspx?id=30679
https://www.microsoft.com/en-us/download/details.aspx?id=5555

Running Strabo:

This is the command line version of Strabo. Here's examples that invokes Strabo for extracting labels from Tianditu English and Chinese text layer:
.\Strabo.Core.exe 12957312 4852401 C:\Users\yaoyichi\Documents\github\strabo-command-line\Strabo.CommandLine\data\intermediate C:\Users\yaoyichi\Documents\github\strabo-command-line\Strabo.CommandLine\data\output Tianditu_eva 8
.\Strabo.Core.exe 13006520 4853838 C:\Users\yaoyichi\Documents\github\strabo-command-line\Strabo.CommandLine\data\intermediate C:\Users\yaoyichi\Documents\github\strabo-command-line\Strabo.CommandLine\data\output Tianditu_cva 8

More Details About Running Strabo:
1. Open acommand line window

2. Go to the Strabo folder (the unzipped folder that contains Strabo.Core.exe). For example:
        cd C:\Users\yaoyichi\Documents\github\strabo-command-line\Strabo.CommandLine\bin\
 
3. Strabo needs 6 parameters as input arguments:strabo.core.exe west_x north_y intermediate_folder output_folder layer thread_number
3.1 The first two parameters (separate by space) are top left point of the target bounding box ("west_x" and "north_y").
3.2 The third and fourth parameters are two paths. "intermediate_folder" is the intermediate folder for storing log temporary files. "output_folder" is the output folder. 
3.3 The fifth parameter is the name of the map service. 
3.4 The last parameter is the number of threads allowed for Strabo to use. If your machine has 8 cores, you can set the thread number to 4. If your machine has 4 cores, you can set the thread number to 2.

4. The input map is stored as SourceMapImage.png in your "intermediate_folder". The output is stored as GeoJSON files in your "output_folder". The GeoJSON file ending "ByPixels" is the results in image coordinates. The GeoJSON file ending "ByRef" is the results in the WMS/WMTS coordinates. For example, the Tianditu_eva results are in EPSG:27700. 




