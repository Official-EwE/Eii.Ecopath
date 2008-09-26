'==============================================================================
'
' $Log: DefaultPlugin.vb,v $
' Revision 1.1  2008/09/26 07:30:35  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2007/03/10 15:26:22  joeb
' Move EwECustomPlugins to EwEDefault
' Added EwENetworkAnalysis
'
' Revision 1.7  2006/09/21 00:57:34  jeroens
' - Disabled mass balance stuff
'
' Revision 1.6  2006/09/20 00:52:55  jeroens
' + Added (disabled) example code how to use the central Command Handler to launch a plug-in form.
'
' Revision 1.5  2006/09/10 13:04:47  jeroens
' - Disabled nav tree item
'
' Revision 1.4  2006/09/06 16:52:24  jeroens
' * Added dockform
'
' Revision 1.3  2006/08/20 21:19:23  jeroens
' * Updated to new plugin subdivisions
'
' Revision 1.2  2006/08/09 19:32:15  jeroens
' * Fixed mass balance invocation bug
'
' Revision 1.1  2006/08/08 14:10:36  jeroens
' + Initial version
'
'==============================================================================

Option Strict On

Imports EwECore
Imports EwEPlugin
Imports EwEUtils.Commands
Imports System.Drawing

''' ---------------------------------------------------------------------------
''' <summary>
''' Default plugin, use as a basis for adding your own plugin functionality.
''' </summary>
''' ---------------------------------------------------------------------------
Public Class DefaultPlugin
    ' Implements IEcopathPlugin ' , INavigationTreeItemPlugin

    ''' <summary>One and only core reference.</summary>
    Private m_core As cCore = Nothing

#If 0 Then

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Initializes this plugin.
    ''' </summary>
    ''' <param name="core">The core this plugin is initialized for.</param>
    ''' -----------------------------------------------------------------------
    Public Sub Initialize(ByVal core As Object) _
        Implements EwEPlugin.IPlugin.Initialize

        ' Sanity check
        Debug.Assert(TypeOf core Is cCore)
        ' Store reference
        Me.m_core = DirectCast(core, cCore)

    End Sub

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Uniquely identifies this plugin as "Default".
    ''' </summary>
    ''' -----------------------------------------------------------------------
    Public ReadOnly Property Name() As String _
        Implements EwEPlugin.IPlugin.Name

        Get
            Return "Default"
        End Get

    End Property

    ''' -----------------------------------------------------------------------
    ''' <summary>
    ''' Mass balance calculation plugin point.
    ''' </summary>
    ''' <param name="EcoPathDataStructures">A reference to the Ecopath data 
    ''' structures as defined in the EwE project.</param>
    ''' <param name="EstimateFor">Enumerated value, stating the purpose the mass 
    ''' balance calculation is invoked.</param>
    ''' <param name="iResult">The result of the mass balance calculation. For 
    ''' possible values refer to the eStatusFlags enumerated type in the EwE project.
    ''' </param>
    ''' <returns>Always True to indicate that this plugin implements this plugin 
    ''' point.</returns>
    ''' -----------------------------------------------------------------------
    Public Function MassBalance(ByVal EcoPathDataStructures As Object, ByVal EstimateFor As Integer, ByRef iResult As Integer) As Boolean _
        Implements EwEPlugin.IEcopathPlugin.MassBalance

        Dim mb As New MassBalance(Me.m_core)
        iResult = mb.Calculate(EcoPathDataStructures, EstimateFor)
        Return True

    End Function

#End If

#If 0 Then

    Public ReadOnly Property ControlImage() As System.Drawing.Image Implements EwEPlugin.IGUIPlugin.ControlImage
        Get
            Return My.Resources.MenuItem1
        End Get
    End Property

    Public ReadOnly Property ControlStatusText() As String Implements EwEPlugin.IGUIPlugin.ControlStatusText
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property ControlText() As String Implements EwEPlugin.IGUIPlugin.ControlText
        Get
            ' ToDo: Localize this
            Return "Plugin item"
        End Get
    End Property

    Public Sub OnControlClick(ByVal sender As Object, ByVal e As System.EventArgs) Implements EwEPlugin.IGUIPlugin.OnControlClick
        ' Invoke 'navigate' command
        Dim cmdH As CommandHandler = CommandHandler.GetInstance()
        Dim cmd As Command = cmdH.GetCommand("navigate")
        Dim cmdNav As NavigationCommand = Nothing

        If cmd Is Nothing Then Return
        If Not (TypeOf cmd Is NavigationCommand) Then Return

        cmdNav = DirectCast(cmd, NavigationCommand)
        cmdNav.Invoke(Me.ControlText, Me.NavigationTreeItemLocation, cCoreStateMonitor.eCoreExecutionState.EcopathReady, GetType(DefaultForm))
    End Sub

    Public ReadOnly Property NavigationTreeItemLocation() As String Implements EwEPlugin.INavigationTreeItemPlugin.NavigationTreeItemLocation
        Get
            Return "ndInputData|ndBasicInput"
        End Get
    End Property

#End If

End Class
