'==============================================================================
'
' $Log: PropertySelectionCommand.vb,v $
' Revision 1.2  2009/05/11 01:51:00  jeroens
' Renamed command classes
'
' Revision 1.1  2008/09/26 07:31:21  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/01 23:45:45  jeroens
' Separated from Scientific Interface
'
' Revision 1.2  2008/05/29 22:22:51  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.1  2007/07/03 07:18:42  jeroens
' * Renamed
'
' Revision 1.2  2007/07/01 05:18:26  jeroens
' * Command distributes a list of properties
'
' Revision 1.1  2006/10/03 03:24:28  jeroens
' * Renamed from SelectionCommand
'
' Revision 1.1  2006/10/02 02:59:45  jeroens
' Initial version
'
'==============================================================================

Option Strict On

Imports EwECore
Imports EwEUtils.Commands
Imports EwEUtils.Core

Namespace Properties

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' This class implements a selection change <see cref="Command">Command</see>.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    <CLSCompliant(False)> _
    Public Class PropertySelectionCommand
        Inherits cCommand

        ''' <summary>Public available name for this command</summary>
        Public Shared COMMAND_NAME As String = "~SelectedProperties"

        ''' <summary>The properties broadcasted by this command</summary>
        Private m_lprop As New List(Of cProperty)

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes and names an instance of this class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Sub New()
            MyBase.New(COMMAND_NAME)
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cCommand.Invoke">Invokes</see> the command, broadcasting a
        ''' data selection throughout the GUI.
        ''' </summary>
        ''' <param name="source">The <see cref="cCoreInputOutputBase">cCoreInputOutput</see> 
        ''' object that was selected.</param>
        ''' <param name="varName">The <see cref="eVarNameFlags">VarName</see> of
        ''' the field that was selected.</param>
        ''' <param name="sourceSec">The <see cref="cCoreInputOutputBase">cCoreInputOutput</see> 
        ''' object that acts as secundary index to the selection.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal pm As cPropertyManager, _
                                    ByVal source As cCoreInputOutputBase, _
                                    ByVal varName As eVarNameFlags, _
                                    Optional ByVal sourceSec As cCoreInputOutputBase = Nothing)

            Dim prop As cProperty = Nothing

            If Not Object.ReferenceEquals(source, Nothing) Then
                ' Get property
                prop = pm.GetProperty(source, varName, sourceSec)
            End If

            Me.Invoke(prop)

        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cCommand.Invoke">Invokes</see> the command, broadcasting a
        ''' data selection throughout the GUI.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke()
            ' Clear list of props
            Me.m_lprop.Clear()
            ' Fire the command
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cCommand.Invoke">Invokes</see> the command, broadcasting a
        ''' data selection throughout the GUI.
        ''' </summary>
        ''' <param name="prop">The <see cref="cProperty">cProperty</see> that 
        ''' was selected.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal prop As cProperty)
            ' Clear list of props
            Me.m_lprop.Clear()
            ' Store prop
            Me.m_lprop.Add(prop)
            ' Fire the command
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cCommand.Invoke">Invokes</see> the command, broadcasting a
        ''' data selection throughout the GUI.
        ''' </summary>
        ''' <param name="aprop">Array of <see cref="cProperty">cProperty</see> 
        ''' instances that were selected.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal aprop() As cProperty)
            ' Clear list of props
            Me.m_lprop.Clear()
            ' Store prop
            Me.m_lprop.AddRange(aprop)
            ' Fire the command
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' <see cref="cCommand.Invoke">Invokes</see> the command, broadcasting a
        ''' data selection throughout the GUI.
        ''' </summary>
        ''' <param name="lprop">List of <see cref="cProperty">cProperty</see> 
        ''' instances that were selected.</param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal lprop As List(Of cProperty))
            ' Clear list of props
            Me.m_lprop.Clear()
            ' Store prop
            Me.m_lprop.AddRange(lprop)
            ' Fire the command
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Get and array of currently selected <see cref="cProperty">cProperty</see> 
        ''' instances.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Public ReadOnly Property Selection() As cProperty()
            Get
                Return Me.m_lprop.ToArray()
            End Get
        End Property

    End Class

End Namespace ' Properties
