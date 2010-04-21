#Region " Imports "

Option Strict On
Imports EwEUtils.Database
Imports EwEUtils.Core
Imports System.Text
Imports System.IO

#End Region ' Imports

Namespace Database

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Interface for implementing an importer to convert an EwE5 document
    ''' into an EwE6 database.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public MustInherit Class cEwE5ModelImporter

#Region " Public helper classes "

        Protected Class cEwE5ModelInfoSort
            Implements IComparer(Of cEwE5ModelInfo)

            Public Function Compare(ByVal x As cEwE5ModelInfo, ByVal y As cEwE5ModelInfo) As Integer _
                Implements IComparer(Of cEwE5ModelInfo).Compare
                Return String.Compare(x.Name, y.Name)
            End Function

        End Class

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper class, provides information of an EwE5 model found in the 
        ''' source database.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Class cEwE5ModelInfo

            Private m_strID As String = ""
            Private m_strName As String = ""
            Private m_strDescription As String = ""
            Private m_nScenarios As Integer = 0

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Constructor, initializes a new instance of this class.
            ''' </summary>
            ''' <param name="strID">EwE5 modelName for this model.</param>
            ''' <param name="strName">EwE5 modelTitle for this model.</param>
            ''' <param name="strDescription">EwE5 model Remarks for this model.</param>
            ''' <param name="nScenarios">Number of EwE5 scenarios in this model.</param>
            ''' ---------------------------------------------------------------
            Public Sub New(ByVal strID As String, ByVal strName As String, ByVal strDescription As String, ByVal nScenarios As Integer)
                Me.m_strID = strID
                Me.m_strName = strName
                Me.m_strDescription = strDescription
                Me.m_nScenarios = nScenarios
            End Sub

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Gets the internal model ID (EwE5 field modelName) for this model.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property ID() As String
                Get
                    Return Me.m_strID
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Gets the human readable model name (EwE5 field modelTitle) for this model.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Name() As String
                Get
                    Return Me.m_strName
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Gets the model description (EwE5 field Remarks) for this model.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property Description() As String
                Get
                    Return Me.m_strDescription
                End Get
            End Property

            ''' ---------------------------------------------------------------
            ''' <summary>
            ''' Gets the number of scenarios in this model.
            ''' </summary>
            ''' ---------------------------------------------------------------
            Public ReadOnly Property NumScenarios() As Integer
                Get
                    Return Me.m_nScenarios
                End Get
            End Property

        End Class

#End Region ' Public helper classes

#Region " Private vars "

        ''' <summary>EWE5 NULL value.</summary>
        Protected Const cEWE5_NULL As Integer = -90

        ''' <summary>Status log.</summary>
        Protected m_sbLog As New StringBuilder
        ''' <summary>Source database file name.</summary>
        Protected m_strEwE5File As String = ""
        ''' <summary>Target database in EwE6 format.</summary>
        Protected m_dbEwE6 As cEwEDatabase ' Import to (write)
        ''' <summary>
        ''' Name of the model to import.
        ''' </summary>
        Protected m_strModelName As String = ""

        ''' <summary>
        ''' The core to use when importing.
        ''' </summary>
        Protected m_core As cCore = Nothing
        ''' <summary>
        ''' Number of steps that the import process will take.
        ''' </summary>
        Protected m_iNumSteps As Integer = 0
        ''' <summary>
        ''' The current step processed by the import.
        ''' </summary>
        Protected m_iStep As Integer = 0

#End Region ' Private vars

#Region " Construction "

        Public Sub New(ByVal core As cCore, ByVal strEwE5File As String)
            Me.m_core = core
            Me.m_strEwE5File = strEwE5File
        End Sub

#End Region ' Construction

#Region " Overridables "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Connects the importer to an EwE5 source database. This database is
        ''' indicated as a file path, and is assumed to be an MS Access database.
        ''' </summary>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>
        ''' Any database connection established via the Open method must be 
        ''' disconnected via the <see cref="Close">Close</see> method.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public MustOverride Function Open() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Disconnects the importer from its EwE5 source document.
        ''' </summary>
        ''' <remarks>
        ''' Any database connection established via the <see cref="Open">Open</see>
        ''' method must be disconnected via the Close method.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public MustOverride Sub Close()

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the importer was connected to its source document 
        ''' via the <see cref="Open">Open</see> method.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public MustOverride Function IsOpen() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Perform the actual import.
        ''' </summary>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Protected MustOverride Function PerformImport() As Boolean

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get a <see cref="cEwE5ModelInfo">descriptive list of models</see> 
        ''' that can be imported from an attached EwE5 document.
        ''' </summary>
        ''' <returns></returns>
        ''' -------------------------------------------------------------------
        Public MustOverride Function GetModels() As cEwE5ModelInfo()

#End Region ' Overridables

#Region " Public access "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Imports and converts a model in an EwE5 database into a new EwE6 database.
        ''' </summary>
        ''' <param name="strModelName">Name of the model in the EwE5 database to import.</param>
        ''' <param name="db">Database to import into.</param>
        ''' <param name="strLogfileName">File to log import progress to.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Import(ByVal strModelName As String, _
                               ByVal db As cEwEDatabase, _
                               ByVal strLogfileName As String) As Boolean

            Dim bSucces As Boolean = False

            Me.m_sbLog.Length = 0
            Me.m_dbEwE6 = db
            Me.m_strModelName = strModelName

            Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_PROGRESS_STARTED, _
                                        strModelName, Date.Now.ToString()), _
                                        eMessageType.DataImport, eMessageImportance.Information, True)

            bSucces = Me.PerformImport()

            If bSucces Then
                Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_PROGRESS_SUCCES, _
                                            strModelName, Date.Now.ToString()), _
                                            eMessageType.NotSet, eMessageImportance.Information, True)
            Else
                Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_PROGRESS_FAILED, _
                                            strModelName, Date.Now.ToString()), _
                                            eMessageType.DataImport, eMessageImportance.Information, True)
            End If

            ' Concoct log file name
            strLogfileName = Path.Combine(Path.GetDirectoryName(db.Name), Path.GetFileNameWithoutExtension(db.Name))
            strLogfileName += "_import_log"
            strLogfileName = Path.ChangeExtension(strLogfileName, "txt")

            ' Write log to text file with the same file name as the destination db name
            cLog.WriteTextToFile(strLogfileName, Me.m_sbLog)

            Return bSucces

        End Function

#End Region ' Public access

#Region " Status logging "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Logs a progress message.
        ''' </summary>
        ''' <param name="strMessage">Progress message.</param>
        ''' <param name="iStep">Progress step. If -1, the internal step admin is 
        ''' automatically incremented.</param>
        ''' -------------------------------------------------------------------
        Protected Sub LogProgress(ByVal strMessage As String, Optional ByVal iStep As Integer = -1)

            Dim sProgress As Single = 0

            ' Need to auto-increment step?
            If (iStep < 0) Then
                ' #Yes: auto-increment
                Me.m_iStep += 1
            Else
                ' #No: set the step
                Me.m_iStep = iStep
            End If

            ' Calculate progress
            If (Me.m_iNumSteps <> 0) Then
                sProgress = CSng(Me.m_iStep / Me.m_iNumSteps)
            Else
                sProgress = 1.0
            End If

            ' Send to log
            Me.LogMessage(strMessage, eMessageType.DataImport, eMessageImportance.Information, False)

            ' Public to core if possible
            If (Me.m_core IsNot Nothing) Then
                ' Send progress message
                Me.m_core.Messages.SendMessage(New cProgressMessage(sProgress, strMessage, eMessageType.DataImport, eCoreComponentType.DataSource))
            End If

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Logs a message
        ''' </summary>
        ''' -------------------------------------------------------------------
        Protected Sub LogMessage(ByVal strMessage As String, _
                                 Optional ByVal msgType As eMessageType = eMessageType.DataImport, _
                                 Optional ByVal msgImportance As eMessageImportance = eMessageImportance.Information, _
                                 Optional ByVal bPublishToInterface As Boolean = False)

            ' Add message to log
            Me.m_sbLog.AppendLine(strMessage)

            ' Publicly log message
            If (bPublishToInterface = True) And (Me.m_core IsNot Nothing) Then
                Me.m_core.m_publisher.SendMessage(New cMessage(strMessage, msgType, eCoreComponentType.DataSource, msgImportance))
            End If

        End Sub

#End Region ' Status logging

    End Class

End Namespace
