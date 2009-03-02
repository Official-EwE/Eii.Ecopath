'==============================================================================
'
' $Log: Shared UIEnums.vb,v $
' Revision 1.4  2009/03/02 17:36:16  jeroens
' Removed ridiculous enum
'
' Revision 1.3  2008/12/15 15:37:44  jeroens
' Moved shape enums from ScInt
'
' Revision 1.2  2008/09/26 00:22:50  villyc
' updating ecosimMonteCarlo to pick vulnerabilities
'
' Revision 1.1  2008/06/01 23:45:44  jeroens
' Separated from Scientific Interface
'
' Revision 1.20  2008/05/23 15:55:25  jeroens
' Separated med/ff behaviour
'
' Revision 1.19  2008/03/17 14:45:58  jeroens
' Tracer run modes enum shared
'
' Revision 1.18  2008/01/22 16:27:43  jeroens
' Renamed ApplyFFtypes
'
' Revision 1.17  2008/01/21 04:05:48  jeroens
' Enum values made descriptive
'
' Revision 1.16  2007/10/29 16:33:56  jeroens
' * Renamed const
'
' Revision 1.15  2007/10/29 13:52:29  jeroens
' * Updated
'
' Revision 1.14  2007/09/06 18:18:21  fgao
' update to apply FF to support both primary producer and consumer
'
' Revision 1.13  2007/09/06 15:37:21  jeroens
' * Time Series thumbnail rendered with Alert icon if missing PoolCode
' * Time Series thumbnail rendered as selective line
'
' Revision 1.12  2007/08/17 13:59:07  jeroens
' + Added 'dot' graph mode
'
' Revision 1.11  2007/07/27 23:45:30  fgao
' MCRun display enums.
'
' Revision 1.10  2007/07/13 00:42:24  jeroens
' * Fixed enum name
'
' Revision 1.9  2007/07/12 19:19:49  jeroens
' - Removed Forcing namespace
'
' Revision 1.8  2007/07/05 21:15:31  jeroens
' * Reworked shape enums
'
' Revision 1.7  2007/07/03 21:51:58  fgao
' Ongoing Egg Production..
'
' Revision 1.6  2007/05/18 01:51:34  jeroens
' * Generalized enums
'
'==============================================================================

Option Explicit On
Option Strict On

Namespace Definitions

    ''' <summary>
    ''' Enumerated type for identifing modifications to a list of items in the user interface, prior to
    ''' updating the list in a batch operation.
    ''' </summary>
    <CLSCompliant(True)> _
    Public Enum AddRemoveItemStatus As Integer
        ''' <summary>Item belongs to the original list.</summary>
        Original = 0
        ''' <summary>Item is flagged to be added to the list.</summary>
        Added
        ''' <summary>Item is flagged for removal from the list.</summary>
        Removed
        ''' <summary>Item does not belong to the list.</summary>
        Invalid
    End Enum

    <CLSCompliant(True)> _
    Public Enum MCRunDisplayInputValue As Integer
        B = 0
        PB
        EE
        BA
        VU
    End Enum

    ''' <summary>
    ''' Enumerated type that states how line graphs will be rendered.
    ''' </summary>
    <CLSCompliant(True)> _
    Public Enum eSketchDrawModeTypes As Integer
        ''' <summary>The area under a graph will be filled.</summary>
        Fill = 0
        ''' <summary>A graph will be rendered as a line.</summary>
        Line
        ''' <summary>A graph will be rendered as individual points.</summary>
        Dots
        ''' <summary>A graph will be rendered as a line, connecting non-zero points only.</summary>
        LineSelective
    End Enum

    ''' <summary>
    ''' Enumerated type that states how graph tick marks will be scaled.
    ''' </summary>
    <CLSCompliant(True)> _
    Public Enum eAxisTickmarkDisplayModeTypes As Integer
        ''' <summary>Tick marks will be only be displayed for the range of values on an axis.</summary>
        Relative
        ''' <summary>Tick marks will display the full (absolute) range of values on an axis.</summary>
        Absolute
    End Enum

    ''' <summary>
    ''' Enumerated type that states how the values on a graph axis will be scaled.
    ''' </summary>
    <CLSCompliant(True)> _
    Public Enum eAxisAutoScaleModeTypes As Integer
        ''' <summary>A graph will automatically scale its axis to show the range of values on an axis.</summary>
        Auto
        ''' <summary>A graph will not scale its axis to show the range of values on an axis.</summary>
        Fixed
    End Enum

    <CLSCompliant(True)> _
    Public Enum eRightClickAutoScaleModeTypes As Integer
        Auto
        Fixed
    End Enum

    <CLSCompliant(True)> _
    Public Enum eApplyTargetTypes As Integer
        NotSet = 0
        Consumer
        PrimaryProducer
    End Enum

    <CLSCompliant(True)> _
    Public Enum eApplyShapeTypes As Integer
        NotSet = 0
        Forcing
        Mediation
    End Enum

    <CLSCompliant(True)> _
    Public Enum eTracerRunModeTypes As Integer
        Disabled = 0
        RunSim
        RunSpace
    End Enum

    ''' <summary>
    ''' Enumerated type for identifying the broad categories of time shapes.
    ''' </summary>
    <CLSCompliant(True)> _
    Public Enum eShapeCategoryTypes As Integer
        NotSet
        ''' <summary>Shape is a <see cref="EwECore.cForcingFunction">Forcing shape</see>.</summary>
        Forcing
        ''' <summary>Shape is a <see cref="EwECore.cMediationFunction">Mediation shape</see>.</summary>
        Mediation
        ''' <summary>Shape is a Egg production shape.</summary>
        EggProduction
    End Enum

End Namespace

