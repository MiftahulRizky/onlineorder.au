Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf

Public Class QuoteConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Function GetListData(thisString As String) As DataSet
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

    Protected Function GetItemData(thisString As String) As String
        Dim result As String = String.Empty
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
        Return result
    End Function

    Protected Function GetItemData_Decimal(thisString As String) As Decimal
        Dim result As Double = 0.00
        Using thisConn As New SqlConnection(myConn)
            thisConn.Open()
            Using myCmd As New SqlCommand(thisString, thisConn)
                Using rdResult = myCmd.ExecuteReader
                    While rdResult.Read
                        result = rdResult.Item(0)
                    End While
                End Using
            End Using
            thisConn.Close()
        End Using
        Return result
    End Function

    Public Sub BindContentQuote(Id As String, Files As String)
        Dim doc As New Document(PageSize.A4, 36, 36, 140, 180)
        Dim pdfFilePath As String = Files
        Using fs As New FileStream(pdfFilePath, FileMode.Create)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)

            Dim headerData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id='" + Id + "'")

            Dim customerId As String = headerData.Tables(0).Rows(0)("CustomerId").ToString()
            Dim orderId As String = headerData.Tables(0).Rows(0)("OrderId").ToString()
            Dim jobId As String = headerData.Tables(0).Rows(0)("JobId").ToString()
            Dim orderNumber As String = headerData.Tables(0).Rows(0)("OrderNumber").ToString()
            Dim orderName As String = headerData.Tables(0).Rows(0)("OrderName").ToString()
            Dim orderDate As DateTime = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CreatedDate")).ToString("MMM, dd yyyy")
            Dim customerName As String = GetItemData("SELECT Name FROM Customers WHERE Id = '" + customerId + "'")

            Dim customerQuoteData As DataSet = GetListData("SELECT TOP 1 * FROM CustomerQuotes WHERE Id='" + customerId + "'")

            writer.PageEvent = New QuoteEvents()
            doc.Open()

            Dim tableHeader As New PdfPTable(2)
            tableHeader.WidthPercentage = 100
            tableHeader.SetWidths(New Single() {0.75F, 0.25F})

            ' Header Font
            Dim headerFont As New Font(Font.FontFamily.TIMES_ROMAN, 9)

            ' Baris 1, Kolom 1
            Dim textLeftCell1 As New PdfPCell(New Phrase("To: " & customerName, headerFont))
            textLeftCell1.Border = 0
            textLeftCell1.PaddingBottom = 2
            tableHeader.AddCell(textLeftCell1)

            ' Baris 1, Kolom 2
            Dim textRightCell1 As New PdfPCell(New Phrase("QUOTE ORDER", headerFont))
            textRightCell1.Border = 0
            textRightCell1.PaddingBottom = 2
            tableHeader.AddCell(textRightCell1)

            ' Baris 2, Kolom 1
            Dim textLeftCell2 As New PdfPCell(New Phrase("Order Number: " & orderNumber, headerFont))
            textLeftCell2.Border = 0
            textLeftCell2.PaddingBottom = 2
            tableHeader.AddCell(textLeftCell2)

            ' Baris 2, Kolom 2
            Dim textRightCell2 As New PdfPCell(New Phrase("Order #: " & orderId, headerFont))
            textRightCell2.Border = 0
            textRightCell2.PaddingBottom = 2
            tableHeader.AddCell(textRightCell2)

            ' Baris 3, Kolom 1
            Dim textLeftCell3 As New PdfPCell(New Phrase("Customer Name: " & orderName, headerFont))
            textLeftCell3.Border = 0
            textLeftCell3.PaddingBottom = 2
            tableHeader.AddCell(textLeftCell3)

            ' Baris 3, Kolom 2
            Dim textRightCell3 As New PdfPCell(New Phrase("Date: " & orderDate.ToString("dd MMM yyyy"), headerFont))
            textRightCell3.Border = 0
            textRightCell3.PaddingBottom = 2
            tableHeader.AddCell(textRightCell3)

            doc.Add(tableHeader)

            Dim emptyLine As New Paragraph(" ", New Font(Font.FontFamily.TIMES_ROMAN, 10))
            emptyLine.SpacingAfter = 3
            doc.Add(emptyLine)

            Dim table As New PdfPTable(5)
            table.WidthPercentage = 100
            table.SetWidths(New Single() {0.07F, 0.2F, 0.46F, 0.1F, 0.17F})

            table.AddCell(CreateHeaderCell("#"))
            table.AddCell(CreateHeaderCell("Location"))
            table.AddCell(CreateHeaderCell("Item Description"))
            table.AddCell(CreateHeaderCell("Qty"))
            table.AddCell(CreateHeaderCell("Total Price"))

            Dim detailData As DataSet = GetListData("SELECT OrderDetails.*, Products.Name AS ProductName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderDetails.HeaderId='" + Id + "' AND OrderDetails.Active=1 ORDER BY OrderDetails.Number ASC")
            If detailData.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
                    Dim finalCost As Decimal = detailData.Tables(0).Rows(i)("FinalCost")

                    Dim productId As String = detailData.Tables(0).Rows(i)("ProductId").ToString()
                    Dim designId As String = GetItemData("SELECT DesignId FROM Products WHERE Id = '" + productId + "'")
                    Dim blindId As String = GetItemData("SELECT BlindId FROM Products WHERE Id = '" + productId + "'")

                    Dim designName As String = GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
                    Dim blindName As String = GetItemData("SELECT Name FROM Blinds WHERE Id = '" + blindId + "'")

                    Dim fabricColourId As String = detailData.Tables(0).Rows(i)("FabricColourId").ToString()
                    Dim fabricColourIdDB As String = detailData.Tables(0).Rows(i)("FabricColourIdB").ToString()

                    Dim fabricColourName As String = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourId + "'")
                    Dim fabricColourNameDB As String = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdDB + "'")

                    Dim width As String = detailData.Tables(0).Rows(i)("Width").ToString()
                    Dim widthB As String = detailData.Tables(0).Rows(i)("WidthB").ToString()
                    Dim widthC As String = detailData.Tables(0).Rows(i)("WidthC").ToString()
                    Dim widthD As String = detailData.Tables(0).Rows(i)("WidthD").ToString()
                    Dim widthE As String = detailData.Tables(0).Rows(i)("WidthE").ToString()
                    Dim widthF As String = detailData.Tables(0).Rows(i)("WidthF").ToString()

                    Dim drop As String = detailData.Tables(0).Rows(i)("Drop").ToString()

                    Dim productName As String = detailData.Tables(0).Rows(i)("ProductName").ToString()
                    Dim itemDescription As String = productName

                    If designName = "Additional" Then
                        Dim addItem As String = detailData.Tables(0).Rows(i)("AddNumber").ToString()
                        If blindName = "Check Measure" Then
                            itemDescription = blindName & " - " & productName
                            If productName = "Template Fee" Then
                                itemDescription = blindName & " - " & productName & " (" & "Item: " & addItem & ")"
                            End If
                        End If
                        If blindName = "Freight" Then
                            itemDescription = blindName
                        End If
                        If blindName = "Installation" Then
                            itemDescription = blindName & " - " & productName & " (" & "Item: " & addItem & ")"
                            If productName = "1-3 Blinds Only" Then
                                itemDescription = blindName & " - " & productName
                            End If
                        End If
                        If blindName = "Minimum Order Surcharge" Or blindName = "Packing Fee" Then
                            itemDescription = productName
                        End If
                        If blindName = "Takedown" Then
                            itemDescription = blindName & " - " & productName & " (" & "Item: " & addItem & ")"
                        End If
                        If blindName = "Travel Charge" Then
                            itemDescription = blindName & " - " & productName
                        End If
                    End If

                    If designName = "Cellular Shades" Or designName = "Curtain" Or designName = "Roman Blind" Or designName = "Skin Only" Then
                        itemDescription = productName
                        itemDescription += vbLf
                        itemDescription += fabricColourName
                    End If

                    If designName = "Veri Shades" Or designName = "Zebra Blind" Then
                        itemDescription = productName & " - " & fabricColourName
                    End If

                    If designName = "Pelmet" Then
                        itemDescription = productName
                        itemDescription += vbLf
                        itemDescription += fabricColourName
                        If fabricColourId = "0" Or fabricColourId = "" Then
                            itemDescription = productName
                        End If
                    End If

                    If designName = "Panel Glide" Then
                        itemDescription = productName
                        itemDescription += vbLf
                        itemDescription += fabricColourName
                        If blindName = "Track Only" Then
                            itemDescription = productName
                        End If
                    End If

                    If designName = "Vertical" Then
                        itemDescription = productName
                        itemDescription += vbLf
                        itemDescription += fabricColourName
                        If blindName = "Blades Only" Then
                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += fabricColourName
                        End If
                        If blindName = "Track Only" Then
                            itemDescription = productName
                        End If
                    End If

                    If designName = "Roller Blind" Then
                        itemDescription = productName
                        itemDescription += vbLf
                        itemDescription += fabricColourName
                        If blindName = "Double Bracket" Then
                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += fabricColourName
                            itemDescription += vbLf
                            itemDescription += fabricColourNameDB
                        End If
                        If blindName = "Link 2 Blinds Dep" Or blindName = "Link 2 Blinds Ind" Or blindName = "Link 2 Blinds Head to Tail" Then
                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += fabricColourName
                        End If
                        If blindName = "Link 3 Blinds Dep" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 3 Blinds Head to Tail with Ind" Then
                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += fabricColourName
                        End If
                        If blindName = "Link 4 Blinds Ind with Dep" Then
                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += fabricColourName
                        End If

                        If blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Then
                            fabricColourIdDB = detailData.Tables(0).Rows(i)("FabricColourIdC").ToString()
                            fabricColourNameDB = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdDB + "'")

                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += "Front: " & fabricColourName
                            itemDescription += vbLf
                            itemDescription += "Back: " & fabricColourNameDB
                        End If

                        If blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                            fabricColourIdDB = detailData.Tables(0).Rows(i)("FabricColourIdD").ToString()
                            fabricColourNameDB = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdDB + "'")

                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += "Front: " & fabricColourName
                            itemDescription += vbLf
                            itemDescription += "Back: " & fabricColourNameDB
                        End If
                    End If

                    If designName = "Panorama PVC Shutters" Then
                        itemDescription = designName
                        itemDescription += vbLf
                        itemDescription += blindName
                    End If

                    If designName = "Panorama PVC Parts" Then
                        Dim category As String = detailData.Tables(0).Rows(i)("PartCategory").ToString()
                        Dim component As String = detailData.Tables(0).Rows(i)("PartComponent").ToString()

                        itemDescription = designName
                        itemDescription += vbLf
                        itemDescription += category & "," & component
                    End If

                    table.AddCell(CreateCell(detailData.Tables(0).Rows(i)("Number").ToString()))
                    table.AddCell(CreateCell(detailData.Tables(0).Rows(i)("Room").ToString()))
                    table.AddCell(CreateCell(itemDescription))
                    table.AddCell(CreateCell(detailData.Tables(0).Rows(i)("Qty").ToString()))
                    table.AddCell(CreateCell("$ " & finalCost.ToString("N2", enUS)))
                Next
            Else
                table.AddCell(CreateCell("No data available"))
                table.AddCell(CreateCell("No data available"))
                table.AddCell(CreateCell("No data available"))
                table.AddCell(CreateCell("No data available"))
                table.AddCell(CreateCell("No data available"))
            End If

            Dim totalAllPrice As Decimal = GetItemData_Decimal("SELECT SUM(Cost + (Cost * MarkUp / 100)) FROM OrderDetails WHERE HeaderId='" + Id + "' AND Active=1")

            Dim gst As Decimal = totalAllPrice * 10 / 100
            Dim inclGST As Decimal = totalAllPrice + gst

            Dim smallFont As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)

            Dim footerCellTotal As New PdfPCell(New Phrase("Total Excl. GST : ", smallFont))
            footerCellTotal.HorizontalAlignment = Element.ALIGN_RIGHT
            footerCellTotal.VerticalAlignment = Element.ALIGN_MIDDLE
            footerCellTotal.Border = 0
            footerCellTotal.Colspan = 4
            table.AddCell(footerCellTotal)
            table.AddCell(CreateCellFooter("$ " & totalAllPrice.ToString("N2", enUS)))

            Dim footerCellGST As New PdfPCell(New Phrase("GST : ", smallFont))
            footerCellGST.HorizontalAlignment = Element.ALIGN_RIGHT
            footerCellGST.VerticalAlignment = Element.ALIGN_MIDDLE
            footerCellGST.Border = 0
            footerCellGST.Colspan = 4
            table.AddCell(footerCellGST)
            table.AddCell(CreateCellFooter("$ " & gst.ToString("N2", enUS)))

            Dim footerCellTotalInclGST As New PdfPCell(New Phrase("Total incl. GST : ", smallFont))
            footerCellTotalInclGST.HorizontalAlignment = Element.ALIGN_RIGHT
            footerCellTotalInclGST.VerticalAlignment = Element.ALIGN_MIDDLE
            footerCellTotalInclGST.Border = 0
            footerCellTotalInclGST.Colspan = 4
            table.AddCell(footerCellTotalInclGST)
            table.AddCell(CreateCellFooter("$ " & inclGST.ToString("N2", enUS)))
            doc.Add(table)
            doc.Close()
        End Using
    End Sub

    Public Sub BindContentQuoteCustomer(Id As String, Files As String)
        Dim doc As New Document(PageSize.A4, 36, 36, 140, 180)
        Dim pdfFilePath As String = Files
        Using fs As New FileStream(pdfFilePath, FileMode.Create)
            Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)

            Dim headerData As DataSet = GetListData("SELECT * FROM OrderHeaders WHERE Id='" + Id + "'")

            Dim customerId As String = headerData.Tables(0).Rows(0)("CustomerId").ToString()
            Dim orderNumber As String = headerData.Tables(0).Rows(0)("OrderNumber").ToString()
            Dim orderName As String = headerData.Tables(0).Rows(0)("OrderName").ToString()
            Dim orderDate As DateTime = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CreatedDate")).ToString("MMM, dd yyyy")

            Dim customerQuoteData As DataSet = GetListData("SELECT TOP 1 * FROM CustomerQuotes WHERE Id='" + customerId + "'")

            Dim customerLogo As String = customerQuoteData.Tables(0).Rows(0)("Logo").ToString()
            Dim customerTerms As String = customerQuoteData.Tables(0).Rows(0)("Terms").ToString()

            Dim customerAddressData As DataSet = GetListData("SELECT * FROM CustomerAddress WHERE CustomerId='" + customerId + "' AND [Primary]=1")
            Dim fullAddress As String = String.Empty
            If customerAddressData.Tables(0).Rows.Count > 0 Then
                Dim unitNumber As String = customerAddressData.Tables(0).Rows(0)("UnitNumber").ToString()
                Dim street As String = customerAddressData.Tables(0).Rows(0)("Street").ToString()
                Dim address As String = unitNumber & " " & street
                If unitNumber = "" Then
                    address = street
                End If

                Dim suburb As String = customerAddressData.Tables(0).Rows(0)("Suburb").ToString()
                Dim states As String = customerAddressData.Tables(0).Rows(0)("States").ToString()
                Dim postCode As String = customerAddressData.Tables(0).Rows(0)("PostCode").ToString()

                fullAddress = address & ", " & suburb & ", " & states & " " & postCode
            End If

            Dim customerContactData As DataSet = GetListData("SELECT * FROM CustomerContacts WHERE CustomerId='" + customerId + "' AND [Primary]=1")
            Dim fullContact As String = ""
            If customerContactData.Tables(0).Rows.Count > 0 Then
                Dim email As String = customerContactData.Tables(0).Rows(0)("Email").ToString()
                Dim phone As String = customerContactData.Tables(0).Rows(0)("Phone").ToString()
                If email = "" Then : email = "-" : End If
                If phone = "" Then : phone = "-" : End If

                fullContact = "Email: " & email & " , " & "Phone: " & phone
            End If

            Dim quoteData As DataSet = GetListData("SELECT * FROM OrderQuotes WHERE Id = '" + Id + "'")
            Dim quoteAddress As String = quoteData.Tables(0).Rows(0)("Address").ToString()
            Dim quoteSuburb As String = quoteData.Tables(0).Rows(0)("Suburb").ToString()
            Dim quoteStates As String = quoteData.Tables(0).Rows(0)("States").ToString()
            Dim quotePostCode As String = quoteData.Tables(0).Rows(0)("PostCode").ToString()
            Dim quotePhone As String = quoteData.Tables(0).Rows(0)("Phone").ToString()
            Dim quoteEmail As String = quoteData.Tables(0).Rows(0)("Email").ToString()

            Dim fullQuoteAddress As String = quoteAddress & ", " & quoteSuburb & ", " & quoteStates & " " & quotePostCode
            Dim fullQuoteContact As String = "Email: " & quoteEmail & " , " & "Phone: " & quotePhone

            writer.PageEvent = New QuoteCustomerEvents(customerLogo, fullAddress, fullContact, customerTerms)
            doc.Open()

            Dim tableHeader As New PdfPTable(2)
            tableHeader.WidthPercentage = 100
            tableHeader.SetWidths(New Single() {0.75F, 0.25F})

            ' Header Font
            Dim headerFont As New Font(Font.FontFamily.TIMES_ROMAN, 9)

            ' Baris 1, Kolom 1
            Dim textLeftCell1 As New PdfPCell(New Phrase("To: " & orderName, headerFont))
            textLeftCell1.Border = 0
            textLeftCell1.PaddingBottom = 2
            tableHeader.AddCell(textLeftCell1)

            ' Baris 1, Kolom 2
            Dim textRightCell1 As New PdfPCell(New Phrase("QUOTE ORDER", headerFont))
            textRightCell1.Border = 0
            textRightCell1.PaddingBottom = 2
            tableHeader.AddCell(textRightCell1)

            ' Baris 2, Kolom 1
            Dim textLeftCell2 As New PdfPCell(New Phrase("Address: " & fullQuoteAddress, headerFont))
            textLeftCell2.Border = 0
            textLeftCell2.PaddingBottom = 2
            tableHeader.AddCell(textLeftCell2)

            ' Baris 2, Kolom 2
            Dim textRightCell2 As New PdfPCell(New Phrase("Order Number: " & orderNumber, headerFont))
            textRightCell2.Border = 0
            textRightCell2.PaddingBottom = 2
            tableHeader.AddCell(textRightCell2)

            ' Baris 3, Kolom 1
            Dim textLeftCell3 As New PdfPCell(New Phrase(fullQuoteContact, headerFont))
            textLeftCell3.Border = 0
            textLeftCell3.PaddingBottom = 2
            tableHeader.AddCell(textLeftCell3)

            ' Baris 3, Kolom 2
            Dim textRightCell3 As New PdfPCell(New Phrase("Date: " & orderDate.ToString("dd MM yyyy"), headerFont))
            textRightCell3.Border = 0
            textRightCell3.PaddingBottom = 2
            tableHeader.AddCell(textRightCell3)

            doc.Add(tableHeader)

            Dim emptyLine As New Paragraph(" ", New Font(Font.FontFamily.TIMES_ROMAN, 10))
            emptyLine.SpacingAfter = 3
            doc.Add(emptyLine)

            Dim table As New PdfPTable(5)
            table.WidthPercentage = 100
            table.SetWidths(New Single() {0.07F, 0.2F, 0.46F, 0.1F, 0.17F})

            table.AddCell(CreateHeaderCell("#"))
            table.AddCell(CreateHeaderCell("Location"))
            table.AddCell(CreateHeaderCell("Item Description"))
            table.AddCell(CreateHeaderCell("Qty"))
            table.AddCell(CreateHeaderCell("Total Price"))

            Dim detailData As DataSet = GetListData("SELECT OrderDetails.*, Products.Name AS ProductName FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderDetails.HeaderId='" + Id + "' AND OrderDetails.Active=1 ORDER BY OrderDetails.Number ASC")
            If detailData.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
                    Dim finalCost As Decimal = detailData.Tables(0).Rows(i)("FinalCost")
                    Dim markUp As Integer = detailData.Tables(0).Rows(i)("MarkUp")

                    Dim sumfinalCost As Decimal = Math.Round(finalCost + (finalCost * markUp / 100), 2)

                    Dim productId As String = detailData.Tables(0).Rows(i)("ProductId").ToString()
                    Dim designId As String = GetItemData("SELECT DesignId FROM Products WHERE Id = '" + productId + "'")
                    Dim blindId As String = GetItemData("SELECT BlindId FROM Products WHERE Id = '" + productId + "'")

                    Dim designName As String = GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
                    Dim blindName As String = GetItemData("SELECT Name FROM Blinds WHERE Id = '" + blindId + "'")

                    Dim fabricColourId As String = detailData.Tables(0).Rows(i)("FabricColourId").ToString()
                    Dim fabricColourIdDB As String = detailData.Tables(0).Rows(i)("FabricColourIdB").ToString()

                    Dim fabricColourName As String = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourId + "'")
                    Dim fabricColourNameDB As String = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdDB + "'")

                    Dim width As String = detailData.Tables(0).Rows(i)("Width").ToString()
                    Dim widthB As String = detailData.Tables(0).Rows(i)("WidthB").ToString()
                    Dim widthC As String = detailData.Tables(0).Rows(i)("WidthC").ToString()
                    Dim widthD As String = detailData.Tables(0).Rows(i)("WidthD").ToString()
                    Dim widthE As String = detailData.Tables(0).Rows(i)("WidthE").ToString()
                    Dim widthF As String = detailData.Tables(0).Rows(i)("WidthF").ToString()

                    Dim drop As String = detailData.Tables(0).Rows(i)("Drop").ToString()

                    Dim productName As String = detailData.Tables(0).Rows(i)("ProductName").ToString()
                    Dim itemDescription As String = productName

                    If designName = "Cellular Shades" Or designName = "Curtain" Or designName = "Roman Blind" Or designName = "Skin Only" Then
                        itemDescription = productName
                        itemDescription += vbLf
                        itemDescription += fabricColourName
                    End If

                    If designName = "Veri Shades" Or designName = "Zebra Blind" Then
                        itemDescription = productName & " - " & fabricColourName
                    End If

                    If designName = "Pelmet" Then
                        itemDescription = productName
                        itemDescription += vbLf
                        itemDescription += fabricColourName
                        If fabricColourId = "0" Or fabricColourId = "" Then
                            itemDescription = productName
                        End If
                    End If

                    If designName = "Panel Glide" Then
                        itemDescription = productName
                        itemDescription += vbLf
                        itemDescription += fabricColourName
                        If blindName = "Track Only" Then
                            itemDescription = productName
                        End If
                    End If

                    If designName = "Vertical" Then
                        itemDescription = productName
                        itemDescription += vbLf
                        itemDescription += fabricColourName
                        If blindName = "Blades Only" Then
                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += fabricColourName
                        End If
                        If blindName = "Track Only" Then
                            itemDescription = productName
                        End If
                    End If

                    If designName = "Roller Blind" Then
                        itemDescription = productName
                        itemDescription += vbLf
                        itemDescription += fabricColourName
                        If blindName = "Double Bracket" Then
                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += fabricColourName
                            itemDescription += vbLf
                            itemDescription += fabricColourNameDB
                        End If
                        If blindName = "Link 2 Blinds Dep" Or blindName = "Link 2 Blinds Ind" Or blindName = "Link 2 Blinds Head to Tail" Then
                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += fabricColourName
                        End If
                        If blindName = "Link 3 Blinds Dep" Or blindName = "Link 3 Blinds Ind with Dep" Or blindName = "Link 3 Blinds Head to Tail with Ind" Then
                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += fabricColourName
                        End If
                        If blindName = "Link 4 Blinds Ind with Dep" Then
                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += fabricColourName
                        End If

                        If blindName = "DB Link 2 Blinds Dep" Or blindName = "DB Link 2 Blinds Ind" Then
                            fabricColourIdDB = detailData.Tables(0).Rows(i)("FabricColourIdC").ToString()
                            fabricColourNameDB = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdDB + "'")

                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += "Front: " & fabricColourName
                            itemDescription += vbLf
                            itemDescription += "Back: " & fabricColourNameDB
                        End If

                        If blindName = "DB Link 3 Blinds Dep" Or blindName = "DB Link 3 Blinds Ind with Dep" Then
                            fabricColourIdDB = detailData.Tables(0).Rows(i)("FabricColourIdD").ToString()
                            fabricColourNameDB = GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdDB + "'")

                            itemDescription = productName
                            itemDescription += vbLf
                            itemDescription += "Front: " & fabricColourName
                            itemDescription += vbLf
                            itemDescription += "Back: " & fabricColourNameDB
                        End If
                    End If

                    If designName = "Panorama PVC Shutters" Then
                        itemDescription = designName
                        itemDescription += vbLf
                        itemDescription += blindName
                    End If

                    If designName = "Panorama PVC Parts" Then
                        Dim category As String = detailData.Tables(0).Rows(i)("PartCategory").ToString()
                        Dim component As String = detailData.Tables(0).Rows(i)("PartComponent").ToString()

                        itemDescription = designName
                        itemDescription += vbLf
                        itemDescription += category & "," & component
                    End If

                    table.AddCell(CreateCell(detailData.Tables(0).Rows(i)("Number").ToString()))
                    table.AddCell(CreateCell(detailData.Tables(0).Rows(i)("Room").ToString()))
                    table.AddCell(CreateCell(itemDescription))
                    table.AddCell(CreateCell(detailData.Tables(0).Rows(i)("Qty").ToString()))
                    table.AddCell(CreateCell("$ " & sumfinalCost.ToString("N2", enUS)))
                Next
            Else
                table.AddCell(CreateCell("No data available"))
                table.AddCell(CreateCell("No data available"))
                table.AddCell(CreateCell("No data available"))
                table.AddCell(CreateCell("No data available"))
                table.AddCell(CreateCell("No data available"))
            End If

            Dim totalAllPrice As Decimal = GetItemData_Decimal("SELECT SUM(FinalCost + (FinalCost * MarkUp / 100)) FROM OrderDetails WHERE HeaderId='" + Id + "' AND Active=1")

            Dim quoteDiscount As Decimal = quoteData.Tables(0).Rows(0)("Discount")
            Dim quoteInstallation As Decimal = quoteData.Tables(0).Rows(0)("Installation")
            Dim quoteCheckMeasure As Decimal = quoteData.Tables(0).Rows(0)("CheckMeasure")
            Dim quoteFreight As Decimal = quoteData.Tables(0).Rows(0)("Freight")

            Dim gst As Decimal = (totalAllPrice - quoteDiscount + quoteInstallation + quoteCheckMeasure + quoteFreight) * 10 / 100

            Dim inclGST As Decimal = totalAllPrice - quoteDiscount + quoteInstallation + quoteCheckMeasure + quoteFreight + gst

            Dim smallFont As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)

            Dim footerCellTotal As New PdfPCell(New Phrase("Total Excl. GST : ", smallFont))
            footerCellTotal.HorizontalAlignment = Element.ALIGN_RIGHT
            footerCellTotal.VerticalAlignment = Element.ALIGN_MIDDLE
            footerCellTotal.Border = 0
            footerCellTotal.Colspan = 4
            table.AddCell(footerCellTotal)
            table.AddCell(CreateCellFooter("$ " & totalAllPrice.ToString("N2", enUS)))

            'DISCOUNT
            If quoteDiscount > 0 Then
                Dim footerCellDiscount As New PdfPCell(New Phrase("Discount : ", smallFont))
                footerCellDiscount.HorizontalAlignment = Element.ALIGN_RIGHT
                footerCellDiscount.VerticalAlignment = Element.ALIGN_MIDDLE
                footerCellDiscount.Border = 0
                footerCellDiscount.Colspan = 4
                table.AddCell(footerCellDiscount)
                table.AddCell(CreateCellFooter("- $ " & quoteDiscount.ToString("N2", enUS)))
            End If

            'INSTALLATION
            If quoteInstallation > 0 Then
                Dim footerCellInstallation As New PdfPCell(New Phrase("Installation : ", smallFont))
                footerCellInstallation.HorizontalAlignment = Element.ALIGN_RIGHT
                footerCellInstallation.VerticalAlignment = Element.ALIGN_MIDDLE
                footerCellInstallation.Border = 0
                footerCellInstallation.Colspan = 4
                table.AddCell(footerCellInstallation)
                table.AddCell(CreateCellFooter("$ " & quoteInstallation.ToString("N2", enUS)))
            End If

            'CHECK MEASURE
            If quoteCheckMeasure > 0 Then
                Dim footerCellMeasure As New PdfPCell(New Phrase("Check Measure : ", smallFont))
                footerCellMeasure.HorizontalAlignment = Element.ALIGN_RIGHT
                footerCellMeasure.VerticalAlignment = Element.ALIGN_MIDDLE
                footerCellMeasure.Border = 0
                footerCellMeasure.Colspan = 4
                table.AddCell(footerCellMeasure)
                table.AddCell(CreateCellFooter("$ " & quoteCheckMeasure.ToString("N2", enUS)))
            End If

            'FREIGHT
            If quoteFreight > 0 Then
                Dim footerCellFreight As New PdfPCell(New Phrase("Freight : ", smallFont))
                footerCellFreight.HorizontalAlignment = Element.ALIGN_RIGHT
                footerCellFreight.VerticalAlignment = Element.ALIGN_MIDDLE
                footerCellFreight.Border = 0
                footerCellFreight.Colspan = 4
                table.AddCell(footerCellFreight)
                table.AddCell(CreateCellFooter("$ " & quoteFreight.ToString("N2", enUS)))
            End If

            'GST
            Dim footerCellGST As New PdfPCell(New Phrase("GST : ", smallFont))
            footerCellGST.HorizontalAlignment = Element.ALIGN_RIGHT
            footerCellGST.VerticalAlignment = Element.ALIGN_MIDDLE
            footerCellGST.Border = 0
            footerCellGST.Colspan = 4
            table.AddCell(footerCellGST)
            table.AddCell(CreateCellFooter("$ " & gst.ToString("N2", enUS)))

            'TOTAL INC GST
            Dim footerCellTotalInclGST As New PdfPCell(New Phrase("Total incl. GST : ", smallFont))
            footerCellTotalInclGST.HorizontalAlignment = Element.ALIGN_RIGHT
            footerCellTotalInclGST.VerticalAlignment = Element.ALIGN_MIDDLE
            footerCellTotalInclGST.Border = 0
            footerCellTotalInclGST.Colspan = 4
            table.AddCell(footerCellTotalInclGST)
            table.AddCell(CreateCellFooter("$ " & inclGST.ToString("N2", enUS)))

            doc.Add(table)
            doc.Close()
        End Using
    End Sub

    Private Function CreateHeaderCell(text As String) As PdfPCell
        Dim smallFont As New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)
        Dim cell As New PdfPCell(New Phrase(text, smallFont))
        cell.MinimumHeight = 26
        cell.HorizontalAlignment = Element.ALIGN_CENTER
        cell.VerticalAlignment = Element.ALIGN_MIDDLE
        cell.PaddingBottom = 6
        Return cell
    End Function

    Private Function CreateCell(text As String) As PdfPCell
        Dim smallFont As New Font(Font.FontFamily.TIMES_ROMAN, 9)
        Dim lines As Integer = text.Split({vbLf, vbCrLf}, StringSplitOptions.None).Length
        Dim lineHeight As Single = 12
        Dim calculatedHeight As Single = lines * lineHeight

        Dim cell As New PdfPCell(New Phrase(text, smallFont))
        cell.HorizontalAlignment = Element.ALIGN_CENTER
        cell.VerticalAlignment = Element.ALIGN_MIDDLE
        cell.MinimumHeight = calculatedHeight
        cell.PaddingBottom = 6
        Return cell
    End Function

    Private Function CreateCellFooter(text As String) As PdfPCell
        Dim smallFont As New Font(Font.FontFamily.TIMES_ROMAN, 9)
        Dim cell As New PdfPCell(New Phrase(text, smallFont))
        cell.MinimumHeight = 20
        cell.Border = 0
        cell.HorizontalAlignment = Element.ALIGN_CENTER
        cell.VerticalAlignment = Element.ALIGN_MIDDLE
        Return cell
    End Function
End Class

Public Class QuoteEvents
    Inherits PdfPageEventHelper

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent
        Dim font As Font = FontFactory.GetFont("Arial", 12, Font.BOLD)

        Dim headerTable As New PdfPTable(1)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True

        Dim imageUrl As String = "https://shutters.onlineorder.au/Content/static/ShutterLogo.png"
        Dim img As Image = Image.GetInstance(imageUrl)
        img.ScaleToFit(120, 60)

        Dim imgCell As New PdfPCell(img)
        imgCell.Border = 0
        imgCell.HorizontalAlignment = Element.ALIGN_LEFT
        imgCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(imgCell)

        Dim phrase As New Phrase()
        Dim chunkAddress As New Chunk("28 Stoddart Road, Prospect NSW 2148" & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10))
        phrase.Add(chunkAddress)
        Dim chunkContact As New Chunk("Phone: 02 9688 1555", New Font(Font.FontFamily.TIMES_ROMAN, 10))
        phrase.Add(chunkContact)

        Dim headerCell As New PdfPCell(phrase)
        headerCell.Border = 0
        headerCell.HorizontalAlignment = Element.ALIGN_LEFT
        headerCell.VerticalAlignment = Element.ALIGN_TOP
        headerCell.PaddingTop = 10
        headerTable.AddCell(headerCell)

        headerTable.WriteSelectedRows(0, -1, 36, document.PageSize.Height - 20, cb)

        Dim termsTable As New PdfPTable(1)
        termsTable.TotalWidth = document.PageSize.Width - 72
        termsTable.LockedWidth = True

        Dim termsText As String = "• Quote valid for 30 days from date of issue." & vbCrLf &
                                  "• Quote is subject to Check Measure or finalised order." & vbCrLf &
                                  "• Clear access to windows is necessary prior to check measure (if applicable)." & vbCrLf &
                                  "• Additional charges may incur for removal of blinds and/or shutters before the installation of produdcts (if applicable)."

        Dim termsPhrase As New Phrase()
        termsPhrase.Add(New Chunk("Terms & Conditions:" & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        termsPhrase.Add(New Chunk(vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10)))
        termsPhrase.Add(New Chunk(termsText & vbCrLf & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10)))

        Dim termsCell As New PdfPCell(termsPhrase)
        termsCell.Border = 0
        termsCell.HorizontalAlignment = Element.ALIGN_LEFT
        termsCell.VerticalAlignment = Element.ALIGN_TOP
        termsCell.PaddingTop = 5
        termsCell.PaddingBottom = 5
        termsTable.AddCell(termsCell)

        termsTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(130), cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True

        footerTable.SetWidths(New Single() {0.85F, 0.15F})

        Dim leftFooterPhrase As New Phrase("Printed on: " & DateTime.Now.ToString("dd MMM yyyy"), New Font(Font.FontFamily.TIMES_ROMAN, 10))
        Dim leftFooterCell As New PdfPCell(leftFooterPhrase)
        leftFooterCell.Border = 0
        leftFooterCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftFooterCell.VerticalAlignment = Element.ALIGN_TOP
        leftFooterCell.PaddingTop = 5
        leftFooterCell.PaddingBottom = 5
        footerTable.AddCell(leftFooterCell)

        Dim rightFooterPhrase As New Phrase("Page " & writer.PageNumber, New Font(Font.FontFamily.TIMES_ROMAN, 10))
        Dim rightFooterCell As New PdfPCell(rightFooterPhrase)
        rightFooterCell.Border = 0
        rightFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightFooterCell.VerticalAlignment = Element.ALIGN_TOP
        rightFooterCell.PaddingTop = 5
        rightFooterCell.PaddingBottom = 5
        footerTable.AddCell(rightFooterCell)

        footerTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(40), cb)
    End Sub
End Class

Public Class QuoteCustomerEvents
    Inherits PdfPageEventHelper

    Private logoText As String
    Private addressText As String
    Private contactText As String
    Private termsText As String

    Public Sub New(Logo As String, Address As String, Contact As String, Terms As String)
        Me.logoText = Logo
        Me.addressText = Address
        Me.contactText = Contact
        Me.termsText = Terms
    End Sub

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent
        Dim font As Font = FontFactory.GetFont("Arial", 12, Font.BOLD)

        Dim headerTable As New PdfPTable(1)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True

        Try
            Dim imageUrl As String = "https://shutters.onlineorder.au/Content/static/customers/" & Me.logoText
            Dim img As Image = Image.GetInstance(imageUrl)

            img.ScaleToFit(120, 60)

            Dim imgCell As New PdfPCell(img)
            imgCell.Border = 0
            imgCell.HorizontalAlignment = Element.ALIGN_LEFT
            imgCell.VerticalAlignment = Element.ALIGN_TOP
            headerTable.AddCell(imgCell)
        Catch ex As Exception
            Dim errorMsg As String = "Logo not found"
            Dim errorPhrase As New Phrase(errorMsg, New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD))
            Dim errorCell As New PdfPCell(errorPhrase)
            errorCell.Border = 0
            errorCell.HorizontalAlignment = Element.ALIGN_LEFT
            errorCell.VerticalAlignment = Element.ALIGN_TOP
            headerTable.AddCell(errorCell)
        End Try


        Dim phrase As New Phrase()
        Dim chunkAddress As New Chunk(Me.addressText & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10))
        phrase.Add(chunkAddress)
        Dim chunkContact As New Chunk(Me.contactText, New Font(Font.FontFamily.TIMES_ROMAN, 10))
        phrase.Add(chunkContact)

        Dim headerCell As New PdfPCell(phrase)
        headerCell.Border = 0
        headerCell.HorizontalAlignment = Element.ALIGN_LEFT
        headerCell.VerticalAlignment = Element.ALIGN_TOP
        headerCell.PaddingTop = 10
        headerTable.AddCell(headerCell)

        headerTable.WriteSelectedRows(0, -1, 36, document.PageSize.Height - 20, cb)

        Dim termsTable As New PdfPTable(1)
        termsTable.TotalWidth = document.PageSize.Width - 72
        termsTable.LockedWidth = True

        Dim termsPhrase As New Phrase()
        termsPhrase.Add(New Chunk("Terms & Conditions:" & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        termsPhrase.Add(New Chunk(vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10))) ' Baris kosong
        termsPhrase.Add(New Chunk(Me.termsText & vbCrLf & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10)))

        Dim termsCell As New PdfPCell(termsPhrase)
        termsCell.Border = 0
        termsCell.HorizontalAlignment = Element.ALIGN_LEFT
        termsCell.VerticalAlignment = Element.ALIGN_TOP
        termsCell.PaddingTop = 5
        termsCell.PaddingBottom = 5
        termsTable.AddCell(termsCell)

        termsTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(160), cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True

        footerTable.SetWidths(New Single() {0.85F, 0.15F})

        Dim leftFooterPhrase As New Phrase("Printed on: " & DateTime.Now.ToString("dd MMM yyyy"), New Font(Font.FontFamily.TIMES_ROMAN, 10))
        Dim leftFooterCell As New PdfPCell(leftFooterPhrase)
        leftFooterCell.Border = 0
        leftFooterCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftFooterCell.VerticalAlignment = Element.ALIGN_TOP
        leftFooterCell.PaddingTop = 5
        leftFooterCell.PaddingBottom = 5
        footerTable.AddCell(leftFooterCell)

        Dim rightFooterPhrase As New Phrase("Page " & writer.PageNumber, New Font(Font.FontFamily.TIMES_ROMAN, 10))
        Dim rightFooterCell As New PdfPCell(rightFooterPhrase)
        rightFooterCell.Border = 0
        rightFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightFooterCell.VerticalAlignment = Element.ALIGN_TOP
        rightFooterCell.PaddingTop = 5
        rightFooterCell.PaddingBottom = 5
        footerTable.AddCell(rightFooterCell)

        footerTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(40), cb)
    End Sub
End Class
