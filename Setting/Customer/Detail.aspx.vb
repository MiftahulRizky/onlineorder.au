Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Web.Services

Partial Class Setting_Customer_Detail
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Dim enUS As CultureInfo = New CultureInfo("en-US")
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    <WebMethod()>
    Public Shared Sub UpdateSession(value As String)
        HttpContext.Current.Session("selectedTabCustomer") = value
    End Sub


    <WebMethod()>
    Public Shared Function FabricColour(fabrictype As String) As List(Of Object)
        Dim settingCfg As New SettingConfig
        Dim result As New List(Of Object)

        Dim dataSet As DataSet = settingCfg.GetListData("SELECT * FROM FabricColours WHERE FabricId = '" + fabrictype + "' AND Active = 1 ORDER BY Name ASC")
        If dataSet.Tables.Count > 0 AndAlso dataSet.Tables(0).Rows.Count > 0 Then
            For Each row As DataRow In dataSet.Tables(0).Rows
                result.Add(New With {.Value = row("Id").ToString(), .Text = row("Colour").ToString()})
            Next
        End If
        Return result
    End Function

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("customerDetail") = "" Then
            Response.Redirect("~/setting/customer/", False)
            Exit Sub
        End If

        lblId.Text = Session("customerDetail")
        If Not Session("selectedTabCustomer") = "" Then
            selected_tab.Value = Session("selectedTabCustomer").ToString()
        End If

        If Not IsPostBack Then
            BackColor()
            BindData(lblId.Text)
            BindContact(lblId.Text)
            BindAddress(lblId.Text)
            BindLogin(lblId.Text, txtSearchLogin.Text)
            BindDiscount(lblId.Text)
            BindProduct(lblId.Text)
            BindQuote(lblId.Text)
        End If
    End Sub

    Protected Sub btnEdit_Click(sender As Object, e As EventArgs)
        Session("customerEdit") = lblId.Text
        Response.Redirect("~/setting/customer/edit", False)
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE Customers SET Active = 0 WHERE Id=@Id UPDATE CustomerLogins SET Active=0 WHERE CustomerId=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"Customers", lblId.Text, Session("LoginId"), "Delete Customer"}
            settingCfg.Log_Customer(dataLog)

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT a reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnDelete_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitCreateOrder_Click(sender As Object, e As EventArgs)
        MessageError_CreateOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showCreateOrder(); };"
        Try
            txtOrderNumber.Text.Trim()
            txtOrderName.Text.Trim()

            If txtOrderNumber.Text = "" Then
                MessageError_CreateOrder(True, "ORDER NUMBER IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If InStr(txtOrderNumber.Text, "\") > 0 Or InStr(txtOrderNumber.Text, "/") > 0 Or InStr(txtOrderNumber.Text, ",") > 0 Or InStr(txtOrderNumber.Text, "&") > 0 Or InStr(txtOrderNumber.Text, ",") > 0 Or InStr(txtOrderNumber.Text, "#") > 0 Or InStr(txtOrderNumber.Text, "'") > 0 Or InStr(txtOrderNumber.Text, ".") > 0 Then
                MessageError_CreateOrder(True, "PLEASE DON'T USE [ / ], [ \ ], [ & ], [ # ], [ ' ], [ . ] AND [ , ]")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If Trim(txtOrderNumber.Text.Length) > 20 Then
                MessageError_CreateOrder(True, "MAXIMUM CHARACTERS FOR ORDER NUMBER IS 20 CHARACTERS !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If txtOrderName.Text = "" Then
                MessageError_CreateOrder(True, "CUSTOMER NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If InStr(txtOrderName.Text, "\") > 0 Or InStr(txtOrderName.Text, "/") > 0 Or InStr(txtOrderName.Text, ",") > 0 Or InStr(txtOrderName.Text, "&") > 0 Or InStr(txtOrderName.Text, ",") > 0 Or InStr(txtOrderName.Text, "#") > 0 Or InStr(txtOrderName.Text, "'") > 0 Or InStr(txtOrderName.Text, ".") > 0 Then
                MessageError_CreateOrder(True, "PLEASE DON'T USE [ / ], [ \ ], [ & ], [ # ], [ ' ], [ . ] AND [ , ]")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If txtOrderNumber.Text = settingCfg.GetItemData("SELECT OrderNumber FROM OrderHeaders WHERE OrderNumber = '" + txtOrderNumber.Text + "' AND CustomerId = '" + lblId.Text + "' AND Active=1") Then
                MessageError_CreateOrder(True, "RETAILER ORDER NUMBER ALREADY EXISTS !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
                Exit Sub
            End If

            If msgErrorCreateOrder.InnerText = "" Then
                Dim orderCfg As New OrderConfig

                Dim headerId As String = orderCfg.CreateOrderHeaderId()
                Dim orderId As String = "SPP-" & headerId
                Dim createdBy As String = UCase(Session("LoginId").ToString())

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderHeaders(Id, OrderId, CustomerId, OrderNumber, OrderName, OrderNote, Status, CreatedBy, CreatedDate, Deposit, Approved, Active) VALUES (@Id, @OrderId, @CustomerId, @OrderNumber, @OrderName, @OrderNote, 'Unsubmitted', @CreatedBy, GETDATE(), 0, 0, 1) INSERT INTO OrderQuotes VALUES (@Id, '', '', '', '', '', '', 0.00, 0.00, 0.00, 0.00)")
                        myCmd.Parameters.AddWithValue("@Id", headerId)
                        myCmd.Parameters.AddWithValue("@OrderId", orderId)
                        myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                        myCmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumber.Text.Trim())
                        myCmd.Parameters.AddWithValue("@OrderName", txtOrderName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@OrderNote", txtOrderNote.Text.Trim())
                        myCmd.Parameters.AddWithValue("@CreatedBy", createdBy)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {headerId, "", Session("LoginId").ToString(), "Create Order"}
                orderCfg.Log_Orders(dataLog)

                Session("headerId") = headerId
                Response.Redirect("~/order/detail", False)
            End If
        Catch ex As Exception
            MessageError_CreateOrder(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_CreateOrder(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnSubmitCreateOrder_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showCreateOrder", thisScript, True)
        End Try
    End Sub

    Private Sub BindData(Id As String)
        BackColor()
        Try
            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM Customers WHERE Id = '" + Id + "'")

            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/customer", False)
                Exit Sub
            End If

            Dim groupName As String = settingCfg.GetItemData("SELECT Name FROM CustomerGroups WHERE Id = '" + myData.Tables(0).Rows(0).Item("Group").ToString() + "'")
            Dim priceGroupName As String = settingCfg.GetItemData("SELECT Name FROM CustomerPriceGroups WHERE Id = '" + myData.Tables(0).Rows(0).Item("Pricing").ToString() + "'")
            Dim onStop As Integer = Convert.ToInt32(myData.Tables(0).Rows(0).Item("OnStop"))
            Dim cashSale As Integer = Convert.ToInt32(myData.Tables(0).Rows(0).Item("CashSale"))
            Dim newsletter As Integer = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Newsletter"))
            Dim minCharge As Integer = Convert.ToInt32(myData.Tables(0).Rows(0).Item("MinimumOrderSurcharge"))

            lblExactId.Text = myData.Tables(0).Rows(0).Item("ExactId").ToString()
            lblName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            lblGroup.Text = groupName
            lblPriceGroup.Text = priceGroupName
            lblType.Text = myData.Tables(0).Rows(0).Item("Type").ToString()
            lblSalesPerson.Text = myData.Tables(0).Rows(0).Item("SalesPerson").ToString()
            divOnStopGreen.Visible = False : divOnStopDanger.Visible = False
            If onStop = 1 Then
                divOnStopGreen.Visible = True : divOnStopDanger.Visible = False
            End If
            If onStop = 0 Then
                divOnStopGreen.Visible = False : divOnStopDanger.Visible = True
            End If

            divCashSaleGreen.Visible = False : divCashSaleDanger.Visible = False
            If cashSale = 1 Then
                divCashSaleGreen.Visible = True : divCashSaleDanger.Visible = False
            End If
            If cashSale = 0 Then
                divCashSaleGreen.Visible = False : divCashSaleDanger.Visible = True
            End If

            divNewsletterGreen.Visible = False : divNewsletterDanger.Visible = False
            If newsletter = 1 Then
                divNewsletterGreen.Visible = True : divNewsletterDanger.Visible = False
            End If
            If newsletter = 0 Then
                divNewsletterGreen.Visible = False : divNewsletterDanger.Visible = True
            End If

            divMinChargeGreen.Visible = False : divMinChargeDanger.Visible = False
            If minCharge = 1 Then
                divMinChargeGreen.Visible = True : divMinChargeDanger.Visible = False
            End If
            If minCharge = 0 Then
                divMinChargeGreen.Visible = False : divMinChargeDanger.Visible = True
            End If

            liContact.Visible = False
            liAddress.Visible = False
            liLogin.Visible = False
            liDiscount.Visible = False
            liAccess.Visible = False
            liQuote.Visible = False
            If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Then
                liContact.Visible = True
                liAddress.Visible = True
                liLogin.Visible = True
                liDiscount.Visible = True
                liAccess.Visible = True
            End If
            If Session("RoleName") = "Representative" Then
                liContact.Visible = True
                liAddress.Visible = True
                liDiscount.Visible = True
            End If
            If Session("RoleName") = "Administrator" Then
                liQuote.Visible = True
            End If

            aDelete.Visible = False
            btnEdit.Visible = False
            aCreateOrder.Visible = False
            If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Then
                aDelete.Visible = True
                btnEdit.Visible = True
                aCreateOrder.Visible = True

                If lblId.Text = "DEFAULT" Then
                    aDelete.Visible = False
                    btnEdit.Visible = False
                    aCreateOrder.Visible = False

                    If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then
                        btnEdit.Visible = True
                        aCreateOrder.Visible = True
                    End If
                End If
            End If

            gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Customers WHERE Type = 'Customers' AND DataId = '" + Id + "'  ORDER BY ActionDate ASC")
            gvListLogs.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "BindData", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub

    Private Sub MessageError_CreateOrder(Show As Boolean, Msg As String)
        divErrorCreateOrder.Visible = Show : msgErrorCreateOrder.InnerText = Msg
    End Sub

    Private Sub MessageError_Log(Show As Boolean, Msg As String)
        divErrorLog.Visible = Show : msgErrorLog.InnerText = Msg
    End Sub

    ' START CUSTOMER CONTACT

    Protected Sub gvListContact_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "tabsContact"
        Try
            gvListContact.PageIndex = e.NewPageIndex
            BindContact(lblId.Text)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Contact(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "gvListContact_PageIndexChanging", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub gvListContact_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "tabsContact"
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageErrorProcess_Contact(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessContact(); };"
                Try
                    lblActionContact.Text = "Edit"
                    titleContact.InnerText = "Edit Customer Contact"
                    lblIdContact.Text = dataId

                    Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerContacts WHERE Id = '" + lblIdContact.Text + "'")

                    txtContactName.Text = thisData.Tables(0).Rows(0).Item("Name").ToString()
                    ddlContactSalutation.SelectedValue = thisData.Tables(0).Rows(0).Item("Salutation").ToString()
                    txtContactRole.Text = thisData.Tables(0).Rows(0).Item("Role").ToString()
                    txtContactEmail.Text = thisData.Tables(0).Rows(0).Item("Email").ToString()
                    txtContactPhone.Text = thisData.Tables(0).Rows(0).Item("Phone").ToString()
                    txtContactMobile.Text = thisData.Tables(0).Rows(0).Item("Mobile").ToString()
                    txtContactFax.Text = thisData.Tables(0).Rows(0).Item("Fax").ToString()
                    txtContactNote.Text = thisData.Tables(0).Rows(0).Item("Note").ToString()

                    Dim tagsArray() As String = thisData.Tables(0).Rows(0).Item("Tags").ToString().Split(",")
                    Dim tagsList As List(Of String) = tagsArray.ToList()

                    For Each i In tagsArray
                        If Not (i.Equals(String.Empty)) Then
                            lbContactTags.Items.FindByValue(i).Selected = True
                        End If
                    Next

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
                    Exit Sub
                Catch ex As Exception
                    MessageErrorProcess_Contact(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageErrorProcess_Contact(True, "Please contact IT at reza@bigblinds.co.id !")
                        mailCfg.MailError(Page.Title, "linkDetailContact_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Customers WHERE Type = 'CustomerContacts' AND DataId = '" + dataId + "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError_Log(True, "Please contact IT at reza@bigblinds.co.id !")
                        mailCfg.MailError(Page.Title, "linkLogContact_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddContact_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Contact(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessContact(); };"
        Session("selectedTabCustomer") = "tabsContact"
        Try
            lblActionContact.Text = "Add"
            titleContact.InnerText = "Add Customer Contact"
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageErrorProcess_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Contact(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnAddContact_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
        End Try
    End Sub

    Protected Sub btnResetPrimaryContact_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "tabsContact"
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET [Primary] = 0 WHERE CustomerId = @Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Session("selectedTabCustomer") = "tabsContacts"
            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Contact(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnResetPrimaryContact_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnProcessContact_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "tabsContact"
        Dim thisScript As String = "window.onload = function() { showProcessContact(); };"
        Try
            If txtContactName.Text = "" Then
                MessageErrorProcess_Contact(True, "CONTACT NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcessContact.InnerText = "" Then
                Dim thisTags As String = String.Empty
                Dim selected As String = String.Empty
                For Each item As ListItem In lbContactTags.Items
                    If item.Selected Then
                        selected += item.Text & ","
                    End If
                Next

                If Not selected = "" Then
                    thisTags = selected.Remove(selected.Length - 1).ToString()
                End If

                If lblActionContact.Text = "Add" Then
                    Dim thisId As String = String.Empty
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerContacts OUTPUT INSERTED.Id VALUES (NEWID(), @CustomerId, @Name, @Salutation, @Role, @Email, @Phone, @Mobile, @Fax, @Tags, @Note, 0)")
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtContactName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Salutation", ddlContactSalutation.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Role", txtContactRole.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtContactEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Phone", txtContactPhone.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Mobile", txtContactMobile.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Fax", txtContactFax.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Note", txtContactNote.Text.Trim())

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            thisId = myCmd.ExecuteScalar().ToString()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerContacts", thisId, Session("LoginId").ToString(), "Contact Created"}
                    settingCfg.Log_Customer(dataLog)

                    Response.Redirect("~/setting/customer/detail", False)
                End If

                If lblActionContact.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET CustomerId=@CustomerId, Name=@Name, Salutation=@Salutation, Role=@Role, Email=@Email, Phone=@Phone, Mobile=@Mobile, Fax=@Fax, Tags=@Tags, Note=@Note WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblIdContact.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Name", txtContactName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Salutation", ddlContactSalutation.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Role", txtContactRole.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Email", txtContactEmail.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Phone", txtContactPhone.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Mobile", txtContactMobile.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Fax", txtContactFax.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Note", txtContactNote.Text.Trim())

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerContacts", lblIdContact.Text, Session("LoginId"), "Contact Updated"}
                    settingCfg.Log_Customer(dataLog)

                    Response.Redirect("~/setting/customer/detail", False)
                End If
            End If
        Catch ex As Exception
            MessageErrorProcess_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Contact(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnProcessContact_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessContact", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDeleteContact_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "tabsContact"
        Try
            lblIdContact.Text = txtIdContactDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerContacts WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblIdContact.Text)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"CustomerContacts", lblIdContact.Text, Session("LoginId"), "Delete Contact"}
            settingCfg.Log_Customer(dataLog)

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnDeleteContact_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitPrimaryContact_Click(sender As Object, e As EventArgs)
        MessageError_Contact(False, String.Empty)
        Session("selectedTabCustomer") = "tabsContact"
        Try
            lblIdContact.Text = txtIdContactPrimary.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerContacts SET [Primary]=0 WHERE CustomerId=@CustomerId UPDATE CustomerContacts SET [Primary]=1 WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblIdContact.Text)
                    myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"CustomerContacts", lblIdContact.Text, Session("LoginId"), "Primary Contact"}
            settingCfg.Log_Customer(dataLog)

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnSubmitPrimaryContact_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindContact(Id As String)
        MessageError_Contact(False, String.Empty)
        lblIdContact.Text = String.Empty
        lblActionContact.Text = String.Empty
        Try
            gvListContact.DataSource = settingCfg.GetListData("SELECT * FROM CustomerContacts WHERE CustomerId= '" + Id + "'")
            gvListContact.DataBind()

            btnAddContact.Visible = False : aResetContact.Visible = False
            If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Then
                btnAddContact.Visible = True
            End If
            If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Then
                aResetContact.Visible = True
            End If
        Catch ex As Exception
            MessageError_Contact(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindContact", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Function VisibleActionContact() As Boolean
        If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Then
            Return True
        End If
        Return False
    End Function

    Protected Function VisiblePrimaryContact(Primary As Boolean) As Boolean
        If Primary = False Then
            Return True
        End If
        Return False
    End Function

    Private Sub MessageError_Contact(Show As Boolean, Msg As String)
        divErrorContact.Visible = Show : msgErrorContact.InnerText = Msg
    End Sub

    Private Sub MessageErrorProcess_Contact(Show As Boolean, Msg As String)
        divErrorProcessContact.Visible = Show : msgErrorProcessContact.InnerText = Msg
    End Sub

    ' END CUSTOMER CONTACT


    ' START CUSTOMER ADDRESS

    Protected Sub gvListAddress_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageErrorProcess_Address(False, String.Empty)
        Session("selectedTabCustomer") = "tabsContact"
        Try
            gvListContact.PageIndex = e.NewPageIndex
            BindAddress(lblId.Text)
        Catch ex As Exception
            MessageErrorProcess_Address(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Address(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "gvListAddress_PageIndexChanging", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub gvListAddress_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "tabsAddress"
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageErrorProcess_Address(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showProcessAddress(); };"
                Try
                    lblActionAddress.Text = "Edit"
                    titleAddress.InnerText = "Edit Customer Address"
                    lblIdAddress.Text = dataId

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerAddress WHERE Id = '" + lblIdAddress.Text + "'")

                    txtAddressDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
                    txtAddressUnitNumber.Text = myData.Tables(0).Rows(0).Item("UnitNumber").ToString()
                    txtAddressStreet.Text = myData.Tables(0).Rows(0).Item("Street").ToString()
                    txtAddressSuburb.Text = myData.Tables(0).Rows(0).Item("Suburb").ToString()
                    ddlAddressStates.SelectedValue = myData.Tables(0).Rows(0).Item("States").ToString()
                    txtAddressPostCode.Text = myData.Tables(0).Rows(0).Item("PostCode").ToString()
                    ddlAddressPort.SelectedValue = myData.Tables(0).Rows(0).Item("Port").ToString()
                    txtAddressInstruction.Text = myData.Tables(0).Rows(0).Item("Instruction").ToString()

                    Dim tagsArray() As String = myData.Tables(0).Rows(0).Item("Tags").ToString().Split(",")
                    Dim tagsList As List(Of String) = tagsArray.ToList()

                    For Each i In tagsArray
                        If Not (i.Equals(String.Empty)) Then
                            lbAddressTags.Items.FindByValue(i).Selected = True
                        End If
                    Next

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Catch ex As Exception
                    MessageErrorProcess_Address(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageErrorProcess_Address(True, "Please contact IT at reza@bigblinds.co.id !")
                        mailCfg.MailError(Page.Title, "linkDetailAddress_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Customers WHERE Type = 'CustomerAddress' AND DataId = '" + dataId + "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError_Log(True, "Please contact IT at reza@bigblinds.co.id !")
                        mailCfg.MailError(Page.Title, "linkLogAddress_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddAddress_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Address(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessAddress(); };"
        Session("selectedTabCustomer") = "tabsAddress"
        Try
            lblActionAddress.Text = "Add"
            titleAddress.InnerText = "Add Customer Address"
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageErrorProcess_Address(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Address(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnAddAddress_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
        End Try
    End Sub

    Protected Sub btnResetPrimaryAddress_Click(sender As Object, e As EventArgs)
        MessageError_Address(False, String.Empty)
        Session("selectedTabCustomer") = "tabsAddress"
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET [Primary] = 0 WHERE CustomerId = @Id")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Address(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnResetPrimaryAddress_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnProcessAddress_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Address(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessAddress(); };"
        Session("selectedTabCustomer") = "tabsAddress"
        Try
            If txtAddressDescription.Text = "" Then
                MessageErrorProcess_Address(True, "DESCRIPTION ADDRESS IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If txtAddressStreet.Text = "" Then
                MessageErrorProcess_Address(True, "STREET ADDRESS IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If txtAddressSuburb.Text = "" Then
                MessageErrorProcess_Address(True, "SUBURB IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If ddlAddressStates.SelectedValue = "" Then
                MessageErrorProcess_Address(True, "STATES IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If txtAddressPostCode.Text = "" Then
                MessageErrorProcess_Address(True, "POST CODE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If ddlAddressPort.SelectedValue = "" Then
                MessageErrorProcess_Address(True, "NEAREST PORT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcessAddress.InnerText = "" Then
                Dim thisTags As String = String.Empty
                If Not lbAddressTags.SelectedValue = "" Then
                    Dim selected As String = String.Empty
                    For Each item As ListItem In lbAddressTags.Items
                        If item.Selected Then
                            selected += item.Text & ","
                        End If
                    Next
                    If Not selected = "" Then
                        thisTags = selected.Remove(selected.Length - 1).ToString()
                    End If
                End If

                If lblActionAddress.Text = "Add" Then
                    Dim thisId As String = String.Empty

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerAddress OUTPUT INSERTED.Id VALUES (NEWID(), @CustomerId, @Description, @UnitNumber, @Street, @Suburb, @States, @PostCode, @Port, @Tags, @Instruction, 0)")
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Description", txtAddressDescription.Text.Trim())
                            myCmd.Parameters.AddWithValue("@UnitNumber", txtAddressUnitNumber.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Street", txtAddressStreet.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Suburb", txtAddressSuburb.Text.Trim())
                            myCmd.Parameters.AddWithValue("@States", ddlAddressStates.SelectedValue)
                            myCmd.Parameters.AddWithValue("@PostCode", txtAddressPostCode.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Port", ddlAddressPort.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Instruction", txtAddressInstruction.Text.Trim())

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            thisId = myCmd.ExecuteScalar().ToString()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerAddress", thisId, Session("LoginId"), "Address Created"}
                    settingCfg.Log_Customer(dataLog)

                    Response.Redirect("~/setting/customer/detail", False)
                End If

                If lblActionAddress.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET CustomerId=@CustomerId, Description=@Description, UnitNumber=@UnitNumber, Street=@Street, Suburb=@Suburb, States=@States, PostCode=@PostCode, Port=@Port, Tags=@Tags, Instruction=@Instruction WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblIdAddress.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@Description", txtAddressDescription.Text.Trim())
                            myCmd.Parameters.AddWithValue("@UnitNumber", txtAddressUnitNumber.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Street", txtAddressStreet.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Suburb", txtAddressSuburb.Text.Trim())
                            myCmd.Parameters.AddWithValue("@States", ddlAddressStates.SelectedValue)
                            myCmd.Parameters.AddWithValue("@PostCode", txtAddressPostCode.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Port", ddlAddressPort.SelectedValue)
                            myCmd.Parameters.AddWithValue("@Tags", thisTags)
                            myCmd.Parameters.AddWithValue("@Instruction", txtAddressInstruction.Text.Trim())

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerAddress", lblIdAddress.Text, Session("LoginId"), "Address Updated"}
                    settingCfg.Log_Customer(dataLog)

                    Response.Redirect("~/setting/customer/detail", False)
                End If
            End If
        Catch ex As Exception
            MessageErrorProcess_Address(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Address(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnProcessAddress_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDeleteAddress_Click(sender As Object, e As EventArgs)
        MessageError_Address(False, String.Empty)
        Session("selectedTabCustomer") = "tabsAddress"
        Try
            lblIdAddress.Text = txtIdAddressDelete.Text
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerAddress WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblIdAddress.Text)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Address(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnDeleteAddress_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitPrimaryAddress_Click(sender As Object, e As EventArgs)
        MessageError_Address(False, String.Empty)
        Session("selectedTabCustomer") = "tabsAddress"
        Try
            lblIdAddress.Text = txtIdAddressPrimary.Text
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerAddress SET [Primary] = 0 WHERE CustomerId = @CustomerId UPDATE CustomerAddress SET [Primary]=1 WHERE Id = @Id")
                    myCmd.Parameters.AddWithValue("@Id", lblIdAddress.Text)
                    myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"CustomerAddress", lblIdAddress.Text, Session("LoginId"), "Set as Primary Address"}
            settingCfg.Log_Customer(dataLog)

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Address(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnSubmitPrimaryAddress_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindAddress(Id As String)
        MessageError_Address(False, String.Empty)
        lblIdAddress.Text = String.Empty
        lblActionAddress.Text = String.Empty
        Try
            Dim thisQuery As String = "SELECT *, CASE WHEN [Primary] = 1 Then 'Yes' ELSE '' END AS PrimaryAddress FROM CustomerAddress WHERE CustomerId= '" + Id + "'"

            gvListAddress.DataSource = settingCfg.GetListData(thisQuery)
            gvListAddress.DataBind()

            btnAddAddress.Visible = False
            aResetPrimaryAddress.Visible = False
            If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Then
                btnAddAddress.Visible = True
            End If
            If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Then
                aResetPrimaryAddress.Visible = True
            End If
        Catch ex As Exception
            MessageError_Address(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindAddress", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Function BindDetailAddress(Id As String) As String
        Dim result As String = String.Empty
        If Not Id = "" Then
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerAddress WHERE Id='" + Id + "'")
            If thisData.Tables(0).Rows.Count > 0 Then
                Dim unitNumber As String = thisData.Tables(0).Rows(0).Item("UnitNumber").ToString()
                Dim street As String = thisData.Tables(0).Rows(0).Item("Street").ToString()
                Dim suburb As String = thisData.Tables(0).Rows(0).Item("Suburb").ToString()
                Dim states As String = thisData.Tables(0).Rows(0).Item("States").ToString()
                Dim postCode As String = thisData.Tables(0).Rows(0).Item("PostCode").ToString()

                result = street & ", " & suburb & ", " & states & " " & postCode
                If Not unitNumber = "" Then
                    result = unitNumber & ", " & street & ", " & suburb & ", " & states & " " & postCode
                End If
            End If
        End If
        Return result
    End Function

    Protected Function VisibleActionAddress() As Boolean
        If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Then
            Return True
        End If
        Return False
    End Function

    Protected Function VisiblePrimaryAddress(Primary As Boolean) As Boolean
        If Primary = False Then
            Return True
        End If
        Return False
    End Function

    Private Sub MessageError_Address(Show As Boolean, Msg As String)
        divErrorAddress.Visible = Show : msgErrorAddress.InnerText = Msg
    End Sub

    Private Sub MessageErrorProcess_Address(Show As Boolean, Msg As String)
        divErrorProcessAddress.Visible = Show : msgErrorProcessAddress.InnerText = Msg
    End Sub

    ' START CUSTOMER LOGIN

    Protected Sub gvListLogin_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Session("selectedTabCustomer") = "tabsLogin"
        MessageError_Login(False, String.Empty)
        Try
            gvListLogin.PageIndex = e.NewPageIndex
            BindLogin(lblId.Text, txtSearchLogin.Text)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Login(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "gvListLogin_PageIndexChanging", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub gvListLogin_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "tabsLogin"
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageErrorProcess_Login(False, String.Empty)

                BindLoginApp()
                BindLoginRole()
                BindLoginLevel()

                Dim thisScript As String = "window.onload = function() { showProcessLogin(); };"
                Try
                    lblIdLogin.Text = dataId
                    lblActionLogin.Text = "Edit"
                    titleLogin.InnerText = "Edit Customer Login"
                    divApplication.Visible = False
                    divAccess.Visible = False
                    divPassword.Visible = False
                    If Session("RoleName") = "Administrator" Then
                        divApplication.Visible = True
                        divAccess.Visible = True
                        divPassword.Visible = True
                    End If

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerLogins WHERE Id = '" + lblIdLogin.Text + "'")
                    ddlLoginAppId.SelectedValue = myData.Tables(0).Rows(0).Item("ApplicationId").ToString()
                    ddlLoginRole.SelectedValue = myData.Tables(0).Rows(0).Item("RoleId").ToString()
                    ddlLoginLevel.SelectedValue = myData.Tables(0).Rows(0).Item("LevelId").ToString()
                    txtLoginUserName.Text = myData.Tables(0).Rows(0).Item("UserName").ToString()
                    lblLoginUserNameOld.Text = myData.Tables(0).Rows(0).Item("UserName").ToString()
                    txtLoginFullName.Text = myData.Tables(0).Rows(0).Item("FullName").ToString()
                    Dim password As String = myData.Tables(0).Rows(0).Item("Password").ToString()
                    txtLoginPassword.Text = settingCfg.Decrypt(password)

                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                Catch ex As Exception
                    MessageErrorProcess_Login(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageErrorProcess_Login(True, "Please contact IT at reza@bigblinds.co.id !")
                        mailCfg.MailError(Page.Title, "linkDetailLogin_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Customers WHERE Type = 'CustomerLogins' AND DataId = '" + dataId + "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError_Log(True, "Please contact IT at reza@bigblinds.co.id !")
                        mailCfg.MailError(Page.Title, "linkLogLogin_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnSearchLogin_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "tabsLogin"
        MessageError_Login(False, String.Empty)
        Try
            BindLogin(lblId.Text, txtSearchLogin.Text)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Login(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnSearchLogin_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnAddLogin_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Login(False, String.Empty)

        BindLoginApp()
        BindLoginRole()
        BindLoginLevel()

        Dim thisScript As String = "window.onload = function() { showProcessLogin(); };"
        Session("selectedTabCustomer") = "tabsLogin"
        Try
            lblActionLogin.Text = "Add"
            titleLogin.InnerText = "Add Customer Login"
            divApplication.Visible = False
            divAccess.Visible = False
            divPassword.Visible = True
            If Session("RoleName") = "Administrator" Then
                divApplication.Visible = True
                divAccess.Visible = True
            End If
            txtLoginPassword.Text = settingCfg.GenerateNewPassword(15)
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageErrorProcess_Login(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Login(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnAddLogin_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
        End Try
    End Sub

    Protected Sub btnMailLogin_Click(sender As Object, e As EventArgs)
        MessageError_Login(False, String.Empty)
        Session("selectedTabCustomer") = "tabsLogin"
        Try
            mailCfg.MailLogin(Session("LoginId").ToString(), lblId.Text)

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Login(True, "Please contact IT at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnMailLogin_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnActiveLogin_Click(sender As Object, e As EventArgs)
        MessageError_Login(False, String.Empty)
        Session("selectedTabCustomer") = "tabsLogin"
        Try
            lblIdLogin.Text = txtIdActiveLogin.Text

            Dim active As Integer = 1
            If txtActiveLogin.Text = "1" Then : active = 0 : End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET Active=@Active WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblIdLogin.Text)
                    myCmd.Parameters.AddWithValue("@Active", active)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"CustomerLogins", lblIdLogin.Text, Session("LoginId").ToString(), "Login Active / Non Active"}
            settingCfg.Log_Customer(dataLog)

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Login(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnActiveLogin_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnDeleteLogin_Click(sender As Object, e As EventArgs)
        MessageError_Login(False, String.Empty)
        Session("selectedTabCustomer") = "tabsLogin"
        Try
            lblIdLogin.Text = txtIdLoginDelete.Text
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerLogins WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblIdLogin.Text)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Login(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnDeleteLogin_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnResetPass_Click(sender As Object, e As EventArgs)
        MessageError_Login(False, String.Empty)
        Session("selectedTabCustomer") = "tabsLogin"
        Try
            lblIdLogin.Text = txtIdResetPass.Text
            Dim newPassword As String = settingCfg.Encrypt(txtNewResetPass.Text)

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET Password=@Password, Reset=1 WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblIdLogin.Text)
                    myCmd.Parameters.AddWithValue("@Password", newPassword)
                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {"CustomerLogins", lblIdLogin.Text, Session("LoginId").ToString(), "Reset Password"}
            settingCfg.Log_Customer(dataLog)

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Login(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnResetPass_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnProcessLogin_Click(sender As Object, e As EventArgs)
        Session("selectedTabCustomer") = "tabsLogin"
        MessageErrorProcess_Login(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showProcessLogin(); };"

        Try
            If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then
                If ddlLoginAppId.SelectedValue = "" Then
                    MessageErrorProcess_Login(True, "APPLICATION ID IS REQUIRED !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                    Exit Sub
                End If

                If ddlLoginRole.SelectedValue = "" Then
                    MessageErrorProcess_Login(True, "ROLE MEMBER IS REQUIRED !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                    Exit Sub
                End If

                If ddlLoginLevel.SelectedValue = "" Then
                    MessageErrorProcess_Login(True, "LEVEL MEMBER IS REQUIRED !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                    Exit Sub
                End If
            End If

            If txtLoginFullName.Text = "" Then
                MessageErrorProcess_Login(True, "FULL NAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                Exit Sub
            End If

            If txtLoginUserName.Text = "" Then
                MessageErrorProcess_Login(True, "USERNAME IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                Exit Sub
            End If

            If Not Regex.IsMatch(txtLoginUserName.Text, "^[a-zA-Z0-9._-]+$") Then
                MessageErrorProcess_Login(True, "INVALID USERNAME. ONLY LETTERS, NUMBERS, DOT (.), UNDERSCRORE (_) & HYPHEN (-) ARE ALLOWED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                Exit Sub
            End If

            Dim checkUsername As String = settingCfg.GetItemData("SELECT UserName FROM CustomerLogins WHERE UserName = '" + txtLoginUserName.Text + "'")

            If lblActionLogin.Text = "Add" Then
                If txtLoginUserName.Text = checkUsername Then
                    MessageErrorProcess_Login(True, "USERNAME ALREADY EXIST !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                    Exit Sub
                End If
            End If

            If lblActionLogin.Text = "Edit" And txtLoginUserName.Text <> lblLoginUserNameOld.Text Then
                If txtLoginUserName.Text = checkUsername Then
                    MessageErrorProcess_Login(True, "USERNAME ALREADY EXIST !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
                    Exit Sub
                End If
            End If

            If msgErrorProcessLogin.InnerText = "" Then
                Dim appId As String = UCase(ddlLoginAppId.SelectedValue).ToString()
                Dim roleId As String = UCase(ddlLoginRole.SelectedValue).ToString()
                Dim levelId As String = UCase(ddlLoginLevel.SelectedValue).ToString()
                If txtLoginPassword.Text = "" Then
                    txtLoginPassword.Text = txtLoginUserName.Text
                End If
                Dim password As String = settingCfg.Encrypt(txtLoginPassword.Text)

                If Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Then
                    appId = UCase(Session("ApplicationId").ToString())
                    roleId = settingCfg.GetItemData("SELECT UPPER(Id) FROM CustomerLoginRoles WHERE Name = 'Customer'")
                    levelId = settingCfg.GetItemData("SELECT UPPER(Id) FROM CustomerLoginLevels WHERE Name = 'Member'")
                End If
                If Session("RoleName") = "Administrator" And (Session("LevelName") = "Member" Or Session("LevelName") = "Support") Then
                    appId = UCase(Session("ApplicationId").ToString())
                    roleId = settingCfg.GetItemData("SELECT UPPER(Id) FROM CustomerLoginRoles WHERE Name = 'Customer'")
                    levelId = settingCfg.GetItemData("SELECT UPPER(Id) FROM CustomerLoginLevels WHERE Name = 'Member'")
                End If

                If lblActionLogin.Text = "Add" Then
                    Dim thisId As String = String.Empty
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerLogins OUTPUT INSERTED.Id VALUES (NEWID(), @AppId, @CustomerId, @RoleId, @LevelId, @UserName, @Password, @FullName, NULL, 0, NULL, 1, 1)")
                            myCmd.Parameters.AddWithValue("@AppId", appId)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@RoleId", roleId)
                            myCmd.Parameters.AddWithValue("@LevelId", levelId)
                            myCmd.Parameters.AddWithValue("@UserName", txtLoginUserName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Password", password)
                            myCmd.Parameters.AddWithValue("@FullName", txtLoginFullName.Text.Trim())

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            thisId = myCmd.ExecuteScalar().ToString()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerLogins", thisId, Session("LoginId").ToString(), "Login Created"}
                    settingCfg.Log_System(dataLog)

                    Response.Redirect("~/setting/customer/detail", False)
                End If

                If lblActionLogin.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerLogins SET ApplicationId=@AppId, CustomerId=@CustomerId, RoleId=@RoleId, LevelId=@LevelId, UserName=@UserName, FullName=@FullName WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblIdLogin.Text)
                            myCmd.Parameters.AddWithValue("@AppId", appId)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@RoleId", roleId)
                            myCmd.Parameters.AddWithValue("@LevelId", levelId)
                            myCmd.Parameters.AddWithValue("@UserName", txtLoginUserName.Text.Trim())
                            myCmd.Parameters.AddWithValue("@Password", password)
                            myCmd.Parameters.AddWithValue("@FullName", txtLoginFullName.Text.Trim())

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerLogins", lblIdLogin.Text, Session("LoginId").ToString(), "Login Updated"}
                    settingCfg.Log_System(dataLog)

                    Response.Redirect("~/setting/customer/detail", False)
                End If
            End If
        Catch ex As Exception
            MessageErrorProcess_Login(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Login(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnProccessLogin_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessLogin", thisScript, True)
        End Try
    End Sub

    Private Sub BindLogin(Id As String, SearchText As String)
        MessageError_Login(False, String.Empty)
        lblIdLogin.Text = String.Empty
        lblActionLogin.Text = String.Empty
        Try
            Dim search As String = String.Empty
            If Not SearchText = "" Then
                search = " AND CustomerLogins.UserName LIKE '%" + SearchText.Trim() + "%'"
            End If
            Dim thisQuery As String = String.Format("SELECT CustomerLogins.*, Applications.Name AS AppName, CustomerLoginRoles.Name AS RoleName, CustomerLoginLevels.Name AS LevelName FROM CustomerLogins LEFT JOIN Applications ON CustomerLogins.ApplicationId = Applications.Id LEFT JOIN CustomerLoginRoles ON CustomerLogins.RoleId = CustomerLoginRoles.Id LEFT JOIN CustomerLoginLevels ON CustomerLogins.LevelId = CustomerLoginLevels.Id WHERE CustomerLogins.CustomerId='" + Id + "' {0} ORDER BY CASE WHEN Applications.Name = 'Application Administrator' THEN 0 WHEN Applications.Name = 'Application Lifestyle' THEN 1 ELSE 2 END, CASE WHEN CustomerLoginRoles.Name = 'Administrator' THEN 0 WHEN CustomerLoginRoles.Name = 'Customer Service' THEN 1 WHEN CustomerLoginRoles.Name = 'Representative' THEN 2 WHEN CustomerLoginRoles.Name = 'Data Entry' THEN 3 WHEN CustomerLoginRoles.Name = 'Account' THEN 4 WHEN CustomerLoginRoles.Name = 'Customer' THEN 5 ELSE 6 END, CASE WHEN CustomerLoginLevels.Name = 'Leader' THEN 0 WHEN CustomerLoginLevels.Name = 'Member' THEN 1 WHEN CustomerLoginLevels.Name = 'Support' THEN 2 ELSE 3 END, CustomerLogins.UserName ASC", search)

            gvListLogin.DataSource = settingCfg.GetListData(thisQuery)
            gvListLogin.DataBind()

            gvListLogin.Columns(1).Visible = False
            btnAddLogin.Visible = True
            If lblId.Text = "DEFAULT" Then
                btnAddLogin.Visible = False
                If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then
                    btnAddLogin.Visible = True
                End If
            End If
            If Session("RoleName") = "Administrator" Then
                gvListLogin.Columns(1).Visible = True
            End If
        Catch ex As Exception
            MessageError_Login(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindLogin", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindLoginApp()
        ddlLoginAppId.Items.Clear()
        Try
            ddlLoginAppId.DataSource = settingCfg.GetListData("SELECT * FROM Applications ORDER BY Name ASC")
            ddlLoginAppId.DataTextField = "Name"
            ddlLoginAppId.DataValueField = "Id"
            ddlLoginAppId.DataBind()

            If ddlLoginAppId.Items.Count > 1 Then
                ddlLoginAppId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlLoginAppId.Items.Clear()
            mailCfg.MailError(Page.Title, "BindLoginApp", Session("LoginId"), ex.ToString())
        End Try
    End Sub

    Private Sub BindLoginRole()
        ddlLoginRole.Items.Clear()
        Try
            ddlLoginRole.DataSource = settingCfg.GetListData("SELECT * FROM CustomerLoginRoles ORDER BY Name ASC")
            ddlLoginRole.DataTextField = "Name"
            ddlLoginRole.DataValueField = "Id"
            ddlLoginRole.DataBind()
            ddlLoginRole.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            ddlLoginRole.Items.Clear()
            mailCfg.MailError(Page.Title, "BindLoginRole", Session("LoginId"), ex.ToString())
        End Try
    End Sub

    Private Sub BindLoginLevel()
        ddlLoginLevel.Items.Clear()
        Try
            ddlLoginLevel.DataSource = settingCfg.GetListData("SELECT * FROM CustomerLoginLevels ORDER BY Name ASC")
            ddlLoginLevel.DataTextField = "Name"
            ddlLoginLevel.DataValueField = "Id"
            ddlLoginLevel.DataBind()
            ddlLoginLevel.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            ddlLoginLevel.Items.Clear()
            mailCfg.MailError(Page.Title, "BindLoginLevel", Session("LoginId"), ex.ToString())
        End Try
    End Sub

    Private Sub MessageError_Login(Show As Boolean, Msg As String)
        divErrorLogin.Visible = Show : msgErrorLogin.InnerText = Msg
    End Sub

    Private Sub MessageErrorProcess_Login(Show As Boolean, Msg As String)
        divErrorProcessLogin.Visible = Show : msgErrorProcessLogin.InnerText = Msg
    End Sub

    ' END CUSTOMER LOGIN

    ' START CUSTOMER DISCOUNT

    Protected Sub gvListDiscount_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError_Discount(False, String.Empty)
        Try
            gvListDiscount.PageIndex = e.NewPageIndex
            BindDiscount(lblId.Text)
        Catch ex As Exception
            MessageError_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Discount(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "gvListDiscount_PageIndexChanging", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub gvListDiscount_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Session("selectedTabCustomer") = "tabsDiscount"
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Detail" Then
                MessageErrorProcess_Discount(False, String.Empty)
                DiscountDesign()
                DiscountFabric()

                Dim thisScript As String = "window.onload = function() { showProcessDiscount(); };"
                Try
                    lblIdDiscount.Text = dataId
                    lblActionDiscount.Text = "Edit"
                    titleDiscount.InnerText = "Edit Customer Discount"

                    divFabricColour.Visible = False
                    divFabricProduct.Visible = False

                    Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerDiscounts WHERE Id = '" + lblIdDiscount.Text + "'")

                    ddlDiscountType.SelectedValue = myData.Tables(0).Rows(0).Item("DiscountType").ToString()
                    If ddlDiscountType.SelectedValue = "Product" Then
                        ddlDiscountDesign.SelectedValue = UCase(myData.Tables(0).Rows(0).Item("DesignId").ToString())
                        ddlDiscountFabric.SelectedValue = ""
                    End If
                    If ddlDiscountType.SelectedValue = "Fabric" Then
                        ddlDiscountDesign.SelectedValue = ""
                        ddlDiscountFabric.SelectedValue = myData.Tables(0).Rows(0).Item("FabricId").ToString()
                        DiscountFabricColour(myData.Tables(0).Rows(0).Item("FabricId").ToString())
                        DiscountFabricProduct(myData.Tables(0).Rows(0).Item("FabricId").ToString())
                        ddlFinalDiscount.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("FinalDiscount"))

                        If Not myData.Tables(0).Rows(0).Item("StartDate").ToString() = "" Then
                            txtDiscountStart.Text = Convert.ToDateTime(myData.Tables(0).Rows(0).Item("StartDate")).ToString("yyyy-MM-dd")
                        End If

                        If Not myData.Tables(0).Rows(0).Item("EndDate").ToString() = "" Then
                            txtDiscountEnd.Text = Convert.ToDateTime(myData.Tables(0).Rows(0).Item("EndDate")).ToString("yyyy-MM-dd")
                        End If

                        Dim coloursArray() As String = myData.Tables(0).Rows(0).Item("FabricColourId").ToString().Split(",")

                        For Each i In coloursArray
                            Dim trimmedId As String = i.Trim()
                            If Not (trimmedId.Equals(String.Empty)) Then
                                lbFabricColour.Items.FindByValue(trimmedId).Selected = True
                            End If
                        Next

                        Dim productsArray() As String = myData.Tables(0).Rows(0).Item("FabricProduct").ToString().Split(",")
                        For Each i In productsArray
                            Dim trimmedId As String = i.Trim()
                            If Not (trimmedId.Equals(String.Empty)) Then
                                lbFabricProduct.Items.FindByValue(trimmedId).Selected = True
                            End If
                        Next

                        divFabricColour.Visible = True
                        divFabricProduct.Visible = True
                    End If

                    Dim discount As Decimal = myData.Tables(0).Rows(0).Item("Discount")
                    txtDiscountValue.Text = discount.ToString("G29", enUS)
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessAddress", thisScript, True)
                Catch ex As Exception
                    MessageErrorProcess_Discount(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageErrorProcess_Discount(True, "Please contact IT at reza@bigblinds.co.id !")
                        mailCfg.MailError(Page.Title, "linkDetailDiscount_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = settingCfg.GetListData("SELECT * FROM Log_Customers WHERE Type = 'CustomerLogins' AND DataId = '" + dataId + "'  ORDER BY ActionDate DESC")
                    gvListLogs.DataBind()
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError_Log(True, "Please contact IT at reza@bigblinds.co.id !")
                        mailCfg.MailError(Page.Title, "linkLogDiscount_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub btnAddDiscount_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Discount(False, String.Empty)
        DiscountDesign()
        DiscountFabric()
        Dim thisScript As String = "window.onload = function() { showProcessDiscount(); };"
        Session("selectedTabCustomer") = "tabsDiscount"
        Try
            lblActionDiscount.Text = "Add"
            titleDiscount.InnerText = "Add Customer Discount"
            divFabricColour.Visible = False
            divFabricProduct.Visible = False
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageErrorProcess_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Discount(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnAddDiscount_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
        End Try
    End Sub

    Protected Sub btnAddCustomDiscount_Click(sender As Object, e As EventArgs)
        Session("customerDiscount") = lblId.Text
        Response.Redirect("~/setting/customer/discountcustom/", False)
    End Sub

    Protected Sub btnProcessDiscount_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Discount(False, String.Empty)
        Session("selectedTabCustomer") = "tabsDiscount"
        Dim thisScript As String = "window.onload = function() { showProcessDiscount(); };"
        Try
            If ddlDiscountType.SelectedValue = "" Then
                MessageErrorProcess_Discount(True, "DISCOUNT TYPE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                Exit Sub
            End If

            If ddlDiscountType.SelectedValue = "Product" And ddlDiscountDesign.SelectedValue = "" Then
                MessageErrorProcess_Discount(True, "PRODUCT IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                Exit Sub
            End If

            If ddlDiscountType.SelectedValue = "Fabric" Then
                If ddlDiscountFabric.SelectedValue = "" Then
                    MessageErrorProcess_Discount(True, "FABRIC IS REQUIRED !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                    Exit Sub
                End If

                If txtDiscountStart.Text = "" Then
                    MessageErrorProcess_Discount(True, "START DATE IS REQUIRED !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                    Exit Sub
                End If

                If Not txtDiscountStart.Text = "" Then
                    If Not IsDate(txtDiscountStart.Text) Then
                        MessageErrorProcess_Discount(True, "START DATE IS REQUIRED !")
                        ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                        Exit Sub
                    End If
                End If

                If txtDiscountEnd.Text = "" Then
                    MessageErrorProcess_Discount(True, "END DATE IS REQUIRED !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                    Exit Sub
                End If

                If Not txtDiscountEnd.Text = "" Then
                    If Not IsDate(txtDiscountEnd.Text) Then
                        MessageErrorProcess_Discount(True, "END DATE IS REQUIRED !")
                        ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                        Exit Sub
                    End If
                End If

                If txtDiscountEnd.Text < txtDiscountStart.Text Then
                    MessageErrorProcess_Discount(True, "END DATE MUST NOT BE LESSER THAN START DATE !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                    Exit Sub
                End If
            End If

            If txtDiscountValue.Text = "" Or txtDiscountValue.Text = "0" Then
                MessageErrorProcess_Discount(True, "DISCOUNT VALUE IS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
                Exit Sub
            End If

            If msgErrorProcessDiscount.InnerText = "" Then
                Dim fabricColourId As String = String.Empty
                Dim fabricCustom As Integer = 0
                Dim fabricProduct As String = String.Empty

                If ddlDiscountType.SelectedValue = "Product" Then
                    ddlDiscountFabric.SelectedValue = ""
                    txtDiscountStart.Text = String.Empty
                    txtDiscountEnd.Text = String.Empty
                    ddlFinalDiscount.SelectedValue = "0"
                End If

                If ddlDiscountType.SelectedValue = "Fabric" Then
                    ddlDiscountDesign.SelectedValue = ""

                    If Not lbFabricColour.SelectedValue = "" Then
                        Dim selected As String = String.Empty
                        For Each item As ListItem In lbFabricColour.Items
                            If item.Selected Then
                                selected += item.Value & ","
                            End If
                        Next
                        fabricColourId = selected.Remove(selected.Length - 1).ToString()
                        fabricCustom = "1"
                    End If

                    If Not lbFabricProduct.SelectedValue = "" Then
                        Dim selected As String = String.Empty
                        For Each item As ListItem In lbFabricProduct.Items
                            If item.Selected Then
                                selected += item.Value & ","
                            End If
                        Next
                        fabricProduct = selected.Remove(selected.Length - 1).ToString()
                    End If
                End If

                If lblActionDiscount.Text = "Add" Then
                    Dim thisId = settingCfg.CreateId("SELECT TOP 1 Id FROM CustomerDiscounts ORDER BY Id DESC")

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerDiscounts VALUES (@Id, 'CustomerAccount', @CustomerId, @DiscountType, @DesignId, @FabricId, NULL, 0, NULL, @Discount, @StartDate, @EndDate, @FinalDiscount, 1)")
                            myCmd.Parameters.AddWithValue("@Id", thisId)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@DiscountType", ddlDiscountType.SelectedValue)
                            myCmd.Parameters.AddWithValue("@DesignId", If(String.IsNullOrEmpty(ddlDiscountDesign.SelectedValue), CType(DBNull.Value, Object), ddlDiscountDesign.SelectedValue))
                            myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(ddlDiscountFabric.SelectedValue), CType(DBNull.Value, Object), ddlDiscountFabric.SelectedValue))
                            myCmd.Parameters.AddWithValue("@Discount", txtDiscountValue.Text)
                            myCmd.Parameters.AddWithValue("@StartDate", If(String.IsNullOrEmpty(txtDiscountStart.Text), CType(DBNull.Value, Object), txtDiscountStart.Text))
                            myCmd.Parameters.AddWithValue("@EndDate", If(String.IsNullOrEmpty(txtDiscountEnd.Text), CType(DBNull.Value, Object), txtDiscountEnd.Text))
                            myCmd.Parameters.AddWithValue("@FinalDiscount", ddlFinalDiscount.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerDiscounts", thisId, Session("LoginId").ToString(), "Customer Discount Created"}
                    settingCfg.Log_System(dataLog)

                    Response.Redirect("~/setting/customer/detail", False)
                End If

                If lblActionDiscount.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerDiscounts SET CustomerBy='CustomerAccount', CustomerData=@CustomerId, DiscountType=@DiscountType, DesignId=@DesignId, FabricId=@FabricId, FabricColourId=@FabricColourId, FabricCustom=@FabricCustom, FabricProduct=@FabricProduct, Discount=@Discount, StartDate=@StartDate, EndDate=@EndDate, FinalDiscount=@FinalDiscount WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblIdDiscount.Text)
                            myCmd.Parameters.AddWithValue("@CustomerId", lblId.Text)
                            myCmd.Parameters.AddWithValue("@DiscountType", ddlDiscountType.SelectedValue)
                            myCmd.Parameters.AddWithValue("@DesignId", If(String.IsNullOrEmpty(ddlDiscountDesign.SelectedValue), CType(DBNull.Value, Object), ddlDiscountDesign.SelectedValue))
                            myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(ddlDiscountFabric.SelectedValue), CType(DBNull.Value, Object), ddlDiscountFabric.SelectedValue))
                            myCmd.Parameters.AddWithValue("@FabricColourId", If(String.IsNullOrEmpty(fabricColourId), CType(DBNull.Value, Object), fabricColourId))
                            myCmd.Parameters.AddWithValue("@FabricCustom", fabricCustom)
                            myCmd.Parameters.AddWithValue("@FabricProduct", If(String.IsNullOrEmpty(fabricProduct), CType(DBNull.Value, Object), fabricProduct))
                            myCmd.Parameters.AddWithValue("@Discount", txtDiscountValue.Text)
                            myCmd.Parameters.AddWithValue("@StartDate", If(String.IsNullOrEmpty(txtDiscountStart.Text), CType(DBNull.Value, Object), txtDiscountStart.Text))
                            myCmd.Parameters.AddWithValue("@EndDate", If(String.IsNullOrEmpty(txtDiscountEnd.Text), CType(DBNull.Value, Object), txtDiscountEnd.Text))
                            myCmd.Parameters.AddWithValue("@FinalDiscount", ddlFinalDiscount.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim dataLog As Object() = {"CustomerDiscounts", lblIdDiscount.Text, Session("LoginId").ToString(), "Customer Discount Created"}
                    settingCfg.Log_System(dataLog)

                    Response.Redirect("~/setting/customer/detail", False)
                End If
            End If
        Catch ex As Exception
            MessageErrorProcess_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Discount(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnProccessDiscount_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
        End Try
    End Sub

    Protected Sub btnDeleteDiscount_Click(sender As Object, e As EventArgs)
        MessageError_Discount(False, String.Empty)
        Session("selectedTabCustomer") = "tabsDiscounts"
        Try
            lblIdDiscount.Text = txtIdDiscountDelete.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerDiscounts WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblIdDiscount.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnDeleteDiscount_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDiscount(Id As String)
        MessageError_Discount(False, String.Empty)
        Session("customerDiscount") = String.Empty
        lblIdDiscount.Text = String.Empty
        lblActionDiscount.Text = String.Empty
        Try
            Dim customerGroup As String = settingCfg.GetItemData("SELECT [Group] FROM Customers WHERE Id='" + Id + "'")
            Dim thisQuery As String = "SELECT * FROM CustomerDiscounts WHERE CustomerData = '" + Id + "' OR CustomerData = '" + customerGroup + "' ORDER BY CASE WHEN DiscountType = 'Product' THEN 1 WHEN DiscountType = 'Fabric' THEN 2 ELSE 3 END, CASE WHEN DiscountType = 'Fabric' THEN CAST(FabricId AS INT) ELSE NULL END ASC, CASE WHEN DiscountType != 'Fabric' THEN Id ELSE NULL END ASC"

            gvListDiscount.DataSource = settingCfg.GetListData(thisQuery)
            gvListDiscount.DataBind()

            btnAddDiscount.Visible = False
            btnAddCustomDiscount.Visible = False
            If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Then
                btnAddDiscount.Visible = True
                btnAddCustomDiscount.Visible = True
            End If
        Catch ex As Exception
            MessageError_Discount(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindDiscount", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub DiscountDesign()
        ddlDiscountDesign.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT *, UPPER(Id) AS IdText FROM Designs WHERE Type <> 'Additional' ORDER BY Name ASC"

            ddlDiscountDesign.DataSource = settingCfg.GetListData(thisQuery)
            ddlDiscountDesign.DataTextField = "Name"
            ddlDiscountDesign.DataValueField = "IdText"
            ddlDiscountDesign.DataBind()

            If ddlDiscountDesign.Items.Count > 0 Then
                ddlDiscountDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DiscountFabric()
        ddlDiscountFabric.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT * FROM Fabrics ORDER BY Name ASC"

            ddlDiscountFabric.DataSource = settingCfg.GetListData(thisQuery)
            ddlDiscountFabric.DataTextField = "Name"
            ddlDiscountFabric.DataValueField = "Id"
            ddlDiscountFabric.DataBind()

            If ddlDiscountFabric.Items.Count > 0 Then
                ddlDiscountFabric.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DiscountFabricColour(fabricType As String)
        lbFabricColour.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(fabricType) Then
                Dim thisQuery As String = "SELECT * FROM FabricColours WHERE FabricId = '" + fabricType + "' ORDER BY Name ASC"

                lbFabricColour.DataSource = settingCfg.GetListData(thisQuery)
                lbFabricColour.DataTextField = "Colour"
                lbFabricColour.DataValueField = "Id"
                lbFabricColour.DataBind()

                If lbFabricColour.Items.Count > 0 Then
                    lbFabricColour.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DiscountFabricProduct(fabricType As String)
        lbFabricProduct.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(fabricType) Then
                Dim thisQuery As String = "SELECT UPPER(Designs.Id) AS DesignId, Designs.Name AS DesignName FROM Fabrics CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS split JOIN Designs on Designs.Id = TRIM(split.value) WHERE Fabrics.Id = '" + fabricType + "'"

                lbFabricProduct.DataSource = settingCfg.GetListData(thisQuery)
                lbFabricProduct.DataTextField = "DesignName"
                lbFabricProduct.DataValueField = "DesignId"
                lbFabricProduct.DataBind()

                If lbFabricProduct.Items.Count > 0 Then
                    lbFabricProduct.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Protected Function VisibleActionDiscount() As Boolean
        If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" Then
            Return True
        End If
        Return False
    End Function

    Protected Function TextDiscount(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerDiscounts WHERE Id = '" + Id + "'")

            Dim type As String = thisData.Tables(0).Rows(0).Item("DiscountType").ToString()
            If type = "Product" Then
                Dim designId As String = thisData.Tables(0).Rows(0).Item("DesignId").ToString()
                Dim designName As String = settingCfg.GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
                result = "Product : " & designName
            End If

            If type = "Fabric" Then
                Dim fabricId As String = thisData.Tables(0).Rows(0).Item("FabricId").ToString()
                Dim fabricColourId As String = thisData.Tables(0).Rows(0).Item("FabricColourId").ToString()
                Dim fabricCustom As String = Convert.ToInt32(thisData.Tables(0).Rows(0).Item("FabricCustom"))
                Dim fabricName As String = settingCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")

                result = "Fabric : " & fabricName

                If fabricCustom = 1 Then
                    If Not String.IsNullOrEmpty(fabricColourId) Then
                        Dim ids As String() = fabricColourId.Split(","c).Where(Function(x) Not String.IsNullOrWhiteSpace(x)).ToArray()

                        Dim names As New List(Of String)

                        For Each thisId As String In ids
                            Dim name As String = settingCfg.GetItemData("SELECT Colour FROM FabricColours WHERE Id = '" & thisId.Trim() & "'")
                            names.Add(name)
                        Next
                        Dim colourName As String = String.Join(",", names)
                        result = "Fabric : " & fabricName & " (" & colourName & ")"
                    End If
                End If
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Protected Function ValueDiscount(Data As Decimal) As String
        Dim result As String = String.Empty
        Try
            If Data > 0 Then
                result = Data.ToString("G29", enUS) & "%"
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Protected Function FinalDiscount(Type As String, Data As Boolean) As String
        Dim result As String = String.Empty
        Try
            If Type = "Fabric" Then
                result = "No"
                If Data = True Then : result = "Yes" : End If
            End If
        Catch ex As Exception
            result = "ERROR"
        End Try
        Return result
    End Function

    Private Sub MessageError_Discount(Show As Boolean, Msg As String)
        divErrorDiscount.Visible = Show : msgErrorDiscount.InnerText = Msg
    End Sub

    Private Sub MessageErrorProcess_Discount(Show As Boolean, Msg As String)
        divErrorProcessDiscount.Visible = Show : msgErrorProcessDiscount.InnerText = Msg
    End Sub

    ' END CUSTOMER DISCOUNT

    ' START CUSTOMER PRODUCT ACCESS

    Protected Sub btnAddProduct_Click(sender As Object, e As EventArgs)
        MessageError_Product(False, String.Empty)
        Session("selectedTabCustomer") = "tabsAccess"
        Try
            Dim designId As String = settingCfg.GetProductAccess()

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerProductAccess VALUES (@Id, @DesignId)")
                    myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    myCmd.Parameters.AddWithValue("@DesignId", designId)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/setting/customer/detail", False)
        Catch ex As Exception
            MessageError_Product(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Product(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnAddProduct_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub linkEditProduct_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Product(False, String.Empty)
        BindDesignProduct()
        Dim thisScript As String = "window.onload = function() { showProcessProduct(); };"
        Session("selectedTabCustomer") = "tabsAccess"
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvListProduct.Rows(rowIndex)

            Dim id As String = row.Cells(0).Text

            lblActionProduct.Text = "Edit"
            titleProduct.InnerText = "Edit Product Access"

            lblIdProduct.Text = id

            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerProductAccess WHERE Id = '" + lblIdProduct.Text + "'")

            Dim tagsArray() As String = myData.Tables(0).Rows(0).Item("DesignId").ToString().Split(",")
            Dim tagsList As List(Of String) = tagsArray.ToList()

            For Each i In tagsArray
                If Not (i.Equals(String.Empty)) Then
                    lbProductTags.Items.FindByValue(i).Selected = True
                End If
            Next

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessProduct", thisScript, True)
        Catch ex As Exception
            MessageErrorProcess_Product(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Product(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "linkDetailProduct_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessProduct", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProcessProduct_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Product(False, String.Empty)
        Session("selectedTabCustomer") = "tabsAccess"
        Dim thisScript As String = "window.onload = function() { showProcessProduct(); };"
        Try
            Dim designId As String = String.Empty
            Dim tags As String = String.Empty
            For Each item As ListItem In lbProductTags.Items
                If item.Selected Then
                    tags += UCase(item.Value).ToString() & ","
                End If
            Next
            If Not tags = "" Then
                designId = tags.Remove(tags.Length - 1).ToString()
            End If

            If msgErrorProcessProduct.InnerText = "" Then
                If lblActionProduct.Text = "Add" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerProductAccess VALUES (@Id, @DesignId)")
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@DesignId", designId)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Response.Redirect("~/setting/customer/detail", False)
                End If

                If lblActionProduct.Text = "Edit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE CustomerProductAccess SET DesignId=@DesignId WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                            myCmd.Parameters.AddWithValue("@DesignId", designId)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Response.Redirect("~/setting/customer/detail", False)
                End If
            End If
        Catch ex As Exception
            MessageErrorProcess_Product(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Product(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnProccessProduct_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessProduct", thisScript, True)
        End Try
    End Sub

    Private Sub BindProduct(Id As String)
        MessageError_Product(False, String.Empty)
        Try
            Dim thisQuery As String = "SELECT * FROM CustomerProductAccess WHERE Id='" + Id + "'"

            gvListProduct.DataSource = settingCfg.GetListData(thisQuery)
            gvListProduct.DataBind()

            btnAddProduct.Visible = False
            If gvListProduct.Rows.Count = 0 Then
                btnAddProduct.Visible = True
            End If
        Catch ex As Exception
            MessageError_Product(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Product(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindProduct", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDesignProduct()
        lbProductTags.Items.Clear()
        Try
            lbProductTags.DataSource = settingCfg.GetListData("SELECT UPPER(Id) AS IdText, * FROM Designs WHERE Type <> 'Pricing' ORDER BY Name ASC")
            lbProductTags.DataTextField = "Name"
            lbProductTags.DataValueField = "IdText"
            lbProductTags.DataBind()
            If lbProductTags.Items.Count > 1 Then
                lbProductTags.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            lbProductTags.Items.Clear()
            mailCfg.MailError(Page.Title, "BindDesignProduct", Session("LoginId"), ex.ToString())
        End Try
    End Sub

    Protected Function BindDetailProduct(CustomerId As String) As String
        Dim result As String = String.Empty
        Try
            Dim hasil As String = String.Empty

            Dim myData As DataSet = settingCfg.GetListData("SELECT Designs.Name AS DesignName FROM CustomerProductAccess CROSS APPLY STRING_SPLIT(CustomerProductAccess.DesignId, ',') AS designArray LEFT JOIN Designs ON designArray.VALUE = Designs.Id WHERE CustomerProductAccess.Id = '" + CustomerId + "' ORDER BY Designs.Name ASC ")
            If Not myData.Tables(0).Rows.Count = 0 Then
                For i As Integer = 0 To myData.Tables(0).Rows.Count - 1
                    Dim designName As String = myData.Tables(0).Rows(i).Item("DesignName").ToString()
                    hasil += designName & ", "
                Next
            End If

            result = hasil.Remove(hasil.Length - 2).ToString()
        Catch ex As Exception
            MessageError_Product(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Product(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "BindDetailProduct", Session("LoginId"), ex.ToString())
            End If
        End Try
        Return result
    End Function

    Private Sub MessageError_Product(Show As Boolean, Msg As String)
        divErrorProduct.Visible = Show : msgErrorProduct.InnerText = Msg
    End Sub

    Private Sub MessageErrorProcess_Product(Show As Boolean, Msg As String)
        divErrorProcessProduct.Visible = Show : msgErrorProcessProduct.InnerText = Msg
    End Sub

    ' END CUSTOMER PRODUCT ACCESS

    ' START CUSTOMER QUOTE

    Protected Sub btnAddQuote_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Quote(False, String.Empty)
        Session("selectedTabCustomer") = "tabsQuote"
        Dim thisScript As String = "window.onload = function() { showProcessQuote(); };"
        Try
            lblActionQuote.Text = "Add"
            titleQuote.InnerText = "Add Customer Quote"
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
            Exit Sub
        Catch ex As Exception
            MessageErrorProcess_Quote(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Quote(True, "Please contact IT at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnAddQuote_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessDiscount", thisScript, True)
        End Try
    End Sub

    Protected Sub linkEditQuote_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Quote(False, String.Empty)
        Session("selectedTabCustomer") = "tabsQuote"
        Dim thisScript As String = "window.onload = function() { showProcessQuote(); };"
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvListQuote.Rows(rowIndex)

            Dim id As String = row.Cells(0).Text

            lblActionQuote.Text = "Edit"
            titleQuote.InnerText = "Edit Customer Quote"

            lblIdQuote.Text = id

            Dim myData As DataSet = settingCfg.GetListData("SELECT * FROM CustomerQuotes WHERE Id = '" + lblIdQuote.Text + "'")

            imgQuoteLogo.ImageUrl = "~/Content/static/customers/" & myData.Tables(0).Rows(0).Item("Logo").ToString()
            txtQuoteTerms.Text = myData.Tables(0).Rows(0).Item("Terms").ToString()

            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessQuote", thisScript, True)
        Catch ex As Exception
            MessageErrorProcess_Quote(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageErrorProcess_Quote(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "linkDetailProduct_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showProcessProduct", thisScript, True)
        End Try
    End Sub

    Protected Sub btnProccessQuote_Click(sender As Object, e As EventArgs)
        MessageErrorProcess_Quote(False, String.Empty)
    End Sub

    Private Sub BindQuote(Id As String)
        MessageError_Quote(False, String.Empty)
        Try
            Dim thisQuery As String = "SELECT * FROM CustomerQuotes WHERE Id='" + Id + "'"
            gvListQuote.DataSource = settingCfg.GetListData(thisQuery)
            gvListQuote.DataBind()

            btnAddQuote.Visible = False
            If gvListQuote.Rows.Count = 0 Then
                btnAddQuote.Visible = True
            End If
        Catch ex As Exception
            MessageError_Quote(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Quote(True, "Please contact IT at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "BindQuote", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError_Quote(Show As Boolean, Msg As String)
        divErrorQuote.Visible = Show : msgErrorQuote.InnerText = Msg
    End Sub

    Private Sub MessageErrorProcess_Quote(Show As Boolean, Msg As String)
        divErrorProcessQuote.Visible = Show : msgErrorProcessQuote.InnerText = Msg
    End Sub

    ' END CUSTOMER QUOTE

    Private Sub BackColor()
        MessageError(False, String.Empty)
        MessageError_Contact(False, String.Empty)
        MessageErrorProcess_Contact(False, String.Empty)
        MessageError_Address(False, String.Empty)
        MessageErrorProcess_Address(False, String.Empty)
        MessageError_Login(False, String.Empty)
        MessageErrorProcess_Login(False, String.Empty)
        MessageError_Discount(False, String.Empty)
        MessageErrorProcess_Discount(False, String.Empty)
        MessageError_Product(False, String.Empty)
        MessageErrorProcess_Product(False, String.Empty)
        MessageError_CreateOrder(False, String.Empty)
        MessageError_Quote(False, String.Empty)
        MessageError_Log(False, String.Empty)
        MessageErrorProcess_Quote(False, String.Empty)

        txtOrderName.BackColor = Drawing.Color.Empty

        Session("selectedTabCustomer") = ""
        Session("customerEdit") = ""
    End Sub

    Protected Function VisibleActions_Login(CustId As String) As Boolean
        If Not CustId = "" Then
            If CustId = "DEFAULT" Then
                If Session("RoleName") = "Administrator" And Session("LevelName") = "Leader" Then
                    Return True
                End If
                Return False
            Else
                Return True
            End If
        End If
        Return False
    End Function

    Protected Function DencryptPassword(Password As String) As String
        Dim result As String = settingCfg.Decrypt(Password)
        Return result
    End Function

    Protected Function TextActive_Login(Active As Boolean) As String
        Dim result As String = "Enable"
        If Active = True Then : Return "Disable" : End If
        Return result
    End Function

    Protected Function VisibleAction_Discount(StoreBy As String) As Boolean
        Dim result As Boolean = False
        If StoreBy = "CustomerAccount" Then : result = True : End If
        Return result
    End Function

    Protected Function VisiblePrimaryYes(Primary As Boolean) As Boolean
        Dim result As Boolean = False
        If Primary = True Then : result = True : End If
        Return result
    End Function

    Protected Function VisiblePrimaryNo(Primary As Boolean) As Boolean
        Dim result As Boolean = False
        If Primary = False Then : result = True : End If
        Return result
    End Function

    Protected Function BindTextLog(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = settingCfg.GetListData("SELECT * FROM Log_Customers WHERE Id = '" + Id + "'")

            Dim actionBy As String = thisData.Tables(0).Rows(0).Item("ActionBy").ToString()
            Dim actionDate As String = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("ActionDate")).ToString("dd MMM yyyy hh:mm")
            Dim description As String = thisData.Tables(0).Rows(0).Item("Description").ToString()

            Dim fullName As String = settingCfg.GetItemData("SELECT FullName FROM CustomerLogins WHERE Id = '" + UCase(actionBy).ToString() + "'")

            result = "<b>" & fullName & "</b> on " & actionDate & ". Action: " & description
        Catch ex As Exception
            result = ""
        End Try
        Return result
    End Function
End Class
