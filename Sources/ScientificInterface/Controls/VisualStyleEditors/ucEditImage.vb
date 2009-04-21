'==============================================================================
'
' $Log: ucEditImage.vb,v $
' Revision 1.3  2009/04/21 19:42:32  jeroens
' Localized
'
' Revision 1.2  2008/11/08 23:50:53  jeroens
' Renamed file commands
'
' Revision 1.1  2008/09/26 07:31:25  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.4  2008/09/09 14:44:56  jeroens
' File dialog interaction performed via central command, which solves Vista incompatibility issues
'
' Revision 1.3  2008/06/02 00:01:47  jeroens
' Added ScientificInterfaceShared
'
' Revision 1.2  2008/01/08 20:07:21  jeroens
' Fixed bug 368
'
' Revision 1.1  2008/01/01 19:51:18  jeroens
' New and/or moved
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEUtils.Commands
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Reflection

#End Region ' Imports

Namespace Controls

    Public Class ucEditImage

#Region " Constructor "

        Public Sub New(ByVal vs As cVisualStyle, ByVal style As cVisualStyle.eVisualStyleTypes)
            MyBase.New(vs, style)
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Events "

        Private Sub ucEditImage_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Dim prov As New cEwEBrushProvider
            Dim avs As cVisualStyle() = prov.GetVisualStyles(-1, cEwEBrushProvider.eBrushType.Glyphs)

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
            Dim cmdh As CommandHandler = CommandHandler.GetInstance()
            Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

            cmdFO.Invoke(My.Resources.FILEFILTER_IMAGE)

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

        Private Sub m_glyphSelect_OnSelectionChanged(ByVal sender As ucGlyphSelect, ByVal e As System.EventArgs) Handles m_glyphSelect.OnSelectionChanged
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
