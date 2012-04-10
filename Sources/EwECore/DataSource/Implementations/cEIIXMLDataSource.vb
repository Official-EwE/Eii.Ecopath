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
            'bSucces = bSucces And Me.LoadEcopathTaxon()
            'bSucces = bSucces And Me.LoadEcopathFleetInfo()
            'bSucces = bSucces And Me.LoadParticleSizeDistribution()
            'bSucces = bSucces And Me.LoadPedigreeLevels()
            'bSucces = bSucces And Me.LoadPedigreeAssignments()

            'bSucces = bSucces And Me.LoadAuxillaryData()

            'ecopathDS.bInitialized = bSucces
            'ecopathDS.onPostInitialization()

            'bSucces = bSucces And Me.LoadEcosimScenarioDefinitions()
            'bSucces = bSucces And Me.LoadEcospaceScenarioDefinitions()
            'bSucces = bSucces And Me.LoadEcotracerScenarioDefinitions()
            'bSucces = bSucces And Me.LoadTimeSeriesDatasets()

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

        For Each row As DataRow In dt.Rows

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

                ecopathDS.GroupColor(iGroup) = Integer.Parse(CStr(Me.ReadSafe(row, "PoolColor", "0")), Globalization.NumberStyles.HexNumber)

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
                ' ecopathDS.??(nPred, nPrey) = CSng(reader("MTI"))
                ' VERIFY_JS: check mapping for Electivity with JB
                ' ecopathDS.??(nPred, nPrey) = CSng(reader("Electivity"))
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
        If (TypeOf data Is String) Then Return CStr(data).Replace(";"c, ":"c).Replace(","c, "."c)
        If (TypeOf data Is Boolean) Then Return data.ToString()
        Return cStringUtils.FormatNumber(data)
    End Function

    Private Function SaveTable(ByVal db As cEwEDatabase, ByVal strTable As String, ByVal doc As XmlDocument) As Boolean

        ' Skip system tables
        If strTable.IndexOf("MSy") = 0 Then Return False

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
            'stanzaDS.MaxStanza = CInt(Me.m_db.GetValue("SELECT MAX(NumGroups) FROM (SELECT COUNT(*) AS NumGroups FROM StanzaLifeStage GROUP BY StanzaID) AS X", 0))
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

            'rdLifeStage = Me.m_db.GetReader(String.Format("SELECT * FROM StanzaLifeStage WHERE (StanzaID={0}) ORDER BY AgeStart ASC", rdStanza("StanzaID")))
            rdLifeStage.DefaultView.RowFilter = "StanzaID=" & CInt(row("StanzaID"))
            rdLifeStage.DefaultView.Sort = "AgeStart ASC"
            iLifeStage = 0

            For Each rowStage As DataRow In rdLifeStage.Rows
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

    Private Function ReadTable(strTable As String) As DataTable

        If Not strTable.StartsWith("/") Then
            strTable = "/EwEModel/" & strTable
        End If

        Dim xn As XmlNode = Me.m_doc.SelectSingleNode(strTable)
        Dim xnData As XmlCDataSection = DirectCast(xn.ChildNodes(0), XmlCDataSection)
        Dim xaCols As XmlAttribute = xn.Attributes("Columns")
        Dim astrCols As String() = xaCols.InnerText.Split(","c)
        Dim dt As New DataTable(xn.Name)

        For i As Integer = 0 To astrCols.Length - 1
            dt.Columns.Add(astrCols(i), GetType(String))
        Next i

        For Each strRow As String In cStringUtils.SplitQualified(xnData.InnerText, ";")
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

        If (Object.ReferenceEquals(objResult, Nothing)) Then
            objResult = objValueDefault
        ElseIf (Not Object.ReferenceEquals(objValueIgnore, Nothing)) _
            And Not (Convert.IsDBNull(objResult)) _
            And Not (Convert.IsDBNull(objValueIgnore)) Then

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

#End Region ' Helper methods

End Class

