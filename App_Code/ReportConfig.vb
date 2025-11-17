Imports System.Data
Imports System.Data.SqlClient
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports OfficeOpenXml
Imports OfficeOpenXml.Style

Public Class ReportConfig

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

    Public Function GetItemData_Integer(thisString As String) As Integer
        Dim result As Integer = 0
        Try
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
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Public Sub BlindsDaily(Files As String, Tanggal As DateTime, Period As String)
        Try
            Dim startDate As String = DateTime.Parse(Tanggal).ToString("yyyy-MM-01")
            Dim endDate As String = DateTime.Parse(Tanggal).ToString("yyyy-MM-dd")

            Dim thisQuery As String = "SELECT OrderHeaders.CustomerId AS CustomerId, Customers.Name AS CustomerName FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId = Customers.Id WHERE OrderHeaders.Active = 1 AND CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND (OrderHeaders.Status = 'In Production' OR OrderHeaders.Status = 'Completed') GROUP BY OrderHeaders.CustomerId, Customers.Name ORDER BY Customers.Name ASC"
            Dim thisData As DataSet = GetListData(thisQuery)

            If thisData.Tables(0).Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
                Dim pdfFilePath As String = Files
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                    Dim pageEvent As New ReportEvents() With {
                        .PageTitle = "Blinds Daily Report",
                        .PagePeriod = Period
                    }
                    writer.PageEvent = pageEvent

                    doc.Open()

                    Dim data As String() = {}

                    Dim tableOne As New PdfPTable(13)
                    tableOne.WidthPercentage = 100
                    tableOne.SetWidths(New Single() {0.2F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F})

                    tableOne.AddCell(HeaderCell("CUSTOMER NAME"))
                    tableOne.AddCell(HeaderCellProduct("ROLLER O"))
                    tableOne.AddCell(HeaderCellProduct("ROMAN O"))
                    tableOne.AddCell(HeaderCellProduct("PANEL GLIDE O"))
                    tableOne.AddCell(HeaderCellProduct("PELMET O"))
                    tableOne.AddCell(HeaderCellProduct("VENETIAN O"))
                    tableOne.AddCell(HeaderCellProduct("VERTICAL O"))

                    tableOne.AddCell(HeaderCellTanggal("Today"))
                    tableOne.AddCell(HeaderCellTanggal("MTD"))
                    tableOne.AddCell(HeaderCellTanggal("Today"))
                    tableOne.AddCell(HeaderCellTanggal("MTD"))
                    tableOne.AddCell(HeaderCellTanggal("Today"))
                    tableOne.AddCell(HeaderCellTanggal("MTD"))
                    tableOne.AddCell(HeaderCellTanggal("Today"))
                    tableOne.AddCell(HeaderCellTanggal("MTD"))
                    tableOne.AddCell(HeaderCellTanggal("Today"))
                    tableOne.AddCell(HeaderCellTanggal("MTD"))
                    tableOne.AddCell(HeaderCellTanggal("Today"))
                    tableOne.AddCell(HeaderCellTanggal("MTD"))

                    For partTwo As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim customerId As String = thisData.Tables(0).Rows(partTwo)("CustomerId").ToString()
                        Dim customerName As String = thisData.Tables(0).Rows(partTwo)("CustomerName").ToString()

                        tableOne.AddCell(ContentCell(customerName))
                        ' ROLLER
                        data = {customerId, "50CE8EDF-E106-414C-BDE3-D7AA8F8046D2", endDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "50CE8EDF-E106-414C-BDE3-D7AA8F8046D2", startDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' ROMAN
                        data = {customerId, "B594C6C7-452D-4CB6-8A20-6914BD97B830", endDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "B594C6C7-452D-4CB6-8A20-6914BD97B830", startDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' PANEL GLIDE
                        data = {customerId, "E7959750-5CF8-48E5-9171-CD71B53CDC2F", endDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "E7959750-5CF8-48E5-9171-CD71B53CDC2F", startDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' PELMET
                        data = {customerId, "3734E56C-9FBE-4897-9424-410833B1A1D3", endDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "3734E56C-9FBE-4897-9424-410833B1A1D3", startDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' VENETIAN
                        data = {customerId, "83C7039F-4A2E-4D6A-9389-761664FD9449", endDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "83C7039F-4A2E-4D6A-9389-761664FD9449", startDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' VERTICAL
                        data = {customerId, "B556E35C-CEAC-40F8-A6CF-156601BD57DA", endDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "B556E35C-CEAC-40F8-A6CF-156601BD57DA", startDate, endDate, "Orion"}
                        tableOne.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                    Next

                    ' FOOTER TOTAL
                    tableOne.AddCell(FooterCell("TOTAL"))

                    ' ROLLER
                    data = {"50CE8EDF-E106-414C-BDE3-D7AA8F8046D2", endDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"50CE8EDF-E106-414C-BDE3-D7AA8F8046D2", startDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' ROMAN
                    data = {"B594C6C7-452D-4CB6-8A20-6914BD97B830", endDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"B594C6C7-452D-4CB6-8A20-6914BD97B830", startDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' PANEL GLIDE
                    data = {"E7959750-5CF8-48E5-9171-CD71B53CDC2F", endDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"E7959750-5CF8-48E5-9171-CD71B53CDC2F", startDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' PELMET
                    data = {"3734E56C-9FBE-4897-9424-410833B1A1D3", endDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"3734E56C-9FBE-4897-9424-410833B1A1D3", startDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' VENETIAN
                    data = {"83C7039F-4A2E-4D6A-9389-761664FD9449", endDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"83C7039F-4A2E-4D6A-9389-761664FD9449", startDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' VERTICAL
                    data = {"B556E35C-CEAC-40F8-A6CF-156601BD57DA", endDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"B556E35C-CEAC-40F8-A6CF-156601BD57DA", startDate, endDate, "Orion"}
                    tableOne.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    doc.Add(tableOne)
                    doc.NewPage()

                    Dim tableTwo As New PdfPTable(13)
                    tableTwo.WidthPercentage = 100
                    tableTwo.SetWidths(New Single() {0.2F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F, 0.066F})

                    tableTwo.AddCell(HeaderCell("CUSTOMER NAME"))
                    tableTwo.AddCell(HeaderCellProduct("ROLLER J"))
                    tableTwo.AddCell(HeaderCellProduct("ROMAN J"))
                    tableTwo.AddCell(HeaderCellProduct("PANEL GLIDE J"))
                    tableTwo.AddCell(HeaderCellProduct("PELMET J"))
                    tableTwo.AddCell(HeaderCellProduct("VENETIAN J"))
                    tableTwo.AddCell(HeaderCellProduct("VERTICAL"))

                    tableTwo.AddCell(HeaderCellTanggal("Today"))
                    tableTwo.AddCell(HeaderCellTanggal("MTD"))
                    tableTwo.AddCell(HeaderCellTanggal("Today"))
                    tableTwo.AddCell(HeaderCellTanggal("MTD"))
                    tableTwo.AddCell(HeaderCellTanggal("Today"))
                    tableTwo.AddCell(HeaderCellTanggal("MTD"))
                    tableTwo.AddCell(HeaderCellTanggal("Today"))
                    tableTwo.AddCell(HeaderCellTanggal("MTD"))
                    tableTwo.AddCell(HeaderCellTanggal("Today"))
                    tableTwo.AddCell(HeaderCellTanggal("MTD"))
                    tableTwo.AddCell(HeaderCellTanggal("Today"))
                    tableTwo.AddCell(HeaderCellTanggal("MTD"))

                    For partThree As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim customerId As String = thisData.Tables(0).Rows(partThree)("CustomerId").ToString()
                        Dim customerName As String = thisData.Tables(0).Rows(partThree)("CustomerName").ToString()

                        tableTwo.AddCell(ContentCell(customerName))
                        ' ROLLER
                        data = {customerId, "50CE8EDF-E106-414C-BDE3-D7AA8F8046D2", endDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "50CE8EDF-E106-414C-BDE3-D7AA8F8046D2", startDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' ROMAN
                        data = {customerId, "B594C6C7-452D-4CB6-8A20-6914BD97B830", endDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "B594C6C7-452D-4CB6-8A20-6914BD97B830", startDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' PANEL GLIDE
                        data = {customerId, "E7959750-5CF8-48E5-9171-CD71B53CDC2F", endDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "E7959750-5CF8-48E5-9171-CD71B53CDC2F", startDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' PELMET
                        data = {customerId, "3734E56C-9FBE-4897-9424-410833B1A1D3", endDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "3734E56C-9FBE-4897-9424-410833B1A1D3", startDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' VENETIAN
                        data = {customerId, "83C7039F-4A2E-4D6A-9389-761664FD9449", endDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "83C7039F-4A2E-4D6A-9389-761664FD9449", startDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' VERTICAL
                        data = {customerId, "B556E35C-CEAC-40F8-A6CF-156601BD57DA", endDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "B556E35C-CEAC-40F8-A6CF-156601BD57DA", startDate, endDate, "JKT"}
                        tableTwo.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                    Next

                    tableTwo.AddCell(FooterCell("TOTAL"))

                    ' ROLLER
                    data = {"50CE8EDF-E106-414C-BDE3-D7AA8F8046D2", endDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"50CE8EDF-E106-414C-BDE3-D7AA8F8046D2", startDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' ROMAN
                    data = {"B594C6C7-452D-4CB6-8A20-6914BD97B830", endDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"B594C6C7-452D-4CB6-8A20-6914BD97B830", startDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' PANEL GLIDE
                    data = {"E7959750-5CF8-48E5-9171-CD71B53CDC2F", endDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"E7959750-5CF8-48E5-9171-CD71B53CDC2F", startDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' PELMET
                    data = {"3734E56C-9FBE-4897-9424-410833B1A1D3", endDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"3734E56C-9FBE-4897-9424-410833B1A1D3", startDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' VENETIAN
                    data = {"83C7039F-4A2E-4D6A-9389-761664FD9449", endDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"83C7039F-4A2E-4D6A-9389-761664FD9449", startDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' VERTICAL
                    data = {"B556E35C-CEAC-40F8-A6CF-156601BD57DA", endDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"B556E35C-CEAC-40F8-A6CF-156601BD57DA", startDate, endDate, "JKT"}
                    tableTwo.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    doc.Add(tableTwo)
                    doc.NewPage()

                    Dim tableThree As New PdfPTable(9)
                    tableThree.WidthPercentage = 100
                    tableThree.SetWidths(New Single() {0.2F, 0.1F, 0.1F, 0.1F, 0.1F, 0.1F, 0.1F, 0.1F, 0.1F})

                    tableThree.AddCell(HeaderCell("CUSTOMER NAME"))
                    tableThree.AddCell(HeaderCellProduct("CURTAIN"))
                    tableThree.AddCell(HeaderCellProduct("VERI SHADES"))
                    tableThree.AddCell(HeaderCellProduct("ZEBRA"))
                    tableThree.AddCell(HeaderCellProduct("CELLULAR SHADES"))

                    tableThree.AddCell(HeaderCellTanggal("Today"))
                    tableThree.AddCell(HeaderCellTanggal("MTD"))
                    tableThree.AddCell(HeaderCellTanggal("Today"))
                    tableThree.AddCell(HeaderCellTanggal("MTD"))
                    tableThree.AddCell(HeaderCellTanggal("Today"))
                    tableThree.AddCell(HeaderCellTanggal("MTD"))
                    tableThree.AddCell(HeaderCellTanggal("Today"))
                    tableThree.AddCell(HeaderCellTanggal("MTD"))
                    For partOne As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim customerId As String = thisData.Tables(0).Rows(partOne)("CustomerId").ToString()
                        Dim customerName As String = thisData.Tables(0).Rows(partOne)("CustomerName").ToString()

                        tableThree.AddCell(ContentCell(customerName))

                        ' CURTAIN
                        data = {customerId, "992EF755-D9A4-4423-8C88-687CD935DF11", endDate, endDate, "Orion"}
                        tableThree.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "992EF755-D9A4-4423-8C88-687CD935DF11", startDate, endDate, "Orion"}
                        tableThree.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' VERI SHADES
                        data = {customerId, "68359197-6083-4489-863E-EBB1AF056D92", endDate, endDate, "Orion"}
                        tableThree.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "68359197-6083-4489-863E-EBB1AF056D92", startDate, endDate, "Orion"}
                        tableThree.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' ZEBRA BLIND
                        data = {customerId, "78400FAF-BA6D-4E8F-923B-68A4CE30C0DD", endDate, endDate, "Orion"}
                        tableThree.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "78400FAF-BA6D-4E8F-923B-68A4CE30C0DD", startDate, endDate, "Orion"}
                        tableThree.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' CELLULAR SHADES
                        data = {customerId, "0FCA2BF9-2849-4E2A-B04D-DF8519DC536F", endDate, endDate, "JKT"}
                        tableThree.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "0FCA2BF9-2849-4E2A-B04D-DF8519DC536F", startDate, endDate, "JKT"}
                        tableThree.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                    Next

                    tableThree.AddCell(FooterCell("TOTAL"))

                    ' CURTAIN
                    data = {"992EF755-D9A4-4423-8C88-687CD935DF11", endDate, endDate, "Orion"}
                    tableThree.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"992EF755-D9A4-4423-8C88-687CD935DF11", startDate, endDate, "Orion"}
                    tableThree.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' VERI SHADES
                    data = {"68359197-6083-4489-863E-EBB1AF056D92", endDate, endDate, "Orion"}
                    tableThree.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"68359197-6083-4489-863E-EBB1AF056D92", startDate, endDate, "Orion"}
                    tableThree.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' ZEBRA BLIND
                    data = {"78400FAF-BA6D-4E8F-923B-68A4CE30C0DD", endDate, endDate, "Orion"}
                    tableThree.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"78400FAF-BA6D-4E8F-923B-68A4CE30C0DD", startDate, endDate, "Orion"}
                    tableThree.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    ' CELLULAR SHADES
                    data = {"0FCA2BF9-2849-4E2A-B04D-DF8519DC536F", endDate, endDate, "JKT"}
                    tableThree.AddCell(FooterCell(GetPerDesignBlinds(data)))
                    data = {"0FCA2BF9-2849-4E2A-B04D-DF8519DC536F", startDate, endDate, "JKT"}
                    tableThree.AddCell(FooterCell(GetPerDesignBlinds(data)))

                    doc.Add(tableThree)

                    doc.Close()
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub ShuttersDaily(Files As String, Tanggal As DateTime, Period As String)
        Try
            Dim startDate As String = DateTime.Parse(Tanggal).ToString("yyyy-MM-01")
            Dim endDate As String = DateTime.Parse(Tanggal).ToString("yyyy-MM-dd")

            Dim thisQuery As String = "SELECT OrderHeaders.CustomerId AS CustomerId, Customers.Name AS CustomerName FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId = Customers.Id LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderHeaders.Active = 1 AND CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE' OR Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12') GROUP BY OrderHeaders.CustomerId, Customers.Name ORDER BY Customers.Name ASC"
            Dim thisData As DataSet = GetListData(thisQuery)

            If thisData.Tables(0).Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
                Dim pdfFilePath As String = Files
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                    Dim pageEvent As New ReportEvents() With {
                        .PageTitle = "Shutters Daily Report",
                        .PagePeriod = Period
                    }
                    writer.PageEvent = pageEvent
                    doc.Open()

                    Dim data As String() = {}

                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100
                    table.SetWidths(New Single() {0.4F, 0.15F, 0.15F, 0.15F, 0.15F})

                    table.AddCell(HeaderCell("CUSTOMER NAME"))
                    table.AddCell(HeaderCellProduct("PANORAMA PVC SHUTTERS"))
                    table.AddCell(HeaderCellProduct("EVOLVE SHUTTERS"))

                    table.AddCell(HeaderCellTanggal("Today"))
                    table.AddCell(HeaderCellTanggal("MTD"))
                    table.AddCell(HeaderCellTanggal("Today"))
                    table.AddCell(HeaderCellTanggal("MTD"))

                    For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim customerId As String = thisData.Tables(0).Rows(i)("CustomerId").ToString()
                        Dim customerName As String = thisData.Tables(0).Rows(i)("CustomerName").ToString()

                        table.AddCell(ContentCell(customerName))

                        ' PANORAMA
                        data = {customerId, "0CB7C37F-D478-49BA-94CB-DCDE83FB84C8", endDate, endDate, "Orion"}
                        table.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "0CB7C37F-D478-49BA-94CB-DCDE83FB84C8", startDate, endDate, "Orion"}
                        table.AddCell(ContentCell(GetPerCustomerBlinds(data)))

                        ' EVOLVE
                        data = {customerId, "F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12", endDate, endDate, "Orion"}
                        table.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                        data = {customerId, "F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12", startDate, endDate, "Orion"}
                        table.AddCell(ContentCell(GetPerCustomerBlinds(data)))
                    Next

                    table.AddCell(FooterCell("TOTAL"))
                    ' PANORAMA
                    data = {"0CB7C37F-D478-49BA-94CB-DCDE83FB84C8", endDate, endDate, "Orion"}
                    table.AddCell(ContentCell(GetPerDesignBlinds(data)))
                    data = {"0CB7C37F-D478-49BA-94CB-DCDE83FB84C8", startDate, endDate, "Orion"}
                    table.AddCell(ContentCell(GetPerDesignBlinds(data)))

                    ' EVOLVE
                    data = {"F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12", endDate, endDate, "Orion"}
                    table.AddCell(ContentCell(GetPerDesignBlinds(data)))
                    data = {"F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12", startDate, endDate, "Orion"}
                    table.AddCell(ContentCell(GetPerDesignBlinds(data)))

                    doc.Add(table)
                    doc.Close()
                End Using
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Sub ShuttersMTM(Files As String, StartDate As DateTime, EndDate As DateTime)
        Try
            ' Jalankan untuk DesignId pertama
            Dim query1 As String = "EXEC report_MTM @DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8', @StartMonth = '" & StartDate.ToString("yyyy-MM-dd") & "', @EndMonth = '" & EndDate.ToString("yyyy-MM-dd") & "';"
            Dim ds1 As DataSet = GetListData(query1)
            Dim dt1 As DataTable = ds1.Tables(0)

            ' Jalankan untuk DesignId kedua
            Dim query2 As String = "EXEC report_MTM @DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7', @StartMonth = '" & StartDate.ToString("yyyy-MM-dd") & "', @EndMonth = '" & EndDate.ToString("yyyy-MM-dd") & "';"
            Dim ds2 As DataSet = GetListData(query2)
            Dim dt2 As DataTable = ds2.Tables(0)

            ' Gabungkan dt1 dan dt2 ke satu DataTable
            Dim dt As DataTable = dt1.Clone()

            ' Salin semua dari dt1
            For Each row As DataRow In dt1.Rows
                dt.ImportRow(row)
            Next

            ' Gabungkan dari dt2 (jumlahkan jika CustomerName sama)
            For Each row As DataRow In dt2.Rows
                Dim custName As String = row("CustomerName").ToString()
                Dim existingRow As DataRow = dt.Select("CustomerName = '" & custName.Replace("'", "''") & "'").FirstOrDefault()

                If existingRow IsNot Nothing Then
                    For Each col As DataColumn In dt.Columns
                        If col.ColumnName <> "CustomerName" Then
                            Dim val1 As Decimal = 0
                            Dim val2 As Decimal = 0
                            Decimal.TryParse(existingRow(col.ColumnName).ToString(), val1)
                            Decimal.TryParse(row(col.ColumnName).ToString(), val2)
                            existingRow(col.ColumnName) = val1 + val2
                        End If
                    Next
                Else
                    dt.ImportRow(row)
                End If
            Next

            ' Jika data ada, generate PDF
            If dt.Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
                Dim pdfFilePath As String = Files
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim period As String = StartDate.ToString("MMM yyyy") & " - " & EndDate.ToString("MMM yyyy")

                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                    Dim pageEvent As New ReportEvents() With {
                        .PageTitle = "Shutters Month to Month",
                        .PagePeriod = period
                    }
                    writer.PageEvent = pageEvent
                    doc.Open()

                    Dim columnCount As Integer = dt.Columns.Count
                    Dim table As New PdfPTable(columnCount)
                    table.WidthPercentage = 100

                    table.AddCell(HeaderCellOriginal("Retailer Name"))
                    For Each col As DataColumn In dt.Columns
                        If col.ColumnName <> "CustomerName" AndAlso col.ColumnName <> "Total" Then
                            table.AddCell(HeaderCellOriginal(col.ColumnName))
                        End If
                    Next
                    table.AddCell(HeaderCellOriginal("Total"))

                    ' Data rows
                    For Each row As DataRow In dt.Rows
                        For Each col As DataColumn In dt.Columns
                            table.AddCell(ContentCell(row(col.ColumnName).ToString()))
                        Next
                    Next

                    ' Hitung total per kolom
                    Dim footerRow(columnCount - 1) As Decimal
                    For Each row As DataRow In dt.Rows
                        For i As Integer = 0 To dt.Columns.Count - 1
                            Dim colName As String = dt.Columns(i).ColumnName
                            If colName <> "CustomerName" Then
                                Dim value As Decimal
                                If Decimal.TryParse(row(colName).ToString(), value) Then
                                    footerRow(i) += value
                                End If
                            End If
                        Next
                    Next

                    ' Footer row (total keseluruhan)
                    For i As Integer = 0 To dt.Columns.Count - 1
                        Dim colName As String = dt.Columns(i).ColumnName
                        If colName = "CustomerName" Then
                            table.AddCell(FooterCell("Total"))
                        Else
                            table.AddCell(FooterCell(footerRow(i).ToString("N0")))
                        End If
                    Next

                    doc.Add(table)
                    doc.Close()
                End Using
            End If
        Catch ex As Exception
        End Try

    End Sub

    'Public Sub ShuttersMTM(Files As String, StartDate As DateTime, EndDate As DateTime)
    '    Try
    '        Dim thisQuery As String = "EXEC report_MTM @DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8', @StartMonth = '" + StartDate.ToString("yyyy-MM-dd") + "', @EndMonth = '" + EndDate.ToString("yyyy-MM-dd") + "';"
    '        Dim thisData As DataSet = GetListData(thisQuery)

    '        If thisData.Tables(0).Rows.Count > 0 Then
    '            Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
    '            Dim pdfFilePath As String = Files
    '            Using fs As New FileStream(pdfFilePath, FileMode.Create)
    '                Dim period As String = StartDate.ToString("MMM yyyy") & " - " & EndDate.ToString("MMM yyyy")

    '                Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
    '                Dim pageEvent As New ReportEvents() With {
    '                    .PageTitle = "Shutters Month to Month",
    '                    .PagePeriod = period
    '                }
    '                writer.PageEvent = pageEvent
    '                doc.Open()

    '                Dim dt As DataTable = thisData.Tables(0)
    '                Dim columnCount As Integer = thisData.Tables(0).Columns.Count

    '                Dim table As New PdfPTable(columnCount)
    '                table.WidthPercentage = 100

    '                table.AddCell(HeaderCellOriginal("Retailer Name"))
    '                For Each col As DataColumn In dt.Columns
    '                    If col.ColumnName <> "CustomerName" AndAlso col.ColumnName <> "Total" Then
    '                        table.AddCell(HeaderCellOriginal(col.ColumnName))
    '                    End If
    '                Next
    '                table.AddCell(HeaderCellOriginal("Total"))

    '                For Each row As DataRow In dt.Rows
    '                    For Each col As DataColumn In dt.Columns
    '                        table.AddCell(ContentCell(row(col.ColumnName).ToString()))
    '                    Next
    '                Next

    '                Dim footerRow(columnCount - 1) As Decimal
    '                For Each row As DataRow In dt.Rows
    '                    For i As Integer = 0 To dt.Columns.Count - 1
    '                        Dim colName As String = dt.Columns(i).ColumnName
    '                        If colName <> "CustomerName" Then
    '                            Dim value As Decimal
    '                            If Decimal.TryParse(row(colName).ToString(), value) Then
    '                                footerRow(i) += value
    '                            End If
    '                        End If
    '                    Next
    '                Next

    '                For i As Integer = 0 To dt.Columns.Count - 1
    '                    Dim colName As String = dt.Columns(i).ColumnName
    '                    If colName = "CustomerName" Then
    '                        table.AddCell(FooterCell("Total"))
    '                    Else
    '                        table.AddCell(FooterCell(footerRow(i).ToString("N0")))
    '                    End If
    '                Next

    '                doc.Add(table)

    '                doc.Close()
    '            End Using
    '        End If
    '    Catch ex As Exception
    '    End Try
    'End Sub

    Public Sub ShuttersOrders(Files As String, StartDate As DateTime, EndDate As DateTime)
        Try
            Dim startDateStr As String = StartDate.ToString("yyyy-MM-dd")
            Dim endDateStr As String = EndDate.ToString("yyyy-MM-dd")

            Dim thisQuery As String = "SELECT DISTINCT OrderHeaders.*, Customers.Name AS CustomerName FROM OrderHeaders INNER JOIN Customers ON OrderHeaders.CustomerId = Customers.Id INNER JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId INNER JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderHeaders.Active = 1 AND CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND (OrderHeaders.Status = 'In Production' OR OrderHeaders.Status = 'Completed') ORDER BY Customers.Name ASC"

            Dim thisData As DataSet = GetListData(thisQuery)

            If thisData.Tables(0).Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4.Rotate(), 36, 36, 80, 72)
                Dim pdfFilePath As String = Files
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim period As String = StartDate.ToString("MMM yyyy") & " - " & EndDate.ToString("MMM yyyy")

                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                    Dim pageEvent As New ReportEvents() With {
                        .PageTitle = "Shutters Orders Report",
                        .PagePeriod = period
                    }
                    writer.PageEvent = pageEvent
                    doc.Open()

                    Dim data As String() = {}

                    Dim table As New PdfPTable(9)
                    table.WidthPercentage = 100
                    table.SetWidths(New Single() {0.05F, 0.1F, 0.1F, 0.2F, 0.15F, 0.1F, 0.1F, 0.1F, 0.1F})

                    table.AddCell(HeaderCellOriginal("No"))
                    table.AddCell(HeaderCellOriginal("Job Number"))
                    table.AddCell(HeaderCellOriginal("Job Date"))
                    table.AddCell(HeaderCellOriginal("Retailer Name"))
                    table.AddCell(HeaderCellOriginal("Retailer Order Number"))
                    table.AddCell(HeaderCellOriginal("Customer Name"))
                    table.AddCell(HeaderCellOriginal("Item Count"))
                    table.AddCell(HeaderCellOriginal("Panel Qty"))
                    table.AddCell(HeaderCellOriginal("M2"))

                    For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        Dim headerId As String = thisData.Tables(0).Rows(i).Item("Id").ToString()

                        Dim itemCount As Integer = GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderDetails.HeaderId = '" + headerId + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderDetails.Active=1")

                        Dim panelQty As Integer = GetItemData_Integer("SELECT SUM(OrderDetails.PanelQty) FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderDetails.HeaderId = '" + headerId + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderDetails.Active=1")

                        Dim squareMetre As Integer = GetItemData_Integer("SELECT SUM(OrderDetails.SquareMetre) FROM OrderDetails LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderDetails.HeaderId = '" + headerId + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderDetails.Active=1")

                        Dim jobDate As String = String.Empty
                        If Not thisData.Tables(0).Rows(0).Item("JobDate").ToString() = "" Then
                            jobDate = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("jobDate")).ToString("dd MMM yyyy")
                        End If

                        table.AddCell(ContentCell(i + 1))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("JobId").ToString()))
                        table.AddCell(ContentCell(jobDate))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("CustomerName").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("OrderNumber").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i).Item("OrderName").ToString()))
                        table.AddCell(ContentCell(itemCount.ToString()))
                        table.AddCell(ContentCell(panelQty.ToString()))
                        table.AddCell(ContentCell(squareMetre.ToString()))
                    Next

                    Dim totalItemCount As Integer = GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails INNER JOIN OrderHeaders ON OrderDetails.HeaderId = OrderHeaders.Id LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderHeaders.Active = 1 AND OrderDetails.Active=1")
                    Dim totalPanelQty As Integer = GetItemData_Integer("SELECT SUM(OrderDetails.PanelQty) FROM OrderDetails INNER JOIN OrderHeaders ON OrderDetails.HeaderId = OrderHeaders.Id LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderHeaders.Active = 1 AND OrderDetails.Active=1")
                    Dim totalSquareMetre As Integer = GetItemData_Integer("SELECT SUM(OrderDetails.SquareMetre) FROM OrderDetails INNER JOIN OrderHeaders ON OrderDetails.HeaderId = OrderHeaders.Id LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDateStr + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDateStr + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderHeaders.Active = 1 AND OrderDetails.Active=1")

                    table.AddCell(FooterCellCol4("TOTAL"))
                    table.AddCell(FooterCell(totalItemCount))
                    table.AddCell(FooterCell(totalPanelQty))
                    table.AddCell(FooterCell(totalSquareMetre))

                    doc.Add(table)

                    doc.Close()
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Customers(Files As String, CashSale As String)
        Try
            Dim thisQuery As String = "SELECT Customers.*, CustomerGroups.Name AS GroupName, CASE WHEN Customers.CashSale = 1 THEN 'Yes' ELSE 'No' END AS CashSaleCustomer, CASE WHEN Customers.OnStop = 1 THEN 'Yes' ELSE 'No' END AS OnStopCustomer FROM Customers LEFT JOIN CustomerGroups ON Customers.[Group] = CustomerGroups.Id ORDER BY Customers.Name ASC"
            If CashSale = "Yes" Then
                thisQuery = "SELECT Customers.*, CustomerGroups.Name AS GroupName, CASE WHEN Customers.CashSale = 1 THEN 'Yes' ELSE 'No' END AS CashSaleCustomer, CASE WHEN Customers.OnStop = 1 THEN 'Yes' ELSE 'No' END AS OnStopCustomer FROM Customers LEFT JOIN CustomerGroups ON Customers.[Group] = CustomerGroups.Id WHERE CashSale = 1 ORDER BY Customers.Name ASC"
            End If

            Dim thisData As DataSet = GetListData(thisQuery)

            If thisData.Tables(0).Rows.Count > 0 Then
                Dim doc As New Document(PageSize.A4, 36, 36, 80, 72)
                Dim pdfFilePath As String = Files
                Using fs As New FileStream(pdfFilePath, FileMode.Create)
                    Dim writer As PdfWriter = PdfWriter.GetInstance(doc, fs)
                    Dim pageEvent As New ReportCustomerEvents() With {
                        .PageTitle = "Customer List"
                    }
                    writer.PageEvent = pageEvent

                    doc.Open()

                    Dim table As New PdfPTable(5)
                    table.WidthPercentage = 100
                    table.SetWidths(New Single() {0.38F, 0.2F, 0.2F, 0.12F, 0.1F})

                    table.AddCell(HeaderCellOriginal("CUSTOMER NAME"))
                    table.AddCell(HeaderCellOriginal("TYPE"))
                    table.AddCell(HeaderCellOriginal("GROUP"))
                    table.AddCell(HeaderCellOriginal("CASH SALE"))
                    table.AddCell(HeaderCellOriginal("ON STOP"))

                    For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i)("Name").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i)("Type").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i)("GroupName").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i)("CashSaleCustomer").ToString()))
                        table.AddCell(ContentCell(thisData.Tables(0).Rows(i)("OnStopCustomer").ToString()))
                    Next

                    doc.Add(table)
                    doc.Close()
                End Using
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Sub Fabrics(Files As String, Logo As String)
        Try
            Dim ds As DataSet = GetListData("SELECT * FROM Fabrics WHERE (DesignId LIKE '%E7959750-5CF8-48E5-9171-CD71B53CDC2F%' OR DesignId LIKE '%50CE8EDF-E106-414C-BDE3-D7AA8F8046D2%' OR DesignId LIKE '%B594C6C7-452D-4CB6-8A20-6914BD97B830%' OR DesignId LIKE '%B556E35C-CEAC-40F8-A6CF-156601BD57DA%') AND Active = 1")

            Using package As New ExcelPackage()
                Dim worksheet As ExcelWorksheet = package.Workbook.Worksheets.Add("Worksheet")

                worksheet.View.ShowGridLines = False
                worksheet.Cells("A1:N1").Merge = True
                worksheet.Cells("A2:N2").Merge = True
                worksheet.Cells("A3:N3").Merge = True
                worksheet.Row(1).Height = 40
                worksheet.Row(2).Height = 25
                worksheet.Row(4).Height = 60

                Dim logoPath As String = Logo
                If File.Exists(logoPath) Then
                    Using logoStream As New FileStream(logoPath, FileMode.Open, FileAccess.Read)
                        Dim picture = worksheet.Drawings.AddPicture("Logo", logoStream)
                        picture.SetPosition(0, 0, 0, 0)
                        picture.SetSize(330, 80)
                    End Using
                Else
                    worksheet.Cells("A1").Value = "Logo Not Found"
                End If

                worksheet.Cells("A1").Value = "Lifestyle Emerge Fabric Collection"
                worksheet.Cells("A1").Style.Font.Bold = True
                worksheet.Cells("A1").Style.Font.Size = 28
                worksheet.Cells("A1").Style.HorizontalAlignment = ExcelHorizontalAlignment.Right
                worksheet.Cells("A1").Style.VerticalAlignment = ExcelVerticalAlignment.Center

                worksheet.Cells("A2").Value = "Effective: " & Now.ToString("dd MMM yyyy")
                worksheet.Cells("A2").Style.Font.Size = 14
                worksheet.Cells("A2").Style.HorizontalAlignment = ExcelHorizontalAlignment.Right
                worksheet.Cells("A2").Style.VerticalAlignment = ExcelVerticalAlignment.Center

                Dim headers As String() = {"Price Group", "Fabric Name", "Fabric Type", "Colours Available", "Composition", "Flame Retardant", "PVC Free = P Lead Free = L", "Greenguard Gold", "Fabric Weight (gsm)", "Vertical", "Roller", "Roman", "Panel Glide", "Usable Width (mm)"}
                Dim columnWidths As Double() = {7, 25, 11, 60, 25, 10, 12, 12, 10, 10, 10, 10, 10, 9}

                For colIndex As Integer = 0 To headers.Length - 1
                    Dim cell = worksheet.Cells(4, colIndex + 1)
                    cell.Value = headers(colIndex)
                    cell.Style.WrapText = True
                    cell.Style.Font.Bold = True
                    cell.Style.Fill.PatternType = ExcelFillStyle.Solid
                    cell.Style.Font.Color.SetColor(System.Drawing.Color.White)
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black)
                    cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center
                    cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center
                    worksheet.Column(colIndex + 1).Width = columnWidths(colIndex)
                Next

                Dim rowIndex As Integer = 4
                For Each row As DataRow In ds.Tables(0).Rows
                    rowIndex += 1

                    Dim fabricId As String = row("Id").ToString()
                    Dim designId As String = row("DesignId").ToString()
                    Dim dataColour As DataSet = GetListData("SELECT Colour FROM FabricColours WHERE FabricId = '" & fabricId & "' AND Active = 1")
                    Dim fabricColour As String = String.Join(", ", dataColour.Tables(0).AsEnumerable().Select(Function(r) r("Colour").ToString()))

                    worksheet.Cells(rowIndex, 1).Value = row("Group").ToString().Replace("Group ", "")
                    worksheet.Cells(rowIndex, 2).Value = row("Name")
                    worksheet.Cells(rowIndex, 3).Value = row("Type")
                    worksheet.Cells(rowIndex, 4).Value = fabricColour
                    worksheet.Cells(rowIndex, 5).Value = row("Composition")
                    worksheet.Cells(rowIndex, 6).Value = If(Convert.ToBoolean(row("FlameReterdant")), "Yes", "No")
                    worksheet.Cells(rowIndex, 7).Value = row("PvcLead")
                    worksheet.Cells(rowIndex, 8).Value = If(Convert.ToBoolean(row("GreenguardGold")), "Yes", "No")
                    worksheet.Cells(rowIndex, 9).Value = row("Weight")

                    worksheet.Cells(rowIndex, 10).Value = If(designId.Contains("B556E35C-CEAC-40F8-A6CF-156601BD57DA"), "Yes", "No")
                    worksheet.Cells(rowIndex, 11).Value = If(designId.Contains("50CE8EDF-E106-414C-BDE3-D7AA8F8046D2"), "Yes", "No")
                    worksheet.Cells(rowIndex, 12).Value = If(designId.Contains("B594C6C7-452D-4CB6-8A20-6914BD97B830"), "Yes", "No")
                    worksheet.Cells(rowIndex, 13).Value = If(designId.Contains("E7959750-5CF8-48E5-9171-CD71B53CDC2F"), "Yes", "No")

                    worksheet.Cells(rowIndex, 14).Value = GetItemData_Integer("SELECT TOP 1 Width FROM FabricColours WHERE FabricId = '" + fabricId + "'")

                    worksheet.Row(rowIndex).Height = 30
                    worksheet.Row(rowIndex).Style.WrapText = True
                Next
                File.WriteAllBytes(Files, package.GetAsByteArray())
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Function HeaderCell(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.Rowspan = 2
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        Return thisCell
    End Function

    Private Function HeaderCellOriginal(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 20
        Return thisCell
    End Function

    Private Function HeaderCellProduct(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.Colspan = 2
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 15
        Return thisCell
    End Function

    Private Function HeaderCellTanggal(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 15
        Return thisCell
    End Function

    Private Function ContentCell(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 8)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 15
        Return thisCell
    End Function

    Private Function FooterCell(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_CENTER
        thisCell.MinimumHeight = 15
        Return thisCell
    End Function

    Private Function FooterCellCol4(Text As String) As PdfPCell
        Dim fontStyle As New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)
        Dim thisCell As New PdfPCell(New Phrase(Text, fontStyle))
        thisCell.HorizontalAlignment = Element.ALIGN_CENTER
        thisCell.VerticalAlignment = Element.ALIGN_RIGHT
        thisCell.MinimumHeight = 15
        thisCell.Colspan = 6
        Return thisCell
    End Function

    Private Function GetPerCustomerBlinds(Data As String()) As Integer
        Dim result As Integer = 0
        Try
            Dim customerId As String = Data(0)
            Dim designId As String = Data(1)
            Dim startDate As String = Data(2)
            Dim endDate As String = Data(3)
            Dim factory As String = Data(4)

            Dim thisQuery As String = "SELECT SUM(OrderDetails.TotalBlinds) FROM OrderHeaders LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderHeaders.CustomerId = '" + customerId + "' AND Products.DesignId = '" + UCase(designId).ToString() + "' AND CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND OrderDetails.Active = 1 AND OrderDetails.Production = '" + factory + "'"

            If designId = "0CB7C37F-D478-49BA-94CB-DCDE83FB84C8" Then
                thisQuery = "SELECT SUM(OrderDetails.PanelQty) FROM OrderHeaders LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderHeaders.CustomerId = '" + customerId + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND OrderDetails.Active = 1 AND OrderDetails.Production = '" + factory + "'"
            End If

            If designId = "F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12" Then
                thisQuery = "SELECT SUM(OrderDetails.PanelQty) FROM OrderHeaders LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderHeaders.CustomerId = '" + customerId + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND OrderDetails.Active = 1 AND OrderDetails.Production = '" + factory + "'"
            End If

            If designId = "83C7039F-4A2E-4D6A-9389-761664FD9449" Then
                thisQuery = "SELECT SUM(OrderDetails.TotalBlinds) FROM OrderHeaders LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderHeaders.CustomerId = '" + customerId + "' AND (Products.DesignId = 'B72EEA9A-5FA8-48EC-9307-C66AAAB1AA8F' OR Products.DesignId = 'C105C02C-E587-40FB-8AAD-0F79DD8B63AE' OR Products.DesignId = '83C7039F-4A2E-4D6A-9389-761664FD9449') AND CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND OrderDetails.Active = 1 AND OrderDetails.Production = '" + factory + "'"
            End If

            If designId = "50CE8EDF-E106-414C-BDE3-D7AA8F8046D2" Then
                thisQuery = "SELECT SUM(OrderDetails.TotalBlinds) FROM OrderHeaders LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE OrderHeaders.CustomerId = '" + customerId + "' AND (Products.DesignId = '50CE8EDF-E106-414C-BDE3-D7AA8F8046D2' OR Products.DesignId = '9BD1C03C-F15F-4323-B7A0-CC988B0E231B') AND CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND OrderDetails.Active = 1 AND OrderDetails.Production = '" + factory + "'"
            End If

            result = GetItemData_Integer(thisQuery)
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function

    Private Function GetPerDesignBlinds(Data As String()) As Integer
        Dim result As Integer = 0
        Try
            Dim designId As String = Data(0)
            Dim startDate As String = Data(1)
            Dim endDate As String = Data(2)
            Dim factory As String = Data(3)

            Dim thisQuery As String = "SELECT SUM(OrderDetails.TotalBlinds) FROM OrderHeaders LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND Products.DesignId = '" + designId + "' AND OrderDetails.Active = 1 AND OrderDetails.Production = '" + factory + "'"

            If designId = "0CB7C37F-D478-49BA-94CB-DCDE83FB84C8" Then
                thisQuery = "SELECT SUM(OrderDetails.PanelQty) FROM OrderHeaders LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND (Products.DesignId = '0CB7C37F-D478-49BA-94CB-DCDE83FB84C8' OR Products.DesignId = '28FFCB2E-F966-4D4B-BFBB-1E541BAB1BC7') AND OrderDetails.Active = 1 AND OrderDetails.Production = '" + factory + "'"
            End If

            If designId = "F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12" Then
                thisQuery = "SELECT SUM(OrderDetails.PanelQty) FROM OrderHeaders LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND (Products.DesignId = 'F70CD0D8-06E5-4C99-B8D8-E9506C1A0F12' OR Products.DesignId = 'DAA2D2CD-B5B6-49FA-A9F8-C65A609440BE') AND OrderDetails.Active = 1 AND OrderDetails.Production = '" + factory + "'"
            End If

            ' ROLLER BLIND
            If designId = "50CE8EDF-E106-414C-BDE3-D7AA8F8046D2" Then
                thisQuery = "SELECT SUM(OrderDetails.TotalBlinds) FROM OrderHeaders LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND (Products.DesignId = '9BD1C03C-F15F-4323-B7A0-CC988B0E231B' OR Products.DesignId = '50CE8EDF-E106-414C-BDE3-D7AA8F8046D2') AND OrderDetails.Active = 1 AND OrderDetails.Production = '" + factory + "'"
            End If

            ' VENETIAN
            If designId = "83C7039F-4A2E-4D6A-9389-761664FD9449" Then
                thisQuery = "SELECT SUM(OrderDetails.TotalBlinds) FROM OrderHeaders LEFT JOIN OrderDetails ON OrderHeaders.Id = OrderDetails.HeaderId LEFT JOIN Products ON OrderDetails.ProductId = Products.Id WHERE CONVERT(DATE, OrderHeaders.JobDate) >= '" + startDate + "' AND CONVERT(DATE, OrderHeaders.JobDate) <= '" + endDate + "' AND (Products.DesignId = 'B72EEA9A-5FA8-48EC-9307-C66AAAB1AA8F' OR Products.DesignId = '83C7039F-4A2E-4D6A-9389-761664FD9449' OR Products.DesignId = 'C105C02C-E587-40FB-8AAD-0F79DD8B63AE') AND OrderDetails.Active = 1 AND OrderDetails.Production = '" + factory + "'"
            End If

            result = GetItemData_Integer(thisQuery)
        Catch ex As Exception
            result = 0
        End Try
        Return result
    End Function
End Class

Public Class ReportEvents
    Inherits PdfPageEventHelper

    Public Property PageTitle As String
    Public Property PagePeriod As String

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent
        Dim font As Font = FontFactory.GetFont("Arial", 12, Font.BOLD)

        Dim headerTable As New PdfPTable(2)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True

        headerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim phrase As New Phrase()
        Dim chunk1 As New Chunk(UCase(PageTitle).ToString(), New Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD))
        phrase.Add(chunk1)
        Dim chunk2 As New Chunk(vbCrLf & vbCrLf)
        phrase.Add(chunk2)
        Dim chunk3 As New Chunk("Period : " & PagePeriod & vbCrLf, New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD))
        phrase.Add(chunk3)

        Dim leftHeaderCell As New PdfPCell(phrase)
        leftHeaderCell.Border = 0
        leftHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(leftHeaderCell)

        Dim rightHeaderCell As New PdfPCell(New Phrase("Date : " & DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"), New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        rightHeaderCell.Border = 0
        rightHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightHeaderCell.VerticalAlignment = Element.ALIGN_BOTTOM
        headerTable.AddCell(rightHeaderCell)

        headerTable.WriteSelectedRows(0, -1, 36, document.PageSize.Height - 20, cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True

        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(New Phrase("All information within this report is private and confidential.", New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD)))
        leftFooterCell.Border = 0
        leftFooterCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(leftFooterCell)

        Dim rightFooterCell As New PdfPCell(New Phrase("Page " & writer.PageNumber, New Font(Font.FontFamily.TIMES_ROMAN, 10)))
        rightFooterCell.Border = 0
        rightFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(rightFooterCell)

        footerTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(36), cb)
    End Sub
End Class

Public Class ReportCustomerEvents
    Inherits PdfPageEventHelper

    Public Property PageTitle As String

    Public Overrides Sub OnEndPage(writer As PdfWriter, document As Document)
        Dim cb As PdfContentByte = writer.DirectContent
        Dim font As Font = FontFactory.GetFont("Arial", 12, Font.BOLD)

        Dim headerTable As New PdfPTable(2)
        headerTable.TotalWidth = document.PageSize.Width - 72
        headerTable.LockedWidth = True

        headerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim phrase As New Phrase()
        Dim chunk1 As New Chunk(UCase(PageTitle).ToString(), New Font(Font.FontFamily.TIMES_ROMAN, 10, Font.BOLD))
        phrase.Add(chunk1)
        Dim chunk2 As New Chunk(vbCrLf & vbCrLf)
        phrase.Add(chunk2)

        Dim leftHeaderCell As New PdfPCell(phrase)
        leftHeaderCell.Border = 0
        leftHeaderCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftHeaderCell.VerticalAlignment = Element.ALIGN_TOP
        headerTable.AddCell(leftHeaderCell)

        Dim rightHeaderCell As New PdfPCell(New Phrase("Date : " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), New Font(Font.FontFamily.TIMES_ROMAN, 9, Font.BOLD)))
        rightHeaderCell.Border = 0
        rightHeaderCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightHeaderCell.VerticalAlignment = Element.ALIGN_BOTTOM
        headerTable.AddCell(rightHeaderCell)

        headerTable.WriteSelectedRows(0, -1, 36, document.PageSize.Height - 36, cb)

        Dim footerTable As New PdfPTable(2)
        footerTable.TotalWidth = document.PageSize.Width - 72
        footerTable.LockedWidth = True

        footerTable.SetWidths(New Single() {0.5F, 0.5F})

        Dim leftFooterCell As New PdfPCell(New Phrase("All information within this report is private and confidential.", New Font(Font.FontFamily.TIMES_ROMAN, 8, Font.BOLD)))
        leftFooterCell.Border = 0
        leftFooterCell.HorizontalAlignment = Element.ALIGN_LEFT
        leftFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(leftFooterCell)

        Dim rightFooterCell As New PdfPCell(New Phrase("Page " & writer.PageNumber, New Font(Font.FontFamily.TIMES_ROMAN, 8)))
        rightFooterCell.Border = 0
        rightFooterCell.HorizontalAlignment = Element.ALIGN_RIGHT
        rightFooterCell.VerticalAlignment = Element.ALIGN_BOTTOM
        footerTable.AddCell(rightFooterCell)

        footerTable.WriteSelectedRows(0, -1, 36, document.PageSize.GetBottom(36), cb)
    End Sub
End Class