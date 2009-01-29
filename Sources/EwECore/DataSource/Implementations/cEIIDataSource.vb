'==============================================================================
'
' $Log: cEIIDataSource.vb,v $
' Revision 1.5  2009/01/29 16:10:48  jeroens
' Moved cEwEDatabase.eAccessTypes to shared enums
'
' Revision 1.4  2009/01/16 23:51:20  jeroens
' Datasource no longer maitains data state by datatype, but by eCoreComponentType
'
' Revision 1.3  2008/11/28 16:54:03  joeb
' Cleaned up ToDo's
'
' Revision 1.2  2008/10/07 00:38:45  jeroens
' Ecosim prey/pred ff table flipped
'
' Revision 1.1  2008/09/26 07:30:14  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.48  2008/09/17 01:23:53  jeroens
' Currency units used correctly by Ecopath
'
' Revision 1.47  2008/08/08 23:17:25  jeroens
' Properly implemented SaveScenarioAs
' Added ImportanceLayers support
'
' Revision 1.46  2008/07/25 03:00:42  jeroens
' Incorporating new file extensions (w Joe)
' Adding error diagnostics on file access
'
' Revision 1.45  2008/07/21 14:04:57  jeroens
' Implemented pedigree interfaces
'
' Revision 1.44  2008/07/05 13:12:19  jeroens
' Updated to read BInput (instead of B)
'
' Revision 1.43  2008/07/04 16:27:41  jeroens
' Brought up to date with core init requirements
'
' Revision 1.42  2008/06/06 15:55:59  joeb
' Moved eDataTypes to EwEUtils.Core
'
' Revision 1.41  2008/02/22 21:44:03  jeroens
' vbK tied to StanzaLifeStage
'
' Revision 1.40  2008/02/11 03:24:48  jeroens
' Datasets are separate entities now, no longer just defined by name in Time Series
'
' Revision 1.39  2008/01/31 17:06:07  jeroens
' Added interface to load one single dataset
'
' Revision 1.38  2008/01/18 01:35:45  jeroens
' Added dataset manipulation method to sim datasource interface
'
'==============================================================================

Option Strict Off ' Aargh! Let's not attempt to purify the old code!

Imports System.IO
Imports EwECore.DataSources
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Database

''' <summary>
''' Data access for an Eii file
''' </summary>
<CLSCompliant(False)> _
Public Class cEIIDataSource
    Implements IEwEDataSource, IEcopathDataSource, IEcosimDatasource

    Private m_filename As String = ""
    Private m_core As cCore = Nothing

#Region " Generic "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Open an existing data source connection
    ''' </summary>
    ''' <param name="strName">Name of the EII file to open.</param>
    ''' <param name="core"><see cref="cCore">Core instance</see> that holds the 
    ''' datastructures to read to, and write from.</param>
    ''' <returns>True if opened successfully.</returns>
    ''' -------------------------------------------------------------------
    Public Function Open(ByVal strName As String, ByVal core As cCore) As eDatasourceAccessType _
            Implements IEwEDataSource.Open

        Dim fnum As Integer = FreeFile()

        ' Still open?
        If (Not String.IsNullOrEmpty(Me.m_filename)) Then Return False
        m_filename = strName

        ' Test if file can be read
        Try
            FileOpen(fnum, strName, OpenMode.Input)
        Catch ex As Exception
            Return False
        End Try

        FileClose(fnum)

        Me.m_filename = strName
        Me.m_core = core
        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Create the EII datasource.
    ''' </summary>
    ''' <param name="strName">Name of the EII file to create.</param>
    ''' <param name="strModelName">Name to assign to the model.</param>
    ''' <param name="core"><see cref="cCore">Core instance</see> that holds the 
    ''' datastructures to read to, and write from.</param>
    ''' <returns>Always false.</returns>
    ''' <remarks>This action is not supported in EwE6.</remarks>
    ''' -------------------------------------------------------------------
    Public Function Create(ByVal strName As String, ByVal strModelName As String, ByVal core As cCore) As eDatasourceAccessType _
             Implements IEwEDataSource.Create
        ' Cannot write EII files (yet)
        Return eDatasourceAccessType.Failed_Unknown
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Close the EII datasource.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function Close() As Boolean _
         Implements IEwEDataSource.Close

        Me.m_filename = ""
        Me.m_core = Nothing
        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Flag a core object as changed in the datasource. Since the EIIDataSource
    ''' does not support incremental saving, this method will contain no implementation
    ''' </summary>
    ''' <param name="cc">The <see cref="eCoreComponentType">core component</see> that changed.</param>
    ''' -------------------------------------------------------------------
    Public Sub SetChanged(ByVal cc As eCoreComponentType) _
            Implements IEwEDataSource.SetChanged
        ' Take no action
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the name of the EII file that this datasource operates on.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Connection() As Object Implements DataSources.IEwEDataSource.Connection
        Get
            Return Me.m_filename
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the name of the EII file that this datasource operates on.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Overrides Function ToString() As String Implements IEwEDataSource.ToString
        Return Me.m_filename
    End Function

    Private Overloads Function CopyEcopathTo(ByVal ds As DataSources.IEcopathDataSource) As Boolean Implements DataSources.IEcopathDataSource.CopyTo
        Return False
    End Function

    Private Overloads Function CopyEcosimTo(ByVal ds As DataSources.IEcosimDatasource) As Boolean Implements DataSources.IEcosimDatasource.CopyTo
        Return False
    End Function

    Public Function Version() As Single Implements IEwEDataSource.Version
        Return -1.0!
    End Function

    Public Function BeginTransaction() As Boolean Implements DataSources.IEwEDataSource.BeginTransaction
        Return True
    End Function

    Public Function EndTransaction(ByVal bCommit As Boolean) As Boolean Implements DataSources.IEwEDataSource.EndTransaction
        Return True
    End Function

#End Region ' Generic

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
        Return False
    End Function

#End Region ' Diagnostics

#End Region

#Region " EwEModel "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Initiates a full load of an ecopath model.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function LoadModel() As Boolean _
        Implements IEcopathDataSource.LoadModel

        'read the contents of the eii file into an EcopathParamters object
        'this is written using vb file access instead of a filestream to keep it as close to the original vb code as possible
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim pvar As Single
        Dim i As Integer
        Dim j As Integer
        Dim K As Integer
        Dim Dummy As Single
        Dim jnk As String
        Dim Import As Integer
        Dim fnum As Integer

        fnum = FreeFile()

        If m_filename = "" Then
            cLog.Write(Me.ToString + ".LoadEcopath(...) No file name specified.")
            Return False
        End If

        Try
            FileOpen(fnum, m_filename, OpenMode.Input)
        Catch ex As Exception
            LoadModel = False
            cLog.Write(Me.ToString + ".LoadEcopath(...) Error opening eii file. " + vbCrLf + m_filename + vbCrLf + "Error:" + ex.Message())
            Exit Function
        End Try

        'fake model data
        m_core.m_EwEModelDBID = 1
        m_core.m_EwEModelName = Path.GetFileName(m_filename)
        m_core.m_EwEModelNumDigits = 3
        m_core.m_EwEModelDescription = "Simulated model read from EII file " & m_filename

        'read the file
        Try
            Input(fnum, ecopathDS.NumGroups) : Input(fnum, ecopathDS.NumLiving) : Input(fnum, Me.m_core.m_EwEModelUnitCurrency) : Input(fnum, ecopathDS.currUnitIndex)

            If Not ecopathDS.redimGroupVariables() Then
                LoadModel = False
                cLog.Write(Me.ToString + ".LoadEcopath(...) Failed to Re-Dimension EcoPath Parameter arrays.")
                Exit Function
            End If

            'groups
            For K = 1 To ecopathDS.NumGroups
                Input(fnum, ecopathDS.GroupName(K)) : Input(fnum, pvar) : Input(fnum, ecopathDS.DtImp(K))
                Input(fnum, ecopathDS.Ex(K)) : Input(fnum, ecopathDS.fCatch(K)) : Input(fnum, ecopathDS.DC(K, 0))
                Input(fnum, ecopathDS.Binput(K)) : Input(fnum, ecopathDS.PBinput(K)) : Input(fnum, ecopathDS.EEinput(K))
                Input(fnum, ecopathDS.GEinput(K)) : Input(fnum, ecopathDS.QBinput(K))

                ecopathDS.GroupDBID(K) = K

                'Input #fnum, GroupName(K), Pvar, DtImp(K), Ex(K), Catch(K), parms.DC(K, 0), parms.B(K), parms.pb(K), parms.ee(K), parms.ge(K), parms.qb(K)
                'jb this does not make any sence
                'it uses the Primary Porduction as the version number ????
                'If pvar < -1.99 Then
                '    txt = "It is not possible to import your old version of the " _
                '        + "Ecopath data file. " _
                '        + "You may have to reenter your data.  " _
                '        + "Open the eii file in Notepad, and check it. " _
                '        + "A testversion of Ecopath with Ecosim had a bug where it would place, " _
                '        + "e.g., '-94-95' instead of '-94 -95' in the eii file. If this is the case then add spaces where needed. " _
                '        + "If not, please email v.christensen@cgiar.org " + vbNewLine _
                '        + "Please edit data.  Press any key to abort. "

                '    MsgBox(txt, vbCritical + vbOKOnly, "Problem importing old file type")

                '    FileClose(fnum)
                '    ReadEii = False
                '    Exit Function
                'End If

                ecopathDS.PP(K) = pvar - 2
                If K > ecopathDS.NumLiving Then ecopathDS.PP(K) = 2
                If ecopathDS.GE(K) = 0 Then ecopathDS.GE(K) = -9

            Next K

            ' "Read DietComp"
            ReDim ecopathDS.DietChanged(1, 0)
            For K = 1 To ecopathDS.NumGroups
                For j = 1 To ecopathDS.NumGroups
                    Input(fnum, ecopathDS.DC(K, j))
                    If ecopathDS.DC(K, j) > 0 Then
                        ecopathDS.DietWasChanged(K, j)
                    End If
                Next j
            Next K

            'jb totp read in original routine using a string will read the entire line
            Input(fnum, jnk)
            'jb I have no idea what this is all about 
            If Import < 0 Then Import = 0

            'Unassimilated food
            For j = 1 To ecopathDS.NumGroups
                Input(fnum, Dummy) : Input(fnum, ecopathDS.GS(j))
                If Dummy < 0 Then Dummy = 0
                ecopathDS.GS(j) = Dummy + ecopathDS.GS(j)
                If ecopathDS.GS(j) > 1 Then ecopathDS.GS(j) = ecopathDS.GS(j) / 100
            Next j

            Input(fnum, jnk)

            'the time unit name
            If EOF(fnum) = False Then
                Dim tmpbuff As String
                Input(fnum, tmpbuff)
                ecopathDS.TimeUnitName = tmpbuff.Trim
                Select Case LCase(ecopathDS.TimeUnitName)
                    Case "year"
                        ecopathDS.TimeUnitIndex = 1
                    Case "day"
                        ecopathDS.TimeUnitIndex = 2
                    Case Else
                        ecopathDS.TimeUnitIndex = 3
                End Select
            End If

            'the ecosystem remarks.
            Input(fnum, jnk)

            For i = 1 To ecopathDS.NumGroups             ' parms.Bomass accumulation added March 95/VC
                Input(fnum, ecopathDS.BA(i))
            Next i

            'If EOF(fnum) = False And NumGroups > NumLiving + 1 Then
            'More than 1 detritusbox Any reason for this??
            For i = 1 To ecopathDS.NumGroups
                For j = ecopathDS.NumLiving + 1 To ecopathDS.NumGroups
                    Input(fnum, ecopathDS.DF(i, j - ecopathDS.NumLiving))     ' Diet Fate array added July 1994/VC
                Next j
            Next i

            Input(fnum, jnk) ' 
            For i = 1 To ecopathDS.NumGroups             ' Emigration added Dec 98/VC
                Input(fnum, ecopathDS.Emigration(i))
            Next i

            Input(fnum, jnk)
            For i = 1 To ecopathDS.NumGroups                 ' immigration added Dec 98/VC
                Input(fnum, ecopathDS.Immig(i))
            Next i

            Input(fnum, jnk)  'NumGear
            Input(fnum, ecopathDS.NumFleet)

            ecopathDS.RedimFleetVariables(True)

            Input(fnum, jnk) 'Gearnames
            For i = 1 To ecopathDS.NumFleet             ' Added Dec 98/VC
                Input(fnum, ecopathDS.FleetName(i))
                ecopathDS.FleetDBID(i) = i
            Next i

            Input(fnum, jnk)  'cost
            For i = 1 To ecopathDS.NumFleet
                'First is fixed cost, second is cost per unit effort' Added Dec 98/VC
                Input(fnum, ecopathDS.CostPct(i, eCostIndex.Fixed))
                Input(fnum, ecopathDS.CostPct(i, eCostIndex.CUPE))
                Input(fnum, ecopathDS.CostPct(i, eCostIndex.Sail))
            Next i

            Input(fnum, jnk)  'landing
            For i = 1 To ecopathDS.NumFleet
                For j = 1 To ecopathDS.NumGroups
                    Input(fnum, ecopathDS.Landing(i, j))    ' Landing added Dec 98/VC
                Next j
            Next i

            Input(fnum, jnk)  'discard
            For i = 1 To ecopathDS.NumFleet
                For j = 1 To ecopathDS.NumGroups
                    Input(fnum, ecopathDS.Discard(i, j))    ' Added Dec 98/VC
                Next j
            Next i

            Input(fnum, jnk)  'discard
            For i = 1 To ecopathDS.NumFleet
                For j = 1 To ecopathDS.NumGroups - ecopathDS.NumLiving
                    Input(fnum, ecopathDS.DiscardFate(i, j))   ' Added Dec 98/VC
                Next j
            Next i

            Input(fnum, jnk)  'market
            For i = 1 To ecopathDS.NumFleet
                For j = 1 To ecopathDS.NumGroups
                    Input(fnum, ecopathDS.Market(i, j))    ' Added Dec 98/VC
                Next j
            Next i

            ecopathDS.NoGearData = False

            'shadow
            Input(fnum, jnk)
            For i = 1 To ecopathDS.NumGroups             ' Added Dec 98/VC
                Input(fnum, ecopathDS.Shadow(i))
            Next i

            'Habitatarea
            Input(fnum, jnk)  '
            For i = 1 To ecopathDS.NumGroups             ' Added Dec 98/VC
                Input(fnum, ecopathDS.Area(i))
                Input(fnum, ecopathDS.BH(i))
            Next i

            FileClose(fnum)

        Catch ex As Exception 'catch any error during the reading of the data
            FileClose(fnum)
            LoadModel = False
            'some kind of a reading error better find out what happend
            cLog.Write(Me.ToString + ".LoadEcopath() Error reading eii file. Error: " + ex.Message())
            Debug.Assert(False)
            Exit Function
        End Try

        For i = 1 To ecopathDS.NumGroups
            If ecopathDS.QB(i) = 0 And ecopathDS.PP(i) = 1 Then ecopathDS.GS(i) = 0
            If ecopathDS.PP(i) = 2 Then ecopathDS.GS(i) = 0
        Next i

        ecopathDS.GS(ecopathDS.NumGroups) = 0

        For i = 1 To ecopathDS.NumGroups
            If ecopathDS.Area(i) <= 0 Or ecopathDS.Area(i) > 1 Then ecopathDS.Area(i) = 1
            If ecopathDS.BH(i) <= 0 And ecopathDS.B(i) > 0 Then ecopathDS.BH(i) = ecopathDS.B(i) / ecopathDS.Area(i)
        Next i

        ecopathDS.bInitialized = True

        Me.LoadStanza()
        Me.LoadEcosimScenarioDefinitions()

        ' Make sure that the core knows not to exect anything else
        ecopathDS.RedimEcospaceScenarios()
        ecopathDS.RedimEcotracerScenarios()

        ' Invoke plugin point
        If (Me.m_core.PluginManager IsNot Nothing) Then Me.m_core.PluginManager.LoadModel(Me)

        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Initiates a save of an EwE model
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Function SaveModel() As Boolean _
             Implements IEcopathDataSource.SaveModel
        Return False
    End Function

#Region " Pedigree "

    Public Function AddPedigreeLevel(ByVal iPosition As Integer, ByVal varName As EwEUtils.Core.eVarNameFlags, ByVal sIndexValue As Single, ByVal sConfidence As Single, ByVal strDescription As String, ByRef iDBID As Integer) As Boolean Implements DataSources.IEcopathDataSource.AddPedigreeLevel
        Return False
    End Function

    Public Function MovePedigreeLevel(ByVal iDBID As Integer, ByVal iPosition As Integer) As Boolean Implements DataSources.IEcopathDataSource.MovePedigreeLevel
        Return False
    End Function

    Public Function RemovePedigreeLevel(ByVal iDBID As Integer) As Boolean Implements DataSources.IEcopathDataSource.RemovePedigreeLevel
        Return False
    End Function

#End Region ' Pedigree

#End Region ' EwE Model

#Region " Ecopath "

#Region " Diagnostics "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States if the datasource has unsaved changes for Ecopath.
    ''' </summary>
    ''' <returns>True if the datasource has pending changes for Ecopath.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsEcopathModified() As Boolean Implements DataSources.IEcopathDataSource.IsEcopathModified

        Return False

    End Function

#End Region ' Diagnostics

#Region " Groups "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Create a record for a new Ecopath group in the datasource.
    ''' </summary>
    ''' <param name="strGroupName">The name of the group to create.</param>
    ''' <param name="sPP">The Type of the new group; 0=consumer, 1=producer, 2=detritus.</param>
    ''' <param name="iPosition">The position of the new group in the group sequence.</param>
    ''' <param name="iDBID">Database ID assigned to the new Group.</param>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' Note that this will not adjust the data arrays. Due to the complex organization of the
    ''' core a full data reload is required after a group is created.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Function AddGroup(ByVal strGroupName As String, ByVal sPP As Single, ByVal iPosition As Integer, ByRef iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.AddGroup

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Dim newNumGroups As Integer = ecopathDS.NumGroups + 1
        ecopathDS.NumGroups = newNumGroups
        ecopathDS.redimGroups()

        LoadStanza()

        If ecosimDS IsNot Nothing Then
            ecosimDS.nGroups = newNumGroups
            ' ecosimDS.RedimVars()
            LoadScenario(-1)
        End If


        'insert the record into the database then
        'popluate the ecopath data structures with the data from the database
        'in this case we can't  because we do not have a proper datasource

        'just fake the Database ID's
        'this is the same numbering system that was used by the EII reading routine
        For i As Integer = 1 To newNumGroups
            ecopathDS.GroupDBID(i) = i
        Next

        Return True 'sweeeeet see that was no problem

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
    Function RemoveGroup(ByVal iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.RemoveGroup

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData

        ecopathDS.NumGroups -= 1
        ecopathDS.redimGroupVariables()

        If m_core.m_EcoSimData IsNot Nothing Then
            m_core.m_EcoSimData.nGroups -= 1
            m_core.m_EcoSimData.RedimVars()
        End If

        'insert the record into the database then
        'popluate the ecopath data structures with the data from the database
        'in this case we can't  because we do not have a proper datasource

        'just fake the Database ID's
        'this is the same numbering system that was used by the EII reading routine
        For i As Integer = 1 To ecopathDS.NumGroups
            ecopathDS.GroupDBID(i) = i
        Next

        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Move an Ecopath group to a different position in the group sequence.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the group to move.</param>
    ''' <param name="iPosition">The new position of the group in the group sequence.</param>
    ''' <returns>Always false.</returns>
    ''' <remarks>
    ''' For now, this method is not supported since all data arrays need to be adjusted
    ''' and there is no real need to implement this for EII datasources.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Function MoveGroup(ByVal iDBID As Integer, ByVal iPosition As Integer) As Boolean _
             Implements IEcopathDataSource.MoveGroup
        Return False
    End Function

#End Region ' Groups

#Region " Fleets "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds a fleet to the datasource.
    ''' </summary>
    ''' <param name="strFleetName">Name of the new fleet.</param>
    ''' <param name="iDBID">Database ID assigned to the new fleet.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function AddFleet(ByVal strFleetName As String, ByVal iPosition As Integer, ByRef iDBID As Integer) As Boolean _
            Implements DataSources.IEcopathDataSource.AddFleet
        ' ToDo_JB: Write this
        Return False
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes a fleet from the datasource.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the fleet to remove.</param>
    ''' <returns>Always false.</returns>
    ''' <remarks>This action is not supported in EwE6.</remarks>
    ''' -------------------------------------------------------------------
    Function RemoveFleet(ByVal iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.RemoveFleet
        Return False
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
        Return False
    End Function

#End Region ' Fleets

#End Region ' Ecopath (Model, Groups, Fleets)

#Region " EcoSim "

#Region " Diagnostics "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States if the datasource has unsaved changes for Ecosim.
    ''' </summary>
    ''' <returns>True if the datasource has pending changes for Ecosim.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsEcosimModified() As Boolean Implements DataSources.IEcosimDatasource.IsEcosimModified

        Return False

    End Function

#End Region ' Diagnostics

#Region " Scenarios "

    Private Function LoadEcosimScenarioDefinitions() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        ecopathDS.NumEcosimScenarios = 1
        ecopathDS.RedimEcosimScenarios()

        ecopathDS.EcosimScenarioName(1) = My.Resources.CoreDefaults.CORE_DEFAULT_SCENARIO()
        ecopathDS.EcosimScenarioDBID(1) = 1
        ecopathDS.EcosimScenarioDescription(1) = "This is a dummy scenario, manually crafted in cEIIDataSource."

        Return True
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads an ecosim scenario from the EII.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the scenario to load.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Function LoadScenario(ByVal iDBID As Integer) As Boolean _
            Implements IEcosimDatasource.LoadEcosimScenario

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData

        'ToDo_jb PopulateEcoSimInputVars this has to totaly change once there is a database
        'this is just to get something working

        'Hack:jb LoadEcosim() set ngroups in EcoSim to same as EcoPath this is until we can read from the datasource
        ecosimDS.nGroups = ecopathDS.NumGroups

        ecosimDS.RedimVars()
        ecosimDS.SetDefaultParameters()

        ecopathDS.ActiveEcosimScenario = 1

        ecosimDS.DimForcingShapes()
        ecosimDS.InitForcingShapes()
        ecosimDS.ReDimMediation()


        'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
        'HACK WARNING this is a temp fix to populate SimDC so that it can be used be tempCreateForcingMediationShapes() to init some fake data
        'this will get overwritten by EcoSim in RemoveImportFromEcosim()
        For iPred As Integer = 1 To ecosimDS.nGroups
            For iPrey As Integer = 1 To ecosimDS.nGroups
                ecosimDS.SimDC(iPred, iPrey) = ecopathDS.DC(iPred, iPrey)
            Next iPrey
        Next iPred

        Dim i As Integer
        'jb Temp Hack to build DBID for each shape 
        For i = 1 To ecosimDS.ForcingShapes
            ecosimDS.ForcingDBIDs(i) = i
        Next
        'jb Temp Hack to build DBID for each shape 
        For i = 1 To ecosimDS.MediationShapes
            ecosimDS.MediationDBIDs(i) = i
        Next

        'fake database IDs
        For i = 1 To ecosimDS.nGroups
            ecosimDS.GroupDBID(i) = i
        Next

        'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

        Return True
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Updates an ecosim scenario in the EII.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the scenario to update.</param>
    ''' <returns>Always false.</returns>
    ''' <remarks>This action is not supported in EwE6.</remarks>
    ''' -------------------------------------------------------------------
    Friend Function SaveEcosimScenario(ByVal iDBID As Integer) As Boolean _
            Implements IEcosimDatasource.SaveEcosimScenario
        Return False
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds an ecosim scenario to the EII.
    ''' </summary>
    ''' <param name="strName">Name to assign to new scenario.</param>
    ''' <param name="strDescription">Description to assign to new scenario.</param>
    ''' <param name="strAuthor">Author to assign to the new scenario.</param>
    ''' <param name="strContact">Contact info to assign to the new scenario.</param>
    ''' <param name="iDBID">Database ID assigned to the new scenario.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function AppendEcosimScenario(ByVal strName As String, ByVal strDescription As String, ByVal strAuthor As String, ByVal strContact As String, ByRef iDBID As Integer) As Boolean _
            Implements IEcosimDatasource.AppendEcosimScenario
        Return False
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes an ecosim scenario from the EII.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the scenario to remove.</param>
    ''' <returns>Always false.</returns>
    ''' -------------------------------------------------------------------
    Friend Function RemoveEcosimScenario(ByVal iDBID As Integer) As Boolean _
            Implements IEcosimDatasource.RemoveEcosimScenario
        Return False
    End Function

    Public Function SaveEcospaceScenarioAs(ByVal strScenarioName As String, ByVal strDescription As String, _
     ByVal strAuthor As String, ByVal strContact As String, ByRef iScenarioID As Integer) As Boolean _
            Implements IEcosimDatasource.SaveEcosimScenarioAs
        Return False
    End Function

#End Region ' Scenarios

#Region " Forcing Shapes "


    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Appends a forcing shape to the EII.
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
    Function AppendShape(ByVal strShapeName As String, ByVal shapeType As eDataTypes, ByRef iDBID As Integer, _
            ByVal asData As Single(), ByVal sYZero As Single, ByVal sYBase As Single, ByVal sYend As Single, ByVal sSteep As Single, ByVal functionType As eShapeFunctionType) As Boolean _
            Implements IEcosimDatasource.AppendShape

        Dim b_return As Boolean
        'increment the number of forcing shapes and pass that into EcoSimDatastructure it will resize to the new number of shapes
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData

        'a proper datasource will 
        'add a record to all tables that need it 
        'compute the new number of shapes and use that to resize the Ecosim Data
        'populate the Ecosim Data in memory with the values from the datasource
        'return the new Ecosim Index and Database ID

        If shapeType = eDataTypes.Mediation Then

            'get the new number of shape by adding one to the existing number of shapes
            Dim tmpNumberOfShapes As Integer = ecosimDS.MediationShapes + 1

            'add the shape to the underlying EcoSim data
            'this will redim to the new number of shapes
            b_return = ecosimDS.ResizeMediationShapes(tmpNumberOfShapes, tmpNumberOfShapes)

            'fake DB id's
            For i As Integer = 1 To ecosimDS.MediationShapes
                ecosimDS.MediationDBIDs(i) = i
            Next

            ''Fake a database ID because there are no database ID in the EII files
            ''this will allow for testing of database ID
            'newDBID = ecosimDS.MediationDBIDs(newEcoSimIndex)

            Return b_return


        Else
            Dim tmpNumberOfShapes As Integer = ecosimDS.ForcingShapes + 1

            'add the shape to the underlying EcoSim data
            b_return = ecosimDS.ResizeForcingShapes(tmpNumberOfShapes, tmpNumberOfShapes)

            'fake DB id's
            For i As Integer = 1 To ecosimDS.ForcingShapes
                ecosimDS.ForcingDBIDs(i) = i
            Next

            ''Fake a database ID because there are no database ID in the EII files
            ''this will allow for testing of database ID
            'newDBID = ecosimDS.ForcingEggProdDBIDs(newEcoSimIndex)

            Return b_return
        End If


    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Deletes a forcing shape from the EII.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the shape to remove.</param>
    ''' <returns>True if successful.</returns>
    ''' -------------------------------------------------------------------
    Function RemoveShape(ByVal iDBID As Integer) As Boolean _
             Implements IEcosimDatasource.RemoveShape

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData

        Debug.Assert(ecosimDS.ForcingShapes - 1 > 0, "No more shapes to remove")
        'jb this is just for testing 
        ecosimDS.ResizeForcingShapes(ecosimDS.ForcingShapes - 1)

        'hack to fake database IDs
        For i As Integer = 1 To ecosimDS.ForcingShapes
            ecosimDS.ForcingDBIDs(i) = i
        Next

        Return True
    End Function

#End Region ' Forcing Functions

#Region " Time series "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds a time series to the datasource.
    ''' </summary>
    ''' <param name="strName">Name of the new Time Series to add.</param>
    ''' <param name="timeSeriesType"><see cref="eTimeSeriesType">Type</see> of the time series.</param>
    ''' <param name="asValues">Initial values to set in the TS.</param>
    ''' <param name="iDBID">Database ID assigned to the new TS.</param>
    ''' <returns>Always false.</returns>
    ''' -------------------------------------------------------------------
    Public Function AppendTimeSeries(ByVal strName As String, ByVal iPool As Integer, ByVal timeSeriesType As eTimeSeriesType, ByVal sWeight As Single, ByVal asValues() As Single, ByRef iDBID As Integer) As Boolean _
            Implements DataSources.IEcosimDatasource.AppendTimeSeries
        Return False
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes a time series from the datasource.
    ''' </summary>
    ''' <param name="iTimeSeriesID">Database ID of the time series to remove.</param>
    ''' <returns>Always false.</returns>
    ''' -------------------------------------------------------------------
    Friend Function RemoveTimeSeries(ByVal iTimeSeriesID As Integer) As Boolean _
            Implements DataSources.IEcosimDatasource.RemoveTimeSeries
        Return False
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Load all time series for a given dataset.
    ''' </summary>
    ''' <param name="iDataset">Index of dataset to load.</param>
    ''' <returns>Always false.</returns>
    ''' -------------------------------------------------------------------
    Public Function LoadTimeSeriesDataset(ByVal iDataset As Integer) As Boolean _
         Implements DataSources.IEcosimDatasource.LoadTimeSeriesDataset
        Return False
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
    ''' <returns>Always false.</returns>
    ''' -------------------------------------------------------------------
    Public Function AppendTimeSeriesDataset(ByVal strDatasetName As String, ByVal strDescription As String, ByVal strAuthor As String, ByVal strContact As String, ByVal iFirstYear As Integer, ByVal iNumYears As Integer, ByRef iDatasetID As Integer) As Boolean Implements DataSources.IEcosimDatasource.AppendTimeSeriesDataset
        Return False
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Import a complete <see cref="cTimeSeriesImport">cTimeSeriesImport</see>
    ''' instance into the datasource.
    ''' </summary>
    ''' <param name="ts">The time series data to import.</param>
    ''' <param name="iDataset">Index of the dataset to add time series to.</param>
    ''' <returns>Always false.</returns>
    ''' -------------------------------------------------------------------
    Public Function ImportTimeSeries(ByVal ts As cTimeSeriesImport, ByVal iDataset As Integer) As Boolean Implements DataSources.IEcosimDatasource.ImportTimeSeries
        Return False
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes all time series belonging to a specific dataset from the datasource.
    ''' </summary>
    ''' <param name="iDataset">Index of the dataset to remove.</param>
    ''' <returns>Always false.</returns>
    ''' -------------------------------------------------------------------
    Public Function RemoveTimeSeriesDataset(ByVal iDataset As Integer) As Boolean _
            Implements DataSources.IEcosimDatasource.RemoveTimeSeriesDataset
        Return False
    End Function

#End Region ' Time series

#End Region ' EcoSim

#Region " Stanza "

    Private Function LoadStanza() As Boolean
        Dim m_stanzaData As cStanzaDatastructures = m_core.m_Stanza

        ''xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        ''HACK WARNING
        ''jb this is totaly bogus 
        ''is is just to get the stanza variables initialized so that I can test the Stanza Groups interface
        ''go with 2 stanza groups 

        ''init the cores stanza data structures
        'm_stanzaData.MaxStanza = 3
        'm_stanzaData.Nsplit = 2
        'm_stanzaData.MaxAgeSplit = 400 '???? 

        'm_stanzaData.redimStanza()

        ''populate the arrays
        'm_stanzaData.Nstanza(1) = 2
        'm_stanzaData.Nstanza(2) = 3

        ''stanza group 1
        ''fish groups 2 and 3
        'm_stanzaData.EcopathCode(1, 1) = 2
        'm_stanzaData.EcopathCode(1, 2) = 3

        ''stanza group 2
        ''fish groups 5,6 and 7
        'm_stanzaData.EcopathCode(2, 1) = 5
        'm_stanzaData.EcopathCode(2, 2) = 6
        'm_stanzaData.EcopathCode(2, 3) = 7
        'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

        'fake a database ID for the EII datasource
        For i As Integer = 1 To m_stanzaData.Nsplit
            m_stanzaData.StanzaDBID(i) = 1
        Next
        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Adds a stanza group to the EII.
    ''' </summary>
    ''' <returns>Always false; mutli-stanza logis is not supported in the EII data format.</returns>
    ''' -------------------------------------------------------------------
    Friend Function AppendStanza(ByVal strStanzaName As String, ByVal aiGroupID() As Integer, ByVal aiStartAge() As Integer, ByRef iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.AppendStanza
        Return False
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes a stanza group from the EII.
    ''' </summary>
    ''' <param name="iDBID">Database ID of the stanza group to remove.</param>
    ''' <returns>Always false; mutli-stanza logis is not supported in the EII data format.</returns>
    ''' -------------------------------------------------------------------
    Function RemoveStanza(ByVal iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.RemoveStanza
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
    ''' <returns>Always false; mutli-stanza logis is not supported in the EII data format.</returns>
    ''' -------------------------------------------------------------------
    Public Function AddStanzaLifestage(ByVal iStanzaDBID As Integer, ByVal iGroupDBID As Integer, ByVal iStartAge As Integer, ByVal sMortality As Single, ByVal sVBK As Single) As Boolean _
            Implements DataSources.IEcopathDataSource.AddStanzaLifestage
        Return False
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Removes a life stage from an existing stanza configuration.
    ''' </summary>
    ''' <param name="iStanzaDBID">Database ID of the stanza group to remove the life stage from.</param>
    ''' <param name="iGroupDBID">Group to remove as the life stage.</param>
    ''' <returns>Always false; mutli-stanza logis is not supported in the EII data format.</returns>
    ''' -------------------------------------------------------------------
    Public Function RemoveStanzaLifestage(ByVal iStanzaDBID As Integer, ByVal iGroupDBID As Integer) As Boolean _
            Implements DataSources.IEcopathDataSource.RemoveStanzaLifestage
        Return False
    End Function

#End Region ' Stanza

#Region "Dead Code used during development"
#If 0 Then
    Private Sub tempCreateForcingMediationShapes()

        'temp
        'ToDo_jb  remove this temp hack to load the forcing functions 
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza

        Try
            '
            'ecosimDS.ForcingShapes = 2 'one Forcing one EggProd
            ' ecosimDS.MediationShapes = 1
            ecosimDS.ResizeForcingShapes(2)
            ' ecosimDS.redimForcingShapes()
            '   ecosimDS.InitForcingShapes()
            ecosimDS.ResizeMediationShapes(1)
            '  ecosimDS.ReDimMediation()

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Forcing Shape
            Dim iForceShape As Integer = 1
            ecosimDS.ForcingTitles(iForceShape) = "Forcing Shape From Datasource"
            ecosimDS.ForcingShapeType(iForceShape) = eDataTypes.Forcing
            ecosimDS.ForcingDBIDs(iForceShape) = CInt(Rnd() * 1000)

            For ipt As Integer = 1 To ecosimDS.ForcePoints
                ecosimDS.zscale(ipt, iForceShape) = (2 / ecosimDS.ForcePoints) * ipt
            Next ipt

            'apply this shape to all the valid pred/prey
            For iPred As Integer = 1 To ecosimDS.nGroups
                For iPrey As Integer = 1 To ecosimDS.nGroups
                    If ecosimDS.SimDC(iPred, iPrey) <> 0 Then
                        ecosimDS.FunctionNumber(iPrey, iPred, 1) = iForceShape
                        ecosimDS.IsMedFunction(iPrey, iPred, 1) = False
                        If iPred = iPrey Then
                            ecosimDS.FunctionType(iPrey, iPred, 1) = 2
                        Else
                            ecosimDS.FunctionType(iPrey, iPred, 1) = 1
                        End If
                    End If
                Next iPrey
            Next iPred
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Egg Production
            Dim iEggShape As Integer = 2

            ecosimDS.ForcingTitles(iEggShape) = "EggProd Shape From Datasource"
            ecosimDS.ForcingShapeType(iEggShape) = eDataTypes.EggProd
            ecosimDS.ForcingDBIDs(iEggShape) = CInt(Rnd() * 1000)

            For ipt As Integer = 1 To ecosimDS.ForcePoints
                ecosimDS.zscale(ecosimDS.ForcePoints - ipt, iEggShape) = (2 / ecosimDS.ForcePoints) * ipt
            Next ipt

            For iStanza As Integer = 1 To stanzaDS.Nsplit 'nSplit is the number of stanza groups
                stanzaDS.EggProdShapeSplit(iStanza) = iEggShape
            Next iStanza
            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx


            'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
            'Mediation 
            ecosimDS.MediationTitles(1) = "Mediation Function from Datasource"
            ecosimDS.MediationDBIDs(1) = CInt(Rnd() * 1000)
            ecosimDS.MedIsUsed(1) = True
            ecosimDS.NMedXused(1) = 2

            'IMedUsed(nGroups + nGear, MediationShapes)
            ecosimDS.IMedUsed(1, 1) = 1
            ecosimDS.IMedUsed(2, 1) = ecosimDS.nGroups

            ecosimDS.MedWeights(1, 1) = 1
            ecosimDS.MedWeights(2, 1) = 0.5

            For ipt As Integer = 1 To ecosimDS.NMedPoints
                ecosimDS.Medpoints(ipt, 1) = (2 / ecosimDS.NMedPoints) * ipt
            Next ipt

            'apply this shape to all the valid pred/prey
            For iPred As Integer = 1 To ecosimDS.nGroups
                For iPrey As Integer = 1 To ecosimDS.nGroups
                    If ecosimDS.SimDC(iPred, iPrey) <> 0 Then
                        ecosimDS.FunctionNumber(iPrey, iPred, 2) = 1
                        ecosimDS.IsMedFunction(iPrey, iPred, 2) = True
                        ecosimDS.FunctionType(iPrey, iPred, 2) = eForcingFunctionApplication.ProductionRate
                    End If
                Next iPrey
            Next iPred

            'shape parameters for both type of shapes
            Dim i As Integer
            For i = 1 To ecosimDS.MediationShapes
                ecosimDS.MediationShapeParams(i).ShapeFunctionType = eShapeFunctionType.NotSet
            Next

            For i = 1 To ecosimDS.ForcingShapes
                ecosimDS.ForcingShapeParams(i).ShapeFunctionType = eShapeFunctionType.Exponential
            Next

        Catch ex As Exception
            Debug.Assert(False, "Error in temporary init of Forcing & Mediation Shapes.")
        End Try


    End Sub
#End If
#End Region

End Class

