'==============================================================================
'
' $Log: cDisplayGroupsCommand.vb,v $
' Revision 1.1  2009/06/06 01:34:29  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Imports EwEUtils.Commands

#End Region ' Imports

Namespace Commands

    Public Class cDisplayGroupsCommand
        Inherits cCommand

        Private m_bShowGroups As Boolean = True
        Private m_bShowTotals As Boolean = False

        Public Shared cCOMMAND_NAME As String = "~displaygroups"

        Public Sub New()
            MyBase.New(cDisplayGroupsCommand.cCOMMAND_NAME)
        End Sub

        Public Overloads Sub Invoke(Optional ByVal bShowGroups As Boolean = True, Optional ByVal bShowTotals As Boolean = False)
            Me.m_bShowGroups = bShowGroups
            Me.m_bShowTotals = bShowTotals
            MyBase.Invoke()
        End Sub

        Public Property ShowGroups() As Boolean
            Get
                Return Me.m_bShowGroups
            End Get
            Set(ByVal value As Boolean)
                Me.m_bShowGroups = value
            End Set
        End Property

        Public Property ShowTotals() As Boolean
            Get
                Return Me.m_bShowTotals
            End Get
            Set(ByVal value As Boolean)
                Me.m_bShowTotals = value
            End Set
        End Property

    End Class

End Namespace
