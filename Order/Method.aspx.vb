Imports System.Data
Imports System.Data.SqlClient
Imports System.Web.Services

Partial Class Order_Method
    Inherits Page

    <WebMethod()>
    Public Shared Function StringData(type As String, dataId As String) As String
        Dim orderCfg As New OrderConfig
        Dim resultName As String = String.Empty
        If type = "DesignName" Then
            resultName = orderCfg.GetItemData("SELECT Name FROM Designs WHERE Id='" + dataId + "'")
        End If
        If type = "BlindName" Then
            resultName = orderCfg.GetItemData("SELECT Name FROM Blinds WHERE Id='" + dataId + "'")
        End If
        If type = "ProductName" Then
            resultName = orderCfg.GetItemData("SELECT Name FROM Products WHERE Id = '" + dataId + "'")
        End If
        If type = "ControlName" Then
            resultName = orderCfg.GetItemData("SELECT ControlType FROM Products WHERE Id = '" + dataId + "'")
        End If
        If type = "FabricName" Then
            resultName = orderCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + dataId + "'")
        End If
        If type = "FabricFactory" Then
            resultName = orderCfg.GetItemData("SELECT Factory FROM Fabrics WHERE Id = '" + dataId + "'")
        End If
        Return resultName
    End Function

    <WebMethod()>
    Public Shared Function ListData(data As JSONList) As List(Of Object)
        Dim orderCfg As New OrderConfig
        Dim result As New List(Of Object)

        Dim type As String = data.type
        Dim customtype As String = data.customtype
        Dim designtype As String = data.designtype
        Dim blindtype As String = data.blindtype
        Dim tubetype As String = data.tubetype
        Dim controltype As String = data.controltype
        Dim fabrictype As String = data.fabrictype
        Dim bottomType As String = data.bottomtype
        Dim production As String = data.production
        Dim fabricchange As String = data.fabricchange

        If type = "BlindType" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT Id, UPPER(Name) AS NameText FROM Blinds WHERE DesignId='" + designtype + "' AND Active=1 ORDER BY Name ASC")
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("NameText").ToString()})
                Next
            End If
        End If

        If type = "BlindTypeShutters" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT Id, UPPER(Name) AS NameText FROM Blinds WHERE DesignId='" + designtype + "' AND Active=1 ORDER BY CASE WHEN Name = 'Panel Only' THEN 1 WHEN Name = 'Hinged' THEN 2 WHEN Name = 'Hinged Bi-fold' THEN 3 WHEN Name = 'Track Bi-fold' THEN 4 WHEN Name = 'Track Sliding' THEN 5 WHEN Name = 'Track Sliding Single Track' THEN 6 WHEN Name = 'Fixed' THEN 7 ELSE 8 END ASC")
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("NameText").ToString()})
                Next
            End If
        End If

        If type = "BlindTypeRoller" Then
            Dim thisQuery As String = "SELECT Id, UPPER(Name) AS NameText FROM Blinds WHERE DesignId='" + designtype + "' AND Active=1 ORDER BY CASE WHEN Name = 'Regular' THEN 1 WHEN Name = 'Double Bracket' THEN 2 WHEN Name = 'Link 2 Blinds Dep' THEN 3 WHEN Name = 'Link 2 Blinds Ind' THEN 4 WHEN Name = 'Link 2 Blinds Head to Tail' THEN 5 WHEN Name = 'Link 3 Blinds Dep' THEN 6 WHEN Name = 'Link 3 Blinds Ind with Dep' THEN 7 WHEN Name = 'Link 3 Blinds Head to Tail with Ind' THEN 8 WHEN Name = 'Link 4 Blinds Ind with Dep' THEN 9 WHEN Name = 'DB Link 2 Blinds Dep' THEN 10 WHEN Name = 'DB Link 2 Blinds Ind' THEN 11 WHEN Name = 'DB Link 3 Blinds Dep' THEN 12 WHEN Name = 'DB Link 3 Blinds Ind with Dep' THEN 13 ELSE 14 END ASC"

            Dim dataSet As DataSet = orderCfg.GetListData(thisQuery)
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("NameText").ToString()})
                Next
            End If
        End If

        If type = "TubeType" Then
            Dim thisQuery As String = "SELECT TubeType, UPPER(TubeType) AS TubeText FROM Products WHERE BlindId = '" + blindtype + "' AND Active = 1 GROUP BY TubeType ORDER BY TubeType ASC"
            If customtype = "Roller" And production = "Orion" Then
                thisQuery = "SELECT TubeType, UPPER(TubeType) AS TubeText FROM Products WHERE BlindId = '" + blindtype + "' AND Active = 1 AND TubeType NOT LIKE '%Cove%' AND TubeType <> 'Como' GROUP BY TubeType ORDER BY TubeType ASC"
            End If

            Dim dataSet As DataSet = orderCfg.GetListData(thisQuery)
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("TubeType").ToString(), .Text = row("TubeText").ToString()})
                Next
            End If
        End If

        If type = "ControlType" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT ControlType, UPPER(ControlType) AS ControlText FROM Products WHERE BlindId='" + blindtype + "' AND TubeType = '" + tubetype + "' AND Active=1 GROUP BY ControlType ORDER BY ControlType ASC")

            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("ControlType").ToString(), .Text = row("ControlText").ToString()})
                Next
            End If
        End If

        If type = "ColourType" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT *, UPPER(ColourType) AS ColourText FROM Products WHERE BlindId='" + blindtype + "' AND TubeType = '" + tubetype + "' AND ControlType='" + controltype + "' AND Active=1 ORDER BY ColourType ASC")

            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("ColourText").ToString()})
                Next
            End If
        End If

        If type = "Products" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT Id, UPPER(Name) AS NameText FROM Products WHERE BlindId = '" + blindtype + "' AND Active = 1")
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("NameText").ToString()})
                Next
            End If
        End If

        If type = "Mounting" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT Name AS NameValue, UPPER(Name) AS NameText FROM Mountings CROSS APPLY STRING_SPLIT(BlindId, ',') AS blindArray WHERE blindArray.VALUE='" + UCase(blindtype).ToString() + "' AND Active=1 ORDER BY Name ASC")
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("NameValue").ToString(), .Text = row("NameText").ToString()})
                Next
            End If
        End If

        If type = "FabricType" Then
            Dim thisQuery As String = "SELECT *, UPPER(Name) AS NameText FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE designArray.VALUE = '" + designtype + "'  AND Active = 1 ORDER BY Name ASC"
            If Not String.IsNullOrEmpty(production) Then
                thisQuery = "SELECT *, UPPER(Name) AS NameText FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE designArray.VALUE = '" + designtype + "' AND Factory = '" + production + "' AND Active = 1 ORDER BY Name ASC"
                If fabricchange = "Yes" Then
                    thisQuery = "SELECT *, UPPER(Name) AS NameText FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE designArray.VALUE = '" + designtype + "' AND Active = 1 ORDER BY Name ASC"
                End If
            End If

            If customtype = "Roller" Then
                thisQuery = "SELECT *, UPPER(Name) AS NameText FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeType, ',') AS tubeArray WHERE designArray.VALUE = '" + designtype + "' AND tubeArray.VALUE = '" + tubetype + "' AND Active = 1 ORDER BY Name ASC"

                If tubetype.Contains("Cove") Then
                    production = "JKT"
                End If

                If Not String.IsNullOrEmpty(production) Then
                    thisQuery = "SELECT *, UPPER(Name) AS NameText FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeType, ',') AS tubeArray WHERE designArray.VALUE = '" + designtype + "' AND tubeArray.VALUE = '" + tubetype + "' AND Factory = '" + production + "' AND Active = 1 ORDER BY Name ASC"
                    If fabricchange = "Yes" Then
                        thisQuery = "SELECT *, UPPER(Name) AS NameText FROM Fabrics CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeType, ',') AS tubeArray WHERE designArray.VALUE = '" + designtype + "' AND tubeArray.VALUE = '" + tubetype + "'AND Active = 1 ORDER BY Name ASC"
                    End If
                End If
            End If

            If customtype = "Vertical" Then
                thisQuery = "SELECT Fabrics.Id, UPPER(Fabrics.Name) AS NameText FROM Fabrics INNER JOIN FabricColours ON Fabrics.Id = FabricColours.FabricId CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS designArray WHERE designArray.VALUE = '" + designtype + "' AND FabricColours.Width = '" + tubetype.Replace("mm", "") + "' AND Fabrics.Active = 1 GROUP BY Fabrics.Id, Fabrics.Name ORDER BY Fabrics.Name ASC"

                If Not String.IsNullOrEmpty(production) Then
                    thisQuery = "SELECT Fabrics.Id, UPPER(Fabrics.Name) AS NameText FROM Fabrics INNER JOIN FabricColours ON Fabrics.Id = FabricColours.FabricId CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS designArray WHERE designArray.VALUE = '" + designtype + "' AND FabricColours.Width = '" + tubetype.Replace("mm", "") + "' AND Fabrics.Factory = '" + production + "' AND Fabrics.Active = 1 GROUP BY Fabrics.Id, Fabrics.Name ORDER BY Fabrics.Name ASC"
                    If fabricchange = "Yes" Then
                        thisQuery = "SELECT Fabrics.Id, UPPER(Fabrics.Name) AS NameText FROM Fabrics INNER JOIN FabricColours ON Fabrics.Id = FabricColours.FabricId CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS designArray WHERE designArray.VALUE = '" + designtype + "' AND FabricColours.Width = '" + tubetype.Replace("mm", "") + "' AND Fabrics.Active = 1 GROUP BY Fabrics.Id, Fabrics.Name ORDER BY Fabrics.Name ASC"
                    End If
                End If
            End If

            Dim dataSet As DataSet = orderCfg.GetListData(thisQuery)
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("NameText").ToString()})
                Next
            End If
        End If

        If type = "FabricColour" Then
            Dim thisQuery As String = "SELECT *, UPPER(Colour) AS ColourText FROM FabricColours WHERE FabricId = '" + fabrictype + "' AND Active = 1 ORDER BY Name ASC"
            If customtype = "Vertical" Then
                thisQuery = "SELECT *, UPPER(Colour) AS ColourText FROM FabricColours WHERE FabricId = '" + fabrictype + "' AND Width = '" + tubetype.Replace("mm", "") + "' AND Active = 1 ORDER BY Name ASC"
            End If

            Dim dataSet As DataSet = orderCfg.GetListData(thisQuery)
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("ColourText").ToString()})
                Next
            End If
        End If

        If type = "BottomType" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT Type AS TypeValue, UPPER(Type) AS TypeText FROM Bottoms CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray WHERE designArray.VALUE='" + designtype + "' AND Active=1 GROUP BY Type ORDER BY Type ASC")
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("TypeValue").ToString(), .Text = row("TypeText").ToString()})
                Next
            End If
        End If

        If type = "BottomColour" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT *, UPPER(Colour) AS ColourText FROM Bottoms CROSS APPLY STRING_SPLIT(designId, ',') AS designArray WHERE Type = '" + bottomType + "' AND designArray.VALUE = '" + designtype + "' AND Active=1 ORDER BY Colour ASC")
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("ColourText").ToString()})
                Next
            End If
        End If

        If type = "BottomTypeRoller" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT Type AS TypeValue, UPPER(Type) AS TypeText FROM Bottoms CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeType, ',') AS tubeArray CROSS APPLY STRING_SPLIT(ControlType, ',') AS controlArray WHERE designArray.VALUE='" + designtype + "' AND tubeArray.VALUE = '" + tubetype + "' AND controlArray.VALUE = '" + controltype + "' AND Active=1 GROUP BY Type ORDER BY Type ASC")

            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("TypeValue").ToString(), .Text = row("TypeText").ToString()})
                Next
            End If
        End If

        If type = "BottomColourRoller" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT *, UPPER(Colour) AS ColourText FROM Bottoms CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeType, ',') AS tubeArray CROSS APPLY STRING_SPLIT(ControlType, ',') AS controlArray WHERE designArray.VALUE='" + designtype + "' AND tubeArray.VALUE = '" + tubetype + "' AND controlArray.VALUE = '" + controltype + "' AND Type = '" + bottomType + "' AND Active=1 ORDER BY Colour ASC")

            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("ColourText").ToString()})
                Next
            End If
        End If

        If type = "ControlColour" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(ControlType, ',') AS controlArray WHERE designArray.VALUE = '" + designtype + "' AND controlArray.VALUE='" + controltype + "' AND Active = 1")

            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("NameText").ToString()})
                Next
            End If
        End If

        If type = "ChainRemote" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Chains CROSS APPLY STRING_SPLIT(DesignId, ',') AS designArray CROSS APPLY STRING_SPLIT(TubeType, ',') AS TubeArray CROSS APPLY string_split(ControlType, ',') AS controlArray WHERE designArray.VALUE = '" + designtype + "' AND controlArray.VALUE='" + controltype + "' AND TubeArray.VALUE = '" + tubetype + "' AND Active=1 ORDER BY Name ASC")

            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("NameText").ToString()})
                Next
            End If
        End If

        If type = "BottomRail" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT *, UPPER(Colour) AS ColourText FROM Bottoms WHERE DesignId='" + designtype + "' AND Active=1 ORDER BY Colour ASC")
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("Id").ToString(), .Text = row("ColourText").ToString()})
                Next
            End If
        End If

        If type = "FabricDiscount" Then
            Dim dataSet As DataSet = orderCfg.GetListData("SELECT Designs.Id AS DesignId, Designs.Name AS DesignName FROM Fabrics CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS split JOIN Designs on Designs.Id = TRIM(split.value) WHERE Fabrics.Id = '" + fabrictype + "'")
            If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
                For Each row As DataRow In dataSet.Tables(0).Rows
                    result.Add(New With {.Value = row("DesignId").ToString(), .Text = row("DesignName").ToString()})
                Next
            End If
        End If
        Return result
    End Function

    <WebMethod>
    Public Shared Function GetDataHeader(headerId As String) As Object
        Dim orderCfg As New OrderConfig
        Dim dataHeader As New Dictionary(Of String, String) From {
            {
                "OrderId",
                orderCfg.GetItemData("SELECT OrderId FROM OrderHeaders WHERE Id = '" + headerId + "'")
            },
            {
                "OrderNumber",
                orderCfg.GetItemData("SELECT OrderNumber FROM OrderHeaders WHERE Id = '" + headerId + "'")
            },
            {
                "OrderName",
                orderCfg.GetItemData("SELECT OrderName FROM OrderHeaders WHERE Id = '" + headerId + "'")
            }
        }
        Return dataHeader
    End Function

    <WebMethod()>
    Public Shared Function EvolveProccess(data As ShutterData) As String
        Dim orderCfg As New OrderConfig

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        If String.IsNullOrEmpty(data.blindtype) Then Return "SHUTTER TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "COLOUR IS REQUIRED !"

        Dim blindName As String = orderCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + data.blindtype + "'")
        Dim roleName As String = orderCfg.GetItemData("SELECT CustomerLoginRoles.Name FROM CustomerLogins INNER JOIN CustomerLoginRoles ON CustomerLogins.RoleId = CustomerLoginRoles.Id WHERE CustomerLogins.Id = '" + data.loginid + "'")

        Dim qty As Integer
        Dim width As Integer
        Dim drop As Integer
        Dim midrailHeight1 As Integer
        Dim midrailHeight2 As Integer
        Dim headerLength As Integer

        Dim horizontalHeight As Integer
        Dim splitHeight1 As Integer
        Dim splitHeight2 As Integer

        Dim gap1 As Integer
        Dim gap2 As Integer
        Dim gap3 As Integer
        Dim gap4 As Integer
        Dim gap5 As Integer
        Dim markup As Integer

        Dim panelQty As Integer = 0
        Dim trackQty As Integer = 0
        Dim trackLength As Integer = 0
        Dim hingeQtyPerPanel As Integer = 0
        Dim panelQtyWithHinge As Integer = 0

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"
        If qty > 5 Then Return "MAXIMUM QTY ORDER IS 5 !"

        If String.IsNullOrEmpty(data.room) Then Return "ROOM / LOCATION IS REQUIRED !"
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"

        If String.IsNullOrEmpty(data.drop) Then Return "HEIGHT IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR HEIGHT ORDER !"

        If String.IsNullOrEmpty(data.louvresize) Then Return "LOUVRE SIZE IS REQUIRED !"

        If blindName = "Track Sliding" Then
            If data.louvreposition = "" Then Return "LOUVRE POSITION IS REQUIRED !"
        End If

        If Not String.IsNullOrEmpty(data.midrailheight1) Then
            If Not Integer.TryParse(data.midrailheight1, midrailHeight1) OrElse midrailHeight1 < 0 Then Return "PLEASE CHECK YOUR MIDRAIL HEIGHT 1 ORDER !"
        End If
        If Not String.IsNullOrEmpty(data.midrailheight2) Then
            If Not Integer.TryParse(data.midrailheight2, midrailHeight2) OrElse midrailHeight2 < 0 Then Return "PLEASE CHECK YOUR MIDRAIL HEIGHT 2 ORDER !"
        End If

        If drop > 1500 AndAlso midrailHeight1 = 0 Then
            Return "MIDRAIL HEIGHT IS REQUIRED. <br /> MAXIMUM ONE SECTION IS 1500MM !"
        End If

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Or blindName = "Track Bi-fold" Then
            If String.IsNullOrEmpty(data.hingecolour) Then Return "HINGE COLOUR IS REQUIRED !"
        End If

        If blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            If data.joinedpanels = "Yes" And String.IsNullOrEmpty(data.hingecolour) Then
                Return "HINGE COLOUR IS REQUIRED !"
            End If
            If Not String.IsNullOrEmpty(data.customheaderlength) Then
                If Not Integer.TryParse(data.customheaderlength, headerLength) OrElse headerLength < 0 Then Return "PLEASE CHECK YOUR CUSTOM HEADER LENGTH !"
                If headerLength > 0 And headerLength > width * 2 Then
                    Return "MINIMUM CUSTOM HEADER LENGTH IS 2x FROM YOUR WIDTH !"
                End If
            End If
        End If

        Dim layoutCode As String = data.layoutcode
        If data.layoutcode = "Other" Then layoutCode = data.layoutcodecustom

        If Not blindName = "Panel Only" Then
            If String.IsNullOrEmpty(data.layoutcode) Then Return "LAYOUT CODE IS REQUIRED !"
            If data.layoutcode = "Other" And String.IsNullOrEmpty(data.layoutcodecustom) Then Return "CUSTOM LAYOUT CODE IS REQUIRED !"

            If blindName = "Hinged" Then
                If layoutCode.Contains("LL") Or layoutCode.Contains("RR") Then
                    Return "YOUR LAYOUT CODE CANNOT BE USED !"
                End If
            End If

            If blindName = "Hinged Bi-fold" Then
                If layoutCode.Contains("LLL") Or layoutCode.Contains("RRR") Then
                    Return "YOUR LAYOUT CODE CANNOT BE USED !"
                End If
            End If

            If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
                If layoutCode.Contains("RL") Then
                    Return "RL LAYOUT CODE CANNOT BE USED. YOU MUST PUT T BETWEEN RL, THAT IS TO BECOME RTL !"
                End If

                Dim checkLayoutD As Boolean = orderCfg.CheckStringLayoutD(layoutCode)
                If checkLayoutD = False Then
                    Return "A DASH (-) IS REQUIRED BEFORE OR AFTER THE LATTER D !"
                End If
            End If

            If blindName = "Track Bi-fold" Then
                Dim stringL As Integer = layoutCode.Split("L").Length - 1
                Dim stringR As Integer = layoutCode.Split("R").Length - 1
                If Not stringL Mod 2 = 0 Then
                    Return "LAYOUT CODE L SHOULD NOT BE ODD !"
                End If
                If Not stringR Mod 2 = 0 Then
                    Return "LAYOUT CODE R SHOULD NOT BE ODD !"
                End If
            End If

            If String.IsNullOrEmpty(data.frametype) Then Return "FRAME TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.frameleft) Then Return "LEFT FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.frameright) Then Return "RIGHT FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.frametop) Then Return "TOP FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.framebottom) Then Return "BOTTOM FRAME IS REQUIRED !"
        End If

        Dim midrailSection As Integer = 0
        If midrailHeight1 > 0 Then midrailSection = 2
        If midrailHeight2 > 0 Then midrailSection = 3

        If midrailSection = 2 Then
            Dim section1 As Decimal = midrailHeight1
            Dim section2 As Decimal = drop - midrailHeight1

            Dim dataGap As Object() = {blindName, section1, data.mounting, data.frametype, data.frametop, data.framebottom, "Bottom"}

            Dim heightDeduct As Decimal = orderCfg.HeightDeductEvolve(dataGap)
            If heightDeduct < 250 Then Return "[MIDRAIL HEIGHT | FIRST SECTION] MINIMUM PANEL HEIGHT IS 250MM !"
            If heightDeduct > 1500 Then Return "[MIDRAIL HEIGHT | FIRST SECTION] MAXIMUM PANEL HEIGHT IS 1500MM !"

            dataGap = {blindName, section2, data.mounting, data.frametype, data.frametop, data.framebottom, "Top"}
            heightDeduct = orderCfg.HeightDeductEvolve(dataGap)

            If heightDeduct < 250 Then Return "[MIDRAIL HEIGHT | SECOND SECTION] MINIMUM PANEL HEIGHT IS 250MM !"
            If heightDeduct > 1500 Then Return "[MIDRAIL HEIGHT | SECOND SECTION] MAXIMUM PANEL HEIGHT IS 1500MM !"
        End If

        If midrailSection = 3 Then
            Dim section1 As Decimal = midrailHeight1
            Dim section2 As Decimal = midrailHeight2 - midrailHeight1
            Dim section3 As Decimal = drop - midrailHeight2

            Dim dataGap As Object() = {blindName, section1, data.mounting, data.frametype, data.frametop, data.framebottom, "Bottom"}

            Dim heightDeduct As Decimal = orderCfg.HeightDeductEvolve(dataGap)
            If heightDeduct < 250 Then Return "[MIDRAIL HEIGHT | FIRST SECTION] MINIMUM PANEL HEIGHT IS 250MM !"
            If heightDeduct > 1500 Then Return "[MIDRAIL HEIGHT | FIRST SECTION] MAXIMUM PANEL HEIGHT IS 1500MM !"

            dataGap = {blindName, section2, data.mounting, data.frametype, data.frametop, data.framebottom, ""}
            heightDeduct = orderCfg.HeightDeductEvolve(dataGap)

            If heightDeduct < 250 Then Return "[MIDRAIL HEIGHT | SECOND SECTION] MINIMUM PANEL HEIGHT IS 250MM !"
            If heightDeduct > 1500 Then Return "[MIDRAIL HEIGHT | SECOND SECTION] MAXIMUM PANEL HEIGHT IS 1500MM !"

            dataGap = {blindName, section3, data.mounting, data.frametype, data.frametop, data.framebottom, "Top"}
            heightDeduct = orderCfg.HeightDeductEvolve(dataGap)

            If heightDeduct < 250 Then Return "[MIDRAIL HEIGHT | SECOND SECTION] MINIMUM PANEL HEIGHT IS 250MM !"
            If heightDeduct > 1500 Then Return "[MIDRAIL HEIGHT | SECOND SECTION] MAXIMUM PANEL HEIGHT IS 1500MM !"
        End If

        If data.frametype = "Insert L 49mm" And (blindName = "Hinged" Or blindName = "Hinged Bi-fold") Then
            If String.IsNullOrEmpty(data.buildout) Then Return "BUILDOUT IS REQUIRED !"
        End If

        If blindName = "Track Bi-fold" Or blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            If data.framebottom = "No" And data.bottomtracktype = "" Then
                Return "BOTTOM TRACK TYPE IS REQUIRED !"
            End If
        End If

        If Not String.IsNullOrEmpty(data.gap1) Then
            If Not Integer.TryParse(data.gap1, gap1) OrElse gap1 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 1 !"
        End If
        If Not String.IsNullOrEmpty(data.gap2) Then
            If Not Integer.TryParse(data.gap2, gap2) OrElse gap2 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 2 !"
        End If
        If Not String.IsNullOrEmpty(data.gap3) Then
            If Not Integer.TryParse(data.gap3, gap3) OrElse gap3 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 3 !"
        End If
        If Not String.IsNullOrEmpty(data.gap4) Then
            If Not Integer.TryParse(data.gap4, gap4) OrElse gap4 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 4 !"
        End If
        If Not String.IsNullOrEmpty(data.gap5) Then
            If Not Integer.TryParse(data.gap5, gap5) OrElse gap5 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 5 !"
        End If

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            If Not String.IsNullOrEmpty(data.horizontaltpostheight) Then
                If Not Integer.TryParse(data.horizontaltpostheight, horizontalHeight) OrElse horizontalHeight < 0 Then Return "PLEASE CHECK YOUR HORIZONTAL T-POST HEIGHT !"
                If horizontalHeight > 0 And String.IsNullOrEmpty(data.horizontaltpost) Then
                    Return "HORIZONTAL T-POST (YES / NO POST) IS REQUIRED !"
                End If
            End If
        End If

        If blindName = "Panel Only" AndAlso String.IsNullOrEmpty(data.panelqty) Then Return "PANEL QTY IS REQUIRED !"

        If String.IsNullOrEmpty(data.tiltrodtype) Then Return "TILTROD TYPE IS REQUIRED !"

        If data.tiltrodsplit = "Other" Then
            If Not String.IsNullOrEmpty(data.splitheight1) Then
                If Not Integer.TryParse(data.splitheight1, splitHeight1) OrElse splitHeight1 <= 0 Then Return "PLEASE CHECK YOUR SPLIT HEIGHT 1 ORDER !"
            End If

            If Not String.IsNullOrEmpty(data.splitheight2) Then
                If Not Integer.TryParse(data.splitheight2, splitHeight2) OrElse splitHeight2 <= 0 Then Return "PLEASE CHECK YOUR SPLIT HEIGHT 1 ORDER !"
            End If
        End If

        Dim datacheckPanelQty As String() = {blindName, data.panelqty, layoutCode, horizontalHeight}
        panelQty = orderCfg.GetPanelQty(datacheckPanelQty)

        'DEDUCTIONS
        If roleName = "Customer" Then
            Dim dataWidthDeductions As Object() = {blindName, "All", width, data.mounting, layoutCode, data.frametype, data.frameleft, data.frameright, panelQty}
            Dim dataHeightDeductions As String() = {blindName, drop, data.mounting, data.frametype, data.frametop, data.framebottom, data.bottomtracktype, data.horizontaltpost}

            Dim widthDeductions As Decimal = orderCfg.WidthDeductEvolve(dataWidthDeductions)
            Dim panelWidth As Decimal = widthDeductions / panelQty

            Dim heightDeduct As Decimal = orderCfg.HeightDeductEvolve(dataHeightDeductions)
            Dim panelHeight As Integer = heightDeduct

            If blindName = "Panel Only" Then
                If width < 250 Then Return "MINIMUM PANEL WIDTH IS 250MM !"
                If width > 914 Then Return "MAXIMUM PANEL WIDTH IS 914mm !"

                If drop < 250 Then Return "MINIMUM PANEL HEIGHT IS 250MM !"
                If drop > 3000 Then Return "MAXIMUM PANEL HEIGHT IS 3000MM !"
            End If

            If blindName = "Hinged" Or blindName = "Fixed" Or blindName = "Track Sliding" Then
                If panelWidth < 250 Then Return "MINIMUM PANEL WIDTH IS 250MM !"
                If panelWidth > 914 Then Return "MAXIMUM PANEL WIDTH IS 914MM !"

                If panelHeight < 250 Then Return "MINIMUM PANEL HEIGHT IS 250MM !"
                If panelHeight > 3000 Then Return "MAXIMUM PANEL HEIGHT IS 3000MM !"
            End If

            If blindName = "Hinged Bi-fold" Then
                If panelWidth < 250 Then Return "MINIMUM PANEL WIDTH IS 250MM !"
                If panelWidth > 610 Then Return "MAXIMUM PANEL WIDTH IS 610MM !"

                If panelHeight < 250 Then Return "MINIMUM PANEL HEIGHT IS 250MM !"
                If panelHeight > 2500 And data.framebottom <> "No" Then Return "MAXIMUM PANEL HEIGHT IS 2500mm !"
                If panelHeight > 3000 Then Return "MAXIMUM PANEL HEIGHT IS 3000MM !"
            End If

            If blindName = "Track Bi-fold" Then
                If panelWidth < 250 Then Return "MINIMUM PANEL WIDTH IS 250MM !"
                If panelWidth > 610 Then Return "MAXIMUM PANEL WIDTH IS 610MM !"

                If panelHeight < 250 Then Return "MINIMUM PANEL HEIGHT IS 250MM !"
                If panelHeight > 3000 Then Return "MAXIMUM PANEL HEIGHT IS 3000MM !"
            End If
        End If

        'GAP POSITION
        If String.IsNullOrEmpty(data.samesizepanel) And roleName = "Customer" Then
            If String.IsNullOrEmpty(data.samesizepanel) And roleName = "Customer" Then
                If blindName = "Hinged" OrElse blindName = "Hinged Bi-fold" Then
                    Dim pemisah As Char() = {"T"c, "C"c, "B"c, "G"c}

                    Dim gaps As Integer() = {gap1, gap2, gap3, gap4, gap5}
                    Dim totalWidth As Integer = width

                    Dim sections As New List(Of String)
                    Dim startIndex As Integer = 0
                    Dim totalPemisah As Integer = 0

                    ' 🔹 Hitung jumlah pemisah & pecah layout jadi section
                    For i As Integer = 1 To layoutCode.Length - 1
                        If pemisah.Contains(layoutCode(i)) Then
                            totalPemisah += 1
                            sections.Add(layoutCode.Substring(startIndex, i - startIndex + 1))
                            startIndex = i
                        End If
                    Next
                    If startIndex < layoutCode.Length Then
                        sections.Add(layoutCode.Substring(startIndex))
                    End If

                    ' 🔹 SKIP semua validasi GAP & panel width jika totalPemisah = 0
                    If totalPemisah > 0 Then
                        ' 🔹 VALIDASI GAP HARUS ADA (TIDAK BOLEH 0)
                        For g As Integer = 0 To totalPemisah - 1
                            If gaps(g) <= 0 Then
                                Return String.Format("GAP {0} IS REQUIRED. PLEASE INPUT A VALID VALUE.", g + 1)
                            End If
                        Next

                        ' 🔹 VALIDASI GAP HARUS BERURUTAN
                        For g As Integer = 0 To totalPemisah - 2
                            If gaps(g) > gaps(g + 1) Then
                                Return String.Format("GAP {0} ({1}) CANNOT BE GREATER THAN GAP {2} ({3}).", g + 1, gaps(g), g + 2, gaps(g + 1))
                            End If
                        Next

                        Dim sumGapUsed As Integer = 0

                        For idx As Integer = 0 To sections.Count - 1
                            Dim section As String = sections(idx)
                            Dim panelCount As Integer = section.Count(Function(ch) "LRFM".Contains(ch))

                            Dim currentGap As Integer
                            If idx = sections.Count - 1 Then
                                ' 🔹 Panel terakhir: sisa width
                                currentGap = totalWidth - sumGapUsed
                            Else
                                If idx = 0 Then
                                    ' 🔹 Section pertama → pakai GAP1 langsung
                                    currentGap = gaps(0)
                                    sumGapUsed += currentGap
                                Else
                                    ' 🔹 Section berikutnya → selisih GAP sekarang - GAP sebelumnya
                                    currentGap = gaps(idx) - gaps(idx - 1)
                                    sumGapUsed += currentGap
                                End If
                            End If

                            Dim dataGap As Object() = {
                                blindName, "Gap", currentGap, data.mounting,
                                section, data.frametype, data.frameleft,
                                data.frameright, panelCount
                            }

                            Dim widthDeduct As Decimal = orderCfg.WidthDeductEvolve(dataGap)

                            ' 🔹 VALIDASI PANEL WIDTH
                            If widthDeduct / panelCount < 200 Then
                                Return String.Format("MINIMUM PANEL WIDTH IS 200MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                            End If
                            If blindName = "Hinged Bi-fold" AndAlso widthDeduct / panelCount > 650 Then
                                Return String.Format("MAXIMUM PANEL WIDTH FOR HINGED BI-FOLD IS 650MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                            End If
                            If widthDeduct / panelCount > 900 Then
                                Return String.Format("MAXIMUM PANEL WIDTH IS 900MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                            End If
                        Next
                    End If
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        Dim productpriceGroupName As String = orderCfg.GetItemData("SELECT Name FROM Designs WHERE Id = '" + data.designid + "'")
        Dim productpriceGroupId As String = orderCfg.GetProductPriceGroupId(data.designid, productpriceGroupName)

        If blindName = "Panel Only" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.hingecolour = ""
            headerLength = 0
            data.layoutcode = "" : data.layoutcodecustom = ""
            data.frametype = ""
            data.frameleft = "" : data.frameright = ""
            data.frametop = "" : data.framebottom = ""
            data.bottomtracktype = ""
            data.buildout = ""
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

            trackQty = 0
            trackLength = 0
            hingeQtyPerPanel = 0
            panelQtyWithHinge = 0
        End If

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            headerLength = 0
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = String.Empty
            End If
            If Not data.frametype = "Insert L 49mm" Then data.buildout = ""

            data.bottomtracktype = ""

            If Not layoutCode.Contains("T") AndAlso Not layoutCode.Contains("B") AndAlso layoutCode.Contains("C") AndAlso layoutCode.Contains("G") Then
                data.samesizepanel = String.Empty
                gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            End If

            If data.samesizepanel = "Yes" Then
                gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            End If

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            If horizontalHeight = 0 Then : data.horizontaltpost = "" : End If
            data.reversehinged = ""
            data.pelmetflat = ""
            data.extrafascia = ""

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

        If blindName = "Track Bi-fold" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.horizontaltpost = ""
            data.horizontaltpostheight = "0"
            data.buildout = ""

            If Not data.mounting = "Inside" Then
                data.semiinsidemount = String.Empty
            End If

            data.customheaderlength = 0
            trackLength = width
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = String.Empty
            End If
            data.buildout = ""
            data.samesizepanel = String.Empty

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If
            If data.framebottom = "Yes" Then
                data.bottomtracktype = ""
            End If

            data.pelmetflat = ""
            data.extrafascia = ""
            data.pelmetflat = "Yes"

            Dim result1 As Integer = 0
            Dim parts As String() = layoutCode.Split("/"c)
            If parts.Length > 0 Then
                result1 = orderCfg.CountMultiLayout(parts(0), New String() {"L", "R", "F"}) - 1
            End If

            Dim result2 As Integer = 0
            If layoutCode.Contains("/") Then
                Dim partss As String() = layoutCode.Split("/"c)
                If partss.Length > 1 Then
                    result2 = orderCfg.CountMultiLayout(partss(1), New String() {"L", "R", "F"}) - 1
                End If
            End If

            panelQtyWithHinge = result1 + result2

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        If blindName = "Track Sliding" Then
            If data.joinedpanels = "" Then
                data.hingecolour = ""
                data.hingesloose = ""
            End If

            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = ""
            End If

            If Not data.mounting = "Inside" Then
                data.semiinsidemount = String.Empty
            End If

            data.buildout = ""
            data.samesizepanel = ""
            data.pelmetflat = "Yes"

            If data.framebottom = "Yes" Then
                data.bottomtracktype = ""
            End If

            data.horizontaltpost = "" : data.horizontaltpostheight = 0
            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight1 = 0
            End If
            data.reversehinged = ""

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

            data.buildout = "" ': data.buildoutposition = ""
            data.samesizepanel = ""

            If data.framebottom = "Yes" Then
                data.bottomtracktype = ""
            End If

            data.horizontaltpostheight = 0 : data.horizontaltpost = ""
            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If
            data.reversehinged = ""
            'data.cutout = ""
            'data.specialshape = ""
            'data.templateprovided = ""
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

        If blindName = "Fixed" Then
            data.louvreposition = ""
            data.joinedpanels = ""
            data.hingecolour = ""

            data.customheaderlength = ""
            If Not data.layoutcode = "Other" Then
                data.layoutcodecustom = ""
            End If
            data.bottomtracktype = ""
            data.buildout = ""
            data.samesizepanel = ""

            data.horizontaltpostheight = 0 : data.horizontaltpost = ""

            If Not data.tiltrodsplit = "Other" Then
                splitHeight1 = 0 : splitHeight2 = 0
            End If

            data.reversehinged = ""
            data.pelmetflat = ""
            data.extrafascia = ""
            data.hingesloose = ""
        End If

        Dim squareMetre As Decimal = Math.Round(width * drop / 1000000, 4)
        Dim linearMetre As Decimal = Math.Round(width / 1000, 4)

        If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, Number, HeaderId, ProductId, ProductPriceGroupId, Qty, Room, Mounting, Width, [Drop], SemiInsideMount, LouvreSize, LouvrePosition, HingeColour, MidrailHeight1, MidrailHeight2, MidrailCritical, Layout, LayoutSpecial, CustomHeaderLength, FrameType, FrameLeft, FrameRight, FrameTop, FrameBottom, BottomTrackType, Buildout, PanelQty, TrackQty, TrackLength, PanelSize, HingeQtyPerPanel, PanelQtyWithHinge, LocationTPost1, LocationTPost2, LocationTPost3, LocationTPost4, LocationTPost5, HorizontalTPost, HorizontalTPostHeight, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, TiltrodType, TiltrodSplit, SplitHeight1, SplitHeight2, SquareMetre, LinearMetre, Notes, Cost, CostOverride, Discount, FinalCost, MarkUp, TotalBlinds, Production, Paid, Active) VALUES (@Id, @Number, @HeaderId, @ProductId, @ProductPriceGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @SemiInsideMount, @LouvreSize, @LouvrePosition, @HingeColour, @MidrailHeight1, @MidrailHeight2, @MidrailCritical, @Layout, @LayoutSpecial, @CustomHeaderLength, @FrameType, @FrameLeft, @FrameRight, @FrameTop, @FrameBottom, @BottomTrackType, @Buildout, @PanelQty, @TrackQty, @TrackLength, @PanelSize, @HingeQtyPerPanel, @PanelQtyWithHinge, @LocationTPost1, @LocationTPost2, @LocationTPost3, @LocationTPost4, @LocationTPost5, @HorizontalTPost, @HorizontalTPostHeight, @JoinedPanels, @ReverseHinged, @PelmetFlat, @ExtraFascia, @HingesLoose, @TiltrodType, @TiltrodSplit, @SplitHeight1, @SplitHeight2, @SquareMetre, @LinearMetre, @Notes, 0, 0, 0, 0, @MarkUp, 0, @Production, 0, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@Number", orderCfg.CreateOrderItemNumber(data.headerid))
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", UCase(data.colourtype).ToString())
                        myCmd.Parameters.AddWithValue("@ProductPriceGroupId", UCase(productpriceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@SemiInsideMount", data.semiinsidemount)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.louvresize)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.louvreposition)
                        myCmd.Parameters.AddWithValue("@HingeColour", data.hingecolour)
                        myCmd.Parameters.AddWithValue("@MidrailHeight1", midrailHeight1)
                        myCmd.Parameters.AddWithValue("@MidrailHeight2", midrailHeight2)
                        myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrailcritical)
                        myCmd.Parameters.AddWithValue("@Layout", data.layoutcode)
                        myCmd.Parameters.AddWithValue("@LayoutSpecial", data.layoutcodecustom)
                        myCmd.Parameters.AddWithValue("@CustomHeaderLength", headerLength)
                        myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                        myCmd.Parameters.AddWithValue("@FrameLeft", data.frameleft)
                        myCmd.Parameters.AddWithValue("@FrameRight", data.frameright)
                        myCmd.Parameters.AddWithValue("@FrameTop", data.frametop)
                        myCmd.Parameters.AddWithValue("@FrameBottom", data.framebottom)
                        myCmd.Parameters.AddWithValue("@BottomTrackType", data.bottomtracktype)
                        myCmd.Parameters.AddWithValue("@Buildout", data.buildout)
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
                        myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                        myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                        myCmd.Parameters.AddWithValue("@Notes", data.notes)
                        myCmd.Parameters.AddWithValue("@MarkUp", markup)
                        myCmd.Parameters.AddWithValue("@Production", "Orion")

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                orderCfg.ResetPriceDetail(data.headerid, itemId)
                Dim cost As Decimal = orderCfg.CountCost(data.headerid, itemId)
                orderCfg.UpdateCost(itemId, cost)
                orderCfg.UpdateCostOverride(itemId, cost)
                orderCfg.UpdateFinalCost(itemId)
                orderCfg.ResetAuthorization(data.headerid, itemId)

                Dim dataLog As Object() = {data.headerid, itemId, data.loginid, "Add Item Order"}
                orderCfg.Log_Orders(dataLog)

                orderCfg.UpdateProductType(data.headerid)
            Next
            Return "Success"
        End If

        If data.itemaction = "EditItem" Or data.itemaction = "ViewItem" Then
            Dim itemId As String = data.itemid

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, ProductPriceGroupId=@ProductPriceGroupId, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, SemiInsideMount=@SemiInsideMount, LouvreSize=@LouvreSize, LouvrePosition=@LouvrePosition, HingeColour=@HingeColour, MidrailHeight1=@MidrailHeight1, MidrailHeight2=@MidrailHeight2, MidrailCritical=@MidrailCritical, Layout=@Layout, LayoutSpecial=@LayoutSpecial, CustomHeaderLength=@CustomHeaderLength, FrameType=@FrameType, FrameLeft=@FrameLeft, FrameRight=@FrameRight, FrameTop=@FrameTop, FrameBottom=@FrameBottom, BottomTrackType=@BottomTrackType, Buildout=@Buildout, PanelQty=@PanelQty, TrackQty=@TrackQty, TrackLength=@TrackLength, PanelSize=@PanelSize, HingeQtyPerPanel=@HingeQtyPerPanel, PanelQtyWithHinge=@PanelQtyWithHinge, LocationTPost1=@LocationTPost1, LocationTPost2=@LocationTPost2, LocationTPost3=@LocationTPost3, LocationTPost4=@LocationTPost4, LocationTPost5=@LocationTPost5, HorizontalTPost=@HorizontalTPost, HorizontalTPostHeight=@HorizontalTPostHeight, JoinedPanels=@JoinedPanels, ReverseHinged=@ReverseHinged, PelmetFlat=@PelmetFlat, ExtraFascia=@ExtraFascia, HingesLoose=@HingesLoose, TiltrodType=@TiltrodType, TiltrodSplit=@TiltrodSplit, SplitHeight1=@SplitHeight1, SplitHeight2=@SplitHeight2, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes=@Notes, Cost=0.00, CostOverride=0.00, FinalCost=0.00, MarkUp=@MarkUp, TotalBlinds=0, Production=@Production, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@ProductId", UCase(data.colourtype).ToString())
                    myCmd.Parameters.AddWithValue("@ProductPriceGroupId", UCase(productpriceGroupId).ToString())
                    myCmd.Parameters.AddWithValue("@Qty", 1)
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@SemiInsideMount", data.semiinsidemount)
                    myCmd.Parameters.AddWithValue("@LouvreSize", data.louvresize)
                    myCmd.Parameters.AddWithValue("@LouvrePosition", data.louvreposition)
                    myCmd.Parameters.AddWithValue("@HingeColour", data.hingecolour)
                    myCmd.Parameters.AddWithValue("@MidrailHeight1", midrailHeight1)
                    myCmd.Parameters.AddWithValue("@MidrailHeight2", midrailHeight2)
                    myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrailcritical)
                    myCmd.Parameters.AddWithValue("@Layout", data.layoutcode)
                    myCmd.Parameters.AddWithValue("@LayoutSpecial", data.layoutcodecustom)
                    myCmd.Parameters.AddWithValue("@CustomHeaderLength", headerLength)
                    myCmd.Parameters.AddWithValue("@FrameType", data.frametype)
                    myCmd.Parameters.AddWithValue("@FrameLeft", data.frameleft)
                    myCmd.Parameters.AddWithValue("@FrameRight", data.frameright)
                    myCmd.Parameters.AddWithValue("@FrameTop", data.frametop)
                    myCmd.Parameters.AddWithValue("@FrameBottom", data.framebottom)
                    myCmd.Parameters.AddWithValue("@BottomTrackType", data.bottomtracktype)
                    myCmd.Parameters.AddWithValue("@Buildout", data.buildout)
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
                    myCmd.Parameters.AddWithValue("@SquareMetre", squareMetre)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)
                    myCmd.Parameters.AddWithValue("@Production", "Orion")

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            orderCfg.ResetPriceDetail(data.headerid, itemId)
            Dim cost As Decimal = orderCfg.CountCost(data.headerid, itemId)
            orderCfg.UpdateCost(itemId, cost)
            orderCfg.UpdateCostOverride(itemId, cost)
            orderCfg.UpdateFinalCost(itemId)
            orderCfg.ResetAuthorization(data.headerid, itemId)

            Dim dataLog As Object() = {data.headerid, itemId, data.loginid, "Edit Item Order"}
            orderCfg.Log_Orders(dataLog)

            orderCfg.UpdateProductType(data.headerid)

            Return "Success"
        End If

        Return "Please contact Customer Service at customerservice@sunlight.com.au"
    End Function

    <WebMethod()>
    Public Shared Function PartsProccess(data As ProcessData) As String
        Dim orderCfg As New OrderConfig

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim qty As Integer
        Dim length As Integer
        Dim markup As Integer

        Dim designName As String = orderCfg.GetItemData("SELECT Name FROM Designs WHERE Id='" + data.designid + "'")

        Dim customerPriceGroup As String = orderCfg.GetItemData("SELECT Customers.Pricing FROM Customers INNER JOIN OrderHeaders ON Customers.Id = OrderHeaders.CustomerId WHERE OrderHeaders.Id='" + data.headerid + "'")


        If String.IsNullOrEmpty(data.blindtype) Then Return "TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "PRODUCT IS REQUIRED !"

        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"

        If String.IsNullOrEmpty(data.category) Then Return "CATEGORY IS REQUIRED !"
        If String.IsNullOrEmpty(data.component) Then Return "COMPONENT IS REQUIRED !"

        '#-------------------------------------------------------------|| Colour Validate ||-------------------------------------------------------------#
        If data.category = "Louvres" OrElse data.category = "Framing | Hinged" OrElse data.category = "Framing | Fixed" OrElse data.category = "Extrusion" Then
            If String.IsNullOrEmpty(data.colour) Then Return "COLOUR IS REQUIRED !"
        End If

        If designName = "Panorama PVC Parts" Then
            If data.category = "Hinges" And (data.component = "76mm Non-Mortise Hinge" Or data.component = "76mm Rabbet Hinge") And String.IsNullOrEmpty(data.colour) Then
                Return "COLOUR IS REQUIRED !"
            End If

            If data.category = "Magnets and Strikers" And (data.component = "Standard Striker Plate" Or data.component = "L Shape Striker Plate") And String.IsNullOrEmpty(data.colour) Then
                Return "COLOUR IS REQUIRED !"
            End If

            If data.category = "Framing | Bi-fold or Sliding" OrElse data.category = "Mist" Then
                If String.IsNullOrEmpty(data.colour) Then Return "COLOUR IS REQUIRED !"
            End If

            If data.category = "Posts" And (data.component = "T-Post" Or data.component = "90° Corner Post" Or data.component = "135° Bay Post") And String.IsNullOrEmpty(data.colour) Then
                Return "COLOUR IS REQUIRED !"
            End If
        End If

        If designName = "Evolve Parts" Then

            If data.category = "Framing | Bi-fold or Sliding" Then
                If Not (data.component ="Fascia H Clip" OrElse data.component = "Fascia Return Connector") Then
                    If String.IsNullOrEmpty(data.colour) Then Return "COLOUR IS REQUIRED !"
                End If
            End If
            If data.category = "Posts" Then
                If String.IsNullOrEmpty(data.colour) Then Return "COLOUR IS REQUIRED !"
            End If

            If data.category = "Hinges" AndAlso (data.component = "60mm Non-Mortise Hinge" OrElse data.component = "60mm Rabbet Hinge") Then
                If String.IsNullOrEmpty(data.colour) Then Return "COLOUR IS REQUIRED !"
            End If

            If data.category = "Track Hardware" AndAlso (data.component = "Top Track" OrElse data.component = "Bottom M Track") Then
                If String.IsNullOrEmpty(data.colour) Then Return "COLOUR IS REQUIRED !"
            End If
        End If
        '#-------------------------------------------------------------|| /Colour Validate ||-------------------------------------------------------------#

        '#-------------------------------------------------------------|| Length Validate ||-------------------------------------------------------------#
        If data.category = "Louvres" Then
            If data.component = "63mm Louvre" OrElse data.component = "89mm Louvre" OrElse data.component = "114mm Louvre" Then
                If String.IsNullOrEmpty(data.length) Then Return "LENGTH IS REQUIRED !"
                If Not Integer.TryParse(data.length, length) OrElse length <= 0 Then Return "PLEASE CHECK YOUR LENGTH ORDER !"
            End If
        End If

        If data.category = "Framing | Hinged" OrElse data.category = "Framing | Fixed" OrElse data.category = "Extrusion" Then
            If String.IsNullOrEmpty(data.length) Then Return "LENGTH IS REQUIRED !"
            If Not Integer.TryParse(data.length, length) OrElse length <= 0 Then Return "PLEASE CHECK YOUR LENGTH ORDER !"
            If length > 3050 Then Return "MAXIMUM LENGTH IS 3050MM !"
        End If

        If designName = "Evolve Parts" Then

            If data.category = "Framing | Bi-fold or Sliding" Then
                If Not (data.component = "Fascia H Clip" OrElse data.component = "Fascia Return Connector" OrElse data.component = "106mm Header Support Bracket" OrElse data.component = "142mm Header Support Bracket") Then
                    If String.IsNullOrEmpty(data.length) Then Return "LENGTH IS REQUIRED !"
                    If Not Integer.TryParse(data.length, length) OrElse length <= 0 Then Return "PLEASE CHECK YOUR LENGTH ORDER !"
                    If length > 3050 Then Return "MAXIMUM LENGTH IS 3050MM !"
                End If
            End If

            If data.category = "Posts" Then
                If data.component = "T Post" OrElse data.component = "90° Corner Post" OrElse data.component = "135° Bay Post"  Then
                    If String.IsNullOrEmpty(data.length) Then Return "LENGTH IS REQUIRED !"
                    If Not Integer.TryParse(data.length, length) OrElse length <= 0 Then Return "PLEASE CHECK YOUR LENGTH ORDER !"
                    If length > 3050 Then Return "MAXIMUM LENGTH IS 3050MM !"
                End If
            End If

        End If

        If designName = "Panorama PVC Parts" Then
            If data.category = "Framing | Bi-fold or Sliding" OrElse data.category = "Posts" Then
                If String.IsNullOrEmpty(data.length) Then Return "LENGTH IS REQUIRED !"
                If Not Integer.TryParse(data.length, length) OrElse length <= 0 Then Return "PLEASE CHECK YOUR LENGTH ORDER !"
                If length > 3050 Then Return "MAXIMUM LENGTH IS 3050MM !"
            End If
        End If


        If data.category = "Track Hardware" And (data.component = "Top Track" Or data.component = "Bottom M Track" Or data.component = "Bottom U Track") Then
            If String.IsNullOrEmpty(data.length) Then Return "LENGTH IS REQUIRED !"
            If Not Integer.TryParse(data.length, length) OrElse length <= 0 Then Return "PLEASE CHECK YOUR LENGTH ORDER !"
        End If
        '#-------------------------------------------------------------|| /Length Validate ||-------------------------------------------------------------#


        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If




       

        Dim productpriceGroupName As String = data.component
        If customerPriceGroup = "Sunlight" Then
            productpriceGroupName = String.Format("{0} | {1}", customerPriceGroup, data.component)
        End If
        Dim productpriceGroupId As String = orderCfg.GetProductPriceGroupId(data.designid, productpriceGroupName)

        '#-------------------------------------------------------------|| Defind Colour & Length Value ||-------------------------------------------------------------#
        If data.category = "Louvres" Then
            If data.component = "Standard Louvre Pin" OrElse data.category = "Louvre Repair Pin" Then
                length = 0
            End If
        End If

        If data.category = "Posts" Then
            If data.component = "Post Mounting Bracket" Then
                data.colour = String.Empty
                length = 0
            End If
        End If

        If data.category = "Hinges" Then
            length = 0
            If data.component = "Hinge Spacer" Then
                data.colour = String.Empty
            End If
        End If

        If data.category = "Magnets and Strikers" Then
            length = 0
            If data.component = "Magnet" Then
                data.colour = String.Empty
            End If
        End If

        If designName = "Panorama PVC Parts" Then
            If data.category = "Track Hardware" Then
                data.colour = String.Empty
                If data.component = "Top Pivot Set" Or data.component = "Bottom Pivot (Incl Bracket)" Or data.component = "Wheel Carrier" Or data.component = "Spring Loaded Guide" Or data.component = "Adjustment Spanner" Or data.component = "Track Stop" Then
                    length = 0
                End If
            End If
        End If

        If designName = "Evolve Parts" Then
            If data.category = "Track Hardware" Then
                If Not (data.component = "Top Track" OrElse data.component = "Bottom M Track") Then
                    data.colour = String.Empty
                    length = 0
                End If
            End If
        End If

        If data.category = "Misc" Then length = 0
        '#-------------------------------------------------------------|| /Defind Colour & Length Value ||-------------------------------------------------------------#

        Dim exactId As String = orderCfg.GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + designName + "'")

        Dim linearMetre As Decimal = Math.Round(length / 1000, 4)

        If data.itemaction = "AddItem" Or data.itemaction = "CopyItem" Then
            Dim itemId As String = orderCfg.CreateOrderItemId()

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, Number, HeaderId, ProductId, ExactId, ProductPriceGroupId, Qty, PartCategory, PartComponent, PartColour, PartLength, LinearMetre, PanelQty, Notes, Cost, CostOverride, Discount, FinalCost, MarkUp, TotalBlinds, Production, Paid, Active) VALUES(@Id, @Number, @HeaderId, @ProductId, @ExactId, @ProductPriceGroupId, @Qty, @PartCategory, @PartComponent, @PartColour, @PartLength, @LinearMetre, @PanelQty, @Notes, 0, 0, 0, 0, @MarkUp, @TotalBlinds, @Production, 0, 1)", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@Number", orderCfg.CreateOrderItemNumber(data.headerid))
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", UCase(data.colourtype).ToString())
                    myCmd.Parameters.AddWithValue("@ExactId", exactId)
                    myCmd.Parameters.AddWithValue("@ProductPriceGroupId", If(String.IsNullOrEmpty(productpriceGroupId), DBNull.Value, UCase(productpriceGroupId).ToString()))
                    myCmd.Parameters.AddWithValue("@Qty", qty)
                    myCmd.Parameters.AddWithValue("@PartCategory", data.category)
                    myCmd.Parameters.AddWithValue("@PartComponent", data.component)
                    myCmd.Parameters.AddWithValue("@PartColour", data.colour)
                    myCmd.Parameters.AddWithValue("@PartLength", length)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@PanelQty", qty)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)
                    myCmd.Parameters.AddWithValue("@TotalBlinds", qty)
                    myCmd.Parameters.AddWithValue("@Production", "Orion")

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderCfg.ResetPriceDetail(data.headerid, itemId)
            Dim cost As Decimal = orderCfg.CountCost(data.headerid, itemId)
            orderCfg.UpdateCost(itemId, cost)
            orderCfg.UpdateCostOverride(itemId, cost)
            orderCfg.UpdateFinalCost(itemId)
            orderCfg.ResetAuthorization(data.headerid, itemId)

            Dim dataLog As Object() = {data.headerid, itemId, data.loginid, "Add Item Order"}
            orderCfg.Log_Orders(dataLog)

            orderCfg.UpdateProductType(data.headerid)

            Return "Success"
        End If

        If data.itemaction = "EditItem" Or data.itemaction = "ViewItem" Then
            Dim itemId As String = data.itemid
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, ExactId=@ExactId, ProductPriceGroupId=@ProductPriceGroupId, Qty=@Qty, PartCategory=@PartCategory, PartComponent=@PartComponent, PartColour=@PartColour, PartLength=@PartLength, LinearMetre=@LinearMetre, PanelQty=@PanelQty, Notes=@Notes, Cost=0.00, CostOverride=0.00, FinalCost=0.00, MarkUp=@MarkUp, Production=@Production, TotalBlinds=@TotalBlinds, Active=1 WHERE Id=@Id", thisConn)
                    myCmd.Parameters.AddWithValue("@Id", data.itemid)
                    myCmd.Parameters.AddWithValue("@ProductId", UCase(data.colourtype).ToString())
                    myCmd.Parameters.AddWithValue("@ExactId", exactId)
                    myCmd.Parameters.AddWithValue("@ProductPriceGroupId", If(String.IsNullOrEmpty(productpriceGroupId), DBNull.Value, UCase(productpriceGroupId).ToString()))
                    myCmd.Parameters.AddWithValue("@Qty", qty)
                    myCmd.Parameters.AddWithValue("@PartCategory", data.category)
                    myCmd.Parameters.AddWithValue("@PartComponent", data.component)
                    myCmd.Parameters.AddWithValue("@PartColour", data.colour)
                    myCmd.Parameters.AddWithValue("@PartLength", length)
                    myCmd.Parameters.AddWithValue("@LinearMetre", linearMetre)
                    myCmd.Parameters.AddWithValue("@PanelQty", qty)
                    myCmd.Parameters.AddWithValue("@Notes", data.notes)
                    myCmd.Parameters.AddWithValue("@MarkUp", markup)
                    myCmd.Parameters.AddWithValue("@TotalBlinds", qty)
                    myCmd.Parameters.AddWithValue("@Production", "Orion")

                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                End Using
            End Using

            orderCfg.ResetPriceDetail(data.headerid, itemId)
            Dim cost As Decimal = orderCfg.CountCost(data.headerid, itemId)
            orderCfg.UpdateCost(itemId, cost)
            orderCfg.UpdateCostOverride(itemId, cost)
            orderCfg.UpdateFinalCost(itemId)
            orderCfg.ResetAuthorization(data.headerid, itemId)

            Dim dataLog As Object() = {data.headerid, itemId, data.loginid, "Edit Item Order"}
            orderCfg.Log_Orders(dataLog)

            orderCfg.UpdateProductType(data.headerid)

            ' Return "itemId :" & itemId
            Return "Success"
        End If

        Return "Please contact Customer Service at customerservice@sunlight.com.au"
    End Function

    <WebMethod()>
    Public Shared Function PanoramaProccess(data As ShutterData) As String
        Dim orderCfg As New OrderConfig

        If String.IsNullOrEmpty(data.blindtype) Then Return "SHUTTER TYPE IS REQUIRED !"
        If String.IsNullOrEmpty(data.colourtype) Then Return "COLOUR IS REQUIRED !"

        Dim blindName As String = orderCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + data.blindtype + "'")

        Dim roleName As String = orderCfg.GetItemData("SELECT CustomerLoginRoles.Name FROM CustomerLogins INNER JOIN CustomerLoginRoles ON CustomerLogins.RoleId = CustomerLoginRoles.Id WHERE CustomerLogins.Id = '" + data.loginid + "'")

        Dim panelQty As Integer = 0
        Dim trackQty As Integer = 0
        Dim trackLength As Integer = 0
        Dim hingeQtyPerPanel As Integer = 0
        Dim panelQtyWithHinge As Integer = 0

        Dim qty As Integer
        If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
        If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY ORDER !"
        If qty > 5 Then Return "MAXIMUM QTY ORDER IS 5 !"

        If String.IsNullOrEmpty(data.room) OrElse data.room.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.room.Contains("&=") OrElse data.room.Contains("&+") Then
            Return "ROOM / LOCATION IS REQUIRED AND MUST NOT CONTAIN: , & ` ' &= &+"
        End If
        If String.IsNullOrEmpty(data.mounting) Then Return "MOUNTING IS REQUIRED !"

        Dim width As Integer
        If String.IsNullOrEmpty(data.width) Then Return "WIDTH IS REQUIRED !"
        If Not Integer.TryParse(data.width, width) OrElse width <= 0 Then Return "PLEASE CHECK YOUR WIDTH ORDER !"

        Dim drop As Integer
        If String.IsNullOrEmpty(data.drop) Then Return "HEIGHT IS REQUIRED !"
        If Not Integer.TryParse(data.drop, drop) OrElse drop <= 0 Then Return "PLEASE CHECK YOUR HEIGHT ORDER !"

        If String.IsNullOrEmpty(data.louvresize) Then Return "LOUVRE SIZE IS REQUIRED !"

        If blindName = "Track Sliding" Then
            If String.IsNullOrEmpty(data.louvreposition) Then Return "SLIDING LOUVRE POSITION IS REQUIRED !"
            If data.louvreposition = "Open" And data.louvresize = "114" Then Return "THE LOUVRE SIZE & LOUVRE POSITION YOU SELECT CANNOT BE PROCESSED !"
        End If

        Dim midrailHeight1 As Integer = 0
        Dim midrailHeight2 As Integer = 0

        If Not String.IsNullOrEmpty(data.midrailheight1) Then
            If Not Integer.TryParse(data.midrailheight1, midrailHeight1) OrElse midrailHeight1 < 0 Then Return "PLEASE CHECK YOUR MIDRAIL HEIGHT 1 ORDER !"
        End If
        If Not String.IsNullOrEmpty(data.midrailheight2) Then
            If Not Integer.TryParse(data.midrailheight2, midrailHeight2) OrElse midrailHeight2 < 0 Then Return "PLEASE CHECK YOUR MIDRAIL HEIGHT 2 ORDER !"
        End If
        If midrailHeight1 >= drop Then
            Return "THE HEIGHT OF MIDRAIL 1 SHOULD NOT BE EQUAL TO OR MORE THAN YOUR ORDER HEIGHT"
        End If
        If midrailHeight2 >= drop Then
            Return "THE HEIGHT OF MIDRAIL 2 SHOULD NOT BE EQUAL TO OR MORE THAN YOUR ORDER HEIGHT"
        End If
        If roleName = "Customer" Then
            'If drop > 1600 Then
            '    If midrailHeight1 = 0 Then
            '        Return "MIDRAIL HEIGHT IS REQUIRED. <br /> MAXIMUM ONE SECTION IS 1500MM !"
            '    End If
            'End If

            'If midrailHeight1 > 0 And midrailHeight2 = 0 Then
            '    If midrailHeight1 > 1600 Then
            '        Return "MAXIMUM MIDRAIL HEIGHT 1 IS 1500MM !"
            '    End If
            '    If drop - midrailHeight1 > 1600 Then
            '        Return "MAXIMUM MIDRAIL HEIGHT FOR OTHER SECTIONS IS 1500mm !"
            '    End If
            'End If
            'If midrailHeight1 > 0 And midrailHeight2 > 0 Then
            '    If midrailHeight1 = midrailHeight2 Then
            '        Return "MIDRAIL HEIGHT IS IN THE SAME POSITION. PLEASE CHANGE MIDRAIL HEIGHT POSITION 2"
            '    End If
            '    If midrailHeight1 > midrailHeight2 Then
            '        Return "THE HEIGHT OF MIDRAIL 1 SHOULD NOT BE GREATER THAN THE HEIGHT OF MIDRAIL 2 !"
            '    End If

            '    If midrailHeight1 > 1500 Then
            '        Return "MAXIMUM ONE SECTION IS 1500MM "
            '    End If
            '    If midrailHeight2 - midrailHeight1 > 1500 Then
            '        Return "MAXIMUM ONE SECTION IS 1500MM !"
            '    End If
            '    If drop - midrailHeight2 > 1500 Then
            '        Return "MAXIMUM ONE SECTION IS 1500MM !"
            '    End If
            'End If
        End If

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Or blindName = "Track Bi-fold" Then
            If String.IsNullOrEmpty(data.hingecolour) Then Return "HINGE COLOUR IS REQUIRED !"
        End If

        Dim headerLength As Integer
        If blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            If data.joinedpanels = "Yes" And String.IsNullOrEmpty(data.hingecolour) Then
                Return "HINGE COLOUR IS REQUIRED !"
            End If
            If Not String.IsNullOrEmpty(data.customheaderlength) Then
                If Not Integer.TryParse(data.customheaderlength, headerLength) OrElse headerLength < 0 Then Return "PLEASE CHECK YOUR CUSTOM HEADER LENGTH !"
                If headerLength > 0 And headerLength > width * 2 Then
                    Return "MINIMUM CUSTOM HEADER LENGTH IS 2x FROM YOUR WIDTH !"
                End If
            End If
        End If

        Dim layoutCode As String = data.layoutcode
        If data.layoutcode = "Other" Then layoutCode = data.layoutcodecustom

        If Not blindName = "Panel Only" Then
            If String.IsNullOrEmpty(data.layoutcode) Then Return "LAYOUT CODE IS REQUIRED !"
            If data.layoutcode = "Other" And String.IsNullOrEmpty(data.layoutcodecustom) Then Return "CUSTOM LAYOUT CODE IS REQUIRED !"

            If blindName = "Hinged" Then
                If layoutCode.Contains("LL") Or layoutCode.Contains("RR") Then
                    Return "YOUR LAYOUT CODE CANNOT BE USED !"
                End If
            End If

            If blindName = "Hinged Bi-fold" Then
                If layoutCode.Contains("LLL") Or layoutCode.Contains("RRR") Then
                    Return "YOUR LAYOUT CODE CANNOT BE USED !"
                End If
            End If

            If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
                If layoutCode.Contains("RL") Then
                    Return "RL LAYOUT CODE CANNOT BE USED. YOU MUST PUT T BETWEEN RL, THAT IS TO BECOME RTL !"
                End If

                Dim checkLayoutD As Boolean = orderCfg.CheckStringLayoutD(layoutCode)
                If checkLayoutD = False Then
                    Return "A DASH (-) IS REQUIRED BEFORE OR AFTER THE LATTER D !"
                End If
            End If

            If blindName = "Track Bi-fold" Then
                Dim stringL As Integer = layoutCode.Split("L").Length - 1
                Dim stringR As Integer = layoutCode.Split("R").Length - 1
                If Not stringL Mod 2 = 0 Then
                    Return "LAYOUT CODE L SHOULD NOT BE ODD !"
                End If
                If Not stringR Mod 2 = 0 Then
                    Return "LAYOUT CODE R SHOULD NOT BE ODD !"
                End If
            End If

            If blindName = "Track Sliding" Then
                If InStr(layoutCode, "M") > 0 And Not data.louvreposition = "Closed" Then
                    Return "LOUVRE POSITION SHOULD BE CLOSED !"
                End If
            End If

            If String.IsNullOrEmpty(data.frametype) Then Return "FRAME TYPE IS REQUIRED !"
            If String.IsNullOrEmpty(data.frameleft) Then Return "LEFT FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.frameright) Then Return "RIGHT FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.frametop) Then Return "TOP FRAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.framebottom) Then Return "BOTTOM FRAME IS REQUIRED !"
        End If

        If blindName = "Track Bi-fold" Or blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
            If String.IsNullOrEmpty(data.bottomtracktype) Then
                Return "BOTTOM TRACK TYPE IS REQUIRED !"
            End If

            If data.bottomtracktype = "M Track" AndAlso data.bottomtrackrecess = "" Then
                Return "BOTTOM TRACK RECESS IS REQUIRED !"
            End If
        End If

        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            If (data.frametype = "Small Bullnose Z Frame" Or data.frametype = "Large Bullnose Z Frame" Or data.frametype = "Colonial Z Frame" Or data.frametype = "Regal Z Frame") And Not String.IsNullOrEmpty(data.buildout) And String.IsNullOrEmpty(data.buildoutposition) Then
                Return "BUILDOUT POSITION IS REQUIRED !"
            End If
        End If

        Dim gap1 As Integer = 0
        Dim gap2 As Integer = 0
        Dim gap3 As Integer = 0
        Dim gap4 As Integer = 0
        Dim gap5 As Integer = 0

        If Not String.IsNullOrEmpty(data.gap1) Then
            If Not Integer.TryParse(data.gap1, gap1) OrElse gap1 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 1 !"
        End If
        If Not String.IsNullOrEmpty(data.gap2) Then
            If Not Integer.TryParse(data.gap2, gap2) OrElse gap2 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 2 !"
        End If
        If Not String.IsNullOrEmpty(data.gap3) Then
            If Not Integer.TryParse(data.gap3, gap3) OrElse gap3 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 3 !"
        End If
        If Not String.IsNullOrEmpty(data.gap4) Then
            If Not Integer.TryParse(data.gap4, gap4) OrElse gap4 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 4 !"
        End If
        If Not String.IsNullOrEmpty(data.gap5) Then
            If Not Integer.TryParse(data.gap5, gap5) OrElse gap5 <= 0 Then Return "PLEASE CHECK YOUR GAP / T-POST 5 !"
        End If

        Dim horizontalHeight As Integer
        If blindName = "Hinged" Or blindName = "Hinged Bi-fold" Then
            If Not String.IsNullOrEmpty(data.horizontaltpostheight) Then
                If Not Integer.TryParse(data.horizontaltpostheight, horizontalHeight) OrElse horizontalHeight < 0 Then Return "PLEASE CHECK YOUR HORIZONTAL T-POST HEIGHT !"
                If horizontalHeight > 0 And String.IsNullOrEmpty(data.horizontaltpost) Then
                    Return "HORIZONTAL T-POST (YES / NO POST) IS REQUIRED !"
                End If
            End If
        End If

        If blindName = "Panel Only" AndAlso String.IsNullOrEmpty(data.panelqty) Then Return "PANEL QTY IS REQUIRED !"

        If String.IsNullOrEmpty(data.tiltrodtype) Then Return "TILTROD TYPE IS REQUIRED !"

        Dim splitHeight1 As Integer = 0
        Dim splitHeight2 As Integer = 0

        If data.tiltrodsplit = "Other" Then
            If Not String.IsNullOrEmpty(data.splitheight1) Then
                If Not Integer.TryParse(data.splitheight1, splitHeight1) OrElse splitHeight1 <= 0 Then Return "PLEASE CHECK YOUR SPLIT HEIGHT 1 ORDER !"
            End If

            If Not String.IsNullOrEmpty(data.splitheight2) Then
                If Not Integer.TryParse(data.splitheight2, splitHeight2) OrElse splitHeight2 <= 0 Then Return "PLEASE CHECK YOUR SPLIT HEIGHT 2 ORDER !"
            End If
        End If

        Dim datacheckPanelQty As String() = {blindName, data.panelqty, layoutCode, horizontalHeight}
        panelQty = orderCfg.GetPanelQty(datacheckPanelQty)

        'DEDUCTIONS
        If roleName = "Customer" And (data.mounting = "Inside" Or data.mounting = "Outside") Then
            Dim dataWidthDeductions As Object() = {blindName, "All", width, data.mounting, layoutCode, data.frametype, data.frameleft, data.frameright, panelQty}
            Dim dataHeightDeductions As String() = {blindName, drop, data.mounting, data.frametype, data.frametop, data.framebottom, data.bottomtracktype, data.horizontaltpost}

            Dim widthDeductions As Decimal = orderCfg.WidthDeductPanorama(dataWidthDeductions)
            Dim panelWidth As Decimal = widthDeductions / panelQty

            Dim heightDeduct As Decimal = orderCfg.HeightDeductPanorama(dataHeightDeductions)
            Dim panelHeight As Integer = heightDeduct

            If blindName = "Panel Only" Then
                If width < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If width > 900 Then Return "MAXIMUM PANEL WIDTH IS 900mm !"

                If drop < 282 And data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If drop < 333 And data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If drop < 384 And data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"
                If drop > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
            End If

            If blindName = "Hinged" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200MM !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900MM !"

                If panelHeight < 282 And data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282MM !"
                If panelHeight < 333 And data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333MM !"
                If panelHeight < 384 And data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384MM !"

                If panelHeight > 1900 And blindName = "Hinged Bi-fold" And (data.framebottom = "No" Or data.framebottom = "Light Block" Or data.framebottom = "L Striker Plate") Then
                    Return "MAXIMUM PANEL HEIGHT IS 1900mm !"
                End If

                If panelHeight > 2500 Then
                    Return "MAXIMUM PANEL HEIGHT IS 2500MM !"
                End If
            End If

            If blindName = "Hinged Bi-fold" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200mm !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900mm !"
                If panelHeight < 282 And data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282mm !"
                If panelHeight < 333 And data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333mm !"
                If panelHeight < 384 And data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384mm !"

                If panelHeight > 1900 And (data.framebottom = "No") Then Return "MAXIMUM PANEL HEIGHT IS 1900mm !"

                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500mm !"
            End If

            If blindName = "Track Bi-fold" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200mm !"
                If panelWidth > 600 Then Return "MAXIMUM PANEL WIDTH IS 600mm !"

                If panelHeight < 282 And data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282mm !"
                If panelHeight < 333 And data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333mm !"
                If panelHeight < 384 And data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384mm !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500mm !"
            End If

            If blindName = "Track Sliding" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200mm !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900mm !"
                If panelHeight < 282 And data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282mm !"
                If panelHeight < 333 And data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333mm !"
                If panelHeight < 384 And data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384mm !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500mm !"
            End If

            If blindName = "Track Sliding Single Track" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200mm !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900mm !"
                If panelHeight < 282 And data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282mm !"
                If panelHeight < 333 And data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333mm !"
                If panelHeight < 384 And data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384mm !"
                If panelHeight > 2500 Then Return "MAXIMUM PANEL HEIGHT IS 2500mm !"
            End If

            If blindName = "Fixed" Then
                If panelWidth < 200 Then Return "MINIMUM PANEL WIDTH IS 200mm !"
                If panelWidth > 900 Then Return "MAXIMUM PANEL WIDTH IS 900mm !"
                If panelHeight < 282 And data.louvresize = "63" Then Return "MINIMUM PANEL HEIGHT IS 282mm !"
                If panelHeight < 333 And data.louvresize = "89" Then Return "MINIMUM PANEL HEIGHT IS 333mm !"
                If panelHeight < 384 And data.louvresize = "114" Then Return "MINIMUM PANEL HEIGHT IS 384mm !"

                If blindName = "Fixed" And data.frametype = "U Channel" And panelHeight > 2527 Then Return "MAXIMUM PANEL HEIGHT IS 2527mm !"
                If blindName = "Fixed" And data.frametype = "19x19 Light Block" And panelHeight > 2506 Then Return "MAXIMUM PANEL HEIGHT IS 2506mm !"
            End If
        End If

        ' GAP POSITION
        If String.IsNullOrEmpty(data.samesizepanel) And roleName = "Customer" Then
            If (blindName = "Hinged" OrElse blindName = "Hinged Bi-fold") AndAlso (data.mounting = "Inside" OrElse data.mounting = "Outside") Then
                Dim pemisah As Char() = {"T"c, "C"c, "B"c, "G"c}

                Dim gaps As Integer() = {gap1, gap2, gap3, gap4, gap5}
                Dim totalWidth As Integer = width

                Dim sections As New List(Of String)
                Dim startIndex As Integer = 0
                Dim totalPemisah As Integer = 0

                For i As Integer = 0 To layoutCode.Length - 1
                    If pemisah.Contains(layoutCode(i)) Then
                        totalPemisah += 1
                        Dim endIndex As Integer = i
                        sections.Add(layoutCode.Substring(startIndex, (endIndex - startIndex) + 1))
                        startIndex = i
                    End If
                Next

                If startIndex < layoutCode.Length Then
                    sections.Add(layoutCode.Substring(startIndex))
                End If

                If totalPemisah > 0 Then
                    For g As Integer = 0 To totalPemisah - 1
                        If gaps(g) <= 0 Then
                            Return String.Format("GAP {0} IS REQUIRED. PLEASE INPUT A VALID VALUE.", g + 1)
                        End If
                    Next

                    For g As Integer = 0 To totalPemisah - 2
                        If gaps(g) > gaps(g + 1) Then
                            Return String.Format("GAP {0} ({1}) CANNOT BE GREATER THAN GAP {2} ({3}).", g + 1, gaps(g), g + 2, gaps(g + 1))
                        End If
                    Next

                    Dim sumGapUsed As Integer = 0

                    For idx As Integer = 0 To sections.Count - 1
                        Dim section As String = sections(idx)

                        Dim panelCount As Integer = section.Count(Function(ch) "LRFM".Contains(ch))

                        Dim currentGap As Integer
                        If idx = sections.Count - 1 Then
                            currentGap = totalWidth - sumGapUsed
                        Else
                            If idx = 0 Then
                                currentGap = gaps(0)
                                sumGapUsed += currentGap
                            Else
                                currentGap = gaps(idx) - gaps(idx - 1)
                                sumGapUsed += currentGap
                            End If
                        End If

                        If currentGap <= 0 Then
                            Return String.Format("GAP 1 IS REQUIRED !", idx + 1)
                            Exit For
                        End If

                        Dim dataGap As Object() = {blindName, "Gap", currentGap, data.mounting, section, data.frametype, data.frameleft, data.frameright, panelCount}

                        Dim widthDeduct As Decimal = orderCfg.WidthDeductPanorama(dataGap)

                        If widthDeduct / panelCount < 200 Then
                            Return String.Format("MINIMUM PANEL WIDTH IS 200MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                        End If
                        If blindName = "Hinged Bi-fold" AndAlso widthDeduct / panelCount > 650 Then
                            Return String.Format("MAXIMUM PANEL WIDTH FOR HINGED BI-FOLD IS 650MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                        End If
                        If widthDeduct / panelCount > 900 Then
                            Return String.Format("MAXIMUM PANEL WIDTH IS 900MM.<br />FINAL PANEL WIDTH IN SECTION {0} IS {1} !", idx + 1, widthDeduct)
                        End If
                    Next
                End If
            End If
        End If

        If Not String.IsNullOrEmpty(data.notes) Then
            If data.notes.IndexOfAny({","c, "&"c, "`"c, "'"c}) >= 0 OrElse data.notes.Contains("&=") OrElse data.notes.Contains("&+") Then
                Return "SPECIAL INFORMATION MUST NOT CONTAIN: , & ` ' &= &+"
            End If
            If data.notes.Trim().Length > 1000 Then Return "MAXIMUM 1000 CHARACTERS !"
        End If

        Dim markup As Integer
        If Not String.IsNullOrEmpty(data.markup) Then
            If Not Integer.TryParse(data.markup, markup) OrElse markup < 0 Then Return "PLEASE CHECK YOUR MARK UP ORDER !"
        End If

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim designName As String = orderCfg.GetItemData("SELECT Name FROM Designs WHERE Id = '" + data.designid + "'")
        Dim micronetId As String = orderCfg.GetItemData("SELECT MicronetId FROM Micronets WHERE Name = '" + designName + "'")
        Dim exactName As String = designName & " - " & blindName
        Dim exactId As String = orderCfg.GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + exactName + "'")

        If blindName = "Panel Only" Then
            data.cutout = ""
            data.specialshape = ""
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
            data.cutout = ""
            data.specialshape = ""
            data.templateprovided = ""

            trackQty = 0
            trackLength = 0
            hingeQtyPerPanel = 0
            panelQtyWithHinge = 0
        End If

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

            If Not layoutCode.Contains("T") AndAlso Not layoutCode.Contains("B") AndAlso layoutCode.Contains("C") AndAlso layoutCode.Contains("G") Then
                data.samesizepanel = String.Empty
                gap1 = 0 : gap2 = 0 : gap3 = 0 : gap4 = 0 : gap5 = 0
            End If

            If data.samesizepanel = "Yes" Then
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

        If blindName = "Track Bi-fold" Then
            data.cutout = ""
            data.specialshape = ""
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
                result1 = orderCfg.CountMultiLayout(parts(0), New String() {"L", "R", "F"}) - 1
            End If

            Dim result2 As Integer = 0
            If layoutCode.Contains("/") Then
                Dim partss As String() = layoutCode.Split("/"c)
                If partss.Length > 1 Then
                    result2 = orderCfg.CountMultiLayout(partss(1), New String() {"L", "R", "F"}) - 1
                End If
            End If

            panelQtyWithHinge = result1 + result2

            hingeQtyPerPanel = 2
            If drop > 800 Then hingeQtyPerPanel = 3
            If drop > 1400 Then hingeQtyPerPanel = 4
            If drop > 2000 Then hingeQtyPerPanel = 5
        End If

        If blindName = "Track Sliding" Then
            data.cutout = ""
            data.specialshape = ""
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

        If blindName = "Fixed" Then
            data.cutout = ""
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

        Dim customerPriceGroup As String = orderCfg.GetItemData("SELECT Customers.Pricing FROM Customers INNER JOIN OrderHeaders ON Customers.Id = OrderHeaders.CustomerId WHERE OrderHeaders.Id='" + data.headerid + "'")

        Dim productpriceGroupName As String = String.Format("Panorama {0}", customerPriceGroup)
        Dim customerId As String = orderCfg.GetItemData("SELECT CustomerId FROM OrderHeaders WHERE Id = '" + data.headerid + "'")
        If customerId = "LS-A329" Then
            productpriceGroupName = "Panorama Standard (SP Special)"
        End If
        If customerPriceGroup = "" Then
            productpriceGroupName = "Panorama Standard"
        End If

        If customerPriceGroup = "" Or customerPriceGroup = "Standard" Or customerPriceGroup = "$164" Or customerPriceGroup = "$168" Or customerPriceGroup = "$185" Or customerPriceGroup = "$190" Or customerPriceGroup = "B" Then
            If data.cutout = "Yes" Then
                productpriceGroupName = String.Format("Panorama {0} - " & "French Door Cut-Out", customerPriceGroup)
                If customerPriceGroup = "" Then
                    productpriceGroupName = "Panorama Standard - French Door Cut-Out"
                End If
            End If
        End If

        If customerPriceGroup = "B" Then
            If blindName = "Hinged Bi-fold" Or blindName = "Track Bi-fold" Then
                productpriceGroupName = "Panorama B - Bi-fold"
            End If
            If blindName = "Track Sliding" Or blindName = "Track Sliding Single Track" Then
                productpriceGroupName = "Panorama B - Sliding"
            End If
        End If

        Dim productpriceGroupId As String = orderCfg.GetProductPriceGroupId(data.designid, productpriceGroupName)

        Dim squareMetre As Decimal = Math.Round(width * drop / 1000000, 4)
        Dim linearMetre As Decimal = Math.Round(width / 1000, 4)

        If data.itemaction = "AddItem" OrElse data.itemaction = "CopyItem" Then
            For i As Integer = 1 To qty
                Dim itemId As String = orderCfg.CreateOrderItemId()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As New SqlCommand("INSERT INTO OrderDetails(Id, Number, HeaderId, ProductId, MicronetId, ExactId, ProductPriceGroupId, Qty, Room, Mounting, Width, [Drop], SemiInsideMount, LouvreSize, LouvrePosition, HingeColour, MidrailHeight1, MidrailHeight2, MidrailCritical, Layout, LayoutSpecial, CustomHeaderLength, FrameType, FrameLeft, FrameRight, FrameTop, FrameBottom, BottomTrackType, BottomTrackRecess, Buildout, BuildoutPosition, PanelQty, TrackQty, TrackLength, PanelSize, HingeQtyPerPanel, PanelQtyWithHinge, LocationTPost1, LocationTPost2, LocationTPost3, LocationTPost4, LocationTPost5, HorizontalTPost, HorizontalTPostHeight, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, TiltrodType, TiltrodSplit, SplitHeight1, SplitHeight2, DoorCutOut, SpecialShape, TemplateProvided, SquareMetre, LinearMetre, Notes, Cost, CostOverride, Discount, FinalCost, MarkUp, TotalBlinds, Production, Paid, Active) VALUES (@Id, @Number, @HeaderId, @ProductId, @MicronetId, @ExactId, @ProductPriceGroupId, @Qty, @Room, @Mounting, @Width, @Drop, @SemiInsideMount, @LouvreSize, @LouvrePosition, @HingeColour, @MidrailHeight1, @MidrailHeight2, @MidrailCritical, @Layout, @LayoutSpecial, @CustomHeaderLength, @FrameType, @FrameLeft, @FrameRight, @FrameTop, @FrameBottom, @BottomTrackType, @BottomTrackRecess, @Buildout, @BuildoutPosition, @PanelQty, @TrackQty, @TrackLength, @PanelSize, @HingeQtyPerPanel, @PanelQtyWithHinge, @LocationTPost1, @LocationTPost2, @LocationTPost3, @LocationTPost4, @LocationTPost5, @HorizontalTPost, @HorizontalTPostHeight, @JoinedPanels, @ReverseHinged, @PelmetFlat, @ExtraFascia, @HingesLoose, @TiltrodType, @TiltrodSplit, @SplitHeight1, @SplitHeight2, @DoorCutOut, @SpecialShape, @TemplateProvided, @SquareMetre, @LinearMetre, @Notes, 0, 0, 0, 0, @MarkUp, 1, @Production, 0, 1)", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", itemId)
                        myCmd.Parameters.AddWithValue("@Number", orderCfg.CreateOrderItemNumber(data.headerid))
                        myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                        myCmd.Parameters.AddWithValue("@ProductId", UCase(data.colourtype).ToString())
                        myCmd.Parameters.AddWithValue("@MicronetId", micronetId)
                        myCmd.Parameters.AddWithValue("@ExactId", exactId)
                        myCmd.Parameters.AddWithValue("@ProductPriceGroupId", UCase(productpriceGroupId).ToString())
                        myCmd.Parameters.AddWithValue("@Qty", 1)
                        myCmd.Parameters.AddWithValue("@Room", data.room)
                        myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                        myCmd.Parameters.AddWithValue("@Width", width)
                        myCmd.Parameters.AddWithValue("@Drop", drop)
                        myCmd.Parameters.AddWithValue("@SemiInsideMount", data.semiinsidemount)
                        myCmd.Parameters.AddWithValue("@LouvreSize", data.louvresize)
                        myCmd.Parameters.AddWithValue("@LouvrePosition", data.louvreposition)
                        myCmd.Parameters.AddWithValue("@HingeColour", data.hingecolour)
                        myCmd.Parameters.AddWithValue("@MidrailHeight1", midrailHeight1)
                        myCmd.Parameters.AddWithValue("@MidrailHeight2", midrailHeight2)
                        myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrailcritical)
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
                        myCmd.Parameters.AddWithValue("@BuildoutPosition", data.buildoutposition)
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
                        myCmd.Parameters.AddWithValue("@Production", "Orion")

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                orderCfg.ResetPriceDetail(data.headerid, itemId)
                Dim cost As Decimal = orderCfg.CountCost(data.headerid, itemId)
                orderCfg.UpdateCost(itemId, cost)
                orderCfg.UpdateCostOverride(itemId, cost)
                orderCfg.UpdateFinalCost(itemId)
                orderCfg.ResetAuthorization(data.headerid, itemId)

                Dim dataLog As Object() = {data.headerid, itemId, data.loginid, "Add Item Order"}
                orderCfg.Log_Orders(dataLog)

                orderCfg.UpdateProductType(data.headerid)
            Next
            Return "Success"
        End If

        If data.itemaction = "EditItem" Or data.itemaction = "ViewItem" Then
            Dim itemId As String = data.itemid

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails Set ProductId=@ProductId, MicronetId=@MicronetId, ExactId=@ExactId, ProductPriceGroupId=@ProductPriceGroupId, Qty=@Qty, Room=@Room, Mounting=@Mounting, Width=@Width, [Drop]=@Drop, SemiInsideMount=@SemiInsideMount, LouvreSize=@LouvreSize, LouvrePosition=@LouvrePosition, HingeColour=@HingeColour, MidrailHeight1=@MidrailHeight1, MidrailHeight2=@MidrailHeight2, MidrailCritical=@MidrailCritical, Layout=@Layout, LayoutSpecial=@LayoutSpecial, CustomHeaderLength=@CustomHeaderLength, FrameType=@FrameType, FrameLeft=@FrameLeft, FrameRight=@FrameRight, FrameTop=@FrameTop, FrameBottom=@FrameBottom, BottomTrackType=@BottomTrackType, BottomTrackRecess=@BottomTrackRecess, Buildout=@Buildout, BuildoutPosition=@BuildoutPosition, PanelQty=@PanelQty, TrackQty=@TrackQty, TrackLength=@TrackLength, PanelSize=@PanelSize, HingeQtyPerPanel=@HingeQtyPerPanel, PanelQtyWithHinge=@PanelQtyWithHinge, LocationTPost1=@LocationTPost1, LocationTPost2=@LocationTPost2, LocationTPost3=@LocationTPost3, LocationTPost4=@LocationTPost4, LocationTPost5=@LocationTPost5, HorizontalTPost=@HorizontalTPost, HorizontalTPostHeight=@HorizontalTPostHeight, JoinedPanels=@JoinedPanels, ReverseHinged=@ReverseHinged, PelmetFlat=@PelmetFlat, ExtraFascia=@ExtraFascia, HingesLoose=@HingesLoose, TiltrodType=@TiltrodType, TiltrodSplit=@TiltrodSplit, SplitHeight1=@SplitHeight1, SplitHeight2=@SplitHeight2, DoorCutOut=@DoorCutOut, SpecialShape=@SpecialShape, TemplateProvided=@TemplateProvided, SquareMetre=@SquareMetre, LinearMetre=@LinearMetre, Notes=@Notes, Cost=0.00, CostOverride=0.00, FinalCost=0.00, MarkUp=@MarkUp, Production=@Production, TotalBlinds=1, Active=1 WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@ProductId", UCase(data.colourtype).ToString())
                    myCmd.Parameters.AddWithValue("@MicronetId", micronetId)
                    myCmd.Parameters.AddWithValue("@ExactId", exactId)
                    myCmd.Parameters.AddWithValue("@ProductPriceGroupId", UCase(productpriceGroupId).ToString())
                    myCmd.Parameters.AddWithValue("@Qty", 1)
                    myCmd.Parameters.AddWithValue("@Room", data.room)
                    myCmd.Parameters.AddWithValue("@Mounting", data.mounting)
                    myCmd.Parameters.AddWithValue("@Width", width)
                    myCmd.Parameters.AddWithValue("@Drop", drop)
                    myCmd.Parameters.AddWithValue("@SemiInsideMount", data.semiinsidemount)
                    myCmd.Parameters.AddWithValue("@LouvreSize", data.louvresize)
                    myCmd.Parameters.AddWithValue("@LouvrePosition", data.louvreposition)
                    myCmd.Parameters.AddWithValue("@HingeColour", data.hingecolour)
                    myCmd.Parameters.AddWithValue("@MidrailHeight1", midrailHeight1)
                    myCmd.Parameters.AddWithValue("@MidrailHeight2", midrailHeight2)
                    myCmd.Parameters.AddWithValue("@MidrailCritical", data.midrailcritical)
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
                    myCmd.Parameters.AddWithValue("@BuildoutPosition", data.buildoutposition)
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
                    myCmd.Parameters.AddWithValue("@Production", "Orion")

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            orderCfg.ResetPriceDetail(data.headerid, itemId)
            Dim cost As Decimal = orderCfg.CountCost(data.headerid, itemId)
            orderCfg.UpdateCost(itemId, cost)
            orderCfg.UpdateCostOverride(itemId, cost)
            orderCfg.UpdateFinalCost(itemId)

            Dim dataLog As Object() = {data.headerid, itemId, data.loginid, "Edit Item Order"}
            orderCfg.Log_Orders(dataLog)

            orderCfg.ResetAuthorization(data.headerid, itemId)

            orderCfg.UpdateProductType(data.headerid)

            Return "Success"
        End If

        Return "Please contact Customer Service at customerservice@sunlight.com.au"
    End Function

    <WebMethod()>
    Public Shared Function AdditionalProccess(data As AdditionalData) As String
        Dim orderCfg As New OrderConfig

        If String.IsNullOrEmpty(data.blindtype) Then Return "CATEGORY IS REQUIRED !"
        If String.IsNullOrEmpty(data.product) Then Return "TYPE IS REQUIRED !"

        Dim blindName As String = orderCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + data.blindtype + "'")
        Dim productName As String = orderCfg.GetItemData("SELECT Name FROM Products WHERE Id = '" + data.product + "'")

        Dim qty As Integer
        If productName = "Parts" Then
            If String.IsNullOrEmpty(data.itemname) Then Return "ITEM NAME IS REQUIRED !"
            If String.IsNullOrEmpty(data.qty) Then Return "QTY IS REQUIRED !"
            If Not Integer.TryParse(data.qty, qty) OrElse qty <= 0 Then Return "PLEASE CHECK YOUR QTY !"
        End If
        If blindName = "Check Measure" And productName = "Template Fee" Then
            If String.IsNullOrEmpty(data.itemnumber) Then Return "NUMBER OF ITEM IS REQUIRED !"
        End If
        If blindName = "Installation" And (productName = "Program Hub" Or productName = "Concrete/Brick" Or productName = "Blind Products") Then
            If String.IsNullOrEmpty(data.itemnumber) Then Return "NUMBER OF ITEM IS REQUIRED !"
        End If
        If blindName = "Takedown" And String.IsNullOrEmpty(data.itemnumber) Then Return "NUMBER OF ITEM IS REQUIRED !"

        Dim addCost As Decimal
        If blindName = "Parts" Then
            If String.IsNullOrEmpty(data.cost) Then Return "UNIT PRICE IS REQUIRED !"
            If Not Decimal.TryParse(data.cost, addCost) OrElse addCost <= 0 Then Return "PLEASE CHECK YOUR UNIT PRICE !"
        End If
        If blindName = "Travel Charge" And productName = "Misc. Travel Charge" Then
            If String.IsNullOrEmpty(data.cost) Then Return "UNIT PRICE IS REQUIRED !"
            If Not Decimal.TryParse(data.cost, addCost) OrElse addCost <= 0 Then Return "PLEASE CHECK YOUR UNIT PRICE !"
        End If

        If blindName = "Freight" And (productName = "Other" Or productName = "TAS - NT - WA" Or productName = "Curtain") Then
            If String.IsNullOrEmpty(data.cost) Then Return "PRICE PER M2 IS REQUIRED !"
            If Not Decimal.TryParse(data.cost, addCost) OrElse addCost <= 0 Then Return "PLEASE CHECK YOUR PRICE PER M2 !"
        End If

        Dim customerPriceGroup As String = orderCfg.GetItemData("SELECT Customers.Pricing FROM Customers INNER JOIN OrderHeaders ON Customers.Id = OrderHeaders.CustomerId WHERE OrderHeaders.Id='" + data.headerid + "'")

        Dim productpriceGroupName As String = productName
        If blindName = "Check Measure" Or blindName = "Installation" Or blindName = "Takedown" Or blindName = "Travel Charge" Then
            productpriceGroupName = blindName & " - " & productName
            If customerPriceGroup = "SISCorporate" Or customerPriceGroup = "SISStandard" Then
                productpriceGroupName = blindName & " - " & productName & " - " & customerPriceGroup.Replace("SIS", "SIS ")
            End If
            If productName = "Misc. Travel Charge" Then
                productpriceGroupName = blindName & " - " & productName
            End If
        End If

        Dim productpriceGroupId As String = orderCfg.GetProductPriceGroupId(data.designid, productpriceGroupName)

        Dim fixQty As Integer = 1
        If blindName = "Parts" Then fixQty = qty

        Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

        Dim micronetId As String = orderCfg.GetItemData("SELECT MicronetId FROM Micronets WHERE Name = '" + blindName + "'")
        If blindName = "Parts" Then micronetId = data.itemcode

        Dim exactId As String = orderCfg.GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + blindName + "'")

        If data.itemaction = "AddItem" Or data.itemaction = "CopyItem" Then
            Dim itemId As String = orderCfg.CreateOrderItemId()

            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderDetails(Id, Number, HeaderId, ProductId, MicronetId, ExactId, ProductPriceGroupId, Qty, AddName, AddNumber, AddPrice, Cost, CostOverride, Discount, FinalCost, MarkUp, TotalBlinds, Paid, Active) VALUES(@Id, @Number, @HeaderId, @ProductId, @MicronetId, @ExactId, @ProductPriceGroupId, @Qty, @AddName, @AddNumber, @AddPrice, 0, 0, 0, 0, 0, 0, 0, 1)")
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@Number", orderCfg.CreateOrderItemNumber(data.headerid))
                    myCmd.Parameters.AddWithValue("@HeaderId", data.headerid)
                    myCmd.Parameters.AddWithValue("@ProductId", UCase(data.product).ToString())
                    myCmd.Parameters.AddWithValue("@MicronetId", micronetId)
                    myCmd.Parameters.AddWithValue("@ExactId", exactId)
                    myCmd.Parameters.AddWithValue("@ProductPriceGroupId", UCase(productpriceGroupId).ToString())
                    myCmd.Parameters.AddWithValue("@Qty", fixQty)
                    myCmd.Parameters.AddWithValue("@AddName", data.itemname)
                    myCmd.Parameters.AddWithValue("@AddNumber", data.itemnumber)
                    myCmd.Parameters.AddWithValue("@AddPrice", addCost)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            orderCfg.ResetPriceDetail(data.headerid, itemId)
            Dim cost As Decimal = orderCfg.CountCost(data.headerid, itemId)
            orderCfg.UpdateCost(itemId, cost)
            orderCfg.UpdateCostOverride(itemId, cost)
            orderCfg.UpdateFinalCost(itemId)
            orderCfg.ResetAuthorization(data.headerid, itemId)

            Dim dataLog As Object() = {data.headerid, itemId, data.loginid, "Add Item Order"}
            orderCfg.Log_Orders(dataLog)

            Return "Success"
        End If

        If data.itemaction = "EditItem" Or data.itemaction = "ViewItem" Then
            Dim itemId As String = data.itemid
            Using thisConn As SqlConnection = New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails SET ProductId=@ProductId, MicronetId=@MicronetId, ExactId=@ExactId, ProductPriceGroupId=@ProductPriceGroupId, Qty=@Qty, AddName=@AddName, AddNumber=@AddNumber, AddPrice=@AddPrice, Cost=0.00, CostOverride=0.00, FinalCost=0.00, Discount=0.00, MarkUp=0, TotalBlinds=0, Active=1 WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@ProductId", UCase(data.product).ToString())
                    myCmd.Parameters.AddWithValue("@MicronetId", micronetId)
                    myCmd.Parameters.AddWithValue("@ExactId", exactId)
                    myCmd.Parameters.AddWithValue("@ProductPriceGroupId", UCase(productpriceGroupId).ToString())
                    myCmd.Parameters.AddWithValue("@Qty", fixQty)
                    myCmd.Parameters.AddWithValue("@AddName", data.itemname)
                    myCmd.Parameters.AddWithValue("@AddNumber", data.itemnumber)
                    myCmd.Parameters.AddWithValue("@AddPrice", addCost)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            orderCfg.ResetPriceDetail(data.headerid, itemId)
            Dim cost As Decimal = orderCfg.CountCost(data.headerid, itemId)
            orderCfg.UpdateCost(itemId, cost)
            orderCfg.UpdateCostOverride(itemId, cost)
            orderCfg.UpdateFinalCost(itemId)
            orderCfg.ResetAuthorization(data.headerid, itemId)

            Dim dataLog As Object() = {data.headerid, itemId, data.loginid, "Edit Item Order"}
            orderCfg.Log_Orders(dataLog)

            Return "Success"
        End If

        Return "Please contact IT Support at reza@bigblinds.co.id"
    End Function

    <WebMethod()>
    Public Shared Function ShutterDetail(itemId As String) As List(Of Dictionary(Of String, Object))
        Dim orderCfg As New OrderConfig
        Dim myData As DataSet = orderCfg.GetListData("SELECT * FROM OrderDetails WHERE Id = '" + itemId + "'")

        Dim resultData As New List(Of Dictionary(Of String, Object))()
        If myData.Tables.Count > 0 Then
            Dim dt As DataTable = myData.Tables(0)
            For Each row As DataRow In dt.Rows

                Dim data As New Dictionary(Of String, Object)()

                Dim productId As String = row("ProductId").ToString()
                Dim blindId As String = orderCfg.GetItemData("SELECT BlindId FROM Products WHERE Id = '" + productId + "'")

                data("BlindType") = blindId
                data("ColourType") = productId
                data("Qty") = row("Qty")
                data("Room") = row("Room")
                data("Mounting") = row("Mounting")
                data("Width") = row("Width")
                data("Drop") = row("Drop")
                data("LouvreSize") = row("LouvreSize")
                data("LouvrePosition") = row("LouvrePosition")
                data("HingeColour") = row("HingeColour")
                data("MidrailHeight1") = row("MidrailHeight1")
                data("MidrailHeight2") = row("MidrailHeight2")
                data("MidrailCritical") = row("MidrailCritical")
                data("LayoutCode") = row("Layout")
                data("LayoutCodeCustom") = row("LayoutSpecial")
                data("CustomHeaderLength") = row("CustomHeaderLength")
                data("FrameType") = row("FrameType")
                data("FrameLeft") = row("FrameLeft")
                data("FrameRight") = row("FrameRight")
                data("FrameTop") = row("FrameTop")
                data("FrameBottom") = row("FrameBottom")
                data("BottomTrackType") = row("BottomTrackType")
                data("BottomTrackRecess") = row("BottomTrackRecess")
                data("Buildout") = row("Buildout")
                data("BuildoutPosition") = row("BuildoutPosition")
                data("PanelQty") = row("PanelQty")
                data("SamePanelSize") = row("PanelSize")
                data("Gap1") = row("LocationTPost1")
                data("Gap2") = row("LocationTPost2")
                data("Gap3") = row("LocationTPost3")
                data("Gap4") = row("LocationTPost4")
                data("Gap5") = row("LocationTPost5")
                data("HorizontalTPost") = row("HorizontalTPost")
                data("HorizontalTPostHeight") = row("HorizontalTPostHeight")
                data("JoinedPanels") = row("JoinedPanels")
                data("ReverseHinged") = row("ReverseHinged")
                data("PelmetFlat") = row("PelmetFlat")
                data("ExtraFascia") = row("ExtraFascia")
                data("HingesLoose") = row("HingesLoose")
                data("SemiInsideMount") = row("SemiInsideMount")
                data("TiltrodType") = row("TiltrodType")
                data("TiltrodSplit") = row("TiltrodSplit")
                data("SplitHeight1") = row("SplitHeight1")
                data("SplitHeight2") = row("SplitHeight2")
                data("DoorCutOut") = row("DoorCutOut")
                data("SpecialShape") = row("SpecialShape")
                data("TemplateProvided") = row("TemplateProvided")
                data("Notes") = row("Notes")
                data("MarkUp") = row("MarkUp")
                resultData.Add(data)
            Next
        End If
        Return resultData
    End Function

    <WebMethod()>
    Public Shared Function Detail(itemId As String) As List(Of Dictionary(Of String, Object))
        If String.IsNullOrEmpty(itemId) Then Return New List(Of Dictionary(Of String, Object))()

        Dim orderCfg As New OrderConfig
        Dim resultData As New List(Of Dictionary(Of String, Object))()

        Dim query As String = "SELECT * FROM OrderDetails WHERE Id = '" & itemId.Replace("'", "''") & "'"
        Dim myData As DataSet = orderCfg.GetListData(query)

        If myData IsNot Nothing AndAlso myData.Tables.Count > 0 AndAlso myData.Tables(0).Rows.Count > 0 Then
            Dim dt As DataTable = myData.Tables(0)

            For Each row As DataRow In dt.Rows
                Dim data As New Dictionary(Of String, Object)()

                Dim productId As String = row("ProductId").ToString()
                Dim designId As String = orderCfg.GetItemData("SELECT DesignId FROM Products WHERE Id = '" + productId + "'")
                Dim blindId As String = orderCfg.GetItemData("SELECT BlindId FROM Products WHERE Id = '" + productId + "'")
                Dim tubeType As String = orderCfg.GetItemData("SELECT TubeType FROM Products WHERE Id = '" + productId + "'")
                Dim controlType As String = orderCfg.GetItemData("SELECT ControlType FROM Products WHERE Id = '" + productId + "'")

                Dim bottomId As String = If(IsDBNull(row("BottomRailId")), "", row("BottomRailId").ToString())
                Dim bottomIdB As String = If(IsDBNull(row("BottomRailIdB")), "", row("BottomRailIdB").ToString())
                Dim bottomIdC As String = If(IsDBNull(row("BottomRailIdC")), "", row("BottomRailIdC").ToString())
                Dim bottomIdD As String = If(IsDBNull(row("BottomRailIdD")), "", row("BottomRailIdD").ToString())
                Dim bottomIdE As String = If(IsDBNull(row("BottomRailIdE")), "", row("BottomRailIdE").ToString())
                Dim bottomIdF As String = If(IsDBNull(row("BottomRailIdF")), "", row("BottomRailIdF").ToString())

                Dim bottomType As String = String.Empty
                Dim bottomTypeB As String = String.Empty
                Dim bottomTypeC As String = String.Empty
                Dim bottomTypeD As String = String.Empty
                Dim bottomTypeE As String = String.Empty
                Dim bottomTypeF As String = String.Empty

                If Not String.IsNullOrEmpty(bottomId) Then
                    bottomType = orderCfg.GetItemData("SELECT Type FROM Bottoms WHERE Id='" + bottomId + "'")
                End If

                If Not String.IsNullOrEmpty(bottomIdB) Then
                    bottomTypeB = orderCfg.GetItemData("SELECT Type FROM Bottoms WHERE Id='" + bottomIdB + "'")
                End If

                If Not String.IsNullOrEmpty(bottomIdC) Then
                    bottomTypeC = orderCfg.GetItemData("SELECT Type FROM Bottoms WHERE Id='" + bottomIdC + "'")
                End If
                If Not String.IsNullOrEmpty(bottomIdD) Then
                    bottomTypeD = orderCfg.GetItemData("SELECT Type FROM Bottoms WHERE Id='" + bottomIdD + "'")
                End If
                If Not String.IsNullOrEmpty(bottomIdE) Then
                    bottomTypeE = orderCfg.GetItemData("SELECT Type FROM Bottoms WHERE Id='" + bottomIdE + "'")
                End If
                If Not String.IsNullOrEmpty(bottomIdF) Then
                    bottomTypeF = orderCfg.GetItemData("SELECT Type FROM Bottoms WHERE Id='" + bottomIdF + "'")
                End If

                Dim designName As String = orderCfg.GetItemData("SELECT Name FROM Designs WHERE Id='" + designId + "'")

                data("BlindType") = blindId
                data("TubeType") = tubeType
                data("ControlType") = controlType
                data("ColourType") = productId
                data("Qty") = row("Qty")
                data("Room") = row("Room")
                data("Mounting") = row("Mounting")
                data("Width") = row("Width")
                data("WidthB") = row("WidthB")
                data("WidthC") = row("WidthC")
                data("WidthD") = row("WidthD")
                data("WidthE") = row("WidthE")
                data("WidthF") = row("WidthF")
                data("Drop") = row("Drop")
                data("DropB") = row("DropB")
                data("DropC") = row("DropC")
                data("DropD") = row("DropD")
                data("DropE") = row("DropE")
                data("DropF") = row("DropF")

                data("ControlPosition") = row("ControlPosition")
                data("ControlPositionB") = row("ControlPositionB")
                data("ControlPositionC") = row("ControlPositionC")
                data("ControlPositionD") = row("ControlPositionD")
                data("ControlPositionE") = row("ControlPositionE")
                data("ControlPositionF") = row("ControlPositionF")

                data("Roll") = row("Roll")
                data("RollB") = row("RollB")
                data("RollC") = row("RollC")
                data("RollD") = row("RollD")
                data("RollE") = row("RollE")
                data("RollF") = row("RollF")

                data("TilterPosition") = row("TilterPosition")
                data("ControlLength") = row("ControlLength")
                data("WandLength") = row("WandLength")

                data("Supply") = row("Supply")
                data("TrackType") = row("TrackType")
                data("TrackLength") = row("TrackLength")
                data("LayoutCode") = row("Layout")
                data("LayoutCodeCustom") = row("LayoutSpecial")
                data("BattenFront") = row("Batten")
                data("BattenRear") = row("BattenB")
                data("PelmetFor") = row("PelmetFor")
                data("ReturnSize") = row("ValanceReturnLength")
                data("BracketSize") = row("BracketSize")
                data("Drawing") = row("StackOption")
                data("Lining") = row("Lining")
                data("Hem") = row("Supply")

                data("Notes") = row("Notes")
                data("MarkUp") = row("MarkUp")

                data("RemoteType") = row("ChainId")
                data("ChainColour") = row("ChainId")
                data("ChainColourB") = row("ChainIdB")
                data("ChainColourC") = row("ChainIdC")
                data("ChainColourD") = row("ChainIdD")
                data("ChainColourE") = row("ChainIdE")
                data("ChainColourF") = row("ChainIdF")

                data("FabricType") = row("FabricId")
                data("FabricTypeB") = row("FabricIdB")
                data("FabricTypeC") = row("FabricIdC")
                data("FabricTypeD") = row("FabricIdD")
                data("FabricTypeE") = row("FabricIdE")
                data("FabricTypeF") = row("FabricIdF")

                data("FabricColour") = row("FabricColourId")
                data("FabricColourB") = row("FabricColourIdB")
                data("FabricColourC") = row("FabricColourIdC")
                data("FabricColourD") = row("FabricColourIdD")
                data("FabricColourE") = row("FabricColourIdE")
                data("FabricColourF") = row("FabricColourIdF")

                data("BottomType") = bottomType
                data("BottomTypeB") = bottomTypeB
                data("BottomTypeC") = bottomTypeC
                data("BottomTypeD") = bottomTypeD
                data("BottomTypeE") = bottomTypeE
                data("BottomTypeF") = bottomTypeF

                data("BottomColour") = row("BottomRailId")
                data("BottomColourB") = row("BottomRailIdB")
                data("BottomColourC") = row("BottomRailIdC")
                data("BottomColourD") = row("BottomRailIdD")
                data("BottomColourE") = row("BottomRailIdE")
                data("BottomColourF") = row("BottomRailIdF")

                data("WandExtendable") = row("WandExtendable")
                data("WandExtendableB") = row("WandExtendableB")
                data("WandExtendableC") = row("WandExtendableC")
                data("WandExtendableD") = row("WandExtendableD")
                data("WandExtendableE") = row("WandExtendableE")
                data("WandExtendableF") = row("WandExtendableF")

                data("Adjusting") = row("Adjusting")
                data("RemoteChannel") = row("RemoteChannel")
                data("RemoteCharger") = row("BatteryCharger")
                data("ExtensionCable") = row("ExtensionCable")
                data("BracketSize") = row("BracketSize")

                data("SpringAssist") = row("SpringAssist")

                If designName = "Roller Blind" Then
                    data("ChainLength") = row("ControlLength")
                    data("ChainLengthB") = row("ControlLengthB")
                    data("ChainLengthC") = row("ControlLengthC")
                    data("ChainLengthD") = row("ControlLengthD")
                    data("ChainLengthE") = row("ControlLengthE")
                    data("ChainLengthF") = row("ControlLengthF")
                End If

                data("TrackColour") = row("TrackColour")
                data("WandPosition") = row("ControlPosition")
                data("WandColour") = row("WandColour")
                data("WandSize") = row("WandLength")
                data("Extension") = row("BracketExtension")

                data("BottomRail") = row("BottomRailId")
                data("SideBySide") = row("SideBySide")

                data("Category") = row("PartCategory")
                data("Component") = row("PartComponent")
                data("Colour") = row("PartColour")
                data("Length") = row("PartLength")

                data("FabricInsert") = row("FabricInsert")
                data("StackPosition") = row("StackOption")
                data("BottomWeight") = row("BottomWeight")
                data("BottomWeightColour") = row("BottomWeightColour")
                data("ExtensionBracket") = row("BracketExtension")
                data("Sloping") = row("Sloping")
                data("BladeQty") = row("QtyBlade")
                data("ControlColour") = row("ControlColour")

                data("Product") = productId
                data("AddNumber") = row("AddNumber")
                data("Cost") = row("AddPrice")
                data("AddName") = row("AddName")
                data("MicronetId") = row("MicronetId")

                If designName = "Roman Blind" Then
                    Dim chainLength As String = String.Empty
                    Dim cordLength As String = String.Empty
                    If controlType = "Chain" Then
                        chainLength = row("ControlLength")
                    End If
                    If controlType = "Reg Cord Lock" Or controlType = "Regular Cord Lock" Then
                        cordLength = row("ControlLength")
                    End If
                    data("ChainLength") = chainLength
                    data("CordLength") = cordLength
                    data("ControlColour") = row("ChainId")
                End If

                resultData.Add(data)
            Next
        End If

        Return resultData
    End Function
End Class

Public Class JSONList
    Public Property type As String
    Public Property customtype As String
    Public Property designtype As String
    Public Property blindtype As String
    Public Property tubetype As String
    Public Property controltype As String
    Public Property fabrictype As String
    Public Property bottomtype As String
    Public Property production As String
    Public Property fabricchange As String
End Class

Public Class ShutterData
    Public Property headerid As String
    Public Property itemaction As String
    Public Property itemid As String
    Public Property designid As String
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
    Public Property notes As String
    Public Property markup As String
    Public Property loginid As String
End Class

Public Class AdditionalData
    Public Property headerid As String
    Public Property itemaction As String
    Public Property itemid As String
    Public Property designid As String
    Public Property blindtype As String
    Public Property product As String
    Public Property itemname As String
    Public Property itemcode As String
    Public Property itemnumber As String
    Public Property qty As String
    Public Property cost As String
    Public Property loginid As String
End Class

Public Class ProcessData
    Public Property headerid As String
    Public Property itemaction As String
    Public Property colourtype As String
    Public Property qty As String
    Public Property itemid As String
    Public Property designid As String
    Public Property blindtype As String
    Public Property product As String
    Public Property category As String
    Public Property length As String
    Public Property component As String
    Public Property loginid As String
    Public Property colour As String
    Public Property notes As String
    Public Property markup As String
End Class
