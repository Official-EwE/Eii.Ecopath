Imports ScientificInterfaceShared.Forms

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMSY
    Inherits frmEwE


    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMSY))
        Me.btRunMSY = New System.Windows.Forms.Button
        Me.btStop = New System.Windows.Forms.Button
        Me.lbFleet = New System.Windows.Forms.Label
        Me.lbiter = New System.Windows.Forms.Label
        Me.lbEffort = New System.Windows.Forms.Label
        Me.txtMSYresults = New System.Windows.Forms.TextBox
        Me.btFleetTradeoffs = New System.Windows.Forms.Button
        Me.rbValue = New System.Windows.Forms.RadioButton
        Me.rbCatch = New System.Windows.Forms.RadioButton
        Me.lblMSY = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'btRunMSY
        '
        resources.ApplyResources(Me.btRunMSY, "btRunMSY")
        Me.btRunMSY.Name = "btRunMSY"
        Me.btRunMSY.UseVisualStyleBackColor = True
        '
        'btStop
        '
        resources.ApplyResources(Me.btStop, "btStop")
        Me.btStop.Name = "btStop"
        Me.btStop.UseVisualStyleBackColor = True
        '
        'lbFleet
        '
        resources.ApplyResources(Me.lbFleet, "lbFleet")
        Me.lbFleet.Name = "lbFleet"
        '
        'lbiter
        '
        resources.ApplyResources(Me.lbiter, "lbiter")
        Me.lbiter.Name = "lbiter"
        '
        'lbEffort
        '
        resources.ApplyResources(Me.lbEffort, "lbEffort")
        Me.lbEffort.Name = "lbEffort"
        '
        'txtMSYresults
        '
        resources.ApplyResources(Me.txtMSYresults, "txtMSYresults")
        Me.txtMSYresults.Name = "txtMSYresults"
        '
        'btFleetTradeoffs
        '
        resources.ApplyResources(Me.btFleetTradeoffs, "btFleetTradeoffs")
        Me.btFleetTradeoffs.Name = "btFleetTradeoffs"
        Me.btFleetTradeoffs.UseVisualStyleBackColor = True
        '
        'rbValue
        '
        resources.ApplyResources(Me.rbValue, "rbValue")
        Me.rbValue.Name = "rbValue"
        Me.rbValue.TabStop = True
        Me.rbValue.UseVisualStyleBackColor = True
        '
        'rbCatch
        '
        resources.ApplyResources(Me.rbCatch, "rbCatch")
        Me.rbCatch.Name = "rbCatch"
        Me.rbCatch.TabStop = True
        Me.rbCatch.UseVisualStyleBackColor = True
        '
        'lblMSY
        '
        resources.ApplyResources(Me.lblMSY, "lblMSY")
        Me.lblMSY.Name = "lblMSY"
        '
        'frmMSY
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lblMSY)
        Me.Controls.Add(Me.rbCatch)
        Me.Controls.Add(Me.rbValue)
        Me.Controls.Add(Me.btFleetTradeoffs)
        Me.Controls.Add(Me.txtMSYresults)
        Me.Controls.Add(Me.lbEffort)
        Me.Controls.Add(Me.lbiter)
        Me.Controls.Add(Me.lbFleet)
        Me.Controls.Add(Me.btStop)
        Me.Controls.Add(Me.btRunMSY)
        Me.Name = "frmMSY"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btRunMSY As System.Windows.Forms.Button
    Friend WithEvents btStop As System.Windows.Forms.Button
    Friend WithEvents lbFleet As System.Windows.Forms.Label
    Friend WithEvents lbiter As System.Windows.Forms.Label
    Friend WithEvents lbEffort As System.Windows.Forms.Label
    Friend WithEvents txtMSYresults As System.Windows.Forms.TextBox
    Friend WithEvents btFleetTradeoffs As System.Windows.Forms.Button
    Friend WithEvents rbValue As System.Windows.Forms.RadioButton
    Friend WithEvents rbCatch As System.Windows.Forms.RadioButton
    Friend WithEvents lblMSY As System.Windows.Forms.Label
End Class
