
Imports System.Data.SqlClient

Partial Class Setting_Customer_Group_Discount_Add
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Customer Service" Then
            Response.Redirect("~/setting/customer/detail", False)
            Exit Sub
        End If

        If Session("customerGroupDiscountAdd") = "" Then
            Response.Redirect("~/setting/customer/group/discount", False)
            Exit Sub
        End If

        lblCustomerId.Text = Session("customerGroupDiscountAdd")

        If Not IsPostBack Then
            BackColor()
            BindDesignType()
            BindFabricType()
            BindFabricColour(ddlFabricType.SelectedValue)
            BindFabricProduct(ddlFabricType.SelectedValue)

            BindComponent(ddlDiscountType.SelectedValue)
        End If
    End Sub

    Protected Sub ddlDiscountType_SelectedIndexChanged(sender As Object, e As EventArgs)
        BackColor()
        BindComponent(ddlDiscountType.SelectedValue)
    End Sub

    Protected Sub ddlFabricType_SelectedIndexChanged(sender As Object, e As EventArgs)
        BackColor()
        BindFabricColour(ddlFabricType.SelectedValue)
        BindFabricProduct(ddlFabricType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        BackColor()
        Try
            If ddlDiscountType.SelectedValue = "" Then
                MessageError(True, "DISCOUNT TYPE IS REQUIRED !")
                ddlDiscountType.BackColor = Drawing.Color.Empty
                ddlDiscountType.Focus()
                Exit Sub
            End If

            If ddlDiscountType.SelectedValue = "Product" Then
                If ddlDesignType.SelectedValue = "" Then
                    MessageError(True, "PRODUCT TYPE IS REQUIRED !")
                    ddlDesignType.BackColor = Drawing.Color.Empty
                    ddlDesignType.Focus()
                    Exit Sub
                End If
            End If

            If ddlDesignType.SelectedValue = "Fabric" Then
                If ddlFabricType.SelectedValue = "" Then
                    MessageError(True, "FABRIC TYPE IS REQUIRED !")
                    ddlFabricType.BackColor = Drawing.Color.Empty
                    ddlFabricType.Focus()
                    Exit Sub
                End If

                If txtStartDate.Text = "" Then
                    MessageError(True, "START DATE IS REQUIRED !")
                    txtStartDate.BackColor = Drawing.Color.Empty
                    txtStartDate.Focus()
                    Exit Sub
                End If

                If txtEndDate.Text = "" Then
                    MessageError(True, "END DATE IS REQUIRED !")
                    txtEndDate.BackColor = Drawing.Color.Empty
                    txtEndDate.Focus()
                    Exit Sub
                End If

                If txtEndDate.Text < txtStartDate.Text Then
                    MessageError(True, "END DATE MUST NOT BE LESSER THAN START DATE !")
                    txtEndDate.BackColor = Drawing.Color.Empty
                    txtEndDate.Focus()
                    Exit Sub
                End If
            End If

            If txtDiscount.Text = "" Then
                MessageError(True, "DISCOUNT VALUE IS REQUIRED !")
                txtDiscount.BackColor = Drawing.Color.Empty
                txtDiscount.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim fabricColourId As String = String.Empty
                Dim fabricProduct As String = String.Empty
                Dim fabricCustom As Integer = 0

                If ddlDiscountType.SelectedValue = "Product" Then
                    ddlFabricType.SelectedValue = ""
                    txtStartDate.Text = String.Empty
                    txtEndDate.Text = String.Empty
                    ddlFinalDiscount.SelectedValue = "0"
                End If

                If ddlDiscountType.SelectedValue = "Fabric" Then
                    ddlDesignType.SelectedValue = ""

                    If Not lbFabricColour.SelectedValue = "" Then
                        Dim selected As String = String.Empty
                        For Each item As ListItem In lbFabricColour.Items
                            If item.Selected Then
                                selected += item.Value & ","
                            End If
                        Next
                        fabricCustom = 1
                        fabricColourId = selected.Remove(selected.Length - 1).ToString()
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

                Dim thisId As String = settingCfg.CreateId("SELECT TOP 1 Id FROM CustomerDiscounts ORDER BY Id DESC")
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("INSERT INTO CustomerDiscounts VALUES (@Id, 'CustomerGroup', @CustomerId, @DiscountType, @DesignId, @FabricId, @FabricColourId, @FabricCustom, @FabricProduct, @Discount, @StartDate, @EndDate, @FinalDiscount, 1)")
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@CustomerId", lblCustomerId.Text)
                        myCmd.Parameters.AddWithValue("@DiscountType", ddlDiscountType.SelectedValue)
                        myCmd.Parameters.AddWithValue("@DesignId", If(String.IsNullOrEmpty(ddlDesignType.SelectedValue), CType(DBNull.Value, Object), ddlDesignType.SelectedValue))
                        myCmd.Parameters.AddWithValue("@FabricId", If(String.IsNullOrEmpty(ddlFabricType.SelectedValue), CType(DBNull.Value, Object), ddlFabricType.SelectedValue))
                        myCmd.Parameters.AddWithValue("@FabricColourId", fabricColourId)
                        myCmd.Parameters.AddWithValue("@FabricCustom", fabricCustom)
                        myCmd.Parameters.AddWithValue("@FabricProduct", fabricProduct)
                        myCmd.Parameters.AddWithValue("@Discount", txtDiscount.Text)
                        myCmd.Parameters.AddWithValue("@StartDate", If(String.IsNullOrEmpty(txtStartDate.Text), CType(DBNull.Value, Object), txtStartDate.Text))
                        myCmd.Parameters.AddWithValue("@EndDate", If(String.IsNullOrEmpty(txtEndDate.Text), CType(DBNull.Value, Object), txtEndDate.Text))
                        myCmd.Parameters.AddWithValue("@FinalDiscount", ddlFinalDiscount.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Response.Redirect("~/setting/customer/detail", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT at reza@bigblinds.co.id !")
                mailCfg.MailError(Page.Title, "btnSubmit_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/customer/detail", False)
    End Sub

    Private Sub BindComponent(Type As String)
        Try
            divDesignType.Visible = False
            divFabricType.Visible = False
            divFabricColour.Visible = False
            divCustomProduct.Visible = False
            divPeriod.Visible = False
            divFinalDiscount.Visible = False

            If Type = "Product" Then
                divDesignType.Visible = True
            End If

            If Type = "Fabric" Then
                divFabricType.Visible = True
                divFabricColour.Visible = True
                divCustomProduct.Visible = True
                divPeriod.Visible = True
                divFinalDiscount.Visible = True
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDesignType()
        ddlDesignType.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT UPPER(Name) AS NameText, UPPER(Id) AS IdText FROM Designs WHERE Type <> 'Additional' ORDER BY Name ASC"

            ddlDesignType.DataSource = settingCfg.GetListData(thisQuery)
            ddlDesignType.DataTextField = "NameText"
            ddlDesignType.DataValueField = "IdText"
            ddlDesignType.DataBind()

            If ddlDesignType.Items.Count > 0 Then
                ddlDesignType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub BindFabricType()
        ddlFabricType.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT *, UPPER(Name) AS NameText FROM Fabrics ORDER BY Name ASC"

            ddlFabricType.DataSource = settingCfg.GetListData(thisQuery)
            ddlFabricType.DataTextField = "NameText"
            ddlFabricType.DataValueField = "Id"
            ddlFabricType.DataBind()

            If ddlFabricType.Items.Count > 0 Then
                ddlFabricType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindFabricColour(fabricType As String)
        lbFabricColour.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(fabricType) Then
                Dim thisQuery As String = "SELECT *, UPPER(Colour) AS ColourText FROM FabricColours WHERE FabricId = '" + fabricType + "' ORDER BY Name ASC"

                lbFabricColour.DataSource = settingCfg.GetListData(thisQuery)
                lbFabricColour.DataTextField = "ColourText"
                lbFabricColour.DataValueField = "Id"
                lbFabricColour.DataBind()

                If lbFabricColour.Items.Count > 0 Then
                    lbFabricColour.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindFabricProduct(fabricType As String)
        lbFabricProduct.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(fabricType) Then
                Dim thisQuery As String = "SELECT UPPER(Designs.Id) AS DesignId, UPPER(Designs.Name) AS DesignName FROM Fabrics CROSS APPLY STRING_SPLIT(Fabrics.DesignId, ',') AS split JOIN Designs on Designs.Id = TRIM(split.value) WHERE Fabrics.Id = '" + fabricType + "'"

                lbFabricProduct.DataSource = settingCfg.GetListData(thisQuery)
                lbFabricProduct.DataTextField = "DesignName"
                lbFabricProduct.DataValueField = "DesignId"
                lbFabricProduct.DataBind()

                If lbFabricProduct.Items.Count > 0 Then
                    lbFabricProduct.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        MessageError(False, String.Empty)

        ddlDiscountType.BackColor = Drawing.Color.Empty
        ddlDesignType.BackColor = Drawing.Color.Empty
        ddlFabricType.BackColor = Drawing.Color.Empty
        txtStartDate.BackColor = Drawing.Color.Empty
        txtEndDate.BackColor = Drawing.Color.Empty
        txtDiscount.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
