#Region " Imports "

Option Strict On
Imports EwECore
Imports ScientificInterfaceShared.Style
Imports EwEUtils.Commands
Imports ScientificInterfaceShared.Commands
Imports System.IO

#End Region ' Imports

Namespace Controls

    ''' <summary>
    ''' User control to help users pick a field from a list of options.
    ''' </summary>
    Public Class ucFieldPicker
        Implements IUIElement

        Private m_aFields As Array = Nothing
        Private m_formatter As ITypeFormatter = Nothing
        Private m_bShowDirPicker As Boolean = False
        Private m_uic As cUIContext = Nothing

        Public Sub New()
            Me.InitializeComponent()
        End Sub

        Public Property UIContext() As cUIContext _
            Implements IUIElement.UIContext
            Get
                Return Me.m_uic
            End Get
            Set(ByVal value As cUIContext)
                Me.m_uic = value
            End Set
        End Property

        Public Property Label() As String
            Get
                Return Me.m_tsddFields.Text
            End Get
            Set(ByVal value As String)
                Me.m_tsddFields.Text = value
            End Set
        End Property

        Public Property Fields() As Array
            Get
                Return Me.m_aFields
            End Get
            Set(ByVal value As Array)
                Me.m_aFields = value
                Me.UpdateDropdown()
            End Set
        End Property

        Public Property TypeFormatter() As ITypeFormatter
            Get
                Return Me.m_formatter
            End Get
            Set(ByVal value As ITypeFormatter)
                Me.m_formatter = value
                Me.UpdateDropdown()
            End Set
        End Property

        Public Property ShowDirectoryPicker() As Boolean
            Get
                Return Me.m_bShowDirPicker
            End Get
            Set(ByVal value As Boolean)
                Me.m_bShowDirPicker = value
                Me.UpdateDropdown()
            End Set
        End Property

        Public Event OnFieldPicked(ByVal sender As ucFieldPicker, ByVal value As Object)

        Private Sub UpdateDropdown()

            Dim item As ToolStripItem = Nothing
            Dim strText As String = ""

            Me.m_tsddFields.DropDownItems.Clear()

            If Me.m_bShowDirPicker Then
                item = New ToolStripButton("Choose folder...", Nothing, AddressOf OnPickDirectory)
                Me.m_tsddFields.DropDown.Items.Add(item)
            End If

            If Me.m_aFields Is Nothing Then Return

            For Each obj As Object In Me.m_aFields
                If (obj IsNot Nothing) Then
                    strText = obj.ToString
                    If Me.m_formatter IsNot Nothing Then
                        Try
                            strText = Me.m_formatter.GetDescriptor(obj, eDescriptorTypes.Name)
                        Catch ex As Exception
                            ' Whoah
                        End Try
                    End If
                    item = New ToolStripButton(strText, Nothing, AddressOf OnItemClicked)
                    item.Tag = obj
                    Me.m_tsddFields.DropDown.Items.Add(item)
                End If
            Next

        End Sub

        Private Sub OnItemClicked(ByVal sender As Object, ByVal e As EventArgs)
            Try
                RaiseEvent OnFieldPicked(Me, DirectCast(sender, ToolStripButton).Tag)
            Catch ex As Exception
                ' Whoops
            End Try
        End Sub

        Public Event OnDirectoryPicked(ByVal sender As ucFieldPicker, ByVal strDirectory As String)

        Private Sub OnPickDirectory(ByVal sender As Object, ByVal e As EventArgs)
            If Me.m_bShowDirPicker And Me.m_uic IsNot Nothing Then

                Dim cmdh As cCommandHandler = Me.m_uic.CommandHandler
                Dim cmd As cDirectoryOpenCommand = DirectCast(cmdh.GetCommand(cDirectoryOpenCommand.COMMAND_NAME), cDirectoryOpenCommand)

                If cmd IsNot Nothing Then
                    cmd.Description = "Select directory"
                    cmd.Directory = Path.GetDirectoryName(".\")
                    cmd.Invoke()
                    If cmd.Result = DialogResult.OK Then
                        Try
                            RaiseEvent OnDirectoryPicked(Me, cmd.Directory)
                        Catch ex As Exception
                            ' Whoops
                        End Try
                    End If
                End If
            End If
        End Sub

    End Class

End Namespace
