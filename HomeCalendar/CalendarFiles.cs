using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Calendar
{

    /// <summary>
    /// It manages the files used in the Calendar project.
    /// </summary>
    public class CalendarFiles
    {
        private static String DefaultSavePath = @"Calendar\";
        private static String DefaultAppData = @"%USERPROFILE%\AppData\Local\";

        // ====================================================================
        // verify that the name of the file, or set the default file, and 
        // is it readable?
        // throws System.IO.FileNotFoundException if file does not exist
        // ====================================================================
        /// <summary>
        /// Verifies that the specified location of the file exists. 
        /// </summary>
        /// <param name="FilePath"> A string that has the location of the file. </param>
        /// <param name="DefaultFileName"> A string that holds the title of the file. </param>
        /// <returns> A string with the name and the location of the file. </returns>
        /// <exception cref="FileNotFoundException"> Thrown when FilePath does not exist. </exception>
        /// <example>
        /// Sample code: 
        /// <code>
        /// <![CDATA[
        /// String file = CalendarFiles.VerifyReadFromFileName("./test.calendar", "testingReadFromFile.txt");
        /// Console.WriteLine(file);
        /// ]]>
        /// </code>
        /// Sample output: 
        /// <code>
        /// ./test.calendar
        /// </code>
        /// </example>
        public static String VerifyReadFromFileName(String? FilePath, String DefaultFileName)
        {

            // ---------------------------------------------------------------
            // if file path is not defined, use the default one in AppData
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does FilePath exist?
            // ---------------------------------------------------------------
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("ReadFromFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ----------------------------------------------------------------
            // valid path
            // ----------------------------------------------------------------
            return FilePath;

        }

        // ====================================================================
        // verify that the name of the file, or set the default file, and 
        // is it writable
        // ====================================================================

        /// <summary>
        /// Verifies if it is possible to write information to a file. 
        /// </summary>
        /// <param name="FilePath"> A string that has the location of the file. </param>
        /// <param name="DefaultFileName"> A string that holds the title of the file. </param>
        /// <returns></returns>
        /// <exception cref="Exception"> Thrown when we do not have permission to write to the file or when FilePath does not exist. </exception>
        /// <example>
        /// Sample code:
        /// <code>
        /// <![CDATA[
        /// String file = CalendarFiles.VerifyWriteToFileName("./test.calendar", "testingWriteFromFile.txt");
        /// Console.WriteLine(file);
        /// ]]>
        /// </code>
        /// Sample output: 
        /// <code>
        /// ./test.calendar
        /// </code>
        /// </example>
        public static String VerifyWriteToFileName(String? FilePath, String DefaultFileName)
        {
            // ---------------------------------------------------------------
            // if the directory for the path was not specified, then use standard application data
            // directory
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                // create the default appdata directory if it does not already exist
                String tmp = Environment.ExpandEnvironmentVariables(DefaultAppData);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                }

                // create the default Calendar directory in the appdirectory if it does not already exist
                tmp = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath);
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                }

                FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does directory where you want to save the file exist?
            // ... this is possible if the user is specifying the file path
            // ---------------------------------------------------------------
            String? folder = Path.GetDirectoryName(FilePath);
            String delme = Path.GetFullPath(FilePath);
            if (!Directory.Exists(folder))
            {
                throw new Exception("SaveToFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ---------------------------------------------------------------
            // can we write to it?
            // ---------------------------------------------------------------
            if (File.Exists(FilePath))
            {
                FileAttributes fileAttr = File.GetAttributes(FilePath);
                if ((fileAttr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    throw new Exception("SaveToFileException:  FilePath(" + FilePath + ") is read only");
                }
            }

            // ---------------------------------------------------------------
            // valid file path
            // ---------------------------------------------------------------
            return FilePath;

        }



    }
}
