' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

''' ---------------------------------------------------------------------------
''' <summary>
''' Data structures for the Pre-bal interfaces.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class cPrebalModel

#Region " Internal helper classes "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class for sorting <see cref="cEcopathGroupOutput">groups</see> by 
    ''' <see cref="cEcopathGroupOutput.TTLX">Trophic Level</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Class cGroupTLComparer
        Implements IComparer(Of cEcopathGroupOutput)

        Public Property Descending As Boolean = True

        Public Function Compare(x As cEcopathGroupOutput, y As cEcopathGroupOutput) As Integer _
            Implements IComparer(Of cEcopathGroupOutput).Compare

            If (x Is Nothing) Or (y Is Nothing) Then Return 1

            Dim tlx As Single = x.TTLX
            Dim tly As Single = y.TTLX
            Dim iFact As Integer

            If (Me.Descending) Then iFact = -1 Else iFact = 1

            If (tlx = tly) Then Return 0
            If (tlx > tly) Then Return iFact
            Return -iFact

        End Function

    End Class

#End Region ' Internal helper classes

#Region " Private variables "

    Private m_core As cCore = Nothing
    Private m_data() As cPrebalPlotData = Nothing

#End Region ' Private variables

#Region " Construction "

    Public Sub New(core As cCore)
        Me.m_core = core

        ReDim Me.m_data(4)
        Me.m_data(eResultTypes.B) = New cPrebalPlotData(eResultTypes.B)
        Me.m_data(eResultTypes.PB) = New cPrebalPlotData(eResultTypes.PB)
        Me.m_data(eResultTypes.QB) = New cPrebalPlotData(eResultTypes.QB)
        Me.m_data(eResultTypes.PQ) = New cPrebalPlotData(eResultTypes.PQ)

    End Sub

#End Region ' Construction

#Region " Event "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Event that notifies that the pre-bal data has been <see cref="cPrebalModel.Update">updated</see>.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    ''' -----------------------------------------------------------------------
    Public Event OnUpdated(sender As Object, args As EventArgs)

#End Region ' Event

#Region " Public interfaces "

    Public Enum eResultTypes As Integer
        NotSet = 0
        B
        PB
        QB
        PQ
    End Enum

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the data for the pre-bal diagnostics. Call this after Ecopath has ran.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Sub Update()
        Try
            Me.DoUpdate()
            RaiseEvent OnUpdated(Me, New EventArgs())
        Catch ex As Exception

        End Try
    End Sub

    Public Function Data(plot As eResultTypes) As cPrebalPlotData
        Try
            Return Me.m_data(plot)
        Catch ex As Exception
            Debug.Assert(False)
        End Try
        Return Nothing
    End Function

#End Region ' Public interfaces

#Region " Internals "

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Update the Pre-bal data cache from EwE model results.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Private Sub DoUpdate()

        Dim comp As New cGroupTLComparer()
        Dim grp As cEcopathGroupOutput = Nothing
        Dim lgrps As New List(Of cEcopathGroupOutput)
        Dim nConsumers As Integer = 0
        Dim iConsumer As Integer = 1

        ' Get all living groups
        For i As Integer = 1 To Me.m_core.nLivingGroups
            grp = Me.m_core.EcopathGroupOutputs(i)
            lgrps.Add(grp)
            If (grp.IsConsumer) Then nConsumers += 1
        Next

        ' Sort the groups by trophic level, descending
        comp.Descending = True
        lgrps.Sort(comp)

        ' Allocate memory
        Me.m_data(eResultTypes.B).Resize(lgrps.Count)
        Me.m_data(eResultTypes.PB).Resize(lgrps.Count)
        Me.m_data(eResultTypes.QB).Resize(nConsumers)
        Me.m_data(eResultTypes.PQ).Resize(nConsumers)

        ' ToDo: take stanza-ness into account: find proper values, and set proper statuses?

        ' Update data
        For i As Integer = 1 To lgrps.Count

            ' Get real EwE group
            grp = lgrps(i - 1)

            Me.m_data(eResultTypes.B).EcopathGroupIndexes(i) = grp.Index
            Me.m_data(eResultTypes.B).Data(i) = grp.Biomass
            Me.m_data(eResultTypes.B).Status(i) = grp.BiomassStatus

            Me.m_data(eResultTypes.PB).EcopathGroupIndexes(i) = grp.Index
            Me.m_data(eResultTypes.PB).Data(i) = grp.PBOutput
            Me.m_data(eResultTypes.PB).Status(i) = grp.PBStatus

            If (grp.IsConsumer) Then

                Me.m_data(eResultTypes.QB).EcopathGroupIndexes(iConsumer) = grp.Index
                Me.m_data(eResultTypes.QB).Data(iConsumer) = grp.QBOutput
                Me.m_data(eResultTypes.QB).Status(iConsumer) = grp.QBStatus

                Me.m_data(eResultTypes.PQ).EcopathGroupIndexes(iConsumer) = grp.Index
                Me.m_data(eResultTypes.PQ).Data(iConsumer) = grp.PBOutput / grp.QBOutput
                Me.m_data(eResultTypes.PQ).Status(iConsumer) = grp.PBStatus Or grp.QBStatus

                iConsumer += 1

            End If

        Next

        ' Check orders of magnitude
        ' Check slope
        ' Check above slope, below slope

    End Sub

#End Region ' Internals

End Class
