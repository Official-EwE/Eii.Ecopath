Option Strict On
Imports System.Data
Imports System.IO
Imports EwEUtils.Database
Imports EwEUtils.Core

Namespace Database

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Imports an EwE5 database into an EwE6 database
    ''' </summary>
    ''' <example>
    ''' The following example illustrates how to use this class:
    ''' <code>
    ''' Dim dbImp as New cEwE6DatabaseImporter(cCore.GetInstance())
    ''' Dim lModels As List(Of cEwE6DatabaseImporter.cEwE5ModelInfo) = Nothing
    ''' Dim model As cEwE6DatabaseImporter.cEwE5ModelInfo = Nothing
    ''' Dim nSucces As Integer = 0
    ''' 
    ''' ' Attach to an MS Access database
    ''' If (dbImp.Open("my_ewe5.mdb")) Then
    '''     ' Can Import?
    '''     If (dmImp.CanImport()) Then
    '''         ' Get models
    '''         lModels = dmImp.GetModels()
    '''         ' Import each model
    '''         For i As Integer = 0 To lModels.Count - 1
    '''            ' Get model
    '''            model = lModes(i)
    '''            ' Import the model
    '''            If (dmImp.Import(model.ID, String.Format("EwE6_{0}.mdb", model.Name))) Then
    '''               ' Count Succes
    '''               nSucces += 1
    '''            End If
    '''         Next i
    '''     End If
    '''     ' Clean up
    '''     dbImp.Close()
    ''' End If
    ''' </code>
    ''' </example>
    ''' -----------------------------------------------------------------------
    Public Class cEwE6DatabaseImporter

        Private Const cDBVERSION_EWE5_MIN As Single = 1.67
        Private Const cDBVERSION_EWE5_MAX As Single = 1.73
        Private Const cDBVERSION_EWE6 As Single = 6.0
        Private Const cDBVERSION_FUTURE As Single = 7.0

#Region " Private bits "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Enum describing the method used to connect to an EwE5 source database.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Enum eOpenType
            NotSet = 0
            File
            Database
        End Enum

        ''' <summary>EWE5 NULL value.</summary>
        Private Const cEWE5_NULL As Integer = -90

        ' Progress admin
        ''' <summary>Attached core.</summary>
        Private m_core As cCore = Nothing
        ''' <summary>Number of import steps to perform.</summary>
        Private m_nSteps As Integer = 0
        ''' <summary>Current import step counter.</summary>
        Private m_iStep As Integer = 0

        ' Databases
        ''' <summary>Source database in EwE5 format.</summary>
        Private m_dbEwE5 As cEwEDatabase ' Import from (read)
        ''' <summary>Target database in EwE6 format.</summary>
        Private m_dbEwE6 As cEwEDatabase ' Import to (write)
        ''' <summary>Flag indicating the method used to attach to the source database.</summary>
        Private m_openType As eOpenType = eOpenType.NotSet

        ' Tables that will receive information throughout the import process
        ''' <summary>Continuous open writer for Remarks.</summary>
        Private m_writerRemarks As cEwEDatabase.cEwEDbWriter = Nothing
        Private m_dtRemarks As DataTable = Nothing

        '''' <summary>Continuous open writer for References.</summary>
        'Private m_writerReferences As cEwEDatabase.cEwEDbWriter = Nothing

        ''' <summary>Primary keys lookup table</summary>
        Private m_adtKeys() As Dictionary(Of String, Integer)
        ''' <summary>Dictionaries, per datatype, of EwE Poolcode index to EwE6 DatabaseID.</summary>
        Private m_adtIndexes() As Dictionary(Of Integer, Integer)

        ''' <summary>Shape counter.</summary>
        Private m_iNextShapeID As Integer = 1

        Private m_sbLog As New Text.StringBuilder

#End Region ' Private bits 

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="core">The Core to send messages through.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(ByVal core As cCore)
            ' Store core ref
            Me.m_core = core
        End Sub

#End Region ' Constructor

#Region " Messages "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Logs a progress message.
        ''' </summary>
        ''' <param name="strMessage">Progress message.</param>
        ''' -------------------------------------------------------------------
        Private Sub LogProgress(ByVal strMessage As String, Optional ByVal iStep As Integer = -1)
            Dim sProgress As Single = 0
            If Me.m_core Is Nothing Then Return
            If iStep = -1 Then m_iStep += 1 Else m_iStep = iStep
            If Me.m_nSteps <> 0 Then sProgress = CSng(Me.m_iStep / Me.m_nSteps) Else sProgress = 1.0
            ' Send as progress message
            Me.m_core.Messages.SendMessage(New cProgressMessage(sProgress, strMessage, eMessageType.DataImport, eCoreComponentType.DataSource))
            ' Send to log as well
            Me.LogMessage(strMessage, eMessageType.DataImport, eMessageImportance.Information, False)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Logs a message
        ''' </summary>
        ''' -------------------------------------------------------------------
        Private Sub LogMessage(ByVal strMessage As String, _
                Optional ByVal msgType As eMessageType = eMessageType.DataImport, _
                Optional ByVal msgImportance As eMessageImportance = eMessageImportance.Information, _
                Optional ByVal bPublishToInterface As Boolean = False)

            Me.LogMessage(New cMessage(strMessage, msgType, eCoreComponentType.DataSource, msgImportance))
            ' Log everything
            Me.m_sbLog.AppendLine(strMessage)

        End Sub

        Private Sub LogMessage(ByVal msg As cMessage, _
                Optional ByVal bPublishToInterface As Boolean = False)

            ' Send warnings and criticals to the core
            bPublishToInterface = bPublishToInterface Or ((msg.Importance = eMessageImportance.Warning) Or (msg.Importance = eMessageImportance.Critical))

            If bPublishToInterface Then
                Me.m_core.m_publisher.AddMessage(msg)
            End If
        End Sub

#End Region ' Messages

#Region " Initialization "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Connects the importer to an EwE5 source database. This database is
        ''' indicated as a file path, and is assumed to be an MS Access database.
        ''' </summary>
        ''' <param name="strEwE5DBName">Path to the EwE5 MS Access database
        ''' to import from.</param>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>
        ''' Any database connection established via the Open method must be 
        ''' disconnected via the <see cref="Close">Close</see> method.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Function Open(ByVal strEwE5DBName As String) As Boolean
            ' Pre
            Debug.Assert(Not (Me.isOpen() Or Me.IsAttached()))

            ' Create db
            Dim db As New cEwEAccessDatabase()
            If db.Open(strEwE5DBName) = eDatasourceAccessType.Opened Then
                Me.m_openType = eOpenType.File
                Me.m_dbEwE5 = db
            End If

            Return Me.isOpen()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Disconnects the importer from its EwE5 source database.
        ''' </summary>
        ''' <remarks>
        ''' Any database connection established via the <see cref="Open">Open</see>
        ''' method must be disconnected via the Close method.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub Close()
            ' Pre
            Debug.Assert(Me.isOpen())

            Me.m_dbEwE5.Close()
            Me.m_dbEwE5 = Nothing
            Me.m_openType = eOpenType.NotSet
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the importer was connected to its source database 
        ''' via the <see cref="Open">Open</see> method.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function isOpen() As Boolean
            Return (Me.m_openType = eOpenType.File)
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Connects the importer to an EwE5 source database, which is passed in
        ''' as a ready-to-use object.
        ''' </summary>
        ''' <param name="db">The EwE5 database to import from.</param>
        ''' <returns>True if succesful.</returns>
        ''' <remarks>
        ''' Any database connection established via the Attach method must be 
        ''' disconnected via the <see cref="Detach">Detach</see> method.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Function Attach(ByRef db As cEwEDatabase) As Boolean
            ' Pre
            Debug.Assert(Not (Me.isOpen() Or Me.IsAttached()))

            If (db.GetConnection().State = ConnectionState.Open) Then
                Me.m_openType = eOpenType.Database
                Me.m_dbEwE5 = db
            End If

            Return Me.IsAttached()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Disconnects the importer from its EwE5 source database.
        ''' </summary>
        ''' <remarks>
        ''' Any database connection established via the <see cref="Attach">Attach</see>
        ''' method must be disconnected via the Detach method.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Sub Detach()
            ' Pre
            Debug.Assert(Me.IsAttached())

            Me.m_dbEwE5 = Nothing
            Me.m_openType = eOpenType.NotSet
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether the importer was connected to its source database 
        ''' via the <see cref="Attach">Attach</see> method.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Function IsAttached() As Boolean
            Return (Me.m_openType = eOpenType.Database)
        End Function

        Public Enum eSourceDatabaseVersionTypes As Integer
            Unknown = 0
            EwE5TooOld
            EwE5Supported
            EwE5TooNew
            EwE6
            UnknownFuture
        End Enum

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' States whether this class is able to import data from a given
        ''' EwE database version number.
        ''' </summary>
        ''' <param name="sVersion">The version number to validate.</param>
        ''' <returns>True if the importer can import from a database
        ''' with the given version number.</returns>
        ''' -------------------------------------------------------------------
        Public Shared Function EstimateVersion(ByVal sVersion As Single) As eSourceDatabaseVersionTypes
            If (sVersion = 0.0!) Then Return eSourceDatabaseVersionTypes.Unknown
            If (sVersion < cDBVERSION_EWE5_MIN) Then Return eSourceDatabaseVersionTypes.EwE5TooOld
            If (sVersion <= cDBVERSION_EWE5_MAX) Then Return eSourceDatabaseVersionTypes.EwE5Supported
            If (sVersion < cDBVERSION_EWE6) Then Return eSourceDatabaseVersionTypes.EwE5TooNew
            If (sVersion < cDBVERSION_FUTURE) Then Return eSourceDatabaseVersionTypes.EwE6
            Return eSourceDatabaseVersionTypes.UnknownFuture
        End Function

#End Region ' Initialization

#Region " Information for the outside world "

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

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, returns the models that were found in the source database.
        ''' </summary>
        ''' <returns>A list of 
        ''' <see cref="cEwE6DatabaseImporter.cEwE5ModelInfo">cEwE5ModelInfo</see>
        ''' objects.</returns>
        ''' -------------------------------------------------------------------
        Public Function GetModels() As List(Of cEwE5ModelInfo)
            ' Pre
            Debug.Assert(Me.isOpen() Or Me.IsAttached())

            Dim l As New List(Of cEwE5ModelInfo)
            Dim mi As cEwE5ModelInfo = Nothing
            Dim r As IDataReader = Me.m_dbEwE5.GetReader("SELECT Models.modelName, Models.modelTitle, Models.remarks, (SELECT COUNT(*) FROM [Ecosim] WHERE (Models.modelName = Ecosim.modelName)) as NumScenarios FROM [Models] GROUP BY Models.modelName, Models.modelTitle, Models.remarks")

            If r Is Nothing Then Return Nothing

            While r.Read()
                mi = New cEwE5ModelInfo(CStr(r(0)), CStr(r(1)), _
                    CStr(Me.FixValue(r, "remarks", My.Resources.CoreMessages.IMPORT_NO_DESCRIPTION)), CInt(r(3)))
                l.Add(mi)
            End While

            Return l
        End Function

#End Region ' Information for the outside world 

#Region " The import "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Imports and converts a model in an EwE5 database into a new EwE6 database.
        ''' </summary>
        ''' <param name="strModelName">Name of the model in the EwE5 database to import.</param>
        ''' <param name="db">Database to import into.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Public Function Import(ByVal strModelName As String, ByVal db As cEwEDatabase, ByRef strLogfileName As String) As Boolean

            Dim bSucces As Boolean = True

            ' Allocate primary key lookup tables
            ReDim m_adtKeys(System.Enum.GetValues(GetType(eDataTypes)).Length)
            ' Allocate object indexes lookup tables
            ReDim Me.m_adtIndexes(System.Enum.GetValues(GetType(eDataTypes)).Length)
            ' Clear log
            Me.m_sbLog.Length = 0

            Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_PROGRESS_STARTED, _
                    strModelName, _
                    Date.Now.ToString()), _
                    eMessageType.DataImport, eMessageImportance.Information, True)

            ' Assume the worst
            bSucces = False

            ' Perform actual import
            bSucces = Me.Import(strModelName, db)
 
            If bSucces Then
                Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_PROGRESS_SUCCES, _
                    strModelName, Date.Now.ToString()), eMessageType.NotSet, eMessageImportance.Information, True)
            Else
                Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_PROGRESS_FAILED, _
                    strModelName, Date.Now.ToString()), eMessageType.DataImport, eMessageImportance.Information, True)
            End If

            ' Concoct log file name
            strLogfileName = Path.Combine(Path.GetDirectoryName(db.Name), Path.GetFileNameWithoutExtension(db.Name))
            strLogfileName += "_import_log"
            strLogfileName = Path.ChangeExtension(strLogfileName, "txt")

            ' Write log to text file with the same file name as the destination db name
            cLog.WriteTextToFile(strLogfileName, Me.m_sbLog)

            Return bSucces

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Imports and converts a model in an EwE5 database into a provided EwE6 database.
        ''' </summary>
        ''' <param name="strModelName">Name of the model in the EwE5 database to import.</param>
        ''' <param name="dbEwE6">An opened EwE6 dataabase.</param>
        ''' <returns>True if succesful.</returns>
        ''' -------------------------------------------------------------------
        Protected Function Import(ByVal strModelName As String, ByVal dbEwE6 As cEwEDatabase) As Boolean

            ' Pre
            Debug.Assert(Me.isOpen() Or Me.IsAttached(), "Must Open() or Attach() to an EwE5 database first")
            Debug.Assert(dbEwE6 IsNot Nothing, "Needs a valid EwE6 database instance")
            Debug.Assert(dbEwE6.GetConnection().State = ConnectionState.Open, "EwE6 database must already be open")

            Dim dbUpd As cDatabaseUpdater = Nothing

            ' Set progress info (fixed)
            Me.m_nSteps = 29
            Me.m_iStep = 0

            Me.m_dbEwE6 = dbEwE6

            ' Open long-term writers
            Me.m_writerRemarks = Me.m_dbEwE6.GetWriter("Remark")
            Me.m_dtRemarks = Me.m_writerRemarks.GetDataTable()

            ' JS 061221: References do not need to be imported 
            ' Me.m_tlReferences = Me.m_dbEwE6.GetSequentialWriter("Reference")

            ' Start the actual import process.
            ' Note that VB6 function names are used here to make it easier to map to the old code.

            ' -------
            ' ECOPATH
            ' -------

            Me.LogProgress(String.Format(My.Resources.CoreMessages.IMPORT_PROGRESS_MODEL, strModelName))
            Me.ImportModels(strModelName)

            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOPATHGROUPS)
            Me.ImportEcopathGroups(strModelName)
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOPATHGROUPS)
            Me.ImportGroupSize(strModelName)
            Me.ImportBasicRemarks(strModelName)

            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_STANZA)
            Me.ImportGroupStanza(strModelName)

            'ImportGroupTaxon strModelName (discontinued in EwE5)
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_POGRESS_DIETCOMP)
            Me.ImportGroupxGroup(strModelName)

            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_FLEET)
            Me.ImportGear(strModelName)
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_CATCH)
            Me.ImportCatch(strModelName)
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_CATCH)
            Me.ImportCatchCodes(strModelName)
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_CATCH)
            Me.ImportDiscardFate(strModelName)

            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_PEDIGREE)
            Me.ImportPedigree(strModelName)

            ' Discontinued in EwE6, but throw a warning when EwE5 data exists
            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECORANGER)
            Me.ImportEcoranger(strModelName)
            'ImportEcoRangerN(strModelName)
            'ImportEcoRangerNxN1(strModelName)

            ' ------
            ' ECOSIM
            ' ------

            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_SCENARIO)
            If Me.ImportEcoSim(strModelName) Then

                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOSIMGROUPS)
                Me.ImportEcoSimN(strModelName)

                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_FORCINGMEDIATION)
                Me.ImportEcoSimnShapes(strModelName)
                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_FORCINGAPPLICATIONS)
                Me.ImportEcoSimNxNInteraction(strModelName)
                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_FORCINGAPPLICATIONS)
                Me.ImportEcoSimNxN(strModelName)
                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_FORCINGAPPLICATIONS)
                Me.ImportEcoSimMedWeights(strModelName)

                ' Discontinued in EwE6, but still throw a warning
                Me.ImportEcoSimPairs(strModelName)

                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_FLEET)
                Me.ImportEcoSimFishGear(strModelName)

                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_TIMESERIES)
                Me.ImportTimeSeries(strModelName)

            End If

            ' --------
            ' ECOSPACE
            ' --------

            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOSPACESCENARIOS)
            If (Me.ImportEcoSpaceScenario(strModelName)) Then

                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROPRESS_ECOSPACEHABITATS)
                Me.ImportEcospaceHabitats(strModelName)

                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOSPACEGROUPS)
                Me.ImportEcoSpaceGroups(strModelName)

                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOSPACEREGIONS)
                Me.ImportEcospaceRegions(strModelName)

                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOSPACEMPAS)
                Me.ImportEcospaceMPA(strModelName)

                ' Import fleets after habitats and MPAs
                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOSPACEFLEETS)
                Me.ImportEcoSpaceFleets(strModelName)

                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOSPACEBASEMAP)
                Me.ImportEcospaceBasemap(strModelName)

            End If

            ' ---------
            ' ECOTRACER
            ' ---------

            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOTRACER)
            If (Me.ImportEcotracer(strModelName)) Then

                Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_ECOTRACERGROUPS)
                Me.ImportEcotracerN(strModelName)

            End If

            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_QUOTES)
            Me.ImportQuotes()

            'ImportFlowBox(strModelName)
            'ImportFlowConnector(strModelName)
            'ImportFlowLabel(strModelName)
            'ImportFlowLineSource(strModelName)
            'ImportFlowLines(strModelName)
            'ImportGroupTaxon(strModelName)
            'ImportOutputParam(strModelName)
            'ImportPyramidMain(strModelName)
            'ImportPyramidSource(strModelName)
            'ImportSummaryStatistics(strModelName)

            ' Save long-term writers
            Me.m_dbEwE6.ReleaseWriter(Me.m_writerRemarks, True)
            ' JS 061221: References do not need to be imported
            'Me.m_dbEwE6.ReleaseWriter(Me.m_writerReferences, True)

            ' Now run all available updates on the new EwE6 database
            dbUpd = New cDatabaseUpdater(6.0)
            dbUpd.UpdateDatabase(Me.m_dbEwE6, Me.m_core.PluginManager)
            dbUpd = Nothing

            ' Release DB
            Me.m_dbEwE6 = Nothing

            Me.LogProgress(My.Resources.CoreMessages.IMPORT_PROGRESS_COMPLETE, Me.m_nSteps)

            Return True
        End Function

#End Region ' The import 

#Region " Implementation "

#Region " Generic "

        Private Function SplitNumberListString(ByVal strMemo As String, Optional ByVal cSplitChar As Char = CChar(" "), _
                Optional ByVal nDefaultNumberLen As Integer = 7) As String()

            Dim astrMemoBits() As String = {""}
            Dim sValue As Single = 0.0!

            If strMemo.Length = 0 Then
                Return astrMemoBits
            End If

            ' Remove irrelevant bits
            strMemo = strMemo.Trim

            ' Check for non-separating comma's
            If (strMemo.IndexOf(CChar(",")) > -1) And (cSplitChar <> CChar(",")) Then
                ' Has no decimal points?
                If strMemo.IndexOf(CChar(".")) = -1 Then
                    ' No decimal points? Assume comma's represent decimal separators and replace 'em all with decimal points
                    strMemo = strMemo.Replace(CChar(","), CChar("."))
                Else
                    ' String contains both comma's and decimal points. Assume that comma's represent thousand separators and remove them
                    strMemo = strMemo.Replace(CChar(","), CChar(""))
                End If
            End If

            ' Is splitter char available?
            If strMemo.IndexOf(cSplitChar) = -1 Then

                ' #Separator character not found. Try to calc length of each number strings by 
                ' examining how far decimal points are spaced apart in the memo string

                ' Find first decimal point location
                Dim iFirst As Integer = strMemo.IndexOf(CChar("."))
                ' Find second decimal point location
                Dim iSecond As Integer = strMemo.IndexOf(CChar("."), iFirst + 1)
                ' Take calculated number string length if two decimal points found. If this fails,
                ' take the default number string length provided as a parameter
                Dim iNumLen As Integer = CInt(IIf(iFirst = -1 Or iSecond = -1, nDefaultNumberLen, iSecond - iFirst))
                ' Calculate the total of number strings in the memo string, rounded up
                Dim iNumBits As Integer = CInt(Math.Ceiling(strMemo.Length / iNumLen))

                ' Allocate space for all number strings
                ReDim astrMemoBits(iNumBits - 1)
                ' Extract 'em all
                For i As Integer = 0 To iNumBits - 1
                    astrMemoBits(i) = strMemo.Substring(i * iNumLen, Math.Min(strMemo.Length - i * iNumLen, iNumLen))
                Next
            Else
                ' #Separator character found: just split the memo string
                astrMemoBits = strMemo.Split(CChar(cSplitChar))
            End If

            ' Now remodel the memo string using real numbers
            For i As Integer = 0 To astrMemoBits.Length - 1
                Try
                    ' Convert number string into a real single
                    sValue = Single.Parse(astrMemoBits(i))
                Catch ex As Exception
                    ' Provide default in case of an exception
                    sValue = 0
                End Try
                astrMemoBits(i) = CStr(sValue)
            Next

            Return astrMemoBits
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, rebuilds a memo string that consists of a series of 
        ''' numbers.
        ''' </summary>
        ''' <param name="strMemo">Memo text to rebuild</param>
        ''' <param name="cSplitChar">Separator character that separates the 
        ''' numbers in the memo text.</param>
        ''' <param name="nDefaultNumberLen">When interpreting a string without
        ''' separators, this value indicates the number of characters that each
        ''' number occupies in the memo string.</param>
        ''' <param name="nRepetition">Optional field, indicating the number of
        ''' times a value for the source string should be repeated.</param>
        ''' <returns>A smaller string representing the same numbers.</returns>
        ''' -------------------------------------------------------------------
        Private Function RebuildNumberListString(ByVal strMemo As String, _
                Optional ByVal cSplitChar As Char = CChar(" "), _
                Optional ByVal nDefaultNumberLen As Integer = 7, _
                Optional ByVal nRepetition As Integer = 1) As String

            Dim astrMemoBits() As String
            Dim sb As New Text.StringBuilder

            If strMemo.Length = 0 Then
                Return strMemo
            End If

            astrMemoBits = Me.SplitNumberListString(strMemo, cSplitChar, nDefaultNumberLen)

            ' Now remodel the memo string using real numbers
            For i As Integer = 0 To astrMemoBits.Length - 1
                For j As Integer = 1 To nRepetition
                    ' Separate numbers with a single space
                    If sb.Length > 0 Then sb.Append(CChar(" "))
                    ' Add the number
                    sb.Append(astrMemoBits(i))
                Next
            Next
            ' There
            Return sb.ToString()
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; converts a string into a pattern of bit flags.
        ''' </summary>
        ''' <param name="strFlags">The string to convert into bit flags.</param>
        ''' <param name="strMatch">The character(s) to match in the string.</param>
        ''' <param name="bMatchAsOne">
        ''' <pata>Flag, indicates how a positive character match affects the bit pattern.</pata>
        ''' <list type="bullet">
        ''' <item><description>
        ''' When set to True, a positive character match generates a 1, and a negative character match generates a 0.
        ''' </description></item>
        ''' <item><description>
        ''' When set to False, a positive character match generates a 0, and a negative character match generates a 1.
        ''' </description></item>
        ''' </list>
        ''' </param>
        ''' <returns>A bit pattern of the provided string.</returns>
        ''' -------------------------------------------------------------------
        Private Function StringToBitFlags(ByVal strFlags As String, ByVal strMatch As String, Optional ByVal bMatchAsOne As Boolean = True) As Integer
            Dim iBitFlags As Integer = 0
            Dim iBit As Integer = 0
            Dim cTest As Char = Nothing

            ' Iterate through the characters in the string, starting at the least precision number (left-most value, highest number)
            ' all the way up to the right of the string, representing the highest precision number.
            For iBit = 0 To strFlags.Length - 1
                ' Shift pattern one bit to the left
                iBitFlags *= 2
                ' Get next bit char to test
                cTest = CChar(strFlags.Substring(iBit, 1))
                ' Is this a character from the match set?
                If (strMatch.IndexOf(cTest) >= 0) Then
                    ' #Yes: Add 1 or 0, depending on bMatchAsOne flag value
                    iBitFlags += CInt(IIf(bMatchAsOne, 1, 0))
                Else
                    ' #No: Add 0 or 1, depending on bMatchAsOne flag value
                    iBitFlags += CInt(IIf(bMatchAsOne, 0, 1))
                End If
            Next
            Return iBitFlags
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, generates a hash key for a data value, optionally 
        ''' in the context of a strong-typed scenario.
        ''' </summary>
        ''' <param name="strKey">The EwE5 value to generate the kay for.</param>
        ''' <param name="iScenarioID">Database ID of scenario this key belongs to, 
        ''' if any. No scenario filter is applied if this value is less than or equals to 0.</param>
        ''' <param name="dtScenario">Data type of this scenario, if any.</param>
        ''' -------------------------------------------------------------------
        Private Function MakeHashKey(ByVal strKey As String, ByVal iScenarioID As Integer, ByVal dtScenario As eDataTypes) As String
            If iScenarioID <= 0 Then
                Return strKey
            Else
                Return String.Format("{0}@{1}({2})", strKey, dtScenario.ToString, iScenarioID)
            End If
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a value in the primary keys hashtable.
        ''' </summary>
        ''' <param name="dt">The <see cref="eDataTypes">data type</see> to access
        ''' the key for.</param>
        ''' <param name="strKey">The EwE5 value to hash</param>
        ''' <param name="iScenarioID">Database ID of scenario this key belongs to, if any.</param>
        ''' <param name="dtScenario">Data type of this scenario, if any.</param>
        ''' <remarks>
        ''' <para>EwE5 identifies objects by name. EwE6 uses database IDs. The 
        ''' <see cref="m_adtKeys">Primary Key hashtable</see> maintains an 
        ''' administration of EwE5 to EwE6 key mappings during the import 
        ''' process.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Property HashKey(ByVal dt As eDataTypes, ByVal strKey As String, _
                Optional ByVal dtScenario As eDataTypes = eDataTypes.NotSet, Optional ByVal iScenarioID As Integer = 0) As Integer
            Get
                ' Get proper dictionary
                Dim dict As Dictionary(Of String, Integer) = m_adtKeys(CInt(dt))
                Dim strKeyInt As String = Me.MakeHashKey(strKey, iScenarioID, dtScenario)

                If (dict Is Nothing) Then
                    'Console.WriteLine("Dictionary not defined, no data imported for datatype {0} ({1})", dt.ToString, strKey)
                    Return 0
                End If

                If Not dict.ContainsKey(strKeyInt) Then
                    'Console.WriteLine("Failed to resolve datatype {0} ({1})", dt.ToString, strKeyInt)
                    Return 0
                End If

                ' Return the item, let this crash if item cannot be found
                Return dict.Item(strKeyInt)
            End Get
            Set(ByVal iValue As Integer)
                ' Get proper dictionary
                Dim dict As Dictionary(Of String, Integer) = m_adtKeys(CInt(dt))
                Dim strKeyInt As String = Me.MakeHashKey(strKey, iScenarioID, dtScenario)
                ' Already allocated?
                If (Object.ReferenceEquals(dict, Nothing)) Then
                    ' #No: create new
                    dict = New Dictionary(Of String, Integer)
                    ' Store dict
                    m_adtKeys(CInt(dt)) = dict
                End If
                ' Store the item, let this crash if the key already exists
                dict(strKeyInt) = iValue
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set a value in the primary keys hashtable.
        ''' </summary>
        ''' <param name="dt">The <see cref="eDataTypes">data type</see> to access
        ''' the key for.</param>
        ''' <para>EwE5 identifies objects by name. EwE6 uses database IDs. The 
        ''' <see cref="m_adtKeys">Primary Key hashtable</see> maintains an 
        ''' administration of EwE5 to EwE6 key mappings during the import 
        ''' process.</para>
        ''' -------------------------------------------------------------------
        Private Property PoolCodeID(ByVal dt As eDataTypes, ByVal iEwE5Index As Integer) As Integer
            Get
                ' Get proper dictionary
                Dim dict As Dictionary(Of Integer, Integer) = Me.m_adtIndexes(CInt(dt))

                If (dict Is Nothing) Then Return 0
                If (Not dict.ContainsKey(iEwE5Index)) Then Return 0
                ' Return the item, let this crash if item cannot be found
                Return dict.Item(iEwE5Index)
            End Get

            Set(ByVal iValue As Integer)
                ' Get proper dictionary
                Dim dict As Dictionary(Of Integer, Integer) = Me.m_adtIndexes(CInt(dt))
                ' Already allocated?
                If (Object.ReferenceEquals(dict, Nothing)) Then
                    ' #No: create new
                    dict = New Dictionary(Of Integer, Integer)
                    ' Store dict
                    Me.m_adtIndexes(CInt(dt)) = dict
                End If
                ' Store the item, let this crash if the key already exists
                dict(iValue) = iValue
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, prepares a database-derived value for use in EwE6. 
        ''' The following fixes are performed:
        ''' <list type="bullet">
        ''' <item><description>Strings are trimmed of leading and trailing whitespace.</description></item>
        ''' <item><description>Numeric values are translated from EwE5 Null values to <see cref="cCore.NULL_VALUE">EwE6 NULL</see>
        ''' values if this value is <paramref name="valDefault">allowed to be NULL</paramref>.</description></item>
        ''' </list>
        ''' </summary>
        ''' <param name="r">The <see cref="IDataReader">data reader</see> to obtain the value from.</param>
        ''' <param name="strField">Field in the reader to obtain the value from.</param>
        ''' <param name="valDefault">Optional default value to return if the value found in the reader is a DBNull value.</param>
        ''' <returns>The groomed and pruned value.</returns>
        ''' -------------------------------------------------------------------
        Private Function FixValue(ByRef r As IDataReader, ByVal strField As String, _
                Optional ByVal valDefault As Object = Nothing) As Object

            Dim value As Object = Nothing

            Try
                ' Try to get variable from DB
                value = r(strField)
            Catch ex As Exception
                ' Set to DBNull in case of an internal explosion
                value = Convert.DBNull
            End Try

            ' Value unknown?
            If Convert.IsDBNull(value) Then
                ' #Yes: is a default provided?
                If valDefault IsNot Nothing Then
                    ' #Yes: Set default
                    value = valDefault
                End If
                ' Return value
                Return value
            End If

            ' ================================== '
            ' Correct numerical EwE5 NULL values '
            ' ================================== '

            ' Is this a numerical value?
            If TypeOf (value) Is Integer Or TypeOf (value) Is Single Or TypeOf (value) Is Double Then
                ' #Yes: is this value an EWE5 NULL value?
                If (CDbl(valDefault) = cCore.NULL_VALUE) And (CDbl(value) <= cEWE5_NULL) Then
                    ' #Yes: translate to EwE6 NULL values
                    value = CInt(cCore.NULL_VALUE)
                End If
            End If

            ' Is a string value?
            If TypeOf (value) Is String Then
                ' #Yes: strip off white space
                value = DirectCast(value, String).Trim()
            End If

            Return value
        End Function

        Private Function ExtractLastSavedJulianDate(ByVal strDescription As String) As Single

            Dim strDate As String = ""
            Dim iLastSeparatorPos As Integer = -1

            If String.IsNullOrEmpty(strDescription) Then Return 0.0!

            iLastSeparatorPos = strDescription.LastIndexOf(";"c)
            If iLastSeparatorPos > -1 Then
                strDate = strDescription.Substring(iLastSeparatorPos + 1)
            Else
                iLastSeparatorPos = strDescription.IndexOf("Created:")
                If iLastSeparatorPos > -1 Then
                    strDate = strDescription.Substring(iLastSeparatorPos + "Created:".Length)
                Else
                    strDate = strDescription
                End If
            End If

            Try
                Return CSng(Date.Parse(strDate).ToOADate())
            Catch ex As Exception
                ' Woops!
            End Try
            Return 0.0!

        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Import generic model information
        ''' </summary>
        ''' <param name="strModelName">Name of the EwE5 model to import</param>
        ''' -------------------------------------------------------------------
        Private Sub ImportModels(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim bWithSeason As Boolean = False
            Dim strYear As String = ""
            Dim dt As DateTime = Nothing
            Dim strUnit As String = ""
            Dim unitCurrency As eUnitCurrencyType = 0
            Dim unitTime As eUnitTimeType = 0

            ' Clear table
            Me.m_dbEwE6.Execute("DELETE * FROM EcopathModel")

            reader = m_dbEwE5.GetReader(String.Format("SELECT * from [Models] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = m_dbEwE6.GetWriter("EcopathModel")

            reader.Read()
            drow = writer.NewRow()
            drow("ModelID") = 1
            drow("Name") = Me.FixValue(reader, "modelTitle")
            drow("Description") = Me.FixValue(reader, "remarks", "")
            drow("NumDigits") = Me.FixValue(reader, "numDigits")

            ' Translate Currency unit
            strUnit = CStr(Me.FixValue(reader, "currencyUnit", ""))
            unitCurrency = DirectCast(CInt(Me.FixValue(reader, "currencyIndex", CInt(eUnitCurrencyType.WetWeight))), eUnitCurrencyType)
            Select Case unitCurrency
                Case eUnitCurrencyType.NotSet
                    unitCurrency = eUnitCurrencyType.WetWeight
                Case eUnitCurrencyType.CustomEnergy, eUnitCurrencyType.CustomNutrient
                Case Else
                    strUnit = ""
            End Select
            drow("UnitCurrency") = CInt(unitCurrency)
            drow("UnitCurrencyCustom") = strUnit

            ' Translate Time unit
            strUnit = CStr(Me.FixValue(reader, "timeUnit", ""))
            unitTime = eUnitTimeType.Custom
            Select Case strUnit.Trim.ToLower()
                Case "year" : unitTime = eUnitTimeType.Year : strUnit = ""
                Case "day" : unitTime = eUnitTimeType.Day : strUnit = ""
                Case Else : unitTime = eUnitTimeType.Custom
            End Select
            drow("UnitTime") = CInt(unitTime)
            drow("UnitTimeCustom") = strUnit

            drow("LastSaved") = Me.ExtractLastSavedJulianDate(CStr(Me.FixValue(reader, "remarks", "")))

            ' ToDo_JS: ImportModels - check if bWithSeason is relevant in EwE6
            bWithSeason = CBool(reader("WithSeason"))
            drow("WithSeason") = bWithSeason

            If (bWithSeason) Then
                ' Convert Year1
                If Not Convert.IsDBNull(reader("Year1")) Then
                    strYear = CStr(reader("Year1"))
                    If strYear.Length > 7 Then
                        dt = New DateTime(CInt(strYear.Substring(0, 4)), CInt(strYear.Substring(5, 2)), CInt(strYear.Substring(7, 2)))
                        drow("DateStart") = dt
                        If (strYear.Length > 9) Then
                            ' ToDo_JS: ImportModels - check if no. of steps per year is relevant in EwE6
                            drow("StepsPerYear") = CInt(strYear.Substring(9))
                        End If
                    End If
                End If
                ' Convert Year2
                If Not Convert.IsDBNull(reader("Year2")) Then
                    strYear = CStr(reader("Year2"))
                    If strYear.Length > 7 Then
                        dt = New DateTime(CInt(strYear.Substring(0, 4)), CInt(strYear.Substring(5, 2)), CInt(strYear.Substring(7, 2)))
                        drow("DateEnd") = dt
                    End If
                End If
            End If

            drow("MonetaryUnit") = Me.FixValue(reader, "monetaryUnit", eUnitMonetaryType.EUR.ToString())
            drow("EcoSimVulMultAll") = Me.FixValue(reader, "EcoSim vulMultAll")
            writer.AddRow(drow)
            writer.Commit()

            Me.AddRemark(reader("remarksCyclePath"), eDataTypes.EwEModel, CInt(drow("ModelID")), eVarNameFlags.CyclePath)

            ' JS 061221: References do not need to be imported for now
            ' ImportRefCode("RefCode", "quickRef")
            ' ImportRefCode("RefCodeCyclePath", "quickRef")

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

#End Region ' Generic

#Region " Stanza "

        Private Sub ImportGroupStanza(ByVal strModelName As String)

            Dim readerStanza As IDataReader = Nothing
            Dim writerStanza As cEwEDatabase.cEwEDbWriter = Nothing
            Dim writerLifeStages As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim drowSelect() As DataRow = Nothing
            Dim strStanzaName As String = ""
            Dim iStanzaID As Integer = 0
            Dim iGroupID As Integer = 0
            Dim iSequence As Integer = 0

            ' Clear table(s)
            Me.m_dbEwE6.Execute("DELETE * FROM Stanza")

            readerStanza = m_dbEwE5.GetReader(String.Format("SELECT * from [Group Stanza] where modelName='{0}' ORDER BY StanzaName, Sequence ASC", strModelName))
            If Object.ReferenceEquals(readerStanza, Nothing) Then Return

            writerStanza = m_dbEwE6.GetWriter("Stanza")
            writerLifeStages = m_dbEwE6.GetWriter("StanzaLifeStage")

            While readerStanza.Read()

                strStanzaName = CStr(readerStanza("StanzaName"))
                iSequence = CInt(readerStanza("Sequence"))

                ' Need to define group first?
                If (Me.HashKey(eDataTypes.Stanza, strStanzaName) = 0) Then

                    ' This must be the first row in the stanza sequence
                    If (iSequence = 1) Then
                        ' Store stanza configuratio
                        iStanzaID += 1

                        ' Write stanza-wide settings
                        drow = writerStanza.NewRow()
                        drow("StanzaID") = iStanzaID
                        drow("StanzaName") = strStanzaName
                        drow("BABsplit") = Me.FixValue(readerStanza, "BABsplit")
                        drow("WmatWinf") = Me.FixValue(readerStanza, "WmatWinf")
                        drow("RecPower") = Me.FixValue(readerStanza, "RecPower")
                        drow("FixedFecundity") = Me.FixValue(readerStanza, "FixedFecundity")

                        ' JS 060615: EggProd shapes are now scenario dependent, handled in table EcosimStanzaShapes.
                        ' drow("EggProdShape") = Me.FixValue(reader("EggProdShape"))
                        ' JS 070328: HatchCode is now scenario dependent, handled in table EcosimStanzaShapes.
                        ' drow("HatchCode") = Me.FixValue(readerStanzaNames, "HatchCode")

                        ' JS 23apr07: Leading B and QB groups are calculated at runtime, no longer stored in DB
                        writerStanza.AddRow(drow)
                        writerStanza.Commit()

                        ' Remember stanza ID mapping
                        Me.HashKey(eDataTypes.Stanza, strStanzaName) = iStanzaID

                        Me.AddRemark(readerStanza("remarks"), eDataTypes.Stanza, iStanzaID, eVarNameFlags.Name)
                    Else
                        ' Import error: stanza config missing essential first stage
                        ' ToDo_JS: globalize this
                        Me.LogMessage(String.Format("Multi-stanza configuration {0} missing essential first life stage. This stanza configuration cannot be imported.", strStanzaName), _
                                eMessageType.DataImport, eMessageImportance.Warning, True)
                    End If
                End If

                ' Is Stanza configuration available?
                If (Me.HashKey(eDataTypes.Stanza, strStanzaName) = iStanzaID) Then

                    ' #Yes: define life stages
                    drow = writerLifeStages.NewRow()

                    ' Fix FK
                    iGroupID = Me.HashKey(eDataTypes.EcoPathGroupInput, CStr(readerStanza("groupName")))

                    drow("StanzaID") = iStanzaID
                    drow("GroupID") = iGroupID
                    drow("Sequence") = iSequence

                    ' Write per-group stanza settings
                    drow("AgeStart") = Me.FixValue(readerStanza, "ageStart")
                    drow("Mortality") = Me.FixValue(readerStanza, "Mortality")

                    ' vbK moved to groups
                    'drow("vbK") = Me.FixValue(readerStanza, "vbK", 0.3)

                    ' JS 060621: Removed unused fields
                    'drow("Loo") = Me.FixValue(reader,"Loo")
                    'drow("WtGrow") = Me.FixValue(reader,"WtGrow")
                    'drow("Prepo") = Me.FixValue(reader,"Prepo")
                    'drow("Spare") = Me.FixValue(reader,"spare")
                    'drow("FixAge") = Me.FixValue(reader,"FixAge")

                    writerLifeStages.AddRow(drow)

                End If

            End While

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writerLifeStages)
            Me.m_dbEwE6.ReleaseWriter(writerStanza)
            Me.m_dbEwE5.ReleaseReader(readerStanza)

        End Sub

#End Region ' Stanza

#Region " Ecopath "

        Private Sub ImportEcopathGroups(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim nGroupID As Integer = 1
            Dim sTemp As Single = 0.0
            Dim iTemp As Integer = 0
            Dim nSequence As Integer = 1 ' Renumber sequence field

            Dim nNumGroups As Integer = CInt(m_dbEwE5.GetValue(String.Format("SELECT COUNT(*) FROM [Group Info] WHERE modelName='{0}'", strModelName)))
            Dim nNumLiving As Integer = CInt(m_dbEwE5.GetValue(String.Format("SELECT COUNT(*) FROM [Group Info] WHERE modelName='{0}' AND (TYPE <= 1)", strModelName)))

            If (nNumGroups = nNumLiving) Then
                ' Need to murder one group?
            End If

            ' Clear table(s)
            Me.m_dbEwE6.Execute("DELETE * FROM EcopathGroup")

            reader = m_dbEwE5.GetReader(String.Format("SELECT * FROM [Group Info] WHERE modelName='{0}' ORDER BY Sequence ASC", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = m_dbEwE6.GetWriter("EcopathGroup")

            While reader.Read()

                drow = writer.NewRow()

                drow("GroupID") = nGroupID
                drow("GroupName") = Me.FixValue(reader, "groupName")
                drow("Sequence") = nSequence
                drow("Type") = Me.FixValue(reader, "Type")
                ' -- validate area --'
                sTemp = CSng(Me.FixValue(reader, "Area"))
                If (sTemp <= 0 Or sTemp > 1) Then
                    sTemp = 1.0
                    Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_FIX_GROUPAREA, _
                            CStr(reader("groupName")), _
                            sTemp), _
                            eMessageType.DataImport, eMessageImportance.Information, True)
                End If
                drow("Area") = sTemp
                ' -- end validate --'
                drow("EcoEfficiency") = Me.FixValue(reader, "EcoEfficiency", cCore.NULL_VALUE)
                drow("ProdBiom") = Me.FixValue(reader, "ProdBiom", cCore.NULL_VALUE)
                drow("ConsBiom") = Me.FixValue(reader, "ConsBiom", cCore.NULL_VALUE)
                drow("ProdCons") = Me.FixValue(reader, "ProdCons", cCore.NULL_VALUE)
                drow("Biomass") = Me.FixValue(reader, "Biomass", cCore.NULL_VALUE)
                drow("BiomAcc") = Me.FixValue(reader, "BiomAcc")
                drow("BiomAccRate") = Me.FixValue(reader, "BiomAccRate")
                ' -- validate unassim --'
                sTemp = CSng(Me.FixValue(reader, "Unassim"))
                If CInt(reader("Type")) = 1 Then
                    ' For producers set the GS to 0
                    sTemp = 0.0
                    Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_FIX_GROUPUNASSIM, _
                            CStr(reader("groupName")), _
                            sTemp), _
                            eMessageType.DataImport, eMessageImportance.Information, True)
                End If
                drow("Unassim") = CSng(IIf(sTemp > 1, sTemp / 100.0, sTemp))
                ' -- end validate --'
                drow("Unassim") = Me.FixValue(reader, "Unassim")
                drow("DtImports") = Me.FixValue(reader, "DtImports")
                drow("Export") = Me.FixValue(reader, "Export")
                drow("Catch") = Me.FixValue(reader, "Catch")
                drow("ImpVar") = Me.FixValue(reader, "ImpVar")
                drow("NonMarketValue") = Me.FixValue(reader, "Non-market value")
                drow("Immigration") = Me.FixValue(reader, "Immigration")
                drow("Emigration") = Me.FixValue(reader, "Emigration")
                drow("EmigRate") = Me.FixValue(reader, "EmigRate")
                drow("ProdResp") = Me.FixValue(reader, "ProdResp")
                drow("RespCons") = Me.FixValue(reader, "RespCons")
                drow("RespBiom") = Me.FixValue(reader, "RespBiom")
                drow("Consumption") = Me.FixValue(reader, "Consumption")
                ' -- validate respiration --'
                sTemp = CSng(reader("Respiration"))
                If CInt(reader("Type")) < 1 Then
                    sTemp = 0.0
                End If
                drow("Respiration") = sTemp
                ' -- end validate --'
                drow("Production") = Me.FixValue(reader, "Production")
                drow("Unassimilated") = Me.FixValue(reader, "Unassimilated")
                drow("GroupIsFish") = Me.FixValue(reader, "GroupIsFish")
                drow("GroupIsInvert") = Me.FixValue(reader, "GroupIsInvert")
                ' JS070412: Poolcolor converted to 8digit hexadecimal string
                iTemp = CInt(Me.FixValue(reader, "PoolColor", &HFF000000)) ' Solid black
                drow("Poolcolor") = String.Format("{0:x8}", iTemp)

                writer.AddRow(drow)
                ' Commit to allow FK in Remark
                writer.Commit()

                ' Remember group ID mapping
                Me.HashKey(eDataTypes.EcoPathGroupInput, CStr(reader("groupName"))) = nGroupID
                ' Remember group poolcode mapping
                PoolCodeID(eDataTypes.EcoPathGroupInput, nSequence) = nGroupID

                ' Import Remarks
                Me.AddRemark(reader("remarks"), eDataTypes.EcoPathGroupInput, nGroupID, eVarNameFlags.Name)
                Me.AddRemark(reader("Non-market remarks"), eDataTypes.EcoPathGroupInput, nGroupID, eVarNameFlags.NonMarketValue)
                Me.AddRemark(reader("Migration remarks"), eDataTypes.EcoPathGroupInput, nGroupID, eVarNameFlags.Immig)

                ' Import pedigree
                Me.AddPedigree(CInt(Me.FixValue(reader, "Pedigree1", cCore.NULL_VALUE)), eDataTypes.EcoPathGroupInput, nGroupID, eVarNameFlags.Biomass)
                Me.AddPedigree(CInt(Me.FixValue(reader, "Pedigree2", cCore.NULL_VALUE)), eDataTypes.EcoPathGroupInput, nGroupID, eVarNameFlags.PBInput)
                Me.AddPedigree(CInt(Me.FixValue(reader, "Pedigree3", cCore.NULL_VALUE)), eDataTypes.EcoPathGroupInput, nGroupID, eVarNameFlags.QBInput)
                Me.AddPedigree(CInt(Me.FixValue(reader, "Pedigree4", cCore.NULL_VALUE)), eDataTypes.EcoPathGroupInput, nGroupID, eVarNameFlags.DietComp)
                Me.AddPedigree(CInt(Me.FixValue(reader, "Pedigree5", cCore.NULL_VALUE)), eDataTypes.EcoPathGroupInput, nGroupID, eVarNameFlags.Landings)

                ' ToDo_JS: 18Jul08: we do not have alternate input yet in EwE6
                ' AddRemark(reader("Altinput remarks"), drow, "GroupID", ?)

                ' JS 061221: References do not need to be imported for now
                ' ImportRefCode("RefCode", "quickRef")
                ' ImportRefCode("Non-market RefCode", "Non-market quickRef")
                ' ImportRefCode("Migration RefCode", "Migration quickRef")
                ' ImportRefCode("Altinput RefCode", "Altinput quickRef")

                nGroupID += 1
                nSequence += 1

            End While

            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Private Sub ImportGroupxGroup(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim readerPred As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim nPreyID As Integer = 0
            Dim nPredatorID As Integer = 0
            Dim sValue As Single = 0.0

            reader = m_dbEwE5.GetReader(String.Format("SELECT * from [Group x Group] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = m_dbEwE6.GetWriter("EcopathDietComp")

            While reader.Read()

                drow = writer.NewRow()

                nPredatorID = Me.HashKey(eDataTypes.EcoPathGroupInput, CStr(reader("groupName")))
                nPreyID = Me.HashKey(eDataTypes.EcoPathGroupInput, CStr(reader("groupColName")))

                ' Establish foreign key relationships
                drow("PreyID") = nPreyID
                drow("PredID") = nPredatorID

                ' -- correct diet --'
                '041019VC: remove diets for producers w/o q/b
                ' If there should be any leftover diet for a producer then get rid of it
                sValue = CSng(Me.FixValue(reader, "diet"))
                ' Is a producer with no q/b? (carbon models can have this)
                readerPred = m_dbEwE6.GetReader(String.Format("SELECT Type, ConsBiom FROM EcopathGroup WHERE (GroupID={0})", nPredatorID))
                readerPred.Read()
                If CSng(readerPred("ConsBiom")) <= 0.0 And CSng(readerPred("Type")) = 1.0 Then
                    ' #Yes: set diet components to 0
                    sValue = 0.0
                End If
                Me.m_dbEwE6.ReleaseReader(readerPred)
                drow("Diet") = sValue
                ' -- end correct --'

                drow("DetritusFate") = Me.FixValue(reader, "detritus fate")
                drow("MTI") = Me.FixValue(reader, "MTI")
                drow("Electivity") = Me.FixValue(reader, "electivity")
                writer.AddRow(drow)

                ' Import remarks
                AddRemark(reader("remarksDiet"), eDataTypes.EcoPathGroupInput, nPredatorID, eVarNameFlags.DietComp, eDataTypes.EcoPathGroupInput, nPreyID)
                AddRemark(reader("remarksDF"), eDataTypes.EcoPathGroupInput, nPredatorID, eVarNameFlags.DiscardFate, eDataTypes.EcoPathGroupInput, nPreyID)

                ' JS 061221: References do not need to be imported for now
                ' ImportRefCode("RefCodeDiet", "quickRefDiet")
                ' ImportRefCode("RefCodeDF", "quickRefDF")

            End While

            ' writer.Commit()
            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Public Sub ImportGroupSize(ByVal strModelName As String)

            Dim strGroupName As String = ""
            Dim reader As IDataReader = Nothing
            Dim readerStanza As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim nGroupID As Integer = 0
            Dim drow As DataRow = Nothing
            Dim drowFK As DataRow = Nothing
            Dim drowSelect() As DataRow = Nothing
            Dim dt As DataTable = Nothing

            ' Merge EwE5 Group Size data with EwE6 GroupInfo
            reader = m_dbEwE5.GetReader(String.Format("SELECT * from [Group size] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = m_dbEwE6.GetWriter("EcopathGroup")

            dt = writer.GetDataTable()

            While reader.Read()

                strGroupName = CStr(reader("groupName"))
                ' Get EwE6 GroupID for this record
                nGroupID = Me.HashKey(eDataTypes.EcoPathGroupInput, strGroupName)
                ' Find row(s) in GroupInfo that correspond to this GroupID
                drowSelect = dt.Select(String.Format("GroupID={0}", nGroupID))

                If (drowSelect.Length = 1) Then

                    drow = drowSelect(0)
                    drow.BeginEdit()

                    ' Import overriding values
                    drow("AinLW") = Me.FixValue(reader, "AinLW")
                    drow("BinLW") = Me.FixValue(reader, "BinLW")
                    drow("Loo") = Me.FixValue(reader, "Loo")
                    drow("winf") = Me.FixValue(reader, "winf")
                    'drow("vbK") = Me.FixValue(reader, "vbK", 0.3)
                    drow("t0") = Me.FixValue(reader, "t0", -9999)
                    drow("Tcatch") = Me.FixValue(reader, "Tcatch")
                    drow("Tmax") = Me.FixValue(reader, "Tmax")

                    Try
                        readerStanza = Me.m_dbEwE5.GetReader(String.Format("SELECT vbK from [Group Stanza] where modelName='{0}' AND groupName='{1}'", strModelName, strGroupName))
                        readerStanza.Read()
                        ' If not a valid stanza group read vbK as 0
                        drow("vbK") = Me.FixValue(readerStanza, "vbK", 0.0)
                        Me.m_dbEwE5.ReleaseReader(readerStanza)
                    Catch ex As Exception
                        drow("vbK") = 0.0!
                    End Try

                    drow.EndEdit()

                End If
            End While

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Private Sub ImportGear(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iTemp As Integer = 0
            Dim nFleetID As Integer = 1
            Dim nSequence As Integer = 1 ' Renumber sequence field

            ' Clear table(s)
            Me.m_dbEwE6.Execute("DELETE * FROM EcopathFleet")

            reader = m_dbEwE5.GetReader(String.Format("SELECT * from [Gear] where modelName='{0}' ORDER BY Sequence ASC", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = m_dbEwE6.GetWriter("EcopathFleet")

            While reader.Read()

                drow = writer.NewRow()

                drow("FleetID") = nFleetID
                drow("FleetName") = Me.FixValue(reader, "gearName")
                drow("Sequence") = nSequence
                drow("FixedCost") = Me.FixValue(reader, "fixedCost")
                drow("VariableCost") = Me.FixValue(reader, "variableCost")
                drow("SailingCost") = Me.FixValue(reader, "SailingCost")
                drow("EPower") = Me.FixValue(reader, "EPower")
                drow("PCapBase") = Me.FixValue(reader, "PCapBase")
                drow("CapDepreciate") = Me.FixValue(reader, "CapDepreciate")
                drow("CapBaseGrowth") = Me.FixValue(reader, "CapBaseGrowth")
                ' JS070412: Poolcolor (was GearColor) converted to 8-digit hexadecimal string
                'iTemp = CInt(Me.FixValue(reader, "PoolColor", Me.FixValue(reader, "CapBaseGrowth", nSequence Mod 14)))
                'drow("PoolColor") = String.Format("{0:x8}", iTemp)

                writer.AddRow(drow)
                writer.Commit()

                ' Remember Fleet ID mapping
                Me.HashKey(eDataTypes.FleetInput, CStr(reader("gearName"))) = nFleetID
                ' Remember fleet poolcode mapping
                PoolCodeID(eDataTypes.FleetInput, nSequence) = nFleetID

                ' Map remarks
                Me.AddRemark(reader("remarksCost"), eDataTypes.FleetInput, nFleetID, eVarNameFlags.FixedCost)
                ' JS 060622: Fleet size remark tied to FleetInput since EwE6 has no FleetSize variable
                Me.AddRemark(reader("remarkFleetSize"), eDataTypes.FleetInput, nFleetID, eVarNameFlags.Name)

                ' JS 061221: References do not need to be imported for now
                ' ImportRefCode("RefCode", "quickRef")
                ' ImportRefCode("RefcodeFleetSize", "quickRef")

                nFleetID += 1
                nSequence += 1

            End While

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Private Sub ImportCatch(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim drowFK As DataRow = Nothing
            Dim nGroupID As Integer = 0
            Dim nFleetID As Integer = 0

            reader = m_dbEwE5.GetReader(String.Format("SELECT * from [Catch] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = m_dbEwE6.GetWriter("EcopathCatch")

            While reader.Read()

                drow = writer.NewRow()

                ' Get GroupID
                nGroupID = Me.HashKey(eDataTypes.EcoPathGroupInput, CStr(reader("groupName")))
                ' Get FleetID (a.k.a. Gear)
                nFleetID = Me.HashKey(eDataTypes.FleetInput, CStr(reader("gearName")))

                drow("GroupID") = nGroupID
                drow("FleetID") = nFleetID
                drow("Landing") = Me.FixValue(reader, "Landing")
                drow("Discards") = Me.FixValue(reader, "discards")
                drow("Price") = Me.FixValue(reader, "price")

                writer.AddRow(drow)
                writer.Commit()

                ' Map remarks
                Me.AddRemark(reader("remarksCatch"), eDataTypes.FleetInput, nFleetID, eVarNameFlags.Landings, eDataTypes.EcoPathGroupInput, nGroupID)
                Me.AddRemark(reader("remarksPrice"), eDataTypes.FleetInput, nFleetID, eVarNameFlags.OffVesselPrice, eDataTypes.EcoPathGroupInput, nGroupID)
                Me.AddRemark(reader("remarksDiscards"), eDataTypes.FleetInput, nFleetID, eVarNameFlags.Discards, eDataTypes.EcoPathGroupInput, nGroupID)

                ' JS 061221: References do not need to be imported for now
                'ImportRefCode("RefCodeCatch", "quickRefCatch")
                'ImportRefCode("RefCodeDiscards", "quickRefDiscards")
                'ImportRefCode("RefCodePrice", "quickRefPrice")

            End While

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Private Sub ImportCatchCodes(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim drowFK As DataRow = Nothing
            Dim nGroupID As Integer = 0

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [Catch Codes] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcopathCatchCode")

            While reader.Read()

                drow = writer.NewRow()

                ' Get GroupID
                nGroupID = Me.HashKey(eDataTypes.EcoPathGroupInput, CStr(reader("groupName")))

                drow("GroupID") = nGroupID
                drow("Code") = Me.FixValue(reader, "code")
                drow("Proportion") = Me.FixValue(reader, "proportion")
                writer.AddRow(drow)

            End While

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Private Sub ImportDiscardFate(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim nGroupID As Integer = 0
            Dim nFleetID As Integer = 0

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [Discard Fate] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcopathDiscardFate")

            While reader.Read()

                drow = writer.NewRow()

                ' Get GroupID
                nGroupID = Me.HashKey(eDataTypes.EcoPathGroupInput, CStr(reader("groupColName")))
                ' Get FleetID (EwE5: Gear)
                nFleetID = Me.HashKey(eDataTypes.FleetInput, CStr(reader("gearName")))

                drow("GroupID") = nGroupID
                drow("FleetID") = nFleetID
                drow("DiscardFate") = Me.FixValue(reader, "DiscardFate")

                writer.AddRow(drow)

                ' Map remarks
                Me.AddRemark(reader("remarks"), eDataTypes.FleetInput, nFleetID, eVarNameFlags.DiscardFate, eDataTypes.EcoPathGroupInput, nGroupID)

                ' JS 061221: References do not need to be imported for now
                'ImportRefCode("RefCode", "quickRefCatch")

            End While

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Private Sub ImportBasicRemarks(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim nGroupID As Integer = 0
            Dim varName As eVarNameFlags = eVarNameFlags.NotSet

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [BasicParam Remarks] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            While reader.Read()
                ' Get GroupID
                nGroupID = Me.HashKey(eDataTypes.EcoPathGroupInput, CStr(reader("groupName")))
                ' Translate col to varname
                Select Case CInt(reader("paramNum"))
                    Case 1 ' feeding type, not used anymore in EwE5
                    Case 2 : varName = eVarNameFlags.Area
                    Case 3 : varName = eVarNameFlags.BiomassAreaInput
                    Case 4 : varName = eVarNameFlags.PBInput
                    Case 5 : varName = eVarNameFlags.QBInput
                    Case 6 : varName = eVarNameFlags.EEInput
                    Case 7 : varName = eVarNameFlags.GEInput
                    Case 8 : varName = eVarNameFlags.BioAccum
                    Case 9 : varName = eVarNameFlags.GS
                    Case 10 : varName = eVarNameFlags.DetImp
                    Case Else : varName = eVarNameFlags.NotSet
                End Select

                If (varName <> eVarNameFlags.NotSet) And (nGroupID > 0) Then
                    Me.AddRemark(reader("remarks"), eDataTypes.EcoPathGroupInput, nGroupID, varName)
                End If

            End While

        End Sub

        Private Sub ImportPedigree(ByVal strModelName As String)

            Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim varName As eVarNameFlags = eVarNameFlags.NotSet
            Dim strDescription As String = ""
            Dim drow As DataRow = Nothing
            Dim iLevelID As Integer = 0

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [Pedigree]"))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("Pedigree", "Sequence")

            While reader.Read()
                ' Translate col to varname
                Select Case CInt(reader("Parameter"))
                    Case 1 : varName = eVarNameFlags.Biomass
                    Case 2 : varName = eVarNameFlags.PBInput
                    Case 3 : varName = eVarNameFlags.QBInput
                    Case 4 : varName = eVarNameFlags.DietComp
                    Case Else : varName = eVarNameFlags.NotSet
                End Select

                strDescription = CStr(Me.FixValue(reader, "Parameter description", ""))

                If (varName <> eVarNameFlags.NotSet) And (Not String.IsNullOrEmpty(strDescription)) Then

                    Try
                        drow = writer.NewRow()
                        drow("LevelID") = iLevelID
                        drow("VarName") = cin.GetVarName(varName)
                        drow("Sequence") = CInt(Me.FixValue(reader, "Option", 0))
                        drow("IndexValue") = CSng(Me.FixValue(reader, "Value", 0.0!))
                        drow("Confidence") = CInt(Me.FixValue(reader, "Var", 0))
                        drow("Description") = strDescription
                        writer.AddRow(drow)
                    Catch ex As Exception

                    End Try

                    iLevelID += 1

                End If

            End While

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

#End Region ' Ecopath

#Region " Ecoranger "

        Private Sub ImportEcoranger(ByVal strModelName As String)

            Dim reader As IDataReader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [EcoRanger] where modelName='{0}'", strModelName))
            If (reader Is Nothing) Then Return
            Me.LogMessage(My.Resources.CoreMessages.IMPORT_WARNING_ECORANGER, eMessageType.DataImport, eMessageImportance.Information, True)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

#End Region ' Ecoranger

#Region " EcoSim "

        Private Function ImportEcoSim(ByVal strModelName As String) As Boolean

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim nScenarioID As Integer = 1
            Dim bHasScenarios As Boolean = False

            ' Clear table(s)
            Me.m_dbEwE6.Execute("DELETE * FROM EcosimScenario")

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [EcoSim] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return False

            writer = Me.m_dbEwE6.GetWriter("EcosimScenario")

            While reader.Read()

                drow = writer.NewRow()

                drow("ScenarioID") = nScenarioID
                drow("ScenarioName") = Me.FixValue(reader, "Scenario")
                drow("Description") = Me.FixValue(reader, "remarks", "")
                ' drow("npairs") = Me.FixValue(reader,"npairs")                   ' DISCONTINUED
                ' drow("TimeStep") = Me.FixValue(reader,"TimeStep")               ' DISCONTINUED
                drow("TotalTime") = Me.FixValue(reader, "TotalTime")
                drow("StepSize") = Me.FixValue(reader, "StepSize")
                drow("EquilibriumStepSize") = Me.FixValue(reader, "EquilibriumStepSize")
                drow("EquilScaleMax") = Me.FixValue(reader, "EquilScaleMax")
                drow("sorwt") = Me.FixValue(reader, "sorwt")
                drow("SystemRecovery") = Me.FixValue(reader, "SystemRecovery")
                drow("Discount") = Me.FixValue(reader, "Discount")
                drow("NudgeStart") = Me.FixValue(reader, "NudgeStart")
                drow("NudgeEnd") = Me.FixValue(reader, "NudgeEnd")
                drow("NudgeFactor") = Me.FixValue(reader, "NudgeFactor")
                drow("UseNudge") = Me.FixValue(reader, "chkNudge")
                drow("DoInteg") = Me.FixValue(reader, "DoInteg")
                ' drow("fValuetoPerturb") = Me.FixValue(reader, "fValuetoPerturb") ' DISCONTINUED
                ' VERIFY_JS: 060530 not read but fixed at 9 in code!
                'drow("NMed") = Me.FixValue(reader, "NMed")                        ' DISCONTINUED
                ' VERIFY_JS: 060530 not read but fixed at 1200 in code!
                'drow("NMedPoints") = Me.FixValue(reader, "NMedPoints")            ' DISCONTINUED
                drow("NutBaseFreeProp") = Me.FixValue(reader, "NutBaseFreeProp")
                ' JS061218: NutForceNumber imported into NutForcingShapeID
                ' drow("NutForceNumber") = CInt(Me.FixValue(reader, "NutForceNumber", 0))
                drow("NutPBmax") = Me.FixValue(reader, "NutPBmax")
                drow("UseVarPQ") = Me.FixValue(reader, "UseVarPQ")
                drow("LastSaved") = Me.ExtractLastSavedJulianDate(CStr(Me.FixValue(reader, "remarks", "")))

                ' Nutrient forcing shape will be resolved when shapes are loaded
                writer.AddRow(drow)

                ' Remember scenario ID mapping
                Me.HashKey(eDataTypes.EcoSimScenario, CStr(reader("Scenario"))) = nScenarioID

                ' JS 061221: References do not need to be imported for now
                ' ImportRefCode("RefCode", "quickRef")

                bHasScenarios = True
                nScenarioID += 1

            End While

            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

            Return bHasScenarios

        End Function

        Private Sub ImportEcoSimN(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iScenarioID As Integer = 0
            Dim iGroupID As Integer = 1

            Dim dtEcosimScenarios As Dictionary(Of String, Integer) = Me.m_adtKeys(CInt(eDataTypes.EcoSimScenario))
            Dim strEcosimScenario As String = ""
            Dim dtEcopathGroups As Dictionary(Of String, Integer) = Me.m_adtKeys(CInt(eDataTypes.EcoPathGroupInput))
            Dim strEcopathGroup As String = ""
            ' Flag stating whether an ecosim group was found for a given ecopath group
            Dim bHasGroup As Boolean = False

            ' JS 070212: Every Ecopath group should have an Ecosim counterpart
            reader = Me.m_dbEwE6.GetReader(String.Format("SELECT * from EcopathGroup"))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcosimScenarioGroup")

            ' For every ecopath group...
            For Each strEcopathGroup In dtEcopathGroups.Keys

                ' and for every ecosim scenario
                For Each strEcosimScenario In dtEcosimScenarios.Keys

                    ' .. create a new ecosim group

                    ' Get scenario link
                    iScenarioID = Me.HashKey(eDataTypes.EcoSimScenario, strEcosimScenario)

                    ' Check if an ecosim group exists for this ecopath group, ecosim scenario combination
                    reader = m_dbEwE5.GetReader(String.Format("SELECT * from [EcoSim N] where modelName='{0}' AND Scenario='{1}' AND groupName='{2}'", _
                            strModelName, strEcosimScenario, strEcopathGroup))

                    bHasGroup = reader.Read()

                    ' Create a new row
                    drow = writer.NewRow()

                    ' Link to ecopath group
                    drow("EcopathGroupID") = Me.HashKey(eDataTypes.EcoPathGroupInput, strEcopathGroup)
                    ' Link to scenario
                    drow("ScenarioID") = iScenarioID
                    ' Set group ID
                    drow("GroupID") = iGroupID

                    ' Does this ecosim group exist in the EwE5 database?
                    If bHasGroup Then
                        ' #Yes: copy content
                        drow("Pbmaxs") = Me.FixValue(reader, "pbmaxs")
                        drow("FtimeMax") = Me.FixValue(reader, "FtimeMax")
                        drow("FtimeAdjust") = Me.FixValue(reader, "FtimeAdjust")
                        drow("MoPred") = Me.FixValue(reader, "MoPred")
                        drow("FishRateMax") = Me.FixValue(reader, "FishRateMax")
                        drow("Show") = True  'reader("ShowHide") ' Show all groups when importing
                        drow("RiskTime") = Me.FixValue(reader, "RiskTime")
                        drow("QmQo") = Me.FixValue(reader, "QmQo")
                        drow("CmCo") = Me.FixValue(reader, "CmCo")
                        drow("SwitchPower") = Me.FixValue(reader, "SwitchPower")
                        If (Me.m_dbEwE5.GetVersion() >= 1.725) Then
                            drow("SalOpt") = Me.FixValue(reader, "SalOpt", 35.0!)
                            drow("SdSalLeft") = Me.FixValue(reader, "SdSal", 1000.0!)
                            drow("SdSalRight") = Me.FixValue(reader, "SdSal", 1000.0!)
                        End If

                        ' No shape imported for this group yet?
                        If Me.HashKey(eDataTypes.FishMort, strEcopathGroup, eDataTypes.EcoSimScenario, iScenarioID) = 0 Then
                            ' Remember key for consecutive group instances in other scenarios
                            Me.HashKey(eDataTypes.FishMort, strEcopathGroup, eDataTypes.EcoSimScenario, iScenarioID) = Me.m_iNextShapeID
                            ' Import the shape
                            Me.ImportShape(iGroupID, Me.m_iNextShapeID, eDataTypes.FishMort, reader)
                            ' Assign shape
                            drow("FishMortShapeID") = Me.m_iNextShapeID
                            ' Next shape
                            Me.m_iNextShapeID += 1
                        Else
                            drow("FishMortShapeID") = Me.HashKey(eDataTypes.FishMort, strEcopathGroup, eDataTypes.EcoSimScenario, iScenarioID)
                        End If
                    Else
                        ' #No: the new group will get all default values
                        Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_FIX_CREATEECOSIMGROUP, _
                                iGroupID, _
                                strEcopathGroup, _
                                strEcosimScenario), _
                                eMessageType.DataImport, eMessageImportance.Information)

                        ' No shape imported for this group yet?
                        If Me.HashKey(eDataTypes.FishMort, strEcopathGroup, eDataTypes.EcoSimScenario, iScenarioID) <> 0 Then
                            drow("FishMortShapeID") = Me.HashKey(eDataTypes.FishMort, strEcopathGroup, eDataTypes.EcoSimScenario, iScenarioID)
                        Else
                            ' Notify world
                            Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_FIX_CREATEFISHMORTSHAPE, _
                                    Me.m_iNextShapeID, _
                                    strEcopathGroup, _
                                    strEcosimScenario), _
                                    eMessageType.DataImport, eMessageImportance.Information)

                            ' Create dummy shape
                            Me.CreateDummyShape(Me.m_iNextShapeID, eDataTypes.FishMort)
                            ' Assign shape
                            drow("FishMortShapeID") = Me.m_iNextShapeID
                            ' Next shape
                            Me.m_iNextShapeID += 1
                        End If

                        ' Populate group fields
                        drow("FishMortShapeID") = 0
                    End If

                    ' Add the row
                    writer.AddRow(drow)
                    ' Commit to row to allow FK links from Remarks
                    writer.Commit()

                    If (bHasGroup) Then
                        ' Import remarks
                        Me.AddRemark(reader("remarks"), eDataTypes.EcoSimScenario, iScenarioID, eVarNameFlags.Name, eDataTypes.EcoSimGroupInput, iGroupID)

                        ' JS 061221: References do not need to be imported for now
                        ' ImportRefCode("RefCode", "quickRef")
                    End If

                    ' Remember Ecosim group input ID mapping
                    Me.HashKey(eDataTypes.EcoSimGroupInput, strEcopathGroup, eDataTypes.EcoSimScenario, iScenarioID) = iGroupID

                    ' Next group
                    iGroupID += 1
                    Me.m_dbEwE5.ReleaseReader(reader)

                Next strEcosimScenario
            Next strEcopathGroup

            Me.m_dbEwE6.ReleaseWriter(writer)

        End Sub

        Private Sub ImportEcoSimPairs(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim readerTmp As IDataReader = Nothing
            Dim strAdult As String = ""
            Dim strJuvinile As String = ""
            Dim bWarned As Boolean = False

            reader = m_dbEwE5.GetReader(String.Format("SELECT * from [EcoSim Pairs] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            While reader.Read

                If Not bWarned Then
                    Me.LogMessage(My.Resources.CoreMessages.IMPORT_WARNING_PAIRSNOTSUPPORTED, eMessageType.DataImport, eMessageImportance.Information, True)
                    bWarned = True
                End If

                Try
                    readerTmp = m_dbEwE5.GetReader(String.Format("SELECT * from [Group Info] where modelName='{0}' and sequence={1}", strModelName, reader("iadult")))
                    readerTmp.Read()
                    strAdult = CStr(readerTmp("groupName"))
                    Me.m_dbEwE5.ReleaseReader(readerTmp)

                    readerTmp = m_dbEwE5.GetReader(String.Format("SELECT * from [Group Info] where modelName='{0}' and sequence={1}", strModelName, reader("ijuv")))
                    readerTmp.Read()
                    strJuvinile = CStr(readerTmp("groupName"))
                    Me.m_dbEwE5.ReleaseReader(readerTmp)

                    Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_WARNING_PAIRDETAILS, _
                            CStr(reader("npairs")), _
                            strJuvinile, _
                            strAdult), _
                            eMessageType.DataImport, eMessageImportance.Information, True)

                Catch ex As Exception

                End Try

            End While

            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Private Sub ImportEcoSimFishGear(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iScenarioID As Integer = 0
            Dim iFleetID As Integer = 1
            Dim iEcopathFleetID As Integer = 0
            Dim iShapeID As Integer = 0

            reader = m_dbEwE5.GetReader(String.Format("SELECT * from [EcoSim FishGear] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcosimScenarioFleet")

            ' In EwE5, FishRateGear shapes are scenario-specific, at least they're embedded in
            ' table [EcoSim FishGear] but seem identical for every occurence of the ecosim fleet.

            ' Here, the shapes will be stored as one per fleet across scenarios.

            While reader.Read()

                drow = writer.NewRow()

                ' Map foreign keys
                iScenarioID = Me.HashKey(eDataTypes.EcoSimScenario, CStr(reader("Scenario")))
                iEcopathFleetID = Me.HashKey(eDataTypes.FleetInput, CStr(reader("gearName")))

                ' Check if shape already imported
                iShapeID = Me.HashKey(eDataTypes.FishingEffort, CStr(reader("gearName")), eDataTypes.EcoSimScenario, iScenarioID)
                ' Not imported yet? Signal that import is needed after the fleet has been defined
                If iShapeID = 0 Then iShapeID = Me.m_iNextShapeID

                drow("ScenarioID") = iScenarioID
                'drow("FleetID") = iFleetID
                drow("EcopathFleetID") = iEcopathFleetID
                drow("FishRateShapeID") = iShapeID

                iFleetID += 1

                writer.AddRow(drow)
                writer.Commit()

                If iShapeID = Me.m_iNextShapeID Then
                    Me.ImportShape(iEcopathFleetID, iShapeID, eDataTypes.FishingEffort, reader)
                    Me.HashKey(eDataTypes.FishingEffort, CStr(reader("gearName")), eDataTypes.EcoSimScenario, iScenarioID) = iShapeID
                    Me.m_iNextShapeID += 1
                End If

            End While

            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

#Region " Forcing shapes "

        Private Sub ImportEcoSimnShapes(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim readerSub As IDataReader = Nothing
            Dim writerSub As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drowSub As DataRow = Nothing
            Dim drowSelect() As DataRow = Nothing
            ' EwE5: Shape number implicitly identifies a shape type by comparing its value to predefined value ranges
            Dim iShapeNumber As Integer = 0
            ' EwE6: Shape type explicitly identifies a shape type
            Dim shapeDataType As eDataTypes = eDataTypes.NotSet
            Dim bIsEggShape As Boolean = False
            Dim bIsTimeShape As Boolean = False
            Dim bIsSeasonal As Boolean = False
            Dim iScenarioID As Integer = 0

            ' JS061218: A great performance boost can be achieved by opening all shape writers here rather than
            '           once for every shape. These writers can be made global to the class, similar to the 
            '           remarks writer, or can have local scope, to be passed on to the writing methods.

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * FROM [Ecosim nshapes] WHERE modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcosimShape")

            While reader.Read()

                ' Get shape number
                iShapeNumber = CInt(reader("nShapeNumber"))
                ' Get scenario number
                iScenarioID = Me.HashKey(eDataTypes.EcoSimScenario, CStr(reader("Scenario")))
                ' Reset seasonal flag
                bIsSeasonal = False

                ' Determine shape type from shape number
                shapeDataType = eDataTypes.NotSet

                Select Case iShapeNumber

                    Case 1 To 99

                        ' EwE5 stores seasonal shapes as IDs 1..3
                        bIsSeasonal = (iShapeNumber <= 3)

                        ' Time and/or egg?
                        bIsEggShape = Me.IsUsedAsEggShape(strModelName, iShapeNumber)
                        bIsTimeShape = Me.IsUsedAsTimeShape(strModelName, iShapeNumber)

                        ' The Eggs win
                        If (bIsTimeShape) Then shapeDataType = eDataTypes.Forcing
                        If (bIsEggShape) Then shapeDataType = eDataTypes.EggProd

                        ' If shape type undetermined (e.g. not allocated, only defined), import as time forcing function
                        ' VERIFY_JS: Check with VC how to import Forcing shapes that are defined but not used in scenarios.
                        '            For now, unused shapes are assigned as Time shapes.
                        If shapeDataType = eDataTypes.NotSet Then shapeDataType = eDataTypes.Forcing

                        ' Found dual assignment?
                        If (bIsEggShape And bIsTimeShape) Then
                            ' VERIFY_JS: Check with VC how to import dual assigned Forcing shapes (egg and time)
                            Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_WARNING_FORCINGMULTIPLEASSIGNMENTS, _
                                    iShapeNumber, _
                                    shapeDataType.ToString()), _
                                    eMessageType.DataImport, eMessageImportance.Information)
                        End If

                    Case 100 To Integer.MaxValue
                        shapeDataType = eDataTypes.Mediation

                    Case Else
                        ' Do not use this shape
                        shapeDataType = eDataTypes.NotSet

                End Select

                ' Is valid shapetype
                If (shapeDataType <> eDataTypes.NotSet) Then
                    ' #Yes: is not imported yet?
                    If (Me.HashKey(shapeDataType, CStr(iShapeNumber), eDataTypes.EcoSimScenario, iScenarioID) = 0) Then
                        ' #Yes: import succesful?
                        If (Me.ImportShape(iShapeNumber, Me.m_iNextShapeID, shapeDataType, reader, bIsSeasonal)) Then
                            ' Store key
                            Me.HashKey(shapeDataType, CStr(iShapeNumber), eDataTypes.EcoSimScenario, iScenarioID) = Me.m_iNextShapeID
                            ' Next
                            Me.m_iNextShapeID += 1
                        Else
                            ' Failed to import shape
                            Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_WARNING_FORCINGNOTIMPORTED, iShapeNumber, shapeDataType.ToString), _
                                    eMessageType.DataImport, eMessageImportance.Information)
                        End If
                    Else
                        ' This indicates an internal error
                        Debug.Assert(False)
                        Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_WARNING_FORCINGDUPLICATE, _
                                iShapeNumber, _
                                shapeDataType.ToString()), _
                                eMessageType.DataImport, eMessageImportance.Information)

                    End If ' Valid ShapeType is set
                Else
                    ' Invalid shape number
                    Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_WARNING_FORCINGTYPEMISSING, _
                            iShapeNumber), _
                            eMessageType.DataImport, eMessageImportance.Information)
                End If ' Not imported yet
            End While

            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

            Me.AssignEcosimScenarioForcingShapes(strModelName)
            Me.AssignStanzaShapes(strModelName)

        End Sub

        Private Function ImportShape(ByVal iShapeNumber As Integer, ByVal iShapeID As Integer, ByVal shapeDataType As eDataTypes, _
                ByVal reader As IDataReader, Optional ByVal bIsSeasonal As Boolean = False) As Boolean

            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim strZScale As String = ""
            Dim strTitle As String = ""
            Dim strType As String = ""
            Dim bSucces As Boolean = True

            Try
                writer = Me.m_dbEwE6.GetWriter("EcosimShape")

                ' Add new shape
                drow = writer.NewRow()
                drow("ShapeID") = iShapeID
                ' ShapeNumber is no longer stored; determined at load
                ' drow("nShapeNumber") = nShapeNumber 
                drow("ShapeType") = CInt(shapeDataType)
                drow("IsSeasonal") = bIsSeasonal
                writer.AddRow(drow)

                Me.m_dbEwE6.ReleaseWriter(writer)
                writer = Nothing

            Catch ex As Exception
                ' No need to localize, send to log only
                Me.LogMessage(String.Format("Forcing data {0} failed to import as type {1}: {2}", iShapeNumber, shapeDataType.ToString(), ex.Message), _
                        eMessageType.DataImport, eMessageImportance.Information)
                Return False
            End Try

            ' import shape specific data in subtable
            Select Case shapeDataType
                Case eDataTypes.Forcing
                    writer = Me.m_dbEwE6.GetWriter("EcoSimShapeTime")
                    drow = writer.NewRow()
                    drow("zMaxScale") = Me.FixValue(reader, "zMaxScale")

                    Me.SplitZScale(CStr(reader("zScale")), strZScale, strType, strTitle)
                    drow("Title") = strTitle
                    drow("zScale") = Me.RebuildNumberListString(strZScale)
                    drow("Yzero") = Me.FixValue(reader, "Yzero")
                    drow("Ybase") = Me.FixValue(reader, "Ybase")
                    drow("Yend") = Me.FixValue(reader, "Yend")
                    drow("Steep") = Me.FixValue(reader, "Steep")
                    ' New in EwE6
                    drow("FunctionType") = eShapeFunctionType.NotSet

                Case eDataTypes.EggProd
                    writer = Me.m_dbEwE6.GetWriter("EcosimShapeEggProd")
                    drow = writer.NewRow()
                    drow("zMaxScale") = Me.FixValue(reader, "zMaxScale")

                    Me.SplitZScale(CStr(reader("zScale")), strZScale, strType, strTitle)
                    drow("Title") = strTitle
                    drow("zScale") = Me.RebuildNumberListString(strZScale)
                    drow("Yzero") = Me.FixValue(reader, "Yzero")
                    drow("Ybase") = Me.FixValue(reader, "Ybase")
                    drow("Yend") = Me.FixValue(reader, "Yend")
                    drow("Steep") = Me.FixValue(reader, "Steep")
                    ' New in EwE6
                    drow("FunctionType") = eShapeFunctionType.NotSet

                Case eDataTypes.Mediation
                    Dim nShapeNumber As Integer = CInt(Me.m_dbEwE6.GetValue("SELECT COUNT(*) FROM EcosimShapeMediation"))

                    writer = Me.m_dbEwE6.GetWriter("EcosimShapeMediation")
                    drow = writer.NewRow()
                    drow("IMedBase") = Me.FixValue(reader, "XBaseLine")
                    drow("zScale") = Me.RebuildNumberListString(CStr(Me.FixValue(reader, "zScale", "")))
                    ' New in EwE6
                    drow("Title") = String.Format(My.Resources.CoreDefaults.CORE_DEFAULT_MEDIATIONSHAPE, CInt(nShapeNumber + 1))
                    drow("Yzero") = Me.FixValue(reader, "Yzero")
                    drow("Ybase") = Me.FixValue(reader, "Ybase")
                    drow("Yend") = Me.FixValue(reader, "Yend")
                    drow("Steep") = Me.FixValue(reader, "Steep")
                    ' New in EwE6
                    drow("FunctionType") = eShapeFunctionType.NotSet

                Case eDataTypes.FishingEffort
                    Dim nShapeNumber As Integer = CInt(Me.m_dbEwE6.GetValue("SELECT COUNT(*) FROM EcosimShapeFishRate"))

                    writer = Me.m_dbEwE6.GetWriter("EcosimShapeFishRate")
                    drow = writer.NewRow()
                    drow("Title") = String.Format(My.Resources.CoreDefaults.CORE_DEFAULT_FISHRATESHAPE, CInt(nShapeNumber + 1))
                    drow("zScale") = Me.RebuildNumberListString(CStr(Me.FixValue(reader, "FishRateGear", "")))

                Case eDataTypes.FishMort
                    Dim nShapeNumber As Integer = CInt(Me.m_dbEwE6.GetValue("SELECT COUNT(*) FROM EcosimShapeFishMort"))

                    writer = Me.m_dbEwE6.GetWriter("EcosimShapeFishMort")
                    drow = writer.NewRow()
                    drow("Title") = String.Format(My.Resources.CoreDefaults.CORE_DEFAULT_FISHMORTSHAPE, CInt(nShapeNumber + 1))
                    drow("zScale") = Me.RebuildNumberListString(CStr(Me.FixValue(reader, "FishRateNo", "")))

                Case Else
                    Debug.Assert(False, "Shape type not set during import; cannot continue")

            End Select

            ' Forge FK
            drow("ShapeID") = iShapeID

            writer.AddRow(drow)
            Me.m_dbEwE6.ReleaseWriter(writer)
            writer = Nothing

            Return bSucces
        End Function

        Private Function CreateDummyShape(ByVal iShapeID As Integer, ByVal shapeDataType As eDataTypes, _
                Optional ByVal bIsSeasonal As Boolean = False) As Boolean

            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim strZScale As String = ""
            Dim strTitle As String = ""
            Dim strType As String = ""
            Dim bSucces As Boolean = True

            Try
                writer = Me.m_dbEwE6.GetWriter("EcosimShape")

                ' Add new shape
                drow = writer.NewRow()
                drow("ShapeID") = iShapeID
                ' ShapeNumber is no longer stored; determined at load
                ' drow("nShapeNumber") = nShapeNumber 
                drow("ShapeType") = CInt(shapeDataType)
                drow("IsSeasonal") = bIsSeasonal
                writer.AddRow(drow)

                Me.m_dbEwE6.ReleaseWriter(writer)
                writer = Nothing

            Catch ex As Exception
                ' No need to localize, send to log only
                Me.LogMessage(String.Format("Failed to create dummy shape {0}: {1}", iShapeID, ex.Message), _
                        eMessageType.DataImport, eMessageImportance.Information)
                Return False
            End Try

            ' import shape specific data in subtable
            Select Case shapeDataType
                Case eDataTypes.Forcing
                    writer = Me.m_dbEwE6.GetWriter("EcoSimShapeTime")
                    drow = writer.NewRow()
                    drow("FunctionType") = eShapeFunctionType.NotSet

                Case eDataTypes.EggProd
                    writer = Me.m_dbEwE6.GetWriter("EcosimShapeEggProd")
                    drow = writer.NewRow()
                    drow("Title") = strTitle
                    ' New in EwE6
                    drow("FunctionType") = eShapeFunctionType.NotSet

                Case eDataTypes.Mediation
                    Dim nShapeNumber As Integer = CInt(Me.m_dbEwE6.GetValue("SELECT COUNT(*) FROM EcosimShapeMediation"))

                    writer = Me.m_dbEwE6.GetWriter("EcosimShapeMediation")
                    drow = writer.NewRow()
                    ' New in EwE6
                    drow("Title") = String.Format(My.Resources.CoreDefaults.CORE_DEFAULT_MEDIATIONSHAPE, CInt(nShapeNumber + 1))
                    ' New in EwE6
                    drow("FunctionType") = eShapeFunctionType.NotSet

                Case eDataTypes.FishingEffort
                    Dim nShapeNumber As Integer = CInt(Me.m_dbEwE6.GetValue("SELECT COUNT(*) FROM EcosimShapeFishRate"))

                    writer = Me.m_dbEwE6.GetWriter("EcosimShapeFishRate")
                    drow = writer.NewRow()
                    drow("Title") = String.Format(My.Resources.CoreDefaults.CORE_DEFAULT_FISHRATESHAPE, CInt(nShapeNumber + 1))

                Case eDataTypes.FishMort
                    Dim nShapeNumber As Integer = CInt(Me.m_dbEwE6.GetValue("SELECT COUNT(*) FROM EcosimShapeFishMort"))

                    writer = Me.m_dbEwE6.GetWriter("EcosimShapeFishMort")
                    drow = writer.NewRow()
                    drow("Title") = String.Format(My.Resources.CoreDefaults.CORE_DEFAULT_FISHMORTSHAPE, CInt(nShapeNumber + 1))

                Case Else
                    Debug.Assert(False, "Shape type not set during import; cannot continue")

            End Select

            ' Forge FK
            drow("ShapeID") = iShapeID

            writer.AddRow(drow)
            Me.m_dbEwE6.ReleaseWriter(writer)
            writer = Nothing

            Return bSucces
        End Function

        Private Sub AssignEcosimScenarioForcingShapes(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim dt As DataTable = Nothing
            Dim iScenarioID As Integer = 0
            Dim iNutShapeNumber As Integer = 0
            Dim iNutShapeID As Integer = 0
            Dim iSalinityShapeNumber As Integer = 0
            Dim iSalinityShapeID As Integer = 0
            Dim drowSelect() As DataRow = Nothing
            Dim drow As DataRow = Nothing

            ' Resolve Scenario dependent forcing shapes
            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * FROM [EcoSim] WHERE modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcosimScenario")

            While reader.Read()

                iScenarioID = HashKey(eDataTypes.EcoSimScenario, CStr(reader("Scenario")))
                iNutShapeID = 0
                iSalinityShapeID = 0

                iNutShapeNumber = CInt(Me.FixValue(reader, "NutForceNumber", 0))
                If Me.m_dbEwE5.GetVersion >= 1.725 Then
                    iSalinityShapeNumber = CInt(Me.FixValue(reader, "NutForceNumber", 0))
                End If

                ' Resolve shape IDs
                If (iNutShapeNumber > 0) Then
                    iNutShapeID = Me.HashKey(eDataTypes.Forcing, CStr(iNutShapeNumber), eDataTypes.EcoSimScenario, iScenarioID)
                End If
                If (iSalinityShapeNumber > 0) Then
                    iSalinityShapeID = Me.HashKey(eDataTypes.Forcing, CStr(iSalinityShapeNumber), eDataTypes.EcoSimScenario, iScenarioID)
                End If

                ' Are there shapes to assign?
                If ((iNutShapeID + iSalinityShapeID) > 0) Then
                    ' #Yes: venture yonder, Jimmy
                    dt = writer.GetDataTable()
                    drowSelect = dt.Select(String.Format("ScenarioID={0}", iScenarioID))
                    If (drowSelect.Length = 1) Then
                        ' Sanity check
                        drow = drowSelect(0)
                        drow.BeginEdit()
                        drow("NutForcingShapeID") = iNutShapeID
                        drow("SalinityForcingShapeID") = iSalinityShapeID
                        drow.EndEdit()
                    End If
                End If

            End While
            Me.m_dbEwE5.ReleaseReader(reader)
            Me.m_dbEwE6.ReleaseWriter(writer)
        End Sub

        Private Sub AssignStanzaShapes(ByVal strModelName As String)

            Dim strEcosimScenario As String = ""
            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim iEggShape As Integer = 0
            Dim iEggShapeID As Integer = 0
            Dim iHatchShape As Integer = 0
            Dim iHatchShapeID As Integer = 0
            Dim dtEcosimScenarios As Dictionary(Of String, Integer) = Me.m_adtKeys(CInt(eDataTypes.EcoSimScenario))
            Dim iNumEcosimScenarios As Integer = 0
            Dim iEcosimScenarioID As Integer = 0
            Dim drow As DataRow = Nothing

            Me.m_dbEwE6.Execute("DELETE * FROM EcosimStanzaShape")

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT groupName, stanzaName, EggProdShape, HatchCode FROM [Group Stanza] WHERE (modelName='{0}')", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            ' Determine number of ecosim scenarios
            iNumEcosimScenarios = dtEcosimScenarios.Values.Count

            writer = Me.m_dbEwE6.GetWriter("EcosimStanzaShape")
            Try
                While reader.Read()
                    iEggShape = CInt(Me.FixValue(reader, "EggProdShape", 0))
                    iHatchShape = CInt(Me.FixValue(reader, "HatchCode", 0))

                    ' Has shape assignments?
                    If (iEggShape + iHatchShape > 0) Then

                        ' JS 24nov07: EwE5 links stanza configs (non-sim scenario dept) to shapes (ecosim secnario dept) via an index that is only
                        '             meaningful from the context of a loaded scenario. EwE6 instead loads shapes ecosim scenario independent.
                        '             Therefore, the importer will be unable to import this link when importing more than one Ecosim scenario: 
                        '             What EwE5 scenario should this shape come from?!

                        ' Has only one Ecosim scenario?
                        If (iNumEcosimScenarios = 1) Then
                            ' Ugh, get one and only scenario ID
                            For Each iEcosimScenarioID In dtEcosimScenarios.Values : Next

                            ' Try to resolve egg prod shape ID for this scenario
                            If iEggShape > 0 Then iEggShapeID = Me.HashKey(eDataTypes.EggProd, CStr(iEggShape), eDataTypes.EcoSimScenario, iEcosimScenarioID)
                            ' Try to resolve forcing shape ID for this scenario
                            If iHatchShape > 0 Then iHatchShapeID = Me.HashKey(eDataTypes.Forcing, CStr(iHatchShape), eDataTypes.EcoSimScenario, iEcosimScenarioID)
                            If (iEggShapeID + iHatchShapeID) > 0 Then
                                drow = writer.NewRow()
                                ' Map foreign keys
                                drow("StanzaID") = HashKey(eDataTypes.Stanza, CStr(reader("stanzaName")))
                                ' Link shapes
                                If (iEggShapeID > 0) Then drow("EggprodShapeID") = iEggShapeID
                                If (iHatchShapeID > 0) Then drow("HatchCodeShapeID") = iHatchShapeID
                                writer.AddRow(drow)
                            End If
                        Else
                            ' Multiple ecosim scenarios: do not import, throw a warning
                            Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_WARNING_MULTISTANZASHAPE, CStr(reader("stanzaName"))), _
                                        eMessageType.DataImport, eMessageImportance.Information)
                        End If
                    End If

                End While
            Catch ex As Exception
            End Try

            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method; splits EwE5 zscale memo into a title and zscale parts.
        ''' </summary>
        ''' <param name="strIn">The string to split.</param>
        ''' <param name="strZScale">Zscale part.</param>
        ''' <param name="strTitle">Title part.</param>
        ''' -------------------------------------------------------------------
        Private Sub SplitZScale(ByVal strIn As String, ByRef strZScale As String, ByRef strType As String, ByRef strTitle As String)
            ' Separate title from Zscale data. EwE5 stores the title in the first 
            ' 20 characters of the ZScale data.
            strType = strIn.Substring(0, 1)
            strTitle = strIn.Substring(1, 19)
            strZScale = strIn.Substring(21)
        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, determines if a given shape number is used as a Time 
        ''' forcing shape.
        ''' </summary>
        ''' <param name="strModelName">The model to check.</param>
        ''' <param name="nShapeNumber">The shape number to check.</param>
        ''' <returns>
        ''' True if the given shape number, for both the given model and scenario,
        ''' is used as a Time forcing shape.</returns>
        ''' -------------------------------------------------------------------
        Private Function IsUsedAsTimeShape(ByVal strModelName As String, ByVal nShapeNumber As Integer) As Boolean
            If (Me.m_dbEwE5.GetVersion < 1.705) Then
                Dim strDetectEggSQL As String = "SELECT COUNT(*) FROM [ECOSIM NXN] WHERE (modelName='{0}') AND (seasonType={1})"
                Return CInt(Me.m_dbEwE5.GetValue(String.Format(strDetectEggSQL, strModelName, nShapeNumber))) > 0
            Else
                Dim strDetectEggSQL As String = "SELECT COUNT(*) FROM [ECOSIM NXN Forcing] WHERE (modelName='{0}') AND (FunctionNumber={1}) AND (IsMedFunction=False)"
                Return CInt(Me.m_dbEwE5.GetValue(String.Format(strDetectEggSQL, strModelName, nShapeNumber))) > 0
            End If
        End Function

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Helper method, determines if a given shape number is used as an
        ''' Egg Production forcing shape.
        ''' </summary>
        ''' <param name="strModelName">The model to check.</param>
        ''' <param name="nShapeNumber">The shape number to check.</param>
        ''' <returns>
        ''' True if the given shape number for the given model is used 
        ''' as an Egg production forcing shape.</returns>
        ''' -------------------------------------------------------------------
        Private Function IsUsedAsEggShape(ByVal strModelName As String, ByVal nShapeNumber As Integer) As Boolean
            ' EggShapes in EwE5 are assigned to stanza groups independent of scenario!
            Dim strDetectEggSQL As String = "SELECT COUNT(*) FROM [GROUP STANZA] WHERE (modelName='{0}') AND (EggProdShape={1})"
            Return CInt(Me.m_dbEwE5.GetValue(String.Format(strDetectEggSQL, strModelName, nShapeNumber))) > 0
        End Function

        Private Sub ImportEcoSimNxN(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim sVul As Single = 0.0!
            Dim sDBVersion As Single = 0.0!
            Dim iScenarioID As Integer = 0
            Dim drow As DataRow = Nothing

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [EcoSim NxN] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcosimScenarioForcingMatrix")

            sDBVersion = CSng(Me.m_dbEwE5.GetValue("SELECT MAX(Version) FROM [Database specifications]"))

            While reader.Read()

                drow = writer.NewRow()

                iScenarioID = Me.HashKey(eDataTypes.EcoSimScenario, CStr(reader("Scenario")))
                ' Link scenario
                drow("ScenarioID") = iScenarioID

                ' JS 5oct08: vulmult indexed by (prey, pred). groupName referred to prey, groupColName to pred
                ' Link prey (group)
                drow("PreyID") = Me.HashKey(eDataTypes.EcoSimGroupInput, CStr(reader("groupName")), eDataTypes.EcoSimScenario, iScenarioID)
                ' Link predator (group)
                drow("PredID") = Me.HashKey(eDataTypes.EcoSimGroupInput, CStr(reader("groupColName")), eDataTypes.EcoSimScenario, iScenarioID)

                ' Vulnerability
                sVul = CSng(Me.FixValue(reader, "vulnerability", 2.0))
                If sVul < 1.0! Then sVul = 2.0!
                drow("vulnerability") = sVul

#If 0 Then ' Discontinued in 1.71, now allocated from [Ecosim NxN Forcing]
                ' Link to forcing shape
                iShape = CInt(reader("seasonType"))
                If (iShape > 0) Then
                    Try
                        ' EwE5 shape assigment may not be valid anymore - test for success
                        iShapeID = HashKey(eDataTypes.Forcing, CStr(iShape), iScenarioID)
                    Catch e As Exception
                        iShapeID = 0
                    End Try
                    drow("ForcingShapeID") = iShapeID
                Else
                    drow("ForcingShapeID") = 0
                End If

                ' Link to mediation shape
                iShape = CInt(reader("MediationType"))
                If (iShape > 0) Then
                    Try
                        ' EwE5 shape assigment may not be valid anymore - test for success
                        iShapeID = HashKey(eDataTypes.Mediation, CStr(CInt(100 + iShape)))
                    Catch e As Exception
                        iShapeID = 0
                    End Try
                    drow("MediationShapeID") = iShapeID
                Else
                    drow("MediationShapeID") = 0
                End If
#End If

                ' Store the row
                writer.AddRow(drow)

            End While

            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Private Sub ImportEcoSimMedWeights(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writerGroup As cEwEDatabase.cEwEDbWriter = Nothing
            Dim writerFleet As cEwEDatabase.cEwEDbWriter = Nothing
            Dim iScenarioID As Integer = 0
            Dim drow As DataRow = Nothing
            Dim strKey As String = ""
            Dim iGroupID As Integer = 0
            Dim iFleetID As Integer = 0
            Dim iShapeID As Integer = 0

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [EcoSim MedWeights] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writerGroup = Me.m_dbEwE6.GetWriter("EcosimScenarioShapeMedWeightsGroup")
            writerFleet = Me.m_dbEwE6.GetWriter("EcosimScenarioShapeMedWeightsFleet")

            While reader.Read()

                ' Group or Fleet?
                ' EwE6 will split this into two tables
                strKey = CStr(reader("groupName"))
                iScenarioID = Me.HashKey(eDataTypes.EcoSimScenario, CStr(reader("Scenario")))
                iGroupID = Me.HashKey(eDataTypes.EcoSimGroupInput, strKey, eDataTypes.EcoSimScenario, iScenarioID)
                iFleetID = Me.HashKey(eDataTypes.FleetInput, strKey)
                iShapeID = Me.HashKey(eDataTypes.Mediation, CStr(100 + CInt(reader("CurPlot"))), eDataTypes.EcoSimScenario, iScenarioID)

                If (iGroupID > 0) Then
                    ' Add group
                    drow = writerGroup.NewRow()
                    drow("ScenarioID") = iScenarioID
                    drow("GroupID") = iGroupID
                    drow("ShapeID") = iShapeID
                    drow("MedWeights") = reader("MedWeights")
                    writerGroup.AddRow(drow)
                ElseIf (iFleetID > 0) Then
                    ' Add fleet
                    drow = writerFleet.NewRow()
                    drow("ScenarioID") = iScenarioID
                    drow("FleetID") = iFleetID
                    drow("ShapeID") = iShapeID
                    drow("MedWeights") = reader("MedWeights")
                    writerFleet.AddRow(drow)
                Else
                    ' Unknown: ignore
                End If

            End While

            Me.m_dbEwE6.ReleaseWriter(writerGroup)
            Me.m_dbEwE6.ReleaseWriter(writerFleet)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Private Sub ImportEcoSimNxNInteraction(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            ' Special field values
            Dim iScenarioID As Integer = 0
            ' EwE5: Shape number implicitly identifies a shape type by comparing its value to predefined value ranges
            Dim iShapeNumber As Integer = 0
            Dim iShapeID As Integer = 0
            Dim iPredID As Integer = 0
            Dim iPreyID As Integer = 0
            Dim bIsMediation As Boolean = False
            Dim iFFApplication As eForcingFunctionApplication = 0

            ' EwE6: Shape type explicitly identifies a shape type
            reader = m_dbEwE5.GetReader(String.Format("SELECT * from [EcoSim NxN Forcing] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcosimScenarioPredPreyShape")

            While reader.Read()

                ' Resolve scenario ID
                iScenarioID = Me.HashKey(eDataTypes.EcoSimScenario, CStr(reader("Scenario")))
                ' Resolve shape ID, depending on shape type
                If (CBool(reader("IsMedFunction")) = True) Then
                    iShapeID = Me.HashKey(eDataTypes.Mediation, CStr(100 + CInt(reader("FunctionNumber"))), eDataTypes.EcoSimScenario, iScenarioID)
                Else
                    iShapeID = Me.HashKey(eDataTypes.Forcing, CStr(reader("FunctionNumber")), eDataTypes.EcoSimScenario, iScenarioID)
                End If
                iPreyID = Me.HashKey(eDataTypes.EcoSimGroupInput, CStr(reader("GroupName")), eDataTypes.EcoSimScenario, iScenarioID)
                iPredID = Me.HashKey(eDataTypes.EcoSimGroupInput, CStr(reader("GroupColName")), eDataTypes.EcoSimScenario, iScenarioID)

                ' MedFunction flag does not need importing since shape type can be looked up via iShapeID

                drow = writer.NewRow()
                drow("ScenarioID") = iScenarioID
                drow("ShapeID") = iShapeID
                drow("PredID") = iPredID
                drow("PreyID") = iPreyID
                drow("FunctionType") = Me.FixValue(reader, "FunctionType", 1)
                writer.AddRow(drow)

            End While

            Me.m_dbEwE5.ReleaseReader(reader)
            Me.m_dbEwE6.ReleaseWriter(writer)

        End Sub ' ImportEcoSimNxNInteraction

#End Region ' Forcing shapes 

#Region " Time series "

        ''' <summary>
        ''' Import Time Series data
        ''' </summary>
        ''' <param name="strModelName"></param>
        Private Sub ImportTimeSeries(ByVal strModelName As String)
            Me.ImportTSDatasets(strModelName)
            Me.ImportTS(strModelName)
        End Sub

        Private Sub ImportTSDatasets(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iDatasetID As Integer = 0
            Dim strDataset As String = ""
            Dim strDatasetLast As String = ""
            Dim iNumYears As Integer = 0

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * FROM [Time Series] WHERE modelName='{0}' ORDER BY Dataset", strModelName))
            writer = Me.m_dbEwE6.GetWriter("EcosimTimeSeriesDataset")

            While reader.Read()

                ' Get dataset name of this time series
                strDataset = CStr(reader("Dataset"))
                ' Is a new dataset?
                If (String.Compare(strDatasetLast, strDataset, False) <> 0) Then
                    ' #Yes: switch datasets
                    ' Was another dataset handled?
                    If (drow IsNot Nothing) Then
                        ' #Yes: Commit this dataset
                        writer.AddRow(drow)
                    End If

                    ' Next dataset!
                    iDatasetID += 1
                    drow = writer.NewRow()
                    drow("DatasetID") = iDatasetID
                    drow("DatasetName") = strDataset
                    drow("Description") = ""
                    drow("Author") = ""
                    drow("Contact") = ""
                    ' All TS within a dataset have the same start year
                    drow("FirstYear") = Me.FixValue(reader, "FirstYear", 1950)
                    ' Calculate number of years in this time series
                    Dim strData As String = CStr(Me.FixValue(reader, "MemoField", ""))
                    ' Set as initial max number of years for this dataset 
                    iNumYears = CInt(strData.Length / 10)
                    drow("NumYears") = iNumYears

                    Me.HashKey(eDataTypes.TimeSeriesDataset, strDataset) = iDatasetID
                    strDatasetLast = strDataset

                Else
                    ' #No: processing same dataset
                    ' Calculate number of years in this time series
                    Dim strData As String = CStr(Me.FixValue(reader, "MemoField", ""))
                    ' Find max across dataset so far
                    iNumYears = Math.Max(iNumYears, CInt(strData.Length / 10))
                    ' Store this max
                    drow("NumYears") = iNumYears
                End If

            End While

            ' Commit last dataset
            If (drow IsNot Nothing) Then writer.AddRow(drow)

            Me.m_dbEwE6.ReleaseWriter(writer, True)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

        Private Sub ImportTS(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writerTimeSeries As cEwEDatabase.cEwEDbWriter = Nothing
            Dim writerGroup As cEwEDatabase.cEwEDbWriter = Nothing
            Dim writerFleet As cEwEDatabase.cEwEDbWriter = Nothing
            Dim writerShape As cEwEDatabase.cEwEDbWriter = Nothing
            Dim writerShapeTime As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iTimeSeriesID As Integer = 0
            Dim iDatasetID As Integer = 0
            Dim iGroupID As Integer = 0
            Dim iFleetID As Integer = 0
            Dim eType As eTimeSeriesType = 0
            Dim strMemo As String = ""

            reader = m_dbEwE5.GetReader(String.Format("SELECT * from [Time Series] where modelName='{0}' ORDER BY SequenceNo ASC", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            ' Time series are scenario-independent
            writerTimeSeries = Me.m_dbEwE6.GetWriter("EcosimTimeSeries")
            writerGroup = Me.m_dbEwE6.GetWriter("EcosimTimeSeriesGroup")
            writerFleet = Me.m_dbEwE6.GetWriter("EcosimTimeSeriesFleet")
            writerShape = Me.m_dbEwE6.GetWriter("EcoSimShape")
            writerShapeTime = Me.m_dbEwE6.GetWriter("EcosimShapeTime")

            While reader.Read()

                ' Map EwE5 time series type to EwE6 eTimeSeriesType enum
                Select Case CInt(Me.FixValue(reader, "DatType", 0))
                    Case 0
                        eType = eTimeSeriesType.BiomassRel
                    Case 1
                        eType = eTimeSeriesType.BiomassAbs
                    Case -1
                        eType = eTimeSeriesType.BiomassForcing
                    Case 2
                        eType = eTimeSeriesType.TimeForcing
                    Case 3
                        eType = eTimeSeriesType.FishingEffort
                    Case 4
                        eType = eTimeSeriesType.FishingMortality
                    Case 5
                        eType = eTimeSeriesType.TotalMortality
                    Case -5
                        eType = eTimeSeriesType.ConstantTotalMortality
                    Case 6
                        eType = eTimeSeriesType.Catches
                    Case -6
                        eType = eTimeSeriesType.CatchesForcing
                    Case 7
                        eType = eTimeSeriesType.AverageWeight
                    Case 8
                        eType = eTimeSeriesType.EcotracerConcRel
                    Case 9
                        eType = eTimeSeriesType.EcotracerConcAbs
                End Select

                ' JS 07may07: time series assignments have changed in EwE6. A time series is always connected to either a fleet
                '             (via EcosimTimeSeriesFleet) or a group (via EcosimTimeSeriesGroup). Both tables then link one-on-one
                '             to the actual time series data in EcosimTimeSeries.
                Select Case cTimeSeriesFactory.TimeSeriesCategory(eType)

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Forcing

                        Me.m_iNextShapeID += 1

                        drow = writerShape.NewRow()
                        drow("ShapeID") = Me.m_iNextShapeID
                        drow("ShapeType") = eDataTypes.Forcing
                        writerShape.AddRow(drow)
                        'writerShape.Commit()

                        drow = writerShapeTime.NewRow()
                        drow("ShapeID") = Me.m_iNextShapeID
                        drow("Title") = Me.FixValue(reader, "DatName", "")
                        drow("YZero") = 0
                        drow("YBase") = 0
                        drow("YEnd") = 0
                        drow("Steep") = 0
                        drow("FunctionType") = eShapeFunctionType.NotSet

                        strMemo = CStr(Me.FixValue(reader, "MemoField", ""))
                        drow("Zscale") = Me.RebuildNumberListString(strMemo, CChar(" "), 10, cCore.N_MONTHS)
                        writerShapeTime.AddRow(drow)

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet

                        iDatasetID = Me.HashKey(eDataTypes.TimeSeriesDataset, CStr(reader("Dataset")))
                        iFleetID = Me.PoolCodeID(eDataTypes.FleetInput, CInt(reader("Pool")))

                        iTimeSeriesID += 1

                        drow = writerTimeSeries.NewRow()
                        drow("TimeSeriesID") = iTimeSeriesID
                        drow("Sequence") = iTimeSeriesID
                        drow("DatasetID") = iDatasetID
                        drow("DatType") = eType
                        drow("DatName") = Me.FixValue(reader, "DatName", "")
                        'drow("FirstYear") = Me.FixValue(reader, "FirstYear", 1950)

                        strMemo = CStr(Me.FixValue(reader, "MemoField", ""))
                        drow("TimeValues") = Me.RebuildNumberListString(strMemo, CChar(" "), 10)
                        'drow("NumYears") = CInt(strMemo.Length / 10)

                        ' JS 06Nov07: Time series imported with weight of 1 (not 0!)
                        drow("WtType") = Me.FixValue(reader, "WtType", 1.0!)
                        writerTimeSeries.AddRow(drow)

                        drow = writerFleet.NewRow()
                        drow("TimeSeriesID") = iTimeSeriesID
                        drow("FleetID") = iFleetID
                        writerFleet.AddRow(drow)

                        ' Is this an existing fleet?
                        If iFleetID = 0 Then
                            Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_WARNING_TIMESERIESFLEET, _
                                    Me.FixValue(reader, "DatName", ""), _
                                    Me.FixValue(reader, "Dataset", ""), _
                                    CInt(reader("Pool"))), _
                                    eMessageType.DataImport, eMessageImportance.Information)
                        End If

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group

                        iDatasetID = Me.HashKey(eDataTypes.TimeSeriesDataset, CStr(reader("Dataset")))
                        iGroupID = Me.PoolCodeID(eDataTypes.EcoPathGroupInput, CInt(reader("Pool")))

                        iTimeSeriesID += 1

                        drow = writerTimeSeries.NewRow()
                        drow("TimeSeriesID") = iTimeSeriesID
                        drow("Sequence") = iTimeSeriesID
                        drow("DatasetID") = iDatasetID
                        drow("DatType") = eType
                        drow("DatName") = Me.FixValue(reader, "DatName", "")
                        'drow("FirstYear") = Me.FixValue(reader, "FirstYear", 1950)

                        strMemo = CStr(Me.FixValue(reader, "MemoField", ""))
                        drow("TimeValues") = Me.RebuildNumberListString(strMemo, CChar(" "), 10)
                        'drow("NumYears") = CInt(strMemo.Length / 10)

                        ' JS 29Nov07: Time series imported with weight of 1 (not 0!)
                        drow("WtType") = Me.FixValue(reader, "WtType", 1.0!)
                        writerTimeSeries.AddRow(drow)

                        drow = writerGroup.NewRow()
                        drow("TimeSeriesID") = iTimeSeriesID
                        drow("GroupID") = iGroupID
                        drow("VariableName") = ""
                        writerGroup.AddRow(drow)

                        ' Is this an existing group?
                        If (iGroupID = 0) Then
                            Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_WARNING_TIMESIERIESGROUP, _
                                    Me.FixValue(reader, "DatName", ""), _
                                    Me.FixValue(reader, "Dataset", ""), _
                                    CInt(reader("Pool"))), _
                                    eMessageType.DataImport, eMessageImportance.Information)
                        End If

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.NotSet
                        'Trying to import unkown time series type - ignore this TS
                        Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_WARNING_TIMESERIESTYPE, _
                                Me.FixValue(reader, "DatName", ""), _
                                Me.FixValue(reader, "Dataset", ""), _
                                eType.ToString()), _
                                eMessageType.DataImport, eMessageImportance.Information)

                End Select

            End While

            Me.m_dbEwE6.ReleaseWriter(writerTimeSeries, True)
            Me.m_dbEwE6.ReleaseWriter(writerGroup, True)
            Me.m_dbEwE6.ReleaseWriter(writerFleet, True)
            Me.m_dbEwE6.ReleaseWriter(writerShape, True)
            Me.m_dbEwE6.ReleaseWriter(writerShapeTime, True)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

#End Region ' Time series 

#End Region ' EcoSim

#Region " Ecospace "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="strModelName"></param>
        ''' <returns>True if a scenario was imported.</returns>
        ''' -------------------------------------------------------------------
        Private Function ImportEcoSpaceScenario(ByVal strModelName As String) As Boolean

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim nScenarioID As Integer = 1
            Dim iEcosimScenarioID As Integer = -1

            ' Clear table(s)
            Me.m_dbEwE6.Execute("DELETE * FROM EcospaceScenario")

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * FROM [EcoSpace] WHERE modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return False

            writer = Me.m_dbEwE6.GetWriter("EcospaceScenario")

            While reader.Read()

                drow = writer.NewRow()
                drow("ScenarioID") = nScenarioID
                drow("ScenarioName") = reader("Scenario")
                drow("Description") = Me.FixValue(reader, "remarks", "")
                ' Try to resolve ecosim scenario ID. The original Ecosim scenario may not exist anymore which is fine.
                iEcosimScenarioID = Me.HashKey(eDataTypes.EcoSimScenario, CStr(Me.FixValue(reader, "SimScenario", "")))
                drow("EcosimScenarioID") = iEcosimScenarioID
                drow("Inrow") = Me.FixValue(reader, "Inrow")
                drow("Incol") = Me.FixValue(reader, "Incol")
                drow("CellLength") = Me.FixValue(reader, "CellLength")
                drow("IDH_UL") = Me.FixValue(reader, "IDH_UL", 0)
                drow("IDH_SS") = Me.FixValue(reader, "IDH_SS", 0)
                drow("TimeStep") = Me.FixValue(reader, "TimeStep", 0.25)
                drow("PredictEffort") = Me.FixValue(reader, "PredictEffort")
                drow("LastSaved") = Me.ExtractLastSavedJulianDate(CStr(Me.FixValue(reader, "remarks", "")))

                ' JS 28nov06: habitats now db-linked in EcospaceScenarioHabitat table to allow any number of habitats
                'If sDBVers < 1.557 Then  'first read to old habitats below then update below
                '    For i As Integer = 0 To 8
                '        drow(String.Format("Habitat{0}", i)) = Me.FixValue(reader, String.Format("Habitat{0}", i))
                '    Next
                'End If

                writer.AddRow(drow)
                writer.Commit()

                ' Import Remarks
                ' Me.AddRemark(reader("remarks"), eDataTypes.EcoSpaceScenario, nScenarioID)

                ' JS 061221: References do not need to be imported for now
                'ImportRefCode("RefCode", "quickRef")

                ' Remember scenario ID mapping
                Me.HashKey(eDataTypes.EcoSpaceScenario, CStr(reader("Scenario"))) = nScenarioID

                nScenarioID += 1

            End While

            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

            ' Return whether at least one scenario was added
            Return (nScenarioID > 1)
        End Function

        Private Sub ImportEcospaceHabitats(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iScenarioID As Integer = 0
            Dim iHabitatID As Integer = 1
            Dim strHabitat As String = ""

            ' First define 'All' habitat for every scenario
            ' The EwE6 database will contain a definition for the All habitat. EwE5 tables provide information for this habitat
            ' while this habitat is not explicitly defined in the EwE5 database. It merely exists in the EwE5 GUI.
            Dim dtEcospaceScenarios As Dictionary(Of String, Integer) = Me.m_adtKeys(CInt(eDataTypes.EcoSpaceScenario))
            Dim strEcospaceScenario As String = ""

            writer = Me.m_dbEwE6.GetWriter("EcospaceScenarioHabitat")

            ' For every ecospace scenario
            For Each iScenarioID In dtEcospaceScenarios.Values
                ' Create new row
                drow = writer.NewRow()
                ' Set FKs
                drow("ScenarioID") = iScenarioID
                ' HabitatID is the unique database ID for an Ecospace habitat
                drow("HabitatID") = iHabitatID
                ' Sequence determines habitat order
                drow("Sequence") = iHabitatID
                drow("HabitatName") = My.Resources.CoreDefaults.CORE_ALL_HABITAT
                ' There
                writer.AddRow(drow)
                ' Remember 'All' Habitat mapping
                Me.HashKey(eDataTypes.EcospaceHabitat, "0", eDataTypes.EcoSpaceScenario, iScenarioID) = iHabitatID
                ' Next
                iHabitatID += 1
            Next

            ' Now import habitat information using most recent EwE5 format
            reader = m_dbEwE5.GetReader(String.Format("SELECT * FROM [Ecospace habitats] WHERE modelName='{0}'", strModelName))
            While reader.Read()
                ' Resolve scenario ID
                iScenarioID = Me.HashKey(eDataTypes.EcoSpaceScenario, CStr(reader("scenario")))
                ' Add new row
                drow = writer.NewRow()
                ' Populate FKs
                drow("ScenarioID") = iScenarioID
                ' HabitatID is the unique database ID for an Ecospace habitat
                drow("HabitatID") = iHabitatID
                ' Sequence determines habitat order
                drow("Sequence") = iHabitatID
                drow("HabitatName") = reader("HabitatText")
                writer.AddRow(drow)

                ' Remember habitat ID mapping
                Me.HashKey(eDataTypes.EcospaceHabitat, CStr(reader("HabitatNo")), eDataTypes.EcoSpaceScenario, iScenarioID) = iHabitatID

                ' Next
                iHabitatID += 1

            End While

            Me.m_dbEwE5.ReleaseReader(reader)
            Me.m_dbEwE6.ReleaseWriter(writer)

            Me.ImportEcospaceHabitatChanges(strModelName)

        End Sub

        Private Sub ImportEcospaceHabitatChanges(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iScenarioID As Integer = 0
            Dim strHabChanges As String = ""
            Dim strHabChange As String = ""
            Dim nHabChanges As Integer = 0

            ' Import habitat changes
            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * FROM [Ecospace habitat changes] WHERE modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcospaceScenarioHabitatChange")

            While reader.Read()

                iScenarioID = Me.HashKey(eDataTypes.EcoSpaceScenario, CStr(reader("Scenario")))
                strHabChanges = CStr(Me.FixValue(reader, "HabChange", ""))
                nHabChanges = CInt(Math.Floor(strHabChanges.Length / 14))

                For iHabChange As Integer = 0 To nHabChanges
                    drow = writer.NewRow()
                    drow("ScenarioID") = iScenarioID
                    drow("Time") = reader("HabTime")
                    drow("Sequence") = iHabChange

                    ' Orig VB comment:
                    's = s + Format(HabChange(0, i), "0000") + Format(HabChange(1, i), "0000") + Format(HabChange(2, i), "00") + Format(HabChange(0, i), "0000")
                    '0: row with 4 digits, 1: col with 4 digits, 2: drawmod with 2 digits, 3: hab etc with 4 digits (can be depth 9999)
                    Try
                        strHabChange = strHabChanges.Substring(iHabChange * 14, 14)
                        drow("InRow") = Integer.Parse(strHabChange.Substring(0, 4))
                        drow("InCol") = Integer.Parse(strHabChange.Substring(4, 4))
                        drow("DrawMod") = Integer.Parse(strHabChange.Substring(8, 2))
                        drow("Change") = Integer.Parse(strHabChange.Substring(10, 4))
                        writer.AddRow(drow)
                    Catch ex As Exception
                        ' No need to localize, log only
                        Me.LogMessage(String.Format("Incomplete habitat change information for scenario {0}, time step {1} not read", CStr(reader("Scenario")), reader("HabTime")))
                    End Try
                Next

            End While

            Me.m_dbEwE5.ReleaseReader(reader)
            Me.m_dbEwE6.ReleaseWriter(writer)

        End Sub

        Private Sub ImportEcospaceRegions(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iScenarioID As Integer = 1
            Dim iRegionID As Integer = 1

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * FROM [EcoSpace regions] WHERE modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcospaceScenarioRegion")

            While reader.Read()

                iScenarioID = Me.HashKey(eDataTypes.EcoSpaceScenario, CStr(reader("scenario")))

                drow = writer.NewRow()
                drow("ScenarioID") = iScenarioID
                drow("RegionID") = iRegionID
                drow("Sequence") = reader("RegionNo")
                drow("RegionName") = reader("RegionText")
                writer.AddRow(drow)

                ' Remember region ID mapping
                Me.HashKey(eDataTypes.EcospaceRegion, CStr(reader("RegionNo")), eDataTypes.EcoSpaceScenario, iScenarioID) = iRegionID

                iRegionID += 1
            End While

            Me.m_dbEwE5.ReleaseReader(reader)
            Me.m_dbEwE6.ReleaseWriter(writer)

        End Sub

        Private Sub ImportEcospaceMPA(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iScenarioID As Integer = 1
            Dim nMPAID As Integer = 1
            Dim strMPA As String = ""
            Dim sbMPA As Text.StringBuilder = Nothing

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * FROM [EcoSpace MPA] WHERE modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcospaceScenarioMPA")

            While reader.Read()

                iScenarioID = Me.HashKey(eDataTypes.EcoSpaceScenario, CStr(reader("Scenario")))

                drow = writer.NewRow()
                drow("ScenarioID") = iScenarioID
                drow("MPAID") = nMPAID
                drow("Sequence") = reader("MPANo")
                drow("MPAName") = reader("MPAName")
                ' MPAMonth: EwE5 uses a string to represent a field of 12 boolean flags, where 'O' indicates that
                ' the MPA is open for fishing, and 'C' that the MPA is closed for fishing.
                ' Ewe6 uses a '1' when the MPA is open for fishing, and '0' when the MPA is closed.
                strMPA = CStr(Me.FixValue(reader, "MPAMonth", ""))
                sbMPA = New Text.StringBuilder()
                For i As Integer = 0 To Math.Min(strMPA.Length, 12) - 1
                    ' Closed for fishing: store as 0, open: store as 1
                    sbMPA.Append(CChar(IIf("Cc".IndexOf(strMPA(i)) >= 0, "0", "1")))
                Next
                drow("MPAMonth") = sbMPA.ToString()
                writer.AddRow(drow)

                ' Remember MPA ID mapping
                Me.HashKey(eDataTypes.EcospaceMPA, CStr(reader("MPANo")), eDataTypes.EcoSpaceScenario, iScenarioID) = nMPAID

                nMPAID += 1
            End While

            Me.m_dbEwE5.ReleaseReader(reader)
            Me.m_dbEwE6.ReleaseWriter(writer)

        End Sub

        Private Sub ImportEcoSpaceGroups(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim writerSub As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim drowSub As DataRow = Nothing
            Dim iScenarioID As Integer = 1
            Dim iGroupID As Integer = 1
            Dim iEcopathGroupID As Integer = -1
            Dim strPreferredHabitats As String = ""
            Dim sValue As Single = 0.0!
            Dim iHabitatID As Integer = 1

            Dim dtEcopathGroups As Dictionary(Of String, Integer) = Me.m_adtKeys(CInt(eDataTypes.EcoPathGroupInput))
            Dim strEcopathGroup As String = ""
            Dim dtEcospaceScenarios As Dictionary(Of String, Integer) = Me.m_adtKeys(CInt(eDataTypes.EcoSpaceScenario))
            Dim strEcospaceScenario As String = ""
            ' Flag stating whether an ecospace group was found for a given ecopath group
            Dim bHasGroup As Boolean = False

            writer = Me.m_dbEwE6.GetWriter("EcospaceScenarioGroup")
            writerSub = Me.m_dbEwE6.GetWriter("EcospaceScenarioGroupHabitat")

            ' For every ecopath group...
            For Each strEcopathGroup In dtEcopathGroups.Keys
                ' and for every ecospace scenario
                For Each strEcospaceScenario In dtEcospaceScenarios.Keys

                    ' .. create a new ecospace group
                    iScenarioID = Me.HashKey(eDataTypes.EcoSpaceScenario, strEcospaceScenario)
                    iEcopathGroupID = Me.HashKey(eDataTypes.EcoPathGroupInput, strEcopathGroup)

                    ' Check if an ecospace group exists for this ecopath group, ecosim scenario combination
                    reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [EcoSpace N] where modelName='{0}' AND Scenario='{1}' AND groupName='{2}'", _
                            strModelName, strEcospaceScenario, strEcopathGroup))

                    bHasGroup = reader.Read()

                    ' Create new row
                    drow = writer.NewRow()
                    drow("ScenarioID") = iScenarioID
                    drow("GroupID") = iGroupID
                    drow("EcopathGroupID") = iEcopathGroupID

                    ' Copy whatever data can be copied
                    If (bHasGroup) Then
                        ' JS 070213: Discontinued fields 'PrefHab0' to 'PrefHab7'
                        ' JS 061201: Discontinued. Field 'PrefHab' is imported incorrectly and not read in EwE5
                        ' drow("PrefHab") = Me.FixValue(reader, "PrefHab")
                        sValue = CSng(Me.FixValue(reader, "Mvel", 0.0!))
                        If (sValue = 0.0!) Then sValue = 300.0!
                        drow("Mvel") = sValue

                        sValue = CSng(Me.FixValue(reader, "RelMoveBad", 0.0!))
                        If (sValue = 0.0!) Then sValue = 2.0!
                        drow("RelMoveBad") = sValue

                        sValue = CSng(Me.FixValue(reader, "RelVulBad", 0.0!))
                        If (sValue = 0.0!) Then sValue = 2.0!
                        drow("RelVulBad") = sValue

                        sValue = CSng(Me.FixValue(reader, "EatEffBad", 0.0!))
                        If (sValue = 0.0!) Then sValue = 0.5!
                        drow("EatEffBad") = sValue

                        ' JS 070116: Discontinued. Field 'RiskSens' is imported in EwE5, but never used
                        ' drow("RiskSens") = Me.FixValue(reader, "RiskSens")
                        drow("IsAdvected") = Me.FixValue(reader, "IsAdvected")
                        drow("IsMigratory") = Me.FixValue(reader, "IsMigratory")
                        drow("MigConcRow") = Me.FixValue(reader, "MigConcRow")
                        drow("MigConcCol") = Me.FixValue(reader, "MigConcCol")
                        drow("PrefRow") = Me.RebuildNumberListString(CStr(Me.FixValue(reader, "PrefRow", "0")), CChar(" "), 5)
                        drow("PrefCol") = Me.RebuildNumberListString(CStr(Me.FixValue(reader, "PrefCol", "0")), CChar(" "), 5)
                    Else
                        ' #No: the new group will get all default values
                        Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_FIX_CREATEECOSPACEGROUP, _
                                iGroupID, _
                                strEcopathGroup, _
                                strEcospaceScenario), _
                                eMessageType.DataImport, eMessageImportance.Information)
                    End If

                    writer.AddRow(drow)
                    writer.Commit()

                    If (bHasGroup) Then

                        ' Import preferred habitats to subtable
                        ' In EwE5, preferred habitats were stored as a string of '0' and '1' values,
                        ' where a '1' indicates a group preference for the habitat whose index
                        ' matches the position of the '1'.
                        strPreferredHabitats = CStr(Me.FixValue(reader, "habitat", ""))
                        ' For all habitat preferences (habitat '0' can also be preferred!)
                        For iHabitatNo As Integer = 0 To strPreferredHabitats.Length - 1
                            ' Does this habitat index represent a preferred habitat?
                            If (strPreferredHabitats.Substring(iHabitatNo, 1) = "1") Then
                                ' #Yes: try to find a matching habitat ID 
                                iHabitatID = Me.HashKey(eDataTypes.EcospaceHabitat, CStr(iHabitatNo), eDataTypes.EcoSpaceScenario, iScenarioID)
                                ' Is habitat ID valid?
                                If (iHabitatID > 0) Then
                                    ' #Yes: Add a habitat preference for this group
                                    drowSub = writerSub.NewRow()
                                    drowSub("ScenarioID") = iScenarioID
                                    drowSub("GroupID") = iGroupID
                                    drowSub("HabitatID") = iHabitatID
                                    writerSub.AddRow(drowSub)
                                End If
                            End If
                        Next

                        ' Import Remarks
                        Me.AddRemark(Me.FixValue(reader, "Remark", ""), eDataTypes.EcospaceGroup, iGroupID, eVarNameFlags.Name)
                        ' References are discarded
                        ' ImportRefCode("RefCode", "quickRef")
                    End If

                    ' Remember ecospace group mapping
                    Me.HashKey(eDataTypes.EcospaceGroup, strEcopathGroup, eDataTypes.EcoSpaceScenario, iScenarioID) = iGroupID

                    ' Next group
                    Me.m_dbEwE5.ReleaseReader(reader)
                    iGroupID += 1

                Next strEcospaceScenario
            Next strEcopathGroup

            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE6.ReleaseWriter(writerSub)

        End Sub

        Private Sub ImportEcoSpaceFleets(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim readerSub As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iScenarioID As Integer = 1
            Dim writerFishMap As cEwEDatabase.cEwEDbWriter = Nothing
            Dim writerHabFish As cEwEDatabase.cEwEDbWriter = Nothing
            Dim writerMPAFish As cEwEDatabase.cEwEDbWriter = Nothing
            Dim strFlags As String = ""
            Dim astrPort As String()
            Dim astrSail As String()
            Dim iCell As Integer = 0
            Dim iFleetID As Integer = 1
            Dim iHabitatID As Integer = 1
            Dim iMPAID As Integer = 1
            Dim nRows As Integer = 0
            Dim nCols As Integer = 0

            ' Generate an Ecospace fleet entry for every Ecopath fleet
            Dim dtEcopathFleets As Dictionary(Of String, Integer) = Me.m_adtKeys(CInt(eDataTypes.FleetInput))
            Dim strEcopathFleet As String = ""
            Dim dtEcospaceScenarios As Dictionary(Of String, Integer) = Me.m_adtKeys(CInt(eDataTypes.EcoSpaceScenario))
            Dim strEcospaceScenario As String = ""
            Dim bHasFleet As Boolean = False

            ' Get writers
            writer = Me.m_dbEwE6.GetWriter("EcospaceScenarioFleet")
            writerFishMap = Me.m_dbEwE6.GetWriter("EcospaceScenarioFleetMap")
            writerHabFish = Me.m_dbEwE6.GetWriter("EcospaceScenarioHabitatFishery")
            writerMPAFish = Me.m_dbEwE6.GetWriter("EcospaceScenarioMPAFishery")

            If dtEcospaceScenarios IsNot Nothing And dtEcopathFleets IsNot Nothing Then

                ' For each ecospace scenario..
                For Each strEcospaceScenario In dtEcospaceScenarios.Keys
                    ' ..and each ecopath fleet
                    For Each strEcopathFleet In dtEcopathFleets.Keys

                        ' Generate an Ecospace fleet entry
                        reader = m_dbEwE5.GetReader(String.Format("SELECT * FROM [EcoSpace Gear] WHERE modelName='{0}' AND Scenario='{1}' AND GearName='{2}'", _
                                strModelName, strEcospaceScenario, strEcopathFleet))
                        bHasFleet = reader.Read()

                        ' Create new row
                        drow = writer.NewRow()

                        iScenarioID = Me.HashKey(eDataTypes.EcoSpaceScenario, strEcospaceScenario)

                        drow("ScenarioID") = iScenarioID
                        drow("FleetID") = iFleetID
                        drow("EcopathFleetID") = Me.HashKey(eDataTypes.FleetInput, strEcopathFleet)

                        ' Copy fields
                        If (bHasFleet) Then
                            drow("EffPower") = Me.FixValue(reader, "EffPower")
                            ' JS 070119: discontinued in favour of finer-grained MPAFish, see below
                            ' drow("MPAfishery") = Me.FixValue(reader, "MPAFishery", "T")
                        End If

                        writer.AddRow(drow)
                        writer.Commit()

                        If bHasFleet Then

                            astrSail = Me.SplitNumberListString(CStr(Me.FixValue(reader, "Sail", "0")), CChar(" "), 5)
                            astrPort = Me.SplitNumberListString(CStr(Me.FixValue(reader, "Port", "0")), CChar(" "), 1)

                            ' Sail data missing? Then Sail and Port data must be read from old table [EcoSpace GearxNxN]
                            If (astrSail.Length = 1) Then

                                Dim nPrevScenarioID As Integer = -1
                                Dim asSail(,) As Single
                                Dim anPort(,) As Integer

                                Try

                                    ' Get # of cols and rows for this scenario, in case an old version Sail() and Port() import is required
                                    readerSub = m_dbEwE6.GetReader(String.Format("SELECT * FROM EcoSpaceScenario WHERE ScenarioID={0}", iScenarioID))
                                    readerSub.Read()
                                    nRows = CInt(readerSub("InRow"))
                                    nCols = CInt(readerSub("InCol"))
                                    Me.m_dbEwE6.ReleaseReader(readerSub)
                                    readerSub = Nothing

                                    ' Allocate space to read cell data
                                    ReDim asSail(nRows, nCols)
                                    ReDim anPort(nRows, nCols)

                                    readerSub = m_dbEwE5.GetReader(String.Format("SELECT * FROM [EcoSpace GearxNxN] WHERE modelName='{0}' AND Scenario='{1}' AND GearName='{2}'", _
                                            strModelName, strEcospaceScenario, strEcopathFleet))

                                    If readerSub IsNot Nothing Then
                                        While readerSub.Read()
                                            asSail(Math.Min(nRows, CInt(reader("InRow"))), _
                                                  Math.Min(nCols, CInt(reader("InCol")))) = CSng(Me.FixValue(readerSub, "Sail", "0"))
                                            anPort(Math.Min(nRows, CInt(reader("InRow"))), _
                                                  Math.Min(nCols, CInt(reader("InCol")))) = CInt(Me.FixValue(readerSub, "Port", "0"))
                                        End While
                                        Me.m_dbEwE5.ReleaseReader(readerSub)
                                        readerSub = Nothing
                                    End If

                                    ' Convert Sail and Port arrays into string()
                                    ReDim astrSail(nRows * nCols)
                                    ReDim astrPort(nRows * nCols)

                                    ' Note that the core uses 1-based offsets for reading and writing Sail and Port
                                    For iRow As Integer = 1 To nRows
                                        For iCol As Integer = 1 To nCols
                                            astrSail(iCell) = CStr(asSail(iRow, iCol))
                                            astrPort(iCell) = CStr(anPort(iRow, iCol))
                                            iCell += 1
                                        Next iCol
                                    Next iRow
                                Catch ex As Exception

                                End Try
                            End If

                            ' Now write astrSail and astrPort to new table 'EcospaceScenarioFleetSpatial'
                            Dim sbSail As New Text.StringBuilder()
                            Dim sbPort As New Text.StringBuilder()

                            readerSub = m_dbEwE5.GetReader(String.Format("SELECT * FROM [EcoSpace] WHERE modelName='{0}' AND Scenario='{1}'", strModelName, strEcospaceScenario))
                            If (readerSub IsNot Nothing) Then
                                readerSub.Read()
                                nRows = CInt(readerSub("Inrow"))
                                nCols = CInt(readerSub("Incol"))
                                Me.m_dbEwE5.ReleaseReader(readerSub)

                                iCell = 0
                                For iRow As Integer = 1 To nRows
                                    For iCol As Integer = 1 To nCols

                                        If iCell > 0 Then sbSail.Append(" ") : sbPort.Append(" ")

                                        If (iCell < astrSail.Length) Then sbSail.Append(CSng(Val(astrSail(iCell)))) Else sbSail.Append(0)
                                        If (iCell < astrPort.Length) Then sbPort.Append(CInt(Val(astrPort(iCell)))) Else sbPort.Append(0)

                                        iCell += 1

                                    Next iCol
                                Next iRow

                                drow = writerFishMap.NewRow()
                                drow("ScenarioID") = iScenarioID
                                drow("FleetID") = iFleetID
                                drow("Sail") = sbSail.ToString()
                                drow("Port") = sbPort.ToString()
                                writerFishMap.AddRow(drow)

                                iCell += 1
                            End If

                            ' Write GearHab flag field to proper table combining (ScenarioID, FleetID, HabitatID)
                            strFlags = CStr(Me.FixValue(reader, "GearHab", ""))
                            ' For all habitats (including habitat '0')
                            For iHabitat As Integer = 0 To strFlags.Length - 1
                                If (strFlags.Substring(iHabitat, 1) = "1") Then
                                    iHabitatID = Me.HashKey(eDataTypes.EcospaceHabitat, CStr(iHabitat), eDataTypes.EcoSpaceScenario, iScenarioID)
                                    ' Is this a valid habitat?
                                    If (iHabitatID > 0) Then
                                        drow = writerHabFish.NewRow()
                                        drow("ScenarioID") = iScenarioID
                                        drow("FleetID") = iFleetID
                                        drow("HabitatID") = iHabitatID
                                        writerHabFish.AddRow(drow)
                                    End If
                                End If
                            Next

                            ' Write MPAfish flag field to proper table combining (ScenarioID, FleetID, MPAID)
                            strFlags = CStr(Me.FixValue(reader, "MPAFish", ""))
                            ' For all MPAs
                            For iMPA As Integer = 1 To strFlags.Length
                                If (strFlags.Substring(iMPA - 1, 1) = "T") Then
                                    iMPAID = Me.HashKey(eDataTypes.EcospaceMPA, CStr(iMPA), eDataTypes.EcoSpaceScenario, iScenarioID)
                                    ' Is this a valid MPA?
                                    If (iMPAID > 0) Then
                                        drow = writerMPAFish.NewRow()
                                        drow("ScenarioID") = iScenarioID
                                        drow("FleetID") = iFleetID
                                        drow("MPAID") = iMPAID
                                        writerMPAFish.AddRow(drow)
                                    End If
                                End If
                            Next

                            ' Import Remarks
                            Me.AddRemark(reader("remark"), eDataTypes.EcospaceFleet, iFleetID, eVarNameFlags.Name)
                        Else
                            Me.LogMessage(String.Format(My.Resources.CoreMessages.IMPORT_FIX_CREATEECOSPACEFLEET, _
                                    iFleetID, _
                                    strEcopathFleet, _
                                    strEcospaceScenario), _
                                    eMessageType.DataImport, eMessageImportance.Information)
                        End If

                        ' JS 061221: References do not need to be imported for now
                        'ImportRefCode("RefCode", "quickRef")

                        ' Remember fleet ID mapping
                        Me.HashKey(eDataTypes.EcospaceFleet, strEcopathFleet, eDataTypes.EcoSpaceScenario, iScenarioID) = iFleetID

                        ' Next fleet
                        iFleetID += 1

                        Me.m_dbEwE5.ReleaseReader(reader)

                    Next
                Next
            End If

            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE6.ReleaseWriter(writerFishMap)
            Me.m_dbEwE6.ReleaseWriter(writerHabFish)
            Me.m_dbEwE6.ReleaseWriter(writerMPAFish)

        End Sub

        Private Sub ImportEcospaceBasemap(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim strScenario As String = ""
            Dim iScenarioID As Integer = 1
            Dim astrDepth() As String = Nothing
            Dim astrHabType() As String = Nothing
            Dim astrRegion() As String = Nothing
            Dim astrMPA() As String = Nothing
            Dim astrRelPP() As String = Nothing
            Dim astrRelCin() As String = Nothing
            Dim iCell As Integer = 0
            Dim iDepth As Integer = 0
            Dim sCellValue As Single = 0
            Dim nRows As Integer = 0
            Dim nCols As Integer = 1

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * FROM [EcoSpace] WHERE modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcospaceScenarioBasemap")

            While reader.Read()

                strScenario = CStr(reader("Scenario"))
                iScenarioID = Me.HashKey(eDataTypes.EcoSpaceScenario, strScenario)
                nRows = CInt(reader("Inrow"))
                nCols = CInt(reader("Incol"))

                ' Depth: 2 formats encountered: '#####' and '#### '
                astrDepth = SplitNumberListString(CStr(Me.FixValue(reader, "Depth", "0")), CChar(" "), 5)
                ' Habtype: 2 formats encountered: '###' and '## '
                astrHabType = SplitNumberListString(CStr(Me.FixValue(reader, "HabType", "0")), CChar(" "), 3)
                ' Region: no live data seen, but spec'ed as 3 digits in length in EwE5 sources
                astrRegion = SplitNumberListString(CStr(Me.FixValue(reader, "Region", "0")), CChar(" "), 3)
                ' MPA: 2 formats encountered, '##' and '# '
                astrMPA = SplitNumberListString(CStr(Me.FixValue(reader, "MPA", "0")), CChar(" "), 2)
                ' RelPP: 2 formats encountered, '#######' and '###### '
                astrRelPP = SplitNumberListString(CStr(Me.FixValue(reader, "RelPP", "0")), CChar(" "), 7)
                ' RelCin: no live data encountered, but spec'ed as 7 digits in length in EwE5 sources
                astrRelCin = SplitNumberListString(CStr(Me.FixValue(reader, "RelCin", "0")), CChar(" "), 7)

                ' Reset cell counter
                iCell = 0

                For iRow As Integer = 1 To nRows
                    For iCol As Integer = 1 To nCols

                        ' Copy depth value
                        If astrDepth.Length > iCell Then
                            Try
                                iDepth = Integer.Parse(astrDepth(iCell))
                            Catch ex As Exception
                                ' hmm
                                iDepth = 0
                            End Try
                        Else
                            iDepth = 0
                        End If

                        ' Create row for water cells only
                        If iDepth > 0 Then

                            drow = writer.NewRow()
                            drow("ScenarioID") = iScenarioID
                            drow("Inrow") = iRow
                            drow("Incol") = iCol
                            drow("Depth") = iDepth

                            ' Resolve habitat ID for deep cell
                            If astrHabType.Length > iCell Then
                                Try
                                    sCellValue = Single.Parse(astrHabType(iCell))
                                Catch ex As Exception
                                    sCellValue = 0
                                End Try
                                If sCellValue <> 0 Then
                                    drow("HabitatID") = Me.HashKey(eDataTypes.EcospaceHabitat, CStr(sCellValue), eDataTypes.EcoSpaceScenario, iScenarioID)
                                End If
                            End If

                            ' Resolve region ID for deep cell
                            If astrRegion.Length > iCell Then
                                Try
                                    sCellValue = Single.Parse(astrRegion(iCell))
                                Catch ex As Exception
                                    sCellValue = 0
                                End Try
                                If sCellValue <> 0 Then
                                    drow("RegionID") = Me.HashKey(eDataTypes.EcospaceRegion, CStr(sCellValue), eDataTypes.EcoSpaceScenario, iScenarioID)
                                End If
                            End If

                            ' Resolve MPA ID for deep cell
                            If astrMPA.Length > iCell Then
                                Try

                                Catch ex As Exception
                                    sCellValue = 0
                                End Try
                                sCellValue = Single.Parse(astrMPA(iCell))
                                If sCellValue <> 0 Then
                                    drow("MPAID") = Me.HashKey(eDataTypes.EcospaceMPA, CStr(sCellValue), eDataTypes.EcoSpaceScenario, iScenarioID)
                                End If
                            End If

                            ' Copy Rel. PP for deep cell
                            If astrRelPP.Length > iCell Then
                                Try
                                    sCellValue = Single.Parse(astrRelPP(iCell))
                                Catch ex As Exception
                                    sCellValue = 0
                                End Try
                                drow("RelPP") = sCellValue
                            End If

                            ' Copy Rel. Cin for deep cell
                            If astrRelCin.Length > iCell Then
                                Try
                                    sCellValue = Single.Parse(astrRelCin(iCell))
                                Catch ex As Exception
                                    sCellValue = 0
                                End Try
                                drow("RelCin") = sCellValue
                            End If

                            ' Add the row
                            writer.AddRow(drow)

                        End If

                        ' Next cell
                        iCell += 1

                    Next iCol
                Next iRow

            End While

            writer.Commit()

            Me.m_dbEwE5.ReleaseReader(reader)
            Me.m_dbEwE6.ReleaseWriter(writer)

        End Sub

#End Region ' Ecospace

#Region " Ecotracer "

        Private Function ImportEcotracer(ByVal strModelName As String) As Boolean

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iScenarioID As Integer = 1

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [EcoTracer] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return False

            writer = Me.m_dbEwE6.GetWriter("EcotracerScenario")

            While reader.Read()

                drow = writer.NewRow()

                drow("ScenarioID") = iScenarioID
                drow("ScenarioName") = Me.FixValue(reader, "Scenario")
                drow("Czero") = Me.FixValue(reader, "cZero", 0.0!)
                drow("Cinflow") = Me.FixValue(reader, "Cinflow", 0.0!)
                drow("Coutflow") = Me.FixValue(reader, "Coutflow", 0.0!)
                drow("Cdecay") = Me.FixValue(reader, "Cdecay", 0.0!)
                drow("LastSaved") = Me.ExtractLastSavedJulianDate(CStr(Me.FixValue(reader, "remarks", "")))

                ' Remember Ecotracer scenario ID mapping
                Me.HashKey(eDataTypes.EcotracerScenario, CStr(reader("Scenario"))) = iScenarioID

                writer.AddRow(drow)

                ' Map remarks
                Me.AddRemark(reader("remarks"), eDataTypes.EcotracerScenario, iScenarioID, eVarNameFlags.Name)

                ' JS 071124: References do not need to be imported for now
                'ImportRefCode("RefCode", "quickRefCatch")

                iScenarioID += 1

            End While

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

            Return True

        End Function

        Private Sub ImportEcotracerN(ByVal strModelName As String)

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iScenarioID As Integer = 0
            Dim iGroupID As Integer = 0

            reader = Me.m_dbEwE5.GetReader(String.Format("SELECT * from [EcoTracer N] where modelName='{0}'", strModelName))
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("EcotracerScenarioGroup")

            While reader.Read()

                ' Get tracer scenario ID
                iScenarioID = Me.HashKey(eDataTypes.EcotracerScenario, CStr(reader("Scenario")))
                ' Get group ID
                iGroupID = Me.HashKey(eDataTypes.EcoPathGroupInput, CStr(reader("groupName")))

                drow = writer.NewRow()

                drow("ScenarioID") = iScenarioID
                drow("EcopathGroupID") = iGroupID
                drow("CZero") = Me.FixValue(reader, "cZero", 0.0!)
                drow("Cimmig") = Me.FixValue(reader, "Cimmig", 0.0!)
                drow("Cenv") = Me.FixValue(reader, "Cenv", 0.0!)
                drow("Cdecay") = Me.FixValue(reader, "Cdecay", 0.0!)

                writer.AddRow(drow)

                ' No remarks or references to map

            End While

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

#End Region ' Ecotracer

#Region " Quotes "

        Private Sub ImportQuotes()

            Dim reader As IDataReader = Nothing
            Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
            Dim drow As DataRow = Nothing
            Dim iScenarioID As Integer = 0
            Dim iGroupID As Integer = 0

            reader = Me.m_dbEwE5.GetReader("SELECT * from Quote")
            If Object.ReferenceEquals(reader, Nothing) Then Return

            writer = Me.m_dbEwE6.GetWriter("Quote")

            While reader.Read()

                drow = writer.NewRow()

                drow("Quote") = reader("Quote")
                drow("Source") = reader("Source")

                writer.AddRow(drow)

            End While

            ' Clean up, store changes
            Me.m_dbEwE6.ReleaseWriter(writer)
            Me.m_dbEwE5.ReleaseReader(reader)

        End Sub

#End Region ' Quotes

#End Region ' Implementation 

#Region " Auxillary data "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a remark to the Auxillary data table
        ''' </summary>
        ''' <param name="objRemark">Remark text, may be DBNull</param>
        ''' <param name="dataType">The <see cref="eDataTypes">Core data type</see> 
        ''' representing the object to store the remark for.</param>
        ''' <param name="nID">The database ID of <paramref name="dataType">dataType</paramref>
        ''' to store the remark for.</param>
        ''' <param name="varName">The <see cref="eVarNameFlags">Core variable name</see>
        ''' to store the remark for.</param>
        ''' <param name="dataTypeSec">The <see cref="eDataTypes">Core data type</see> 
        ''' representing the secundary object (or index) to store the remark for.</param>
        ''' <param name="nIDSec">The database ID of <paramref name="dataTypeSec">dataTypeSec</paramref>.</param>
        ''' <remarks>
        ''' <para>All imported remarks should bear a relationship to an existing 
        ''' core object instance, variable type an optional subgroup.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub AddRemark(ByVal objRemark As Object, _
                ByVal dataType As eDataTypes, ByVal nID As Integer, _
                ByVal varName As eVarNameFlags, _
                Optional ByVal dataTypeSec As eDataTypes = eDataTypes.NotSet, Optional ByVal nIDSec As Integer = -1)

            Dim strRemark As String = ""

            ' No data? Abort
            If (objRemark Is Nothing) Then Return
            ' No data? Abort
            If Convert.IsDBNull(objRemark) Then Return
            ' Convert
            strRemark = objRemark.ToString().Trim()
            ' Still add?
            If String.IsNullOrEmpty(strRemark) Then Return
            ' Add
            Me.AddAuxillaryData(strRemark, cCore.NULL_VALUE, dataType, nID, varName, dataTypeSec, nIDSec)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a pedigree indicator to the Auxillary data table
        ''' </summary>
        ''' <param name="objPedigree">Pedigree indicator, may be DBNull</param>
        ''' <param name="dataType">The <see cref="eDataTypes">Core data type</see> 
        ''' representing the object to store the pedigree indicator for.</param>
        ''' <param name="nID">The database ID of <paramref name="dataType">dataType</paramref>
        ''' to store the remark for.</param>
        ''' <param name="varName">The <see cref="eVarNameFlags">Core variable name</see>
        ''' to store the pedigree indicator for.</param>
        ''' <param name="dataTypeSec">The <see cref="eDataTypes">Core data type</see> 
        ''' representing the secundary object (or index) to store the pedigree indicator for.</param>
        ''' <param name="nIDSec">The database ID of <paramref name="dataTypeSec">dataTypeSec</paramref>.</param>
        ''' -------------------------------------------------------------------
        Private Sub AddPedigree(ByVal objPedigree As Object, _
                 ByVal dataType As eDataTypes, ByVal nID As Integer, _
                 ByVal varName As eVarNameFlags, _
                 Optional ByVal dataTypeSec As eDataTypes = eDataTypes.NotSet, Optional ByVal nIDSec As Integer = -1)

            Dim iPedigree As Integer = cCore.NULL_VALUE
            ' No data? Abort
            If (objPedigree Is Nothing) Then Return
            ' No data? Abort
            If Convert.IsDBNull(objPedigree) Then Return
            ' Get actual value
            iPedigree = CInt(objPedigree)
            ' Valid?
            If (iPedigree < 0) Then Return

            ' Add
            Me.AddAuxillaryData("", iPedigree, dataType, nID, varName, dataTypeSec, nIDSec)

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Add a record to the Auxillary data table
        ''' </summary>
        ''' <param name="strRemark">Remark text to add.</param>
        ''' <param name="iPedigree">Pedigree indicator to add.</param>
        ''' <param name="dataType">The <see cref="eDataTypes">Core data type</see> 
        ''' representing the object to store the remark for.</param>
        ''' <param name="nID">The database ID of <paramref name="dataType">dataType</paramref>
        ''' to store the remark for.</param>
        ''' <param name="varName">The <see cref="eVarNameFlags">Core variable name</see>
        ''' to store the remark for.</param>
        ''' <param name="dataTypeSec">The <see cref="eDataTypes">Core data type</see> 
        ''' representing the secundary object (or index) to store the remark for.</param>
        ''' <param name="nIDSec">The database ID of <paramref name="dataTypeSec">dataTypeSec</paramref>.</param>
        ''' <remarks>
        ''' <para>All imported remarks should bear a relationship to an existing 
        ''' core object instance, variable type an optional subgroup.</para>
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Private Sub AddAuxillaryData(ByVal strRemark As String, ByVal iPedigree As Integer, _
            ByVal dataType As eDataTypes, ByVal nID As Integer, _
            ByVal varName As eVarNameFlags, _
            ByVal dataTypeSec As eDataTypes, ByVal nIDSec As Integer)

            Dim strValueID As String = ""
            Dim drow As DataRow = Nothing
            Dim bNewRow As Boolean = True

            ' Sanity check
            Debug.Assert(dataType > eDataTypes.NotSet And nID > 0, "Auxillary data cannot be added without a valid object identifier")

            strValueID = cValueID.GenerateAbstract(dataType, nID, varName, dataTypeSec, nIDSec)
            drow = Me.m_dtRemarks.Rows.Find(strValueID)

            If (drow Is Nothing) Then

                ' Both null? Abort!
                If String.IsNullOrEmpty(strRemark) And (iPedigree < 0) Then Return

                ' Create new row
                drow = Me.m_dtRemarks.NewRow()
                drow("ValueID") = strValueID
                bNewRow = True
            Else
                ' Try to complete values
                If String.IsNullOrEmpty(strRemark) Then strRemark = CStr(drow("Remark"))
                If (iPedigree < 0) Then iPedigree = CInt(drow("Pedigree"))

                ' Both null? Abort!
                If String.IsNullOrEmpty(strRemark) And (iPedigree < 0) Then Return

                ' Start editing existing row
                drow.BeginEdit()
                bNewRow = False
            End If

            ' Store remark text
            drow("Remark") = strRemark
            ' Store pedigree
            drow("Pedigree") = iPedigree

            ' Forge FK for cascading deletes 
            Select Case dataType
                Case eDataTypes.EwEModel
                    drow("ModelID") = nID
                Case eDataTypes.EcoPathGroupInput
                    drow("EcopathGroupID") = nID
                Case eDataTypes.EcoSimGroupInput
                    drow("EcosimGroupID") = nID
                Case eDataTypes.Stanza
                    drow("StanzaID") = nID
                Case eDataTypes.FleetInput
                    drow("FleetID") = nID
                Case eDataTypes.EcoSimScenario
                    drow("EcosimScenarioID") = nID
                Case eDataTypes.EggProd, eDataTypes.Forcing, eDataTypes.Mediation, _
                     eDataTypes.FishMort, eDataTypes.FishingEffort
                    drow("ShapeID") = nID
                Case eDataTypes.EcoSpaceScenario
                    drow("EcospaceScenarioID") = nID
                Case eDataTypes.EcotracerScenario
                    drow("EcotracerScenarioID") = nID
                Case Else
                    Debug.Assert(False, String.Format("Importer error: remark link to datatype {0} not implemented"), dataType.ToString())
            End Select

            If bNewRow Then
                ' Add new row 
                Me.m_dtRemarks.Rows.Add(drow)
            Else
                ' Update exsting row
                drow.EndEdit()
            End If

        End Sub

#End Region ' Auxillary data

    End Class

End Namespace
