#Region " Imports "

Option Explicit On
Option Strict On

Imports System.Drawing.Drawing2D
Imports System.Drawing
Imports System.Text.RegularExpressions
Imports ScientificInterface.Other
Imports EwECore
Imports EwEUtils.Core

#End Region

Namespace Ecosim

    ''' <summary>
    ''' </summary>
    Public Class frmFishingEffort

#Region " Private variables "

        Private m_handler As cFishingEffortShapeGUIHandler = Nothing

#End Region ' Private variables

#Region " Constructors "

        Public Sub New()
            Me.InitializeComponent()
        End Sub

#End Region ' Constructors

#Region " Private event handlers "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            Me.m_handler = New cFishingEffortShapeGUIHandler(Me.UIContext, _
                Me.m_shapeToolBox, Me.m_sketchPad, _
                Me.m_shapeToolboxToolbar, Me.m_sketchPadToolbar)
            Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager}
        End Sub

#End Region ' Private event handlers

#Region " Internal implementation "

        Public Overrides Sub OnCoreMessage(ByVal msg As EwECore.cMessage)
            Select Case msg.Source
                Case eCoreComponentType.ShapesManager
                    If (msg.DataType = eDataTypes.FishingEffort) Then
                        Me.m_handler.Refresh()
                    End If
            End Select
        End Sub

#End Region ' Internal implementation

    End Class

End Namespace


