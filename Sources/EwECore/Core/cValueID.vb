'==============================================================================
'
' $Log: cValueID.vb,v $
' Revision 1.1  2008/09/26 07:30:13  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.11  2008/05/29 22:22:48  jeroens
' Moved eVarNameFlags to EwEUtils
'
' Revision 1.10  2007/07/07 00:20:53  jeroens
' ! Profiled
'
' Revision 1.9  2006/10/01 02:27:23  jeroens
' * Fixed comment bug
'
' Revision 1.8  2006/07/12 16:34:46  jeroens
' - Reverted silly property secundary index ID. Secundary object was solid and that will stay, other interfaces just muddle the design.
'
' Revision 1.7  2006/07/10 18:43:32  jeroens
' + Some values require an Index number rather than an object. This is true for for instance Fleet.FixedCost.
'
' Revision 1.6  2006/06/13 08:29:01  cvsuser
' * Secundary index now an object, not an integer. The secundary index object will resolve its iGroup at runtime to allow for dynamic object creation and destruction
'
' Revision 1.5  2006/06/06 14:14:23  jeroens
' + Added ICoreInterface
'
' Revision 1.4  2006/05/29 11:59:45  jeroens
' * If not specified, var name NAME is used by default
'
' Revision 1.3  2006/05/03 14:29:21  cvsuser
' + Fixed ambiguity when generating IDs for objects in absentia
'
' Revision 1.2  2006/05/03 13:44:09  cvsuser
' + Added comments
' + Added regions to create some order in the chaos
'
' Revision 1.1  2006/05/03 13:31:33  cvsuser
' + Moved to here (was GUI class PropertyIDGenerator)
'
'==============================================================================

Option Strict On
Imports EwEUtils.Core

''' -----------------------------------------------------------------------
''' <summary>
''' <para>Helper class; assists in the generation of an unique IDs for EwE values.</para>
''' <para>This ID is required to be able to associate EwE values with arbitrary data
''' such as remarks and references</para>
''' </summary>
''' -----------------------------------------------------------------------
Public Class cValueID

#Region " For non-core objects "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Computes a value ID for a given set of parameters that functionally identify the value.
    ''' </summary>
    ''' <param name="strInstance"></param>
    ''' <param name="strVariableName"></param>
    ''' <param name="strIndex"></param>
    ''' <returns>A computed ID</returns>
    ''' -------------------------------------------------------------------
    Public Shared Function Generate(ByVal strInstance As String, Optional ByVal strVariableName As String = "", Optional ByVal strIndex As String = "") As String
        Dim strID As String = String.Empty

        If (strInstance <> String.Empty) Then strID = strInstance Else strID = "<unknown>"
        If (strVariableName <> String.Empty) Then strID = strID & ("-" & strVariableName)
        ' JS 01Mar06: nitpicking - variable part and iGroup part now distinguishable via
        '             different separators
        If (Not String.IsNullOrEmpty(strIndex)) Then strID = strID & ("(" & strIndex & ")")

        Return strID
    End Function

#End Region ' For non-core objects

#Region " For core object instances "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Computes a value ID for a given set of parameters that functionally identify the value.
    ''' </summary>
    ''' <param name="obj">The ICoreInterface derived class that contains or defines the value</param>
    ''' <param name="eVariable">The <see cref="eVarNameFlags">Core variable name</see> that
    ''' defines the value.</param>
    ''' <param name="objSec">The ICoreInterface derived class that serves as an index on
    ''' the variable indicated by <paramref name="eVariable">eVariable</paramref>.</param>
    ''' <returns>A computed ID</returns>
    ''' -------------------------------------------------------------------
    Public Shared Function Generate(ByVal obj As EwECore.ICoreInterface, _
            Optional ByVal eVariable As eVarNameFlags = eVarNameFlags.Name, _
            Optional ByVal objSec As ICoreInterface = Nothing) As String
        Dim strSource As String = obj.GetID()
        Dim strVarName As String = cCoreEnumNamesIndex.GetInstance().GetVarName(eVariable)
        Dim strIndex As String = ""
        If (objSec IsNot Nothing) Then strIndex = objSec.GetID()
        Return cValueID.Generate(strSource, strVarName, strIndex)
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Computes a value ID for a given set of parameters for variables which names cannot be deducted by the core
    ''' </summary>
    ''' <param name="obj">The ICoreInoutOutout derived class that contains or defines the value</param>
    ''' <param name="strCustomVariableName">An arbitrary variable that defines the value, or "" if this filter does not apply.</param>
    ''' <param name="objSec">The ICoreInterface derived class that serves as an index on
    ''' the variable indicated by <paramref name="strCustomVariableName">strCustomVariableName</paramref>.</param>
    ''' <returns>A computed ID</returns>
    ''' -------------------------------------------------------------------
    Public Shared Function Generate(ByVal obj As EwECore.ICoreInterface, _
            ByVal strCustomVariableName As String, _
            Optional ByVal objSec As ICoreInterface = Nothing) As String
        Dim strIndex As String = ""
        If (objSec IsNot Nothing) Then strIndex = objSec.getID()
        Return cValueID.Generate(obj.getID(), strCustomVariableName, strIndex)
    End Function

#End Region ' For full-fledged core objects

#Region " For core object definitions "

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Generates an ID for a core data type and ID
    ''' </summary>
    ''' <param name="dataType">The <see cref="eDataTypes">Core data type</see> representing the object to generate the ID for</param>
    ''' <param name="nID">The unique ID of the object to generate the ID for</param>
    ''' <returns>A computer ID</returns>
    ''' -------------------------------------------------------------------
    Public Shared Function getDataTypeID(ByVal dataType As eDataTypes, ByVal nID As Integer) As String
        ' JS 06jul07: profiled, m3 runs the the fastest
        'Return String.Concat(cCoreEnumNamesIndex.GetInstance().GetDataTypeName(dataType), CChar("_"), CStr(nID)) ' m1
        'Return cCoreEnumNamesIndex.GetInstance().GetDataTypeName(dataType) & CChar("_") & CStr(nID)              ' m2
        Return cCoreEnumNamesIndex.GetInstance().GetDataTypeName(dataType) & "_" & nID                            ' m3
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Computes a value ID for a core data object <i>in absentia</i>
    ''' </summary>
    ''' <param name="dataType">The <see cref="eDataTypes">Core data type</see> representing the object to generate the ID for</param>
    ''' <param name="nID">The unique ID of the object to generate the ID for</param>
    ''' <param name="eVariable">The <see cref="eVarNameFlags">Core variable name</see> that
    ''' defines the value, or "" if this filter does not apply.</param>
    ''' <param name="dataTypeSec">The <see cref="eDataTypes">Core data type</see> representing the secundary object to generate the ID for</param>
    ''' <param name="nIDSec">The unique ID of the secundary object to generate the ID for</param>
    ''' <returns>A computed ID</returns>
    ''' -------------------------------------------------------------------
    Public Shared Function GenerateAbstract(ByVal dataType As eDataTypes, ByVal nID As Integer, _
            Optional ByVal eVariable As eVarNameFlags = eVarNameFlags.Name, _
            Optional ByVal dataTypeSec As eDataTypes = eDataTypes.NotSet, Optional ByVal nIDSec As Integer = -1) As String
        Dim strSource As String = cValueID.getDataTypeID(dataType, nID)
        Dim strVarName As String = cCoreEnumNamesIndex.GetInstance().GetVarName(eVariable)
        Dim strIndex As String = ""

        If (dataTypeSec <> eDataTypes.NotSet And nIDSec >= 0) Then strIndex = cValueID.getDataTypeID(dataTypeSec, nIDSec)

        Return cValueID.Generate(strSource, strVarName, strIndex)
    End Function

    ''' -------------------------------------------------------------------
    ''' <summary>
    ''' Computes a value ID for a core data object <i>in absentia</i>
    ''' </summary>
    ''' <param name="dataType">The <see cref="eDataTypes">Core data type</see> representing the object to generate the ID for</param>
    ''' <param name="nID">The unique ID of the object to generate the ID for</param>
    ''' <param name="strCustomVariableName">An arbitrary variable that defines the value, or "" if this filter does not apply.</param>
    ''' <returns>A computed ID</returns>
    ''' -------------------------------------------------------------------
    Public Shared Function GenerateAbstract(ByVal dataType As eDataTypes, ByVal nID As Integer, _
            ByVal strCustomVariableName As String, _
            Optional ByVal dataTypeSec As eDataTypes = eDataTypes.NotSet, Optional ByVal nIDSec As Integer = -1) As String
        Dim strSource As String = cValueID.getDataTypeID(dataType, nID)
        Dim strIndex As String = ""

        If (dataTypeSec <> eDataTypes.NotSet And nIDSec >= 0) Then strIndex = cValueID.getDataTypeID(dataTypeSec, nIDSec)

        Return cValueID.Generate(strSource, strCustomVariableName, strIndex)
    End Function

#End Region ' For core object definitions

End Class
