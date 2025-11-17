Imports System.Data
Imports System.Data.SqlClient

Partial Class Shipment_Detail
    Inherits Page

    Dim shipmentCfg As New ShipmentConfig
    Dim mailCfg As New MailConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("shipmentId") = "" Then
            Response.Redirect("~/shipment/", False)
            Exit Sub
        End If

        lblShipmentId.Text = Session("shipmentId")
        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindDataShipment(lblShipmentId.Text)
        End If
    End Sub

    Protected Sub btnEdit_Click(sender As Object, e As EventArgs)
        MessageError_Edit(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showEdit(); };"
        Try
            Dim myData As DataSet = shipmentCfg.GetListData("SELECT * FROM OrderShipments WHERE Id = '" + lblShipmentId.Text + "'")

            txtShipmentNumber.Text = myData.Tables(0).Rows(0).Item("ShipmentNumber").ToString()
            txtETAPort.Text = Convert.ToDateTime(myData.Tables(0).Rows(0).Item("ETAPort")).ToString("yyyy-MM-dd")
            txtETACutomer.Text = Convert.ToDateTime(myData.Tables(0).Rows(0).Item("ETACustomer")).ToString("yyyy-MM-dd")

            ClientScript.RegisterStartupScript(Me.GetType(), "showEdit", thisScript, True)
        Catch ex As Exception
            MessageError_Edit(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Edit(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnEdit_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showEdit", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitComplete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderShipments SET Completed = 1 WHERE Id=@ShipmentId UPDATE OrderHeaders SET Status='Completed', CompletedDate=GETDATE() WHERE ShipmentId=@ShipmentId")
                    myCmd.Parameters.AddWithValue("@ShipmentId", lblShipmentId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            BindDataShipment(lblShipmentId.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitComplete_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitEdit_Click(sender As Object, e As EventArgs)
        MessageError_Edit(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showEdit(); };"
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderShipments SET ShipmentNumber=@ShipmentNumber, ETAPort=@ETAPort, ETACustomer=@ETACustomer WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblShipmentId.Text)
                    myCmd.Parameters.AddWithValue("@ShipmentNumber", txtShipmentNumber.Text)
                    myCmd.Parameters.AddWithValue("@ETAPort", txtETAPort.Text)
                    myCmd.Parameters.AddWithValue("@ETACustomer", txtETACutomer.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/shipment/detail", False)
        Catch ex As Exception
            MessageError_Edit(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Edit(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitEdit_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showEdit", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderShipments WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblShipmentId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders SET ShipmentId = NULL WHERE ShipmentId=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblShipmentId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/shipment", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitDelete_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub chkAll_CheckedChanged(sender As Object, e As EventArgs)
        Dim isChecked As Boolean = (TryCast(sender, CheckBox)).Checked
        For Each row As GridViewRow In gvList.Rows
            TryCast(row.FindControl("chkRow"), CheckBox).Checked = isChecked
        Next
        VisibleEmail()
    End Sub

    Protected Sub chkRow_CheckedChanged(sender As Object, e As EventArgs)
        Dim isChecked As Boolean = True
        For Each row As GridViewRow In gvList.Rows
            If Not (TryCast(row.FindControl("chkRow"), CheckBox)).Checked Then
                isChecked = False
                Exit For
            End If
        Next
        Dim chkAll As CheckBox = TryCast(gvList.HeaderRow.FindControl("chkAll"), CheckBox)
        chkAll.Checked = isChecked
        VisibleEmail()
    End Sub

    Protected Sub btnSubmitNewShipment_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            For Each row As GridViewRow In gvList.Rows
                Dim chkRow As CheckBox = CType(row.FindControl("chkRow"), CheckBox)

                If chkRow IsNot Nothing AndAlso chkRow.Checked Then
                    Dim id As String = row.Cells(1).Text
                    mailCfg.MailNewShipment(id)
                End If
            Next
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitAmendedShipment_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitAmendedShipment_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            For Each row As GridViewRow In gvList.Rows
                Dim chkRow As CheckBox = CType(row.FindControl("chkRow"), CheckBox)

                If chkRow IsNot Nothing AndAlso chkRow.Checked Then
                    Dim id As String = row.Cells(1).Text

                    mailCfg.MailAmendedShipment(id)
                End If
            Next
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitAmendedShipment_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDataShipment(shipmentId As String)
        MessageError(False, String.Empty)
        Try
            Dim headerData As DataSet = shipmentCfg.GetListData("SELECT OrderShipments.*, CustomerLogins.FullName AS FullName FROM OrderShipments LEFT JOIN CustomerLogins ON OrderShipments.CreatedBy = CustomerLogins.Id WHERE OrderShipments.Id = '" + shipmentId + "'")

            If headerData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/shipment", False)
                Exit Sub
            End If

            lblShipmentNumber.Text = headerData.Tables(0).Rows(0).Item("ShipmentNumber").ToString()
            lblEtaPort.Text = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("ETAPort")).ToString("dd MMM yyyy")
            lblEtaCustomer.Text = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("ETACustomer")).ToString("dd MMM yyyy")
            lblCreatedBy.Text = headerData.Tables(0).Rows(0).Item("FullName").ToString()
            lblCreatedDate.Text = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CreatedDate")).ToString("dd MMM yyyy")
            lblCompleted.Text = Convert.ToInt32(headerData.Tables(0).Rows(0).Item("Completed"))

            aComplete.Visible = False
            btnEdit.Visible = False
            aDelete.Visible = False
            If lblCompleted.Text = 0 Then
                aComplete.Visible = True
                btnEdit.Visible = True
                aDelete.Visible = True
            End If

            gvList.DataSource = shipmentCfg.GetListData("SELECT OrderHeaders.*, Customers.Name AS CustomerName, CASE WHEN Customers.CashSale = 1 THEN 'Cash Sale' ELSE 'Account' END AS Term, CASE WHEN Customers.OnStop = 1 THEN 'Yes' ELSE '' END AS CustOnStop FROM OrderHeaders LEFT JOIN Customers ON OrderHeaders.CustomerId = Customers.Id WHERE OrderHeaders.ShipmentId = '" + shipmentId + "'")
            gvList.DataBind()

            VisibleEmail()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "BindDataShipment", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub VisibleEmail()
        divEmail.Visible = False
        For Each row As GridViewRow In gvList.Rows
            If row.RowType = DataControlRowType.DataRow Then
                Dim chkRow As CheckBox = TryCast(row.Cells(0).FindControl("chkRow"), CheckBox)
                If chkRow.Checked Then
                    divEmail.Visible = True
                End If
            End If
        Next
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_Edit(Show As Boolean, Msg As String)
        divErrorEdit.Visible = Show : msgErrorEdit.InnerText = Msg
    End Sub

    Protected Function BindPrimaryContact(CustId As String) As String
        Dim result As String = String.Empty
        Try
            result = shipmentCfg.GetItemData("SELECT Email FROM CustomerContacts WHERE CustomerId = '" + CustId + "' AND [Primary] = 1")
        Catch ex As Exception
            result = "Error"
        End Try
        Return result
    End Function
End Class
