// ===============================================================================
// This file is part of Ecopath with Ecosim (EwE)
//
// EwE is free software: you can redistribute it and/or modify it under the terms
// of the GNU General Public License version 2 as published by the Free Software 
// Foundation.
//
// EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
// PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with EwE.
// If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
//
//
// Copyright 1991- 
//    Ecopath International Initiative, Barcelona, Spain
// ===============================================================================

#region  Imports 

using System;
using System.IO;

#endregion

namespace ValueChain
{
    /// =======================================================================
/// <summary>
/// Helper class offering miscellaneous file-related functionalities.
/// </summary>
/// =======================================================================
    internal class cFileUtils
    {

        /// -------------------------------------------------------------------
    /// <summary>
    /// Convert a text into a string that would be accepted by the OS as a
    /// valid file name.
    /// </summary>
    /// <param name="strText">Text to convert into a file name.</param>
    /// <param name="bProtectPath">Flag stating whether any path information
    /// included in <paramref name="strText">strText</paramref> should be
    /// preserved. If False, an path information is stripped off.</param>
    /// <returns></returns>
    /// -------------------------------------------------------------------
        public static string ToValidFileName(string strText, bool bProtectPath)
        {

            string strPath = "";
            string strFile = "";

            if (string.IsNullOrEmpty(strText))
                return "";

            // 1. Strip off path part
            if (bProtectPath)
            {

                try
                {
                    // Find path\file separator position
                    int iLastSep = strText.LastIndexOf('\\');
                    if (iLastSep == -1)
                        iLastSep = strText.LastIndexOf('/');
                    strPath = strText.Substring(0, iLastSep + 1);
                    strFile = strText.Substring(iLastSep + 1);
                }
                catch (Exception ex)
                {
                    strPath = "";
                    strFile = strText;
                }

                bProtectPath = !string.IsNullOrEmpty(strPath);
            }
            else
            {
                strFile = strText;
            }

            // Clean up
            // strFile = strText.Replace(" ", "_") ' Spaces are definitely allowed under 32 bit ;-)
            strFile = strFile.Replace(@"\", "-");
            strFile = strFile.Replace("/", "-");

            // Replace invalid file name chars with hyphens
            foreach (char c in Path.GetInvalidPathChars())
            {
                if (strPath.IndexOf(c) > -1)
                {
                    strPath = strPath.Replace(Convert.ToString(c), "");
                }
            }

            // Replace invalid file name chars with hyphens
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                if (strFile.IndexOf(c) > -1)
                {
                    strFile = strFile.Replace(c, '-');
                }
            }

            if (bProtectPath)
            {
                strText = Path.Combine(strPath, strFile);
            }
            // Replace all accidental 'double dots'
            // removed ".." replacement so ToValidFileName can resolve relative paths
            else
            {
                strText = strFile;
            }

            return strText.Trim();

        }

        /// -------------------------------------------------------------------
    /// <summary>
    /// Convert a text into a valid file extension.
    /// </summary>
    /// <param name="strText">Text to convert into a file extension.</param>
    /// <returns></returns>
    /// -------------------------------------------------------------------
        public static string ToValidFileExt(string strText, string strDefault)
        {

            if (string.IsNullOrWhiteSpace(strText))
                strText = strDefault;
            if (string.IsNullOrWhiteSpace(strText))
                return "";

            if (strText[0] != '.')
                return "." + strText;
            return strText;

        }

        /// -----------------------------------------------------------------------
    /// <summary>
    /// Checks if a directory is available, and optionally tries to create the directory if missing.
    /// </summary>
    /// <param name="strDirectory">The directory to check.</param>
    /// <param name="bCreate">Optional flag, stating whether the directory 
    /// should be created if it does not exist yet.</param>
    /// <returns>True if the directory is available.</returns>
    /// -----------------------------------------------------------------------
        public static bool IsDirectoryAvailable(string strDirectory, bool bCreate = false)
        {

            // Test if already exists as a file
            if (File.Exists(strDirectory))
                return false;

            bool bExists = Directory.Exists(strDirectory);

            if (!bExists)
            {
                try
                {
                    if (bCreate)
                        bExists = Directory.CreateDirectory(strDirectory) is not null;
                }
                catch (Exception ex)
                {
                    // Whoah
                }
            }

            return bExists;

        }

    }
}