#region File Description
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
//-----------------------------------------------------------------------------
// Based on the WinFormsContentLoading sample by Microsoft
//-----------------------------------------------------------------------------
// Extract takes from 3D model files and save them in to separate files
//-----------------------------------------------------------------------------
// Originally designed because the FBX importer with XNA 4.0 only supports 
// one animation (take) per file.  Blender exports multiple takes per
// file and it is difficult to get Blender to do otherwise.
//
// In addition the takes are also converted and saved to the format used
// by my game for individual animation clips.
//-----------------------------------------------------------------------------
// To extract files for use in my game this tool uses a config file.
//
// File format for Take Conversion config files (.takes)
//  (Most things are case sensitive)
//
// Source file
// X|Y|Z
// RigType|Armature Rig Prefix
// HeadBones|Neck|Head
// ArmBones|L-Upper|L-Hand|R-Hand|...
// type|SourceTakeName|OutputTakeName
// type|SourceTakeName|OutputTakeName
// type|SourceTakeName|OutputTakeName
// ...
//
// Source file
//  The full name and relative path to the file to extract animations from.
//  At the moment only FBX files are supported.
//
// X|Y|Z
//  Rotation to apply to the model while loading
//  e.g. 90|0|180 or 0|0|0
//
// RigType identifies different armature configurations: Human, Alien, LocalHuman etc.
//  In conjunction with the type it is used to extract different bones in to
//  the output take file
//  e.g. RigType|alien or RigType|human
//
// HeadBones and ArmBones: 
//  The list of bones used in that animation part.
//  e.g. ArmBones|L-Collar|L-UpperArm|L-Forearm|L-Hand|R-Collar|R-UpperArm|R-Forearm|R-Hand|R-Aim
//
// Type is the name of the full or part animation and can be: clip, head or arms
//  Clip = Full animation
//  Head = Bones used to move the head to look round
//  Arms = Bones used to move the arms to aim a weapon (this does not usually include the fingers)
// SourceTakeName = the name of the animation take in the source file
// OutputTakeName = the name used in game to reference the take.
//      This does not include the rig armature name.
// e.g. Arms|Snipe|AimRifle or Clip|Patrol2|Patrol
//
//-----------------------------------------------------------------------------
// The output clip format is simply the Keyframes saved from the SkinningData
// It can be read back in as an AnimationClip for use with the SkinningSample
//-----------------------------------------------------------------------------
#endregion

#region MIT License
/*
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
#endregion

#region Using Statements
using System;
using System.Windows.Forms;
#endregion

//-----------------------------------------------------------------------------
// TODO:
//-----------------------------------------------------------------------------
// - Change the ContentBuilder so it does not use a hard coded path to the pipeline DLL
// - Save separate FBX files with just one take in each
// - Save a bone map (index to bone name)

// - Include the bone names for head or arms within the takes file
// Type|Arms|L-Arm|R-Arm etc.
// Type|Head|Head|Neck

// Extract individual takes from a model that has one very long take.
//  Like the Mech robots have.
//  Type = List

// Pass parameters to the processor
// http://forums.create.msdn.com/forums/p/64081/392300.aspx#392300
/*
Re: How do I add parameters to the ModelProcessor in ContentBuilder? Reply Quote  
 if (processor == "ModelProcessor")   
{   
     buildItem.SetMetadata("ProcessorParameters_TextureFormat", "Color");   
}  

Heres a snippit from an old project of mine. 

To find out the names of the pieces of metadata, set a parameter on a piece of content as you want it. Right click on the content project in the solution explorer, then choose unload. Right click again, chooses edit. Find the piece of content in the file, and you should see entries like 

<ProcessorParameters_GenerateMipmaps>True</ProcessorParameters_GenerateMipmaps>  
for all peramiters set to non standard values. Thats what you need to feed MSBuild.

Remember to reload the content project afterewards :) 
 
 */

//-----------------------------------------------------------------------------

#region Source Control
//-----------------------------------------------------------------------------
// GoogleCode project home
//  http://code.google.com/p/take-extractor/
// SVN working URLs
//  Upload https://take-extractor.googlecode.com/svn/trunk/
//  Download http://take-extractor.googlecode.com/svn/trunk/
// Prefer to use the GUI client
//        http://tortoisesvn.tigris.org/
// My settings subversion
//  in the 'servers' ini file
//      [global]
//      store-passwords = no
//      store-plaintext-passwords = no
//  in the 'config' ini file
//      [auth]
//      store-passwords = no
//      store-auth-creds = no
//      [miscellany]
//      global-ignores = obj bin *.cachefile *.suo *.user
//-----------------------------------------------------------------------------
// Subversion
//   - Software
//      Command line and server
//        http://subversion.apache.org/
//        http://subversion.tigris.org/
//      Download
//        http://www.sliksvn.com/en/download
//      Windows Explorer extension GUI
//        http://tortoisesvn.tigris.org/
//   - Hosting
//      http://unfuddle.com  (Free 200Meg or $9 per month)
//      http://www.sliksvn.com  (Free 100Meg or €5 per month)
//      http://code.google.com/p/support/wiki/GettingStarted  (Only for open source projects)
//   - Documentation
//      http://svnbook.red-bean.com/
//   - Commands
//      svn help [subcommand]
//      svn import [path] URL -m "note" --username ????
//          Get the inital project on to the Subversion repository.
//          [path] can be .
//          You still need to checkout that initial version to start using it.
//      svn checkout URL --username ????
//          e.g. svn checkout http://svn.collab.net/repos/svn/ourproject
//          This gets the latest version of the files or all of them if this is the first time
//      svn update --username ????
//          Bring the local version up to the version stored in the repository.
//          If there are any conflicts they will be highlighted at this point.
//      svn status
//          List any local files that are not uinder version control
//      svn add "path\filename.ext"
//          Add any files not currently under version control
//      svn delete "path\filename.ext"
//          Remove files no longer needed under version control
//      svn commit -m "note" --username ????
//          Sends all the changes back to the server.
//      Ignoring some files
//          global-ignores
//              Contains file and folder name paterns to match and ignore
//              Configured using the Runtime Configuration Area
//              Simply a folder called Subversion in the Application Data
//              director for each user of subversion.
//              global-ignores is in the config file.
//              http://svnbook.red-bean.com/en/1.5/svn.advanced.confarea.html
//          svn:ignore
//              Contains the names of files within the folder svn:ignore is in which svn should ignore
//-----------------------------------------------------------------------------
#endregion


namespace Extractor
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class ProgramStart
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
