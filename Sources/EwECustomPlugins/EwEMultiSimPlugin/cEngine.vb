' ===============================================================================
' This file is part of Ecopath with Ecosim (EwE)
'
' EwE is free software: you can redistribute it and/or modify it under the terms
' of the GNU General Public License version 2 as published by the Free Software 
' Foundation.
'
' EwE is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; 
' without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR 
' PURPOSE. See the GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License along with EwE.
' If not, see <http://www.gnu.org/licenses/gpl-2.0.html>. 
'
' Copyright 1991-2012 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.IO
Imports EwECore
Imports EwECore.Ecosim
Imports ScientificInterfaceShared.Controls
Imports EwEUtils.Utilities
Imports EwEUtils.SystemUtilities
Imports EwEUtils.Core
Imports SharedResources = ScientificInterfaceShared.My.Resources

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' The perturb engine that does all the work: modifies forcing functions, runs 
''' Ecosim, and organizes extraction of results.
''' </summary>
''' ---------------------------------------------------------------------------
Friend Class cEngine
    Inherits cThreadWaitBase

#Region " Private helper classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cFFCache

        Private m_ff As cForcingFunction = Nothing
        Private m_asData As Single() = Nothing

        Public Sub New(ff As cForcingFunction)
            Me.m_ff = ff
            Me.m_asData = ff.ShapeData
        End Sub

        Public Sub Restore()
            Me.StartEdit()
            Me.EndEdit()
        End Sub

        Public Sub StartEdit()
            Me.m_ff.LockUpdates()
            For i As Integer = 0 To Me.m_ff.ShapeData.Length - 1
                Me.m_ff.ShapeData(i) = Me.m_asData(i)
            Next
        End Sub

        Public Sub EndEdit()
            Me.m_ff.UnlockUpdates(False)
        End Sub

        Public ReadOnly Property FF As cForcingFunction
            Get
                Return Me.m_ff
            End Get
        End Property

    End Class

#End Region ' Private helper classes

#Region " Privates "

    Private m_uic As cUIContext = Nothing
    Private m_man As cForcingFunctionManager = Nothing
    Private m_astrFiles As String()
    Private m_strOutFolder As String = ""
    Private m_bReadMonthly As Boolean = False
    Private m_options As cEcosimResultWriter.eResultTypes() = Nothing
    Private m_FFCache As New Dictionary(Of String, cFFCache)

    Private m_dgtProgress As cEngine.RunProgressDelegate = Nothing
    Private m_dgtComplete As cEngine.RunCompletedDelegate = Nothing
    Private m_bStopRun As Boolean = False

    Private m_bCreateRunFolder As Boolean = False

    ' -- progress 
    Private m_iNumSteps As Integer
    Private m_iStep As Integer

#End Region ' Privates

#Region " Public bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create the worker.
    ''' </summary>
    ''' <param name="uic">The UI context to operate onto.</param>
    ''' -----------------------------------------------------------------------
    Public Sub New(ByVal uic As cUIContext)

        Me.m_uic = uic
        Me.m_man = uic.Core.ForcingShapeManager

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set whether the run creates a new folder for every run.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Property CreateUniqueRunFolder As Boolean
        Get
            Return Me.m_bCreateRunFolder
        End Get
        Set(value As Boolean)
            Me.m_bCreateRunFolder = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get current run progress.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Progress As Single
        Get
            If Not Me.IsRunning Then Return 0
            Return CSng(Me.m_iStep / Math.Max(1, Me.m_iNumSteps))
        End Get
    End Property

    Public Delegate Sub RunProgressDelegate()
    Public Delegate Sub RunCompletedDelegate()

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Run!
    ''' </summary>
    ''' <param name="dgtComplete">Delegate to call when the run has finished.</param>
    ''' <param name="astrFiles">The files to read and apply.</param>
    ''' <param name="strOutFolder">Output folder.</param>
    ''' <param name="bReadMonthly">States whether files should be read as monthly (true) or annual (false) values.</param>
    ''' annual values (<see cref="TriState.[False]"/>), or in both modes (<see cref="TriState.UseDefault"/>).</param>
    ''' <param name="options"><see cref="cEcosimResultWriter.eResultTypes">Output options</see>.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Run(ByVal dgtComplete As RunCompletedDelegate, _
                   ByVal astrFiles As String(), _
                   ByVal strOutFolder As String, _
                   ByVal bReadMonthly As Boolean, _
                   ByVal options As cEcosimResultWriter.eResultTypes())

        If Me.IsRunning Then Return

        Dim core As cCore = Me.m_uic.Core
        If Not core.SaveChanges() Then Return

        Me.m_bReadMonthly = bReadMonthly
        Me.m_astrFiles = astrFiles
        Me.m_options = options
        Me.m_iStep = 1
        Me.m_iNumSteps = Me.m_astrFiles.Length

        Dim strDate As String = Date.Now.ToString("yy-MM-dd hh-mm")
        Dim strScope As String = cSystemUtils.IIF(bReadMonthly, "monthly", "annual")

        If Me.m_bCreateRunFolder Then
            Me.m_strOutFolder = Path.Combine(strOutFolder, cFileUtils.ToValidFileName(String.Format("Run {0} {1}", strDate, strScope), False))
        Else
            Me.m_strOutFolder = strOutFolder
        End If

        For Each ff As cForcingFunction In Me.m_man
            Me.m_FFCache(ff.Name) = New cFFCache(ff)
        Next

        Me.m_dgtComplete = dgtComplete
        Me.SetWait()

        Try
            Dim thrd As New Threading.Thread(AddressOf RunThreaded)
            thrd.Start()
        Catch ex As Exception
            ' Whoah!
        End Try

    End Sub

    Public Overrides Function StopRun(Optional WaitTimeInMillSec As Integer = -1) As Boolean
        Me.m_bStopRun = True
        Return True
    End Function

#End Region ' Public bits

#Region " Internals "

    Private Sub ReadCSVIntoFF(ByVal strFileName As String)

        Dim reader As StreamReader = Nothing
        Dim values() As String = Nothing
        Dim strName As String = ""
        Dim month As Integer = 0
        Dim value As Single = 0.0!
        Dim lff As New List(Of cFFCache)
        Dim ff As cForcingFunction = Nothing
        Dim msgStatus As cMessage = Nothing
        Dim iRepetitions As Integer = 0

        If Me.m_bReadMonthly Then iRepetitions = 1 Else iRepetitions = cCore.N_MONTHS

        If File.Exists(strFileName) Then

            ' Prevent 'our' FFs from updating prematurely while reading CSV file
            For Each ffc As cFFCache In m_FFCache.Values
                ffc.StartEdit()
            Next

            ' (4) Open the CSV file for reading
            reader = New StreamReader(strFileName) 'read in csv files x1,x2,x3 etc

            ' First line holds FF names
            values = cStringUtils.SplitQualified(reader.ReadLine(), ","c)
            ' Map to FF Cache items
            For i As Integer = 0 To values.Length - 1
                strName = values(i).Trim
                If Me.m_FFCache.ContainsKey(strName) Then
                    lff.Add(Me.m_FFCache(strName))
                Else
                    lff.Add(Nothing)
                    If (msgStatus Is Nothing) Then
                        msgStatus = New cMessage(String.Format(My.Resources.ERROR_UNKNOWN_FUNCTIONS, Path.GetFileNameWithoutExtension(strFileName)), _
                                                 eMessageType.DataValidation, _
                                                 eCoreComponentType.External, _
                                                 eMessageImportance.Warning)
                        msgStatus.Hyperlink = strFileName
                    End If
                    msgStatus.AddVariable(New cVariableStatus(eStatusFlags.FailedValidation, _
                                                              String.Format(My.Resources.ERROR_UNKNOWN_FUNCTION, strName), _
                                                              eVarNameFlags.NotSet, _
                                                              eDataTypes.External, _
                                                              eCoreComponentType.External, 0))
                End If
            Next

            If (msgStatus IsNot Nothing) Then
                Me.m_uic.Core.Messages.SendMessage(msgStatus)
            Else

                ' Check if the end of the file is not reached, the peek would return 0 if at the end of the file
                'From While to end While is a loop that runs if certain conditions are true (as long as there are characters to read left in the file.
                While reader.Peek() > 0

                    'split the line into individual values (seperated by commas)
                    values = cStringUtils.SplitQualified(reader.ReadLine, ","c)

                    For j As Integer = 1 To iRepetitions

                        'month from above +1 for the output of months and also to input for forcing functions
                        month = month + 1

                        For i As Integer = 0 To Math.Min(values.Length, lff.Count) - 1

                            If lff(i) IsNot Nothing Then
                                ' Get a FF from the FF manager
                                ff = lff(i).FF
                                ' By default, do not force a value
                                value = 0.0
                                ' Is a value?
                                If Not String.IsNullOrWhiteSpace(values(i)) Then
                                    ' Try to convert this value and set it into the FF
                                    Try
                                        ' Convert a value from string to a floating point number
                                        value = Single.Parse(values(i))
                                    Catch ex As Exception
                                        ' Alert that CSV is somehow malformed
                                        Debug.Assert(False, "Value '" & values(i) & "' unreadable, a number was expected")
                                    End Try
                                End If

                                ' Does still fit?
                                If (month < ff.ShapeData.Length) Then
                                    ' Set value into FF for a given month
                                    ff.ShapeData(month) = value
                                End If
                            End If
                        Next
                    Next

                End While

                ' CSV has been read, now release the update lock on FFs and apply the content of FFs to Ecosim
                For Each ffc As cFFCache In m_FFCache.Values
                    ffc.EndEdit()
                Next
                Me.m_man.Update()

            End If

            ' Close reader to release the csv file
            reader.Close()

        End If

    End Sub

    Private Sub WriteResults(ByVal strPath As String, ByVal strFile As String, _
                             ByVal outputs As cEcosimResultWriter.eResultTypes())

        Dim resultsWriter As New cEcosimResultWriter(Me.m_uic.Core)
        resultsWriter.WriteResults(strPath, outputs)

    End Sub

    Private Sub UpdateProgress(ByVal strMessage As String, status As eStatusFlags)

        Try
            Dim msg As New cMessage(strMessage, eMessageType.Any, eCoreComponentType.External, eMessageImportance.Information)
            Me.m_uic.Core.Messages.SendMessage(msg)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub RunThreaded()

        Dim core As cCore = Me.m_uic.Core
        Me.m_bStopRun = False

        core.SetBatchLock(cCore.eBatchLockType.Update)
        core.SetStopRunDelegate(AddressOf StopRun)
        cApplicationStatusNotifier.StartProgress(core, My.Resources.STATUS_INITIALIZING, -1)
        Try
            Dim iNum As Integer = Me.m_astrFiles.Length
            Dim i As Integer = 0

            While i < iNum And Not Me.m_bStopRun

                Dim strFile As String = Me.m_astrFiles(i)
                Dim strFileShort As String = Path.GetFileNameWithoutExtension(strFile)
                Dim strFolder As String = Path.Combine(Me.m_strOutFolder, strFileShort)

                If Not cFileUtils.IsDirectoryAvailable(strFolder, True) Then
                    Me.m_bStopRun = True
                End If

                If Not Me.m_bStopRun Then
                    core.SetStopRunDelegate(AddressOf StopRun)
                    cApplicationStatusNotifier.UpdateProgress(core, String.Format(My.Resources.STATUS_LOADING, strFileShort), CSng((1 + i * 4) / (iNum * 4)))
                    Me.ReadCSVIntoFF(strFile)
                End If

                If Not Me.m_bStopRun Then
                    core.SetStopRunDelegate(AddressOf StopRun)
                    cApplicationStatusNotifier.UpdateProgress(core, String.Format(My.Resources.STATUS_RUNNING, strFileShort), CSng((2 + i * 4) / (iNum * 4)))
                    core.RunEcoSim(Nothing, False)
                End If

                If Not Me.m_bStopRun Then
                    core.SetStopRunDelegate(AddressOf StopRun)
                    cApplicationStatusNotifier.UpdateProgress(core, String.Format(My.Resources.STATUS_SAVING, strFileShort), CSng((3 + i * 4) / (iNum * 4)))
                    Me.WriteResults(strFolder, strFile, Me.m_options)
                End If

                i += 1

            End While
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        cApplicationStatusNotifier.UpdateProgress(core, My.Resources.STATUS_RESTORING, -1)
        For Each ffc As cFFCache In Me.m_FFCache.Values
            ffc.Restore()
        Next

        Me.m_man.Update()
        core.DiscardChanges()
        GC.Collect()

        core.SetStopRunDelegate(Nothing)
        core.ReleaseBatchLock(cCore.eBatchChangeLevelFlags.NotSet)
        cApplicationStatusNotifier.EndProgress(core)

        '= agk ='
        Me.ReleaseWait()
        If (Me.m_dgtComplete IsNot Nothing) Then Me.m_dgtComplete.Invoke()

    End Sub

#End Region ' Internals

End Class
