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
' The Cefas MSE plug-in was developed by the Centre for Environment, Fisheries and 
' Aquaculture Science (Cefas). 
'
' EwE copyright: 1991- UBC Fisheries Centre, Vancouver BC, Canada.
' Cefas MSE plug-in copyright: 2013- Cefas, Lowestoft, UK.
' ===============================================================================
'

#Region " Imports "

Option Strict On
Option Explicit On

Imports System.IO
Imports EwECore
Imports EwEUtils.Utilities

#End Region ' Imports 

''' <summary>
''' Class to group a list of Harvest Control Rules into an object
''' </summary>
Public Class Strategy
    Implements IList(Of HCR_Group)
    Implements IMSEData

    Private m_HCRsList As New List(Of HCR_Group)
    Private m_core As cCore = Nothing
    Private m_MSE As cMSE = Nothing
    Private m_EcosimData As cEcosimDatastructures = Nothing
    Public Property Name As String = ""
    Public Property FileName As String = ""

    Public Sub New(ByVal StrategyName As String, StrategyNumber As Integer, ByVal theFilename As String, Core As cCore, MSE As cMSE)
        Me.m_core = Core
        Me.m_MSE = MSE
        Me.m_EcosimData = MSE.EcosimData
        Me.Name = StrategyName
        Me.FileName = theFilename
        Me.Regulations = New cRegulations(MSE, Core)
        Me.StrategyNumber = StrategyNumber
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Resolve a name and index to a <see cref="cEcoPathGroupInput"/> instance.
    ''' </summary>
    ''' <param name="strName">The name to resolve.</param>
    ''' <param name="iIndex">The index to resolve.</param>
    ''' <returns>A <see cref="cEcoPathGroupInput"/> instance, or Nothing if
    ''' the index or name did not match any of the present groups.</returns>
    ''' <remarks>Note that name comparison is not case sensitive.</remarks>
    ''' -----------------------------------------------------------------------
    Private Function ResolveGroup(strName As String, iIndex As Integer) As cEcoPathGroupInput
        If (iIndex < 1) Or (iIndex > Me.m_core.nGroups) Then Return Nothing
        Dim grp As cEcoPathGroupInput = Me.m_core.EcoPathGroupInputs(iIndex)
        Dim grpName As String = cMSEUtils.FromCSVField(strName)
        If String.Compare(grp.Name, grpName, True) <> 0 Then
            Return Nothing
        End If
        Return grp
    End Function

    Public Property StrategyNumber() As Integer
    Public Property Regulations As cRegulations

    Public Overrides Function ToString() As String
        Return MyBase.ToString() & ":" & Me.Name
    End Function

#Region " IList implementation "

    Public Sub Add(item As HCR_Group) Implements ICollection(Of HCR_Group).Add
        If Not Me.Contains(item) Or item.TypeOfHCR = HCRType.Conservation Then
            Me.m_HCRsList.Add(item)
        End If
    End Sub

    Public Sub Clear() Implements ICollection(Of HCR_Group).Clear
        Me.m_HCRsList.Clear()
    End Sub

    Public Function Contains(item As HCR_Group) As Boolean Implements ICollection(Of HCR_Group).Contains
        For Each Rule As HCR_Group In Me
            If Object.ReferenceEquals(item.GroupB, Rule.GroupB) And Object.ReferenceEquals(item.GroupF, Rule.GroupF) Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Sub CopyTo(array() As HCR_Group, arrayIndex As Integer) Implements ICollection(Of HCR_Group).CopyTo
        ' NOP
    End Sub

    Public ReadOnly Property Count As Integer Implements ICollection(Of HCR_Group).Count
        Get
            Return Me.m_HCRsList.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of HCR_Group).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function Remove(item As HCR_Group) As Boolean Implements ICollection(Of HCR_Group).Remove
        Return Me.m_HCRsList.Remove(item)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of HCR_Group) Implements IEnumerable(Of HCR_Group).GetEnumerator
        Return Me.m_HCRsList.GetEnumerator()
    End Function

    Public Function IndexOf(item As HCR_Group) As Integer Implements IList(Of HCR_Group).IndexOf
        Return Me.m_HCRsList.IndexOf(item)
    End Function

    Public Sub Insert(index As Integer, item As HCR_Group) Implements IList(Of HCR_Group).Insert
        Me.m_HCRsList.Insert(index, item)
    End Sub

    Public Function StrategyContainsHCRforiGrp(ByVal iGrp As Integer) As Boolean
        'Checks to see whether this strategy has an HCR for the indexed group
        For Each iHCR In m_HCRsList
            If iHCR.GroupF.Index = iGrp Then
                Return True
            End If
        Next
        Return False
    End Function

    Default Public Property Item(index As Integer) As HCR_Group Implements IList(Of HCR_Group).Item
        Get
            Return Me.m_HCRsList.Item(index)
        End Get
        Set(value As HCR_Group)
            Me.m_HCRsList(index) = value
        End Set
    End Property

    Public Sub RemoveAt(index As Integer) Implements IList(Of HCR_Group).RemoveAt
        Me.m_HCRsList.RemoveAt(index)
    End Sub

    Private Function Bogus() As IEnumerator Implements IEnumerable.GetEnumerator
        ' NOP
        Return Nothing
    End Function

#End Region ' IList implementation

#Region " IMSEData implementation "

    Public Sub Defaults() _
        Implements IMSEData.Defaults
        Me.Clear()
    End Sub

    Public Function IsChanged() As Boolean _
        Implements IMSEData.IsChanged
        ' ToDo: implement this properly
        Return False
    End Function

    Public Function Load(Optional msg As cMessage = Nothing, _
                         Optional strFilename As String = "") As Boolean _
        Implements IMSEData.Load

        Dim strMsg As String = ""
        Dim buff As String
        Dim recs() As String
        Dim bSuccess As Boolean = True

        If (String.IsNullOrWhiteSpace(strFilename)) Then
            strFilename = Me.FileName
        End If

        If Not File.Exists(strFilename) Then
            ' OK: data loaded with defaults
            Return bSuccess
        End If

        Dim reader As StreamReader = cMSEUtils.GetReader(strFilename)
        If (reader IsNot Nothing) Then

            reader.ReadLine()
            Do Until reader.EndOfStream
                buff = reader.ReadLine()
                recs = buff.Split(","c)

                Dim tempHCRGroup As HCR_Group
                'Each HCR Group needs to be a new object
                tempHCRGroup = New HCR_Group(m_core, m_MSE)

                Try
                    ' Resolve group
                    tempHCRGroup.GroupB = Me.ResolveGroup(recs(0), cStringUtils.ConvertToInteger(recs(1)))
                    tempHCRGroup.LowerLimit = cStringUtils.ConvertToSingle(recs(2))
                    tempHCRGroup.UpperLimit = cStringUtils.ConvertToSingle(recs(3))
                    tempHCRGroup.GroupF = Me.ResolveGroup(recs(4), cStringUtils.ConvertToInteger(recs(5)))
                    tempHCRGroup.MaxF = cStringUtils.ConvertToSingle(recs(6))
                    If Not [Enum].TryParse(recs(7), tempHCRGroup.TypeOfHCR) Then
                        tempHCRGroup.TypeOfHCR = CType(CInt(recs(7)), HCRType)
                    End If
                    'backwards compatability with older file that don't contain Time Frame Rules
                    If recs.Length > 8 Then
                        tempHCRGroup.TimeFrameRule.NYears = cStringUtils.ConvertToInteger(recs(8))
                    Else
                        tempHCRGroup.TimeFrameRule.NYears = 0
                    End If
                Catch ex As Exception
                    ' Whoah!
                    cMSEUtils.LogError(msg, "Strategy could not load from " & strFilename & ". " & ex.Message)
                    bSuccess = False
                End Try

                ' Only add valid strategies!
                If tempHCRGroup.isValid(strMsg) Then
                    Me.Add(tempHCRGroup)
                Else
                    cMSEUtils.LogError(msg, "Strategy loaded from " & strFilename & " is not valid.")
                    bSuccess = False
                End If

            Loop
        End If 'cMSEUtils.readToTag(reader, START_TAG)

        cMSEUtils.ReleaseReader(reader)

        'for debugging
        Debug.Assert(bSuccess, Me.ToString + ".Read() Failed to read strategies from file.")

        Return bSuccess
    End Function

    Public Function Save(Optional strFilename As String = "") As Boolean _
        Implements IMSEData.Save

        If (String.IsNullOrWhiteSpace(strFilename)) Then
            strFilename = Me.FileName
        End If

        Dim strm As StreamWriter = cMSEUtils.GetWriter(strFilename, False)
        If (strm IsNot Nothing) Then

            'msg.AddVariable(New cVariableStatus(eStatusFlags.OK, _
            '                                    String.Format(My.Resources.STATUS_SAVED_DETAIL, Path.GetFileName(iStrategy.FileName)), _
            '                                    eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))
            strm.WriteLine("GroupNameForBiomass,GroupNumberForBiomass,LowerLimit,UpperLimit,GroupNameForF,GroupNumberForF,MaxF,CostFunctionType, NYears(Time Frame Rule)")
            For Each iHCR In Me
                strm.WriteLine(cStringUtils.ToCSVField(iHCR.GroupB.Name) & "," & _
                                          cStringUtils.ToCSVField(iHCR.GroupB.Index) & "," & _
                                          cStringUtils.ToCSVField(iHCR.LowerLimit) & "," & _
                                          cStringUtils.ToCSVField(iHCR.UpperLimit) & "," & _
                                          cStringUtils.ToCSVField(iHCR.GroupF.Name) & "," & _
                                          cStringUtils.ToCSVField(iHCR.GroupF.Index) & "," & _
                                          cStringUtils.ToCSVField(iHCR.MaxF) & "," & _
                                          cStringUtils.ToCSVField(iHCR.TypeOfHCR) & "," & _
                                          cStringUtils.ToCSVField(iHCR.TimeFrameRule.NYears))
            Next
            cMSEUtils.ReleaseWriter(strm)
        End If

        Return True
    End Function

#End Region ' IMSEData implementation

    Public Function FileExists(Optional strFilename As String = "") As Boolean Implements IMSEData.FileExists
        Return FileExists(strFilename)
    End Function
End Class
