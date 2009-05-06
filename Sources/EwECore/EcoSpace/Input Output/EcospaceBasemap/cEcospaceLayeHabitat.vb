'==============================================================================
'
' $Log: cEcospaceLayeHabitat.vb,v $
' Revision 1.1  2009/05/06 12:32:24  jeroens
' Initial version
'
'==============================================================================

#Region " Imports "

Option Strict On
Imports EwEUtils.Core

#End Region ' Imports

Public Class cEcospaceLayerHabitat
    Inherits cEcospaceLayerIntegerNxM

    Public Sub New(ByVal theCore As cCore, ByVal manager As cEcospaceBasemap, ByVal meta As cVariableMetaData)
        MyBase.New(theCore, manager, EwEUtils.Core.eVarNameFlags.LayerHabitat, cCore.NULL_VALUE, meta)
        Me.m_dataType = eDataTypes.EcospaceLayerHabitat
    End Sub

End Class
