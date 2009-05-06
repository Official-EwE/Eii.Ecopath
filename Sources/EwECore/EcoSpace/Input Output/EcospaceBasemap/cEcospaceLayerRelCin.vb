'==============================================================================
'
' $Log: cEcospaceLayerRelCin.vb,v $
' Revision 1.1  2009/05/06 12:32:25  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcospaceLayerRelCin
    Inherits cEcospaceLayerSingleNxM

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerRelCin, cCore.NULL_VALUE)
        Me.m_dataType = eDataTypes.EcospaceLayerRelCin
    End Sub

End Class
