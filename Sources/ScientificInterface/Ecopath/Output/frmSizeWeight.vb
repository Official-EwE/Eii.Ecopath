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
' Copyright 1991- 
'    Ecopath International Initiative, Barcelona, Spain
' ===============================================================================
'

Imports ZedGraph
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Ecopath.Output

    Public Class SizeWeightPlot

#Region " Variables "

        Private m_zgh As cZedGraphHelper = Nothing

#End Region ' Variables

#Region " Constructor "

        Public Sub New()

            Me.InitializeComponent()

        End Sub

#End Region ' Constructor

#Region " Event handlers "

        Protected Overrides Sub OnLoad(e As System.EventArgs)

            'Dim parms As cPSDParameters = Nothing
            'Dim str As String = ""
            'Dim msg As cMessage = Nothing

            Me.m_zgh = New cZedGraphHelper()
            Me.m_zgh.Attach(Me.UIContext, Me.zgcZedGraphCntl)

            Me.AddCurves(Me.CreatePane(My.Resources.CAPTION_SIZEWT, SharedResources.HEADER_SIZECLASS, ""))

            Me.UpdatePlot()

            'parms = Me.me.UIContext.Core.ParticleSizeDistributionParameters
            'If parms.PSDEnabled = False Then
            '    str = My.Resources.PSD_MSG_PSDDISABLED
            '    msg = New cMessage(str, eMessageType.TooManyMissingParameters, eCoreComponentType.EcoPath, eMessageImportance.Warning)
            '    Me.me.UIContext.Core.Messages.SendMessage(msg)
            'End If
        End Sub

        'Private Sub SizeWeightPlot_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        '    Dim parms As cPSDParameters = Nothing

        '    parms = Me.me.UIContext.Core.ParticleSizeDistributionParameters
        '    If parms.PSDEnabled = False Then Me.Close()
        'End Sub

        Protected Overrides Sub OnFormClosed(e As System.Windows.Forms.FormClosedEventArgs)
            Me.m_zgh.Detach()
            Me.m_zgh = Nothing
            MyBase.OnFormClosed(e)
        End Sub
#End Region ' Event handlers

#Region " Helper methods "

        Private Function CreatePane(strTitle As String, strXAxisTitle As String, _
                                    strYAxisTitle As String) As GraphPane
            Dim pane As GraphPane = Me.zgcZedGraphCntl.GraphPane

            Me.InitGraphPane(strTitle, strXAxisTitle, strYAxisTitle, pane)
            Return pane
        End Function

        Private Sub InitGraphPane(strTitle As String, strXAxisTitle As String, _
                                    strYAxisTitle As String, pane As GraphPane)
            pane.Title.Text = strTitle
            pane.Title.FontSpec.IsBold = False
            pane.Title.FontSpec.Size = 16

            pane.XAxis.Scale.IsVisible = True 'False
            pane.XAxis.Title.Text = strXAxisTitle
            pane.XAxis.Title.FontSpec.Size = 14

            pane.YAxis.Scale.IsVisible = True 'False
            pane.YAxis.Title.Text = strYAxisTitle
            pane.YAxis.Title.FontSpec.Size = 14

            pane.XAxis.Scale.Min = 1
            pane.XAxis.Scale.Max = Me.UIContext.Core.nLivingGroups
            pane.YAxis.Scale.Min = 0

            pane.YAxis.MinorTic.IsAllTics = False
            pane.XAxis.MinorTic.IsAllTics = False

            'Me.UpdateColors()
        End Sub

        Private Sub AddCurves(pane As GraphPane)
            Dim resultLists As New List(Of PointPairList)
            Dim grpOutput As cEcopathGroupOutput = Nothing

            Me.InitLists(resultLists, 2)

            For iGroup As Integer = 1 To Me.UIContext.Core.nLivingGroups
                grpOutput = Me.UIContext.Core.EcopathGroupOutputs(iGroup)
                resultLists(0).Add(iGroup, grpOutput.BiomassAvgSzWt)
                resultLists(1).Add(iGroup, grpOutput.BiomassSzWt)
            Next

            ' Clear pane
            pane.CurveList.Clear()

            Me.AddCurveToGraphPane(pane, My.Resources.LEGEND_BYNUM, resultLists(0), Color.Blue)
            Me.AddCurveToGraphPane(pane, My.Resources.LEGEND_BYBIOMASS, resultLists(1), Color.Brown)
        End Sub

        Private Sub InitLists(ByRef lists As List(Of PointPairList), size As Integer)
            ' Init the result lists
            For i As Integer = 1 To size
                Dim list As New PointPairList()
                lists.Add(list)
            Next
        End Sub

        Private Sub AddCurveToGraphPane(pane As GraphPane, legend As String, list As PointPairList, _
                                        clr As Color)
            pane.AddCurve(legend, list, clr, SymbolType.None)
        End Sub

        Private Sub UpdatePlot()
            Me.zgcZedGraphCntl.AxisChange()
            Me.zgcZedGraphCntl.Refresh()
        End Sub

        Private Function IsGroupSelected() As Boolean()
            Dim bGroupSelected(Me.UIContext.Core.nLivingGroups) As Boolean

            For i As Integer = 1 To Me.UIContext.Core.nLivingGroups
                bGroupSelected(i) = Me.UIContext.StyleGuide.GroupVisible(i)
            Next
            Return bGroupSelected
        End Function

#End Region ' Helper methods

    End Class

End Namespace