#Region " Imports "

Option Strict On
Imports EwEUtils.Commands
Imports EwEUtils.Core

#End Region ' Imports

Namespace Commands

    ''' ---------------------------------------------------------------------------
    ''' <summary>
    ''' Command to invoke the 'EditPedigree' interface
    ''' </summary>
    ''' ---------------------------------------------------------------------------
    Public Class cEditPedigreeCommand
        Inherits cCommand

        ''' <summary>Static name for this command.</summary>
        Public Shared cCOMMAND_NAME As String = "~EditPedigree"

        ''' <summary>Variable to invoke the command with.</summary>
        Private m_varname As eVarNameFlags = eVarNameFlags.NotSet

        Public Sub New(ByVal cmdh As cCommandHandler)
            MyBase.new(cmdh, cEditPedigreeCommand.cCOMMAND_NAME)
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <inheritdocs cref="cCommand.Invoke"/>
        ''' <param name="varname">The variable to launch the command for.</param>
        ''' ---------------------------------------------------------------------------
        Public Overloads Sub Invoke(Optional ByVal varname As eVarNameFlags = eVarNameFlags.NotSet)
            Me.m_varname = varname
            MyBase.Invoke()
        End Sub

        ''' ---------------------------------------------------------------------------
        ''' <summary>
        ''' Get/set the variable to invoke the 'Edit pedigree' interface with
        ''' </summary>
        ''' ---------------------------------------------------------------------------
        Public Property Variable() As eVarNameFlags
            Get
                Return Me.m_varname
            End Get
            Set(ByVal value As eVarNameFlags)
                Me.m_varname = value
            End Set
        End Property

    End Class

End Namespace ' Commands
