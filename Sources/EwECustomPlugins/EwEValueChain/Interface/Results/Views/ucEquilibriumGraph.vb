#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing
Imports System.Collections.Generic
Imports ZedGraph
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class ucEquilibriumGraph
    Inherits ZedGraphControl
    Implements IResultView
    Implements IGraphView

    Private m_core As cCore = cCore.GetInstance()
    Private m_zgh As cZedGraphHelper = Nothing
    Private m_aVars() As cResults.eVariableType = Nothing

    Public Sub New()
        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Nothing, Me)
        Me.PrepareGraph()
    End Sub

    Protected Overrides Sub Finalize()
        Me.m_zgh.Detach()
        Me.m_zgh = Nothing
        MyBase.Finalize()
    End Sub

    Public Sub ShowResults(ByVal iFleet As Integer, ByVal lUnits As List(Of cUnit), ByVal result As cResults) _
        Implements IResultView.ShowResults

        Dim cr As New ZedGraph.ColorSymbolRotator()
        Dim line As LineItem = Nothing
        Dim lLines As New List(Of LineItem)
        Dim aobjSnapshots As Object() = result.Snapshots()
        Dim sEffort As Single = 0.0!
        Dim sMin As Single = 0.0!
        Dim sMax As Single = 1.0!

        For Each var As cResults.eVariableType In Me.m_aVars

            line = New LineItem(var.ToString())
            line.Color = cr.NextColor()
            line.Symbol = New ZedGraph.Symbol(SymbolType.None, line.Color)

            For iSnapshot As Integer = 0 To aobjSnapshots.Length - 1
                sEffort = CSng(aobjSnapshots(iSnapshot))
                If iSnapshot = 0 Then
                    sMin = sEffort : sMax = sEffort
                Else
                    sMin = Math.Min(sMin, sEffort)
                    sMax = Math.Max(sMax, sEffort)
                End If
                line.AddPoint(CDbl(sEffort), result.GetSnapshotTotal(var, sEffort, lUnits))
            Next

            lLines.Add(line)

        Next var

        ' Fix scale
        Me.MasterPane.PaneList(0).XAxis.Scale.Min = sMin
        Me.MasterPane.PaneList(0).XAxis.Scale.Max = sMax

        Me.m_zgh.PlotLines(lLines)

    End Sub

    Public Sub SetData(ByVal strGraphTitle As String, _
                       ByVal strXAxisLabel As String, ByVal aUnitsXAxis() As cStyleGuide.eUnitType, _
                       ByVal strYAxisLabel As String, ByVal aUnitsYAxis() As cStyleGuide.eUnitType, _
                       ByVal aVars() As cResults.eVariableType) Implements IGraphView.SetData

        Me.m_zgh.ConfigurePane(strGraphTitle, strXAxisLabel, aUnitsXAxis, strYAxisLabel, aUnitsYAxis, True)
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

        Me.m_zgh.ShowPointValue = True
        Me.m_zgh.AutoscalePane = True

    End Sub

#End Region ' Internals

End Class
