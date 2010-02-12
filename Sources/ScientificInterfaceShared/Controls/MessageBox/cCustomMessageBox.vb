#Region " Imports "

Imports System.Windows.Forms
Imports System.Drawing
Imports Microsoft.Win32

#End Region ' Imports

Namespace Controls

    ''' =======================================================================
    ''' <summary>
    ''' Factory for conjuring a custom message box.
    ''' </summary>
    ''' =======================================================================
    Friend Class cCustomMessageBox

#Region " Declarations "

        Friend Structure sMessageBoxOptions
            Public Icon As Icon
            Public Prompt As String
            Public Caption As String
            Public Sound As String
            Public MessageBoxIcon As MessageBoxIcon
            Public Buttons As MessageBoxButtons
            Public Result As DialogResult
            Public CanSuppress As Boolean
        End Structure

#End Region ' Declarations

#Region " Public Show Options "

        Friend Shared Function Show(ByVal Text As String) As DialogResult
            Return cCustomMessageBox.Show(Text, "")
        End Function

        Friend Shared Function Show(ByVal Text As String, _
                                    ByVal Caption As String) As DialogResult
            Return cCustomMessageBox.Show(Text, Caption, Nothing)
        End Function

        Friend Shared Function Show(ByVal Text As String, _
                                    ByVal Caption As String, _
                                    ByVal Buttons As MessageBoxButtons, _
                                    Optional ByVal Sound As String = "") As DialogResult
            Return cCustomMessageBox.Show(Text, Caption, Buttons, MessageBoxIcon.Information, Sound)
        End Function

        Friend Shared Function Show(ByVal Text As String, _
                                    ByVal Caption As String, _
                                    ByVal Buttons As MessageBoxButtons, _
                                    ByRef Suppress As Boolean, _
                                    Optional ByVal Sound As String = "") As DialogResult
            Return cCustomMessageBox.Show(Text, Caption, Buttons, MessageBoxIcon.Information, Suppress, Sound)
        End Function

        Friend Shared Function Show(ByVal Text As String, _
                                    ByVal Caption As String, _
                                    ByVal Buttons As MessageBoxButtons, _
                                    ByVal IconType As MessageBoxIcon, _
                                    Optional ByVal Sound As String = "") As DialogResult

            Dim options As New sMessageBoxOptions

            ' Provide defaults
            If String.IsNullOrEmpty(Caption) Then Caption = "Message"

            With options
                .Prompt = Text
                .Caption = Caption
                .Sound = Sound
                .Buttons = Buttons
                .MessageBoxIcon = IconType
                .Icon = cCustomMessageBox.GetIcon(MessageBoxIcon.Information)
                .CanSuppress = False
            End With

            cCustomMessageBox.ShowDialog(options)
            Return options.Result
        End Function

        Friend Shared Function Show(ByVal Text As String, _
                                    ByVal Caption As String, _
                                    ByVal Buttons As MessageBoxButtons, _
                                    ByVal IconType As MessageBoxIcon, _
                                    ByRef Suppress As Boolean, _
                                    Optional ByVal Sound As String = "") As DialogResult
            Dim options As New sMessageBoxOptions

            ' Provide defaults
            If String.IsNullOrEmpty(Caption) Then Caption = "Message"

            With options
                .Prompt = Text
                .Caption = Caption
                .Sound = Sound
                .Buttons = Buttons
                .MessageBoxIcon = IconType
                .Icon = cCustomMessageBox.GetIcon(MessageBoxIcon.Information)
                .CanSuppress = True
            End With

            cCustomMessageBox.ShowDialog(options)

            Suppress = options.CanSuppress
            Return options.Result

        End Function


#End Region ' Public Show Options

#Region " Private Subs And Functions "

        Private Shared Function GetIcon(ByVal Icon As MessageBoxIcon) As Icon

            Dim objIcon As Icon = Nothing

            Select Case Icon
                Case MessageBoxIcon.Asterisk
                    objIcon = SystemIcons.Asterisk
                Case MessageBoxIcon.Error
                    objIcon = SystemIcons.Error
                Case MessageBoxIcon.Exclamation
                    objIcon = SystemIcons.Exclamation
                Case MessageBoxIcon.Hand, _
                     MessageBoxIcon.Stop
                    objIcon = SystemIcons.Hand
                Case MessageBoxIcon.Information
                    objIcon = SystemIcons.Information
                Case MessageBoxIcon.Question
                    objIcon = SystemIcons.Question
                Case MessageBoxIcon.Warning
                    objIcon = SystemIcons.Warning
                Case Else
                    ' NOP
            End Select

            Return objIcon

        End Function

        Private Shared Sub ShowDialog(ByVal options As sMessageBoxOptions)

            Select Case options.Buttons

                Case MessageBoxButtons.OK
                    cCustomMessageBox.Make1Button(options)

                Case MessageBoxButtons.YesNo, _
                     MessageBoxButtons.OKCancel, _
                     MessageBoxButtons.RetryCancel
                    cCustomMessageBox.Make2Button(options)

                Case MessageBoxButtons.AbortRetryIgnore, _
                     MessageBoxButtons.YesNoCancel
                    cCustomMessageBox.Make3Button(options)

            End Select
        End Sub

        Private Shared Sub Make3Button(ByVal options As sMessageBoxOptions)
            Dim frm As Form = Nothing
            If options.CanSuppress Then
                frm = New frmThreeButtonsCheckBox(options)
            Else
                frm = New frmThreeButtons(options)
            End If
            frm.ShowDialog()
        End Sub

        Private Shared Sub Make2Button(ByVal options As sMessageBoxOptions)
            Dim frm As Form = Nothing
            If options.CanSuppress Then
                frm = New frmTwoButtonsCheckBox(options)
            Else
                frm = New frmTwoButtons(options)
            End If
            frm.ShowDialog()
        End Sub

        Private Shared Sub Make1Button(ByVal options As sMessageBoxOptions)
            Dim frm As frmOneButton = New frmOneButton(options)
            frm.ShowDialog()
        End Sub

#End Region ' Private Subs And Functions

    End Class

End Namespace ' Controls
