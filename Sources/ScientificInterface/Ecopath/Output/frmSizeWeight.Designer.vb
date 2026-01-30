' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports ScientificInterfaceShared.Forms

Namespace Ecopath.Output

    Partial Class SizeWeightPlot
        Inherits frmEwE

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()>
        Protected Overrides Sub Dispose(disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SizeWeightPlot))
            Me.zgcZedGraphCntl = New ZedGraph.ZedGraphControl()
            Me.SuspendLayout()
            '
            'zgcZedGraphCntl
            '
            resources.ApplyResources(Me.zgcZedGraphCntl, "zgcZedGraphCntl")
            Me.zgcZedGraphCntl.Name = "zgcZedGraphCntl"
            Me.zgcZedGraphCntl.ScrollGrace = 0.0R
            Me.zgcZedGraphCntl.ScrollMaxX = 0.0R
            Me.zgcZedGraphCntl.ScrollMaxY = 0.0R
            Me.zgcZedGraphCntl.ScrollMaxY2 = 0.0R
            Me.zgcZedGraphCntl.ScrollMinX = 0.0R
            Me.zgcZedGraphCntl.ScrollMinY = 0.0R
            Me.zgcZedGraphCntl.ScrollMinY2 = 0.0R
            '
            'SizeWeightPlot
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
            Me.Controls.Add(Me.zgcZedGraphCntl)
            Me.Name = "SizeWeightPlot"
            Me.TabText = ""
            Me.ResumeLayout(False)

        End Sub
        Friend WithEvents zgcZedGraphCntl As ZedGraph.ZedGraphControl
    End Class

End Namespace
