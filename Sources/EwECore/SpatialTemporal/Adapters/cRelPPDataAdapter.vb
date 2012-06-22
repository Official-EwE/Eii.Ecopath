#Region " Imports "

Option Strict On
Imports EwEUtils.Core
Imports EwEUtils.SpatialData
Imports EwEUtils.Utilities

#End Region ' Imports

Namespace SpatialData

    ''' <summary>
    ''' Data Adapter specific to Relative PP
    ''' </summary>
    ''' <remarks>
    ''' Clears the Ecospace RelPP scaler if data is loaded
    ''' </remarks>
    Public Class cRelPPDataAdapter
        Inherits cSpatialScalarDataAdapter

#Region " Private vars "

        Private m_sPreservedScale As Double = cCore.NULL_VALUE
        Private m_spaceData As cEcospaceDataStructures

#End Region ' Private vars

#Region " Constructor "

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, ByVal cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Overrides "

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialScalarDataAdapter.Initialize"/>.
        ''' -------------------------------------------------------------------
        Friend Overrides Sub Initialize()

            MyBase.Initialize()
            Me.m_spaceData = Me.m_core.m_EcoSpaceData

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataAdapter.InitRun"/>
        ''' <remarks>
        ''' Overridden to clear the PP scale factor.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Public Overrides Sub InitRun()
            MyBase.InitRun()

            ' Reset preserved PP scale
            Me.m_sPreservedScale = cCore.NULL_VALUE

        End Sub

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialScalarDataAdapter.Adapt"/>
        ''' <remarks>
        ''' Called before data from an external source is copied into <see cref="cEcospaceDataStructures.RelPP"/>
        ''' EcoSpace uses an internal scaler to scale PP data to Ecopath levels. <see cref="cEcospaceDataStructures.PPScale"/>
        ''' This is the mean relative PP across all water cells computed from the currently loaded  <see cref="cEcospaceDataStructures.RelPP"/> array.
        ''' <see cref="cSpatialScalarDataAdapter.SetCell"/> will scale external data to a the first timestep or a user defined value.
        ''' </remarks>
        ''' -------------------------------------------------------------------
        Protected Friend Overrides Function Adapt(ByVal bm As cEcospaceBasemap, _
                                                  ByVal layer As cEcospaceLayer, _
                                                  ByVal iTime As Integer, _
                                                  ByVal dt As Date, _
                                                  ByVal dataExternal As ISpatialRaster) As Boolean

            Try
                ' Set PP scale value first time data is encountered for a run

                ' Is PP scale factor (still) clear?
                If (Me.m_sPreservedScale = cCore.NULL_VALUE) And (Me.m_spaceData.PPScale <> cCore.NULL_VALUE) Then
                    Me.m_sPreservedScale = Me.m_spaceData.PPScale
                    Me.m_spaceData.PPScale = 1.0F
                End If
            Catch ex As Exception
                System.Console.WriteLine("Exception: " & Me.ToString & ".PreAdapt() " & ex.Message)
                Return False
            End Try

            Return MyBase.Adapt(bm, layer, iTime, dt, dataExternal)

        End Function

        ''' -------------------------------------------------------------------
        ''' <inheritdocs cref="cSpatialDataAdapter.EndRun"/>
        ''' <summary>
        ''' Overridden to restore PP scale factor.
        ''' </summary>
        ''' -------------------------------------------------------------------
        Public Overrides Sub EndRun()
            MyBase.EndRun()

            ' Has preserved PP scale?
            If (Me.m_sPreservedScale <> cCore.NULL_VALUE) Then
                ' #Yes: Restore preserved PP scale
                Me.m_spaceData.PPScale = Me.m_sPreservedScale
                Me.m_sPreservedScale = cCore.NULL_VALUE
            End If

        End Sub

#End Region ' Overrides

    End Class

End Namespace



