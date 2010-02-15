#Region " Imports "

Option Strict On

Imports EwECore.Database
Imports EwECore.DataSources
Imports EwECore.Auxiliary
Imports System.Data
Imports System.Text
Imports EwEPlugin
Imports EwEUtils.Utilities
Imports EwEUtils.Database
Imports EwEUtils.Core

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' <see cref="IEwEDataSource">EwE datasource</see> implementation for reading
''' and writing Ecopath, Ecosim and Ecospace data from a database.
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class cDBDataSource
    Implements IEwEDataSource
    Implements IEcopathDataSource
    Implements IEcosimDatasource
    Implements IEcospaceDatasource
    Implements IEcotracerDatasource

#Region " Internal definitions "

    ''' <summary>Core components stored with Ecopath.</summary>
    Private Shared s_EcopathComponents() As eCoreComponentType = {eCoreComponentType.Core, eCoreComponentType.DataSource, eCoreComponentType.EcoPath}
    ''' <summary>Core components stored with Ecosim.</summary>
    Private Shared s_EcosimComponents() As eCoreComponentType = {eCoreComponentType.EcoSim, eCoreComponentType.ShapesManager, eCoreComponentType.TimeSeries, eCoreComponentType.EcoSimFitToTimeSeries, eCoreComponentType.EcoSimMonteCarlo, eCoreComponentType.PPIManager, eCoreComponentType.FishingPolicySearch, eCoreComponentType.MSE, eCoreComponentType.SearchObjective}
    ''' <summary>Core components stored with Ecospace.</summary>
    Private Shared s_EcospaceComponents() As eCoreComponentType = {eCoreComponentType.EcoSpace, eCoreComponentType.MPAOptimization}
    ''' <summary>Core components stored with Ecotracer.</summary>
    Private Shared s_EcotracerComponents() As eCoreComponentType = {eCoreComponentType.Ecotracer}

#End Region ' Internal definitions

#Region " Private vars "

    ''' <summary>The <see cref="cEwEDatabase">Database</see> connected to this datasource.</summary>
    Private m_db As cEwEDatabase = Nothing
    ''' <summary>The <see cref="cCore">core</see> connected to this datasource.</summary>
    Private m_core As cCore = Nothing
    ''' <summary>Datasource name</summary>
    Private m_strName As String = ""

#End Region ' Private vars

#Region " Generic "

    Public Sub New(ByRef db As cEwEDatabase)

        ' Pre
        Debug.Assert(db IsNot Nothing)
        ' Store ref to DB
        Me.m_db = db

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States whether the local OS supports connecting to a datasource
    ''' of a given type.
    ''' </summary>
    ''' <param name="dst"></param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Public Function IsOSSupported(ByVal dst As eDataSourceTypes) As Boolean _
        Implements DataSources.IEwEDataSource.IsOSSupported
        Return Me.m_db.CanConnect(dst)
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Open an existing DB.
    ''' </summary>
    ''' <param name="strName">Name of the DB database to open.</param>
    ''' <param name="core"><see cref="cCore">Core instance</see> that holds the 
    ''' datastructures to read to, and write from.</param>
    ''' <returns>True if opened successfully.</returns>
    ''' -------------------------------------------------------------------
    Public Function Open(ByVal strName As String, ByVal core As cCore, _
                         Optional ByVal datasourceType As eDataSourceTypes = eDataSourceTypes.NotSet) As eDatasourceAccessType _
                         Implements DataSources.IEwEDataSource.Open

        ' Attempt to open existing
        Dim atResult As eDatasourceAccessType = Me.m_db.Open(strName, datasourceType)
        ' Any luck?
        If atResult = eDatasourceAccessType.Success Then
            ' Store core
            Me.m_core = core
            Me.m_strName = strName
        End If
        ' Report succes
        Return atResult

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States whether a datasource is already open.
    ''' </summary>
    ''' <returns>True if the datasource is open.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsOpen() As Boolean _
             Implements IEwEDataSource.IsOpen
        Return Me.m_db.IsConnected
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Create a new DB, overwriting an existing file.
    ''' </summary>
    ''' <param name="strName">Name of the datasource to create.</param>
    ''' <param name="strModelName">Name to assign to the model.</param>
    ''' <param name="core"><see cref="cCore">Core instance</see> that holds the 
    ''' datastructures to read to, and write from.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function Create(ByVal strName As String, ByVal strModelName As String, ByVal core As cCore) As eDatasourceAccessType _
             Implements DataSources.IEwEDataSource.Create

        ' Create new db
        Dim atResult As eDatasourceAccessType = Me.m_db.Create(strName, strModelName, True)

        If atResult = eDatasourceAccessType.Success Then
            atResult = Me.Open(strName, core)
        End If

        Return atResult

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Close the DB.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function Close() As Boolean _
            Implements IEwEDataSource.Close

        ' Clear changed admin
        Me.ClearChanged()
        ' Close current db
        Me.m_db.Close()

        ' Forget the core
        Me.m_core = Nothing
        Me.m_strName = ""
        ' Clear version
        Me.m_sVersion = cDATABASE_NOVERSION

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the datasource is connected.
    ''' </summary>
    ''' <returns>True if connected.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsConnected() As Boolean
        Return Me.m_db.IsConnected()
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the connection to the <see cref="cEwEDatabase">database</see>
    ''' that this datasource operates on.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Connection() As Object _
            Implements DataSources.IEwEDataSource.Connection
        Get
            Return Me.m_db
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns a string representation of the datasource.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Overrides Function ToString() As String _
            Implements IEwEDataSource.ToString
        If Me.m_db Is Nothing Then Return ""
        Return Me.m_db.Name
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Switch an open datasource to a new database of the same type.
    ''' </summary>
    ''' <param name="strFileName">New FN to copy the DB to</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>This will open the new database if succesful.</remarks>
    ''' -------------------------------------------------------------------
    Public Function SaveAs(ByVal strFileName As String, ByVal strModelName As String) As eDatasourceAccessType
        Return Me.m_db.SaveAs(strFileName, strModelName, True)
    End Function

    ''' <summary>Unknown version.</summary>
    Public Const cDATABASE_NOVERSION As Single = -1.0!
    ''' <summary>Database version number.</summary>
    Private m_sVersion As Single = cDATABASE_NOVERSION

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Returns the version of the datasource.
    ''' </summary>
    ''' <returns>A version number, or <see cref="cDATABASE_NOVERSION">cDATABASE_NOVERSION</see> 
    ''' if the database is not connected.</returns>
    ''' -------------------------------------------------------------------
    Public Function Version() As Single Implements IEwEDataSource.Version
        If (Me.IsConnected = True) Then
            If (Me.m_sVersion = -1.0!) Then
                Me.m_sVersion = Me.m_db.GetVersion()
            End If
            Return Me.m_sVersion
        End If
        Return cDATABASE_NOVERSION
    End Function

    Public Function BeginTransaction() As Boolean _
        Implements DataSources.IEwEDataSource.BeginTransaction
        Return Me.m_db.BeginTransaction()
    End Function

    Public Function EndTransaction(ByVal bCommit As Boolean) As Boolean _
        Implements DataSources.IEwEDataSource.EndTransaction
        If bCommit Then
            Return Me.m_db.CommitTransaction()
        Else
            Return Me.m_db.RollbackTransaction
        End If
    End Function

#Region " Helper methods "

    'Private Function RunUpdates() As Boolean

    '    If Me.m_core.PluginManager IsNot Nothing Then

    '        ' Check if updates available
    '        If Me.m_core.PluginManager.HasDatabaseUpdates(db, 6.0) Then

    '            Select Case MsgBox(My.Resources.PROMPT_IMPORT_UPDATEBACKUP, MsgBoxStyle.YesNoCancel Or MsgBoxStyle.Question)
    '                Case MsgBoxResult.Yes
    '                    Try
    '                        Dim strDir As String = Path.GetDirectoryName(strFileName)
    '                        Dim strFile As String = Path.GetFileNameWithoutExtension(strFileName)
    '                        Dim strExt As String = Path.GetExtension(strFileName)

    '                        strFile = FileUtilities.ToValidFileName(String.Format("{0}_backup_{1}", strFile, Date.Now), False)

    '                        ' Create backup copy
    '                        File.Copy(strFileName, Path.Combine(strDir, strFile + strExt), True)
    '                    Catch ex As Exception
    '                        Me.m_core.Messages.SendMessage( _
    '                            New cMessage(String.Format(My.Resources.PROMPT_BACKUPFAILED, strFileName, ex.Message), _
    '                                         eMessageType.DataImport, _
    '                                         eCoreComponentType.Core, _
    '                                         eMessageImportance.Warning))
    '                        Return False
    '                    End Try
    '                    ' Fall through

    '                Case MsgBoxResult.No
    '                    ' Update existing copy
    '                    ' Fall through 

    '                Case MsgBoxResult.Cancel
    '                    ' Leave DB alone, don't open
    '                    Return False

    '            End Select

    '            ' Run all available updates on the new EwE6 database
    '            Dim dbUpd As New cDatabaseUpdater(6.0)
    '            dbUpd.UpdateDatabase(db, Me.m_core.PluginManager)
    '            dbUpd = Nothing

    '        End If
    '    End If

    'End Function

    Private Overloads Function CopyEcopathTo(ByVal ds As DataSources.IEcopathDataSource) As Boolean Implements DataSources.IEcopathDataSource.CopyTo
        Return False
    End Function

    Private Overloads Function CopyEcosimTo(ByVal ds As DataSources.IEcosimDatasource) As Boolean Implements DataSources.IEcosimDatasource.CopyTo
        Return False
    End Function

    Private Overloads Function CopyEcospaceTo(ByVal ds As DataSources.IEcospaceDatasource) As Boolean Implements DataSources.IEcospaceDatasource.CopyTo
        Return False
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <para>Helper method, splits a string of numbers into an array of strings,
    ''' each string representing a number. This method assumes that numbers are
    ''' separated by a ASCII character 32, a single space.</para>
    ''' </summary>
    ''' <param name="strNumberString">A comma-seoarated string of numbers to split.</param>
    ''' <returns>
    ''' An array of strings, each representing a number in the string.
    ''' </returns>
    ''' <remarks>
    ''' <para>This method tries to resolve number formatting issues, introduced
    ''' in models written by systems with different locale settings.</para>
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function SplitNumberString(ByRef strNumberString As String) As String()
        Dim charSeparators() As Char = {" "c}
        If strNumberString.IndexOf(CChar(",")) > -1 Then strNumberString = strNumberString.Replace(CChar(","), CChar("."))
        Return strNumberString.Trim().Split(charSeparators, StringSplitOptions.RemoveEmptyEntries)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, reads data from a column that may not exist. In that case,
    ''' an optional default value is returned
    ''' </summary>
    ''' <param name="reader">The <see cref="IDataReader">IDataReader</see> to read from.</param>
    ''' <param name="strField">The name of the DB field (column) to read.</param>
    ''' <param name="objValueDefault">A default value to return if the field could not be read.</param>
    ''' <returns>The value of the requested column, or the provided default if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Private Function ReadSafe(ByVal reader As IDataReader, ByVal strField As String, Optional ByVal objValueDefault As Object = Nothing) As Object

        Dim objResult As Object = Nothing

        If reader Is Nothing Then Return objValueDefault

        Try
            objResult = reader.Item(strField)
        Catch ex As InvalidOperationException
            Console.WriteLine("DB: field '{0}' has no value, returning provided default '{1}'", strField, objValueDefault)
        Catch ex As IndexOutOfRangeException
            Console.WriteLine("DB: field '{0}' not found in table, returning provided default '{1}'", strField, objValueDefault)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Console.WriteLine("DB: Exception {2} occurred while accessing field '{0}', returning provided default '{1}'", strField, objValueDefault, ex.ToString)
        End Try

        If (Object.ReferenceEquals(objResult, Nothing)) Then
            objResult = objValueDefault
        End If

        If (Convert.IsDBNull(objResult)) Then
            objResult = objValueDefault
        End If

        Return objResult
    End Function

    Private Function BuildWhereClause(ByVal strVariable As String, ByVal astrValues() As String) As String

        Debug.Assert(Not astrValues Is Nothing)
        Debug.Assert(Not astrValues.Length = 0)

        Dim sbFilter As New StringBuilder
        For iValue As Integer = 0 To astrValues.Length - 1
            If iValue > 0 Then
                sbFilter.Append(" OR ")
            End If
            sbFilter.Append(String.Format("({0}='{1}')", strVariable, astrValues(iValue)))
        Next
        Return sbFilter.ToString()

    End Function

#End Region ' Helper methods

#End Region ' Generic

#Region " Change management "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States whether the datasource has unsaved changes that do not relate
    ''' to any of the supported sub-models.
    ''' </summary>
    ''' <returns>True if the datasource has pending changes.</returns>
    ''' -------------------------------------------------------------------
    Friend Function IsChanged() As Boolean Implements DataSources.IEwEDataSource.IsModified
        If Not Me.IsConnected() Then Return False
        Return Me.IsChanged(Nothing)
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Clears all changed information for either a given data type or for 
    ''' the entire datasource.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Friend Sub ClearChanged() Implements IEwEDataSource.ClearChanged
        Me.ClearChanged(Nothing)
    End Sub

    ''' <summary>Dictionary of changed core components.</summary>
    Private m_dictChangedComponents As New Dictionary(Of eCoreComponentType, Boolean)

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Flag a core object as changed in the datasource.
    ''' </summary>
    ''' <param name="cc">The <see cref="eDataTypes">Type</see> of the object that changed.</param>
    ''' -------------------------------------------------------------------
    Public Sub SetChanged(ByVal cc As eCoreComponentType) _
            Implements IEwEDataSource.SetChanged
        Me.m_dictChangedComponents.Item(cc) = True
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; states whether there are pending changes for a particular
    ''' <see cref="eCoreComponentType">EwE component</see>.
    ''' </summary>
    ''' <param name="acomponents">The EwE components to check.</param>
    ''' <returns>True if there are any pending changes for any datatype that
    ''' belongs to this EwE component.</returns>
    ''' -------------------------------------------------------------------
    Private Function IsChanged(ByVal acomponents As eCoreComponentType()) As Boolean
        Dim bIsChanged As Boolean = False
        If (acomponents Is Nothing) Then
            Return (Me.m_dictChangedComponents.Count > 0)
        Else
            For Each component As eCoreComponentType In acomponents
                bIsChanged = bIsChanged Or Me.m_dictChangedComponents.ContainsKey(component)
            Next
        End If
        Return bIsChanged
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Clears the changed administration for all datatypes that belong to
    ''' a given <see cref="eCoreComponentType">EwE component</see>.
    ''' </summary>
    ''' <param name="acomponents">The EwE components to clear the changed
    ''' adminsitration for.</param>
    ''' -------------------------------------------------------------------
    Private Sub ClearChanged(ByVal acomponents As eCoreComponentType())

        If (acomponents Is Nothing) Then
            Me.m_dictChangedComponents.Clear()
        Else
            For Each component As eCoreComponentType In acomponents
                If Me.m_dictChangedComponents.ContainsKey(component) Then
                    Me.m_dictChangedComponents.Remove(component)
                End If
            Next component
        End If
    End Sub

#End Region ' Change management

#Region " Private helper bits "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, maintains a list of database ID mappings per datatype. Use this class
    ''' when duplicating objects in the database. Via the mappings, newly created objects
    ''' (with new DBID values) can be saved using content of their originals (with old
    ''' DBID values)
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cIDMappings

        ''' <summary>Array of ID mappings, per datatype.</summary>
        Private m_dictMappings() As Dictionary(Of Integer, Integer)

        Public Sub New()
            Me.Initialize()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Initialize the ID mapper by allocating space for the lookup tables.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub Initialize()
            ' Allocate space
            Dim nNumDatatypes As Integer = System.Enum.GetValues(GetType(eDataTypes)).Length
            ReDim Me.m_dictMappings(nNumDatatypes)
            For i As Integer = 0 To nNumDatatypes
                Me.m_dictMappings(i) = New Dictionary(Of Integer, Integer)
            Next
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Add an ID mapping for a specific object.
        ''' </summary>
        ''' <param name="dt">The <see cref="eDataTypes">Data Type</see> of the object to map.</param>
        ''' <param name="iIDOrg">The original database ID of the object. This is the value
        ''' under which the object is stored in the current database, and how it is currently
        ''' known in the core database ID arrays.</param>
        ''' <param name="iIDNew">The mapped database ID of the object. This is the value that
        ''' has been assigned by the datasource when creating a new instance of the object
        ''' in the database.</param>
        ''' -----------------------------------------------------------------------
        Public Sub Add(ByVal dt As eDataTypes, ByVal iIDOrg As Integer, ByVal iIDNew As Integer)
            ' Only add useful mappings, please!
            If iIDOrg = iIDNew Then Return

            Try
                Dim d As Dictionary(Of Integer, Integer) = Me.m_dictMappings(CInt(dt))

                ' Development-time sanity checks.
                Debug.Assert(d IsNot Nothing, String.Format("cIDMappings.Add: no dictionary for datatype {0} ({1}), something is very wrong!", dt.ToString, CInt(dt)))
                Debug.Assert(Not d.ContainsKey(iIDOrg), String.Format("cIDMappings: DBID {0} is already used to define a mapping", iIDOrg))
                Debug.Assert(Not d.ContainsValue(iIDNew), String.Format("cIDMappings: DBID {0} already mapped to", iIDNew))

                d.Add(iIDOrg, iIDNew)

            Catch ex As Exception
                ' Development-time panic event.
                Debug.Assert(False, String.Format("cIDMappings.Add: ID Mapping failed '{0}'", ex.Message))
            End Try
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns a mapped ID for a specific core object. If no mapping exists, the
        ''' original ID is returned.
        ''' </summary>
        ''' <param name="dt">The <see cref="eDataTypes">Data Type</see> of the object
        ''' to retrieve the mapping for.</param>
        ''' <param name="iIDOrg">The original database ID of the object.</param>
        ''' <returns>A mapped ID if present, or the original ID if no mapping was found.</returns>
        ''' -----------------------------------------------------------------------
        Public Function GetID(ByVal dt As eDataTypes, ByVal iIDOrg As Integer) As Integer
            Try
                Dim d As Dictionary(Of Integer, Integer) = Me.m_dictMappings(CInt(dt))
                If d.ContainsKey(iIDOrg) Then
                    Return d.Item(iIDOrg)
                End If
            Catch ex As Exception
                ' Woops
            End Try
            Return iIDOrg
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Returns whether a mapping exists for a core object.
        ''' </summary>
        ''' <param name="dt">The <see cref="eDataTypes">Data Type</see> of the object to
        ''' test a mapping for.</param>
        ''' <param name="iIDOrg">The original database ID of the object to test.</param>
        ''' <returns>True if a mapping exists.</returns>
        ''' -----------------------------------------------------------------------
        Public Function HasMapping(ByVal dt As eDataTypes, ByVal iIDOrg As Integer) As Boolean
            Dim d As Dictionary(Of Integer, Integer) = Me.m_dictMappings(CInt(dt))
            Return d.ContainsKey(iIDOrg)
        End Function

    End Class

    Private Shared Function GetJulianDate() As Single
        Return CSng(Date.Now().ToOADate())
    End Function

#End Region ' Private helper bits

#Region " Messages "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Logs a message
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub LogMessage(ByVal strMessage As String, _
            Optional ByVal msgType As eMessageType = eMessageType.DataModified, _
            Optional ByVal msgImportance As eMessageImportance = eMessageImportance.Information)

        If (Me.m_core IsNot Nothing) Then
            Me.m_core.m_publisher.AddMessage(New cMessage(strMessage, msgType, eCoreComponentType.DataSource, msgImportance))
        End If
        Console.WriteLine(strMessage)

    End Sub

#End Region ' Messages

#Region " Generic datasource "

#Region " Cleanup "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Compact the data in the datasource. Please ensure that this operation
    ''' is possible via <see cref="CanCompact">CanCompact</see>.
    ''' </summary>
    ''' <param name="strTarget">The destination to compact the datasource to.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Public Function Compact(ByVal strTarget As String) As eDatasourceAccessType _
        Implements DataSources.IEwEDataSource.Compact
        Return Me.m_db.Compact(strTarget, strTarget)
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Returns whether the data underlying the datasource can be compacted.
    ''' </summary>
    ''' <param name="strTarget">The destination to test whether the datasource 
    ''' can compact to.</param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function CanCompact(ByVal strTarget As String) As Boolean _
        Implements IEwEDataSource.CanCompact
        Return Me.m_db.CanCompact(strTarget, strTarget)
    End Function

#End Region ' Cleanup

#End Region ' Generic datasource

#Region " EwEModel "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Initiates a full load of an EwE model.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function LoadModel() As Boolean _
         Implements IEcopathDataSource.LoadModel

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim bSucces As Boolean = True

        bSucces = Me.LoadModelInfo()
        If bSucces = False Then Return False

        bSucces = bSucces And Me.LoadEcopathGroups()
        bSucces = bSucces And Me.LoadEcopathFleetInfo()
        bSucces = bSucces And Me.LoadParticleSizeDistribution()
        bSucces = bSucces And Me.LoadAuxillaryData()

        ecopathDS.bInitialized = bSucces

        ecopathDS.onPostInitialization()

        bSucces = bSucces And Me.LoadEcosimScenarioDefinitions()
        bSucces = bSucces And Me.LoadEcospaceScenarioDefinitions()
        bSucces = bSucces And Me.LoadEcotracerScenarioDefinitions()
        bSucces = bSucces And Me.LoadTimeSeriesDatasets()

        ' Clear changed admin
        Me.ClearChanged(s_EcopathComponents)

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Initiates a save of an EwE model
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function SaveModel() As Boolean _
             Implements IEcopathDataSource.SaveModel

        Dim bSucces As Boolean = Me.m_db.BeginTransaction()

        ' Start saving
        bSucces = Me.SaveModelInfo()
        bSucces = bSucces And Me.SaveEcopathGroups()
        bSucces = bSucces And Me.SaveEcopathFleetInfo()
        bSucces = bSucces And Me.SaveParticleSizeDistribution()
        bSucces = bSucces And Me.SaveAuxillaryData()
        bSucces = bSucces And Me.SaveEcosimScenarioDefinitions()
        bSucces = bSucces And Me.SaveEcospaceScenarioDefinitions()
        bSucces = bSucces And Me.SaveEcotracerScenarioDefinitions()

        If bSucces Then
            bSucces = Me.m_db.CommitTransaction()
        Else
            Me.m_db.RollbackTransaction()
        End If

        ' Save succesful?
        If bSucces Then
            ' #Yes: Clear ecopath changed flags
            Me.ClearChanged(s_EcopathComponents)
        End If

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, loads model info for the current model.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function LoadModelInfo() As Boolean

        Dim reader As IDataReader = Me.m_db.GetReader("SELECT * FROM EcopathModel")
        Dim bSucces As Boolean = True

        ' Crash prevention check
        If Object.ReferenceEquals(reader, Nothing) Then
            'Debug.Assert(False, "Failed to access table EcopathModel")
            Return False
        End If

        Try
            ' There is only one model in an EwE6 database
            reader.Read()

            Me.m_core.m_EwEModelDBID = CInt(reader("ModelID"))
            Me.m_core.m_EwEModelName = CStr(reader("Name"))
            Me.m_core.m_EwEModelDescription = CStr(reader("Description"))
            Me.m_core.m_EwEModelAuthor = CStr(Me.ReadSafe(reader, "Author", ""))
            Me.m_core.m_EwEModelContact = CStr(Me.ReadSafe(reader, "Contact", ""))
            Me.m_core.m_EwEModelArea = CSng(Me.ReadSafe(reader, "Area", 1.0))
            Me.m_core.m_EwEModelNumDigits = CInt(reader("NumDigits"))
            Me.m_core.m_EwEModelGroupDigits = CBool(Me.ReadSafe(reader, "GroupDigits", False))
            Me.m_core.m_EwEModelUnitCurrency = DirectCast(Me.ReadSafe(reader, "UnitCurrency", eUnitCurrencyType.WetWeight), eUnitCurrencyType)
            Me.m_core.m_EwEModelUnitCurrencyCustom = CStr(Me.ReadSafe(reader, "UnitCurrencyCustom", ""))
            Me.m_core.m_EwEModelUnitTime = DirectCast(Me.ReadSafe(reader, "UnitTime", eUnitTimeType.Year), eUnitTimeType)
            Me.m_core.m_EwEModelUnitTimeCustom = CStr(Me.ReadSafe(reader, "UnitTimeCustom", ""))
            Me.m_core.m_EwEModelUnitMonetary = DirectCast(Me.ReadSafe(reader, "UnitMonetary", eUnitMonetaryType.EUR), eUnitMonetaryType)
            'Me.m_core.m_EwEModelUnitMonetaryCustom = CStr(Me.ReadSafe(reader, "UnitTimeCustom", ""))
            Me.m_core.m_EwEModelLastSaved = CSng(Me.ReadSafe(reader, "LastSaved", 0))

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EcopathModel", ex.Message))
            bSucces = False
        End Try

        Me.m_db.ReleaseReader(reader)
        reader = Nothing

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Updates model info into the database.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveModelInfo() As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bNewRow As Boolean = False
        Dim bSucces As Boolean = True

        Try
            ' This will no longer work because of tables linking to ModelID
            'Me.m_db.Execute("DELETE * FROM EcopathModel")
            writer = Me.m_db.GetWriter("EcopathModel")
            dt = writer.GetDataTable()

            drow = dt.Rows.Find(Me.m_core.m_EwEModelDBID)

            bNewRow = (drow Is Nothing)
            If bNewRow Then
                drow = writer.NewRow()
            Else
                drow.BeginEdit()
            End If

            drow("Name") = Me.m_core.m_EwEModelName
            drow("Description") = Me.m_core.m_EwEModelDescription
            drow("Author") = Me.m_core.m_EwEModelAuthor
            drow("Contact") = Me.m_core.m_EwEModelContact
            drow("Area") = Me.m_core.m_EwEModelArea
            drow("NumDigits") = Me.m_core.m_EwEModelNumDigits
            drow("GroupDigits") = Me.m_core.m_EwEModelGroupDigits
            drow("UnitCurrency") = Me.m_core.m_EwEModelUnitCurrency
            drow("UnitCurrencyCustom") = Me.m_core.m_EwEModelUnitCurrencyCustom
            drow("UnitTime") = Me.m_core.m_EwEModelUnitTime
            drow("UnitTimeCustom") = Me.m_core.m_EwEModelUnitTimeCustom
            drow("UnitMonetary") = Me.m_core.m_EwEModelUnitMonetary
            drow("LastSaved") = cDBDataSource.GetJulianDate()

            If bNewRow Then
                writer.AddRow(drow)
            Else
                drow.EndEdit()
            End If

            writer.Commit()

        Catch ex As Exception
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the list of available Ecosim scenarios.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' Note that this will NOT load any actual Ecosim scenario. Scenario definitions 
    ''' merely provide a preview of available Ecosim scenarios in the database.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcosimScenarioDefinitions() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim reader As IDataReader = Me.m_db.GetReader("SELECT * FROM EcosimScenario")
        Dim iScenario As Integer = 1
        Dim bSucces As Boolean = True

        ecopathDS.NumEcosimScenarios = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM EcoSimScenario"))
        ecopathDS.RedimEcosimScenarios()

        If ecopathDS.NumEcosimScenarios = 0 Then Return bSucces

        Try
            While reader.Read()
                ecopathDS.EcosimScenarioDBID(iScenario) = CInt(reader("ScenarioID"))
                ecopathDS.EcosimScenarioName(iScenario) = CStr(reader("ScenarioName"))
                ecopathDS.EcosimScenarioDescription(iScenario) = CStr(reader("Description"))
                ecopathDS.EcosimScenarioAuthor(iScenario) = CStr(Me.ReadSafe(reader, "Author", ""))
                ecopathDS.EcosimScenarioContact(iScenario) = CStr(Me.ReadSafe(reader, "Contact", ""))
                ecopathDS.EcosimScenarioLastSaved(iScenario) = CSng(Me.ReadSafe(reader, "LastSaved", 0))
                iScenario += 1
            End While
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading ecosim scenario definition {1}", ex.Message, iScenario))
            bSucces = False
        End Try

        Me.m_db.ReleaseReader(reader)
        reader = Nothing

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves the list of available Ecosim scenarios.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' Note that this will NOT save any actual Ecosim scenario. Here, only the
    ''' Ecosim scenario preview information is updated.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function SaveEcosimScenarioDefinitions() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenario As Integer = 0
        Dim bSucces As Boolean = True

        Try
            writer = Me.m_db.GetWriter("EcosimScenario")
            dt = writer.GetDataTable()

            For iScenario = 1 To ecopathDS.NumEcosimScenarios

                drow = dt.Rows.Find(ecopathDS.EcosimScenarioDBID(iScenario))
                Debug.Assert(drow IsNot Nothing, String.Format("Cannot find existing row for ecosim scenario ID {0}", ecopathDS.EcosimScenarioDBID(iScenario)))

                drow.BeginEdit()
                drow("ScenarioName") = ecopathDS.EcosimScenarioName(iScenario)
                drow("Description") = ecopathDS.EcosimScenarioDescription(iScenario)
                drow("Author") = ecopathDS.EcosimScenarioAuthor(iScenario)
                drow("Contact") = ecopathDS.EcosimScenarioContact(iScenario)
                drow("LastSaved") = ecopathDS.EcosimScenarioLastSaved(iScenario)
                drow.EndEdit()

            Next

        Catch ex As Exception
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the list of available Ecospace scenarios.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' Note that this will NOT load any actual Ecospace scenario. Scenario definitions 
    ''' merely provide a preview of available Ecospace scenarios in the database.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcospaceScenarioDefinitions() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim reader As IDataReader = Me.m_db.GetReader("SELECT * FROM EcospaceScenario")
        Dim iScenario As Integer = 1
        Dim bSucces As Boolean = True

        ecopathDS.NumEcospaceScenarios = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM EcospaceScenario"))
        ecopathDS.RedimEcospaceScenarios()

        If ecopathDS.NumEcospaceScenarios = 0 Then Return bSucces

        Try
            While reader.Read()
                ecopathDS.EcospaceScenarioDBID(iScenario) = CInt(reader("ScenarioID"))
                ecopathDS.EcospaceScenarioName(iScenario) = CStr(reader("ScenarioName"))
                ecopathDS.EcospaceScenarioDescription(iScenario) = CStr(reader("Description"))
                ecopathDS.EcospaceScenarioAuthor(iScenario) = CStr(Me.ReadSafe(reader, "Author", ""))
                ecopathDS.EcospaceScenarioContact(iScenario) = CStr(Me.ReadSafe(reader, "Contact", ""))
                ecopathDS.EcospaceScenarioLastSaved(iScenario) = CSng(Me.ReadSafe(reader, "LastSaved", 0))
                iScenario += 1
            End While
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading ecospace scenario definition {1}", ex.Message, iScenario))
            bSucces = False
        End Try

        Me.m_db.ReleaseReader(reader)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves the list of available Ecospace scenarios.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' Note that this will NOT save any actual Ecospace scenario. Here, only the
    ''' Ecospace scenario preview information is updated.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function SaveEcospaceScenarioDefinitions() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenario As Integer = 0
        Dim bSucces As Boolean = True

        Try
            writer = Me.m_db.GetWriter("EcospaceScenario")
            dt = writer.GetDataTable()

            For iScenario = 1 To ecopathDS.NumEcospaceScenarios

                drow = dt.Rows.Find(ecopathDS.EcospaceScenarioDBID(iScenario))
                Debug.Assert(drow IsNot Nothing, String.Format("Cannot find existing row for ecospace scenario ID {0}", ecopathDS.EcospaceScenarioDBID(iScenario)))

                drow.BeginEdit()
                drow("ScenarioName") = ecopathDS.EcospaceScenarioName(iScenario)
                drow("Description") = ecopathDS.EcospaceScenarioDescription(iScenario)
                drow("Author") = ecopathDS.EcospaceScenarioAuthor(iScenario)
                drow("Contact") = ecopathDS.EcospaceScenarioContact(iScenario)
                drow("LastSaved") = ecopathDS.EcospaceScenarioLastSaved(iScenario)
                drow.EndEdit()

            Next

        Catch ex As Exception
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the list of available Ecotracer scenarios.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' Note that this will NOT load any actual Ecotracer scenario. Scenario definitions 
    ''' merely provide a preview of available Ecotracer scenarios in the database.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcotracerScenarioDefinitions() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim reader As IDataReader = Me.m_db.GetReader("SELECT * FROM EcotracerScenario")
        Dim iScenario As Integer = 1
        Dim bSucces As Boolean = True

        ecopathDS.NumEcotracerScenarios = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM EcotracerScenario"))
        ecopathDS.RedimEcotracerScenarios()

        If ecopathDS.NumEcotracerScenarios = 0 Then Return bSucces

        Try
            While reader.Read()
                ecopathDS.EcotracerScenarioDBID(iScenario) = CInt(reader("ScenarioID"))
                ecopathDS.EcotracerScenarioName(iScenario) = CStr(reader("ScenarioName"))
                ecopathDS.EcotracerScenarioDescription(iScenario) = CStr(reader("Description"))
                ecopathDS.EcotracerScenarioAuthor(iScenario) = CStr(Me.ReadSafe(reader, "Author", ""))
                ecopathDS.EcotracerScenarioContact(iScenario) = CStr(Me.ReadSafe(reader, "Contact", ""))
                ecopathDS.EcotracerScenarioLastSaved(iScenario) = CSng(Me.ReadSafe(reader, "LastSaved", 0))
                iScenario += 1
            End While
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading ecospace scenario definition {1}", ex.Message, iScenario))
            bSucces = False
        End Try

        Me.m_db.ReleaseReader(reader)

        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Saves the list of available Ecotracer scenarios.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' Note that this will NOT save any actual Ecotracer scenario. Here, only the
    ''' Ecotracer scenario preview information is updated.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function SaveEcotracerScenarioDefinitions() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenario As Integer = 0
        Dim bSucces As Boolean = True

        Try
            writer = Me.m_db.GetWriter("EcotracerScenario")
            dt = writer.GetDataTable()

            For iScenario = 1 To ecopathDS.NumEcotracerScenarios

                drow = dt.Rows.Find(ecopathDS.EcotracerScenarioDBID(iScenario))
                Debug.Assert(drow IsNot Nothing, String.Format("Cannot find existing row for ecotracer scenario ID {0}", ecopathDS.EcotracerScenarioDBID(iScenario)))

                drow.BeginEdit()
                drow("ScenarioName") = ecopathDS.EcotracerScenarioName(iScenario)
                drow("Description") = ecopathDS.EcotracerScenarioDescription(iScenario)
                drow("Author") = ecopathDS.EcotracerScenarioAuthor(iScenario)
                drow("Contact") = ecopathDS.EcotracerScenarioContact(iScenario)
                drow("LastSaved") = ecopathDS.EcotracerScenarioLastSaved(iScenario)
                drow.EndEdit()

            Next

        Catch ex As Exception
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        Return bSucces
    End Function

#Region " Pedigree "

#Region " Load "

    Private Function LoadPedigreeLevels() As Boolean

        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim reader As IDataReader = Me.m_db.GetReader("SELECT * FROM Pedigree ORDER BY Sequence ASC")
        Dim iLevel As Integer = 1
        Dim bSucces As Boolean = True

        ' Init data structure
        ecopathDS.NumPedigreeLevels = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM Pedigree"))

        ' Allocate space
        ecopathDS.RedimPedigreeLevels()

        While reader.Read()

            Try
                ecopathDS.PedigreeLevelDBID(iLevel) = CInt(reader("LevelID"))
                ecopathDS.PedigreeLevelVarName(iLevel) = cin.GetVarName(CStr(reader("VarName")))
                ecopathDS.PedigreeLevelIndexValue(iLevel) = CSng(reader("IndexValue"))
                ecopathDS.PedigreeLevelConfidence(iLevel) = CSng(reader("Confidence"))
                ecopathDS.PedigreeLevelDescription(iLevel) = CStr(reader("Description"))

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading pedigree level {1}", ex.Message, iLevel))
                bSucces = False
            End Try

            iLevel += 1

        End While

        ' Sanity check
        Debug.Assert(iLevel - 1 = ecopathDS.NumPedigreeLevels)

        Me.m_db.ReleaseReader(reader)
        reader = Nothing

        Return bSucces
    End Function

#End Region ' Load

#Region " Save "

    Public Function SavePedigreeLevels() As Boolean

        Dim cin As cCoreEnumNamesIndex = cCoreEnumNamesIndex.GetInstance()
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim iLevel As Integer = 0
        Dim bSucces As Boolean = True

        Try
            writer = Me.m_db.GetWriter("Pedigree")
            dt = writer.GetDataTable()

            For iLevel = 1 To ecopathDS.NumPedigreeLevels

                ' Find existing row
                drow = dt.Rows.Find(ecopathDS.PedigreeLevelDBID(iLevel))
                Debug.Assert(drow IsNot Nothing, String.Format("Cannot find existing row for pedigree level {0}", ecopathDS.PedigreeLevelDBID(iLevel)))

                drow.BeginEdit()
                drow("Sequence") = iLevel
                drow("VarName") = CStr(cin.GetVarName(ecopathDS.PedigreeLevelVarName(iLevel)))
                drow("IndexValue") = ecopathDS.PedigreeLevelIndexValue(iLevel)
                drow("Confidence") = ecopathDS.PedigreeLevelConfidence(iLevel)
                drow("Description") = ecopathDS.PedigreeLevelDescription(iLevel)

                drow.EndEdit()

            Next iLevel

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving pedigree level", ex.Message))
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        Return bSucces
    End Function

#End Region ' Save

#Region " Modify "

    Public Function AddPedigreeLevel(ByVal iPosition As Integer, ByVal varName As eVarNameFlags, ByVal sIndexValue As Single, ByVal sConfidence As Single, ByVal strDescription As String, ByRef iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.AddPedigreeLevel

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            Try
                iDBID = CInt(Me.m_db.GetValue("SELECT MAX(LevelID) FROM Pedigree")) + 1
            Catch
                iDBID = 1
            End Try

            ' Start writing, protect sequence
            writer = Me.m_db.GetWriter("Pedigree", "Sequence")

            ' Get new row to add
            drow = writer.NewRow()
            drow("LevelID") = iDBID
            drow("VarName") = CInt(varName)
            drow("IndexValue") = sIndexValue
            drow("Confidence") = sConfidence
            drow("Description") = strDescription
            drow("Sequence") = iPosition

            ' Commit to db
            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        bSucces = bSucces And Me.LoadPedigreeLevels()

        Return bSucces

    End Function

    Public Function MovePedigreeLevel(ByVal iDBID As Integer, ByVal iPosition As Integer) As Boolean _
            Implements IEcopathDataSource.MovePedigreeLevel

        Dim bSucces As Boolean = True
        Try
            Me.m_db.Execute(String.Format("UPDATE Pedigree SET Sequence={1} WHERE (LevelID={0})", iDBID, iPosition))
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while moving PedigreeLevel {1}", ex.Message, iDBID))
            bSucces = False
        End Try
        Return bSucces

    End Function

    Public Function RemovePedigreeLevel(ByVal iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.RemovePedigreeLevel

        Dim bSucces As Boolean = True
        Try
            Me.m_db.Execute(String.Format("DELETE FROM Pedigree WHERE (LevelID={0})", iDBID))
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing PedigreeLevel {1}", ex.Message, iDBID))
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Modify

#End Region ' Pedigree

#Region " PSD "

#Region " Load "

    Private Function LoadParticleSizeDistribution() As Boolean

        Dim psdDS As cPSDDatastructures = Me.m_core.m_PSDData
        Dim reader As IDataReader = Me.m_db.GetReader("SELECT * FROM EcopathPSD")
        Dim bSucces As Boolean = True

        If reader IsNot Nothing Then

            reader.Read()
            Try

                psdDS.NAgeSteps = CInt(Me.ReadSafe(reader, "NumAgeSteps", 101))
                psdDS.MortalityType = CType(CInt(Me.ReadSafe(reader, "MortalityType", 0)), ePSDMortalityTypes)
                psdDS.NWeightClasses = CInt(Me.ReadSafe(reader, "NumWeightClasses", 25))
                psdDS.FirstWeightClass = CSng(Me.ReadSafe(reader, "FirstWeightClass", 0.125))
                psdDS.ClimateType = CType(CInt(Me.ReadSafe(reader, "ClimateType", eClimateTypes.Temperate)), eClimateTypes)

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading EcopathPSD", ex.Message))
                bSucces = False
            End Try

            Me.m_db.ReleaseReader(reader)
            reader = Nothing

        End If

        Return bSucces
    End Function

#End Region ' Load

#Region " Save "

    Public Function SaveParticleSizeDistribution() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim psdDS As cPSDDatastructures = Me.m_core.m_PSDData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bNewRow As Boolean = False
        Dim bSucces As Boolean = True

        Try
            writer = Me.m_db.GetWriter("EcopathPSD")
            dt = writer.GetDataTable()

            ' Find existing row
            drow = dt.Rows.Find(Me.m_core.m_EwEModelDBID)
            bNewRow = (drow Is Nothing)

            If bNewRow Then
                drow = dt.NewRow()
                drow("ModelID") = Me.m_core.m_EwEModelDBID
            Else
                drow.BeginEdit()
            End If

            drow("NumAgeSteps") = psdDS.NAgeSteps
            drow("MortalityType") = psdDS.MortalityType
            drow("NumWeightClasses") = psdDS.NWeightClasses
            drow("FirstWeightClass") = psdDS.FirstWeightClass
            drow("ClimateType") = psdDS.ClimateType

            If bNewRow Then
                writer.AddRow(drow)
            Else
                drow.EndEdit()
            End If

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving PSD", ex.Message))
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        Return bSucces
    End Function

#End Region ' Save

#End Region ' PSD

#End Region ' EwEModel

#Region " Stanza "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load all model-generic stanza information.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadStanza() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim rdStanza As IDataReader = Nothing
        Dim rdLifeStage As IDataReader = Nothing
        Dim iStanza As Integer = 0
        Dim iLifeStage As Integer = 0
        Dim iGroup As Integer = 0
        Dim sTemp As Single = 0.0
        Dim bSucces As Boolean = True

        ' Count the number of rows in StanzaInfo; this is the number of split groups that we're going to work with
        stanzaDS.Nsplit = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM Stanza"))
        ' Get max no of stanza
        stanzaDS.MaxStanza = 0

        If (stanzaDS.Nsplit > 0) Then
            Try
                ' Get the highest number of groups in all split groups. Note that the sequence value field is not used here.
                stanzaDS.MaxStanza = CInt(Me.m_db.GetValue("SELECT MAX(NumGroups) FROM (SELECT COUNT(*) AS NumGroups FROM StanzaLifeStage GROUP BY StanzaID)"))
            Catch ex As Exception
                ' There are probably no stanza groups defined yet
                stanzaDS.MaxStanza = 0
            End Try
        Else

        End If
        ' Get the number of groups from ecopath
        stanzaDS.nGroups = ecopathDS.NumGroups

        If stanzaDS.MaxAgeSplit < cCore.MAX_AGE Then
            'VILLY: NEED TO REPLACE THIS WITH DYNAMIC CALCULATION ALLOWING FOR CHANGES IN K DURING EXECUTION
            stanzaDS.MaxAgeSplit = cCore.MAX_AGE
        End If

        stanzaDS.redimStanza()

        '' Set all group vbK values to -1
        'For iGroup = 1 To ecopathDS.NumGroups
        '    ecopathDS.vbKInput(iGroup) = -1.0!
        'Next

        ' First read Stanza
        rdStanza = Me.m_db.GetReader("SELECT * FROM Stanza")
        If rdStanza IsNot Nothing Then
            iStanza = 0
            While rdStanza.Read()

                ' Is valid stanza?
                iLifeStage = CInt(Me.m_db.GetValue(String.Format("SELECT * FROM StanzaLifeStage WHERE (StanzaID={0})", CInt(rdStanza("StanzaID")))))
                If (iLifeStage > 0) Then

                    ' Read this stanza
                    iStanza += 1

                    Try

                        stanzaDS.StanzaDBID(iStanza) = CInt(rdStanza("StanzaID"))
                        ' JS 06jun20: StanzaName array 1-dimensional. GroupNames only seem to matter to the EwE5 GUI.
                        '             EwE6 will resolve stanza group names via iCoreInputOutput objects to keep track of 'live' changes.
                        stanzaDS.StanzaName(iStanza) = CStr(rdStanza("StanzaName"))

                        stanzaDS.RecPowerSplit(iStanza) = CSng(rdStanza("RecPower"))
                        stanzaDS.BABsplit(iStanza) = CSng(rdStanza("BabSplit"))
                        stanzaDS.WmatWinf(iStanza) = CSng(rdStanza("WMatWinf"))
                        ' stanzaDS.HatchCode(iStanza) = CInt(rdStanza("HatchCode"))
                        stanzaDS.FixedFecundity(iStanza) = CBool(rdStanza("FixedFecundity"))

                        ' JS 23apr07: Leading B and QB groups are calculated at runtime, no longer stored in DB

                    Catch ex As Exception
                        Me.LogMessage(String.Format("Error {0} occurred while reading Stanza {1}", ex.Message, stanzaDS.StanzaName(iStanza)))
                        bSucces = False
                    End Try

                    rdLifeStage = Me.m_db.GetReader(String.Format("SELECT * FROM StanzaLifeStage WHERE (StanzaID={0}) ORDER BY AgeStart ASC", rdStanza("StanzaID")))
                    iLifeStage = 0
                    While rdLifeStage.Read()

                        ' Next life stage in this stanza
                        iLifeStage += 1

                        ' Store Stanza configuration
                        Try

                            ' Resolve group index
                            iGroup = Array.IndexOf(ecopathDS.GroupDBID, CInt(rdLifeStage("GroupID")))
                            ' JS 06jun20: Disabled (see comment above)
                            ' ecosimDS.StanzaName(nStanza, nGroup) = ecopathDS.GroupName(iGroup)
                            stanzaDS.EcopathCode(iStanza, iLifeStage) = iGroup
                            stanzaDS.Stanza_Z(iStanza, iLifeStage) = CSng(rdLifeStage("Mortality"))
                            stanzaDS.SpeciesCode(iGroup, 0) = iStanza
                            stanzaDS.Age1(iStanza, iLifeStage) = CInt(rdLifeStage("AgeStart"))

                        Catch ex As Exception
                            Me.LogMessage(String.Format("Error {0} occurred while reading StanzaLifeStage {1}", ex.Message, stanzaDS.StanzaName(iStanza), ecopathDS.GroupName(iGroup)))
                            bSucces = False
                        End Try

                        ' Inform Ecopath
                        ecopathDS.StanzaGroup(iGroup) = True

                    End While

                    Me.m_db.ReleaseReader(rdLifeStage)

                    ' Update number of groups in this stanza
                    stanzaDS.Nstanza(iStanza) = iLifeStage
                    Debug.Assert(iLifeStage >= 1, String.Format("Stanza group {0}, ID {1} has no life stages!", stanzaDS.StanzaName(iStanza), stanzaDS.StanzaDBID(iStanza)))
                Else
                    Me.LogMessage(String.Format("Stanza group {0}, ID {1} has no life stages. This group is not read", rdStanza("StanzaName"), rdStanza("StanzaID")), eMessageType.Any, eMessageImportance.Maintenance)
                End If

            End While

            Me.m_db.ReleaseReader(rdStanza)
            rdStanza = Nothing

        End If

        Return bSucces
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Updates a stanza group in the DB.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function SaveStanza() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bNewRow As Boolean = False
        Dim iGroupID As Integer = 0
        Dim iGroup As Integer = 1

        Try
            '' This will delete Ecosim stanza shape assignments
            'Me.m_db.Execute("DELETE * FROM Stanza")

            writer = Me.m_db.GetWriter("Stanza")
            dt = writer.GetDataTable()

            For iStanza As Integer = 1 To stanzaDS.Nsplit

                ' Sanity check: has life stages?
                If (stanzaDS.Nstanza(iStanza) > 0) Then

                    drow = dt.Rows.Find(stanzaDS.StanzaDBID(iStanza))
                    bNewRow = (drow Is Nothing)

                    If bNewRow Then
                        drow = writer.NewRow()
                        drow("StanzaID") = stanzaDS.StanzaDBID(iStanza)
                    Else
                        drow.BeginEdit()
                    End If

                    drow("StanzaName") = stanzaDS.StanzaName(iStanza)
                    drow("RecPower") = stanzaDS.RecPowerSplit(iStanza)
                    drow("BabSplit") = stanzaDS.BABsplit(iStanza)
                    drow("WMatWinf") = stanzaDS.WmatWinf(iStanza)
                    drow("FixedFecundity") = stanzaDS.FixedFecundity(iStanza)

                    ' JS 23apr07: Leading B and QB groups are calculated at runtime, no longer stored in DB

                    If bNewRow Then
                        writer.AddRow(drow)
                    Else
                        drow.EndEdit()
                    End If
                Else
                    ' Hmm, something is very wrong here. This stanza group should not have existed!
                    Debug.Assert(False)
                End If
            Next
            Me.m_db.ReleaseWriter(writer)
        Catch ex As Exception
            Return False
        End Try

        Try
            ' This is ok since no other objects link to the life stages
            Me.m_db.Execute("DELETE * FROM StanzaLifeStage")

            writer = Me.m_db.GetWriter("StanzaLifeStage")
            For iStanza As Integer = 1 To stanzaDS.Nsplit
                For iLifeStage As Integer = 1 To stanzaDS.MaxStanza
                    iGroupID = ecopathDS.GroupDBID(stanzaDS.EcopathCode(iStanza, iLifeStage))
                    If (iGroupID > 0) Then
                        iGroup = stanzaDS.EcopathCode(iStanza, iLifeStage)
                        drow = writer.NewRow()
                        drow("StanzaID") = stanzaDS.StanzaDBID(iStanza)
                        drow("GroupID") = ecopathDS.GroupDBID(iGroup)
                        drow("Sequence") = iLifeStage
                        drow("AgeStart") = stanzaDS.Age1(iStanza, iLifeStage)
                        drow("Mortality") = stanzaDS.Stanza_Z(iStanza, iLifeStage)
                        'drow("vbK") = ecopathDS.vbKInput(iGroup)
                        writer.AddRow(drow)
                    End If
                Next iLifeStage
            Next iStanza
            Me.m_db.ReleaseWriter(writer)
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds a stanza group to the DB.
    ''' </summary>
    ''' <param name="strStanzaName">Name to assign to new stanza group.</param>
    ''' <param name="aiGroupID">Array of <see cref="cEcoPathGroupInput">Ecopath group</see>
    ''' IDs to assign to this multi-stanza configuration.</param>
    ''' <param name="iDBID">Database ID assigned to the new stanza group.</param>
    ''' <returns>Always false.</returns>
    ''' -------------------------------------------------------------------
    Friend Function AppendStanza(ByVal strStanzaName As String, ByVal aiGroupID() As Integer, ByVal iGroupAges() As Integer, _
                ByRef iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.AppendStanza

        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True
        Dim iMaxAge As Integer = 0
        Dim iMaxAgeGroup As Integer = 0

        ' Need to get a balanced set of values
        If aiGroupID.Length <> iGroupAges.Length Then
            Return False
        End If

        ' Process inputs
        For i As Integer = 0 To aiGroupID.Length - 1
            ' Test if groups exist
            If CInt(Me.m_db.GetValue(String.Format("SELECT COUNT(*) FROM EcopathGroup WHERE GroupID={0}", aiGroupID(i)))) = 0 Then
                Debug.Assert(False, String.Format("Invalid group ID {0} specified", aiGroupID(i)))
                Return False
            End If
            ' Find max age
            If iGroupAges(i) > iMaxAge Then iMaxAge = iGroupAges(i) : iMaxAgeGroup = i
        Next i

        Try
            Try
                iDBID = CInt(Me.m_db.GetValue("SELECT MAX(StanzaID) FROM Stanza")) + 1
            Catch e As Exception
                iDBID = 1
            End Try

            writer = Me.m_db.GetWriter("Stanza")

            drow = writer.NewRow()
            drow("StanzaID") = iDBID
            drow("StanzaName") = strStanzaName
            writer.AddRow(drow)

            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Try
            writer = Me.m_db.GetWriter("StanzaLifeStage")
            For i As Integer = 0 To aiGroupID.Length - 1
                ' Start new row
                drow = writer.NewRow()
                drow("StanzaID") = iDBID
                drow("GroupID") = aiGroupID(i)
                drow("AgeStart") = iGroupAges(i)
                drow("Sequence") = (i + 1)
                'drow("vbK") = 0.3
                writer.AddRow(drow)
            Next
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes a stanza group from the DB.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the stanza group to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function RemoveStanza(ByVal iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.RemoveStanza
        Try
            Me.m_db.Execute(String.Format("DELETE FROM Stanza WHERE (StanzaID={0})", iDBID))
            Return True
        Catch ex As Exception
            ' Kaboom
        End Try
        Return False
    End Function


    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds a life stage to an existing stanza configuration.
    ''' </summary>
    ''' <param name="iStanzaDBID">Database ID of the stanza group to add the life stage to.</param>
    ''' <param name="iGroupDBID">Group to add as a life stage.</param>
    ''' <param name="iStartAge">Start age of this life stage.</param>
    ''' <param name="sMortality">Mortality for this life stage.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function AddStanzaLifestage(ByVal iStanzaDBID As Integer, ByVal iGroupDBID As Integer, _
                                       ByVal iStartAge As Integer, ByVal sMortality As Single) As Boolean _
            Implements DataSources.IEcopathDataSource.AddStanzaLifestage

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            writer = Me.m_db.GetWriter("StanzaLifeStage")

            ' Start new row
            drow = writer.NewRow()
            drow("StanzaID") = iStanzaDBID
            drow("GroupID") = iGroupDBID
            drow("AgeStart") = iStartAge
            'drow("vbK") = sVBK
            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes a life stage from an existing stanza configuration.
    ''' </summary>
    ''' <param name="iStanzaDBID">Database ID of the stanza group to remove the life stage from.</param>
    ''' <param name="iGroupDBID">Group to remove as the life stage.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function RemoveStanzaLifestage(ByVal iStanzaDBID As Integer, ByVal iGroupDBID As Integer) As Boolean Implements DataSources.IEcopathDataSource.RemoveStanzaLifestage

        Dim bSucces As Boolean = True
        Try
            Me.m_db.Execute(String.Format("DELETE FROM StanzaLifeStage WHERE (StanzaID={0}) AND (GroupID={1})", iStanzaDBID, iGroupDBID))
        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Stanza

#Region " Ecopath "

#Region " Diagnostics "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States if the datasource has unsaved changes for Ecopath.
    ''' </summary>
    ''' <returns>True if the datasource has pending changes for Ecopath.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsEcopathModified() As Boolean Implements DataSources.IEcopathDataSource.IsEcopathModified

        If Not Me.IsConnected() Then Return False
        Return Me.IsChanged(s_EcopathComponents)

    End Function

#End Region ' Diagnostics

#Region " Groups "

#Region " Load "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads Ecopath Group information.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function LoadEcopathGroups() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim psdDS As cPSDDatastructures = Me.m_core.m_PSDData
        Dim reader As IDataReader = Me.m_db.GetReader("SELECT * FROM EcopathGroup ORDER BY Sequence ASC")
        Dim iGroup As Integer = 1
        Dim sTemp As Single = 0.0
        Dim strTemp As String = ""
        Dim bSucces As Boolean = True

        ' Init data structure
        ecopathDS.NumGroups = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM EcopathGroup"))
        psdDS.NumGroups = ecopathDS.NumGroups

        ecopathDS.NumLiving = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM EcopathGroup WHERE (TYPE <= 1)"))
        psdDS.NumLiving = ecopathDS.NumLiving

        ecopathDS.NumDetrit = ecopathDS.NumGroups - ecopathDS.NumLiving


        ' Allocate space
        If (Not ecopathDS.redimGroupVariables() Or Not psdDS.redimGroupVariables()) Then
            ' It would be quite remarkable to fail here... log message?
            Return False
        End If

        While reader.Read()

            Try
                ecopathDS.GroupDBID(iGroup) = CInt(reader("GroupID"))
                ecopathDS.GroupName(iGroup) = CStr(reader("GroupName"))
                ecopathDS.PP(iGroup) = CSng(reader("Type"))
                ecopathDS.Area(iGroup) = CSng(reader("Area"))
                ecopathDS.BH(iGroup) = ecopathDS.B(iGroup) / ecopathDS.Area(iGroup)
                ecopathDS.BA(iGroup) = CSng(reader("BiomAcc"))
                ' VERIFY_JS: Check default value for BiomAccRate. 0 is assumed
                ecopathDS.BaBi(iGroup) = CSng(reader("BiomAccRate"))
                ecopathDS.GS(iGroup) = CSng(reader("Unassim"))
                ecopathDS.DtImp(iGroup) = CSng(reader("DtImports"))
                ecopathDS.Ex(iGroup) = CSng(reader("Export"))
                ecopathDS.fCatch(iGroup) = CSng(reader("Catch"))
                ecopathDS.DCInput(iGroup, 0) = CSng(reader("ImpVar"))
                ecopathDS.GroupIsFish(iGroup) = CBool(reader("GroupIsFish"))
                ecopathDS.GroupIsInvert(iGroup) = CBool(reader("GroupIsInvert"))
                ecopathDS.Shadow(iGroup) = CSng(reader("NonMarketValue"))
                ecopathDS.Resp(iGroup) = CSng(reader("Respiration"))
                ecopathDS.Immig(iGroup) = CSng(reader("Immigration"))
                ecopathDS.Emigration(iGroup) = CSng(reader("Emigration"))
                ecopathDS.Emig(iGroup) = CSng(Me.ReadSafe(reader, "EmigRate", 0.0!))

                ' PSD
                ecopathDS.vbK(iGroup) = CSng(Me.ReadSafe(reader, "VBK", -1))
                psdDS.AinLWInput(iGroup) = CSng(reader("AinLW"))
                psdDS.BinLWInput(iGroup) = CSng(reader("BinLW"))
                psdDS.LooInput(iGroup) = CSng(reader("Loo"))
                psdDS.WinfInput(iGroup) = CSng(reader("Winf"))
                psdDS.t0Input(iGroup) = CSng(reader("t0"))
                psdDS.TcatchInput(iGroup) = CSng(reader("Tcatch"))
                psdDS.TmaxInput(iGroup) = CSng(reader("Tmax"))


                'variables with input output pairs
                ecopathDS.EEinput(iGroup) = CSng(reader("EcoEfficiency"))
                ecopathDS.PBinput(iGroup) = CSng(reader("ProdBiom"))
                ecopathDS.QBinput(iGroup) = CSng(reader("ConsBiom"))
                ecopathDS.GEinput(iGroup) = CSng(reader("ProdCons"))
                ecopathDS.Binput(iGroup) = CSng(reader("Biomass"))
                ecopathDS.BHinput(iGroup) = ecopathDS.Binput(iGroup) / ecopathDS.Area(iGroup)

                ecopathDS.GroupColor(iGroup) = Integer.Parse(CStr(reader("PoolColor")), Globalization.NumberStyles.HexNumber)

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading group {1}", ex.Message, ecopathDS.GroupName(iGroup)))
                bSucces = False
            End Try

            iGroup += 1

        End While

        Debug.Assert(iGroup - 1 = ecopathDS.NumGroups)

        Me.m_db.ReleaseReader(reader)
        reader = Nothing

        bSucces = bSucces And Me.LoadEcopathDietComp()
        bSucces = bSucces And Me.LoadStanza()

        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Update group info in the datasource.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function SaveEcopathGroups() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim psdDS As cPSDDatastructures = Me.m_core.m_PSDData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim iGroup As Integer = 0
        Dim bSucces As Boolean = True

        Try
            writer = Me.m_db.GetWriter("EcopathGroup")
            dt = writer.GetDataTable()

            For iGroup = 1 To ecopathDS.NumGroups

                ' Find existing row
                drow = dt.Rows.Find(ecopathDS.GroupDBID(iGroup))
                Debug.Assert(drow IsNot Nothing, String.Format("Cannot find existing row for group {0}", ecopathDS.GroupDBID(iGroup)))

                drow.BeginEdit()
                drow("GroupID") = ecopathDS.GroupDBID(iGroup)
                drow("Sequence") = iGroup
                drow("GroupName") = ecopathDS.GroupName(iGroup)
                drow("Type") = ecopathDS.PP(iGroup)
                drow("Area") = ecopathDS.Area(iGroup)
                drow("BiomAcc") = ecopathDS.BA(iGroup)
                drow("BiomAccRate") = ecopathDS.BaBi(iGroup)
                drow("Unassim") = ecopathDS.GS(iGroup)
                drow("DtImports") = ecopathDS.DtImp(iGroup)
                drow("Export") = ecopathDS.Ex(iGroup)
                drow("Catch") = ecopathDS.fCatch(iGroup)
                drow("ImpVar") = ecopathDS.DCInput(iGroup, 0)
                drow("GroupIsFish") = ecopathDS.GroupIsFish(iGroup)
                drow("GroupIsInvert") = ecopathDS.GroupIsInvert(iGroup)
                drow("NonMarketValue") = ecopathDS.Shadow(iGroup)
                drow("Respiration") = ecopathDS.Resp(iGroup)

                'variable with input/output pair only the input gets saved
                drow("EcoEfficiency") = ecopathDS.EEinput(iGroup)
                drow("ProdBiom") = ecopathDS.PBinput(iGroup)
                drow("ConsBiom") = ecopathDS.QBinput(iGroup)
                drow("ProdCons") = ecopathDS.GEinput(iGroup)
                drow("Biomass") = ecopathDS.Binput(iGroup)
                ecopathDS.BHinput(iGroup) = ecopathDS.Binput(iGroup) / ecopathDS.Area(iGroup)

                drow("Immigration") = ecopathDS.Immig(iGroup)
                drow("Emigration") = ecopathDS.Emigration(iGroup)
                drow("EmigRate") = ecopathDS.Emig(iGroup)
                drow("PoolColor") = String.Format("{0:x8}", ecopathDS.GroupColor(iGroup))

                'PSD
                drow("VBK") = ecopathDS.vbK(iGroup)
                drow("Tcatch") = psdDS.Tcatch(iGroup)
                drow("AinLW") = psdDS.AinLWInput(iGroup)
                drow("BinLW") = psdDS.BinLWInput(iGroup)
                drow("Loo") = psdDS.LooInput(iGroup)
                drow("Winf") = psdDS.WinfInput(iGroup)
                drow("t0") = psdDS.t0Input(iGroup)
                drow("Tmax") = psdDS.TmaxInput(iGroup)

                drow.EndEdit()

            Next iGroup

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving EcopathGroup", ex.Message))
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        bSucces = bSucces And Me.SaveEcopathDietComp()
        bSucces = bSucces And Me.SaveStanza()

        Return bSucces

    End Function

#End Region ' Save

#Region " Modify "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Create a record for a new Ecopath group in the datasource.
    ''' </summary>
    ''' <param name="strGroupName">The name of the group to create.</param>
    ''' <param name="sPP">The type of the new group; 0=consumer, 1=producer, 2=detritus, or a cons/prod ratio.</param>
    ''' <param name="sVBK">The vbK value to pass to the group.</param>
    ''' <param name="iPosition">The position of the new group in the group sequence.</param>
    ''' <param name="iDBID">Database ID assigned to the new Group.</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' Note that this will not adjust the data arrays. Due to the complex organization of the
    ''' core a full data reload is required after a group is created.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public Function AddGroup(ByVal strGroupName As String, ByVal sPP As Single, ByVal sVBK As Single, _
                             ByVal iPosition As Integer, ByRef iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.AddGroup

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            Try
                iDBID = CInt(Me.m_db.GetValue("SELECT MAX(GroupID) FROM EcopathGroup")) + 1
            Catch
                iDBID = 1
            End Try

            ' Start writing, protect sequence
            writer = Me.m_db.GetWriter("EcopathGroup", "Sequence")

            ' Get new row to add
            drow = writer.NewRow()
            ' Database will take care of defaults, only take care of the bare necessities
            drow("GroupID") = iDBID
            drow("GroupName") = strGroupName
            drow("Type") = sPP
            drow("vbK") = sVBK
            drow("t0") = -9999 ' Fix default
            drow("Sequence") = iPosition

            ' Commit to db
            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        ' Set initial diet data for this group
        If sPP < 2 Then
            Try
                ' Start writing
                writer = Me.m_db.GetWriter("EcopathDietComp")

                ' For all detritus groups
                For iPrey As Integer = ecopathDS.NumLiving + 1 To ecopathDS.NumGroups
                    ' Get new row to add
                    drow = writer.NewRow()
                    ' Database will take care of defaults, only take care of the bare necessities
                    drow("PredID") = iDBID
                    drow("PreyID") = ecopathDS.GroupDBID(iPrey)
                    ' Commit to db
                    writer.AddRow(drow)
                Next iPrey

                Me.m_db.ReleaseWriter(writer, True)

            Catch ex As Exception
                bSucces = False
            End Try
        End If

        ' Create this group for each ecosim scenario
        bSucces = bSucces And Me.AddEcosimGroupToAllScenarios(iDBID)
        ' Create this group for each ecospace scenario
        bSucces = bSucces And Me.AddEcospaceGroupToAllScenarios(iDBID, (sPP = 2.0))
        ' Create this group for each ecotracer scenario
        bSucces = bSucces And Me.AddEcotracerGroupToAllScenarios(iDBID)

        Return bSucces

    End Function

    Private Function AddCatchDataForGroup(ByVal iGroupID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim iFleetID As Integer = 0
        Dim bSucces As Boolean = True

        For iFleet As Integer = 1 To ecopathDS.NumFleet
            iFleetID = ecopathDS.FleetDBID(iFleet)
            bSucces = bSucces And Me.AddCatch(iGroupID, iFleetID)
            bSucces = bSucces And Me.AddDiscardFate(iGroupID, iFleetID)
        Next
        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Remove a group from the datasource.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the group to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' Note that this will not adjust the data arrays. Due to the complex organization of the
    ''' core a full data reload is required after a group is removed.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public Function RemoveGroup(ByVal iDBID As Integer) As Boolean _
             Implements IEcopathDataSource.RemoveGroup

        Dim bSucces As Boolean = True

        Try
            ' Remove all Ecosim groups related to this Ecopath group
            Dim reader As IDataReader = Me.m_db.GetReader(String.Format("SELECT GroupID FROM EcosimScenarioGroup WHERE EcopathGroupID={0}", iDBID))
            If (reader IsNot Nothing) Then
                While reader.Read()
                    bSucces = Me.RemoveEcosimGroup(CInt(reader("GroupID")))
                End While
            End If
            Me.m_db.ReleaseReader(reader)

            ' Oh, now wait until we need to do this for Ecospace...

            ' Now Ecosim is clean, delete the group from Ecopath
            bSucces = bSucces And Me.m_db.Execute(String.Format("DELETE FROM EcopathGroup WHERE (GroupID={0})", iDBID))

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing group {1}", ex.Message, iDBID))
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Move an Ecopath group to a different position in the group sequence.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the group to move.</param>
    ''' <param name="iPosition">The new position of the group in the group sequence.</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' This method will directly modify the entry in the database
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Function MoveGroup(ByVal iDBID As Integer, ByVal iPosition As Integer) As Boolean _
             Implements IEcopathDataSource.MoveGroup

        Dim bSucces As Boolean = True
        Try
            Me.m_db.Execute(String.Format("UPDATE EcopathGroup SET Sequence={1} WHERE (GroupID={0})", iDBID, iPosition))
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while moving group {1}", ex.Message, iDBID))
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Modify

#Region " DietComp "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads ecopath diet composition information.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function LoadEcopathDietComp() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim reader As IDataReader = Nothing
        Dim iPred As Integer = 0
        Dim iPrey As Integer = 0
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader("SELECT * FROM EcopathDietComp")
            While reader.Read()

                iPred = Array.IndexOf(ecopathDS.GroupDBID, CInt(reader("PredID")))
                iPrey = Array.IndexOf(ecopathDS.GroupDBID, CInt(reader("PreyID")))

                Debug.Assert(iPred >= 0 And iPrey >= 0)

                ecopathDS.DCInput(iPred, iPrey) = CSng(reader("Diet"))
                If iPrey > ecopathDS.NumLiving Then
                    ecopathDS.DF(iPred, iPrey - ecopathDS.NumLiving) = CSng(reader("DetritusFate"))
                End If

                ' 060528JS: ASSERT on "diet leftovers" from previous incarnations, including 041020VC fix for carbon groups
                ' The actual data fix is performed once during EwE5 import, and should not reoccur when running EwE6.
                If ecopathDS.PP(iPred) = 1 And ecopathDS.QB(iPred) <= 0 Then
                    Debug.Assert(ecopathDS.DCInput(iPred, iPrey) = 0, _
                        String.Format("Database corrupted on DCInput({0},{1})={2}, expected 0", iPred, iPrey, ecopathDS.DCInput(iPred, iPrey)))
                End If

                ' VERIFY_JS: check mapping for MTI with JB
                ' ecopathDS.??(nPred, nPrey) = CSng(reader("MTI"))
                ' VERIFY_JS: check mapping for Electivity with JB
                ' ecopathDS.??(nPred, nPrey) = CSng(reader("Electivity"))
            End While
            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EcopathDietComp {1}, {2}", ex.Message, ecopathDS.GroupName(iPred), ecopathDS.GroupName(iPrey)))
            bSucces = False
        End Try

        ' Read 'Import'
        reader = Me.m_db.GetReader("SELECT * FROM EcopathGroup ORDER BY Sequence ASC")
        iPred = 1
        While reader.Read()
            If CSng(reader("ImpVar")) > 0 Then ecopathDS.DCInput(iPred, 0) = CSng(reader("ImpVar"))
            iPred += 1
        End While
        Me.m_db.ReleaseReader(reader)

        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Writes the DietComp information to the database.
    ''' </summary>
    ''' <returns>True if succesful</returns>
    ''' -------------------------------------------------------------------
    Private Function SaveEcopathDietComp() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim idPred As Integer = 0
        Dim iPred As Integer = 0
        Dim idPrey As Integer = 0
        Dim iPrey As Integer = 0

        Dim bSucces As Boolean = True

        Try
            ' No incremental save for now
            Me.m_db.Execute("DELETE * FROM EcopathDietComp")

            writer = Me.m_db.GetWriter("EcopathDietComp")
            ' DietComp is stored in EwE as an indexed list per predator
            For iPred = 1 To ecopathDS.NumGroups

                ' Get DBID for predator to update
                idPred = ecopathDS.GroupDBID(iPred)

                For iPrey = 1 To ecopathDS.NumGroups

                    ' Get DBID for prey to update
                    idPrey = ecopathDS.GroupDBID(iPrey)

                    drow = writer.NewRow()
                    drow("PredID") = idPred
                    drow("PreyID") = idPrey
                    drow("Diet") = ecopathDS.DCInput(iPred, iPrey)
                    If iPrey > ecopathDS.NumLiving Then
                        drow("DetritusFate") = ecopathDS.DF(iPred, iPrey - ecopathDS.NumLiving)
                    Else
                        drow("DetritusFate") = 0
                    End If

                    ' VERIFY_JS: check mapping for MTI with JB
                    ' drow("MTI") = ??
                    ' VERIFY_JS: check mapping for Electivity with JB
                    ' drow("Electivity") = ??

                    writer.AddRow(drow)

                Next iPrey
            Next iPred

            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

#End Region ' DietComp

#End Region ' Groups

#Region " Fleets "

#Region " Helper methods "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States if there is catch for at least one group.
    ''' </summary>
    ''' <returns>True if catch was found.</returns>
    ''' -------------------------------------------------------------------
    Private Function IsFishing() As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim bIsFishing As Boolean = False
        Dim iGroup As Integer = 1

        While iGroup < ecopathDS.NumGroups And Not bIsFishing
            bIsFishing = (ecopathDS.fCatch(iGroup) > 0.0)
            iGroup += 1
        End While

        Return bIsFishing
    End Function

    Private Function AddCatch(ByVal iGroupID As Integer, ByVal iFleetID As Integer) As Boolean
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            writer = Me.m_db.GetWriter("EcopathCatch")
            drow = writer.NewRow()
            drow("GroupID") = iGroupID
            drow("FleetID") = iFleetID
            ' All other values will receive defaults
            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer)
        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

    Private Function AddDiscardFate(ByVal iGroupID As Integer, ByVal iFleetID As Integer) As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iGroup As Integer = Array.IndexOf(ecopathDS.GroupDBID, iGroupID)
        Dim bSucces As Boolean = True

        If (iGroup <= ecopathDS.NumLiving) Then Return True

        Try
            writer = Me.m_db.GetWriter("EcopathDiscardFate")
            drow = writer.NewRow()
            drow("GroupID") = iGroupID
            drow("FleetID") = iFleetID
            ' Set default database value
            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer)
        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Helper methods

#Region " Load "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads all fleet-related data.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' If there is <see cref="IsFishing">no fishing</see>, the fleet data will not be loaded.
    ''' This check is inherited from EwE5.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Private Function LoadEcopathFleetInfo() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim bSucces As Boolean = True

        ecopathDS.NoGearData = Not IsFishing()

        ecopathDS.NumFleet = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM EcopathFleet"))

        ' This will be necessary when reading Gear tables. Can only call this after groups are read.
        If Not ecopathDS.RedimFleetVariables(True) Then
            Return False
        End If

        bSucces = LoadEcopathFleets()
        bSucces = bSucces And LoadEcopathCatch()
        bSucces = bSucces And LoadEcopathDiscardFate()

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads all Ecopath fleets.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function LoadEcopathFleets() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim reader As IDataReader = Nothing
        Dim iFleet As Integer = 1
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader("SELECT * FROM EcopathFleet ORDER BY Sequence ASC")
            While reader.Read()

                ecopathDS.FleetDBID(iFleet) = CInt(reader("FleetID"))
                ecopathDS.FleetName(iFleet) = CStr(reader("FleetName"))
                ecopathDS.CostPct(iFleet, eCostIndex.Fixed) = CSng(reader("FixedCost"))
                ecopathDS.CostPct(iFleet, eCostIndex.Sail) = CSng(reader("SailingCost"))
                ecopathDS.CostPct(iFleet, eCostIndex.CUPE) = CSng(reader("variableCost"))
                ecopathDS.FleetColor(iFleet) = Integer.Parse(CStr(reader("PoolColor")), Globalization.NumberStyles.HexNumber)
                iFleet += 1

            End While

            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EcopathFleet {1}", ex.Message, iFleet))
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function LoadEcopathCatch() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim reader As IDataReader = Nothing
        Dim iFleet As Integer = 0
        Dim iGroup As Integer = 0
        Dim bSucces As Boolean = True

        Try

            reader = Me.m_db.GetReader("SELECT * FROM EcopathCatch")
            While reader.Read()

                iGroup = Array.IndexOf(ecopathDS.GroupDBID, CInt(reader("GroupID")))
                iFleet = Array.IndexOf(ecopathDS.FleetDBID, CInt(reader("FleetID")))

                ' JS270707: no need to assert any longer
                'Debug.Assert(iGroup >= 0 And iFleet >= 0)

                If (iGroup >= 1 And iFleet >= 1) Then
                    ecopathDS.Landing(iFleet, iGroup) = CSng(reader("Landing"))
                    ecopathDS.Discard(iFleet, iGroup) = CSng(reader("discards"))
                    ecopathDS.Market(iFleet, iGroup) = CSng(reader("price"))
                    ecopathDS.PropDiscardMort(iFleet, iGroup) = CSng(Me.ReadSafe(reader, "DiscardMortality", 0.0!))
                Else
                    Me.LogMessage(String.Format("Error {0} occurred while appending loading catch for group {0}, fleet {1}", iGroup, iFleet))
                    bSucces = False
                End If

            End While

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading catch {1}, {2}", ex.Message, iGroup, iFleet))
            bSucces = False
        End Try

        Me.m_db.ReleaseReader(reader)

        Return bSucces

    End Function

    Private Function LoadEcopathDiscardFate() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim reader As IDataReader = Nothing
        Dim iFleet As Integer = 0
        Dim iGroup As Integer = 0
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader("SELECT * FROM EcopathDiscardFate")
            If reader IsNot Nothing Then

                While reader.Read()

                    iGroup = Array.IndexOf(ecopathDS.GroupDBID, CInt(reader("GroupID")))
                    iFleet = Array.IndexOf(ecopathDS.FleetDBID, CInt(reader("FleetID")))

                    ' JS 27jul07: no need to assert any longer
                    'Debug.Assert(iGroup >= 0 And iFleet >= 0)

                    If (iGroup > ecopathDS.NumLiving) Then
                        ecopathDS.DiscardFate(iFleet, iGroup - ecopathDS.NumLiving) = CSng(reader("DiscardFate"))
                        'Else
                        '    '' ToDo_JS: localize this
                        '    'Me.LogMessage(String.Format("DiscardFate value ignored for living group {0}, fleet ", iGroup), eMessageType.Any, eMessageImportance.Information)
                        '    ' Keep on chugging, do not make assignment
                        '    bSucces = True
                    End If

                End While
                Me.m_db.ReleaseReader(reader)

            End If

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading DiscardFate {1}, {2}", ex.Message, iGroup, iFleet))
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Saves all fleet-related data to the datasource.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function SaveEcopathFleetInfo() As Boolean

        Dim bSucces As Boolean = True

        bSucces = SaveEcopathFleets()
        bSucces = bSucces And SaveCatch()
        bSucces = bSucces And SaveDiscardFate()

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Saves all fleets.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function SaveEcopathFleets() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Me.m_db.GetWriter("EcopathFleet")
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim iFleet As Integer = 0
        Dim bAddNewRow As Boolean = False
        Dim bSucces As Boolean = True

        Try

            writer = Me.m_db.GetWriter("EcopathFleet")
            dt = writer.GetDataTable()

            For iFleet = 1 To ecopathDS.NumFleet

                ' Get existing row, or create new row if a fleet does not yet exist in the DB. This can
                ' happen when a fleet is added to the models without properly adding it to the database;
                ' this code needs to be prepared for that eventuality.
                drow = dt.Rows.Find(ecopathDS.FleetDBID(iFleet))
                bAddNewRow = (drow Is Nothing)

                If bAddNewRow Then drow = writer.NewRow()
                Debug.Assert(drow IsNot Nothing, String.Format("No existing row for fleet {0}", ecopathDS.FleetDBID(iFleet)))

                drow("Sequence") = iFleet
                If bAddNewRow Then drow("FleetID") = ecopathDS.FleetDBID(iFleet)
                drow("FleetName") = ecopathDS.FleetName(iFleet)
                drow("FixedCost") = ecopathDS.CostPct(iFleet, eCostIndex.Fixed)
                drow("SailingCost") = ecopathDS.CostPct(iFleet, eCostIndex.Sail)
                drow("variableCost") = ecopathDS.CostPct(iFleet, eCostIndex.CUPE)
                drow("PoolColor") = String.Format("{0:x8}", ecopathDS.FleetColor(iFleet))

                If bAddNewRow Then writer.AddRow(drow)
            Next iFleet
            ' Save changes
            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving EcopathFleet", ex.Message))
            bSucces = False
        End Try

        Return bSucces
    End Function

    Private Function SaveCatch() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iFleet As Integer = 0
        Dim iGroup As Integer = 0
        Dim bSucces As Boolean = True

        Try
            Me.m_db.Execute("DELETE * FROM EcopathCatch")

            writer = Me.m_db.GetWriter("EcopathCatch")

            For iFleet = 1 To ecopathDS.NumFleet
                For iGroup = 1 To ecopathDS.NumGroups

                    ' JS 04aug08: only save rows with data
                    If (ecopathDS.Landing(iFleet, iGroup) > 0.0!) Or _
                       (ecopathDS.Discard(iFleet, iGroup) > 0.0!) Or _
                       ((ecopathDS.Market(iFleet, iGroup) > 0.0!) And (ecopathDS.Market(iFleet, iGroup) < 1.0!)) Or _
                       (ecopathDS.PropDiscardMort(iFleet, iGroup) > 0.0!) Then

                        drow = writer.NewRow()
                        drow("FleetID") = ecopathDS.FleetDBID(iFleet)
                        drow("GroupID") = ecopathDS.GroupDBID(iGroup)
                        drow("Landing") = ecopathDS.Landing(iFleet, iGroup)
                        drow("Discards") = ecopathDS.Discard(iFleet, iGroup)
                        drow("Price") = ecopathDS.Market(iFleet, iGroup)
                        drow("DiscardMortality") = ecopathDS.PropDiscardMort(iFleet, iGroup)
                        writer.AddRow(drow)

                    End If

                Next iGroup
            Next iFleet

            ' Save changes
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving catch", ex.Message))
            bSucces = False
        End Try

        Return bSucces
    End Function

    Private Function SaveDiscardFate() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iFleet As Integer = 0
        Dim iGroup As Integer = 0
        Dim bSucces As Boolean = True

        Try
            Me.m_db.Execute("DELETE * FROM EcopathDiscardFate")

            writer = Me.m_db.GetWriter("EcopathDiscardFate")

            For iFleet = 1 To ecopathDS.NumFleet
                For iGroup = 1 To ecopathDS.NumGroups - ecopathDS.NumLiving

                    drow = writer.NewRow()
                    drow("FleetID") = ecopathDS.FleetDBID(iFleet)
                    drow("GroupID") = ecopathDS.GroupDBID(iGroup + ecopathDS.NumLiving)
                    drow("DiscardFate") = ecopathDS.DiscardFate(iFleet, iGroup)
                    writer.AddRow(drow)

                Next iGroup
            Next iFleet

            ' Save changes
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving DiscardFate {1}, {2}", ex.Message, iGroup, iFleet))
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Save

#Region " Modify "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds a fleet to the datasource.
    ''' </summary>
    ''' <param name="strFleetName">Name of the new fleet.</param>
    ''' <param name="iDBID">Database ID assigned to the new fleet.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function AddFleet(ByVal strFleetName As String, ByVal iPosition As Integer, ByRef iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.AddFleet

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            iDBID = CInt(Me.m_db.GetValue("SELECT MAX(FleetID) FROM EcopathFleet")) + 1
        Catch
            iDBID = 1
        End Try

        Try
            ' Start writing, protect sequence
            writer = Me.m_db.GetWriter("EcopathFleet", "Sequence")
            drow = writer.NewRow()
            drow("FleetID") = iDBID
            drow("FleetName") = strFleetName
            drow("Sequence") = iPosition
            drow("PoolColor") = "00000000"
            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while adding fleet {1}", ex.Message, strFleetName))
            bSucces = False
        End Try

        ' Add Catch
        bSucces = bSucces And Me.AddCatchDataForFleet(iDBID)
        ' Create ecosim fleet forcing bits

        ' Create fleet objects though
        bSucces = bSucces And Me.AddEcosimFleetToAllScenarios(iDBID)
        bSucces = bSucces And Me.AddEcospaceFleetToAllScenarios(iDBID)

        Return bSucces

    End Function

    Private Function AddCatchDataForFleet(ByVal iFleetID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim iGroupID As Integer = 0
        Dim bSucces As Boolean = True

        For iGroup As Integer = 1 To ecopathDS.NumLiving
            iGroupID = ecopathDS.GroupDBID(iGroup)
            bSucces = bSucces And Me.AddCatch(iGroupID, iFleetID)
        Next

        ' JS 21oct09: Send all detritus to only the LAST detritus group (bug 460)
        '             This code assumes that detritus groups are at the end of the group list
        bSucces = bSucces And Me.AddDiscardFate(ecopathDS.GroupDBID(ecopathDS.NumGroups), iFleetID)

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes a fleet from the datasource.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the fleet to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Function RemoveFleet(ByVal iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.RemoveFleet

        Dim bSucces As Boolean = True
        Try
            bSucces = Me.RemoveEcosimFleet(iDBID)
            bSucces = bSucces And Me.m_db.Execute(String.Format("DELETE FROM EcopathFleet WHERE (FleetID={0})", iDBID))
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing fleet {1}", ex.Message, iDBID))
            bSucces = False
        End Try
        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Move an Ecopath fleet to a different position in the fleet sequence.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the fleet to move.</param>
    ''' <param name="iPosition">The new position of the fleet in the fleet sequence.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function MoveFleet(ByVal iDBID As Integer, ByVal iPosition As Integer) As Boolean _
            Implements DataSources.IEcopathDataSource.MoveFleet

        Dim bSucces As Boolean = True
        Try
            Me.m_db.Execute(String.Format("UPDATE EcopathFleet SET Sequence={1} WHERE (FleetID={0})", iDBID, iPosition))
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while moving fleet {1}", ex.Message, iDBID))
            bSucces = False
        End Try
        Return bSucces
    End Function

#End Region '  Modify

#End Region ' Fleets

#Region " Datasets "

    Private Function LoadTimeSeriesDatasets() As Boolean

        Dim tsDS As cTimeSeriesDataStructures = Me.m_core.m_TSData
        Dim reader As IDataReader = Nothing
        Dim iDataset As Integer = 1
        Dim bSucces As Boolean = True

        Try
            tsDS.nDatasets = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM EcosimTimeSeriesDataset"))
        Catch ex As Exception
            tsDS.nDatasets = 0
        End Try

        tsDS.RedimTimeSeriesDatasets()

        reader = Me.m_db.GetReader("SELECT * FROM EcosimTimeSeriesDataset")
        If reader IsNot Nothing Then
            Try
                While reader.Read()
                    tsDS.iDatasetDBID(iDataset) = CInt(reader("DatasetID"))
                    tsDS.strDatasetNames(iDataset) = CStr(reader("DatasetName"))
                    tsDS.strDatasetDescription(iDataset) = CStr(Me.ReadSafe(reader, "Description", ""))
                    tsDS.strDatasetAuthor(iDataset) = CStr(Me.ReadSafe(reader, "Author", ""))
                    tsDS.strDatasetContact(iDataset) = CStr(Me.ReadSafe(reader, "Contact", ""))
                    tsDS.nDatasetFirstYear(iDataset) = CInt(reader("FirstYear"))
                    tsDS.nDatasetNumYears(iDataset) = CInt(reader("NumYears"))
                    tsDS.nDatasetNumTimeSeries(iDataset) = CInt(Me.m_db.GetValue(String.Format("SELECT COUNT(*) FROM EcosimTimeSeries WHERE (DatasetID={0})", CInt(reader("DatasetID")))))
                    iDataset += 1
                End While
            Catch ex As Exception
                bSucces = False
            End Try
            Me.m_db.ReleaseReader(reader)
        End If

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds an time series dataset to the datasource.
    ''' </summary>
    ''' <param name="strDatasetName">Name to assign to new dataset.</param>
    ''' <param name="strDescription">Description to assign to new dataset.</param>
    ''' <param name="strAuthor">Author to assign to the new dataset.</param>
    ''' <param name="strContact">Contact info to assign to the new dataset.</param>
    ''' <param name="iFirstYear">First year of the dataset.</param>
    ''' <param name="iNumYears">Number of years in the dataset.</param>
    ''' <param name="iDatasetID">Database ID assigned to the new dataset.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function AppendTimeSeriesDataset(ByVal strDatasetName As String, ByVal strDescription As String, _
            ByVal strAuthor As String, ByVal strContact As String, _
            ByVal iFirstYear As Integer, ByVal iNumYears As Integer, _
            ByRef iDatasetID As Integer) As Boolean Implements DataSources.IEcosimDatasource.AppendTimeSeriesDataset

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim idm As New cIDMappings()
        Dim bSucces As Boolean = True

        Try
            ' Delete existing dataset with same name, if any
            Dim reader As IDataReader = Me.m_db.GetReader(String.Format("SELECT DatasetID FROM EcosimTimeSeriesDataset WHERE DatasetName='{0}'", strDatasetName))
            Dim lDatasetID As New List(Of Integer)
            While reader.Read
                lDatasetID.Add(CInt(reader("DatasetID")))
            End While
            Me.m_db.ReleaseReader(reader)

            ' Delete dataset(s)
            For Each iDatasetIDTemp As Integer In lDatasetID
                bSucces = bSucces And Me.RemoveTimeSeriesDatasetID(iDatasetIDTemp)
            Next

            ' Still looking good?
            If bSucces Then

                Try
                    iDatasetID = CInt(Me.m_db.GetValue("SELECT MAX(DatasetID) FROM EcosimTimeSeriesDataset")) + 1
                Catch ex As InvalidCastException
                    iDatasetID = 1
                End Try

                writer = Me.m_db.GetWriter("EcosimTimeSeriesDataset")

                drow = writer.NewRow()
                drow("DatasetID") = iDatasetID
                drow("DatasetName") = strDatasetName
                drow("Description") = strDescription
                drow("Author") = strAuthor
                drow("Contact") = strContact
                drow("FirstYear") = iFirstYear
                drow("NumYears") = iNumYears
                'drow("LastSaved") = cDBDataSource.GetJulianDate()
                writer.AddRow(drow)

                Me.m_db.ReleaseWriter(writer)

                ' Reload time series dataset
                If bSucces Then Me.LoadTimeSeriesDatasets()

            End If

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while appending dataset {1}", ex.Message, strDatasetName))
            bSucces = False
        End Try

        Return bSucces
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes all time series belonging to a specific dataset from the datasource.
    ''' </summary>
    ''' <param name="iDataset">Index of the dataset to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function RemoveTimeSeriesDataset(ByVal iDataset As Integer) As Boolean _
            Implements DataSources.IEcosimDatasource.RemoveTimeSeriesDataset
        Dim tsDS As cTimeSeriesDataStructures = Me.m_core.m_TSData
        Return Me.RemoveTimeSeriesDatasetID(tsDS.iDatasetDBID(iDataset))
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes all time series belonging to a specific dataset from the datasource.
    ''' </summary>
    ''' <param name="iDatasetID">Database ID of the dataset to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function RemoveTimeSeriesDatasetID(ByVal iDatasetID As Integer) As Boolean

        Dim bSucces As Boolean = True
        Try
            ' Cascading delete may fail due to 'weak' relations set by updates. Aargh, how I dislike Access!!!
            ' Solution: manually delete all dataset links
            Me.m_db.Execute(String.Format("DELETE FROM EcosimTimeSeries WHERE (DatasetID={0})", iDatasetID))
            Me.m_db.Execute(String.Format("DELETE FROM EcosimTimeSeriesDataset WHERE (DatasetID={0})", iDatasetID))
        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Datasets

#End Region ' Ecopath

#Region " EcoSim "

#Region " Diagnostics "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States if the datasource has unsaved changes for Ecosim.
    ''' </summary>
    ''' <returns>True if the datasource has pending changes for Ecosim.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsEcosimModified() As Boolean Implements DataSources.IEcosimDatasource.IsEcosimModified

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData

        ' Hmm, maybe the datasource should have a better way to 'remember' whether a sim scenario has been loaded.
        If Not Me.IsConnected() Then Return False
        If ecopathDS.ActiveEcosimScenario < 0 Then Return False
        Return Me.IsChanged(s_EcosimComponents)

    End Function

#End Region ' Diagnostics

#Region " Scenarios "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads an ecosim scenario from the DB.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the scenario to load.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function LoadEcosimScenario(ByVal iDBID As Integer) As Boolean _
            Implements IEcosimDatasource.LoadEcosimScenario

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        ecosimDS.nGroups = ecopathDS.NumGroups

        ecosimDS.RedimVars()
        ecosimDS.SetDefaultParameters()

        Me.m_core.m_QuotaData.RedimVars()
        Me.m_core.m_MSEData.RedimVars()

        reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimScenario WHERE (ScenarioID={0})", iDBID))
        Try
            ' Read the one record
            reader.Read()

            ecosimDS.NumYears = CInt(reader("TotalTime"))
            ecosimDS.StepSize = CSng(reader("StepSize"))
            ecosimDS.EquilibriumStepSize = CSng(reader("EquilibriumStepSize"))
            ecosimDS.EquilScaleMax = CSng(reader("EquilScaleMax"))
            ecosimDS.SorWt = CSng(reader("sorwt"))
            ecosimDS.SystemRecovery = CSng(reader("SystemRecovery"))
            ecosimDS.Discount = CSng(reader("Discount"))

            'ecosimDS.NudgeStart = CSng(reader("NudgeStart"))
            'ecosimDS.NudgeEnd = CSng(reader("NudgeEnd"))
            'ecosimDS.NudgeFactor = CSng(reader("NudgeFactor"))
            'ecosimDS.DoInteg = CSng(reader("DoInteg"))
            'ecosimDS.chkNudge = CBool(reader("UseNudge"))

            'drow("NMed") = Me.FixValue(reader("NMed"))                        ' DISCONTINUED
            'drow("NMedPoints") = Me.FixValue(reader("NMedPoints"))            ' DISCONTINUED

            ecosimDS.NutBaseFreeProp = CSng(reader("NutBaseFreeProp"))
            ecosimDS.NutPBmax = CSng(reader("NutPBmax"))

            'ecosimDS.UseVarPQ = CBool(reader("UseVarPQ"))
            'VC090403: the var P/Q was being set to true by default, It shouldn't be, this should be done in interface only
            ecosimDS.UseVarPQ = False

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading Scenario {1}", ex.Message, iDBID))
            bSucces = False
        End Try
        Me.m_db.ReleaseReader(reader)

        'jb added to redim time variables in ecosim data structures
        ecosimDS.RedimTime()

        ' Set active scenario
        ecopathDS.ActiveEcosimScenario = Array.IndexOf(ecopathDS.EcosimScenarioDBID, iDBID)

        bSucces = bSucces And Me.LoadEcosimGroups(iDBID)
        bSucces = bSucces And Me.LoadEcosimFleets(iDBID)
        bSucces = bSucces And Me.LoadEcosimQuota(iDBID)
        bSucces = bSucces And Me.LoadShapes()
        bSucces = bSucces And Me.LoadEcosimMSE(iDBID)

        Me.ClearChanged(s_EcosimComponents)

        Return bSucces
    End Function

    Friend Function SaveEcosimScenarioAs(ByVal strScenarioName As String, ByVal strDescription As String, _
      ByVal strAuthor As String, ByVal strContact As String, ByRef iScenarioID As Integer) As Boolean _
             Implements IEcosimDatasource.SaveEcosimScenarioAs

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        ' Delete existing scenario
        Me.m_db.Execute(String.Format("DELETE FROM EcosimScenario WHERE ScenarioName='{0}'", strScenarioName))

        Try
            iScenarioID = CInt(Me.m_db.GetValue("SELECT MAX(ScenarioID) FROM EcosimScenario")) + 1
        Catch ex As Exception
            iScenarioID = 1
        End Try

        Try
            writer = Me.m_db.GetWriter("EcosimScenario")
            drow = writer.NewRow()
            drow("ScenarioID") = iScenarioID
            drow("ScenarioName") = strScenarioName
            drow("Description") = strDescription
            drow("Author") = strAuthor
            drow("Contact") = strContact
            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer)
        Catch ex As Exception
            bSucces = False
        End Try

        Return (bSucces And Me.SaveEcosimScenario(iScenarioID))

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Save the current active Ecosim scenario in the datasource under
    ''' a given database ID.
    ''' </summary>
    ''' <param name="iScenarioID">Database ID to save the current scenario to.
    ''' If this parameter is left blank, the current scenario is saved
    ''' under its own database ID.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function SaveEcosimScenario(ByVal iScenarioID As Integer) As Boolean _
            Implements IEcosimDatasource.SaveEcosimScenario

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData

        ' Abort if there is no active scenario
        If ecopathDS.ActiveEcosimScenario <= 0 Then Return False

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenario As Integer = Array.IndexOf(ecopathDS.EcosimScenarioDBID, iScenarioID)
        Dim iActiveScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim bSucces As Boolean = True
        Dim idm As cIDMappings = Nothing

        ' Prepare for saving
        idm = New cIDMappings()
        If iScenarioID = 0 Then iScenarioID = iActiveScenarioID

        ' Duplicating a scenario?
        If iScenarioID <> iActiveScenarioID Then
            ' #Yes: add ID mapping to allow copying of scenario content
            idm.Add(eDataTypes.EcoSimScenario, iActiveScenarioID, iScenarioID)
        End If

        bSucces = Me.m_db.BeginTransaction()

        Try

            writer = Me.m_db.GetWriter("EcosimScenario")
            dt = writer.GetDataTable()
            drow = dt.Rows.Find(iScenarioID)

            drow("TotalTime") = ecosimDS.NumYears
            drow("StepSize") = ecosimDS.StepSize
            drow("EquilibriumStepSize") = ecosimDS.EquilibriumStepSize
            drow("EquilScaleMax") = ecosimDS.EquilScaleMax
            drow("sorwt") = ecosimDS.SorWt
            drow("SystemRecovery") = ecosimDS.SystemRecovery
            drow("Discount") = ecosimDS.Discount

            'drow("NudgeStart") = ecosimDS.NudgeStart
            'drow("NudgeEnd") = ecosimDS.NudgeEnd 
            'drow("NudgeFactor") = ecosimDS.NudgeFactor
            'drow("DoInteg") = ecosimDS.DoInteg 
            'drow("UseNudge") = ecosimDS.chkNudge

            drow("NutBaseFreeProp") = ecosimDS.NutBaseFreeProp
            drow("NutForcingShapeID") = ecosimDS.ForcingDBIDs(ecosimDS.NutForceNumber)
            drow("SalinityForcingShapeID") = ecosimDS.ForcingDBIDs(ecosimDS.SalinityForceNo)
            drow("TemperatureForcingShapeID") = ecosimDS.ForcingDBIDs(ecosimDS.TemperatureForceNo)
            drow("NutPBmax") = ecosimDS.NutPBmax
            'drow("UseVarPQ") = ecosimDS.UseVarPQ
            drow("LastSaved") = cDBDataSource.GetJulianDate()

            ' Save changes
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving Scenario {1}", ex.Message, iScenarioID))
            bSucces = False
        End Try

        bSucces = bSucces And Me.SaveEcosimGroups(idm)
        bSucces = bSucces And Me.SaveEcosimFleets(idm)
        bSucces = bSucces And Me.SaveEcosimQuota(idm)
        bSucces = bSucces And Me.SaveShapes(idm)
        bSucces = bSucces And Me.SaveTimeSeries(idm)
        bSucces = bSucces And Me.SaveEcosimMSE(idm)

        If bSucces Then
            ' Commit save
            bSucces = Me.m_db.CommitTransaction(True)
        Else
            Me.m_db.RollbackTransaction()
        End If

        If (bSucces) Then
            ' Clear changed admin
            Me.ClearChanged(s_EcosimComponents)
            ' Reload ecosim scenario definitions 
            Me.LoadEcosimScenarioDefinitions()
        End If

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds a scenario to the DB.
    ''' </summary>
    ''' <param name="strScenarioName">Name to assign to new scenario.</param>
    ''' <param name="strDescription">Description to assign to new scenario.</param>
    ''' <param name="strAuthor">Author to assign to the new scenario.</param>
    ''' <param name="strContact">Contact info to assign to the new scenario.</param>
    ''' <param name="iScenarioID">Database ID assigned to the new scenario.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function AppendEcosimScenario(ByVal strScenarioName As String, ByVal strDescription As String, _
            ByVal strAuthor As String, ByVal strContact As String, ByRef iScenarioID As Integer) As Boolean _
            Implements IEcosimDatasource.AppendEcosimScenario

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim idm As New cIDMappings()
        Dim bSucces As Boolean = True

        Try
            ' Delete existing scenario with same name, if any
            bSucces = Me.m_db.Execute(String.Format("DELETE FROM EcosimScenario WHERE (ScenarioName='{0}')", strScenarioName))

            Try
                iScenarioID = CInt(Me.m_db.GetValue("SELECT MAX(ScenarioID) FROM EcosimScenario")) + 1
            Catch ex As InvalidCastException
                iScenarioID = 1
            End Try

            writer = Me.m_db.GetWriter("EcosimScenario")

            drow = writer.NewRow()
            drow("ScenarioID") = iScenarioID
            drow("ScenarioName") = strScenarioName
            drow("Description") = strDescription
            drow("Author") = strAuthor
            drow("Contact") = strContact
            drow("LastSaved") = cDBDataSource.GetJulianDate()
            writer.AddRow(drow)

            Me.m_db.ReleaseWriter(writer)

            ' Create ecosim groups for the new scenario
            For i As Integer = 1 To ecopathDS.GroupDBID.Length - 1
                bSucces = bSucces And Me.CreateRepairEcosimGroup(ecopathDS.GroupDBID(i), iScenarioID)
            Next
            ' Create ecosim fleets for the new scenario
            For i As Integer = 1 To ecopathDS.FleetDBID.Length - 1
                ' Sanity check to skip the 'all' fleet
                If ecopathDS.FleetDBID(i) > 0 Then
                    bSucces = bSucces And Me.CreateRepairEcosimFleet(ecopathDS.FleetDBID(i), iScenarioID)
                End If
            Next

            ' Reload scenario definitions
            bSucces = bSucces And Me.LoadEcosimScenarioDefinitions()

            Me.ClearChanged(s_EcosimComponents)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while appending Scenario {1}", ex.Message, strScenarioName))
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes a scenario from the DB.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the scenario to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function RemoveEcosimScenario(ByVal iDBID As Integer) As Boolean _
            Implements IEcosimDatasource.RemoveEcosimScenario

        Dim bSucces As Boolean = True

        Try
            ' Delete 'soft links': database links forged by database updates
            '    DB update 6.04022
            bSucces = bSucces And Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioQuota WHERE (ScenarioID={0})", iDBID))
            '    DB update 6.07001
            bSucces = bSucces And Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioMSE WHERE (ScenarioID={0})", iDBID))
            ' Delete actual scenario
            bSucces = bSucces And Me.m_db.Execute(String.Format("DELETE FROM EcosimScenario WHERE (ScenarioID={0})", iDBID))
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing Ecosim scenarioID {1}", ex.Message, iDBID))
            bSucces = False
        End Try

        ' Reload scenario definitions
        bSucces = bSucces And Me.LoadEcosimScenarioDefinitions()

        Return bSucces
    End Function

#End Region ' Scenarios

#Region " Groups, fleets "

#Region " Modify "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create or fixes a group in each ecosim scenario
    ''' </summary>
    ''' <param name="iEcopathGroupID">Ecopath Group DBID</param>
    ''' -----------------------------------------------------------------------
    Private Function AddEcosimGroupToAllScenarios(ByVal iEcopathGroupID As Integer) As Boolean

        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader(String.Format("SELECT ScenarioID FROM EcoSimScenario"))
            While reader.Read()
                bSucces = bSucces And CreateRepairEcosimGroup(iEcopathGroupID, CInt(reader("ScenarioID")))
            End While
            Me.m_db.ReleaseReader(reader)
        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create or repair an ecosim group in a given scenario.
    ''' </summary>
    ''' <param name="iEcopathGroupID">Ecopath Group DBID.</param>
    ''' <param name="iScenarioID">Scenario ID to add the group to.</param>
    ''' -----------------------------------------------------------------------
    Private Function CreateRepairEcosimGroup(ByVal iEcopathGroupID As Integer, ByVal iScenarioID As Integer) As Boolean

        Dim readerGroup As IDataReader = Nothing
        Dim writerShape As cEwEDatabase.cEwEDbWriter = Nothing
        Dim writerGroup As cEwEDatabase.cEwEDbWriter = Nothing
        Dim writerAssgn As cEwEDatabase.cEwEDbWriter = Nothing
        Dim bValueFound As Boolean = False
        Dim bGroupFound As Boolean = False
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True
        Dim iGroupID As Integer = 1
        Dim iFishMortShapeID As Integer = -1

        readerGroup = Me.m_db.GetReader(String.Format("SELECT GroupID, FishMortShapeID FROM EcosimScenarioGroup WHERE (EcopathGroupID={0}) AND (ScenarioID={1})", iEcopathGroupID, iScenarioID))
        If readerGroup IsNot Nothing Then
            Try
                readerGroup.Read()

                ' Try to find existing Sim group ID
                iGroupID = CInt(readerGroup(0))
                ' Try to find existing Fish mort shape ID
                iFishMortShapeID = CInt(readerGroup(1))

                ' It this did not fail we have found a group, whoot! whoot!
                bGroupFound = True
            Catch ex As InvalidOperationException
                iGroupID = -1
                iFishMortShapeID = -1
                bGroupFound = False
            End Try
            Me.m_db.ReleaseReader(readerGroup)
        End If

        ' Resolve group ID
        If (iGroupID <= 0) Then
            Try
                iGroupID = CInt(Me.m_db.GetValue("SELECT MAX(GroupID) FROM EcosimScenarioGroup")) + 1
            Catch ex As Exception
                iGroupID = 1
            End Try
        End If

        ' Resolve Fish mort ID
        If (iFishMortShapeID <= 0) Then
            Try
                iFishMortShapeID = CInt(Me.m_db.GetValue("SELECT MAX(ShapeID) FROM EcosimShape")) + 1
            Catch ex As InvalidCastException
                iFishMortShapeID = 1
            End Try
        End If

        ' *** Next: Critical bits, create missing entries in DB ***

        ' Already exists in EcosimShape?
        bValueFound = (CInt(Me.m_db.GetValue(String.Format("SELECT ShapeID FROM EcosimShape WHERE (ShapeID={0})", iFishMortShapeID))) > 0)
        If Not bValueFound Then
            Try
                writerShape = Me.m_db.GetWriter("EcosimShape")
                drow = writerShape.NewRow()
                drow("ShapeID") = iFishMortShapeID
                drow("ShapeType") = eDataTypes.FishMort
                drow("IsSeasonal") = False
                writerShape.AddRow(drow)
                Me.m_db.ReleaseWriter(writerShape)

                ' Log repair state
                Me.LogMessage(String.Format("Added missing shape definition {0} for Ecosim group {1}", iFishMortShapeID, iGroupID))

            Catch ex As Exception
                bSucces = False
                ' Log failure
                Me.LogMessage(String.Format("Failed to add shape definition {0} for Ecosim group {1}", iFishMortShapeID, iGroupID), eMessageType.NotSet, eMessageImportance.Critical)
            End Try
        End If

        ' Already exists in EcosimShapeFishMort?
        bValueFound = (CInt(Me.m_db.GetValue(String.Format("SELECT ShapeID FROM EcosimShapeFishMort WHERE (ShapeID={0})", iFishMortShapeID))) > 0)
        If Not bValueFound Then
            Try
                writerAssgn = Me.m_db.GetWriter("EcosimShapeFishMort")
                drow = writerAssgn.NewRow()
                drow("ShapeID") = iFishMortShapeID
                drow("Title") = String.Format(My.Resources.CoreDefaults.CORE_DEFAULT_FISHMORTSHAPE, iFishMortShapeID)
                drow("Zscale") = "0"
                writerAssgn.AddRow(drow)
                Me.m_db.ReleaseWriter(writerAssgn)

                ' Log repair state
                Me.LogMessage(String.Format("Added missing fishing mortality shape {0} for Ecosim group {1}", iFishMortShapeID, iGroupID))

            Catch ex As Exception
                bSucces = False
                ' Log failure
                Me.LogMessage(String.Format("Failed to add fishing mortality shape {0} for Ecosim group {1}", iFishMortShapeID, iGroupID), eMessageType.NotSet, eMessageImportance.Critical)
            End Try
        End If

        If Not bGroupFound Then
            Try
                writerGroup = Me.m_db.GetWriter("EcosimScenarioGroup")
                drow = writerGroup.NewRow()
                drow("ScenarioID") = iScenarioID
                drow("GroupID") = iGroupID
                drow("EcopathGroupID") = iEcopathGroupID
                drow("FishMortShapeID") = iFishMortShapeID
                writerGroup.AddRow(drow)
                Me.m_db.ReleaseWriter(writerGroup)

                ' Log repair state
                Me.LogMessage(String.Format("Added missing Ecosim group {0}", iGroupID))

            Catch ex As Exception
                bSucces = False
                ' Log failure
                Me.LogMessage(String.Format("Failed to add Ecosim group {0}", iGroupID), eMessageType.NotSet, eMessageImportance.Critical)
            End Try
        End If

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create or fixes a fleet in each ecosim scenario
    ''' </summary>
    ''' <param name="iEcopathFleetID">Ecopath fleet DBID</param>
    ''' -----------------------------------------------------------------------
    Private Function AddEcosimFleetToAllScenarios(ByVal iEcopathFleetID As Integer) As Boolean

        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader(String.Format("SELECT ScenarioID FROM EcoSimScenario"))
            While reader.Read()
                bSucces = bSucces And CreateRepairEcosimFleet(iEcopathFleetID, CInt(reader("ScenarioID")))
            End While
            Me.m_db.ReleaseReader(reader)
        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Create or repair an ecosim group in a given scenario.
    ''' </summary>
    ''' <param name="iEcopathFleetID">Ecopath Group DBID.</param>
    ''' <param name="iScenarioID">Scenario ID to add the group to.</param>
    ''' -----------------------------------------------------------------------
    Private Function CreateRepairEcosimFleet(ByVal iEcopathFleetID As Integer, ByVal iScenarioID As Integer) As Boolean

        Dim readerFleet As IDataReader = Nothing
        Dim writerFleet As cEwEDatabase.cEwEDbWriter = Nothing
        Dim bFleetFound As Boolean = False
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        readerFleet = Me.m_db.GetReader(String.Format("SELECT EcopathFleetID FROM EcoSimScenarioFleet WHERE (EcopathFleetID={0}) AND (ScenarioID={1})", iEcopathFleetID, iScenarioID))
        If readerFleet IsNot Nothing Then
            Try
                readerFleet.Read()

                ' Try to find existing Sim fleet ID
                Dim iDummy As Integer = CInt(readerFleet(0))
                ' It this did not fail we have found a fleet
                bFleetFound = True
            Catch ex As InvalidOperationException
                bFleetFound = False
            End Try
            Me.m_db.ReleaseReader(readerFleet)
        End If

        ' *** Next: Critical bits, create missing entries in DB ***

        If Not bFleetFound Then
            Try
                writerFleet = Me.m_db.GetWriter("EcoSimScenarioFleet")
                drow = writerFleet.NewRow()
                drow("ScenarioID") = iScenarioID
                drow("EcopathFleetID") = iEcopathFleetID
                writerFleet.AddRow(drow)
                bSucces = bSucces And Me.m_db.ReleaseWriter(writerFleet, True)

                ' Log repair state
                Me.LogMessage(String.Format("Added missing Ecosim fleet {0}", iEcopathFleetID))

            Catch ex As Exception
                bSucces = False
                ' Log failure
                Me.LogMessage(String.Format("Failed to add Ecosim fleet {0}", iEcopathFleetID), eMessageType.NotSet, eMessageImportance.Critical)
            End Try
        End If

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <para>
    ''' *Sigh*
    ''' </para>
    ''' <para>
    ''' Due to the limited capabilities of Microzork Access SQL, database 
    ''' update-generated foreign keys to fleets and groups cannot cacading 
    ''' delete. Hence, we need to eradicate linked groups and fleets via code.
    ''' </para> 
    ''' </summary>
    ''' <param name="iEcopathFleetID"></param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function RemoveEcosimFleet(ByVal iEcopathFleetID As Integer) As Boolean
        Dim bSucces As Boolean = True
        Try
            bSucces = bSucces And Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioQuota WHERE FleetID={0}", iEcopathFleetID))
            bSucces = bSucces And Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioFleet WHERE EcopathFleetID={0}", iEcopathFleetID))
        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' <para>
    ''' *Sigh*
    ''' </para>
    ''' <para>
    ''' Due to the limited capabilities of Microzork Access SQL, database 
    ''' update-generated foreign keys to fleets and groups cannot cacading 
    ''' delete. Hence, we need to eradicate linked groups and fleets via code.
    ''' </para> 
    ''' </summary>
    ''' <param name="iDBID">DBID of the Ecosim group to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function RemoveEcosimGroup(ByVal iDBID As Integer) As Boolean
        Dim bSucces As Boolean = True
        Try

            ' Big sigh, it's even worse...
            bSucces = bSucces And Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioQuota WHERE EcosimGroupID={0}", iDBID))
            bSucces = bSucces And Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioGroup WHERE GroupID={0}", iDBID))

        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces
    End Function

#End Region ' Modify

#Region " Load "

    Private Function LoadEcosimGroups(ByVal iScenarioID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim quotaDS As cQuotaDataStructures = Me.m_core.m_QuotaData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True
        Dim iEcopathGroup As Integer = 0

        For igroup As Integer = 1 To ecosimDS.nGroups

            ' Me.CreateRepairEcosimGroup(ecopathDS.GroupDBID(j), iScenarioID, True)

            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcoSimScenarioGroup WHERE (ScenarioID={0}) AND (EcopathGroupID={1})", iScenarioID, ecopathDS.GroupDBID(igroup)))

            Try
                reader.Read()

                ' Find ecopath group index to store matching ecosim group data at
                iEcopathGroup = Array.IndexOf(ecopathDS.GroupDBID, CInt(reader("EcopathGroupID")))

                ' Read fields
                ecosimDS.GroupDBID(iEcopathGroup) = CInt(reader("GroupID"))
                ecosimDS.PBmaxs(iEcopathGroup) = CSng(reader("pbmaxs"))
                ecosimDS.FtimeMax(iEcopathGroup) = CSng(reader("FtimeMax"))
                ecosimDS.FtimeAdjust(iEcopathGroup) = CSng(reader("FtimeAdjust"))
                ecosimDS.MoPred(iEcopathGroup) = CSng(reader("MoPred"))
                ecosimDS.FishRateMax(iEcopathGroup) = CSng(reader("FishRateMax"))
                ' ecosimDS.ShowGroup(i) = CBool(reader("Show"))

                ecosimDS.RiskTime(iEcopathGroup) = CSng(reader("RiskTime"))
                ecosimDS.QmQo(iEcopathGroup) = CSng(reader("QmQo"))
                ecosimDS.CmCo(iEcopathGroup) = CSng(reader("CmCo"))
                ecosimDS.SwitchPower(iEcopathGroup) = CSng(reader("SwitchPower"))
                ecosimDS.GroupFishRateNoDBID(iEcopathGroup) = CInt(reader("FishMortShapeID"))
                ecosimDS.SalOpt(iEcopathGroup) = CSng(Me.ReadSafe(reader, "SalOpt", 35.0!))
                ecosimDS.SdSalLeft(iEcopathGroup) = CSng(Me.ReadSafe(reader, "SdSalLeft", 1000.0!))
                ecosimDS.SdSalRight(iEcopathGroup) = CSng(Me.ReadSafe(reader, "SdSalRight", 1000.0!))
                ecosimDS.TempOpt(iEcopathGroup) = CSng(Me.ReadSafe(reader, "TempOpt", 10.0!))
                ecosimDS.TempLeft(iEcopathGroup) = CSng(Me.ReadSafe(reader, "TempLeft", 1000.0!))
                ecosimDS.TempRight(iEcopathGroup) = CSng(Me.ReadSafe(reader, "TempRight", 1000.0!))

                quotaDS.Blim(iEcopathGroup) = CSng(Me.ReadSafe(reader, "Blim", -9999))
                quotaDS.Bbase(iEcopathGroup) = CSng(Me.ReadSafe(reader, "Bbase", -9999))
                quotaDS.Fopt(iEcopathGroup) = CSng(Me.ReadSafe(reader, "Fopt", -9999))
                quotaDS.FixedEscapement(iEcopathGroup) = CSng(Me.ReadSafe(reader, "FixedEscapement", 0.0!))

                mseDS.CVbiomEst(iEcopathGroup) = CSng(Me.ReadSafe(reader, "BiomassCV", mseDS.CVbiomEst(iEcopathGroup)))
                mseDS.BioRiskValue(iEcopathGroup, 0) = CSng(Me.ReadSafe(reader, "LowerRisk", mseDS.BioRiskValue(iEcopathGroup, 0)))
                mseDS.BioRiskValue(iEcopathGroup, 1) = CSng(Me.ReadSafe(reader, "UpperRisk", mseDS.BioRiskValue(iEcopathGroup, 1)))

                mseDS.DefaultBioBounds(iEcopathGroup)
                mseDS.BioBounds(iEcopathGroup).Lower = CSng(Me.ReadSafe(reader, "BiomassRefLower", mseDS.BioBounds(iEcopathGroup).Lower))
                mseDS.BioBounds(iEcopathGroup).Upper = CSng(Me.ReadSafe(reader, "BiomassRefUpper", mseDS.BioBounds(iEcopathGroup).Upper))

                mseDS.DefaultCatchBoundsGroup(iEcopathGroup)
                mseDS.CatchGroupBounds(iEcopathGroup).Lower = CSng(Me.ReadSafe(reader, "CatchRefLower", mseDS.CatchGroupBounds(iEcopathGroup).Lower))
                mseDS.CatchGroupBounds(iEcopathGroup).Upper = CSng(Me.ReadSafe(reader, "CatchRefUpper", mseDS.CatchGroupBounds(iEcopathGroup).Upper))

                ' bSucces = bSucces And Me.LoadFishMortShape(CInt(reader("FishMortShapeID")), iEcopathGroup)

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading EcoSim group info for group {1}", ex.Message, iEcopathGroup))
                bSucces = False
            End Try

            Me.m_db.ReleaseReader(reader)
            reader = Nothing
        Next
        Return bSucces

    End Function

    Private Function LoadEcosimFleets(ByVal iScenarioID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim quotaDS As cQuotaDataStructures = Me.m_core.m_QuotaData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim reader As IDataReader = Nothing
        Dim iFleetID As Integer = -1
        Dim iShapeID As Integer = -1
        Dim bSucces As Boolean = True
        Dim asDummy(ecosimDS.NTimes) As Single

        Dim dtNewFleetShapes As New Dictionary(Of Integer, Integer)

        For iPt As Integer = 0 To ecosimDS.NTimes : asDummy(iPt) = 1.0 : Next

        ' For each fleet
        For iFleet As Integer = 1 To ecosimDS.nGear
            Try
                ' Read shape for this fleet
                iFleetID = ecopathDS.FleetDBID(iFleet)
                reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcoSimScenarioFleet WHERE (ScenarioID={0}) AND (EcopathFleetID={1})", iScenarioID, iFleetID))
                reader.Read()
                iShapeID = CInt(Me.ReadSafe(reader, "FishRateShapeID", -1))
            Catch ex As Exception
                ' A different error occurred: abort!
                bSucces = False
            End Try

            If iShapeID <= 0 Then
                ' Define a new shape for this fleet
                Me.AppendShapeImpl(ecopathDS.FleetName(iFleet), eDataTypes.FishingEffort, iShapeID, asDummy, 0, 0, 0, 0, eShapeFunctionType.NotSet)
                dtNewFleetShapes.Add(iFleetID, iShapeID)
                iShapeID += 1
            End If

            If iShapeID > -1 Then
                ' JS 10Aug07: Don't fail in case FishRateShape is missing. Only those present are loaded, only those loaded are saved.
                '             Since these shapes do not need to be present we can be somewhat forgiving in this particular case.
                If Not LoadFishingRateShape(iShapeID, iFleet) Then
                    Me.LogMessage(String.Format("Warning: Fishing rate shape {0} is referenced but not present in database for EcoSim fleet {1} (ID {2})", iShapeID, iFleet, iFleetID))
                End If
            End If

            Try
                ecosimDS.Epower(iFleet) = CSng(Me.ReadSafe(reader, "Epower", 3))
                ecosimDS.PcapBase(iFleet) = CSng(Me.ReadSafe(reader, "PCapBase", 0.5))
                ecosimDS.CapDepreciate(iFleet) = CSng(Me.ReadSafe(reader, "CapDepreciate", 0.06))
                ecosimDS.CapBaseGrowth(iFleet) = CSng(Me.ReadSafe(reader, "CapBaseGrowth", 0.2))

                quotaDS.MaxEffort(iFleet) = CSng(Me.ReadSafe(reader, "MaxEffort", cCore.NULL_VALUE))
                quotaDS.QuotaType(iFleet) = DirectCast(CInt(Me.ReadSafe(reader, "QuotaType", 0)), eQuotaTypes)

                mseDS.CVFest(iFleet) = CSng(Me.ReadSafe(reader, "CV", mseDS.CVFest(iFleet)))
                mseDS.Qgrow(iFleet) = CSng(Me.ReadSafe(reader, "QIncrease", mseDS.Qgrow(iFleet)))

                mseDS.DefaultCatchBoundsFleet(iFleet)
                mseDS.CatchFleetBounds(iFleet).Lower = CSng(Me.ReadSafe(reader, "CatchRefLower", mseDS.CatchFleetBounds(iFleet).Lower))
                mseDS.CatchFleetBounds(iFleet).Upper = CSng(Me.ReadSafe(reader, "CatchRefUpper", mseDS.CatchFleetBounds(iFleet).Upper))
                mseDS.EffortFleetBounds(iFleet).Lower = CSng(Me.ReadSafe(reader, "EffortRefLower", mseDS.EffortFleetBounds(iFleet).Lower))
                mseDS.EffortFleetBounds(iFleet).Upper = CSng(Me.ReadSafe(reader, "EffortRefUpper", mseDS.EffortFleetBounds(iFleet).Upper))
                'mseDS.MSYEvaluateFleet(iFleet) = (CInt(Me.ReadSafe(reader, "MSYEvaluateFleet", True)) = 1)

            Catch ex As Exception
                bSucces = False
            End Try

            Me.m_db.ReleaseReader(reader)
        Next

        ' Store new shape links
        Dim writer As cEwEDatabase.cEwEDbWriter = Me.m_db.GetWriter("EcosimScenarioFleet")
        Dim dt As DataTable = writer.GetDataTable()
        Dim objKeys() As Object = {iScenarioID, Nothing}
        Dim drow As DataRow = Nothing

        ' Store new IDs
        For Each iFleetID In dtNewFleetShapes.Keys
            iShapeID = dtNewFleetShapes(iFleetID)
            objKeys(1) = iFleetID
            drow = dt.Rows.Find(objKeys)
            ' Check wheter a new row or an existing row
            Debug.Assert(Not Object.ReferenceEquals(drow, Nothing))
            Try
                drow.BeginEdit()
                drow("FishRateShapeID") = iShapeID
                drow.EndEdit()
            Catch ex As Exception
                bSucces = False
            End Try
        Next

        Me.m_db.ReleaseWriter(writer)
        Return bSucces

    End Function

    Private Function LoadEcosimQuota(ByVal iScenarioID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim quotaDS As cQuotaDataStructures = Me.m_core.m_QuotaData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim reader As IDataReader = Nothing
        Dim iFleetID As Integer = -1
        Dim iFleet As Integer = -1
        Dim iGroupID As Integer = -1
        Dim iGroup As Integer = -1
        Dim bSucces As Boolean = True

        reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcoSimScenarioQuota WHERE (ScenarioID={0})", iScenarioID))

        Try
            While reader.Read()
                iFleetID = CInt(reader("FleetID"))
                iFleet = Array.IndexOf(ecopathDS.FleetDBID, iFleetID)

                iGroupID = CInt(reader("EcosimGroupID"))
                iGroup = Array.IndexOf(ecosimDS.GroupDBID, iGroupID)

                If (iFleet > 0) And (iGroup > 0) Then
                    quotaDS.Quota(iFleet, iGroup) = CSng(reader("Quota"))
                    mseDS.Fweight(iFleet, iGroup) = CSng(Me.ReadSafe(reader, "FWeight", 1.0))
                End If
            End While

        Catch ex As Exception
            bSucces = False
        End Try
        Me.m_db.ReleaseReader(reader)

        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    Private Function SaveEcosimGroups(ByRef idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim quotaDS As cQuotaDataStructures = Me.m_core.m_QuotaData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bNewRow As Boolean = False
        Dim iScenarioID As Integer = 0
        Dim iNextGroupID As Integer = 0
        Dim iGroupID As Integer = 0
        Dim bSucces As Boolean = True
        Dim objKeys() As Object = {Nothing, Nothing}

        Dim iActiveEcosimScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim bDuplicating As Boolean = (idm.GetID(eDataTypes.EcoSimScenario, iActiveEcosimScenarioID) <> iActiveEcosimScenarioID)
        Dim iNextShapeID As Integer = 0
        Dim iShapeID As Integer = 0

        ' Obtain mapped scenario ID
        iScenarioID = idm.GetID(eDataTypes.EcoSimScenario, ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario))

        ' Get next available shape ID
        Try
            iNextShapeID = CInt(Me.m_db.GetValue("SELECT MAX(ShapeID) FROM EcoSimShape")) + 1
        Catch ex As Exception
            iNextShapeID = 1
        End Try

        ' Get next available group ID
        Try
            iNextGroupID = CInt(Me.m_db.GetValue("SELECT MAX(GroupID) FROM EcosimScenarioGroup")) + 1
        Catch ex As Exception
            iNextGroupID = 1
        End Try

        ' JS 28may07: Change of strategy. The primary key in table EcosimScenarioGroup has been changed from
        '             (ScenarioID, SimGroupID) to (ScenarioID, PathGroupID) for the simple reason that when
        '             overwriting an existing scenario the new SimGroupIDs are unknown, while PathGroupIDs
        '             are always known. 
        '             This change in primary keys will not jeopardize performance or referential integrity.
        objKeys(0) = iScenarioID

        Try
            writer = Me.m_db.GetWriter("EcosimScenarioGroup")
            dt = writer.GetDataTable()
            For i As Integer = 1 To ecosimDS.nGroups

                ' Find row for scenario and ecopath ID
                objKeys(1) = ecopathDS.GroupDBID(i)
                drow = dt.Rows.Find(objKeys)

                bNewRow = (drow Is Nothing)
                If bNewRow Then
                    drow = writer.NewRow()
                    drow("ScenarioID") = iScenarioID
                    drow("EcopathGroupID") = ecopathDS.GroupDBID(i)
                    drow("GroupID") = iNextGroupID
                    iNextGroupID += 1
                Else
                    drow.BeginEdit()
                End If

                iGroupID = CInt(drow("GroupID"))

                ' Store ecosim group ID mapping now we know it
                ' JS 12Jul09: group mapping is stored by ECOPATH group ID since this is the only constant
                '             factor while appending Ecosim scenarios. Above, CreateRepairEcosimGroup is
                '             called to complement missing Ecosim groups, which will create the groups
                '             in the database for a given Ecosim scenario but this will not update the
                '             ecosim datastructures. This caused the ID mapping context to be populated
                '             with Ecosim IDs for groups from the previous scenario, NOT the new scenario,
                '             thus creating Ecosim scenarios what were bugged right from the start.
                idm.Add(eDataTypes.EcoSimGroupInput, ecopathDS.GroupDBID(i), iGroupID)

                drow("pbmaxs") = ecosimDS.PBmaxs(i)
                drow("FtimeMax") = ecosimDS.FtimeMax(i)
                drow("FtimeAdjust") = ecosimDS.FtimeAdjust(i)
                drow("MoPred") = ecosimDS.MoPred(i)
                drow("FishRateMax") = ecosimDS.FishRateMax(i)
                ' drow("Show") = ecosimDS.ShowGroup(i)
                drow("RiskTime") = ecosimDS.RiskTime(i)
                drow("QmQo") = ecosimDS.QmQo(i)
                drow("CmCo") = ecosimDS.CmCo(i)
                drow("SwitchPower") = ecosimDS.SwitchPower(i)

                ' JS 01Jan09: mort shapes unique per scenario
                If bDuplicating Then
                    idm.Add(eDataTypes.FishMort, ecosimDS.GroupFishRateNoDBID(i), iNextShapeID)
                    iNextShapeID += 1
                End If
                drow("FishMortShapeID") = idm.GetID(eDataTypes.FishMort, ecosimDS.GroupFishRateNoDBID(i))

                drow("SalOpt") = ecosimDS.SalOpt(i)
                drow("SdSalLeft") = ecosimDS.SdSalLeft(i)
                drow("SdSalRight") = ecosimDS.SdSalRight(i)
                drow("TempOpt") = ecosimDS.TempOpt(i)
                drow("TempLeft") = ecosimDS.TempLeft(i)
                drow("TempRight") = ecosimDS.TempRight(i)

                drow("Blim") = quotaDS.Blim(i)
                drow("Bbase") = quotaDS.Bbase(i)
                drow("Fopt") = quotaDS.Fopt(i)
                drow("FixedEscapement") = quotaDS.FixedEscapement(i)

                drow("BiomassCV") = mseDS.CVbiomEst(i)
                drow("LowerRisk") = mseDS.BioRiskValue(i, 0)
                drow("UpperRisk") = mseDS.BioRiskValue(i, 1)
                drow("BiomassRefLower") = mseDS.BioBounds(i).Lower
                drow("BiomassRefUpper") = mseDS.BioBounds(i).Upper
                drow("CatchRefLower") = mseDS.CatchGroupBounds(i).Lower
                drow("CatchRefUpper") = mseDS.CatchGroupBounds(i).Upper

                If bNewRow Then
                    writer.AddRow(drow)
                Else
                    drow.EndEdit()
                End If
            Next i
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function SaveEcosimFleets(ByRef idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim quotaDS As cQuotaDataStructures = Me.m_core.m_QuotaData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bNewRow As Boolean = False
        Dim iScenarioID As Integer = 0
        Dim bSucces As Boolean = True
        Dim objKeys() As Object = {Nothing, Nothing}

        Dim iActiveEcosimScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim bDuplicating As Boolean = (idm.GetID(eDataTypes.EcoSimScenario, iActiveEcosimScenarioID) <> iActiveEcosimScenarioID)
        Dim iNextShapeID As Integer = 0
        Dim iShapeID As Integer = 0

        ' Obtain mapped scenario ID
        iScenarioID = idm.GetID(eDataTypes.EcoSimScenario, ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario))

        ' Get next available shape ID
        Try
            iNextShapeID = CInt(Me.m_db.GetValue("SELECT MAX(ShapeID) FROM EcoSimShape")) + 1
        Catch ex As Exception
            iNextShapeID = 1
        End Try

        objKeys(0) = iScenarioID

        Try
            writer = Me.m_db.GetWriter("EcosimScenarioFleet")
            dt = writer.GetDataTable()
            For iFleet As Integer = 1 To ecopathDS.NumFleet

                objKeys(1) = idm.GetID(eDataTypes.FleetInput, ecopathDS.FleetDBID(iFleet))
                drow = dt.Rows.Find(objKeys)
                ' Check wheter a new row or an existing row
                bNewRow = Object.ReferenceEquals(drow, Nothing)
                ' New row?
                If bNewRow Then
                    ' #Yes: create new row
                    drow = writer.NewRow()
                    ' Populate PK
                    drow("ScenarioID") = objKeys(0)
                    drow("EcopathFleetID") = objKeys(1)
                Else
                    ' #No: edit the row
                    drow.BeginEdit()
                End If

                If bDuplicating Then
                    iShapeID = iNextShapeID
                    iNextShapeID += 1
                Else
                    iShapeID = CInt(drow("FishRateShapeID"))
                End If
                idm.Add(eDataTypes.FishingEffort, ecosimDS.FishRateGearDBID(iFleet), iShapeID)

                ' Write dynamic bit
                drow("FishRateShapeID") = iShapeID
                drow("MaxEffort") = quotaDS.MaxEffort(iFleet)
                drow("QuotaType") = CInt(quotaDS.QuotaType(iFleet))
                drow("Epower") = ecosimDS.Epower(iFleet)
                drow("PCapBase") = ecosimDS.PcapBase(iFleet)
                drow("CapDepreciate") = ecosimDS.CapDepreciate(iFleet)
                drow("CapBaseGrowth") = ecosimDS.CapBaseGrowth(iFleet)

                drow("CV") = mseDS.CVFest(iFleet)
                drow("QIncrease") = mseDS.Qgrow(iFleet)
                drow("CatchRefLower") = mseDS.CatchFleetBounds(iFleet).Lower
                drow("CatchRefUpper") = mseDS.CatchFleetBounds(iFleet).Upper
                drow("EffortRefLower") = mseDS.CatchFleetBounds(iFleet).Lower
                drow("EffortRefUpper") = mseDS.CatchFleetBounds(iFleet).Upper
                'drow("MSYEvaluateFleet") = CInt(IIf(mseDS.MSYEvaluateFleet(iFleet), 1, 0))

                ' Wrap up: was this a new row?
                If bNewRow Then
                    ' #Yes: add it to the writer
                    writer.AddRow(drow)
                Else
                    ' #No: done editing
                    drow.EndEdit()
                End If
            Next iFleet
            ' Done
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function SaveEcosimQuota(ByRef idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim quotaDS As cQuotaDataStructures = Me.m_core.m_QuotaData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim strSQL As String = ""
        Dim iScenarioID As Integer = 0
        Dim bSucces As Boolean = True

        ' Obtain mapped scenario ID
        iScenarioID = idm.GetID(eDataTypes.EcoSimScenario, ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario))

        strSQL = String.Format("DELETE FROM EcosimScenarioQuota WHERE (ScenarioID={0})", iScenarioID)
        bSucces = Me.m_db.Execute(strSQL)

        Try
            writer = Me.m_db.GetWriter("EcosimScenarioQuota")
            For iFleet As Integer = 1 To ecopathDS.NumFleet
                For iGroup As Integer = 1 To ecopathDS.NumGroups
                    ' Conjure row
                    drow = writer.NewRow()
                    ' Populate key
                    drow("ScenarioID") = iScenarioID
                    drow("FleetID") = idm.GetID(eDataTypes.FleetInput, ecopathDS.FleetDBID(iFleet))
                    drow("EcosimGroupID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecopathDS.GroupDBID(iGroup))
                    ' Write dynamic bit
                    drow("Quota") = quotaDS.Quota(iFleet, iGroup)
                    drow("Fweight") = mseDS.Fweight(iFleet, iGroup)
                    ' Add new row to the writer
                    writer.AddRow(drow)
                Next iGroup
            Next iFleet
            ' Done
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Save

#End Region ' Groups, fleets

#Region " Forcing and Mediaton shapes "

    Private Function LoadShapes() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim reader As IDataReader = Nothing
        Dim iShapeID As Integer = 0
        Dim shapeDataType As eDataTypes = eDataTypes.NotSet
        Dim iForcingShape As Integer = 0
        Dim iMediationShape As Integer = 0
        Dim iFishingMortShape As Integer = 0
        Dim iFishRateShape As Integer = 0
        Dim bSucces As Boolean = True

        Dim strQuery As String = ""

        strQuery = String.Format("SELECT COUNT(*) FROM EcosimShape WHERE (ShapeType={0} OR ShapeType={1})", CInt(eDataTypes.EggProd), CInt(eDataTypes.Forcing))
        ecosimDS.ForcingShapes = CInt(Me.m_db.GetValue(strQuery))

        strQuery = String.Format("SELECT COUNT(*) FROM EcosimShape WHERE (ShapeType={0})", CInt(eDataTypes.Mediation))
        ecosimDS.MediationShapes = CInt(Me.m_db.GetValue(strQuery))

        ecosimDS.DimForcingShapes()
        ecosimDS.InitForcingShapes()
        ecosimDS.ReDimMediation()

        Try

            reader = Me.m_db.GetReader("SELECT * FROM EcosimShape")
            While reader.Read()

                iShapeID = CInt(reader("ShapeID"))
                shapeDataType = DirectCast(reader("ShapeType"), eDataTypes)

                Select Case shapeDataType

                    Case eDataTypes.EggProd
                        iForcingShape += 1
                        bSucces = bSucces And Me.LoadEggShape(iShapeID, iForcingShape, CBool(reader("IsSeasonal")))

                    Case eDataTypes.Forcing
                        iForcingShape += 1
                        bSucces = bSucces And Me.LoadTimeShape(iShapeID, iForcingShape, CBool(reader("IsSeasonal")))

                    Case eDataTypes.Mediation
                        iMediationShape += 1
                        bSucces = bSucces And Me.LoadMediationShape(iShapeID, iMediationShape)

                    Case eDataTypes.FishingEffort
                        ' Shape type loaded from LoadEcosimFleets(); do not handle here
                        'iFishRateShape += 1
                        'bSucces = bSucces And Me.LoadFishingRateShape(iShapeID, iFishRateShape)

                    Case eDataTypes.FishMort
                        ' Shape type loaded from LoadEcosimGroups(); do not handle here
                        'iFishingMortShape += 1
                        'bSucces = bSucces And Me.LoadFishMortShape(iShapeID, iFishingMortShape)

                    Case Else
                        Debug.Assert(False, String.Format("Cannot load invalid shapetype {0} for shape ID {1}", shapeDataType, iShapeID))

                End Select

            End While
            Me.m_db.ReleaseReader(reader)
            reader = Nothing

        Catch ex As Exception
            bSucces = False
        End Try

        '' Sanity checks discontinued since core may arbitrarily set ecosimDS.MediationShapes to 9
        'Debug.Assert(ecosimDS.MediationShapes = iMediationShape)

        Try
            ' Read and assign scenario forcing shape number(s)
            reader = Me.m_db.GetReader(String.Format("SELECT NutForcingShapeID, SalinityForcingShapeID, TemperatureForcingShapeID FROM EcosimScenario WHERE (ScenarioID={0})", iScenarioID))
            reader.Read()
            iForcingShape = CInt(Me.ReadSafe(reader, "NutForcingShapeID", 0))
            ecosimDS.NutForceNumber = Math.Max(0, Array.IndexOf(ecosimDS.ForcingDBIDs, iForcingShape))
            iForcingShape = CInt(Me.ReadSafe(reader, "SalinityForcingShapeID", 0))
            ecosimDS.SalinityForceNo = Math.Max(0, Array.IndexOf(ecosimDS.ForcingDBIDs, iForcingShape))
            iForcingShape = CInt(Me.ReadSafe(reader, "TemperatureForcingShapeID", 0))
            ecosimDS.TemperatureForceNo = Math.Max(0, Array.IndexOf(ecosimDS.ForcingDBIDs, iForcingShape))
            Me.m_db.ReleaseReader(reader)
            reader = Nothing
        Catch ex As Exception
            bSucces = False
        End Try

        bSucces = bSucces And Me.LoadForcingMatrix()
        bSucces = bSucces And Me.LoadPredPreyInteraction()
        bSucces = bSucces And Me.LoadMediationWeights()
        bSucces = bSucces And Me.LoadStanzaShapeAssignments()

        Return bSucces

    End Function

#Region " Shape load helpers "

    Private Function LoadEggShape(ByVal iShapeID As Integer, ByVal iForcingShape As Integer, _
            Optional ByVal bIsSeasonal As Boolean = False) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim shapeParms As New cEcosimDatastructures.ShapeParameters()
        Dim readerShape As IDataReader = Nothing
        Dim astrZScale() As String
        Dim bSucces As Boolean = True

        Try

            readerShape = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimShapeEggProd WHERE (ShapeID={0})", iShapeID))
            readerShape.Read()
            shapeParms.YZero = CSng(readerShape("Yzero"))
            shapeParms.YBase = CSng(readerShape("Ybase"))
            shapeParms.YEnd = CSng(readerShape("Yend"))
            shapeParms.Steep = CSng(readerShape("Steep"))
            ' sp.ZScale = CInt(readerShape("ZScale"))
            shapeParms.ShapeFunctionType = CType(readerShape("FunctionType"), eShapeFunctionType)

            ' Read z-scale
            astrZScale = Me.SplitNumberString(CStr(readerShape("Zscale")))
            For ipt As Integer = 1 To Math.Min(ecosimDS.ForcePoints, astrZScale.Length)
                ecosimDS.zscale(ipt, iForcingShape) = StringUtils.ConvertToSingle(astrZScale(ipt - 1), 0)
            Next ipt
            For ipt As Integer = Math.Min(ecosimDS.ForcePoints, astrZScale.Length) + 1 To ecosimDS.ForcePoints
                ecosimDS.zscale(ipt, iForcingShape) = 1.0
            Next

            ecosimDS.ForcingShapeParams(iForcingShape) = shapeParms
            ecosimDS.ForcingDBIDs(iForcingShape) = iShapeID
            ecosimDS.ForcingTitles(iForcingShape) = CStr(readerShape("Title"))
            ecosimDS.ForcingShapeType(iForcingShape) = eDataTypes.EggProd
            ecosimDS.isSeasonal(iForcingShape) = bIsSeasonal

            Me.m_db.ReleaseReader(readerShape)
            readerShape = Nothing

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EggShape {1}", ex.Message, iShapeID))
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function LoadTimeShape(ByVal iShapeID As Integer, _
                                   ByVal iForcingShape As Integer, _
                                   Optional ByVal bIsSeasonal As Boolean = False) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim shapeParms As New cEcosimDatastructures.ShapeParameters()
        Dim readerShape As IDataReader = Nothing
        Dim astrZScale() As String
        Dim bSucces As Boolean = True

        Try
            readerShape = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimShapeTime WHERE (ShapeID={0})", iShapeID))
            readerShape.Read()

            ' Read shape parameters
            shapeParms.YZero = CSng(readerShape("Yzero"))
            shapeParms.YBase = CSng(readerShape("Ybase"))
            shapeParms.YEnd = CSng(readerShape("Yend"))
            shapeParms.Steep = CSng(readerShape("Steep"))
            shapeParms.ShapeFunctionType = CType(readerShape("FunctionType"), eShapeFunctionType)

            ' Read z-scale
            astrZScale = Me.SplitNumberString(CStr(readerShape("Zscale")))
            For ipt As Integer = 1 To Math.Min(ecosimDS.ForcePoints, astrZScale.Length)
                ecosimDS.zscale(ipt, iForcingShape) = StringUtils.ConvertToSingle(astrZScale(ipt - 1), 0)
            Next ipt
            For ipt As Integer = Math.Min(ecosimDS.ForcePoints, astrZScale.Length) + 1 To ecosimDS.ForcePoints
                ecosimDS.zscale(ipt, iForcingShape) = 1.0
            Next

            ecosimDS.ForcingShapeParams(iForcingShape) = shapeParms
            ecosimDS.ForcingDBIDs(iForcingShape) = iShapeID
            ecosimDS.ForcingTitles(iForcingShape) = CStr(readerShape("Title"))
            ecosimDS.ForcingShapeType(iForcingShape) = eDataTypes.Forcing
            ecosimDS.ForcingApplicationType(iForcingShape) = DirectCast(Me.ReadSafe(readerShape, "ApplicationType", eForcingApplicationTypes.NotSet), eForcingApplicationTypes)
            ecosimDS.isSeasonal(iForcingShape) = bIsSeasonal

            Me.m_db.ReleaseReader(readerShape)
            readerShape = Nothing

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading TimeShape {1}", ex.Message, iShapeID))
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function LoadMediationShape(ByVal iShapeID As Integer, ByVal iMediationShape As Integer) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim shapeParms As New cEcosimDatastructures.ShapeParameters()
        Dim readerShape As IDataReader = Nothing
        Dim astrZScale() As String
        Dim bSucces As Boolean = True

        Try
            readerShape = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimShapeMediation WHERE (ShapeID={0})", iShapeID))
            readerShape.Read()

            ' Init shapeParms
            shapeParms.YZero = CSng(readerShape("Yzero"))
            shapeParms.YBase = CSng(readerShape("Ybase"))
            shapeParms.YEnd = CSng(readerShape("Yend"))
            shapeParms.Steep = CSng(readerShape("Steep"))
            ' shapeParms.ZScale = CInt(readerShape("ZScale"))
            shapeParms.ShapeFunctionType = CType(readerShape("FunctionType"), eShapeFunctionType)

            ' Read z-scale
            astrZScale = Me.SplitNumberString(CStr(readerShape("Zscale")))
            ' Write points
            For ipt As Integer = 1 To Math.Min(ecosimDS.NMedPoints, astrZScale.Length)
                ecosimDS.Medpoints(ipt, iMediationShape) = StringUtils.ConvertToSingle(astrZScale(ipt - 1), 0)
            Next ipt
            For ipt As Integer = Math.Min(ecosimDS.NMedPoints, astrZScale.Length) + 1 To ecosimDS.NMedPoints
                ecosimDS.Medpoints(ipt, iMediationShape) = 1.0
            Next

            ecosimDS.MediationShapeParams(iMediationShape) = shapeParms
            ecosimDS.MediationDBIDs(iMediationShape) = iShapeID
            ecosimDS.MediationTitles(iMediationShape) = CStr(readerShape("Title"))
            ecosimDS.IMedBase(iMediationShape) = CInt(Me.ReadSafe(readerShape, "IMedBase", 1200 / 3))

            Me.m_db.ReleaseReader(readerShape)
            readerShape = Nothing

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading MediationShape {1}", ex.Message, iShapeID))
            bSucces = False
        End Try


        Return bSucces

    End Function

    Private Function LoadForcingMatrix() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim reader As IDataReader = Nothing
        Dim iPredator As Integer = 0
        Dim iPrey As Integer = 0
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimScenarioForcingMatrix WHERE (ScenarioID={0})", iScenarioID))
            While reader.Read()

                ' Find iPredator
                iPredator = Array.IndexOf(ecosimDS.GroupDBID, CInt(reader("PredID")))
                ' Find iPrey
                iPrey = Array.IndexOf(ecosimDS.GroupDBID, CInt(reader("PreyID")))

                ecosimDS.VulMult(iPrey, iPredator) = CSng(reader("vulnerability"))

            End While
            Me.m_db.ReleaseReader(reader)
            reader = Nothing

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading ForcingMatrix", ex.Message))
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function LoadPredPreyInteraction() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim reader As IDataReader = Nothing
        Dim iPredator As Integer = 0
        Dim iPrey As Integer = 0
        Dim iShapeID As Integer = 0
        Dim iShape As Integer = 0
        Dim bSucces As Boolean = True
        Dim iFNo(ecosimDS.nGroups, ecosimDS.nGroups) As Integer

        Try

            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimScenarioPredPreyShape WHERE (ScenarioID={0})", iScenarioID))
            While reader.Read()

                ' Find iPredator
                iPredator = Array.IndexOf(ecosimDS.GroupDBID, CInt(reader("PredID")))
                ' Find iPrey
                iPrey = Array.IndexOf(ecosimDS.GroupDBID, CInt(reader("PreyID")))
                ' Next shape
                iFNo(iPrey, iPredator) += 1
                ' Resolve shape ID
                iShapeID = CInt(reader("ShapeID"))
                ' Determine shape type
                iShape = Array.IndexOf(ecosimDS.MediationDBIDs, iShapeID)
                ' Is a mediation shape?
                If iShape <> -1 Then
                    ' #Yes: flag as mediation shape
                    ecosimDS.IsMedFunction(iPrey, iPredator, iFNo(iPrey, iPredator)) = True
                Else
                    ' #No: flag as other shape
                    ecosimDS.IsMedFunction(iPrey, iPredator, iFNo(iPrey, iPredator)) = False
                    ' Obtain forcing index
                    iShape = Array.IndexOf(ecosimDS.ForcingDBIDs, iShapeID)
                End If
                ' Update sim fields
                ecosimDS.FunctionNumber(iPrey, iPredator, iFNo(iPrey, iPredator)) = iShape
                ecosimDS.FunctionType(iPrey, iPredator, iFNo(iPrey, iPredator)) = CType(reader("FunctionType"), eForcingFunctionApplication)
            End While

            Me.m_db.ReleaseReader(reader)
            reader = Nothing

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading PredPreyInteraction", ex.Message))
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the mediation weights for the active scenario.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadMediationWeights() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim readerGroup As IDataReader = Nothing
        Dim readerFleet As IDataReader = Nothing
        Dim iGroup As Integer = 0
        Dim iFleet As Integer = 0
        Dim iShape As Integer = 0
        Dim bSucces As Boolean = True

        Try
            readerGroup = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimScenarioShapeMedWeightsGroup WHERE (ScenarioID={0})", iScenarioID))
            If (readerGroup IsNot Nothing) Then
                While readerGroup.Read()
                    iShape = Array.IndexOf(ecosimDS.MediationDBIDs, readerGroup("ShapeID"))
                    iGroup = Array.IndexOf(ecosimDS.GroupDBID, readerGroup("GroupID"))
                    If (iGroup <> -1 And iShape <> -1) Then
                        ecosimDS.MedWeights(iGroup, iShape) = CSng(readerGroup("MedWeights"))
                    End If
                End While
                Me.m_db.ReleaseReader(readerGroup)
                readerGroup = Nothing
            End If
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading group MediationWeights", ex.Message))
            bSucces = False
        End Try

        Try
            readerFleet = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimScenarioShapeMedWeightsFleet WHERE (ScenarioID={0})", iScenarioID))
            If (readerFleet IsNot Nothing) Then
                While readerFleet.Read()
                    iShape = Array.IndexOf(ecosimDS.MediationDBIDs, readerFleet("ShapeID"))
                    iFleet = Array.IndexOf(ecopathDS.FleetDBID, readerFleet("FleetID"))
                    If (iFleet <> -1 And iShape <> -1) Then ecosimDS.MedWeights(iFleet + ecosimDS.nGroups, iShape) = CSng(readerFleet("MedWeights"))
                End While
                Me.m_db.ReleaseReader(readerFleet)
                readerFleet = Nothing
            End If
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading fleet MediationWeights", ex.Message))
            bSucces = False
        End Try

        Return True
    End Function

    Private Function LoadStanzaShapeAssignments() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim reader As IDataReader = Nothing
        Dim iStanza As Integer = 0
        Dim iShape As Integer = 0
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader("SELECT * FROM EcosimStanzaShape")
            While reader.Read()
                ' Get iStanza 
                iStanza = Array.IndexOf(stanzaDS.StanzaDBID, CInt(reader("StanzaID")))
                ' Is valid stanza?
                If (iStanza > 0) Then
                    ' #Yes: has egg production shape?
                    If Not Convert.IsDBNull(reader("EggprodShapeID")) Then
                        ' #Yes: resolve shape index iShape
                        iShape = Array.IndexOf(ecosimDS.ForcingDBIDs, CInt(reader("EggprodShapeID")))
                        ' Is a valid shape index?
                        If (iShape > 0) Then
                            ' #Yes: assign
                            stanzaDS.EggProdShapeSplit(iStanza) = iShape
                        End If
                    End If
                    ' #Yes: has hatch code forcing shape?
                    If Not Convert.IsDBNull(reader("HatchCodeShapeID")) Then
                        ' #Yes: resolve shape index iShape
                        iShape = Array.IndexOf(ecosimDS.ForcingDBIDs, CInt(reader("HatchCodeShapeID")))
                        ' Is a valid shape index?
                        If (iShape > 0) Then
                            ' #Yes: assign
                            stanzaDS.HatchCode(iStanza) = iShape
                        End If
                    End If
                End If ' Is valid stanza
            End While

            Me.m_db.ReleaseReader(reader)
            reader = Nothing

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading stanza shape assignments", ex.Message))
            bSucces = False
        End Try
        Return bSucces
    End Function

    Private Function LoadFishingRateShape(ByVal iShapeID As Integer, ByVal iFishingRateShape As Integer) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim readerShape As IDataReader = Nothing
        Dim strMemo As String = ""
        Dim astrMemoBits() As String
        Dim bSucces As Boolean = True

        If iShapeID = 0 Then Return bSucces

        Try

            readerShape = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimShapeFishRate WHERE (ShapeID={0})", iShapeID))
            readerShape.Read()

            ecosimDS.FishRateGearTitle(iFishingRateShape) = CStr(readerShape("Title"))
            strMemo = CStr(readerShape("zScale"))
            astrMemoBits = strMemo.Trim.Split(CChar(" "))
            For j As Integer = 1 To Math.Min(ecosimDS.NTimes, astrMemoBits.Length)
                ecosimDS.FishRateGear(iFishingRateShape, j) = StringUtils.ConvertToSingle(astrMemoBits(j - 1), 1)
            Next
            ecosimDS.FishRateGearDBID(iFishingRateShape) = iShapeID

            Me.m_db.ReleaseReader(readerShape)
            readerShape = Nothing

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading FishingRate {1}", ex.Message, iShapeID))
            bSucces = False
        End Try

        Return bSucces

    End Function

#If 0 Then ' Fish mort data is solely an output: no more need to store in DB

    Private Function LoadFishMortShape(ByVal iShapeID As Integer, ByVal iForcingShape As Integer) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim readerShape As IDataReader = Nothing
        Dim strMemo As String = ""
        Dim astrMemoBits() As String
        Dim bSucces As Boolean = True

        If iShapeID = 0 Then Return bSucces

        Try

            readerShape = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimShapeFishMort WHERE (ShapeID={0})", iShapeID))
            readerShape.Read()
            ' Store ID
            ecosimDS.FishRateNoDBID(iForcingShape) = iShapeID
            ' Store title
            ecosimDS.FishRateNoTitle(iForcingShape) = CStr(readerShape("Title"))
            ' Store points
            strMemo = CStr(readerShape("zScale"))
            ' Got points?
            If Not String.IsNullOrEmpty(strMemo) Then
                ' #Yes: split and process
                astrMemoBits = strMemo.Trim.Split(CChar(" "))
                For j As Integer = 1 To Math.Min(ecosimDS.NTimes, astrMemoBits.Length)
                    ecosimDS.FishRateNo(iForcingShape, j) = StringUtils.ConvertToSingle(astrMemoBits(j - 1), 0)
                Next
            End If

            Me.m_db.ReleaseReader(readerShape)
            readerShape = Nothing

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading fish mortality shape {1}", ex.Message, iShapeID))
            bSucces = False
        End Try

        Return bSucces

    End Function

#End If

#End Region ' Shape load helpers

    Private Function SaveShapes(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim iShape As Integer = 0
        Dim iShapeID As Integer = 0
        Dim drow As DataRow = Nothing
        Dim bNewRow As Boolean = False
        Dim bSucces As Boolean = True

        Try
            ' Start writing
            writer = Me.m_db.GetWriter("EcoSimShape")
            dt = writer.GetDataTable()

            For iShape = 1 To ecosimDS.ForcingShapes
                ' JS 10aug07: this should be an assert
                If (ecosimDS.ForcingDBIDs(iShape) > 0) Then
                    drow = dt.Rows.Find(ecosimDS.ForcingDBIDs(iShape))
                    bNewRow = (drow Is Nothing)

                    If bNewRow Then
                        drow = writer.NewRow()
                        drow("ShapeID") = ecosimDS.ForcingDBIDs(iShape)
                    Else
                        drow.BeginEdit()
                    End If
                    drow("ShapeType") = ecosimDS.ForcingShapeType(iShape)
                    drow("IsSeasonal") = ecosimDS.isSeasonal(iShape)
                    If bNewRow Then
                        writer.AddRow(drow)
                    Else
                        drow.EndEdit()
                    End If
                    writer.Commit()

                    Select Case ecosimDS.ForcingShapeType(iShape)
                        Case eDataTypes.EggProd
                            bSucces = bSucces And SaveEggShape(iShape)
                        Case eDataTypes.Forcing
                            bSucces = bSucces And SaveTimeShape(iShape)
                        Case Else
                            Debug.Assert(False)
                    End Select

                End If
            Next iShape

            For iShape = 1 To ecosimDS.MediationShapes
                If (ecosimDS.MediationDBIDs(iShape) > 0) Then
                    drow = dt.Rows.Find(ecosimDS.MediationDBIDs(iShape))
                    bNewRow = (drow Is Nothing)

                    If bNewRow Then
                        drow = writer.NewRow()
                        drow("ShapeID") = ecosimDS.MediationDBIDs(iShape)
                    Else
                        drow.BeginEdit()
                    End If

                    drow("ShapeType") = eDataTypes.Mediation

                    If bNewRow Then
                        writer.AddRow(drow)
                    Else
                        drow.EndEdit()
                    End If
                    writer.Commit()
                    bSucces = bSucces And SaveMediationShape(iShape)
                End If
            Next iShape

            ' JS 01Jan10: duplicate effort shapes if duplicating a scenario
            For iShape = 1 To ecosimDS.FishRateGearDBID.Length - 1
                iShapeID = idm.GetID(eDataTypes.FishingEffort, ecosimDS.FishRateGearDBID(iShape))
                If (iShapeID > 0) Then

                    drow = dt.Rows.Find(iShapeID)
                    bNewRow = (drow Is Nothing)

                    If bNewRow Then
                        drow = writer.NewRow()
                        drow("ShapeID") = iShapeID
                    Else
                        drow.BeginEdit()
                    End If

                    drow("ShapeType") = eDataTypes.FishingEffort

                    If bNewRow Then
                        writer.AddRow(drow)
                        writer.Commit()
                    Else
                        drow.EndEdit()
                    End If
                    bSucces = bSucces And Me.SaveFishingRateShape(iShape, idm)
                End If
            Next iShape

            ' JS 01Jan10: duplicate mortality shapes if duplicating a scenario
            For iShape = 1 To ecosimDS.FishRateNoDBID.Length - 1
                iShapeID = idm.GetID(eDataTypes.FishMort, ecosimDS.FishRateNoDBID(iShape))
                If (iShapeID > 0) Then

                    drow = dt.Rows.Find(iShapeID)
                    bNewRow = (drow Is Nothing)

                    If bNewRow Then
                        drow = writer.NewRow()
                        drow("ShapeID") = iShapeID
                    Else
                        drow.BeginEdit()
                    End If

                    drow("ShapeType") = eDataTypes.FishMort

                    If bNewRow Then
                        writer.AddRow(drow)
                        writer.Commit()
                    Else
                        drow.EndEdit()
                    End If
                    ' bSucces = bSucces And Me.SaveFishMortShape(iShape, idm)
                End If
            Next iShape

            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving forcing shapes", ex.Message))
            bSucces = False
        End Try

        bSucces = bSucces And SaveForcingMatrix(idm)
        bSucces = bSucces And SavePredPreyInteraction(idm)
        bSucces = bSucces And SaveMediationWeights(idm)
        bSucces = bSucces And SaveStanzaShapeAssignments(idm)

        Return bSucces

    End Function

#Region " Shape save helpers "

    Private Function SaveEggShape(ByVal iShape As Integer) As Boolean

        ' ToDo: see if passing in an adapter and a datatable may speed up the save process significantly

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iDBID As Integer = ecosimDS.ForcingDBIDs(iShape)
        Dim shapeParms As cEcosimDatastructures.ShapeParameters = ecosimDS.ForcingShapeParams(iShape)
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim sbZScale As New Text.StringBuilder()
        Dim drow As DataRow = Nothing
        Dim bNewRow As Boolean = False
        Dim bSucces As Boolean = True

        ' Sanity check
        Debug.Assert(ecosimDS.ForcingShapeType(iShape) = eDataTypes.EggProd)

        Try
            writer = Me.m_db.GetWriter("EcosimShapeEggProd")
            dt = writer.GetDataTable()
            drow = dt.Rows.Find(iDBID)
            bNewRow = (drow Is Nothing)

            If bNewRow Then
                drow = writer.NewRow()
                drow("ShapeID") = iDBID
            Else
                drow.BeginEdit()
            End If

            drow("Title") = ecosimDS.ForcingTitles(iShape)
            drow("YZero") = shapeParms.YZero
            drow("YBase") = shapeParms.YBase
            drow("YEnd") = shapeParms.YEnd
            drow("Steep") = shapeParms.Steep
            drow("FunctionType") = CInt(shapeParms.ShapeFunctionType)
            ' Assemble Zscale
            For ipt As Integer = 1 To ecosimDS.ForcePoints
                If (ipt > 1) Then sbZScale.Append(" ")
                sbZScale.Append(StringUtils.FormatSingle(ecosimDS.zscale(ipt, iShape)))
            Next
            drow("Zscale") = sbZScale.ToString()

            If bNewRow Then
                writer.AddRow(drow)
            Else
                drow.EndEdit()
            End If

            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function SaveTimeShape(ByVal iShape As Integer) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iDBID As Integer = ecosimDS.ForcingDBIDs(iShape)
        Dim shapeParms As cEcosimDatastructures.ShapeParameters = ecosimDS.ForcingShapeParams(iShape)
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim sbZScale As New Text.StringBuilder()
        Dim adrows() As DataRow = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        ' Sanity check
        Debug.Assert(ecosimDS.ForcingShapeType(iShape) = eDataTypes.Forcing)

        Try
            writer = Me.m_db.GetWriter("EcosimShapeTime")
            dt = writer.GetDataTable()
            adrows = dt.Select(String.Format("ShapeID={0}", iDBID))
            If adrows.Length = 1 Then
                drow = adrows(0)
                drow.BeginEdit()
            Else
                drow = writer.NewRow()
                drow("ShapeID") = iDBID
            End If

            drow("Title") = ecosimDS.ForcingTitles(iShape)
            drow("YZero") = shapeParms.YZero
            drow("YBase") = shapeParms.YBase
            drow("YEnd") = shapeParms.YEnd
            drow("Steep") = shapeParms.Steep
            drow("FunctionType") = CInt(shapeParms.ShapeFunctionType)
            drow("ApplicationType") = ecosimDS.ForcingApplicationType(iShape)

            ' Assemble Zscale
            For ipt As Integer = 1 To ecosimDS.ForcePoints
                If (ipt > 1) Then sbZScale.Append(" ")
                sbZScale.Append(StringUtils.FormatSingle(ecosimDS.zscale(ipt, iShape)))
            Next
            drow("Zscale") = sbZScale.ToString()

            If adrows.Length = 1 Then
                drow.EndEdit()
            Else
                writer.AddRow(drow)
            End If
            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function SaveMediationShape(ByVal iShape As Integer) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iDBID As Integer = ecosimDS.MediationDBIDs(iShape)
        Dim shapeParms As cEcosimDatastructures.ShapeParameters = ecosimDS.MediationShapeParams(iShape)
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim sbZScale As New Text.StringBuilder()
        Dim adrows() As DataRow = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            writer = Me.m_db.GetWriter("EcosimShapeMediation")
            dt = writer.GetDataTable()
            adrows = dt.Select(String.Format("ShapeID={0}", iDBID))
            If adrows.Length = 1 Then
                drow = adrows(0)
                drow.BeginEdit()
            Else
                drow = writer.NewRow()
                drow("ShapeID") = iDBID
            End If

            drow("Title") = ecosimDS.MediationTitles(iShape)
            drow("YZero") = shapeParms.YZero
            drow("YBase") = shapeParms.YBase
            drow("YEnd") = shapeParms.YEnd
            drow("Steep") = shapeParms.Steep
            drow("IMedBase") = ecosimDS.IMedBase(iShape)
            drow("FunctionType") = CInt(shapeParms.ShapeFunctionType)
            ' Assemble Zscale
            For ipt As Integer = 1 To ecosimDS.NMedPoints
                If (ipt > 1) Then sbZScale.Append(" ")
                sbZScale.Append(StringUtils.FormatSingle(ecosimDS.Medpoints(ipt, iShape)))
            Next
            drow("Zscale") = sbZScale.ToString()

            If adrows.Length = 1 Then
                drow.EndEdit()
            Else
                writer.AddRow(drow)
            End If
            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function SaveForcingMatrix(ByRef idm As cIDMappings) As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iPredator As Integer = 0
        Dim iPrey As Integer = 0
        Dim iShapeID As Integer = 0
        Dim bSucces As Boolean = True

        Try
            Me.m_db.Execute(String.Format("DELETE FROM EcoSimScenarioForcingMatrix WHERE (ScenarioID={0})", iScenarioID))
            writer = Me.m_db.GetWriter("EcoSimScenarioForcingMatrix")

            For iPredator = 1 To ecosimDS.nGroups
                For iPrey = 1 To ecosimDS.nGroups

                    drow = writer.NewRow()
                    drow("PredID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecopathDS.GroupDBID(iPredator))
                    drow("PreyID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecopathDS.GroupDBID(iPrey))
                    drow("ScenarioID") = idm.GetID(eDataTypes.EcoSimScenario, iScenarioID)
                    drow("vulnerability") = ecosimDS.VulMult(iPrey, iPredator)
                    writer.AddRow(drow)

                Next iPrey
            Next iPredator

            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

    Private Function SavePredPreyInteraction(ByRef idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iScenarioID As Integer = idm.GetID(eDataTypes.EcoSimScenario, ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario))
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iShape As Integer = 0
        Dim bSucces As Boolean = True

        Try

            Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioPredPreyShape WHERE (ScenarioID={0})", iScenarioID))
            writer = Me.m_db.GetWriter("EcosimScenarioPredPreyShape")

            For iPredator As Integer = 1 To ecosimDS.nGroups
                For iPrey As Integer = 1 To ecosimDS.nGroups
                    For iShapeNo As Integer = 1 To ecosimDS.MaxFunctions - 1

                        Try

                            ' Get shape assignment
                            iShape = ecosimDS.FunctionNumber(iPrey, iPredator, iShapeNo)
                            ' Is an assignment?
                            If (iShape > 0) Then
                                ' Save assignment
                                drow = writer.NewRow()
                                drow("ScenarioID") = iScenarioID
                                drow("PredID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecopathDS.GroupDBID(iPredator))
                                drow("PreyID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecopathDS.GroupDBID(iPrey))
                                If (ecosimDS.IsMedFunction(iPrey, iPredator, iShapeNo)) Then
                                    drow("ShapeID") = ecosimDS.MediationDBIDs(iShape)
                                Else
                                    drow("ShapeID") = ecosimDS.ForcingDBIDs(iShape)
                                End If
                                drow("FunctionType") = ecosimDS.FunctionType(iPrey, iPredator, iShapeNo)
                                writer.AddRow(drow)
                            End If
                        Catch ex As Exception
                            'Debug.Assert(False, String.Format("Index error on pred {0}, prey {1}, shape {2}", iPredator, iPrey, iShape))
                        End Try

                    Next iShapeNo
                Next iPrey
            Next iPredator

            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving PredPreyInteraction", ex.Message))
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function SaveMediationWeights(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iScenarioID As Integer = 0
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        ' Obtain mapped scenario ID
        iScenarioID = idm.GetID(eDataTypes.EcoSimScenario, ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario))

        Try
            Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioshapeMedWeightsGroup WHERE (ScenarioID={0})", iScenarioID))
            writer = Me.m_db.GetWriter("EcosimScenarioshapeMedWeightsGroup")
            For iGroup As Integer = 1 To ecosimDS.nGroups
                For iShape As Integer = 1 To ecosimDS.MediationShapes
                    If ecosimDS.MedWeights(iGroup, iShape) > 0 Then
                        drow = writer.NewRow()
                        drow("ScenarioID") = iScenarioID
                        ' Ecosim groups unique per scenario: map this
                        drow("GroupID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecopathDS.GroupDBID(iGroup))
                        drow("ShapeID") = ecosimDS.MediationDBIDs(iShape)
                        drow("MedWeights") = ecosimDS.MedWeights(iGroup, iShape)
                        writer.AddRow(drow)
                    End If
                Next iShape
            Next iGroup
            Me.m_db.ReleaseWriter(writer, True)

            Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioShapeMedWeightsFleet WHERE (ScenarioID={0})", iScenarioID))
            writer = Me.m_db.GetWriter("EcosimScenarioShapeMedWeightsFleet")
            For iFleet As Integer = 1 To ecosimDS.nGear
                For iShape As Integer = 1 To ecosimDS.MediationShapes
                    If ecosimDS.MedWeights(iFleet + ecosimDS.nGroups, iShape) > 0 Then
                        drow = writer.NewRow()
                        drow("ScenarioID") = iScenarioID
                        drow("FleetID") = ecopathDS.FleetDBID(iFleet)
                        drow("ShapeID") = ecosimDS.MediationDBIDs(iShape)
                        drow("MedWeights") = ecosimDS.MedWeights(iFleet + ecosimDS.nGroups, iShape)
                        writer.AddRow(drow)
                    End If
                Next iShape
            Next iFleet
            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

    Private Function SaveStanzaShapeAssignments(ByVal idm As cIDMappings) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            ' Erase all
            Me.m_db.Execute("DELETE * FROM EcosimStanzaShape")
            ' Get writer
            writer = Me.m_db.GetWriter("EcosimStanzaShape")

            ' For every stanza
            For iStanza As Integer = 1 To stanzaDS.Nsplit
                ' Has any shape assignment?
                If (stanzaDS.EggProdShapeSplit(iStanza) > 0) Or (stanzaDS.HatchCode(iStanza) > 0) Then
                    ' #Yes: start new row
                    drow = writer.NewRow()
                    ' Set PK
                    drow("StanzaID") = stanzaDS.StanzaDBID(iStanza)

                    ' EggProdShapeID identifies the egg prod shape assigned. Do not specify anything
                    ' to leave the field at DBNull
                    If (stanzaDS.EggProdShapeSplit(iStanza) > 0) Then
                        drow("EggprodShapeID") = ecosimDS.ForcingDBIDs(CInt(stanzaDS.EggProdShapeSplit(iStanza)))
                    Else
                        ' For missing shape this value MUST BE set to DBNull (not 0)
                    End If

                    ' HatchCodeShapeID identifies the egg prod shape assigned. Do not specify anything
                    ' to leave the field at DBNull
                    If (stanzaDS.HatchCode(iStanza) > 0) Then
                        drow("HatchCodeShapeID") = ecosimDS.ForcingDBIDs(CInt(stanzaDS.HatchCode(iStanza)))
                    Else
                        ' For missing shape this value MUST BE set to DBNull (not 0)
                    End If

                    ' Done
                    writer.AddRow(drow)
                End If
            Next
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

    Private Function SaveFishingRateShape(ByVal iShape As Integer, ByVal idm As cIDMappings) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iDBID As Integer = idm.GetID(eDataTypes.FishingEffort, ecosimDS.FishRateGearDBID(iShape))
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim sbZScale As New Text.StringBuilder()
        Dim adrows() As DataRow = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Debug.Assert(iDBID > 0, String.Format("Invalid ID for FishingRate shape {0}", iDBID))

        Try
            writer = Me.m_db.GetWriter("EcosimShapeFishRate")
            dt = writer.GetDataTable()
            adrows = dt.Select(String.Format("ShapeID={0}", iDBID))
            If adrows.Length = 1 Then
                drow = adrows(0)
                drow.BeginEdit()
            Else
                drow = writer.NewRow()
                drow("ShapeID") = iDBID
            End If

            drow("ShapeID") = iDBID
            drow("Title") = ecosimDS.FishRateGearTitle(iShape)
            For ipt As Integer = 1 To ecosimDS.NTimes
                If (ipt > 1) Then sbZScale.Append(" ")
                sbZScale.Append(StringUtils.FormatSingle(ecosimDS.FishRateGear(iShape, ipt)))
            Next
            drow("Zscale") = sbZScale.ToString()

            If adrows.Length = 1 Then
                drow.EndEdit()
            Else
                writer.AddRow(drow)
            End If
            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

#If 0 Then ' Fish mort data is solely an output: no more need to store in DB

    Private Function SaveFishMortShape(ByVal iShape As Integer, ByVal idm As cIDMappings) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iDBID As Integer = idm.GetID(eDataTypes.FishMort, ecosimDS.FishRateNoDBID(iShape))
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim sbZScale As New Text.StringBuilder()
        Dim adrows() As DataRow = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Debug.Assert(iDBID > 0, String.Format("Invalid ID for FishMortShape shape {0}", iDBID))

        Try
            writer = Me.m_db.GetWriter("EcosimShapeFishMort")
            dt = writer.GetDataTable()
            adrows = dt.Select(String.Format("ShapeID={0}", iDBID))
            If adrows.Length = 1 Then
                drow = adrows(0)
                drow.BeginEdit()
            Else
                drow = writer.NewRow()
                drow("ShapeID") = iDBID
            End If

            drow("Title") = ecosimDS.FishRateNoTitle(iShape)
            For ipt As Integer = 1 To ecosimDS.NTimes
                If (ipt > 1) Then sbZScale.Append(" ")
                sbZScale.Append(StringUtils.FormatSingle(ecosimDS.FishRateNo(iShape, ipt)))
            Next
            drow("Zscale") = sbZScale.ToString()

            If adrows.Length = 1 Then
                drow.EndEdit()
            Else
                writer.AddRow(drow)
            End If
            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

#End If

#End Region ' Shape save helpers

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Appends a forcing shape to the datasource.
    ''' </summary>
    ''' <param name="strShapeName">Name to assign to new shape.</param>
    ''' <param name="shapeType"><see cref="eDataTypes">Type of the shape</see> to add.</param>
    ''' <param name="iDBID">Database ID assigned to the new shape.</param>
    ''' <param name="asData">Shape point data.</param>
    ''' <param name="sYZero">Zero data point shape primitive was created from.</param>
    ''' <param name="sYBase">Base Y shape primitive was created from.</param>
    ''' <param name="sYend">End Y shape primitve was created from.</param>
    ''' <param name="sSteep">Steep value that shape primitive was created from.</param>
    ''' <param name="functionType">Primitive function type shape was created from.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function AppendShape(ByVal strShapeName As String, ByVal shapeType As eDataTypes, ByRef iDBID As Integer, _
            ByVal asData As Single(), ByVal sYZero As Single, ByVal sYBase As Single, ByVal sYend As Single, ByVal sSteep As Single, ByVal functionType As eShapeFunctionType) As Boolean _
            Implements IEcosimDatasource.AppendShape

        If Me.AppendShapeImpl(strShapeName, shapeType, iDBID, asData, sYZero, sYBase, sYend, sSteep, functionType) Then
            ' #Yes: reload
            'jb the number of shapes has changed in the database so we need to reload all the shape data in memory
            Return Me.LoadShapes()
        End If

        Return False

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Append a shape to the database, internal implementation.
    ''' </summary>
    ''' <param name="strShapeName"></param>
    ''' <param name="shapeType"></param>
    ''' <param name="iDBID"></param>
    ''' <param name="asData"></param>
    ''' <param name="sYZero"></param>
    ''' <param name="sYBase"></param>
    ''' <param name="sYend"></param>
    ''' <param name="sSteep"></param>
    ''' <param name="functionType"></param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Private Function AppendShapeImpl(ByVal strShapeName As String, ByVal shapeType As eDataTypes, ByRef iDBID As Integer, _
            ByVal asData As Single(), ByVal sYZero As Single, ByVal sYBase As Single, ByVal sYend As Single, ByVal sSteep As Single, ByVal functionType As eShapeFunctionType) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim writerID As cEwEDatabase.cEwEDbWriter = Me.m_db.GetWriter("EcoSimShape")
        Dim writerShape As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try

            Try
                iDBID = CInt(Me.m_db.GetValue("SELECT MAX(ShapeID) FROM EcoSimShape")) + 1
            Catch
                iDBID = 1
            End Try

            drow = writerID.NewRow()
            drow("ShapeID") = iDBID
            drow("ShapeType") = shapeType
            drow("IsSeasonal") = (shapeType = eDataTypes.EggProd)
            writerID.AddRow(drow)
            writerID.Commit()

            ' Write sub-shape row
            Select Case shapeType

                Case eDataTypes.EggProd
                    writerShape = Me.m_db.GetWriter("EcosimShapeEggProd")

                Case eDataTypes.Forcing
                    writerShape = Me.m_db.GetWriter("EcosimShapeTime")

                Case eDataTypes.Mediation
                    writerShape = Me.m_db.GetWriter("EcosimShapeMediation")

                Case eDataTypes.FishingEffort
                    writerShape = Me.m_db.GetWriter("EcosimShapeFishRate")

                Case eDataTypes.FishMort
                    writerShape = Me.m_db.GetWriter("EcosimShapeFishMort")

                Case eDataTypes.NotSet
                    Debug.Assert(False, String.Format("Cannot load invalid shapetype for shape ID {0}", iDBID))
                    Return False

            End Select

            ' Sanity check
            Debug.Assert(writerShape IsNot Nothing)

            drow = writerShape.NewRow()
            drow("ShapeID") = iDBID
            drow("Title") = strShapeName

            If Object.ReferenceEquals(asData, Nothing) Then
                drow("zScale") = ""
            Else
                Dim sbZScale As New Text.StringBuilder()
                ' Assemble Zscale
                For ipt As Integer = 1 To Math.Min(ecosimDS.ForcePoints, asData.Length - 1)
                    If (ipt > 1) Then sbZScale.Append(" ")
                    sbZScale.Append(StringUtils.FormatSingle(asData(ipt)))
                Next
                drow("zScale") = sbZScale.ToString()
            End If

            ' Specific bits
            Select Case shapeType
                Case eDataTypes.FishingEffort
                Case eDataTypes.FishMort
                Case Else
                    drow("YZero") = sYZero
                    drow("YBase") = sYBase
                    drow("YEnd") = sYend
                    drow("Steep") = sSteep
                    drow("FunctionType") = CInt(functionType)
            End Select

            writerShape.AddRow(drow)

            Me.m_db.ReleaseWriter(writerShape, True)
            Me.m_db.ReleaseWriter(writerID)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while appending shape {1}, {2}", ex.Message, strShapeName, shapeType.ToString()))
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Deletes a forcing shape from the DB.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the shape to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' <remarks>The number of shapes has changed in the database so all the
    ''' shape data is reloaded in memory.</remarks>
    ''' -------------------------------------------------------------------
    Friend Function RemoveShape(ByVal iDBID As Integer) As Boolean _
            Implements IEcosimDatasource.RemoveShape

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim bSucces As Boolean = True

        Try

            ' Manually set 'soft' shape links to 0
            Me.m_db.Execute(String.Format("UPDATE EcoSimStanzaShape Set EggProdShapeID=NULL WHERE (EggProdShapeID={0})", iDBID))
            Me.m_db.Execute(String.Format("UPDATE EcoSimStanzaShape Set HatchCodeShapeID=NULL WHERE (HatchCodeShapeID={0})", iDBID))
            Me.m_db.Execute("DELETE FROM EcoSimStanzaShape WHERE ((HatchCodeShapeID=NULL) AND (EggProdShapeID=NULL))")

            Me.m_db.Execute(String.Format("UPDATE EcoSimScenario Set SalinityForcingShapeID=NULL WHERE (SalinityForcingShapeID={0})", iDBID))
            Me.m_db.Execute(String.Format("UPDATE EcoSimScenario Set NutForcingShapeID=NULL WHERE (NutForcingShapeID={0})", iDBID))

            ' Delete mediation weights
            Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioshapeMedWeightsGroup WHERE (ShapeID={0})", iDBID))
            Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioShapeMedWeightsFleet WHERE (ShapeID={0})", iDBID))

            ' Delete pred/prey interactions
            Me.m_db.Execute(String.Format("DELETE FROM EcosimScenarioPredPreyShape WHERE (ShapeID={0})", iDBID))

            ' Destroy the given shape
            Me.m_db.Execute(String.Format("DELETE FROM EcoSimShape WHERE (ShapeID={0})", iDBID))
            ' Reload shapes data
            bSucces = Me.LoadShapes()

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while deleting shape {1}", ex.Message, iDBID))
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Forcing and Mediaton shapes

#Region " Time series "

#Region " Import "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Import a <see cref="cTimeSeriesImport">cTimeSeriesImport</see> instance into the datasource.
    ''' </summary>
    ''' <param name="ts">The time series data to import.</param>
    ''' <param name="iDataset">Index of the dataset to add the time series to.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function ImportTimeSeries(ByVal ts As cTimeSeriesImport, ByVal iDataset As Integer) As Boolean _
          Implements IEcosimDatasource.ImportTimeSeries

        Select Case cTimeSeriesFactory.TimeSeriesCategory(ts.TimeSeriesType)
            Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group, _
                 cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet
                Return Me.AddAsTimeSeries(ts, iDataset)
            Case cTimeSeriesFactory.eTimeSeriesCategoryType.Forcing
                Return Me.AddAsForcingFunction(ts)
            Case cTimeSeriesFactory.eTimeSeriesCategoryType.NotSet
                Debug.Assert(False)
                Return False
        End Select

    End Function

#Region " Import helpers "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ts"></param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Private Function AddAsForcingFunction(ByVal ts As cTimeSeriesImport) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim iShape As Integer = 0
        Dim drow As DataRow = Nothing
        Dim iNextShapeID As Integer = 0
        Dim bSucces As Boolean = True
        Dim sbZScale As New Text.StringBuilder()
        Dim iRepetitions As Integer = 1

        ' ToDo: find if FF with this name exists. If so, overwrite

        Try
            iNextShapeID = CInt(Me.m_db.GetValue("SELECT MAX(ShapeID) FROM EcoSimShape")) + 1
        Catch
            iNextShapeID = 1
        End Try

        Try
            ' Start writing
            writer = Me.m_db.GetWriter("EcoSimShape")
            drow = writer.NewRow()
            drow("ShapeID") = iNextShapeID
            drow("ShapeType") = eDataTypes.Forcing
            writer.AddRow(drow)
            writer.Commit()
            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception

        End Try

        Try
            writer = Me.m_db.GetWriter("EcosimShapeTime")

            drow = writer.NewRow()
            drow("ShapeID") = iNextShapeID
            drow("Title") = ts.Name
            drow("YZero") = 0
            drow("YBase") = 0
            drow("YEnd") = 0
            drow("Steep") = 0
            drow("FunctionType") = eShapeFunctionType.NotSet

            ' Assemble Zscale. 
            ' JS 04april09: Time Series are most likely ANNUAL, FFs are MONTHLY
            If ts.IsMonthly Then iRepetitions = 1 Else iRepetitions = cCore.N_MONTHS

            For iYear As Integer = 0 To ts.XMax - 1
                For iMonth As Integer = 1 To iRepetitions
                    If sbZScale.Length > 0 Then sbZScale.Append(" ")
                    sbZScale.Append(StringUtils.FormatSingle(ts.ShapeData(iYear)))
                Next
            Next

            drow("Zscale") = sbZScale.ToString()
            writer.AddRow(drow)

            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ts"></param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Private Function AddAsTimeSeries(ByVal ts As cTimeSeriesImport, ByVal iDataset As Integer) As Boolean

        Dim tsds As cTimeSeriesDataStructures = Me.m_core.m_TSData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iTimeSeriesID As Integer = 0
        Dim sbValues As New Text.StringBuilder()
        Dim bSucces As Boolean = True

        Try
            iTimeSeriesID = CInt(Me.m_db.GetValue("SELECT MAX(TimeSeriesID) FROM EcosimTimeSeries")) + 1
        Catch ex As Exception
            iTimeSeriesID = 1
        End Try

        ' Time series are scenario-independant
        writer = Me.m_db.GetWriter("EcosimTimeSeries", "Sequence")

        drow = writer.NewRow()
        drow("TimeSeriesID") = iTimeSeriesID
        drow("Sequence") = iTimeSeriesID
        drow("DatName") = ts.Name
        drow("DatType") = ts.TimeSeriesType
        drow("WtType") = ts.WtType
        drow("DatasetID") = tsds.iDatasetDBID(iDataset)

        ' Concoct time series memo
        For iYear As Integer = 0 To ts.XMax - 1
            If (iYear > 0) Then sbValues.Append(" ")
            sbValues.Append(StringUtils.FormatSingle(ts.ShapeData(iYear)))
        Next
        drow("TimeValues") = sbValues.ToString()

        writer.AddRow(drow)
        Me.m_db.ReleaseWriter(writer, True)

        Select Case cTimeSeriesFactory.TimeSeriesCategory(ts.TimeSeriesType)
            Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group
                bSucces = bSucces And Me.AddGroupTimeSeries(ts, iTimeSeriesID)
            Case cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet
                bSucces = bSucces And Me.AddFleetTimeSeries(ts, iTimeSeriesID)
        End Select

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ts"></param>
    ''' <param name="iTimeSeriesID"></param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Private Function AddGroupTimeSeries(ByVal ts As cTimeSeriesImport, ByVal iTimeSeriesID As Integer) As Boolean

        Dim writerGroup As cEwEDatabase.cEwEDbWriter = Nothing
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        ' Validate DatPool
        If (ts.DatPool < 1) Or (ts.DatPool >= ecopathDS.GroupDBID.Length) Then
            ' No group for this pool ID
            Return False
        End If

        Try
            writerGroup = Me.m_db.GetWriter("EcosimTimeSeriesGroup")
            drow = writerGroup.NewRow()
            drow("TimeSeriesID") = iTimeSeriesID
            drow("GroupID") = ecopathDS.GroupDBID(ts.DatPool)
            drow("VariableName") = ts.CustomVariableName()
            writerGroup.AddRow(drow)
            Me.m_db.ReleaseWriter(writerGroup, True)
        Catch ex As Exception
            ' Woops
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ts"></param>
    ''' <param name="iTimeSeriesID"></param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Private Function AddFleetTimeSeries(ByVal ts As cTimeSeriesImport, ByVal iTimeSeriesID As Integer) As Boolean

        Dim writerFleet As cEwEDatabase.cEwEDbWriter = Nothing
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        ' Validate DatPool
        If (ts.DatPool < 1) Or (ts.DatPool >= ecopathDS.FleetDBID.Length) Then
            ' No fleet for this pool ID
            Return False
        End If

        Try
            writerFleet = Me.m_db.GetWriter("EcosimTimeSeriesFleet")
            drow = writerFleet.NewRow()
            drow("TimeSeriesID") = iTimeSeriesID
            drow("FleetID") = ecopathDS.FleetDBID(ts.DatPool)
            writerFleet.AddRow(drow)
        Catch ex As Exception
            ' Woops
            bSucces = False
        End Try

        Me.m_db.ReleaseWriter(writerFleet, True)

        Return bSucces

    End Function

#End Region ' Import helpers

#End Region ' Import

#Region " Load "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Load all time series for a given dataset.
    ''' </summary>
    ''' <param name="iDataset">Index of dataset to load.</param>
    ''' <returns></returns>
    ''' -------------------------------------------------------------------
    Public Function LoadTimeSeriesDataset(ByVal iDataset As Integer) As Boolean _
              Implements DataSources.IEcosimDatasource.LoadTimeSeriesDataset

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim tsDS As cTimeSeriesDataStructures = Me.m_core.m_TSData
        Dim strSQL As String = ""
        Dim reader As IDataReader = Nothing
        Dim readerSub As IDataReader = Nothing
        Dim astrTimeValues() As String
        Dim iTimeSeriesID As Integer = 0
        Dim iSeries As Integer = 1
        Dim iIndex As Integer = 0
        Dim iYear As Integer = 0
        Dim bSucces As Boolean = True

        tsDS.ClearTimeSeries()
        tsDS.ActiveDatasetIndex = iDataset
        tsDS.nMaxYears = tsDS.nDatasetNumYears(iDataset)

        ' JS 20oct07: datasource should NOT do this; is responsibility of core logic
        tsDS.nGroups = ecopathDS.NumGroups

        If (iDataset > 0) Then

            Try
                tsDS.nNumTimeSeries = CInt(Me.m_db.GetValue(String.Format("SELECT COUNT(*) FROM EcosimTimeSeries WHERE (DatasetID={0})", tsDS.iDatasetDBID(iDataset))))
            Catch ex As Exception
                tsDS.nNumTimeSeries = 0
            End Try

        End If

        tsDS.RedimTimeSeries()
        tsDS.RedimEnabledTimeSeries()

        If tsDS.nNumTimeSeries = 0 Then Return bSucces

        strSQL = String.Format("SELECT * FROM EcosimTimeSeries WHERE (DatasetID={0}) ORDER BY Sequence ASC", tsDS.iDatasetDBID(iDataset))
        reader = Me.m_db.GetReader(strSQL)
        Try
            While reader.Read()

                tsDS.iTimeSeriesDBID(iSeries) = CInt(reader("TimeSeriesID"))
                tsDS.strName(iSeries) = CStr(reader("DatName"))
                tsDS.TimeSeriesType(iSeries) = DirectCast(CInt(reader("DatType")), eTimeSeriesType)
                tsDS.sWeight(iSeries) = CSng(reader("WtType"))

                Select Case cTimeSeriesFactory.TimeSeriesCategory(CType(tsDS.TimeSeriesType(iSeries), eTimeSeriesType))

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group
                        readerSub = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimTimeSeriesGroup WHERE (TimeSeriesID={0})", reader("TimeSeriesID")))
                        Try
                            readerSub.Read()
                            iIndex = Array.IndexOf(ecopathDS.GroupDBID, CInt(readerSub("GroupID")))
                            tsDS.strCustomVariableName(iSeries) = CStr(readerSub("VariableName"))
                        Catch ex As Exception
                            iIndex = -1
                        End Try
                        Me.m_db.ReleaseReader(readerSub)
                        readerSub = Nothing

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet
                        readerSub = Me.m_db.GetReader(String.Format("SELECT * FROM EcosimTimeSeriesFleet WHERE (TimeSeriesID={0})", reader("TimeSeriesID")))
                        Try
                            readerSub.Read()
                            iIndex = Array.IndexOf(ecopathDS.FleetDBID, CInt(readerSub("FleetID")))
                        Catch ex As Exception
                            iIndex = -1
                        End Try
                        Me.m_db.ReleaseReader(readerSub)
                        readerSub = Nothing

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Forcing
                        Debug.Assert(False, String.Format("Time series {0} should have been imported as a forcing function", reader("TimeSeriesID")))
                        bSucces = False

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.NotSet
                        Debug.Assert(False, String.Format("Time series {0} is of an unknown type", reader("TimeSeriesID")))
                        bSucces = False

                End Select

                tsDS.iPool(iSeries) = iIndex

                astrTimeValues = CStr(reader("TimeValues")).Split(CChar(" "))

                'Debug.Assert((astrTimeValues.Length - 1) <= tsDS.nMaxYears)

                For iYear = 1 To Math.Min(tsDS.nDatasetNumYears(iDataset), astrTimeValues.Length)
                    Try
                        tsDS.sValues(iYear, iSeries) = StringUtils.ConvertToSingle(astrTimeValues(iYear - 1))
                    Catch ex As Exception
                        ex = ex
                        ' Woops
                    End Try
                Next

                iSeries += 1
            End While

            Me.m_db.ReleaseReader(reader)
        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    Private Function SaveTimeSeries(ByRef idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim tsDS As cTimeSeriesDataStructures = Me.m_core.m_TSData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim writerGroups As cEwEDatabase.cEwEDbWriter = Nothing
        Dim writerFleets As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim dtFleets As DataTable = Nothing
        Dim dtGroups As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bHasRow As Boolean = False
        Dim sbValues As New Text.StringBuilder()
        Dim iPoolID As Integer = 0
        Dim bSucces As Boolean = True

        Try

            ' Time series are scenario-independent
            writer = Me.m_db.GetWriter("EcosimTimeSeries", "Sequence")
            dt = writer.GetDataTable()

            writerGroups = Me.m_db.GetWriter("EcosimTimeSeriesGroup")
            dtGroups = writerGroups.GetDataTable()

            writerFleets = Me.m_db.GetWriter("EcosimTimeSeriesFleet")
            dtFleets = writerFleets.GetDataTable()

            For iTS As Integer = 1 To tsDS.nNumTimeSeries

                drow = dt.Rows.Find(tsDS.iTimeSeriesDBID(iTS))
                Debug.Assert(drow IsNot Nothing, String.Format("Cannot find time series {0}", tsDS.iTimeSeriesDBID(iTS)))

                drow.BeginEdit()
                drow("DatName") = tsDS.strName(iTS)
                drow("DatType") = tsDS.TimeSeriesType(iTS)
                drow("WtType") = tsDS.sWeight(iTS)

                ' Concoct time series memo
                sbValues.Length = 0
                For iYear As Integer = 1 To tsDS.nDatasetNumYears(tsDS.ActiveDatasetIndex)
                    If (iYear > 1) Then sbValues.Append(" ")
                    sbValues.Append(StringUtils.FormatSingle(tsDS.sValues(iYear, iTS)))
                Next
                drow("TimeValues") = sbValues.ToString()

                drow.EndEdit()

                Select Case cTimeSeriesFactory.TimeSeriesCategory(DirectCast(tsDS.TimeSeriesType(iTS), eTimeSeriesType))

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet

                        drow = dtFleets.Rows.Find(tsDS.iTimeSeriesDBID(iTS))
                        bHasRow = (Object.ReferenceEquals(drow, Nothing) = False)

                        If bHasRow Then drow.BeginEdit() Else drow = writerFleets.NewRow() : drow("TimeSeriesID") = tsDS.iTimeSeriesDBID(iTS)

                        If (tsDS.iPool(iTS) > 0) Then
                            iPoolID = ecopathDS.FleetDBID(tsDS.iPool(iTS))
                        Else
                            iPoolID = 0
                        End If
                        drow("FleetID") = iPoolID

                        If bHasRow Then drow.EndEdit() Else writerFleets.AddRow(drow)

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group

                        drow = dtGroups.Rows.Find(tsDS.iTimeSeriesDBID(iTS))
                        bHasRow = (Object.ReferenceEquals(drow, Nothing) = False)

                        If bHasRow Then drow.BeginEdit() Else drow = writerGroups.NewRow() : drow("TimeSeriesID") = tsDS.iTimeSeriesDBID(iTS)

                        If (tsDS.iPool(iTS) > 0) Then
                            iPoolID = ecopathDS.GroupDBID(tsDS.iPool(iTS))
                        Else
                            iPoolID = 0
                        End If

                        drow("GroupID") = iPoolID
                        drow("VariableName") = tsDS.strCustomVariableName(iTS)
                        If bHasRow Then drow.EndEdit() Else writerGroups.AddRow(drow)

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Forcing, cTimeSeriesFactory.eTimeSeriesCategoryType.NotSet
                        Debug.Assert(False)

                End Select

            Next iTS

            Me.m_db.ReleaseWriter(writerGroups)
            Me.m_db.ReleaseWriter(writerFleets)
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception

            bSucces = False

        End Try
        Return bSucces

    End Function

#End Region ' Save

#Region " Modify "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds a time series to the datasource.
    ''' </summary>
    ''' <param name="strName">Name of the new Time Series to add.</param>
    ''' <param name="timeSeriesType"><see cref="eTimeSeriesType">Type</see> of the time series.</param>
    ''' <param name="iPool">Group/fleet code to assign to TS.</param>
    ''' <param name="sWeight">Relative weight of TS.</param>
    ''' <param name="asValues">Initial values to set in the TS.</param>
    ''' <param name="iDBID">Database ID assigned to the new TS.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function AppendTimeSeries(ByVal strName As String, _
        ByVal iPool As Integer, ByVal timeSeriesType As eTimeSeriesType, _
        ByVal sWeight As Single, ByVal asValues() As Single, _
        ByRef iDBID As Integer) As Boolean _
            Implements DataSources.IEcosimDatasource.AppendTimeSeries

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim tsDS As cTimeSeriesDataStructures = Me.m_core.m_TSData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim writerSub As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iPosition As Integer = 0
        Dim drowSub As DataRow = Nothing
        Dim bSucces As Boolean = True
        Dim sbValues As New StringBuilder

        If tsDS.ActiveDatasetIndex < 0 Then
            Console.WriteLine("No dataset loaded, cannot add time series")
            Return False
        End If

        Try
            iDBID = CInt(Me.m_db.GetValue("SELECT MAX(TimeSeriesID) FROM EcosimTimeSeries")) + 1
            iPosition = CInt(Me.m_db.GetValue("SELECT MAX(Sequence) FROM EcosimTimeSeries")) + 1
        Catch
            iDBID = 1
        End Try

        Try
            ' Start writing, protect sequence
            writer = Me.m_db.GetWriter("EcosimTimeSeries", "Sequence")
            drow = writer.NewRow()
            drow("TimeSeriesID") = iDBID
            drow("DatasetID") = tsDS.iDatasetDBID(tsDS.ActiveDatasetIndex)
            drow("DatName") = strName
            drow("DatType") = timeSeriesType
            drow("Sequence") = iPosition
            drow("WtType") = sWeight

            ' Concoct time series memo
            For iYear As Integer = 0 To asValues.Length - 1
                If (iYear > 0) Then sbValues.Append(" ")
                sbValues.Append(StringUtils.FormatSingle((iYear)))
            Next
            drow("TimeValues") = sbValues.ToString()
            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer)

            Select Case cTimeSeriesFactory.TimeSeriesCategory(timeSeriesType)

                Case cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet
                    writerSub = Me.m_db.GetWriter("EcosimTimeSeriesFleet")
                    drowSub = writerSub.NewRow()
                    drowSub("TimeSeriesID") = iDBID
                    drowSub("FleetID") = ecopathDS.FleetDBID(iPool)
                    writerSub.AddRow(drowSub)
                    Me.m_db.ReleaseWriter(writerSub)

                Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group
                    writerSub = Me.m_db.GetWriter("EcosimTimeSeriesGroup")
                    drowSub = writerSub.NewRow()
                    drowSub("TimeSeriesID") = iDBID
                    drowSub("GroupID") = ecopathDS.GroupDBID(iPool)
                    writerSub.AddRow(drowSub)
                    Me.m_db.ReleaseWriter(writerSub)

                Case Else
                    Debug.Assert(False)

            End Select

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while appending time series {1}", ex.Message, strName))
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes a time series from the datasource.
    ''' </summary>
    ''' <param name="iTimeSeriesID">Database ID of the time series to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function RemoveTimeSeries(ByVal iTimeSeriesID As Integer) As Boolean _
            Implements DataSources.IEcosimDatasource.RemoveTimeSeries

        Dim bSucces As Boolean = True
        Try
            Me.m_db.Execute(String.Format("DELETE FROM EcosimTimeSeries WHERE (TimeSeriesID = {0})", iTimeSeriesID))
        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Modify

#End Region ' Time series

#Region " MSE "

#Region " Load "

    Private Function LoadEcosimMSE(ByVal iScenarioID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim reader As IDataReader = Me.m_db.GetReader(String.Format("SELECT * FROM EcoSimScenarioMSE WHERE (ScenarioID={0})", iScenarioID))
        Dim bSucces As Boolean = True

        If reader IsNot Nothing Then

            reader.Read()
            Try

                mseDS.AssessMethod = DirectCast(Me.ReadSafe(reader, "AssessMethod", eAssessmentMethods.CatchEstmBio), eAssessmentMethods)
                mseDS.AssessPower = CSng(Me.ReadSafe(reader, "AssessPower", 1))
                mseDS.NTrials = CInt(Me.ReadSafe(reader, "NTrials", 10))
                mseDS.MSYStartTimeIndex = CInt(Me.ReadSafe(reader, "StartIndex", 2))

                For iGroup As Integer = 1 To ecopathDS.NumGroups
                    mseDS.GstockPred(iGroup) = CSng(Me.ReadSafe(reader, "ForcastGain", 0.6))
                    mseDS.KalmanGain(iGroup) = CSng(Me.ReadSafe(reader, "KalmanGain", 0.6))
                Next iGroup

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading EcopathPSD", ex.Message))
                bSucces = False
            End Try

            Me.m_db.ReleaseReader(reader)
            reader = Nothing

        End If

        Return bSucces
    End Function

#End Region ' Load

#Region " Save "

    Private Function SaveEcosimMSE(ByRef idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim strSQL As String = ""
        Dim iScenarioID As Integer = 0
        Dim bSucces As Boolean = True

        ' Obtain mapped scenario ID
        iScenarioID = idm.GetID(eDataTypes.EcoSimScenario, ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario))

        strSQL = String.Format("DELETE FROM EcosimScenarioMSE WHERE (ScenarioID={0})", iScenarioID)
        bSucces = Me.m_db.Execute(strSQL)

        Try
            writer = Me.m_db.GetWriter("EcosimScenarioMSE")
            drow = writer.NewRow()

            drow("ScenarioID") = iScenarioID
            drow("AssessMethod") = mseDS.AssessMethod
            drow("AssessPower") = mseDS.AssessPower
            drow("ForcastGain") = mseDS.GstockPred(1)
            drow("KalmanGain") = mseDS.KalmanGain(1)
            drow("Ntrials") = mseDS.NTrials
            drow("StartIndex") = mseDS.MSYStartTimeIndex

            writer.AddRow(drow)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving MSE", ex.Message))
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        Return bSucces

    End Function

#End Region ' Save

#End Region ' MSE

#End Region ' EcoSim

#Region " Ecospace "

#Region " Diagnostics "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States if the datasource has unsaved changes for Ecospace.
    ''' </summary>
    ''' <returns>True if the datasource has pending changes for Ecospace.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsEcospaceModified() As Boolean Implements DataSources.IEcospaceDatasource.IsEcospaceModified

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData

        ' Hmm, maybe the datasource should have a better way to 'remember' whether a space scenario has been loaded.
        If Not Me.IsConnected() Then Return False
        If ecopathDS.ActiveEcospaceScenario < 0 Then Return False

        Return Me.IsChanged(s_EcospaceComponents)

    End Function

#End Region ' Diagnostics

#Region " Scenarios "

#Region " Load "

    Public Function LoadEcospaceScenario(ByVal iScenarioID As Integer) As Boolean _
            Implements DataSources.IEcospaceDatasource.LoadEcospaceScenario

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        'jb Jan-17-07 moved SetDefaults to run before any data has been loaded
        'this will load the default values into Ecospace before anything else is loaded
        ecospaceDS.NGroups = ecopathDS.NumGroups
        ecospaceDS.nFleets = ecopathDS.NumFleet
        ecospaceDS.nLiving = ecopathDS.NumLiving

        ' Next is a dangerous solution that may need to be revamped. It is assumed that
        ' SetDefaults properly redimensions the ecospaceDS group variables, which
        ' may wreck havoc if the implementation of SetDefaults were to change.
        ecospaceDS.SetDefaults()

        reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenario WHERE (ScenarioID={0})", iScenarioID))
        Try
            ' Read the one record
            reader.Read()
            ' Remember link with Ecosim scenario, if any
            ecospaceDS.EcosimScenarioDBID = CInt(reader("EcosimScenarioID"))
            ecospaceDS.InRow = CInt(reader("Inrow"))
            ecospaceDS.InCol = CInt(reader("Incol"))
            ecospaceDS.CellLength = CSng(reader("CellLength"))
            ecospaceDS.IDH_UL = CSng(reader("IDH_UL"))
            ecospaceDS.IDH_SS = CSng(reader("IDH_SS"))
            ecospaceDS.TimeStep = CSng(reader("TimeStep"))
            ecospaceDS.PredictEffort = CBool(reader("PredictEffort"))

            ' JS 05apr08: pragmatic fix to prevent mayhem
            If ecospaceDS.TimeStep <= 0 Then ecospaceDS.TimeStep = CSng(1 / 12)

            ecospaceDS.TotalTime = CSng(reader("TotalTime"))
            ecospaceDS.IFDPower = CSng(reader("IFDPower"))
            ecospaceDS.nSpaceSolverThreads = CInt(reader("NumThreads"))
            ecospaceDS.nGridSolverThreads = CInt(reader("NumThreads"))
            stanzaDS.NPacketsMultiplier = CSng(reader("NumPacketsMultiplier"))
            ecospaceDS.AdjustSpace = CBool(reader("AdjustSpace"))
            ecospaceDS.UseExact = CBool(reader("UseExact"))
            ecospaceDS.Tol = CSng(Me.ReadSafe(reader, "Tolerance", 0.01!))

            Select Case CInt(reader("ModelType"))
                Case 0
                    ecospaceDS.NewMultiStanza = False
                    ecospaceDS.UseIBM = False
                Case 1
                    ecospaceDS.UseIBM = True
                    ecospaceDS.NewMultiStanza = False
                Case Else
                    ecospaceDS.UseIBM = False
                    ecospaceDS.NewMultiStanza = True
            End Select

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading Ecospace Scenario {1}", ex.Message, iScenarioID))
            bSucces = False
        End Try
        Me.m_db.ReleaseReader(reader)

        ' Read number of importance layers
        ecospaceDS.nImportanceLayers = CInt(Me.m_db.GetValue(String.Format("SELECT COUNT(*) FROM EcospaceScenarioWeightLayer WHERE ScenarioID={0}", iScenarioID)))

        'set the size of the variables that hold the map data to InRow and InCol
        ecospaceDS.ReDimMapDims()

        ' Set active scenario
        ecopathDS.ActiveEcospaceScenario = Array.IndexOf(ecopathDS.EcospaceScenarioDBID, iScenarioID)

        bSucces = bSucces And Me.LoadEcospaceHabitats(iScenarioID)
        bSucces = bSucces And Me.LoadEcospaceMPAs(iScenarioID)
        bSucces = bSucces And Me.LoadEcospaceRegions(iScenarioID)
        bSucces = bSucces And Me.LoadEcospaceGroups(iScenarioID)
        bSucces = bSucces And Me.LoadEcospaceFleets(iScenarioID)
        ' Load basemap last
        bSucces = bSucces And Me.LoadEcospaceBasemap(iScenarioID)
        bSucces = bSucces And Me.LoadEcospaceWeightLayers(iScenarioID)

        Me.ClearChanged(s_EcospaceComponents)

        Return bSucces
    End Function

#End Region ' Load

#Region " Save "

    Public Function SaveEcospaceScenarioAs(ByVal strScenarioName As String, ByVal strDescription As String, _
         ByVal strAuthor As String, ByVal strContact As String, ByRef iScenarioID As Integer) As Boolean _
                Implements IEcospaceDatasource.SaveEcospaceScenarioAs

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iActiveScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim idm As New cIDMappings()
        Dim iIDtmp As Integer = 0
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Me.m_db.BeginTransaction()

        ' Delete existing scenario
        Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenario WHERE ScenarioName='{0}'", strScenarioName))

        Try
            iScenarioID = CInt(Me.m_db.GetValue("SELECT MAX(ScenarioID) FROM EcospaceScenario")) + 1
        Catch ex As Exception
            iScenarioID = 1
        End Try

        idm.Add(eDataTypes.EcoSpaceScenario, iActiveScenarioID, iScenarioID)
        Try
            writer = Me.m_db.GetWriter("EcospaceScenario")
            drow = writer.NewRow()
            drow("ScenarioID") = iScenarioID
            drow("ScenarioName") = strScenarioName
            drow("Description") = strDescription
            drow("Author") = strAuthor
            drow("Contact") = strContact
            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer)
        Catch ex As Exception
            bSucces = False
        End Try

        ' ------
        ' Generate Ecopath objects for new Ecospace scenario
        ' ------

        ' First duplicate all Ecospace 'objects'
        For i As Integer = 1 To ecopathDS.NumGroups
            ' Add group to the new scenario
            bSucces = bSucces And Me.AddEcospaceGroup(ecopathDS.GroupDBID(i), iScenarioID, (ecopathDS.PP(i) = 2.0), iIDtmp)
            idm.Add(eDataTypes.EcospaceGroup, ecospaceDS.GroupDBID(i), iIDtmp)
        Next

        For i As Integer = 1 To ecopathDS.NumFleet
            ' Add fleet to the new scenario
            bSucces = bSucces And Me.AddEcospaceFleet(ecopathDS.FleetDBID(i), iScenarioID, iIDtmp)
            idm.Add(eDataTypes.EcospaceFleet, ecospaceDS.FleetDBID(i), iIDtmp)
        Next

        bSucces = bSucces And Me.SaveEcospaceScenario(idm)

        If bSucces Then
            Me.m_db.CommitTransaction(True)
        Else
            Me.m_db.RollbackTransaction()
        End If
        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Updates the active ecospace scenario under the given ID in the datasource.
    ''' This method is the one external interface to save an Ecospace scenario
    ''' and everything under it.
    ''' </summary>
    ''' <param name="iScenarioID">Database ID of the scenario to update. This
    ''' parameter is optional; if left to zero the active scenario will be saved.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function SaveEcospaceScenario(ByVal iScenarioID As Integer) As Boolean _
            Implements DataSources.IEcospaceDatasource.SaveEcospaceScenario

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iActiveScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim idm As cIDMappings = Nothing
        Dim bSucces As Boolean = True

        ' Abort if there is no active scenario
        If iActiveScenarioID = 0 Then Return False

        ' Prepare for saving
        idm = New cIDMappings()
        If iScenarioID = 0 Then iScenarioID = iActiveScenarioID

        ' Duplicating a scenario?
        If iScenarioID <> iActiveScenarioID Then
            ' #Yes: add ID mapping to allow copying of scenario content
            idm.Add(eDataTypes.EcoSpaceScenario, iActiveScenarioID, iScenarioID)
        End If

        ' Start transaction
        bSucces = Me.m_db.BeginTransaction()
        ' Save scenario
        bSucces = bSucces And Me.SaveEcospaceScenario(idm)

        ' Commit transaction
        If bSucces Then
            bSucces = Me.m_db.CommitTransaction(True)
        Else
            Me.m_db.RollbackTransaction()
        End If

        ' Reload ecospace scenario definitions to update lastsaved data
        Me.LoadEcospaceScenarioDefinitions()

        Return bSucces
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Internal method; updates the active ecospace scenario in the datasource,
    ''' optionally saving to a different scenario.
    ''' </summary>
    ''' <param name="idm"><see cref="cIDMappings">ID mapping</see> providing
    ''' ID mappings when saving to a different scenario ID.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function SaveEcospaceScenario(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenario As Integer = ecopathDS.ActiveEcospaceScenario
        Dim iScenarioID As Integer = 0
        Dim bSucces As Boolean = True

        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, ecopathDS.EcospaceScenarioDBID(iScenario))

        bSucces = Me.m_db.BeginTransaction()

        Try

            writer = Me.m_db.GetWriter("EcospaceScenario")
            dt = writer.GetDataTable()
            drow = dt.Rows.Find(iScenarioID)

            drow.BeginEdit()
            drow("Inrow") = ecospaceDS.InRow
            drow("Incol") = ecospaceDS.InCol
            drow("CellLength") = ecospaceDS.CellLength
            drow("IDH_UL") = ecospaceDS.IDH_UL
            drow("IDH_SS") = ecospaceDS.IDH_SS
            drow("TimeStep") = ecospaceDS.TimeStep
            drow("PredictEffort") = ecospaceDS.PredictEffort

            drow("TotalTime") = ecospaceDS.TotalTime
            drow("IFDPower") = ecospaceDS.IFDPower
            drow("NumThreads") = ecospaceDS.nSpaceSolverThreads
            drow("NumPacketsMultiplier") = stanzaDS.NPacketsMultiplier

            drow("ModelType") = 0
            If ecospaceDS.UseIBM Then drow("ModelType") = 1
            If ecospaceDS.NewMultiStanza Then drow("ModelType") = 2

            'JS 06Jul07: commented-out unused field 'BiomassInitType'
            'drow("BiomassInitType") = 0
            drow("AdjustSpace") = ecospaceDS.AdjustSpace
            drow("UseExact") = ecospaceDS.UseExact

            If Me.m_sVersion >= 6.01 Then
                drow("Tolerance") = ecospaceDS.Tol
            End If
            drow("LastSaved") = cDBDataSource.GetJulianDate()

            drow.EndEdit()

            ' Save changes
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving ecospace scenario {1}", ex.Message, iScenarioID))
            bSucces = False
        End Try

        bSucces = bSucces And Me.SaveEcospaceHabitats(idm)
        bSucces = bSucces And Me.SaveEcospaceMPAs(idm)
        bSucces = bSucces And Me.SaveEcospaceRegions(idm)
        bSucces = bSucces And Me.SaveEcospaceGroups(idm)
        bSucces = bSucces And Me.SaveEcospaceFleets(idm)
        bSucces = bSucces And Me.SaveEcospaceBasemap(idm)
        bSucces = bSucces And Me.SaveEcospaceWeightLayers(idm)

        If bSucces Then
            bSucces = Me.m_db.CommitTransaction(True)
        Else
            Me.m_db.RollbackTransaction()
        End If

        If bSucces Then
            ' Clear changed admin
            Me.ClearChanged(s_EcospaceComponents)
            ' Reload ecospace scenario definitions 
            Me.LoadEcospaceScenarioDefinitions()
        End If

        Return bSucces

    End Function

#End Region ' Save

#Region " Modify "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds an ecospace scenario to the datasource.
    ''' </summary>
    ''' <param name="strScenarioName">Name to assign to new scenario.</param>
    ''' <param name="strDescription">Description to assign to new scenario.</param>
    ''' <param name="strAuthor">Author to assign to the new scenario.</param>
    ''' <param name="strContact">Contact info to assign to the new scenario.</param>
    ''' <param name="iScenarioID">Database ID assigned to the new scenario.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function AppendEcospaceScenario(ByVal strScenarioName As String, ByVal strDescription As String, _
             ByVal strAuthor As String, ByVal strContact As String, _
             ByVal InRow As Integer, ByVal InCol As Integer, _
             ByVal sOriginLat As Single, ByVal sOriginLon As Single, ByVal sCellSize As Single, _
             ByRef iScenarioID As Integer) As Boolean _
             Implements DataSources.IEcospaceDatasource.AppendEcospaceScenario

        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iIDtmp As Integer = 0
        Dim bSucces As Boolean = True

        Try
            ' Delete any existing scenario
            bSucces = Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenario WHERE ScenarioName='{0}'", strScenarioName))

            Try
                iScenarioID = CInt(Me.m_db.GetValue("SELECT MAX(ScenarioID) FROM EcospaceScenario")) + 1
            Catch
                iScenarioID = 1
            End Try

            writer = Me.m_db.GetWriter("EcospaceScenario")

            drow = writer.NewRow()
            drow("ScenarioID") = iScenarioID
            drow("ScenarioName") = strScenarioName
            drow("Description") = strDescription
            drow("Author") = strAuthor
            drow("Contact") = strContact
            drow("LastSaved") = cDBDataSource.GetJulianDate()
            drow("InRow") = InRow
            drow("InCol") = InCol
            drow("ModelType") = 2
            writer.AddRow(drow)

            Me.m_db.ReleaseWriter(writer)

            ' ------
            ' Fill basemap with all water cells and RelPP values
            ' ------
            writer = Me.m_db.GetWriter("EcospaceScenarioBasemap")
            For i As Integer = 1 To InRow
                For j As Integer = 1 To InCol
                    drow = writer.NewRow()
                    drow("ScenarioID") = iScenarioID
                    drow("InRow") = i
                    drow("InCol") = j
                    drow("Depth") = 1
                    drow("RelPP") = 1 ' Fixes bug 410
                    writer.AddRow(drow)
                Next
            Next
            Me.m_db.ReleaseWriter(writer)

            ' First duplicate all Ecospace 'objects'
            For i As Integer = 1 To ecopathDS.NumGroups
                ' Add group to the new scenario
                bSucces = bSucces And Me.AddEcospaceGroup(ecopathDS.GroupDBID(i), iScenarioID, _
                                                          (ecopathDS.PP(i) = 2.0), iIDtmp)
            Next

            For i As Integer = 1 To ecopathDS.NumFleet
                ' Add fleet to the new scenario
                bSucces = bSucces And Me.AddEcospaceFleet(ecopathDS.FleetDBID(i), iScenarioID, iIDtmp)
            Next

            ' Add default 'All' habitat
            bSucces = bSucces And Me.AddEcospaceHabitat("All", iScenarioID, iIDtmp)

            ' Reload scenario definitions
            bSucces = bSucces And Me.LoadEcospaceScenarioDefinitions()

            Me.ClearChanged(s_EcospaceComponents)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while appending Scenario {1}", ex.Message, strScenarioName))
            bSucces = False
        End Try

        Return bSucces

    End Function

    Public Function RemoveEcospaceScenario(ByVal iScenarioID As Integer) As Boolean _
            Implements DataSources.IEcospaceDatasource.RemoveEcospaceScenario

        Dim bSucces As Boolean = True

        Try
            ' Delete 'soft links'
            '    Update 6.04005
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioWeightLayerCell WHERE (ScenarioID={0})", iScenarioID))
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioWeightLayer WHERE (ScenarioID={0})", iScenarioID))
            ' Delete scenario
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenario WHERE (ScenarioID={0})", iScenarioID))
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing Ecospace scenarioID {1}", ex.Message, iScenarioID))
            bSucces = False
        End Try

        ' Reload scenario definitions
        bSucces = bSucces And Me.LoadEcospaceScenarioDefinitions()

        Return bSucces

    End Function

#End Region ' Modify

#End Region ' Scenarios

#Region " Basemap "

#Region " Resizing "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Resizes the basemap in an Ecospace scenario.
    ''' </summary>
    ''' <param name="InRow">New number of rows to assign to the basemap.</param>
    ''' <param name="InCol">New number of columns to assign to the basemap.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function ResizeEcospaceBasemap(ByVal InRow As Integer, ByVal InCol As Integer) As Boolean _
             Implements IEcospaceDatasource.ResizeEcospaceBasemap

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True
        Dim strSQL As String = String.Format("DELETE FROM EcospaceScenarioBasemap WHERE (ScenarioID={0}) AND ((InRow > {1}) OR (InCol > {2}))", iScenarioID, InRow, InCol)

        ' Get ID of scenario to save to
        iScenarioID = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)

        Try
            writer = Me.m_db.GetWriter("EcospaceScenario")
            dt = writer.GetDataTable()
            drow = dt.Rows.Find(iScenarioID)

            drow.BeginEdit()
            drow("Inrow") = InRow
            drow("Incol") = InCol
            drow.EndEdit()

            Me.m_db.ReleaseWriter(writer, True)

            ' Delete unused cells
            bSucces = Me.m_db.Execute(strSQL)

            ' Assign newly created cells as water cells
            writer = Me.m_db.GetWriter("EcospaceScenarioBasemap")
            For i As Integer = 1 To InRow
                For j As Integer = 1 To InCol
                    If ((i > ecospaceDS.InRow) Or (j > ecospaceDS.InCol)) Then
                        drow = writer.NewRow()
                        drow("ScenarioID") = iScenarioID
                        drow("InRow") = i
                        drow("InCol") = j
                        drow("Depth") = 1
                        writer.AddRow(drow)
                    End If
                Next
            Next
            Me.m_db.ReleaseWriter(writer, True)

            ' Reallocate fish and port cells

        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Resizing

#Region " Load "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the spatial data associated with an Ecospace scenario.
    ''' </summary>
    ''' <param name="iScenarioID">The scenario to load the data for.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadEcospaceBasemap(ByVal iScenarioID As Integer) As Boolean
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True
        Dim iRow As Integer = 0
        Dim iCol As Integer = 0
        Dim iID As Integer = 0
        Dim sScalePP As Single = 0

        ' Change all cells to land by default since the database will read only water cells
        For i As Integer = 1 To ecospaceDS.InRow
            For j As Integer = 1 To ecospaceDS.InCol
                ecospaceDS.Depth(i, j) = 0
            Next
        Next

        Try
            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioBasemap WHERE (ScenarioID={0})", iScenarioID))
            While reader.Read()

                iRow = CInt(reader("InRow"))
                iCol = CInt(reader("InCol"))

                ' Valid cell?
                If ((iRow <= ecospaceDS.InRow) And (iCol <= ecospaceDS.InCol)) Then

                    ' Read scalars
                    ecospaceDS.Depth(iRow, iCol) = CInt(reader("Depth"))
                    ecospaceDS.RelPP(iRow, iCol) = CSng(reader("RelPP"))
                    ecospaceDS.RelCin(iRow, iCol) = CSng(reader("RelCin"))
                    ' Read FKs
                    iID = CInt(reader("HabitatID"))
                    ecospaceDS.HabType(iRow, iCol) = CInt(IIf((iID > 0), Math.Max(0, Array.IndexOf(ecospaceDS.HabitatDBID, iID)), 0))
                    iID = CInt(reader("RegionID"))
                    ecospaceDS.Region(iRow, iCol) = CInt(IIf((iID > 0), Math.Max(0, Array.IndexOf(ecospaceDS.RegionDBID, iID)), 0))
                    iID = CInt(reader("MPAID"))
                    ecospaceDS.MPA(iRow, iCol) = CInt(IIf((iID > 0), Math.Max(0, Array.IndexOf(ecospaceDS.MPADBID, iID)), 0))
                    ' Update trackers
                    sScalePP = CSng(Math.Max(ecospaceDS.RelPP(iRow, iCol), sScalePP))

                End If

            End While

            Me.m_db.ReleaseReader(reader)
            reader = Nothing

        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces
    End Function

#End Region ' Load

#Region " Save "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Save the spatial data associated with an Ecospace scenario.
    ''' </summary>
    ''' <param name="idm">The scenario to save the data for.</param>
    ''' <returns>True if succesful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function SaveEcospaceBasemap(ByVal idm As cIDMappings) As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim iScenarioID As Integer = 0
        Dim iRow As Integer = 0
        Dim iCol As Integer = 0
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        ' Get ID of scenario to save to
        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario))

        Try
            ' Destroy current
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioBasemap WHERE (ScenarioID={0})", iScenarioID))

            ' Rebuild
            writer = Me.m_db.GetWriter("EcospaceScenarioBasemap")
            ' Every cell will need a row in the database, because every cell is assigned to a habitat.
            ' JS070226: should profile to see whether it's faster to update existing rows rather than
            '           destroying and rebuilding the entire table content.
            For iRow = 1 To ecospaceDS.InRow
                For iCol = 1 To ecospaceDS.InCol
                    ' Create new row
                    drow = writer.NewRow()
                    ' Store simple values
                    drow("ScenarioID") = iScenarioID
                    drow("InRow") = iRow
                    drow("InCol") = iCol
                    drow("Depth") = ecospaceDS.Depth(iRow, iCol)
                    drow("RelPP") = ecospaceDS.RelPP(iRow, iCol)
                    drow("RelCin") = ecospaceDS.RelCin(iRow, iCol)
                    ' Store (mapped) habitat ID
                    drow("HabitatID") = idm.GetID(eDataTypes.EcospaceHabitat, ecospaceDS.HabitatDBID(ecospaceDS.HabType(iRow, iCol)))
                    ' Store (mapped) region ID
                    drow("RegionID") = idm.GetID(eDataTypes.EcospaceRegion, ecospaceDS.RegionDBID(ecospaceDS.Region(iRow, iCol)))
                    ' Store (mapped) MPA ID
                    drow("MPAID") = idm.GetID(eDataTypes.EcospaceMPA, ecospaceDS.MPADBID(ecospaceDS.MPA(iRow, iCol)))
                    ' Add the row
                    writer.AddRow(drow)
                Next iCol
            Next iRow

            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            ' Don't be alarmed..
            Debug.Assert(False, String.Format("Error saving basemap: '{0}'", ex.Message))
            '..be very, very afraid
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Save

#End Region ' Basemap

#Region " Habitats "

#Region " Load "

    Private Function LoadEcospaceHabitats(ByVal iScenarioID As Integer) As Boolean

        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True
        Dim i As Integer = 0

        ' Start loading
        ' Note that 'All' habitat should arrive at array pos 0, therefore i starts at 0
        Try
            ' Allocate space for habitat data
            ecospaceDS.NoHabitats = CInt(Me.m_db.GetValue(String.Format("SELECT COUNT(*) FROM EcospaceScenarioHabitat WHERE ScenarioID={0}", iScenarioID)))
            ecospaceDS.NoHabChanges = CInt(Me.m_db.GetValue(String.Format("SELECT COUNT(*) FROM EcospaceScenarioHabitatChange WHERE ScenarioID={0}", iScenarioID)))
            ecospaceDS.RedimHabitatVariables(False)

            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioHabitat WHERE (ScenarioID={0}) ORDER BY Sequence ASC", iScenarioID))
            While reader.Read()
                ecospaceDS.HabitatDBID(i) = CInt(reader("HabitatID"))
                ecospaceDS.HabitatText(i) = CStr(reader("HabitatName"))
                i += 1
            End While
            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading Ecospace habitat for habitat {1}", ex.Message, i))
            bSucces = False
        End Try


        ' Load related data
        bSucces = bSucces And Me.LoadEcospaceHabitatChanges(iScenarioID)

        Return bSucces

    End Function

    Private Function LoadEcospaceHabitatChanges(ByVal iScenarioID As Integer) As Boolean

        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True
        Dim iTime As Integer = 0
        Dim iSequence As Integer = 0

        Try
            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioHabitatChange WHERE (ScenarioID={0}) ORDER BY Time", iScenarioID))
            While reader.Read()

                ' Read fields
                iTime = CInt(reader("Time"))
                iSequence = CInt(reader("Sequence"))

                ecospaceDS.HabTime(iSequence) = iTime
                ecospaceDS.HabChange(0, iSequence) = CInt(reader("InCol"))
                ecospaceDS.HabChange(1, iSequence) = CInt(reader("InRow"))
                ecospaceDS.HabChange(2, iSequence) = CInt(reader("DrawMod"))
                ecospaceDS.HabChange(3, iSequence) = CInt(reader("Change"))

            End While
            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading Ecospace habitat for time {1}, Sequence {2}", ex.Message, iTime, iSequence))
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    Private Function SaveEcospaceHabitats(ByRef idm As cIDMappings) As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioIDSrc As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim iScenarioIDDest As Integer = 0
        Dim iID As Integer = 0
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True
        Dim bNewRow As Boolean = True
        Dim objKeys() As Object = {Nothing, Nothing}

        iScenarioIDDest = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioIDSrc)
        objKeys(0) = iScenarioIDSrc

        Try
            iID = CInt(Me.m_db.GetValue("SELECT MAX(HabitatID) FROM EcospaceScenarioHabitat")) + 1
        Catch ex As Exception
            iID = 1
        End Try

        Try
            writer = Me.m_db.GetWriter("EcospaceScenarioHabitat", "Sequence", String.Format("ScenarioID={0}", iScenarioIDDest))
            dt = writer.GetDataTable()
            For iHabitat As Integer = 0 To ecospaceDS.NoHabitats - 1

                bNewRow = (iScenarioIDDest <> iScenarioIDSrc)

                If bNewRow Then
                    drow = writer.NewRow()
                    drow("ScenarioID") = iScenarioIDDest
                    drow("HabitatID") = iID
                    idm.Add(eDataTypes.EcospaceHabitat, ecospaceDS.HabitatDBID(iHabitat), iID)
                    iID += 1
                Else
                    objKeys(1) = idm.GetID(eDataTypes.EcospaceHabitat, ecospaceDS.HabitatDBID(iHabitat))
                    drow = dt.Rows.Find(objKeys)
                    drow.BeginEdit()
                End If

                drow("HabitatName") = ecospaceDS.HabitatText(iHabitat)
                drow("Sequence") = iHabitat

                If bNewRow Then
                    writer.AddRow(drow)
                Else
                    drow.EndEdit()
                End If

            Next iHabitat
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces And SaveEcospaceHabitatChanges(idm)

    End Function

    Private Function SaveEcospaceHabitatChanges(ByRef idm As cIDMappings) As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioID)

        Try
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioHabitatChange WHERE ScenarioID={0}", iScenarioID))

            writer = Me.m_db.GetWriter("EcospaceScenarioHabitatChange")
            For iChange As Integer = 1 To ecospaceDS.NoHabChanges

                drow = writer.NewRow()
                drow("ScenarioID") = iScenarioID
                drow("Time") = ecospaceDS.HabTime(iChange)
                drow("Sequence") = iChange
                drow("InRow") = ecospaceDS.HabChange(0, iChange)
                drow("InCol") = ecospaceDS.HabChange(1, iChange)
                drow("DrawMod") = ecospaceDS.HabChange(2, iChange)
                drow("Change") = ecospaceDS.HabChange(3, iChange)
                writer.AddRow(drow)

            Next iChange

            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

#End Region ' Save

#Region " Modify "

    ''' <summary>
    ''' Append an habitat to the current ecospace scenario
    ''' </summary>
    ''' <param name="strHabitatName"></param>
    ''' <param name="iDBID"></param>
    ''' <returns></returns>
    Public Function AddEcospaceHabitat(ByVal strHabitatName As String, ByRef iDBID As Integer) As Boolean _
            Implements DataSources.IEcospaceDatasource.AddEcospaceHabitat

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)

        Return Me.AddEcospaceHabitat(strHabitatName, iScenarioID, iDBID)

    End Function

    ''' <summary>
    ''' Append an habitat to a given ecospace scenario
    ''' </summary>
    ''' <param name="strHabitatName"></param>
    ''' <param name="iDBID"></param>
    ''' <returns></returns>
    Private Function AddEcospaceHabitat(ByVal strHabitatName As String, ByVal iScenarioID As Integer, ByRef iDBID As Integer) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True
        Dim iPosition As Integer = 1

        Try
            iDBID = CInt(Me.m_db.GetValue("SELECT MAX(HabitatID) FROM EcospaceScenarioHabitat")) + 1
            iPosition = CInt(Me.m_db.GetValue("SELECT Count(*) FROM EcospaceScenarioHabitat")) + 1
        Catch ex As Exception
            iDBID = 1
            iPosition = 1
        End Try

        ' The writer needed here will maintain row sequence for the given scenario only
        writer = Me.m_db.GetWriter("EcospaceScenarioHabitat", "Sequence", String.Format("ScenarioID={0}", iScenarioID))

        drow = writer.NewRow()
        drow("ScenarioID") = iScenarioID
        drow("HabitatID") = iDBID
        drow("HabitatName") = strHabitatName
        drow("Sequence") = iPosition
        writer.AddRow(drow)

        Me.m_db.ReleaseWriter(writer)

        Return bSucces
    End Function

    ''' <summary>
    ''' Remove an ecospace habitat from the current scenario
    ''' </summary>
    ''' <param name="iHabitatID"></param>
    ''' <returns></returns>
    Public Function RemoveHabitat(ByVal iHabitatID As Integer) As Boolean _
            Implements DataSources.IEcospaceDatasource.RemoveEcospaceHabitat

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim bSucces As Boolean = True

        Try
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioHabitat WHERE (ScenarioID={0}) AND (HabitatID={1})", iScenarioID, iHabitatID))
            ' This could have far-fetched consequences throughout the scenario; the entire scenario should be reloaded.
            bSucces = Me.LoadEcospaceScenario(iScenarioID)
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing Ecospace habitatID {1}", ex.Message, iHabitatID))
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Modify

#End Region ' Habitats

#Region " Regions "

#Region " Load "

    Private Function LoadEcospaceRegions(ByVal iScenarioID As Integer) As Boolean
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True
        Dim i As Integer = 0

        ' Read fields
        Try
            ' Allocate space for region data
            ecospaceDS.NoRegions = CInt(Me.m_db.GetValue(String.Format("SELECT COUNT(*) FROM EcospaceScenarioRegion WHERE ScenarioID={0}", iScenarioID)))
            ecospaceDS.ReDimRegionVars()

            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioRegion WHERE (ScenarioID={0}) ORDER BY Sequence ASC", iScenarioID))
            While reader.Read()
                i += 1
                ecospaceDS.RegionDBID(i) = CInt(reader("RegionID"))
                ecospaceDS.RegionName(i) = CStr(reader("RegionName"))
            End While
            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading Ecospace region {1}", ex.Message, i))
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    Private Function SaveEcospaceRegions(ByVal idm As cIDMappings) As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioIDSrc As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim iScenarioIDDest As Integer = 0
        Dim iID As Integer = 0
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True
        Dim bNewRow As Boolean = True
        Dim objKeys() As Object = {Nothing, Nothing}

        iScenarioIDDest = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioIDSrc)
        objKeys(0) = iScenarioIDDest

        Try
            iID = CInt(Me.m_db.GetValue("SELECT MAX(RegionID) FROM EcospaceScenarioRegion")) + 1
        Catch ex As Exception
            iID = 1
        End Try

        Try
            writer = Me.m_db.GetWriter("EcospaceScenarioRegion", "Sequence", String.Format("ScenarioID={0}", iScenarioIDDest))
            dt = writer.GetDataTable()
            For iRegion As Integer = 1 To ecospaceDS.NoRegions

                ' Find existing row
                objKeys(1) = idm.GetID(eDataTypes.EcospaceRegion, ecospaceDS.RegionDBID(iRegion))
                drow = dt.Rows.Find(objKeys)

                bNewRow = (iScenarioIDDest <> iScenarioIDSrc) Or (drow Is Nothing)

                If bNewRow Then
                    drow = writer.NewRow()
                    drow("ScenarioID") = iScenarioIDDest
                    drow("RegionID") = iID
                    idm.Add(eDataTypes.EcospaceRegion, ecospaceDS.RegionDBID(iRegion), iID)
                    iID += 1
                Else
                    drow.BeginEdit()
                End If

                drow("RegionName") = ecospaceDS.RegionName(iRegion)
                drow("Sequence") = iRegion

                If bNewRow Then
                    writer.AddRow(drow)
                Else
                    drow.EndEdit()
                End If


            Next iRegion
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

#End Region ' Save

#Region " Modify "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds an ecospace region to active scenario in the datasource.
    ''' </summary>
    ''' <param name="strRegionName">Name to assign to new region.</param>
    ''' <param name="iDBID">Database ID assigned to the new region.</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>This call will reload ecospace regions.</remarks>
    ''' -------------------------------------------------------------------
    Public Function AppendEcospaceRegion(ByVal strRegionName As String, ByRef iDBID As Integer) As Boolean _
            Implements DataSources.IEcospaceDatasource.AppendEcospaceRegion

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)

        Return Me.AddEcospaceRegion(strRegionName, iScenarioID, iDBID)

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Internal call to actually add an ecospace region to a select
    ''' scenario in the datasource.
    ''' </summary>
    ''' <param name="strRegionName">Name to assign to new region.</param>
    ''' <param name="iDBID">Database ID assigned to the new region.</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' <para>This method serves two purposes:</para>
    ''' <list type="bullet">
    ''' <item><description>To add a region to the current scenario;</description></item>
    ''' <item><description>To duplicate a region into a new scenario.</description></item>
    ''' </list>
    ''' <para>Note that this call will not reload any data.</para>
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Private Function AddEcospaceRegion(ByVal strRegionName As String, ByVal iScenarioID As Integer, ByRef iDBID As Integer) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        ' Sanity checks
        If iScenarioID <= 0 Then
            Debug.Assert(False, String.Format("Invalid scenario ID {0} specified in AddEcospaceRegion", iScenarioID))
            Return False
        End If

        Try
            ' RegionID unique across scenarios
            iDBID = CInt(Me.m_db.GetValue("SELECT MAX(RegionID) FROM EcospaceScenarioRegion")) + 1
        Catch ex As Exception
            iDBID = 1
        End Try

        Try

            writer = Me.m_db.GetWriter("EcospaceScenarioRegion", "Sequence")

            drow = writer.NewRow()
            drow("ScenarioID") = iScenarioID
            drow("RegionID") = iDBID
            drow("RegionName") = strRegionName
            drow("Sequence") = iDBID
            writer.AddRow(drow)

            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            Console.WriteLine("Error {0} occurred while adding ecospace region {1} ({2}) to scenario {3}", ex.Message, strRegionName, iDBID, iScenarioID)
            bSucces = False
        End Try

        Return bSucces
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes an ecospace region from the active scenario in the datasource.
    ''' </summary>
    ''' <param name="iRegionID">Database ID of the region to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>Note that there is no need for an internal DeleteEcospaceRegion
    ''' method; </remarks>
    ''' -------------------------------------------------------------------
    Public Function RemoveEcospaceRegion(ByVal iRegionID As Integer) As Boolean _
             Implements DataSources.IEcospaceDatasource.RemoveEcospaceRegion

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim bSucces As Boolean = True

        Try
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioRegion WHERE (ScenarioID={0}) AND (RegionID={1})", iScenarioID, iRegionID))
            ' This could have far-fetched consequences throughout the scenario; the entire scenario should be reloaded.
            bSucces = Me.LoadEcospaceScenario(iScenarioID)
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing Ecospace regionID {1}", ex.Message, iRegionID))
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Modify

#End Region ' Regions

#Region " Groups "

#Region " Load "

    Private Function LoadEcospaceGroups(ByVal iScenarioID As Integer) As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True
        Dim astrSplit As String() = Nothing
        Dim iGroup As Integer = 0

        ' Group redimensioning is handled from the LoadScenario call
        'ecospaceDS.RedimGroupVariables(False)

        ' Read the data
        Try
            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioGroup WHERE (ScenarioID={0})", iScenarioID))
            While reader.Read()

                ' Resolve group index
                iGroup = Array.IndexOf(ecopathDS.GroupDBID, CInt(reader("EcopathGroupID")))
                ' Sanity check
                Debug.Assert(iGroup > -1)
                ' Load the data
                ecospaceDS.GroupDBID(iGroup) = CInt(reader("GroupID"))
                ecospaceDS.EcopathGroupDBID(iGroup) = CInt(reader("EcopathGroupID"))
                ecospaceDS.Mvel(iGroup) = CSng(reader("Mvel"))
                ecospaceDS.RelMoveBad(iGroup) = CSng(reader("RelMoveBad"))
                ecospaceDS.RelVulBad(iGroup) = CSng(reader("RelVulBad"))
                ecospaceDS.EatEffBad(iGroup) = CSng(reader("EatEffBad"))
                ' VERIFY_JS: RiskSens imported but not used in EwE5
                ' ecospaceDS.RiskSens(i) = CSng(reader("RiskSens"))
                ecospaceDS.IsAdvected(iGroup) = CBool(reader("IsAdvected"))
                ecospaceDS.IsMigratory(iGroup) = CBool(reader("IsMigratory"))
                ecospaceDS.MigConcRow(iGroup) = CSng(reader("MigConcRow"))
                ecospaceDS.MigConcCol(iGroup) = CSng(reader("MigConcCol"))
                ecospaceDS.barrierAvoidanceWeight(iGroup) = CSng(ReadSafe(reader, "BarrierAvoidanceWeight", ecospaceDS.barrierAvoidanceWeight(iGroup)))
                ' Monthly PrefRow
                astrSplit = CStr(reader("PrefRow")).Split(CChar(" "))
                For iMonth As Integer = 1 To Math.Min(cCore.N_MONTHS, astrSplit.Length)
                    ecospaceDS.PrefRow(iGroup, iMonth) = StringUtils.ConvertToInteger(astrSplit(iMonth - 1))
                Next
                ' Monthly PrefCol
                astrSplit = CStr(reader("PrefCol")).Split(CChar(" "))
                For iMonth As Integer = 1 To Math.Min(cCore.N_MONTHS, astrSplit.Length)
                    ecospaceDS.Prefcol(iGroup, iMonth) = StringUtils.ConvertToInteger(astrSplit(iMonth - 1))
                Next

            End While
            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading Ecospace group {1}", ex.Message, iGroup))
            bSucces = False
        End Try


        ' Load habitat preferences
        bSucces = bSucces And Me.LoadEcospaceGroupHabitats(iScenarioID)
        Return bSucces

    End Function

    Private Function LoadEcospaceGroupHabitats(ByVal iScenarioID As Integer) As Boolean
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim iGroupID As Integer = 0
        Dim iGroup As Integer = -1
        Dim iHabitatID As Integer = 0
        Dim iHabitat As Integer = -1
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioGroupHabitat WHERE (ScenarioID={0})", iScenarioID))
            While reader.Read()

                ' Get group index
                iGroupID = CInt(reader("GroupID"))
                iGroup = Array.IndexOf(ecospaceDS.GroupDBID, iGroupID)
                ' Get habitat index
                iHabitatID = CInt(reader("HabitatID"))
                iHabitat = Array.IndexOf(ecospaceDS.HabitatDBID, iHabitatID)
                ' Sanity check
                If (iGroup = -1) Or (iHabitat = -1) Then
                    If (iGroup = -1) Then Me.LogMessage(String.Format("LoadEcospaceGroupHabitats: Group ID {0} no longer exist", iGroupID))
                    If (iHabitat = -1) Then Me.LogMessage(String.Format("LoadEcospaceGroupHabitats: Habitat ID {1} no longer exist", iHabitatID))
                Else
                    ' Flag as preferred
                    ecospaceDS.PrefHab(iGroup, 0) = False
                    ecospaceDS.PrefHab(iGroup, iHabitat) = True
                End If

            End While
            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading Ecospace group preferred habitats", ex.Message))
            bSucces = False
        End Try

        Return bSucces
    End Function

#End Region ' Load

#Region " Save "

    Private Function SaveEcospaceGroups(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim sbTemp As New StringBuilder
        Dim drow As DataRow = Nothing
        Dim iGroup As Integer = 0
        Dim iGroupID As Integer = 0
        Dim objKeys() As Object = {Nothing, Nothing} ' Composite key to find group per scenario
        Dim bSucces As Boolean = True

        ' Get mapped scenario ID, in case saving to a different scenario
        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioID)
        objKeys(0) = iScenarioID

        Try
            writer = Me.m_db.GetWriter("EcospaceScenarioGroup")
            dt = writer.GetDataTable()

            For iGroup = 1 To ecopathDS.NumGroups

                ' Find group ID, it may be mapped to a different ID when saving to a new scenario
                iGroupID = idm.GetID(eDataTypes.EcospaceGroup, ecospaceDS.GroupDBID(iGroup))
                objKeys(1) = iGroupID

                ' Find existing row
                drow = dt.Rows.Find(objKeys)
                Debug.Assert(drow IsNot Nothing, String.Format("Cannot find existing row for group {0} ({1})", iGroupID, ecospaceDS.GroupDBID(iGroup)))

                ' JS: NEVER MODIFY THE GROUPID ONCE CREATED, this should ONLY be done at creation time!
                ' drow("GroupID") = ecopathDS.GroupDBID(iGroup)

                drow("Mvel") = ecospaceDS.Mvel(iGroup)
                drow("RelMoveBad") = ecospaceDS.RelMoveBad(iGroup)
                drow("RelVulBad") = ecospaceDS.RelVulBad(iGroup)
                drow("EatEffBad") = ecospaceDS.EatEffBad(iGroup)
                drow("IsAdvected") = ecospaceDS.IsAdvected(iGroup)
                drow("IsMigratory") = ecospaceDS.IsMigratory(iGroup)
                drow("MigConcRow") = ecospaceDS.MigConcRow(iGroup)
                drow("MigConcCol") = ecospaceDS.MigConcCol(iGroup)
                drow("BarrierAvoidanceWeight") = ecospaceDS.barrierAvoidanceWeight(iGroup)

                sbTemp.Length = 0
                For iMonth As Integer = 1 To cCore.N_MONTHS
                    If iMonth > 1 Then sbTemp.Append(CChar(" "))
                    sbTemp.Append(StringUtils.FormatSingle(ecospaceDS.PrefRow(iGroup, iMonth)))
                Next
                drow("PrefRow") = sbTemp.ToString()

                sbTemp.Length = 0
                For iMonth As Integer = 1 To cCore.N_MONTHS
                    If iMonth > 1 Then sbTemp.Append(CChar(" "))
                    sbTemp.Append(StringUtils.FormatSingle(ecospaceDS.Prefcol(iGroup, iMonth)))
                Next
                drow("PrefCol") = sbTemp.ToString()

            Next iGroup

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving EcospaceGroup", ex.Message))
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        Return bSucces And Me.SaveEcospaceGroupHabitats(idm)

    End Function

    Private Function SaveEcospaceGroupHabitats(ByVal idm As cIDMappings) As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenarioID As Integer = idm.GetID(eDataTypes.EcoSpaceScenario, ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario))
        Dim iGroupID As Integer = 0
        Dim iGroup As Integer = 0
        Dim iHabitatID As Integer = 0
        Dim iHabitat As Integer = 0

        Dim bSucces As Boolean = True

        Try
            ' No incremental save for now
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioGroupHabitat WHERE ScenarioID={0}", iScenarioID))

            writer = Me.m_db.GetWriter("EcospaceScenarioGroupHabitat")
            For iGroup = 1 To ecopathDS.NumGroups
                iGroupID = idm.GetID(eDataTypes.EcospaceGroup, ecospaceDS.GroupDBID(iGroup))

                For iHabitat = 0 To ecospaceDS.NoHabitats
                    iHabitatID = idm.GetID(eDataTypes.EcospaceHabitat, ecospaceDS.HabitatDBID(iHabitat))

                    If (ecospaceDS.PrefHab(iGroup, iHabitat) = True) Then

                        drow = writer.NewRow()
                        drow("ScenarioID") = iScenarioID
                        drow("GroupID") = iGroupID
                        drow("HabitatID") = iHabitatID
                        writer.AddRow(drow)

                    End If

                Next iHabitat
            Next iGroup

            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Save

#Region " Modify "

    ''' <summary>
    ''' Create a group for each ecospace scenario
    ''' </summary>
    ''' <param name="iEcopathGroupID">Ecopath Group DBID</param>
    Private Function AddEcospaceGroupToAllScenarios(ByVal iEcopathGroupID As Integer, ByVal bIsDetritus As Boolean) As Boolean

        Dim reader As IDataReader = Nothing
        Dim iID As Integer = 0
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader(String.Format("SELECT ScenarioID FROM EcoSpaceScenario"))
            While reader.Read()
                bSucces = bSucces And AddEcospaceGroup(iEcopathGroupID, CInt(reader("ScenarioID")), bIsDetritus, iID)
            End While
            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' <summary>
    ''' Add a group to a given Ecospace scenario.
    ''' </summary>
    ''' <param name="iEcopathGroupID"><see cref="cEcoPathGroupInput.DBID">Ecopath ID</see> of this group</param>
    ''' <param name="iScenarioID"><see cref="cEcospaceScenario.DBID">Ecospace scenario ID</see> of the scenario to add the group to.</param>
    ''' <returns>True if succesful.</returns>
    Private Function AddEcospaceGroup(ByVal iEcopathGroupID As Integer, ByVal iScenarioID As Integer, _
                                      ByVal bIsDetritus As Boolean, ByRef iGroupID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim iGroup As Integer = 0
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            iGroupID = CInt(Me.m_db.GetValue("SELECT MAX(GroupID) FROM EcospaceScenarioGroup")) + 1
        Catch ex As Exception
            iGroupID = 1
        End Try

        Try
            ' Is this a detritus group?
            iGroup = Array.IndexOf(ecopathDS.GroupDBID, iEcopathGroupID)

            ' Add group
            writer = Me.m_db.GetWriter("EcospaceScenarioGroup")

            drow = writer.NewRow()
            drow("ScenarioID") = iScenarioID
            drow("EcopathGroupID") = iEcopathGroupID
            drow("GroupID") = iGroupID
            ' Detritus default of 10, non-detritus 300
            drow("MVel") = CSng(IIf(bIsDetritus, 10, 300))
            writer.AddRow(drow)

            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

#End Region ' Modify

#End Region ' Groups

#Region " Fleets "

#Region " Load "

    Private Function LoadEcospaceFleets(ByVal iScenarioID As Integer) As Boolean
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True
        Dim iFleet As Integer = 0

        ecospaceDS.ReDimFleets()
        reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioFleet WHERE (ScenarioID={0})", iScenarioID))

        Try
            While reader.Read()
                iFleet += 1
                ecospaceDS.FleetDBID(iFleet) = CInt(reader("FleetID"))
                ecospaceDS.EcopathFleetDBID(iFleet) = CInt(reader("EcopathFleetID"))
                ecospaceDS.EffPower(iFleet) = CSng(reader("EffPower"))
            End While

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading Ecospace fleet {1}", ex.Message, iFleet))
            bSucces = False
        End Try

        Me.m_db.ReleaseReader(reader)

        ' Read port data
        bSucces = bSucces And Me.LoadEcospaceFleetMap(iScenarioID)
        ' Read habitat fishery
        bSucces = bSucces And Me.LoadEcospaceHabitatFishery(iScenarioID)
        ' Read MPA fishery
        bSucces = bSucces And Me.LoadEcospaceMPAFishery(iScenarioID)
        ' There
        Return bSucces

    End Function

    Private Function LoadEcospaceFleetMap(ByVal iScenarioID As Integer) As Boolean
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim iFleet As Integer = 0
        Dim iRow As Integer = 0
        Dim iCol As Integer = 0
        Dim iPort As Integer = 0
        Dim iCell As Integer = 0
        Dim bSucces As Boolean = True

        ' Clear
        For iFleet = 1 To Me.m_core.nFleets
            For iRow = 0 To ecospaceDS.InRow
                For iCol = 0 To ecospaceDS.InCol
                    ecospaceDS.Port(iFleet, iRow, iCol) = False
                Next iCol
            Next iRow
        Next iFleet

        reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioFleetMap WHERE (ScenarioID={0})", iScenarioID))
        Try
            While reader.Read()

                iFleet = Array.IndexOf(ecospaceDS.FleetDBID, CInt(reader("FleetID")))
                iRow = CInt(reader("InRow"))
                iCol = CInt(reader("InCol"))
                iPort = CInt(reader("PortID"))

                ' Set Port 
                ecospaceDS.Port(iFleet, iRow, iCol) = (iPort > 0)

            End While

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EcospaceScenarioFleetMap for iFleet {1}, scenario ID {2}", ex.Message, iFleet, iScenarioID))
            bSucces = False
        End Try
        Me.m_db.ReleaseReader(reader)

        Return bSucces
    End Function

    Private Function LoadEcospaceHabitatFishery(ByVal iScenarioID As Integer) As Boolean
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim iFleet As Integer = 0
        Dim iHabitat As Integer = 0
        Dim bSucces As Boolean = True

        reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioHabitatFishery WHERE (ScenarioID={0})", iScenarioID))
        Try
            While reader.Read()
                iFleet = Array.IndexOf(ecospaceDS.FleetDBID, CInt(reader("FleetID")))
                iHabitat = Array.IndexOf(ecospaceDS.HabitatDBID, CInt(reader("HabitatID")))
                'jb habitats and fleets both use the zero index
                If (iFleet >= 0 And iHabitat >= 0) Then
                    ' Clear default 'all' habitat assignment
                    ecospaceDS.GearHab(iFleet, 0) = False
                    ecospaceDS.GearHab(iFleet, iHabitat) = True
                End If
            End While
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EcospaceScenarioHabitatFishery for iFleet {1}, iHabitat {2}", ex.Message, iFleet, iHabitat))
            bSucces = False
        End Try
        Me.m_db.ReleaseReader(reader)

        Return bSucces
    End Function

    Private Function LoadEcospaceMPAFishery(ByVal iScenarioID As Integer) As Boolean
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim iFleet As Integer = 0
        Dim iMPA As Integer = 0
        Dim bSucces As Boolean = True

        reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioMPAFishery WHERE (ScenarioID={0})", iScenarioID))
        Try
            While reader.Read()
                iFleet = Array.IndexOf(ecospaceDS.FleetDBID, CInt(reader("FleetID")))
                iMPA = Array.IndexOf(ecospaceDS.MPADBID, CInt(reader("MPAID")))
                ' Crash prevention, should not be necessary but hey
                If (iFleet >= 0 And iMPA > 0) Then
                    ' Clear default 'all' habitat assignment
                    ecospaceDS.MPAfishery(iFleet, 0) = False
                    ecospaceDS.MPAfishery(iFleet, iMPA) = True
                End If
            End While
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading ReadEcospaceMPAFishery for iFleet {1}, iMPA {2}", ex.Message, iFleet, iMPA))
            bSucces = False
        End Try
        Me.m_db.ReleaseReader(reader)

        Return bSucces
    End Function

#End Region ' Load

#Region " Save "

    Private Function SaveEcospaceFleets(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim drow As DataRow = Nothing
        Dim iFleet As Integer = 0
        Dim iFleetID As Integer = 0
        Dim objKeys() As Object = {Nothing, Nothing} ' Composite key to find group per scenario
        Dim bSucces As Boolean = True

        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioID)
        objKeys(0) = iScenarioID

        Try
            writer = Me.m_db.GetWriter("EcospaceScenarioFleet")
            dt = writer.GetDataTable()

            For iFleet = 1 To ecospaceDS.nFleets

                ' Find fleet ID, it may be mapped to a different ID when saving to a new scenario
                iFleetID = idm.GetID(eDataTypes.EcospaceFleet, ecospaceDS.FleetDBID(iFleet))
                objKeys(1) = iFleetID

                ' Find existing row
                drow = dt.Rows.Find(objKeys)
                Debug.Assert(drow IsNot Nothing, String.Format("Cannot find existing row for fleet {0} ({1})", iFleetID, ecospaceDS.FleetDBID(iFleet)))

                ' Update fleet vars
                drow("EffPower") = ecospaceDS.EffPower(iFleet)

            Next iFleet

            ' Save changes
            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving Ecospace Fleet", ex.Message))
            bSucces = False
        End Try


        ' Save Sail and Port data
        bSucces = bSucces And Me.SaveEcospaceFleetMap(idm)
        ' Save habitat fishery
        bSucces = bSucces And Me.SaveEcospaceHabitatFishery(idm)
        ' Save MPA fishery
        bSucces = bSucces And Me.SaveEcospaceMPAFishery(idm)

        ' There
        Return bSucces

    End Function

    Private Function SaveEcospaceFleetMap(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim iFleet As Integer = 0
        Dim iRow As Integer = 0
        Dim iCol As Integer = 0
        Dim iPortID As Integer = 1
        Dim bSucces As Boolean = True

        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioID)

        Try
            ' Erase
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioFleetMap WHERE ScenarioID={0}", iScenarioID))
            writer = Me.m_db.GetWriter("EcospaceScenarioFleetMap")

            For iFleet = 1 To ecospaceDS.nFleets
                For iRow = 1 To ecospaceDS.InRow
                    For iCol = 1 To ecospaceDS.InCol
                        If ecospaceDS.Port(iFleet, iRow, iCol) Then

                            drow = writer.NewRow()
                            drow("ScenarioID") = iScenarioID
                            drow("FleetID") = idm.GetID(eDataTypes.EcospaceFleet, ecospaceDS.FleetDBID(iFleet))
                            drow("InRow") = iRow
                            drow("InCol") = iCol
                            drow("PortID") = iPortID
                            writer.AddRow(drow)

                            iPortID += 1 ' Haha

                        End If
                    Next iCol
                Next iRow
            Next iFleet

            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving EcospaceScenarioFleetMap", ex.Message))
            bSucces = False
        End Try

        ' Report outcome
        Return bSucces

    End Function

    Private Function SaveEcospaceHabitatFishery(ByVal idm As cIDMappings) As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim iFleet As Integer = 0
        Dim iHabitat As Integer = 0
        Dim bSucces As Boolean = True

        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioID)

        Try
            ' Erase
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioHabitatFishery WHERE ScenarioID={0}", iScenarioID))
            writer = Me.m_db.GetWriter("EcospaceScenarioHabitatFishery")

            For iFleet = 1 To ecospaceDS.nFleets
                For iHabitat = 0 To ecospaceDS.NoHabitats

                    If (ecospaceDS.GearHab(iFleet, iHabitat) = True) Then

                        drow = writer.NewRow()
                        drow("ScenarioID") = iScenarioID
                        drow("FleetID") = idm.GetID(eDataTypes.EcospaceFleet, ecospaceDS.FleetDBID(iFleet))
                        drow("HabitatID") = idm.GetID(eDataTypes.EcospaceHabitat, ecospaceDS.HabitatDBID(iHabitat))
                        writer.AddRow(drow)

                    End If
                Next iHabitat
            Next iFleet

            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving EcospaceScenarioHabitatFishery", ex.Message))
            bSucces = False
        End Try

        ' Save changes
        Return bSucces
    End Function

    Private Function SaveEcospaceMPAFishery(ByVal idm As cIDMappings) As Boolean
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim iFleet As Integer = 0
        Dim iMPA As Integer = 0
        Dim bSucces As Boolean = True

        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioID)

        Try
            ' Erase
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioMPAFishery WHERE ScenarioID={0}", iScenarioID))
            writer = Me.m_db.GetWriter("EcospaceScenarioMPAFishery")

            For iFleet = 1 To ecospaceDS.nFleets
                For iMPA = 1 To ecospaceDS.MPAno

                    If (ecospaceDS.MPAfishery(iFleet, iMPA) = True) Then

                        drow = writer.NewRow()
                        drow("ScenarioID") = iScenarioID
                        drow("FleetID") = idm.GetID(eDataTypes.EcospaceFleet, ecospaceDS.FleetDBID(iFleet))
                        drow("MPAID") = idm.GetID(eDataTypes.EcospaceMPA, ecospaceDS.MPADBID(iMPA))
                        writer.AddRow(drow)

                    End If
                Next iMPA
            Next iFleet

            Me.m_db.ReleaseWriter(writer, True)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving EcospaceScenarioMPAFishery", ex.Message))
            bSucces = False
        End Try

        ' Save changes
        Return bSucces
    End Function

#End Region ' Save

#Region " Modify "

    ''' <summary>
    ''' Create a fleet for each ecospace scenario
    ''' </summary>
    ''' <param name="iEcopathFleetID">Ecopath Fleet DBID</param>
    Private Function AddEcospaceFleetToAllScenarios(ByVal iEcopathFleetID As Integer) As Boolean

        Dim reader As IDataReader = Nothing
        Dim iID As Integer = 0
        Dim bSucces As Boolean = True

        Try

            reader = Me.m_db.GetReader(String.Format("SELECT ScenarioID FROM EcospaceScenario"))
            While reader.Read()
                bSucces = bSucces And AddEcospaceFleet(iEcopathFleetID, CInt(reader("ScenarioID")), iID)
            End While
            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' <summary>
    ''' Add an ecospace fleet to a given ecospace scenario.
    ''' </summary>
    ''' <param name="iEcopathFleetID">Ecopath Fleet DBID.</param>
    ''' <param name="iScenarioID">Scenario ID to add the fleet to.</param>
    Private Function AddEcospaceFleet(ByVal iEcopathFleetID As Integer, ByVal iScenarioID As Integer, ByRef iFleetID As Integer) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            iFleetID = CInt(Me.m_db.GetValue("SELECT MAX(FleetID) FROM EcospaceScenarioFleet")) + 1
        Catch ex As Exception
            iFleetID = 1
        End Try

        Try
            ' Add fleet
            writer = Me.m_db.GetWriter("EcospaceScenarioFleet")
            drow = writer.NewRow()
            drow("ScenarioID") = iScenarioID
            drow("FleetID") = iFleetID
            drow("EcopathFleetID") = iEcopathFleetID

            ' ToDo_JS: Figure out defaults for remaining row values
            ' EffPower:   ?
            ' MPAFishery: isn't the value for this implicied by EcospaceScenarioMPAFishery?

            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Modify

#End Region ' Fleets

#Region " MPAs "

#Region " Load "

    Private Function LoadEcospaceMPAs(ByVal iScenarioID As Integer) As Boolean
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim reader As IDataReader = Nothing
        Dim strMPAMonth As String = ""
        Dim bSucces As Boolean = True
        Dim iMPA As Integer = 0

        ' Allocate space for MPA data
        ecospaceDS.MPAno = CInt(Me.m_db.GetValue(String.Format("SELECT COUNT(*) FROM EcospaceScenarioMPA WHERE ScenarioID={0}", iScenarioID)))
        ecospaceDS.RedimMPAVariables()

        ' Load the data
        reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioMPA WHERE (ScenarioID={0}) ORDER BY Sequence ASC", iScenarioID))

        Try
            While reader.Read()

                ' Read fields
                iMPA += 1
                ' Get the data
                ecospaceDS.MPADBID(iMPA) = CInt(reader("MPAID"))
                ecospaceDS.MPAname(iMPA) = CStr(reader("MPAName"))

                ' Read month bit pattern
                strMPAMonth = CStr(reader("MPAMonth"))
                For iMonth As Integer = 0 To Math.Min(cCore.N_MONTHS, strMPAMonth.Length) - 1
                    ' MPAmonth is an array of boolean flags depicting wheter an MPA is open for fishing,
                    ' where closed months are stored as 0, and open months are stored as 1
                    ' EcospaceDS.MPAmonth: False if closed, True if open
                    ecospaceDS.MPAmonth(iMonth + 1, iMPA) = (strMPAMonth.Substring(iMonth, 1) = "1")
                Next iMonth

            End While

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EcospaceScenarioMPA {1}", ex.Message, iMPA))
            bSucces = False
        End Try

        Me.m_db.ReleaseReader(reader)

        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    Private Function SaveEcospaceMPAs(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim iScenarioIDSrc As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim iScenarioIDDest As Integer = 0
        Dim iID As Integer = 0
        Dim drow As DataRow = Nothing
        Dim bNewRow As Boolean = True
        Dim sbMPAMonth As New Text.StringBuilder
        Dim objKeys() As Object = {Nothing, Nothing} ' Composite key to find MPA per scenario
        Dim bSucces As Boolean = True

        Try
            iID = CInt(Me.m_db.GetValue("SELECT MAX(MPAID) FROM EcospaceScenarioMPA")) + 1
        Catch ex As Exception
            iID = 1
        End Try

        iScenarioIDDest = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioIDSrc)
        objKeys(0) = iScenarioIDDest

        Try
            writer = Me.m_db.GetWriter("EcospaceScenarioMPA")
            dt = writer.GetDataTable()

            For iMPA As Integer = 1 To ecospaceDS.MPAno

                ' Try to find row
                objKeys(1) = ecospaceDS.MPADBID(iMPA)
                drow = dt.Rows.Find(objKeys)

                bNewRow = (iScenarioIDSrc <> iScenarioIDDest) Or (drow Is Nothing)

                If bNewRow Then
                    drow = writer.NewRow()
                    drow("ScenarioID") = iScenarioIDDest
                    drow("MPAID") = iID
                    idm.Add(eDataTypes.EcospaceMPA, ecospaceDS.MPADBID(iMPA), iID)
                    iID += 1
                Else
                    drow.BeginEdit()
                End If

                ' Update fleet vars
                drow("MPAName") = ecospaceDS.MPAname(iMPA)

                ' Create MPA month bit pattern
                sbMPAMonth.Length = 0
                For iMonth As Integer = 1 To cCore.N_MONTHS
                    ' Closed for fishing: store as 0, open: store as 1
                    sbMPAMonth.Append(CStr(IIf(ecospaceDS.MPAmonth(iMonth, iMPA), "1", "0")))
                Next iMonth
                drow("MPAMonth") = sbMPAMonth.ToString()

                If bNewRow Then
                    writer.AddRow(drow)
                Else
                    drow.EndEdit()
                End If

            Next iMPA

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving Ecospace MPA", ex.Message))
            bSucces = False
        Finally
            Me.m_db.ReleaseWriter(writer)
            writer = Nothing
        End Try

        Return bSucces
    End Function

#End Region ' Save

#Region " Modify "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a MPA to the active scenario
    ''' </summary>
    ''' <param name="strMPAName"></param>
    ''' <param name="iDBID"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Function AppendEcospaceMPA(ByVal strMPAName As String, ByVal bMPAMonths() As Boolean, ByRef iDBID As Integer) As Boolean _
            Implements DataSources.IEcospaceDatasource.AppendEcospaceMPA

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)

        Return Me.AddEcospaceMPA(strMPAName, iScenarioID, bMPAMonths, iDBID)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a MPA to a given scenario.
    ''' </summary>
    ''' <param name="strMPAName"></param>
    ''' <param name="iScenarioID"></param>
    ''' <param name="iDBID"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function AddEcospaceMPA(ByVal strMPAName As String, ByVal iScenarioID As Integer, ByVal bMPAMonths() As Boolean, ByRef iDBID As Integer) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True
        Dim sbMPAMonth As New Text.StringBuilder

        Try
            ' MPAID unique for all scenarios
            iDBID = CInt(Me.m_db.GetValue("SELECT MAX(MPAID) FROM EcospaceScenarioMPA")) + 1
        Catch ex As Exception
            iDBID = 1
        End Try

        writer = Me.m_db.GetWriter("EcospaceScenarioMPA", "Sequence")

        drow = writer.NewRow()
        drow("ScenarioID") = iScenarioID
        drow("MPAID") = iDBID
        drow("MPAName") = strMPAName
        drow("Sequence") = iDBID

        sbMPAMonth.Length = 0
        For iMonth As Integer = 1 To Math.Min(cCore.N_MONTHS, bMPAMonths.Length - 1)
            ' Closed for fishing: store as 0, open: store as 1
            sbMPAMonth.Append(CStr(IIf(bMPAMonths(iMonth), "1", "0")))
        Next iMonth
        drow("MPAMonth") = sbMPAMonth.ToString()

        writer.AddRow(drow)

        Me.m_db.ReleaseWriter(writer)

        Return bSucces
    End Function

    Public Function RemoveEcospaceMPA(ByVal iDBID As Integer) As Boolean _
            Implements DataSources.IEcospaceDatasource.RemoveEcospaceMPA

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim bSucces As Boolean = True

        Try
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioMPA WHERE (ScenarioID={0}) AND (MPAID={1})", iScenarioID, iDBID))
            ' This could have far-fetched consequences throughout the scenario; the entire scenario should be reloaded.
            bSucces = Me.LoadEcospaceScenario(iScenarioID)
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing Ecospace MPAID {1}", ex.Message, iDBID))
            bSucces = False
        End Try
        Return bSucces
    End Function

#End Region ' Modify

#End Region ' MPAs

#Region " Weight layers "

#Region " Load "

    Private Function LoadEcospaceWeightLayers(ByVal iScenarioID As Integer) As Boolean

        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim readerLayer As IDataReader = Nothing
        Dim readerCells As IDataReader = Nothing
        Dim l As cEcospaceDataStructures.cLayerImportanceData = Nothing
        Dim bSucces As Boolean = True
        Dim iRow As Integer = 0
        Dim iCol As Integer = 0
        Dim iLayer As Integer = 0

        Try
            readerLayer = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioWeightLayer WHERE (ScenarioID={0})", iScenarioID))
            While readerLayer.Read()
                ' Get layer (0-based)
                l = ecospaceDS.ImportanceLayers(iLayer)
                ' Populate it
                l.DBID = CInt(readerLayer("LayerID"))
                l.strName = CStr(readerLayer("Name"))
                l.strDescription = CStr(readerLayer("Description"))
                l.sWeight = CSng(readerLayer("Weight"))

                Try
                    ' Read layer data
                    readerCells = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioWeightLayerCell WHERE LayerID={0}", l.DBID))
                    While readerCells.Read()
                        iRow = CInt(readerCells("InRow"))
                        iCol = CInt(readerCells("InCol"))
                        ' Valid cell?
                        If ((iRow <= ecospaceDS.InRow) And (iCol <= ecospaceDS.InCol)) Then
                            l.Data(iRow, iCol) = CSng(readerCells("Weight"))
                        End If
                    End While

                Catch ex As Exception
                    bSucces = False
                Finally
                    Me.m_db.ReleaseReader(readerCells)
                    readerCells = Nothing
                End Try
                ' Next!
                iLayer += 1
            End While

        Catch ex As Exception
            bSucces = False
        Finally
            Me.m_db.ReleaseReader(readerLayer)
            readerLayer = Nothing
        End Try

        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    Private Function SaveEcospaceWeightLayers(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioIDSrc As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim iScenarioIDdest As Integer = 0
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim l As cEcospaceDataStructures.cLayerImportanceData = Nothing
        Dim lID As Integer = 0
        Dim drow As DataRow = Nothing
        Dim bNewRow As Boolean = False
        Dim bSucces As Boolean = True
        Dim objKeys() As Object = {Nothing, Nothing}

        ' Get ID of scenario to save to
        iScenarioIDdest = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioIDSrc)
        objKeys(0) = iScenarioIDdest

        Try
            lID = CInt(Me.m_db.GetValue("SELECT MAX(LayerID) FROM EcospaceScenarioWeightLayer")) + 1
        Catch ex As Exception
            lID = 1
        End Try

        Try
            writer = Me.m_db.GetWriter("EcospaceScenarioWeightLayer", "Sequence", String.Format("ScenarioID={0}", iScenarioIDdest))
            dt = writer.GetDataTable()

            For iLayer As Integer = 0 To ecospaceDS.nImportanceLayers - 1

                ' Get layer
                l = ecospaceDS.ImportanceLayers(iLayer)

                ' Try to find existing row
                objKeys(1) = idm.GetID(eDataTypes.EcospaceLayerImportance, l.DBID)
                drow = dt.Rows.Find(objKeys)

                bNewRow = (iScenarioIDSrc <> iScenarioIDdest) Or (drow Is Nothing)

                If bNewRow Then
                    drow = writer.NewRow()
                    drow("ScenarioID") = iScenarioIDdest
                    drow("LayerID") = lID
                    idm.Add(eDataTypes.EcospaceLayerImportance, l.DBID, lID)
                    lID += 1
                Else
                    drow.BeginEdit()
                End If

                drow("Name") = l.strName
                drow("Description") = l.strDescription
                drow("Weight") = l.sWeight
                drow("Sequence") = iLayer

                If bNewRow Then
                    writer.AddRow(drow)
                Else
                    drow.EndEdit()
                End If

            Next iLayer

        Catch ex As Exception
            bSucces = False
        Finally
            Me.m_db.ReleaseWriter(writer)
            writer = Nothing
        End Try

        Return Me.SaveEcospaceWeightLayerCells(idm)

    End Function

    Private Function SaveEcospaceWeightLayerCells(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim l As cEcospaceDataStructures.cLayerImportanceData = Nothing
        Dim lID As Integer = 0
        Dim iRow As Integer = 0
        Dim iCol As Integer = 0
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        ' Get ID of scenario to save to
        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioID)

        Try
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioWeightLayerCell WHERE ScenarioID={0}", iScenarioID))

            writer = Me.m_db.GetWriter("EcospaceScenarioWeightLayerCell")

            For iLayer As Integer = 0 To ecospaceDS.nImportanceLayers - 1

                l = ecospaceDS.ImportanceLayers(iLayer)
                lID = idm.GetID(eDataTypes.EcospaceLayerImportance, l.DBID)

                For iRow = 1 To ecospaceDS.InRow
                    For iCol = 1 To ecospaceDS.InCol

                        ' Need to save this?
                        If l.Data(iRow, iCol) <> 0.0! Then
                            ' Create new row
                            drow = writer.NewRow()
                            ' Store simple values
                            drow("ScenarioID") = iScenarioID
                            drow("LayerID") = idm.GetID(eDataTypes.EcospaceLayerImportance, lID)
                            drow("InRow") = iRow
                            drow("InCol") = iCol
                            drow("Weight") = l.Data(iRow, iCol)
                            writer.AddRow(drow)
                        End If
                    Next iCol
                Next iRow

            Next iLayer

            Me.m_db.ReleaseWriter(writer)
            writer = Nothing

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

#End Region ' Save

#Region " Modify "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds an ecospace Importance Layer to the active scenario in the
    ''' datasource.
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="strDescription"></param>
    ''' <param name="sWeight"></param>
    ''' <param name="iDBID">Database ID assigned to the new layer.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function AppendEcospaceImportanceLayer(ByVal strName As String, ByVal strDescription As String, ByVal sWeight As Single, ByRef iDBID As Integer) As Boolean _
            Implements DataSources.IEcospaceDatasource.AppendEcospaceImportanceLayer

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)

        Return Me.AddEcospaceImportanceLayer(strName, iScenarioID, strDescription, sWeight, iDBID)

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Add a ImportanceLayer to a given scenario.
    ''' </summary>
    ''' <param name="strName"></param>
    ''' <param name="strDescription"></param>
    ''' <param name="sWeight"></param>
    ''' <param name="iDBID"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function AddEcospaceImportanceLayer(ByVal strName As String, ByVal iScenarioID As Integer, ByVal strDescription As String, ByVal sWeight As Single, ByRef iDBID As Integer) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            ' MPAID unique for all scenarios
            iDBID = CInt(Me.m_db.GetValue("SELECT MAX(LayerID) FROM EcospaceScenarioWeightLayer")) + 1
        Catch ex As Exception
            iDBID = 1
        End Try

        writer = Me.m_db.GetWriter("EcospaceScenarioWeightLayer", "Sequence")

        drow = writer.NewRow()
        drow("ScenarioID") = iScenarioID
        drow("LayerID") = iDBID
        drow("Name") = strName
        drow("Sequence") = iDBID
        drow("Description") = strDescription
        drow("Weight") = sWeight
        writer.AddRow(drow)

        Me.m_db.ReleaseWriter(writer)

        Return bSucces
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds an ecospace Importance Layer from the active scenario in the
    ''' datasource.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the layer to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function RemoveEcospaceImportanceLayer(ByVal iDBID As Integer) As Boolean _
            Implements IEcospaceDatasource.RemoveEcospaceImportanceLayer

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim iScenarioID As Integer = ecopathDS.EcospaceScenarioDBID(ecopathDS.ActiveEcospaceScenario)
        Dim bSucces As Boolean = True

        Try
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioWeightLayerCell WHERE (LayerID={0})", iDBID))
            Me.m_db.Execute(String.Format("DELETE FROM EcospaceScenarioWeightLayer WHERE (ScenarioID={0}) AND (LayerID={1})", iScenarioID, iDBID))
            ' This could have far-fetched consequences throughout the scenario; the entire scenario should be reloaded.
            bSucces = Me.LoadEcospaceScenario(iScenarioID)
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing Ecospace Importance Layer {1}", ex.Message, iDBID))
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Modify

#End Region ' Weight layers

#End Region ' Ecospace

#Region " Ecotracer "

#Region " Diagnostics "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States if the datasource has unsaved changes for Ecotracer.
    ''' </summary>
    ''' <returns>True if the datasource has pending changes for Ecotracer.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsEcotracerModified() As Boolean _
             Implements DataSources.IEcotracerDatasource.IsEcotracerModified

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData

        ' Hmm, maybe the datasource should have a better way to 'remember' whether a tracer scenario has been loaded.
        If Not Me.IsConnected() Then Return False
        If ecopathDS.ActiveEcotracerScenario < 0 Then Return False

        Return Me.IsChanged(s_EcotracerComponents)

    End Function

#End Region ' Diagnostics

#Region " Load "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads an Ecotracer scenario from the datasource.
    ''' </summary>
    ''' <param name="iScenarioID">Database ID of the scenario to load.</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>An implementing class should ensure that this load will cascade to
    ''' load all information pertaining to a scenario.</remarks>
    ''' -------------------------------------------------------------------
    Public Function LoadEcotracerScenario(ByVal iScenarioID As Integer) As Boolean _
        Implements DataSources.IEcotracerDatasource.LoadEcotracerScenario

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim tracerDS As cContaminantTracerDataStructures = Me.m_core.m_tracerData
        Dim iConForceNumber As Integer = 0
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        ' JS08Dec07: Ideally, this should happen here but both Ecosim and Ecospace
        '            assume that the tracer data has already been dimensioned to the
        '            number of groups long before a tracer scenario has been loaded.
        '            This needs to change!
        tracerDS.RedimByNGroups(ecopathDS.NumGroups)

        reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcotracerScenario WHERE (ScenarioID={0})", iScenarioID))
        Try
            ' Read the one record
            reader.Read()
            tracerDS.Czero(0) = CSng(reader("Czero"))
            tracerDS.Cinflow(0) = CSng(reader("Cinflow"))
            tracerDS.CoutFlow(0) = CSng(reader("Coutflow"))
            tracerDS.cdecay(0) = CSng(reader("Cdecay"))
            'iConForceNumber = CInt(Me.ReadSafe(reader, "ConForcingShapeID", 0))
            'tracerDS.ConForceNumber = Math.Max(0, Array.IndexOf(ecosimDS.ForcingDBIDs, iConForceNumber))

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading Ecotracer Scenario {1}", ex.Message, iScenarioID))
            bSucces = False
        End Try

        Me.m_db.ReleaseReader(reader)

        ' Set active tracer scenario
        ecopathDS.ActiveEcotracerScenario = Array.IndexOf(ecopathDS.EcotracerScenarioDBID, iScenarioID)

        ' Load additional data
        bSucces = bSucces And Me.LoadEcotracerGroups(iScenarioID)

        Me.ClearChanged(s_EcotracerComponents)

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Load Ecotracer groups from the datasource.
    ''' </summary>
    ''' <param name="iScenarioID">The Ecotracer scenario to load groups for.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function LoadEcotracerGroups(ByVal iScenarioID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim tracerDS As cContaminantTracerDataStructures = Me.m_core.m_tracerData
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True
        Dim iGroup As Integer = 0

        ' Read the data
        Try
            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcotracerScenarioGroup WHERE (ScenarioID={0})", iScenarioID))
            While reader.Read()

                ' Resolve group index
                iGroup = Array.IndexOf(ecopathDS.GroupDBID, CInt(reader("EcopathGroupID")))
                ' Sanity check
                Debug.Assert(iGroup > -1)
                ' Load the data
                tracerDS.Czero(iGroup) = CSng(reader("Czero"))
                tracerDS.Cimmig(iGroup) = CSng(reader("Cimmig"))
                tracerDS.Cenv(iGroup) = CSng(reader("Cenv"))
                tracerDS.cdecay(iGroup) = CSng(reader("Cdecay"))
                tracerDS.CexcretionRate(iGroup) = CSng(reader("Cexcretionrate"))

            End While
            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading Ecotracer group {1}", ex.Message, iGroup))
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Save the current active Ecotracer scenario in the datasource under
    ''' a given database ID.
    ''' </summary>
    ''' <param name="iScenarioID">Database ID to save the current scenario to.
    ''' If this parameter is left blank, the current scenario is saved
    ''' under its own database ID.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function SaveEcotracerScenario(ByVal iScenarioID As Integer) As Boolean _
         Implements DataSources.IEcotracerDatasource.SaveEcotracerScenario

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim tracerDS As cContaminantTracerDataStructures = Me.m_core.m_tracerData
        Dim iActiveScenarioID As Integer = ecopathDS.EcotracerScenarioDBID(ecopathDS.ActiveEcotracerScenario)
        Dim idm As cIDMappings = Nothing
        Dim bSucces As Boolean = True

        ' Abort if there is no active scenario
        If iActiveScenarioID = 0 Then Return False

        ' Prepare for saving
        idm = New cIDMappings()
        If iScenarioID = 0 Then iScenarioID = iActiveScenarioID

        ' Duplicating a scenario?
        If iScenarioID <> iActiveScenarioID Then
            ' #Yes: add ID mapping to allow copying of scenario content
            idm.Add(eDataTypes.EcotracerScenario, iActiveScenarioID, iScenarioID)
        End If

        ' Start transaction
        bSucces = Me.m_db.BeginTransaction()
        ' Save scenario
        bSucces = bSucces And Me.SaveEcotracerScenario(idm)
        ' Commit transaction
        If bSucces Then
            bSucces = Me.m_db.CommitTransaction(True)
        Else
            Me.m_db.RollbackTransaction()
        End If

        If bSucces Then
            ' Reload ecotracer scenario definitions
            Me.LoadEcotracerScenarioDefinitions()
            ' Clear changed admin
            Me.ClearChanged(s_EcotracerComponents)
        End If

        Return bSucces
    End Function

#Region " Internals "

    Private Function SaveEcotracerScenario(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim tracerDS As cContaminantTracerDataStructures = Me.m_core.m_tracerData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenario As Integer = ecopathDS.ActiveEcotracerScenario
        Dim iScenarioID As Integer = 0
        Dim bSucces As Boolean = True

        iScenarioID = idm.GetID(eDataTypes.EcotracerScenario, ecopathDS.EcotracerScenarioDBID(iScenario))

        Try

            writer = Me.m_db.GetWriter("EcotracerScenario")
            dt = writer.GetDataTable()
            drow = dt.Rows.Find(iScenarioID)

            drow.BeginEdit()
            drow("Czero") = tracerDS.Czero(0)
            drow("Cinflow") = tracerDS.Cinflow(0)
            drow("Coutflow") = tracerDS.CoutFlow(0)
            drow("Cdecay") = tracerDS.cdecay(0)
            drow("ConForcingShapeID") = ecosimDS.ForcingDBIDs(tracerDS.ConForceNumber)
            drow("LastSaved") = cDBDataSource.GetJulianDate()
            drow.EndEdit()

            ' Save changes
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving ecotracer scenario {1}", ex.Message, iScenarioID))
            bSucces = False
        End Try

        bSucces = bSucces And Me.SaveEcotracerGroups(idm)

        Return bSucces

    End Function

    Private Function SaveEcotracerGroups(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim tracerDS As cContaminantTracerDataStructures = Me.m_core.m_tracerData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim iScenarioID As Integer = ecopathDS.EcotracerScenarioDBID(ecopathDS.ActiveEcotracerScenario)
        Dim drow As DataRow = Nothing
        Dim iGroup As Integer = 0
        Dim objKeys() As Object = {Nothing, Nothing} ' Composite key to find group per scenario
        Dim bSucces As Boolean = True

        ' Get mapped scenario ID, in case saving to a different scenario
        iScenarioID = idm.GetID(eDataTypes.EcotracerScenario, iScenarioID)
        objKeys(0) = iScenarioID

        Try
            writer = Me.m_db.GetWriter("EcotracerScenarioGroup")
            dt = writer.GetDataTable()

            For iGroup = 1 To ecopathDS.NumGroups

                ' Find group ID, it may be mapped to a different ID when saving to a new scenario
                objKeys(1) = ecopathDS.GroupDBID(iGroup)

                ' Find existing row
                drow = dt.Rows.Find(objKeys)
                Debug.Assert(drow IsNot Nothing, String.Format("Cannot find existing row for group {0}", ecopathDS.GroupDBID(iGroup)))

                drow("CZero") = tracerDS.Czero(iGroup)
                drow("Cimmig") = tracerDS.Cimmig(iGroup)
                drow("Cenv") = tracerDS.Cenv(iGroup)
                drow("Cdecay") = tracerDS.cdecay(iGroup)
                drow("Cexcretionrate") = tracerDS.CexcretionRate(iGroup)

            Next iGroup

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving EcotracerGroup", ex.Message))
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        Return bSucces

    End Function

#End Region ' Internals

#End Region ' Save

#Region " Modify "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds an Ecotracer scenario to the datasource.
    ''' </summary>
    ''' <param name="strScenarioName">Name to assign to new scenario.</param>
    ''' <param name="strDescription">Description to assign to new scenario.</param>
    ''' <param name="strAuthor">Author to assign to the new scenario.</param>
    ''' <param name="strContact">Contact info to assign to the new scenario.</param>
    ''' <param name="iScenarioID">Database ID assigned to the new scenario.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function AppendEcotracerScenario(ByVal strScenarioName As String, ByVal strDescription As String, ByVal strAuthor As String, ByVal strContact As String, ByRef iScenarioID As Integer) As Boolean _
             Implements DataSources.IEcotracerDatasource.AppendEcotracerScenario

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim tracerDS As cContaminantTracerDataStructures = Me.m_core.m_tracerData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            Try
                iScenarioID = CInt(Me.m_db.GetValue("SELECT MAX(ScenarioID) FROM EcotracerScenario")) + 1
            Catch
                iScenarioID = 1
            End Try

            Me.m_db.BeginTransaction()

            writer = Me.m_db.GetWriter("EcotracerScenario")

            drow = writer.NewRow()
            drow("ScenarioID") = iScenarioID
            drow("ScenarioName") = strScenarioName
            drow("Description") = strDescription
            drow("Author") = strAuthor
            drow("Contact") = strContact
            drow("LastSaved") = cDBDataSource.GetJulianDate()
            writer.AddRow(drow)

            Me.m_db.ReleaseWriter(writer)

            ' ------
            ' Generate Ecopath objects for new Ecotracer scenario
            ' ------

            ' First duplicate all Ecospace 'objects'
            For i As Integer = 1 To ecopathDS.NumGroups
                ' Add group to the new scenario
                bSucces = bSucces And Me.AddEcotracerGroup(ecopathDS.GroupDBID(i), iScenarioID)
            Next

            If bSucces Then
                bSucces = Me.m_db.CommitTransaction(True)
            Else
                Me.m_db.RollbackTransaction()
            End If

            ' Reload scenario definitions
            bSucces = bSucces And Me.LoadEcotracerScenarioDefinitions()

            Me.ClearChanged(s_EcotracerComponents)

        Catch ex As Exception
            Me.m_db.RollbackTransaction()
            Me.LogMessage(String.Format("Error {0} occurred while appending Ecotracer scenario {1}", ex.Message, strScenarioName))
            bSucces = False
        End Try

        Return bSucces
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes an Ecotracer scenario from the datasource.
    ''' </summary>
    ''' <param name="iScenarioID">Database ID of the scenario to remove.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function RemoveEcotracerScenario(ByVal iScenarioID As Integer) As Boolean _
             Implements DataSources.IEcotracerDatasource.RemoveEcotracerScenario

        Dim bSucces As Boolean = True

        Try
            ' Delete 'soft links'
            '    DB update 6.036!
            Me.m_db.Execute(String.Format("DELETE FROM EcotracerScenarioGroup WHERE (ScenarioID={0})", iScenarioID))
            ' Delete scenario
            Me.m_db.Execute(String.Format("DELETE FROM EcotracerScenario WHERE (ScenarioID={0})", iScenarioID))
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing Ecotracer scenarioID {1}", ex.Message, iScenarioID))
            bSucces = False
        End Try

        ' Reload scenario definitions
        bSucces = bSucces And Me.LoadEcotracerScenarioDefinitions()

        Return bSucces

    End Function

    ''' <summary>
    ''' Create a group for each Ecotracer scenario
    ''' </summary>
    ''' <param name="iEcopathGroupID">Ecopath Group DBID</param>
    Private Function AddEcotracerGroupToAllScenarios(ByVal iEcopathGroupID As Integer) As Boolean

        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader(String.Format("SELECT ScenarioID FROM EcotracerScenario"))
            While reader.Read()
                bSucces = bSucces And AddEcotracerGroup(iEcopathGroupID, CInt(reader("ScenarioID")))
            End While
            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' <summary>
    ''' Add a group to a given Ecotracer scenario.
    ''' </summary>
    ''' <param name="iEcopathGroupID"><see cref="cEcoPathGroupInput.DBID">Ecopath ID</see> of this group</param>
    ''' <param name="iScenarioID"><see cref="cEcotracerScenario.DBID">Ecotracer scenario ID</see> of the scenario to add the group to.</param>
    ''' <returns>True if succesful.</returns>
    Private Function AddEcotracerGroup(ByVal iEcopathGroupID As Integer, ByVal iScenarioID As Integer) As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            ' Add group
            writer = Me.m_db.GetWriter("EcotracerScenarioGroup")

            drow = writer.NewRow()
            drow("ScenarioID") = iScenarioID
            drow("EcopathGroupID") = iEcopathGroupID
            writer.AddRow(drow)

            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            bSucces = False
        End Try

        Return bSucces
    End Function

#End Region ' Modify

#End Region ' Ecotracer

#Region " Auxillary data "

    Private Function LoadAuxillaryData() As Boolean

        Dim reader As IDataReader = Me.m_db.GetReader("SELECT * FROM Remark")
        Dim dataType As eDataTypes = eDataTypes.NotSet
        Dim iDBID As Integer = -1
        Dim bSucces As Boolean = True

        Try
            While reader.Read()
                If Not Convert.IsDBNull(reader("ModelID")) Then
                    dataType = eDataTypes.EwEModel
                    iDBID = CInt(reader("ModelID"))
                End If
                If Not Convert.IsDBNull(reader("EcopathGroupID")) Then
                    dataType = eDataTypes.EcoPathGroupInput
                    iDBID = CInt(reader("EcopathGroupID"))
                End If
                If Not Convert.IsDBNull(reader("EcosimGroupID")) Then
                    dataType = eDataTypes.EcoSimGroupInput
                    iDBID = CInt(reader("EcosimGroupID"))
                End If
                If Not Convert.IsDBNull(reader("StanzaID")) Then
                    dataType = eDataTypes.Stanza
                    iDBID = CInt(reader("StanzaID"))
                End If
                If Not Convert.IsDBNull(reader("FleetID")) Then
                    dataType = eDataTypes.FleetInput
                    iDBID = CInt(reader("FleetID"))
                End If
                If Not Convert.IsDBNull(reader("EcosimScenarioID")) Then
                    dataType = eDataTypes.EcoSimScenario
                    iDBID = CInt(reader("EcosimScenarioID"))
                End If
                If Not Convert.IsDBNull(reader("ShapeID")) Then
                    dataType = eDataTypes.EggProd
                    iDBID = CInt(reader("ShapeID"))
                End If
                If Not Convert.IsDBNull(reader("EcospaceScenarioID")) Then
                    dataType = eDataTypes.EcoSpaceScenario
                    iDBID = CInt(reader("EcospaceScenarioID"))
                End If
                'If Not Convert.IsDBNull(reader("EcospaceHabitatID")) Then
                '    dataType = eDataTypes.EcospaceHabitat
                '    iDBID = CInt(reader("EcospaceHabitatID"))
                'End If

                Try
                    If (Not Convert.IsDBNull(reader("Remark"))) Then
                        Me.m_core.StoreRemark(CStr(reader("Remark")), CStr(reader("ValueID")))
                    End If
                Catch ex As Exception
                    ' All well
                End Try

                Try
                    If (Not Convert.IsDBNull(reader("VisualStyle"))) Then
                        Me.m_core.StoreVisualStyle(cVisualStyleReader.StringToStyle(CStr(reader("VisualStyle"))), CStr(reader("ValueID")))
                    End If
                Catch ex As Exception

                End Try

            End While
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading AuxillaryData", ex.Message))
            bSucces = False
        End Try

        bSucces = bSucces And Me.LoadPedigreeLevels()

        Me.m_db.ReleaseReader(reader)
        Return bSucces

    End Function

    Private Function SaveAuxillaryData() As Boolean

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim ad As cAuxiliaryData = Nothing
        Dim bSucces As Boolean = True

        Try
            Me.m_db.Execute("DELETE * FROM Remark")
            writer = Me.m_db.GetWriter("Remark")

            For Each strValueID As String In Me.m_core.m_dtAuxiliaryData.Keys
                ' Get actual remark instance
                ad = m_core.m_dtAuxiliaryData(strValueID)
                ' Has anything to save?
                If ((Not String.IsNullOrEmpty(ad.Remark)) Or _
                    (Not Object.ReferenceEquals(ad.VisualStyle, Nothing)) Or _
                    (ad.Pedigree >= 0)) Then

                    ' Make row
                    drow = writer.NewRow()
                    drow("ValueID") = strValueID
                    drow("Remark") = ad.Remark
                    drow("Pedigree") = CInt(IIf(ad.Pedigree > 0, ad.Pedigree, cCore.NULL_VALUE))

                    Try
                        If ad.VisualStyle IsNot Nothing Then
                            drow("VisualStyle") = cVisualStyleReader.StyleToString(ad.VisualStyle)
                        Else
                            drow("VisualStyle") = ""
                        End If
                    Catch ex As Exception
                    End Try

                    Select Case ad.DataType
                        Case eDataTypes.EwEModel
                            drow("ModelID") = ad.DBID
                        Case eDataTypes.EcoPathGroupInput
                            drow("EcopathGroupID") = ad.DBID
                        Case eDataTypes.EcoSimGroupInput
                            drow("EcosimGroupID") = ad.DBID
                        Case eDataTypes.Stanza
                            drow("StanzaID") = ad.DBID
                        Case eDataTypes.FleetInput
                            drow("FleetID") = ad.DBID
                        Case eDataTypes.EcoSimScenario
                            drow("EcosimScenarioID") = ad.DBID
                        Case eDataTypes.EggProd, eDataTypes.Forcing, eDataTypes.Mediation, _
                             eDataTypes.FishMort, eDataTypes.FishingEffort
                            drow("ShapeID") = ad.DBID
                        Case eDataTypes.EcoSpaceScenario
                            drow("EcospaceScenarioID") = ad.DBID
                    End Select
                    writer.AddRow(drow)
                End If
            Next
        Catch ex As Exception
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        bSucces = bSucces And Me.SavePedigreeLevels()

        Return bSucces

    End Function

#End Region ' Auxillary data

End Class
