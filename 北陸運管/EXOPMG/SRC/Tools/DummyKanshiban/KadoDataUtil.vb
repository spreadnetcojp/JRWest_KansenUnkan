' **********************************************************************
'   ƒVƒXƒeƒ€–¼FVŠ²üŽ©“®‰üŽDƒVƒXƒeƒ€i‰^—pŠÇ—ƒT[ƒo^’[––j
'
'   Copyright Toshiba Solutions Corporation 2017 All rights reserved.
'
' ----------------------------------------------------------------------
'   •ÏX—š—ð:
'   Ver      “ú•t        ’S“–       ƒRƒƒ“ƒg
'   0.0      2017/11/21  (NES)¬—Ñ  V‹Kì¬
' **********************************************************************
Option Explicit On
Option Strict On

Imports System.IO
Imports System.Text

Imports JR.ExOpmg.Common

Public Class KadoDataUtil

    Private Class FieldRef
        Public Field As XlsField
        Public BitOffset As Integer
        Public Index As Integer

        Public Sub New(ByVal oField As XlsField, ByVal bitOfs As Integer, ByVal i As Integer)
            Field = oField
            BitOffset = bitOfs
            Index = i
        End Sub
    End Class

    Private Shared oFieldRefs(1) As Dictionary(Of String, FieldRef)
    Private Shared totalBitCount(1) As Integer

    Private Const AggregateFieldsOrigin As Integer = 15
    Private Shared ReadOnly oFields As XlsField()() = { _
        New XlsField() { _
            New XlsField(8*1, "X2", 1, " "c, "Šî–{ƒwƒbƒ_[ ƒf[ƒ^Ží•Ê", "DataKind"), _
            New XlsField(8*1, "D3", 2, "-"c, "Šî–{ƒwƒbƒ_[ ‰wƒR[ƒh", "Station"), _
            New XlsField(8*7, "X14", 1, " "c, "Šî–{ƒwƒbƒ_[ ˆ—“úŽž"), _
            New XlsField(8*1, "D", 1, " "c, "Šî–{ƒwƒbƒ_[ ƒR[ƒi["), _
            New XlsField(8*1, "D", 1, " "c, "Šî–{ƒwƒbƒ_[ †‹@"), _
            New XlsField(8*4, "D", 1, " "c, "Šî–{ƒwƒbƒ_[ ƒV[ƒPƒ“ƒXNo", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*1, "X2", 1, " "c, "Šî–{ƒwƒbƒ_[ ƒo[ƒWƒ‡ƒ“"), _
            New XlsField(8*7, "X14", 1, " "c, "‹¤’Ê•” WŒvŠJŽn“úŽž"), _
            New XlsField(8*7, "X14", 1, " "c, "‹¤’Ê•” WŒvI—¹(ŽûW)“úŽž"), _
            New XlsField(8*7, "X14", 1, " "c, "‹¤’Ê•” ‰üŽD‘¤”À‘—•”“_ŒŸ“úŽž"), _
            New XlsField(8*7, "X14", 1, " "c, "‹¤’Ê•” WŽD‘¤”À‘—•”“_ŒŸ“úŽž"), _
            New XlsField(8*8, "X16", 1, " "c, "‹¤’Ê•” ‰üŽD‘¤”À‘—•””Ô†"), _
            New XlsField(8*8, "X16", 1, " "c, "‹¤’Ê•” WŽD‘¤”À‘—•””Ô†"), _
            New XlsField(8*1, "D", 48, " "c, "‹¤’Ê•” ‰üŽD‘¤ŒŸ’mƒZƒ“ƒTƒŒƒxƒ‹"), _
            New XlsField(8*1, "D", 48, " "c, "‹¤’Ê•” WŽD‘¤ŒŸ’mƒZƒ“ƒTƒŒƒxƒ‹"), _
            New XlsField(8*1, "X2", 48, " "c, "‹¤’Ê•” —\”õ"), _
            New XlsField(8*4, "D", 1, " "c, "WŒv001 ‰ü(‚`)‘“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv002 ‰ü(‚`)‘“Š“ü–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv003 ‰ü(‚`)‚P–‡“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv004 ‰ü(‚`)‚Q–‡“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv005 ‰ü(‚`)‚R–‡“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv006 ‰ü(‚`)‚S–‡“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv007 ‰ü(‚`)‚T–‡ˆÈã“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv008 ‰ü(‚`)ˆêŠ‡“Š“üŒ”i‚Q–‡j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv009 ‰ü(‚`)ˆêŠ‡“Š“üŒ”i‚R–‡j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv010 ‰ü(‚`)ˆêŠ‡“Š“üŒ”i‚S–‡j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv011 ‰ü(‚`)ˆêŠ‡“Š“üŒ”i‚T–‡ˆÈãj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv012 ‰ü(‚`)‘S–‡”•\“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv013 ‰ü(‚`)‘S–‡”— “Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv014 ‰ü(‚`)— •\¬‡“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv015 ‰ü(‚`)•\“Š“ü–‡”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv016 ‰ü(‚`)•\“Š“ü–‡”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv017 ‰ü(‚`)•\“Š“ü–‡”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv018 ‰ü(‚`)•\“Š“ü–‡”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv019 ‰ü(‚`)— “Š“ü–‡”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv020 ‰ü(‚`)— “Š“ü–‡”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv021 ‰ü(‚`)— “Š“ü–‡”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv022 ‰ü(‚`)— “Š“ü–‡”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv023 ‰ü(‚`)Œ””»’è‚n‚jŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv024 ‰ü(‚`)Œ””»’è‚n‚j–‡”i‡Œvj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv025 ‰ü(‚`)Œ””»’è‚n‚j–‡”i‚P–‡“Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv026 ‰ü(‚`)Œ””»’è‚n‚j–‡”i‚Q–‡“Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv027 ‰ü(‚`)Œ””»’è‚n‚j–‡”i‚R–‡“Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv028 ‰ü(‚`)Œ””»’è‚n‚j–‡”i‚S–‡“Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv029 ‰ü(‚`)Œ””»’è‚n‚j–‡”iNRZ´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv030 ‰ü(‚`)Œ””»’è‚n‚j–‡”iFM´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv031 ‰ü(‚`)Œ””»’è‚n‚j–‡”iNRZ’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv032 ‰ü(‚`)Œ””»’è‚n‚j–‡”iFM’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv033 ‰ü(‚`)Œ””»’è‚n‚j–‡”iFM‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv034 ‰ü(‚`)Œ””»’è‚n‚j–‡”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv035 ‰ü(‚`)”»’è‘ÎÛŠOŒ”“Š“ü–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv036 ‰ü(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”iæŽÔŒ”“Š“ü‘Ò‚¿j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv037 ‰ü(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”i“Á‹}Œ”“Š“ü‘Ò‚¿j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv038 ‰ü(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”i“–‰w–˜Œ”“Š“ü‘Ò‚¿j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv039 ‰ü(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”iæŽÔŒ”+“–‰w–˜Œ”“Š“ü‘Ò‚¿j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv040 ‰ü(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”i“Á‹}Œ”+“–‰w–˜Œ”“Š“ü‘Ò‚¿j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv041 ‰ü(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”iæŽÔŒ”+“Á‹}Œ”“Š“ü‘Ò‚¿j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv042 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv043 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv044 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv045 ‰ü(‚`)‚»‚Ì‘¼‚h‚bˆ—Žó•t–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv046 ‰ü(‚`)‚h‚b‚P–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv047 ‰ü(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”iÝ—ˆ‚h‚bj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv048 ‰ü(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”iVŠ²üê—pŒ”“–‰w–˜Œ”‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv049 ‰ü(‚`)‚²—˜—p•[”­Œ”–‡”i—ÝŒvj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv050 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv051 ‰ü(‚`)Œ””»’è‚m‚fŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv052 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fi•\“Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv053 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fi— “Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv054 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fiÊßØÃ¨´×°F´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv055 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fiÊßØÃ¨´×°F’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv056 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fiÌ«°Ï¯Ä´×°F´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv057 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fiÌ«°Ï¯Ä´×°F’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv058 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fiÌ«°Ï¯Ä´×°F‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv059 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fiÌ«°Ï¯Ä´×°F‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv060 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fi“ñd‰»´×°j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv061 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fi»ÑÁª¯¸´×°F´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv062 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fi»ÑÁª¯¸´×°F’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv063 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fi»ÑÁª¯¸´×°F‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv064 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fi»ÑÁª¯¸´×°F‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv065 ‰ü(‚`)ˆÙíŒ””»’è‚m‚fi”ñŽ¥‹C‰»Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv066 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv067 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv068 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fiŒ”Ží”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv069 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fi‘ålŒ”¬Ž™Œ”¬Ý”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv070 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fiŠúŠÔ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv071 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fi‹æŠÔ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv072 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fi“üêŒ”ŽžŠÔ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv073 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fiI—ñŽÔ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv074 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fiŽg—pÏ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv075 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv076 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fi•¡æ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv077 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fi•¡”–‡—LŒø”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv078 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fiŽg—pŠJŽnŒã”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv079 ‰ü(‚`)–³ŒøŒ””»’è‚m‚fi“Š“ü–‡””»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv080 ‰ü(‚`)—LŒø‘g‡‚¹”»’è‚m‚fiæŽÔŒ”“Š“ü‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv081 ‰ü(‚`)—LŒø‘g‡‚¹”»’è‚m‚fi“Á‹}Œ”“Š“ü‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv082 ‰ü(‚`)—LŒø‘g‡‚¹”»’è‚m‚fi“–‰w–˜Œ”“Š“ü‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv083 ‰ü(‚`)—LŒø‘g‡‚¹”»’è‚m‚fiæŽÔŒ”E“–‰w–˜Œ”“Š“ü‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv084 ‰ü(‚`)—LŒø‘g‡‚¹”»’è‚m‚fi“Á‹}Œ”E“–‰w–˜Œ”“Š“ü‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv085 ‰ü(‚`)—LŒø‘g‡‚¹”»’è‚m‚fiæŽÔŒ”E“Á‹}Œ”“Š“ü‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv086 ‰ü(‚`)—LŒø‘g‡‚¹”»’è‚m‚fi‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv087 ‰ü(‚`)‘g‡‚¹”»’è‚m‚fiæŽÔŒ”¥“Á‹}Œ”‹æŠÔ”äŠr”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv088 ‰ü(‚`)‘g‡‚¹ˆÙíiVŠ²üê—pŒ”“–‰w–˜Œ”‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv089 ‰ü(‚`)‘g‡‚¹”»’è‚m‚fiÚ‘±”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv090 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv091 ‰ü(‚`)‘g‡‚¹”»’è‚m‚fi•¹—p”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv092 ‰ü(‚`)Ý—ˆIC{VŠ²üŽ¥‹C‚R–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv093 ‰ü(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”i‚d‚w‚h‚bA(Š²)’èŠúŒ”(IC)j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv094 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv095 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv096 ‰ü(‚`)•s³”»’è‚m‚fi•¡”‰ñŽg—pˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv097 ‰ü(‚`)‚h‚b‚Q–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv098 ‰ü(‚`)‚»‚Ì‘¼‚m‚fi’x•¥‚¢”»’è•s‰Âj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv099 ‰ü(‚`)‚h‚cƒ`ƒFƒbƒN”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv100 ‰ü(‚`)‘‚h‚bŽ¥‹C•¹—pŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv101 ‰ü(‚`)Ž¥‹C‘žŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv102 ‰ü(‚`)Ž¥‹C‘žŒ”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv103 ‰ü(‚`)Ž¥‹C‘žŒ”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv104 ‰ü(‚`)Ž¥‹C‘žŒ”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv105 ‰ü(‚`)Ž¥‹C‘žŒ”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv106 ‰ü(‚`)Ž¥‹C‘žØÄ×²Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv107 ‰ü(‚`)Ž¥‹C‘žØÄ×²‰ñ”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv108 ‰ü(‚`)Ž¥‹C‘žØÄ×²‰ñ”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv109 ‰ü(‚`)Ž¥‹C‘žØÄ×²‰ñ”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv110 ‰ü(‚`)Ž¥‹C‘žØÄ×²‰ñ”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv111 ‰ü(‚`)Ž¥‹C‘žØÄ×²¨‚n‚j‰ñ”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv112 ‰ü(‚`)Ž¥‹C‘žØÄ×²¨‚n‚j‰ñ”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv113 ‰ü(‚`)Ž¥‹C‘žØÄ×²¨‚n‚j‰ñ”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv114 ‰ü(‚`)Ž¥‹C‘žØÄ×²¨‚n‚j‰ñ”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv115 ‰ü(‚`)Ž¥‹C‘žØÄ×²¨‚m‚f‰ñ”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv116 ‰ü(‚`)Ž¥‹C‘žØÄ×²¨‚m‚f‰ñ”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv117 ‰ü(‚`)Ž¥‹C‘žØÄ×²¨‚m‚f‰ñ”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv118 ‰ü(‚`)Ž¥‹C‘žØÄ×²¨‚m‚f‰ñ”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv119 ‰ü(‚`)ƒpƒ“ƒ`‰ñ”i’¼Úˆóü•”F‰üŽD´ÄÞŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv120 ‰ü(‚`)ƒpƒ“ƒ`‰ñ”i’¼Úˆóü•”F‰üŽD85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv121 ‰ü(‚`)ƒpƒ“ƒ`‰ñ”i’¼Úˆóü•”FWŽD´ÄÞŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv122 ‰ü(‚`)ƒpƒ“ƒ`‰ñ”i’¼Úˆóü•”FWŽD85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv123 ‰ü(‚`)ƒpƒ“ƒ`‰ñ”i“]ŽÊˆóü•”F85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv124 ‰ü(‚`)ˆóü‰ñ”i’¼Úˆóü•”Fã‘¤´ÄÞŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv125 ‰ü(‚`)ˆóü‰ñ”i’¼Úˆóü•”Fã‘¤85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv126 ‰ü(‚`)ˆóü‰ñ”i’¼Úˆóü•”F‰º‘¤´ÄÞŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv127 ‰ü(‚`)ˆóü‰ñ”i’¼Úˆóü•”F‰º‘¤85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv128 ‰ü(‚`)ˆóü‰ñ”i“]ŽÊˆóü•”F85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv129 ‰ü(‚e)‚r‚m‚c|‚l‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv130 ‰ü(‚e)‚r‚m‚c|‚l‚S“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv131 ‰ü(‚e)‚r‚m‚c|‚l‚T“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv132 ‰ü(‚e)‚r‚m‚c|‚l‚U“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv133 ‰ü(‚e)‚r‚m‚c|‚l‚V“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv134 ‰ü(‚e)‚r‚m‚c|‚o‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv135 ‰ü(‚e)‚r‚m‚c|‚o‚S“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv136 ‰ü(‚e)‚r‚m‚c|‚o‚T“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv137 ‰ü(‚e)‚l‚s‚q|‚d‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv138 ‰ü(‚e)‚l‚s‚q|‚d‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv139 ‰ü(‚e)‚l‚s‚q|‚g‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv140 ‰ü(‚e)‚l‚s‚q|‚g‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv141 ‰ü(‚e)‚l‚s‚q|‚g‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv142 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv143 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv144 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv145 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv146 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv147 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv148 ‰ü(‚e)•ª—£•”Žæž“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv149 ‰ü(‚e)•ª—£•”ŒJo‚µ“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv150 ‰ü(‚e)®—ñ•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv151 ‰ü(‚`)‘WŽD–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv152 ‰ü(‚`)‚P–‡WŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv153 ‰ü(‚`)‚Q–‡WŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv154 ‰ü(‚`)‚R–‡WŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv155 ‰ü(‚`)‚S–‡WŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv156 ‰ü(‚`)‘•ÊWŽD–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv157 ‰ü(‚`)‚P–‡•ÊWŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv158 ‰ü(‚`)‚Q–‡•ÊWŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv159 ‰ü(‚`)‚R–‡•ÊWŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv160 ‰ü(‚`)‚S–‡•ÊWŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv161 ‰ü(‚`)•Û—¯Œ”iˆ—ˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv162 ‰ü(‚`)•Û—¯Œ”i•s³j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv163 ‰ü(‚`)“ñd‰»‚É‚æ‚é‹~Ï–‡”iB,GÄ×¯¸j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv164 ‰ü(‚`)“ñd‰»‚É‚æ‚é‹~Ï–‡”iB,GÄ×¯¸ˆÈŠOj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv165 ‰ü(‚`)®—ñ•”“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv166 ‰ü(‚`)Œ””½“]‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv167 ‰ü(‚`)‚d‚w‚h‚b{Ž¥‹C‚P–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv168 ‰ü(‚`)‚d‚w‚h‚b{Ž¥‹C‚Q–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv169 ‰ü(‚`)‚d‚w‚h‚b{Ž¥‹C‚R–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv170 ‰ü(‚`)Ý—ˆ‚h‚b{VŠ²üŽ¥‹C‚P–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv171 ‰ü(‚`)‰^‹xˆ—‘ÎÛŒ”“Š“ü–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv172 ‰ü(‚`)‘SŽÔŽ©—RÈ‘ÎÛŒ”“Š“ü–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv173 ‰ü(‚`)’x•¥‚¢‘ÎÛŒ”“Š“ü–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv174 ‰ü(‚`)Ý—ˆ‚h‚b{VŠ²üŽ¥‹C‚Q–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv175 ‰ü(‚e)‚r‚m‚c|‚`‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv176 ‰ü(‚e)‚r‚m‚c|‚`‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv177 ‰ü(‚e)‚r‚m‚c|‚`‚S“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv178 ‰ü(‚e)‚r‚m‚c|‚`‚T“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv179 ‰ü(‚e)‚r‚m‚c|‚l‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv180 ‰ü(‚e)‚r‚m‚c|‚o‚U“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv181 ‰ü(‚e)‚r‚m‚c|‚o‚V“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv182 ‰ü(‚e)‚r‚m‚c|‚o‚W“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv183 ‰ü(‚e)‚r‚m‚c|‚o‚X“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv184 ‰ü(‚e)‚r‚m‚c|‚d‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv185 ‰ü(‚e)‚r‚m‚c|‚d‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv186 ‰ü(‚e)‚r‚m‚c|‚d‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv187 ‰ü(‚e)‚r‚m‚c|‚d‚T“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv188 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv189 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv190 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv191 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv192 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv193 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv194 ‰ü(‚e)‚l‚s‚q|‚`‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv195 ‰ü(‚e)‚l‚s‚q|‚`‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv196 ‰ü(‚e)‚l‚s‚q|‚`‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv197 ‰ü(‚e)‚l‚s‚q|‚l‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv198 ‰ü(‚`)‘‚h‚bˆ—Žó•t–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv199 ‰ü(‚`)‚d‚w‚h‚bˆ—Žó•t–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv200 ‰ü(‚`)Ý—ˆ‚h‚bˆ—Žó•t–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv201 W(‚`)‘“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv202 W(‚`)‘“Š“ü–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv203 W(‚`)‚P–‡“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv204 W(‚`)‚Q–‡“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv205 W(‚`)‚R–‡“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv206 W(‚`)‚S–‡“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv207 W(‚`)‚T–‡ˆÈã“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv208 W(‚`)ˆêŠ‡“Š“üŒ”i‚Q–‡j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv209 W(‚`)ˆêŠ‡“Š“üŒ”i‚R–‡j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv210 W(‚`)ˆêŠ‡“Š“üŒ”i‚S–‡j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv211 W(‚`)ˆêŠ‡“Š“üŒ”i‚T–‡ˆÈãj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv212 W(‚`)‘S–‡”•\“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv213 W(‚`)‘S–‡”— “Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv214 W(‚`)— •\¬‡“Š“üŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv215 W(‚`)•\“Š“ü–‡”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv216 W(‚`)•\“Š“ü–‡”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv217 W(‚`)•\“Š“ü–‡”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv218 W(‚`)•\“Š“ü–‡”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv219 W(‚`)— “Š“ü–‡”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv220 W(‚`)— “Š“ü–‡”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv221 W(‚`)— “Š“ü–‡”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv222 W(‚`)— “Š“ü–‡”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv223 W(‚`)Œ””»’è‚n‚jŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv224 W(‚`)Œ””»’è‚n‚j–‡”i‡Œvj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv225 W(‚`)Œ””»’è‚n‚j–‡”i‚P–‡“Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv226 W(‚`)Œ””»’è‚n‚j–‡”i‚Q–‡“Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv227 W(‚`)Œ””»’è‚n‚j–‡”i‚R–‡“Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv228 W(‚`)Œ””»’è‚n‚j–‡”i‚S–‡“Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv229 W(‚`)Œ””»’è‚n‚j–‡”iNRZ´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv230 W(‚`)Œ””»’è‚n‚j–‡”iFM´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv231 W(‚`)Œ””»’è‚n‚j–‡”iNRZ’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv232 W(‚`)Œ””»’è‚n‚j–‡”iFM’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv233 W(‚`)Œ””»’è‚n‚j–‡”iFM‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv234 W(‚`)Œ””»’è‚n‚j–‡”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv235 W(‚`)”»’è‘ÎÛŠOŒ”“Š“ü–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv236 W(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”iæŽÔŒ”“Š“ü‘Ò‚¿j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv237 W(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”i“Á‹}Œ”“Š“ü‘Ò‚¿j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv238 (‚`)^“üêŒ”•s³—˜—pi’Ê˜H‚ð’Ê‰ß‚¹‚¸–ß‚ésˆ×j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv239 (‚`)^“üêŒ”•s³—˜—pi‚Ql‘g‚É‚æ‚è˜A‘±“Š“ü‚·‚ésˆ×j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv240 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv241 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv242 W(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”i“–‰w‚©‚çŒ”“Š“ü‘Ò‚¿j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv243 W(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”i“Á‹}Œ”{“–‰w‚©‚çŒ”“Š“ü‘Ò‚¿j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv244 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv245 W(‚`)‚»‚Ì‘¼‚h‚bˆ—–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv246 W(‚`)‚h‚bˆ—Œ”i‚P–‡ˆ—j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv247 W(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”iÝ—ˆ‚h‚bj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv248 W(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”iVŠ²üê—pŒ”“–‰w‚©‚çŒ”‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv249 W(‚`)‚²—˜—p•[”­Œ”–‡”i—ÝŒvj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv250 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv251 W(‚`)Œ””»’è‚m‚fŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv252 W(‚`)ˆÙíŒ””»’è‚m‚fi•\“Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv253 W(‚`)ˆÙíŒ””»’è‚m‚fi— “Š“üj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv254 W(‚`)ˆÙíŒ””»’è‚m‚fiÊßØÃ¨´×°F´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv255 W(‚`)ˆÙíŒ””»’è‚m‚fiÊßØÃ¨´×°F’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv256 W(‚`)ˆÙíŒ””»’è‚m‚fiÌ«°Ï¯Ä´×°F´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv257 W(‚`)ˆÙíŒ””»’è‚m‚fiÌ«°Ï¯Ä´×°F’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv258 W(‚`)ˆÙíŒ””»’è‚m‚fiÌ«°Ï¯Ä´×°F‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv259 W(‚`)ˆÙíŒ””»’è‚m‚fiÌ«°Ï¯Ä´×°F‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv260 W(‚`)ˆÙíŒ””»’è‚m‚fi“ñd‰»´×°j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv261 W(‚`)ˆÙíŒ””»’è‚m‚fi»ÑÁª¯¸´×°F´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv262 W(‚`)ˆÙíŒ””»’è‚m‚fi»ÑÁª¯¸´×°F’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv263 W(‚`)ˆÙíŒ””»’è‚m‚fi»ÑÁª¯¸´×°F‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv264 W(‚`)ˆÙíŒ””»’è‚m‚fi»ÑÁª¯¸´×°F‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv265 W(‚`)ˆÙíŒ””»’è‚m‚fi”ñŽ¥‹C‰»Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv266 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv267 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv268 W(‚`)–³ŒøŒ””»’è‚m‚fiŒ”Ží”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv269 W(‚`)–³ŒøŒ””»’è‚m‚fi‘ålŒ”¬Ž™Œ”¬Ý”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv270 W(‚`)–³ŒøŒ””»’è‚m‚fiŠúŠÔ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv271 W(‚`)–³ŒøŒ””»’è‚m‚fi‹æŠÔ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv272 W(‚`)–³ŒøŒ””»’è‚m‚fi“üêŒ”ŽžŠÔ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv273 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv274 W(‚`)–³ŒøŒ””»’è‚m‚fiŽg—pÏ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv275 W(‚`)–³ŒøŒ””»’è‚m‚fiŽ©‰w‰ºŽÔ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv276 W(‚`)–³ŒøŒ””»’è‚m‚fi•¡æ”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv277 W(‚`)–³ŒøŒ””»’è‚m‚fi•¡”–‡—LŒø”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv278 W(‚`)–³ŒøŒ””»’è‚m‚fiŽg—pŠJŽnŒã”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv279 W(‚`)–³ŒøŒ””»’è‚m‚fi“Š“ü–‡””»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv280 W(‚`)—LŒø‘g‡‚¹”»’è‚m‚fiæŽÔŒ”“Š“ü‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv281 W(‚`)—LŒø‘g‡‚¹”»’è‚m‚fi“Á‹}Œ”“Š“ü‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv282 W(‚`)—LŒø‘g‡‚¹”»’è‚m‚fi“–‰w‚©‚çæŽÔŒ”‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv283 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv284 W(‚`)—LŒø‘g‡‚¹”»’è‚m‚fi“Á‹}Œ”{“–‰w‚©‚çæŽÔŒ”‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv285 W(‚`)—LŒø‘g‡‚¹”»’è‚m‚fiæŽÔŒ”E“Á‹}Œ”“Š“ü‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv286 W(‚`)—LŒø‘g‡‚¹”»’è‚m‚fi‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv287 W(‚`)‘g‡‚¹”»’è‚m‚fiæŽÔŒ”¥“Á‹}Œ”‹æŠÔ”äŠr”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv288 W(‚`)—LŒø‘g‡‚¹”»’è‚m‚fiVŠ²üê—pŒ”“–‰w‚©‚çæŽÔŒ”‚È‚µj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv289 W(‚`)‘g‡‚¹”»’è‚m‚fiÚ‘±”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv290 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv291 W(‚`)‘g‡‚¹”»’è‚m‚fi•¹—p”»’èj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv292 W(‚`)Ý—ˆ‚h‚b{VŠ²üŽ¥‹C‚R–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv293 W(‚`)’Ç‰Á“Š“ü‘Ò‚¿Œ”i‚d‚w‚h‚bA(Š²)’èŠúŒ”(IC)j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv294 W(‚`)•s³”»’è‚m‚fi“üoêƒTƒCƒNƒ‹ˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv295 W(‚`)•s³”»’è‚m‚fi“¯ˆê‰w“üoêˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv296 W(‚`)•s³”»’è‚m‚fi•¡”‰ñŽg—pˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv297 W(‚`)‚h‚bˆ—Œ”i‚Q–‡ˆ—j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv298 W(‚`)‚»‚Ì‘¼‚m‚fi’x•¥‚¢”»’è•s‰Âj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv299 W(‚`)‚h‚cƒ`ƒFƒbƒN”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv300 W(‚`)‘‚h‚bŽ¥‹C•¹—pŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv301 W(‚`)Ž¥‹C‘žŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv302 W(‚`)Ž¥‹C‘žŒ”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv303 W(‚`)Ž¥‹C‘žŒ”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv304 W(‚`)Ž¥‹C‘žŒ”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv305 W(‚`)Ž¥‹C‘žŒ”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv306 W(‚`)Ž¥‹C‘žØÄ×²Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv307 W(‚`)Ž¥‹C‘žØÄ×²‰ñ”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv308 W(‚`)Ž¥‹C‘žØÄ×²‰ñ”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv309 W(‚`)Ž¥‹C‘žØÄ×²‰ñ”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv310 W(‚`)Ž¥‹C‘žØÄ×²‰ñ”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv311 W(‚`)Ž¥‹C‘žØÄ×²¨‚n‚j‰ñ”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv312 W(‚`)Ž¥‹C‘žØÄ×²¨‚n‚j‰ñ”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv313 W(‚`)Ž¥‹C‘žØÄ×²¨‚n‚j‰ñ”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv314 W(‚`)Ž¥‹C‘žØÄ×²¨‚n‚j‰ñ”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv315 W(‚`)Ž¥‹C‘žØÄ×²¨‚m‚f‰ñ”i´ÄÞÓÝ¿ÝŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv316 W(‚`)Ž¥‹C‘žØÄ×²¨‚m‚f‰ñ”i’èŠúŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv317 W(‚`)Ž¥‹C‘žØÄ×²¨‚m‚f‰ñ”i‘åŒ^Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv318 W(‚`)Ž¥‹C‘žØÄ×²¨‚m‚f‰ñ”i‚»‚Ì‘¼j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv319 W(‚`)ƒpƒ“ƒ`‰ñ”i’¼Úˆóü•”F‰üŽD´ÄÞŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv320 W(‚`)ƒpƒ“ƒ`‰ñ”i’¼Úˆóü•”F‰üŽD85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv321 W(‚`)ƒpƒ“ƒ`‰ñ”i’¼Úˆóü•”FWŽD´ÄÞŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv322 W(‚`)ƒpƒ“ƒ`‰ñ”i’¼Úˆóü•”FWŽD85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv323 W(‚`)ƒpƒ“ƒ`‰ñ”i“]ŽÊˆóü•”F85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv324 W(‚`)ˆóü‰ñ”i’¼Úˆóü•”Fã‘¤´ÄÞŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv325 W(‚`)ˆóü‰ñ”i’¼Úˆóü•”Fã‘¤85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv326 W(‚`)ˆóü‰ñ”i’¼Úˆóü•”F‰º‘¤´ÄÞŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv327 W(‚`)ˆóü‰ñ”i’¼Úˆóü•”F‰º‘¤85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv328 W(‚`)ˆóü‰ñ”i“]ŽÊ’¼Úˆóü•”F85mmŒ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv329 W(‚e)‚r‚m‚c|‚l‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv330 W(‚e)‚r‚m‚c|‚l‚S“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv331 W(‚e)‚r‚m‚c|‚l‚T“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv332 W(‚e)‚r‚m‚c|‚l‚U“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv333 W(‚e)‚r‚m‚c|‚l‚V“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv334 W(‚e)‚r‚m‚c|‚o‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv335 W(‚e)‚r‚m‚c|‚o‚S“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv336 W(‚e)‚r‚m‚c|‚o‚T“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv337 W(‚e)‚l‚s‚q|‚d‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv338 W(‚e)‚l‚s‚q|‚d‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv339 W(‚e)‚l‚s‚q|‚g‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv340 W(‚e)‚l‚s‚q|‚g‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv341 W(‚e)‚l‚s‚q|‚g‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv342 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv343 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv344 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv345 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv346 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv347 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv348 W(‚e)•ª—£•”Žæž“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv349 W(‚e)•ª—£•”ŒJo‚µ“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv350 W(‚e)®—ñ•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv351 W(‚`)‘WŽD–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv352 W(‚`)‚P–‡WŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv353 W(‚`)‚Q–‡WŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv354 W(‚`)‚R–‡WŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv355 W(‚`)‚S–‡WŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv356 W(‚`)‘•ÊWŽD–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv357 W(‚`)‚P–‡•ÊWŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv358 W(‚`)‚Q–‡•ÊWŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv359 W(‚`)‚R–‡•ÊWŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv360 W(‚`)‚S–‡•ÊWŽDŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv361 W(‚`)•Û—¯Œ”iˆ—ˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv362 W(‚`)•Û—¯Œ”i•s³j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv363 W(‚`)“ñd‰»‚É‚æ‚é‹~Ï–‡”iB,GÄ×¯¸j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv364 W(‚`)“ñd‰»‚É‚æ‚é‹~Ï–‡”iB,GÄ×¯¸ˆÈŠOj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv365 W(‚`)®—ñ•”“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv366 W(‚`)Œ””½“]‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv367 W(‚`)‚d‚w‚h‚b{Ž¥‹C‚P–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv368 W(‚`)‚d‚w‚h‚b{Ž¥‹C‚Q–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv369 W(‚`)‚d‚w‚h‚b{Ž¥‹C‚R–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv370 W(‚`)Ý—ˆ‚h‚b{VŠ²üŽ¥‹C‚P–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv371 W(‚`)‰^‹x •úo–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv372 W(‚`)‘SŽÔŽ©—RÈ •úo–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv373 W(‚`)’x•¥‚¢ ˆóŽš–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv374 W(‚`)Ý—ˆ‚h‚b{VŠ²üŽ¥‹C‚Q–‡ˆ—Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv375 W(‚e)‚r‚m‚c|‚`‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv376 W(‚e)‚r‚m‚c|‚`‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv377 W(‚e)‚r‚m‚c|‚`‚S“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv378 W(‚e)‚r‚m‚c|‚`‚T“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv379 W(‚e)‚r‚m‚c|‚l‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv380 W(‚e)‚r‚m‚c|‚o‚U“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv381 W(‚e)‚r‚m‚c|‚o‚V“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv382 W(‚e)‚r‚m‚c|‚o‚W“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv383 W(‚e)‚r‚m‚c|‚o‚X“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv384 W(‚e)‚r‚m‚c|‚d‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv385 W(‚e)‚r‚m‚c|‚d‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv386 W(‚e)‚r‚m‚c|‚d‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv387 W(‚e)‚r‚m‚c|‚d‚T“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv388 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv389 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv390 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv391 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv392 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv393 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv394 W(‚e)‚l‚s‚q|‚`‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv395 W(‚e)‚l‚s‚q|‚`‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv396 W(‚e)‚l‚s‚q|‚`‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv397 W(‚e)‚l‚s‚q|‚l‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv398 W(‚`)‘‚h‚bˆ—Žó•t–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv399 W(‚`)‚d‚w‚h‚bˆ—Žó•t–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv400 W(‚`)Ý—ˆ‚h‚bˆ—Žó•t–‡”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv401 (‚e)Žå‹@WŽDˆê’U•Û—¯‚`“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv402 (‚e)Žå‹@WŽDˆê’U•Û—¯‚a“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv403 (‚e)]‹@WŽDˆê’U•Û—¯‚`“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv404 (‚`)³Œ”ƒJƒEƒ“ƒ^–ž”t‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv405 (‚e)]‹@WŽDˆê’U•Û—¯‚a“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv406 (‚e)Žå‹@‰EƒhƒA“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv407 (‚e)Žå‹@¶ƒhƒA“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv408 (‚e)]‹@‰EƒhƒA“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv409 (‚e)]‹@¶ƒhƒA“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv410 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv411 ‰ü(‚e)‚r‚m‚c|‚`‚P“®ì‰ñ”  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv412 ‰ü(‚e)‚r‚m‚c|‚l‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv413 ‰ü(‚e)‚r‚m‚c|‚d‚S“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv414 ‰ü(‚e)‚l‚s‚q|‚l‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv415 ‰ü(‚e)‚l‚s‚q|‚o‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv416 ‰ü(‚e)‚l‚s‚q|‚o‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv417 ‰ü(‚e)‚l‚s‚q|‚o‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv418 ‰ü(‚e)‚l‚s‚q|‚o‚S“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv419 ‰ü(‚e)“ÇŽæ‚è•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv420 ‰ü(‚e)Œ””½“]•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv421 ‰ü(‚e)•Û—¯•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv422 ‰ü(‚e)’¼Úƒpƒ“ƒ`•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv423 ‰ü(‚e)’¼Úˆóü•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv424 ‰ü(‚e)“]ŽÊƒpƒ“ƒ`•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv425 ‰ü(‚e)“]ŽÊˆóü•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv426 ‰ü(‚e)•úo•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv427 ‰ü(‚e)WŽD•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv428 ‰ü(‚e)•ÊWŽD•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv429 ‰ü(‚e)”­Œ”“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv430 ‰ü(‚e)‚s‚o‚g’¼Ú‚kˆóü‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv431 ‰ü(‚e)‚s‚o‚g’¼Ú‚tˆóü‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv432 ‰ü(‚e)‚s‚o‚g“]ŽÊˆóü‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv433 ‰ü(‚e)‚s‚o‚g”­Œ”ˆóü‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv434 ‰ü(‚e)’¼Úƒ³‚Rƒpƒ“ƒ`“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv435 ‰ü(‚e)“]ŽÊƒ³‚Rƒpƒ“ƒ`“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv436 ‰ü(‚e)‚l‚f|‚q‚t‘åŒ^Œ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv437 ‰ü(‚e)‚l‚f|‚q‚t•’ÊŒ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv438 ‰ü(‚e)‚l‚f|‚q‚k‘åŒ^Œ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv439 ‰ü(‚e)‚l‚f|‚q‚k•’ÊŒ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv440 ‰ü(‚e)‚l‚f|‚v‘åŒ^Œ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv441 ‰ü(‚e)‚l‚f|‚v•’ÊŒ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv442 ‰ü(‚e)‚l‚f|‚u‘åŒ^Œ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv443 ‰ü(‚e)‚l‚f|‚u•’ÊŒ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv444 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv445 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv446 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv447 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv448 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv449 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv450 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv451 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv452 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv453 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv454 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv455 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv456 W(‚e)‚r‚m‚c|‚`‚P“®ì‰ñ”  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv457 W(‚e)‚r‚m‚c|‚l‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv458 W(‚e)‚r‚m‚c|‚d‚S“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv459 W(‚e)‚l‚s‚q|‚l‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv460 W(‚e)‚l‚s‚q|‚o‚P“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv461 W(‚e)‚l‚s‚q|‚o‚Q“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv462 W(‚e)‚l‚s‚q|‚o‚R“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv463 W(‚e)‚l‚s‚q|‚o‚S“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv464 W(‚e)“ÇŽæ‚è•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv465 W(‚e)Œ””½“]•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv466 W(‚e)•Û—¯•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv467 W(‚e)’¼Úƒpƒ“ƒ`•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv468 W(‚e)’¼Úˆóü•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv469 W(‚e)“]ŽÊƒpƒ“ƒ`•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv470 W(‚e)“]ŽÊˆóü•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv471 W(‚e)•úo•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv472 W(‚e)WŽD•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv473 W(‚e)•ÊWŽD•””À‘—‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv474 W(‚e)”­Œ”“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv475 W(‚e)‚s‚o‚g’¼Ú‚kˆóü‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv476 W(‚e)‚s‚o‚g’¼Ú‚tˆóü‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv477 W(‚e)‚s‚o‚g“]ŽÊˆóü‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv478 W(‚e)‚s‚o‚g”­Œ”ˆóü‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv479 W(‚e)’¼Úƒ³‚Rƒpƒ“ƒ`“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv480 W(‚e)“]ŽÊƒ³‚Rƒpƒ“ƒ`“®ì‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv481 W(‚e)‚l‚f|‚q‚t‘åŒ^Œ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv482 W(‚e)‚l‚f|‚q‚t•’ÊŒ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv483 W(‚e)‚l‚f|‚q‚k‘åŒ^Œ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv484 W(‚e)‚l‚f|‚q‚k•’ÊŒ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv485 W(‚e)‚l‚f|‚v‘åŒ^Œ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv486 W(‚e)‚l‚f|‚v•’ÊŒ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv487 W(‚e)‚l‚f|‚u‘åŒ^Œ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv488 W(‚e)‚l‚f|‚u•’ÊŒ”’Ê‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv489 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv490 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv491 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv492 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv493 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv494 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv495 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv496 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv497 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv498 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv499 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv500 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian)}, _
        New XlsField() { _
            New XlsField(8*1, "X2", 1, " "c, "Šî–{ƒwƒbƒ_[ ƒf[ƒ^Ží•Ê", "DataKind"), _
            New XlsField(8*1, "D3", 2, "-"c, "Šî–{ƒwƒbƒ_[ ‰wƒR[ƒh", "Station"), _
            New XlsField(8*7, "X14", 1, " "c, "Šî–{ƒwƒbƒ_[ ˆ—“úŽž"), _
            New XlsField(8*1, "D", 1, " "c, "Šî–{ƒwƒbƒ_[ ƒR[ƒi["), _
            New XlsField(8*1, "D", 1, " "c, "Šî–{ƒwƒbƒ_[ †‹@"), _
            New XlsField(8*4, "D", 1, " "c, "Šî–{ƒwƒbƒ_[ ƒV[ƒPƒ“ƒXNo", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*1, "X2", 1, " "c, "Šî–{ƒwƒbƒ_[ ƒo[ƒWƒ‡ƒ“"), _
            New XlsField(8*7, "X14", 1, " "c, "‹¤’Ê•” WŒvŠJŽn“úŽž"), _
            New XlsField(8*7, "X14", 1, " "c, "‹¤’Ê•” WŒvI—¹(ŽûW)“úŽž"), _
            New XlsField(8*7, "X14", 1, " "c, "‹¤’Ê•” ‰üŽD‘¤”À‘—•”“_ŒŸ“úŽž"), _
            New XlsField(8*7, "X14", 1, " "c, "‹¤’Ê•” WŽD‘¤”À‘—•”“_ŒŸ“úŽž"), _
            New XlsField(8*8, "X16", 1, " "c, "‹¤’Ê•” ‰üŽD‘¤”À‘—•””Ô†"), _
            New XlsField(8*8, "X16", 1, " "c, "‹¤’Ê•” WŽD‘¤”À‘—•””Ô†"), _
            New XlsField(8*1, "D", 48, " "c, "‹¤’Ê•” ‰üŽD‘¤ŒŸ’mƒZƒ“ƒTƒŒƒxƒ‹"), _
            New XlsField(8*1, "D", 48, " "c, "‹¤’Ê•” WŽD‘¤ŒŸ’mƒZƒ“ƒTƒŒƒxƒ‹"), _
            New XlsField(8*1, "X2", 48, " "c, "‹¤’Ê•” —\”õ"), _
            New XlsField(8*4, "D", 1, " "c, "WŒv001 ‰ü(‚`)“ÇŽæˆÙí|ãƒwƒbƒhiƒGƒhƒ‚ƒ“ƒ\ƒ“Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv002 ‰ü(‚`)“ÇŽæˆÙí|ãƒwƒbƒhi‚W‚T‚‚Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv003 ‰ü(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒhiƒGƒhƒ‚ƒ“ƒ\ƒ“Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv004 ‰ü(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒhi‚W‚T‚‚Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv005 ‰ü(‚`)“ÇŽæˆÙí|ãƒwƒbƒh@‚Pƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv006 ‰ü(‚`)“ÇŽæˆÙí|ãƒwƒbƒh@‚Qƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv007 ‰ü(‚`)“ÇŽæˆÙí|ãƒwƒbƒh@‚Rƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv008 ‰ü(‚`)“ÇŽæˆÙí|ãƒwƒbƒh@‚Sƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv009 ‰ü(‚`)“ÇŽæˆÙí|ãƒwƒbƒh@‚Tƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv010 ‰ü(‚`)“ÇŽæˆÙí|ãƒwƒbƒh@‚Uƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv011 ‰ü(‚`)“ÇŽæˆÙí|ãƒwƒbƒh@‚Vƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv012 ‰ü(‚`)“ÇŽæˆÙí|ãƒwƒbƒh@‚Wƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv013 ‰ü(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh@‚Pƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv014 ‰ü(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh@‚Qƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv015 ‰ü(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh@‚Rƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv016 ‰ü(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh@‚Sƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv017 ‰ü(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh@‚Tƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv018 ‰ü(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh@‚Uƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv019 ‰ü(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh@‚Vƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv020 ‰ü(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh@‚Wƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv021 ‰ü(‚`)‘žˆÙí‰ñ”|ƒGƒhƒ‚ƒ“ƒ\ƒ“Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv022 ‰ü(‚`)‘žˆÙí‰ñ”|’èŠúŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv023 ‰ü(‚`)‘žˆÙí‰ñ”|‘åŒ^Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv024 ‰ü(‚`)‘žˆÙí‰ñ”|‚»‚Ì‘¼iSFƒJ[ƒhj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv025 ‰ü(‚`)‘žˆÙí˜A‘±|ƒGƒhƒ‚ƒ“ƒ\ƒ“Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv026 ‰ü(‚`)‘žˆÙí˜A‘±|’èŠúŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv027 ‰ü(‚`)‘žˆÙí˜A‘±|‘åŒ^Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv028 ‰ü(‚`)‘žˆÙí˜A‘±|‚»‚Ì‘¼iSFƒJ[ƒhj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv029 ‰ü(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh@‚Pƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv030 ‰ü(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh@‚Qƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv031 ‰ü(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh@‚Rƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv032 ‰ü(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh@‚Sƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv033 ‰ü(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh@‚Tƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv034 ‰ü(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh@‚Uƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv035 ‰ü(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh@‚Vƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv036 ‰ü(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh@‚Wƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv037 ‰ü(‚`)‚h‚b‚q‚vˆÙíŒŸ’m‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv038 ‰ü(‚`)‚²—˜—p•[”­Œ”ˆÙíŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv039 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv040 ‰ü(‚`)‘‚h‚b–¢—¹Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv041 ‰ü(‚`)‚h‚b“ÇŽæ‚è–¢—¹Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv042 ‰ü(‚`)‚d‚w‚h‚b‘ž‚Ý–¢—¹Œ”i‚P–‡ˆ—Žžj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv043 ‰ü(‚`)Ý—ˆ‚h‚b‘ž‚Ý–¢—¹Œ”i‚P–‡ˆ—Žžj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv044 ‰ü(‚`)‚h‚b“ÇŽæ”»’èˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv045 ‰ü(‚`)‚h‚b–‡”’´‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv046 ‰ü(‚`)‚h‚b‚h‚c‚‰”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv047 ‰ü(‚`)‚d‚w‚h‚b—\–ñî•ñŒŸõ‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv048 ‰ü(‚`)‚d‚w‚h‚bƒo[ƒWƒ‡ƒ“”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv049 ‰ü(‚`)‚d‚w‚h‚bƒf[ƒ^€–Ú”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv050 ‰ü(‚`)‚d‚w‚h‚bƒJ[ƒhŽg—p•s‰Â”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv051 ‰ü(‚`)‚d‚w‚h‚bÅI—˜—p“ú•t”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv052 ‰ü(‚`)‚d‚w‚h‚bƒlƒKƒ`ƒFƒbƒN”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv053 ‰ü(‚`)‚d‚w‚h‚b“üoêƒV[ƒPƒ“ƒX”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv054 ‰ü(‚`)‚d‚w‚h‚b—\–ñî•ñ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv055 ‰ü(‚`)‚d‚w‚h‚bI—ñŽÔ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv056 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv057 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv058 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv059 ‰ü(‚`)‚d‚w‚h‚bÝ—ˆü–¢oê‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv060 ‰ü(‚`)EXIC“–‰w–˜Œ”‚È‚µ‚m‚f“–‰w‚©‚çŒ”‚È‚µ‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv061 ‰ü(‚`)Ý—ˆ‚h‚bƒo[ƒWƒ‡ƒ“”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv062 ‰ü(‚`)Ý—ˆ‚h‚b‚h‚bŽí•Ê”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv063 ‰ü(‚`)Ý—ˆ‚h‚bƒf[ƒ^€–Ú”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv064 ‰ü(‚`)Ý—ˆ‚h‚bƒJ[ƒh³“–«”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv065 ‰ü(‚`)Ý—ˆ‚h‚bƒ}ƒXƒ^ƒf[ƒ^”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv066 ‰ü(‚`)Ý—ˆ‚h‚bŠˆ«‰»”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv067 ‰ü(‚`)Ý—ˆ‚h‚bƒJ[ƒhŽg—p•s‰Â”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv068 ‰ü(‚`)Ý—ˆ‚h‚bƒlƒKƒ`ƒFƒbƒN”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv069 ‰ü(‚`)Ý—ˆ‚h‚b’èŠúŒ”ŠúŠÔ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv070 ‰ü(‚`)Ý—ˆ‚h‚b“üoêƒV[ƒPƒ“ƒX”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv071 ‰ü(‚`)Ý—ˆ‚h‚b—˜—p“ú•t”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv072 ‰ü(‚`)Ý—ˆ‚h‚bŽ©‰w‰ºŽÔ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv073 ‰ü(‚`)Ý—ˆ‚h‚b‹æŠÔ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv074 ‰ü(‚`)Ý—ˆ‚h‚b“üoêƒR[ƒh”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv075 ‰ü(‚`)Ý—ˆ‚h‚bŽcŠz”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv076 ‰ü(‚`)Ý—ˆ‚h‚b¸ŽZ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv077 ‰ü(‚`)Ý—ˆ‚h‚bˆê“_’Ê‰ß”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv078 ‰ü(‚`)Ý—ˆIC•s³”»’è‚m‚fi“üoêƒTƒCƒNƒ‹ˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv079 ‰ü(‚`)Ý—ˆIC•s³”»’è‚m‚fi“üoêŽžŠÔˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv080 ‰ü(‚`)Ý—ˆIC•s³”»’è‚m‚fi“¯ˆê‰w“üoêˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv081 ‰ü(‚`)Ý—ˆIC•s³”»’è‚m‚fi˜A‘±“üêEoêˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv082 ‰ü(‚`)Ý—ˆ‚h‚bVŠ²ü—LŒøŒ”‚È‚µ‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv083 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv084 ‰ü(‚`)Ž¥‹C‚h‚b•¹—p‘å¬¬Ý”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv085 ‰ü(‚`)Ž¥‹C‚h‚b•¹—pVŠ²ü‹æŠÔd•¡‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv086 ‰ü(‚`)Ž¥‹C‚h‚b•¹—pÚ‘±‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv087 ‰ü(‚`)Ž¥‹C‚h‚b•¹—p“–‰w–˜Œ”•¡”–‡‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv088 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv089 ‰ü(‚`)Ž¥‹C‚h‚b•¹—p—LŒøŒ”•¡”–‡‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv090 ‰ü(‚`)Ž¥‹C‚h‚b•¹—p¸ŽZ•s‰Â‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv091 ‰ü(‚`)‚d‚w‚h‚b‘ž‚ÝˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv092 ‰ü(‚`)Ý—ˆ‚h‚b‘ž‚ÝˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv093 ‰ü(‚`)Ý—ˆ‚h‚bƒeƒXƒgƒJ[ƒh”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv094 ‰ü(‚`)Ý—ˆ‚h‚b’èŠú‹æŠÔƒGƒŠƒA‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv095 ‰ü(‚`)Ý—ˆ‚h‚bÅI—˜—p“ú•t”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv096 ‰ü(‚`)Ý—ˆ‚h‚b‘¼ŽÐŠ„ˆø‚h‚bƒJ[ƒh‚r‚e—˜—p‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv097 ‰ü(‚`)‚d‚w‚h‚b‘ž‚Ý–¢—¹Œ”i‚Q–‡ˆ—Žžj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv098 ‰ü(‚`)Ý—ˆ‚h‚b‘ž‚Ý–¢—¹Œ”i‚Q–‡ˆ—Žžj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv099 ‰ü(‚`)‘‚h‚b”»’è‚m‚fŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv100 ‰ü(‚`)Ý—ˆ‚h‚b‰ïŽÐŠÔŒo˜H˜A‘±«”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv101 ‰ü(‚e)•ª—£•”¾Ý»“dŒ¹      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv102 ‰ü(‚e)•ª—£•”¿ÚÉ²ÄÞ“dŒ¹      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv103 ‰ü(‚e)•ª—£•”¿ÚÉ²ÄÞPLŒŸ’m    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv104 ‰ü(‚e)•ª—£•”Ó°ÀÄÞ×²ÊÞ±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv105 ‰ü(‚e)•ª—£•”Ó°À“dŒ¹±×°Ñ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv106 ‰ü(‚e){‚Q‚S‚u“dŒ¹          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv107 ‰ü(‚e)Ž¥‹C•”¾Ý»“dŒ¹         ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv108 ‰ü(‚e)Ž¥‹C•”¿ÚÉ²ÄÞ“dŒ¹      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv109 ‰ü(‚e)Ž¥‹C•”¿ÚÉ²ÄÞPLŒŸ’m    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv110 ‰ü(‚e)Ž¥‹C×²Ä±×°Ñ(ONŽžŠÔ)   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv111 ‰ü(‚e)Ž¥‹C•”Ó°ÀÄÞ×²ÊÞ±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv112 ‰ü(‚e)Ž¥‹C×²Ä“dŒ¹“dˆ³     ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv113 ‰ü(‚e)ˆóü`•úo•”¾Ý»“dŒ¹   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv114 ‰ü(‚e)ˆóü`•úo•”¿ÚÉ²ÄÞPL  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv115 ‰ü(‚e)ˆóü•”Ó°ÀÄÞ×²ÊÞ±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv116 ‰ü(‚e)•úo•”Ó°ÀÄÞ×²ÊÞ±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv117 ‰ü(‚e)WŽD•”Ó°ÀÄÞ×²ÊÞ±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv118 ‰ü(‚e)”­Œ”•”H1Ó°À±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv119 ‰ü(‚e)”­Œ”•”H2Ó°À±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv120 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv121 ‰ü(‚e)‚d‚Q‚o‚q‚n‚lˆÙí    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv122 ‰ü(‚e)’¼Úƒpƒ“ƒ`ˆÙí      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv123 ‰ü(‚e)“]ŽÊƒpƒ“ƒ`ˆÙí      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv124 ‰ü(‚e)’¼Úãˆóü“®ìˆÙí  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv125 ‰ü(‚e)’¼Ú‰ºˆóü“®ìˆÙí  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv126 ‰ü(‚e)“]ŽÊˆóü“®ìˆÙí    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv127 ‰ü(‚e)“]ŽÊƒŠƒ{ƒ“Ø‚ê      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv128 ‰ü(‚e)”­Œ”Û°ÙŽ†Ø‚ê       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv129 ‰ü(‚e)”­Œ”Û°ÙŽ†¾¯Ä•s—Ç    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv130 ‰ü(‚e)”­Œ”•”¶¯ÀˆÊ’uˆÙí   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv131 ‰ü(‚e)•ª—£•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv132 ‰ü(‚e)®—ñ•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv133 ‰ü(‚e)”½“]•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv134 ‰ü(‚e)‘ž‘OŒ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv135 ‰ü(‚e)•Û—¯‚PŒ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv136 ‰ü(‚e)•Û—¯‚QŒ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv137 ‰ü(‚e)•Û—¯‚RŒ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv138 ‰ü(‚e)”­Œ”•Û—¯•”Œ”‹l‚è  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv139 ‰ü(‚e)’¼Úƒpƒ“ƒ`‘OŒ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv140 ‰ü(‚e)’¼Úƒpƒ“ƒ`ŒãŒ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv141 ‰ü(‚e)’¼Ú‰ºˆóü•”Œ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv142 ‰ü(‚e)’¼Úãˆóü•”Œ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv143 ‰ü(‚e)“]ŽÊƒpƒ“ƒ`‘OŒ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv144 ‰ü(‚e)“]ŽÊƒpƒ“ƒ`ŒãŒ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv145 ‰ü(‚e)“]ŽÊˆóü•”Œ”‹l‚è  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv146 ‰ü(‚e)’¼ÚˆóüˆÙíŒ”‹l‚è  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv147 ‰ü(‚e)“]ŽÊˆóüˆÙíŒ”‹l‚è  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv148 ‰ü(‚e)WÏ•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv149 ‰ü(‚e)•úo•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv150 ‰ü(‚e)WŽD•”Œ”‹l‚è    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv151 ‰ü(‚e)•úo•”Œ”‹l‚è(Žæ)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv152 ‰ü(‚e)WŽD•”Œ”‹l‚è(Žæ)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv153 ‰ü(‚e)ˆóü`•úo•”Œ”‹l‚è  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv154 ‰ü(‚e)”­Œ”•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv155 ‰ü(‚e)”­Œ”•”‘•“UŒ”‹l‚è    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv156 ‰ü(‚e)•Û—¯‚P‚·‚è”²‚¯      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv157 ‰ü(‚e)•Û—¯‚Q‚·‚è”²‚¯      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv158 ‰ü(‚e)•Û—¯‚R‚·‚è”²‚¯      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv159 ‰ü(‚e)’¼Ú•”½Ä¯Êß‚·‚è”²‚¯ ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv160 ‰ü(‚e)“]ŽÊ•”½Ä¯Êß‚·‚è”²‚¯ ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv161 ‰ü(‚e)”½“]•”‚·‚è”²‚¯      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv162 ‰ü(‚e)”½“]•”U‚è•ª‚¯ˆÙí  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv163 ‰ü(‚e)•Û—¯•ªŠòU‚è•ª‚¯ˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv164 ‰ü(‚e)ˆóü•ªŠòU‚è•ª‚¯ˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv165 ‰ü(‚e)WŽDU‚è•ª‚¯ˆÙí    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv166 ‰ü(‚e)•úoU‚è•ª‚¯ˆÙí    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv167 ‰ü(‚e)ˆê’UWŽDU‚è•ª‚¯ˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv168 ‰ü(‚e)Ž¥‹CCPUˆÙí‚P       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv169 ‰ü(‚e)Ž¥‹CCPUˆÙí‚Q       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv170 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv171 ‰ü(‚e)ƒZƒ“ƒTˆÙí          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv172 ‰ü(‚e)ƒZƒbƒg•s—Ç          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv173 ‰ü(‚e)ƒRƒ}ƒ“ƒhˆÙí  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv174 ‰ü(‚e)d‘—ŒŸ’m‰ñ”        ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv175 ‰ü(‚e)‚sŒŸŒÌá‰ñ”        ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv176 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv177 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv178 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv179 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv180 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv181 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv182 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv183 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv184 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv185 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv186 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv187 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv188 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv189 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv190 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv191 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv192 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv193 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv194 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv195 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv196 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv197 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv198 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv199 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv200 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv201 W(‚`)“ÇŽæˆÙí|ãƒwƒbƒhiƒGƒhƒ‚ƒ“ƒ\ƒ“Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv202 W(‚`)“ÇŽæˆÙí|ãƒwƒbƒhi‚W‚T‚‚Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv203 W(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒhiƒGƒhƒ‚ƒ“ƒ\ƒ“Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv204 W(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒhi‚W‚T‚‚Œ”j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv205 W(‚`)“ÇŽæˆÙí|ãƒwƒbƒh ‚Pƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv206 W(‚`)“ÇŽæˆÙí|ãƒwƒbƒh ‚Qƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv207 W(‚`)“ÇŽæˆÙí|ãƒwƒbƒh ‚Rƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv208 W(‚`)“ÇŽæˆÙí|ãƒwƒbƒh ‚Sƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv209 W(‚`)“ÇŽæˆÙí|ãƒwƒbƒh ‚Tƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv210 W(‚`)“ÇŽæˆÙí|ãƒwƒbƒh ‚Uƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv211 W(‚`)“ÇŽæˆÙí|ãƒwƒbƒh ‚Vƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv212 W(‚`)“ÇŽæˆÙí|ãƒwƒbƒh ‚Wƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv213 W(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh ‚Pƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv214 W(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh ‚Qƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv215 W(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh ‚Rƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv216 W(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh ‚Sƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv217 W(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh ‚Tƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv218 W(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh ‚Uƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv219 W(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh ‚Vƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv220 W(‚`)“ÇŽæˆÙí|‰ºƒwƒbƒh ‚Wƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv221 W(‚`)‘žˆÙí‰ñ”|ƒGƒhƒ‚ƒ“ƒ\ƒ“Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv222 W(‚`)‘žˆÙí‰ñ”|’èŠúŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv223 W(‚`)‘žˆÙí‰ñ”|‘åŒ^Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv224 W(‚`)‘žˆÙí‰ñ”|‚»‚Ì‘¼iSFƒJ[ƒhj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv225 W(‚`)‘žˆÙí˜A‘±|ƒGƒhƒ‚ƒ“ƒ\ƒ“Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv226 W(‚`)‘žˆÙí˜A‘±|’èŠúŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv227 W(‚`)‘žˆÙí˜A‘±|‘åŒ^Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv228 W(‚`)‘žˆÙí˜A‘±|‚»‚Ì‘¼iSFƒJ[ƒhj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv229 W(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh ‚Pƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv230 W(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh ‚Qƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv231 W(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh ‚Rƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv232 W(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh ‚Sƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv233 W(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh ‚Tƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv234 W(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh ‚Uƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv235 W(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh ‚Vƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv236 W(‚`)‘žˆÙí˜A‘±|‰ºƒwƒbƒh ‚Wƒgƒ‰ƒbƒN", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv237 W(‚`)‚h‚b‚q‚vˆÙíŒŸ’m‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv238 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv239 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv240 W(‚`)‘‚h‚b–¢—¹Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv241 W(‚`)‚h‚b“ÇŽæ‚è–¢—¹Œ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv242 W(‚`)‚d‚w‚h‚b‘ž‚Ý–¢—¹Œ”i‚P–‡ˆ—Žžj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv243 W(‚`)Ý—ˆ‚h‚b‘ž‚Ý–¢—¹Œ”i‚P–‡ˆ—Žžj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv244 W(‚`)‚h‚b“ÇŽæ”»’èˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv245 W(‚`)‚h‚b–‡”’´‰ß", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv246 W(‚`)‚h‚b‚h‚c‚‰”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv247 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv248 W(‚`)‚d‚w‚h‚bƒo[ƒWƒ‡ƒ“”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv249 W(‚`)‚d‚w‚h‚bƒf[ƒ^€–Ú”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv250 W(‚`)‚d‚w‚h‚bƒJ[ƒhŽg—p•s‰Â”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv251 W(‚`)‚d‚w‚h‚bÅI—˜—p“ú•t”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv252 W(‚`)‚d‚w‚h‚bƒlƒKƒ`ƒFƒbƒN”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv253 W(‚`)‚d‚w‚h‚b“üoêƒV[ƒPƒ“ƒX”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv254 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv255 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv256 W(‚`)‚d‚w‚h‚b—˜—p“ú•t”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv257 W(‚`)‚d‚w‚h‚bŽ©‰w‰ºŽÔ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv258 W(‚`)‚d‚w‚h‚b‹æŠÔ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv259 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv260 W(‚`)EXIC“–‰w–˜Œ”‚È‚µNG“–‰w‚©‚çŒ”‚È‚µNG", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv261 W(‚`)Ý—ˆ‚h‚bƒo[ƒWƒ‡ƒ“”»’è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv262 W(‚`)Ý—ˆ‚h‚b‚h‚bŽí•Ê”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv263 W(‚`)Ý—ˆ‚h‚bƒf[ƒ^€–Ú”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv264 W(‚`)Ý—ˆ‚h‚bƒJ[ƒh³“–«”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv265 W(‚`)Ý—ˆ‚h‚bƒ}ƒXƒ^ƒf[ƒ^”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv266 W(‚`)Ý—ˆ‚h‚bŠˆ«‰»”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv267 W(‚`)Ý—ˆ‚h‚bƒJ[ƒhŽg—p•s‰Â”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv268 W(‚`)Ý—ˆ‚h‚bƒlƒKƒ`ƒFƒbƒN”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv269 W(‚`)Ý—ˆ‚h‚b’èŠúŒ”ŠúŠÔ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv270 W(‚`)Ý—ˆ‚h‚b“üoêƒV[ƒPƒ“ƒX”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv271 W(‚`)Ý—ˆ‚h‚b—˜—p“ú•t”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv272 W(‚`)Ý—ˆ‚h‚bŽ©‰w‰ºŽÔ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv273 W(‚`)Ý—ˆ‚h‚b‹æŠÔ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv274 W(‚`)Ý—ˆ‚h‚b“üoêƒR[ƒh”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv275 W(‚`)Ý—ˆ‚h‚bŽcŠz”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv276 W(‚`)Ý—ˆ‚h‚b¸ŽZ”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv277 W(‚`)Ý—ˆ‚h‚bˆê“_’Ê‰ß”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv278 W(‚`)Ý—ˆIC•s³”»’è‚m‚fi“üoêƒTƒCƒNƒ‹ˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv279 W(‚`)Ý—ˆ‚h‚b•s³”»’è‚m‚f(“üoêŽžŠÔˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv280 W(‚`)Ý—ˆ‚h‚b•s³”»’è‚m‚fi“¯ˆê‰w“üoêˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv281 W(‚`)Ý—ˆIC•s³”»’è‚m‚fi˜A‘±“üêEoêˆÙíj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv282 W(‚`)Ý—ˆ‚h‚bVŠ²ü—LŒøŒ”‚È‚µ‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv283 W(‚`)Ý—ˆ‚h‚b–¢oê‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv284 W(‚`)Ž¥‹C‚h‚b•¹—p‘å¬¬Ý”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv285 W(‚`)Ž¥‹C‚h‚b•¹—pVŠ²ü‹æŠÔd•¡‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv286 W(‚`)Ž¥‹C‚h‚b•¹—pÚ‘±‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv287 W(‚`)Ž¥‹C‚h‚b•¹—p“–‰w–˜Œ”•¡”–‡‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv288 W(‚`)Ž¥‹CIC•¹—p“–‰w‚©‚çŒ”•¡”–‡‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv289 W(‚`)Ž¥‹CIC•¹—p—LŒøŒ”•¡”–‡‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv290 W(‚`)Ž¥‹C‚h‚b•¹—p¸ŽZ•s‰Â‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv291 W(‚`)‚d‚w‚h‚b‘ž‚ÝˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv292 W(‚`)Ý—ˆ‚h‚b‘ž‚ÝˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv293 W(‚`)Ý—ˆ‚h‚bƒeƒXƒgƒJ[ƒh”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv294 W(‚`)Ý—ˆ‚h‚b’èŠú‹æŠÔƒGƒŠƒA‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv295 W(‚`)Ý—ˆ‚h‚bÅI—˜—p“ú•t”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv296 W(‚`)Ý—ˆ‚h‚b‘¼ŽÐŠ„ˆø‚h‚bƒJ[ƒh‚r‚e—˜—p‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv297 W(‚`)‚d‚w‚h‚b‘ž‚Ý–¢—¹Œ”i‚Q–‡ˆ—Žžj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv298 W(‚`)Ý—ˆ‚h‚b‘ž‚Ý–¢—¹Œ”i‚Q–‡ˆ—Žžj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv299 W(‚`)‘‚h‚b”»’è‚m‚fŒ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv300 W(‚`)Ý—ˆ‚h‚b‰ïŽÐŠÔŒo˜H˜A‘±«”»’è‚m‚f", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv301 W(‚e)•ª—£•”¾Ý»“dŒ¹      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv302 W(‚e)•ª—£•”¿ÚÉ²ÄÞ“dŒ¹      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv303 W(‚e)•ª—£•”¿ÚÉ²ÄÞPLŒŸ’m    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv304 W(‚e)•ª—£•”Ó°ÀÄÞ×²ÊÞ±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv305 W(‚e)•ª—£•”Ó°À“dŒ¹±×°Ñ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv306 W(‚e){‚Q‚S‚u“dŒ¹          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv307 W(‚e)Ž¥‹C•”¾Ý»“dŒ¹         ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv308 W(‚e)Ž¥‹C•”¿ÚÉ²ÄÞ“dŒ¹      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv309 W(‚e)Ž¥‹C•”¿ÚÉ²ÄÞPLŒŸ’m    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv310 W(‚e)Ž¥‹C×²Ä±×°Ñ(ONŽžŠÔ)   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv311 W(‚e)Ž¥‹C•”Ó°ÀÄÞ×²ÊÞ±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv312 W(‚e)Ž¥‹C×²Ä“dŒ¹“dˆ³     ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv313 W(‚e)ˆóü`•úo•”¾Ý»“dŒ¹   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv314 W(‚e)ˆóü`•úo•”¿ÚÉ²ÄÞPL  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv315 W(‚e)ˆóü•”Ó°ÀÄÞ×²ÊÞ±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv316 W(‚e)•úo•”Ó°ÀÄÞ×²ÊÞ±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv317 W(‚e)WŽD•”Ó°ÀÄÞ×²ÊÞ±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv318 W(‚e)”­Œ”•”H1Ó°À±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv319 W(‚e)”­Œ”•”H2Ó°À±×°Ñ   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv320 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv321 W(‚e)‚d‚Q‚o‚q‚n‚lˆÙí    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv322 W(‚e)’¼Úƒpƒ“ƒ`ˆÙí      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv323 W(‚e)“]ŽÊƒpƒ“ƒ`ˆÙí      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv324 W(‚e)’¼Úãˆóü“®ìˆÙí  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv325 W(‚e)’¼Ú‰ºˆóü“®ìˆÙí  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv326 W(‚e)“]ŽÊˆóü“®ìˆÙí    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv327 W(‚e)“]ŽÊƒŠƒ{ƒ“Ø‚ê      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv328 W(‚e)”­Œ”Û°ÙŽ†Ø‚ê       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv329 W(‚e)”­Œ”Û°ÙŽ†¾¯Ä•s—Ç    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv330 W(‚e)”­Œ”•”¶¯ÀˆÊ’uˆÙí   ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv331 W(‚e)•ª—£•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv332 W(‚e)®—ñ•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv333 W(‚e)”½“]•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv334 W(‚e)‘ž‘OŒ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv335 W(‚e)•Û—¯‚PŒ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv336 W(‚e)•Û—¯‚QŒ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv337 W(‚e)•Û—¯‚RŒ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv338 W(‚e)”­Œ”•Û—¯•”Œ”‹l‚è  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv339 W(‚e)’¼Úƒpƒ“ƒ`‘OŒ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv340 W(‚e)’¼Úƒpƒ“ƒ`ŒãŒ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv341 W(‚e)’¼Ú‰ºˆóü•”Œ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv342 W(‚e)’¼Úãˆóü•”Œ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv343 W(‚e)“]ŽÊƒpƒ“ƒ`‘OŒ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv344 W(‚e)“]ŽÊƒpƒ“ƒ`ŒãŒ”‹l‚è", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv345 W(‚e)“]ŽÊˆóü•”Œ”‹l‚è  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv346 W(‚e)’¼ÚˆóüˆÙíŒ”‹l‚è  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv347 W(‚e)“]ŽÊˆóüˆÙíŒ”‹l‚è  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv348 W(‚e)WÏ•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv349 W(‚e)•úo•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv350 W(‚e)WŽD•”Œ”‹l‚è    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv351 W(‚e)•úo•”Œ”‹l‚è(Žæ)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv352 W(‚e)WŽD•”Œ”‹l‚è(Žæ)  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv353 W(‚e)ˆóü`•úo•”Œ”‹l‚è  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv354 W(‚e)”­Œ”•”Œ”‹l‚è      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv355 W(‚e)”­Œ”•”‘•“UŒ”‹l‚è    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv356 W(‚e)•Û—¯‚P‚·‚è”²‚¯      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv357 W(‚e)•Û—¯‚Q‚·‚è”²‚¯      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv358 W(‚e)•Û—¯‚R‚·‚è”²‚¯      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv359 W(‚e)’¼Ú•”½Ä¯Êß‚·‚è”²‚¯ ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv360 W(‚e)“]ŽÊ•”½Ä¯Êß‚·‚è”²‚¯ ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv361 W(‚e)”½“]•”‚·‚è”²‚¯      ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv362 W(‚e)”½“]•”U‚è•ª‚¯ˆÙí  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv363 W(‚e)•Û—¯•ªŠòU‚è•ª‚¯ˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv364 W(‚e)ˆóü•ªŠòU‚è•ª‚¯ˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv365 W(‚e)WŽDU‚è•ª‚¯ˆÙí    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv366 W(‚e)•úoU‚è•ª‚¯ˆÙí    ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv367 W(‚e)ˆê’UWŽDU‚è•ª‚¯ˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv368 W(‚e)Ž¥‹CCPUˆÙí‚P       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv369 W(‚e)Ž¥‹CCPUˆÙí‚Q       ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv370 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv371 W(‚e)ƒZƒ“ƒTˆÙí          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv372 W(‚e)ƒZƒbƒg•s—Ç          ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv373 W(‚e)ƒRƒ}ƒ“ƒhˆÙí  ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv374 W(‚e)d‘—ŒŸ’m‰ñ”        ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv375 W(‚e)‚sŒŸŒÌá‰ñ”        ", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv376 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv377 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv378 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv379 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv380 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv381 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv382 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv383 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv384 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv385 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv386 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv387 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv388 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv389 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv390 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv391 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv392 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv393 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv394 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv395 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv396 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv397 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv398 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv399 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv400 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv401 (‚e)lŠÔŒŸ’mŒÌái”½ŽËj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv402 (‚e)lŠÔŒŸ’mŒÌái“§‰ßj", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv403 (‚e)ƒ‰ƒCƒ“ƒZƒ“ƒTŒÌáŒx‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv404 (‚e)Žå‹@WŽDˆê’U•Û—¯‚`ˆÙí‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv405 (‚e)Žå‹@WŽDˆê’U•Û—¯‚aˆÙí‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv406 (‚e)]‹@WŽDˆê’U•Û—¯‚`ˆÙí‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv407 (‚e)]‹@WŽDˆê’U•Û—¯‚aˆÙí‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv408 i‚ejŽå‹@WŽDˆê’U•Û—¯‚`–ž”tŒŸ’m‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv409 i‚ejŽå‹@WŽDˆê’U•Û—¯‚a–ž”tŒŸ’m‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv410 i‚ej]‹@WŽDˆê’U•Û—¯‚`–ž”tŒŸ’m‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv411 i‚ej]‹@WŽDˆê’U•Û—¯‚a–ž”tŒŸ’m‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv412 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv413 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv414 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv415 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv416 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv417 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv418 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv419 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv420 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv421 (‚`)ƒhƒAŒÌá|WŽDE\“à‘¤", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv422 (‚`)ƒhƒAŒÌá|WŽDE\ŠO‘¤", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv423 (‚`)ƒhƒAŒÌá|‰üŽDE\“à‘¤", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv424 (‚`)ƒhƒAŒÌá|‰üŽDE\ŠO‘¤", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv425 (‚`)ˆ—’†’fˆÙí", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv426 (‚`)‹@ŠíˆÙíŽ©“®•œ‹A‚ÌÄ‹N“®‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv427 (‚`)È“d—Íƒ‚[ƒh‹­§•œ‹A‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv428 (‚`)‹ßÚƒZƒ“ƒTŒÌá‰ñ”", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv429 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv430 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv431 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv432 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv433 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv434 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv435 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv436 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv437 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv438 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv439 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv440 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv441 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv442 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv443 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv444 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv445 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv446 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv447 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv448 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv449 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv450 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv451 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv452 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv453 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv454 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv455 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv456 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv457 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv458 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv459 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv460 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv461 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv462 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv463 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv464 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv465 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv466 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv467 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv468 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv469 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv470 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv471 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv472 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv473 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv474 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv475 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv476 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv477 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv478 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv479 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv480 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv481 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv482 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv483 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv484 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv485 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv486 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv487 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv488 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv489 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv490 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv491 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv492 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv493 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv494 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv495 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv496 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv497 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv498 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv499 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian), _
            New XlsField(8*4, "D", 1, " "c, "WŒv500 i‹ó‚«j", Nothing, XlsByteOrder.LittleEndian)}}

    Shared Sub New()
        For k As Integer = 0 To 1
            oFieldRefs(k) = New Dictionary(Of String, FieldRef)
            Dim bits As Integer = 0
            For i As Integer = 0 To oFields(k).Length - 1
                Dim oField As XlsField = oFields(k)(i)
                oFieldRefs(k).Add(oField.MetaName, New FieldRef(oField, bits, i))
                bits += oField.ElementBits * oField.ElementCount
            Next i
            totalBitCount(k) = bits
        Next k
    End Sub

    Public Shared ReadOnly Property RecordLengthInBits(ByVal k As Integer) As Integer
        Get
            Return totalBitCount(k)
        End Get
    End Property

    Public Shared ReadOnly Property RecordLengthInBytes(ByVal k As Integer) As Integer
        Get
            Return (totalBitCount(k) + 7) \ 8
        End Get
    End Property

    Public Shared ReadOnly Property Fields(ByVal k As Integer) As XlsField()
        Get
            Return oFields(k)
        End Get
    End Property

    Public Shared ReadOnly Property Field(ByVal k As Integer, ByVal sMetaName As String) As XlsField
        Get
            Return oFieldRefs(k)(sMetaName).Field
        End Get
    End Property

    Public Shared Function FieldIndexOf(ByVal k As Integer, ByVal sMetaName As String) As Integer
        Return oFieldRefs(k)(sMetaName).Index
    End Function

    Public Shared Function GetFieldValueFromBytes(ByVal k As Integer, ByVal sMetaName As String, ByVal oBytes As Byte()) As String
        Dim oRef As FieldRef = oFieldRefs(k)(sMetaName)
        Return oRef.Field.CreateValueFromBytes(oBytes, oRef.BitOffset)
    End Function

    Public Shared Sub SetFieldValueToBytes(ByVal k As Integer, ByVal sMetaName As String, ByVal sValue As String, ByVal oBytes As Byte())
        Dim oRef As FieldRef = oFieldRefs(k)(sMetaName)
        oRef.Field.CopyValueToBytes(sValue, oBytes, oRef.BitOffset)
    End Sub

    Public Shared Sub InitBaseHeaderFields(ByVal k As Integer, ByVal machine As EkCode, ByVal d As DateTime, ByVal seqNum As UInteger, ByVal oBytes As Byte())
        SetFieldValueToBytes(k, "Šî–{ƒwƒbƒ_[ ƒf[ƒ^Ží•Ê", If(k = 0, "A7", "A8"), oBytes)
        SetFieldValueToBytes(k, "Šî–{ƒwƒbƒ_[ ‰wƒR[ƒh", machine.ToString("%3R-%3S"), oBytes)
        SetFieldValueToBytes(k, "Šî–{ƒwƒbƒ_[ ˆ—“úŽž", d.ToString("yyyyMMddHHmmss"), oBytes)
        SetFieldValueToBytes(k, "Šî–{ƒwƒbƒ_[ ƒR[ƒi[", machine.ToString("%C"), oBytes)
        SetFieldValueToBytes(k, "Šî–{ƒwƒbƒ_[ †‹@", machine.ToString("%U"), oBytes)
        SetFieldValueToBytes(k, "Šî–{ƒwƒbƒ_[ ƒV[ƒPƒ“ƒXNo", seqNum.ToString(), oBytes)
        SetFieldValueToBytes(k, "Šî–{ƒwƒbƒ_[ ƒo[ƒWƒ‡ƒ“", "01", oBytes)
    End Sub

    Public Shared Sub InitCommonPartFields(ByVal k As Integer, ByVal machine As EkCode, ByVal d As DateTime, ByVal oBytes As Byte())
        SetFieldValueToBytes(k, "‹¤’Ê•” WŒvŠJŽn“úŽž", d.ToString("yyyyMMddHHmmss"), oBytes)
        SetFieldValueToBytes(k, "‹¤’Ê•” WŒvI—¹(ŽûW)“úŽž", "00000000000000", oBytes)
        SetFieldValueToBytes(k, "‹¤’Ê•” ‰üŽD‘¤”À‘—•”“_ŒŸ“úŽž", "00000000000000", oBytes)
        SetFieldValueToBytes(k, "‹¤’Ê•” WŽD‘¤”À‘—•”“_ŒŸ“úŽž", "00000000000000", oBytes)
        'TODO: ‚±‚Ì‚Q€–Ú‚Í‘‹ˆŒü‚¯‚ÌŽÀ‘•‚É‚È‚Á‚Ä‚¨‚èA‰üŽD‹@—p‚É‚Â‚­‚è‚È‚¨‚µ‚½‚¢‚ªA‚à‚Æ‚É‚È‚éî•ñ‚ª‚È‚¢‚Ì‚ÅA‚±‚Ì‚Ü‚Ü‚Å‚æ‚¢‹C‚àB
        SetFieldValueToBytes(k, "‹¤’Ê•” ‰üŽD‘¤”À‘—•””Ô†", machine.ToString("%3R%3S%2C%2U"), oBytes)
        SetFieldValueToBytes(k, "‹¤’Ê•” WŽD‘¤”À‘—•””Ô†", machine.ToString("%3R%3S%2C%2U"), oBytes)
        SetFieldValueToBytes(k, "‹¤’Ê•” ‰üŽD‘¤ŒŸ’mƒZƒ“ƒTƒŒƒxƒ‹", Field(k, "‹¤’Ê•” ‰üŽD‘¤ŒŸ’mƒZƒ“ƒTƒŒƒxƒ‹").CreateDefaultValue(), oBytes)
        SetFieldValueToBytes(k, "‹¤’Ê•” WŽD‘¤ŒŸ’mƒZƒ“ƒTƒŒƒxƒ‹", Field(k, "‹¤’Ê•” WŽD‘¤ŒŸ’mƒZƒ“ƒTƒŒƒxƒ‹").CreateDefaultValue(), oBytes)
        SetFieldValueToBytes(k, "‹¤’Ê•” —\”õ", Field(k, "‹¤’Ê•” —\”õ").CreateDefaultValue(), oBytes)
    End Sub

    Public Shared Sub UpdateSummaryFields(ByVal oBytes As Byte()())
        'TODO: ‰üŽD‹@—p‚É‚Â‚­‚è‚È‚¨‚·B
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 293).MetaName, GetSummary(294, 344, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 273).MetaName, GetSummary(274, 282, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 203).MetaName, GetSummary(204, 262, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 184).MetaName, GetSummary(185, 192, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 149).MetaName, GetSummary(150, 173, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 128).MetaName, GetSummary(129, 138, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 112).MetaName, GetSummary(113, 117, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 9).MetaName, GetSummary(99, 101, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 8).MetaName, GetSummary(97, 98, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 7).MetaName, GetSummary(79, 86, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 6).MetaName, GetSummary(57, 68, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 5).MetaName, GetSummary(41, 46, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 4).MetaName, GetSummary(23, 30, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 3).MetaName, GetFieldValueFromBytes(Fields(AggregateFieldsOrigin + 7).MetaName, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 2).MetaName, GetSummary(New Integer() {6, 8}, oBytes), oBytes)
        'SetFieldValueToBytes(Fields(AggregateFieldsOrigin + 1).MetaName, GetSummary(New Integer() {4, 5, 9}, oBytes), oBytes)
    End Sub

    Private Shared Function GetSummary(ByVal k As Integer, ByVal firstAggregateNumber As Integer, ByVal lastAggregateNumber As Integer, ByVal oBytes As Byte()) As String
        Dim sum As Long = 0
        For i As Integer = AggregateFieldsOrigin + firstAggregateNumber To AggregateFieldsOrigin + lastAggregateNumber
            sum += Long.Parse(GetFieldValueFromBytes(k, oFields(k)(i).MetaName, oBytes))
        Next i
        If sum > UInteger.MaxValue Then
            sum = UInteger.MaxValue
        End If
        Return sum.ToString()
    End Function

    Private Shared Function GetSummary(ByVal aggregateIds As AggregateIdentifier(), ByVal oBytes As Byte()()) As String
        Dim sum As Long = 0
        For Each id As AggregateIdentifier In aggregateIds
            Dim k As Integer = id.Kind
            Dim i As Integer = AggregateFieldsOrigin + id.Number
            sum += Long.Parse(GetFieldValueFromBytes(k, oFields(k)(i).MetaName, oBytes(k)))
        Next id
        If sum > UInteger.MaxValue Then
            sum = UInteger.MaxValue
        End If
        Return sum.ToString()
    End Function

    Private Structure AggregateIdentifier
        Public Kind As Integer
        Public Number As Integer
        Public Sub New(ByVal k As Integer, ByVal n As Integer)
            Kind = k
            Number = n
        End Sub
    End Structure

End Class
