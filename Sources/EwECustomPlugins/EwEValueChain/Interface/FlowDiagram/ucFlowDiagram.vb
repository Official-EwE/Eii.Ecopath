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
' Copyright 1991-2013 UBC Fisheries Centre, Vancouver BC, Canada.
' ===============================================================================
'
#Region " Imports "

Option Strict On
Imports System.Windows.Forms
Imports EwEUtils.Database.cEwEDatabase
Imports ScientificInterfaceShared.Controls
Imports ScientificInterfaceShared.Style
Imports System.Drawing

#End Region ' Imports

Public Class ucFlowDiagram
    Inherits UserControl
    Implements IUIElement


#Region " Private bits "

    Private Shared g_iLastItem As Integer = 0
    Private Shared g_iLastUnit As Integer = 0

    Private Enum eViewModeType As Integer
        Grid = 0
        Graph
        GraphEquilibrium
    End Enum

    Enum eGraphDataType As Integer
        CostRevenue = 0
        Cost
        Revenue
        Jobs
        Dependents
    End Enum

    ''' <summary>Instance of the Ecost model to poke and prod.</summary>
    Private m_model As cModel = Nothing
    ''' <summary>Instance of model results to reflect.</summary>
    Private m_result As cResults = Nothing
    ''' <summary>UI context to operate on.</summary>
    Private m_uic As cUIContext = Nothing
    Private m_data As IFlowDiagramData = Nothing

    Private m_doodler As cFlowDiagramRenderer = Nothing
    Private m_tree As cFlowDiagramTree = Nothing

#End Region ' Private bits

    Public Sub New(ByVal uic As cUIContext, _
                   ByVal data As cData, _
                   ByVal model As cModel, _
                   ByVal result As cResults)

        Me.InitializeComponent()

        Me.m_uic = uic
        Me.m_data = New cFlowDiagramData(uic, model, data, result)
        Me.m_model = model
        Me.m_result = result

        Me.m_tree = New cFlowDiagramTree(Me.m_data)
        Me.m_doodler = New cFlowDiagramRenderer(Me.m_data, Me.m_tree)

    End Sub

    Public Property UIContext() As cUIContext _
      Implements IUIElement.UIContext
        Get
            Return Me.m_uic
        End Get
        Set(ByVal value As cUIContext)
            If (Me.m_uic IsNot Nothing) Then
                RemoveHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            End If
            Me.m_uic = value
            If (Me.m_uic IsNot Nothing) Then
                AddHandler Me.m_uic.StyleGuide.StyleGuideChanged, AddressOf OnStyleGuideChanged
            End If
        End Set
    End Property

    Protected ReadOnly Property StyleGuide() As cStyleGuide
        Get
            Return Me.m_uic.StyleGuide
        End Get
    End Property

    Protected Overridable Sub OnStyleGuideChanged(ByVal ct As cStyleGuide.eChangeType)
        ' Yo!
        Me.m_pbFD.Invalidate()
    End Sub

    Private Sub OnPaintPictureBox(sender As Object, e As System.Windows.Forms.PaintEventArgs) _
        Handles m_pbFD.Paint

        Dim rc As Rectangle = Me.m_pbFD.ClientRectangle
        Me.m_doodler.DrawFlowDiagram(e.Graphics, rc)

    End Sub

    ''' <summary>
    ''' Overridden to elimate flickering.
    ''' </summary>
    Protected Overrides Sub OnPaintBackground(ByVal pevent As PaintEventArgs)
        ' NOP
    End Sub

End Class
