#Region " Imports "

Option Strict On
Option Explicit On

Imports EwECore
Imports System.IO
Imports ZedGraph
Imports System.Text
Imports System.Globalization
Imports EwEUtils.Core
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Win32Api

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Content manager base class for invoking NA pyramid interfaces.
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public MustInherit Class cPyramid
    Inherits cContentManager

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Overridden to set up the pyramid file, outsource writing the file and
    ''' invoke the pyramid app.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Overrides Sub DisplayData()

        Dim model As cEwEModel = Me.NetworkManager.Core.EwEModel
        Dim core As cCore = Me.NetworkManager.Core
        Dim ciEnUSLocale As New CultureInfo("en-US")
        Dim sw As StreamWriter = Nothing
        Dim strOutputFile As String = ""
        Dim strOutputFile83 As String = Space(255)
        Dim bSucces As Boolean = False

        strOutputFile = modUtility.PyramidTempFile(model.Name, Me.PyramidType, ".txt")
        sw = New StreamWriter(strOutputFile, False, New System.Text.UTF8Encoding())
        bSucces = Me.WritePyramidFile(core, sw, ciEnUSLocale)
        sw.Close()

        If bSucces Then
            Try
                Kernel32.GetShortPathName(strOutputFile, strOutputFile83, 255)
                bSucces = SystemUtilities.AppExec("pyramid.exe", strOutputFile83, "", "EwENetworkAnalysis")
            Catch ex As Exception

            End Try
        End If

        If Not bSucces Then
            Dim sb As New StringBuilder
            For Each str As String In SystemUtilities.ApplicationLaunchLocations
                If sb.Length > 0 Then sb.Append(", ")
                sb.Append(str)
            Next
            Dim msg As New cMessage(String.Format(My.Resources.PROMPT_APPLAUNCH_FAILED, "pyramid.exe", sb.ToString), _
                                    eMessageType.Any, eCoreComponentType.External, eMessageImportance.Critical)
            Me.NetworkManager.Core.Messages.SendMessage(msg)
        End If

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Override this to state the type of pyramid file to write.
    ''' </summary>
    ''' <returns>A <see cref="ePyramidTypes">pyramid type</see> indicator.</returns>
    ''' -----------------------------------------------------------------------
    MustOverride Function PyramidType() As ePyramidTypes

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Override this to write the pyramid data file.
    ''' </summary>
    ''' <param name="core">Core to use.</param>
    ''' <param name="sw">Stream writer to use.</param>
    ''' <param name="ci">Number format to use.</param>
    ''' <returns>True if all cool and froody.</returns>
    ''' -----------------------------------------------------------------------
    MustOverride Function WritePyramidFile(ByVal core As cCore, _
                                           ByVal sw As StreamWriter, _
                                           ByVal ci As CultureInfo) As Boolean

End Class
