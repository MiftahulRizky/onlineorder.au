Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports System.Net
Imports System.Xml
Imports WinSCP

Public Class ExactConfig
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Function GetListData(thisString As String) As DataSet
        Dim thisCmd As New SqlCommand(thisString)
        Using thisConn As New SqlConnection(myConn)
            Using thisAdapter As New SqlDataAdapter()
                thisCmd.Connection = thisConn
                thisAdapter.SelectCommand = thisCmd
                Using thisDataSet As New DataSet()
                    thisAdapter.Fill(thisDataSet)
                    Return thisDataSet
                End Using
            End Using
        End Using
    End Function

    Public Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
        Try
            Using thisConn As New SqlConnection(myConn)
                thisConn.Open()
                Using myCmd As New SqlCommand(thisString, thisConn)
                    Using rdResult = myCmd.ExecuteReader
                        While rdResult.Read
                            result = rdResult.Item(0).ToString()
                        End While
                    End Using
                End Using
                thisConn.Close()
            End Using
        Catch ex As Exception
            result = String.Empty
        End Try
        Return result
    End Function

    Public Sub Connect(data As String)
        Dim host As String = "server.sydneyblinds.com.au"
        Dim port As Integer = 30150
        Dim username As String = "exact03-user"
        Dim password As String = "Prussia$rwanda$grapple6#"

        Dim sessionOptions As New SessionOptions With {
                .Protocol = Protocol.Ftp,
                .HostName = host,
                .UserName = username,
                .Password = password,
                .FtpSecure = FtpSecure.Explicit,
                .PortNumber = port,
                .GiveUpSecurityAndAcceptAnyTlsHostCertificate = True
            }

        Using session As New Session()
            session.Open(sessionOptions)
            If session.Opened Then
                Dim localPath As String = Path.GetTempFileName()
                Using webClient As New WebClient()
                    Try
                        webClient.DownloadFile(data, localPath)
                    Catch ex As Exception
                    End Try
                End Using

                Dim remoteFileName As String = Path.GetFileName(New Uri(data).LocalPath)
                Dim remotePath As String = "./" & remoteFileName

                Dim transferOptions As New TransferOptions With {
                        .TransferMode = TransferMode.Binary
                    }

                Dim transferResult As TransferOperationResult = session.PutFiles(localPath, remotePath, False, transferOptions)

                Try
                    If File.Exists(localPath) Then File.Delete(localPath)
                Catch ex As Exception
                End Try
            End If
        End Using
    End Sub

    Public Sub CreateXML(Id As String, fileName As String, folderPath As String)
        Dim sb As New StringBuilder()
        sb.AppendLine("<?xml version=""1.0""?>")

        Dim settings As New XmlWriterSettings()
        settings.Indent = True
        settings.OmitXmlDeclaration = True
        settings.Encoding = New UTF8Encoding(False)

        Dim headerData As DataSet = GetListData("SELECT * FROM OrderHeaders_Shutters WHERE Id = '" + Id + "'")

        Dim orderId As String = headerData.Tables(0).Rows(0).Item("OrderId").ToString()
        Dim orderNumber As String = headerData.Tables(0).Rows(0).Item("OrderNumber").ToString()
        Dim orderName As String = headerData.Tables(0).Rows(0).Item("OrderName").ToString()
        Dim customerId As String = headerData.Tables(0).Rows(0).Item("CustomerId").ToString()
        Dim jobDate As String = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("JobDate")).ToString("yyyy-MM-dd")
        Dim shipmentId As String = headerData.Tables(0).Rows(0).Item("ShipmentId").ToString()

        Dim customerAccount As String = GetItemData("SELECT Account FROM Customers WHERE Id = '" + customerId + "'")

        Dim exactCustomer As String = String.Empty
        If customerAccount = "Master" Or customerAccount = "Regular" Then
            exactCustomer = GetItemData("SELECT ExactId FROM Customers WHERE Id='" + customerId + "'")
        End If
        If customerAccount = "Sub" Then
            Dim masterId As String = GetItemData("SELECT MasterId FROM Customers WHERE Id='" + customerId + "'")
            exactCustomer = GetItemData("SELECT ExactId FROM Customers WHERE Id='" + masterId + "'")
        End If

        Dim etaCustomer As String = String.Empty
        If Not String.IsNullOrEmpty(shipmentId) Then
            Dim shipmentData As DataSet = GetListData("SELECT * FROM OrderShipments WHERE Id='" + shipmentId + "'")
            etaCustomer = Convert.ToDateTime(shipmentData.Tables(0).Rows(0).Item("ETACustomer")).ToString("yyyy-MM-dd")
        End If

        Using stringWriter As New StringWriter(sb)
            Using writer As XmlWriter = XmlWriter.Create(stringWriter, settings)
                writer.WriteStartDocument()
                writer.WriteStartElement("eExact")
                writer.WriteAttributeString("xmlns", "xsi", Nothing, "http://www.w3.org/2001/XMLSchema-instance")
                writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "eExact-Schema.xsd")

                writer.WriteStartElement("Orders")
                writer.WriteStartElement("Order")
                writer.WriteAttributeString("type", "V")

                writer.WriteElementString("YourRef", Id)
                writer.WriteElementString("Description", orderId & " - " & orderNumber & " - " & orderName)

                writer.WriteStartElement("Resource")
                writer.WriteAttributeString("number", "99")
                writer.WriteString("")
                writer.WriteEndElement()

                writer.WriteStartElement("OrderedBy")
                writer.WriteStartElement("Debtor")
                writer.WriteAttributeString("code", exactCustomer)
                writer.WriteString("")
                writer.WriteEndElement()
                writer.WriteElementString("Date", jobDate)
                writer.WriteEndElement()

                Dim detailData As DataSet = GetListData("SELECT * FROM OrderDetails_Shutters WHERE HeaderId = '" & Id & "' AND Active = 1 ORDER BY Id ASC")

                For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
                    Dim itemId As String = detailData.Tables(0).Rows(i).Item("Id").ToString()
                    Dim productId As String = detailData.Tables(0).Rows(i).Item("ProductId").ToString()
                    Dim finalCost As Decimal = detailData.Tables(0).Rows(i)("FinalCost")
                    Dim finalCostString As String = finalCost.ToString(CultureInfo.InvariantCulture)

                    Dim exactProduct As String = detailData.Tables(0).Rows(i).Item("ExactId").ToString()
                    If String.IsNullOrEmpty(exactProduct) Then
                        Dim designId As String = GetItemData("SELECT DesignId FROM Products WHERE Id='" + productId + "'")
                        Dim blindId As String = GetItemData("SELECT BlindId FROM Products WHERE Id='" + productId + "'")
                        Dim designName As String = GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
                        Dim blindName As String = GetItemData("SELECT Name FROM Blinds WHERE Id = '" + blindId + "'")
                        If designName = "Panorama PVC Shutters" Then
                            Dim exactName As String = designName & " - " & blindName
                            exactProduct = GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + exactName + "'")
                        End If
                        If designName = "Panorama PVC Parts" Then
                            Dim exactName As String = designName
                            exactProduct = GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + exactName + "'")
                        End If
                        If designName = "Additional" Then
                            Dim exactName As String = blindName
                            exactProduct = GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + exactName + "'")
                        End If
                    End If

                    Dim productName As String = GetItemData("SELECT Name FROM Products WHERE Id = '" & UCase(productId) & "'")
                    Dim width As String = detailData.Tables(0).Rows(i).Item("Width").ToString()
                    Dim drop As String = detailData.Tables(0).Rows(i).Item("Drop").ToString()
                    Dim itemDescription As String = productName & " " & width & "x" & drop

                    writer.WriteStartElement("OrderLine")

                    writer.WriteStartElement("Item")
                    writer.WriteAttributeString("code", exactProduct)
                    writer.WriteString("")
                    writer.WriteEndElement()

                    writer.WriteElementString("Quantity", "1")

                    writer.WriteStartElement("Price")
                    writer.WriteAttributeString("type", "S")

                    writer.WriteStartElement("Currency")
                    writer.WriteAttributeString("code", "AUD")
                    writer.WriteString("")
                    writer.WriteEndElement()

                    writer.WriteElementString("Value", finalCostString)
                    writer.WriteEndElement() ' Price

                    writer.WriteStartElement("Delivery")
                    writer.WriteElementString("Date", etaCustomer)
                    writer.WriteEndElement()

                    writer.WriteElementString("Text", itemDescription)
                    writer.WriteEndElement() ' OrderLine
                Next

                writer.WriteEndElement() ' Order
                writer.WriteEndElement() ' Orders
                writer.WriteEndElement() ' eExact
                writer.WriteEndDocument()
            End Using
        End Using

        Dim filePath As String = Path.Combine(folderPath, fileName)
        Dim xmlFinal As String = sb.ToString().Replace(" />", "/>")
        File.WriteAllText(filePath, xmlFinal, New UTF8Encoding(False))
    End Sub

    Public Sub CreateXMLB(Id As String, fileName As String, folderPath As String)
        Dim sb As New StringBuilder()
        sb.AppendLine("<?xml version=""1.0""?>")

        Dim settings As New XmlWriterSettings()
        settings.Indent = True
        settings.OmitXmlDeclaration = True
        settings.Encoding = New UTF8Encoding(False)

        Dim headerData As DataSet = GetListData("SELECT * FROM view_order_headers WHERE Id = '" + Id + "' AND OrderType = 'Blinds'")
        If headerData.Tables(0).Rows.Count < 0 Then Exit Sub

        For h As Integer = 0 To headerData.Tables(0).Rows.Count - 1
            Dim orderId As String = headerData.Tables(0).Rows(h).Item("OrderId").ToString()
            Dim orderNumber As String = headerData.Tables(0).Rows(h).Item("OrderNumber").ToString()
            Dim orderName As String = headerData.Tables(0).Rows(h).Item("OrderName").ToString()
            Dim customerId As String = headerData.Tables(0).Rows(h).Item("CustomerId").ToString()
            Dim jobDate As String = Convert.ToDateTime(headerData.Tables(0).Rows(h).Item("JobDate")).ToString("yyyy-MM-dd")
            Dim shipmentId As String = headerData.Tables(0).Rows(h).Item("ShipmentId").ToString()

            Dim customerAccount As String = GetItemData("SELECT Account FROM Customers WHERE Id = '" + customerId + "'")

            Dim exactCustomer As String = String.Empty
            If customerAccount = "Master" Or customerAccount = "Regular" Or customerAccount = "REGULAR" Then
                exactCustomer = GetItemData("SELECT ExactId FROM Customers WHERE Id='" + customerId + "'")
            End If

            If customerAccount = "Sub" Then
                Dim masterId As String = GetItemData("SELECT MasterId FROM Customers WHERE Id='" + customerId + "'")
                exactCustomer = GetItemData("SELECT ExactId FROM Customers WHERE Id='" + masterId + "'")
            End If

            Dim etaCustomer As String = String.Empty
            If Not String.IsNullOrEmpty(shipmentId) Then
                Dim shipmentData As DataSet = GetListData("SELECT * FROM OrderShipments WHERE Id='" + shipmentId + "'")
                etaCustomer = Convert.ToDateTime(shipmentData.Tables(0).Rows(0).Item("ETACustomer")).ToString("yyyy-MM-dd")
            End If

            Using stringWriter As New StringWriter(sb)
                Using writer As XmlWriter = XmlWriter.Create(stringWriter, settings)
                    writer.WriteStartDocument()
                    writer.WriteStartElement("eExact")
                    writer.WriteAttributeString("xmlns", "xsi", Nothing, "http://www.w3.org/2001/XMLSchema-instance")
                    writer.WriteAttributeString("xsi", "noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance", "eExact-Schema.xsd")

                    writer.WriteStartElement("Orders")
                    writer.WriteStartElement("Order")
                    writer.WriteAttributeString("type", "V")

                    writer.WriteElementString("YourRef", Id)
                    writer.WriteElementString("Description", orderId & " - " & orderNumber & " - " & orderName)

                    writer.WriteStartElement("Resource")
                    writer.WriteAttributeString("number", "99")
                    writer.WriteString("")
                    writer.WriteEndElement()

                    writer.WriteStartElement("OrderedBy")
                    writer.WriteStartElement("Debtor")
                    writer.WriteAttributeString("code", exactCustomer)
                    writer.WriteString("")
                    writer.WriteEndElement()
                    writer.WriteElementString("Date", jobDate)
                    writer.WriteEndElement()

                    Dim detailData As DataSet = GetListData("SELECT * FROM OrderDetails WHERE HeaderId = '" & Id & "' AND Active = 1 ORDER BY Id ASC")

                    For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
                        Dim itemId As String = detailData.Tables(0).Rows(i).Item("Id").ToString()
                        Dim kitId As String = detailData.Tables(0).Rows(i).Item("KitId").ToString()
                        Dim Qty As String = detailData.Tables(0).Rows(i).Item("Qty").ToString()
                        Dim totalMatrix As Decimal = detailData.Tables(0).Rows(i).Item("TotalMatrix").ToString()
                        Dim totalCharge As Decimal = detailData.Tables(0).Rows(i).Item("TotalCharge").ToString()
                        ' Dim Matrix As String = GetItemData(String.Format("SELECT SUM(( odp.Cost * odp.Qty ) - ( odp.Qty * ISNULL( odp.Discount, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountB, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountC, 0 ) )) As Matrix FROM OrderDetailsPrice odp INNER JOIN OrderDetails od ON odp.ItemId=od.id WHERE odp.HeaderId='{0}' AND odp.ItemId='{1}' AND odp.Type='Matrix' AND od.Active='1'", Id, itemId))
                        Dim Matrix As String = GetItemData(String.Format("SELECT SUM( odp.Cost - ISNULL( odp.Discount, 0 ) - ISNULL( odp.DiscountB, 0 ) - ISNULL( odp.DiscountC, 0 ) ) AS Matrix  FROM OrderDetailsPrice odp INNER JOIN OrderDetails od ON odp.ItemId= od.id WHERE odp.HeaderId= '{0}' AND odp.ItemId= '{1}' AND odp.Type= 'Matrix' AND od.Active= '1'", Id, itemId))

                        ' Dim Charge As String = GetItemData(String.Format("SELECT SUM(( odp.Cost * odp.Qty ) - ( odp.Qty * ISNULL( odp.Discount, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountB, 0 ) ) - ( odp.Qty * ISNULL( odp.DiscountC, 0 ) )) As Charge FROM OrderDetailsPrice odp INNER JOIN OrderDetails od ON odp.ItemId=od.Id WHERE odp.HeaderId='{0}' AND odp.ItemId='{1}' AND odp.Type='Charge' AND odp.Description NOT LIKE '%Powder Coating%' AND odp.Description NOT LIKE '%Tracking & Interlock%' AND od.Active='1'", Id, itemId))
                        Dim Charge As String = GetItemData(String.Format("SELECT SUM( odp.Cost - ISNULL( odp.Discount, 0 ) - ISNULL( odp.DiscountB, 0 ) - ISNULL( odp.DiscountC, 0 ) ) AS Charge FROM OrderDetailsPrice odp INNER JOIN OrderDetails od ON odp.ItemId=od.Id WHERE odp.HeaderId='{0}' AND odp.ItemId='{1}' AND odp.Type='Charge' AND odp.Description NOT LIKE '%Powder Coating%' AND odp.Description NOT LIKE '%Tracking & Interlock%' AND od.Active='1'", Id, itemId))
                        
                        ' Dim finalCost As Decimal = totalMatrix + totalCharge
                        Dim finalCost As Decimal = If(Matrix = "", 0D, CDec(Matrix)) + If(Charge = "", 0D, CDec(Charge))
                        Dim finalCostString As String = finalCost.ToString(CultureInfo.InvariantCulture)

                        Dim exactProduct As String = detailData.Tables(0).Rows(i).Item("ExactId").ToString()
                        If String.IsNullOrEmpty(exactProduct) Then
                            Dim designId As String = GetItemData("SELECT DesignId FROM HardwareKits WHERE Id='" + UCase(kitId) + "'")
                            Dim blindId As String = GetItemData("SELECT BlindId FROM HardwareKits WHERE Id='" + UCase(kitId) + "'")
                            Dim designName As String = GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
                            Dim blindName As String = GetItemData("SELECT Name FROM Blinds WHERE Id = '" + blindId + "'")

                            Dim exactName As String = designName & " - " & blindName
                            exactProduct = GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + exactName + "'")

                    '         If designName = "Panorama PVC Parts" Then
                    '             Dim exactName As String = designName
                    '             exactProduct = GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + exactName + "'")
                    '         End If
                    '         If designName = "Additional" Then
                    '             Dim exactName As String = blindName
                    '             exactProduct = GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + exactName + "'")
                    '         End If
                        End If

                        Dim productName As String = GetItemData("SELECT Name FROM HardwareKits WHERE Id = '" & UCase(kitId) & "'")
                        Dim width As String = detailData.Tables(0).Rows(i).Item("Width").ToString()
                        Dim drop As String = detailData.Tables(0).Rows(i).Item("Drop").ToString()
                        Dim itemDescription As String = FindDescription(itemId)

                        writer.WriteStartElement("OrderLine")

                        writer.WriteStartElement("Item")
                        writer.WriteAttributeString("code", exactProduct)
                        writer.WriteString("")
                        writer.WriteEndElement()

                        writer.WriteElementString("Quantity", Qty)

                        writer.WriteStartElement("Price")
                        writer.WriteAttributeString("type", "S")

                        writer.WriteStartElement("Currency")
                        writer.WriteAttributeString("code", "AUD")
                        writer.WriteString("")
                        writer.WriteEndElement()

                        writer.WriteElementString("Value", finalCostString)
                        writer.WriteEndElement() ' Price

                        writer.WriteStartElement("Delivery")
                        writer.WriteElementString("Date", etaCustomer)
                        writer.WriteEndElement()

                        writer.WriteElementString("Text", itemDescription)
                        writer.WriteEndElement() ' OrderLine
                    Next

                    writer.WriteEndElement() ' Order
                    writer.WriteEndElement() ' Orders
                    writer.WriteEndElement() ' eExact
                    writer.WriteEndDocument()
                End Using
            End Using
        Next

        Dim filePath As String = Path.Combine(folderPath, fileName)
        Dim xmlFinal As String = sb.ToString().Replace(" />", "/>")
        File.WriteAllText(filePath, xmlFinal, New UTF8Encoding(False))
    End Sub

    Private Function FindDescription(id As String) As string
        Try
            Dim result As String = ""
            Dim Data As Dataset = GetListData(String.Format("SELECT Width, [Drop], SlatSize, DesignId, KitName, BracketType, TubeType, DesignName, BlindName, FabricName, FabricType, FabricColour, CustomerId FROM view_details WHERE Id ='{0}' AND Active=1", id))
            Dim DataCount As Integer = Data.Tables(0).Rows.Count
            Dim DataRow As DataRow = Data.Tables(0).Rows(0)
            IF DataCount > 0 Then

                Dim Width As String = DataRow.Item("Width").ToString()
                Dim Drop As String = DataRow.Item("Drop").ToString()
                Dim SlatSize As String = DataRow.Item("SlatSize").ToString()
                Dim FabricName As String = DataRow.Item("FabricName").ToString()
                Dim FabricType As String = DataRow.Item("FabricType").ToString()
                Dim FabricColour As String = DataRow.Item("FabricColour").ToString()
                Dim DesignId As String = DataRow.Item("DesignId").ToString()
                Dim KitName As String = DataRow.Item("KitName").ToString()
                Dim BracketType As String = DataRow.Item("BracketType").ToString()
                Dim TubeType As String = DataRow.Item("TubeType").ToString()
                Dim DesignName As String = DataRow.Item("DesignName").ToString()
                Dim BlindName As String = DataRow.Item("BlindName").ToString()
                Dim CustomerId As String = DataRow.Item("CustomerId").ToString()

                Dim CustDiscValue As Double = 0
                Dim CustDisc As String = GetItemData(String.Format("SELECT Discount FROM CustomerDiscounts WHERE CustomerData='{0}' AND DesignId='{1}'", CustomerId, DesignId))

                If Not String.IsNullOrWhiteSpace(CustDisc) Then
                    Double.TryParse(CustDisc, CustDiscValue)
                End If

                Dim Disc As String = String.Format("(Inc {0}% Disc. Plus Additional 0% Disc.)", CustDiscValue.ToString("0.##"))
                Dim Size As String = String.Format("{0}x{1}", Width, Drop)

                result = String.Format("{0} {1} {2}", KitName, Size, Disc)

                If DesignName = "Roller Blinds" Then
                    result = String.Format("{0} - {1} #{2} {3} {4}", BlindName, KitName, FabricName, Size, Disc)
                End If

                If DesignName = "Venetian Blinds" Or DesignName = "Veri Shades" Then
                    result = String.Format("{0} {1} {2}", KitName, Size, Disc)
                End If

                If DesignName = "Vertical Blinds" Then
                    SlatSize = SlatSize.Replace("mm", "")

                    result = String.Format("{0}mm {1} #{2} {3} {4}", SlatSize, KitName, FabricName, Size, Disc)
                    If BlindName = "Slat Only" Then
                        result = String.Format("{0}mm {1} #{2} (Drop : {3}mm) {4}", SlatSize, KitName, FabricName, Drop, Disc)
                    End If
                    If BlindName = "Track Only" Then
                        result = String.Format("{0}mm {1} (Width : {2}mm) {3}", SlatSize, KitName, Width, Disc)
                    End If

                End If
            End If

            Return result
        Catch ex As Exception
            Return "Oops... something went wrong"
        End Try
    End Function
End Class
