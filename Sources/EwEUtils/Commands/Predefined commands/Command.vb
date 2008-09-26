'==============================================================================
'
' $Log: Command.vb,v $
' Revision 1.1  2008/09/26 07:31:09  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.2  2008/09/23 16:17:05  jeroens
' Command updated prior to invoking
'
' Revision 1.1  2008/09/09 14:41:45  jeroens
' Moved
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' The Command class implements a generic user interface command that can be 
    ''' linked to a series of User Interface Controls. All linked Controls will
    ''' be updated whenever the Command state changes.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class Command

#Region " Construction "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of this class.
        ''' </summary>
        ''' <param name="strName">The name to assign to the command.</param>
        ''' -----------------------------------------------------------------------
        Public Sub New(ByVal strName As String, Optional ByVal objTag As Object = Nothing)
            ' Store name
            Me.m_strName = strName
            ' Store tag
            Me.m_objTag = objTag
            ' Create storage for associated controls
            Me.m_dictControls = New Dictionary(Of Object, ControlHandler)
        End Sub

#End Region ' Construction

#Region " Adding and removing GUI controls "

        ''' <summary>Controls connected to this command.</summary>
        Private m_dictControls As Dictionary(Of Object, ControlHandler)

        ''' ----------------------------------------------------------------------
        ''' <summary>
        ''' Call to add a User Interface Control to a command.
        ''' </summary>
        ''' <param name="objGUI">The control to add.</param>
        ''' <remarks>
        ''' The <see cref="CommandHandler">CommandHandler</see> predefines a few
        ''' <see cref="ControlHandler">ControlHandler</see> Types that implement
        ''' GUI behaviour for specific User Interface Control classes. Ensure
        ''' that the objGUI object has an associated ControlHandler available
        ''' otherwise the given Control will not be updated whenever the Command
        ''' state is changed.
        ''' </remarks>
        ''' ----------------------------------------------------------------------
        Public Sub AddControl(ByVal objGUI As Object)
            Try
                Dim cmdh As CommandHandler = CommandHandler.GetInstance()
                Dim t As Type = cmdh.GetControlHandlerType(objGUI)
                Dim objControlHandler As Object = Nothing
                Dim objParms() As Object = {Me, objGUI}

                If (t IsNot Nothing) Then
                    objControlHandler = Activator.CreateInstance(t, objParms)
                    If (TypeOf objControlHandler Is ControlHandler) Then
                        Me.m_dictControls.Add(objGUI, DirectCast(objControlHandler, ControlHandler))
                    End If
                End If
            Catch ex As Exception
            End Try
        End Sub

        ''' ----------------------------------------------------------------------
        ''' <summary>
        ''' Call to remove a User Interface Control from a command.
        ''' </summary>
        ''' <param name="objGUI">The control to remove.</param>
        ''' ----------------------------------------------------------------------
        Public Sub RemoveControl(ByVal objGUI As Object)
            Try
                Me.m_dictControls.Remove(objGUI)
            Catch ex As Exception
            End Try
        End Sub

#End Region ' Adding and removing GUI controls 

#Region " Execution and updating "

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event that will be called before when a Command is executed.
        ''' </summary>
        ''' <param name="cmd">The command that is invoked.</param>
        ''' -----------------------------------------------------------------------
        Public Event OnPreInvoke(ByVal cmd As Command)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event that will be called when a Command is executed.
        ''' </summary>
        ''' <param name="cmd">The command that is invoked.</param>
        ''' -----------------------------------------------------------------------
        Public Event OnInvoke(ByVal cmd As Command)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event that will be called after a Command is executed.
        ''' </summary>
        ''' <param name="cmd">The command that is invoked.</param>
        ''' -----------------------------------------------------------------------
        Public Event OnPostInvoke(ByVal cmd As Command)

        Private m_bInvoking As Boolean = False

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Base implementation for invoking a Command. Use either this implementation
        ''' or subclass the Command and implement a complex Invoke() variant.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overridable Sub Invoke()

            ' Force the command to update
            Me.Update()

            ' Command enabled?
            If Me.Enabled Then
                ' Set invoking flag
                Me.m_bInvoking = True
                ' #Yes: raise the event 
                RaiseEvent OnPreInvoke(Me)
                RaiseEvent OnInvoke(Me)
                RaiseEvent OnPostInvoke(Me)
                ' Clear invoking flag
                Me.m_bInvoking = False
                ' Update associated user controls
                Me.Update()
            End If

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Event that will be called when a command is updated.
        ''' </summary>
        ''' <param name="cmd">The command that is updated.</param>
        ''' -----------------------------------------------------------------------
        Public Event OnUpdate(ByVal cmd As Command)

        ''' <summary>Update lock flag to prevent involuntary loops.</summary>
        Private m_bLockUpdates As Boolean = False

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Base implementation for updating a Command. Use either this implementation
        ''' or subclass the Command and implement a complex Update() variant.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overridable Sub Update()
            ' Can update?
            If Not m_bLockUpdates Then
                ' #Yes: lock to prevent loops
                m_bLockUpdates = True
                ' Call for changes
                RaiseEvent OnUpdate(Me)
                ' Dispatch changes
                For Each ctrlh As ControlHandler In Me.m_dictControls.Values
                    ctrlh.Update()
                Next
                ' Unlock
                m_bLockUpdates = False
            End If
        End Sub

#End Region ' Execution and updating

#Region " Public properties "

        ''' <summary>Command enabled state.</summary>
        Private m_bEnabled As Boolean = True

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/Set the command enabled state.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Enabled() As Boolean
            Get
                Return Me.m_bEnabled
            End Get
            Set(ByVal bEnable As Boolean)
                Me.m_bEnabled = bEnable
                Me.Update()
            End Set
        End Property

        ''' <summary>Command checked state.</summary>
        Private m_bChecked As Boolean = False

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/Set the command checked state.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Checked() As Boolean
            Get
                Return Me.m_bChecked
            End Get
            Set(ByVal bCheck As Boolean)
                Me.m_bChecked = bCheck
                Me.Update()
            End Set
        End Property

        ''' <summary>Name of the command.</summary>
        Private m_strName As String = ""

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get the command name.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Name() As String
            Get
                Return Me.m_strName
            End Get
        End Property

        ''' <summary>Optional Tag attached to the command.</summary>
        Private m_objTag As Object = Nothing

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the command tag.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Property Tag() As Object
            Get
                Return Me.m_objTag
            End Get
            Set(ByVal objTag As Object)
                Me.m_objTag = objTag
            End Set
        End Property

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get whether the command is currently invoking.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property IsInvoking() As Boolean
            Get
                Return Me.m_bInvoking
            End Get
        End Property

#End Region ' Public properties

    End Class

End Namespace
