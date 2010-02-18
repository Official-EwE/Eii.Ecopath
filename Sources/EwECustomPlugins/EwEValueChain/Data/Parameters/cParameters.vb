'==============================================================================
'
' $Log: cParameters.vb,v $
' Revision 1.1  2009/04/13 17:41:12  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports System.ComponentModel
Imports System.Reflection

#End Region ' Imports

''' ---------------------------------------------------------------------------
''' <summary>
''' Parameters that dictate the behaviour of the Value Chain plug-in.
''' </summary>
''' ---------------------------------------------------------------------------
<Serializable()> _
Public Class cParameters
    Inherits EwEUtils.Database.cEwEDatabase.cOOPStorable

    Private m_bRunWithEcopath As Boolean = False
    Private m_bRunWithEcosim As Boolean = False
    Private m_bRunSearches As Boolean = False
    Private m_bResultsByFleet As Boolean = False
    Private m_sEffortMin As Single = 0.0!
    Private m_sEffortMax As Single = 4.0!
    Private m_sEffortInc As Single = 0.25!
    Private m_liFleets As New List(Of Integer)

#Region " Properties "

    Public ReadOnly Property EquilibriumFleetsToVary() As List(Of Integer)
        Get
            Return Me.m_liFleets
        End Get
    End Property

    Public Property EquilibriumEffortMin() As Single
        Get
            Return Me.m_sEffortMin
        End Get
        Set(ByVal value As Single)
            Me.m_sEffortMin = value
        End Set
    End Property

    Public Property EquilibriumEffortMax() As Single
        Get
            Return Me.m_sEffortMax
        End Get
        Set(ByVal value As Single)
            Me.m_sEffortMax = value
        End Set
    End Property

    Public Property EquilibriumEffortIncrement() As Single
        Get
            Return Me.m_sEffortInc
        End Get
        Set(ByVal value As Single)
            Me.m_sEffortInc = value
        End Set
    End Property

    Public Property RunWithEcopath() As Boolean
        Get
            Return Me.m_bRunWithEcopath
        End Get
        Set(ByVal bRunWithEcopath As Boolean)
            If (bRunWithEcopath <> Me.m_bRunWithEcopath) Then
                Me.m_bRunWithEcopath = bRunWithEcopath
                Me.SetChanged()
            End If
        End Set
    End Property

    Public Property RunWithEcosim() As Boolean
        Get
            Return Me.m_bRunWithEcosim
        End Get
        Set(ByVal bRunWithEcosim As Boolean)
            If (Me.m_bRunWithEcosim <> bRunWithEcosim) Then
                Me.m_bRunWithEcosim = bRunWithEcosim
                Me.SetChanged()
            End If
        End Set
    End Property

    Public Property RunWithSearches() As Boolean
        Get
            Return Me.m_bRunSearches
        End Get
        Set(ByVal bRunWithFishingPolicySearch As Boolean)
            If (Me.m_bRunSearches <> bRunWithFishingPolicySearch) Then
                Me.m_bRunSearches = bRunWithFishingPolicySearch
                Me.SetChanged()
            End If
        End Set
    End Property

    Public Property ResultsByFleet() As Boolean
        Get
            Return Me.m_bResultsByFleet
        End Get
        Set(ByVal bResultsByFleet As Boolean)
            If (Me.m_bResultsByFleet <> bResultsByFleet) Then
                Me.m_bResultsByFleet = bResultsByFleet
                Me.SetChanged()
            End If
        End Set
    End Property

#End Region ' Parameters

End Class
