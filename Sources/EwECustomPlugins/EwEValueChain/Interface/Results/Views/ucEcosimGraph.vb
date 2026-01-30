' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore
Imports ScientificInterfaceShared.Controls
Imports ZedGraph

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================

Public Class ucEcosimGraph
    Inherits ZedGraphControl
    Implements IResultView
    Implements IGraphView

    Private m_zgh As cZedGraphHelper = Nothing
    Private m_data As cData = Nothing
    Private m_aVars() As cResults.eVariableType = Nothing

    Public Sub New(data As cData, uic As cUIContext)
        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(uic, Me)
        Me.m_zgh.ShowPointValue = True
        Me.m_data = data
        Me.PrepareGraph()
    End Sub

    Protected Overrides Sub Finalize()
        Me.m_zgh.Detach()
        Me.m_zgh = Nothing
        MyBase.Finalize()
    End Sub

    Public Sub ShowResults(iFleet As Integer, lUnits As cUnit(), result As cResults,
                           iTimeStep As Integer) _
             Implements IResultView.ShowResults

        Dim cr As New ZedGraph.ColorSymbolRotator()
        Dim lLines As New List(Of LineItem)
        Dim line As LineItem = Nothing
        Dim iBaseYear As Integer = 0

        iBaseYear = Me.m_data.Core.EcosimFirstYear

        For Each vn As cResults.eVariableType In Me.m_aVars

            line = New LineItem(vn.ToString())
            line.Color = cr.NextColor()
            line.Symbol = New ZedGraph.Symbol(SymbolType.None, line.Color)

            For iTimeStep = 1 To result.NumTimeSteps
                line.AddPoint(CDbl(iBaseYear + ((iTimeStep - 1) / cCore.N_MONTHS)),
                              result.GetTimeStepTotal(vn, iTimeStep, lUnits, iFleet, cResults.GetVariableContributionType(vn)))
            Next iTimeStep

            lLines.Add(line)

        Next vn

        ' Fix scale
        If result.NumTimeSteps > 1 Then

            Me.MasterPane.PaneList(0).XAxis.Scale.Min = iBaseYear
            Me.MasterPane.PaneList(0).XAxis.Scale.Max = iBaseYear + (result.NumTimeSteps / cCore.N_MONTHS)

        End If

        Me.m_zgh.PlotLines(lLines.ToArray)

    End Sub

    Public Sub SetData(strGraphTitle As String,
                       strXAxisLabel As String,
                       strYAxisLabel As String,
                       aVars() As cResults.eVariableType) Implements IGraphView.SetData

        Me.m_zgh.ConfigurePane(strGraphTitle, strXAxisLabel, strYAxisLabel, True)
        Me.m_aVars = aVars

    End Sub

#Region " Internals "

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'ucGraph
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.Name = "ucGraph"
        Me.Size = New System.Drawing.Size(485, 268)
        Me.ResumeLayout(False)

    End Sub

    Private Sub PrepareGraph()

        Me.m_zgh.AutoscalePane() = True

    End Sub

#End Region ' Internals

End Class
