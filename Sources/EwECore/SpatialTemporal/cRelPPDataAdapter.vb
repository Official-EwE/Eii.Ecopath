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
        ''' Called before data from an external source is copied into <see cref="cEcospaceDataStructures.RelPP"></see>
        ''' EcoSpace uses an internal scaler to scale PP data to Ecopath levels. <see cref="cEcospaceDataStructures.PPScale"></see>
        ''' This is the mean relative PP across all water cells computed from the currently loaded  <see cref="cEcospaceDataStructures.RelPP"></see> array.
        ''' <see cref="cSpatialScalarDataAdapter.SetCell"></see> will scale external data to a the first timestep or a user defined value.
        ''' </summary>
        ''' <param name="bm"></param>
        ''' <param name="layer"></param>
        ''' <param name="iTime"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function PreAdapt(ByVal bm As cEcospaceBasemap, ByVal layer As cEcospaceLayer, ByVal iTime As Integer) As Boolean
            Try
                MyBase.PreAdapt(bm, layer, iTime)
                Me.m_spaceData.PPScale = 1.0F

            Catch ex As Exception
                System.Console.WriteLine("Exception: " & Me.ToString & ".PreAdapt() " & ex.Message)
                Return False
            End Try
            Return True
        End Function

#End Region

    End Class

End Namespace



