This project is no longer updated.  It has been extended to add additional features so the project has been renamed, see:
http://code.google.com/p/3d-model-prep/

This application was created to help get the animations from FBX files created by Blender in to XNA.  It is probably also useful for any other suitable FBX or DirectX files and includes a 3D animated model viewer to speed up testing.

XNA has two built in importers for 3D models, DirectX(.X) and Autodesk FBX (.fbx).  Both formats can be difficult to use with some modelling programmes.  The Autodesk FBX importer used in XNA 4.0 introduced the limitation that each FBX file can only have one animation.   I have another project that provides scripts to export compatible FBX files from Blender.

The primary goal of this programme is to get the individual animations in to files usable by the content pipeline of XNA.  Alternately use the SkinnedModelImporter class that is on the download list to automatically process FBX files with Multiple Takes in.

<img src='http://2.bp.blogspot.com/_3Om8cOayVsM/TVJPkFkWOCI/AAAAAAAAARs/zKZuqTikWUY/s1600/TakeExtractor2011-02-09.jpg' width='400'>

The TakeExtractor project uses components from the pipeline that cannot be redistributed so it will only work on a development machine and is therefore only available as source code.<br>
