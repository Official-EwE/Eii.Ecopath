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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Data.SqlClient
Imports System.IO
Imports System.Reflection
Imports System.Text
Imports EwECore
Imports EwEPlugin
Imports EwEPlugin.Data
Imports EwEUtils.Core
Imports EwEUtils.Database

#End Region

''' ---------------------------------------------------------------------------
''' <summary>
''' Central plug-in point for this plug-in.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cSAUPTaxonDataPlugin
    Implements IDataSearchProducerPlugin
    Implements IConfigurablePlugin
    Implements IDisposedPlugin

#Region " Private vars "

    Private m_bInitOk As Boolean = False
    Private m_core As cCore = Nothing

    ''' <summary>Broadcaster for distributing data.</summary>
    Private m_broadcaster As IDataBroadcaster = Nothing
    ''' <summary>Search term.</summary>
    Private m_term As ITaxonSearchData = Nothing
    ''' <summary>Search results.</summary>
    Private m_results As cSAUPTaxonSearchResults = Nothing
    ''' <summary>Data provider enabled state.</summary>
    Private m_bEnabled As Boolean = True
    ''' <summary>Flag stating whether a search is in progress.</summary>
    Private m_bSearching As Boolean = False

    Friend Enum eConnectionType As Integer
        SQLServer = 0
        Access = 1
    End Enum

    Private m_conntype As eConnectionType = eConnectionType.SQLServer
    Private m_strAccessDatabase As String = ""
    Private m_strSQLDatabase As String = ""
    Private m_strSQLHost As String = ""
    Private m_strSQLUserName As String = ""
    Private m_strSQLPassword As String = ""

    ''' <summary>Taxon database connection.</summary>
    Private m_conn As IDbConnection = Nothing

#End Region ' Private vars

#Region " Plugin points implementation "

#Region " Init "

    ''' <inheritdocs cref="IPlugin.Initialize"/>
    Friend Sub Initialize(ByVal core As Object) _
        Implements IPlugin.Initialize

        Debug.Assert(TypeOf core Is EwECore.cCore, Me.ToString & ".Initialize() argument core is not a cCore object.")
        m_bInitOk = False
        Try
            If TypeOf core Is EwECore.cCore Then
                Me.m_core = DirectCast(core, EwECore.cCore)
                Me.m_bInitOk = True
                'System.Console.WriteLine(Me.ToString & ".Initialize() Successfull.")
            Else
                'some kind of a message
                System.Console.WriteLine(Me.ToString & ".Initialize() Failed.")
                Return
            End If
        Catch ex As Exception
            cLog.Write(ex)
            System.Console.WriteLine(Me.ToString & ".Initialize() Error: " & ex.Message)
            Debug.Assert(False, ex.Message)
            Return
        End Try

    End Sub

#End Region ' Init

#Region " Generic "

    ''' <inheritdocs cref="IPlugin.Author"/>
    Friend ReadOnly Property Author() As String _
        Implements IPlugin.Author
        Get
            Return "UBC Fisheries Centre"
        End Get
    End Property

    ''' <inheritdocs cref="IPlugin.Contact"/>
    Friend ReadOnly Property Contact() As String _
        Implements IPlugin.Contact
        Get
            Return "mailto:ewedevteam@gmail.com"
        End Get
    End Property

    ''' <inheritdocs cref="IPlugin.Description"/>
    Friend ReadOnly Property Description() As String _
        Implements IPlugin.Description
        Get
            Return "Plug-in for obtaining taxonomy data from the SAUP taxonomy database"
        End Get
    End Property

    ''' <inheritdocs cref="IPlugin.Name"/>
    Friend ReadOnly Property Name() As String _
        Implements IPlugin.Name
        Get
            Return "SAUP taxon search"
        End Get
    End Property

#End Region ' Generic

#Region " Data "

    ''' <inheritdocs cref="IDataProducerPlugin.Broadcaster"/>
    Friend Sub Broadcaster(ByVal broadcaster As IDataBroadcaster) _
        Implements IDataProducerPlugin.Broadcaster
        Me.m_broadcaster = broadcaster
    End Sub

    ''' <inheritdocs cref="IDataProducerPlugin.GetDataByType"/>
    Friend Function GetDataByType(ByVal typeData As System.Type, ByRef data As IPluginData) As Boolean _
        Implements IDataProducerPlugin.GetDataByType
        If (TypeOf data Is ITaxonSearchData) Then data = DirectCast(Me.m_term, IPluginData)
        Return Me.IsEnabled And Me.IsConfigured
    End Function

    ''' <inheritdocs cref="IDataProducerPlugin.IsDataAvailable"/>
    Friend Function IsDataAvailable(ByVal typeData As System.Type, _
                                    Optional ByVal runType As EwEUtils.Core.IRunType = Nothing) As Boolean _
        Implements IDataProducerPlugin.IsDataAvailable
        Return (GetType(ITaxonSearchData).IsAssignableFrom(typeData))
    End Function

    ''' <inheritdocs cref="IDataProducerPlugin.IsEnabled"/>
    Friend Function IsEnabled() As Boolean _
        Implements IDataProducerPlugin.IsEnabled
        Return Me.m_bEnabled
    End Function

    ''' <inheritdocs cref="IDataProducerPlugin.IsEnabled"/>
    Friend Function IsEnabled(ByVal typeData As System.Type, _
                              ByVal runType As EwEUtils.Core.IRunType) As Boolean _
        Implements IDataProducerPlugin.IsEnabled
        Return Me.m_bEnabled
    End Function

    ''' <inheritdocs cref="IDataProducerPlugin.SetEnabled"/>
    Friend Function SetEnabled(ByVal bEnable As Boolean) As Boolean _
        Implements IDataProducerPlugin.SetEnabled
        Me.m_bEnabled = bEnable
    End Function

    ''' <inheritdocs cref="IDataProducerPlugin.SetEnabled"/>
    Friend Sub SetEnabled(ByVal typeData As System.Type, _
                          ByVal runType As EwEUtils.Core.IRunType, _
                          ByVal bEnable As Boolean) _
        Implements IDataProducerPlugin.SetEnabled
        ' NOP
    End Sub

#End Region ' Data

#Region " Search "

    ''' <summary>
    ''' SAUP taxonomy levels as defined in Taxonnom / taxon db
    ''' </summary>
    Private Enum SAUPTaxLevel As Integer
        Phylum = 1
        [Class] = 2
        Order = 3
        Family = 4
        Genus = 5
        Species = 6
    End Enum

    ''' <inheritdocs cref="IDataSearchProducerPlugin.StartSearch"/>
    Friend Function StartSearch(ByVal data As Object, iMaxResults As Integer) As Boolean _
        Implements IDataSearchProducerPlugin.StartSearch

        If Not (Me.IsConnected() And Me.IsEnabled()) Then Return False
        ' Test data type
        If Not (TypeOf data Is ITaxonSearchData) Then Return False

        ' Get ready
        Me.m_term = DirectCast(data, ITaxonSearchData)
        Me.m_results = Nothing

        Return Me.Search(DirectCast(data, ITaxonSearchData), iMaxResults)

    End Function

    ''' <inheritdocs cref="IDataSearchProducerPlugin.StartSearch"/>
    Friend Function StopSearch() As Boolean _
        Implements IDataSearchProducerPlugin.StopSearch
        ' Do not perform any action since searches are instantaneous.
        Return True
    End Function

    ''' <inheritdocs cref="IDataSearchProducerPlugin.IsSeaching"/>
    Friend Function IsSeaching() As Boolean _
        Implements IDataSearchProducerPlugin.IsSeaching
        Return Me.m_bSearching
    End Function

    ''' <inheritdocs cref="IDataSearchProducerPlugin.SearchResults"/>
    Friend Function SearchResults(ByVal dataTerm As Object, ByRef results As IDataSearchResults) As Boolean _
        Implements IDataSearchProducerPlugin.SearchResults

        If (Object.ReferenceEquals(dataTerm, Me.m_term)) Then
            results = Me.m_results
            Return True
        End If
        Return False

    End Function

    ''' <inheritdocs cref="IDataSearchProducerPlugin.CreateSearchTerm"/>
    Public Function CreateSearchTerm() As Object _
        Implements EwEPlugin.Data.IDataSearchProducerPlugin.CreateSearchTerm
        Return New cSAUPTaxonData(EwEUtils.Utilities.cTypeUtils.TypeToString(Me.GetType()))
    End Function

#End Region ' Search

#Region " Configurable "

    ''' <inheritdocs cref="IConfigurablePlugin.IsConfigured"/>
    Friend Function IsConfigured() As Boolean _
        Implements IConfigurablePlugin.IsConfigured
        ' Configuration ready when connected to DB
        Return Me.IsConnected()
    End Function

    ''' <inheritdocs cref="IConfigurablePlugin.GetConfigUI"/>
    Friend Function GetConfigUI() As System.Windows.Forms.Control _
        Implements IConfigurablePlugin.GetConfigUI
        Return New frmConfig(Me)
    End Function

#End Region ' Configurable

#Region " Disposal "

    ''' -----------------------------------------------------------------------
    ''' <inheritdocs cref="IDisposedPlugin.Dispose"/>
    ''' -----------------------------------------------------------------------
    Friend Sub Dispose() _
        Implements IDisposedPlugin.Dispose
        ' Just in case
        Me.Disconnect()
    End Sub

#End Region ' Disposal

#End Region ' Plugin points implementation

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Execute a database search.
    ''' </summary>
    ''' <param name="taxon">The term to search for.</param>
    ''' <returns>True if successful.</returns>
    ''' <remarks>
    ''' If successful, the local results will be populated.
    ''' </remarks>
    ''' -----------------------------------------------------------------------
    Private Function Search(ByVal taxon As ITaxonSearchData, iMaxResults As Integer) As Boolean

        Debug.Assert(Me.IsConnected())

        Dim qb As New cQueryBuilder("SELECT TOP " & iMaxResults & " * FROM TAXON [WHERE]")
        Dim reader As IDataReader = Nothing
        Dim sbFilter As New StringBuilder
        Dim lResults As New List(Of ITaxonSearchData)

        ' Started searching
        Me.m_bSearching = True

        Try
            ' Search term is only contained in taxon.Common
            ' SearchFields will determine which taxon levels should be searched
            If (Not String.IsNullOrWhiteSpace(taxon.SourceKey)) And (String.Compare(taxon.Source, Me.Name, True) = 0) Then
                ' Refresh
                qb.AddClause(String.Format("TaxonKey={0}", Long.Parse(taxon.SourceKey)))
            Else
                ' Early bail-out
                If String.IsNullOrWhiteSpace(taxon.Common) Then Return False

                ' Search
                If Not String.IsNullOrWhiteSpace(taxon.Common) And ((taxon.SearchFields And eTaxonClassificationType.Common) > 0) Then
                    qb.AddClause(String.Format("CommonName LIKE '%{0}%' OR TaxonName LIKE '%{0}%'", taxon.Common))
                Else
                    qb.AddClause(String.Format("TaxonName LIKE '%{0}%'", taxon.Common))
                End If

                If ((taxon.SearchFields And eTaxonClassificationType.Phylum) > 0) Then
                    If sbFilter.Length > 0 Then sbFilter.Append(",")
                    sbFilter.Append(SAUPTaxLevel.Phylum)
                End If
                If ((taxon.SearchFields And eTaxonClassificationType.Class) > 0) Then
                    If sbFilter.Length > 0 Then sbFilter.Append(",")
                    sbFilter.Append(SAUPTaxLevel.Class)
                End If
                If ((taxon.SearchFields And eTaxonClassificationType.Order) > 0) Then
                    If sbFilter.Length > 0 Then sbFilter.Append(",")
                    sbFilter.Append(SAUPTaxLevel.Order)
                End If
                If ((taxon.SearchFields And eTaxonClassificationType.Family) > 0) Then
                    If sbFilter.Length > 0 Then sbFilter.Append(",")
                    sbFilter.Append(SAUPTaxLevel.Family)
                End If
                If ((taxon.SearchFields And eTaxonClassificationType.Genus) > 0) Then
                    If sbFilter.Length > 0 Then sbFilter.Append(",")
                    sbFilter.Append(SAUPTaxLevel.Genus)
                End If
                If ((taxon.SearchFields And eTaxonClassificationType.Species) > 0) Then
                    If sbFilter.Length > 0 Then sbFilter.Append(",")
                    sbFilter.Append(SAUPTaxLevel.Species)
                End If

                If (sbFilter.Length > 0) Then
                    qb.AddClause(String.Format("TaxLevel IN ({0})", sbFilter.ToString))
                End If

                If (taxon.North > cCore.NULL_VALUE) Or (taxon.South > cCore.NULL_VALUE) Then
                    ' Filter using TaxonDist table, LatNorth and LatSouth fields
                    ' East and west are not supported in SAUP taxon table
                    sbFilter.Length = 0
                    sbFilter.Append("EXISTS (SELECT * FROM TaxonDist WHERE Taxon.TaxonKey=TaxonDist.TaxonKey")
                    If (taxon.North > cCore.NULL_VALUE) Then sbFilter.Append(" AND TaxonDist.LatNorth >= " & taxon.North)
                    If (taxon.South > cCore.NULL_VALUE) Then sbFilter.Append(" AND TaxonDist.LatSouth <= " & taxon.South)
                    sbFilter.Append(")")
                    qb.AddClause(sbFilter.ToString)
                End If

            End If

        Catch ex As Exception
            ' Hmm
            Debug.Assert(False, ex.Message)
        End Try

        Try
            Dim strQuery As String = qb.ToString()
            Select Case Me.ConnectionType

                Case eConnectionType.Access
                    Debug.Assert(TypeOf Me.m_conn Is OleDb.OleDbConnection)

                    Dim dbC As OleDb.OleDbCommand = New OleDb.OleDbCommand(strQuery, DirectCast(Me.m_conn, OleDb.OleDbConnection))
                    reader = dbC.ExecuteReader()

                Case eConnectionType.SQLServer
                    Debug.Assert(TypeOf Me.m_conn Is SqlClient.SqlConnection)

                    Dim sqlC As SqlClient.SqlCommand = New SqlCommand(strQuery, DirectCast(Me.m_conn, SqlClient.SqlConnection))
                    reader = sqlC.ExecuteReader()

            End Select

        Catch ex As Exception
            Debug.Assert(False, ex.Message)
            Return False
        End Try

        Try
            While reader.Read()
                lResults.Add(Me.ReadTaxon(reader))
            End While
        Catch ex As Exception
            ' Woops
        End Try

        reader.Close()
        reader = Nothing

        ' Create new results
        Me.m_results = New cSAUPTaxonSearchResults(Me.m_term, lResults.ToArray(), EwEUtils.Utilities.cTypeUtils.TypeToString(Me.GetType()))
        ' Broadcast results
        Me.m_broadcaster.BroadcastData(Me.Name, Me.m_results)

        ' Done searching
        Me.m_bSearching = False

        Return True

    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Not very fail-safe method to read a string from a database reader.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="strField"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function ReadSave(ByVal reader As IDataReader, ByVal strField As String) As String
        If Not Convert.IsDBNull(reader(strField)) Then Return CStr(reader(strField))
        Return ""
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Read a taxon record from a database reader.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function ReadTaxon(ByVal reader As IDataReader) As ITaxonSearchData

        Dim taxon As New cSAUPTaxonData(EwEUtils.Utilities.cTypeUtils.TypeToString(Me.GetType()))

        taxon.Common = Me.ReadSave(reader, "CommonName")
        Select Case DirectCast(CInt(Me.ReadSave(reader, "TaxLevel")), SAUPTaxLevel)
            Case SAUPTaxLevel.Phylum
                taxon.Phylum = Me.ReadSave(reader, "TaxonName")
                taxon.SearchFields = eTaxonClassificationType.Phylum
            Case SAUPTaxLevel.Class
                taxon.Class = Me.ReadSave(reader, "TaxonName")
                taxon.SearchFields = eTaxonClassificationType.Class
            Case SAUPTaxLevel.Order
                taxon.Order = Me.ReadSave(reader, "TaxonName")
                taxon.SearchFields = eTaxonClassificationType.Order
            Case SAUPTaxLevel.Family
                taxon.Family = Me.ReadSave(reader, "TaxonName")
                taxon.SearchFields = eTaxonClassificationType.Family
            Case SAUPTaxLevel.Genus
                taxon.Genus = Me.ReadSave(reader, "TaxonName")
                taxon.SearchFields = eTaxonClassificationType.Genus
            Case SAUPTaxLevel.Species
                taxon.Species = Me.ReadSave(reader, "TaxonName")
                taxon.SearchFields = eTaxonClassificationType.Species
        End Select
        taxon.SourceKey = Me.ReadSave(reader, "TaxonKey")
        taxon.CodeSAUP = Long.Parse(taxon.SourceKey)
        Single.TryParse(Me.ReadSave(reader, "LatNorth"), taxon.North)
        Single.TryParse(Me.ReadSave(reader, "Latsouth"), taxon.South)

        Return taxon

    End Function

    Friend Sub ReadConfiguration()

        Try
            Me.ConnectionType = DirectCast(My.Settings.ConnType, eConnectionType)
            Me.AccessDatabase = My.Settings.AccessDB
            Me.SQLHost = My.Settings.SQLHost
            Me.SQLDatabase = My.Settings.SQLDB
            Me.SQLUserName = My.Settings.SQLUser
            Me.SQLPassword = My.Settings.SQLPassword

        Catch ex As Exception

        End Try

    End Sub

    Friend Sub WriteConfiguration()

        My.Settings.ConnType = Me.ConnectionType
        My.Settings.AccessDB = Me.AccessDatabase
        My.Settings.SQLHost = Me.SQLHost
        My.Settings.SQLDB = Me.SQLDatabase
        My.Settings.SQLUser = Me.SQLUserName
        My.Settings.SQLPassword = Me.SQLPassword
        My.Settings.Save()

    End Sub

#End Region ' Internals

#Region " Friendly bits "

    Friend Property ConnectionType() As eConnectionType
        Get
            Return Me.m_conntype
        End Get
        Set(ByVal value As eConnectionType)
            Me.m_conntype = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the SQL server database to connect to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Property AccessDatabase() As String
        Get
            Return Me.m_strAccessDatabase
        End Get
        Set(ByVal value As String)
            Me.m_strAccessDatabase = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the SQL server host mchine to connect to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Property SQLHost() As String
        Get
            Return Me.m_strSQLHost
        End Get
        Set(ByVal value As String)
            Me.m_strSQLHost = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the SQL server database to connect to.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Property SQLDatabase() As String
        Get
            Return Me.m_strSQLDatabase
        End Get
        Set(ByVal value As String)
            Me.m_strSQLDatabase = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the SQL server database username to connect with.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Property SQLUserName() As String
        Get
            Return Me.m_strSQLUserName
        End Get
        Set(ByVal value As String)
            Me.m_strSQLUserName = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Get/set the name of the SQL server database password to connect with.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Friend Property SQLPassword() As String
        Get
            Return Me.m_strSQLPassword
        End Get
        Set(ByVal value As String)
            Me.m_strSQLPassword = value
        End Set
    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' States whether the plug-in is connected to the SAUP taxon database.
    ''' </summary>
    ''' <returns>True if connected.</returns>
    ''' -----------------------------------------------------------------------
    Friend Function IsConnected() As Boolean
        Return (Me.m_conn IsNot Nothing)
    End Function

    Friend Sub Disconnect()
        If Not Me.IsConnected Then Return
        Me.m_conn.Close()
        Me.m_conn = Nothing
    End Sub

    Friend Sub Connect()
        Try
            ' Just in case
            Me.Disconnect()

            ' Sanity check
            Debug.Assert(Not Me.IsConnected)

            ' Hook up new
            Select Case Me.ConnectionType

                Case eConnectionType.Access
                    Dim strConnect As String = ""
                    Select Case Path.GetExtension(Me.AccessDatabase).ToLower
                        Case ".mdb"
                            strConnect = "PROVIDER=Microsoft.Jet.OLEDB.4.0; Data Source=" & Me.AccessDatabase
                        Case ".accdb"
                            strConnect = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Me.AccessDatabase & ";Persist Security Info=False;"
                    End Select
                    Me.m_conn = New OleDb.OleDbConnection(strConnect)

                Case eConnectionType.SQLServer
                    Dim strConnect As String = String.Format("Initial Catalog={0};Data Source={1};User ID={2};Password={3};Connection Timeout=500;", _
                                                             Me.SQLDatabase, Me.SQLHost, Me.SQLUserName, Me.SQLPassword)
                    Me.m_conn = New SqlClient.SqlConnection(strConnect)

                Case Else
                    Debug.Assert(False)

            End Select
        Catch ex As Exception
            ' Whoah!
            Me.m_conn = Nothing
        End Try
    End Sub

#End Region ' Friendly bits

End Class

