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
' Copyright 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'

#Region " Imports "

Option Strict On

Imports System.IO
Imports EwECore.DataSources
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Database
'
#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Data access for an EwE5 .EII file
''' </summary>
''' ===========================================================================
Public Class cEIIDataSource
    Implements IEwEDataSource
    Implements IEcopathDataSource
    Implements IEcosimDatasource
    Implements IEcospaceDatasource

    Private m_strFilename As String = ""
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
    Public Function Open(ByVal strName As String, _
                         ByVal core As cCore, _
                         Optional ByVal datasourceType As eDataSourceTypes = eDataSourceTypes.NotSet, _
                         Optional ByVal bReadOnly As Boolean = False) As eDatasourceAccessType _
                     Implements DataSources.IEwEDataSource.Open

        If (Not String.IsNullOrEmpty(Me.m_strFilename)) Then Return eDatasourceAccessType.Failed_UnknownType
        If Not File.Exists(strName) Then Return eDatasourceAccessType.Failed_FileNotFound

        Me.m_strFilename = strName
        Me.m_core = core
        Return eDatasourceAccessType.Opened

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' States whether a datasource is already open.
    ''' </summary>
    ''' <returns>True if the datasource is open.</returns>
    ''' -------------------------------------------------------------------
    Public Function IsOpen() As Boolean _
             Implements IEwEDataSource.IsOpen
        Return (Not String.IsNullOrEmpty(Me.m_strFilename))
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

        Me.m_strFilename = ""
        Me.m_core = Nothing
        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Flag a core object as changed in the datasource. Since the EIIDataSource
    ''' does not support saving of data, this method contains no implementation
    ''' </summary>
    ''' <param name="cc">The <see cref="eCoreComponentType">core component</see> that changed.</param>
    ''' -------------------------------------------------------------------
    Friend Sub SetChanged(ByVal cc As eCoreComponentType) _
            Implements IEwEDataSource.SetChanged
        ' Take no action
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Clear change flags in the datasource. Since the EIIDataSource does 
    ''' not support saving of data, this method contains no implementation
    ''' </summary>
    ''' -------------------------------------------------------------------
    Friend Sub ClearChanged() _
        Implements IEwEDataSource.ClearChanged
        ' Take no actions
    End Sub

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the name of the EII file that this datasource operates on.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public ReadOnly Property Connection() As Object Implements DataSources.IEwEDataSource.Connection
        Get
            Return Me.m_strFilename
        End Get
    End Property

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Get the name of the EII file that this datasource operates on.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Public Overrides Function ToString() As String Implements IEwEDataSource.ToString
        Return Me.m_strFilename
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

#Region " Ecopath "

#Region " Load "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Initiates a full load of an ecopath model.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Public Function LoadModel() As Boolean _
        Implements IEcopathDataSource.LoadModel

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim i As Integer

        If Me.LoadEcopath() Then

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

            Me.LoadSpaceScenarioDefinitions()
            ' Make sure that the core knows not to expect anything else
            ecopathDS.RedimEcospaceScenarios()
            ecopathDS.RedimEcotracerScenarios()


            ' Invoke plugin point
            If (Me.m_core.PluginManager IsNot Nothing) Then Me.m_core.PluginManager.LoadModel(Me)

            Return True

        End If

        Return False

    End Function

    Private Function LoadEcopath() As Boolean

        'read the contents of the eii file into an EcopathParameters object
        'this is written using vb file access instead of a filestream to keep it as close to the original vb code as possible
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim psdDS As cPSDDatastructures = Me.m_core.m_PSDData
        Dim pvar As Single
        Dim i As Integer
        Dim j As Integer
        Dim K As Integer

        Dim quotes() As Char = {CChar(""""), CChar(" ")}
        Dim eiiStrm As System.IO.StreamReader

        If Not File.Exists(m_strFilename) Then
            cLog.Write(Me.ToString + ".LoadEcopath(...) No file name specified.")
            Return False
        End If

        Try
            eiiStrm = New System.IO.StreamReader(m_strFilename)
        Catch ex As Exception
            cLog.Write(Me.ToString + ".LoadEcopath(...) Error opening eii file. '" & Me.m_strFilename & "' Error:" + ex.Message())
            Return False
        End Try

        'fake model data
        ecopathDS.ModelDBID = 1
        ecopathDS.ModelName = Path.GetFileName(m_strFilename)
        ecopathDS.ModelNumDigits = 3
        ecopathDS.ModelDescription = "Model read from EII file " & Me.m_strFilename

        'read the file
        Try
            Dim buff As String
            Dim recs() As String
            buff = eiiStrm.ReadLine()
            recs = buff.Split(CChar(","))

            Integer.TryParse(recs(0), ecopathDS.NumGroups)
            Integer.TryParse(recs(1), ecopathDS.NumLiving)
            ecopathDS.ModelUnitCurrencyCustom = recs(2)
            Integer.TryParse(recs(3), ecopathDS.ModelUnitCurrency)

            If Not ecopathDS.redimGroupVariables() Or Not psdDS.redimGroupVariables() Then
                cLog.Write(Me.ToString + ".LoadModel(...) Failed to Re-Dimension group parameter arrays.")
                Return False
            End If
            Dim iNextIndex As Integer

            ' Read groups
            For K = 1 To ecopathDS.NumGroups

                ' Replace double spaces with single space
                buff = eiiStrm.ReadLine().Replace("  ", " ")
                recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
                iNextIndex = 0

                'Debug.Assert(data.Length = 10, "EII DataSource wrong number of recs in group section.")
                ecopathDS.GroupName(K) = Me.GetNextValue(recs, iNextIndex).Trim(quotes)
                Single.TryParse(Me.GetNextValue(recs, iNextIndex), pvar)
                Single.TryParse(Me.GetNextValue(recs, iNextIndex), ecopathDS.DtImp(K))
                Single.TryParse(Me.GetNextValue(recs, iNextIndex), ecopathDS.Ex(K))
                Single.TryParse(Me.GetNextValue(recs, iNextIndex), ecopathDS.fCatch(K))
                Single.TryParse(Me.GetNextValue(recs, iNextIndex), ecopathDS.DC(K, 0))
                Single.TryParse(Me.GetNextValue(recs, iNextIndex), ecopathDS.Binput(K))
                Single.TryParse(Me.GetNextValue(recs, iNextIndex), ecopathDS.PBinput(K))
                Single.TryParse(Me.GetNextValue(recs, iNextIndex), ecopathDS.EEinput(K))
                Single.TryParse(Me.GetNextValue(recs, iNextIndex), ecopathDS.GEinput(K))
                Single.TryParse(Me.GetNextValue(recs, iNextIndex), ecopathDS.QBinput(K))

                ecopathDS.BHinput(K) = ecopathDS.Binput(K) / ecopathDS.Area(K)
                ecopathDS.GroupDBID(K) = K
                ecopathDS.PP(K) = pvar - 2

                If K > ecopathDS.NumLiving Then ecopathDS.PP(K) = 2
                If ecopathDS.GE(K) = 0 Then ecopathDS.GE(K) = cCore.NULL_VALUE

            Next K


            ' Read DietComp
            ReDim ecopathDS.DietChanged(1, 0)
            For K = 1 To ecopathDS.NumGroups
                ' Replace double spaces with single space
                buff = eiiStrm.ReadLine().Replace("  ", " ")
                recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
                iNextIndex = 0
                For j = 1 To ecopathDS.NumGroups

                    Single.TryParse(Me.GetNextValue(recs, iNextIndex), ecopathDS.DCInput(K, j))
                    ' Input(fnum, ecopathDS.DCInput(K, j))
                    If ecopathDS.DCInput(K, j) > 0 Then
                        ecopathDS.DietWasChanged(K, j)
                    End If
                Next j
            Next K

            If eiiStrm.EndOfStream Then Return True
            'If EOF(fnum) Then Return True

            'junk 
            buff = eiiStrm.ReadLine()

            ''jb totp read in original routine using a string will read the entire line
            'Input(fnum, jnk)
            ''jb I have no idea what this is all about 
            'If Import < 0 Then Import = 0

            ''Unassimilated food
            'Data looks like this
            '-91  20  -91  20  -91  20  -91  20  -91  20  -91  20  -91  20  -91  20  -91  0  -92  0 
            buff = eiiStrm.ReadLine().Replace("  ", " ")
            recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
            Dim iRec As Integer = 1
            For j = 1 To ecopathDS.NumGroups

                Single.TryParse(recs(iRec), ecopathDS.GS(j))
                iRec += 2
                ecopathDS.GS(j) = ecopathDS.GS(j)
                If ecopathDS.GS(j) > 1 Then ecopathDS.GS(j) = ecopathDS.GS(j) / 100
            Next j

            'junk
            buff = eiiStrm.ReadLine()
            'Input(fnum, jnk)

            buff = eiiStrm.ReadLine().Replace("  ", " ")
            recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")

            ''the time unit name
            ecopathDS.TimeUnitName = recs(0)
            If ecopathDS.TimeUnitName.Contains("year") Then
                ecopathDS.ModelUnitTime = eUnitTimeType.Year
            ElseIf ecopathDS.TimeUnitName.Contains("day") Then
                ecopathDS.ModelUnitTime = eUnitTimeType.Day
            End If

            'the ecosystem remarks.
            'junk
            buff = eiiStrm.ReadLine()

            'parms.Bomass accumulation added March 95/VC
            '-91  20  -91  20  -91  20  -91  20  -91  20  -91  20  -91  20  -91  20  -91  0  -92  0 
            buff = eiiStrm.ReadLine().Replace("  ", " ")
            recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
            For i = 1 To ecopathDS.NumGroups
                Single.TryParse(recs(i - 1), ecopathDS.BA(i))
            Next i

            ' Diet Fate array added July 1994/VC
            'If EOF(fnum) = False And NumGroups > NumLiving + 1 Then
            'More than 1 detritusbox Any reason for this??
            For i = 1 To ecopathDS.NumGroups
                buff = eiiStrm.ReadLine().Replace("  ", " ")
                recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
                For j = ecopathDS.NumLiving + 1 To ecopathDS.NumGroups
                    Single.TryParse(recs(j - ecopathDS.NumLiving - 1), ecopathDS.DF(i, j - ecopathDS.NumLiving))
                    ' Input(fnum, ecopathDS.DF(i, j - ecopathDS.NumLiving))    
                Next j
            Next i

            ' Emigration added Dec 98/VC
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("Emigration"), "EII datasource file format may be wrong!")
            buff = eiiStrm.ReadLine().Replace("  ", " ")
            recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
            'Input(fnum, jnk) ' 
            For i = 1 To ecopathDS.NumGroups
                Single.TryParse(recs(i - 1), ecopathDS.Emigration(i))
                ' Input(fnum, ecopathDS.Emigration(i))
            Next i

            'immigration added Dec 98/VC
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("Immig"), "EII datasource file format may be wrong!")
            buff = eiiStrm.ReadLine().Replace("  ", " ")
            recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
            For i = 1 To ecopathDS.NumGroups
                Single.TryParse(recs(i - 1), ecopathDS.Immig(i))
                ' Input(fnum, ecopathDS.Immig(i))
            Next i

            'NumGear
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("NumGear"), "EII datasource file format may be wrong!")
            buff = eiiStrm.ReadLine().Replace("  ", " ")
            Integer.TryParse(buff, ecopathDS.NumFleet)
            ecopathDS.RedimFleetVariables(True)

            'Gearnames
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("Gearnames"), "EII datasource file format may be wrong!")
            For i = 1 To ecopathDS.NumFleet
                buff = eiiStrm.ReadLine().Replace("  ", " ")
                ecopathDS.FleetName(i) = buff.Trim(quotes) ' Added Dec 98/VC
                '  Input(fnum, ecopathDS.FleetName(i))
                ecopathDS.FleetDBID(i) = i
            Next i

            'cost
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("cost"), "EII datasource file format may be wrong!")
            'Input(fnum, jnk)  
            For i = 1 To ecopathDS.NumFleet
                'First is fixed cost, second is cost per unit effort' Added Dec 98/VC
                buff = eiiStrm.ReadLine().Replace("  ", " ")
                recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
                Single.TryParse(recs(0), ecopathDS.CostPct(i, eCostIndex.Fixed))
                Single.TryParse(recs(1), ecopathDS.CostPct(i, eCostIndex.CUPE))
                Single.TryParse(recs(2), ecopathDS.CostPct(i, eCostIndex.Sail))
            Next i

            'landing
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("landing"), "EII datasource file format may be wrong!")
            'Input(fnum, jnk)  
            For i = 1 To ecopathDS.NumFleet
                buff = eiiStrm.ReadLine().Replace("  ", " ")
                recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
                For j = 1 To ecopathDS.NumGroups
                    Single.TryParse(recs(j - 1), ecopathDS.Landing(i, j))
                    '  Input(fnum, ecopathDS.Landing(i, j))    ' Landing added Dec 98/VC
                Next j
            Next i

            'discard
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("Discard"), "EII datasource file format may be wrong!")
            'Input(fnum, jnk)  
            For i = 1 To ecopathDS.NumFleet
                buff = eiiStrm.ReadLine().Replace("  ", " ")
                recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
                For j = 1 To ecopathDS.NumGroups
                    Single.TryParse(recs(j - 1), ecopathDS.Discard(i, j))
                    '  Input(fnum, ecopathDS.Landing(i, j))    ' Landing added Dec 98/VC
                Next j
            Next i

            'discard fate
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("DiscardFate"), "EII datasource file format may be wrong!")
            For i = 1 To ecopathDS.NumFleet
                buff = eiiStrm.ReadLine().Replace("  ", " ")
                recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
                For j = 1 To ecopathDS.NumGroups - ecopathDS.NumLiving
                    Single.TryParse(recs(j - 1), ecopathDS.DiscardFate(i, j))
                    ' Input(fnum, ecopathDS.DiscardFate(i, j))   ' Added Dec 98/VC
                Next j
            Next i

            'market
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("Market"), "EII datasource file format may be wrong!")
            'Input(fnum, jnk)  
            For i = 1 To ecopathDS.NumFleet
                buff = eiiStrm.ReadLine().Replace("  ", " ")
                recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
                For j = 1 To ecopathDS.NumGroups
                    Single.TryParse(recs(j - 1), ecopathDS.Market(i, j))
                    '  Input(fnum, ecopathDS.Landing(i, j))    ' Landing added Dec 98/VC
                Next j
            Next i

            'ecopathDS.NoGearData = False

            ''shadow
            'Input(fnum, jnk)
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("Shadow"), "EII datasource file format may be wrong!")
            buff = eiiStrm.ReadLine().Replace("  ", " ")
            recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
            For i = 1 To ecopathDS.NumGroups             ' Added Dec 98/VC
                Single.TryParse(recs(i - 1), ecopathDS.Shadow(i))
                '  Input(fnum, ecopathDS.Shadow(i))
            Next i

            ''Habitatarea
            buff = eiiStrm.ReadLine()
            Debug.Assert(buff.Contains("Area&HabitatBiomass(BH)"), "EII datasource file format may be wrong!")
            buff = eiiStrm.ReadLine().Replace("  ", " ")
            recs = EwEUtils.Utilities.cStringUtils.SplitQualified(buff, " ")
            iRec = 0
            For i = 1 To ecopathDS.NumGroups
                Single.TryParse(recs(iRec), ecopathDS.Area(i))
                iRec += 1
                Single.TryParse(recs(iRec), ecopathDS.BH(i))
                iRec += 1
            Next i

            eiiStrm.Close()
            ecopathDS.RedimPedigree()

        Catch ex As Exception 'catch any error during the reading of the data
            'FileClose(fnum)
            'some kind of a reading error better find out what happend
            cLog.Write(Me.ToString + ".LoadEcopath() Error reading eii file. Error: " + ex.Message())
            Debug.Assert(False)
            Return False
        End Try

        Return True

    End Function

#End Region ' Load

#Region " Save "

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

#End Region ' Save

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
    Function AddGroup(ByVal strGroupName As String, ByVal sPP As Single, ByVal sVBK As Single, _
                      ByVal iPosition As Integer, ByRef iDBID As Integer) As Boolean _
            Implements IEcopathDataSource.AddGroup
        Return False
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
        Return False
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

#Region " Pedigree "

    Public Function AddPedigreeLevel(ByVal iPosition As Integer, ByVal strName As String, ByVal iColor As Integer, ByVal strDescription As String, ByVal varName As eVarNameFlags, ByVal sIndexValue As Single, ByVal sConfidence As Single, ByRef iDBID As Integer) As Boolean _
     Implements DataSources.IEcopathDataSource.AddPedigreeLevel
        Return False
    End Function

    Public Function MovePedigreeLevel(ByVal iDBID As Integer, ByVal iPosition As Integer) As Boolean Implements DataSources.IEcopathDataSource.MovePedigreeLevel
        Return False
    End Function

    Public Function RemovePedigreeLevel(ByVal iDBID As Integer) As Boolean Implements DataSources.IEcopathDataSource.RemovePedigreeLevel
        Return False
    End Function

#End Region ' Pedigree

#Region " Taxon "

    Public Function AddTaxon(ByVal iTargetDBID As Integer, ByVal bIsStanza As Boolean, ByVal data As ITaxonSearchData, ByVal sProportion As Single, ByRef iDBID As Integer) As Boolean _
        Implements DataSources.IEcopathDataSource.AddTaxon
        Return False
    End Function

    Public Function RemoveTaxon(ByVal iTaxonID As Integer) As Boolean _
        Implements DataSources.IEcopathDataSource.RemoveTaxon
        Return False
    End Function

#End Region ' Taxon

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

    Private Function LoadSpaceScenarioDefinitions() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        ecopathDS.NumEcospaceScenarios = 1

        ecopathDS.RedimEcospaceScenarios()
        ecopathDS.EcospaceScenarioName(1) = My.Resources.CoreDefaults.CORE_DEFAULT_SCENARIO()
        ecopathDS.EcospaceScenarioDBID(1) = 1
        ecopathDS.EcospaceScenarioDescription(1) = "This is a dummy scenario, manually crafted in cEIIDataSource."

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
        ecosimDS.nGear = ecopathDS.NumFleet
        ecosimDS.NumYears = 50

        ecosimDS.RedimVars()
        ecosimDS.RedimTime()
        ecosimDS.SetDefaultParameters()

        ecopathDS.ActiveEcosimScenario = 1

        ecosimDS.DimForcingShapes()
        ecosimDS.InitForcingShapes()
        ecosimDS.BioMedData.ReDimMediation(ecopathDS.NumGroups, ecopathDS.NumFleet)
        ecosimDS.PriceMedData.ReDimMediation(ecopathDS.NumGroups, ecopathDS.NumFleet)

        Me.m_core.m_MSEData.redimTime()
        Me.m_core.m_MSEData.RedimVars()

        For igrp As Integer = 1 To ecopathDS.NumGroups
            Me.m_core.m_MSEData.DefaultBioBounds(igrp)
            Me.m_core.m_MSEData.DefaultCatchBoundsGroup(igrp)
        Next igrp

        For iflt As Integer = 1 To ecopathDS.NumFleet
            Me.m_core.m_MSEData.DefaultCatchBoundsFleet(iflt)
        Next iflt

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
        For i = 1 To ecosimDS.BioMedData.MediationShapes
            ecosimDS.BioMedData.MediationDBIDs(i) = i
        Next
        For i = 1 To ecosimDS.PriceMedData.MediationShapes
            ecosimDS.PriceMedData.MediationDBIDs(i) = i
        Next

        'fake database IDs
        For i = 1 To ecosimDS.nGroups
            ecosimDS.GroupDBID(i) = i
        Next

        For i = 1 To ecosimDS.nGear
            ecosimDS.FleetDBID(i) = i
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
            ByVal asData As Single(), ByVal sYZero As Single, ByVal sYBase As Single, ByVal sYend As Single, ByVal sSteep As Single, ByVal functionType As Long) As Boolean _
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
            Return False
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
    ''' <inheritdocs cref="IEcosimDatasource.AppendTimeSeriesDataset"/>
    ''' <returns>Always false.</returns>
    ''' -------------------------------------------------------------------
    Public Function AppendTimeSeriesDataset(ByVal strDatasetName As String, ByVal strDescription As String, _
                                            ByVal strAuthor As String, ByVal strContact As String, _
                                            ByVal iFirstYear As Integer, ByVal iNumPoints As Integer, ByVal interval As eTSDataSetInterval, _
                                            ByRef iDatasetID As Integer) As Boolean _
        Implements DataSources.IEcosimDatasource.AppendTimeSeriesDataset
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

#Region "Ecospace"

    Public Function LoadEcospaceScenario(ByVal iDBID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.LoadEcospaceScenario
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecospaceDS As cEcospaceDataStructures = Me.m_core.m_EcoSpaceData
        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim spatialDS As SpatialData.cSpatialDataStructures = Me.m_core.m_SpatialData
        'Dim reader As IDataReader = Nothing
        Dim bSucces As Boolean = True

        'jb Jan-17-07 moved SetDefaults to run before any data has been loaded
        'this will load the default values into Ecospace before anything else is loaded
        ecospaceDS.NGroups = ecopathDS.NumGroups
        ecospaceDS.nFleets = ecopathDS.NumFleet
        ecospaceDS.nLiving = ecopathDS.NumLiving
        ecospaceDS.nImportanceLayers = 0 'CInt(Me.m_db.GetValue(String.Format("SELECT COUNT(*) FROM EcospaceScenarioWeightLayer WHERE ScenarioID={0}", iScenarioID), 0))
        ecospaceDS.nEnvironmentalDriverLayers = 0 'CInt(Me.m_db.GetValue(String.Format("SELECT COUNT(*) FROM EcospaceScenarioDriverLayer WHERE ScenarioID={0}", iScenarioID), 0))

        ' Next is a dangerous solution that may need to be revamped. It is assumed that
        ' SetDefaults properly redimensions the ecospaceDS group variables, which
        ' may wreck havoc if the implementation of SetDefaults were to change.
        ecospaceDS.SetDefaults()
        spatialDS.SetDefaults()

        Try
            ' Remember link with Ecosim scenario, if any
            ecospaceDS.EcosimScenarioDBID = 1
            ecospaceDS.InRow = 320
            ecospaceDS.InCol = 720
            ecospaceDS.CellLength = 100
            ecospaceDS.Lat1 = 0
            ecospaceDS.Lon1 = 0
            ecospaceDS.TimeStep = 1 / 12
            ecospaceDS.PredictEffort = True

            ' JS 05apr08: pragmatic fix to prevent mayhem
            If ecospaceDS.TimeStep <= 0 Then ecospaceDS.TimeStep = 1.0! / cCore.N_MONTHS

            ecospaceDS.TotalTime = 50
            ecospaceDS.IFDPower = 0.5
            ecospaceDS.nSpaceSolverThreads = 1
            ecospaceDS.nGridSolverThreads = 1
            ecospaceDS.nEffortDistThreads = 1
            ecospaceDS.nRegions = 0
            ecospaceDS.AdjustSpace = True
            ecospaceDS.UseExact = False
            ' ecospaceDS.Tol = CSng(Me.m_db.ReadSafe(reader, "Tolerance", 0.01!))

            ecospaceDS.NewMultiStanza = False
            ecospaceDS.UseIBM = False

        Catch ex As Exception
            bSucces = False
        End Try

        ' JS 08Jl14: redimForRun is called too many times

        'set the size of the variables that hold the map data to InRow and InCol
        'Call cEcospace.redimForRun() First because it allocates bigger blocks of memory
        'this should help Out of Memory exceptions caused by heap fragmentation by doing the big stuff first
        Me.m_core.m_Ecospace.redimForRun()
        ecospaceDS.ReDimMapDims()

        ' Set active scenario
        ecopathDS.ActiveEcospaceScenario = 1

        For i As Integer = 1 To ecospaceDS.NGroups
            ecospaceDS.GroupDBID(i) = i
            ecospaceDS.CapCalType(i) = eEcospaceCapacityCalType.EnvResponses
        Next

        For i As Integer = 1 To ecospaceDS.nFleets
            ecospaceDS.FleetDBID(i) = i
        Next


        For irow As Integer = 1 To ecospaceDS.InRow
            For icol As Integer = 1 To ecospaceDS.InCol
                For igrp As Integer = 1 To ecospaceDS.NGroups
                    ecospaceDS.HabCapInput(irow, icol, igrp) = 1
                Next
            Next
        Next




        ' Load base map first
        'bSucces = bSucces And Me.LoadEcospaceMap(iScenarioID)
        'bSucces = bSucces And Me.LoadEcospaceHabitats(iScenarioID)
        'bSucces = bSucces And Me.LoadEcospaceMPAs(iScenarioID)
        'bSucces = bSucces And Me.LoadEcospaceGroups(iScenarioID)
        'bSucces = bSucces And Me.LoadEcospaceFleets(iScenarioID)
        'bSucces = bSucces And Me.LoadEcospaceWeightLayers(iScenarioID)
        'bSucces = bSucces And Me.LoadEcospaceDriverLayers(iScenarioID)
        'bSucces = bSucces And Me.LoadEcospaceDataAdapters(iScenarioID)
        'bSucces = bSucces And Me.LoadAuxillaryData()

        'Me.ClearChanged(s_EcospaceComponents)

        Return bSucces
    End Function

    Public Function AddEcospaceDriverLayer(ByVal strName As String, ByVal strDescription As String, ByRef iDBID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.AddEcospaceDriverLayer
        Return False
    End Function

    Public Function AddEcospaceHabitat(ByVal strHabitatName As String, ByRef iHabitatID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.AddEcospaceHabitat
        Return False
    End Function

    Public Function AppendEcospaceImportanceLayer(ByVal strName As String, ByVal strDescription As String, ByVal sWeight As Single, ByRef iDBID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.AppendEcospaceImportanceLayer
        Return False
    End Function

    Public Function AppendEcospaceMPA(ByVal strScenarioName As String, ByVal bMPAMonths() As Boolean, ByRef iDBID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.AppendEcospaceMPA
        Return False
    End Function

    Public Function AppendEcospaceScenario(ByVal strScenarioName As String, ByVal strDescription As String, ByVal strAuthor As String, ByVal strContact As String, ByVal InRow As Integer, ByVal InCol As Integer, ByVal sOriginLat As Single, ByVal sOriginLon As Single, ByVal sCellLength As Single, ByRef iDBID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.AppendEcospaceScenario
        Return False
    End Function

    Public Overloads Function CopyTo(ByVal ds As DataSources.IEcospaceDatasource) As Boolean Implements DataSources.IEcospaceDatasource.CopyTo
        Return False
    End Function

    Public Function IsEcospaceModified() As Boolean Implements DataSources.IEcospaceDatasource.IsEcospaceModified
        Return False
    End Function

    Public Function RemoveEcospaceDriverLayer(ByVal iDBID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.RemoveEcospaceDriverLayer
        Return False
    End Function

    Public Function MoveEcospaceDriverLayer(iDBID As Integer, iPosition As Integer) As Boolean Implements DataSources.IEcospaceDatasource.MoveEcospaceDriverLayer
        Return False
    End Function

    Public Function RemoveEcospaceHabitat(ByVal iHabitatID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.RemoveEcospaceHabitat
        Return False
    End Function

    Public Function RemoveEcospaceImportanceLayer(ByVal iDBID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.RemoveEcospaceImportanceLayer
        Return False
    End Function

    Public Function RemoveEcospaceMPA(ByVal iDBID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.RemoveEcospaceMPA
        Return False
    End Function

    Public Function RemoveEcospaceScenario(ByVal iDBID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.RemoveEcospaceScenario
        Return False
    End Function

    Public Function ResizeEcospaceBasemap(ByVal InRow As Integer, ByVal InCol As Integer) As Boolean Implements DataSources.IEcospaceDatasource.ResizeEcospaceBasemap
        Return False
    End Function

    Public Function SaveEcospaceScenario(ByVal iDBID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.SaveEcospaceScenario
        Return False
    End Function

    Public Function SaveEcospaceScenarioAs1(ByVal strScenarioName As String, ByVal strDescription As String, ByVal strAuthor As String, ByVal strContact As String, ByRef iScenarioID As Integer) As Boolean Implements DataSources.IEcospaceDatasource.SaveEcospaceScenarioAs
        Return False
    End Function

#End Region

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
        m_stanzaData.OnPostInitialization()

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
    Public Function AddStanzaLifestage(ByVal iStanzaDBID As Integer, ByVal iGroupDBID As Integer, _
                                       ByVal iStartAge As Integer, ByVal sMortality As Single) As Boolean _
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

#Region " Interface Implementations "

    Public Function Compact(ByVal strTarget As String) As eDatasourceAccessType _
        Implements DataSources.IEwEDataSource.Compact
        Return eDatasourceAccessType.Failed_OSUnsupported
    End Function

    Public Function CanCompact(ByVal strTarget As String) As Boolean _
    Implements IEwEDataSource.CanCompact
        Return False
    End Function

    Public Function IsOSSupported(ByVal dst As EwEUtils.Core.eDataSourceTypes) As Boolean _
        Implements IEwEDataSource.IsOSSupported
        Return True ' We can do this!
    End Function

    Public Function Directory() As String Implements DataSources.IEwEDataSource.Directory
        Return Path.GetDirectoryName(Me.m_strFilename)
    End Function

    Public Function Extension() As String Implements DataSources.IEwEDataSource.Extension
        Return Path.GetExtension(Me.m_strFilename)
    End Function

    Public Function FileName() As String Implements DataSources.IEwEDataSource.FileName
        Return Path.GetFileName(Me.m_strFilename)
    End Function

    Public Function IsReadOnly() As Boolean Implements DataSources.IEwEDataSource.IsReadOnly
        Return True
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        GC.SuppressFinalize(Me)
    End Sub

#End Region ' Interface Implementations

#Region " Helper methods "

    Private Function GetNextValue(ByVal data() As String, ByRef iNextIndex As Integer) As String
        Dim strData As String = ""
        Do While String.IsNullOrWhiteSpace(strData)
            strData = data(iNextIndex)
            iNextIndex += 1
        Loop
        Return strData
    End Function

#End Region ' Helper methods

#Region " Methods replaced for Mono compatibility "

#If 0 Then

    Private Function LoadEII_org() As Boolean
    'The original EI reading used VB IO 
    'this was replaced with System.IO stream classes
        'read the contents of the eii file into an EcopathParameters object
        'this is written using vb file access instead of a filestream to keep it as close to the original vb code as possible
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim psdDS As cPSDDatastructures = Me.m_core.m_PSDData
        Dim pvar As Single
        Dim i As Integer
        Dim j As Integer
        Dim K As Integer
        Dim Dummy As Single
        Dim jnk As String
        Dim Import As Integer
        Dim fnum As Integer

        fnum = FreeFile()

        If m_strFilename = "" Then
            cLog.Write(Me.ToString + ".LoadEcopath(...) No file name specified.")
            Return False
        End If

        Try
            FileOpen(fnum, m_strFilename, OpenMode.Input)
        Catch ex As Exception
            cLog.Write(Me.ToString + ".LoadEcopath(...) Error opening eii file. " + vbCrLf + m_strFilename + vbCrLf + "Error:" + ex.Message())
            Return False
        End Try

        'fake model data
        ecopathDS.ModelDBID = 1
        ecopathDS.ModelName = Path.GetFileName(m_strFilename)
        ecopathDS.ModelNumDigits = 3
        ecopathDS.ModelDescription = "Simulated model read from EII file " & m_strFilename

        'read the file
        Try
            Input(fnum, ecopathDS.NumGroups)
            Input(fnum, ecopathDS.NumLiving)
            Input(fnum, ecopathDS.ModelUnitCurrencyCustom)
            Input(fnum, ecopathDS.currUnitIndex)

            If Not ecopathDS.redimGroupVariables() Or Not psdDS.redimGroupVariables() Then
                cLog.Write(Me.ToString + ".LoadModel(...) Failed to Re-Dimension group parameter arrays.")
                Return False
            End If

            'groups
            For K = 1 To ecopathDS.NumGroups
                Input(fnum, ecopathDS.GroupName(K)) : Input(fnum, pvar) : Input(fnum, ecopathDS.DtImp(K))
                Input(fnum, ecopathDS.Ex(K)) : Input(fnum, ecopathDS.fCatch(K)) : Input(fnum, ecopathDS.DC(K, 0))
                Input(fnum, ecopathDS.Binput(K)) : Input(fnum, ecopathDS.PBinput(K)) : Input(fnum, ecopathDS.EEinput(K))
                Input(fnum, ecopathDS.GEinput(K)) : Input(fnum, ecopathDS.QBinput(K))

                ecopathDS.BHinput(K) = ecopathDS.Binput(K) / ecopathDS.Area(K)

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
                '        + "If not, please email v.christensen@cgiar.org " + Environment.NewLine  _
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
                    Input(fnum, ecopathDS.DCInput(K, j))
                    If ecopathDS.DCInput(K, j) > 0 Then
                        ecopathDS.DietWasChanged(K, j)
                    End If
                Next j
            Next K

            If EOF(fnum) Then Return True

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
                        ecopathDS.ModelUnitTime = eUnitTimeType.Year
                    Case "day"
                        ecopathDS.ModelUnitTime = eUnitTimeType.Day
                    Case Else
                        ecopathDS.ModelUnitTime = eUnitTimeType.Custom
                        ecopathDS.ModelUnitTimeCustom = ecopathDS.TimeUnitName

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

            ecopathDS.RedimPedigree()

        Catch ex As Exception 'catch any error during the reading of the data
            FileClose(fnum)
            'some kind of a reading error better find out what happend
            cLog.Write(Me.ToString + ".LoadEcopath() Error reading eii file. Error: " + ex.Message())
            Debug.Assert(False)
            Return False
        End Try

        Return True

    End Function

#End If

#End Region ' Methods replaced for Mono compatibility

End Class

