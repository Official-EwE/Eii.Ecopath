' SPDX-License-Identifier: EUPL-1.2
' This file is part of Ecopath with Ecosim (EwE).
' Copyright © 1991– Ecopath International Initiative (EII)

Imports EwECore.Auxiliary
Imports ScientificInterfaceShared.Commands
Imports ScientificInterfaceShared.Style
Imports SharedResources = ScientificInterfaceShared.My.Resources

Namespace Controls

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' User control for editing the image part of a <see cref="cVisualStyle"/>.
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public Class ucEditImage

#Region " Constructor "

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <param name="uic">UIContext to operate onto.</param>
        ''' <param name="vs">The <see cref="cVisualStyle"/> to create the editor for.</param>
        ''' <param name="style">Aspect of the style that needs editing.</param>
        ''' -------------------------------------------------------------------
        Public Sub New(uic As cUIContext,
                       vs As cVisualStyle,
                       style As cVisualStyle.eVisualStyleTypes)
            MyBase.New(uic, vs, style)
            Me.InitializeComponent()
        End Sub

#End Region ' Constructor

#Region " Events "

        Protected Overrides Sub OnLoad(e As System.EventArgs)
            MyBase.OnLoad(e)

            If (Me.UIContext Is Nothing) Then Return

            Dim avs As cVisualStyle() = Me.UIContext.StyleGuide.GetVisualStyles(-1, cStyleGuide.eBrushType.Glyphs)

            For Each vs As cVisualStyle In avs
                Dim img As Image = ConvertFromImageString(vs.ImageString)
                If img IsNot Nothing Then
                    Me.m_glyphSelect.AddImage(img)
                End If
            Next
            Dim currentImg As Image = ConvertFromImageString(Me.VisualStyle.ImageString)
            If currentImg IsNot Nothing Then
                Me.m_glyphSelect.AddImage(currentImg)
            End If
            Me.m_glyphSelect.Enabled = True
            Me.m_btnImport.Enabled = True

            Me.VisualStyle = Me.VisualStyle

        End Sub

        Private Sub OnAddImage(sender As System.Object, e As System.EventArgs) Handles m_btnImport.Click

            Dim img As Image = Nothing
            Dim cmdh As cCommandHandler = Me.UIContext.CommandHandler
            Dim cmdFO As cFileOpenCommand = DirectCast(cmdh.GetCommand(cFileOpenCommand.COMMAND_NAME), cFileOpenCommand)

            cmdFO.Invoke(SharedResources.FILEFILTER_IMAGE)

            If (cmdFO.Result = System.Windows.Forms.DialogResult.OK) Then
                Try
                    ' Create image
                    img = Image.FromFile(cmdFO.FileName)
                    ' Add image
                    If Me.m_glyphSelect.AddImage(img) Then
                        ' Select it
                        Me.m_glyphSelect.SelectedImage = img
                    Else
                        ' Warn user
                        Dim msg As New cMessage(My.Resources.PROMPT_FILEIMPORT_INVALIDIMAGEFORGLYPH, eMessageType.DataImport, eCoreComponentType.External, eMessageImportance.Warning)
                        Me.UIContext.Core.Messages.SendMessage(msg)
                    End If
                Catch ex As Exception
                    ' Neh
                End Try
            End If
        End Sub

        Private Sub m_glyphSelect_OnSelectionChanged(sender As ucGlyphSelect, e As System.EventArgs) _
            Handles m_glyphSelect.OnSelectionChanged
            Me.FireStyleChangedEvent()
        End Sub

#End Region ' Events

#Region " Overridables "

        Public Overrides Property VisualStyle As cVisualStyle
            Get
                Return MyBase.VisualStyle
            End Get
            Set(value As cVisualStyle)
                MyBase.VisualStyle = value
                If (MyBase.VisualStyle IsNot Nothing) And (Me.m_glyphSelect IsNot Nothing) Then
                    Dim img As Image = ConvertFromImageString(Me.VisualStyle.ImageString)
                    Me.m_glyphSelect.SelectedImage = img
                End If
            End Set
        End Property

        Public Overrides Function Apply(vs As cVisualStyle) As Boolean
            Dim img As Image = Me.m_glyphSelect.SelectedImage
            If (img IsNot Nothing) Then
                ' Convert System.Drawing.Image to Base64 PNG string
                Try
                    Using ms As New System.IO.MemoryStream()
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png)
                        vs.ImageString = Convert.ToBase64String(ms.ToArray())
                    End Using
                Catch ex As Exception
                    ' Failed to convert
                    Return False
                End Try
            End If
            Return True
        End Function

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Convert Base64 PNG string to System.Drawing.Image.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Shared Function ConvertFromImageString(base64Png As String) As Image
            If String.IsNullOrEmpty(base64Png) Then Return Nothing
            Try
                Dim imageBytes As Byte() = Convert.FromBase64String(base64Png)
                Using ms As New System.IO.MemoryStream(imageBytes)
                    Return New Bitmap(ms)
                End Using
            Catch
                Return Nothing
            End Try
        End Function

#End Region ' Overridables

    End Class

End Namespace
