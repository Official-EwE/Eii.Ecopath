Imports EwECore

Public Class cResultsHolder

    Implements EwEPlugin.IMenuItemPlugin

    Private ResultsForm As frmResults
    Private m_core As cCore

    Public ReadOnly Property ControlImage() As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ControlText() As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            Return "Results Extractor"
        End Get
    End Property

    Public ReadOnly Property ControlTooltipText() As String Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return EwEUtils.Core.eCoreExecutionState.EcosimCompleted
        End Get
    End Property

    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) Implements EwEPlugin.IGUIPlugin.OnControlClick

        If ResultsForm Is Nothing Then
            ResultsForm = New frmResults
            ResultsForm.Initialize(m_core)
            ResultsForm.StartForm(sender, e, frmPlugin)
        End If
        If ResultsForm IsNot Nothing And ResultsForm.IsDisposed Then
            ResultsForm = New frmResults
            ResultsForm.Initialize(m_core)
            ResultsForm.StartForm(sender, e, frmPlugin)
        End If
        ResultsForm.Show()

    End Sub

    Public ReadOnly Property MenuItemLocation() As String Implements EwEPlugin.IMenuItemPlugin.MenuItemLocation
        Get
            Return "MenuTools"
        End Get
    End Property

    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "Mark Platts CEFAS"
        End Get
    End Property

    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mark.platts@cefas.co.uk"
        End Get
    End Property

    Public ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Return "Plug-in to aid in extracting results, parameter values and data from EwE"
        End Get
    End Property

    Public Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        m_core = core
    End Sub

    Public ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name
        Get
            Return Me.ControlText()
        End Get
    End Property

End Class
