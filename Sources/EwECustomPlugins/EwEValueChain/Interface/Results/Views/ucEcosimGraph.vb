#Region " Imports "

Option Strict On
Imports EwECore
Imports System.Drawing
Imports System.Collections.Generic
Imports ZedGraph
Imports ScientificInterfaceShared.Style
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Properties
Imports EwEUtils.Commands

#End Region ' Imports

''' ===========================================================================
''' <summary>
''' 
''' </summary>
''' ===========================================================================
<CLSCompliant(False)> _
Public Class ucEcosimGraph
    Inherits ZedGraphControl
    Implements IResultView
    Implements IGraphView

    Private m_uic As cUIContext = New cUIContext(cCore.GetInstance(), _
                                                 cStyleGuide.GetInstance(), _
                                                 cPropertyManager.getinstance(), _
                                                 cCommandHandler.GetInstance())
    Private m_zgh As cZedGraphHelper = Nothing
    Private m_data As cData = Nothing
    Private m_aVars() As cResults.eVariableType = Nothing

    Public Sub New(ByVal data As cData)

        Me.m_zgh = New cZedGraphHelper()
        Me.m_zgh.Attach(Me.m_uic, Me)
        Me.m_data = data
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
        Dim lLines As New List(Of LineItem)
        Dim line As LineItem = Nothing
        Dim iBaseYear As Integer = 0

        iBaseYear = Me.m_data.Core.EcosimFirstYear

        For Each var As cResults.eVariableType In Me.m_aVars

            line = New LineItem(var.ToString())
            line.Color = cr.NextColor()
            line.Symbol = New ZedGraph.Symbol(SymbolType.None, line.Color)

            For iTimeStep As Integer = 1 To result.NumTimeSteps
                line.AddPoint(CDbl(iBaseYear + ((iTimeStep - 1) / cCore.N_MONTHS)), _
                              result.GetTimeStepTotal(var, iTimeStep, lUnits, iFleet))
            Next iTimeStep

            lLines.Add(line)

        Next var

        ' Fix scale
        If result.NumTimeSteps > 1 Then

            Me.MasterPane.PaneList(0).XAxis.Scale.Min = iBaseYear
            Me.MasterPane.PaneList(0).XAxis.Scale.Max = iBaseYear + (result.NumTimeSteps / cCore.N_MONTHS)

        End If

        Me.m_zgh.PlotLines(lLines.ToArray)

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

        Me.m_zgh.AutoscalePane() = True

    End Sub

#End Region ' Internals

End Class
