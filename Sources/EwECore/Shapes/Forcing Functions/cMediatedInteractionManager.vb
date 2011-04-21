Option Strict On
Option Explicit On

Imports EwEUtils.Core

''' <summary>
''' Manages all mediated shape interactions.
''' </summary>
Public Class cMediatedInteractionManager

    Private m_core As cCore
    Private m_interactionsPredPrey As New Dictionary(Of String, cPredPreyInteraction)
    Private m_interactionsLandings As New Dictionary(Of String, cLandingsInteraction)
    Private m_EPData As cEcopathDataStructures
    Private m_ESData As cEcosimDatastructures

#Region "Private functions"

    Friend Function getHashKey(ByVal iIndex1 As Integer, ByVal iIndex2 As Integer) As String
        Return iIndex1.ToString & "_" & iIndex2.ToString
    End Function

#End Region

#Region "Construction and Initialization"

    Friend Sub New(ByRef EcoPathData As cEcopathDataStructures, ByRef EcoSimData As cEcosimDatastructures, ByRef theCore As cCore)

        m_EPData = EcoPathData
        m_ESData = EcoSimData
        m_core = theCore

        Init()
        Load()

    End Sub

    Public Function Init() As Boolean

        m_interactionsPredPrey.Clear()
        m_interactionsLandings.Clear()

        For ipred As Integer = 1 To m_EPData.NumGroups
            For iprey As Integer = 1 To m_EPData.NumGroups

                If Me.isPredPrey(ipred, iprey) Then
                    Dim interaction As New cPredPreyInteraction(ipred, iprey, Me)
                    Me.m_interactionsPredPrey.Add(getHashKey(ipred, iprey), interaction)
                End If

            Next iprey
        Next ipred

        For iFleet As Integer = 1 To m_EPData.NumFleet
            For iGroup As Integer = 1 To Me.m_EPData.NumGroups

                If Me.isLandings(iFleet, iGroup) Then
                    Dim interaction As New cLandingsInteraction(iFleet, iGroup, Me)
                    Me.m_interactionsLandings.Add(getHashKey(iFleet, iGroup), interaction)
                End If

            Next
        Next

    End Function

    Public Function Load() As Boolean
        For Each interaction As cMediatedInteraction In Me.m_interactionsPredPrey.Values
            interaction.Load()
        Next
        For Each interaction As cMediatedInteraction In Me.m_interactionsLandings.Values
            interaction.Load()
        Next
    End Function

    Public Sub Clear()

        Try
            For Each interaction As cMediatedInteraction In Me.m_interactionsPredPrey.Values
                interaction.Clear()
            Next
            For Each interaction As cMediatedInteraction In Me.m_interactionsLandings.Values
                interaction.Clear()
            Next

            m_interactionsPredPrey.Clear()
            m_interactionsLandings.Clear()

        Catch ex As Exception
            cLog.Write(ex)
        End Try

    End Sub

#End Region

#Region "Public Properties"

    Public ReadOnly Property isPredPrey(ByVal PredIndex As Integer, ByVal PreyIndex As Integer) As Boolean
        Get
            Try

                'True for primary producer pairs
                If PredIndex = PreyIndex And m_EPData.PP(PreyIndex) = 1 Then
                    Return True
                End If

                If m_EPData.DC(PredIndex, PreyIndex) > 0 Then
                    Return True
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            End Try

        End Get
    End Property

    Public ReadOnly Property PredPreyInteraction(ByVal PredIndex As Integer, ByVal PreyIndex As Integer) As cPredPreyInteraction
        Get
            Try
                Dim key As String = getHashKey(PredIndex, PreyIndex)
                If m_interactionsPredPrey.ContainsKey(key) Then
                    Return Me.m_interactionsPredPrey.Item(key)
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".Item() Failed to find cPredPreyInteraction().")
                Return Nothing
            End Try
        End Get
    End Property

    Public ReadOnly Property isLandings(ByVal iFleet As Integer, ByVal iGroup As Integer) As Boolean
        Get
            Try
                Return (Me.m_EPData.Landing(iFleet, iGroup) > 0)
            Catch ex As Exception
                Return False
            End Try
        End Get
    End Property

    Public ReadOnly Property LandingInteraction(ByVal iFleet As Integer, ByVal iGroup As Integer) As cLandingsInteraction
        Get
            Try
                Dim key As String = getHashKey(iFleet, iGroup)
                If m_interactionsLandings.ContainsKey(key) Then
                    Return Me.m_interactionsLandings.Item(key)
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".Item() Failed to find cPriceElasticityInteraction().")
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' Get the maximum number of shapes that can be assigned to an interaction.
    ''' </summary>
    Public ReadOnly Property MaxNShapes() As Integer
        Get
            Return Me.getEcoSimData.MaxFunctions
        End Get
    End Property

    ''' <summary>
    ''' Get whether a given forcing function has been applied at least once.
    ''' </summary>
    ''' <param name="ffTest">Forcing Function to test.</param>
    ''' <returns>True if the given Forcing Function is applied at least once.</returns>
    Public Function IsApplied(ByVal ffTest As cForcingFunction) As Boolean

        Dim bIsApplied As Boolean = False
        Dim ffApplied As cForcingFunction = Nothing
        Dim eft As eForcingFunctionApplication = eForcingFunctionApplication.NotSet

        ' JS 02nov07: this method can be optimized; the PPImanager can cache
        '             ff applications in an array aiFF() = iApplyCount. This will
        '             save this method from having to iterate over its internal 
        '             datastructures.
        For Each interaction As cMediatedInteraction In Me.m_interactionsPredPrey.Values
            For iShape As Integer = 1 To interaction.NAppliedShapes
                interaction.getShape(iShape, ffApplied, eft)
                If Object.ReferenceEquals(ffApplied, ffTest) Then Return True
            Next
        Next
        Return False

    End Function

#End Region

#Region "Friend functions "

    Friend Function getEcoPathData() As cEcopathDataStructures
        Return m_EPData
    End Function

    Friend Function getEcoSimData() As cEcosimDatastructures
        Return m_ESData
    End Function

    Friend Function getCore() As cCore
        Return m_core
    End Function

#End Region

End Class
