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
Imports EwECore.DataSources
Imports EwEPlugin
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports System.Xml
Imports System.Text
Imports System.Data.OleDb
Imports EwEUtils.Utilities
Imports EwECore.MSE

'
#End Region ' Imports

''' ===========================================================================
''' <summary>
''' Data access for an EwE6 .eiixml file
''' </summary>
''' ===========================================================================
Public Class cEIIXMLDataSource
    Implements IEwEDataSource
    Implements IEcopathDataSource
    Implements IEcosimDatasource

    Private m_strFilename As String = ""
    Private m_core As cCore = Nothing
    Private m_doc As XmlDocument = Nothing

    Private Shared s_dtExcludedDBEntries As New Dictionary(Of String, String())

    Public Sub New()
        s_dtExcludedDBEntries("EcopathGroup") = New String() {"PoolColor"}
        s_dtExcludedDBEntries("EcopathFleet") = New String() {"PoolColor"}
        s_dtExcludedDBEntries("Auxillary") = New String() {}
        s_dtExcludedDBEntries("Quotes") = New String() {}
        s_dtExcludedDBEntries("UpdateLog") = New String() {}
        s_dtExcludedDBEntries("Pedigree") = New String() {}
        s_dtExcludedDBEntries("EcopathGroupPedigree") = New String() {}
        s_dtExcludedDBEntries("Taxon") = New String() {}
        s_dtExcludedDBEntries("EcopathGroupTaxon") = New String() {}
        s_dtExcludedDBEntries("EcopathStanzaTaxon") = New String() {}

        ' Exclude value chain
        s_dtExcludedDBEntries("cUnit") = New String() {}
        s_dtExcludedDBEntries("cConsumerUnitDefault") = New String() {}
        s_dtExcludedDBEntries("cConsumerUnit") = New String() {}
        s_dtExcludedDBEntries("cDistributionUnit") = New String() {}
        s_dtExcludedDBEntries("cDistributionUnitDefault") = New String() {}
        s_dtExcludedDBEntries("cEconomicUnit") = New String() {}
        s_dtExcludedDBEntries("cFlowDiagram") = New String() {}
        s_dtExcludedDBEntries("cFlowPosition") = New String() {}
        s_dtExcludedDBEntries("cLink") = New String() {}
        s_dtExcludedDBEntries("cLinkDefault") = New String() {}
        s_dtExcludedDBEntries("cLinkLandings") = New String() {}
        s_dtExcludedDBEntries("cOOPStorable") = New String() {}
        s_dtExcludedDBEntries("cParameters") = New String() {}
        s_dtExcludedDBEntries("cProcessingUnit") = New String() {}
        s_dtExcludedDBEntries("cProcessingUnitDefault") = New String() {}
        s_dtExcludedDBEntries("cProducerUnit") = New String() {}
        s_dtExcludedDBEntries("cProducerUnitDefault") = New String() {}
        s_dtExcludedDBEntries("cRetailerUnit") = New String() {}
        s_dtExcludedDBEntries("cRetailerUnitDefault") = New String() {}
        s_dtExcludedDBEntries("cWholesalerUnit") = New String() {}
        s_dtExcludedDBEntries("cWholesalerUnitDefault") = New String() {}
    End Sub

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

        If (String.IsNullOrWhiteSpace(strName)) Then Return eDatasourceAccessType.Failed_UnknownType
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
        Me.m_doc = Nothing

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

        Dim strEwEVersion As String
        Dim sDBVersion As Single
        Dim strModel As String = ""
        Dim bSucces As Boolean = True

        If (Me.m_core Is Nothing) Then Return False

        Me.ClearChanged()

        Me.m_doc = New XmlDocument()
        Dim xnModel As XmlNode = Nothing

        Try
            Me.m_doc.Load(Me.m_strFilename)
            xnModel = Me.m_doc.SelectSingleNode("EwEModel")
            Try
                sDBVersion = Single.Parse(xnModel.Attributes("DBVersion").InnerText)
            Catch ex As Exception
                sDBVersion = 6.120011
            End Try

            Try
                strEwEVersion = xnModel.Attributes("EwEVersion").InnerText
            Catch ex As Exception

            End Try

            bSucces = Me.LoadModelInfo()
            If bSucces = False Then Return False

            bSucces = bSucces And Me.LoadEcopathGroups()
            bSucces = bSucces And Me.LoadEcopathTaxon()
            bSucces = bSucces And Me.LoadEcopathFleetInfo()
            'bSucces = bSucces And Me.LoadParticleSizeDistribution()
            bSucces = bSucces And Me.LoadPedigreeLevels()
            bSucces = bSucces And Me.LoadPedigreeAssignments()

            'bSucces = bSucces And Me.LoadAuxillaryData()

            ecopathDS.bInitialized = bSucces
            ecopathDS.onPostInitialization()

            bSucces = bSucces And Me.LoadEcosimScenarioDefinitions()
            bSucces = bSucces And Me.LoadEcospaceScenarioDefinitions()
            bSucces = bSucces And Me.LoadEcotracerScenarioDefinitions()
            bSucces = bSucces And Me.LoadTimeSeriesDatasets()

            ' Clear changed admin
            Me.ClearChanged()

            Return bSucces
        Catch ex As Exception
            Return False
        End Try

        Return False

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, loads model info for the current model.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function LoadModelInfo() As Boolean

        Dim dt As DataTable = Me.ReadTable("EcopathModel")
        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim sVal1 As Single = 0.0!
        Dim sVal2 As Single = 0.0!
        Dim bSucces As Boolean = True

        ' Crash prevention check
        If Object.ReferenceEquals(dt, Nothing) Then
            'Debug.Assert(False, "Failed to access table EcopathModel")
            Return False
        End If

        Try
            ' There is only one model in an EwE6 database
            For Each row As DataRow In dt.Rows

                ecopathDS.ModelDBID = CInt(row("ModelID"))
                ecopathDS.ModelName = CStr(row("Name"))
                ecopathDS.ModelDescription = CStr(row("Description"))
                ecopathDS.ModelAuthor = CStr(Me.ReadSafe(row, "Author", ""))
                ecopathDS.ModelContact = CStr(Me.ReadSafe(row, "Contact", ""))
                ecopathDS.ModelArea = CSng(Me.ReadSafe(row, "Area", 1.0))
                ecopathDS.ModelNumDigits = CInt(row("NumDigits"))
                ecopathDS.ModelGroupDigits = (CInt(Me.ReadSafe(row, "GroupDigits", False)) <> 0)
                ecopathDS.ModelUnitCurrency = DirectCast(CInt(Me.ReadSafe(row, "UnitCurrency", eUnitCurrencyType.WetWeight)), eUnitCurrencyType)
                ecopathDS.ModelUnitCurrencyCustom = CStr(Me.ReadSafe(row, "UnitCurrencyCustom", ""))
                ecopathDS.ModelUnitTime = DirectCast(CInt(Me.ReadSafe(row, "UnitTime", eUnitTimeType.Year)), eUnitTimeType)
                ecopathDS.ModelUnitTimeCustom = CStr(Me.ReadSafe(row, "UnitTimeCustom", ""))
                ecopathDS.ModelUnitMonetary = DirectCast(Me.ReadSafe(row, "UnitMonetary", "EUR"), String)
                'ecopathDS.m_EwEModelUnitMonetaryCustom = CStr(Me.ReadSafe(row, "UnitTimeCustom", ""))
                ecopathDS.FirstYear = CInt(Me.ReadSafe(row, "FirstYear", 0))
                ecopathDS.NumYears = Math.Max(1, CInt(Me.ReadSafe(row, "NumYears", 1)))
                'ecopathDS.ModelUnitArea = DirectCast(Me.ReadSafe(row, "UnitArea", eUnitAreaType.Km2), eUnitAreaType)
                'ecopathDS.ModelUnitAreaCustom = CStr(Me.ReadSafe(row, "UnitAreaCustom", ""))

                Dim sLat1 As Single = CSng(Me.ReadSafe(row, "MaxLat", cCore.NULL_VALUE))
                Dim sLat2 As Single = CSng(Me.ReadSafe(row, "MinLat", cCore.NULL_VALUE))
                ecopathDS.ModelNorth = Math.Max(sLat1, sLat2)
                ecopathDS.ModelSouth = Math.Min(sLat1, sLat2)

                ecopathDS.ModelWest = CSng(Me.ReadSafe(row, "MinLon", cCore.NULL_VALUE))
                ecopathDS.ModelEast = CSng(Me.ReadSafe(row, "MaxLon", cCore.NULL_VALUE))

                ecopathDS.ModelAreaName = CStr(Me.ReadSafe(row, "AreaName", ""))
                ecopathDS.ModelLastSaved = CDbl(Me.ReadSafe(row, "LastSaved", 0))

            Next

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EcopathModel", ex.Message))
            bSucces = False
        End Try

        dt.Clear()

        Return bSucces
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads Ecopath Group information.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function LoadEcopathGroups() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim psdDS As cPSDDatastructures = Me.m_core.m_PSDData
        Dim dt As DataTable = Me.ReadTable("EcopathGroup")
        Dim iGroup As Integer = 1
        Dim bSucces As Boolean = True

        ' Init data structure
        ecopathDS.NumGroups = dt.Rows.Count
        ecopathDS.NumLiving = 0
        For Each row As DataRow In dt.Rows
            If (CInt(row("Type")) <= 1) Then ecopathDS.NumLiving += 1
        Next
        ecopathDS.NumDetrit = ecopathDS.NumGroups - ecopathDS.NumLiving

        ' Allocate space
        If (Not ecopathDS.redimGroupVariables() Or Not psdDS.redimGroupVariables()) Then
            ' It would be quite remarkable to fail here... log message?
            Return False
        End If

        dt.DefaultView.Sort = "Sequence ASC"
        For Each row As DataRow In dt.DefaultView.ToTable.Rows

            Try
                ecopathDS.GroupDBID(iGroup) = CInt(row("GroupID"))
                ecopathDS.GroupName(iGroup) = CStr(row("GroupName"))
                ecopathDS.PP(iGroup) = CSng(row("Type"))
                ecopathDS.Area(iGroup) = CSng(row("Area"))
                ecopathDS.BH(iGroup) = ecopathDS.B(iGroup) / ecopathDS.Area(iGroup)
                ecopathDS.BA(iGroup) = CSng(row("BiomAcc"))
                ' VERIFY_JS: Check default value for BiomAccRate. 0 is assumed
                ecopathDS.BaBi(iGroup) = CSng(row("BiomAccRate"))
                ecopathDS.GS(iGroup) = CSng(row("Unassim"))
                ecopathDS.DtImp(iGroup) = CSng(row("DtImports"))
                ecopathDS.Ex(iGroup) = CSng(row("Export"))
                ecopathDS.fCatch(iGroup) = CSng(row("Catch"))
                ecopathDS.DCInput(iGroup, 0) = CSng(row("ImpVar"))
                ecopathDS.GroupIsFish(iGroup) = ParseBoolean(CStr(row("GroupIsFish")))
                ecopathDS.GroupIsInvert(iGroup) = ParseBoolean(CStr(row("GroupIsInvert")))
                ecopathDS.Shadow(iGroup) = CSng(row("NonMarketValue"))
                ecopathDS.Resp(iGroup) = CSng(row("Respiration"))
                ecopathDS.Immig(iGroup) = CSng(row("Immigration"))
                ecopathDS.Emigration(iGroup) = CSng(row("Emigration"))
                ecopathDS.Emig(iGroup) = CSng(Me.ReadSafe(row, "EmigRate", 0.0!))

                ' PSD
                ecopathDS.vbK(iGroup) = CSng(Me.ReadSafe(row, "VBK", -1))
                psdDS.AinLWInput(iGroup) = CSng(row("AinLW"))
                psdDS.BinLWInput(iGroup) = CSng(row("BinLW"))
                psdDS.LooInput(iGroup) = CSng(row("Loo"))
                psdDS.WinfInput(iGroup) = CSng(row("Winf"))
                psdDS.t0Input(iGroup) = CSng(row("t0"))
                psdDS.TcatchInput(iGroup) = CSng(row("Tcatch"))
                psdDS.TmaxInput(iGroup) = CSng(row("Tmax"))

                'variables with input output pairs
                ecopathDS.EEinput(iGroup) = CSng(row("EcoEfficiency"))
                ecopathDS.OtherMortinput(iGroup) = CSng(Me.ReadSafe(row, "OtherMort", cCore.NULL_VALUE))
                ecopathDS.PBinput(iGroup) = CSng(row("ProdBiom"))
                ecopathDS.QBinput(iGroup) = CSng(row("ConsBiom"))
                ecopathDS.GEinput(iGroup) = CSng(row("ProdCons"))
                ecopathDS.Binput(iGroup) = CSng(row("Biomass"))
                ecopathDS.BHinput(iGroup) = ecopathDS.Binput(iGroup) / ecopathDS.Area(iGroup)

                'ecopathDS.GroupColor(iGroup) = Integer.Parse(CStr(Me.ReadSafe(row, "PoolColor", "0")), Globalization.NumberStyles.HexNumber)

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading group {1}", ex.Message, ecopathDS.GroupName(iGroup)))
                bSucces = False
            End Try

            iGroup += 1

        Next

        Debug.Assert(iGroup - 1 = ecopathDS.NumGroups)

        dt.Clear()
        dt = Nothing

        bSucces = bSucces And Me.LoadEcopathDietComp()
        bSucces = bSucces And Me.LoadStanza()

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads ecopath diet composition information.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function LoadEcopathDietComp() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim dt As DataTable = Me.ReadTable("EcopathDietComp")
        Dim iPred As Integer = 0
        Dim iPrey As Integer = 0
        Dim sDiet As Single = 0.0!
        Dim bSucces As Boolean = True

        Try
            For Each row As DataRow In dt.Rows

                iPred = Array.IndexOf(ecopathDS.GroupDBID, CInt(row("PredID")))
                iPrey = Array.IndexOf(ecopathDS.GroupDBID, CInt(row("PreyID")))

                Debug.Assert(iPred >= 0 And iPrey >= 0)
                sDiet = CSng(row("Diet"))

                ' Set diet to 0 for non-living groups (fixes #878)
                If (sDiet > 0) And (iPred > ecopathDS.NumLiving) Then sDiet = 0
                ecopathDS.DCInput(iPred, iPrey) = sDiet

                If iPrey > ecopathDS.NumLiving Then
                    ecopathDS.DF(iPred, iPrey - ecopathDS.NumLiving) = CSng(row("DetritusFate"))
                End If

                ' 060528JS: ASSERT on "diet leftovers" from previous incarnations, including 041020VC fix for carbon groups
                ' The actual data fix is performed once during EwE5 import, and should not reoccur when running EwE6.
                If ecopathDS.PP(iPred) = 1 And ecopathDS.QB(iPred) <= 0 Then
                    If (ecopathDS.DCInput(iPred, iPrey) <> 0) Then
                        cLog.Write(String.Format("Database error on DCInput({0},{1})={2}, expected 0", iPred, iPrey, ecopathDS.DCInput(iPred, iPrey)))
                    End If
                End If

                ' VERIFY_JS: check mapping for MTI with JB
                ' ecopathDS.??(nPred, nPrey) = CSng(drow("MTI"))
                ' VERIFY_JS: check mapping for Electivity with JB
                ' ecopathDS.??(nPred, nPrey) = CSng(drow("Electivity"))
            Next
            dt.Clear()

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EcopathDietComp {1}, {2}", ex.Message, ecopathDS.GroupName(iPred), ecopathDS.GroupName(iPrey)))
            bSucces = False
        End Try

        ' Read 'Import'
        dt = Me.ReadTable("EcopathGroup")
        iPred = 1
        For Each row As DataRow In dt.Rows
            If CSng(row("ImpVar")) > 0 Then ecopathDS.DCInput(iPred, 0) = CSng(row("ImpVar"))
            iPred += 1
        Next
        dt.Clear()

        Return True

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads all fleet-related data.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' <remarks>
    ''' If there is no <see cref="IsFishing">fishing</see>, the fleet data will not be loaded.
    ''' This check is inherited from EwE5.
    ''' </remarks>
    ''' -------------------------------------------------------------------
    Private Function LoadEcopathFleetInfo() As Boolean

        Dim bSucces As Boolean = LoadEcopathFleets()
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
        Dim dtFleets As DataTable = Me.ReadTable("EcopathFleet")
        Dim iFleet As Integer = 1
        Dim bSucces As Boolean = True

        ecopathDS.NoGearData = Not IsFishing()
        ecopathDS.NumFleet = dtFleets.Rows.Count()

        If Not ecopathDS.RedimFleetVariables(True) Then Return False

        Try
            dtFleets.DefaultView.Sort = "Sequence ASC"
            For Each drow As DataRow In dtFleets.DefaultView.ToTable.Rows

                ecopathDS.FleetDBID(iFleet) = CInt(drow("FleetID"))
                ecopathDS.FleetName(iFleet) = CStr(drow("FleetName"))
                ecopathDS.CostPct(iFleet, eCostIndex.Fixed) = CSng(drow("FixedCost"))
                ecopathDS.CostPct(iFleet, eCostIndex.Sail) = CSng(drow("SailingCost"))
                ecopathDS.CostPct(iFleet, eCostIndex.CUPE) = CSng(drow("variableCost"))
                'ecopathDS.FleetColor(iFleet) = Integer.Parse(CStr(drow("PoolColor")), Globalization.NumberStyles.HexNumber)
                iFleet += 1

            Next

            dtFleets.Clear()

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EcopathFleet {1}", ex.Message, iFleet))
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function LoadEcopathCatch() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim dtCatch As DataTable = Me.ReadTable("EcopathCatch")
        Dim iFleet As Integer = 0
        Dim iGroup As Integer = 0
        Dim bSucces As Boolean = True

        Try

            For Each drow As DataRow In dtCatch.Rows

                iGroup = Array.IndexOf(ecopathDS.GroupDBID, CInt(drow("GroupID")))
                iFleet = Array.IndexOf(ecopathDS.FleetDBID, CInt(drow("FleetID")))

                If (iGroup >= 1 And iFleet >= 1) Then
                    ecopathDS.Landing(iFleet, iGroup) = CSng(drow("Landing"))
                    ecopathDS.Discard(iFleet, iGroup) = CSng(drow("discards"))
                    ecopathDS.Market(iFleet, iGroup) = CSng(drow("price"))
                    ecopathDS.PropDiscardMort(iFleet, iGroup) = CSng(Me.ReadSafe(drow, "DiscardMortality", 0.0!))
                Else
                    Me.LogMessage(String.Format("Error {0} occurred while appending loading catch for group {0}, fleet {1}", iGroup, iFleet))
                    bSucces = False
                End If

            Next

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading catch {1}, {2}", ex.Message, iGroup, iFleet))
            bSucces = False
        End Try

        dtCatch.Clear()

        Return bSucces

    End Function

    Private Function LoadEcopathDiscardFate() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim dt As DataTable = Me.ReadTable("EcopathDiscardFate")
        Dim iFleet As Integer = 0
        Dim iGroup As Integer = 0
        Dim bSucces As Boolean = True

        Try
            For Each drow As DataRow In dt.Rows

                iGroup = Array.IndexOf(ecopathDS.GroupDBID, CInt(drow("GroupID")))
                iFleet = Array.IndexOf(ecopathDS.FleetDBID, CInt(drow("FleetID")))

                If (iGroup > ecopathDS.NumLiving) Then
                    ecopathDS.DiscardFate(iFleet, iGroup - ecopathDS.NumLiving) = CSng(drow("DiscardFate"))
                End If

            Next
            dt.Clear()

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading DiscardFate {1}, {2}", ex.Message, iGroup, iFleet))
            bSucces = False
        End Try

        Return bSucces

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the pedigree level definitions.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadPedigreeLevels() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData

        ' Init data structure
        ecopathDS.NumPedigreeLevels = 0
        ecopathDS.RedimPedigree()
        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Load the pedigree level assignments.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' -----------------------------------------------------------------------
    Private Function LoadPedigreeAssignments() As Boolean
        Return True
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads Ecopath taxonomy information.
    ''' </summary>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Private Function LoadEcopathTaxon() As Boolean

        Dim taxonDS As cTaxonDataStructures = Me.m_core.m_TaxonData
        taxonDS.NumTaxon = 0
        taxonDS.RedimTaxon()
        Return True

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
        Dim dt As DataTable = Me.ReadTable("EcosimScenario")
        Dim iScenario As Integer = 1
        Dim bSucces As Boolean = True

        ecopathDS.NumEcosimScenarios = dt.Rows.Count
        ecopathDS.RedimEcosimScenarios()
        If (ecopathDS.NumEcosimScenarios = 0) Then Return bSucces

        Try
            For Each drow As DataRow In dt.Rows
                ecopathDS.EcosimScenarioDBID(iScenario) = CInt(drow("ScenarioID"))
                ecopathDS.EcosimScenarioName(iScenario) = CStr(drow("ScenarioName"))
                ecopathDS.EcosimScenarioDescription(iScenario) = CStr(drow("Description"))
                ecopathDS.EcosimScenarioAuthor(iScenario) = CStr(Me.ReadSafe(drow, "Author", ""))
                ecopathDS.EcosimScenarioContact(iScenario) = CStr(Me.ReadSafe(drow, "Contact", ""))
                ecopathDS.EcosimScenarioLastSaved(iScenario) = CDbl(Me.ReadSafe(drow, "LastSaved", 0))
                iScenario += 1
            Next
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading ecosim scenario definition {1}", ex.Message, iScenario))
            bSucces = False
        End Try

        dt.Clear()

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
        Dim dt As DataTable = Me.ReadTable("EcospaceScenario")
        Dim iScenario As Integer = 1
        Dim bSucces As Boolean = True

        ecopathDS.NumEcospaceScenarios = dt.Rows.Count
        ecopathDS.RedimEcospaceScenarios()
        If (ecopathDS.NumEcospaceScenarios = 0) Then Return bSucces

        Try
            For Each drow As DataRow In dt.Rows
                ecopathDS.EcospaceScenarioDBID(iScenario) = CInt(drow("ScenarioID"))
                ecopathDS.EcospaceScenarioName(iScenario) = CStr(drow("ScenarioName"))
                ecopathDS.EcospaceScenarioDescription(iScenario) = CStr(drow("Description"))
                ecopathDS.EcospaceScenarioAuthor(iScenario) = CStr(Me.ReadSafe(drow, "Author", ""))
                ecopathDS.EcospaceScenarioContact(iScenario) = CStr(Me.ReadSafe(drow, "Contact", ""))
                ecopathDS.EcospaceScenarioLastSaved(iScenario) = CDbl(Me.ReadSafe(drow, "LastSaved", 0))
                iScenario += 1
            Next
        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading ecospace scenario definition {1}", ex.Message, iScenario))
            bSucces = False
        End Try

        dt.Clear()
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

        ecopathDS.NumEcotracerScenarios = 0
        ecopathDS.RedimEcotracerScenarios()

        Return True
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Load all time series dataset definitions for Ecopath.
    ''' </summary>
    ''' <returns>True if successful.</returns>
    ''' <remarks>Yeah, this is odd; time series can only be used with Ecosim
    ''' but this logic just reads which time series will be available for Ecosim
    ''' later on; it is convenient to know which data sets are provided with
    ''' the model, just as it is convenient to know which scenarios are
    ''' before they are loaded ;)</remarks>
    ''' -------------------------------------------------------------------
    Private Function LoadTimeSeriesDatasets() As Boolean

        Dim tsDS As cTimeSeriesDataStructures = Me.m_core.m_TSData
        Dim dt As DataTable = Me.ReadTable("EcosimTimeSeriesDataset")
        Dim iDataset As Integer = 1
        Dim bSucces As Boolean = True

        tsDS.nDatasets = dt.Rows.Count

        tsDS.RedimTimeSeriesDatasets()

        Try
            For Each drow As DataRow In dt.Rows
                tsDS.iDatasetDBID(iDataset) = CInt(drow("DatasetID"))
                tsDS.strDatasetNames(iDataset) = CStr(drow("DatasetName"))
                tsDS.strDatasetDescription(iDataset) = CStr(Me.ReadSafe(drow, "Description", ""))
                tsDS.strDatasetAuthor(iDataset) = CStr(Me.ReadSafe(drow, "Author", ""))
                tsDS.strDatasetContact(iDataset) = CStr(Me.ReadSafe(drow, "Contact", ""))
                tsDS.nDatasetFirstYear(iDataset) = CInt(drow("FirstYear"))
                tsDS.nDatasetNumYears(iDataset) = CInt(drow("NumYears"))
                tsDS.nDatasetNumTimeSeries(iDataset) = 0 ' CInt(Me.GetValue(String.Format("SELECT COUNT(*) FROM EcosimTimeSeries WHERE (DatasetID={0})", CInt(drow("DatasetID")))))
                iDataset += 1
            Next
        Catch ex As Exception
            bSucces = False
        End Try

        dt.Clear()

        Return bSucces

    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Loads an ecosim scenario from the DB.
    ''' </summary>
    ''' <param name="iScenarioID">Database ID of the scenario to load.</param>
    ''' <returns>True if succesful.</returns>
    ''' -------------------------------------------------------------------
    Friend Function LoadEcosimScenario(ByVal iScenarioID As Integer) As Boolean _
            Implements IEcosimDatasource.LoadEcosimScenario

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim dt As DataTable = Me.ReadTable("EcosimScenario")
        Dim bSucces As Boolean = True

        bSucces = Me.LoadEcosimModel()

        ecosimDS.RedimVars()
        ecosimDS.SetDefaultParameters()
        mseDS.RedimVars()
        mseDS.setDefaultRegValues()

        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        For Each drow As DataRow In dt.DefaultView.ToTable.Rows

            Try
                ecosimDS.NumYears = CInt(drow("TotalTime"))
                ecosimDS.StepSize = CSng(drow("StepSize"))
                ecosimDS.EquilibriumStepSize = CSng(drow("EquilibriumStepSize"))
                ecosimDS.EquilScaleMax = CSng(drow("EquilScaleMax"))
                ecosimDS.SorWt = CSng(drow("sorwt"))
                ecosimDS.SystemRecovery = CSng(drow("SystemRecovery"))
                ecosimDS.Discount = CSng(drow("Discount"))

                'ecosimDS.NudgeStart = CSng(drow("NudgeStart"))
                'ecosimDS.NudgeEnd = CSng(drow("NudgeEnd"))
                'ecosimDS.NudgeFactor = CSng(drow("NudgeFactor"))
                'ecosimDS.DoInteg = CSng(drow("DoInteg"))
                'ecosimDS.chkNudge = CBool(drow("UseNudge"))

                'drow("NMed") = Me.FixValue(drow("NMed"))                        ' DISCONTINUED
                'drow("NMedPoints") = Me.FixValue(drow("NMedPoints"))            ' DISCONTINUED

                ecosimDS.NutBaseFreeProp = CSng(drow("NutBaseFreeProp"))
                ecosimDS.NutPBmax = CSng(drow("NutPBmax"))

                'ecosimDS.UseVarPQ = CBool(drow("UseVarPQ"))
                'VC090403: the var P/Q was being set to true by default, It shouldn't be, this should be done in interface only
                ecosimDS.UseVarPQ = False

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading Scenario {1}", ex.Message, iScenarioID))
                bSucces = False
            End Try
        Next
        dt.Clear()

        'jb added to redim time variables in ecosim data structures
        ecosimDS.RedimTime()

        Me.m_core.m_MSEData.redimTime()

        ' Set active scenario
        ecopathDS.ActiveEcosimScenario = Array.IndexOf(ecopathDS.EcosimScenarioDBID, iScenarioID)

        bSucces = bSucces And Me.LoadEcosimGroups(iScenarioID)
        bSucces = bSucces And Me.LoadEcosimFleets(iScenarioID)
        bSucces = bSucces And Me.LoadShapes()
        'bSucces = bSucces And Me.LoadEcosimMSE(iScenarioID)
        'bSucces = bSucces And Me.LoadAuxillaryData()

        Me.ClearChanged()

        Return bSucces

    End Function

    Private Function LoadEcosimModel() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim dt As DataTable = Me.ReadTable("EcosimModel")
        Dim bSuccess As Boolean = True

        For Each drow As DataRow In dt.Rows
            Try
                ecosimDS.ForcePoints = CInt(Me.ReadSafe(drow, "ForcePoints", cEcosimDatastructures.DEFAULT_N_FORCINGPOINTS))
            Catch ex As Exception
                bSuccess = False
            End Try
        Next

        ecosimDS.nGroups = ecopathDS.NumGroups

        dt.Clear()
        Return bSuccess

    End Function

    Private Function LoadEcosimGroups(ByVal iScenarioID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim dt As DataTable = Me.ReadTable("EcoSimScenarioGroup")
        Dim bSucces As Boolean = True
        Dim iEcopathGroupID As Integer = 0
        Dim iGroup As Integer = 0

        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        For Each drow As DataRow In dt.DefaultView.ToTable.Rows

            iEcopathGroupID = CInt(drow("EcopathGroupID"))
            iGroup = Array.IndexOf(ecopathDS.GroupDBID, iEcopathGroupID)

            Debug.Assert(iGroup > 0)

            Try

                ' Read fields
                ecosimDS.GroupDBID(iGroup) = CInt(drow("GroupID"))
                ecosimDS.PBmaxs(iGroup) = CSng(drow("pbmaxs"))
                ecosimDS.FtimeMax(iGroup) = CSng(drow("FtimeMax"))
                ecosimDS.FtimeAdjust(iGroup) = CSng(drow("FtimeAdjust"))
                ecosimDS.MoPred(iGroup) = CSng(drow("MoPred"))
                ecosimDS.FishRateMax(iGroup) = CSng(drow("FishRateMax"))
                ' ecosimDS.ShowGroup(i) = CBool(drow("Show"))

                ecosimDS.RiskTime(iGroup) = CSng(drow("RiskTime"))
                ecosimDS.QmQo(iGroup) = CSng(drow("QmQo"))
                ecosimDS.CmCo(iGroup) = CSng(drow("CmCo"))
                ecosimDS.SwitchPower(iGroup) = CSng(drow("SwitchPower"))
                ecosimDS.GroupFishRateNoDBID(iGroup) = CInt(drow("FishMortShapeID"))
                ecosimDS.SalOpt(iGroup) = CSng(Me.ReadSafe(drow, "SalOpt", 35.0!))
                ecosimDS.SdSalLeft(iGroup) = CSng(Me.ReadSafe(drow, "SdSalLeft", 1000.0!))
                ecosimDS.SdSalRight(iGroup) = CSng(Me.ReadSafe(drow, "SdSalRight", 1000.0!))
                ecosimDS.TempOpt(iGroup) = CSng(Me.ReadSafe(drow, "TempOpt", 10.0!))
                ecosimDS.TempLeft(iGroup) = CSng(Me.ReadSafe(drow, "TempLeft", 1000.0!))
                ecosimDS.TempRight(iGroup) = CSng(Me.ReadSafe(drow, "TempRight", 1000.0!))

                mseDS.Blim(iGroup) = CSng(Me.ReadSafe(drow, "Blim", mseDS.Blim(iGroup), cCore.NULL_VALUE))
                mseDS.Bbase(iGroup) = CSng(Me.ReadSafe(drow, "Bbase", mseDS.Bbase(iGroup), cCore.NULL_VALUE))
                mseDS.Fopt(iGroup) = CSng(Me.ReadSafe(drow, "Fopt", mseDS.Fopt(iGroup), cCore.NULL_VALUE))
                mseDS.FixedEscapement(iGroup) = CSng(Me.ReadSafe(drow, "FixedEscapement", 0.0!, cCore.NULL_VALUE))
                mseDS.FixedF(iGroup) = CSng(Me.ReadSafe(drow, "FixedF", 0.0!, cCore.NULL_VALUE))

                mseDS.CVbiomEst(iGroup) = CSng(Me.ReadSafe(drow, "BiomassCV", mseDS.CVbiomEst(iGroup), cCore.NULL_VALUE))
                mseDS.BioRiskValue(iGroup, 0) = CSng(Me.ReadSafe(drow, "LowerRisk", mseDS.BioRiskValue(iGroup, 0), cCore.NULL_VALUE))
                mseDS.BioRiskValue(iGroup, 1) = CSng(Me.ReadSafe(drow, "UpperRisk", mseDS.BioRiskValue(iGroup, 1), cCore.NULL_VALUE))

                mseDS.DefaultBioBounds(iGroup)
                mseDS.BioBounds(iGroup).Lower = CSng(Me.ReadSafe(drow, "BiomassRefLower", mseDS.BioBounds(iGroup).Lower, cCore.NULL_VALUE))
                mseDS.BioBounds(iGroup).Upper = CSng(Me.ReadSafe(drow, "BiomassRefUpper", mseDS.BioBounds(iGroup).Upper, cCore.NULL_VALUE))

                mseDS.DefaultCatchBoundsGroup(iGroup)
                mseDS.CatchGroupBounds(iGroup).Lower = CSng(Me.ReadSafe(drow, "CatchRefLower", mseDS.CatchGroupBounds(iGroup).Lower, cCore.NULL_VALUE))
                mseDS.CatchGroupBounds(iGroup).Upper = CSng(Me.ReadSafe(drow, "CatchRefUpper", mseDS.CatchGroupBounds(iGroup).Upper, cCore.NULL_VALUE))

                mseDS.RstockRatio(iGroup) = CSng(Me.ReadSafe(drow, "RStockRatio", mseDS.RstockRatio(iGroup), cCore.NULL_VALUE))
                mseDS.RHalfB0Ratio(iGroup) = CSng(Me.ReadSafe(drow, "RHalfB0Ratio", mseDS.RHalfB0Ratio(iGroup), cCore.NULL_VALUE))
                mseDS.cvRec(iGroup) = CSng(Me.ReadSafe(drow, "RecruitmentCV", mseDS.cvRec(iGroup), cCore.NULL_VALUE))

                ' Me.LoadFishMortShape(CInt(drow("FishMortShapeID")), iGroup)

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading EcoSim group info for group {1}", ex.Message, iGroup))
                bSucces = False
            End Try
        Next
        dt.Clear()

        bSucces = bSucces And Me.LoadEcosimGroupYear(iScenarioID)
        Return bSucces

    End Function

    Private Function LoadEcosimGroupYear(ByVal iScenarioID As Integer) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim dt As DataTable = Me.ReadTable("EcosimScenarioGroupYear")
        Dim iGroupID As Integer = -1
        Dim iGroup As Integer = -1
        Dim iYear As Integer = -1
        Dim bSucces As Boolean = True

        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        For Each drow As DataRow In dt.DefaultView.ToTable.Rows
            Try
                iGroupID = CInt(drow("GroupID"))
                iGroup = Array.IndexOf(ecosimDS.GroupDBID, iGroupID)
                iYear = CInt(drow("TimeYear"))
                If (iGroup > 0) And (iGroup <= ecosimDS.nGroups) And _
                   (iYear > 0) And (iYear <= mseDS.nYears) Then
                    mseDS.CVBiomT(iGroup, iYear) = CSng(drow("CVBiom"))
                End If
            Catch ex As Exception
                bSucces = False
            End Try
        Next
        dt.Clear()

        Return bSucces

    End Function

    Private Function LoadEcosimFleets(ByVal iScenarioID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim dt As DataTable = Me.ReadTable("EcoSimScenarioFleet")
        Dim dtFishMort As DataTable = Me.ReadTable("EcosimShapeFishRate")
        Dim iFleet As Integer = 0
        Dim iFleetID As Integer = -1
        Dim iShapeID As Integer = -1
        Dim bSucces As Boolean = True
        Dim asDummy(ecosimDS.NTimes) As Single

        Dim dtNewFleetShapes As New Dictionary(Of Integer, Integer)

        For iPt As Integer = 0 To ecosimDS.NTimes : asDummy(iPt) = 1.0 : Next

        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        dt.DefaultView.Sort = "EcopathFleetID ASC"

        For Each drow As DataRow In dt.DefaultView.ToTable.Rows

            iFleetID = CInt(drow("EcopathFleetID"))
            iFleet = Array.IndexOf(ecopathDS.FleetDBID, iFleetID)
            Debug.Assert(iFleet > 0)

            iShapeID = CInt(Me.ReadSafe(drow, "FishRateShapeID", -1))
            Debug.Assert(iShapeID > 0, "No effort shape defined for fleet " & iFleet)

            If iShapeID > -1 Then
                ' JS 10Aug07: Don't fail in case FishRateShape is missing. Only those present are loaded, only those loaded are saved.
                '             Since these shapes do not need to be present we can be somewhat forgiving in this particular case.
                If Not LoadFishingRateShape(dtFishMort, iShapeID, iFleet) Then
                    Me.LogMessage(String.Format("Warning: Fishing rate shape {0} is referenced but not present in database for EcoSim fleet {1} (ID {2})", iShapeID, iFleet, iFleetID))
                End If
            End If

            Try
                ecosimDS.FleetDBID(iFleet) = CInt(drow("FleetID"))
                ecosimDS.Epower(iFleet) = CSng(Me.ReadSafe(drow, "Epower", 3))
                ecosimDS.PcapBase(iFleet) = CSng(Me.ReadSafe(drow, "PCapBase", 0.5))
                ecosimDS.CapDepreciate(iFleet) = CSng(Me.ReadSafe(drow, "CapDepreciate", 0.06))
                ecosimDS.CapBaseGrowth(iFleet) = CSng(Me.ReadSafe(drow, "CapBaseGrowth", 0.2))
                ecosimDS.EffortConversionFactor(iFleet) = CSng(Me.ReadSafe(drow, "EffortConversionFactor", 1.0!))

                mseDS.MaxEffort(iFleet) = CSng(Me.ReadSafe(drow, "MaxEffort", cCore.NULL_VALUE))
                mseDS.QuotaType(iFleet) = DirectCast(CInt(Me.ReadSafe(drow, "QuotaType", 0)), eQuotaTypes)
                mseDS.CVFest(iFleet) = CSng(Me.ReadSafe(drow, "CV", mseDS.CVFest(iFleet)))
                mseDS.Qgrow(iFleet) = CSng(Me.ReadSafe(drow, "QIncrease", mseDS.Qgrow(iFleet)))

                mseDS.DefaultCatchBoundsFleet(iFleet)
                mseDS.CatchFleetBounds(iFleet).Lower = CSng(Me.ReadSafe(drow, "CatchRefLower", mseDS.CatchFleetBounds(iFleet).Lower))
                mseDS.CatchFleetBounds(iFleet).Upper = CSng(Me.ReadSafe(drow, "CatchRefUpper", mseDS.CatchFleetBounds(iFleet).Upper))
                mseDS.EffortFleetBounds(iFleet).Lower = CSng(Me.ReadSafe(drow, "EffortRefLower", mseDS.EffortFleetBounds(iFleet).Lower))
                mseDS.EffortFleetBounds(iFleet).Upper = CSng(Me.ReadSafe(drow, "EffortRefUpper", mseDS.EffortFleetBounds(iFleet).Upper))
                'mseDS.MSYEvaluateFleet(iFleet) = (CInt(Me.ReadSafe(drow, "MSYEvaluateFleet", True)) = 1)

            Catch ex As Exception
                bSucces = False
            End Try

        Next
        dt.Clear()

        bSucces = bSucces And Me.LoadEcosimFleetYear(iScenarioID)
        bSucces = bSucces And Me.LoadEcosimQuota(iScenarioID)

        Return bSucces

    End Function

    Private Function LoadEcosimFleetYear(ByVal iScenarioID As Integer) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim dt As DataTable = Me.ReadTable("EcoSimScenarioFleetYear")
        Dim iFleetID As Integer = -1
        Dim iFleet As Integer = -1
        Dim iYear As Integer = -1
        Dim bSucces As Boolean = True

        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        For Each drow As DataRow In dt.DefaultView.ToTable.Rows
            Try
                iFleetID = CInt(drow("FleetID"))
                iFleet = Array.IndexOf(ecosimDS.FleetDBID, iFleetID)
                iYear = CInt(drow("TimeYear"))
                If (iFleet > 0) And (iFleet <= ecosimDS.nGear) And _
                   (iYear > 0) And (iYear <= mseDS.nYears) Then
                    mseDS.CVFT(iFleet, iYear) = CSng(drow("CV"))
                End If

            Catch ex As Exception
                bSucces = False
            End Try
        Next
        dt.Clear()

        Return bSucces

    End Function

    Private Function LoadEcosimQuota(ByVal iScenarioID As Integer) As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim mseDS As cMSEDataStructures = Me.m_core.m_MSEData
        Dim dt As DataTable = Me.ReadTable("EcoSimScenarioQuota")
        Dim iFleetID As Integer = -1
        Dim iFleet As Integer = -1
        Dim iGroupID As Integer = -1
        Dim iGroup As Integer = -1
        Dim bSucces As Boolean = True

        For Each drow As DataRow In dt.Rows

            Try
                iFleetID = CInt(drow("FleetID"))
                iFleet = Array.IndexOf(ecopathDS.FleetDBID, iFleetID)

                iGroupID = CInt(drow("EcosimGroupID"))
                iGroup = Array.IndexOf(ecosimDS.GroupDBID, iGroupID)

                If (iFleet > 0) And (iGroup > 0) Then
                    mseDS.Quotashare(iFleet, iGroup) = CSng(Me.ReadSafe(drow, "QuotaShare", mseDS.Quotashare(iFleet, iGroup)))
                    mseDS.Fweight(iFleet, iGroup) = CSng(Me.ReadSafe(drow, "FWeight", 1.0))
                End If

            Catch ex As Exception
                bSucces = False
            End Try
        Next
        dt.Clear()
        Return bSucces

    End Function


    Private Function LoadShapes() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim PredPreyMedDS As cMediationDataStructures = Me.m_core.m_EcoSimData.BioMedData
        Dim LandingsMedDS As cMediationDataStructures = Me.m_core.m_EcoSimData.PriceMedData
        Dim CapEnvResMedDS As cMediationDataStructures = Me.m_core.m_EcoSimData.CapEnvResData
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim dt As DataTable = Me.ReadTable("EcosimShape")
        Dim iShapeID As Integer = 0
        Dim shapeDataType As eDataTypes = eDataTypes.NotSet
        Dim iForcingShape As Integer = 0
        Dim iPredPreyMediationShape As Integer = 0
        Dim iLandingsMediationShape As Integer = 0
        Dim iCapEnvResMediationShape As Integer = 0
        Dim iFishingMortShape As Integer = 0
        Dim iFishRateShape As Integer = 0
        Dim bSucces As Boolean = True

        ecosimDS.ForcingShapes = 0
        PredPreyMedDS.MediationShapes = 0
        LandingsMedDS.MediationShapes = 0
        CapEnvResMedDS.MediationShapes = 0

        For Each drow As DataRow In dt.Rows
            Select Case DirectCast(CInt(drow("ShapeType")), eDataTypes)
                Case eDataTypes.EggProd, eDataTypes.Forcing : ecosimDS.ForcingShapes += 1
                Case eDataTypes.Mediation : PredPreyMedDS.MediationShapes += 1
                Case eDataTypes.PriceMediation : LandingsMedDS.MediationShapes += 1
                Case eDataTypes.CapacityMediation : CapEnvResMedDS.MediationShapes += 1
            End Select
        Next

        ecosimDS.DimForcingShapes()
        ecosimDS.InitForcingShapes()
        PredPreyMedDS.ReDimMediation(ecosimDS.nGroups, ecosimDS.nGear)
        LandingsMedDS.ReDimMediation(ecosimDS.nGroups, ecosimDS.nGear)
        CapEnvResMedDS.ReDimMediation(ecosimDS.nGroups, ecosimDS.nGear)

        Dim dtEgg As DataTable = Me.ReadTable("EcosimShapeEggProd")
        Dim dtTime As DataTable = Me.ReadTable("EcosimShapeTime")
        Dim dtMed As DataTable = Me.ReadTable("EcosimShapeMediation")
        For Each drow As DataRow In dt.Rows

            Try

                iShapeID = CInt(drow("ShapeID"))
                shapeDataType = DirectCast(drow("ShapeType"), eDataTypes)

                Select Case shapeDataType

                    Case eDataTypes.EggProd
                        iForcingShape += 1
                        bSucces = bSucces And Me.LoadEggShape(dtEgg, iShapeID, iForcingShape, CInt(drow("IsSeasonal")) <> 0)

                    Case eDataTypes.Forcing
                        iForcingShape += 1
                        bSucces = bSucces And Me.LoadTimeShape(dtTime, iShapeID, iForcingShape, CInt(drow("IsSeasonal")) <> 0)

                    Case eDataTypes.Mediation
                        iPredPreyMediationShape += 1
                        bSucces = bSucces And Me.LoadMediationShape(dtMed, iShapeID, iPredPreyMediationShape, PredPreyMedDS)

                    Case eDataTypes.PriceMediation
                        iLandingsMediationShape += 1
                        bSucces = bSucces And Me.LoadMediationShape(dtMed, iShapeID, iLandingsMediationShape, LandingsMedDS)

                    Case eDataTypes.CapacityMediation
                        iCapEnvResMediationShape += 1
                        bSucces = bSucces And Me.LoadMediationShape(dtMed, iShapeID, iCapEnvResMediationShape, CapEnvResMedDS)

                    Case eDataTypes.FishingEffort
                        'iFishRateShape += 1
                        'bSucces = bSucces And Me.LoadFishingRateShape(iShapeID, iFishRateShape)

                    Case eDataTypes.FishMort
                        'iFishingMortShape += 1
                        'bSucces = bSucces And Me.LoadFishMortShape(iShapeID, iFishingMortShape)

                    Case Else
                        Debug.Assert(False, String.Format("Cannot load invalid shapetype {0} for shape ID {1}", shapeDataType, iShapeID))

                End Select

            Catch ex As Exception
                bSucces = False
            End Try
        Next

        dt = Me.ReadTable("EcosimScenario")
        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)

        Try
            Dim drow As DataRow = dt.DefaultView.ToTable.Rows(0)
            ' Read and assign scenario forcing shape number(s)
            iForcingShape = CInt(Me.ReadSafe(drow, "NutForcingShapeID", 0))
            ecosimDS.NutForceNumber = Math.Max(0, Array.IndexOf(ecosimDS.ForcingDBIDs, iForcingShape))
            iForcingShape = CInt(Me.ReadSafe(drow, "SalinityForcingShapeID", 0))
            ecosimDS.SalinityForceNo = Math.Max(0, Array.IndexOf(ecosimDS.ForcingDBIDs, iForcingShape))
            iForcingShape = CInt(Me.ReadSafe(drow, "TemperatureForcingShapeID", 0))
            ecosimDS.TemperatureForceNo = Math.Max(0, Array.IndexOf(ecosimDS.ForcingDBIDs, iForcingShape))
        Catch ex As Exception
            bSucces = False
        End Try
        dt.Clear()

        bSucces = bSucces And Me.LoadEcosimVulnerabilities()
        bSucces = bSucces And Me.LoadPredPreyInteractions()
        bSucces = bSucces And Me.LoadLandingInteractions()
        bSucces = bSucces And Me.LoadMediationWeights()
        bSucces = bSucces And Me.LoadStanzaShapeAssignments()

        Return bSucces

    End Function

#Region " Shape load helpers "

    Private Function LoadEggShape(ByVal dt As DataTable, _
                                  ByVal iShapeID As Integer, _
                                  ByVal iForcingShape As Integer, _
            Optional ByVal bIsSeasonal As Boolean = False) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim shapeParms As New cEcosimDatastructures.ShapeParameters()
        Dim drow As DataRow = Nothing
        Dim astrZScale() As String
        Dim bSucces As Boolean = True

        'readerShape = Me.GetReader(String.Format("SELECT * FROM EcosimShapeEggProd WHERE (ShapeID={0})", iShapeID))
        dt.DefaultView.RowFilter = CStr("ShapeID=" & iShapeID)
        Try
            drow = dt.DefaultView.ToTable.Rows(0)
            shapeParms.YZero = CSng(drow("Yzero"))
            shapeParms.YBase = CSng(drow("Ybase"))
            shapeParms.YEnd = CSng(drow("Yend"))
            shapeParms.Steep = CSng(drow("Steep"))
            ' sp.ZScale = CInt(readerShape("ZScale"))
            shapeParms.ShapeFunctionType = CType(drow("FunctionType"), eShapeFunctionType)

            ' Read z-scale
            astrZScale = Me.SplitNumberString(CStr(drow("Zscale")))
            For ipt As Integer = 1 To Math.Min(ecosimDS.ForcePoints, astrZScale.Length)
                ecosimDS.zscale(ipt, iForcingShape) = cStringUtils.ConvertToSingle(astrZScale(ipt - 1), 0)
            Next ipt
            For ipt As Integer = Math.Min(ecosimDS.ForcePoints, astrZScale.Length) + 1 To ecosimDS.ForcePoints
                ecosimDS.zscale(ipt, iForcingShape) = 1.0
            Next

            ecosimDS.ForcingShapeParams(iForcingShape) = shapeParms
            ecosimDS.ForcingDBIDs(iForcingShape) = iShapeID
            ecosimDS.ForcingTitles(iForcingShape) = CStr(drow("Title"))
            ecosimDS.ForcingShapeType(iForcingShape) = eDataTypes.EggProd
            ecosimDS.isSeasonal(iForcingShape) = bIsSeasonal

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading EggShape {1}", ex.Message, iShapeID))
            bSucces = False
        End Try
        dt.DefaultView.RowFilter = ""

        Return bSucces

    End Function

    Private Function LoadTimeShape(ByVal dtTime As DataTable, _
                                   ByVal iShapeID As Integer, _
                                   ByVal iForcingShape As Integer, _
                                   Optional ByVal bIsSeasonal As Boolean = False) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim shapeParms As New cEcosimDatastructures.ShapeParameters()
        Dim drow As DataRow = Nothing
        Dim astrZScale() As String
        Dim bSucces As Boolean = True

        dtTime.DefaultView.RowFilter = CStr("ShapeID=" & iShapeID)
        drow = dtTime.DefaultView.ToTable.Rows(0)
        Try
            ' Read shape parameters
            shapeParms.YZero = CSng(drow("Yzero"))
            shapeParms.YBase = CSng(drow("Ybase"))
            shapeParms.YEnd = CSng(drow("Yend"))
            shapeParms.Steep = CSng(drow("Steep"))
            shapeParms.ShapeFunctionType = CType(drow("FunctionType"), eShapeFunctionType)

            ' Read z-scale
            Dim sLast As Single = 1.0!
            astrZScale = Me.SplitNumberString(CStr(drow("Zscale")))
            For ipt As Integer = 1 To Math.Min(ecosimDS.ForcePoints, astrZScale.Length)
                sLast = cStringUtils.ConvertToSingle(astrZScale(ipt - 1), 0)
                ecosimDS.zscale(ipt, iForcingShape) = sLast
            Next ipt
            For ipt As Integer = Math.Min(ecosimDS.ForcePoints, astrZScale.Length) + 1 To ecosimDS.ForcePoints
                ecosimDS.zscale(ipt, iForcingShape) = sLast
            Next

            ecosimDS.ForcingShapeParams(iForcingShape) = shapeParms
            ecosimDS.ForcingDBIDs(iForcingShape) = iShapeID
            ecosimDS.ForcingTitles(iForcingShape) = CStr(drow("Title"))
            ecosimDS.ForcingShapeType(iForcingShape) = eDataTypes.Forcing
            ecosimDS.ForcingApplicationType(iForcingShape) = DirectCast(CInt(Me.ReadSafe(drow, "ApplicationType", eForcingApplicationTypes.NotSet)), eForcingApplicationTypes)
            ecosimDS.isSeasonal(iForcingShape) = bIsSeasonal

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading TimeShape {1}", ex.Message, iShapeID))
            bSucces = False
        End Try
        dtTime.DefaultView.RowFilter = ""

        Return bSucces

    End Function

    Private Function LoadMediationShape(ByVal dtMed As DataTable, _
                                        ByVal iShapeID As Integer, _
                                        ByVal iMediationShape As Integer, _
                                        ByVal medData As cMediationDataStructures) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim shapeParms As New cEcosimDatastructures.ShapeParameters()
        Dim astrZScale() As String
        Dim bSucces As Boolean = True

        'readerShape = Me.GetReader(String.Format("SELECT * FROM EcosimShapeMediation WHERE (ShapeID={0})", iShapeID))
        dtMed.DefaultView.RowFilter = CStr("ShapeID=" & iShapeID)

        Try
            Dim drow As DataRow = dtMed.DefaultView.ToTable.Rows(0)

            ' Init shapeParms
            shapeParms.YZero = CSng(drow("Yzero"))
            shapeParms.YBase = CSng(drow("Ybase"))
            shapeParms.YEnd = CSng(drow("Yend"))
            shapeParms.Steep = CSng(drow("Steep"))
            ' shapeParms.ZScale = CInt(readerShape("ZScale"))
            shapeParms.ShapeFunctionType = CType(drow("FunctionType"), eShapeFunctionType)

            ' Read z-scale
            astrZScale = Me.SplitNumberString(CStr(drow("Zscale")))
            ' Write points
            For ipt As Integer = 1 To Math.Min(medData.NMedPoints, astrZScale.Length)
                medData.Medpoints(ipt, iMediationShape) = cStringUtils.ConvertToSingle(astrZScale(ipt - 1), 0)
            Next ipt
            For ipt As Integer = Math.Min(medData.NMedPoints, astrZScale.Length) + 1 To medData.NMedPoints
                medData.Medpoints(ipt, iMediationShape) = 1.0
            Next

            medData.MediationShapeParams(iMediationShape) = shapeParms
            medData.MediationDBIDs(iMediationShape) = iShapeID
            medData.MediationTitles(iMediationShape) = CStr(drow("Title"))
            medData.IMedBase(iMediationShape) = CInt(Me.ReadSafe(drow, "IMedBase", 1200 / 3))
            medData.XAxisMin(iMediationShape) = CSng(Me.ReadSafe(drow, "XAxisMin", 0))
            medData.XAxisMax(iMediationShape) = CSng(Me.ReadSafe(drow, "XAxisMax", 1))

        Catch ex As Exception
            Me.LogMessage(String.Format("Error {0} occurred while reading MediationShape {1}", ex.Message, iShapeID))
            bSucces = False
        End Try

        Return bSucces

    End Function

    Private Function LoadEcosimVulnerabilities() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim dt As DataTable = Me.ReadTable("EcosimScenarioForcingMatrix")
        Dim iPredator As Integer = 0
        Dim iPrey As Integer = 0
        Dim bSucces As Boolean = True

        For iPredator = 1 To Me.m_core.nGroups
            For iPrey = 1 To Me.m_core.nGroups
                ecosimDS.VulMult(iPrey, iPredator) = 2.0!
            Next iPrey
        Next iPredator

        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        For Each drow As DataRow In dt.DefaultView.ToTable.Rows

            Try
                ' Find iPredator
                iPredator = Array.IndexOf(ecosimDS.GroupDBID, CInt(drow("PredID")))
                ' Find iPrey
                iPrey = Array.IndexOf(ecosimDS.GroupDBID, CInt(drow("PreyID")))

                If (iPredator > -1 And iPrey > -1) Then
                    ecosimDS.VulMult(iPrey, iPredator) = CSng(drow("vulnerability"))
                End If

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading ForcingMatrix", ex.Message))
                bSucces = False
            End Try
        Next
        dt.Clear()

        Return bSucces

    End Function

    Private Function LoadPredPreyInteractions() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim dt As DataTable = Me.ReadTable("EcosimScenarioPredPreyShape")
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim iPredator As Integer = 0
        Dim iPrey As Integer = 0
        Dim iShapeID As Integer = 0
        Dim iShape As Integer = 0
        Dim bSucces As Boolean = True
        Dim iFNo(ecosimDS.nGroups, ecosimDS.nGroups) As Integer

        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        For Each drow As DataRow In dt.DefaultView.ToTable.Rows

            Try

                ' Find iPredator
                iPredator = Array.IndexOf(ecosimDS.GroupDBID, CInt(drow("PredID")))
                ' Find iPrey
                iPrey = Array.IndexOf(ecosimDS.GroupDBID, CInt(drow("PreyID")))
                ' Next shape
                iFNo(iPrey, iPredator) += 1
                ' Protect from data overflow
                If (iFNo(iPrey, iPredator) <= cMediationDataStructures.MAXFUNCTIONS) Then
                    ' Resolve shape ID
                    iShapeID = CInt(drow("ShapeID"))
                    ' Determine shape type
                    iShape = Array.IndexOf(ecosimDS.BioMedData.MediationDBIDs, iShapeID)
                    ' Is a mediation shape?
                    If iShape <> -1 Then
                        ' #Yes: flag as mediation shape
                        ecosimDS.BioMedData.IsMedFunction(iPrey, iPredator, iFNo(iPrey, iPredator)) = True
                    Else
                        ' #No: flag as other shape
                        ecosimDS.BioMedData.IsMedFunction(iPrey, iPredator, iFNo(iPrey, iPredator)) = False
                        ' Obtain forcing index
                        iShape = Array.IndexOf(ecosimDS.ForcingDBIDs, iShapeID)
                    End If

                    If iShape <> -1 Then
                        ' Update sim fields
                        ecosimDS.BioMedData.FunctionNumber(iPrey, iPredator, iFNo(iPrey, iPredator)) = iShape
                        Dim iFT As Integer = CInt(drow("FunctionType"))
                        ' Fixes #980: eForcingFunctionApplication types ProductionRate and SearchRate are now synonymous.
                        '             ProdRate = 6 is discontinued. If a 6 occurs and pred=prey (which indicates PP) a default of 1 is substituted.
                        If (iFT = 6) And (iPredator = iPrey) Then iFT = eForcingFunctionApplication.ProductionRate
                        ecosimDS.BioMedData.FunctionType(iPrey, iPredator, iFNo(iPrey, iPredator)) = iFT
                    Else
                        Me.LogMessage(String.Format("Shape {0} cannot be used for pred/prey interactions; assignment discarded", iShapeID))
                    End If
                End If

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading PredPreyInteraction", ex.Message))
                bSucces = False
            End Try
        Next
        dt.Clear()

        Return bSucces

    End Function

    Private Function LoadLandingInteractions() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim dt As DataTable = Me.ReadTable("EcosimScenarioPredPreyShape")
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim iFleet As Integer = 0
        Dim iGroup As Integer = 0
        Dim iShapeID As Integer = 0
        Dim iShape As Integer = 0
        Dim bSucces As Boolean = True
        Dim iFNo(ecosimDS.nGroups, ecosimDS.nGear) As Integer

        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        For Each drow As DataRow In dt.DefaultView.ToTable.Rows

            Try

                ' Find iFleet
                iFleet = Array.IndexOf(ecosimDS.FleetDBID, CInt(drow("FleetID")))
                ' Find iGroup
                iGroup = Array.IndexOf(ecosimDS.GroupDBID, CInt(drow("GroupID")))
                ' Next shape
                iFNo(iGroup, iFleet) += 1
                ' Resolve shape ID
                iShapeID = CInt(drow("ShapeID"))
                ' Resolve iShape
                iShape = Array.IndexOf(ecosimDS.PriceMedData.MediationDBIDs, iShapeID)

                If iShape > -1 Then
                    ecosimDS.PriceMedData.PriceMedFuncNum(iGroup, iFleet, iFNo(iGroup, iFleet)) = iShape
                Else
                    Me.LogMessage(String.Format("Shape {0} cannot be used for landings interactions; assignment discarded", iShapeID))
                End If

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading Landing interaction", ex.Message))
                bSucces = False
            End Try
        Next
        dt.Clear()

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
        Dim medData As cMediationDataStructures = Nothing
        Dim dt As DataTable = Nothing
        Dim iScenarioID As Integer = ecopathDS.EcosimScenarioDBID(ecopathDS.ActiveEcosimScenario)
        Dim iGroup As Integer = 0
        Dim iFleet As Integer = 0
        Dim iShape As Integer = 0
        Dim bSucces As Boolean = True

        ' === Pred/prey mediations ===
        medData = ecosimDS.BioMedData
        dt = Me.ReadTable("EcosimScenarioShapeMedWeightsGroup")
        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        For Each drow As DataRow In dt.DefaultView.ToTable.Rows
            Try
                iShape = Array.IndexOf(medData.MediationDBIDs, drow("ShapeID"))
                iGroup = Array.IndexOf(ecosimDS.GroupDBID, drow("GroupID"))
                If (iGroup <> -1 And iShape <> -1) Then
                    medData.MedWeights(iGroup, iShape) = CSng(drow("MedWeights"))
                End If
            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading group MediationWeights", ex.Message))
                bSucces = False
            End Try
        Next
        dt.Clear()
        dt = Nothing

        dt = Me.ReadTable("EcosimScenarioShapeMedWeightsFleet")
        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        For Each drow As DataRow In dt.DefaultView.ToTable.Rows
            Try
                iShape = Array.IndexOf(medData.MediationDBIDs, drow("ShapeID"))
                ' Unfortunate legacy: fleet refers to Ecopath fleet, not Ecosim as it should have
                iFleet = Array.IndexOf(ecopathDS.FleetDBID, drow("FleetID"))
                If (iFleet <> -1 And iShape <> -1) Then
                    medData.MedWeights(iFleet + ecosimDS.nGroups, iShape) = CSng(drow("MedWeights"))
                End If
            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading fleet MediationWeights", ex.Message))
                bSucces = False
            End Try
        Next
        dt.Clear()
        dt = Nothing

        ' === Landings mediations === 
        medData = ecosimDS.PriceMedData
        dt = Me.ReadTable("EcosimScenarioshapeMedWeightsLandings")
        dt.DefaultView.RowFilter = CStr("ScenarioID=" & iScenarioID)
        For Each drow As DataRow In dt.DefaultView.ToTable.Rows
            Try
                iShape = Array.IndexOf(medData.MediationDBIDs, drow("ShapeID"))
                iGroup = Array.IndexOf(ecosimDS.GroupDBID, drow("GroupID"))
                iFleet = Array.IndexOf(ecosimDS.FleetDBID, drow("FleetID"))
                If (iGroup > 0 And iShape > 0) Then
                    iFleet = Math.Max(0, iFleet)
                    medData.MedPriceWeights(iGroup, iFleet, iShape) = CSng(drow("MedWeights"))
                End If
            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading group MediationWeights", ex.Message))
                bSucces = False
            End Try
        Next
        dt.Clear()
        dt = Nothing

        Return True

    End Function

    Private Function LoadStanzaShapeAssignments() As Boolean

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim dt As DataTable = Me.ReadTable("EcosimScenarioPredPreyShape")
        Dim iStanza As Integer = 0
        Dim iShape As Integer = 0
        Dim bSucces As Boolean = True

        For Each drow As DataRow In dt.Rows
            Try
                ' Get iStanza 
                iStanza = Array.IndexOf(stanzaDS.StanzaDBID, CInt(drow("StanzaID")))
                ' Is valid stanza?
                If (iStanza > 0) Then
                    ' #Yes: has egg production shape?
                    If Not Convert.IsDBNull(drow("EggprodShapeID")) Then
                        ' #Yes: resolve shape index iShape
                        iShape = Array.IndexOf(ecosimDS.ForcingDBIDs, CInt(drow("EggprodShapeID")))
                        ' Is a valid shape index?
                        If (iShape > 0) Then
                            ' #Yes: assign
                            stanzaDS.EggProdShapeSplit(iStanza) = iShape
                        End If
                    End If
                    ' #Yes: has hatch code forcing shape?
                    If Not Convert.IsDBNull(drow("HatchCodeShapeID")) Then
                        ' #Yes: resolve shape index iShape
                        iShape = Array.IndexOf(ecosimDS.ForcingDBIDs, CInt(drow("HatchCodeShapeID")))
                        ' Is a valid shape index?
                        If (iShape > 0) Then
                            ' #Yes: assign
                            stanzaDS.HatchCode(iStanza) = iShape
                        End If
                    End If
                End If ' Is valid stanza

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading stanza shape assignments", ex.Message))
                bSucces = False
            End Try
        Next
        dt.Clear()

        Return bSucces
    End Function

    Private Function LoadFishingRateShape(ByVal dtFishRate As DataTable, _
                                          ByVal iShapeID As Integer, _
                                          ByVal iFishingRateShape As Integer) As Boolean

        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim readerShape As IDataReader = Nothing
        Dim strMemo As String = ""
        Dim astrMemoBits() As String
        Dim bSucces As Boolean = True

        If iShapeID = 0 Then Return bSucces

        dtFishRate.DefaultView.RowFilter = CStr("ShapeID=" & iShapeID)
        For Each drow As DataRow In dtFishRate.DefaultView.ToTable.Rows
            Try
                ecosimDS.FishRateGearTitle(iFishingRateShape) = CStr(readerShape("Title"))
                strMemo = CStr(readerShape("zScale"))
                astrMemoBits = strMemo.Trim.Split(CChar(" "))
                For j As Integer = 1 To Math.Min(ecosimDS.NTimes, astrMemoBits.Length)
                    ecosimDS.FishRateGear(iFishingRateShape, j) = cStringUtils.ConvertToSingle(astrMemoBits(j - 1), 1)
                Next
                ecosimDS.FishRateGearDBID(iFishingRateShape) = iShapeID

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading FishingRate {1}", ex.Message, iShapeID))
                bSucces = False
            End Try
        Next

        Return bSucces

    End Function

#End Region ' Shape load helpers

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

#Region " Save from database "

    ''' <summary>
    ''' Save an Ecopath database to a file
    ''' </summary>
    ''' <param name="db"></param>
    ''' <param name="strFile"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SaveFromDB(db As cEwEDatabase, strFile As String) As Boolean

        Dim conn As OleDbConnection = DirectCast(db.GetConnection(), OleDbConnection)
        Dim dtTables As DataTable = Nothing
        Dim drow As DataRow = Nothing
        Dim doc As New XmlDocument()

        dtTables = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, New String() {Nothing, Nothing, Nothing, Nothing})
        If (dtTables Is Nothing) Then Return False

        Dim decl As XmlDeclaration = doc.CreateXmlDeclaration("1.0", "", "")
        doc.AppendChild(decl)

        Dim elm As XmlElement = doc.CreateElement("EwEModel")
        Dim xa As XmlAttribute = doc.CreateAttribute("Name")
        xa.InnerText = db.FileName
        elm.Attributes.Append(xa)

        xa = doc.CreateAttribute("DBVersion")
        xa.InnerText = db.GetVersion().ToString
        elm.Attributes.Append(xa)

        xa = doc.CreateAttribute("EwEVersion")
        xa.InnerText = cAssemblyUtils.GetVersion().ToString
        elm.Attributes.Append(xa)
        doc.AppendChild(elm)

        For Each drow In dtTables.Rows
            Try
                Me.SaveTable(db, CStr(drow(2)), doc)
            Catch ex As Exception

            End Try
        Next

        doc.Save(strFile)

        Return True

    End Function

    Private Function Columns(ByVal db As cEwEDatabase, strTable As String) As String()

        Dim conn As OleDbConnection = DirectCast(db.GetConnection(), OleDbConnection)
        Dim dtTables As DataTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, New String() {Nothing, Nothing, strTable, Nothing})
        Dim lstrColumns As New List(Of String)
        Dim astrExcl As String() = New String() {}

        ' Has exclusion entries for table?
        If s_dtExcludedDBEntries.ContainsKey(strTable) Then
            ' #Yes: get it
            astrExcl = s_dtExcludedDBEntries(strTable)
            ' Is an empty array?
            If (astrExcl.Length = 0) Then
                ' #Yes: No columns to write
                Return lstrColumns.ToArray()
            End If
        End If

        ' Summarize columns
        For Each drow As DataRow In dtTables.Rows
            Dim strCol As String = CStr(drow(3))
            If Array.IndexOf(astrExcl, strCol) = -1 Then
                lstrColumns.Add(strCol)
            End If
        Next
        Return lstrColumns.ToArray()

    End Function

    Private Function Field(rd As IDataReader, strCol As String) As String

        Dim data As Object = rd(strCol)

        If Convert.IsDBNull(data) Then Return ""

        If (TypeOf data Is String) Then
            Dim strData As String = CStr(data).Replace("""", "")
            If (strData.IndexOfAny(New Char() {";"c, ","c}) > -1) Then
                Return """" & strData & """"
            End If
            Return strData
        End If

        If (TypeOf data Is Boolean) Then Return data.ToString()

        Return cStringUtils.FormatNumber(data)

    End Function

    Private Function SaveTable(ByVal db As cEwEDatabase, ByVal strTable As String, ByVal doc As XmlDocument) As Boolean

        ' Skip system tables and bogus tables
        If (strTable.IndexOf("MSy") = 0) Then Return False
        If (strTable.IndexOfAny(New Char() {"_"c, " "c, "-"c, "."c}) > -1) Then Return False

        Dim astrCols As String() = Me.Columns(db, strTable)

        ' Skip table if nothing to write
        If (astrCols.Length = 0) Then Return True

        Dim row As IDataReader = db.GetReader("SELECT * FROM [" & strTable & "]")
        Dim xn As XmlNode = doc.CreateElement(strTable)
        Dim xa As XmlAttribute = Nothing
        Dim iNum As Integer = 0
        Dim sb As New StringBuilder()

        ' - Columns
        For Each strCol As String In astrCols
            If sb.Length > 0 Then sb.Append(",")
            sb.Append(strCol)
        Next
        xa = doc.CreateAttribute("Columns")
        xa.InnerText = sb.ToString
        xn.Attributes.Append(xa)

        sb.Length = 0
        While row.Read
            Dim b As Boolean = False
            For Each strCol As String In astrCols
                If b Then sb.Append(",")
                sb.Append(Me.Field(row, strCol))
                b = True
            Next
            sb.Append(";")
            iNum += 1
        End While

        ' Num rows
        xa = doc.CreateAttribute("Num")
        xa.InnerText = CStr(iNum)
        xn.Attributes.Append(xa)

        If (iNum > 0) Then
            xn.AppendChild(doc.CreateCDataSection(sb.ToString))
        End If

        doc.DocumentElement.AppendChild(xn)

        Return True

    End Function

#End Region ' Save from database

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

#Region " Modifications not allowed by this type of DS "

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

    Public Function AddPedigreeLevel(iPosition As Integer, strName As String, iColor As Integer, strDescription As String, varName As eVarNameFlags, sIndexValue As Single, sConfidence As Single, ByRef iDBID As Integer) As Boolean _
     Implements DataSources.IEcopathDataSource.AddPedigreeLevel
        Return False
    End Function

    Public Function MovePedigreeLevel(iDBID As Integer, iPosition As Integer) As Boolean Implements DataSources.IEcopathDataSource.MovePedigreeLevel
        Return False
    End Function

    Public Function RemovePedigreeLevel(iDBID As Integer) As Boolean Implements DataSources.IEcopathDataSource.RemovePedigreeLevel
        Return False
    End Function

#End Region ' Pedigree

#Region " Taxon "

    Public Function AddTaxon(iTargetDBID As Integer, bIsStanza As Boolean, data As ITaxonSearchData, sProportion As Single, ByRef iDBID As Integer) As Boolean _
        Implements DataSources.IEcopathDataSource.AddTaxon
        Return False
    End Function

    Public Function RemoveTaxon(iTaxonID As Integer) As Boolean _
        Implements DataSources.IEcopathDataSource.RemoveTaxon
        Return False
    End Function

#End Region ' Taxon

#End Region ' odifications not allowed by this type of DS

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

        Dim ecopathDS As cEcopathDataStructures = Me.m_core.m_EcoPathData
        Dim ecosimDS As cEcosimDatastructures = Me.m_core.m_EcoSimData
        Dim stanzaDS As cStanzaDatastructures = Me.m_core.m_Stanza
        Dim rdStanza As DataTable = Nothing
        Dim rdLifeStage As DataTable = Nothing
        Dim iStanza As Integer = 0
        Dim iLifeStage As Integer = 0
        Dim iGroup As Integer = 0
        Dim sTemp As Single = 0.0
        Dim bSucces As Boolean = True

        rdStanza = Me.ReadTable("Stanza")
        rdLifeStage = Me.ReadTable("StanzaLifeStage")

        ' Count the number of rows in StanzaInfo; this is the number of split groups that we're going to work with
        stanzaDS.Nsplit = rdStanza.Rows.Count
        ' Get max no of stanza
        stanzaDS.MaxStanza = 0

        If (stanzaDS.Nsplit > 0) Then
            'stanzaDS.MaxStanza = CInt(Me..GetValue("SELECT MAX(NumGroups) FROM (SELECT COUNT(*) AS NumGroups FROM StanzaLifeStage GROUP BY StanzaID) AS X", 0))
            Dim dic As New Dictionary(Of Integer, Integer)
            For Each row As DataRow In rdLifeStage.Rows
                iStanza = CInt(row("StanzaID"))
                If dic.ContainsKey(iStanza) Then iLifeStage = dic(iStanza) Else iLifeStage = 0
                iLifeStage += 1
                dic(iStanza) = iLifeStage
                stanzaDS.MaxStanza = Math.Max(stanzaDS.MaxStanza, iLifeStage)
            Next
        End If

        ' Get the number of groups from ecopath
        stanzaDS.nGroups = ecopathDS.NumGroups

        If stanzaDS.MaxAgeSplit < cCore.MAX_AGE Then
            'VILLY: NEED TO REPLACE THIS WITH DYNAMIC CALCULATION ALLOWING FOR CHANGES IN K DURING EXECUTION
            stanzaDS.MaxAgeSplit = cCore.MAX_AGE
        End If

        stanzaDS.redimStanza()

        ' First read Stanza
        iStanza = 0
        For Each row As DataRow In rdStanza.Rows

            ' JS 11May2010: Stanza configs without stanza groups are now loaded.
            '               This *could* screw up the core calculations, but in a way
            '               it already did by allowing empty stanza groups to be defined
            '               in the system by allowing stanzaDS.nGroups to be non-zero,
            '               even if stanzaDS.MaxStanza were 0. Based on this behaviour
            '               there seems little harm by allowing the empty stanza group
            '               names to be available in the core and to an interface.

            ' Read this stanza
            iStanza += 1

            Try

                stanzaDS.StanzaDBID(iStanza) = CInt(row("StanzaID"))
                ' JS 20jun06: StanzaName array 1-dimensional. GroupNames only seem to matter to the EwE5 GUI.
                '             EwE6 will resolve stanza group names via ICoreInputOutput objects to keep track of 'live' changes.
                stanzaDS.StanzaName(iStanza) = CStr(row("StanzaName"))

                stanzaDS.RecPowerSplit(iStanza) = CSng(row("RecPower"))
                stanzaDS.BABsplit(iStanza) = CSng(row("BabSplit"))
                stanzaDS.WmatWinf(iStanza) = CSng(row("WMatWinf"))
                ' stanzaDS.HatchCode(iStanza) = CInt(rdStanza("HatchCode"))
                stanzaDS.FixedFecundity(iStanza) = ParseBoolean(CStr(row("FixedFecundity")))
                stanzaDS.EggAtSpawn(iStanza) = ParseBoolean(CStr(Me.ReadSafe(row, "EggAtSpawn", True)))

                ' JS 23apr07: Leading B and QB groups are calculated at runtime, no longer stored in DB
                ' JS 23nov10: Hah, three and a half years later these values are stored again
                stanzaDS.BaseStanza(iStanza) = CInt(Me.ReadSafe(row, "LeadingLifeStage", cCore.NULL_VALUE))

                ' Truncate
                stanzaDS.BaseStanza(iStanza) = Math.Max(1, Math.Min(stanzaDS.Nstanza(iStanza), stanzaDS.BaseStanza(iStanza)))

            Catch ex As Exception
                Me.LogMessage(String.Format("Error {0} occurred while reading Stanza {1}", ex.Message, stanzaDS.StanzaName(iStanza)))
                bSucces = False
            End Try

            'rdLifeStage = Me..GetReader(String.Format("SELECT * FROM StanzaLifeStage WHERE (StanzaID={0}) ORDER BY AgeStart ASC", rdStanza("StanzaID")))
            rdLifeStage.DefaultView.RowFilter = "StanzaID=" & CInt(row("StanzaID"))
            rdLifeStage.DefaultView.Sort = "AgeStart ASC"
            iLifeStage = 0

            For Each rowStage As DataRow In rdLifeStage.DefaultView.ToTable.Rows
                ' Next life stage in this stanza
                iLifeStage += 1

                ' Store Stanza configuration
                Try

                    ' Resolve group index
                    iGroup = Array.IndexOf(ecopathDS.GroupDBID, CInt(rowStage("GroupID")))
                    ' JS 20jun06: Disabled (see comment above)
                    ' ecosimDS.StanzaName(nStanza, nGroup) = ecopathDS.GroupName(iGroup)
                    stanzaDS.EcopathCode(iStanza, iLifeStage) = iGroup
                    stanzaDS.Stanza_Z(iStanza, iLifeStage) = CSng(rowStage("Mortality"))
                    stanzaDS.SpeciesCode(iGroup, 0) = iStanza
                    stanzaDS.Age1(iStanza, iLifeStage) = CInt(rowStage("AgeStart"))

                Catch ex As Exception
                    Me.LogMessage(String.Format("Error {0} occurred while reading StanzaLifeStage {1}", ex.Message, stanzaDS.StanzaName(iStanza), ecopathDS.GroupName(iGroup)))
                    bSucces = False
                End Try

                ' Inform Ecopath
                ecopathDS.StanzaGroup(iGroup) = True
            Next
            ' Update number of groups in this stanza
            stanzaDS.Nstanza(iStanza) = iLifeStage
        Next

        rdStanza.Clear()
        rdLifeStage.Clear()

        Return bSucces
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

    Private Function ReadTable(strTable As String) As DataTable

        If Not strTable.StartsWith("/") Then
            strTable = "/EwEModel/" & strTable
        End If

        Dim xn As XmlNode = Me.m_doc.SelectSingleNode(strTable)
        Dim xnData As XmlCDataSection = DirectCast(xn.ChildNodes(0), XmlCDataSection)
        Dim xaCols As XmlAttribute = xn.Attributes("Columns")
        Dim astrRows As String() = Nothing
        Dim astrCols As String() = xaCols.InnerText.Split(","c)
        Dim dt As New DataTable(xn.Name)

        For i As Integer = 0 To astrCols.Length - 1
            dt.Columns.Add(astrCols(i), GetType(String))
        Next i

        If (xnData IsNot Nothing) Then
            astrRows = cStringUtils.SplitQualified(xnData.InnerText, ";")
            For Each strRow As String In astrRows
                If Not String.IsNullOrWhiteSpace(strRow) Then
                    Dim drow As DataRow = dt.NewRow()
                    Dim astrData As String() = Nothing
                    If strRow.Contains("""") Then astrData = cStringUtils.SplitQualified(strRow, ",") Else astrData = strRow.Split(","c)
                    For i As Integer = 0 To astrData.Length - 1
                        drow(astrCols(i)) = astrData(i)
                    Next
                    dt.Rows.Add(drow)
                End If
            Next
        End If

        Return dt

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper method, reads data from a column that may not exist. In that case,
    ''' an optional default value is returned
    ''' </summary>
    ''' <param name="row">The <see cref="DataRow"/> to read from.</param>
    ''' <param name="strField">The name of the DB field (column) to read.</param>
    ''' <param name="objValueDefault">A default value to return if the field could not be read.</param>
    ''' <param name="objValueIgnore">Value to interpret as 'no value. When encountered, the default value will be returned.</param>
    ''' <returns>The value of the requested column, or the provided default if an error occurred.</returns>
    ''' -----------------------------------------------------------------------
    Public Function ReadSafe(ByVal row As DataRow, _
                             ByVal strField As String, _
                             Optional ByVal objValueDefault As Object = Nothing, _
                             Optional ByVal objValueIgnore As Object = CSng(-9999)) As Object

        Dim objResult As Object = Nothing

        If (row Is Nothing) Then Return objValueDefault

        Try
            If row.Table.Columns.Contains(strField) Then
                objResult = row(strField)
            End If
        Catch ex As IndexOutOfRangeException
            ' Ugh
        Catch ex As InvalidOperationException
            'Console.WriteLine("DB: field '{0}' has no value, returning provided default '{1}'", strField, objValueDefault)
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Console.WriteLine("DB: Exception {2} occurred while accessing field '{0}', returning provided default '{1}'", strField, objValueDefault, ex.ToString)
        End Try

        If (Object.ReferenceEquals(objResult, Nothing) Or String.IsNullOrWhiteSpace(CStr(objResult))) Then
            objResult = objValueDefault
        ElseIf (Not Object.ReferenceEquals(objValueIgnore, Nothing)) _
            And Not (String.IsNullOrWhiteSpace(CStr(objResult))) _
            And Not (String.IsNullOrWhiteSpace(CStr(objValueIgnore))) Then

            ' Compare ignore values
            If TypeOf objResult Is String Then
                Try
                    If (String.Compare(CStr(objResult), Convert.ToString(objValueIgnore), True) = 0) Then
                        objResult = objValueDefault
                    End If
                Catch ex As Exception
                End Try
            ElseIf TypeOf objResult Is Boolean Then
                Try
                    If (CBool(objResult) = Convert.ToBoolean(objValueIgnore)) Then
                        objResult = objValueDefault
                    End If
                Catch ex As Exception
                End Try
            Else
                Try
                    If (CSng(objResult) = Convert.ToSingle(objValueIgnore)) Then
                        objResult = objValueDefault
                    End If
                Catch ex As Exception
                End Try
            End If

        End If

        If (Convert.IsDBNull(objResult)) Then
            objResult = objValueDefault
        End If

        Return objResult
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Logs a message to the application log.
    ''' </summary>
    ''' -------------------------------------------------------------------
    Private Sub LogMessage(ByVal strMessage As String, _
            Optional ByVal msgType As eMessageType = eMessageType.DataModified, _
            Optional ByVal msgImportance As eMessageImportance = eMessageImportance.Information)

        If (Me.m_core IsNot Nothing) Then
            Me.m_core.m_publisher.AddMessage(New cMessage(strMessage, msgType, eCoreComponentType.DataSource, msgImportance))
        End If
        'Console.WriteLine(strMessage)

    End Sub

    Private Function ParseBoolean(strVal As String) As Boolean
        If String.IsNullOrWhiteSpace(strVal) Then Return False
        If strVal = "1" Then Return True
        If strVal = "0" Then Return False
        Return Boolean.Parse(strVal)
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

#End Region ' Helper methods

End Class

