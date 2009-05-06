'==============================================================================
'
' $Log: cEcospaceLayerRelPP.vb,v $
' Revision 1.1  2009/05/06 12:32:25  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcospaceLayerRelPP
    Inherits cEcospaceLayerSingleNxM

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerRelPP)
        Me.m_dataType = eDataTypes.EcospaceLayerRelPP
    End Sub

End Class
