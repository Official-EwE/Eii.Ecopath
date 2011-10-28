#Region " Imports "

Option Strict On
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Reflection
Imports EwECore
Imports EwECore.Auxiliary
Imports SharedResources = ScientificInterfaceShared.My.Resources
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Style

#End Region ' Imports

Namespace Controls

    Public Class ucEditImage

#Region " Constructor "

        Public Sub New(ByVal uic As cUIContext, _
                       ByVal vs As cVisualStyle, _
                       ByVal style As cVisualStyle.eVisualStyleTypes)
            MyBase.New(uic, vs, style)
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Events "

        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim avs As cVisualStyle() = Me.UIContext.StyleGuide.GetVisualStyles(-1, cStyleGuide.eBrushType.Glyphs)

            For Each vs As cVisualStyle In avs
                Me.m_glyphSelect.AddImage(vs.Image)
            Next
            Me.m_glyphSelect.AddImage(Me.VisualStyle.Image)
            Me.m_glyphSelect.SelectedImage = Me.VisualStyle.Image
            Me.m_glyphSelect.Enabled = True
            Me.btnImport.Enabled = True
        End Sub

        Private Sub btnImport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnImport.Click

            Dim img As Image = Nothing
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

            cmdFO.Invoke(SharedResources.FILEFILTER_IMAGE)

            If (cmdFO.Result = Windows.Forms.DialogResult.OK) Then
                Try
                    ' Create image
                    img = Image.FromFile(cmdFO.FileName)
                    ' Add image
                    If Me.m_glyphSelect.AddImage(img) Then
                        ' Select it
                        Me.m_glyphSelect.SelectedImage = img
                    Else
                        ' Warn user
                        MsgBox(My.Resources.PROMPT_FILEIMPORT_INVALIDIMAGEFORGLYPH, MsgBoxStyle.Information Or MsgBoxStyle.OkOnly)
                    End If
                Catch ex As Exception
                    ' Neh
                End Try
            End If
        End Sub

        Private Sub m_glyphSelect_OnSelectionChanged(ByVal sender As ucGlyphSelect, ByVal e As System.EventArgs) _
            Handles m_glyphSelect.OnSelectionChanged
            Me.FireStyleChangedEvent()
        End Sub

#End Region ' Events

#Region " Overridables "

        Public Overrides Function Apply(ByVal vs As cVisualStyle) As Boolean
            vs.Image = Me.m_glyphSelect.SelectedImage
            Return True
        End Function

#End Region ' Overridables

    End Class

End Namespace
