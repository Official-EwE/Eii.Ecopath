'==============================================================================
'
' $Log: cDirectoryOpenCommand.vb,v $
' Revision 1.1  2009/05/11 01:46:57  jeroens
' Initial version
'
' Revision 1.2  2008/11/10 05:33:32  jeroens
' Renamed
'
' Revision 1.1  2008/09/09 14:42:27  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports System.Windows.Forms

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Generic command to launch an interface to select an 'open file' location.
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cDirectoryOpenCommand
        Inherits cCommand

#Region " Privates "

        ''' <summary>Dialog prompt</summary>
        Private m_strDescription As String = ""
        ''' <summary>Name of the file to open.</summary>
        Private m_strDirectory As String = ""
        ''' <summary>The dialog result.</summary>
        Private m_iResult As DialogResult = DialogResult.OK

#End Region ' Privates

#Region " Singleton "

        Private Shared __inst__ As cDirectoryOpenCommand = Nothing

        Public Shared Function GetInstance() As cDirectoryOpenCommand
            If cDirectoryOpenCommand.__inst__ Is Nothing Then
                cDirectoryOpenCommand.__inst__ = New cDirectoryOpenCommand()
            End If
            Return __inst__
        End Function

#End Region ' Singleton

        ''' -----------------------------------------------------------------------
        ''' <summary>The name of this command.</summary>
        ''' -----------------------------------------------------------------------
        Public Shared COMMAND_NAME As String = "~opendirectory"

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' Constructor, initializes a new instance of the NavigationCommand class.
        ''' </summary>
        ''' -----------------------------------------------------------------------
        Private Sub New()
            MyBase.New(COMMAND_NAME)
        End Sub

        Public Overrides Sub Invoke()
            Me.m_iResult = DialogResult.Cancel
            MyBase.Invoke()
        End Sub

        ''' -----------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <param name="strDirectory"></param>
        ''' -----------------------------------------------------------------------
        Public Overloads Sub Invoke(ByVal strDirectory As String, ByVal strDescription As String)

            Me.m_strDescription = strDescription
            Me.m_strDirectory = strDirectory
            Me.Invoke()

        End Sub

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the prompt to display in the dialog.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Description() As String
            Get
                Return Me.m_strDescription
            End Get
            Set(ByVal strDescription As String)
                Me.m_strDescription = strDescription
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the directory that the command was invoked with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Directory() As String
            Get
                Return Me.m_strDirectory
            End Get
            Set(ByVal strDirectory As String)
                Me.m_strDirectory = strDirectory
            End Set
        End Property

        ''' -------------------------------------------------------------------
        ''' <summary>
        ''' The result that the dialog closed with.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Property Result() As DialogResult
            Get
                Return Me.m_iResult
            End Get
            Set(ByVal value As DialogResult)
                Me.m_iResult = value
            End Set
        End Property

    End Class

End Namespace
