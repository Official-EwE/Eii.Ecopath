' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

''' =======================================================================
''' <summary>
''' Form baseclass for implementing an Ecosim 'Apply Forcing' or 'Apply 
''' Mediation' interface.
''' </summary>
''' =======================================================================
Public Class frmApplyShapeBase
    Inherits frmEwE

    Public Sub New()
        Me.InitializeComponent()
    End Sub

#Region " Baseclass overrides "

    Protected Overrides Sub OnLoad(e As System.EventArgs)
        MyBase.OnLoad(e)

        If Me.UIContext Is Nothing Then Return
        Me.Grid.UIContext = Me.UIContext

        ' Hook up to core messages
        ' * Shapes manager to refresh lists of avialable FFs
        ' * Ecopath to refresh lists of available groups
        Me.CoreComponents = New eCoreComponentType() {eCoreComponentType.ShapesManager, eCoreComponentType.Ecopath, eCoreComponentType.MediatedInteractionManager}

    End Sub

#End Region ' Baseclass overrides

#Region " Base functionality "

    Protected Sub ClearAll()
        Me.Grid.ClearAllPairs()
    End Sub

    Protected Sub SetAll()
        Me.Grid.SetAllPairs()
    End Sub

#End Region ' Base functionality

#Region " Mandatory overrides "


    Protected Overridable ReadOnly Property Grid() As gridApplyShapeBase
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides Sub OnCoreMessage(msg As EwECore.cMessage)

        Dim bMustRedimension As Boolean = False
        Dim bMustUpdate As Boolean = False

        If (msg.Source = eCoreComponentType.ShapesManager) Then
            If (msg.Type = eMessageType.DataAddedOrRemoved) Then
                ' Redimension when number of shapes has changed
                bMustRedimension = True
            End If
        End If

        If (msg.Source = eCoreComponentType.MediatedInteractionManager) Then
            ' Update content to show new assignments
            bMustUpdate = True
        End If

        If bMustRedimension Then
            Me.Grid.RefreshContent()
        Else
            If bMustUpdate Then
                Me.Grid.UpdateContent()
            End If
        End If
    End Sub

#End Region ' Mandatory overrides

    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmApplyShapeBase))
        Me.SuspendLayout()
        '
        'frmApplyShapeBase
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.Name = "frmApplyShapeBase"
        Me.TabText = ""
        Me.ResumeLayout(False)

    End Sub
End Class