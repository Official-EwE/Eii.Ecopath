'==============================================================================
'
' $Log: cDBDataSource.vb,v $
' Revision 1.1  2008/09/26 07:30:14  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.242  2008/09/25 02:30:57  jeroens
' Moved max fishing mortaility from search datastructures to Ecosim
'
' Revision 1.241  2008/09/22 14:01:20  jeroens
' Fixed issue 466
'
' Revision 1.240  2008/08/27 14:03:43  jeroens
' Fixed issue 428
'
' Revision 1.239  2008/08/15 03:44:16  jeroens
' MPAaaaaargh
'
' Revision 1.238  2008/08/14 18:11:56  jeroens
' Fixed duplicating issue when saving weight layers
'
' Revision 1.237  2008/08/14 04:54:50  jeroens
' Fixed bad bug in determining no. of importance layers
'
' Revision 1.236  2008/08/13 19:51:45  jeroens
' Fixed possible Ecospace scenario save crash
' Increased ability to repair DB
'
' Revision 1.235  2008/08/11 22:14:07  jeroens
' Importance layer cells stored and written
'
' Revision 1.234  2008/08/11 02:02:21  jeroens
' Uses renamed basemap cLayerImportanceData data
'
' Revision 1.233  2008/08/10 17:07:41  jeroens
' Debugged Importance layer craetion/deletion
'
' Revision 1.232  2008/08/08 23:17:23  jeroens
' Properly implemented SaveScenarioAs
' Added ImportanceLayers support
'
' Revision 1.231  2008/08/07 13:39:41  jeroens
' Enough of this MPA stuff!!!! Now it works
'
' Revision 1.230  2008/08/07 03:58:29  jeroens
' Fixed MPA closed/open confusion
'
' Revision 1.229  2008/08/04 16:07:00  jeroens
' Fixed issue 528
'
' Revision 1.228  2008/07/29 13:05:53  jeroens
' Fixed bug in reading monetary units
'
' Revision 1.227  2008/07/25 03:00:40  jeroens
' Incorporating new file extensions (w Joe)
' Adding error diagnostics on file access
'
' Revision 1.226  2008/07/23 18:01:49  jeroens
' Custom monetary units not supported yet
'
' Revision 1.225  2008/07/21 18:44:56  jeroens
' Woops
'
' Revision 1.224  2008/07/21 14:07:30  jeroens
' Added pedigree interfaces
'
' Revision 1.223  2008/07/17 19:22:50  jeroens
' Added MonetaryUnit to EwEModel
'
' Revision 1.222  2008/07/16 21:34:33  jeroens
' Slowly getting paranoid...
'
' Revision 1.221  2008/07/16 21:16:49  jeroens
' Fixed issue 500
'
' Revision 1.220  2008/07/14 17:07:55  jeroens
' Fixed dataset delete bug
'
' Revision 1.219  2008/07/10 19:16:24  jeroens
' Cleared default "year" for Time units
'
' Revision 1.218  2008/07/10 18:29:52  jeroens
' Fixed units to properly behave
'
' Revision 1.217  2008/07/03 20:12:43  jeroens
' New Ecospace scenarios will receive RelPP cells of 1 (fixes bug 410)
'
' Revision 1.216  2008/07/02 20:43:56  jeroens
' Default Ecospace model type set to 'Multi-stanza'
'
' Revision 1.215  2008/06/06 15:55:57  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.214  2008/05/26 20:32:56  jeroens
' Added model time unit, currency unit
'
' Revision 1.213  2008/05/20 13:41:31  jeroens
' Increased robustness when reading TS
'
' Revision 1.212  2008/04/08 20:34:40  jeroens
' Even invalid database IDs will flag the datasource dirty
'
' Revision 1.211  2008/04/07 21:46:56  jeroens
' Added default detritus fate to new groups
'
' Revision 1.210  2008/04/07 17:00:48  jeroens
' Transactions committed properly
'
' Revision 1.209  2008/04/06 03:49:48  jeroens
' Prevented 0 length time steps in space
'
' Revision 1.208  2008/04/02 18:52:51  jeroens
' Missing fishing rate shapes are created quietly when loading
'
' Revision 1.207  2008/04/02 00:39:35  jeroens
' DiscardFate errors bypassed, no longer stop the model from loading
'
' Revision 1.206  2008/03/07 18:19:59  jeroens
' Added Ecopath Area
'
' Revision 1.205  2008/02/28 20:33:18  joeb
' Added Left and Right Salinity
'
' Revision 1.204  2008/02/28 16:04:12  jeroens
' Fixed hang bug in LoadEcospaceFleetMap
'
' Revision 1.203  2008/02/25 18:44:36  jeroens
' AppendTimeSeries made public
'
' Revision 1.202  2008/02/25 13:57:24  jeroens
' Added crash test on AppendTimeSeries
'
' Revision 1.201  2008/02/25 13:48:21  jeroens
' Fixed Time series save issue
'
' Revision 1.200  2008/02/22 21:44:01  jeroens
' vbK tied to StanzaLifeStage
'
' Revision 1.199  2008/02/22 18:31:25  jeroens
' Fixed bug 373
'
'==============================================================================

Option Strict On

Imports EwECore.Database
Imports EwECore.DataSources
Imports EwECore.Auxiliary
Imports System.Data
Imports System.Text
Imports EwEPlugin
Imports EwEUtils.Database
Imports EwEUtils.Core

''' ---------------------------------------------------------------------------
''' <summary>
''' <see cref="IEwEDataSource">EwE datasource</see> implementation for reading
''' and writing Ecopath, Ecosim and Ecospace data from a database.
''' </summary>
''' ---------------------------------------------------------------------------
<CLSCompliant(False)> _
Public Class cDBDataSource
    : Implements IEwEDataSource, IEcopathDataSource, IEcosimDatasource, IEcospaceDatasource, IEcotracerDatasource

    ''' <summary>The <see cref="cEwEDatabase">Database</see> connected to this datasource.</summary>
    Private m_db As cEwEDatabase = Nothing
    ''' <summary>The <see cref="cCore">core</see> connected to this datasource.</summary>
    Private m_core As cCore = Nothing
    Private m_strName As String = ""

#Region " Generic "

    Public Sub New(ByRef db As cEwEDatabase)

        ' Pre
        Debug.Assert(db IsNot Nothing)
        ' Store ref to DB
        Me.m_db = db
        ' Update internal admin
        Me.RegisterDataTypeComponents()

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Open an existing DB.
    ''' </summary>
    ''' <param name="strName">Name of the DB database to open.</param>
    ''' <param name="core"><see cref="cCore">Core instance</see> that holds the 
    ''' datastructures to read to, and write from.</param>
    ''' <returns>True if opened successfully.</returns>
    ''' -------------------------------------------------------------------
    Public Function Open(ByVal strName As String, ByVal core As cCore) As cEwEDatabase.eAccessType _
            Implements DataSources.IEwEDataSource.Open

        ' Attempt to open existing
        Dim atResult As cEwEDatabase.eAccessType = Me.m_db.Open(strName)
        ' Any luck?
        If atResult = cEwEDatabase.eAccessType.Opened Then
            ' Store core
            Me.m_core = core
            Me.m_strName = strName
        End If
        ' Report succes
        Return atResult

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
    Public Function Create(ByVal strName As String, ByVal strModelName As String, ByVal core As cCore) As cEwEDatabase.eAccessType _
             Implements DataSources.IEwEDataSource.Create

        ' Create new db
        Dim atResult As cEwEDatabase.eAccessType = Me.m_db.Create(strName, strModelName, True)

        If atResult = cEwEDatabase.eAccessType.Created Then
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
    Public Function SaveAs(ByVal strFileName As String, ByVal strModelName As String) As cEwEDatabase.eAccessType
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

    Public Function BeginTransaction() As Boolean Implements DataSources.IEwEDataSource.BeginTransaction
        Return True
        'Return Me.m_db.BeginTransaction()
    End Function

    Public Function EndTransaction(ByVal bCommit As Boolean) As Boolean Implements DataSources.IEwEDataSource.EndTransaction
        Return True
        'If bCommit Then
        '    Return Me.m_db.CommitTransaction()
        'Else
        '    Return Me.m_db.RollbackTransaction
        'End If
    End Function

#Region " Helper methods "

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

    ''' <summary>Dictionary of changed database IDs, categorized per eDataType.</summary>
    Private m_dictChangedDBIDs As New Dictionary(Of eDataTypes, List(Of Integer))
    ''' <summary>Dictonary stating to what message source each datatype belongs.</summary>
    Private m_dictDataTypeComponents As New Dictionary(Of eDataTypes, eMessageSource)

    Private Sub RegisterDataTypeComponents()

        ' Configure Model descriptive values
        m_dictDataTypeComponents.Add(eDataTypes.EwEModel, eMessageSource.DataSource)
        m_dictDataTypeComponents.Add(eDataTypes.EcoSimScenario, eMessageSource.DataSource)
        m_dictDataTypeComponents.Add(eDataTypes.EcoSpaceScenario, eMessageSource.DataSource)
        m_dictDataTypeComponents.Add(eDataTypes.EcotracerScenario, eMessageSource.DataSource)
        m_dictDataTypeComponents.Add(eDataTypes.PedigreeLevel, eMessageSource.DataSource)

        ' Configure Ecopath
        m_dictDataTypeComponents.Add(eDataTypes.EcoPathGroupInput, eMessageSource.EcoPath)
        m_dictDataTypeComponents.Add(eDataTypes.FleetInput, eMessageSource.EcoPath)
        m_dictDataTypeComponents.Add(eDataTypes.Stanza, eMessageSource.EcoPath)

        ' Configure Ecosim
        m_dictDataTypeComponents.Add(eDataTypes.EcoSimGroupInput, eMessageSource.EcoSim)
        m_dictDataTypeComponents.Add(eDataTypes.EcoSimModelParameter, eMessageSource.EcoSim)
        m_dictDataTypeComponents.Add(eDataTypes.Forcing, eMessageSource.EcoSim)
        m_dictDataTypeComponents.Add(eDataTypes.EggProd, eMessageSource.EcoSim)
        m_dictDataTypeComponents.Add(eDataTypes.Mediation, eMessageSource.EcoSim)
        m_dictDataTypeComponents.Add(eDataTypes.FishingRate, eMessageSource.EcoSim)
        m_dictDataTypeComponents.Add(eDataTypes.FishMort, eMessageSource.EcoSim)
        m_dictDataTypeComponents.Add(eDataTypes.GroupTimeSeries, eMessageSource.EcoSim)
        m_dictDataTypeComponents.Add(eDataTypes.FleetTimeSeries, eMessageSource.EcoSim)

        ' Configure Ecospace
        m_dictDataTypeComponents.Add(eDataTypes.EcospaceModelParameter, eMessageSource.EcoSpace)
        m_dictDataTypeComponents.Add(eDataTypes.EcospaceBasemap, eMessageSource.EcoSpace)
        m_dictDataTypeComponents.Add(eDataTypes.EcospaceBasemapLayer, eMessageSource.EcoSpace)
        m_dictDataTypeComponents.Add(eDataTypes.EcospaceHabitat, eMessageSource.EcoSpace)
        m_dictDataTypeComponents.Add(eDataTypes.EcospaceRegion, eMessageSource.EcoSpace)
        m_dictDataTypeComponents.Add(eDataTypes.EcospaceMPA, eMessageSource.EcoSpace)
        m_dictDataTypeComponents.Add(eDataTypes.EcospaceGroup, eMessageSource.EcoSpace)
        m_dictDataTypeComponents.Add(eDataTypes.EcospaceFleet, eMessageSource.EcoSpace)
        m_dictDataTypeComponents.Add(eDataTypes.EcospaceImportanceLayer, eMessageSource.EcoSpace)

        ' Configure Ecotracer
        m_dictDataTypeComponents.Add(eDataTypes.EcotracerGroupInput, eMessageSource.Ecotracer)
        m_dictDataTypeComponents.Add(eDataTypes.EcotracerModelParameters, eMessageSource.Ecotracer)

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Flag a core object as changed in the datasource.
    ''' </summary>
    ''' <param name="dataType">The <see cref="eDataTypes">Type</see> of the object that changed.</param>
    ''' <param name="iDBID">The database ID of the object that changed.</param>
    ''' -------------------------------------------------------------------
    Public Sub SetChanged(ByVal dataType As eDataTypes, Optional ByVal iDBID As Integer = 0) _
            Implements IEwEDataSource.SetChanged

        Dim lInt As List(Of Integer) = Nothing

        If (Me.m_dictChangedDBIDs.ContainsKey(dataType)) Then
            lInt = Me.m_dictChangedDBIDs(dataType)
        Else
            lInt = New List(Of Integer)
            Me.m_dictChangedDBIDs.Add(dataType, lInt)
        End If

        ' JS 08apr08: For now, also flag core as dirty on invalid DBID values
        'If iDBID > 0 And Not lInt.Contains(iDBID) Then
        If Not lInt.Contains(iDBID) Then
            lInt.Add(iDBID)
        End If

    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Gets a 1-based array of changed DBIDs for a given <see cref="eDataTypes">DataType</see>.
    ''' </summary>
    ''' <param name="dataType">The <see cref="eDataTypes">DataType</see> to obtain the list for.</param>
    ''' <returns>An array representing DBIDs that are flagged as changed, and thus need saving.</returns>
    ''' <remarks>The EwECore uses 1-based arrays *shudder*.</remarks>
    ''' -------------------------------------------------------------------
    Private Function GetChangedIDArray(ByVal dataType As eDataTypes) As Integer()

        Dim al As List(Of Integer) = Nothing
        Dim aIDs() As Integer = Nothing

        If (Me.m_dictChangedDBIDs.ContainsKey(dataType)) Then
            al = Me.m_dictChangedDBIDs(dataType)
            aIDs = al.ToArray()
        End If

        Return aIDs
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, states whether there are pending changes for a particular
    ''' datatype and optional database ID.
    ''' </summary>
    ''' <param name="dataType">The <see cref="eDataTypes">data type</see> to test
    ''' for changes.</param>
    ''' <param name="iDBID">The optional database ID to test for changes.</param>
    ''' <returns>True if there are pending changes for the given datatype
    ''' and optional database ID.</returns>
    ''' -------------------------------------------------------------------
    Private Function IsChanged(ByVal dataType As eDataTypes, Optional ByVal iDBID As Integer = 0) As Boolean
        Dim bChanged As Boolean = False

        If (Me.m_dictChangedDBIDs.ContainsKey(dataType)) Then
            If iDBID = 0 Then
                bChanged = True
            Else
                Dim lInt As List(Of Integer) = Me.m_dictChangedDBIDs(dataType)
                bChanged = (lInt.IndexOf(iDBID) <> -1)
            End If
        End If

        Return bChanged
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, clears all changed information for either a given
    ''' data type or for the entire datasource.
    ''' </summary>
    ''' <param name="dataType">Datatype to clear the changed adminsitration
    ''' for. If not specified, the entire changed administration will be 
    ''' cleared.</param>
    ''' -------------------------------------------------------------------
    Private Sub ClearChanged(Optional ByVal dataType As eDataTypes = eDataTypes.NotSet)
        If (dataType = eDataTypes.NotSet) Then
            Me.m_dictChangedDBIDs.Clear()
        Else
            If (Me.m_dictChangedDBIDs.ContainsKey(dataType)) Then
                Me.m_dictChangedDBIDs.Remove(dataType)
            End If
        End If
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method; states whether there are pending changes for a particular
    ''' <see cref="eMessageSource">EwE component</see>.
    ''' </summary>
    ''' <param name="component">The EwE component to check.</param>
    ''' <returns>True if there are any pending changes for any datatype that
    ''' belongs to this EwE component.</returns>
    ''' <remarks>
    ''' The EwE component data type registration is kept locally in this datasource,
    ''' and is configured in <see cref="RegisterDataTypeComponents">RegisterDataTypeComponents</see>.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Private Function IsChanged(ByVal component As eMessageSource) As Boolean

        Dim bChanged As Boolean = False

        For Each dt As eDataTypes In Me.m_dictDataTypeComponents.Keys
            If Me.m_dictDataTypeComponents(dt) = component Then
                If Me.IsChanged(dt) Then Return True
            End If
        Next
        Return False

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Clears the changed administration for all datatypes that belong to
    ''' a given <see cref="eMessageSource">EwE component</see>.
    ''' </summary>
    ''' <param name="component">The EwE component to clear the changed
    ''' adminsitration for.</param>
    ''' <remarks>
    ''' The EwE component data type registration is kept locally in this datasource,
    ''' and is configured in <see cref="RegisterDataTypeComponents">RegisterDataTypeComponents</see>.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Private Sub ClearChanged(ByVal component As eMessageSource)

        For Each dt As eDataTypes In Me.m_dictDataTypeComponents.Keys
            If Me.m_dictDataTypeComponents(dt) = component Then
                Me.ClearChanged(dt)
            End If
        Next

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
                Debug.Assert(Not d.ContainsKey(iIDOrg), String.Format("cIDMappings: DBID {0} already mapped", iIDOrg))

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
            Me.m_core.m_publisher.AddMessage(New cMessage(strMessage, msgType, eMessageSource.DataSource, msgImportance))
        End If
        Console.WriteLine(strMessage)

    End Sub

#End Region ' Messages

#Region " Generic datasource "

#Region " Diagnostics "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States whether the datasource has unsaved changes that do not relate
    ''' to any of the supported sub-models.
    ''' </summary>
    ''' <returns>True if the datasource has pending changes.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsModified() As Boolean Implements DataSources.IEwEDataSource.IsModified
        If Not Me.IsConnected() Then Return False
        Return Me.IsChanged(eMessageSource.DataSource)
    End Function

#End Region ' Diagnostics

#End Region

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
        bSucces = bSucces And Me.LoadGroupInfo()
        bSucces = bSucces And Me.LoadFleetInfo()
        bSucces = bSucces And Me.LoadAuxillaryData()

        ecopathDS.bInitialized = bSucces

        ecopathDS.onPostInitialization()

        bSucces = bSucces And Me.LoadEcosimScenarioDefinitions()
        bSucces = bSucces And Me.LoadEcospaceScenarioDefinitions()
        bSucces = bSucces And Me.LoadEcotracerScenarioDefinitions()

        ' Clear changed admin
        Me.ClearChanged()

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
        bSucces = bSucces And Me.SaveGroupInfo()
        bSucces = bSucces And Me.SaveFleetInfo()
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
            ' #Yes: Clear changed flags
            Me.ClearChanged(eMessageSource.EcoPath)
            Me.ClearChanged(eMessageSource.DataSource)
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
            Debug.Assert(False, "Failed to access table EopathModel")
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
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try
            Me.m_db.Execute("DELETE * FROM EcopathModel")
            writer = Me.m_db.GetWriter("EcopathModel")

            drow = writer.NewRow()
            drow("ModelID") = Me.m_core.m_EwEModelDBID
            drow("Name") = Me.m_core.m_EwEModelName
            drow("Description") = Me.m_core.m_EwEModelDescription
            drow("Author") = Me.m_core.m_EwEModelAuthor
            drow("Contact") = Me.m_core.m_EwEModelContact
            drow("Area") = Me.m_core.m_EwEModelArea
            drow("NumDigits") = Me.m_core.m_EwEModelNumDigits
            drow("UnitCurrency") = Me.m_core.m_EwEModelUnitCurrency
            drow("UnitCurrencyCustom") = Me.m_core.m_EwEModelUnitCurrencyCustom
            drow("UnitTime") = Me.m_core.m_EwEModelUnitTime
            drow("UnitTimeCustom") = Me.m_core.m_EwEModelUnitTimeCustom
            drow("UnitMonetary") = Me.m_core.m_EwEModelUnitMonetary
            'drow("UnitMonetaryCustom") = Me.m_core.m_EwEModelUnitMonetaryCustom
            drow("LastSaved") = cDBDataSource.GetJulianDate()
            writer.AddRow(drow)

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
        Dim reader As IDataReader = Me.m_db.GetReader("SELECT * FROM PedigreeLevel ORDER BY Sequence ASC")
        Dim iLevel As Integer = 1
        Dim bSucces As Boolean = True

        ' Init data structure
        ecopathDS.NumPedigreeLevels = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM PedigreeLevel"))

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
            writer = Me.m_db.GetWriter("PedigreeLevel")
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
                iDBID = CInt(Me.m_db.GetValue("SELECT MAX(LevelID) FROM PedigreeLevel")) + 1
            Catch
                iDBID = 1
            End Try

            ' Start writing, protect sequence
            writer = Me.m_db.GetWriter("PedigreeLevel", "Sequence")

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
            Me.m_db.Execute(String.Format("UPDATE PedigreeLevel SET Sequence={1} WHERE (LevelID={0})", iDBID, iPosition))
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
            Me.m_db.Execute(String.Format("DELETE FROM PedigreeLevel WHERE (LevelID={0})", iDBID))
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing PedigreeLevel {1}", ex.Message, iDBID))
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Modify

#End Region ' Pedigree

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

        ' Set all group vbK values to -1
        For iGroup = 1 To ecopathDS.NumGroups
            ecopathDS.vbKInput(iGroup) = -1.0!
        Next

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
                            ecopathDS.vbKInput(iGroup) = CSng(Me.ReadSafe(rdLifeStage, "vbK", 0.3!))

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
        Dim drow As DataRow = Nothing
        Dim iGroupID As Integer = 0
        Dim iGroup As Integer = 1

        Try
            Me.m_db.Execute("DELETE * FROM Stanza")

            writer = Me.m_db.GetWriter("Stanza")
            For iStanza As Integer = 1 To stanzaDS.Nsplit

                ' Sanity check: has life stages?
                If (stanzaDS.Nstanza(iStanza) > 0) Then
                    drow = writer.NewRow()
                    drow("StanzaID") = stanzaDS.StanzaDBID(iStanza)
                    drow("StanzaName") = stanzaDS.StanzaName(iStanza)
                    drow("RecPower") = stanzaDS.RecPowerSplit(iStanza)
                    drow("BabSplit") = stanzaDS.BABsplit(iStanza)
                    drow("WMatWinf") = stanzaDS.WmatWinf(iStanza)
                    drow("FixedFecundity") = stanzaDS.FixedFecundity(iStanza)
                    ' JS 23apr07: Leading B and QB groups are calculated at runtime, no longer stored in DB
                    writer.AddRow(drow)
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
                        drow("vbK") = ecopathDS.vbKInput(iGroup)
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
                drow("vbK") = 0.3
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
            Me.m_db.Execute(String.Format("DELETE * FROM Stanza WHERE (StanzaID={0})", iDBID))
            Return Me.LoadGroupInfo()
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
    Public Function AddStanzaLifestage(ByVal iStanzaDBID As Integer, ByVal iGroupDBID As Integer, ByVal iStartAge As Integer, ByVal sMortality As Single, ByVal sVBK As Single) As Boolean _
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
            drow("vbK") = sVBK
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
            Me.m_db.Execute(String.Format("DELETE * FROM StanzaLifeStage WHERE (StanzaID={0}) AND (GroupID={1})", iStanzaDBID, iGroupDBID))
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
        Return Me.IsChanged(eMessageSource.EcoPath)

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
    Private Function LoadGroupInfo() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim reader As IDataReader = Me.m_db.GetReader("SELECT * FROM EcopathGroup ORDER BY Sequence ASC")
        Dim iGroup As Integer = 1
        Dim sTemp As Single = 0.0
        Dim strTemp As String = ""
        Dim bSucces As Boolean = True

        ' Init data structure
        ecopathDS.NumGroups = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM EcopathGroup"))
        ecopathDS.NumLiving = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM EcopathGroup WHERE (TYPE <= 1)"))
        ecopathDS.NumDetrit = ecopathDS.NumGroups - ecopathDS.NumLiving

        ' Allocate space
        If (Not ecopathDS.redimGroupVariables()) Then
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

                'variables with input output pairs
                ecopathDS.EEinput(iGroup) = CSng(reader("EcoEfficiency"))
                ecopathDS.PBinput(iGroup) = CSng(reader("ProdBiom"))
                ecopathDS.QBinput(iGroup) = CSng(reader("ConsBiom"))
                ecopathDS.GEinput(iGroup) = CSng(reader("ProdCons"))
                ecopathDS.Binput(iGroup) = CSng(reader("Biomass"))
                ecopathDS.BHinput(iGroup) = ecopathDS.Binput(iGroup) / ecopathDS.Area(iGroup)
                ecopathDS.vbKInput(iGroup) = -1

                ecopathDS.GroupColor(iGroup) = Integer.Parse(CStr(reader("PoolColor")), Globalization.NumberStyles.HexNumber)

                '' Read overriding values, if any
                'If CSng(reader("AinLW")) > 0 Then
                '    ' ?? = CSng(reader("AinLW"))
                '    ' ?.GrowthInput(nGroup, eGrowthInput.AinLW) = true
                'End If
                'If CSng(reader("BinLW")) > 0 Then
                '    ' ?? = CSng(reader("BinLW"))
                '    ' ?.GrowthInput(nGroup, eGrowthInput.BinLW) = true
                'End If
                'If CSng(reader("Loo")) > 0 Then
                '    ' ?? = CSng(reader("Loo"))
                '    ' ?.GrowthInput(nGroup, eGrowthInput.Loo) = true
                'End If
                'If CSng(reader("winf")) > 0 Then
                '    ' ?? = CSng(reader("winf"))
                '    ' ?.GrowthInput(nGroup, eGrowthInput.winf) = true
                'End If
                'If CSng(reader("t0")) > 0 Then
                '    ' ?? = CSng(reader("t0"))
                '    ' ?.GrowthInput(nGroup, eGrowthInput.t0) = true
                'End If
                'If CSng(reader("Tcatch")) > 0 Then
                '    ' ?? = CSng(reader("Tcatch"))
                '    ' ?.GrowthInput(nGroup, eGrowthInput.Tcatch) = true
                'Else
                '    ' ?.Tcatch = 0
                'End If
                'If CSng(reader("Tmax")) > 0 Then
                '    ' ?? = CSng(reader("Tmax"))
                '    ' ?.GrowthInput(nGroup, eGrowthInput.Tmax) = true
                'End If

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading group {1}", ex.Message, ecopathDS.GroupName(iGroup)))
                bSucces = False
            End Try

            iGroup += 1

        End While

        Debug.Assert(iGroup - 1 = ecopathDS.NumGroups)

        Me.m_db.ReleaseReader(reader)
        reader = Nothing

        bSucces = bSucces And Me.LoadDietComp()
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
    Private Function SaveGroupInfo() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
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
                'drow("vbK") = ecopathDS.vbKInput(iGroup)
                drow("PoolColor") = String.Format("{0:x8}", ecopathDS.GroupColor(iGroup))

                ' Write overriding values, if any
                ' drow("AinLW") = CSng(IIf(?.GrowthInput(nGroup, eGrowthInput.AinLW), ?.AinLW, -1.0)
                ' drow("BinLW") = CSng(IIf(?.GrowthInput(nGroup, eGrowthInput.BinLW), ?.BinLW, -1.0)
                ' drow("Loo") = CSng(IIf(?.GrowthInput(nGroup, eGrowthInput.Loo), ?.Loo, -1.0)
                ' drow("Winf") = CSng(IIf(?.GrowthInput(nGroup, eGrowthInput.Winf), ?.Winf, -1.0)
                ' drow("t0") = CSng(IIf(?.GrowthInput(nGroup, eGrowthInput.t0), ?.t0, -1.0)
                ' drow("Tcatch") = CSng(IIf(?.GrowthInput(nGroup, eGrowthInput.TCatch), ?.TCatch, -1.0)
                ' drow("Tmax") = CSng(IIf(?.GrowthInput(nGroup, eGrowthInput.Tmax), ?.Tmax, -1.0)

                drow.EndEdit()

            Next iGroup

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving EcopathGroup", ex.Message))
            bSucces = False
        End Try

        ' Save changes
        Me.m_db.ReleaseWriter(writer, True)

        bSucces = bSucces And Me.SaveDietComp()
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
    ''' <param name="sPP">The Type of the new group; 0=consumer, 1=producer, 2=detritus, or a cons/prod ratio.</param>
    ''' <param name="iPosition">The position of the new group in the group sequence.</param>
    ''' <param name="iDBID">Database ID assigned to the new Group.</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' Note that this will not adjust the data arrays. Due to the complex organization of the
    ''' core a full data reload is required after a group is created.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Public Function AddGroup(ByVal strGroupName As String, ByVal sPP As Single, ByVal iPosition As Integer, ByRef iDBID As Integer) As Boolean _
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
        bSucces = bSucces And Me.AddEcospaceGroupToAllScenarios(iDBID)
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
            Me.m_db.Execute(String.Format("DELETE FROM EcopathGroup WHERE (GroupID={0})", iDBID))
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while removing group {1}", ex.Message, iDBID))
            bSucces = False
        End Try

        ' Cascading deletion will delete corresponding groups from Ecosim

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
    Private Function LoadDietComp() As Boolean

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
    Private Function SaveDietComp() As Boolean

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
    Private Function LoadFleetInfo() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim bSucces As Boolean = True

        ecopathDS.NoGearData = Not IsFishing()

        ecopathDS.NumFleet = CInt(Me.m_db.GetValue("SELECT COUNT(*) FROM EcopathFleet"))

        ' This will be necessary when reading Gear tables. Can only call this after groups are read.
        If Not ecopathDS.RedimFleetVariables(True) Then
            Return False
        End If

        bSucces = LoadFleets()
        bSucces = bSucces And LoadCatch()
        bSucces = bSucces And LoadDiscardFate()

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads all fleets.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function LoadFleets() As Boolean

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
                ecopathDS.Epower(iFleet) = CSng(reader("Epower"))
                ecopathDS.PcapBase(iFleet) = CSng(reader("PCapBase"))
                ecopathDS.CapDepreciate(iFleet) = CSng(reader("CapDepreciate"))
                ecopathDS.CapBaseGrowth(iFleet) = CSng(reader("CapBaseGrowth"))
                'ecopathDS.FleetColor(iFleet) = Integer.Parse(CStr(reader("PoolColor")), Globalization.NumberStyles.HexNumber)
                iFleet += 1

            End While

            Me.m_db.ReleaseReader(reader)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EcopathFleet {1}", ex.Message, iFleet))
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function LoadCatch() As Boolean

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

    Private Function LoadDiscardFate() As Boolean

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
                    Else
                        Me.LogMessage(String.Format("DiscardFate value read for invalid iGroup {0}, should be at least {1}", iGroup, ecopathDS.NumLiving), eMessageType.Any, eMessageImportance.Warning)
                        ' Keep on chugging, do not make assignment
                        bSucces = True
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
    Private Function SaveFleetInfo() As Boolean

        Dim bSucces As Boolean = True

        bSucces = SaveFleets()
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
    Private Function SaveFleets() As Boolean

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
                drow("Epower") = ecopathDS.Epower(iFleet)
                drow("PCapBase") = ecopathDS.PcapBase(iFleet)
                drow("CapDepreciate") = ecopathDS.CapDepreciate(iFleet)
                drow("CapBaseGrowth") = ecopathDS.CapBaseGrowth(iFleet)
                'drow("PoolColor") = String.Format("{0:x8}", ecopathDS.FleetColor(iFleet))

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
                       ((ecopathDS.Market(iFleet, iGroup) > 0.0!) And (ecopathDS.Market(iFleet, iGroup) < 1.0!)) Then

                        drow = writer.NewRow()
                        drow("FleetID") = ecopathDS.FleetDBID(iFleet)
                        drow("GroupID") = ecopathDS.GroupDBID(iGroup)
                        drow("Landing") = ecopathDS.Landing(iFleet, iGroup)
                        drow("Discards") = ecopathDS.Discard(iFleet, iGroup)
                        drow("Price") = ecopathDS.Market(iFleet, iGroup)
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
            writer.AddRow(drow)
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while appending fleet {1}", ex.Message, strFleetName))
            bSucces = False
        End Try

        ' Add Catch
        bSucces = bSucces And Me.AddCatchDataForFleet(iDBID)
        ' Create ecosim fleet forcing bits

        ' Create ecospace fleet objects though
        bSucces = bSucces And Me.AddEcospaceFleetToAllScenarios(iDBID)

        Return bSucces

    End Function

    Private Function AddCatchDataForFleet(ByVal iFleetID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim iGroupID As Integer = 0
        Dim bSucces As Boolean = True

        For iGroup As Integer = 1 To ecopathDS.NumGroups
            iGroupID = ecopathDS.GroupDBID(iGroup)
            bSucces = bSucces And Me.AddCatch(iGroupID, iFleetID)

            If iGroup > ecopathDS.NumLiving Then
                bSucces = bSucces And Me.AddDiscardFate(iGroupID, iFleetID)
            End If
        Next
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
            Me.m_db.Execute(String.Format("DELETE FROM EcopathFleet WHERE (FleetID={0})", iDBID))
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

        Return Me.IsChanged(eMessageSource.EcoSim)

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
            ecosimDS.UseVarPQ = CBool(reader("UseVarPQ"))

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
        bSucces = bSucces And Me.LoadShapes()
        bSucces = bSucces And Me.LoadTimeSeriesDatasets()

        Return bSucces
    End Function

    Friend Function SaveEcosimScenarioAs(ByVal strScenarioName As String, ByVal strDescription As String, _
      ByVal strAuthor As String, ByVal strContact As String, ByRef iScenarioID As Integer) As Boolean _
             Implements IEcosimDatasource.SaveEcosimScenarioAs

        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        ' Delete existing scenario
        Me.m_db.Execute(String.Format("DELETE * FROM EcosimScenario WHERE ScenarioName='{0}'", strScenarioName))

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
            drow("NutPBmax") = ecosimDS.NutPBmax
            drow("UseVarPQ") = ecosimDS.UseVarPQ
            drow("LastSaved") = cDBDataSource.GetJulianDate()

            ' Save changes
            Me.m_db.ReleaseWriter(writer)

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while saving Scenario {1}", ex.Message, iScenarioID))
            bSucces = False
        End Try

        bSucces = bSucces And Me.SaveEcosimGroups(idm)
        bSucces = bSucces And Me.SaveEcosimFleets(idm)
        bSucces = bSucces And Me.SaveShapes(idm)
        bSucces = bSucces And Me.SaveTimeSeries(idm)

        If bSucces Then
            bSucces = Me.m_db.CommitTransaction(True)
        Else
            Me.m_db.RollbackTransaction()
        End If

        ' Reload ecosim scenario definitions to update lastsaved data
        Me.LoadEcosimScenarioDefinitions()

        Me.ClearChanged(eMessageSource.EcoSim)

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
    ''' <param name="iDBID">Database ID assigned to the new scenario.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function AppendEcosimScenario(ByVal strScenarioName As String, ByVal strDescription As String, _
            ByVal strAuthor As String, ByVal strContact As String, ByRef iDBID As Integer) As Boolean _
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
                iDBID = CInt(Me.m_db.GetValue("SELECT MAX(ScenarioID) FROM EcosimScenario")) + 1
            Catch ex As InvalidCastException
                iDBID = 1
            End Try

            writer = Me.m_db.GetWriter("EcosimScenario")

            drow = writer.NewRow()
            drow("ScenarioID") = iDBID
            drow("ScenarioName") = strScenarioName
            drow("Description") = strDescription
            drow("Author") = strAuthor
            drow("Contact") = strContact
            drow("LastSaved") = cDBDataSource.GetJulianDate()
            writer.AddRow(drow)

            Me.m_db.ReleaseWriter(writer)

            ' Create ecosim groups for the new scenario
            For i As Integer = 1 To ecopathDS.GroupDBID.Length - 1
                bSucces = bSucces And Me.CreateRepairEcosimGroup(ecopathDS.GroupDBID(i), iDBID)
            Next

            ' Reload scenario definitions
            bSucces = bSucces And Me.LoadEcosimScenarioDefinitions()

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
            Me.m_db.Execute(String.Format("DELETE * FROM EcosimScenario WHERE (ScenarioID={0})", iDBID))
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

        Try
            readerGroup = Me.m_db.GetReader(String.Format("SELECT GroupID, FishMortShapeID FROM EcosimScenarioGroup WHERE (EcopathGroupID={0}) AND (ScenarioID={1})", iEcopathGroupID, iScenarioID))
            readerGroup.Read()
            ' Try to find existing Sim group ID
            iGroupID = CInt(readerGroup(0))
            ' Try to find existing Fish mort shape ID
            iFishMortShapeID = CInt(readerGroup(1))
            Me.m_db.ReleaseReader(readerGroup)

            ' It this did not fail we have found a group, whoot! whoot!
            bGroupFound = True
        Catch ex As Exception
            iGroupID = -1
            iFishMortShapeID = -1
            bGroupFound = False
        End Try

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

#Region " Load "

    Private Function LoadEcosimGroups(ByVal iScenarioID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True
        Dim i As Integer = 0

        For j As Integer = 1 To ecosimDS.nGroups

            ' Me.CreateRepairEcosimGroup(ecopathDS.GroupDBID(j), iScenarioID, True)

            reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcoSimScenarioGroup WHERE (ScenarioID={0}) AND (EcopathGroupID={1})", iScenarioID, ecopathDS.GroupDBID(j)))

            Try
                reader.Read()

                ' Find ecopath group index to store matching ecosim group data at
                i = Array.IndexOf(ecopathDS.GroupDBID, CInt(reader("EcopathGroupID")))

                ' Debug.Assert(i = j)

                ' Read fields
                ecosimDS.GroupDBID(i) = CInt(reader("GroupID"))
                ecosimDS.PBmaxs(i) = CSng(reader("pbmaxs"))
                ecosimDS.FtimeMax(i) = CSng(reader("FtimeMax"))
                ecosimDS.FtimeAdjust(i) = CSng(reader("FtimeAdjust"))
                ecosimDS.MoPred(i) = CSng(reader("MoPred"))
                ecosimDS.FishRateMax(i) = CSng(reader("FishRateMax"))
                ' ecosimDS.ShowGroup(i) = CBool(reader("Show"))
                ecosimDS.FLimit(i) = CSng(Me.ReadSafe(reader, "FishMortMax", 1000.0!))

                ecosimDS.RiskTime(i) = CSng(reader("RiskTime"))
                ecosimDS.QmQo(i) = CSng(reader("QmQo"))
                ecosimDS.CmCo(i) = CSng(reader("CmCo"))
                ecosimDS.SwitchPower(i) = CSng(reader("SwitchPower"))
                ecosimDS.GroupFishRateNoDBID(i) = CInt(reader("FishMortShapeID"))
                ecosimDS.SalOpt(i) = CSng(Me.ReadSafe(reader, "SalOpt", 35))
                ecosimDS.SdSalLeft(i) = CSng(Me.ReadSafe(reader, "SdSalLeft", 1000.0!))
                ecosimDS.SdSalRight(i) = CSng(Me.ReadSafe(reader, "SdSalRight", 1000.0!))

                bSucces = bSucces And Me.LoadFishMortShape(CInt(reader("FishMortShapeID")), i)

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading EcoSim group info for group {1}", ex.Message, i))
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
        Dim reader As IDataReader = Nothing
        Dim iFleetID As Integer = -1
        Dim iShapeID As Integer = -1
        Dim bSucces As Boolean = True
        Dim asDummy(ecosimDS.NTimes) As Single

        ' For each fleet
        For iFleet As Integer = 1 To ecosimDS.nGear
            Try
                ' Read shape for this fleet
                iFleetID = ecopathDS.FleetDBID(iFleet)
                reader = Me.m_db.GetReader(String.Format("SELECT FishRateShapeID FROM EcoSimScenarioFleet WHERE (ScenarioID={0}) AND (EcopathFleetID={1})", iScenarioID, iFleetID))
                reader.Read()
                iShapeID = CInt(Me.ReadSafe(reader, "FishRateShapeID", -1))
            Catch ex As Exception
                ' A different error occurred: abort!
                bSucces = False
            End Try

            If iShapeID <= 0 Then
                Me.AppendShapeImpl(ecopathDS.FleetName(iFleet), eDataTypes.FishingRate, iShapeID, asDummy, 0, 0, 0, 0, eShapeFunctionType.NotSet)
            End If

            If iShapeID > -1 Then
                ' JS 10Aug07: Don't fail in case FishRateShape is missing. Only those present are loaded, only those loaded are saved.
                '             Since these shapes do not need to be present we can be somewhat forgiving in this particular case.
                If Not LoadFishingRateShape(iShapeID, iFleet) Then
                    Me.LogMessage(String.Format("Warning: Fishing rate shape {0} is referenced but not present in database for EcoSim fleet {1} (ID {2})", iShapeID, iFleet, iFleetID))
                End If
            End If

            Me.m_db.ReleaseReader(reader)

        Next
        Return bSucces

    End Function

#End Region ' Load

#Region " Save "

    Private Function SaveEcosimGroups(ByRef idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim iScenarioID As Integer = 0
        Dim iGroupID As Integer = 0
        Dim bSucces As Boolean = True
        Dim objKeys() As Object = {Nothing, Nothing}

        ' Obtain mapped scenario ID
        iScenarioID = idm.GetID(eDataTypes.EcoSimScenario, ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario))

        ' JS 10aug07: First try to repair Ecosim groups
        For i As Integer = 1 To ecosimDS.nGroups
            Me.CreateRepairEcosimGroup(ecopathDS.GroupDBID(i), iScenarioID)
        Next i

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
                Debug.Assert(drow IsNot Nothing, String.Format("Cannot find ecosim group {0} (path group {1}) for scenario {2}", ecosimDS.GroupDBID(i), ecopathDS.GroupDBID(i), iScenarioID))

                ' Store ecosim group ID mapping now we know it
                idm.Add(eDataTypes.EcoSimGroupInput, ecosimDS.GroupDBID(i), CInt(drow("GroupID")))

                drow.BeginEdit()
                drow("pbmaxs") = ecosimDS.PBmaxs(i)
                drow("FtimeMax") = ecosimDS.FtimeMax(i)
                drow("FtimeAdjust") = ecosimDS.FtimeAdjust(i)
                drow("MoPred") = ecosimDS.MoPred(i)
                drow("FishRateMax") = ecosimDS.FishRateMax(i)
                drow("FishMortMax") = ecosimDS.FLimit(i)
                ' drow("Show") = ecosimDS.ShowGroup(i)
                drow("RiskTime") = ecosimDS.RiskTime(i)
                drow("QmQo") = ecosimDS.QmQo(i)
                drow("CmCo") = ecosimDS.CmCo(i)
                drow("SwitchPower") = ecosimDS.SwitchPower(i)
                drow("FishMortShapeID") = ecosimDS.GroupFishRateNoDBID(i)
                drow("SalOpt") = ecosimDS.SalOpt(i)
                drow("SdSalLeft") = ecosimDS.SdSalLeft(i)
                drow("SdSalRight") = ecosimDS.SdSalRight(i)

                drow.EndEdit()

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
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim bNewRow As Boolean = False
        Dim bSucces As Boolean = True
        Dim objKeys() As Object = {Nothing, Nothing}

        objKeys(0) = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)

        Try
            writer = Me.m_db.GetWriter("EcosimScenarioFleet")
            dt = writer.GetDataTable()
            For i As Integer = 1 To ecopathDS.NumFleet

                objKeys(1) = ecopathDS.FleetDBID(i)
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

                ' Write dynamic bit
                drow("FishRateShapeID") = ecosimDS.FishRateGearDBID(i)

                ' Wrap up: was this a new row?
                If bNewRow Then
                    ' #Yes: add it to the writer
                    writer.AddRow(drow)
                Else
                    ' #No: done editing
                    drow.EndEdit()
                End If
            Next i
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

                    Case eDataTypes.FishingRate
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
            reader = Me.m_db.GetReader(String.Format("SELECT NutForcingShapeID, SalinityForcingShapeID FROM EcosimScenario WHERE (ScenarioID={0})", iScenarioID))
            reader.Read()
            iForcingShape = CInt(Me.ReadSafe(reader, "NutForcingShapeID", 0))
            ecosimDS.NutForceNumber = Math.Max(0, Array.IndexOf(ecosimDS.ForcingDBIDs, iForcingShape))
            iForcingShape = CInt(Me.ReadSafe(reader, "SalinityForcingShapeID", 0))
            ecosimDS.SalinityForceNo = Math.Max(0, Array.IndexOf(ecosimDS.ForcingDBIDs, iForcingShape))
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
                If String.IsNullOrEmpty(astrZScale(ipt - 1)) Then
                    ecosimDS.zscale(ipt, iForcingShape) = 0
                Else
                    ecosimDS.zscale(ipt, iForcingShape) = CSng(astrZScale(ipt - 1))
                End If
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

    Private Function LoadTimeShape(ByVal iShapeID As Integer, ByVal iForcingShape As Integer, _
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
            ' sp.ZScale = CInt(readerShape("ZScale"))
            shapeParms.ShapeFunctionType = CType(readerShape("FunctionType"), eShapeFunctionType)

            ' Read z-scale
            astrZScale = Me.SplitNumberString(CStr(readerShape("Zscale")))
            For ipt As Integer = 1 To Math.Min(ecosimDS.ForcePoints, astrZScale.Length)
                If String.IsNullOrEmpty(astrZScale(ipt - 1)) Then
                    ecosimDS.zscale(ipt, iForcingShape) = 0
                Else
                    ecosimDS.zscale(ipt, iForcingShape) = CSng(astrZScale(ipt - 1))
                End If
            Next ipt
            For ipt As Integer = Math.Min(ecosimDS.ForcePoints, astrZScale.Length) + 1 To ecosimDS.ForcePoints
                ecosimDS.zscale(ipt, iForcingShape) = 1.0
            Next

            ecosimDS.ForcingShapeParams(iForcingShape) = shapeParms
            ecosimDS.ForcingDBIDs(iForcingShape) = iShapeID
            ecosimDS.ForcingTitles(iForcingShape) = CStr(readerShape("Title"))
            ecosimDS.ForcingShapeType(iForcingShape) = eDataTypes.Forcing
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
                If String.IsNullOrEmpty(astrZScale(ipt - 1)) Then
                    ecosimDS.Medpoints(ipt, iMediationShape) = 0
                Else
                    ecosimDS.Medpoints(ipt, iMediationShape) = CSng(astrZScale(ipt - 1))
                End If
            Next ipt
            For ipt As Integer = Math.Min(ecosimDS.NMedPoints, astrZScale.Length) + 1 To ecosimDS.NMedPoints
                ecosimDS.Medpoints(ipt, iMediationShape) = 1.0
            Next

            ecosimDS.MediationShapeParams(iMediationShape) = shapeParms
            ecosimDS.MediationDBIDs(iMediationShape) = iShapeID
            ecosimDS.MediationTitles(iMediationShape) = CStr(readerShape("Title"))
            ecosimDS.MedXbase(iMediationShape) = CSng(readerShape("XBaseLine"))

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

                ecosimDS.VulMult(iPredator, iPrey) = CSng(reader("vulnerability"))
                ' JS 060627: Removed from DB since FlowType is fixed to 2.0 in the model logic.
                ' ecosimDS.FlowType(iPredator, iPrey) = CSng(2.0) 'Following CJW per 160700

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
                iFNo(iPredator, iPrey) += 1
                ' Resolve shape ID
                iShapeID = CInt(reader("ShapeID"))
                ' Determine shape type
                iShape = Array.IndexOf(ecosimDS.MediationDBIDs, iShapeID)
                ' Is a mediation shape?
                If iShape <> -1 Then
                    ' #Yes: flag as mediation shape
                    ecosimDS.IsMedFunction(iPredator, iPrey, iFNo(iPredator, iPrey)) = True
                Else
                    ' #No: flag as other shape
                    ecosimDS.IsMedFunction(iPredator, iPrey, iFNo(iPredator, iPrey)) = False
                    ' Obtain forcing index
                    iShape = Array.IndexOf(ecosimDS.ForcingDBIDs, iShapeID)
                End If
                ' Update sim fields
                ecosimDS.FunctionNumber(iPredator, iPrey, iFNo(iPredator, iPrey)) = iShape
                ecosimDS.FunctionType(iPredator, iPrey, iFNo(iPredator, iPrey)) = CType(reader("FunctionType"), eForcingFunctionApplication)
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
                If String.IsNullOrEmpty(astrMemoBits(j - 1)) Then
                    ecosimDS.FishRateGear(iFishingRateShape, j) = 0
                Else
                    ecosimDS.FishRateGear(iFishingRateShape, j) = Single.Parse(astrMemoBits(j - 1))
                End If
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
                    If String.IsNullOrEmpty(astrMemoBits(j - 1)) Then
                        ecosimDS.FishRateNo(iForcingShape, j) = 0
                    Else
                        ecosimDS.FishRateNo(iForcingShape, j) = Single.Parse(astrMemoBits(j - 1))
                    End If
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

#End Region ' Shape load helpers

    Private Function SaveShapes(ByVal idm As cIDMappings) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim dt As DataTable = Nothing
        Dim iShape As Integer = 0
        Dim adrows() As DataRow = Nothing
        Dim drow As DataRow = Nothing
        Dim iShapeID As Integer = 0
        Dim bSucces As Boolean = True

        Try
            ' Start writing
            writer = Me.m_db.GetWriter("EcoSimShape")
            dt = writer.GetDataTable()

            For iShape = 1 To ecosimDS.ForcingShapes
                ' JS 10aug07: this should be an assert
                If (ecosimDS.ForcingDBIDs(iShape) > 0) Then
                    Select Case ecosimDS.ForcingShapeType(iShape)
                        Case eDataTypes.EggProd
                            adrows = dt.Select(String.Format("ShapeID={0}", ecosimDS.ForcingDBIDs(iShape)))
                        Case eDataTypes.Forcing
                            adrows = dt.Select(String.Format("ShapeID={0}", ecosimDS.ForcingDBIDs(iShape)))
                        Case Else
                            Debug.Assert(False)
                    End Select
                    If adrows.Length = 1 Then
                        drow = adrows(0)
                    Else
                        drow = writer.NewRow()
                        drow("ShapeID") = ecosimDS.ForcingDBIDs(iShape)
                    End If
                    drow("ShapeType") = ecosimDS.ForcingShapeType(iShape)
                    drow("IsSeasonal") = ecosimDS.isSeasonal(iShape)
                    If adrows.Length = 1 Then
                        drow.EndEdit()
                    Else
                        writer.AddRow(drow)
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
                    iShapeID = ecosimDS.MediationDBIDs(iShape)
                    adrows = dt.Select(String.Format("ShapeID={0}", iShapeID))
                    If adrows.Length = 1 Then
                        drow = adrows(0)
                    Else
                        drow = writer.NewRow()
                        drow("ShapeID") = iShapeID
                    End If
                    drow("ShapeType") = eDataTypes.Mediation
                    If adrows.Length = 1 Then
                        drow.EndEdit()
                    Else
                        writer.AddRow(drow)
                    End If
                    writer.Commit()
                    bSucces = bSucces And SaveMediationShape(iShape)
                End If
            Next iShape

            For iShape = 1 To ecosimDS.FishRateGearDBID.Length - 1
                If (ecosimDS.FishRateGearDBID(iShape) > 0) Then
                    iShapeID = ecosimDS.FishRateGearDBID(iShape)
                    adrows = dt.Select(String.Format("ShapeID={0}", iShapeID))
                    If adrows.Length = 1 Then
                        drow = adrows(0)
                    Else
                        drow = writer.NewRow()
                        drow("ShapeID") = iShapeID
                    End If
                    drow("ShapeType") = eDataTypes.FishingRate
                    If adrows.Length = 1 Then
                        drow.EndEdit()
                    Else
                        writer.AddRow(drow)
                        writer.Commit()
                    End If
                    bSucces = bSucces And Me.SaveFishingRateShape(iShape)
                End If
            Next iShape

            For iShape = 1 To ecosimDS.FishRateNoDBID.Length - 1
                If (ecosimDS.FishRateNoDBID(iShape) > 0) Then
                    iShapeID = ecosimDS.FishRateNoDBID(iShape)
                    adrows = dt.Select(String.Format("ShapeID={0}", iShapeID))
                    If adrows.Length = 1 Then
                        drow = adrows(0)
                    Else
                        drow = writer.NewRow()
                        drow("ShapeID") = iShapeID
                    End If
                    drow("ShapeType") = eDataTypes.FishMort
                    If adrows.Length = 1 Then
                        drow.EndEdit()
                    Else
                        writer.AddRow(drow)
                        writer.Commit()
                    End If
                    bSucces = bSucces And Me.SaveFishMortShape(iShape)
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
        Debug.Assert(ecosimDS.ForcingShapeType(iShape) = eDataTypes.EggProd)

        Try
            writer = Me.m_db.GetWriter("EcosimShapeEggProd")
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
            ' Assemble Zscale
            For ipt As Integer = 1 To ecosimDS.ForcePoints
                If (ipt > 1) Then sbZScale.Append(" ")
                sbZScale.Append(ecosimDS.zscale(ipt, iShape))
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
            ' Assemble Zscale
            For ipt As Integer = 1 To ecosimDS.ForcePoints
                If (ipt > 1) Then sbZScale.Append(" ")
                sbZScale.Append(ecosimDS.zscale(ipt, iShape))
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
            drow("XBaseLine") = ecosimDS.MedXbase(iShape)
            drow("FunctionType") = CInt(shapeParms.ShapeFunctionType)
            ' Assemble Zscale
            For ipt As Integer = 1 To ecosimDS.NMedPoints
                If (ipt > 1) Then sbZScale.Append(" ")
                sbZScale.Append(ecosimDS.Medpoints(ipt, iShape))
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
            Me.m_db.Execute(String.Format("DELETE * FROM EcoSimScenarioForcingMatrix WHERE (ScenarioID={0})", iScenarioID))
            writer = Me.m_db.GetWriter("EcoSimScenarioForcingMatrix")

            For iPredator = 1 To ecosimDS.nGroups
                For iPrey = 1 To ecosimDS.nGroups

                    drow = writer.NewRow()
                    drow("PredID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecosimDS.GroupDBID(iPredator))
                    drow("PreyID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecosimDS.GroupDBID(iPrey))
                    drow("ScenarioID") = idm.GetID(eDataTypes.EcoSimScenario, iScenarioID)
                    drow("vulnerability") = ecosimDS.VulMult(iPredator, iPrey)
                    drow("flowtype") = ecosimDS.FlowType(iPredator, iPrey)
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
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim iShape As Integer = 0
        Dim bSucces As Boolean = True

        Try

            Me.m_db.Execute(String.Format("DELETE * FROM EcosimScenarioPredPreyShape WHERE (ScenarioID={0})", iScenarioID))
            writer = Me.m_db.GetWriter("EcosimScenarioPredPreyShape")

            For iPredator As Integer = 1 To ecosimDS.nGroups
                For iPrey As Integer = 1 To ecosimDS.nGroups
                    For iShapeNo As Integer = 1 To ecosimDS.MaxFunctions - 1

                        Try

                            ' Get shape assignment
                            iShape = ecosimDS.FunctionNumber(iPredator, iPrey, iShapeNo)
                            ' Is an assignment?
                            If (iShape > 0) Then
                                ' Save assignment
                                drow = writer.NewRow()
                                drow("PredID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecosimDS.GroupDBID(iPredator))
                                drow("PreyID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecosimDS.GroupDBID(iPrey))
                                drow("ScenarioID") = idm.GetID(eDataTypes.EcoSimScenario, iScenarioID)
                                If (ecosimDS.IsMedFunction(iPredator, iPrey, iShapeNo)) Then
                                    drow("ShapeID") = ecosimDS.MediationDBIDs(iShape)
                                Else
                                    drow("ShapeID") = ecosimDS.ForcingDBIDs(iShape)
                                End If
                                drow("FunctionType") = ecosimDS.FunctionType(iPredator, iPrey, iShapeNo)
                                writer.AddRow(drow)
                            End If
                        Catch ex As Exception
                            Debug.Assert(False, String.Format("Index error on {0}, {1}, {2}", iPredator, iPrey, iShape))
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
        Dim iScenarioID As Integer = idm.GetID(eDataTypes.EcoSimScenario, ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario))
        Dim writer As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Try

            Me.m_db.Execute(String.Format("DELETE * FROM EcosimScenarioshapeMedWeightsGroup WHERE (ScenarioID={0})", iScenarioID))
            writer = Me.m_db.GetWriter("EcosimScenarioshapeMedWeightsGroup")
            For iGroup As Integer = 1 To ecosimDS.nGroups
                For iShape As Integer = 1 To ecosimDS.MediationShapes
                    If ecosimDS.MedWeights(iGroup, iShape) > 0 Then
                        drow = writer.NewRow()
                        drow("ScenarioID") = iScenarioID
                        ' Ecosim groups unique per scenario: map this
                        drow("GroupID") = idm.GetID(eDataTypes.EcoSimGroupInput, ecosimDS.GroupDBID(iGroup))
                        drow("ShapeID") = ecosimDS.MediationDBIDs(iShape)
                        drow("MedWeights") = ecosimDS.MedWeights(iGroup, iShape)
                        writer.AddRow(drow)
                    End If
                Next iShape
            Next iGroup
            Me.m_db.ReleaseWriter(writer, True)

            Me.m_db.Execute(String.Format("DELETE * FROM EcosimScenarioShapeMedWeightsFleet WHERE (ScenarioID={0})", iScenarioID))
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
                    End If

                    ' HatchCodeShapeID identifies the egg prod shape assigned. Do not specify anything
                    ' to leave the field at DBNull
                    If (stanzaDS.HatchCode(iStanza) > 0) Then
                        drow("HatchCodeShapeID") = ecosimDS.ForcingDBIDs(CInt(stanzaDS.HatchCode(iStanza)))
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

    Private Function SaveFishingRateShape(ByVal iShape As Integer) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iDBID As Integer = ecosimDS.FishRateGearDBID(iShape)
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
                sbZScale.Append(ecosimDS.FishRateGear(iShape, ipt))
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

    Private Function SaveFishMortShape(ByVal iShape As Integer) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iDBID As Integer = ecosimDS.FishRateNoDBID(iShape)
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
                sbZScale.Append(ecosimDS.FishRateNo(iShape, ipt))
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

    Private Function AppendShapeImpl(ByVal strShapeName As String, ByVal shapeType As eDataTypes, ByRef iDBID As Integer, _
            ByVal asData As Single(), ByVal sYZero As Single, ByVal sYBase As Single, ByVal sYend As Single, ByVal sSteep As Single, ByVal functionType As eShapeFunctionType) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim writerID As cEwEDatabase.cEwEDbWriter = Me.m_db.GetWriter("EcoSimShape")
        Dim writerShape As cEwEDatabase.cEwEDbWriter = Nothing
        Dim drow As DataRow = Nothing
        Dim bSucces As Boolean = True

        Me.BeginTransaction()

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

                Case eDataTypes.FishingRate
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
                For ipt As Integer = 1 To Math.Min(ecosimDS.ForcePoints, asData.Length)
                    If (ipt > 1) Then sbZScale.Append(" ")
                    sbZScale.Append(asData(ipt))
                Next
                drow("zScale") = sbZScale.ToString()
            End If

            ' Specific bits
            Select Case shapeType
                Case eDataTypes.FishingRate
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

        Me.EndTransaction(bSucces)

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
            Me.m_db.Execute("DELETE * FROM EcoSimStanzaShape WHERE ((HatchCodeShapeID=NULL) AND (EggProdShapeID=NULL))")

            Me.m_db.Execute(String.Format("UPDATE EcoSimScenario Set SalinityForcingShapeID=NULL WHERE (SalinityForcingShapeID={0})", iDBID))
            Me.m_db.Execute(String.Format("UPDATE EcoSimScenario Set NutForcingShapeID=NULL WHERE (NutForcingShapeID={0})", iDBID))

            ' Destroy the given shape
            Me.m_db.Execute(String.Format("DELETE * FROM EcoSimShape WHERE (ShapeID={0})", iDBID))
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

#End Region ' Datasets

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

            ' Assemble Zscale
            For iYear As Integer = 0 To ts.XMax - 1
                If (iYear > 0) Then sbZScale.Append(" ")
                sbZScale.Append(ts.ShapeData(iYear))
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
            sbValues.Append(ts.ShapeData(iYear))
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

#Region " Datasets "

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
            Me.m_db.Execute(String.Format("DELETE * FROM EcosimTimeSeries WHERE (DatasetID={0})", iDatasetID))
            Me.m_db.Execute(String.Format("DELETE * FROM EcosimTimeSeriesDataset WHERE (DatasetID={0})", iDatasetID))
        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Datasets

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

        tsDS.nNumTimeSeries = 0
        tsDS.nMaxYears = 0
        tsDS.NdatType = 0
        tsDS.NdatYear = 0
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
        tsDS.RedimAppliedTimeSeries()

        If tsDS.nNumTimeSeries = 0 Then Return bSucces

        strSQL = String.Format("SELECT * FROM EcosimTimeSeries WHERE (DatasetID={0}) ORDER BY Sequence ASC", tsDS.iDatasetDBID(iDataset))
        reader = Me.m_db.GetReader(strSQL)
        Try
            While reader.Read()

                tsDS.iTimeSeriesDBID(iSeries) = CInt(reader("TimeSeriesID"))
                tsDS.strName(iSeries) = CStr(reader("DatName"))
                tsDS.iType(iSeries) = CInt(reader("DatType"))
                tsDS.sWeight(iSeries) = CSng(reader("WtType"))

                Select Case cTimeSeriesFactory.TimeSeriesCategory(CType(tsDS.iType(iSeries), eTimeSeriesType))

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
                        tsDS.sValues(iYear, iSeries) = CSng(astrTimeValues(iYear - 1))
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
                drow("DatType") = tsDS.iType(iTS)
                drow("WtType") = tsDS.sWeight(iTS)

                ' Concoct time series memo
                sbValues.Length = 0
                For iYear As Integer = 1 To tsDS.nDatasetNumYears(tsDS.ActiveDatasetIndex)
                    If (iYear > 1) Then sbValues.Append(" ")
                    sbValues.Append(tsDS.sValues(iYear, iTS))
                Next
                drow("TimeValues") = sbValues.ToString()

                drow.EndEdit()

                Select Case cTimeSeriesFactory.TimeSeriesCategory(DirectCast(tsDS.iType(iTS), eTimeSeriesType))
                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Fleet
                        drow = dtFleets.Rows.Find(tsDS.iTimeSeriesDBID(iTS))
                        bHasRow = (Object.ReferenceEquals(drow, Nothing) = False)

                        If bHasRow Then drow.BeginEdit() Else drow = writer.NewRow() : drow("TimeSeriesID") = tsDS.iTimeSeriesDBID(iTS)
                        If (tsDS.iPool(iTS) > 0) Then
                            iPoolID = ecopathDS.FleetDBID(tsDS.iPool(iTS))
                        Else
                            iPoolID = 0
                        End If
                        drow("FleetID") = iPoolID
                        If bHasRow Then drow.EndEdit() Else writer.AddRow(drow)

                    Case cTimeSeriesFactory.eTimeSeriesCategoryType.Group
                        drow = dtGroups.Rows.Find(tsDS.iTimeSeriesDBID(iTS))
                        bHasRow = (Object.ReferenceEquals(drow, Nothing) = False)

                        If bHasRow Then drow.BeginEdit() Else drow = writer.NewRow() : drow("TimeSeriesID") = tsDS.iTimeSeriesDBID(iTS)
                        If (tsDS.iPool(iTS) > 0) Then
                            iPoolID = ecopathDS.GroupDBID(tsDS.iPool(iTS))
                        Else
                            iPoolID = 0
                        End If
                        drow("GroupID") = iPoolID
                        drow("VariableName") = tsDS.strCustomVariableName(iTS)
                        If bHasRow Then drow.EndEdit() Else writer.AddRow(drow)

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
                sbValues.Append(asValues(iYear))
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
            Me.m_db.Execute(String.Format("DELETE * FROM EcosimTimeSeries WHERE (TimeSeriesID = {0})", iTimeSeriesID))
        Catch ex As Exception
            bSucces = False
        End Try
        Return bSucces

    End Function

#End Region ' Modify

#End Region ' Time series

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

        Return Me.IsChanged(eMessageSource.EcoSpace)

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
            ecospaceDS.Inrow = CInt(reader("Inrow"))
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
        Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenario WHERE ScenarioName='{0}'", strScenarioName))

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
            bSucces = bSucces And Me.AddEcospaceGroup(ecopathDS.GroupDBID(i), iScenarioID, iIDtmp)
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

        If bSucces Then Me.ClearChanged(eMessageSource.EcoSpace)

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
        Dim bDuplicating As Boolean = False
        Dim bSaving As Boolean = False

        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, ecopathDS.EcospaceScenarioDBID(iScenario))
        bSaving = (Me.IsChanged(eDataTypes.EcoSpaceScenario) Or idm.HasMapping(eDataTypes.EcoSpaceScenario, iScenarioID))

        Try

            writer = Me.m_db.GetWriter("EcospaceScenario")
            dt = writer.GetDataTable()
            drow = dt.Rows.Find(iScenarioID)

            drow.BeginEdit()
            drow("Inrow") = ecospaceDS.Inrow
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
                bSucces = bSucces And Me.AddEcospaceGroup(ecopathDS.GroupDBID(i), iScenarioID, iIDtmp)
            Next

            For i As Integer = 1 To ecopathDS.NumFleet
                ' Add fleet to the new scenario
                bSucces = bSucces And Me.AddEcospaceFleet(ecopathDS.FleetDBID(i), iScenarioID, iIDtmp)
            Next

            ' Add default 'All' habitat
            bSucces = bSucces And Me.AddEcospaceHabitat("All", iScenarioID, iIDtmp)

            ' Reload scenario definitions
            bSucces = bSucces And Me.LoadEcospaceScenarioDefinitions()

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
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenario WHERE (ScenarioID={0})", iScenarioID))
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
                    If ((i > ecospaceDS.Inrow) Or (j > ecospaceDS.InCol)) Then
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
        For i As Integer = 1 To ecospaceDS.Inrow
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
                If ((iRow <= ecospaceDS.Inrow) And (iCol <= ecospaceDS.InCol)) Then

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
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenarioBasemap WHERE (ScenarioID={0})", iScenarioID))

            ' Rebuild
            writer = Me.m_db.GetWriter("EcospaceScenarioBasemap")
            ' Every cell will need a row in the database, because every cell is assigned to a habitat.
            ' JS070226: should profile to see whether it's faster to update existing rows rather than
            '           destroying and rebuilding the entire table content.
            For iRow = 1 To ecospaceDS.Inrow
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
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenarioHabitatChange WHERE ScenarioID={0}", iScenarioID))

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
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenarioHabitat WHERE (ScenarioID={0}) AND (HabitatID={1})", iScenarioID, iHabitatID))
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
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenarioRegion WHERE (ScenarioID={0}) AND (RegionID={1})", iScenarioID, iRegionID))
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
                    ecospaceDS.PrefRow(iGroup, iMonth) = Integer.Parse(astrSplit(iMonth - 1))
                Next
                ' Monthly PrefCol
                astrSplit = CStr(reader("PrefCol")).Split(CChar(" "))
                For iMonth As Integer = 1 To Math.Min(cCore.N_MONTHS, astrSplit.Length)
                    ecospaceDS.Prefcol(iGroup, iMonth) = Integer.Parse(astrSplit(iMonth - 1))
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
        Dim iScenarioID As Integer = Array.IndexOf(ecopathDS.EcospaceScenarioDBID, ecopathDS.ActiveEcospaceScenario)
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
                    sbTemp.Append(ecospaceDS.PrefRow(iGroup, iMonth))
                Next
                drow("PrefRow") = sbTemp.ToString()

                sbTemp.Length = 0
                For iMonth As Integer = 1 To cCore.N_MONTHS
                    If iMonth > 1 Then sbTemp.Append(CChar(" "))
                    sbTemp.Append(ecospaceDS.Prefcol(iGroup, iMonth))
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
        Dim iScenarioID As Integer = idm.GetID(eDataTypes.EcoSpaceScenario, Array.IndexOf(ecopathDS.EcospaceScenarioDBID, ecopathDS.ActiveEcospaceScenario))
        Dim iGroupID As Integer = 0
        Dim iGroup As Integer = 0
        Dim iHabitatID As Integer = 0
        Dim iHabitat As Integer = 0

        Dim bSucces As Boolean = True

        Try
            ' No incremental save for now
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenarioGroupHabitat WHERE ScenarioID={0}", iScenarioID))

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
    Private Function AddEcospaceGroupToAllScenarios(ByVal iEcopathGroupID As Integer) As Boolean

        Dim reader As IDataReader = Nothing
        Dim iID As Integer = 0
        Dim bSucces As Boolean = True

        Try
            reader = Me.m_db.GetReader(String.Format("SELECT ScenarioID FROM EcoSpaceScenario"))
            While reader.Read()
                bSucces = bSucces And AddEcospaceGroup(iEcopathGroupID, CInt(reader("ScenarioID")), iID)
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
    Private Function AddEcospaceGroup(ByVal iEcopathGroupID As Integer, ByVal iScenarioID As Integer, ByRef iGroupID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim iGroup As Integer = 0
        Dim bDetritus As Boolean = False
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
            bDetritus = (ecopathDS.PP(iGroup) = 2.0)

            ' Add group
            writer = Me.m_db.GetWriter("EcospaceScenarioGroup")

            drow = writer.NewRow()
            drow("ScenarioID") = iScenarioID
            drow("EcopathGroupID") = iEcopathGroupID
            drow("GroupID") = iGroupID
            ' Detritus default of 10, non-detritus 300
            drow("MVel") = CSng(IIf(bDetritus, 10, 300))
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

        ' Read Sail and Port data
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
        Dim iCell As Integer = 0
        Dim astrSail As String() = Nothing
        Dim sSail As Single = 0.0
        Dim astrPort As String() = Nothing
        Dim bPort As Boolean = False
        Dim bSucces As Boolean = True
        Dim bDoneReading As Boolean = False

        reader = Me.m_db.GetReader(String.Format("SELECT * FROM EcospaceScenarioFleetMap WHERE (ScenarioID={0})", iScenarioID))
        Try
            While reader.Read()
                iFleet = Array.IndexOf(ecospaceDS.FleetDBID, CInt(reader("FleetID")))
                astrSail = Me.SplitNumberString(CStr(reader("Sail")))
                astrPort = Me.SplitNumberString(CStr(reader("Port")))

                iCell = 0
                iRow = 1
                bDoneReading = (iCell >= astrSail.Length And iCell >= astrPort.Length)

                While iRow < ecospaceDS.Inrow And Not bDoneReading
                    iCol = 1
                    While iCol < ecospaceDS.InCol And Not bDoneReading
                        Try
                            sSail = Single.Parse(astrSail(iCell))
                        Catch ex As Exception
                            sSail = 0.0!
                        End Try

                        Try
                            bPort = (astrPort(iCell) = "1")
                        Catch ex As Exception
                            bPort = False
                        End Try

                        ' Set Sail (haha)
                        ecospaceDS.Sail(iFleet, iRow, iCol) = sSail
                        ' Set Port 
                        ecospaceDS.Port(iFleet, iRow, iCol) = bPort

                        iCol += 1
                        iCell += 1
                        bDoneReading = (iCell >= astrSail.Length And iCell >= astrPort.Length)

                    End While
                    iRow += 1

                End While ' iRow
            End While ' Reader.read

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
        Dim iScenarioID As Integer = Array.IndexOf(ecopathDS.EcospaceScenarioDBID, ecopathDS.ActiveEcospaceScenario)
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
        Dim iScenarioID As Integer = Array.IndexOf(ecopathDS.EcospaceScenarioDBID, ecopathDS.ActiveEcospaceScenario)
        Dim iFleet As Integer = 0
        Dim iRow As Integer = 0
        Dim iCol As Integer = 0
        Dim iCell As Integer = 0
        Dim sbSail As StringBuilder = Nothing
        Dim sbPort As StringBuilder = Nothing
        Dim bSucces As Boolean = True

        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioID)

        Try
            ' Erase
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenarioFleetMap WHERE ScenarioID={0}", iScenarioID))
            writer = Me.m_db.GetWriter("EcospaceScenarioFleetMap")

            For iFleet = 1 To ecospaceDS.nFleets

                iCell = 0
                sbSail = New StringBuilder
                sbPort = New StringBuilder
                For iRow = 1 To ecospaceDS.Inrow
                    For iCol = 1 To ecospaceDS.InCol

                        If (iCell > 0) Then sbSail.Append(" ") : sbPort.Append(" ")
                        sbSail.Append(String.Format("{0:f}", ecospaceDS.Sail(iFleet, iRow, iCol)))
                        sbPort.Append(IIf(ecospaceDS.Port(iFleet, iRow, iCol) = True, 1, 0))
                        iCell += 1

                    Next iCol
                Next iRow

                drow = writer.NewRow()
                drow("ScenarioID") = iScenarioID
                drow("FleetID") = idm.GetID(eDataTypes.EcospaceFleet, ecospaceDS.FleetDBID(iFleet))
                drow("Sail") = sbSail.ToString
                drow("Port") = sbPort.ToString
                writer.AddRow(drow)

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
        Dim iScenarioID As Integer = Array.IndexOf(ecopathDS.EcospaceScenarioDBID, ecopathDS.ActiveEcospaceScenario)
        Dim iFleet As Integer = 0
        Dim iHabitat As Integer = 0
        Dim bSucces As Boolean = True

        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioID)

        Try
            ' Erase
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenarioHabitatFishery WHERE ScenarioID={0}", iScenarioID))
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
        Dim iScenarioID As Integer = Array.IndexOf(ecopathDS.EcospaceScenarioDBID, ecopathDS.ActiveEcospaceScenario)
        Dim iFleet As Integer = 0
        Dim iMPA As Integer = 0
        Dim bSucces As Boolean = True

        iScenarioID = idm.GetID(eDataTypes.EcoSpaceScenario, iScenarioID)

        Try
            ' Erase
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenarioMPAFishery WHERE ScenarioID={0}", iScenarioID))
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
        Dim iScenarioID As Integer = Array.IndexOf(ecopathDS.EcospaceScenarioDBID, ecopathDS.ActiveEcospaceScenario)

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
        Dim iScenarioID As Integer = Array.IndexOf(ecopathDS.EcospaceScenarioDBID, ecopathDS.ActiveEcospaceScenario)
        Dim bSucces As Boolean = True

        Try
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenarioMPA WHERE (ScenarioID={0}) AND (MPAID={1})", iScenarioID, iDBID))
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
                        If ((iRow <= ecospaceDS.Inrow) And (iCol <= ecospaceDS.InCol)) Then
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
                objKeys(1) = idm.GetID(eDataTypes.EcospaceImportanceLayer, l.DBID)
                drow = dt.Rows.Find(objKeys)

                bNewRow = (iScenarioIDSrc <> iScenarioIDdest) Or (drow Is Nothing)

                If bNewRow Then
                    drow = writer.NewRow()
                    drow("ScenarioID") = iScenarioIDdest
                    drow("LayerID") = lID
                    idm.Add(eDataTypes.EcospaceImportanceLayer, l.DBID, lID)
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
            Me.m_db.Execute(String.Format("DELETE * FROM EcospaceScenarioWeightLayerCell WHERE ScenarioID={0}", iScenarioID))

            writer = Me.m_db.GetWriter("EcospaceScenarioWeightLayerCell")

            For iLayer As Integer = 0 To ecospaceDS.nImportanceLayers - 1

                l = ecospaceDS.ImportanceLayers(iLayer)
                lID = idm.GetID(eDataTypes.EcospaceImportanceLayer, l.DBID)

                For iRow = 1 To ecospaceDS.Inrow
                    For iCol = 1 To ecospaceDS.InCol

                        ' Need to save this?
                        If l.Data(iRow, iCol) <> 0.0! Then
                            ' Create new row
                            drow = writer.NewRow()
                            ' Store simple values
                            drow("ScenarioID") = iScenarioID
                            drow("LayerID") = idm.GetID(eDataTypes.EcospaceImportanceLayer, lID)
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
        Dim iScenarioID As Integer = Array.IndexOf(ecopathDS.EcospaceScenarioDBID, ecopathDS.ActiveEcospaceScenario)

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
        Dim iScenarioID As Integer = Array.IndexOf(ecopathDS.EcospaceScenarioDBID, ecopathDS.ActiveEcospaceScenario)
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

        Return Me.IsChanged(eMessageSource.Ecotracer)

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

        ' Reload ecotracer scenario definitions to update lastsaved data
        Me.LoadEcotracerScenarioDefinitions()

        If bSucces Then Me.ClearChanged(eMessageSource.Ecotracer)

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
        Dim bSaving As Boolean = False

        iScenarioID = idm.GetID(eDataTypes.EcotracerScenario, ecopathDS.EcotracerScenarioDBID(iScenario))
        bSaving = (Me.IsChanged(eDataTypes.EcotracerScenario) Or idm.HasMapping(eDataTypes.EcotracerScenario, iScenarioID))

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
        Dim iScenarioID As Integer = Array.IndexOf(ecopathDS.EcotracerScenarioDBID, ecopathDS.ActiveEcotracerScenario)
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
            Me.m_db.Execute(String.Format("DELETE * FROM EcotracerScenario WHERE (ScenarioID={0})", iScenarioID))
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
                             eDataTypes.FishMort, eDataTypes.FishingRate
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
