Namespace Ecopath

    <Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
    Partial Class EditMultiStanza
        Inherits System.Windows.Forms.Form

        'Form overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            Me.components = New System.ComponentModel.Container
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EditMultiStanza))
            Me.btnCalculate = New System.Windows.Forms.Button
            Me.btnOK = New System.Windows.Forms.Button
            Me.btnCancel = New System.Windows.Forms.Button
            Me.plMultiStanzaGrid = New System.Windows.Forms.Panel
            Me.Label2 = New System.Windows.Forms.Label
            Me.Label3 = New System.Windows.Forms.Label
            Me.Label4 = New System.Windows.Forms.Label
            Me.Label5 = New System.Windows.Forms.Label
            Me.Label6 = New System.Windows.Forms.Label
            Me.Label7 = New System.Windows.Forms.Label
            Me.txtK = New System.Windows.Forms.TextBox
            Me.txtRecPwr = New System.Windows.Forms.TextBox
            Me.txtBAB = New System.Windows.Forms.TextBox
            Me.txtWmatWinf = New System.Windows.Forms.TextBox
            Me.m_zgc = New ZedGraph.ZedGraphControl
            Me.chkFFecun = New System.Windows.Forms.CheckBox
            Me.cmbSpeciesName = New System.Windows.Forms.ComboBox
            Me.cmbFF = New System.Windows.Forms.ComboBox
            Me.SuspendLayout()
            '
            'btnCalculate
            '
            resources.ApplyResources(Me.btnCalculate, "btnCalculate")
            Me.btnCalculate.Name = "btnCalculate"
            Me.btnCalculate.UseVisualStyleBackColor = True
            '
            'btnOK
            '
            resources.ApplyResources(Me.btnOK, "btnOK")
            Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.btnOK.Name = "btnOK"
            Me.btnOK.UseVisualStyleBackColor = True
            '
            'btnCancel
            '
            resources.ApplyResources(Me.btnCancel, "btnCancel")
            Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.btnCancel.Name = "btnCancel"
            Me.btnCancel.UseVisualStyleBackColor = True
            '
            'plMultiStanzaGrid
            '
            resources.ApplyResources(Me.plMultiStanzaGrid, "plMultiStanzaGrid")
            Me.plMultiStanzaGrid.Name = "plMultiStanzaGrid"
            Me.plMultiStanzaGrid.TabStop = True
            '
            'Label2
            '
            resources.ApplyResources(Me.Label2, "Label2")
            Me.Label2.Name = "Label2"
            '
            'Label3
            '
            resources.ApplyResources(Me.Label3, "Label3")
            Me.Label3.Name = "Label3"
            '
            'Label4
            '
            resources.ApplyResources(Me.Label4, "Label4")
            Me.Label4.Name = "Label4"
            '
            'Label5
            '
            resources.ApplyResources(Me.Label5, "Label5")
            Me.Label5.Name = "Label5"
            '
            'Label6
            '
            resources.ApplyResources(Me.Label6, "Label6")
            Me.Label6.Name = "Label6"
            '
            'Label7
            '
            resources.ApplyResources(Me.Label7, "Label7")
            Me.Label7.Name = "Label7"
            '
            'txtK
            '
            resources.ApplyResources(Me.txtK, "txtK")
            Me.txtK.Name = "txtK"
            '
            'txtRecPwr
            '
            resources.ApplyResources(Me.txtRecPwr, "txtRecPwr")
            Me.txtRecPwr.Name = "txtRecPwr"
            '
            'txtBAB
            '
            resources.ApplyResources(Me.txtBAB, "txtBAB")
            Me.txtBAB.Name = "txtBAB"
            '
            'txtWmatWinf
            '
            resources.ApplyResources(Me.txtWmatWinf, "txtWmatWinf")
            Me.txtWmatWinf.Name = "txtWmatWinf"
            '
            'm_zgc
            '
            resources.ApplyResources(Me.m_zgc, "m_zgc")
            Me.m_zgc.Name = "m_zgc"
            Me.m_zgc.ScrollGrace = 0
            Me.m_zgc.ScrollMaxX = 0
            Me.m_zgc.ScrollMaxY = 0
            Me.m_zgc.ScrollMaxY2 = 0
            Me.m_zgc.ScrollMinX = 0
            Me.m_zgc.ScrollMinY = 0
            Me.m_zgc.ScrollMinY2 = 0
            Me.m_zgc.TabStop = False
            '
            'chkFFecun
            '
            resources.ApplyResources(Me.chkFFecun, "chkFFecun")
            Me.chkFFecun.Name = "chkFFecun"
            Me.chkFFecun.UseVisualStyleBackColor = True
            '
            'cmbSpeciesName
            '
            resources.ApplyResources(Me.cmbSpeciesName, "cmbSpeciesName")
            Me.cmbSpeciesName.FormattingEnabled = True
            Me.cmbSpeciesName.Name = "cmbSpeciesName"
            '
            'cmbFF
            '
            resources.ApplyResources(Me.cmbFF, "cmbFF")
            Me.cmbFF.FormattingEnabled = True
            Me.cmbFF.Name = "cmbFF"
            '
            'EditMultiStanza
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.CancelButton = Me.btnCancel
            Me.Controls.Add(Me.cmbFF)
            Me.Controls.Add(Me.cmbSpeciesName)
            Me.Controls.Add(Me.chkFFecun)
            Me.Controls.Add(Me.m_zgc)
            Me.Controls.Add(Me.txtWmatWinf)
            Me.Controls.Add(Me.txtBAB)
            Me.Controls.Add(Me.txtRecPwr)
            Me.Controls.Add(Me.txtK)
            Me.Controls.Add(Me.Label7)
            Me.Controls.Add(Me.Label6)
            Me.Controls.Add(Me.Label5)
            Me.Controls.Add(Me.Label4)
            Me.Controls.Add(Me.Label3)
            Me.Controls.Add(Me.Label2)
            Me.Controls.Add(Me.plMultiStanzaGrid)
            Me.Controls.Add(Me.btnCancel)
            Me.Controls.Add(Me.btnOK)
            Me.Controls.Add(Me.btnCalculate)
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "EditMultiStanza"
            Me.ShowIcon = False
            Me.ShowInTaskbar = False
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
        Friend WithEvents btnCalculate As System.Windows.Forms.Button
        Friend WithEvents btnOK As System.Windows.Forms.Button
        Friend WithEvents btnCancel As System.Windows.Forms.Button
        Friend WithEvents plMultiStanzaGrid As System.Windows.Forms.Panel
        Friend WithEvents Label2 As System.Windows.Forms.Label
        Friend WithEvents Label3 As System.Windows.Forms.Label
        Friend WithEvents Label4 As System.Windows.Forms.Label
        Friend WithEvents Label5 As System.Windows.Forms.Label
        Friend WithEvents Label6 As System.Windows.Forms.Label
        Friend WithEvents Label7 As System.Windows.Forms.Label
        Friend WithEvents cmbSpeciesName As System.Windows.Forms.ComboBox
        Friend WithEvents txtK As TextBox
        Friend WithEvents txtRecPwr As TextBox
        Friend WithEvents txtBAB As TextBox
        Friend WithEvents txtWmatWinf As TextBox
        Friend WithEvents m_zgc As ZedGraph.ZedGraphControl
        Friend WithEvents chkFFecun As System.Windows.Forms.CheckBox
        Friend WithEvents cmbFF As System.Windows.Forms.ComboBox
    End Class

End Namespace
