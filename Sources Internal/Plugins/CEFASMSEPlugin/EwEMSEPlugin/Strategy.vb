Option Strict On
Option Explicit On

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

Imports EwECore
Imports LumenWorks.Framework.IO.Csv
Imports System.IO
Imports EwEUtils.Utilities

#End Region ' Imports 

''' <summary>
''' Class to group a list of Harvest Control Rules into an object
''' </summary>
Public Class Strategy
    Implements IList(Of HCR_Group)

    Public Const START_TAG As String = "<STRATEGY_START>"
    Public Const END_TAG As String = "<STRATEGY_END>"

    Private mHCRsList As New List(Of HCR_Group)
    Private mRegulateMethods As cRegulations
    Private mStrategyNumber As Integer

    Private mCore As cCore

    Public Property Name As String
    Public Property FileName As String

    'Public Sub New()
    '    ' Hm 
    'End Sub

    Public Sub New(ByVal StrategyName As String, StrategyNumber As Integer, ByVal theFilename As String, Core As cCore, MSE As cMSE)
        'Me.New()

        Me.mCore = Core
        Me.Name = StrategyName
        Me.FileName = theFilename
        mRegulateMethods = New cRegulations(MSE, Core)
        mStrategyNumber = StrategyNumber

    End Sub

    Public Function Read() As Boolean
        Dim buff As String
        Dim recs() As String
        Dim breturn As Boolean = False

        If Not File.Exists(Me.FileName) Then
            'message of some sort
            Return False
        End If

        Try

            Dim reader As StreamReader = cMSEUtils.GetReader(Me.FileName)
            If (reader IsNot Nothing) Then
                '  csvHCR = New CsvReader(reader, False)

                If cMSEUtils.readToTag(reader, START_TAG) Then
                    reader.ReadLine()
                    Do Until reader.EndOfStream
                        buff = reader.ReadLine()
                        recs = buff.Split(","c)

                        'Read up to the END_TAG
                        If Not recs(0).Contains(END_TAG) Then
                            Dim tempHCRGroup As HCR_Group
                            'Each HCR Group needs to be a new object
                            tempHCRGroup = New HCR_Group(mCore)

                            ' Resolve group
                            tempHCRGroup.GroupB = Me.ResolveGroup(recs(0), cStringUtils.ConvertToInteger(recs(1)))
                            tempHCRGroup.LowerLimit = cStringUtils.ConvertToDouble(recs(2))
                            tempHCRGroup.UpperLimit = cStringUtils.ConvertToDouble(recs(3))
                            tempHCRGroup.GroupF = Me.ResolveGroup(recs(4), cStringUtils.ConvertToInteger(recs(5)))
                            tempHCRGroup.MaxF = cStringUtils.ConvertToDouble(recs(6))
                            ' tempHCRGroup.CostFunction = HCR_Group.toCostFunctionEnum(csv(7))

                            Dim strMsg As String = ""
                            ' Only add valid strategies!
                            If tempHCRGroup.isValid(strMsg) Then
                                Me.Add(tempHCRGroup)
                            End If

                            breturn = True
                        Else 'recs(0).Contains(END_TAG)
                            'Reached the END_TAG in the file
                            'Bump out of the reading loop
                            Exit Do
                        End If

                    Loop
                End If 'cMSEUtils.readToTag(reader, START_TAG)

                cMSEUtils.ReleaseReader(reader)

            End If 'reader IsNot Nothing

        Catch ex As Exception
            System.Console.WriteLine(Me.ToString + ".Read() Exception: " + ex.Message)
        End Try

        'for debugging
        Debug.Assert(breturn, Me.ToString + ".Read() Failed to read strategies from file.")

        Return breturn
    End Function

    Public Function Save() As Boolean
        Dim strm As StreamWriter
        'Create a new file
        strm = cMSEUtils.GetWriter(Me.FileName, False)
        If (strm IsNot Nothing) Then

            'msg.AddVariable(New cVariableStatus(eStatusFlags.OK, _
            '                                    String.Format(My.Resources.STATUS_SAVED_DETAIL, Path.GetFileName(iStrategy.FileName)), _
            '                                    eVarNameFlags.NotSet, eDataTypes.NotSet, eCoreComponentType.External, 0))
            strm.WriteLine(START_TAG)
            strm.WriteLine("GroupNameForBiomass,GroupNumberForBiomass,LowerLimit,UpperLimit,GroupNameForF,GroupNumberForF,MaxF,CostFunctionType")
            For Each iHCR In Me
                strm.WriteLine(cStringUtils.ToCSVField(iHCR.GroupB.Name) & "," & _
                                          cStringUtils.ToCSVField(iHCR.GroupB.Index) & "," & _
                                          cStringUtils.ToCSVField(iHCR.LowerLimit) & "," & _
                                          cStringUtils.ToCSVField(iHCR.UpperLimit) & "," & _
                                          cStringUtils.ToCSVField(iHCR.GroupF.Name) & "," & _
                                          cStringUtils.ToCSVField(iHCR.GroupF.Index) & "," & _
                                          cStringUtils.ToCSVField(iHCR.MaxF) & "," & _
                                          cStringUtils.ToCSVField(iHCR.TypeOfHCR))
            Next
            strm.WriteLine(END_TAG)
            cMSEUtils.ReleaseWriter(strm)
        End If

        Return True

    End Function

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
        If (iIndex < 1) Or (iIndex > Me.mCore.nGroups) Then Return Nothing
        Dim grp As cEcoPathGroupInput = Me.mCore.EcoPathGroupInputs(iIndex)
        Dim grpName As String = cMSEUtils.FromCSVField(strName)
        If String.Compare(grp.Name, grpName, True) <> 0 Then
            Return Nothing
        End If
        Return grp
    End Function

    Public ReadOnly Property Regulations As cRegulations
        Get
            Return mRegulateMethods
        End Get
    End Property

    Public Property StrategyNumber() As Integer
        Get
            Return mStrategyNumber
        End Get
        Set(ByVal value As Integer)
            mStrategyNumber = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Return MyBase.ToString() & ":" & Me.Name
    End Function

    'Public Function LoadRegulations() As Boolean
    '    Return mRegulateMethods.LoadRegsFromCSV(mStrategyNumber)
    'End Function

    Public Sub Add(item As HCR_Group) Implements ICollection(Of HCR_Group).Add
        If Not Me.Contains(item) Then
            Me.mHCRsList.Add(item)
        End If
    End Sub

    Public Sub Clear() Implements ICollection(Of HCR_Group).Clear
        Me.mHCRsList.Clear()
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

    Public Property RegMethods As cRegulations
        Get
            Return mRegulateMethods
        End Get
        Set(value As cRegulations)
            mRegulateMethods = value
        End Set
    End Property

    Public ReadOnly Property Count As Integer Implements ICollection(Of HCR_Group).Count
        Get
            Return Me.mHCRsList.Count
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean Implements ICollection(Of HCR_Group).IsReadOnly
        Get
            Return False
        End Get
    End Property

    Public Function Remove(item As HCR_Group) As Boolean Implements ICollection(Of HCR_Group).Remove
        Return Me.mHCRsList.Remove(item)
    End Function

    Public Function GetEnumerator() As IEnumerator(Of HCR_Group) Implements IEnumerable(Of HCR_Group).GetEnumerator
        Return Me.mHCRsList.GetEnumerator()
    End Function

    Public Function IndexOf(item As HCR_Group) As Integer Implements IList(Of HCR_Group).IndexOf
        Return Me.mHCRsList.IndexOf(item)
    End Function

    Public Sub Insert(index As Integer, item As HCR_Group) Implements IList(Of HCR_Group).Insert
        Me.mHCRsList.Insert(index, item)
    End Sub

    Default Public Property Item(index As Integer) As HCR_Group Implements IList(Of HCR_Group).Item
        Get
            Return Me.mHCRsList.Item(index)
        End Get
        Set(value As HCR_Group)
            Me.mHCRsList(index) = value
        End Set
    End Property

    Public Sub RemoveAt(index As Integer) Implements IList(Of HCR_Group).RemoveAt
        Me.mHCRsList.RemoveAt(index)
    End Sub

    Private Function Bogus() As IEnumerator Implements IEnumerable.GetEnumerator
        ' NOP
        Return Nothing
    End Function

End Class
