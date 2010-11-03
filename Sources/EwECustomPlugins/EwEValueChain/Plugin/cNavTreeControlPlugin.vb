#Region " Imports "

Option Strict On
Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Core
Imports System.Text

#End Region ' Imports

Public MustInherit Class cNavTreeControlPlugin
    Implements INavigationTreeItemPlugin

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Name"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property Name() As String Implements EwEPlugin.IPlugin.Name

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IGUIPlugin.ControlImage"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property ControlImage() As System.Drawing.Image _
        Implements EwEPlugin.IGUIPlugin.ControlImage

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IGUIPlugin.ControlText"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property ControlText() As String _
        Implements EwEPlugin.IGUIPlugin.ControlText

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IGUIPlugin.ControlTooltipText"/>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property ControlTooltipText() As String _
        Implements EwEPlugin.IGUIPlugin.ControlTooltipText
        Get
            Return "value chain plug-in"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IGUIPlugin.EnabledState"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property EnabledState() As EwEUtils.Core.eCoreExecutionState _
        Implements EwEPlugin.IGUIPlugin.EnabledState
        Get
            Return eCoreExecutionState.EcopathCompleted
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IGUIPlugin.OnControlClick"/>
    ''' -----------------------------------------------------------------------
    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs, ByRef frmPlugin As System.Windows.Forms.Form) _
        Implements EwEPlugin.IGUIPlugin.OnControlClick
        frmPlugin = cPluginPoint.SwitchForm(Me.FormPage)
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation"/>
    ''' -----------------------------------------------------------------------
    Public MustOverride ReadOnly Property NavigationTreeItemLocation() As String _
        Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Author"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Author() As String Implements EwEPlugin.IPlugin.Author
        Get
            Return "UBC Fisheries Centre, ECOST project, North Sea Centre"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Contact"/>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Contact() As String Implements EwEPlugin.IPlugin.Contact
        Get
            Return "mailto:v.christensen@fisheries.ubc.ca,j.steenbeek@fisheries.ubc.ca"
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Description"/>
    ''' -----------------------------------------------------------------------
    Public Overridable ReadOnly Property Description() As String Implements EwEPlugin.IPlugin.Description
        Get
            Dim sb As New StringBuilder()
            sb.AppendLine("ValueChain - an economic fisheries model for EwE6")
            sb.AppendLine("")
            sb.AppendLine("This plug-in calculates a range of economic and social-economic indicators based on Ecopath and Ecosim data, where users can define economic systems as value chains of desired complexity.")
            sb.AppendLine("")
            sb.AppendLine("This plug-in was developed in conjunction with the ECOST project (http://www.ird.fr/ecostproject), and was partially funded by the North Sea Centre in Hirtshals, Denmark.")
            Return sb.ToString()
        End Get
    End Property

    ''' -----------------------------------------------------------------------
    ''' <inheritdoc cref="EwEPlugin.IPlugin.Initialize"/>
    ''' -----------------------------------------------------------------------
    Public Overridable Sub Initialize(ByVal core As Object) Implements EwEPlugin.IPlugin.Initialize
        ' NOP
    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Must override to define the name of the <see cref="frmMain.ShowForm"></see>value chain page that a 
    ''' navigation item opens.
    ''' </summary>
    ''' <returns></returns>
    ''' -----------------------------------------------------------------------
    Public MustOverride Function FormPage() As String

End Class
