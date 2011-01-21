Option Explicit On

''' <summary>
''' Manages the predator prey forcing and mediation shape interaction.
''' </summary>
''' <remarks>A pred/prey interaction can have up to five forcing or mediation shapes applied as modifiers to 
''' search rate(a), vulnerability(v), foraging arena(A) and v-A. 
''' </remarks>
Public Class cPPIManager
    Implements Collections.IEnumerable

    Private m_core As cCore
    Private m_PPIs As New Dictionary(Of String, cPredPreyInteraction)
    Private m_EPData As cEcopathDataStructures
    Private m_ESData As cEcosimDatastructures
    Private m_shapes As New List(Of cForcingFunction)

#Region "Private functions"

    Friend Function getKey(ByVal iPred As Integer, ByVal iprey As Integer) As String
        Return iPred.ToString & "_" & iprey.ToString
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
        Dim ppInter As cPredPreyInteraction

        m_PPIs.Clear()
        m_shapes.Clear()

        For ipred As Integer = 1 To m_EPData.NumGroups
            For iprey As Integer = 1 To m_EPData.NumGroups

                If m_EPData.DC(ipred, iprey) Then
                    ppInter = New cPredPreyInteraction(ipred, iprey, Me)
                    m_PPIs.Add(getKey(ipred, iprey), ppInter)
                End If

                'producers must have an interaction with themselves
                'so the user can add production forcing
                If ipred = iprey And m_EPData.PP(iprey) = 1 Then
                    ppInter = New cPredPreyInteraction(ipred, iprey, Me)
                    'make sure there is not already a key for this PP
                    'this should not happen but can if the data is wrong
                    If Not m_PPIs.ContainsKey(getKey(ipred, iprey)) Then
                        m_PPIs.Add(getKey(ipred, iprey), ppInter)
                    End If
                End If
            Next iprey
        Next ipred

        For Each shape As cForcingFunction In m_core.ForcingShapeManager
            m_shapes.Add(shape)
        Next

        For Each shape As cForcingFunction In m_core.MediationShapeManager
            m_shapes.Add(shape)
        Next

    End Function

    Public Function Load() As Boolean
        Dim keyValue As KeyValuePair(Of String, cPredPreyInteraction)
        Dim ppi As cPredPreyInteraction = Nothing

        For Each keyValue In m_PPIs
            ppi = keyValue.Value
            'tell the cPredPreyInteraction object to load from the underlying ecosim data
            ppi.Load()
        Next

    End Function

    Public Sub Clear()

        Try
            For Each ppi As cPredPreyInteraction In Me.m_PPIs.Values
                ppi.Clear()
            Next

            m_PPIs.Clear()
            m_shapes.Clear()
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

    Default ReadOnly Property Interaction(ByVal PredIndex As Integer, ByVal PreyIndex As Integer) As cPredPreyInteraction
        Get
            Try

                Dim key As String = getKey(PredIndex, PreyIndex)
                If m_PPIs.ContainsKey(key) Then
                    Return m_PPIs.Item(key)
                Else
                    Return Nothing
                End If
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".Item() Failed to find cPredPreyInteraction().")
                Return Nothing
            End Try
        End Get
    End Property


    ReadOnly Property Shapes(ByVal ShapeIndex As Integer) As cForcingFunction
        Get
            Try
                Return m_shapes.Item(ShapeIndex - 1)
            Catch ex As Exception
                Debug.Assert(False, Me.ToString & ".Shapes() Failed to find Shape.")
                Return Nothing
            End Try
        End Get
    End Property

    ''' <summary>
    ''' Get the number of shapes that are in this manager
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>This is the total number of shapes forcing and mediation</remarks>
    Public ReadOnly Property NShapes() As Integer
        Get
            Return m_shapes.Count
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
        Dim eft As eForcingFunctionApplication = 0

        ' JS 02nov07: this method can be optimized; the PPImanager can cache
        '             ff applications in an array aiFF() = iApplyCount. This will
        '             save this method from having to iterate over its internal 
        '             datastructures.
        For Each ppi As cPredPreyInteraction In Me.m_PPIs.Values
            For iShape As Integer = 1 To ppi.NAppliedShapes
                ppi.getShape(iShape, ffApplied, eft)
                If Object.ReferenceEquals(ffApplied, ffTest) Then Return True
            Next
        Next
        Return False

    End Function

#End Region

#Region "Friend functions for Pred Prey interaction objects."

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

    Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return m_shapes.GetEnumerator()
    End Function
End Class
