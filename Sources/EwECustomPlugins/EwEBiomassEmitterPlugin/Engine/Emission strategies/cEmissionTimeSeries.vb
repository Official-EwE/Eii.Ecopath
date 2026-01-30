' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore

Public Class cEmissionTimeSeries
    Inherits cEmission

    Public Sub New(data As cData, iGroup As Integer, iTarget As Integer)
        MyBase.New(data)
        Me.Target = iTarget
        Me.Group = iGroup
    End Sub

    Public ReadOnly Property Target As Integer = 0
    Public ReadOnly Property Group As Integer = 0
    Private ReadOnly Property DataPoints As New Dictionary(Of Date, Single)

    Public Property Datapoint(t As Date) As Single
        Get
            If Not Me.DataPoints.ContainsKey(t) Then Return cCore.NULL_VALUE
            Return Me.DataPoints(t)
        End Get
        Set(value As Single)
            If (value > 0) Then
                Me.DataPoints(t) = value
            End If
        End Set
    End Property

    Public Overrides Property Enable As Boolean = True

    Public Overrides Function IsValid() As Boolean
        Dim core As cCore = Me.Data.Core
        Dim bValid As Boolean = ((Me.Group >= 1) And (Me.Group <= core.nGroups))
        Select Case Me.Data.TargetType
            Case eTargetType.Region
                bValid = bValid And (Me.Target >= 1) And (Me.Target <= core.nRegions)
            Case eTargetType.MPA
                bValid = bValid And (Me.Target >= 1) And (Me.Target <= core.nMPAs)
            Case eTargetType.Habitat
                bValid = bValid And (Me.Target >= 1) And (Me.Target <= core.nHabitats)
            Case Else
                Debug.Assert(False)
        End Select
        Return bValid
    End Function

    Public Function NumDataPointsForRun() As Integer
        ' No need to dig further if invalid
        If (Not Me.Enable) Then Return 0

        Dim core As cCore = Me.Data.Core
        Dim parms As cEcospaceModelParameters = core.EcospaceModelParameters

        Dim ys As Integer = core.EcosimFirstYear()
        Dim ye As Integer = CInt(Math.Floor(parms.TotalTime * parms.NumberOfTimeStepsPerYear)) + ys
        Dim n As Integer = 0

        For Each dt As Date In Me.DataPoints.Keys
            If (dt.Year >= ys) And (dt.Year < ye) Then
                n += 1
            End If
        Next
        Return n

    End Function

    Public Function ForcingValue(t As Date, group As Integer) As Single
        If (group <> Me.Group) Then Return cCore.NULL_VALUE
        Return Me.Datapoint(t)
    End Function

    Public Overrides Function ToString() As String
        If Not Me.IsValid Then
            Return "(invalid group or target)"
        Else
            Return (Me.NumDataPointsForRun() & " datapoints for run")
        End If
    End Function

End Class
