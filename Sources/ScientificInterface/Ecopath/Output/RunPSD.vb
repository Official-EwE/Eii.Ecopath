' =============================================================================
'
' $Log: RunPSD.vb,v $
' Revision 1.13  2009/03/23 20:45:49  joeh
' Add functionality to the Run button
'
' Revision 1.12  2009/03/21 00:31:19  jeroens
' PSD params exposes nWeightClasses
'
' Revision 1.11  2009/03/20 18:01:02  joeh
' Remove some redundant Imports statements
'
' Revision 1.10  2009/03/19 01:14:04  joeh
' no message
'
' Revision 1.9  2009/03/18 13:32:05  jeroens
' Uses implemented PSD classes
'
' Revision 1.8  2009/03/17 23:37:34  joeh
' Add codes for the Selected Group feature
'
' Revision 1.7  2009/03/17 19:38:08  joeh
' Add latitudes of NW and SE corners of model
'
' Revision 1.6  2009/03/14 18:34:07  joeh
' Change dXValue of double type to sXValue of single type
' Add linear regression of the system PSD
'
' Revision 1.5  2009/03/12 23:51:06  joeh
' Add codes for tabulation of PSD contribution data
'
' Revision 1.4  2009/03/12 01:50:29  joeh
' Add codes for PSD histogram (PSDContributionPlot)
'
' Revision 1.3  2009/03/11 00:14:29  joeh
' Add PSD calculation
'
' Revision 1.2  2009/02/21 00:24:14  jeroens
' Added headers
'
' =============================================================================

#Region " Imports "

Option Explicit On
Option Strict On

Imports EwECore
Imports EwEUtils.Core
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Properties
Imports ZedGraph

#End Region 'Imports

Namespace Ecopath.Output

    Public Class RunPSD

#Region "Variables"

        Private m_core As cCore = Nothing
        Private m_zgh As ZedGraphHelper = Nothing
        Private m_fpLorenzenLatNWCorner As cEwEFormatProvider = Nothing
        Private m_fpLorenzenLatSECorner As cEwEFormatProvider = Nothing
        Private m_fpNoOfPointsPSD As cEwEFormatProvider = Nothing
        Private m_fpMinWeight As cEwEFormatProvider = Nothing

#End Region 'Variables

#Region "Constructor/Destructor"

        Public Sub New()
            InitializeComponent()
            Me.m_core = cCore.GetInstance()
            Me.m_zgh = New ZedGraphHelper(Me.zgcZedGraphCntl)
        End Sub

#End Region 'Constructor/Destructor

#Region "Event handlers"

        Private Sub RunPSD_Load(ByVal sender As Object, ByVal e As System.EventArgs) _
            Handles Me.Load

            Me.UpdateToolStrip()
            Me.PlotCurve()

            'AddCurves(CreatePane(My.Resources.PSD_PLOTCAPTION_PSD, My.Resources.PSD_XAXISLABEL_WEIGHTCLASS, _
            '                     My.Resources.PSD_YAXISLABEL_BIOMASS))
            'UpdatePlot()
        End Sub

        Private Sub RunPSD_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) _
            Handles Me.FormClosing

            Me.m_fpLorenzenLatNWCorner.Release()
            Me.m_fpLorenzenLatSECorner.Release()
            Me.m_fpNoOfPointsPSD.Release()
            Me.m_fpMinWeight.Release()

            ' Show/Hide Groups
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = cmdh.GetCommand("DisplayGroups")
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.RemoveControl(Me.btnShowHideGroups)
            End If

        End Sub

        Private Sub mnuItmGroupPB_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuItmGroupPB.CheckedChanged
            mnuItmLorenzen.Checked = Not mnuItmGroupPB.Checked
        End Sub

        Private Sub mnuItmLorenzen_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles mnuItmLorenzen.CheckedChanged
            mnuItmGroupPB.Checked = Not mnuItmLorenzen.Checked
        End Sub

        Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click
            UpdateVariables()
            m_core.RunEcoPath()
            PlotCurve()
        End Sub

#End Region 'Event handlers

#Region "Helper methods"
        Private Function CreatePane(ByVal strTitle As String, ByVal strXAxisTitle As String, _
                                    ByVal strYAxisTitle As String) As GraphPane
            Dim pane As GraphPane = Me.zgcZedGraphCntl.GraphPane

            InitGraphPane(strTitle, strXAxisTitle, strYAxisTitle, pane)
            Return pane
        End Function

        Private Sub InitGraphPane(ByVal strTitle As String, ByVal strXAxisTitle As String, _
                                    ByVal strYAxisTitle As String, ByVal pane As GraphPane)

            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters

            pane.Title.Text = strTitle
            pane.Title.FontSpec.IsBold = False
            pane.Title.FontSpec.Size = 16

            pane.XAxis.Scale.IsVisible = True 'False
            pane.XAxis.Title.Text = strXAxisTitle
            pane.XAxis.Title.FontSpec.Size = 14

            pane.YAxis.Scale.IsVisible = True 'False
            pane.YAxis.Title.Text = strYAxisTitle
            pane.YAxis.Title.FontSpec.Size = 14

            pane.XAxis.Scale.Min = Math.Log10(parms.FirstWeightClass)
            pane.XAxis.Scale.Max = Math.Log10(parms.FirstWeightClass * 2 ^ (m_core.nWeightClasses - 1))
            pane.YAxis.Scale.Min = 0

            pane.YAxis.MinorTic.IsAllTics = False
            pane.XAxis.MinorTic.IsAllTics = False

            'Me.UpdateColors()
        End Sub

        Private Sub AddCurves(ByVal pane As GraphPane)

            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim resultLists As New List(Of PointPairList)
            Dim sXValue As Single = 0
            Dim sSystemPSD(m_core.nWeightClasses) As Single
            Dim sSlope As Single
            Dim sIntercept As Single

            InitLists(resultLists, 2)

            'Find system PSD by summing the group PSD
            FindSystemPSD(sSystemPSD)

            'Find regression of the system PSD
            FindRegression(sSlope, sIntercept, sSystemPSD)

            For iWtClass As Integer = 1 To m_core.nWeightClasses
                If sSystemPSD(iWtClass) * 100000 > 0 Then
                    sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))

                    'PSD plot
                    resultLists(0).Add(Math.Log10(sXValue), Math.Log10(sSystemPSD(iWtClass) * 100000)) '* 100000 for plotting purpose
                    'PSD regression plot
                    resultLists(1).Add(Math.Log10(sXValue), sSlope * Math.Log10(sXValue) + sIntercept)
                End If
            Next

            ' Clear pane
            pane.CurveList.Clear()

            AddCurveToGraphPane(pane, resultLists(0), "", Color.Transparent)
            AddCurveToGraphPane(pane, resultLists(1), "Slope = " & sSlope.ToString("F4") & _
                                " Intercept = " & sIntercept.ToString("F4"), Color.Black)
        End Sub

        Private Sub InitLists(ByRef lists As List(Of PointPairList), ByVal size As Integer)
            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next
        End Sub

        Private Sub AddCurveToGraphPane(ByVal pane As GraphPane, ByVal list As PointPairList, _
                                        ByVal strLabel As String, ByVal lineClr As Color)
            Dim lnItem As LineItem

            lnItem = pane.AddCurve(strLabel, list, lineClr)

            If lineClr = Color.Transparent Then
                lnItem.Line.IsVisible = False
                lnItem.Symbol.Type = SymbolType.Circle
                lnItem.Symbol.Border.IsVisible = False
                lnItem.Symbol.Fill.IsVisible = True
                lnItem.Symbol.Fill.Brush = Brushes.Black
            Else
                lnItem.Line.IsVisible = True
                lnItem.Symbol.Type = SymbolType.None
            End If

        End Sub

        Private Sub UpdateToolStrip()
            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim grpInput As cEcoPathGroupInput = Nothing
            Dim sgStyleGuide As StyleGuide = StyleGuide.GetInstance
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmd As Command = Nothing

            'Me.m_fpNoOfPointsPSD = New cPropertyFormatProvider(Me.tbxNoOfPointsPSD.Control, _
            '        Me.m_core.ParticleSizeDistributionParameters, eVarNameFlags.PSDNumWeightClasses)

            'Mortality type
            If parms.MortalityType = EwEUtils.Core.ePSDMortalityTypes.GroupZ Then
                mnuItmGroupPB.Checked = True
            End If
            If parms.MortalityType = ePSDMortalityTypes.Lorenzen Then
                mnuItmLorenzen.Checked = True
                Me.m_fpLorenzenLatNWCorner = New cEwEFormatProvider(Me.tbxNWLat.Control, GetType(Single))
                Me.m_fpLorenzenLatNWCorner.Value = parms.LohrenzenLatNWCorner
                Me.m_fpLorenzenLatSECorner = New cEwEFormatProvider(Me.tbxSELat.Control, GetType(Single))
                Me.m_fpLorenzenLatSECorner.Value = parms.LohrenzenLatSECorner
            End If

            'Group included in PSD
            cmd = cmdh.GetCommand("DisplayGroups")
            If Not Object.ReferenceEquals(cmd, Nothing) Then
                cmd.AddControl(Me.btnShowHideGroups)
            End If
            For iGroup As Integer = 1 To m_core.nLivingGroups
                grpInput = m_core.EcoPathGroupInputs(iGroup)
                sgStyleGuide.GroupVisible(iGroup) = grpInput.PSDIncluded
            Next

            'Number of weight class
            Me.m_fpNoOfPointsPSD = New cEwEFormatProvider(Me.tbxNoOfPointsPSD.Control, GetType(Integer))
            Me.m_fpNoOfPointsPSD.Value = parms.NumWeightClasses

            'First weight class
            Me.m_fpMinWeight = New cEwEFormatProvider(Me.tbxMinWeight.Control, GetType(Single))
            Me.m_fpMinWeight.Value = parms.FirstWeightClass
        End Sub

        Private Sub UpdateVariables()
            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim sgStyleGuide As StyleGuide = StyleGuide.GetInstance
            Dim grpInput As cEcoPathGroupInput = Nothing

            'Mortality type
            If mnuItmGroupPB.Checked Then
                parms.MortalityType = EwEUtils.Core.ePSDMortalityTypes.GroupZ
            End If
            If mnuItmLorenzen.Checked Then
                parms.MortalityType = EwEUtils.Core.ePSDMortalityTypes.Lorenzen
                parms.LohrenzenLatNWCorner = CSng(m_fpLorenzenLatNWCorner.Value)
                parms.LohrenzenLatSECorner = CSng(m_fpLorenzenLatSECorner.Value)
            End If

            'Group included in PSD
            For iGroup As Integer = 1 To m_core.nLivingGroups
                grpInput = m_core.EcoPathGroupInputs(iGroup)
                grpInput.PSDIncluded = sgStyleGuide.GroupVisible(iGroup)
            Next

            'Number of weight class
            parms.NumWeightClasses = CInt(m_fpNoOfPointsPSD.Value)

            'First weight class
            parms.FirstWeightClass = CSng(m_fpMinWeight.Value)
        End Sub

        Private Sub UpdatePlot()
            Me.zgcZedGraphCntl.AxisChange()
            Me.zgcZedGraphCntl.Refresh()
        End Sub

        Private Sub PlotCurve()
            AddCurves(CreatePane(My.Resources.PSD_PLOTCAPTION_PSD, My.Resources.PSD_XAXISLABEL_WEIGHTCLASS, _
                                 My.Resources.PSD_YAXISLABEL_BIOMASS))
            UpdatePlot()
        End Sub

        Private Sub FindSystemPSD(ByVal sSystemPSD() As Single)
            Dim grpInput As cEcoPathGroupInput = Nothing
            Dim grpOutput As cEcoPathGroupOutput = Nothing

            'Find the system PSD by summing the group PSD
            For iGroup As Integer = 1 To m_core.nLivingGroups
                grpInput = m_core.EcoPathGroupInputs(iGroup)
                If grpInput.PSDIncluded Then
                    grpOutput = m_core.EcoPathGroupOutputs(iGroup)
                    For iWtClass As Integer = 1 To m_core.nWeightClasses
                        sSystemPSD(iWtClass) = sSystemPSD(iWtClass) + grpOutput.PSD(iWtClass)
                    Next
                End If
            Next
        End Sub

        Private Sub FindRegression(ByRef sSlope As Single, ByRef sIntercept As Single, _
                                   ByVal sSystemPSD() As Single)

            Dim parms As cPSDParameters = Me.m_core.ParticleSizeDistributionParameters
            Dim sXValue As Single = 0
            Dim dSumX As Double = 0
            Dim dSumY As Double = 0
            Dim iNum As Integer = 0
            Dim dXMean As Double
            Dim dYMean As Double
            Dim dSumXdevYdev As Double = 0
            Dim dSumXdevSq As Double = 0

            For iWtClass As Integer = 1 To m_core.nWeightClasses
                If sSystemPSD(iWtClass) * 100000 > 0 Then
                    sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))

                    dSumX = dSumX + Math.Log10(sXValue)
                    dSumY = dSumY + Math.Log10(sSystemPSD(iWtClass) * 100000)
                    iNum = iNum + 1
                End If
            Next
            dXMean = dSumX / iNum
            dYMean = dSumY / iNum

            For iWtClass As Integer = 1 To m_core.nWeightClasses
                If sSystemPSD(iWtClass) * 100000 > 0 Then
                    sXValue = CSng(parms.FirstWeightClass * 2 ^ (iWtClass - 1))

                    dSumXdevYdev = dSumXdevYdev + (Math.Log10(sXValue) - dXMean) * (Math.Log10(sSystemPSD(iWtClass) * 100000) - dYMean)
                    dSumXdevSq = dSumXdevSq + (Math.Log10(sXValue) - dXMean) ^ 2
                End If
            Next

            sSlope = CSng(dSumXdevYdev / dSumXdevSq)
            sIntercept = CSng(dYMean - sSlope * dXMean)
        End Sub
#End Region 'Helper methods

    End Class

End Namespace