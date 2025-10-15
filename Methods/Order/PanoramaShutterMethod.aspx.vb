Imports System.Web.Services
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic



Partial Class Methods_Method 
    Inherits System.Web.UI.Page
    Shared publicCfg As New PublicConfig()

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetDesignType(ByVal designId As String) As Object
        Dim designName As String = publicCfg.GetDesignName(designId)
        Dim result As New Dictionary(Of String, String) From {
            {"designName", designName}
        }
        Return result
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function GetHeaderData(ByVal headerId As String) As Object
        Dim orderno As String = publicCfg.GetOrderNo(headerId)
        Dim ordercust As String = publicCfg.GetOrderCust(headerId)
        Dim result As New Dictionary(Of String, String) From {
            {"orderNo", orderno},
            {"orderCust", ordercust}
        }
        Return result
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindBlindType(ByVal designId As String) As Object
        Try
            Dim dataBlindType As DataSet = publicCfg.GetListData("SELECT * FROM Blinds WHERE DesignId='" + designId + "' AND Active=1 ORDER BY Name ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If dataBlindType IsNot Nothing AndAlso dataBlindType.Tables.Count > 0 Then
                For Each row As DataRow In dataBlindType.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("Name").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function


    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindColourType(ByVal designId As String, ByVal blindId As String) As Object
        Try
            Dim dataBlindType As DataSet = publicCfg.GetListData("SELECT Id, ColourType FROM HardwareKits WHERE DesignId = '" + designId + "' AND BlindId='" + UCase(blindId).ToString() + "' AND Active=1 ORDER BY ColourType ASC")
            Dim list As New List(Of Dictionary(Of String, String))()
            If dataBlindType IsNot Nothing AndAlso dataBlindType.Tables.Count > 0 Then
                For Each row As DataRow In dataBlindType.Tables(0).Rows
                    Dim result As New Dictionary(Of String, String) From {
                        {"value", row("Id").ToString()},
                        {"text", row("ColourType").ToString()}
                    }
                    list.Add(result)
                Next
            End If
            Return list
        Catch ex As Exception
            ' Return sebagai objek error agar bisa ditangani di sisi client
            Return New With {.error = ex.Message}
        End Try
    End Function


    '#------------------------||BindItemOrder||------------------------#
    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function BindItemOrder(ByVal itemId As String) As Object
        Try
            ' Gunakan parameterized query (idealnya pakai SqlParameter, ini simulasi fungsi GetListData Anda)
            Dim dataBlindType As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id = '" + itemId + "'")

            Dim data As DataSet = DirectCast(dataBlindType, DataSet)

            Dim resultList As New List(Of Dictionary(Of String, String))()

            If data IsNot Nothing AndAlso data.Tables.Count > 0 Then
                For Each row As DataRow In data.Tables(0).Rows
                    Dim dict As New Dictionary(Of String, String)()
                    For Each col As DataColumn In data.Tables(0).Columns
                        dict(col.ColumnName) = row(col).ToString()
                    Next
                    resultList.Add(dict)
                Next
            End If

            Return resultList
        Catch ex As Exception
            ' Tangani error agar bisa dikenali di JavaScript
            Return New With {.error = True, .message = ex.Message}
        End Try
    End Function



    '#--------------|| inisiasi field form ||--------------#
    Public Class FormData
        Public Property blindtype As String
        Public Property colourtype As String
        Public Property qty As String
        Public Property room As String
        Public Property mounting As String
        Public Property width As String
        Public Property drop As String
        Public Property louvresize As String
        Public Property louvreposition As String
        Public Property midrailheight1 As String
        Public Property midrailheight2 As String
        Public Property midrailcritical As String
        Public Property panelqty As String
        Public Property joinedpanels As String
        Public Property hingecolour As String
        Public Property semiinsidemount As String
        Public Property customheaderlength As String
        Public Property layoutcode As String
        Public Property layoutcodecustom As String
        Public Property frametype As String
        Public Property frameleft As String
        Public Property frameright As String
        Public Property frametop As String
        Public Property framebottom As String
        Public Property bottomtracktype As String
        Public Property bottomtrackrecess As String
        Public Property buildout As String
        Public Property buildoutposition As String
        Public Property samesizepanel As String
        Public Property gap1 As String
        Public Property gap2 As String
        Public Property gap3 As String
        Public Property gap4 As String
        Public Property gap5 As String
        Public Property horizontaltpostheight As String
        Public Property horizontaltpost As String
        Public Property tiltrodtype As String
        Public Property tiltrodsplit As String
        Public Property splitheight1 As String
        Public Property splitheight2 As String
        Public Property reversehinged As String
        Public Property pelmetflat As String
        Public Property extrafascia As String
        Public Property hingesloose As String
        Public Property cutout As String
        Public Property specialshape As String
        Public Property templateprovided As String
        Public Property markup As String
        Public Property notes As String
        Public Property headerid As String
        Public Property itemaction As String
        Public Property itemid As String
        Public Property designid As String
        Public Property loginid As String
        Public Property rolename As String
    End Class


    '#--------------|| define class response ||--------------#
    Public Class ErrorDetail
        Public Property message As String
        Public Property field As String
    End Class

    Public Class ErrorResponse
        Public Property [error] As ErrorDetail
    End Class

    Public Class SuccessResponse
        Public Property success As String
    End Class

    Private Shared Function IsSuccess(msg As String, field As String) As String
    
    End Function

    Private Shared Function IsError(msg As String, field As String) As ErrorResponse
        Return New ErrorResponse With {.error = New ErrorDetail With { .message = msg, .field = field}}
    End Function

    <WebMethod()>
    Public Shared Function SubmitForm(data As FormData) As Object
        '#-----------------------|| validation set rules ||-----------------------#
        '#-----------------------|| blindtype ||-----------------------#
        If String.IsNullOrEmpty(data.blindtype) Then Return IsError("type is required !", "blindtype")

        '#-----------------------|| colourtype ||-----------------------#
        If String.IsNullOrEmpty(data.colourtype) Then Return IsError("colour is required !", "colourtype")

        Dim blindName As String = publicCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + data.blindtype + "'")

        Dim panelQty As Integer = 0
        Dim trackQty As Integer = 0
        Dim trackLength As Integer = 0
        Dim hingeQtyPerPanel As Integer = 0
        Dim panelQtyWithHinge As Integer = 0

        '#-----------------------|| qty ||-----------------------#
        Dim qty As Integer
        If String.IsNullOrEmpty(data.qty) Then Return IsError("qty is required !", "qty")
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return IsError("qty must be a positive integer !", "qty")
        If qty > 5 Then Return IsError("qty must be less than or equal to 5 !", "qty")

        '#-----------------------|| room ||-----------------------#
        If String.IsNullOrEmpty(data.room) Then Return IsError("room to install is required !", "room")
        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return IsError("ROOM TO INSTALL IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+", "room")
        End If
        
        '#-----------------------|| mounting ||-----------------------#
        If String.IsNullOrEmpty(data.mounting) Then Return IsError("mounting is required !", "mounting")
        

        '#-----------------------|| width ||-----------------------#
        Dim width As Integer
        If String.IsNullOrEmpty(data.width) Then Return IsError("width is required !", "width")
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return IsError("width must be a positive integer !", "width")

        '#-----------------------|| drop ||-----------------------#
        Dim drop As Integer
        If String.IsNullOrEmpty(data.drop) Then Return IsError("drop is required !", "drop")
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return IsError("drop must be a positive integer !", "drop")

        '#-----------------------|| louvresize ||-----------------------#
        If String.IsNullOrEmpty(data.louvresize) Then Return IsError("louvre size is required !", "louvresize")

        '#-----------------------|| louvreposition ||-----------------------#
        If blindName = "Track Sliding" Then
            If String.IsNullOrEmpty(data.louvreposition) Then Return IsError("louvre position is required !", "louvreposition")
            If data.louvreposition = "Open" And data.louvresize = "114" Then Return IsError("the louvre size & louvre position  you selected cannot be processed !", "louvreposition")
        End if

        '#-----------------------|| midrailheight ||-----------------------#
        Dim midrailHeight1 As Integer
        Dim midrailHeight2 As Integer

        If Not String.IsNullOrEmpty(data.midrailheight1) Then '#if not null or empty
            If Not Integer.TryParse(data.midrailheight1, midrailHeight1) OrElse midrailHeight1 < 0 Then Return IsError("please check your midrail height 1 order !", "midrailheight1")
        End If

        if Not String.IsNullOrEmpty(data.midrailheight2) Then '#if not null or empty
            If Not Integer.TryParse(data.midrailheight2, midrailHeight2) OrElse midrailHeight2 < 0 Then Return IsError("please check your midrail height 2 order !", "midrailheight2")
        End If

        If midrailHeight1 >= drop Then Return IsError("the height of the midrail 1 must be less than the height !", "midrailheight1")
        If midrailHeight2 >= drop Then Return IsError("the height of the midrail 2 must be less than the height !", "midrailheight2")

        If drop > 1500 Then
            If midrailHeight1 = 0 Then Return IsError("midrail height 1 is required. <br> maximum panel height is 1500mm !", "midrailheight1")
        End If

        If midrailHeight1 > 0 And midrailHeight2 = 0 Then
            If midrailHeight1 > 1500 Then Return IsError("maximum midrail height 1 is 1500mm !", "midrailheight1")
            If drop - midrailHeight1 > 1500 Then Return IsError("maximum midrail height 2 is 1500mm !", "midrailheight2")
        End If

        If midrailHeight1 > 0 And midrailHeight2 > 0 Then
            If midrailHeight1 = midrailHeight2 Then Return IsError("midrail height is in the same position. please change midrail height position 2 !", "midrailheight2")
            If midrailHeight1 > midrailHeight2 Then Return IsError("the height of midrail 1 should not be greater than the height of midrail 2 !", "midrailheight1")
            If midrailHeight1 > 1500 Then Return IsError("maximum midrail height 1 is 1500mm !", "midrailheight1")
            If midrailHeight2 - midrailHeight1 > 1500 Then Return IsError("maximum midrail height 2 is 1500mm !", "midrailheight2")
            If drop - midrailHeight2 > 1500 Then Return IsError("maximum midrail height 2 is 1500mm !", "midrailheight2")
        End IF

        '#-----------------------|| hingecolour ||-----------------------#
        IF blindName = "Hinged" Or blindName = "Hinged Bi-fold" Or blindName = "Track Bi-fold" Then
            If String.IsNullOrEmpty(data.hingecolour) Then Return IsError("hinge colour is required !", "hingecolour")
        End If

        '#-----------------------|| hingecolour & customheaderlength ||-----------------------#
        Dim headerLength As Integer
        IF blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            If data.joinedpanels = "Yes" And String.IsNullOrEmpty(data.hingecolour) Then Return IsError("hinge colour is required !", "hingecolour")

            If Not String.IsNullOrEmpty(data.customheaderlength) Then
                If Not Integer.TryParse(data.customheaderlength, headerLength) OrElse headerLength < 0 Then Return IsError("please check your custom header length !", "customheaderlength")
                If headerLength > 0 And headerLength > width * 2 Then Return IsError("minimum custom header length is width x 2 !", "customheaderlength")
            End If
        End If

        '#-----------------------|| layoutcode & layoutcodecustom & louvreposition & frametype & frameleft & frameright & frametop & framebottom ||-----------------------#

        Dim layoutCode As String = data.layoutcode
        IF data.layoutcode = "Other" Then layoutCode = data.layoutcodecustom
        
        If Not blindName = "Panel Only" Then
            If String.IsNullOrEmpty(data.layoutcode) Then Return IsError("layout code is required !", "layoutcode")
            If data.layoutcode = "Other" And String.IsNullOrEmpty(data.layoutcodecustom) Then Return IsError("layout code custom is required !", "layoutcodecustom")

            If blindName = "Hinged" Then
                If layoutCode.Contains("LL") Or layoutCode.Contains("RR") Then Return IsError("your layout code cannot be used !", "layoutcode")
            End If

            If blindName = "Hinged Bi-fold" Then
                If layoutCode.Contains("LLL") Or layoutCode.Contains("RRR") Then Return IsError("your layout code cannot be used !", "layoutcode")
            End If

            If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
                If layoutCode.Contains("RL")  Then Return IsError("your layout code cannot be used !", "layoutcode")

                Dim checkLayoutD As Boolean = publicCfg.CheckStringLayoutD(layoutCode)
                If checkLayoutD = False Then Return IsError("your layout code cannot be used !", "layoutcode")
            End If

            If BlindName = "Track Bi-fold" Then
                Dim stringL As Integer = layoutCode.Split("L").Length - 1
                Dim stringR As Integer = layoutCode.Split("R").Length - 1
                If Not stringL Mod 2 = 0 Then Return IsError("layout code L should not be odd !", "layoutcode")
                If Not stringR Mod 2 = 0 Then Return IsError("layout code R should not be odd !", "layoutcode")
            End If

            If blindName = "Track Sliding" Then
                If InStr(layoutCode, "M") > 0 And Not data.louvreposition = "Closed" Then Return IsError("louvre position should be closed !", "louvreposition")
            End If

            If String.IsNullOrEmpty(data.frametype) Then Return IsError("frame type is required !", "frametype")
            If String.IsNullOrEmpty(data.frameleft) Then Return IsError("frame left is required !", "frameleft")
            If String.IsNullOrEmpty(data.frameright) Then Return IsError("frame right is required !", "frameright")
            If String.IsNullOrEmpty(data.frametop) Then Return IsError("frame top is required !", "frametop")
            If String.IsNullOrEmpty(data.framebottom) Then Return IsError("frame bottom is required !", "framebottom")
        End If

        '#-----------------------|| bottomtracktype ||-----------------------#
        If blindName = "Track Bi-fold" Or blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            If String.IsNullOrEmpty(data.bottomtracktype) Then Return IsError("bottom track type is required !", "bottomtracktype")
        End If

        '#-----------------------|| buildoutposition ||-----------------------#
        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            If (data.frametype = "Small Bullnose Z Frame" Or data.frametype = "Large Bullnose Z Frame" Or data.frametype = "Colonial Z Frame" Or data.frametype = "Regal Z Frame") And Not String.IsNullOrEmpty(data.buildout) And String.IsNullOrEmpty(data.buildoutposition) Then
                Return IsError("buildout position is required !", "buildoutposition")
            End If
        End If

        '#-----------------------|| gap1, gap2, gap3, gap4, gap5 ||-----------------------#
        Dim gap1 As Integer
        Dim gap2 As Integer
        Dim gap3 As Integer
        Dim gap4 As Integer
        Dim gap5 As Integer

        If Not String.IsNullOrEmpty(data.gap1) Then
            If Not Integer.TryParse(data.gap1, gap1) OrElse gap1 <= 0 Then Return IsError("please check your gap / t-post 1 !", "gap1")
        End If

        If Not String.IsNullOrEmpty(data.gap2) Then
            If Not Integer.TryParse(data.gap2, gap2) OrElse gap2 <= 0 Then Return IsError("please check your gap / t-post 2 !", "gap2")
        End If

        If Not String.IsNullOrEmpty(data.gap3) Then
            If Not Integer.TryParse(data.gap3, gap3) OrElse gap3 <= 0 Then Return IsError("please check your gap / t-post 3 !", "gap3")
        End If

        If Not String.IsNullOrEmpty(data.gap4) Then
            If Not Integer.TryParse(data.gap4, gap4) OrElse gap4 <= 0 Then Return IsError("please check your gap / t-post 4 !", "gap4")
        End If

        If Not String.IsNullOrEmpty(data.gap5) Then
            If Not Integer.TryParse(data.gap5, gap5) OrElse gap5 <= 0 Then Return IsError("please check your gap / t-post 5 !", "gap5")
        End If

        '#-----------------------|| horizontaltpostheight & horizontaltpost ||-----------------------#
        Dim horizontalHeight As Integer
        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            If Not String.IsNullOrEmpty(data.horizontaltpostheight) Then
                If Not Integer.TryParse(data.horizontaltpostheight, horizontalHeight) OrElse horizontalHeight < 0 Then
                   Return IsError("please check your horizontal t-post height !", "horizontaltpostheight")
                End If
                If horizontalHeight > 0 And String.IsNullOrEmpty(data.horizontaltpost) Then
                    Return IsError("horizontal t-post is required !", "horizontaltpost")
                End If
            End If
        End If

        '#-----------------------|| panelqty ||-----------------------#
        If blindName = "Panel Only" AndAlso String.IsNullOrEmpty(data.panelqty) Then Return IsError("panel qty is required !", "panelqty")

        '#-----------------------|| tiltrodtype ||-----------------------#
        If String.IsNullOrEmpty(data.tiltrodtype) Then Return IsError("tiltrod type is required !", "tiltrodtype")

        '#-----------------------|| tiltrodsplit ||-----------------------#
        Dim splitheight1 As Integer
        Dim splitheight2 As Integer

        If data.tiltrodsplit = "Other" Then
            If String.IsNullOrEmpty(data.splitheight1) Then Return IsError("split height 1 is required !", "splitheight1")
            If Not Integer.TryParse(data.splitheight1, splitheight1) OrElse splitheight1 <= 0 Then Return IsError("please check your split height 1 !", "splitheight1")
            If String.IsNullOrEmpty(data.splitheight2) Then Return IsError("split height 2 is required !", "splitheight2")
            If Not Integer.TryParse(data.splitheight2, splitheight2) OrElse splitheight2 <= 0 Then Return IsError("please check your split height 2 !", "splitheight2")
        End If
        
        '#-----------------------------------------|| Deduction||---------------------------------------------#
        Dim datacheckPanelQty As String() = {blindName, data.panelqty, layoutCode, horizontalHeight}
        panelQty = publicCfg.GetPanelQty(datacheckPanelQty)

        '#----------------------------|| Deduction ||----------------------------#
        If data.rolename = "Customer" Then
            Dim dataWidthDeductions As String() = {blindName, "All", width, data.mounting, layoutCode, data.frametype, data.frameleft, data.frameright, panelQty}
            Dim dataHeightDeductions As String() = {blindName, drop, data.mounting, data.frametype, data.frametop, data.framebottom, data.bottomtracktype, data.horizontaltpost}

            Dim widthDeductions As Decimal = publicCfg.WidthDeductPanorama(dataWidthDeductions)
            Dim panelWidth As Decimal = widthDeductions / panelQty

            Dim heightDeduct As Decimal = publicCfg.HeightDeductPanorama(dataHeightDeductions)
            Dim panelHeight As Decimal = heightDeduct

            If blindName = "Panel Only" Then
                If width < 200 Then Return IsError("minimum panel width is 200mm !", "width")
                If width > 900 Then Return IsError("maximum panel width is 900mm !", "width")
                If drop < 282 And data.louvresize = "63" Then Return IsError("minimum panel height is 282mm !", "drop")
                If drop < 333 And data.louvresize = "89" Then Return IsError("minimum panel height is 333mm !", "drop")
                If drop < 384 And data.louvresize = "114" Then Return IsError("minimum panel height is 384mm !", "drop")
                If drop > 2500 Then Return IsError("maximum panel height is 2500mm !", "drop")
            End If

            If blindName = "Hinged" Then
                If panelWidth < 200 Then Return IsError("minimum panel width is 200mm !", "width")
                If panelWidth > 900 Then Return IsError("maximum panel width is 900mm !", "width")

                If panelHeight < 282 And data.louvresize = "63" Then Return IsError("minimum panel height is 282mm !", "drop")
                If panelHeight < 333 And data.louvresize = "89" Then Return IsError("minimum panel height is 333mm !", "drop")
                If panelHeight < 384 And data.louvresize = "114" Then Return IsError("minimum panel height is 384mm !", "drop")
                If panelHeight > 1900 And blindName = "Hinged Bi-fold" And (data.framebottom = "No" Or data.framebottom ="L Striker Plate") Then
                    Return IsError("maximum panel height is 1900mm !", "drop")
                End If
                If panelHeight > 2500 Then
                    Return IsError("maximum panel height is 2500mm !", "drop")
                End If
            End If

            If blindName = "Hinged Bi-fold" Then
                If panelWidth < 200 Then Return IsError("minimum panel width is 200mm !", "width")
                If panelWidth > 900 Then Return IsError("maximum panel width is 900mm !", "width")

                If panelHeight < 282 And data.louvresize = "63" Then Return IsError("minimum panel height is 282mm !", "drop")
                If panelHeight < 333 And data.louvresize = "89" Then Return IsError("minimum panel height is 333mm !", "drop")
                If panelHeight < 384 And data.louvresize = "114" Then Return IsError("minimum panel height is 384mm !", "drop")
                If panelHeight > 1900 And (data.framebottom = "No") Then Return IsError("maximum panel height is 1900mm !", "drop")
                If panelHeight > 2500 Then Return IsError("maximum panel height is 2500mm !", "drop")
            End If

            If blindName = "Track Bi-fold" Then
                If panelWidth < 200 Then Return IsError("minimum panel width is 200mm !", "width")
                If panelWidth > 900 Then Return IsError("maximum panel width is 900mm !", "width")

                If panelHeight < 282 And data.louvresize = "63" Then Return IsError("minimum panel height is 282mm !", "drop")
                If panelHeight < 333 And data.louvresize = "89" Then Return IsError("minimum panel height is 333mm !", "drop")
                If panelHeight < 384 And data.louvresize = "114" Then Return IsError("minimum panel height is 384mm !", "drop")
                If panelHeight > 2500 Then Return IsError("maximum panel height is 2500mm !", "drop")
            End If

            If blindName = "Track Sliding" Then
                If panelWidth < 200 Then Return IsError("minimum panel width is 200mm !", "width")
                If panelWidth > 900 Then Return IsError("maximum panel width is 900mm !", "width")

                If panelHeight < 282 And data.louvresize = "63" Then Return IsError("minimum panel height is 282mm !", "drop")
                If panelHeight < 333 And data.louvresize = "89" Then Return IsError("minimum panel height is 333mm !", "drop")
                If panelHeight < 384 And data.louvresize = "114" Then Return IsError("minimum panel height is 384mm !", "drop")
                If panelHeight > 2500 Then Return IsError("maximum panel height is 2500mm !", "drop")
            End If

            If blindName = "Track Sliding Single Track" Then
                If panelWidth < 200 Then Return IsError("minimum panel width is 200mm !", "width")
                If panelWidth > 900 Then Return IsError("maximum panel width is 900mm !", "width")

                If panelHeight < 282 And data.louvresize = "63" Then Return IsError("minimum panel height is 282mm !", "drop")
                If panelHeight < 333 And data.louvresize = "89" Then Return IsError("minimum panel height is 333mm !", "drop")
                If panelHeight < 384 And data.louvresize = "114" Then Return IsError("minimum panel height is 384mm !", "drop")
                If panelHeight > 2500 Then Return IsError("maximum panel height is 2500mm !", "drop")
            End If

            If blindName = "Fixed" Then
                If panelWidth < 200 Then Return IsError("minimum panel width is 200mm !", "width")
                If panelWidth > 900 Then Return IsError("maximum panel width is 900mm !", "width")

                If panelHeight < 282 And data.louvresize = "63" Then Return IsError("minimum panel height is 282mm !", "drop")
                If panelHeight < 333 And data.louvresize = "89" Then Return IsError("minimum panel height is 333mm !", "drop")
                If panelHeight < 384 And data.louvresize = "114" Then Return IsError("minimum panel height is 384mm !", "drop")
                If data.frametype = "U Channel" And panelHeight > 2527 Then Return IsError("maximum panel height is 2527mm !", "drop")
                If data.frametype = "19x19 Light Block" And panelHeight > 2506 Then Return IsError("maximum panel height is 2506mm !", "drop")
            End If

        End If

        '#-------------------------|| Gap Position ||---------------------------#
        ' If String.IsNullOrEmpty(data.samesizepanel) And roleName = "Customer" Then
        '     If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
        '         Dim pemisah As Char() = {"T"c, "C"c, "B"c, "G"c}
        
        '         Dim gaps As Integer() = {gap1, gap2, gap3, gap4, gap5}
        '         Dim totalWidth As Integer = width
        
        '         Dim sections As New List(Of String)
        '         Dim startIndex As Integer = 0
        '         Dim totalPemisah As Integer = 0
        
        '         For i As Integer = 0 To layoutCode.Length - 1
        '             If pemisah.Contains(layoutCode(i)) Then
        '                 totalPemisah += 1
        '                 Dim endIndex As Integer = i
        '                 sections.Add(layoutCode.Substring(startIndex, (endIndex - startIndex) + 1))
        '                 startIndex = i
        '             End If
        '         Next
        
        '         If startIndex < layoutCode.Length Then
        '             sections.Add(layoutCode.Substring(startIndex))
        '         End If
        
        '         If totalPemisah > 0 Then
        '             For g As Integer = 0 To totalPemisah - 1
        '                 If gaps(g) <= 0 Then
        '                     Return $"GAP {g + 1} IS REQUIRED. PLEASE INPUT A VALID VALUE."
        '                 End If
        '             Next
        
        '             For g As Integer = 0 To totalPemisah - 2
        '                 If gaps(g) > gaps(g + 1) Then
        '                     Return $"GAP {g + 1} ({gaps(g)}) CANNOT BE GREATER THAN GAP {g + 2} ({gaps(g + 1)})."
        '                 End If
        '             Next
        
        '             Dim sumGapUsed As Integer = 0
        
        '             For idx As Integer = 0 To sections.Count - 1
        '                 Dim section As String = sections(idx)
        
        '                 Dim panelCount As Integer = section.Count(Function(ch) "LRFM".Contains(ch))
        
        '                 Dim currentGap As Integer
        '                 If idx = sections.Count - 1 Then
        '                     currentGap = totalWidth - sumGapUsed
        '                 Else
        '                     If idx = 0 Then
        '                         currentGap = gaps(0)
        '                         sumGapUsed += currentGap
        '                     Else
        '                         currentGap = gaps(idx) - gaps(idx - 1)
        '                         sumGapUsed += currentGap
        '                     End If
        '                 End If
        
        '                 Dim dataGap As Object() = {
        '                     blindName, "Gap", currentGap, data.mounting,
        '                     section, data.frametype, data.frameleft,
        '                     data.frameright, panelCount
        '                 }
        
        '                 Dim widthDeduct As Decimal = orderCfg.WidthDeductPanorama(dataGap)
        
        '                 If widthDeduct < 200 Then
        '                     Return $"MINIMUM PANEL WIDTH IS 200MM.<br />FINAL PANEL WIDTH IN SECTION {idx + 1} IS {widthDeduct} !"
        '                 End If
        '                 If blindName = "Hinged Bi-fold" AndAlso widthDeduct > 650 Then
        '                     Return $"MAXIMUM PANEL WIDTH FOR HINGED BI-FOLD IS 650MM.<br />FINAL PANEL WIDTH IN SECTION {idx + 1} IS {widthDeduct} !"
        '                 End If
        '                 If widthDeduct > 900 Then
        '                     Return $"MAXIMUM PANEL WIDTH IS 900MM.<br />FINAL PANEL WIDTH IN SECTION {idx + 1} IS {widthDeduct} !"
        '                 End If
        '             Next
        '         End If
        '     End If
        ' End If

        '#--------------------------|| notes ||--------------------------#
        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.Trim().Length > 1000 Then Return IsError("notes must be less than 1000 characters !", "notes")
        End If

        '#--------------------------|| notes ||--------------------------#
        Dim markup As Integer
        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return IsError("please check your markup !", "markup")
        End If

        '#------------------------------------------------|| Prepare Submit ||-------------------------------------------------#
        '#-----------------------------------|| Set default values before submission ||----------------------------------------#
        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        '#----------------------------|| Panel Only / Prepare Submit ||----------------------------#
        If blindName = "Panel Only" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.hingecolour = ""
            data.semiinsidemount = ""
            headerLength = 0
            data.layoutcode = "" : data.layoutcodecustom = ""
            data.frametype = ""
            data.frameleft = "" : data.frameright = ""
            data.frametop = "" : data.framebottom = ""
            data.bottomtracktype = "" : data.bottomtrackrecess = ""
            data.buildout = "" : data.buildoutposition = ""
            data.samesizepanel = ""
            gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            data.horizontaltpost = "" : horizontalHeight = 0
            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            data.reversehinged = ""
            data.pelmetflat = ""
            data.extrafascia = ""
            data.hingesloose = ""
            If data.specialshape = "" Then data.templateprovided = ""

            trackQty = 0
            trackLength = 0
            hingeQtyPerPanel = 0
            panelQtyWithHinge = 0
        End If

        '#----------------------------|| Hinged Or Hinged Bi-fold / Prepare Submit ||----------------------------#
        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.semiinsidemount = ""
            headerLength = "0"
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = String.Empty
            End If

            data.bottomtracktype = "" : data.bottomtrackrecess = ""
            If data.buildout = "" Or (data.buildout = "Beaded L 48mm" Or data.buildout = "Insert L 50mm" Or data.buildout = "Insert L 63mm" Or data.buildout = "No Frame") Then
                data.buildoutposition = String.Empty
            End If

            If layoutCode.Contains("T") And (layoutCode.Contains("B") Or layoutCode.Contains("C") Or layoutCode.Contains("G")) Then
                data.samesizepanel = String.Empty
            End If

            If data.samesizepanel = "Yes" Or Not layoutCode.Contains("T") Then
                gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            End If

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            If horizontalHeight = 0 Then : data.horizontaltpost = "" : End If
            data.reversehinged = ""
            data.pelmetflat = ""
            data.extrafascia = ""
            If data.specialshape = "" Then data.templateprovided = ""

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5

            Dim countL As Integer = 0
            Dim countR As Integer = 0
            countL = layoutCode.Split("L").Length - 1
            countR = layoutCode.Split("R").Length - 1

            panelQtyWithHinge = countL + countR
        End If

        '#----------------------------|| Track Bi-fold / Prepare Submit ||----------------------------#
        If blindName = "Track Bi-fold" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.horizontaltpost = ""
            data.horizontaltpostheight = "0"
            data.buildout = ""
            data.buildoutposition = ""
            If data.mounting = "Outside" Then data.semiinsidemount = ""

            data.customheaderlength = 0
            trackLength = width
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = String.Empty
            End If
            If data.bottomtracktype = "U Track" Then
                data.bottomtrackrecess = "Yes"
            End If
            data.buildout = "" : data.buildoutposition = ""
            data.samesizepanel = String.Empty

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If
            If data.specialshape = "" Then data.templateprovided = ""

            Dim result1 As Integer = 0
            Dim parts As String() = layoutCode.Split("/"c)
            If parts.Length > 0 Then
                result1 = publicCfg.CountMultiLayout(parts(0), New String() {"L", "R", "F"}) - 1
            End If

            Dim result2 As Integer = 0
            If layoutCode.Contains("/") Then
                Dim partss As String() = layoutCode.Split("/"c)
                If partss.Length > 1 Then
                    result2 = publicCfg.CountMultiLayout(partss(1), New String() {"L", "R", "F"}) - 1
                End If
            End If

            panelQtyWithHinge = result1 + result2

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        '#----------------------------|| Track Sliding / Prepare Submit ||----------------------------#
        If blindName = "Track Sliding" Then
            If data.mounting = "Outside" Then
                data.semiinsidemount = ""
            End If
            If data.joinedpanels = "" Then
                data.hingecolour = ""
                data.hingesloose = ""
            End If

            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = ""
            End If

            If data.bottomtracktype = "U Track" Then
                data.bottomtrackrecess = "Yes"
            End If

            data.buildout = "" : data.buildoutposition = ""
            data.samesizepanel = ""

            data.horizontaltpost = "" : data.horizontaltpostheight = 0
            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight1 = 0
            End If
            data.reversehinged = ""
            If data.specialshape = "" Then data.templateprovided = ""

            Dim countM As Integer = 0
            countM = layoutCode.Split("M").Length - 1

            trackQty = 2
            If countM > 0 Then trackQty = 3

            Dim countFF As Integer = 0
            Dim countMM As Integer = 0
            Dim countBB As Integer = 0
            countFF = layoutCode.Split("FF").Length - 1
            countMM = layoutCode.Split("MM").Length - 1
            countBB = layoutCode.Split("BB").Length - 1

            panelQtyWithHinge = countFF + countMM + countBB

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        '#----------------------------|| Track Sliding Single Track / Prepare Submit ||----------------------------#
        If blindName = "Track Sliding Single Track" Then
            If data.mounting = "Outside" Then
                data.semiinsidemount = ""
            End If
            data.louvreposition = ""
            If data.joinedpanels = "" Then
                data.hingecolour = ""
                data.hingesloose = ""
            End If
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = ""
            End If

            data.buildout = "" : data.buildoutposition = ""
            data.samesizepanel = ""

            If data.bottomtracktype = "U Track" Then
                data.bottomtrackrecess = "Yes"
            End If

            data.horizontaltpostheight = 0 : data.horizontaltpost = ""
            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If
            data.reversehinged = ""
            If data.specialshape = "" Then
                data.templateprovided = ""
            End If
            trackQty = 1

            Dim countFF As Integer = 0
            Dim countMM As Integer = 0
            Dim countBB As Integer = 0
            countFF = layoutCode.Split("FF").Length - 1
            countMM = layoutCode.Split("MM").Length - 1
            countBB = layoutCode.Split("BB").Length - 1

            panelQtyWithHinge = countFF + countMM + countBB

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        '#----------------------------|| Fixed / Prepare Submit ||----------------------------#
        If blindName = "Fixed" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.hingecolour = ""

            data.semiinsidemount = ""
            data.customheaderlength = ""
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = ""
            End If
            data.bottomtracktype = "" : data.bottomtrackrecess = ""
            data.buildout = "" : data.buildoutposition = ""
            data.samesizepanel = ""

            data.horizontaltpostheight = 0 : data.horizontaltpost = ""

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            data.reversehinged = ""
            data.pelmetflat = ""
            data.extrafascia = ""
            data.hingesloose = ""
            If data.specialshape = "" Then data.templateprovided = ""
        End If

        
        Dim priceGroupName As String = "Panorama - " & blindName
        Dim priceGroupId As String = publicCfg.GetPriceGroupId(data.designId,priceGroupName)

        Dim squareMetre As Decimal = Math.Round(width * drop / 1000000, 4)
        Dim linearMetre As Decimal = Math.Round(width / 1000, 4)


        '#----------------------------|| Create Order Item ||----------------------------#
        If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
            For i As Integer = 1 To qty
                Dim itemId As String = publicCfg.CreateOrderItemId()    
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, HeaderId, KitId, SoeKitId, PriceGroupId, BlindNo, Qty, Location, Mounting, Width, [Drop], SemiInsideMount, LouvreSize, LouvrePosition, HingeColour, MidrailHeight1, MidrailHeight2, MidrailCritical, Layout, LayoutSpecial, CustomHeaderLength, FrameType, FrameLeft, FrameRight, FrameTop, FrameBottom, BottomTrackType, BottomTrackRecess, Buildout, BuildoutPosition, PanelQty, TrackQty, TrackLength, PanelSize, HingeQtyPerPanel, PanelQtyWithHinge, LocationTPost1, LocationTPost2, LocationTPost3, LocationTPost4, LocationTPost5, HorizontalTPost, HorizontalTPostHeight, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, TiltrodType, TiltrodSplit, SplitHeight1, SplitHeight2, DoorCutOut, SpecialShape, TemplateProvided, SquareMetre, LinearMetre, Notes, Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active) VALUES (@Id, @HeaderId, @KitId, @SoeKitId, @PriceGroupId, 'Blind 1', @Qty, @Location, @Mounting, @Width, @Drop, @SemiInsideMount, @LouvreSize, @LouvrePosition, @HingeColour, @MidrailHeight1, @MidrailHeight2, @MidrailCritical, @Layout, @LayoutSpecial, @CustomHeaderLength, @FrameType, @FrameLeft, @FrameRight, @FrameTop, @FrameBottom, @BottomTrackType, @BottomTrackRecess, @Buildout, @BuildoutPosition, @PanelQty, @TrackQty, @TrackLength, @PanelSize, @HingeQtyPerPanel, @PanelQtyWithHinge, @LocationTPost1, @LocationTPost2, @LocationTPost3, @LocationTPost4, @LocationTPost5, @HorizontalTPost, @HorizontalTPostHeight, @JoinedPanels, @ReverseHinged, @PelmetFlat, @ExtraFascia, @HingesLoose, @TiltrodType, @TiltrodSplit, @SplitHeight1, @SplitHeight2, @DoorCutOut, @SpecialShape, @TemplateProvided, @SquareMetre, @LinearMetre, @Notes, 0, 0, 0, 0, 0, 0, @MarkUp, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@HeaderId", UCase(data.headerid).ToString())
                        myCmd.Parameters.AddWithValue("@KitId", UCase(data.colourtype).ToString())
                        myCmd.Parameters.AddWithValue("@SoeKitId", "1010")
                        myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(priceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Location", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@SemiInsideMount", data.semiinsidemount)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.louvresize)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.louvreposition)
                        myCmd.Parameters.AddWithValue("@HingeColour", data.hingecolour)
                        myCmd.Parameters.AddWithValue("@MidrailHeight1", midrailHeight1)
                        myCmd.Parameters.AddWithValue("@MidrailHeight2", midrailHeight2)
                        myCmd.Parameters.AddWithValue("@MidrailCritical", If(data.midrailcritical = "", DBNull.Value, data.midrailcritical))
                        myCmd.Parameters.AddWithValue("@Layout", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@LayoutSpecial", data.layoutcodecustom)
                        myCmd.Parameters.AddWithValue("@CustomHeaderLength", headerLength)
                        myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.frameleft)
                        myCmd.Parameters.AddWithValue("@FrameRight", data.frameright)
                        myCmd.Parameters.AddWithValue("@FrameTop", data.frametop)
                        myCmd.Parameters.AddWithValue("@FrameBottom", data.framebottom)
                        myCmd.Parameters.AddWithValue("@BottomTrackType", data.bottomtracktype)
                        myCmd.Parameters.AddWithValue("@BottomTrackRecess", data.bottomtrackrecess)
                        myCmd.Parameters.AddWithValue("@Buildout", data.buildout)
                        myCmd.Parameters.AddWithValue("@BuildoutPosition", If(data.buildoutposition = "", DBNull.Value, data.buildoutposition))
                        myCmd.Parameters.AddWithValue("@PanelQty", panelQty)
                        myCmd.Parameters.AddWithValue("@TrackQty", trackQty)
                        myCmd.Parameters.AddWithValue("@TrackLength", trackLength)
                        myCmd.Parameters.AddWithValue("@PanelSize", data.samesizepanel)
                        myCmd.Parameters.AddWithValue("@HingeQtyPerPanel", hingeQtyPerPanel)
                        myCmd.Parameters.AddWithValue("@PanelQtyWithHinge", panelQtyWithHinge)
                        myCmd.Parameters.AddWithValue("@LocationTPost1", gap1)
                        myCmd.Parameters.AddWithValue("@LocationTPost2", gap2)
                        myCmd.Parameters.AddWithValue("@LocationTPost3", gap3)
                        myCmd.Parameters.AddWithValue("@LocationTPost4", gap4)
                        myCmd.Parameters.AddWithValue("@LocationTPost5", gap5)
                        myCmd.Parameters.AddWithValue("@HorizontalTPost", data.horizontaltpost)
                        myCmd.Parameters.AddWithValue("@HorizontalTPostHeight", horizontalHeight)
                        myCmd.Parameters.AddWithValue("@JoinedPanels", data.joinedpanels)
                        myCmd.Parameters.AddWithValue("@ReverseHinged", data.reversehinged)
                        myCmd.Parameters.AddWithValue("@PelmetFlat", data.pelmetflat)
                        myCmd.Parameters.AddWithValue("@ExtraFascia", data.extrafascia)
                        myCmd.Parameters.AddWithValue("@HingesLoose", data.hingesloose)
                        myCmd.Parameters.AddWithValue("@TiltrodType", data.tiltrodtype)
                        myCmd.Parameters.AddWithValue("@TiltrodSplit", data.tiltrodsplit)
                        myCmd.Parameters.AddWithValue("@SplitHeight1", splitHeight1)
                        myCmd.Parameters.AddWithValue("@SplitHeight2", splitHeight2)
                        myCmd.Parameters.AddWithValue("@DoorCutOut", data.cutout)
                        myCmd.Parameters.AddWithValue("@SpecialShape", data.specialshape)
                        myCmd.Parameters.AddWithValue("@TemplateProvided", data.templateprovided)
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)
                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                publicCfg.ResetPriceDetail(itemId)
                publicCfg.HitungHarga(data.headerid, itemId)
                publicCfg.HitungSurcharge(data.headerid, itemId)
            Next


            Return New SuccessResponse With {
                .success = "Data has been saved successfully."
            }
        End If

        '#----------------------------|| Update Item ||----------------------------#
        If data.itemaction = "EditItem" Or data.itemaction = "ViewItem" Then
            Dim itemId As String = data.itemid
            
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET KitId=@KitId, SoeKitId=@SoeKitId, PriceGroupId=@PriceGroupId, BlindNo='Blind 1', Qty=@Qty, Location=@Location, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, SemiInsideMount=@SemiInsideMount, LouvreSize=@LouvreSize, LouvrePosition=@LouvrePosition, HingeColour=@HingeColour, MidrailHeight1=@MidrailHeight1, MidrailHeight2=@MidrailHeight2, MidrailCritical=@MidrailCritical, Layout=@Layout, LayoutSpecial=@LayoutSpecial, CustomHeaderLength=@CustomHeaderLength, FrameType=@FrameType, FrameLeft=@FrameLeft, FrameRight=@FrameRight, FrameTop=@FrameTop, FrameBottom=@FrameBottom, BottomTrackType=@BottomTrackType, BottomTrackRecess=@BottomTrackRecess, Buildout=@Buildout, BuildoutPosition=@BuildoutPosition, PanelQty=@PanelQty, TrackQty=@TrackQty, TrackLength=@TrackLength, PanelSize=@PanelSize, HingeQtyPerPanel=@HingeQtyPerPanel, PanelQtyWithHinge=@PanelQtyWithHinge, LocationTPost1=@LocationTPost1, LocationTPost2=@LocationTPost2, LocationTPost3=@LocationTPost3, LocationTPost4=@LocationTPost4, LocationTPost5=@LocationTPost5, HorizontalTPost=@HorizontalTPost, HorizontalTPostHeight=@HorizontalTPostHeight, JoinedPanels=@JoinedPanels, ReverseHinged=@ReverseHinged, PelmetFlat=@PelmetFlat, ExtraFascia=@ExtraFascia, HingesLoose=@HingesLoose, TiltrodType=@TiltrodType, TiltrodSplit=@TiltrodSplit, SplitHeight1=@SplitHeight1, SplitHeight2=@SplitHeight2, DoorCutOut=@DoorCutOut, SpecialShape=@SpecialShape, TemplateProvided=@TemplateProvided, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes=@Notes, Matrix=0.00, Charge=0.00, Discount=0.00, TotalMatrix=0.00, TotalCharge=0.00, TotalDiscount=0.00, MarkUp=@MarkUp, Active=1 WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    ' myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@KitId", UCase(data.colourtype).ToString())
                    myCmd.Parameters.AddWithValue("@SoeKitId", "1010")
                    myCmd.Parameters.AddWithValue("@PriceGroupId", UCase(priceGroupId).ToString())
                    myCmd.Parameters.AddWithValue("@Qty", 1)
                    myCmd.Parameters.AddWithValue("@Location", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@SemiInsideMount", data.semiinsidemount)
                    myCmd.Parameters.AddWithValue("@LouvreSize", data.louvresize)
                    myCmd.Parameters.AddWithValue("@LouvrePosition", data.louvreposition)
                    myCmd.Parameters.AddWithValue("@HingeColour", data.hingecolour)
                    myCmd.Parameters.AddWithValue("@MidrailHeight1", midrailHeight1)
                    myCmd.Parameters.AddWithValue("@MidrailHeight2", midrailHeight2)
                    myCmd.Parameters.AddWithValue("@MidrailCritical", If(data.midrailcritical = "", System.DBNull.Value, data.midrailcritical))
                    myCmd.Parameters.AddWithValue("@Layout", data.layoutcode)
                    myCmd.Parameters.AddWithValue("@LayoutSpecial", data.layoutcodecustom)
                    myCmd.Parameters.AddWithValue("@CustomHeaderLength", headerLength)
                    myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                    myCmd.Parameters.AddWithValue("@FrameLeft", data.frameleft)
                    myCmd.Parameters.AddWithValue("@FrameRight", data.frameright)
                    myCmd.Parameters.AddWithValue("@FrameTop", data.frametop)
                    myCmd.Parameters.AddWithValue("@FrameBottom", data.framebottom)
                    myCmd.Parameters.AddWithValue("@BottomTrackType", data.bottomtracktype)
                    myCmd.Parameters.AddWithValue("@BottomTrackRecess", data.bottomtrackrecess)
                    myCmd.Parameters.AddWithValue("@Buildout", data.buildout)
                    myCmd.Parameters.AddWithValue("@BuildoutPosition", If(data.buildoutposition = "", DBNull.Value, data.buildoutposition))
                    myCmd.Parameters.AddWithValue("@PanelQty", panelQty)
                    myCmd.Parameters.AddWithValue("@TrackQty", trackQty)
                    myCmd.Parameters.AddWithValue("@TrackLength", trackLength)
                    myCmd.Parameters.AddWithValue("@PanelSize", data.samesizepanel)
                    myCmd.Parameters.AddWithValue("@HingeQtyPerPanel", hingeQtyPerPanel)
                    myCmd.Parameters.AddWithValue("@PanelQtyWithHinge", panelQtyWithHinge)
                    myCmd.Parameters.AddWithValue("@LocationTPost1", gap1)
                    myCmd.Parameters.AddWithValue("@LocationTPost2", gap2)
                    myCmd.Parameters.AddWithValue("@LocationTPost3", gap3)
                    myCmd.Parameters.AddWithValue("@LocationTPost4", gap4)
                    myCmd.Parameters.AddWithValue("@LocationTPost5", gap5)
                    myCmd.Parameters.AddWithValue("@HorizontalTPost", data.horizontaltpost)
                    myCmd.Parameters.AddWithValue("@HorizontalTPostHeight", horizontalHeight)
                    myCmd.Parameters.AddWithValue("@JoinedPanels", data.joinedpanels)
                    myCmd.Parameters.AddWithValue("@ReverseHinged", data.reversehinged)
                    myCmd.Parameters.AddWithValue("@PelmetFlat", data.pelmetflat)
                    myCmd.Parameters.AddWithValue("@ExtraFascia", data.extrafascia)
                    myCmd.Parameters.AddWithValue("@HingesLoose", data.hingesloose)
                    myCmd.Parameters.AddWithValue("@TiltrodType", data.tiltrodtype)
                    myCmd.Parameters.AddWithValue("@TiltrodSplit", data.tiltrodsplit)
                    myCmd.Parameters.AddWithValue("@SplitHeight1", splitHeight1)
                    myCmd.Parameters.AddWithValue("@SplitHeight2", splitHeight2)
                    myCmd.Parameters.AddWithValue("@DoorCutOut", data.cutout)
                    myCmd.Parameters.AddWithValue("@SpecialShape", data.specialshape)
                    myCmd.Parameters.AddWithValue("@TemplateProvided", data.templateprovided)
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            publicCfg.ResetPriceDetail(itemId)
            publicCfg.HitungHarga(data.headerid, itemId)
            publicCfg.HitungSurcharge(data.headerid, itemId)

            Return New SuccessResponse With {
                .success = "Data has been updated successfully."
            }
        End If
    End Function




    



End Class
