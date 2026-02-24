' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Namespace Controls.Map.Layers

    ''' <summary>
    ''' Visual editor for <see cref="cLayerEditorHabitat"/>
    ''' </summary>
    Public Class ucLayerEditorHabitat

        Public Sub New()
            MyBase.New()
            Me.InitializeComponent()
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)
            Me.m_cbUseHabitatAreaCorrection.Checked = Me.UIContext.StyleGuide.UseHabitatAreaCorrection
            Me.UpdateEditor()
        End Sub

        Protected Overloads Property Editor() As cLayerEditorHabitat
            Get
                Return DirectCast(MyBase.Editor, cLayerEditorHabitat)
            End Get
            Set(editor As cLayerEditorHabitat)
                ' Sanity check
                Debug.Assert(TypeOf editor Is cLayerEditorHabitat, "ucLayerEditorHabitat connected to wrong editor class")
                ' Set
                MyBase.Editor = editor
                Me.UpdateEditor()
            End Set
        End Property

        Private Sub OnUseHabitatAreaCorrectionChanged(sender As Object, e As EventArgs) Handles m_cbUseHabitatAreaCorrection.CheckedChanged
            If (Me.IsAttached) Then
                Me.UIContext.StyleGuide.UseHabitatAreaCorrection = Me.m_cbUseHabitatAreaCorrection.Checked
                Me.UpdateEditor()
            End If
        End Sub

        Private Sub UpdateEditor()
            If (Me.IsAttached) Then
                Me.Editor.UseHabitatAreaCorrection = Me.UIContext.StyleGuide.UseHabitatAreaCorrection
            End If
        End Sub

    End Class

End Namespace
