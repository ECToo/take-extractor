#region File Description
// Author: JCBDigger
// URL: http://Games.DiscoverThat.co.uk
// URL: http://www.MistyManor.co.uk
//-----------------------------------------------------------------------------
// Based on the WinFormsContentLoading sample by Microsoft
//-----------------------------------------------------------------------------
// Extract takes from 3D model files and save them in to separate files
//-----------------------------------------------------------------------------
// Originally designed because the Autodesk FBX importer included with XNA 4.0 
// only supports one animation (take) per file.
// Blender can export multiple takes per file.
//
// In addition the takes are also converted and saved to the keyframe format used
// by my game for individual animation clips.
//
// The clips can be loaded back and viewed individually.
//-----------------------------------------------------------------------------
// To extract files for use in my game this tool uses a config file.
//
// File formats for Take Conversion config files (.takes)
// There are several formats the first line of the 'takes' file
// indicates what format the rest of the file is in.
// (Most things are case sensitive)
//
// Format Type 1
// =============
// 1
// Source file containing all the animations and model in one file
// X|Y|Z rotation to apply to the model while loading typically 90, 0, 180
// RigType|Armature Rig Prefix
// HeadBones|Neck|Head
// ArmBones|L-Upper|L-Hand|R-Hand|...
// type|SourceTakeName|OutputTakeName
// type|SourceTakeName|OutputTakeName
// type|SourceTakeName|OutputTakeName
// ...
//
// Format Type
//  1
//
// Source file
//  The full name and relative path to the file to extract the model and animations from.
//  At the moment only FBX files are supported.
//  All paths have to be relative to the folder that the 'takes' file is loaded from.
//
// X|Y|Z
//  Rotation to apply to the model while loading
//  e.g. To convert from Blender Z Up to XNA Y Up use 90|0|180
//       for no rotation use 0|0|0
//
// RigType identifies different armature configurations: Human, Alien, LocalHuman etc.
//  In conjunction with the type it is used to filter which bones are extracted in to
//  the output take file
//  e.g. RigType|alien or RigType|human
//
// HeadBones and ArmBones: 
//  The list of bones used in that animation part.
//  e.g. ArmBones|L-Collar|L-UpperArm|L-Forearm|L-Hand|R-Collar|R-UpperArm|R-Forearm|R-Hand|R-Aim
//
// Type is the name of the full or part animation and can be: clip, head or arms
//  Clip = Full animation
//  Head = Just the bones used to move the head to look round
//  Arms = Just the bones used to move the arms to aim a weapon (this does not usually include the fingers)
// SourceTakeName = the name of the animation take in the source file
// OutputTakeName = the name used in game to reference the take.
//      This does not include the rig armature name.
// e.g. Arms|Snipe|AimRifle or Clip|Patrol2|Patrol
//
// Format Type 2
// =============
// 2
// Source file containing the model
// X|Y|Z rotation to apply to the model while loading typically 90, 0, 180
// RigType|Armature Rig Prefix
// HeadBones|Neck|Head
// ArmBones|L-Upper|L-Hand|R-Hand|...
// type|SourceAnimationFileName|OutputTakeName
// type|SourceAnimationFileName|OutputTakeName
// type|SourceAnimationFileName|OutputTakeName
// ...
//
// Format Type
//  2
//
// Source file
//  Only the model data is used from this file any animations in it are ignored
//  All paths have to be relative to the folder that the 'takes' file is loaded from.
//
// Rotation, RigType, HeadBones, ArmBones, the clip type and the OutputTakeName are the same as for type 1 files.
//
// SourceAnimationFileName = the relative path to the file containing the animation.  Only the 
//  first animation in the file is used.
//  All paths have to be relative to the folder that the 'takes' file is loaded from.
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
// FEATURES
//-----------------------------------------------------------------------------
// - Load and view models, zoom in and out and rotate round
// - Split FBX files in to one file per animation
// - Load animations from FBX files
// - Load animations from AnimationClips
// - Save animationClips in the format used by the Microsoft Skinning Sample
// - Use a config file to convert multiple animations in to animationClip format
// - Show a floor
//-----------------------------------------------------------------------------


//-----------------------------------------------------------------------------
// TODO (Extras):
//-----------------------------------------------------------------------------
// - Extract individual takes from a model that has one very long take.
//      Like the Mech robots have.
//      Add a new type
//      Type = List
//      Enter the range of keyframes (from|To) inclusive
//      The name of the animation to save (used for both the FBX and the keyframe files)
// e.g.
//  List|1250|2300|Walk
//
// - Add a UI for adding animations to the merge animations processor
//     Merge animations:
//      http://blogs.msdn.com/b/shawnhar/archive/2010/06/18/merging-animation-files.aspx
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
//      Server
//        http://subversion.apache.org/
//        http://subversion.tigris.org/
//      Command line client
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
