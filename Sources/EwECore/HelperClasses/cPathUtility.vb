Option Strict On
Imports System.IO
Imports System.Text.RegularExpressions
Imports EwEUtils.Utilities

''' ===========================================================================
''' <summary>
''' <para>Utility class for translating a path mask into a full path. The mask can be 
''' composed of valid path characters and <see cref="cPathUtility.ePathPlaceholderTypes">place holders</see>.</para>
''' <para>Place holder texts are expected to be wrapped in curly brackets, { },
''' and are not case-sensitive.</para>
''' </summary>
''' <remarks>
''' <example>
''' <para>A few path examples are:</para>
''' <list type="bullet">
''' <item>{ModelPath}\backup\{ModelFile}_{ModelVersion}.{ModelExt}</item>
''' <item>{MyDocuments}\EwEModels\backup\{modelfile}_{date}_{time}.{modelext}</item>
''' </list>
''' </example>
''' </remarks>
''' ===========================================================================
Public Class cPathUtility

    ''' <summary>
    ''' Enumerated type defining supported path mask placeholders.
    ''' </summary>
    Public Enum ePathPlaceholderTypes As Integer
        ''' <summary>Place holder to receive a model file name.</summary>
        ModelFile
        ''' <summary>Place holder to receive a model file directory.</summary>
        ModelPath
        ''' <summary>Place holder to receive a model file extension.</summary>
        ModelExt
        ''' <summary>Place holder to receive a model version number.</summary>
        ModelVersion
        ''' <summary>Place holder to receive the current date, in short format.</summary>
        [Date]
        ''' <summary>Place holder to receive the current time, in short format.</summary>
        [Time]
        ''' <summary>Place holder to receive the current documents directory.</summary>
        MyDocuments
        ''' <summary>Place holder to receive the current application data directory.</summary>
        MyAppData
        ''' <summary>Place holder to receive the shared application data directory.</summary>
        SharedAppData
        ''' <summary>Place holder to receive the current desktop directory.</summary>
        Desktop
        ''' <summary>Place holder to receive the temp directory.</summary>
        TempFiles
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Resolve a path mask to a true file path.
    ''' </summary>
    ''' <param name="strMask">The path mask to resolve.</param>
    ''' <param name="core">The core that provides model information.</param>
    ''' <param name="strPathOut">The resulting path.</param>
    ''' <returns>True if successful.</returns>
    ''' <remarks>Note that the core needs to have a model loaded.</remarks>
    ''' -----------------------------------------------------------------------
    Public Shared Function ResolvePath(ByVal strMask As String, ByVal core As cCore, ByRef strPathOut As String) As Boolean
        If (core Is Nothing) Then Return False
        If (Not core.StateMonitor.HasEcopathLoaded) Then Return False
        Dim strVersion As String = core.DataSource.Version.ToString
        Return cPathUtility.ResolvePath(strMask, core.DataSource.ToString, strVersion, strPathOut)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Resolve a path mask to a true file path.
    ''' </summary>
    ''' <param name="strMask">The mask to resolve.</param>
    ''' <param name="strPathIn">The source path to obtain mask fields from.</param>
    ''' <param name="strModelVersion">The model file version to substitute.</param>
    ''' <param name="strPathOut">The resulting path.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function ResolvePath(ByVal strMask As String, _
                                       ByVal strPathIn As String, _
                                       ByVal strModelVersion As String, _
                                       ByRef strPathOut As String) As Boolean
        Dim strModelFile As String = Path.GetFileNameWithoutExtension(strPathIn)
        Dim strModelPath As String = Path.GetDirectoryName(strPathIn)
        Dim strModelExt As String = Path.GetExtension(strPathIn)
        Return cPathUtility.ResolvePath(strMask, strModelFile, strModelPath, strModelExt, strModelVersion, strPathOut)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Resolve a path mask to a true file path.
    ''' </summary>
    ''' <param name="strMask">The mask to resolve.</param>
    ''' <param name="strModelFile">The model file name to substitute.</param>
    ''' <param name="strModelPath">The model file directory to substitute.</param>
    ''' <param name="strModelExt">The model file extension to substitute.</param>
    ''' <param name="strModelVersion">The model file version to substitute.</param>
    ''' <param name="strPathOut">The resulting path.</param>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Public Shared Function ResolvePath(ByVal strMask As String, _
                                       ByVal strModelFile As String, ByVal strModelPath As String, ByVal strModelExt As String, ByVal strModelVersion As String, _
                                       ByRef strPathOut As String) As Boolean
        ' Pre-doctor
        strPathOut = strMask

        ' loop-ti-loop
        For Each pht As ePathPlaceholderTypes In [Enum].GetValues(GetType(ePathPlaceholderTypes))
            strPathOut = Regex.Replace(strPathOut, _
                                       "{" & pht.ToString & "}", _
                                       ResolvePlaceholder(pht, strModelFile, strModelPath, strModelExt, strModelVersion), _
                                       RegexOptions.IgnoreCase Or RegexOptions.IgnorePatternWhitespace)
        Next

        ' Remove invalid path chars
        strPathOut = cFileUtils.ToValidFileName(strPathOut, True)

        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Change a place holder into a value.
    ''' </summary>
    ''' <param name="placeholder">The place holder to substitute.</param>
    ''' <param name="strModelFile">The model file name to use.</param>
    ''' <param name="strModelPath">The model file directory to use.</param>
    ''' <param name="strModelExt">The model file extension to use.</param>
    ''' <param name="strModelVersion">The model file version to use.</param>
    ''' <returns>A susbstituted value.</returns>
    ''' -----------------------------------------------------------------------
    Private Shared Function ResolvePlaceholder(ByVal placeholder As ePathPlaceholderTypes, _
                                               ByVal strModelFile As String, _
                                               ByVal strModelPath As String, _
                                               ByVal strModelExt As String, _
                                               ByVal strModelVersion As String) As String

        Dim strResolved As String = ""
        Select Case placeholder
            Case ePathPlaceholderTypes.Date : strResolved = Date.Now.ToShortDateString
            Case ePathPlaceholderTypes.Time : strResolved = Date.Now.ToShortTimeString
            Case ePathPlaceholderTypes.ModelFile : strResolved = strModelFile
            Case ePathPlaceholderTypes.ModelPath : strResolved = strModelPath
            Case ePathPlaceholderTypes.ModelExt : strResolved = strModelExt
            Case ePathPlaceholderTypes.ModelVersion : strResolved = strModelVersion
            Case ePathPlaceholderTypes.MyDocuments : strResolved = My.Computer.FileSystem.SpecialDirectories.MyDocuments
            Case ePathPlaceholderTypes.MyAppData : strResolved = My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData
            Case ePathPlaceholderTypes.Desktop : strResolved = My.Computer.FileSystem.SpecialDirectories.Desktop
            Case ePathPlaceholderTypes.SharedAppData : strResolved = My.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData
            Case ePathPlaceholderTypes.TempFiles : strResolved = My.Computer.FileSystem.SpecialDirectories.Temp
            Case Else : Debug.Assert(False)
        End Select
        Return strResolved

    End Function

End Class
