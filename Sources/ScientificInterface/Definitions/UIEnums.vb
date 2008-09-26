'==============================================================================
'
' $Log: UIEnums.vb,v $
' Revision 1.1  2008/09/26 07:31:27  sherman
' --== DELETED HISTORY ==--
'
' Revision 1.1  2008/06/02 00:01:48  jeroens
' Added ScientificInterfaceShared
'
'==============================================================================

Option Strict On

''' <summary>
''' Enumerated type that states how line graphs will be rendered.
''' </summary>
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
''' Enumerated type that states whether a graph will display its axes.
''' </summary>
Public Enum eAxisDisplayModeTypes As Integer
    ''' <summary>Axes will be displayed.</summary>
    Show
    ''' <summary>Axes will not be displayed.</summary>
    Hide
End Enum

''' <summary>
''' Enumerated type that states how graph tick marks will be scaled.
''' </summary>
Public Enum eAxisTickmarkDisplayModeTypes As Integer
    ''' <summary>Tick marks will be only be displayed for the range of values on an axis.</summary>
    Relative
    ''' <summary>Tick marks will display the full (absolute) range of values on an axis.</summary>
    Absolute
End Enum

''' <summary>
''' Enumerated type that states how the values on a graph axis will be scaled.
''' </summary>
Public Enum eAxisAutoScaleModeTypes As Integer
    ''' <summary>A graph will automatically scale its axis to show the range of values on an axis.</summary>
    Auto
    ''' <summary>A graph will not scale its axis to show the range of values on an axis.</summary>
    Fixed
End Enum

Public Enum eRightClickAutoScaleModeTypes As Integer
    Auto
    Fixed
End Enum

Public Enum eApplyTargetTypes As Integer
    NotSet = 0
    Consumer
    PrimaryProducer
End Enum

Public Enum eApplyShapeTypes As Integer
    NotSet = 0
    Forcing
    Mediation
End Enum

Public Enum eTracerRunModeTypes As Integer
    Disabled = 0
    RunSim
    RunSpace
End Enum

''' <summary>
''' Enumerated type for identifying the broad categories of time shapes.
''' </summary>
Public Enum eShapeCategoryTypes As Integer
    NotSet
    ''' <summary>Shape is a <see cref="EwECore.cForcingFunction">Forcing shape</see>.</summary>
    Forcing
    ''' <summary>Shape is a <see cref="EwECore.cMediationFunction">Mediation shape</see>.</summary>
    Mediation
    ''' <summary>Shape is a Egg production shape.</summary>
    EggProduction
End Enum
