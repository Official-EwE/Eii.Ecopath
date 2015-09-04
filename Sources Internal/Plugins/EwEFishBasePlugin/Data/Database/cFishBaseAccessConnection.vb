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
Imports System.Text
Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Database
Imports EwEUtils.Utilities

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Buffer class for interfacing with a FishBase Access database.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cFishBaseAccessConnnection
    Inherits cFishBaseConnection

#Region " Private vars "

    Private m_thread As Threading.Thread = Nothing
    Private m_strSQL As String = ""
    Private m_conn As OleDb.OleDbConnection = Nothing

#End Region ' Private vars 

#Region " Construction / destruction "

    Public Sub New(ppt As cFishBasePlugin)
        MyBase.New(ppt)
    End Sub

    Public Overrides Sub Dispose()
    End Sub

#End Region ' Construction / destruction

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Attempt to connect the FishBase server.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Shadows Function Connect(strMDB As String) As Boolean
        If Me.IsConnected Then Me.Disconnect()
        Dim str As String = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Persist Security Info=False;", strMDB)
        Dim conn As New OleDb.OleDbConnection(str)
        Try
            conn.Open()
            If (conn.State = ConnectionState.Open) Then
                Me.m_conn = conn
                Me.IsConnected = True
            End If
            Return True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Exclamation Or MsgBoxStyle.OkOnly)
        End Try
        Return False
    End Function

    Public Overrides Function Disconnect() As Boolean
        If (Me.IsConnected) Then
            Me.m_conn.Close()
            Me.m_conn.Dispose()
            Me.m_conn = Nothing
            Me.IsConnected = False
        End If
        Return True
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Start executing a FishBase search
    ''' </summary>
    ''' <param name="taxon"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public Overrides Function Search(ByVal taxon As ITaxonSearchData, iMaxResults As Integer) As Boolean

        If (Not MyBase.Search(taxon, iMaxResults)) Then Return False

        If (Me.m_thread IsNot Nothing) Then
            Try
                Me.m_thread.Abort()
            Catch ex As Exception
                ' Ok
            End Try
            Me.m_thread = Nothing
        End If

        Dim qb As New cQueryBuilder("SELECT species.DateModified As DateModified, species.SpecCode AS Code, classes.Class AS Cls, orders.Order AS Ord, families.Family AS Fam, genera.GenName AS Gen, species.Species AS Spec, comnames.ComName AS Comm, species.Vulnerability AS Vul, stocks.IUCN_Code AS IUCN, stocks.Occurrence AS Occ, stocks.Ecology AS Eco, stocks.SAUP_ID AS CodeSaup FROM comnames INNER JOIN (((((stocks INNER JOIN species ON stocks.SpecCode = species.SpecCode) INNER JOIN families ON species.FamCode = families.FamCode) INNER JOIN classes ON families.Class = classes.Class) INNER JOIN genera ON species.Genus = genera.GenName) INNER JOIN orders ON families.Ordnum = orders.Ordnum) ON comnames.SpecCode = stocks.SpecCode [WHERE] ORDER BY species.SpecCode")
        Dim bSearchComm As Boolean = ((taxon.SearchFields And eTaxonClassificationType.Common) > 0)

        ' Started searching
        Me.IsSearching = True

        Try
            ' Search term is only contained in taxon.Common
            ' SearchFields will determine which taxon levels should be searched
            If (Not String.IsNullOrWhiteSpace(taxon.SourceKey)) And (String.Compare(taxon.Source, Me.PluginPoint.Name, True) = 0) Then
                ' Refresh
                qb.AddClause(String.Format("species.SpecCode={0}", Long.Parse(taxon.SourceKey)))
            Else
                ' Early bail-out
                If String.IsNullOrWhiteSpace(taxon.Common) Then Return False

                Dim sbClause As New StringBuilder()

                ' Search
                If bSearchComm Then
                    sbClause.Append(String.Format("comnames.ComName LIKE '{0}%'", taxon.Common))
                End If

                If ((taxon.SearchFields And eTaxonClassificationType.Phylum) > 0) Then
                    If sbClause.Length > 0 Then sbClause.Append(" OR")
                    If bSearchComm Then
                        sbClause.Append(String.Format("(classes.Class LIKE ""{0}%"") OR (classes.CommonName LIKE ""{0}%"")", taxon.Common))
                    Else
                        sbClause.Append(String.Format("(classes.Class LIKE ""{0}%"")", taxon.Common))
                    End If
                End If

                If ((taxon.SearchFields And eTaxonClassificationType.Class) > 0) Then
                    If sbClause.Length > 0 Then sbClause.Append(" OR")
                    If bSearchComm Then
                        sbClause.Append(String.Format("(classes.Class LIKE ""{0}%"") OR (classes.CommonName LIKE ""{0}%"")", taxon.Common))
                    Else
                        sbClause.Append(String.Format("(classes.Class LIKE ""{0}%"")", taxon.Common))
                    End If
                End If

                If ((taxon.SearchFields And eTaxonClassificationType.Order) > 0) Then
                    If sbClause.Length > 0 Then sbClause.Append(" OR")
                    If bSearchComm Then
                        sbClause.Append(String.Format("(orders.Order LIKE ""{0}%"") OR (orders.CommonName LIKE ""{0}%"")", taxon.Common))
                    Else
                        sbClause.Append(String.Format("(orders.Order LIKE ""{0}%"")", taxon.Common))
                    End If
                End If

                If ((taxon.SearchFields And eTaxonClassificationType.Family) > 0) Then
                    If sbClause.Length > 0 Then sbClause.Append(" OR")
                    If bSearchComm Then
                        sbClause.Append(String.Format("(families.Family LIKE ""{0}%"") OR (families.CommonName LIKE ""{0}%"")", taxon.Common))
                    Else
                        sbClause.Append(String.Format("(families.Family LIKE ""{0}%"")", taxon.Common))
                    End If
                End If

                If ((taxon.SearchFields And eTaxonClassificationType.Genus) > 0) Then
                    If sbClause.Length > 0 Then sbClause.Append(" OR")
                    If bSearchComm Then
                        sbClause.Append(String.Format("(genera.GenName LIKE ""{0}%"") OR (genera.GenComName LIKE ""{0}%"")", taxon.Common))
                    Else
                        sbClause.Append(String.Format("(genera.GenName LIKE ""{0}%"")", taxon.Common))
                    End If
                End If

                If ((taxon.SearchFields And eTaxonClassificationType.Species) > 0) Then
                    If sbClause.Length > 0 Then sbClause.Append(" OR")
                    If bSearchComm Then
                        sbClause.Append(String.Format("(species.Species LIKE ""{0}%"") OR (comnames.ComName LIKE ""{0}%"")", taxon.Common))
                    Else
                        sbClause.Append(String.Format("(species.Species LIKE ""{0}%"")", taxon.Common))
                    End If
                End If
                qb.AddClause(sbClause.ToString)

                If (taxon.North > cCore.NULL_VALUE) Or (taxon.South > cCore.NULL_VALUE) Then
                    ' Filter using stocks extent fields
                    Dim sbFilter As New StringBuilder()
                    sbFilter.Append("EXISTS (SELECT * FROM stocks WHERE species.SpecCode=stocks.SpecCode")
                    If (taxon.North > cCore.NULL_VALUE) Then sbFilter.Append(" AND stocks.NorthernMost >= " & taxon.North)
                    If (taxon.South > cCore.NULL_VALUE) Then sbFilter.Append(" AND stocks.SouthernMost <= " & taxon.South)
                    'If (taxon.West > cCore.NULL_VALUE) Then sbFilter.Append(" AND stocks.WesternMost >= " & taxon.North)
                    'If (taxon.East > cCore.NULL_VALUE) Then sbFilter.Append(" AND stocks.EasternMost <= " & taxon.South)
                    sbFilter.Append(")")
                    qb.AddClause(sbFilter.ToString)
                End If

            End If

        Catch ex As Exception
            ' Hmm
            Debug.Assert(False, ex.Message)
        End Try

        Me.m_strSQL = qb.ToString

        Me.m_thread = New Threading.Thread(AddressOf Me.SearchThreaded)
        Me.m_thread.Start()

    End Function

    Private Sub SearchThreaded()

        Dim cmd As New OleDb.OleDbCommand(Me.m_strSQL, Me.m_conn)
        Dim reader As IDataReader = Nothing
        Dim iRow As Integer = 0
        Dim lResults As New List(Of ITaxonSearchData)

        Try
            reader = cmd.ExecuteReader()
        Catch ex As Threading.ThreadAbortException
            If reader IsNot Nothing Then
                reader.Close()
                reader = Nothing
            End If
            ' Done searching
            Me.m_thread = Nothing
            Me.IsSearching = False
            Return
        Catch ex As Exception
            Debug.Assert(False, ex.Message)
        End Try

        Try
            While reader.Read() And (iRow < Me.PluginPoint.MaxResults)
                lResults.Add(Me.ReadTaxon(reader))
                iRow += 1
            End While
        Catch ex As Exception
            ' Woops
        End Try
        reader.Close()

        ' Hand out results
        Me.PluginPoint.BroadcastResults(lResults.ToArray())

        ' Done searching
        Me.m_thread = Nothing
        Me.IsSearching = False

    End Sub

#Region " Internals "


    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Not very fail-safe method to read a string from a database reader.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <param name="strField"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function ReadSave(ByVal reader As IDataReader, ByVal strField As String, Optional strDefault As String = "") As String
        If Not Convert.IsDBNull(reader(strField)) Then Return CStr(reader(strField))
        Return strDefault
    End Function

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Read a taxon record from a database reader.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Private Function ReadTaxon(ByVal reader As IDataReader) As ITaxonSearchData

        Dim taxon As cFishBaseTaxonData = DirectCast(Me.PluginPoint.CreateSearchTerm(), cFishBaseTaxonData)
        Dim strDateModified As String = Me.ReadSave(reader, "DateModified", "")
        Dim dt As Date

        taxon.SourceKey = Me.ReadSave(reader, "Code")
        taxon.CodeFB = Convert.ToInt32(taxon.SourceKey)
        taxon.CodeSAUP = CLng(Me.ReadSave(reader, "CodeSAUP", "0"))

        'taxon.Phylum = Me.ReadSave(reader, "TaxonName")
        taxon.Class = Me.ReadSave(reader, "Cls")
        taxon.Order = Me.ReadSave(reader, "Ord")
        taxon.Family = Me.ReadSave(reader, "Fam")
        taxon.Genus = Me.ReadSave(reader, "Gen")
        taxon.Species = Me.ReadSave(reader, "Spec")
        taxon.Common = Me.ReadSave(reader, "Comm")
        'Single.TryParse(Me.ReadSave(reader, "LatNorth"), taxon.North)
        'Single.TryParse(Me.ReadSave(reader, "Latsouth"), taxon.South)
        taxon.VulnerabilityIndex = CInt(Me.ReadSave(reader, "Vul", "0"))
        taxon.OrganismType = eOrganismTypes.Fishes
        taxon.OccurrenceStatus = eOccurrenceStatusTypes.NotSet
        taxon.IUCNConservationStatus = Me.IUCNstatus(Me.ReadSave(reader, "IUCN"))

        Try
            dt = Date.Parse(strDateModified)
        Catch ex As Exception
            dt = Date.Now
        End Try
        taxon.LastUpdated = cDateUtils.DateToJulian()

        Return taxon

    End Function

#End Region ' Internals

End Class
