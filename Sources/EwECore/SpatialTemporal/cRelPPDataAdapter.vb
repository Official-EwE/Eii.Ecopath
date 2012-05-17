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

        Private m_sPreservedScale As Single = cCore.NULL_VALUE
        Private m_spaceData As cEcospaceDataStructures

#Region " Constructor "

        Public Sub New(ByVal core As cCore, ByVal varName As eVarNameFlags, ByVal cc As eCoreCounterTypes)
            MyBase.New(core, varName, cc)
        End Sub

#End Region ' Constructor

#Region " Overrides "

        Friend Overrides Sub Initialize()

            MyBase.Initialize()
            Me.m_spaceData = Me.m_core.m_EcoSpaceData

        End Sub

        ''' <summary>
        ''' Called before data from an external source is copied into <see cref="cEcospaceDataStructures.RelPP"/>
        ''' EcoSpace uses an internal scaler to scale PP data to Ecopath levels. <see cref="cEcospaceDataStructures.PPScale"/>
        ''' This is the mean relative PP across all water cells computed from the currently loaded  <see cref="cEcospaceDataStructures.RelPP"/> array.
        ''' <see cref="cSpatialScalarDataAdapter.SetCell"/> will scale external data to a the first timestep or a user defined value.
        ''' </summary>
        ''' <param name="bm"></param>
        ''' <param name="layer"></param>
        ''' <param name="iTime"></param>
        ''' <returns></returns>
        Protected Overrides Function PreAdapt(ByVal bm As cEcospaceBasemap, ByVal layer As cEcospaceLayer, ByVal iTime As Integer, dt As Date) As Boolean
            Try
                If MyBase.PreAdapt(bm, layer, iTime, dt) Then
                    Me.m_sPreservedScale = Me.m_spaceData.PPScale
                    Me.m_spaceData.PPScale = 1.0F
                End If

            Catch ex As Exception
                System.Console.WriteLine("Exception: " & Me.ToString & ".PreAdapt() " & ex.Message)
                Return False
            End Try
            Return True
        End Function

        ''' <inheritdocs cref="cSpatialDataAdapter.EndRun"/>
        ''' <summary>
        ''' Overridden to initialize PP scale factor.
        ''' </summary>
        Public Overrides Sub InitRun()
            MyBase.InitRun()

            ' Reset preserved PP scale
            Me.m_sPreservedScale = cCore.NULL_VALUE

        End Sub

        ''' <inheritdocs cref="cSpatialDataAdapter.EndRun"/>
        ''' <summary>
        ''' Overridden to restore PP scale factor.
        ''' </summary>
        Public Overrides Sub EndRun()
            MyBase.EndRun()

            ' Has preserved PP scale?
            If (Me.m_sPreservedScale <> cCore.NULL_VALUE) Then
                ' #Yes: Restore preserved PP scale
                Me.m_spaceData.PPScale = Me.m_sPreservedScale
                ' Not really necessary, but ok.
                Me.m_sPreservedScale = cCore.NULL_VALUE
            End If

        End Sub

#End Region

    End Class

End Namespace



