'==============================================================================
'
' $Log: Shared UIEnums.vb,v $
' Revision 1.2  2008/10/02 17:05:26  villyc
' mc ecobio updates
'
' Revision 1.1  2008/09/26 07:31:20  sherman
' --== DELETED HISTORY ==--
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

End Namespace

