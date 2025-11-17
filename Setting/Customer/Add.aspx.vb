
Imports System.Data.SqlClient

Partial Class Setting_Customer_Add
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BackColor()
            BindCustomerMaster()
            BindCustomerPriceGroup()
            BindComponentForm(ddlAccount.SelectedValue)
        End If
    End Sub

    Protected Sub ddlAccount_SelectedIndexChanged(sender As Object, e As EventArgs)
        BackColor()
        BindComponentForm(ddlAccount.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If Not String.IsNullOrEmpty(txtId.Text) Then
                If txtId.Text.Length > 100 Then
                    MessageError(True, "MAXIMUM CHARACTERS FOR DEBTOR CODE IS 100 !")
                    txtId.BackColor = Drawing.Color.Red
                    txtId.Focus()
                    Exit Sub
                End If
                If txtId.Text = settingCfg.GetItemData("SELECT Id FROM Customers WHERE Id='" + txtId.Text + "'") Then
                    MessageError(True, "ID / DEBTOR CODE ALREADY EXISTS !")
                    txtId.BackColor = Drawing.Color.Red
                    txtId.Focus()
                    Exit Sub
                End If
            End If

            If ddlAccount.SelectedValue = "" Then
                MessageError(True, "ACCOUNT IS REQUIRED !")
                ddlAccount.BackColor = Drawing.Color.Red
                ddlAccount.Focus()
                Exit Sub
            End If

            If ddlAccount.SelectedValue = "Sub" Then
                If ddlMaster.SelectedValue = "" Then
                    MessageError(True, "MASTER ID IS REQUIRED !")
                    ddlAccount.BackColor = Drawing.Color.Red
                    ddlAccount.Focus()
                    Exit Sub
                End If
            End If

            If ddlAccount.SelectedValue = "Master" Or ddlAccount.SelectedValue = "Regular" Then
                If txtExactId.Text = "" Then
                    MessageError(True, "EXACT ID IS REQUIRED !")
                    txtExactId.BackColor = Drawing.Color.Red
                    txtExactId.Focus()
                    Exit Sub
                End If
            End If

            If txtName.Text = "" Then
                MessageError(True, "CUSTOMER NAME IS REQUIRED !")
                txtName.BackColor = Drawing.Color.Red
                txtName.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If txtId.Text = "" Then
                    Dim lastId As String = settingCfg.GetItemData("SELECT TOP 1 Id FROM (SELECT Id, TRY_CAST(SUBSTRING(Id, CHARINDEX('A', Id) + 1, LEN(Id)) AS INT) AS NumericValue FROM Customers) AS Subquery WHERE NumericValue IS NOT NULL ORDER BY NumericValue DESC;")
                    Dim pos As Integer = lastId.IndexOf("A")
                    Dim result As String = String.Empty

                    If pos <> -1 Then
                        Dim extracted As String = lastId.Substring(pos + 1)
                        Dim num As Integer
                        If Integer.TryParse(extracted, num) Then
                            num += 1
                            result = num.ToString()
                        Else
                            result = Now.ToString("ddMMyyhhmmss")
                        End If
                    Else
                        result = Now.ToString("ddMMyyhhmmss")
                    End If
                    txtId.Text = "LS-A" & result
                End If

                If ddlAccount.SelectedValue = "Sub" Then
                    txtExactId.Text = settingCfg.GetItemData("SELECT ExactId FROM Customers WHERE Id = '" + ddlMaster.SelectedValue + "'")
                End If

                If ddlAccount.SelectedValue = "Master" Or ddlAccount.SelectedValue = "Regular" Then
                    ddlMaster.SelectedValue = ""
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO Customers VALUES (@Id, @Account, @MasterId, NULL, @ExactId, @Name, 'Shutters', NULL, @PriceGroup, 'Office', @OnStop, @CashSale, @Newsletter, @MininimuOrderSurcharge, 1)")
                        myCmd.Parameters.AddWithValue("@Id", txtId.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Account", ddlAccount.SelectedValue)
                        myCmd.Parameters.AddWithValue("@MasterId", ddlMaster.SelectedValue)
                        myCmd.Parameters.AddWithValue("@ExactId", txtExactId.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@PriceGroup", ddlPriceGroup.SelectedValue)
                        myCmd.Parameters.AddWithValue("@OnStop", ddlOnStop.SelectedValue)
                        myCmd.Parameters.AddWithValue("@CashSale", ddlCashSale.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Newsletter", ddlNewsletter.SelectedValue)
                        myCmd.Parameters.AddWithValue("@MininimuOrderSurcharge", ddlMinimumOrderSurcharge.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                ' CUSTOMER QUOTES
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerQuotes VALUES (@Id, 'LS.png', '')")
                        myCmd.Parameters.AddWithValue("@Id", txtId.Text.Trim())

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                ' CUSTOMER PRODUCT ACCESS
                Dim getDesign As String = settingCfg.GetProductAccess()
                Dim designId = UCase(getDesign).ToString()
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerProductAccess VALUES (@Id, @DesignId)")
                        myCmd.Parameters.AddWithValue("@Id", txtId.Text.Trim())
                        myCmd.Parameters.AddWithValue("@DesignId", designId)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {"Customers", txtId.Text, Session("LoginId"), "Customer Created"}
                settingCfg.Log_Customer(dataLog)

                Session("customerDetail") = txtId.Text
                Response.Redirect("~/setting/customer/detail", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnSubmit_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/customer/", False)
    End Sub

    Private Sub BindCustomerMaster()
        ddlMaster.Items.Clear()
        Try
            ddlMaster.DataSource = settingCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Customers WHERE Account='Master' ORDER BY Id ASC")
            ddlMaster.DataTextField = "NameText"
            ddlMaster.DataValueField = "Id"
            ddlMaster.DataBind()

            If ddlMaster.Items.Count > 0 Then
                ddlMaster.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            mailCfg.MailError(Page.Title, "BindCustomerMaster", Session("LoginId"), ex.ToString())
        End Try
    End Sub

    Private Sub BindCustomerPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingCfg.GetListData("SELECT *,  UPPER(Name) AS NameText FROM CustomerPriceGroups WHERE Active=1 ORDER BY Id ASC")
            ddlPriceGroup.DataTextField = "NameText"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            If ddlPriceGroup.Items.Count > 0 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            mailCfg.MailError(Page.Title, "BindCustomerPriceGroup", Session("LoginId"), ex.ToString())
        End Try
    End Sub

    Private Sub BindComponentForm(Account As String)
        BackColor()
        Try
            divId.Visible = False
            divMaster.Visible = False
            divInternal.Visible = False
            divExact.Visible = False

            If String.IsNullOrEmpty(Account) Then
                Exit Sub
            End If

            If Session("RoleName") = "Administrator" Then
                divId.Visible = True
            End If

            If Account = "Master" Or Account = "Regular" Then
                divInternal.Visible = True
                divExact.Visible = True
            End If

            If Account = "Sub" Then
                divMaster.Visible = True
            End If
        Catch ex As Exception
            mailCfg.MailError(Page.Title, "BindComponentForm", Session("LoginId"), ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        MessageError(False, "")

        ddlAccount.BackColor = Drawing.Color.Empty
        txtExactId.BackColor = Drawing.Color.Empty
        ddlMaster.BackColor = Drawing.Color.Empty
        txtName.BackColor = Drawing.Color.Empty
        ddlPriceGroup.BackColor = Drawing.Color.Empty
        ddlOnStop.BackColor = Drawing.Color.Empty
        ddlNewsletter.BackColor = Drawing.Color.Empty
        ddlCashSale.BackColor = Drawing.Color.Empty
        ddlMinimumOrderSurcharge.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
