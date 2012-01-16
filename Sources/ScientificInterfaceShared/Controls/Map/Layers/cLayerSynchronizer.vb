#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Controls.Map.Layers

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Helper class, synchronizes group, fleet and month selections between
    ''' an array of <see cref="cLayer">UI map layers</see>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class cLayerSynchronizer

        Private m_bInSync As Boolean = False

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Apply the same group, fleet and month selection in a given <paramref name="l">layer</paramref>
        ''' to an collection of layers.
        ''' </summary>
        ''' <param name="l">The layer to update group, fleet and month selection from.</param>
        ''' <param name="layers">The collection of layers to apply the selection to.</param>
        ''' -------------------------------------------------------------------
        Public Sub Synchronize(l As cLayer, layers As ICollection(Of cLayer))

            If (Me.m_bInSync) Then Return

            Me.m_bInSync = True

            ' ToDo: rework this!

            'If TypeOf l.Data Is ICoreFleetFilter Then
            '    Dim iFleet As Integer = DirectCast(l.Data, ICoreFleetFilter).Fleet
            '    For Each lTest As cLayer In layers
            '        If Not Object.ReferenceEquals(l, lTest) And TypeOf lTest.Data Is ICoreFleetFilter Then
            '            DirectCast(lTest.Data, ICoreFleetFilter).Fleet = iFleet
            '        End If
            '    Next
            'ElseIf TypeOf l Is ICoreGroupFilter Then
            '    Dim iGroup As Integer = DirectCast(l.Data, ICoreGroupFilter).Group
            '    For Each lTest As cLayer In layers
            '        If Not Object.ReferenceEquals(l, lTest) And TypeOf lTest.Data Is ICoreGroupFilter Then
            '            DirectCast(lTest.Data, ICoreGroupFilter).Group = iGroup
            '        End If
            '    Next
            'ElseIf TypeOf l Is ICoreMonthFilter Then
            '    Dim iMonth As Integer = DirectCast(l.Data, ICoreMonthFilter).Month
            '    For Each lTest As cLayer In layers
            '        If Not Object.ReferenceEquals(l, lTest) And TypeOf lTest.Data Is ICoreMonthFilter Then
            '            DirectCast(lTest.Data, ICoreMonthFilter).Month = iMonth
            '        End If
            '    Next
            'End If

            Me.m_bInSync = False

        End Sub

    End Class

End Namespace
